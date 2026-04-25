#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます

using LibTvsPlayer.DataTypes;
using LibTvsPlayer.Helpers;
using LibTvsPlayer.Services;
using NUnit.Framework;
using System.Formats.Tar;
using System.IO.Compression;

namespace LibTvsPlayer.Tests
{
    public class Class1
    {
        [Test]
        [Ignore("private use")]
        public async Task CollectTimestampedTvsChunkRefsTest()
        {
            using var tvsFileStream = File.OpenRead(@"test.tvs");

            Memory<byte> tvsBody;
            {
                using var ms = new MemoryStream();
                await tvsFileStream.CopyToAsync(ms);
                tvsBody = ms.ToArray();
            }

            var list = await new CollectTimestampedTvsChunkRefs(
                new ParseTvsStruc(),
                new ReadTvsChunk(
                    new ParseKey()
                )
            )
                .CollectAsync(
                    async (buffer, position) =>
                    {
                        tvsBody
                            .Slice(Convert.ToInt32(position), buffer.Length)
                            .CopyTo(buffer);
                    }
                );

            list
                .ForEach(it => Console.WriteLine(it));
        }
    }
}
