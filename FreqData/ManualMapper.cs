using Gma.System.MouseKeyHook;
using Microsoft.Xna.Framework.Input;
using OsuParsers.Beatmaps;
using OsuParsers.Beatmaps.Objects;
using OsuParsers.Decoders;
using OsuParsers.Enums.Beatmaps;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Keys = System.Windows.Forms.Keys;
using XKeys = Microsoft.Xna.Framework.Input.Keys;

namespace FreqData
{
    internal class ManualMapper
    {
        private IKeyboardMouseEvents hk;

        internal Player p;

        internal string File { get; set; }

        internal float Length { get; set; }

        internal float Position
        {
            get
            {
                return p.Position;
            }
            set
            {
                p.Position = (long)value;
            }
        }

        //internal int ObjectCount 


        List<float> dividers = new List<float>();

        List<HitObject> hitObjects = new List<HitObject>();

        internal float TempoDivider;
        int dividerIndex = 0;

        bool[] states = new bool[4] { false, false, false, false };
        readonly Keys[] keys = new Keys[4] { Keys.D, Keys.F, Keys.J, Keys.K };

        KeyEventArgsTimed[] events = new KeyEventArgsTimed[4] {
            new KeyEventArgsTimed(new KeyEventArgs((System.Windows.Forms.Keys)Keys.F), true),
            new KeyEventArgsTimed(new KeyEventArgs((System.Windows.Forms.Keys)Keys.F), true),
            new KeyEventArgsTimed(new KeyEventArgs((System.Windows.Forms.Keys)Keys.J), true),
            new KeyEventArgsTimed(new KeyEventArgs((System.Windows.Forms.Keys)Keys.K), true)
        };

        string[] timelineValues = new string[2];

        internal double bpm = 0;
        internal float mspb = 0;
        internal long gap = 0;

        float nextBeat = 0;
        private SynchronizationContext syncContext;
        private KeyboardState keyboardOldState;

        internal Beatmap bm;

        public ManualMapper(float tempoDivider = NoteDuration.Sixteenth)
        {
            p = new Player();
            TempoDivider = tempoDivider;
            Subscribe();
        }

        internal void loadFile(string fileName)
        {
            hitObjects.Clear();
            File = fileName;
            p.Play(File);
            p.Volume = (float)Mapper.Instance.tMusic.Value / 100f;
            p.Pause();
            Length = p.Length;

            FileInfo file = new FileInfo(fileName);
            FileInfo bpmFile = new FileInfo(Path.Combine(file.DirectoryName, file.Name + ".kbpm"));


            if (!bpmFile.Exists)
            {
                ///??
                return;
            }

            var streamreader = bpmFile.OpenText();
            string[] data = streamreader.ReadToEnd().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            timelineValues = data;

            bpm = toDouble(timelineValues[0]);
            gap = (long)(Math.Round(toDouble(timelineValues[1]), 4) * 1000f);

            mspb = (float)Math.Round(60000d / bpm, 12);

            loadTimeLine(mspb, gap, p.Length);
        }

        internal void loadFile(Beatmap beatmap, string path)
        {
            hitObjects.Clear();
            bm = beatmap;
            File = Path.Combine((new FileInfo(path)).DirectoryName, beatmap.GeneralSection.AudioFilename);
            p.Play(File);
            p.Volume = (float)Mapper.Instance.tMusic.Value / 100f;
            p.Pause();
            Length = p.Length;

            // For now only get 1st timing point
            bpm = 60000d / beatmap.TimingPoints.First().BeatLength;
            gap = beatmap.TimingPoints.First().Offset;

            mspb = (float)beatmap.TimingPoints.First().BeatLength;

            loadTimeLine(mspb, gap, p.Length);

        }


        public void checkKeyboardEvents()
        {
            KeyboardState kbActualState = Keyboard.GetState();



            //if (currentPressedKeys.Length < 1 && oldPressedKeys.Length < 1)
            //    return;
            XKeys[] k = (XKeys[])Enum.GetValues(typeof(XKeys));
            foreach (XKeys aKey in k)
            {
                if (keyboardOldState.IsKeyUp(aKey) && kbActualState.IsKeyDown(aKey))
                {
                    //Console.WriteLine($"pressed: {aKey}");
                    onKeyDown(this, new KeyEventArgsTimed(aKey, false));
                }
                else if (keyboardOldState.IsKeyDown(aKey) && kbActualState.IsKeyUp(aKey))
                {
                    //Console.WriteLine($"released: {aKey}");
                    onKeyUp(this, new KeyEventArgsTimed(aKey, true));
                }
            }

            keyboardOldState = kbActualState;
        }

