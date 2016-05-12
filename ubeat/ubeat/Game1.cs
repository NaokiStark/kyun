using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.IO;
using System.Diagnostics;
using ubeat.Audio;
using ubeat.GameScreen;
using System.Runtime.InteropServices;


namespace ubeat
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public MainWindow mainWindow;
        public NPlayer player;
        public static Game1 Instance = null;
        //Beatmaps
        public SoundEffect soundEffect;
        public List<Beatmap.ubeatBeatMap> Beatmaps = new List<Beatmap.ubeatBeatMap>();
        public float elapsed = 0;
        public bool VideoEnabled { get; set; }
        public Vector2 wSize = new Vector2(800,600);
        public GameTime GameTimeP { get; set; }
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        private float vol=0;

        public float GeneralVolume
        {
            get
            {
                return vol;
            }
            set
            {
                float val = value;
                if (val < 0) val = 0;
                if (val > 1) val = 1;
                
                if (player != null)
                    if(player.soundOut!=null)
                        player.soundOut.Volume = val;

                vol = val;
                
                Settings1.Default.Volume = val;
                Settings1.Default.Save();
                VolDlg.VolShow();
            }
        }


        public Game1()
        {
           
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            GameTimeP = new GameTime();

            Content.RootDirectory = "Content";

            
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f);

            //this.IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;

            VideoEnabled = Settings1.Default.Video;
        }

        Grid grid;
        public void GameStart(Beatmap.ubeatBeatMap bm)
        {
            grid = new Grid(bm);
            grid.Play();
        }
        public SpriteFont fontDefault;
        public void GameStop()
        {
            
        }
        public List<Beatmap.Mapset> AllBeatmaps { get; set; }
        protected override void Initialize()
        {

            VolDlg = new VolumeDlg();

            base.Initialize();
            player = new NPlayer();

            //Loads Beatmaps
            Logger.Instance.Info("");
            Logger.Instance.Info("Loading beatmaps.");
            Logger.Instance.Info("");
            AllBeatmaps = new List<Beatmap.Mapset>();

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
                                        bmms = new Beatmap.Mapset(bmp.Title, bmp.Artist, bmp.Creator);
                                    bmms.Add(bmp);


                                }

                                Debug.WriteLine("File: {0}s", flieCnt);
                            }

                        }
                        if (bmms != null)
                        {
                            Beatmap.Mapset mapst = Beatmap.Mapset.OrderByDiff(bmms);

                            AllBeatmaps.Add(mapst);
                        }
                        float pctg = (float)dCount / (float)fCount * 100f;
                        if (pctg % 20 == 0)
                            Logger.Instance.Info("-> {0}%", pctg);
                    }
                }
                else
                {
                    Logger.Instance.Warn("Could not find Osu! beatmaps folder, please, make sure that if exist.");
                }
            }
            else
            {
                Logger.Instance.Warn("osu! beatmaps is not setted, if you have osu beatmaps, set folder in config and restart ubeat.");
            }

            loadLocalMaps();
            Logger.Instance.Info("");
            Logger.Instance.Info("Done.");
            Logger.Instance.Info("");
            Logger.Instance.Info("----------------");
            Logger.Instance.Info("");

            

            //Loads main Screen
            

            hideGameWindow();
        }

        void loadLocalMaps()
        {
            Logger.Instance.Info("Loading local");

            DirectoryInfo osuDirPath = new DirectoryInfo(Path.Combine(System.Windows.Forms.Application.StartupPath,"Maps"));
            if (!osuDirPath.Exists)
                osuDirPath.Create();

            DirectoryInfo[] osuMapsDirs = osuDirPath.GetDirectories();
            int flieCnt = 0;


            int fCount = osuMapsDirs.Length;
            int dCount = 0;

            foreach (DirectoryInfo odir in osuMapsDirs)
            {
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
                                bmms = new Beatmap.Mapset(bmp.Title, bmp.Artist, bmp.Creator);
                            bmms.Add(bmp);


                        }

                        Debug.WriteLine("File: {0}s", flieCnt);
                    }

                }
                if (bmms != null)
                {
                    Beatmap.Mapset mapst = Beatmap.Mapset.OrderByDiff(bmms);

                    AllBeatmaps.Add(mapst);
                }
                float pctg = (float)dCount / (float)fCount * 100f;
                if (pctg % 20 == 0)
                    Logger.Instance.Info("-> {0}%", pctg);
            }

        }

        void mainWindow_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if(player.soundOut!=null)
                player.soundOut.Dispose();
            this.Exit();
        }

        void hideGameWindow()
        {
            Logger.Instance.Info("Loading enviroment");

            List<Screen.ScreenMode> srcm = Screen.ScreenModeManager.GetSupportedModes();

            wSize.X = srcm[Settings1.Default.ScreenMode].Width;
            wSize.Y = srcm[Settings1.Default.ScreenMode].Height;

            this.graphics.PreferredBackBufferWidth = (int)wSize.X;
            this.graphics.PreferredBackBufferHeight = (int)wSize.Y;

            this.graphics.ApplyChanges();


            /*
            this.graphics.PreferredBackBufferWidth = this.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            this.graphics.PreferredBackBufferHeight = this.GraphicsDevice.Adapter.CurrentDisplayMode.Height;

            */

            if(srcm[Settings1.Default.ScreenMode].WindowMode != Screen.WindowDisposition.Windowed){
                
                System.Windows.Forms.Form FormGame = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
            
                FormGame.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                FormGame.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }
           
            
            mainWindow = new MainWindow();
            mainWindow.FormClosed += mainWindow_FormClosed;
            
            showMain();
           
        }
        
        void showMain()
        {
            mainWindow.WindowState = System.Windows.Forms.FormWindowState.Normal;
            mainWindow.Width = (int)wSize.X;
            mainWindow.Height = (int)wSize.Y; 
            mainWindow.Show();
            GeneralVolume = Settings1.Default.Volume;
            Logger.Instance.Info("");
            Logger.Instance.Info("Kansei shimashita (/^-^)/!");
            Logger.Instance.Info("");
            Logger.Instance.Info("----------------");
           
        }

        #region TEXTURES
        public Texture2D buttonDefault;
        public Texture2D buttonHolder;
        public Texture2D waitDefault;
        public Texture2D HolderFillDeff;
        public Texture2D radiance;
        public Texture2D PauseSplash;
        public Texture2D PerfectTx;
        public Texture2D ExcellentTx;
        public Texture2D GoodTx;
        public Texture2D MissTx;
        public SoundEffect HitHolderFilling;
        public SoundEffect HolderTick;
        public SoundEffect HolderHit;
        public SoundEffect ComboBreak;
        public Texture2D FailSplash;
        public Texture2D Push;
        public Texture2D Hold;
        
        protected override void LoadContent()
        {
           
            Logger.Instance.Info("Loading textures");

            spriteBatch = new SpriteBatch(GraphicsDevice);

            buttonDefault = Content.Load<Texture2D>("button_0");
            buttonHolder = Content.Load<Texture2D>("holder_0");
            waitDefault = Content.Load<Texture2D>("approach");
            HolderFillDeff = Content.Load<Texture2D>("HolderFill");
            radiance = Content.Load<Texture2D>("radiance");
            PauseSplash = Content.Load<Texture2D>("pausesplash");
            PerfectTx = Content.Load<Texture2D>("Perfect");
            ExcellentTx= Content.Load<Texture2D>("Excellent");
            GoodTx = Content.Load<Texture2D>("Good");
            MissTx = Content.Load<Texture2D>("Miss");
            FailSplash = Content.Load<Texture2D>("failsplash");
            Push = Content.Load<Texture2D>("push");
            Hold = Content.Load<Texture2D>("hold");


            fontDefault = Content.Load<SpriteFont>("SpriteFont1");

            soundEffect = Content.Load<SoundEffect>("normal-hitnormal2");
            HitHolderFilling = Content.Load<SoundEffect>("HolderFilling");
            HolderTick = Content.Load<SoundEffect>("HolderTick");
            HolderHit = Content.Load<SoundEffect>("soft-hitclap2");
            ComboBreak = Content.Load<SoundEffect>("combobreak");

            Logger.Instance.Info("");
            Logger.Instance.Info("Done.");
            Logger.Instance.Info("");
            Logger.Instance.Info("----------------");
        }

        
        protected override void UnloadContent()
        {
            if (player != null)
                player.Dispose();

            Logger.Instance.Info(" Bye! ");
        }

