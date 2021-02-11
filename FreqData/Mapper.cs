using OsuParsers.Beatmaps;
using OsuParsers.Decoders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Un4seen.Bass;

namespace FreqData
{
    public partial class Mapper : Form
    {
        public static int eff, metronome = 0;
        ManualMapper m;
        public static Mapper Instance = null;

        

        public static bool[] keys = new bool[] { false, false, false, false, false };

        public Mapper()
        {
            Instance = this;
            Bass.BASS_PluginLoad(AppDomain.CurrentDomain.BaseDirectory + "bass_fx.dll");
            Bass.BASS_Init(1, 44100, BASSInit.BASS_DEVICE_STEREO, IntPtr.Zero);
            eff = EffectsPlayer.LoadSoundBass("soft-hitnormal.wav");
            metronome = EffectsPlayer.LoadSoundBass("metronomelow.wav");

            m = new ManualMapper();
            FormClosing += Mapper_FormClosing;
            InitializeComponent();
        }

        private void Mapper_FormClosing(object sender, FormClosingEventArgs e)
        {
            m.Unsubscribe();
        }

        private void Mapper_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            comboBox1.SelectedIndex = 4;
        }

        private void opnBtn_Click(object sender, EventArgs e)
        {
            var opd = new OpenFileDialog();


            opd.Filter = "AudioFile|*.mp3|All files|*.*";
            // opd.FilterIndex = 2;
            opd.ShowDialog();

            if (opd.FileName == "")
                return;

            switchTempo();

            m.loadFile(opd.FileName);


        }

        private void switchTempo()
        {

            switch (comboBox1.SelectedIndex)
            {
                case 0: // 1/1
                    m.TempoDivider = NoteDuration.Whole;
                    break;
                case 1: // 1/2
                    m.TempoDivider = NoteDuration.Half;
                    break;
                case 2: // 1/4
                    m.TempoDivider = NoteDuration.Quarter;
                    break;
                case 3: // 1/8
                    m.TempoDivider = NoteDuration.Eighth;
                    break;
                case 4: // 1/16
                    m.TempoDivider = NoteDuration.Sixteenth;
                    break;
                case 5: // 1/3
                    m.TempoDivider = NoteDuration.Third;
                    break;
                case 6: // 1/6
                    m.TempoDivider = NoteDuration.Sixth;
                    break;
                case 7: // 1/12
                    m.TempoDivider = NoteDuration.Twelfth;
                    break;
                case 8: // 1/24 - HYPE
                    m.TempoDivider = NoteDuration.TwentyFourth;
                    break;
                case 9: // 1/32 - EXTRA-HYPE
                    m.TempoDivider = NoteDuration.Tempo32;
                    break;
            }

            m.loadTimeLine(m.mspb, m.gap, m.p.Length);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switchTempo();
            m.Start(((float)trackBar1.Value + 5f) / 10f);
            m.p.Volume = (float)tMusic.Value / 100f;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            panel1.Visible = keys[0];
            panel2.Visible = keys[1];
            panel3.Visible = keys[2];
            panel4.Visible = keys[3];
            panel5.Visible = keys[4];

            statusLbl.Text = $"Status: {m.p.PlayState}";
            if (m.p.PlayState != BassPlayState.Playing)
            {

                return;
            }

            TimeSpan lgth = TimeSpan.FromMilliseconds(m.p.Length);
            TimeSpan position = TimeSpan.FromMilliseconds(m.p.Position);

            tLengthlbl.Text = $"{lgth.ToString(@"hh\:mm\:ss\.fff")}";
            actPoslbl.Text = $"{position.ToString(@"hh\:mm\:ss\.fff")}";


        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void tEff_Scroll(object sender, EventArgs e)
        {

        }

        private void tMusic_Scroll(object sender, EventArgs e)
        {
            m.p.Volume = (float)tMusic.Value / 100f;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if(m.p.PlayState == BassPlayState.Playing)
            {
                m.p.SetVelocity(((float)trackBar1.Value + 5f) / 10f);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var opd = new OpenFileDialog();


            opd.Filter = "osu!Beatmap|*.osu|All files|*.*";
            // opd.FilterIndex = 2;
            opd.ShowDialog();

            if (opd.FileName == "")
                return;

            Beatmap beatmap = BeatmapDecoder.Decode(opd.FileName);
            switchTempo();

            m.loadFile(beatmap, opd.FileName);
            SystemSounds.Asterisk.Play();
            Thread.Sleep(500);
            SystemSounds.Hand.Play();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            m.Save();
        }
    }
}
