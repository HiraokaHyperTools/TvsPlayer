using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Helpers
{
    public record class FillColorCmd(
        byte R,
        byte G,
        byte B,
        int Width,
        int Height,
        int Tx,
        int Ty);
}
