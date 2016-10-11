using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Redux.Utilities.Managers;
using ubeat.Audio;
using ubeat.GameScreen;
using ubeat.Utils;

namespace ubeat
{
    public class UbeatGame : Game
    {

        LoadingWindow lwnd;
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public NPlayer Player;
        public static UbeatGame Instance = null;
        public KeyboardManager kbmgr;
        //Beatmaps
        public SoundEffect soundEffect;
        public List<Beatmap.ubeatBeatMap> Beatmaps = new List<Beatmap.ubeatBeatMap>();

        public Beatmap.ubeatBeatMap SelectedBeatmap { get; set; }

        public float elapsed = 0;
        public bool VideoEnabled { get; set; }
        public Vector2 wSize = new Vector2(800,600);
        public GameTime GameTimeP { get; set; }
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        //4 mainscreen

        public bool FistLoad;

        private float _vol = 0;

        public float GeneralVolume
        {
            get
            {
                return _vol;
            }
            set
            {
                float val = value;
                if (val < 0) val = 0;
                if (val > 1) val = 1;
                
                if (Player != null)
                    if(Player.soundOut != null)
                        Player.soundOut.Volume = val;

                _vol = val;
                
                Settings1.Default.Volume = val;
                Settings1.Default.Save();
                VolDlg.VolShow();
            }
        }
        public FrameCounter frameCounter;

        public UbeatGame()
        {
            frameCounter = new FrameCounter();
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            GameTimeP = new GameTime();

            Content.RootDirectory = "Content";

            
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f);

            kbmgr = new KeyboardManager(this);
            VideoEnabled = Settings1.Default.Video;
        }

        Grid grid;
        public void GameStart(Beatmap.ubeatBeatMap bm,bool automode=false)
        {
            grid = new Grid(bm);
            ScreenManager.ChangeTo(grid);
            grid.Play(null, automode);
        }

        public SpriteFont defaultFont;

        public void GameStop()
        {
            
        }
        public List<Beatmap.Mapset> AllBeatmaps { get; set; }
        protected override void Initialize()
        {
            lwnd = new LoadingWindow();
            lwnd.Show();
            System.Windows.Forms.Application.DoEvents();


            VolDlg = new VolumeDlg();

            base.Initialize();
            Player = new NPlayer();

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
                                        bmms = new Beatmap.Mapset(bmp.Title, bmp.Artist, bmp.Creator,bmp.Tags);
                                    bmms.Add(bmp);


                                }

                               // Debug.WriteLine("File: {0}s", flieCnt);
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


            lwnd.Close();
            Logger.Instance.Info("");
            Logger.Instance.Info("Done.");
            Logger.Instance.Info("");
            Logger.Instance.Info("----------------");
            Logger.Instance.Info("");

            

            //Loads main Screen
            

