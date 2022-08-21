﻿using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using kyun.UIObjs;
using System;
using osuBMParser;

namespace kyun.Beatmap
{
    public class ubeatBeatMap : IBeatmap
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Creator { get; set; }
        public string Version { get; set; }
        public List<string> Tags { get; set; }
        public float BPM { get; set; }
        public long MSPB { get; set; }
        public List<IHitObj> HitObjects { get; set; }
        public string SongPath { get; set; }
        public float HPDrainRate { get; set; }
        public float OverallDifficulty { get; set; }
        public float ApproachRate { get; set; }
        public string Background { get; set; }
        public string Video { get; set; }
        public long VideoStartUp { get; set; }
        public int SleepTime { get; set; }
        public List<Break> Breaks { get; set; }
        public List<TimingPoint> TimingPoints { get; set; }
        public int osuBeatmapType { get; set; }
        public float SliderMultiplier { get; set; }
        public float CircleSizeBase = 3;
        internal float cs = 0;
        public OsuGameMode Osu_Gamemode { get; set; }

        public float CircleSize
        {
            get
            {
                return Math.Max(cs, CircleSizeBase);
            }
            set
            {
                cs = value;
            }
        }

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

        public string FilePath { get; set; }

        public static ubeatBeatMap FromFile(string path)
        {
            StreamReader ubeatFile = new StreamReader(path);

            string fileMap = ubeatFile.ReadToEnd();

            ubeatFile.Close();

            JObject jMap = JObject.Parse(fileMap);

            OsuGameMode tmpMode = OsuGameMode.Standard;


            ubeatBeatMap tmpmap = new ubeatBeatMap()
            {
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
                FilePath = path,
                Osu_Gamemode = tmpMode
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
                        StartTime = int.Parse(jamap[a]["start"].ToString()),
                        Location = int.Parse(jamap[a]["location"].ToString()),
                        BeatmapContainer = tmpmap,
                        HitSound = 0
                    };
                }
                else
                {
                    tmpHitObj = new HitHolder()
                    {
                        StartTime = int.Parse(jamap[a]["start"].ToString()),
                        EndTime = int.Parse(jamap[a]["end"].ToString()),
                        Length = int.Parse(jamap[a]["length"].ToString()),
                        Location = int.Parse(jamap[a]["location"].ToString()),
                        BeatmapContainer = tmpmap,
                        HitSound = 0
                    };
                }
                tmoHitObjs.Add(tmpHitObj);
            }

            tmpmap.HitObjects = tmoHitObjs;

            return tmpmap;
        }

        public TimingPoint GetTimingPointFor(long time, bool inherited = true)
        {
            if (TimingPoints == null)
            {
                return new TimingPoint(0, -100, TimingPoints[0].Meter, TimingPoints[0].SampleType, TimingPoints[0].SampleSet, TimingPoints[0].Volume, false, false);
            }
            else if (time == 0 && !inherited)
            {
                return TimingPoints[0];
            }
            else if (time == 0 && inherited)
            {
                return new TimingPoint(0, -100, TimingPoints[0].Meter, TimingPoints[0].SampleType, TimingPoints[0].SampleSet, TimingPoints[0].Volume, true, false); ;
            }

            for (var i = TimingPoints.Count - 1; i >= 0; i--)
            {
                if (TimingPoints[i].Offset <= time && (!(TimingPoints[i].MsPerBeat < 0) && !inherited)) { return TimingPoints[i]; }
            }
            return TimingPoints[0];
        }

        public TimingPoint GetTimingPointForV2(long time)
        {
            if (TimingPoints == null)
                return null;

            TimingPoint tm = TimingPoints[0];
            for (var i = TimingPoints.Count - 1; i > -1; i--)
            {
                if (TimingPoints[i].Offset <= time)
                {
                    tm = TimingPoints[i];
                    break;
                }
            }

            return tm;
        }

        public TimingPoint GetNextTimingPointFor(long offset)
        {
            if (TimingPoints == null)
                return null;

            TimingPoint tm = TimingPoints[0];
            for (var i = TimingPoints.Count - 1; i > -1; i--)
            {
                if (i + 1 < TimingPoints.Count)
                {
                    if (TimingPoints[i].Offset <= offset)
                    {
                        tm = TimingPoints[i + 1];
                        break;
                    }
                }   
                //tm = TimingPoints[i];                
            }

            return tm;
        }

        public TimingPoint GetInheritedPointFor(long time)
        {
            if (TimingPoints == null)
                return null;

            return TimingPoints.Find((x) =>
            {
                return (x.MsPerBeat < 0) && x.Offset >= time;
            });
        }
    }
}
