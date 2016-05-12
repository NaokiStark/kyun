using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ubeat.Beatmap;
using ubeat.GameScreen;
using ubeat.Score;

namespace ubeat
{
    public partial class ScoreScreen : Form
    {
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        public static int GWL_STYLE = -16;
        public static int WS_CHILD = 0x40000000; 
        
        public ScoreScreen()
        {
            InitializeComponent();
            for(int a =0;a< this.Controls.Count;a++)
                this.Controls[a].Click +=ScoreScreen_Click;
        }

        private void ScoreScreen_Load(object sender, EventArgs e)
        {
            this.label6.Text = string.Format("{0} - {1} [{2}]", Grid.Instance.bemap.Artist, Grid.Instance.bemap.Title, Grid.Instance.bemap.Version);
            int perfect=0;
            int excellent = 0;
            int good = 0;
            int bad = 0;
            int miss = 0;
            ulong total = 0;
            float acc = 0;
            for (int a = 0; a < Grid.Instance.bemap.HitObjects.Count; a++)
            {
                IHitObj ho = Grid.Instance.bemap.HitObjects[a];
                switch(ho.GetScore()){
                    case Score.ScoreType.Perfect:
                        perfect++;
                        break;
                    case Score.ScoreType.Excellent:
                        excellent++;
                        break;
                    case Score.ScoreType.Good:
                        good++;
                        break;
                    case Score.ScoreType.Miss:
                        miss++;
                        break;
                }
                //total += (ulong)((long)ho.GetScoreValue() * Combo.Instance.ActualMultiplier);
                acc += ho.GetAccuracyPercentage();
            }

            label7.Text = perfect.ToString();
            label8.Text = excellent.ToString();
            label9.Text = good.ToString();
            lCombo.Text = Combo.Instance.MaxMultiplier.ToString();
            label12.Text = miss.ToString();
            label13.Text = Math.Round((float)acc / (float)Grid.Instance.bemap.HitObjects.Count,2).ToString() + "%" ;
            label10.Text = Grid.Instance.ScoreDispl.TotalScore.ToString();

            try
            {
                //bg
                FileStream filestream = new FileStream(Grid.Instance.bemap.Background, FileMode.Open, FileAccess.Read);
                var img = Bitmap.FromStream(filestream);

                pictureBox1.Image = img;

                filestream.Close();
                filestream.Dispose();
            }
            catch
            {
                Logger.Instance.Warn("Nopenope");
            }

            /*
            if (Game1.Instance.wSize.X < 801)
            {
                this.Left = 0;
            }
            else
            {*/
                this.Left = (int)(Game1.Instance.wSize.X / 2) - (this.Width / 2);
            /*}
            if (Game1.Instance.wSize.Y < 601)
            {
                this.Top = 0;
            }
            else
            {*/
                this.Top = (int)(Game1.Instance.wSize.Y / 2) - (this.Height / 2);
            //}

            //Paste Window
            IntPtr hostHandle = MainWindow.Instance.Handle;
            IntPtr guestHandle = this.Handle;

            SetWindowLong(guestHandle, GWL_STYLE, GetWindowLong(guestHandle, GWL_STYLE) | WS_CHILD);
            SetParent(guestHandle, hostHandle);
            this.Show();
        }

        private void ScoreScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (BeatmapSelector.Instance == null)
            {
                new BeatmapSelector().Show();
            }
            else
            {
                BeatmapSelector.Instance.Show();
                BeatmapSelector.Instance.Enabled = true;
            }
            Game1.Instance.player.Play(Grid.Instance.bemap.SongPath);
            Game1.Instance.player.Volume = Game1.Instance.GeneralVolume;
            //BeatmapSelector.Instance.ShowControls();
        }

        private void ScoreScreen_KeyPress(object sender, KeyPressEventArgs e)
        {
            
           
        }

        private void ScoreScreen_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void ScoreScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode== Keys.Escape)
            {
                e.Handled = true;
                this.Close();
            }
        }

        
    }
}