            hideGameWindow();
        }

        public void ToggleVSync(bool b)
        {
            //
            graphics.SynchronizeWithVerticalRetrace = b;

            if (b)
                ChangeFrameRate(60f);
            else
                ChangeFrameRate(Settings1.Default.FrameRate);
            
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
                                bmms = new Beatmap.Mapset(bmp.Title, bmp.Artist, bmp.Creator,bmp.Tags);
                            bmms.Add(bmp);


                        }

                       // Debug.WriteLine("File: {0}s", flieCnt);
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

            AllBeatmaps = AllBeatmaps.OrderBy(x => x.Artist).ToList<Beatmap.Mapset>();

        }

        void mainWindow_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if(Player.soundOut!=null)
                Player.soundOut.Dispose();
            this.Exit();
        }

        void hideGameWindow()
        {
            Logger.Instance.Info("Loading environment");

            List<Screen.ScreenMode> srcm = Screen.ScreenModeManager.GetSupportedModes();

            wSize.X = srcm[Settings1.Default.ScreenMode].Width;
            wSize.Y = srcm[Settings1.Default.ScreenMode].Height;

            this.graphics.PreferredBackBufferWidth = (int)wSize.X;
            this.graphics.PreferredBackBufferHeight = (int)wSize.Y;

            try
            {
                this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / Settings1.Default.FrameRate);
            }
            catch
            {
                this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60f);
                Settings1.Default.FrameRate = 60;
                Settings1.Default.Save();
            }

            ToggleVSync(Settings1.Default.VSync);

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
           

            //Old

            /*
            mainWindow = new MainWindow();
            mainWindow.FormClosed += mainWindow_FormClosed;
            
            showMain();*/
           
            //New


           

            GeneralVolume = Settings1.Default.Volume;
            this.FistLoad = true;
            ScreenManager.ChangeTo(new MainScreen());
        }
        
        void showMain()
        {
           
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
       // public SoundEffect HolderTick;
        public SoundEffect HolderHit;
        //public SoundEffect ComboBreak;
        //public SoundEffect ButtonHit;
        public Texture2D FailSplash;
        public Texture2D Push;
        public Texture2D Hold;
        public Texture2D StartButton;
        public Texture2D ExitButton;
        public Texture2D ConfigButton;
        public Texture2D AutoModeButton;
        public Texture2D AutoModeButtonSel;
        public Texture2D Logo;
        public Texture2D SpaceSkip;
        //public SoundEffect ButtonOver;
        public SpriteFont GeneralBig;
        public SpriteFont ListboxFont;
        /*update NAudio*/

        public CachedSound ButtonHit;
        public CachedSound ComboBreak;
        public CachedSound ButtonOver;
        public CachedSound HitButton;
        public CachedSound HitHolder;
        public CachedSound HolderFilling;
        public CachedSound HolderTick;
        public CachedSound SelectorHit;
        public CachedSound ScrollHit;
        

        List<CachedSound> SoundsEff = new List<CachedSound>();

        protected override void LoadContent()
        {
           
            Logger.Instance.Info("Loading textures");

            spriteBatch = new SpriteBatch(GraphicsDevice);

        
            buttonDefault = Content.Load<Texture2D>("button_0");
            buttonHolder = Content.Load<Texture2D>("holder_0");
            waitDefault = Content.Load<Texture2D>("approachv2");
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
            Logo = Content.Load<Texture2D>("logo");
            AutoModeButton = Content.Load<Texture2D>("autoBtn");
            AutoModeButtonSel = Content.Load<Texture2D>("autoBtnSel");
            SpaceSkip = Content.Load<Texture2D>("SpaceSkip");


            //test
            StartButton = Content.Load<Texture2D>("PlayMain");
            ExitButton = Content.Load<Texture2D>("ExitMain");
            ConfigButton = Content.Load<Texture2D>("ConfigMain");

            defaultFont = Content.Load<SpriteFont>("SpriteFont1");
            GeneralBig = Content.Load<SpriteFont>("General");
            ListboxFont = Content.Load<SpriteFont>("Listbox");


            HolderFilling = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\HolderFilling.wav");
            HitButton = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\HitButton.wav");
            HitHolder = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\HitHolder.wav");
            HolderTick = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\HolderTick.wav");
            ComboBreak = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\ComboBreak.wav");
            ButtonOver = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\ButtonOver.wav");
            ButtonHit = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\ButtonHit.wav");
            SelectorHit = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\SelectorHit.wav");
            ScrollHit = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\Scroll.wav");

            SoundsEff.Add(HolderFilling);
            SoundsEff.Add(HitButton);
            SoundsEff.Add(HitHolder);
            SoundsEff.Add(HolderTick);
            SoundsEff.Add(ComboBreak);
            SoundsEff.Add(ButtonOver);
            SoundsEff.Add(ButtonHit);


            Logger.Instance.Info("");
            Logger.Instance.Info("Done.");
            Logger.Instance.Info("");
            Logger.Instance.Info("----------------");
        }

        
        protected override void UnloadContent()
        {
            if (Player != null)
                Player.Dispose();
            /*
            foreach (CachedSound ndi in SoundsEff)
                ndi.Dispose();
            */
            AudioPlaybackEngine.Instance.Dispose();
            //_UI.Shutdown();

            Logger.Instance.Info("Bye!");
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
            
            float frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            kbmgr.Update(gameTime);
            ScreenManager.Update(gameTime);
            
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            ScreenManager.Render();

            spriteBatch.End();

            base.Draw(gameTime);


            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            frameCounter.Update(deltaTime);
        }

        #endregion

        //console
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public void ChangeResolution(Screen.ScreenMode screenMode)
        {
            this.SuppressDraw();

            System.Windows.Forms.Form FormGame = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Window.Handle);

            if (screenMode.WindowMode != Screen.WindowDisposition.Windowed)
            {
                FormGame.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                FormGame.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }
            else
            {
                FormGame.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                FormGame.WindowState = System.Windows.Forms.FormWindowState.Normal;

            }

            this.wSize.X = screenMode.Width;
            this.wSize.Y = screenMode.Height;

            this.graphics.PreferredBackBufferWidth = (int)wSize.X;
            this.graphics.PreferredBackBufferHeight = (int)wSize.Y;


            this.graphics.ApplyChanges();

            if(ScreenManager.ActualScreen!=null)
                ScreenManager.ActualScreen.Redraw();            

        }

        public void ToggleFullscreen(bool enabled=false)
        {
            System.Windows.Forms.Form FormGame = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
            if (enabled)
            {  
                FormGame.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                FormGame.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }
            else
            {
                if (Screen.ScreenModeManager.GetSupportedModes()[Settings1.Default.VideoMode].WindowMode == Screen.WindowDisposition.Windowed)
                {
                    FormGame.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                    FormGame.WindowState = System.Windows.Forms.FormWindowState.Normal;
                }
                else
                {
                    FormGame.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    FormGame.WindowState = System.Windows.Forms.FormWindowState.Normal;
                }
            }

            this.graphics.IsFullScreen=enabled;
            this.graphics.ApplyChanges();
        }

        public void ChangeFrameRate(float fps)
        {
            if (fps > 250)
                this.IsFixedTimeStep = false;
            else
            {
                this.IsFixedTimeStep = true;
                this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / fps);
            }
        }

        public VolumeDlg VolDlg { get; set; }
    }
}
