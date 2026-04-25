using LibTvsPlayer.DataTypes;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Services
{
    public class ParseKey
    {
        public List<KeyRecord> Parse(ReadOnlyMemory<byte> ptr)
        {
            var keyRecords = new List<KeyRecord>();
            var span = ptr.Span;

            while (span.Length != 0)
            {
                if (span.Length < 10)
                {
                    throw new EndOfStreamException();
                }
                var crlf1 = BinaryPrimitives.ReadInt16LittleEndian(span);
                if (crlf1 != 0x0A0D)
                {
                    throw new InvalidDataException($"Expected CRLF, got {crlf1:X4}");
                }
                var timestamp = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(2));
                var crlf2 = BinaryPrimitives.ReadInt16LittleEndian(span.Slice(6));
                if (crlf2 != 0x0A0D)
                {
                    throw new InvalidDataException($"Expected CRLF, got {crlf2:X4}");
                }
                var chunkTag = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(8));

                var keyTags = new List<KeyTag>();

                span = span.Slice(10);
                while (true)
                {
                    if (span.Length == 0)
                    {
                        // EOF
                        break;
                    }
                    if (span.Length < 2)
                    {
                        throw new EndOfStreamException();
                    }
                    var ifCrlf = BinaryPrimitives.ReadInt16LittleEndian(span);
                    if (ifCrlf == 0x0A0D)
                    {
                        break;
                    }
                    var tag = span[0];
                    var len = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(1));
                    span = span.Slice(5);
                    if (span.Length < len)
                    {
                        throw new EndOfStreamException();
                    }

                    keyTags.Add(
                        new KeyTag(
                            Tag: tag,
                            Value: span.Slice(0, len).ToArray()
                        )
                    );

                    span = span.Slice(len);
                }

                keyRecords.Add(
                    new KeyRecord(
                        Timestamp: timestamp,
                        RecordType: chunkTag,
                        KeyTags: keyTags
                    )
                );
            }

            return keyRecords;
        }
    }
}
