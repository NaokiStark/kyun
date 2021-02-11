using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FreqData
{
    public class FFMpegMp3SkipFrame
    {
        public static int getSkippingFrames(string file) {

            int skipping = 0;

            return 1105;

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "FFMpeg.exe",
                Arguments = $"-i \"{file}\" -f null - -v 48",
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            Process p = new Process();
            p.StartInfo = startInfo;

            p.Start();
            
            bool done = false;

            Regex rg = new Regex("skip (\\d+)/(\\d+) samples");
            while (!done && !p.StandardError.EndOfStream)
            {
                string line = p.StandardError.ReadLine();
                if(line == null)
                {
                    continue;
                }

                if (rg.IsMatch(line))
                {
                    done = true;
                    Match mt = rg.Match(line);
                    Console.WriteLine(1);
                    skipping = int.Parse(mt.Groups[1].Value);
                }
                else{
                    if (p.HasExited)
                    {
                        done = true;
                        skipping = 1105; //??
                    }
                }
            }
            p.WaitForExit();
            return skipping;
        }
    }
}
