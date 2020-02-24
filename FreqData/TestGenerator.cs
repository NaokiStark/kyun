using FreqData.Generator;
using OsuParsers.Beatmaps;
using OsuParsers.Beatmaps.Objects;
using OsuParsers.Decoders;
using OsuParsers.Enums.Beatmaps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Un4seen.Bass;

namespace FreqData
{
    public partial class TestGenerator : Form
    {

        Player player;
        private int metronome;
        private int eff;
        private int eff2;
        private int eff3;
        private int eff4;
        SongPattern result;

        BeatmapDiff difficulty = BeatmapDiff.Easy;

        TempoDivider divider = TempoDivider.Four;

        float v1, v2, v3, v4;

        public TestGenerator()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {

            var opd = new OpenFileDialog();


            opd.Filter = "AudioFile|*.mp3|All files|*.*";
            // opd.FilterIndex = 2;
            opd.ShowDialog();

            if (opd.FileName == "")
                return;

            button2.Enabled = false;
            var gen = new PatternGenerator();
            result = await gen.Generate(opd.FileName, difficulty, divider);



            if (result.Patterns.Count < 1)
            {
                MessageBox.Show("Error!");
                return;
            }

            result.SongPath = opd.FileName;

            Text = $"Count: {result.Patterns.Count}";


            button2.Enabled = saveBM.Enabled = true;
        }

        private void saveBM_Click(object sender, EventArgs e)
        {

            FileInfo fileInfo = new FileInfo(result.SongPath);

            string osuFile = Path.Combine(fileInfo.DirectoryName, @"Generated-" + fileInfo.Name + " ["+ difficulty.ToString() + "].osu");

            if (!File.Exists(osuFile))
            {
                File.Create(osuFile).Close();
            }
            else
            {
                File.Delete(osuFile);
                File.Create(osuFile).Close();
            }

            Beatmap beatmap = BeatmapDecoder.Decode(osuFile);

            beatmap.MetadataSection.Artist = "kyun";
            beatmap.MetadataSection.Creator = "[ NekoChan ]";
            beatmap.MetadataSection.Title = "Beatmap generated";
            beatmap.MetadataSection.Version = difficulty.ToString();
            beatmap.MetadataSection.BeatmapID = -1;
            beatmap.GeneralSection.AudioFilename = result.SongPath;
            //beatmap.GeneralSection.AudioLeadIn = 1000;
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
                Offset = result.Offset + (int)offsetNUD.Value,
                BeatLength = (60000f / (float)((int)result.BPM)),
                Inherited = true,
                Effects = Effects.None,
                TimeSignature = TimeSignature.SimpleQuadruple,
                SampleSet = SampleSet.Normal,
                Volume = 60,
            });

            beatmap.TimingPoints.Add(new OsuParsers.Beatmaps.Objects.TimingPoint
            {
                Offset = result.Offset + (int)offsetNUD.Value,
                BeatLength = -100,
                Inherited = false,
                Effects = Effects.None,
                TimeSignature = TimeSignature.SimpleQuadruple,
                SampleSet = SampleSet.Normal,
                Volume = 60,
            });

            //for (int a = 0; a < result.Patterns.Count; a++)
            //{
            //    int position = new System.Random((int)result.Patterns[a].Key).Next(640);

            //    switch (result.Patterns[a].Value)
            //    {
            //        case 0:
            //            position = 64;
            //            break;
            //        case 1:
            //            position = 192;
            //            break;
            //        case 2:
            //            position = 320;
            //            break;
            //        case 3:
            //            position = 448;
            //            break;
            //    }

            //bool slider = false;
            //int sliderStart = 0;
            //int sliderEnd = 0;

            ////make sliders in same row
            //if (a < result.Patterns.Count - 1)
            //{
            //    if (a > 0)
            //    {

            //        float difference = NoteDuration.Sixteenth * (float)beatmap.TimingPoints[0].BeatLength;
            //        int positiveIndex = 1;
            //        bool endOfSlider = true;
            //        while (endOfSlider)
            //        {
            //            if (a + positiveIndex >= result.Patterns.Count)
            //            {
            //                if (slider)
            //                {
            //                    positiveIndex -= 1;
            //                }
            //                endOfSlider = false;
            //                break;
            //            }
            //            if (result.Patterns[a + positiveIndex].Key - result.Patterns[a + positiveIndex - 1].Key > difference - 10 &&
            //                result.Patterns[a + positiveIndex].Key - result.Patterns[a + positiveIndex - 1].Key < difference + 10
            //                )
            //            {
            //                if (result.Patterns[a + positiveIndex].Value == result.Patterns[a + positiveIndex - 1].Value)
            //                {
            //                    slider = true;
            //                    if (positiveIndex == 1)
            //                    {
            //                        sliderStart = a;
            //                    }

            //                }
            //                positiveIndex++;
            //            }
            //            else
            //            {
            //                endOfSlider = false;
            //            }
            //        }
            //        if (slider)
            //        {
            //            sliderEnd = positiveIndex + a;
            //        }

            //    }
            //}

            //if (slider)
            //{
            //    List<System.Numerics.Vector2> sliderPoints = new List<System.Numerics.Vector2>();
            //    sliderPoints.Add(new System.Numerics.Vector2(position, 0));

            //    List<HitSoundType> hitSoundTypes = new List<HitSoundType>();
            //    hitSoundTypes.Add(HitSoundType.Normal);

            //    List<Tuple<SampleSet, SampleSet>> tuples = new List<Tuple<SampleSet, SampleSet>>();
            //    tuples.Add(new Tuple<SampleSet, SampleSet>(SampleSet.Normal, SampleSet.Normal));

