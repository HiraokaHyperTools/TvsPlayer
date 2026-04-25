using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Services
{
    public delegate Task ReadAsyncDelegate(Memory<byte> buffer, long position);
}
