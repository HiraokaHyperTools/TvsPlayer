using LibTvsPlayer.DataTypes;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Helpers
{
    public class ParseRect21
    {
        public static readonly ParseRect21 Default = new ParseRect21();

        public IReadOnlyList<Rect21> Parse(ReadOnlySpan<byte> span)
        {
            var array = new Rect21[span.Length / 8];

            for (int x = 0; 8 <= span.Length; x++)
            {
                array[x] = new Rect21(
                    BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(0)),
                    BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(2)),
                    BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(4)),
                    BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(6))
                );

                span = span.Slice(8);
            }

            return array;
        }
    }
}
