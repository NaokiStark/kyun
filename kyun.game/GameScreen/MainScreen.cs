using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Troschuetz.Random.Generators;
using kyun.Beatmap;
using kyun.Utils;
using System.Threading;
using kyun.GameScreen.UI.Particles;
using Microsoft.Xna.Framework;
using kyun.OsuUtils;
using kyun.Audio;
using kyun.game;

namespace kyun.GameScreen
{
    public partial class MainScreen : ScreenBase
    {
        int lastIndex = 0;
        static IScreen instance = null;
        public static IScreen Instance
        {
            get
            {
                if (instance == null)
                    instance = new MainScreen();
                ((MainScreen)instance).leaving = false;
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public MainScreen(bool LoadRandom=true) 
            : base("MainScreen")
        {

            ecolors = new List<int[]>();
            ecolors.Add(new int[] { 206, 53, 39 });
            ecolors.Add(new int[] { 237, 245, 8 });
            ecolors.Add(new int[] { 34, 92, 173 });
            ecolors.Add(new int[] { 35, 196, 91 });
            ecolors.Add(new int[] { 145, 35, 196 });

            if (KyunGame.xmas)
            {
                ecolors.Clear();
                ecolors.Add(new int[] { 24, 114, 21 });
                ecolors.Add(new int[] { 206, 59, 59 });
            }


            KyunGame.Instance.IsMouseVisible=true;

            ScreenInstance = this;

            noLoadRnd = !LoadRandom;

            LoadInterface(); //Load :D

            StrBtn.Click += StrBtn_Click;
            ExtBtn.Click += ExtBtn_Click;
            CnfBtn.Click += CnfBtn_Click;

            btnNext.Click += BtnNext_Click;
            btnPause.Click += BtnPause_Click;
            btnPlay.Click += BtnPlay_Click;
            btnPrev.Click += BtnPrev_Click;
            btnStop.Click += BtnStop_Click;

            AllowVideo = true;//test




            EnphasisColor = ecolors[OsuBeatMap.rnd.Next(0, ecolors.Count - 1)];


            onKeyPress += MainScreen_onKeyPress;

            KyunGame.Instance.OnPeak += Instance_OnPeak;

            KyunGame.Instance.discordHandler.SetState("Idle", "Waiting for something...");
        }


        private bool switchParticle = false;
        private void Instance_OnPeak(object sender, EventArgs e)
        {
            if (!Visible) return;
            if (KyunGame.Instance.Player.PlayState != BassPlayState.Playing) return;

            if (particleEngine.ParticleCount > 50) return;

            Screen.ScreenMode actualMode = Screen.ScreenModeManager.GetActualMode();

            int randomNumber = OsuUtils.OsuBeatMap.GetRnd(1, 10, -1);

            for(int a = 0; a < randomNumber; a++)
            {
                switchParticle = OsuBeatMap.rnd.NextBoolean();
                int startLeft = 0;
                int startTop = 0;
                if (switchParticle)
                {
                    startTop = OsuUtils.OsuBeatMap.GetRnd(25, actualMode.Height - 25, -1);
                    startLeft = OsuUtils.OsuBeatMap.GetRnd(25, actualMode.Width + 500, -1);

                    Particle particle = particleEngine.AddNewParticle(SpritesContent.Instance.MenuSnow,
                        new Microsoft.Xna.Framework.Vector2((5f * (float)(OsuUtils.OsuBeatMap.rnd.NextDouble() * 2 - 1)) / 10f, Math.Abs(5f * (float)(OsuUtils.OsuBeatMap.rnd.NextDouble() * 2 - 1)) / 10f),
                        new Microsoft.Xna.Framework.Vector2(startLeft, 0),
                        (30 + OsuUtils.OsuBeatMap.rnd.Next(40)) * 100,
                        0.01f * (float)(OsuUtils.OsuBeatMap.rnd.NextDouble() * 2f - 1)
                        );

                    particle.Opacity = 0.6f;
                    particle.Scale = (float)OsuUtils.OsuBeatMap.rnd.NextDouble(0.1, 0.6);
                    if (KyunGame.xmas)
                        particle.TextureColor = Color.Yellow;
                    particle.StopAtBottom = true;
                }
                else
                {
                    //int startTop = OsuUtils.OsuBeatMap.GetRnd(25, actualMode.Height - 25, -1);
                    startLeft = OsuUtils.OsuBeatMap.GetRnd(-50, actualMode.Width + 500, -1);

                    float vel = (float)OsuUtils.OsuBeatMap.rnd.NextDouble(0.2, 1);

                    int black_rand = 20;

                    if (KyunGame.xmas)
                    {
                        black_rand = OsuBeatMap.rnd.Next(250, 255);
                    }
                    else
                    {
                        black_rand = OsuBeatMap.rnd.Next(20, 40);
                    }                  

                    Color ccolor = (squareYesNo) ?
                                    LoadScreen.getColorRange(EnphasisColor[0], EnphasisColor[1], EnphasisColor[2]) :
                                    Color.FromNonPremultiplied(black_rand, black_rand, black_rand, 255);

                    if (KyunGame.xmas && OsuBeatMap.rnd.NextBoolean())
                    {
                        ccolor = LoadScreen.getColorRange(ecolors[1][0], ecolors[1][1], ecolors[1][2]);
                    }

                    Particle particle = particleEngine.AddNewSquareParticle(SpritesContent.Instance.SquareParticle,
                        new Vector2(0, vel),
                        new Vector2(startLeft, actualMode.Height),
                        (30 + OsuUtils.OsuBeatMap.rnd.Next(40)) * 100,
                        0.01f * (float)(OsuUtils.OsuBeatMap.rnd.NextDouble() * 2f - 1),
                        ccolor
                        );
                    particle.Scale = (float)OsuUtils.OsuBeatMap.rnd.NextDouble(0.4, 0.7);
                    particle.Opacity = /*(float)OsuUtils.OsuBeatMap.rnd.NextDouble(0.4, 0.9)*/0.95f;
                    squareYesNo = !squareYesNo;
                }

            }
        }

        private void MainScreen_onKeyPress(object sender, InputEvents.KeyPressEventArgs args)
        {
            switch (args.Key)
            {
                case Microsoft.Xna.Framework.Input.Keys.Left:
                    _prev();
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Right:
                    _next();
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Escape:
                    ExtBtn_Click(new object(), new EventArgs());
                    break;
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            if (KyunGame.Instance.Player.PlayState != BassPlayState.Stopped)
                KyunGame.Instance.Player.Stop();
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            _prev();
        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            if(KyunGame.Instance.Player.PlayState == BassPlayState.Stopped)
            {
                
                AVPlayer.Play(KyunGame.Instance.SelectedMapset[0].SongPath);
            }
            else if(KyunGame.Instance.Player.PlayState == BassPlayState.Paused)
            {
                KyunGame.Instance.Player.Paused = !KyunGame.Instance.Player.Paused;
            }
        }

        private void BtnPause_Click(object sender, EventArgs e)
        {
            if(KyunGame.Instance.Player.PlayState != BassPlayState.Stopped)
                KyunGame.Instance.Player.Paused = !KyunGame.Instance.Player.Paused;
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            _next();
           
        }

        void MainScreen_OnLoad(object sender, EventArgs e)
        {

            BackgroundDim = .7f;
            /*
            if (!noLoadRnd && !UbeatGame.Instance.FistLoad)
                playRandomSong();
            else if (UbeatGame.Instance.FistLoad)
            {
                PlayUbeatMain();
                UbeatGame.Instance.FistLoad = false;
            }
            else
                ChangeBeatmapDisplay(UbeatGame.Instance.SelectedBeatmap);
            */
            if (KyunGame.Instance.Player.PlayState == BassPlayState.Stopped)
            {
                PlayUbeatMain();
            }
            else
            {
                changeCoverDisplay(((LoadScreen)LoadScreen.Instance).selected_song.Cover);

                float titleSize = SpritesContent.Instance.SettingsFont.MeasureString(((LoadScreen)LoadScreen.Instance).selected_song.Title).X;
                float artSize = SpritesContent.Instance.SettingsFont.MeasureString(((LoadScreen)LoadScreen.Instance).selected_song.Artist).X;

                float maxSize = Math.Max(titleSize, artSize);

                coverBox.Resize(new Vector2((maxSize * .8f) + 20, coverSize));
                coverLabel.Text = ((LoadScreen)LoadScreen.Instance).selected_song.Title;
                coverLabelArt.Text = ((LoadScreen)LoadScreen.Instance).selected_song.Artist;
                ChangeBackground((Texture2D)null);
            }
            
            KyunGame.Instance.Player.OnStopped += player_OnStopped;

            /*
            if (!KyunGame.Instance.ppyMode)
            {
                ntfr.ShowDialog("Now!                               More bugs!");
            }
            else
            {
                ntfr.ShowDialog("You are in osu! v0.1 (ubeat codename)!, to show changes, click here.");
            }*/
            ntfr.ShowDialog("Welcome!");
            //ntfr.ShowDialog("Tu version es nueva, asi que esto handa piolaso amewo", 10000, Notifications.NotificationType.Critical);

        } 

        void CnfBtn_Click(object sender, EventArgs e)
        {
            /*new ElCosoQueSirveParaLasOpcionesDelJuegoYOtrasWeas.Settings().Show();*/
            //ScreenManager.ChangeTo(new SettingsScreen());
            ScreenManager.ChangeTo(SettingsScreen.Instance);
        }

        void StrBtn_Click(object sender, EventArgs e)
        {
            leaving = true;
           
            ScreenManager.ChangeTo(BeatmapScreen.Instance);
        }

        void ExtBtn_Click(object sender, EventArgs e)
        {

            leaving = true;
           
          ScreenManager.ChangeTo(new LeaveScreen());
        }

        public void PlayUbeatMain()
        {
            PlayingInit = true;
            

            
            //AVPlayer.Play(mainBm.SongPath, "", true);
           
            ChangeBeatmapDisplay(mainBm);

        }

        void player_OnStopped()
        {
            if (!Visible || leaving || changingSong) return;

            /*
            if (PlayingInit)
                PlayUbeatMain();
            else */

            BassPlayState pbs = KyunGame.Instance.Player.PlayState;
            if(pbs == BassPlayState.Stopped)
            {
                playRandomSong();
            }
                
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

            int mstIndex = 0;
            changingSong = true;
            Random c = new Random(DateTime.Now.Millisecond);

            List<Beatmap.Mapset> bms = InstanceManager.AllBeatmaps;

            if (bms.Count < 1) return;
            Beatmap.Mapset bsel;
            if (bms.Count == 1)
                bsel = bms[mstIndex];
            else
            {
                mstIndex = OsuUtils.OsuBeatMap.GetRnd(0, InstanceManager.AllBeatmaps.Count - 1, -1);
                bsel = bms[mstIndex];
                lastIndex = mstIndex;
            }



            Beatmap.ubeatBeatMap ubm;

            if (bsel.Count == 1)
                ubm = bsel[0];
            else
                ubm = bsel[OsuUtils.OsuBeatMap.rnd.Next(0, bsel.Count - 1)];

            ChangeBeatmapDisplay(ubm);
            KyunGame.Instance.SelectedBeatmap = ubm;

            //ChangeBackground(ubm.Background);
            changingSong = false;

            if (BeatmapScreen.Instance != null)
            {
                ((BeatmapScreen)BeatmapScreen.Instance).lbox.selectedIndex = mstIndex;
                ((BeatmapScreen)BeatmapScreen.Instance).lbox.vertOffset = (mstIndex > 2) ? mstIndex - 1 : 0;
            }

            KyunGame.Instance.discordHandler.SetState($"{ubm.Artist} - {ubm.Title}", "Playing music");
        }

        private void playSong(string path)
        {

            KyunGame.Instance.Player.Play(path);
            KyunGame.Instance.Player.Volume = KyunGame.Instance.GeneralVolume;
        }

        public override void ChangeBeatmapDisplay(ubeatBeatMap bm, bool overrideBg = false)
        {
            changingSong = true;
            base.ChangeBeatmapDisplay(bm, false);

            Background = SpritesContent.Instance.DefaultBackground;
            changeCoverDisplay(bm.Background);

            float titleSize = SpritesContent.Instance.SettingsFont.MeasureString(bm.Title).X;
            float artSize = SpritesContent.Instance.SettingsFont.MeasureString(bm.Artist).X;

            float maxSize = Math.Max(titleSize, artSize);

            coverBox.Resize(new Vector2((maxSize * .8f) + 20, coverSize));
            coverLabel.Text = bm.Title;
            coverLabelArt.Text = bm.Artist;
            changingSong = false ;
        }

        public void ChangeMainDisplay(int mps)
        {
            //ChangeBackground(InstanceManager.AllBeatmaps[mps][0].Background);
            Label1.Text = InstanceManager.AllBeatmaps[mps][0].Artist + " - " + InstanceManager.AllBeatmaps[mps][0].Title;
            lastIndex = mps;
        }

        private void _next()
        {
            int mstIndex = lastIndex;


            if (mstIndex >= InstanceManager.AllBeatmaps.Count - 1)
            {
                mstIndex = OsuUtils.OsuBeatMap.GetRnd(0, InstanceManager.AllBeatmaps.Count - 1, -1);
            }
            else
            {
                mstIndex++;
            }

            KyunGame.Instance.SelectedMapset = InstanceManager.AllBeatmaps[mstIndex];
            
            ChangeBeatmapDisplay(KyunGame.Instance.SelectedMapset[0]);
            var ubm = KyunGame.Instance.SelectedMapset[0];
            //ChangeBackground(UbeatGame.Instance.SelectedMapset[0].Background);
            lastIndex = mstIndex;

            if (BeatmapScreen.Instance != null)
            {
                ((BeatmapScreen)BeatmapScreen.Instance).lbox.selectedIndex = mstIndex;
                ((BeatmapScreen)BeatmapScreen.Instance).lbox.vertOffset = (mstIndex > 2) ? mstIndex - 1 : 0;
            }
            KyunGame.Instance.discordHandler.SetState($"{ubm.Artist} - {ubm.Title}", "Playing music");
        }

        private void _prev()
        {
            int mstIndex = lastIndex;


            if (mstIndex < 1)
            {
                mstIndex = OsuUtils.OsuBeatMap.GetRnd(0, InstanceManager.AllBeatmaps.Count - 1, -1);
            }
            else
            {
                mstIndex--;
            }


            KyunGame.Instance.SelectedMapset = InstanceManager.AllBeatmaps[mstIndex];
           
            ChangeBeatmapDisplay(KyunGame.Instance.SelectedMapset[0]);
            var ubm = KyunGame.Instance.SelectedMapset[0];
            //ChangeBackground(UbeatGame.Instance.SelectedMapset[0].Background);
            lastIndex = mstIndex;
            if (BeatmapScreen.Instance != null)
            {
                ((BeatmapScreen)BeatmapScreen.Instance).lbox.selectedIndex = mstIndex;
                ((BeatmapScreen)BeatmapScreen.Instance).lbox.vertOffset = (mstIndex > 2)? mstIndex - 1 : 0;
            }
            KyunGame.Instance.discordHandler.SetState($"{ubm.Artist} - {ubm.Title}", "Playing music");
        }

        private void HideControls()
        {
            hidding = true;
            StateHidden = true;
        }

        #region Properties

        bool noLoadRnd = false;
        private bool leaving;
        private bool changingSong;

        bool PlayingInit { get; set; }
        public int[] EnphasisColor { get; private set; }

        bool StateHidden;
        int maxElapsedToHide = 5000;
        int actualElapsed;
        bool hidding;
        private bool squareYesNo;
        private List<int[]> ecolors;

        #endregion

    }
}


