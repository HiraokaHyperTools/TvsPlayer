using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.DataTypes
{
    public record TimestampedTvsChunkRef(
        int Timestamp,
        bool IsKeyFrame,
        TvsChunk Chunk)
    {
    }
}
