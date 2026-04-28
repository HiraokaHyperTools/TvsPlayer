using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Helpers
{
    /// <summary>
    /// 32-bpp (RGBA) off screen bitmap writer.
    /// </summary>
    public class WriteToOffScreen
    {
        private readonly Memory<byte> _offScreen;
        private readonly int _screenWidth;
        private readonly int _screenHeight;

        public WriteToOffScreen(
            Memory<byte> offScreen,
            int screenWidth,
            int screenHeight)
        {
            _offScreen = offScreen;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
        }

        public void Bitblt(
            ReadOnlySpan<byte> source,
            int width,
            int height,
            int tx,
            int ty)
        {
            var writeSpan = _offScreen.Span;
            var writeStride = _screenWidth * 4;
            var readSpan = source;
            var readStride = width * 4;
            var numBytesPerLine = Math.Min(writeStride - 4 * tx, readStride);

            writeSpan = writeSpan.Slice(4 * (tx + _screenWidth * ty));

            height = Math.Min(height, _screenHeight - ty);

            for (int y = 0; y < height; y++)
            {
                if (y != 0)
                {
                    readSpan = readSpan.Slice(readStride);
                    writeSpan = writeSpan.Slice(writeStride);
                }

                readSpan
                    .Slice(
                        0,
                        numBytesPerLine
                    )
                    .CopyTo(
                        writeSpan
                    );
            }
        }

        public void FillColor(
            byte r,
            byte g,
            byte b,
            int width,
            int height,
            int tx,
            int ty)
        {
            var color = new byte[] { r, g, b, 255 };

            var writeSpan = _offScreen.Span;
            var writeStride = _screenWidth * 4;

            writeSpan = writeSpan.Slice(4 * (tx + _screenWidth * ty));

            for (int y = 0; y < height; y++)
            {
                if (y != 0)
                {
                    writeSpan = writeSpan.Slice(writeStride);
                }

                for (int x = 0; x < width; x++)
                {
                    color.CopyTo(writeSpan.Slice(4 * x));
                }
            }
        }


        public void WriteError(
            string message,
            int width,
            int height,
            int tx,
            int ty)
        {
            FillColor(0xEE, 0x00, 0x00, width, height, tx, ty);
        }
    }
}
