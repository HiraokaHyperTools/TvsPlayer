using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.DataTypes
{
    public record ParsedTvsStruc(
        long AfterEndPosition,
        IDictionary<string, string> HeaderDict,
        IReadOnlyList<TvsChunk> TvsChunks);
}
