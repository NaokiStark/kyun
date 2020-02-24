using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Un4seen.Bass;

namespace FreqData
{
    public partial class FreqForm : Form
    {

        Player p = new Player();
        System.Timers.Timer peaktm = new System.Timers.Timer();

        float max1, max2, max3, max4;
        int eff, eff2, eff3, eff4;
        private int eff5;
        double lastBPMTaken = 0;
        float averageBPM = 0;

        List<float> takes = new List<float>();

        List<long> takes2 = new List<long>();

        List<double> difftakes = new List<double>();

        Stopwatch stw = new Stopwatch();

        TimeSpan elapx;
        public FreqForm()
        {
            InitializeComponent();

        }

        private void FreqForm_Load(object sender, EventArgs e)
        {
            freqs.Add(5, 199);
            freqs.Add(60, 300);
            freqs.Add(2001, 4000);
            freqs.Add(4001, 20000);

            Bass.BASS_PluginLoad(AppDomain.CurrentDomain.BaseDirectory + "bass_fx.dll");
            Bass.BASS_Init(1, 44100, BASSInit.BASS_DEVICE_STEREO, IntPtr.Zero);

            CheckForIllegalCrossThreadCalls = false;
            stw.Reset();
            peaktm.Interval = 1;
            peaktm.Elapsed += Peaktm_Elapsed;
            peaktm.Start();
            stw.Start();
            elapx = stw.Elapsed;
            eff = EffectsPlayer.LoadSoundBass("soft-hitnormal.wav");
            eff2 = EffectsPlayer.LoadSoundBass("taiko-soft-hitnormal.wav");
            eff3 = EffectsPlayer.LoadSoundBass("normal-hitfinish11.wav");
            eff4 = EffectsPlayer.LoadSoundBass("normal-hitfinish.wav");
            eff5 = EffectsPlayer.LoadSoundBass("metronomelow.wav");


        }

        private void Peaktm_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Peaktm_Tick(sender, e);
        }

        bool sust1, sust2, sust3, sust4;

        private void BPMlbl_Click(object sender, EventArgs e)
        {

        }

        Dictionary<int, int> freqs = new Dictionary<int, int>();
        int el1, el2, el3, el4;

