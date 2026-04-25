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
    public class CollectTimestampedTvsChunkRefs
    {
        private readonly ParseTvsStruc _parseTvsStruc;
        private readonly ReadTvsChunk _readTvsChunk;

        public CollectTimestampedTvsChunkRefs(
            ParseTvsStruc parseTvsStruc,
            ReadTvsChunk readTvsChunk)
        {
            _parseTvsStruc = parseTvsStruc;
            _readTvsChunk = readTvsChunk;
        }

        public async Task<IReadOnlyList<TimestampedTvsChunkRef>> CollectAsync(ReadAsyncDelegate readAsync)
        {
            var list = new List<TimestampedTvsChunkRef>();
            {
                var parsed = await _parseTvsStruc.ParseAsync(readAsync);
                foreach (var chunk in parsed.TvsChunks)
                {
                    var keyRecords = await _readTvsChunk.ReadAsync(
                        readAsync,
                        chunk
                    );

                    var timestamps = new SortedSet<int>();

                    foreach (var (keyRecord, index) in keyRecords.Select((keyRecord, index) => (keyRecord, index)))
                    {
                        timestamps.Add(keyRecord.Timestamp);
                    }

                    list.AddRange(
                        timestamps
                            .Select(
                                (timestamp, index) => new TimestampedTvsChunkRef(
                                    Timestamp: timestamp,
                                    IsKeyFrame: index == 0,
                                    Chunk: chunk
                                )
                            )
                    );
                }
            }

            return list.AsReadOnly();
        }
    }
}
