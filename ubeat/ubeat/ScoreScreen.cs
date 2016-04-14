using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ubeat.Beatmap;
using ubeat.GameScreen;

namespace ubeat
{
    public partial class ScoreScreen : Form
    {
        public ScoreScreen()
        {
            InitializeComponent();
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
                total += (ulong)ho.GetScoreValue();
                acc += ho.GetAccuracyPercentage();
            }

            label7.Text = perfect.ToString();
            label8.Text = excellent.ToString();
            label9.Text = good.ToString();
            label12.Text = miss.ToString();
            label13.Text = Math.Round((float)acc / (float)Grid.Instance.bemap.HitObjects.Count,2).ToString() + "%" ;
            label10.Text = Grid.Instance.ScoreDispl.TotalScore.ToString();
        }

        private void ScoreScreen_FormClosing(object sender, FormClosingEventArgs e)
        {

            MainWindow.Instance.playRandomSong();
            MainWindow.Instance.ShowControls();
        }

        private void ScoreScreen_KeyPress(object sender, KeyPressEventArgs e)
        {
            //this.Close();
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

        
    }
}
