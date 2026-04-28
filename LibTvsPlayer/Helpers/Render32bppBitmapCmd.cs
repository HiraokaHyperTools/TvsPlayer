using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Helpers
{
    public record Render32bppBitmapCmd(
        byte[] Bits,
        int Width,
        int Height,
        int Tx,
        int Ty,
        bool Partial);
}