        internal void loadTimeLine(double mspb, long offset, long duration)
        {
            dividerIndex = 0;
            dividers.Clear();
            float time = offset;

            while (time < duration)
            {
                dividers.Add(time);
                time += AddTime(TempoDivider, (float)mspb);
            }
        }

        private float AddTime(float noteDuration, float mpb)
        {
            return noteDuration * mpb;
        }

        internal void Start(float vel = 1)
        {
            hitObjects.Clear();
            p.Play(File);
            p.SetVelocity(vel);
            nextBeat = gap;
            syncContext = System.Threading.SynchronizationContext.Current;
            Task.Run(() =>
            {

                while (p.PlayState == BassPlayState.Playing)
                {

                    //Thread.Sleep(TimeSpan.FromMilliseconds(0.1));
                    if (dividerIndex >= dividers.Count)
                    {
                        syncContext.Send(state =>
                        {
                            checkKeyboardEvents(); //Send in the main thread
                        }, null);
                        continue;
                    }


                    float position = p.Position - 1;
                    if (position > dividers[dividerIndex]
                        && position > dividers[Math.Min(dividers.Count - 1, dividerIndex + 1)])
                    {

                        dividerIndex++;
                    }

                    if (position >= nextBeat)
                    {
                        EffectsPlayer.PlayEffect(Mapper.metronome, (float)Mapper.Instance.tMetronome.Value / 100f);
                        nextBeat += (mspb * ((Mapper.Instance.checkBox1.Checked) ? TempoDivider : 1));
                        Mapper.keys[4] = true;
                        Task.Run(() => toggleKey(4, false, 30));
                    }

                    syncContext.Send(state =>
                    {
                        checkKeyboardEvents(); //Send in the main thread
                    }, null);
                }

            });
        }


        internal void Stop()
        {
            p.Stop();
        }

        internal void Save()
        {
            FileInfo fileInfo = new FileInfo(File);

            string osuFile = Path.Combine(fileInfo.DirectoryName, @"Generated-" + fileInfo.Name + " [MM].osu");

            if (!System.IO.File.Exists(osuFile))
            {
                System.IO.File.Create(osuFile).Close();
            }
            else
            {
                System.IO.File.Delete(osuFile);
                System.IO.File.Create(osuFile).Close();
            }

            string diff = "";

            while (string.IsNullOrWhiteSpace(diff = Microsoft.VisualBasic.Interaction.InputBox("Diff Name", "Mapper"))) { }

            string creator = "";

            while (string.IsNullOrWhiteSpace(creator = Microsoft.VisualBasic.Interaction.InputBox("Creator name", "Mapper"))) { }

            // All osu! shit

            Beatmap beatmap = BeatmapDecoder.Decode(osuFile);

            beatmap.MetadataSection.Artist = bm.MetadataSection.Artist;
            beatmap.MetadataSection.Creator = creator;
            beatmap.MetadataSection.Title = bm.MetadataSection.Title;
            beatmap.MetadataSection.TitleUnicode = bm.MetadataSection.TitleUnicode;
            beatmap.MetadataSection.Version = diff;
            beatmap.MetadataSection.BeatmapID = -1;
            beatmap.GeneralSection.AudioFilename = new FileInfo(File).Name;
            beatmap.GeneralSection.Countdown = false;
            beatmap.GeneralSection.SampleSet = OsuParsers.Enums.Beatmaps.SampleSet.Normal;
            beatmap.GeneralSection.Mode = OsuParsers.Enums.Ruleset.Mania;
            beatmap.GeneralSection.PreviewTime = 1000;
            beatmap.DifficultySection.ApproachRate = 5;
            beatmap.DifficultySection.CircleSize = 4;
            beatmap.DifficultySection.HPDrainRate = 3;
            beatmap.DifficultySection.OverallDifficulty = 3;
            beatmap.DifficultySection.SliderTickRate = 1;
            beatmap.DifficultySection.SliderMultiplier = 1.4;
            beatmap.EditorSection.DistanceSpacing = 1.2;
            beatmap.EditorSection.BeatDivisor = 4;
            beatmap.EditorSection.GridSize = 4;
            beatmap.EditorSection.TimelineZoom = 1;


            beatmap.Version = 14;
            beatmap.TimingPoints.Add(new OsuParsers.Beatmaps.Objects.TimingPoint
            {
                Offset = (int)gap,
                BeatLength = (60000f / (float)((int)bpm)),
                Inherited = true,
                Effects = Effects.None,
                TimeSignature = TimeSignature.SimpleQuadruple,
                SampleSet = SampleSet.Normal,
                Volume = 60,
            });

            beatmap.TimingPoints.Add(new OsuParsers.Beatmaps.Objects.TimingPoint
            {
                Offset = (int)gap,
                BeatLength = -100,
                Inherited = false,
                Effects = Effects.None,
                TimeSignature = TimeSignature.SimpleQuadruple,
                SampleSet = SampleSet.Normal,
                Volume = 60,
            });

            beatmap.HitObjects = hitObjects;
            beatmap.Write(osuFile);
            Console.WriteLine("osu! beatmap saved");
        }

