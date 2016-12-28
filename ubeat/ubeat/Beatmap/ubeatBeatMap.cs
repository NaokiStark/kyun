using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using ubeat.UIObjs;

namespace ubeat.Beatmap
{
    public class ubeatBeatMap : IBeatmap
    {
        public string Title { get;  set; }
        public string Artist { get;  set; }
        public string Creator { get;  set; }
        public string Version { get;  set; }
        public List<string> Tags { get;  set; }
        public float BPM { get;  set; }
        public long MSPB { get;  set; }
        public List<IHitObj> HitObjects { get; set; }
        public string SongPath { get;  set; }
        public float HPDrainRate { get;  set; }
        public float OverallDifficulty { get;  set; }
        public float ApproachRate { get; set; }
        public string Background { get; set; }
        public string Video { get; set; }
        public long VideoStartUp { get; set; }
        public int SleepTime { get; set; }
        public List<Break> Breaks { get; set; }

        public int Timing300
        {
            get
            {
                return (int)((float)Score.ScoreType.Perfect - this.OverallDifficulty * 6f);
            }
        }

        public int Timing100
        {
            get
            {
                return (int)((float)Score.ScoreType.Excellent - this.OverallDifficulty * 6f);
            }
        }

        public int Timing50
        {
            get
            {
                return (int)((float)Score.ScoreType.Good - this.OverallDifficulty * 6f);
            }
        }
        
        public static ubeatBeatMap FromFile(string path)
        {
            StreamReader ubeatFile = new StreamReader(path);

            string fileMap = ubeatFile.ReadToEnd();

            ubeatFile.Close();

            JObject jMap = JObject.Parse(fileMap);
            ubeatBeatMap tmpmap = new ubeatBeatMap() {
                ApproachRate = float.Parse(jMap["approachRate"].ToString()),
                Artist = (string)jMap["artist"],
                Creator = (string)jMap["creator"],
                SongPath = (string)jMap["songPath"],
                Version = (string)jMap["version"],
                Title = (string)jMap["title"],
                Tags = new List<string>(), //ToDo
                Background = (string)jMap["background"],
                HPDrainRate = float.Parse((string)jMap["HPDrain"]),
                SleepTime = int.Parse(jMap["sleep"].ToString()),
                OverallDifficulty = float.Parse((string)jMap["overallDifficulty"]),
                Video = (string)jMap["video"],
                VideoStartUp = int.Parse(jMap["videoStartUp"].ToString()),
            };

            string stringBM = (string)jMap["objects"];

            JArray jamap = JArray.Parse(stringBM);

            List<IHitObj> tmoHitObjs = new List<IHitObj>();
            IHitObj tmpHitObj;
            for (int a = 0; a > jamap.Count; a++)
            {
                tmpHitObj = new HitButton();
                if (jamap[a]["type"].ToString() == "Hit")
                {
                    tmpHitObj = new HitButton()
                    {
                        StartTime=int.Parse(jamap[a]["start"].ToString()),
                        Location = int.Parse(jamap[a]["location"].ToString()),
                        BeatmapContainer = tmpmap
                    };
                }
                else
                {
                    tmpHitObj = new HitHolder()
                    {
                        StartTime = int.Parse(jamap[a]["start"].ToString()),
                        EndTime = int.Parse(jamap[a]["end"].ToString()),
                        Length=int.Parse(jamap[a]["length"].ToString()),
                        Location=int.Parse(jamap[a]["location"].ToString()),
                        BeatmapContainer = tmpmap
                    };
                }
                tmoHitObjs.Add(tmpHitObj);
            }

            tmpmap.HitObjects = tmoHitObjs;

            return tmpmap;
        }
    }
}
