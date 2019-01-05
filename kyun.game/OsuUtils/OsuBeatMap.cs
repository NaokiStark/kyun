using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Troschuetz.Random.Generators;
using kyun.Beatmap;
using kyun.UIObjs;
using osuBMParser;

namespace kyun.OsuUtils
{
    public class OsuBeatMap : ubeatBeatMap
    {

        public static OsuBeatMap FromFile(string path, bool loadHitobjects = false)
        {
            try
            {

                int leadIn = 0;
                Random cRnd = new Random(DateTime.Now.Millisecond);
                osuBMParser.Beatmap osbm = new osuBMParser.Beatmap(path, loadHitobjects);
                string relPath = new FileInfo(path).DirectoryName;

                OsuGameMode tmpMode = OsuGameMode.Standard;

                switch (osbm.Mode)
                {
                    case 1:
                        tmpMode = OsuGameMode.Taiko;
                        break;
                    case 2:
                        tmpMode = OsuGameMode.CTB;
                        break;
                    case 3:
                        tmpMode = OsuGameMode.Mania;

                        break;
                }

                OsuBeatMap tmpbm = new OsuBeatMap()
                {
                    Artist = StringHelper.SanitizeUnicode(osbm.Artist),
                    ApproachRate = /*(osbm.ApproachRate < 1)? osbm.OverallDifficulty: ((osbm.Mode<1) ?osbm.ApproachRate:osbm.ApproachRate*1.2f)*/ osbm.ApproachRate,
                    BPM = 0,
                    Creator = StringHelper.SanitizeUnicode(osbm.Creator),
                    HPDrainRate = osbm.HpDrainRate,
                    SongPath = relPath + @"\" + osbm.AudioFileName,
                    OverallDifficulty = osbm.OverallDifficulty,
                    Title = StringHelper.SanitizeUnicode(osbm.Title),
                    Version = StringHelper.SanitizeUnicode(osbm.Version),
                    Background = relPath + @"\" + osbm.Background,
                    SleepTime = osbm.AudioLeadIn,
                    Video = relPath + @"\" + osbm.Video,
                    VideoStartUp = osbm.VideoStartUp,
                    osuBeatmapType = osbm.Mode,
                    FilePath = path, //Added
                    TimingPoints = osbm.TimingPoints,
                    SliderMultiplier = osbm.SliderMultiplier,
                    CircleSize = osbm.CircleSize,
                    Osu_Gamemode = tmpMode
                };

                tmpbm.Breaks = new List<Beatmap.Break>();
                foreach (osuBMParser.Break brk in osbm.Breaks)
                {
                    tmpbm.Breaks.Add(Beatmap.Break.FromParser(brk));
                }

                tmpbm.Tags = osbm.Tags;

                int lasN = 0;
                List<IHitObj> hitObjs = new List<Beatmap.IHitObj>();

                TimingPoint tm = osbm.TimingPoints[0];
                int tmCount = 0;
                int fVer = int.Parse((osbm.FormatVersion == null) ? "14" : osbm.FormatVersion);

                int offset = 0;
                /*
                offset = ((fVer >= 14 && osbm.Mode == 0) ? 63 : 53);
                offset -= 35;*/

                /*
                if (fVer > 9)
                     offset -= 98;
                 else if (fVer < 10)
                     offset -= 50;*/

                int col1 = 480 / 3;
                int col2 = (480 / 3) * 2;

                int row1 = 640 / 3;
                int row2 = (640 / 3) * 2;

                if (osbm.HitObjects == null)
                {
                    //WTF
                    osbm.HitObjects = new List<HitObject>();
                    osbm.HitObjects.Add(new HitCircle
                    {
                        Time = 0,
                        Position = new Vector2(0, 0),
                        HitSound = 0,
                        Addition = new List<int>(),
                        IsNewCombo = true
                    });
                }
                else if (osbm.HitObjects.Count < 1)
                {
                    osbm.HitObjects.Add(new HitCircle
                    {
                        Time = 0,
                        Position = new Vector2(0, 0),
                        HitSound = 0,
                        Addition = new List<int>(),
                        IsNewCombo = true
                    });
                }

                // Check this shit 
                if (osbm.HitObjects[0].Time < 2500)
                    leadIn = 2500;

                tmpbm.SleepTime = leadIn; // audio-Video

                tmpbm.BPM = tm.MsPerBeat;

                if (!loadHitobjects)
                {
                    tmpbm.HitObjects = hitObjs;
                    return tmpbm;
                }


                foreach (osuBMParser.HitObject ho in osbm.HitObjects)
                {

                    ho.Time += offset;
                    ho.Time += leadIn;
                    if (tmCount < osbm.TimingPoints.Count)
                    {
                        if ((ho.Time - leadIn) < osbm.TimingPoints[tmCount].Offset)
                        {

                            tmCount++;
                            if (tmCount >= osbm.TimingPoints.Count)
                            {
                                tm = osbm.TimingPoints[0];
                            }
                            else
                            {
                                tm = osbm.TimingPoints[tmCount];
                            }
                        }
                    }

                    int[] location = { 0, 0 };

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
                            Location = ((osbm.Source != "ubeat") ? lasN = GetRnd(97, 106, lasN) : fL + 96),
                            OsuLocation = new Microsoft.Xna.Framework.Vector2(ho.Position.x, ho.Position.y),
                            HitSound = ho.HitSound
                        };

                        int lastCh = 0;

                        if (hitObjs.Count > 0)
                        {
                            if (hitObjs.Last().Location == obj.Location)
                            {
                                lastCh = obj.Location;
                                if (hitObjs.Last() is HitHolder)
                                {
                                    if (hitObjs.Last().EndTime > obj.StartTime || Math.Abs(hitObjs.Last().EndTime - obj.StartTime) < 150)
                                    {
                                        while (lastCh == hitObjs.Last().Location && lastCh == hitObjs[hitObjs.Count - 2].Location)
                                        {
                                            lastCh = obj.Location = lasN = GetRnd(97, 106, lasN);
                                        }
                                    }
                                }
                                else if (hitObjs.Last() is HitButton)
                                {
                                    if (hitObjs.Last().StartTime == obj.StartTime || Math.Abs(hitObjs.Last().StartTime - obj.StartTime) < 150)
                                    {
                                        while (lastCh == hitObjs.Last().Location && lastCh == hitObjs[hitObjs.Count - 2].Location)
                                        {
                                            lastCh = obj.Location = lasN = GetRnd(97, 106, lasN);
                                        }
                                    }
                                }

                            }
                        }

                        obj.MsPerBeat = getTimingPoint((int)obj.StartTime, osbm).MsPerBeat;

                        hitObjs.Add(obj);
                    }
                    else if (ho is osuBMParser.HitSlider)
                    {

                        HitSlider slider = (HitSlider)ho;

                        float sliderVelocityInOsuFormat = slider.SliderTimingPoint.MsPerBeat;

                        if (sliderVelocityInOsuFormat >= 0)
                        {
                            sliderVelocityInOsuFormat = -100f; //DEFAUL VALUE FOR OSU
                        }

                        decimal velocity = (decimal)Math.Abs(100 / sliderVelocityInOsuFormat);

                        if (osbm.SliderMultiplier == 0f)
                            osbm.SliderMultiplier = 1f;

                        decimal pxPerBeat = (decimal)osbm.SliderMultiplier * 100 * velocity;

                        decimal beatsNumber = ((decimal)slider.PixelLength * (decimal)slider.Repeat) / pxPerBeat;

                        TimingPoint tmc = getTimingPoint(slider.Time, osbm);

                        //THE FINAL FUCKING LENGTH TIME FOR THIS SHITTY SLIDER
                        long Length = Math.Abs((long)(beatsNumber * (decimal)tmc.MsPerBeat));



                        IHitObj obj = new HitHolder()
                        {
                            StartTime = ho.Time,
                            Length = Length,
                            EndTime = (Length) + (decimal)ho.Time,
                            BeatmapContainer = tmpbm,
                            //Location = lasN = GetRnd(97, 106, lasN)
                            Location = ((osbm.Source != "ubeat") ? lasN = GetRnd(97, 106, lasN) : fL + 96),
                            OsuLocation = new Microsoft.Xna.Framework.Vector2(ho.Position.x, ho.Position.y),
                            HitSound = ho.HitSound
                        };

                        obj.MsPerBeat = tmc.MsPerBeat;

                        //MANIA LONGNOTE
                        if (slider.Type == HitSlider.SliderType.LONGNOTE)
                        {
                            if (slider.EndTime < slider.Time)
                            {
                                /*
                                ((HitHolder)obj).Length = slider.EndTime;
                                ((HitHolder)obj).EndTime = slider.Time + slider.EndTime;
                                */
                                ((HitHolder)obj).EndTime = slider.EndTime;
                                ((HitHolder)obj).Length = Math.Abs(slider.EndTime - slider.Time);
                            }
                            else
                            {

                                ((HitHolder)obj).EndTime = slider.EndTime;
                                ((HitHolder)obj).Length = Math.Abs(slider.EndTime - slider.Time);

                            }
                        }

                        int lastCh = 0;

                        if (hitObjs.Count > 0)
                        {
                            if (hitObjs.Last().Location == obj.Location)
                            {
                                lastCh = obj.Location;
                                if (hitObjs.Last() is HitHolder)
                                {
                                    if (hitObjs.Last().EndTime > obj.StartTime || Math.Abs(hitObjs.Last().EndTime - obj.StartTime) < 150)
                                    {
                                        while (lastCh == hitObjs.Last().Location && lastCh == hitObjs[hitObjs.Count - 2].Location)
                                        {
                                            lastCh = obj.Location = lasN = GetRnd(97, 106, lasN);
                                        }
                                    }
                                }
                                else if (hitObjs.Last() is HitButton)
                                {
                                    if (hitObjs.Last().StartTime == obj.StartTime || Math.Abs(hitObjs.Last().StartTime - obj.StartTime) < 150)
                                    {
                                        while (lastCh == hitObjs.Last().Location && lastCh == hitObjs[hitObjs.Count - 2].Location)
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
                            Length = (decimal)(((HitSpinner)ho).EndTime + offset + leadIn) - (decimal)((HitSpinner)ho).Time,
                            EndTime = (decimal)((HitSpinner)ho).EndTime + offset + leadIn,
                            //Location = lasN = GetRnd(97, 106,lasN),
                            Location = ((osbm.Source != "ubeat") ? lasN = GetRnd(97, 106, lasN) : fL + 96),
                            BeatmapContainer = tmpbm,
                            OsuLocation = new Microsoft.Xna.Framework.Vector2(640 / 2, 480 / 2),
                            HitSound = ho.HitSound
                        };

                        obj.MsPerBeat = getTimingPoint((int)obj.StartTime, osbm).MsPerBeat;

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
                                        while (lastCh == hitObjs.Last().Location && lastCh == hitObjs[hitObjs.Count - 2].Location)
                                        {
                                            lastCh = obj.Location = lasN = GetRnd(97, 106, lasN);
                                        }
                                    }
                                }
                                else if (hitObjs.Last() is HitButton)
                                {
                                    if (hitObjs.Last().StartTime == obj.StartTime || Math.Abs(hitObjs.Last().StartTime - obj.StartTime) < 2)
                                    {
                                        while (lastCh == hitObjs.Last().Location && lastCh == hitObjs[hitObjs.Count - 2].Location)
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
                tmpbm.BPM = tm.MsPerBeat;
                return tmpbm;
            }
            catch
            {
                return null;
            }
        }

        //public static Random rnd = new Random(DateTime.Now.Millisecond);
        public static NR3Generator rnd = new NR3Generator();

        

        public static int GetRnd(int min, int max, int last)
        {

            int rnddd = rnd.Next(min, max);
            while ((rnddd = rnd.Next(min, max)) == last) ;

            return rnddd;
        }
        public static float getSliderTime(float sliderMultiplier, float beatLength, float pixelLength)
        {
            return beatLength * (pixelLength / sliderMultiplier) / 100f;
        }
        public static TimingPoint GetTimingPointFor(osuBMParser.Beatmap bm, long Time, bool inherit = false)
        {

            for (int a = bm.TimingPoints.Count - 1; a >= 0; a--)
            {
                if (bm.TimingPoints[a].Offset <= Time && (!inherit == (bm.TimingPoints[a].MsPerBeat > 0)))
                {
                    return bm.TimingPoints[a];
                }
            }

            return bm.TimingPoints[0];
        }


        public static TimingPoint getTimingPoint(int time, osuBMParser.Beatmap beatmap)
        {
            for (var i = beatmap.TimingPoints.Count - 1; i >= 0; i--)
            {
                if (beatmap.TimingPoints[i].Offset <= time && beatmap.TimingPoints[i].Inherited) { return beatmap.TimingPoints[i]; }
            }
            return beatmap.TimingPoints[0];
        }

    }


}
