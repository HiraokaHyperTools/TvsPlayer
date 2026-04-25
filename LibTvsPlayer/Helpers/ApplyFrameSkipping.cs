using LibTvsPlayer.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Helpers
{
    public class ApplyFrameSkipping
    {
        public IReadOnlyList<TimestampedTvsChunkRef> Filter(
            IEnumerable<TimestampedTvsChunkRef> items)
        {
            var list = new List<TimestampedTvsChunkRef>();
            var lastUsedTimestamp = -1;
            TimestampedTvsChunkRef? previous = null;

            foreach (var one in items)
            {
                if (one.IsKeyFrame)
                {
                    if (lastUsedTimestamp != one.Timestamp)
                    {
                        if (previous != null)
                        {
                            list.Add(previous); // insert the last delta frame before adding this key frame.
                        }

                        list.Add(one);
                        lastUsedTimestamp = one.Timestamp;
                    }
                }
                else
                {
                    if (lastUsedTimestamp == -1 || lastUsedTimestamp + 1000 <= one.Timestamp)
                    {
                        list.Add(one);
                        lastUsedTimestamp = one.Timestamp;
                    }
                }

                previous = one;
            }

            return list.AsReadOnly();
        }
    }
}
