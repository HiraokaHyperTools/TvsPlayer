using LibTvsPlayer.DataTypes;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Helpers
{
    public class DecodeMouseRecord
    {
        public class State
        {

        }

        public record Changes(
            Point? SetMousePos = null,
            ReadOnlyMemory<byte>? SetMouseBitmap = null);

        public Changes Consume(KeyRecord keyRecord, State state)
        {
            if ((keyRecord.RecordType & 0xFF) == 0x000C)
            {
                var keyTags = keyRecord.KeyTags;

                var tag00 = keyTags.SingleOrDefault(it => it.Tag == 0x00);
                var tag06 = keyTags.SingleOrDefault(it => it.Tag == 0x06);
                var tag07 = keyTags.SingleOrDefault(it => it.Tag == 0x07);

                Point? setMousePos = null;
                ReadOnlyMemory<byte>? setMouseBitmap = null;

                if (true
                    && tag06 != null
                    && tag06.Value.Length == 4
                    && tag07 != null
                    && tag07.Value.Length == 4
                )
                {
                    setMousePos = new Point(
                        BinaryPrimitives.ReadInt32LittleEndian(tag06.Value.Span),
                        BinaryPrimitives.ReadInt32LittleEndian(tag07.Value.Span)
                    );
                }

                if (true
                    && tag00 != null
                    && tag00.Value.Length == 4096
                )
                {
                    setMouseBitmap = tag00.Value;
                }

                return new Changes(
                    SetMousePos: setMousePos,
                    SetMouseBitmap: setMouseBitmap
                );
            }

            return new Changes();
        }
    }
}
