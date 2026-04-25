using LibTvsPlayer.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Services
{
    public class TileConfHelperV2
    {
        public static readonly TileConfHelperV2 Default = new TileConfHelperV2();

        public IReadOnlyList<TileConf> GetTileConfsFromBlockConf(ReadOnlySpan<byte> blockConf)
        {
            var list = new List<TileConf>();
            var even = blockConf.Length == 0 || (blockConf[0] & 1) == 0;
            var count = 0;
            var state = 0;
            var offset = 0;

            if (even)
            {
                list.Add(new TileConf(1, 1, offset, 0));
            }
            for (int y = 0; y < blockConf.Length; y++)
            {
                var one = blockConf[y];

                for (int x = 0; x < 8; x++)
                {
                    var isSet = (one & 1) != 0;

                    if (state == 0) // initial
                    {
                        if (isSet)
                        {
                            state = 1;
                            count = 2;
                        }
                        else
                        {
                            state = 2;
                            count = 1;
                        }
                    }
                    else if (state == 1) // previously set
                    {
                        if (isSet)
                        {
                            count += 1;
                        }
                        else
                        {
                            list.Add(new TileConf(count, 1, offset, 0));
                            offset += count;
                            state = 2;
                            count = 1;
                        }
                    }
                    else if (state == 2) // previously reset
                    {
                        if (isSet)
                        {
                            offset += count;

                            state = 1;
                            count = 1;
                        }
                        else
                        {
                            count += 1;
                        }
                    }

                    one >>= 1;
                }
            }
            if (state == 1 && count != 0)
            {
                list.Add(new TileConf(count, 1, offset, 0));
            }

            return list.AsReadOnly();
        }
    }
}
