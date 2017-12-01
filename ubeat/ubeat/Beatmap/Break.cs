using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kyun.Beatmap
{
    public class Break : osuBMParser.Break
    {
        public static Break FromParser(osuBMParser.Break brk)
        {

            return new Break()
            {
                Start = brk.Start,
                End = brk.End
            };
        }
    }
}
