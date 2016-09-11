using System;
using System.Collections.Generic;
using System.Linq;
using osuBMParser;
using System.IO;
using ubeat.Beatmap;
using ubeat.UIObjs;
using Troschuetz.Random.Generators;

namespace ubeat.OsuUtils
{
    public class OsuBeatMap : ubeatBeatMap
    {

        public static new OsuBeatMap FromFile(string path)
        {
            try
            {
                int leadIn = 0;
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

                
                tmpbm.Tags = osbm.Tags;

                int lasN = 0;
                List<IHitObj> hitObjs = new List<Beatmap.IHitObj>();

                TimingPoint tm = osbm.TimingPoints[0];
                int tmCount = 0;
                int fVer = int.Parse(osbm.FormatVersion);

                int offset = 0;
                offset = ((fVer >= 14 && osbm.Mode == 0) ? 63 : 53);
                
               /* 
               if (fVer > 9)
                    offset -= 125;
                else if (fVer < 10)
                    offset -= 50;*/

                int col1 = 480 / 3;
                int col2 = (480 / 3)*2;

                int row1 = 640 / 3;
                int row2 = (640 / 3) * 2;

                // Check this shit 
                if (osbm.HitObjects[0].Time < 2500)
                    leadIn = 2500;

                tmpbm.SleepTime = leadIn; // audio-Video

                foreach (osuBMParser.HitObject ho in osbm.HitObjects)
                {
                    ho.Time += offset;
                    ho.Time += leadIn;
                    if(tmCount < osbm.TimingPoints.Count){
                        if ((ho.Time - leadIn) < osbm.TimingPoints[tmCount].Offset)
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

                            int lastCh = 0;

                            if (hitObjs.Count > 0)
                            {
                                if (hitObjs.Last().Location == obj.Location)
                                {
                                    lastCh = obj.Location;
                                    if (hitObjs.Last() is HitHolder)
                                    {
                                        if (hitObjs.Last().EndTime > obj.StartTime || Math.Abs(hitObjs.Last().EndTime - obj.StartTime) < 2)
                                        {
                                            while (lastCh == hitObjs.Last().Location && lastCh == hitObjs[hitObjs.Count-2].Location)
                                            {
                                                lastCh = obj.Location = lasN = GetRnd(97, 106, lasN);
                                            }
                                        }
                                    }
                                    else if (hitObjs.Last() is HitButton)
                                    {
                                        if (hitObjs.Last().StartTime == obj.StartTime || Math.Abs(hitObjs.Last().StartTime - obj.StartTime)<2)
                                        {
                                            while (lastCh == hitObjs.Last().Location && lastCh == hitObjs[hitObjs.Count-2].Location)
                                            {
                                                lastCh = obj.Location = lasN = GetRnd(97, 106, lasN);
                                            }
                                        }
                                    }

                                }
                            }


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


                            int lastCh = 0;

                            if (hitObjs.Count > 0)
                            {
                                if (hitObjs.Last().Location == obj.Location)
                                {
                                    lastCh = obj.Location;
                                    if (hitObjs.Last() is HitHolder)
                                    {
                                        if (hitObjs.Last().EndTime > obj.StartTime || Math.Abs(hitObjs.Last().EndTime - obj.StartTime) < 2)
                                        {
                                            while (lastCh == hitObjs.Last().Location && lastCh == hitObjs[hitObjs.Count-2].Location)
                                            {
                                                lastCh = obj.Location = lasN = GetRnd(97, 106, lasN);
                                            }
                                        }
                                    }
                                    else if (hitObjs.Last() is HitButton)
                                    {
                                        if (hitObjs.Last().StartTime == obj.StartTime || Math.Abs(hitObjs.Last().StartTime - obj.StartTime) < 2)
                                        {
                                            while (lastCh == hitObjs.Last().Location && lastCh == hitObjs[hitObjs.Count-2].Location)
                                            {
                                                lastCh = obj.Location = lasN = GetRnd(97, 106, lasN);
                                            }
                                        }
                                    }
                                }
                            }

                            hitObjs.Add(obj);
                        }
                        else if (ho is osuBMParser.HitSpinner)
                        {

                            IHitObj obj = new HitHolder()
                            {
                                StartTime = (decimal)((HitSpinner)ho).Time,
                                Length = (decimal)(((HitSpinner)ho).EndTime+offset+leadIn) - (decimal)((HitSpinner)ho).Time,
                                EndTime = (decimal)((HitSpinner)ho).EndTime + offset + leadIn,
                                //Location = lasN = GetRnd(97, 106,lasN),
                                Location = ((osbm.Source != "ubeat") ? lasN = GetRnd(97, 106, lasN) : fL + 96),
                                BeatmapContainer = tmpbm

                            };

                            int lastCh = 0;

                            if (hitObjs.Count > 0)
                            {
                                if (hitObjs.Last().Location == obj.Location)
                                {
                                    lastCh = obj.Location;
                                    if (hitObjs.Last() is HitHolder)
                                    {
                                        if (hitObjs.Last().EndTime > obj.StartTime || Math.Abs(hitObjs.Last().EndTime - obj.StartTime) < 2)
                                        {
                                            while (lastCh == hitObjs.Last().Location && lastCh == hitObjs[hitObjs.Count-2].Location)
                                            {
                                                lastCh = obj.Location = lasN = GetRnd(97, 106, lasN);
                                            }
                                        }
                                    }
                                    else if (hitObjs.Last() is HitButton)
                                    {
                                        if (hitObjs.Last().StartTime == obj.StartTime || Math.Abs(hitObjs.Last().StartTime - obj.StartTime) < 2)
                                        {
                                            while (lastCh == hitObjs.Last().Location && lastCh == hitObjs[hitObjs.Count-2].Location)
                                            {
                                                lastCh = obj.Location = lasN = GetRnd(97, 106, lasN);
                                            }
                                        }
                                    }

                                }
                            }

                            hitObjs.Add(obj);
                        }

                }

                tmpbm.HitObjects = hitObjs;
                tmpbm.BPM = GetTimingPointFor(osbm,0).MsPerBeat; 
                return tmpbm;
            }
            catch
            {
                return null;
            }
        }
        
        //public static Random rnd = new Random(DateTime.Now.Millisecond);
        public static NR3Generator rnd = new NR3Generator();
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
        public static TimingPoint GetTimingPointFor(osuBMParser.Beatmap bm, long Time, bool inherit =false)
        {

            for (int a = bm.TimingPoints.Count-1; a >= 0; a--)
            {
                if (bm.TimingPoints[a].Offset <= Time && (!inherit == (bm.TimingPoints[a].MsPerBeat>0)))
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
