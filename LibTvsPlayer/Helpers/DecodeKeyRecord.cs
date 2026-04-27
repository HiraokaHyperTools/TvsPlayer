using LibTvsPlayer.DataTypes;
using LibTvsPlayer.Services;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace LibTvsPlayer.Helpers
{
    public class DecodeKeyRecord
    {
        private readonly TileConfHelperV2 _tileConfHelper;
        private readonly Changes _noop = new();

        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public int ClutEntryUnitSize { get; set; }
        public ReadOnlyMemory<byte>? JpegHeader { get; set; }

        public DecodeKeyRecord(
            TileConfHelperV2 tileConfHelper)
        {
            _tileConfHelper = tileConfHelper;
        }

        public record Changes(
            bool ScreenSizeChanged = false,
            RenderJpegCmd? RenderJpeg = null,
            IReadOnlyList<Render32bppBitmapCmd>? Render32bppBitmaps = null,
            CopyBlockCmd? CopyBlock = null,
            IReadOnlyList<RenderBlockErrorCmd>? RenderBlockErrors = null);

        public Changes Consume(KeyRecord keyRecord)
        {
            if (keyRecord.RecordType == 0x070C)
            {
                return _noop; // unknown
            }

            if (false
                || keyRecord.KeyTags.Count < 3
                || keyRecord.KeyTags[0].Tag != 0x00
                || keyRecord.KeyTags[0].Value.Length < 1
            )
            {
                return _noop; // unknown
            }

            var tag00Span = keyRecord.KeyTags[0].Value.Span;

            if (tag00Span.Length == 4)
            {
                if (false
                    || keyRecord.KeyTags.Count < 3
                    || keyRecord.KeyTags[1].Tag != 0x01
                    || keyRecord.KeyTags[1].Value.Length != 4
                )
                {
                    return _noop; // unknown
                }

                var tag01Span = keyRecord.KeyTags[1].Value.Span;

                ScreenWidth = BinaryPrimitives.ReadInt32LittleEndian(tag00Span);
                ScreenHeight = BinaryPrimitives.ReadInt32LittleEndian(tag01Span);

                var tag06Tag = keyRecord.KeyTags.SingleOrDefault(it => it.Tag == 0x06);
                if (tag06Tag != null)
                {
                    var tag06 = BinaryPrimitives.ReadInt32LittleEndian(tag06Tag.Value.Span);
                    ClutEntryUnitSize = (tag06 == 0x50) ? 3 : 2;
                }
                else
                {
                    ClutEntryUnitSize = 2;
                }

                return new Changes(ScreenSizeChanged: true); // done
            }

            if (tag00Span.Length != 1)
            {
                return _noop; // unknown
            }

            var dataType = keyRecord.KeyTags[0].Value.Span[0];

            var blockIndexTag = keyRecord.KeyTags
                .SingleOrDefault(tag => true
                    && tag.Tag == 0x19
                    && tag.Value.Length == 2
                );

            if (blockIndexTag == null)
            {
                return _noop; // invalid form
            }

            var blockIndex = BinaryPrimitives.ReadUInt16LittleEndian(blockIndexTag.Value.Span);

            if (false) { }
            // RLE
            else if (dataType == 0x0C)
            {
                var list = new List<Render32bppBitmapCmd>();
                var errors = new List<RenderBlockErrorCmd>();

                var paletteTag = keyRecord.KeyTags
                    .SingleOrDefault(tag => true
                        && tag.Tag == 0x1C
                    );

                var packedRleTag = keyRecord.KeyTags
                    .SingleOrDefault(tag => true
                        && tag.Tag == 0x1D
                    );

                var blockConfTag = keyRecord.KeyTags
                    .SingleOrDefault(tag => true
                        && tag.Tag == 0x1B
                    );

                var blockConf = (blockConfTag != null)
                    ? blockConfTag.Value.Span
                    : ReadOnlySpan<byte>.Empty;

                var tileConfs = _tileConfHelper.GetTileConfsFromBlockConf(blockConf);

                if (paletteTag != null && packedRleTag != null)
                {
                    var palette = paletteTag.Value;
                    var packedRle = packedRleTag.Value;

                    var colors = ConvertDeltaPaletteToColors(palette, ClutEntryUnitSize).Span;

                    var numColorsDefined = palette.Length / ClutEntryUnitSize;

                    if (true
                        && ParsePackedRleBlocks(packedRle) is PackedRleBlock[] packedRleBlocks
                        && packedRleBlocks != null
                        && ComputeTransferBlocks(ScreenWidth, ScreenHeight, blockIndex, tileConfs) is var transferBlocks
                        && transferBlocks.Length == packedRleBlocks.Length
                    )
                    {
                        var wideMode = 128 <= numColorsDefined;

                        foreach (var transferBlock in transferBlocks)
                        {
                            var cxBitmap = transferBlock.Width;
                            var cyBitmap = transferBlock.Height;

                            var pixels = new byte[4 * cxBitmap * cyBitmap];
                            var pixelsSpan = pixels.AsSpan();

                            try
                            {
                                var packedRleBlock = packedRleBlocks[transferBlock.Index];
                                var rle = packedRleBlock.Rle.Span;
                                while (rle.Length != 0)
                                {
                                    var control = rle[0];
                                    rle = rle.Slice(1);
                                    if (wideMode ? (control < 0xFF) : (control < 0x80))
                                    {
                                        colors.Slice(4 * control, 4).CopyTo(pixelsSpan);
                                        pixelsSpan = pixelsSpan.Slice(4);
                                    }
                                    else if (control < 0xFF)
                                    {
                                        var count = rle[0];
                                        rle = rle.Slice(1);

                                        var color = (byte)(control - 0x80);
                                        var colorSpan = colors.Slice(4 * color, 4);

                                        for (int x = 0; x < count; x++)
                                        {
                                            colorSpan.CopyTo(pixelsSpan);
                                            pixelsSpan = pixelsSpan.Slice(4);
                                        }

                                    }
                                    else
                                    {
                                        wideMode = true;

                                        var what = rle[0];
                                        var howMany = rle[1];
                                        rle = rle.Slice(2);

                                        var colorSpan = colors.Slice(4 * what, 4);

                                        for (int x = 0; x < howMany; x++)
                                        {
                                            colorSpan.CopyTo(pixelsSpan);
                                            pixelsSpan = pixelsSpan.Slice(4);
                                        }
                                    }
                                }

                                list.Add(
                                    new Render32bppBitmapCmd(
                                        Bits: pixels,
                                        Width: cxBitmap,
                                        Height: cyBitmap,
                                        Tx: transferBlock.X,
                                        Ty: transferBlock.Y
                                    )
                                );
                            }
                            catch (Exception ex)
                            {
                                errors.Add(
                                    new RenderBlockErrorCmd(
                                        Exception: ex,
                                        Width: cxBitmap,
                                        Height: cyBitmap,
                                        Tx: transferBlock.X,
                                        Ty: transferBlock.Y
                                    )
                                );
                            }

                        }
                    }
                }

                return new Changes(
                    Render32bppBitmaps: list.AsReadOnly(),
                    RenderBlockErrors: errors.AsReadOnly()
                ); // done
            }
            // JPEG
            else if (dataType == 0x0D)
            {
                var jpegHeaderTag = keyRecord.KeyTags
                    .SingleOrDefault(tag => true
                        && tag.Tag == 0x11
                        && tag.Value.Length == 177
                    );
                if (jpegHeaderTag != null)
                {
                    JpegHeader = jpegHeaderTag.Value;
                }

                var jpegDataTag = keyRecord.KeyTags
                    .SingleOrDefault(tag => true
                        && tag.Tag == 0x1D
                        && 64 < tag.Value.Length
                        && tag.Value.Span[0] == 0xFF
                        && tag.Value.Span[1] == 0xC4
                    );

                if (true
                    && JpegHeader is ReadOnlyMemory<byte> jpegHeader
                    && jpegDataTag != null
                )
                {
                    var tileConfs = _tileConfHelper.GetTileConfsFromBlockConf([]);

                    var transferBlocks = ComputeTransferBlocks(
                        ScreenWidth,
                        ScreenHeight,
                        blockIndex,
                        tileConfs
                    );

                    var transferBlock = transferBlocks.Single();

                    var cx = Convert.ToUInt16(transferBlock.Width);
                    var cy = Convert.ToUInt16(transferBlock.Height);

                    var jpegBytes = new byte[jpegHeader.Length + jpegDataTag.Value.Length];

                    jpegHeader
                        .CopyTo(
                            jpegBytes
                                .AsMemory(
                                    0,
                                    jpegHeader.Length
                                )
                        );
                    jpegDataTag.Value
                        .CopyTo(
                            jpegBytes
                                .AsMemory(
                                    jpegHeader.Length,
                                    jpegDataTag.Value.Length
                                )
                        );
                    BinaryPrimitives.WriteUInt16BigEndian(jpegBytes.AsSpan(0xA3, 2), cx); // Y_image
                    BinaryPrimitives.WriteUInt16BigEndian(jpegBytes.AsSpan(0xA5, 2), cy); // X_image

                    return new Changes(
                        RenderJpeg: new RenderJpegCmd(
                            Jpeg: jpegBytes,
                            Width: transferBlock.Width,
                            Height: transferBlock.Height,
                            Tx: transferBlock.X,
                            Ty: transferBlock.Y
                        )
                    );
                }
            }
            // fill?
            else if (dataType == 0x0A)
            {
                var tileConfs = _tileConfHelper.GetTileConfsFromBlockConf([]);

                var transferBlocks = ComputeTransferBlocks(
                    ScreenWidth,
                    ScreenHeight,
                    blockIndex,
                    tileConfs
                );

                var transferBlock = transferBlocks.Single();

                var cx = Convert.ToUInt16(transferBlock.Width);
                var cy = Convert.ToUInt16(transferBlock.Height);

                var tag19Tag = keyRecord.KeyTags
                    .SingleOrDefault(tag => true
                        && tag.Tag == 0x19
                        && tag.Value.Length == 2
                    );

                if (tag19Tag != null)
                {
                    var transferSourceBlocks = ComputeTransferBlocks(
                        ScreenWidth,
                        ScreenHeight,
                        BinaryPrimitives.ReadUInt16LittleEndian(tag19Tag.Value.Span),
                        tileConfs
                    );

                    return new Changes(
                        CopyBlock: new CopyBlockCmd(
                            Srcx: transferSourceBlocks[0].X,
                            Srcy: transferSourceBlocks[0].Y,
                            Width: transferBlock.Width,
                            Height: transferBlock.Height,
                            Tx: transferBlock.X,
                            Ty: transferBlock.Y
                        )
                    );
                }
            }

            return _noop; // done
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BlockId">0 based index of sequence of 64x64 blocks from left to right and from top to down</param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="NumPixels"></param>
        /// <param name="Index">0 based block number in this tile conf</param>
        private record TransferBlock(
            int BlockId,
            int X,
            int Y,
            int Width,
            int Height,
            int NumPixels,
            int Index);

        private TransferBlock[] ComputeTransferBlocks(
            int scrX,
            int scrY,
            int blockId,
            IReadOnlyList<TileConf> tileConfs)
        {
            var numXBlocks = (scrX + 63) / 64;

            return tileConfs
                .Select(
                    (tileConf, index) =>
                    {
                        var blockX = (blockId % numXBlocks) + tileConf.XOffset;
                        var blockY = (blockId / numXBlocks) + tileConf.YOffset;
                        var blockWidth = Math.Min(64 * tileConf.NumX, scrX - blockX * 64);
                        var blockHeight = Math.Min(64 * tileConf.NumY, scrY - blockY * 64);
                        return new TransferBlock(
                            BlockId: blockId,
                            X: 64 * blockX,
                            Y: 64 * blockY,
                            Width: blockWidth,
                            Height: blockHeight,
                            NumPixels: blockWidth * blockHeight,
                            Index: index
                        );
                    }
                )
                .ToArray();
        }

        private ReadOnlyMemory<byte> ConvertDeltaPaletteToColors(
            ReadOnlyMemory<byte> palette,
            int entryUnitSize)
        {
            var paletteColors = new byte[1024].AsMemory();
            var paletteColorsSpan = paletteColors.Span;
            if (entryUnitSize == 2)
            {
                var current = 0;
                var span = palette.Span;
                for (int i = 0; i < 256 && 2 <= span.Length; i++)
                {
                    var delta = BinaryPrimitives.ReadUInt16LittleEndian(span);
                    current += delta;
                    var r = (current >> 8) & 0xF;
                    var g = (current >> 4) & 0xF;
                    var b = (current >> 0) & 0xF;
                    paletteColorsSpan[0] = (byte)(r * 17);
                    paletteColorsSpan[1] = (byte)(g * 17);
                    paletteColorsSpan[2] = (byte)(b * 17);
                    paletteColorsSpan[3] = 0xFF; // opaque alpha
                    paletteColorsSpan = paletteColorsSpan.Slice(4);
                    span = span.Slice(2);
                }
            }
            else if (entryUnitSize == 3)
            {
                var current = 0U;
                var span = palette.Span;
                for (int i = 0; i < 256 && 3 <= span.Length; i++)
                {
                    var delta = BinaryPrimitives.ReadUInt32LittleEndian([span[0], span[1], span[2], 0,]);
                    current += delta;
                    var r = (current >> 16) & 0xFF;
                    var g = (current >> 8) & 0xFF;
                    var b = (current >> 0) & 0xFF;
                    paletteColorsSpan[0] = (byte)(r);
                    paletteColorsSpan[1] = (byte)(g);
                    paletteColorsSpan[2] = (byte)(b);
                    paletteColorsSpan[3] = 0xFF; // opaque alpha
                    paletteColorsSpan = paletteColorsSpan.Slice(4);
                    span = span.Slice(3);
                }
            }
            return paletteColors;
        }

        private PackedRleBlock[]? ParsePackedRleBlocks(ReadOnlyMemory<byte> ptr)
        {
            var list = new List<PackedRleBlock>();

            while (ptr.Length != 0)
            {
                if (ptr.Length < 3)
                {
                    return null;
                }
                var ver = ptr.Span[0];
                ptr = ptr.Slice(1);
                if (ver != 1)
                {
                    return null;
                }
                var len = BinaryPrimitives.ReadUInt16LittleEndian(ptr.Span);
                ptr = ptr.Slice(2);
                if (ptr.Length < len)
                {
                    return null;
                }
                list.Add(new PackedRleBlock(ptr.Slice(0, len)));
                ptr = ptr.Slice(len);
            }

            return list.ToArray();
        }
    }
}
