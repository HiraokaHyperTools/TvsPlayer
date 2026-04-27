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
            var compressedData = new byte[chunk.CompressedSize];
            await readAsync(compressedData, chunk.ChunkPosition);
            // remove zlib header (2 bytes) and decompress
            using var compressedStream = new MemoryStream(compressedData, 2, compressedData.Length - 2, false);
            using var sourceStream = new DeflateStream(
                compressedStream,
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
