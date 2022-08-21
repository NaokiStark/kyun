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
        Dictionary<int, Dictionary<int, Dictionary<int, int>>> sampleSet = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();

        public Dictionary<string, Dictionary<string, Dictionary<int, int>>> nSampleSet = new Dictionary<string, Dictionary<string, Dictionary<int, int>>>();

        public static CustomSampleSet LoadFromBeatmap(string location)
        {
            var tmp = new CustomSampleSet();
            tmp.getMess();

            DirectoryInfo di = new DirectoryInfo(location);

            FileInfo[] wavFiles = di.GetFiles("*.wav");
            FileInfo[] mp3Files = di.GetFiles("*.mp3");
            FileInfo[] oggFiles = di.GetFiles("*.ogg");

            List<FileInfo> files = wavFiles.ToList();
            files.AddRange(mp3Files);
            files.AddRange(oggFiles);


            foreach (FileInfo f in files)
            {
                try
                {
                    string filename = f.Name;
                    Regex regexSampleSet = new Regex(@"([A-Za-z]+)\-[A-Za-z]+[0-9]+\.[mp3|wav|ogg]");
                    Regex regexSampleSetN1 = new Regex(@"([A-Za-z]+)\-[A-Za-z]+\.[mp3|wav|ogg]");

                    Regex regexCustomSampleSet = new Regex(@"[A-Za-z]+\-[A-Za-z]+([0-9]+)\.[mp3|wav|ogg]");
                    Regex regexInstrument = new Regex(@"[A-Za-z]+\-([A-Za-z]+)[0-9]+\.[mp3|wav|ogg]");
                    Regex regexInstrumentN1 = new Regex(@"[A-Za-z]+\-([A-Za-z]+)\.[mp3|wav|ogg]");

                    string finalSampleset = regexSampleSet.Match(filename).Groups[1].Value;
                    string finalCustomSampleSet = regexCustomSampleSet.Match(filename).Groups[1].Value;
                    string finalInstrument = regexInstrument.Match(filename).Groups[1].Value;
                    if (string.IsNullOrWhiteSpace(finalSampleset))
                    {
                        finalSampleset = regexSampleSetN1.Match(filename).Groups[1].Value;
                        if (string.IsNullOrWhiteSpace(finalSampleset))
                        {
                            continue;
                        }
                        finalCustomSampleSet = "1";
                        finalInstrument = regexInstrumentN1.Match(filename).Groups[1].Value;
                    }
                    //int fileOpen = SpritesContent.Instance.LoadSoundBass(Path.Combine(location, filename));
                    int fileOpen = SpritesContent.Instance.LoadSoundBass(Path.Combine(location, filename));
                    if (finalInstrument == "sliderslide")
                    {
                        continue;
                    }
                    // ok!
                    if (fileOpen != 0)
                    {
                        int cSampleInt = int.Parse(finalCustomSampleSet); // custom
                        try
                        {
                            tmp.nSampleSet[finalSampleset][finalInstrument].Add(cSampleInt, fileOpen);
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        Logger.Instance.Warn("Error loading sample: " + filename);
                    }
                }
                catch (Exception ex)
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
                string cinstru = "hitnormal";
                switch (instrument)
                {
                    case 2:
                        cinstru = "hitwhistle";
                        break;
                    case 4:
                        cinstru = "hitfinish";
                        break;
                    case 8:
                        cinstru = "hitclap";
                        break;
                }

                string strsampleset = "default";

                switch (sampleset)
                {
                    case 1:
                        strsampleset = "normal";
                        break;
                    case 2:
                        strsampleset = "soft";
                        break;
                    case 3:
                        strsampleset = "drum";
                        break;
                }

                if (sampleset >= nSampleSet.Count)
                {
                    return 0;
                }
                try
                {
                    if (nSampleSet[strsampleset][cinstru].ContainsKey(customSampleset))
                    {
                        value = nSampleSet[strsampleset][cinstru][customSampleset];
                    }

                }
                catch
                {
                    value = 0;
                }
            }
            catch { }

            return value;
        }

        private void getMess()
        {

            nSampleSet.Add("default", new Dictionary<string, Dictionary<int, int>>());
            nSampleSet.Add("normal", new Dictionary<string, Dictionary<int, int>>());
            nSampleSet.Add("soft", new Dictionary<string, Dictionary<int, int>>());
            nSampleSet.Add("drum", new Dictionary<string, Dictionary<int, int>>());

            nSampleSet["default"].Add("hitnormal", new Dictionary<int, int>());
            nSampleSet["default"].Add("hitwhistle", new Dictionary<int, int>());
            nSampleSet["default"].Add("hitfinish", new Dictionary<int, int>());
            nSampleSet["default"].Add("hitclap", new Dictionary<int, int>());

            nSampleSet["normal"].Add("hitnormal", new Dictionary<int, int>());
            nSampleSet["normal"].Add("hitwhistle", new Dictionary<int, int>());
            nSampleSet["normal"].Add("hitfinish", new Dictionary<int, int>());
            nSampleSet["normal"].Add("hitclap", new Dictionary<int, int>());

            nSampleSet["soft"].Add("hitnormal", new Dictionary<int, int>());
            nSampleSet["soft"].Add("hitwhistle", new Dictionary<int, int>());
            nSampleSet["soft"].Add("hitfinish", new Dictionary<int, int>());
            nSampleSet["soft"].Add("hitclap", new Dictionary<int, int>());

            nSampleSet["drum"].Add("hitnormal", new Dictionary<int, int>());
            nSampleSet["drum"].Add("hitwhistle", new Dictionary<int, int>());
            nSampleSet["drum"].Add("hitfinish", new Dictionary<int, int>());
            nSampleSet["drum"].Add("hitclap", new Dictionary<int, int>());
        }

        public void CleanUp()
        {
            foreach (string itm in nSampleSet.Keys)
            {
                foreach (string dicx in nSampleSet[itm].Keys)
                {
                    foreach (int channel in nSampleSet[itm][dicx].Values)
                    {
                        if (channel != 0)
                        {
                            SpritesContent.Instance.FreeAudioChannel(channel);
                        }
                    }
                }
            }
        }
    }
}
