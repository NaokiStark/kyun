using FreqData.Generator;
using OsuParsers.Beatmaps;
using OsuParsers.Beatmaps.Objects;
using OsuParsers.Enums.Beatmaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troschuetz.Random.Generators;

namespace FreqData
{
    internal class SongPattern
    {
        /// <summary>
        /// Song Path
        /// </summary>
        internal string SongPath { get; set; }

        public BeatmapDiff difficulty = BeatmapDiff.Easy;

        internal float BPM { get; set; }
        internal int Offset { get; set; }

        internal List<float>[] splitedCols = new List<float>[4];

        /// <summary>
        /// Patterns
        /// </summary>
        internal List<KeyValuePair<float, int>> Patterns = new List<KeyValuePair<float, int>>();


        public SongPattern()
        {
            for (int a = 0; a < splitedCols.Length; a++)
            {
                splitedCols[a] = new List<float>();
            }
        }

        /// <summary>
        /// Add new note
        /// </summary>
        /// <param name="time">Time in ms</param>
        /// <param name="range">Intended for range, (0 = low, 1 = midlow, 2 = highmid, 3 = high)</param>
        internal void Add(float time, int range)
        {

            Console.WriteLine($"Placed object in: {time} | {range}");

            Patterns.Add(new KeyValuePair<float, int>(time, range));

            splitedCols[range].Add(time);

        }


        internal List<HitObject> makePatternsAndAddSliders(Beatmap beatmap, int offset)
        {
            NR3Generator rn = new NR3Generator(180207795);
            
            var temp = new List<HitObject>();

            float beatlength = (60000f / (float)((int)BPM));

            for (int a = 0; a < splitedCols.Length; a++)
            {
                var percusion = HitSoundType.Normal;
                
                int position = 64;
                switch (a)
                {
                    case 0:
                        position = 64;
                       
                        break;
                    case 1:
                        position = 192;
                       
                        break;
                    case 2:
                        position = 320;
                        
                        break;
                    case 3:
                        position = 448;

                        break;
                }

                for (int b = 0; b < splitedCols[a].Count; b++)
                {
                    bool slider = false;
                    int sliderStart = 0;
                    int sliderEnd = 0;


                    if (b < splitedCols[a].Count - 1)
                    {
                        if (b > 0)
                        {
                            float difference = NoteDuration.Sixteenth * (float)beatlength;
                            int positiveIndex = 1;
                            bool endOfSlider = true;
                            while (endOfSlider)
                            {
                                if (b + positiveIndex >= splitedCols[a].Count)
                                {
                                    if (slider)
                                    {
                                        positiveIndex -= 1;
                                    }
                                    endOfSlider = false;
                                    break;
                                }
                                if (splitedCols[a][b + positiveIndex] - splitedCols[a][b + positiveIndex - 1] > difference - 10 &&
                                    splitedCols[a][b + positiveIndex] - splitedCols[a][b + positiveIndex - 1] < difference + 10
                                    )
                                {
                                    slider = true;
                                    if (positiveIndex == 1)
                                    {
                                        sliderStart = b;
                                    }

                                    positiveIndex++;
                                }
                                else
                                {
                                    endOfSlider = false;
                                }
                            }
                            if (slider)
                            {
                                sliderEnd = positiveIndex + b;
                            }

                        }
                    }


                    if((difficulty == BeatmapDiff.Insane || difficulty == BeatmapDiff.Extra) && slider)
                    {
                        if(difficulty == BeatmapDiff.Extra)
                        {
                            slider = rn.NextBoolean(); 
                        }
                        else
                        {
                            int n = rn.Next(0, 2);
                            if(n != 1)
                            {
                                slider = false;
                            }
                        }
                    }

                    if (slider)
                    {
                        List<System.Numerics.Vector2> sliderPoints = new List<System.Numerics.Vector2>();
                        sliderPoints.Add(new System.Numerics.Vector2(position, 0));

                        List<HitSoundType> hitSoundTypes = new List<HitSoundType>();
                        hitSoundTypes.Add(HitSoundType.Normal);

                        List<Tuple<SampleSet, SampleSet>> tuples = new List<Tuple<SampleSet, SampleSet>>();
                        tuples.Add(new Tuple<SampleSet, SampleSet>(SampleSet.Normal, SampleSet.Normal));

                        float diff = (float)splitedCols[a][b] + splitedCols[a][sliderEnd];

                        decimal velocity = (decimal)Math.Abs(100 / beatmap.TimingPoints[1].BeatLength);

                        if (beatmap.DifficultySection.SliderMultiplier == 0f)
                            beatmap.DifficultySection.SliderMultiplier = 1f;

                        decimal pxPerBeat = velocity * 100 * (decimal)beatmap.DifficultySection.SliderMultiplier;

                        float len = ((float)pxPerBeat) * (NoteDuration.Sixteenth * (sliderEnd - b));

                        temp.Add(new Slider(new System.Numerics.Vector2(position, 0),
                           (int)splitedCols[a][b] + (int)offset, (int)splitedCols[a][sliderEnd] + offset, HitSoundType.Normal, CurveType.Linear
                           , sliderPoints,
                           1, len, hitSoundTypes, tuples, new Extras(), false, 0));
                        b = sliderEnd;
                    }
                    else
                    {
                        switch (a)
                        {
                            case 0:
                                var finishOpp = rn.Next(0, 20);
                                if (finishOpp == 2)
                                {
                                    percusion = HitSoundType.Finish;
                                }
                                break;
                            case 1:
                                var clapOpp = rn.Next(0, 20);
                                if (clapOpp == 2)
                                {
                                    percusion = HitSoundType.Clap;
                                }
                                break;
                            case 2:
                                percusion = HitSoundType.Normal;
                                break;
                            case 3:
                                percusion = HitSoundType.Whistle;
                                break;
                        }
                        temp.Add(new Circle(new System.Numerics.Vector2(position, 0),
                                (int)splitedCols[a][b] + offset, 0,
                                percusion, new Extras(), false, 0));
                    }
                }
            }

            temp = temp.OrderBy(x => x.StartTime).ToList<HitObject>();
            return temp;
        }

    }
}
