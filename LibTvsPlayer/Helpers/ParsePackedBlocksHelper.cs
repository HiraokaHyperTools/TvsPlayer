using LibTvsPlayer.DataTypes;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Helpers
{
    public class ParsePackedBlocksHelper
    {
        public static readonly ParsePackedBlocksHelper Default = new ParsePackedBlocksHelper();

        public PackedBlock[]? ParsePackedBlocks(ReadOnlyMemory<byte> ptr)
        {
            var list = new List<PackedBlock>();

            while (ptr.Length != 0)
            {
                if (ptr.Length < 3)
                {
                    return null;
                }
                var ver = ptr.Span[0];
                ptr = ptr.Slice(1);
                if (ver != 1)
                {
                    return null;
                }
                var len = BinaryPrimitives.ReadUInt16LittleEndian(ptr.Span);
                ptr = ptr.Slice(2);
                if (ptr.Length < len)
                {
                    return null;
                }
                list.Add(new PackedBlock(ptr.Slice(0, len)));
                ptr = ptr.Slice(len);
            }

            return list.ToArray();
        }
    }
}
