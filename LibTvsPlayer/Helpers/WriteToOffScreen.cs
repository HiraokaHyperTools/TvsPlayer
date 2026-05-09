using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Helpers
{
    /// <summary>
    /// 32-bpp (RGBA) off screen bitmap writer.
    /// </summary>
    public class WriteToOffScreen
    {
        private readonly Memory<byte> _offScreen;
        private readonly int _screenWidth;
        private readonly int _screenHeight;

        public WriteToOffScreen(
            Memory<byte> offScreen,
            int screenWidth,
            int screenHeight)
        {
            _offScreen = offScreen;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
        }

        public void Bitblt(
            ReadOnlySpan<byte> source,
            int width,
            int height,
            int tx,
            int ty)
        {
            var writeSpan = _offScreen.Span;
            var writeStride = _screenWidth * 4;
            var readSpan = source;
            var readStride = width * 4;
            var numBytesPerLine = Math.Min(writeStride - 4 * tx, readStride);

            writeSpan = writeSpan.Slice(4 * (tx + _screenWidth * ty));

            height = Math.Min(height, _screenHeight - ty);

            for (int y = 0; y < height; y++)
            {
                if (y != 0)
                {
                    readSpan = readSpan.Slice(readStride);
                    writeSpan = writeSpan.Slice(writeStride);
                }

                readSpan
                    .Slice(
                        0,
                        numBytesPerLine
                    )
                    .CopyTo(
                        writeSpan
                    );
            }
        }

        public void FillColor(
            byte r,
            byte g,
            byte b,
            int width,
            int height,
            int tx,
            int ty)
        {
            var color = new byte[] { r, g, b, 255 };

            var writeSpan = _offScreen.Span;
            var writeStride = _screenWidth * 4;

            writeSpan = writeSpan.Slice(4 * (tx + _screenWidth * ty));

            height = Math.Min(height, _screenHeight - ty);

            for (int y = 0; y < height; y++)
            {
                if (y != 0)
                {
                    writeSpan = writeSpan.Slice(writeStride);
                }

                for (int x = 0; x < width; x++)
                {
                    color.CopyTo(writeSpan.Slice(4 * x));
                }
            }
        }


        public void WriteError(
            string message,
            int width,
            int height,
            int tx,
            int ty)
        {
            FillColor(0xEE, 0x00, 0x00, width, height, tx, ty);
        }

        public void SaveTo(
            Span<byte> destination,
            int width,
            int height,
            int sx,
            int sy)
        {
            var writeSpan = destination;
            var writeStride = 4 * width;
            var readSpan = _offScreen.Span;
            var readStride = 4 * _screenWidth;
            var numBytesPerLine = Math.Min(writeStride, readStride - 4 * sx);

            readSpan = readSpan.Slice(4 * (sx + _screenWidth * sy));

            height = Math.Min(height, _screenHeight - sy);

            for (int y = 0; y < height; y++)
            {
                if (y != 0)
                {
                    readSpan = readSpan.Slice(readStride);
                    writeSpan = writeSpan.Slice(writeStride);
                }

                readSpan
                    .Slice(
                        0,
                        numBytesPerLine
                    )
                    .CopyTo(
                        writeSpan
                    );
            }
        }

        public void BitbltSrcAlpha(
            ReadOnlySpan<byte> source,
            int width,
            int height,
            int tx,
            int ty)
        {
            var writeSpan = _offScreen.Span;
            var writeStride = _screenWidth * 4;
            var readSpan = source;
            var readStride = width * 4;
            var numBytesPerLine = Math.Min(writeStride - 4 * tx, readStride);

            writeSpan = writeSpan.Slice(4 * (tx + _screenWidth * ty));

            height = Math.Min(height, _screenHeight - ty);

            for (int y = 0; y < height; y++)
            {
                if (y != 0)
                {
                    readSpan = readSpan.Slice(readStride);
                    writeSpan = writeSpan.Slice(writeStride);
                }

                for (int t = 0; t < numBytesPerLine; t += 4)
                {
                    var s32 = BinaryPrimitives.ReadUInt32LittleEndian(readSpan.Slice(t));
                    var d32 = BinaryPrimitives.ReadUInt32LittleEndian(writeSpan.Slice(t));

                    byte srcAlpha = (byte)(s32 >> 24);
                    if (srcAlpha != 255)
                    {
                        var invSrcAlpha = 255 - srcAlpha;
                        byte srcX = (byte)(s32 >> 16);
                        byte srcY = (byte)(s32 >> 8);
                        byte srcZ = (byte)(s32 >> 0);
                        byte dstX = (byte)(d32 >> 16);
                        byte dstY = (byte)(d32 >> 8);
                        byte dstZ = (byte)(d32 >> 0);

                        d32 = (
                            0xFF000000U
                            | (uint)(byte)((srcX * srcAlpha + dstX * invSrcAlpha) / 255) << 16
                            | (uint)(byte)((srcY * srcAlpha + dstY * invSrcAlpha) / 255) << 8
                            | (uint)(byte)((srcZ * srcAlpha + dstZ * invSrcAlpha) / 255) << 0
                        );
                    }
                    else
                    {
                        d32 = s32;
                    }

                    BinaryPrimitives.WriteUInt32LittleEndian(writeSpan.Slice(t), d32);
                }
            }
        }
    }
}
