using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using kyun.GameScreen.UI;
using kyun.GameScreen.UI.Particles;
using kyun.OsuUtils;
using kyun.Screen;
using kyun.Utils;
using kyun.Video;
using kyun.Audio;

namespace kyun.GameScreen
{
    public class LoadScreen : ScreenBase
    {
        private Image Logo;
        private Image LoadingSpinner;
        private AudioVideoPlayer auplayer;
        private FilledRectangle rectanglexd;
        private float spinnerRotation;
        private int[] EnphasisColor;

        public string[] selected_song;
        private bool loadDone;
        private Label labelEndLoad;
        private Label labelLoadingText;

        static IScreen instance = null;

        public static IScreen Instance
        {
            get
            {
                if (instance == null)
                    instance = new LoadScreen();
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public static Color getColorRange(int r, int g, int b)
        {
            //Color.FromNonPremultiplied(206, 53, 39, 255)
            int mr = Math.Max(r - 15, 1);
            int mg = Math.Max(g - 15, 1);
            int mb = Math.Max(b - 15, 1);

            int mxr = Math.Min(r + 15, 255);
            int mxg = Math.Min(g + 15, 255);
            int mxb = Math.Min(b + 15, 255);

            return Color.FromNonPremultiplied(OsuBeatMap.rnd.Next(mr, mxr) , OsuBeatMap.rnd.Next(mg, mxg), OsuBeatMap.rnd.Next(mb, mxb), 255);
        }

        public LoadScreen()
        {
            //Colors :F

            List<int[]> ecolors = new List<int[]>();
            ecolors.Add(new int[] { 206, 53, 39 });
            ecolors.Add(new int[] { 237, 245, 8 });
            ecolors.Add(new int[] { 34, 92, 173 });
            ecolors.Add(new int[] { 35, 196, 91 });
            ecolors.Add(new int[] { 145, 35, 196 });


            EnphasisColor = ecolors[OsuBeatMap.rnd.Next(0, ecolors.Count - 1)];

            //cover and songs

            int rndn = new Random(DateTime.Now.Second).Next(20);

            //I Dunno, just randomize
            for (int a = 0; a < rndn; a++)
                OsuBeatMap.rnd.Next();


            List<string[]> songs = new List<string[]>();

            songs.Add(new string[] { AppDomain.CurrentDomain.BaseDirectory + @"\Assets\tofubeats.mp3", AppDomain.CurrentDomain.BaseDirectory + @"\Assets\tofubeats.jpg", "CAND¥¥¥LAND ft LIZ (Pa's Lam System Remix)", "tofubeats" });
            songs.Add(new string[] { AppDomain.CurrentDomain.BaseDirectory + @"\Assets\DJ Noriken Magicalgirl_Syndrome.mp3", AppDomain.CurrentDomain.BaseDirectory + @"\Assets\DJ Noriken.jpg", "#Magicalgirl_Syndrome", "DJ Noriken" });
            songs.Add(new string[] { AppDomain.CurrentDomain.BaseDirectory + @"\Assets\Dunderpatrullen - To The Moon.mp3", AppDomain.CurrentDomain.BaseDirectory + @"\Assets\tothemoon.jpg", "To The Moon", "Dunderpatrullen" });
            songs.Add(new string[] { AppDomain.CurrentDomain.BaseDirectory + @"\Assets\DJ Noriken Magicalgirl_Syndrome.mp3", AppDomain.CurrentDomain.BaseDirectory + @"\Assets\DJ Noriken.jpg", "#Magicalgirl_Syndrome", "DJ Noriken" });
            songs.Add(new string[] { AppDomain.CurrentDomain.BaseDirectory + @"\Assets\tofubeats.mp3", AppDomain.CurrentDomain.BaseDirectory + @"\Assets\tofubeats.jpg", "CAND¥¥¥LAND ft LIZ (Pa's Lam System Remix)", "tofubeats" });
            songs.Add(new string[] { AppDomain.CurrentDomain.BaseDirectory + @"\Assets\Dunderpatrullen - To The Moon.mp3", AppDomain.CurrentDomain.BaseDirectory + @"\Assets\tothemoon.jpg", "To The Moon", "Dunderpatrullen" });


            selected_song = songs[OsuBeatMap.rnd.Next(0, songs.Count - 1)];

            auplayer = new AudioVideoPlayer();


            Background = SpritesContent.Instance.DefaultBackground;
            particleEngine = new ParticleEngine();

            rPeak = false;

            ScreenMode mode = ScreenModeManager.GetActualMode();

            Vector2 logoPosition = new Vector2((mode.Width / 2) - (SpritesContent.Instance.Logo.Width / 2), (mode.Height / 2) - (SpritesContent.Instance.Logo.Height / 2) - SpritesContent.Instance.Logo.Height / 2);

            Logo = new Image(SpritesContent.Instance.IsoLogo)
            {
                Position = logoPosition,
                BeatReact = false,
                //Visible = false

            };

            rectanglexd = new FilledRectangle(new Vector2(300, 150), Color.Black * .75f);

            rectanglexd.Texture = SpritesContent.RoundCorners(rectanglexd.Texture, 10);

            rectanglexd.Position = new Vector2((mode.Width / 2) - (rectanglexd.Texture.Width / 2), mode.Height - rectanglexd.Texture.Height);


            labelLoadingText = new Label(0) {
                Text = "Loading",
                Position = new Vector2(rectanglexd.Position.X + (rectanglexd.Texture.Width / 2), rectanglexd.Position.Y + 10),
                Centered = true,
                Scale = 1,
                Font = SpritesContent.Instance.TitleFont,
                Visible = true
            };


            Vector2 spinnerPosition = new Vector2((mode.Width / 2), labelLoadingText.Position.Y + 20 + SpritesContent.Instance.LoadingSpinnerTx.Height);

            LoadingSpinner = new UI.Image(SpritesContent.Instance.LoadingSpinnerTx)
            {
                Position = spinnerPosition
            };



            labelEndLoad = new Label(0) {
                Text = "Click here to start!",
                Position = new Vector2(LoadingSpinner.Position.X, LoadingSpinner.Position.Y - 20),
                Centered = true,
                Scale = 1,
                Font = SpritesContent.Instance.SettingsFont,
                Visible = false
            };

            lprgs = new ProgressBar(mode.Width, 4) {
                BarColor = Color.FromNonPremultiplied(109, 158, 237, 255)
            };

            float coverSize = 75;

            System.Drawing.Image cimg = System.Drawing.Image.FromFile(selected_song[1]);

            System.Drawing.Bitmap cbimg = SpritesContent.ResizeImage(cimg, (int)(((float)cimg.Width / (float)cimg.Height) * coverSize), (int)coverSize);

            System.Drawing.Bitmap ccbimg;
            MemoryStream istream;
            if (cbimg.Width != cbimg.Height)
            {
                ccbimg = SpritesContent.cropAtRect(cbimg, new System.Drawing.Rectangle((int)(cbimg.Width - coverSize) / 2, 0, (int)coverSize, (int)coverSize));
                istream = SpritesContent.BitmapToStream(ccbimg);
            }
            else
            {
                istream = SpritesContent.BitmapToStream(cbimg);
            }


            coverimg = new Image(Texture2D.FromStream(KyunGame.Instance.GraphicsDevice, (Stream)istream))
            {
                BeatReact = false,
                Position = new Vector2(0, 4),
            };

            coverBox = new FilledRectangle(new Vector2((SpritesContent.Instance.SettingsFont.MeasureString(selected_song[2]).X * .8f) + 20, coverSize), Color.Black * .8f)
            {
                Position = new Vector2(coverSize, coverimg.Position.Y)
            };

            coverLabel = new Label(0) {
                Text = selected_song[2],
                Font = SpritesContent.Instance.SettingsFont,
                Position = new Vector2(coverSize + 5, coverimg.Position.Y),
                Scale = .8f
            };

            coverLabelArt = new Label(0)
            {
                Text = selected_song[3],
                Font = SpritesContent.Instance.SettingsFont,
                Position = new Vector2(coverSize + 15, coverimg.Position.Y + 20),
                Scale = .6f
            };


            Controls.Add(auplayer);
            Controls.Add(particleEngine);
            Controls.Add(rectanglexd);
            Controls.Add(Logo);

            Controls.Add(labelLoadingText);
            Controls.Add(labelEndLoad);
            Controls.Add(LoadingSpinner);
            Controls.Add(lprgs);

            Controls.Add(coverBox);
            Controls.Add(coverimg);
            Controls.Add(coverLabel);
            Controls.Add(coverLabelArt);


            onKeyPress += LoadScreen_onKeyPress;
            rectanglexd.Click += LoadScreen_onClick;

            System.Windows.Forms.Application.DoEvents();
            Thread tr = new Thread(new ThreadStart(loadBeatmaps));
            tr.IsBackground = true;

            tr.Start();

            auplayer.Play(selected_song[0]);
            //bplayer.Play(selected_song[0]);
            //auplayer.Play(AppDomain.CurrentDomain.BaseDirectory + @"\Assets\Junk - enchanted.mp3", "", true);
            
            
            KyunGame.Instance.IsMouseVisible = true;
            //UbeatGame.Instance.OnPeak += Instance_OnPeak;
        }

        bool squareYesNo = false;
        private void addParticle()
        {
            if (!Visible) return;
            if (KyunGame.Instance.Player.PlayState != BassPlayState.Playing) return;

            if (particleEngine.ParticleCount > 40) return;

            Screen.ScreenMode actualMode = Screen.ScreenModeManager.GetActualMode();

            int randomNumber = OsuUtils.OsuBeatMap.GetRnd(1, 10, -1);

            
            for (int a = 0; a < randomNumber; a++)
            {
                //int startTop = OsuUtils.OsuBeatMap.GetRnd(25, actualMode.Height - 25, -1);
                int startLeft = OsuUtils.OsuBeatMap.GetRnd(-50, actualMode.Width + 500, -1);

                float vel = (float)OsuUtils.OsuBeatMap.rnd.NextDouble(0.2, 1);

                int black_rand = OsuBeatMap.rnd.Next(20, 40);
                Color ccolor = (squareYesNo) ? 
                                getColorRange(EnphasisColor[0], EnphasisColor[1], EnphasisColor[2]) :
                                Color.FromNonPremultiplied(black_rand, black_rand, black_rand, 255);

                Particle particle = particleEngine.AddNewSquareParticle(SpritesContent.Instance.SquareParticle,
                    new Vector2(0, vel),
                    new Vector2(startLeft, actualMode.Height),
                    (30 + OsuUtils.OsuBeatMap.rnd.Next(40)) * 100,
                    0.01f * (float)(OsuUtils.OsuBeatMap.rnd.NextDouble() * 2f - 1),
                    ccolor
                    );
                particle.Scale = (float)OsuUtils.OsuBeatMap.rnd.NextDouble(0.4, 0.7);
                particle.Opacity = (float)OsuUtils.OsuBeatMap.rnd.NextDouble(0.4, 0.9);

                squareYesNo = !squareYesNo;
            }
        }

        private void LoadScreen_onClick(object sender, EventArgs e)
        {
            if (loadDone)
            {
                auplayer.Stop();
                ScreenManager.ChangeTo(MainScreen.Instance);
            }
        }
        

        /*
        private void LoadScreen_onTouch(object sender, ubeatTouchEventArgs e)
        {
            Rectangle rg = new Rectangle((int)rectanglexd.Position.X, (int)rectanglexd.Position.Y, rectanglexd.Texture.Width, rectanglexd.Texture.Height);
            if (UbeatGame.Instance.touchHandler.TouchIntersecs(rg))
            {
                if (loadDone)
                {
                    auplayer.Stop();
                    ScreenManager.ChangeTo(MainScreen.Instance);
                }
            }
        }*/


        private void LoadScreen_onKeyPress(object sender, InputEvents.KeyPressEventArgs args)
        {
            if(args.Key == Microsoft.Xna.Framework.Input.Keys.Space)
            {
                if (loadDone)
                {
                    auplayer.Stop();
                    ScreenManager.ChangeTo(MainScreen.Instance);
                }
            }
        }



        private void checkToEnd()
        {
            if (loadDone)
            {
                LoadingSpinner.Visible = false;
                labelEndLoad.Visible = true;
                labelLoadingText.Text = "Loaded!";

            }

            if(loadDone && auplayer.audioplayer.PlayState == BassPlayState.Stopped)
            {
                auplayer.Stop();
                ScreenManager.ChangeTo(MainScreen.Instance);
            }
        }

        internal override void UpdateControls()
        {
            base.UpdateControls();
            checkToEnd();
            addParticle();
            //lprgs.Value = loadingValue;
        }

        public override void Render()
        {
            RenderBg();
            float elapsed = (float)KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalSeconds*12;
            spinnerRotation += elapsed;

            float circle = MathHelper.Pi * 2;
            spinnerRotation = spinnerRotation % circle;


            foreach (UIObjectBase ctr in Controls)
            {
                if (ctr.Equals(LoadingSpinner))
                {
                    if (!ctr.Visible)
                        continue;

                    KyunGame.Instance.SpriteBatch.Draw(LoadingSpinner.Texture, new Rectangle(
                        (int)LoadingSpinner.Position.X,
                        (int)LoadingSpinner.Position.Y,
                        LoadingSpinner.Texture.Width,
                        LoadingSpinner.Texture.Height
                        ), null, Color.White, spinnerRotation, new Vector2(LoadingSpinner.Texture.Width /2, LoadingSpinner.Texture.Height/2), SpriteEffects.None, 0);
                    continue;
                }

                if (ctr.Texture == Utils.SpritesContent.Instance.TopEffect)
                    continue;

                ctr.Render();
            }
            RenderPeak();
        }

        int loadingValue=0;
        private ProgressBar lprgs;
        private ParticleEngine particleEngine;
        private Image coverimg;
        private FilledRectangle coverBox;
        private Label coverLabel;
        private Label coverLabelArt;
        private BPlayer bplayer;

        void loadBeatmaps()
        {
            Logger.Instance.Info("");
            Logger.Instance.Info("Loading beatmaps.");
            Logger.Instance.Info("");


            if (!InstanceManager.Instance.IntancedBeatmaps)
            {
                InstanceManager.AllBeatmaps = new List<Beatmap.Mapset>();
                if (Settings1.Default.osuBeatmaps != "")
                {

                    DirectoryInfo osuDirPath = new DirectoryInfo(Settings1.Default.osuBeatmaps);
                    if (osuDirPath.Exists)
                    {
                        DirectoryInfo[] osuMapsDirs = osuDirPath.GetDirectories();
                        int flieCnt = 0;


                        int fCount = osuMapsDirs.Length;
                        int dCount = 0;

                        foreach (DirectoryInfo odir in osuMapsDirs)
                        {
                            System.Windows.Forms.Application.DoEvents();

                            dCount++;
                            FileInfo[] fils = odir.GetFiles();
                            // Mapset
                            Beatmap.Mapset bmms = null;
                            foreach (FileInfo fff in fils)
                            {

                                if (fff.Extension.ToLower() == ".osu")
                                {

                                    flieCnt++;
                                    OsuUtils.OsuBeatMap bmp = OsuUtils.OsuBeatMap.FromFile(fff.FullName);
                                    if (bmp != null)
                                    {

                                        //Beatmaps.Add(bmp);
                                        if (bmms == null)
                                            bmms = new Beatmap.Mapset(bmp.Title, bmp.Artist, bmp.Creator, bmp.Tags);
                                        bmms.Add(bmp);


                                    }

                                    // Debug.WriteLine("File: {0}s", flieCnt);
                                }

                            }
                            if (bmms != null)
                            {
                                Beatmap.Mapset mapst = Beatmap.Mapset.OrderByDiff(bmms);

                                InstanceManager.AllBeatmaps.Add(mapst);
                            }
                            float pctg = (float)dCount / (float)fCount * 100f;
                            if ((int)pctg % 5 == 0)
                            {

                                if(loadingValue != (int)pctg)
                                {
                                    Logger.Instance.Info("-> {0}%", pctg.ToString("0"));
                                    
                                }
                               
                            }
                            loadingValue = (int)pctg;

                            lprgs.Value = pctg;

                        }
                    }
                    else
                    {
                        
                        Logger.Instance.Warn("Could not find Osu! beatmaps folder, please, make sure that if exist.");
                    }
                }
                else
                {
                    Logger.Instance.Warn(Settings1.Default.osuBeatmaps);
                    Logger.Instance.Warn("osu! beatmaps is not setted, if you have osu beatmaps, set folder in config and restart ubeat.");
                }
            }
            loadLocalMaps();
            Logger.Instance.Info("");
            Logger.Instance.Info("Done.");
            Logger.Instance.Info("");
            Logger.Instance.Info("----------------");
            Logger.Instance.Info("");

            loadDone = true;
            
        }


        void loadLocalMaps()
        {
            if (!InstanceManager.Instance.IntancedBeatmaps)
            {

                Logger.Instance.Info("Loading local");

                DirectoryInfo osuDirPath = new DirectoryInfo(Path.Combine(System.Windows.Forms.Application.StartupPath, "Maps"));
                if (!osuDirPath.Exists)
                    osuDirPath.Create();

                DirectoryInfo[] osuMapsDirs = osuDirPath.GetDirectories();
                int flieCnt = 0;


                int fCount = osuMapsDirs.Length;
                int dCount = 0;

                foreach (DirectoryInfo odir in osuMapsDirs)
                {
                    System.Windows.Forms.Application.DoEvents();

                    dCount++;
                    FileInfo[] fils = odir.GetFiles();
                    // Mapset
                    Beatmap.Mapset bmms = null;
                    foreach (FileInfo fff in fils)
                    {

                        if (fff.Extension.ToLower() == ".osu")
                        {

                            flieCnt++;
                            OsuUtils.OsuBeatMap bmp = OsuUtils.OsuBeatMap.FromFile(fff.FullName);
                            if (bmp != null)
                            {

                                //Beatmaps.Add(bmp);
                                if (bmms == null)
                                    bmms = new Beatmap.Mapset(bmp.Title, bmp.Artist, bmp.Creator, bmp.Tags);
                                bmms.Add(bmp);


                            }

                            // Debug.WriteLine("File: {0}s", flieCnt);
                        }

                    }
                    if (bmms != null)
                    {
                        Beatmap.Mapset mapst = Beatmap.Mapset.OrderByDiff(bmms);

                        InstanceManager.AllBeatmaps.Add(mapst);
                    }
                    float pctg = (float)dCount / (float)fCount * 100f;
                    if (pctg % 20 == 0)
                        Logger.Instance.Info("-> {0}%", pctg);
                }

                InstanceManager.AllBeatmaps = InstanceManager.AllBeatmaps.OrderBy(x => x.Title).ToList<Beatmap.Mapset>();
                InstanceManager.Instance.IntancedBeatmaps = true;
            }
        }
    }
}
