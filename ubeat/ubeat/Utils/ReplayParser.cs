using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ubeat.Utils
{
    public class ReplayParser
    {
        public static Replay FromFile(string path)
        {
            Replay tmpReplay = new Replay();
            

            StreamReader strR = new StreamReader(path);


            return tmpReplay;
        }
    }
}
