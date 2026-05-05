using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.DataTypes
{
    public record class PackedRectBlock(
        ushort Tx,
        ushort Ty,
        ushort Right,
        ushort Bottom,
        ReadOnlyMemory<byte> Block)
    {
        public static readonly PackedRectBlock Empty = new PackedRectBlock(0, 0, 0, 0, ReadOnlyMemory<byte>.Empty);
    }
}
