using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Helpers
{
    public delegate void RenderJpegDelegate(ReadOnlyMemory<byte> Jpeg, int width, int height, int tx, int ty);
}
