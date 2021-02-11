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

        public delegate void StageChangedHandler(object obj, BMGeneratorStatusArgs args);
        public event StageChangedHandler onStageChanged;

        public BeatmapDiff difficulty = BeatmapDiff.Easy;

        TempoDivider divider = TempoDivider.Four;

        float v1, v2, v3, v4;

        BMGeneratorStatusArgs statusArgs = new BMGeneratorStatusArgs();

        public TestGenerator()
        {
            InitializeComponent();
        }

        public static async Task<string> GenNew(string filename, string folderSave, BeatmapDiff diff, string[] songData, StageChangedHandler stageChanged)
        {
            var obj = new TestGenerator(filename);
            obj.onStageChanged += stageChanged;

            obj.statusArgs.porcentage = .1f;
            obj.statusArgs.stageName = "Openning file";
            obj.onStageChanged?.Invoke(obj, obj.statusArgs);

            var gen = new PatternGenerator();
            gen.frmInstance = obj;

            obj.difficulty = diff;
            obj.divider = TempoDivider.Four;

            obj.statusArgs.porcentage = .15f;
            obj.statusArgs.stageName = "Generating patterns";
            obj.result = await gen.Generate(filename, obj.difficulty, obj.divider);
            obj.result.SongPath = filename;

            if (obj.result.Patterns.Count < 1)
            {
                return null;
            }

            obj.statusArgs.porcentage = .80f;
            obj.statusArgs.stageName = "Saving Beatmap";

            string osufile = obj.saveBeatmap(folderSave, songData);

            obj.statusArgs.porcentage = 1f;
            obj.statusArgs.stageName = "Done!";
            obj.onStageChanged?.Invoke(obj, obj.statusArgs);
            return osufile;
        }

        public TestGenerator(string filename = "")
        {

        }

        public void inform(float prc, string stage)
        {
            statusArgs.porcentage = prc;
            statusArgs.stageName = stage;
            onStageChanged?.Invoke(this, statusArgs);
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

            string osuFile = Path.Combine(fileInfo.DirectoryName, @"Generated-" + fileInfo.Name + " [" + difficulty.ToString() + "].osu");

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
            beatmap.GeneralSection.Countdown = false;
            beatmap.GeneralSection.SampleSet = OsuParsers.Enums.Beatmaps.SampleSet.Normal;
            beatmap.GeneralSection.Mode = OsuParsers.Enums.Ruleset.Mania;
            beatmap.GeneralSection.PreviewTime = 1000;
            beatmap.DifficultySection.ApproachRate = 5;
            beatmap.DifficultySection.CircleSize = 4;
            beatmap.DifficultySection.HPDrainRate = 3;

            float oDiff = 3;
            float beatLength = -54.054f;

            switch (difficulty)
            {
                case BeatmapDiff.Normal:
                    oDiff = 5;
                    beatLength = -80f;
                    break;
                case BeatmapDiff.Hard:
                    oDiff = 7;
                    beatLength = -66.666666666f;
                    break;
                case BeatmapDiff.Insane:
                case BeatmapDiff.Extra:
                    oDiff = 8;
                    beatLength = -54.054054054f;
                    break;
            }
            beatmap.DifficultySection.OverallDifficulty = oDiff;


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
                BeatLength = beatLength,
                Inherited = false,
                Effects = Effects.None,
                TimeSignature = TimeSignature.SimpleQuadruple,
                SampleSet = SampleSet.Normal,
                Volume = 60,
            });


            beatmap.HitObjects = result.makePatternsAndAddSliders(beatmap, (int)offsetNUD.Value);

            beatmap.Write(osuFile);
            Console.WriteLine("osu! beatmap saved");
        }

        private string saveBeatmap(string folder, string[] songData)
        {

            float coffset = 0;
            FileInfo fileInfo = new FileInfo(result.SongPath);

            statusArgs.porcentage = .85f;
            onStageChanged?.Invoke(this, statusArgs);

            
            
            string title = songData[1];
            string artist = songData[0];
            

            folder = Path.Combine(folder, ((title == "") ? "No title" : title) + "-" + ((artist == "") ? "Unknown" : artist));

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string osuFile = Path.Combine(folder, @"Generated-" + ((title == "") ? "No title" : title) + "-" + ((artist == "") ? "Unknown" : artist) + " [" + difficulty.ToString() + "].osu");

            if (!File.Exists(osuFile))
            {
                File.Create(osuFile).Close();
            }
            else
            {
                File.Delete(osuFile);
                File.Create(osuFile).Close();
            }
            try
            {
                File.Copy(result.SongPath, Path.Combine(folder, "audio.mp3"), true);
            }
            catch
            {

            }

            Beatmap beatmap = BeatmapDecoder.Decode(osuFile);

            beatmap.MetadataSection.Artist = (artist == "") ? "Unknown" : artist;
            beatmap.MetadataSection.Creator = "kyun";
            beatmap.MetadataSection.Title = (title == "") ? "No title" : title;
            beatmap.MetadataSection.Version = difficulty.ToString();
            beatmap.MetadataSection.BeatmapID = -1;
            beatmap.GeneralSection.AudioFilename = "audio.mp3";
            beatmap.GeneralSection.Countdown = false;
            beatmap.GeneralSection.SampleSet = OsuParsers.Enums.Beatmaps.SampleSet.Normal;
            beatmap.GeneralSection.Mode = OsuParsers.Enums.Ruleset.Taiko;
            beatmap.GeneralSection.PreviewTime = 1000;
            beatmap.DifficultySection.CircleSize = 4;
            beatmap.DifficultySection.HPDrainRate = 3;

            float oDiff = 3;
            float beatLength = -100f;
            float approachrate = 5;

            switch (difficulty)
            {
                case BeatmapDiff.Normal:
                    oDiff = 5;
                    beatLength = -80f;
                    approachrate = 7f;
                    break;
                case BeatmapDiff.Hard:
                    oDiff = 7;
                    beatLength = -66.666666666f;
                    approachrate = 8.5f;
                    break;
                case BeatmapDiff.Insane:
                case BeatmapDiff.Extra:
                    oDiff = 8;
                    approachrate = 9.5f;
                    beatLength = -54.054054054f;
                    break;
            }
            beatmap.DifficultySection.OverallDifficulty = oDiff;
            beatmap.DifficultySection.ApproachRate = approachrate;


            beatmap.DifficultySection.SliderTickRate = 1;
            beatmap.DifficultySection.SliderMultiplier = 1.4;
            beatmap.EditorSection.DistanceSpacing = 1.2;
            beatmap.EditorSection.BeatDivisor = 4;
            beatmap.EditorSection.GridSize = 4;
            beatmap.EditorSection.TimelineZoom = 1;


            beatmap.Version = 14;
            beatmap.TimingPoints.Add(new OsuParsers.Beatmaps.Objects.TimingPoint
            {
                Offset = result.Offset,
                BeatLength = (60000f / (float)((int)result.BPM)),
                Inherited = true,
                Effects = Effects.None,
                TimeSignature = TimeSignature.SimpleQuadruple,
                SampleSet = SampleSet.Normal,
                Volume = 50,
            });

            beatmap.TimingPoints.Add(new OsuParsers.Beatmaps.Objects.TimingPoint
            {
                Offset = result.Offset,
                BeatLength = beatLength,
                Inherited = false,
                Effects = Effects.None,
                TimeSignature = TimeSignature.SimpleQuadruple,
                SampleSet = SampleSet.Normal,
                Volume = 50,
            });

            statusArgs.porcentage = .90f;
            statusArgs.stageName = "Merging osu objects to beatmap";
            onStageChanged?.Invoke(this, statusArgs);

            beatmap.HitObjects = result.makePatternsAndAddSliders(beatmap, (int)coffset);

            statusArgs.porcentage = .99f;
            statusArgs.stageName = "Saving...";
            onStageChanged?.Invoke(this, statusArgs);

            beatmap.Write(osuFile);

            Console.WriteLine("osu! beatmap saved");
            return osuFile;

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

        private void AsyncLoad()
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

    public class BMGeneratorStatusArgs : EventArgs
    {
        public float porcentage = 0;
        public string stageName = "Starting";
    }
}