        private void Peaktm_Tick(object sender, EventArgs e)
        {
            TimeSpan s = stw.Elapsed - elapx;
            if (p.PlayState == BassPlayState.Stopped)
            {
                max1 = max2 = max3 = max4 = 0;
                return;
            }

            double BPM = p.GetBPM();

            if (lastBPMTaken != BPM)
            {
                //if(p.Position < 61 * 1000)
                //{
                takes.Add((float)BPM);

                foreach (float take in takes)
                {
                    averageBPM += take;
                }
                averageBPM = (averageBPM / takes.Count);
                lastBPMTaken = BPM;
                //}
                //else
                //{
                BPMlbl.ForeColor = Color.Red;
                //}

            }

            BPMlbl.Text = $"BPM: {BPM.ToString("0.00", CultureInfo.InvariantCulture)} | Average: {averageBPM.ToString("0.00", CultureInfo.InvariantCulture)}";

            if (p.PeakVol >= 0f)
            {
                float[] values = p.DetectFrequency(freqs, 2048);

                //var val1 = p.DetectFrequency(61, 250);
                //var val2 = p.DetectFrequency(251, 2000);
                //var val3 = p.DetectFrequency(2001, 4000);
                //var val4 = p.DetectFrequency(4001, 20000);

                var val1 = values[0];
                var val2 = (float)Math.Sqrt(values[1]);
                var val3 = values[2];
                var val4 = values[3];

                labelData1.Text = $"{val1.ToString("0.0000", CultureInfo.InvariantCulture)}";
                labelData2.Text = $"{val2.ToString("0.0000", CultureInfo.InvariantCulture)}";
                labelData3.Text = $"{val3.ToString("0.0000", CultureInfo.InvariantCulture)}";
                labelData4.Text = $"{val4.ToString("0.0000", CultureInfo.InvariantCulture)}";

                if (max1 < val1) max1 = val1;
                if (max2 < val2) max2 = val2;
                if (max3 < val3) max3 = val3;
                if (max4 < val4) max4 = val4;

                lmax1.Text = $"{el1.ToString("000")} | {max1.ToString("0.0000", CultureInfo.InvariantCulture)}";
                lmax2.Text = $"{max2.ToString("0.0000", CultureInfo.InvariantCulture)}";
                lmax3.Text = $"{max3.ToString("0.0000", CultureInfo.InvariantCulture)}";
                lmax4.Text = $"{max4.ToString("0.0000", CultureInfo.InvariantCulture)}";

                int interval = s.Milliseconds;//(int)(1000f / 60f);

                if (val2 >= tPrc(max2) && max2 > 0.04f)
                {

                    if (!sust1 && el1 >= 5 )
                    {
                        takes2.Add(p.Position);

                        if(takes2.Count < 2)
                        {
                            difftakes.Add(takes2.First());
                        }
                        else
                        {
                            long last = takes2.Last();
                            long beforeLast = takes2[takes2.Count - 2];

                            long result = last - beforeLast;

                            if (result <= 375 && result >= 85)
                            {
                                difftakes.Add(result);
                                lastTakeDlbl.Text = $"{result}";
                                textBox1.AppendText(result + Environment.NewLine);

                            }
                        }

                        float takesBPM = 0;

                        int countProm = 0;
                        for (int a = 0; a < takes2.Count; a++)
                        {

                            if (a == 0)
                            {
                                takesBPM = takes2[a];
                                continue;
                            }

                            float takediff = takes2[a] - takes2[a - 1];

                            if (takediff >= 375 || takediff <= 85)
                                continue;

                            countProm++;
                            takesBPM += takediff;
                        }

                        fBPMlbl.Text = $"BPM2: {(30000f / ((float)takesBPM / (float)countProm)).ToString("0.00", CultureInfo.InvariantCulture)}";

                        el1 = 0;
                        sust1 = true;
                        panel1.BackColor = Color.Red;
                        EffectsPlayer.PlayEffect(eff5, .3f);
                    }

                }
                else
                {
                    el1 += interval;
                    sust1 = false;
                    panel1.BackColor = Color.White;
                }

                if (val2 >= tPrc(max2) && max2 > 0.04f)
                {
                    if (!sust2 && el2 > 50)
                    {
                        el2 = 0;
                        sust2 = true;
                        panel2.BackColor = Color.Blue;
                        //EffectsPlayer.PlayEffect(eff4, .3f);
                    }
                }
                else
                {
                    el2 += interval;
                    sust2 = false;

                    panel2.BackColor = Color.White;
                }

                if (val3 >= tPrc(max3) && max3 > 0.02f)
                {
                    if (!sust3 && el3 > 50)
                    {
                        el3 = 0;
                        sust3 = true;
                        panel3.BackColor = Color.Green;
                        //EffectsPlayer.PlayEffect(eff2, .3f);
                    }
                }
                else
                {
                    el3 += interval;
                    sust3 = false;
                    panel3.BackColor = Color.White;
                }

                if (val4 >= tPrc(max4) && max4 > 0.025f)
                {
                    if (!sust4 && el4 > 50)
                    {
                        el4 = 0;
                        sust4 = true;
                        panel4.BackColor = Color.Magenta;
                        //EffectsPlayer.PlayEffect(eff2, .3f);
                    }
                }
                else
                {
                    el4 += interval;
                    sust4 = false;

                    panel4.BackColor = Color.White;
                }

                max1 = Math.Max(0, max1 - (.0000001f * elapx.Milliseconds));
                max2 = Math.Max(0, max2 - .000001f * elapx.Milliseconds * 2f);
                max3 = Math.Max(0, max3 - .00001f * elapx.Milliseconds * 3f);
                max4 = Math.Max(0, max4 - .00001f * elapx.Milliseconds * 4f);
            }

            elapx = stw.Elapsed;
        }

        private float tPrc(float v)
        {
            return v - v * .1f;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var opd = new OpenFileDialog();


            opd.Filter = "AudioFile|*.mp3|All files|*.*";
            // opd.FilterIndex = 2;
            opd.ShowDialog();

            if (opd.FileName == "")
                return;


            textBox1.Text = "";

            averageBPM = max1 = max2 = max3 = max4 = 0;
            lastBPMTaken = 0;
            BPMlbl.ForeColor = Color.Black;

            takes.Clear();
            takes2.Clear();
            difftakes.Clear();

            p.Play(opd.FileName);
            string[] tags = Bass.BASS_ChannelGetTagsID3V2(p.GetHandler());
            string title = "";
            string artist = "";
            if (tags != null)
            {
                foreach (string val in tags)
                {
                    if (val.StartsWith("TIT2="))
                    {
                        title = val.Replace("TIT2=", "");
                    }

                    if (val.StartsWith("TIT1="))
                    {
                        title = val.Replace("TIT1=", "");
                    }

                    if (val.StartsWith("TPE1="))
                    {
                        artist = val.Replace("TPE1=", "");
                    }
                }

            }


            Text = $"{artist} - {title}";

            p.Volume = .5f;
            p.SetVelocity((float)trackBar1.Value / 100f);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            p.Pause();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (p.PlayState == BassPlayState.Stopped)
            {
                return;
            }

            p.SetVelocity((float)trackBar1.Value / 100f);

            velLabel.Text = ((float)trackBar1.Value / 100f).ToString("0.00", CultureInfo.InvariantCulture);
        }
    }
}
