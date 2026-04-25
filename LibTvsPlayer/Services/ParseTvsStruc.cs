using LibTvsPlayer.DataTypes;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Services
{
    public class ParseTvsStruc
    {
        private readonly byte[] _crlf = new byte[] { (byte)'\r', (byte)'\n', };

        public async Task<ParsedTvsStruc> ParseAsync(ReadAsyncDelegate readAsync)
        {
            var header = new byte[1024];
            await readAsync(header, 0);

            var headerDict = new Dictionary<string, string>();

            long ConsumeHeader()
            {
                var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(header));
                {
                    var line = ReadLine(ref reader).Trim();
                    if (line != "TVS")
                    {
                        throw new InvalidDataException($"Expected TVS, got {line}");
                    }
                }
                while (true)
                {
                    var line = ReadLine(ref reader).Trim('\r', '\n');
                    if (line == "BEGIN")
                    {
                        break;
                    }
                    var pair = line.Split('\t', 2);
                    if (pair.Length != 2)
                    {
                        throw new InvalidDataException($"Expected key-value pair, got {line}");
                    }
                    headerDict[pair[0]] = pair[1];
                }

                return reader.Consumed;
            }

            var consumed = ConsumeHeader();

            var list = new List<TvsChunk>();

            while (true)
            {
                var cmd = new byte[3];
                await readAsync(cmd, consumed);
                consumed += 3;
                var cmdStr = Encoding.Latin1.GetString(cmd);
                if (cmdStr == "END")
                {
                    var crlf = new byte[2];
                    await readAsync(crlf, consumed);
                    consumed += 2;
                    if (crlf[0] != '\r' || crlf[1] != '\n')
                    {
                        throw new InvalidDataException($"Expected CRLF after END, got {crlf[0]:X2} {crlf[1]:X2}");
                    }
                    break;
                }
                else if (cmdStr == "KEY")
                {
                    var keyHeader = new byte[8];
                    await readAsync(keyHeader, consumed);
                    consumed += 8;
                    var compressedChunkSize = BinaryPrimitives.ReadInt32LittleEndian(keyHeader.AsSpan(0));
                    var uncompressedChunkSize = BinaryPrimitives.ReadInt32LittleEndian(keyHeader.AsSpan(4));
                    list.Add(new TvsChunk(consumed, compressedChunkSize, uncompressedChunkSize));
                    consumed += compressedChunkSize;
                }
                else if (cmdStr.Substring(0, 2) == "\r\n")
                {
                    // Skip crlf before next command
                    consumed -= 1;
                }
                else
                {
                    throw new InvalidDataException($"Expected KEY or END, got {cmdStr}");
                }
            }

            return new ParsedTvsStruc(
                consumed,
                headerDict,
                list.AsReadOnly()
            );
        }

        private string ReadLine(ref SequenceReader<byte> reader)
        {
            if (reader.TryReadTo(out ReadOnlySpan<byte> line, _crlf, advancePastDelimiter: true))
            {
                return Encoding.Latin1.GetString(line) + "\r\n";
            }

            throw new EndOfStreamException();
        }

    }
}
