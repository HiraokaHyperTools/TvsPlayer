using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.DataTypes
{
    public record TvsChunk(
        long ChunkPosition,
        int CompressedSize,
        int UncompressedSize)
    {
    }
}
