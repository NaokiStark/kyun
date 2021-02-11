using kyun.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace kyun.game.Beatmap
{
    public class CustomSampleSet
    {
        Dictionary<int, Dictionary<int, Dictionary<int, int>>> sampleSet = new Dictionary<int, Dictionary<int,Dictionary<int, int>>>();

        public List<List<Dictionary<int, int>>> nSampleSet = new List<List<Dictionary<int, int>>>();

        public static CustomSampleSet LoadFromBeatmap(string location)
        {
            var tmp = new CustomSampleSet();

            tmp.nSampleSet.Add(new List<Dictionary<int, int>>());
            tmp.nSampleSet.Add(new List<Dictionary<int, int>>());
            tmp.nSampleSet.Add(new List<Dictionary<int, int>>());

            for(int a = 0; a < 3; a++)
            {
                for(int b = 0; b < 4; b++)
                {
                    tmp.nSampleSet[a].Add(new Dictionary<int, int>());
                }
            }
            


            DirectoryInfo di = new DirectoryInfo(location);

            FileInfo[] wavFiles = di.GetFiles("*.wav");
            FileInfo[] mp3Files = di.GetFiles("*.mp3");

            List<FileInfo> files = wavFiles.ToList();
            files.AddRange(mp3Files);
            
            
            foreach (FileInfo f in files)
            {
                try
                {
                    string filename = f.Name;
                    Regex regexSampleSet = new Regex(@"([A-Za-z]+)\-[A-Za-z]+[0-9]+.[mp3|wav]");
                    Regex regexCustomSampleSet = new Regex(@"[A-Za-z]+\-[A-Za-z]+([0-9]+).[mp3|wav]");
                    Regex regexInstrument = new Regex(@"[A-Za-z]+\-([A-Za-z]+)[0-9]+.[mp3|wav]");

                    string finalSampleset = regexSampleSet.Match(filename).Groups[1].Value;
                    string finalCustomSampleSet = regexCustomSampleSet.Match(filename).Groups[1].Value;
                    string finalInstrument = regexInstrument.Match(filename).Groups[1].Value;
                    if (string.IsNullOrWhiteSpace(finalSampleset))
                    {
                        continue;
                    }
                    int fileOpen = SpritesContent.Instance.LoadSoundBass(Path.Combine(location, filename));
                    // ok!
                    if (fileOpen != 0)
                    {
                        int cSampleInt = int.Parse(finalCustomSampleSet); // custom
                        int instrumentSample = 0; //
                        int sampleInt = 0; // 0 = default, 1 = normal, 2 = soft, 3 = drum
                        switch (finalInstrument.ToLower())
                        {
                            case "hitnormal":
                                instrumentSample = 0;
                                break;
                            case "hitclap":
                                instrumentSample = 2;
                                break;
                            case "hitfinish":
                                instrumentSample = 4;
                                break;
                            case "hitwhistle":
                                instrumentSample = 8;
                                break;
                        }
                        switch (finalSampleset.ToLower())
                        {
                            case "normal":
                                sampleInt = 1;
                                break;
                            case "soft":
                                sampleInt = 2;
                                break;
                            case "drum":
                                sampleInt = 3;
                                break;
                        }

                        tmp.nSampleSet[sampleInt - 1][(int)Math.Round(Math.Sqrt(instrumentSample), 0)].Add(cSampleInt, fileOpen);
                    }
                    else
                    {
                        Logger.Instance.Warn("Error loading sample: " + filename);
                    }
                }
                catch(Exception ex)
                {
                    Logger.Instance.Warn(ex.StackTrace);
                    continue;
                }
            }

            

            return tmp;
        }

        public int GetSample(int sampleset, int instrument, int customSampleset)
        {
            int value = 0;

            try
            {
                int cinstru = 0;
                switch (instrument)
                {
                    case 2:
                        cinstru = 1;
                        break;
                    case 4:
                        cinstru = 2;
                        break;
                    case 8:
                        cinstru = 3;
                        break;
                }

                if(nSampleSet.Count - 1 < sampleset)
                {
                    return 0;
                }

                if (nSampleSet[sampleset - 1][cinstru].ContainsKey(customSampleset))
                {
                    value = nSampleSet[sampleset - 1][cinstru][customSampleset];
                }
                //Old version or unproper hitsounding maybe?
                /*
                else if (nSampleSet[sampleset][cinstru].ContainsKey(customSampleset)){
                    value = nSampleSet[sampleset][cinstru][customSampleset];
                }*/

                
            }
            catch { }

            return value;           
        }

        public void CleanUp()
        {
            foreach(List<Dictionary<int, int>> itm in nSampleSet)
            {
                foreach(Dictionary<int, int> dicx in itm)
                {
                    foreach (int channel in dicx.Values)
                    {
                        if(channel != 0)
                        {
                            SpritesContent.Instance.FreeAudioChannel(channel);
                        }
                    }
                }
            }
        }
    }
}
