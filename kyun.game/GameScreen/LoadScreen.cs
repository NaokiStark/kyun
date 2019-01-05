using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using kyun.GameScreen.UI;
using kyun.GameScreen.UI.Particles;
using kyun.OsuUtils;
using kyun.Screen;
using kyun.Utils;
using kyun.Video;
using kyun.Audio;
using kyun.Beatmap;
using kyun.GameModes.Classic;
using Newtonsoft.Json.Linq;
using kyun.game;
using kyun.game.GameScreen.UI;
using kyun.game.Database;

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

        public SongInfo selected_song;
        private bool loadDone;
        private Label labelEndLoad;
        private Label labelLoadingText;
        private bool issueChange = false;
        private ubeatBeatMap TutorialBeatmap;
        private bool _resetDb;
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

            return Color.FromNonPremultiplied(OsuBeatMap.rnd.Next(mr, mxr), OsuBeatMap.rnd.Next(mg, mxg), OsuBeatMap.rnd.Next(mb, mxb), 255);
        }

        public LoadScreen(bool resetDatabase)
        {
            instance = this;
            _resetDb = resetDatabase;
            loadInterface();
        }

        public LoadScreen()
        {
            loadInterface();
        }

        private void loadInterface()
        {
            //Colors :F

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


            EnphasisColor = ecolors[OsuBeatMap.rnd.Next(0, ecolors.Count - 1)];

            //cover and songs

            int rndn = new Random(DateTime.Now.Second).Next(20);

            //I Dunno, just randomize
            for (int a = 0; a < rndn; a++)
                OsuBeatMap.rnd.Next();


            /**/
            List<SongInfo> songsList = new List<SongInfo>();

            DirectoryInfo songDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/songs"));

            DirectoryInfo[] sDirs = songDir.GetDirectories();

            try
            {
                int nxt = OsuBeatMap.rnd.Next(0, sDirs.Length - 1);
                DirectoryInfo dr = sDirs[nxt];

                FileInfo[] fls = dr.GetFiles();

                foreach (FileInfo file in fls)
                {
                    if (!file.Name.ToLower().EndsWith(".json"))
                        continue;

                    StreamReader streader = File.OpenText(file.FullName);

                    JObject jo = JObject.Parse(streader.ReadToEnd());

                    var songInfo = new SongInfo();
                    songInfo.Artist = (string)jo["artist"];
                    songInfo.Title = (string)jo["title"];
                    songInfo.Cover = Path.Combine(file.DirectoryName, (string)jo["cover"]);
                    songInfo.Song = Path.Combine(file.DirectoryName, (string)jo["song"]);
                    selected_song = songInfo;

                    streader.Close();
                    streader.Dispose();

                    break;

                }
            }
            catch (Exception EXX)
            {
                Logger.Instance.Warn(EXX.Message + EXX.StackTrace);
            }
            /**/

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

            //rectanglexd.Texture = SpritesContent.RoundCorners(rectanglexd.Texture, 10);

            rectanglexd.Position = new Vector2((mode.Width / 2) - (rectanglexd.Texture.Width / 2), mode.Height - rectanglexd.Texture.Height);


            labelLoadingText = new Label(0)
            {
                Text = "Loading beatmaps",
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



            labelEndLoad = new Label(0)
            {
                Text = "Click here to start!",
                Position = new Vector2(LoadingSpinner.Position.X, LoadingSpinner.Position.Y - 20),
                Centered = true,
                Scale = 1,
                Font = SpritesContent.Instance.SettingsFont,
                Visible = false
            };

            lprgs = new ProgressBar(mode.Width, 4)
            {
                BarColor = Color.FromNonPremultiplied(109, 158, 237, 255)
            };

            float coverSize = 75;

            System.Drawing.Image cimg = System.Drawing.Image.FromFile(selected_song.Cover);

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


            coverimg = new Image(ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, (Stream)istream))
            {
                BeatReact = false,
                Position = new Vector2(0, 4),
            };

            coverLabel = new Label(0)
            {
                Text = selected_song.Title,
                Font = SpritesContent.Instance.SettingsFont,
                Position = new Vector2(coverSize + 5, coverimg.Position.Y),
                Scale = .8f
            };

            coverLabelArt = new Label(0)
            {
                Text = selected_song.Artist,
                Font = SpritesContent.Instance.SettingsFont,
                Position = new Vector2(coverSize + 15, coverimg.Position.Y + 20),
                Scale = .6f
            };

            float titleSize = 1;
            float artSize = 1;
            try
            {
                titleSize = SpritesContent.Instance.SettingsFont.MeasureString(selected_song.Title).X;
                artSize = SpritesContent.Instance.SettingsFont.MeasureString(selected_song.Artist).X;
            }
            catch
            {
                titleSize = SpritesContent.Instance.MSGothic2.MeasureString(selected_song.Title).X;
                artSize = SpritesContent.Instance.MSGothic2.MeasureString(selected_song.Artist).X;
            }


            coverBox = new FilledRectangle(new Vector2(1), Color.Black * .8f)
            {
                Position = new Vector2(coverSize, coverimg.Position.Y)
            };



            float maxSize = Math.Max(titleSize, artSize);

            coverBox.Resize(new Vector2((maxSize * .8f) + 20, coverSize));


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
            //tr.IsBackground = true;

            tr.Start();

            auplayer.Play(selected_song.Song);
            //bplayer.Play(selected_song[0]);
            //auplayer.Play(AppDomain.CurrentDomain.BaseDirectory + @"\Assets\Junk - enchanted.mp3", "", true);

            
            KyunGame.Instance.Notifications.ShowDialog(suggestions[OsuBeatMap.rnd.Next(0, suggestions.Length-1)], 15000, Notifications.NotificationType.Info);
            //UbeatGame.Instance.OnPeak += Instance_OnPeak;
        }

        public void PickSong()
        {


        }

        bool squareYesNo = false;
        private void addParticle()
        {
            if (!Visible) return;
            if (KyunGame.Instance.Player.PlayState != BassPlayState.Playing) return;

            if (particleEngine.ParticleCount > 40) return;

            Screen.ScreenMode actualMode = Screen.ScreenModeManager.GetActualMode();

            int randomNumber = OsuUtils.OsuBeatMap.GetRnd(1, 10, -1);

            bool stwp = false;

            for (int a = 0; a < randomNumber; a++)
            {
                int startLeft = 0;
                if (KyunGame.xmas && stwp)
                {

                    int startTop = 0;
                    startTop = OsuUtils.OsuBeatMap.GetRnd(25, actualMode.Height - 25, -1);
                    startLeft = OsuUtils.OsuBeatMap.GetRnd(25, actualMode.Width + 500, -1);

                    Particle pparticle = particleEngine.AddNewParticle(SpritesContent.Instance.MenuSnow,
                        new Microsoft.Xna.Framework.Vector2((5f * (float)(OsuUtils.OsuBeatMap.rnd.NextDouble() * 2 - 1)) / 10f, Math.Abs(5f * (float)(OsuUtils.OsuBeatMap.rnd.NextDouble() * 2 - 1)) / 10f),
                        new Microsoft.Xna.Framework.Vector2(startLeft, 0),
                        (30 + OsuUtils.OsuBeatMap.rnd.Next(40)) * 100,
                        0.01f * (float)(OsuUtils.OsuBeatMap.rnd.NextDouble() * 2f - 1)
                        );

                    pparticle.Opacity = 0.6f;
                    pparticle.Scale = (float)OsuUtils.OsuBeatMap.rnd.NextDouble(0.1, 0.6);
                    pparticle.StopAtBottom = true;
                    if (KyunGame.xmas)
                        pparticle.TextureColor = Color.Yellow;
                    stwp = !stwp;
                    continue;
                }
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
                                getColorRange(EnphasisColor[0], EnphasisColor[1], EnphasisColor[2]) :
                                Color.FromNonPremultiplied(black_rand, black_rand, black_rand, 255);

                if (KyunGame.xmas && OsuBeatMap.rnd.NextBoolean())
                {
                    ccolor = getColorRange(ecolors[1][0], ecolors[1][1], ecolors[1][2]);
                }

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
                stwp = !stwp;
            }
        }

        private void LoadScreen_onClick(object sender, EventArgs e)
        {
            issueChange = true;

            if (loadDone)
            {

                auplayer.Stop();
                //if (!Settings1.Default.Tutorial)
                ScreenManager.ChangeTo(MainScreen.Instance);
                /*else
                {
                    ScreenManager.ChangeTo(ClassicModeScreen.GetInstance());
                    ClassicModeScreen c = (ClassicModeScreen)ClassicModeScreen.GetInstance();
                    c.Play(TutorialBeatmap);
                }*/
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
            issueChange = true;
            if (args.Key == Microsoft.Xna.Framework.Input.Keys.Space)
            {
                if (loadDone)
                {
                    auplayer.Stop();
                    //if (!Settings1.Default.Tutorial)
                    ScreenManager.ChangeTo(MainScreen.Instance);
                    /*else
                    {
                        ScreenManager.ChangeTo(ClassicModeScreen.GetInstance());
                        ClassicModeScreen c = (ClassicModeScreen)ClassicModeScreen.GetInstance();
                        c.Play(TutorialBeatmap);
                    }*/
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

            if (loadDone && auplayer.audioplayer.PlayState == BassPlayState.Stopped && !issueChange)
            {
                auplayer.Stop();
                //if (!Settings1.Default.Tutorial)
                ScreenManager.ChangeTo(MainScreen.Instance);
                /*else
                {
                    ScreenManager.ChangeTo(ClassicModeScreen.GetInstance());
                    ClassicModeScreen c = (ClassicModeScreen)ClassicModeScreen.GetInstance();
                    c.Play(TutorialBeatmap);
                }*/
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
            float elapsed = (float)KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalSeconds * 12;
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
                        ), null, Color.White, spinnerRotation, new Vector2(LoadingSpinner.Texture.Width / 2, LoadingSpinner.Texture.Height / 2), SpriteEffects.None, 0);
                    continue;
                }

                if (ctr.Texture == Utils.SpritesContent.Instance.TopEffect)
                    continue;

                ctr.Render();
            }
            RenderPeak();
        }

        int loadingValue = 0;
        private ProgressBar lprgs;
        private ParticleEngine particleEngine;
        private Image coverimg;
        private FilledRectangle coverBox;
        private Label coverLabel;
        private Label coverLabelArt;
        private BPlayer bplayer;
        private List<int[]> ecolors;

        static int id = 0;
        static int bid = 0;

        int getBeatmapsCount()
        {
            return Settings1.Default.BeatmapsCount;
        }
        
        void loadBeatmaps()
        {
            if (_resetDb)
            {
                Logger.Instance.Info("");
                Logger.Instance.Info("Clearing beatmaps in database.");
                Logger.Instance.Info("");
                labelLoadingText.Text = "Cleaning...";
                DatabaseInterface.Instance.DeleteBeatmaps(); //This make break all
            }

            Logger.Instance.Info("");
            Logger.Instance.Info("Loading beatmaps.");
            Logger.Instance.Info("");

            labelLoadingText.Text = "Loading beatmaps";

            var db = DatabaseInterface.Instance;


            if (InstanceManager.AllBeatmaps == null)
            {
                InstanceManager.AllBeatmaps = new List<Beatmap.Mapset>();
            }
            else
            {
                InstanceManager.AllBeatmaps.Clear();
            }

            var dbMapsets = db.GetBeatmaps();
            //var dbBeatmaps = db.GetAllBeatmaps();


            if (Settings1.Default.osuBeatmaps != "")
            {

                DirectoryInfo osuDirPath = new DirectoryInfo(Settings1.Default.osuBeatmaps);
                if (osuDirPath.Exists)
                {
                    DirectoryInfo[] osuMapsDirs = osuDirPath.GetDirectories();

                    
                    int flieCnt = 0;
                    int bmcount = getBeatmapsCount();
                    //int dbcount = dbBeatmaps.Count;
                    

                    if (dbMapsets.Count > 0)
                    {
                        InstanceManager.AllBeatmaps = dbMapsets.OrderBy(x => x.Title).ToList();
                        lprgs.Value = 100;
                        GC.Collect();
                        Logger.Instance.Info("Loaded from database");
                        Logger.Instance.Info("--------------------");

                        loadDone = true;
                        return;
                    }

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
                                    bid++;
                                    bmp.Id = bid;
                                    
                                    //Beatmaps.Add(bmp);
                                    if (bmms == null)
                                    {
                                        id++;
                                        bmms = new Beatmap.Mapset(bmp.Title, bmp.Artist, bmp.Creator, bmp.Tags) { Id = id };
                                        
                                    }
                                    else
                                    {
                                        
                                    }
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

                            if (loadingValue != (int)pctg)
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

            loadLocalMaps();
            Logger.Instance.Info("");
            Logger.Instance.Info("Done.");
            Logger.Instance.Info("");
            Logger.Instance.Info("----------------");
            Logger.Instance.Info("");
            GC.Collect();

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
                                bid++;
                                bmp.Id = bid;
                                
                                if (bmp.Title.Contains("kyun! Tutorial"))
                                {
                                    TutorialBeatmap = bmp;
                                }

                                //Beatmaps.Add(bmp);
                                if (bmms == null)
                                {
                                    id++;
                                    bmms = new Beatmap.Mapset(bmp.Title, bmp.Artist, bmp.Creator, bmp.Tags) { Id = id };
                                    
                                }

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

            var db = DatabaseInterface.Instance;
            labelLoadingText.Text = "Updating database";
            db.SaveBeatmaps(InstanceManager.AllBeatmaps);
            Settings1.Default.BeatmapsCount = InstanceManager.AllBeatmaps.Count;
            Settings1.Default.Save();

            // Settings1.Default.Reset();

            if (false)
            {
                if (TutorialBeatmap == null)
                {
                    Settings1.Default.Tutorial = false; //Skip tutorial :/

                    return;
                }


                KyunGame.Instance.SelectedBeatmap = TutorialBeatmap;
                if (ClassicModeScreen.Instance == null)
                    ClassicModeScreen.Instance = new ClassicModeScreen();
            }
        }

        private string[] suggestions = new string[] { "You can add your osu! library on Config",
                                                      "You can breathe",
                                                      "Use [X] or [Y] to make fast combos on Circles! mode",
                                                      "To add beatmaps, Drag And Drop \".osz\" files in this window",
                                                      "Need beatmaps? Download in bloodcat.com/osu/ and drop \".osz\" files here!",
                                                      "Thank you for play! --- You can leave a comment or suggestion in fabistark.itch.io/kyun",
                                                      "This is a beta version",
                                                      "owo",
                                                      "uwu",
                                                      "¿Hola? ¿Hay alguien aquí?",
                                                      "Here should be a suggestion, but I could not think of one, sorry"};
    }
}
