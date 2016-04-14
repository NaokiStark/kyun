using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using osuBMParser;
using System.IO;
using ubeat.Beatmap;
using ubeat.UIObjs;
namespace ubeat.OsuUtils
{
    public class OsuBeatMap:ubeat.Beatmap.ubeatBeatMap
    {

        public static OsuBeatMap FromFile(string path)
        {
            try
            {
                Random cRnd= new Random(DateTime.Now.Millisecond);
                osuBMParser.Beatmap osbm = new osuBMParser.Beatmap(path);
                string relPath = new FileInfo(path).DirectoryName;
                OsuBeatMap tmpbm = new OsuBeatMap()
                {
                    Artist = osbm.Artist,
                    ApproachRate = osbm.ApproachRate,
                    BPM = 0,
                    Creator = osbm.Creator,
                    HPDrainRate = osbm.HpDrainRate,
                    SongPath = relPath + @"\" + osbm.AudioFileName,
                    OverallDifficulty = osbm.OverallDifficulty,
                    Title = osbm.Title,
                    Version = osbm.Version,
                    Background = relPath + @"\" + osbm.Background,
                    SleepTime  = osbm.AudioLeadIn
                };

                List<string> tgs = new List<string>();
                foreach (string ttt in osbm.Tags)
                {
                    tgs.Add(ttt);
                }
                tmpbm.Tags = tgs;

                int lasN = 0;
                List<IHitObj> hitObjs = new List<Beatmap.IHitObj>();
                foreach (osuBMParser.HitObject ho in osbm.HitObjects)
                {
                    if (ho is osuBMParser.HitCircle)
                    {
                        IHitObj obj = new HitButton()
                        {
                            StartTime = ho.Time,
                            BeatmapContainer=tmpbm,
                            Location = lasN = GetRnd(97, 106,lasN)
                        };
                        hitObjs.Add(obj);
                    }
                    else if (ho is osuBMParser.HitSlider)
                    {
                        TimingPoint tmpO = GetTimingPointFor(osbm, (HitSlider)ho, true);

                        decimal totalLength = (decimal)Math.Abs((decimal)tmpO.MsPerBeat) * (decimal)((decimal)((osuBMParser.HitSlider)ho).PixelLength / (decimal)osbm.SliderMultiplier) / (decimal)100;
                        totalLength = totalLength * (decimal)((osuBMParser.HitSlider)ho).Repeat;
                        IHitObj obj = new HitHolder()
                        {
                            StartTime = ho.Time,
                            Length = totalLength,
                            EndTime = (totalLength) + (decimal)ho.Time,
                            BeatmapContainer=tmpbm,
                            Location = lasN = GetRnd(97, 106, lasN)
                        };
                        hitObjs.Add(obj);
                    }
                    else if (ho is osuBMParser.HitSpinner)
                    {
                        IHitObj obj = new HitHolder()
                        {
                            StartTime = (decimal)((HitSpinner)ho).Time,
                            Length = (decimal)((HitSpinner)ho).EndTime - (decimal)((HitSpinner)ho).Time,
                            EndTime = (decimal)((HitSpinner)ho).EndTime,
                            Location = lasN = GetRnd(97, 106,lasN),
                            BeatmapContainer=tmpbm

                        };
                        hitObjs.Add(obj);
                    }

                }

                tmpbm.HitObjects = hitObjs;

                return tmpbm;
            }
            catch
            {
                return null;
            }
        }
        public static Random rnd = new Random(DateTime.Now.Millisecond);
        public static int GetRnd(int min,int max,int last)
        {

            int rnddd = rnd.Next(min, max);
            while ((rnddd = rnd.Next(min, max)) == last) ;

            return rnddd;
        }
        public static TimingPoint GetTimingPointFor(osuBMParser.Beatmap bm, osuBMParser.HitSlider obj,bool inherit =false)
        {

            for (int a = bm.TimingPoints.Count-1; a >= 0; a--)
            {
                if (bm.TimingPoints[a].Offset <= obj.Time && (inherit == bm.TimingPoints[a].Inherited))
                {
                    return bm.TimingPoints[a];
                }/*
                else if (bm.TimingPoints[a].Offset <= obj.Time && ((inherit == bm.TimingPoints[a].Inherited))||a==0)
                {
                    return bm.TimingPoints[a];
                }*/

            }

            return bm.TimingPoints[0];
        }
    }
}
