using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Troschuetz.Random.Generators;
using ubeat.Beatmap;
using ubeat.Extensions;

namespace ubeat.GameScreen
{
    public partial class MainScreen : IScreen
    {
        public MainScreen(bool LoadRandom=true)
        {
            UbeatGame.Instance.IsMouseVisible=true;

            ScreenInstance = this;

            noLoadRnd = !LoadRandom;

            LoadInterface(); //Load :D

            StrBtn.Click += StrBtn_Click;
            ExtBtn.Click += ExtBtn_Click;
            CnfBtn.Click += CnfBtn_Click;
        }

        void MainScreen_OnLoad(object sender, EventArgs e)
        {
            if (!noLoadRnd && !UbeatGame.Instance.FistLoad)
                playRandomSong();
            else if (UbeatGame.Instance.FistLoad)
            {
                PlayUbeatMain();
                UbeatGame.Instance.FistLoad = false;
            }
            else
                ChangeBeatmapDisplay(UbeatGame.Instance.SelectedBeatmap);

            UbeatGame.Instance.Player.OnStopped += player_OnStopped;

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
            UbeatGame.Instance.Exit();
        }

        public void PlayUbeatMain()
        {
            PlayingInit = true;
            string[] songs = { "Shiawase no Sakura Namiki.mp3", "Sakura no THEME II.mp3" };
            string[] bgs = { "bg2.png", "bg.png" };
            float[] mspb = {483.90999274135f,428f};
            int song = /*getRndNotRepeated(0, songs.Length - 1)*/1;

            ubeatBeatMap mainBm = new ubeatBeatMap()
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

            UbeatGame.Instance.Player.Play(mainBm.SongPath);
            UbeatGame.Instance.Player.soundOut.Volume = UbeatGame.Instance.GeneralVolume;
            UbeatGame.Instance.SelectedBeatmap = mainBm;

            Label1.Text = string.Join(" - ", UbeatGame.Instance.SelectedBeatmap.Artist, UbeatGame.Instance.SelectedBeatmap.Title);

            ScreenInstance.LoadCurrentGameInstanceBackground();
        }

        void player_OnStopped()
        {
            if (!Visible) return;

            if (PlayingInit)
                UbeatGame.Instance.Player.Play(UbeatGame.Instance.SelectedBeatmap.SongPath);
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

            List<Beatmap.Mapset> bms = UbeatGame.Instance.AllBeatmaps;

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
            UbeatGame.Instance.Player.Play(songpath);
            UbeatGame.Instance.Player.soundOut.Volume = UbeatGame.Instance.GeneralVolume;
            UbeatGame.Instance.SelectedBeatmap = ubm;

            Label1.Text = ubm.Artist + " - "+ ubm.Title;

            ScreenInstance.LoadCurrentGameInstanceBackground();
        }

        void ChangeBeatmapDisplay(ubeatBeatMap bm)
        {
            if (UbeatGame.Instance.SelectedBeatmap.SongPath != bm.SongPath)
            {
                UbeatGame.Instance.Player.Play(bm.SongPath);
                UbeatGame.Instance.Player.soundOut.Volume = UbeatGame.Instance.GeneralVolume;
            }

            UbeatGame.Instance.SelectedBeatmap = bm;
            Label1.Text = bm.Artist + " - " + bm.Title;

            ScreenInstance.LoadCurrentGameInstanceBackground();
        }

        #region Properties

        public IScreen ScreenInstance { get; set; }
        bool noLoadRnd = false;
        bool PlayingInit { get; set; }

        #endregion

    }
}


