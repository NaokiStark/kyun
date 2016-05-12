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
                    SleepTime  = osbm.AudioLeadIn,
                    Video = relPath + @"\" + osbm.Video
                };

                List<string> tgs = new List<string>();
                foreach (string ttt in osbm.Tags)
                {
                    tgs.Add(ttt);
                }
                tmpbm.Tags = tgs;

                

                int lasN = 0;
                List<IHitObj> hitObjs = new List<Beatmap.IHitObj>();

                TimingPoint tm = osbm.TimingPoints[0];
                int tmCount = 0;
                int fVer = int.Parse(osbm.FormatVersion);

                int offset = ((fVer > 13 && osbm.Mode == 0) ? 100 : 53);

                int col1 = 480 / 3;
                int col2 = (480 / 3)*2;

                int row1 = 640 / 3;
                int row2 = (640 / 3) * 2;

                foreach (osuBMParser.HitObject ho in osbm.HitObjects)
                {
                    ho.Time += offset;
                    if(tmCount < osbm.TimingPoints.Count){
                        if (ho.Time < osbm.TimingPoints[tmCount].Offset)
                        {
                            tmCount++;
                            tm = osbm.TimingPoints[tmCount];                            
                        }
                    }

                    int[] location = {0,0};

                    if (ho.Position.x < row1)
                        location[1] = 1;
                    else if (ho.Position.x < row2)
                        location[1] = 2;
                    else if (ho.Position.x > row2)
                        location[1] = 3;

                    if (ho.Position.y < col1)
                        location[0] = 1;
                    else if (ho.Position.y < col2)
                        location[0] = 2;
                    else if (ho.Position.y > col2)
                        location[0] = 3;

                    int fL = 5;

                    if (location[0] == 1 && location[1] == 1)
                        fL = 7;
                    else if (location[0] == 1 && location[1] == 2)
                        fL = 8;
                    else if (location[0] == 1 && location[1] == 3)
                        fL = 9;
                    else if (location[0] == 2 && location[1] == 1)
                        fL = 4;
                    else if (location[0] == 2 && location[1] == 2)
                        fL = 5;
                    else if (location[0] == 2 && location[1] == 3)
                        fL = 6;
                    else if (location[0] == 3 && location[1] == 1)
                        fL = 1;
                    else if (location[0] == 3 && location[1] == 2)
                        fL = 2;
                    else if (location[0] == 3 && location[1] == 3)
                        fL = 3;

                        if (ho is osuBMParser.HitCircle)
                        {
                            IHitObj obj = new HitButton()
                            {
                                StartTime = ho.Time,
                                BeatmapContainer = tmpbm,
                                //Location = lasN = GetRnd(97, 106,lasN)
                                Location = ((osbm.Source != "ubeat") ? lasN = GetRnd(97, 106, lasN) : fL + 96)
                            };
                            hitObjs.Add(obj);
                        }
                        else if (ho is osuBMParser.HitSlider)
                        {
                            //TimingPoint tmpO = GetTimingPointFor(osbm, (HitSlider)ho, true);
                            TimingPoint tmpO = tm;
                            decimal totalLength = 0;
                            /*
                            decimal totalLength = (decimal)Math.Abs((decimal)tmpO.MsPerBeat) * (decimal)((decimal)((osuBMParser.HitSlider)ho).PixelLength / (decimal)osbm.SliderMultiplier) / (decimal)100;
                            totalLength = totalLength * (decimal)((osuBMParser.HitSlider)ho).Repeat;*/
                            totalLength = (decimal)getSliderTime(osbm.SliderMultiplier, tmpO.MsPerBeat, ((osuBMParser.HitSlider)ho).PixelLength);
                            totalLength = totalLength * (decimal)((osuBMParser.HitSlider)ho).Repeat;
                            IHitObj obj = new HitHolder()
                            {
                                StartTime = ho.Time,
                                Length = totalLength,
                                EndTime = (totalLength) + (decimal)ho.Time,
                                BeatmapContainer = tmpbm,
                                //Location = lasN = GetRnd(97, 106, lasN)
                                Location = ((osbm.Source!="ubeat")?lasN = GetRnd(97, 106, lasN):fL + 96)
                            };
                            hitObjs.Add(obj);
                        }
                        else if (ho is osuBMParser.HitSpinner)
                        {
                            ((HitSpinner)ho).EndTime += offset;
                            IHitObj obj = new HitHolder()
                            {
                                StartTime = (decimal)((HitSpinner)ho).Time,
                                Length = (decimal)((HitSpinner)ho).EndTime - (decimal)((HitSpinner)ho).Time,
                                EndTime = (decimal)((HitSpinner)ho).EndTime,
                                //Location = lasN = GetRnd(97, 106,lasN),
                                Location = ((osbm.Source != "ubeat") ? lasN = GetRnd(97, 106, lasN) : fL + 96),
                                BeatmapContainer = tmpbm

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
        public static float getSliderTime(float sliderMultiplier, float beatLength,float pixelLength)
        {
            return beatLength * (pixelLength / sliderMultiplier) / 100f;
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