        public void Subscribe()
        {
            //hk = Hook.GlobalEvents();

            //hk.KeyDown += (obj, args) =>
            //{
            //    onKeyDown(obj, new KeyEventArgsTimed(args, false));
            //}; ;
            //hk.KeyUp += (obj, args) =>
            //{
            //    onKeyUp(obj, new KeyEventArgsTimed(args, true));
            //}; ;

        }

        public void onKeyUp(object sender, KeyEventArgsTimed e)
        {
            processKey(e);
        }

        public void onKeyDown(object sender, KeyEventArgsTimed e)
        {
            processKey(e);
        }

        int lastCol = 0;
        float lastTime = 0;

        private void processKey(KeyEventArgsTimed e)
        {
            if (p.PlayState != BassPlayState.Playing) return;

            if (!keys.Contains(e.KeyData)) { return; }

            int keyIndex = Array.IndexOf(keys, e.KeyData);



            //Console.WriteLine(e.ToString());

            if (e.IsKeyUp)
            {
                if (states[keyIndex])
                {
                    if (e.Timestamp - events[keyIndex].Timestamp < (mspb * p.Velocity) * TempoDivider)
                    {
                        //Console.WriteLine("added circle");
                        //Make circle

                        float time = dividers[dividerIndex];

                        int actualCol = getColFor(keyIndex);

                        if (lastCol == actualCol && time == lastTime)
                        {
                            return;
                        }

                        lastCol = actualCol;
                        lastTime = time;

                        hitObjects.Add(new Circle(new System.Numerics.Vector2(actualCol, 0),
                                (int)time, 0,
                                HitSoundType.Normal, new Extras(), false, 0));
                    }
                    else
                    {


                        //make slider
                        List<System.Numerics.Vector2> sliderPoints = new List<System.Numerics.Vector2>();
                        sliderPoints.Add(new System.Numerics.Vector2(getColFor(keyIndex), 0));

                        List<HitSoundType> hitSoundTypes = new List<HitSoundType>();
                        hitSoundTypes.Add(HitSoundType.Normal);

                        List<Tuple<SampleSet, SampleSet>> tuples = new List<Tuple<SampleSet, SampleSet>>();
                        tuples.Add(new Tuple<SampleSet, SampleSet>(SampleSet.Normal, SampleSet.Normal));

                        int dIndex = dividerIndex;

                        float actualtime = dividers[dIndex];

                        float aDiff = e.Timestamp - events[keyIndex].Timestamp - 10; //20ms to test

                        int sIndex = findKeyTimeFor(actualtime - aDiff);

                        if (dIndex - sIndex < 2 && (TempoDivider <= NoteDuration.Twelfth && dIndex - sIndex < 1))
                        {

                            hitObjects.Add(new Circle(new System.Numerics.Vector2(getColFor(keyIndex), 0),
                                 (int)actualtime, 0,
                                 HitSoundType.Normal, new Extras(), false, 0));
                        }
                        else
                        {

                            if (dIndex - sIndex < 2 && TempoDivider < NoteDuration.Eighth)
                            {
                                hitObjects.Add(new Circle(new System.Numerics.Vector2(getColFor(keyIndex), 0),
                                    (int)actualtime, 0,
                                    HitSoundType.Normal, new Extras(), false, 0));
                            }
                            else
                            {


                                float firstTime = dividers[sIndex];


                                float diff = actualtime - firstTime;

                                decimal velocity = (decimal)Math.Abs(100 / -100);

                                float SliderMultiplier = 1.4f;

                                decimal pxPerBeat = velocity * 100 * (decimal)SliderMultiplier;


                                float c = -1000 * SliderMultiplier * diff / (mspb * (float)velocity);

                                float len = ((dIndex - sIndex) * TempoDivider * (float)pxPerBeat)/*(NoteDuration.Sixteenth * (dIndex - sIndex))*/;

                                //Console.WriteLine($"added slider: | {dIndex} | {sIndex} | {dIndex - sIndex} | {firstTime} | {actualtime} | {diff} | {len}");
                                EffectsPlayer.PlayEffect(Mapper.eff, (float)Mapper.Instance.tEff.Value / 100f);
                                hitObjects.Add(new Slider(new System.Numerics.Vector2(getColFor(keyIndex), 0),
                                   (int)firstTime, (int)actualtime, HitSoundType.Normal, CurveType.Linear
                                   , sliderPoints,
                                   1, len, hitSoundTypes, tuples, new Extras(), false, 0));
                            }
                        }
                    }

                    states[keyIndex] = false;
                    Task.Run(() => toggleKey(keyIndex, false, 20));
                }
            }
            else
            {
                EffectsPlayer.PlayEffect(Mapper.eff, (float)Mapper.Instance.tEff.Value / 100f);
                Mapper.keys[keyIndex] = states[keyIndex] = true;

            }

            events[keyIndex] = e;
        }

