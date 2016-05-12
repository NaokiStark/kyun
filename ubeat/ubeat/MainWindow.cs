﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ubeat.Audio;
using ubeat.GameScreen;

namespace ubeat
{
    public partial class MainWindow : Form
    {
        public static MainWindow Instance = null;
        public BeatmapSelector bmselector;

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        public static int GWL_STYLE = -16;
        public static int WS_CHILD = 0x40000000; 

        public MainWindow()
        {

            Instance = this;
            player = Game1.Instance.player;
            InitializeComponent();
        }
        NPlayer player;
        private void MainWindow_Load(object sender, EventArgs e)
        {
            playRandomSong();

            label2.Visible = false;

            //// DEBUG ////
            /*
            Timer tm = new Timer();
            tm.Tick += (s, se) => {
                label2.Text = string.Format("==Player Debug==\r\nCurrent File: {0}\r\nCurrent Length: {1}\r\nCurrent Position: {2}\r\nCurrent counter Position: {3} \r\nCurrent Difference (Raw - Counter): {4}\r\nPlayer State: {5}",
                    player.ActualSong,
                    "",
                    player.RawPosition,
                    player.Position,
                    player.RawPosition - player.Position,
                    player.PlayState.ToString()
                    );
            };
            tm.Start();*/
            //// EOF DEBUG ///

            this.Hide();
            IntPtr hostHandle = Game1.Instance.Window.Handle;
            IntPtr guestHandle = this.Handle;

            
            this.Top = 0;
            this.Left = 0;
            

            SetWindowLong(guestHandle, GWL_STYLE, GetWindowLong(guestHandle, GWL_STYLE) | WS_CHILD);
            SetParent(guestHandle, hostHandle);
            this.Show();
        }

        public void ShowAsync()
        {
           
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(ShowAsync));
            }
            else
            {                
                this.Show();
                new ScoreScreen().Show();
            }
        }

        void player_onEnd(string ev)
        {
            if (BeatmapSelector.Instance == null)
                playRandomSong();
            else if (Grid.Instance != null)
            {
                if (!Grid.Instance.inGame)
                {
                    return;
                }
            }
            else if (!BeatmapSelector.Instance.Visible)
                playRandomSong();
            else
                player.Play(BeatmapSelector.Instance.lastSelected);
        }
        public void ShowControls()
        {
            panel1.Visible = true;
            label1.Visible = true;
            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
        }
        public void HideControls()
        {
            panel1.Visible = false;
            label1.Visible = false;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
        }
        public void playRandomSong()
        {
            Random c = new Random(DateTime.Now.Millisecond);

            List<Beatmap.Mapset> bms = Game1.Instance.AllBeatmaps;

            if (bms.Count < 1) return;

            Beatmap.Mapset bsel = bms[c.Next(0, bms.Count - 1)];
            Beatmap.ubeatBeatMap ubm = bsel[c.Next(0, bsel.Count - 1)];
            string songpath = ubm.SongPath;
            player.Play(songpath);
            player.soundOut.Volume = Game1.Instance.GeneralVolume;
            //player.onEnd += player_onEnd;

            Image img;
            try
            {
                FileStream filestream = new FileStream(ubm.Background, FileMode.Open, FileAccess.Read);
                img = Bitmap.FromStream(filestream);
                this.BackgroundImage = img;
                filestream.Close();
                filestream.Dispose();
            }
            catch
            {
                Logger.Instance.Warn("BACKGROUND NOT FOUND!!");
            }          
         }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
           this.Close();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        public void ChangeBeatmap(Beatmap.ubeatBeatMap bm)
        {
            string songpath = bm.SongPath;
            player.Play(songpath);
            player.Volume = Game1.Instance.GeneralVolume;
            //player.onEnd += player_onEnd;
            try
            {
                this.BackgroundImage = Image.FromFile(bm.Background);
            }
            catch
            {
                Logger.Instance.Warn("This beatmap has not image");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
           
            this.HideControls();
            new BeatmapSelector().Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Game1.Instance.player.Paused = !Game1.Instance.player.Paused;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new ElCosoQueSirveParaLasOpcionesDelJuegoYOtrasWeas.Settings().Show();
        }
    }
}
