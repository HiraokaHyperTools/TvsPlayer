using LibTvsPlayer.DataTypes;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Helpers
{
    public class ParsePackedRectBlockHelper
    {
        public static readonly ParsePackedRectBlockHelper Default = new ParsePackedRectBlockHelper();

        public PackedRectBlock Parse(ReadOnlyMemory<byte> raw)
        {
            if (8 <= raw.Length)
            {
                var rawSpan = raw.Span;
                return new PackedRectBlock(
                    BinaryPrimitives.ReadUInt16LittleEndian(rawSpan.Slice(0)),
                    BinaryPrimitives.ReadUInt16LittleEndian(rawSpan.Slice(2)),
                    BinaryPrimitives.ReadUInt16LittleEndian(rawSpan.Slice(4)),
                    BinaryPrimitives.ReadUInt16LittleEndian(rawSpan.Slice(6)),
                    raw.Slice(8)
                );
            }
            else
            {
                return PackedRectBlock.Empty;
            }
        }

        public IEnumerable<PackedRectBlock> Convert(IEnumerable<PackedBlock> packedBlocks)
        {
            return packedBlocks
                .Select(it => Parse(it.Block));
        }
    }
}
