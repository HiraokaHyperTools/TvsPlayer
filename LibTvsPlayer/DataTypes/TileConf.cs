using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.DataTypes
{
    /// <summary>
    /// 1x1, 2x1, 1x2, and so on
    /// </summary>
    /// <param name="NumX">Number of blocks in right</param>
    /// <param name="NumY">Number of blocks in lower (down)</param>
    /// <param name="XOffset">Skip blocks in right</param>
    /// <param name="YOffset">Skip blocks in lower (down)</param>
    public record TileConf(int NumX, int NumY, int XOffset, int YOffset);
}
