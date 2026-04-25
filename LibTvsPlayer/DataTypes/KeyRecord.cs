using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.DataTypes
{
    public record class KeyRecord(
        int Timestamp,
        ushort RecordType,
        List<KeyTag> KeyTags)
    {
    }
}
