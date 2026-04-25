using LibTvsPlayer.DataTypes;
using LibTvsPlayer.Helpers;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Services
{
    public class ReadTvsChunk
    {
        private readonly ParseKey _parseKey;

        public ReadTvsChunk(
            ParseKey parseKey)
        {
            _parseKey = parseKey;
        }

        public async Task<IReadOnlyList<KeyRecord>> ReadAsync(
            ReadAsyncDelegate readAsync,
            TvsChunk chunk)
        {
            using var sourceStream = new DeflateStream(
                new ReadAsyncToStreamProxy(
                    readAsync,
                    chunk.ChunkPosition + 2,
                    chunk.CompressedSize - 2
                ),
                CompressionMode.Decompress
            );
            using var keyFileStream = new MemoryStream();
            await sourceStream.CopyToAsync(keyFileStream);
            if (keyFileStream.Length != chunk.UncompressedSize)
            {
                throw new InvalidDataException($"Expected uncompressed size {chunk.UncompressedSize}, got {keyFileStream.Length}");
            }
            var keyData = keyFileStream.ToArray();
            var keyRecords = _parseKey.Parse(keyData);
            return keyRecords.AsReadOnly();
        }
    }
}
