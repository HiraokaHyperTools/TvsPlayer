using LibTvsPlayer.DataTypes;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Services
{
    public class DetectVideoFrameSize
    {
        private readonly ParseKey _parseKey;
        private readonly ParseTvsStruc _parseTvsStruc;

        public DetectVideoFrameSize(
            ParseTvsStruc parseTvsStruc,
            ParseKey parseKey)
        {
            _parseKey = parseKey;
            _parseTvsStruc = parseTvsStruc;
        }

        public async Task<Size?> DetectAsync(
            ReadAsyncDelegate readAsync)
        {
            var struc = await _parseTvsStruc.ParseAsync(readAsync)
                .ConfigureAwait(false);

            foreach (var chunkRef in struc.TvsChunks)
            {
                var compressed = new byte[chunkRef.CompressedSize];
                await readAsync(compressed, chunkRef.ChunkPosition)
                    .ConfigureAwait(false);
                using var ms = new MemoryStream(compressed, false);
                using var zlib = new ZLibStream(ms, CompressionMode.Decompress);
                var keyFile = new byte[chunkRef.UncompressedSize];
                await zlib.ReadExactlyAsync(keyFile)
                    .ConfigureAwait(false);

                foreach (var keyRecord in _parseKey.Parse(keyFile))
                {
                    if (true
                        && 2 <= keyRecord.KeyTags.Count
                        && keyRecord.KeyTags[0].Tag == 0x00
                        && keyRecord.KeyTags[0].Value.Length == 4
                        && keyRecord.KeyTags[1].Tag == 0x01
                        && keyRecord.KeyTags[1].Value.Length == 4
                    )
                    {
                        return new Size(
                            BinaryPrimitives.ReadInt32LittleEndian(keyRecord.KeyTags[0].Value.Span),
                            BinaryPrimitives.ReadInt32LittleEndian(keyRecord.KeyTags[1].Value.Span)
                        );
                    }
                }
            }

            return null;
        }
    }
}
