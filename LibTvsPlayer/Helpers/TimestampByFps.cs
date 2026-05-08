using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Helpers
{
    public class TimestampByFps
    {
        private decimal _step;
        private decimal _next;

        public TimestampByFps(decimal fps)
        {
            _step = 1000 / fps;
        }

        public int Reach(int givenTimestamp)
        {
            var num = 0;
            while (_next < givenTimestamp)
            {
                _next += _step;
                num += 1;
            }
            return num;
        }
    }
}