            //    float diff = (float)result.Patterns[a].Key + result.Patterns[sliderEnd].Key;

            //    decimal velocity = (decimal)Math.Abs(100 / beatmap.TimingPoints[1].BeatLength);

            //    if (beatmap.DifficultySection.SliderMultiplier == 0f)
            //        beatmap.DifficultySection.SliderMultiplier = 1f;

            //    decimal pxPerBeat = velocity * 100 / (decimal)beatmap.DifficultySection.SliderMultiplier;

            //    float len = ((float)pxPerBeat * NoteDuration.Sixteenth) * (sliderEnd - a);

            //    beatmap.HitObjects.Add(new Slider(new System.Numerics.Vector2(position, 0),
            //       (int)result.Patterns[a].Key + (int)offsetNUD.Value, (int)result.Patterns[sliderEnd].Key + (int)offsetNUD.Value, HitSoundType.Normal, CurveType.Linear
            //       , sliderPoints,
            //       1, (float)pxPerBeat, hitSoundTypes, tuples, new Extras(), false, 0));

            //    for(int c = a; c < sliderEnd; c++)
            //    {
            //        if(result.Patterns[c].Value == result.Patterns[a].Value)
            //        {
            //            result.Patterns.RemoveAt(c);
            //        }                        
            //    }
            //}
            //else
            //{
            //beatmap.HitObjects.Add(new Circle(new System.Numerics.Vector2(position, 0),
            //    (int)result.Patterns[a].Key + (int)offsetNUD.Value, 0,
            //    HitSoundType.Normal, new Extras(), false, 0));
            //}

            //}

            beatmap.HitObjects = result.makePatternsAndAddSliders(beatmap, (int)offsetNUD.Value);

            beatmap.Write(osuFile);
            Console.WriteLine("osu! beatmap saved");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (player.PlayState != BassPlayState.Stopped)
                player.Stop();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    difficulty = BeatmapDiff.Easy;
                    break;
                case 1:
                    difficulty = BeatmapDiff.Normal;
                    break;
                case 2:
                    difficulty = BeatmapDiff.Hard;
                    break;
                case 3:
                    difficulty = BeatmapDiff.Insane;
                    break;
                case 4:
                    difficulty = BeatmapDiff.Extra;
                    break;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    divider = TempoDivider.Four;
                    break;
                case 1:
                    divider = TempoDivider.Three;
                    break;
            }
        }

        private void TestGenerator_Load(object sender, EventArgs e)
        {
            Bass.BASS_PluginLoad(AppDomain.CurrentDomain.BaseDirectory + "bass_fx.dll");
            Bass.BASS_Init(1, 44100, BASSInit.BASS_DEVICE_STEREO, IntPtr.Zero);

            player = new Player();

            metronome = EffectsPlayer.LoadSoundBass("normal-hitnormal.wav");
            eff = EffectsPlayer.LoadSoundBass("kick.wav");
            eff2 = EffectsPlayer.LoadSoundBass("snare.wav");
            eff3 = EffectsPlayer.LoadSoundBass("normal-hitnormal.wav");
            eff4 = EffectsPlayer.LoadSoundBass("hihat.wav");

            Timer tm = new Timer();

            tm.Interval = 10;
            tm.Tick += Tm_Tick;
            tm.Start();
        }

        private void Tm_Tick(object sender, EventArgs e)
        {
            if (v1 < 0)
            {
                labelData1.BackColor = Color.White;
            }
            else
            {
                v1 -= 5;
            }

            if (v2 < 0)
            {
                labelData2.BackColor = Color.White;
            }
            else
            {
                v2 -= 5;
            }

            if (v3 < 0)
            {
                labelData3.BackColor = Color.White;

            }
            else
            {
                v3 -= 5;
            }

            if (v4 < 0)
            {
                labelData4.BackColor = Color.White;
            }
            else
            {
                v4 -= 5;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {

            List<KeyValuePair<float, int>> pattrns = new List<KeyValuePair<float, int>>();

            foreach (KeyValuePair<float, int> vals in result.Patterns)
                pattrns.Add(new KeyValuePair<float, int>(vals.Key, vals.Value));

            player.Play(result.SongPath);
            player.Volume = .2f;


            int patIndex = 0;

            await Task.Run(() =>
            {
                while (player.PlayState == BassPlayState.Playing)
                {
                    System.Threading.Thread.Sleep(1);
                    long position = player.Position;

                    if (pattrns.Count < 1) continue;

                    if (patIndex >= pattrns.Count) continue;


                    if (position >= (int)pattrns[patIndex].Key + (int)offsetNUD.Value)
                    {
                        switch (pattrns[patIndex].Value)
                        {
                            case 0:
                                labelData1.BackColor = Color.Red;
                                v1 = 50;
                                EffectsPlayer.PlayEffect(eff3, .07f);
                                break;
                            case 1:
                                labelData2.BackColor = Color.Blue;
                                v2 = 50;
                                EffectsPlayer.PlayEffect(eff3, .07f);
                                break;
                            case 2:
                                labelData3.BackColor = Color.Green;
                                v3 = 50;
                                EffectsPlayer.PlayEffect(eff3, .07f);
                                break;
                            case 3:
                                labelData4.BackColor = Color.Pink;
                                v4 = 50;
                                EffectsPlayer.PlayEffect(eff3, .07f);
                                break;

                        }

                        patIndex++;
                    }

                }
            });

        }
    }
}