#endregion

        #region GameUpdates
        protected override void Update(GameTime gameTime)
        {
            //Update Gametime FIRST
            GameTimeP = gameTime;

            if (Keyboard.GetState().IsKeyDown(Keys.Add))
                GeneralVolume += .02f;

            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
                GeneralVolume -= .02f;

            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (grid != null)
            {
                grid.Update(gameTime);
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (grid != null)
            {
                grid.Render();
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        //console
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public void ChangeResolution(Screen.ScreenMode screenMode)
        {
            this.SuppressDraw();

            mainWindow.SuspendLayout();
            System.Windows.Forms.Form FormGame = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);

            if (screenMode.WindowMode != Screen.WindowDisposition.Windowed)
            {
                FormGame.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                FormGame.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                mainWindow.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }
            else
            {
                FormGame.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                FormGame.WindowState = System.Windows.Forms.FormWindowState.Normal;
                mainWindow.WindowState = System.Windows.Forms.FormWindowState.Normal;

            }

            this.wSize.X = screenMode.Width;
            this.wSize.Y = screenMode.Height;

            this.graphics.PreferredBackBufferWidth = (int)wSize.X;
            this.graphics.PreferredBackBufferHeight = (int)wSize.Y;


            this.graphics.ApplyChanges();

            mainWindow.Width = (int)wSize.X;
            mainWindow.Height = (int)wSize.Y;
          
            if (BeatmapSelector.Instance != null)
            {
                MainWindow.Instance.ShowControls();
                BeatmapSelector.Instance.Close();
            }
            mainWindow.ResumeLayout();
        }

        public void ChangeFrameRate(float fps)
        {
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / fps);
        }

        public VolumeDlg VolDlg { get; set; }
    }
}