        private float findTimeFor(float time)
        {
            float ptime = 0;
            for (int a = 0; a < dividers.Count; a++)
            {
                if (dividers[a] >= time && time < dividers[Math.Min(a + 1, dividers.Count - 1)])
                {
                    ptime = dividers[a];
                    break;
                }

            }

            return ptime;
        }

        private void toggleKey(int keyIndex, bool k, int t = 20)
        {
            Thread.Sleep(t);
            Mapper.keys[keyIndex] = k;
        }

        private int findKeyTimeFor(float time)
        {
            int ptime = 0;
            for (int a = 0; a < dividers.Count; a++)
            {
                if (dividers[a] >= time && time < dividers[Math.Min(a + 1, dividers.Count - 1)])
                {
                    ptime = a;
                    break;
                }
            }

            return ptime;
        }

        private int getColFor(int index)
        {
            int position = 64;
            switch (index)
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
            return position;
        }

        public void Unsubscribe()
        {

            //hk.Dispose();
        }

        private float toFloat(string val)
        {
            return float.Parse(val, CultureInfo.InvariantCulture.NumberFormat);
        }

        private double toDouble(string val)
        {
            return double.Parse(val, CultureInfo.InvariantCulture.NumberFormat);
        }
    }

    internal class KeyEventArgsTimed : KeyEventArgs
    {
        public float Timestamp { get; }
        public bool IsKeyUp { get; }

        public KeyEventArgsTimed(KeyEventArgs e, bool up) : base(e.KeyData)
        {
            IsKeyUp = up;
            Timestamp = (float)DateTime.Now.TimeOfDay.TotalMilliseconds;
        }

        public KeyEventArgsTimed(XKeys e, bool up) : base((System.Windows.Forms.Keys)e)
        {
            IsKeyUp = up;
            Timestamp = (float)DateTime.Now.TimeOfDay.TotalMilliseconds;
        }

        public KeyEventArgsTimed(Keys keyData) : base((System.Windows.Forms.Keys)keyData)
        {
            Timestamp = (float)DateTime.Now.TimeOfDay.TotalMilliseconds;
        }

        public override string ToString()
        {
            return $"{Timestamp} | {IsKeyUp} | {KeyData}";
        }
    }

    internal class NoteDuration
    {
        public const float Whole = 4f;
        public const float Half = 2f;
        public const float Quarter = 1f;
        public const float Eighth = 0.5f;
        public const float Sixteenth = 0.25f;
        public const float Tempo32 = 0.125f; //omG THIS 

        public const float Third = 16f / 3f;
        public const float Sixth = 8f / 3f;
        public const float Twelfth = 4f / 3f;
        public const float TwentyFourth = 2f / 3f;
    }
}
