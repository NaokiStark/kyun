using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Troschuetz.Random.Generators;
using ubeat.Beatmap;

namespace ubeat.GameScreen
{
    public partial class MainScreen : IScreen
    {
        public MainScreen(bool LoadRandom=true)
        {
            Game1.Instance.IsMouseVisible=true;

            ScreenInstance = this;

            noLoadRnd = !LoadRandom;

            LoadInterface(); //Load :D

            StrBtn.Click += StrBtn_Click;
            ExtBtn.Click += ExtBtn_Click;
            CnfBtn.Click += CnfBtn_Click;
        }

        void MainScreen_OnLoad(object sender, EventArgs e)
        {
            if (!noLoadRnd && !Game1.Instance.FistLoad)
                playRandomSong();
            else if (Game1.Instance.FistLoad)
            {
                PlayUbeatMain();
                Game1.Instance.FistLoad = false;
            }
            else
                ChangeBeatmapDisplay(Game1.Instance.SelectedBeatmap);

            Game1.Instance.player.OnStopped += player_OnStopped;

        }

        void CnfBtn_Click(object sender, EventArgs e)
        {
            /*new ElCosoQueSirveParaLasOpcionesDelJuegoYOtrasWeas.Settings().Show();*/
            ScreenManager.ChangeTo(new SettingsScreen());
        }

        void StrBtn_Click(object sender, EventArgs e)
        {
            ScreenManager.ChangeTo(new BeatmapScreen());
        }

        void ExtBtn_Click(object sender, EventArgs e)
        {
            Game1.Instance.Exit();
        }

        public void PlayUbeatMain()
        {
            PlayingInit = true;
            string[] songs = { "Shiawase no Sakura Namiki.mp3", "Sakura no THEME II.mp3" };
            string[] bgs = { "bg2.png", "bg.png" };
            float[] mspb = {483.90999274135f,428f};
            int song = getRndNotRepeated(0, songs.Length - 1);
            Beatmap.ubeatBeatMap mainBm = new Beatmap.ubeatBeatMap()
            {
                Artist = "",
                BPM = mspb[song],
                SongPath = AppDomain.CurrentDomain.BaseDirectory + @"\Assets\"+songs[song],
                ApproachRate=10,
                Background = AppDomain.CurrentDomain.BaseDirectory + @"\Assets\" + bgs[song],
                Creator ="Fabi",
                OverallDifficulty=10,
                Version="",
                SleepTime=0,
                Title = "",
                
            };

            Game1.Instance.player.Play(mainBm.SongPath);
            Game1.Instance.player.soundOut.Volume = Game1.Instance.GeneralVolume;
            Game1.Instance.SelectedBeatmap = mainBm;
            Label1.Text = mainBm.Artist + " - " + mainBm.Title;
            try
            {
                FileStream bgFstr = new FileStream(mainBm.Background, FileMode.Open);
                Background = Texture2D.FromStream(Game1.Instance.GraphicsDevice, bgFstr);
                bgFstr.Close();
            }
            catch
            {
                Logger.Instance.Warn("BACKGROUND NOT FOUND!!");
            }
        }

        void player_OnStopped()
        {
            if (!Visible) return;

            if (PlayingInit)
                Game1.Instance.player.Play(Game1.Instance.SelectedBeatmap.SongPath);
            else
                playRandomSong();
        }

        int getRndNotRepeated(int min, int max)
        {
            NR3Q2Generator rnd = new NR3Q2Generator(DateTime.Now.Millisecond);

            int nummm = rnd.Next(min,max);
            int cnt = 0;
            while (nummm == Settings1.Default.LastScr)
            {
                nummm = rnd.Next(max);
                if(nummm == Settings1.Default.LastScr)
                    if (cnt != Settings1.Default.LastScr && cnt <= max)
                        nummm = cnt;
                cnt++;
            }
            
            Settings1.Default.LastScr = nummm;
            Settings1.Default.Save();
            return nummm;
        }

        public void playRandomSong()
        {
            Random c = new Random(DateTime.Now.Millisecond);

            List<Beatmap.Mapset> bms = Game1.Instance.AllBeatmaps;

            if (bms.Count < 1) return;
            Beatmap.Mapset bsel;
            if (bms.Count == 1)
                bsel = bms[0];
            else
                bsel = bms[OsuUtils.OsuBeatMap.rnd.Next(0, bms.Count - 1)];


            Beatmap.ubeatBeatMap ubm;

            if (bsel.Count == 1)
                ubm = bsel[1];
            else
                ubm = bsel[OsuUtils.OsuBeatMap.rnd.Next(0, bsel.Count - 1)];

            string songpath = ubm.SongPath;
            Game1.Instance.player.Play(songpath);
            Game1.Instance.player.soundOut.Volume = Game1.Instance.GeneralVolume;
            Game1.Instance.SelectedBeatmap = ubm;
            Label1.Text = ubm.Artist +" - "+ ubm.Title;
            try
            {
                FileStream bgFstr = new FileStream(ubm.Background, FileMode.Open);
                Background = Texture2D.FromStream(Game1.Instance.GraphicsDevice, bgFstr);
                bgFstr.Close();
            }
            catch
            {
                Logger.Instance.Warn("BACKGROUND NOT FOUND!!");
            }
        }

        void ChangeBeatmapDisplay(ubeatBeatMap bm)
        {
            if (Game1.Instance.SelectedBeatmap.SongPath != bm.SongPath)
            {
                Game1.Instance.player.Play(bm.SongPath);
                Game1.Instance.player.soundOut.Volume = Game1.Instance.GeneralVolume;
            }

            Game1.Instance.SelectedBeatmap = bm;
            Label1.Text = bm.Artist + " - " + bm.Title;

            try
            {
                FileStream bgFstr = new FileStream(bm.Background, FileMode.Open);
                Background = Texture2D.FromStream(Game1.Instance.GraphicsDevice, bgFstr);
                bgFstr.Close();
            }
            catch
            {
                Logger.Instance.Warn("BACKGROUND NOT FOUND!!");
            }
        }

        #region Properties

        public IScreen ScreenInstance { get; set; }
        bool noLoadRnd = false;
        bool PlayingInit { get; set; }

        #endregion

    }
}


