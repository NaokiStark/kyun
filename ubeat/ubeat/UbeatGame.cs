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
using NAudio.Wave.SampleProviders;

namespace ubeat
{
    public class UbeatGame : Game
    {
        //Puto
        //LoadingWindow LoadingWindow;
        GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public NPlayer Player;
        public static UbeatGame Instance = null;
        public KeyboardManager KeyBoardManager;
        //Beatmaps

        public bool ppyMode {
            get
            {
                return fuckMode;
            }
            set
            {
                fuckMode = value;
                peppyMode();
            }
        }

        bool fuckMode;

        public SoundEffect soundEffect;

        public Beatmap.ubeatBeatMap SelectedBeatmap { get; set; }

        public float elapsed = 0;
        public bool VideoEnabled { get; set; }
        public Vector2 wSize = new Vector2(800, 600);
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
                    if (Player.soundOut != null)
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
            Graphics = new GraphicsDeviceManager(this);
            GameTimeP = new GameTime();

            Content.RootDirectory = "Content";
            

            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f);

            KeyBoardManager = new KeyboardManager(this);
            VideoEnabled = Settings1.Default.Video;
        }

        Grid grid;
        public void GameStart(Beatmap.ubeatBeatMap bm, bool automode = false)
        {
            grid = new Grid(bm);
            ScreenManager.ChangeTo(grid);
            grid.Play(null, automode);
        }

        //public SpriteFont defaultFont;


        void peppyMode()
        {
            if (fuckMode)
            {
                SpritesContent.Instance.Logo = ContentLoader.LoadTextureFromAssets("Losu.png");
            }
            else
            {
                SpritesContent.Instance.Logo = Content.Load<Texture2D>("logo");
            }
            

        }

        
        void loadEnviroment()
        {
            Logger.Instance.Info("Loading environment");

            List<Screen.ScreenMode> srcm = Screen.ScreenModeManager.GetSupportedModes();

            wSize.X = srcm[Settings1.Default.ScreenMode].Width;
            wSize.Y = srcm[Settings1.Default.ScreenMode].Height;

            this.Graphics.PreferredBackBufferWidth = (int)wSize.X;
            this.Graphics.PreferredBackBufferHeight = (int)wSize.Y;

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
            ToggleFullscreen(Settings1.Default.FullScreen);

            /*
            this.graphics.PreferredBackBufferWidth = this.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            this.graphics.PreferredBackBufferHeight = this.GraphicsDevice.Adapter.CurrentDisplayMode.Height;

            */
            System.Windows.Forms.Form FormGame = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Window.Handle);

            if (srcm[Settings1.Default.ScreenMode].WindowMode != Screen.WindowDisposition.Windowed)
            {

              

                FormGame.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                FormGame.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }


          
            this.FistLoad = true;

#if DEBUG
            Logger.Instance.Debug("!!!! GRAPHIC CARD !!!!");
            Logger.Instance.Debug("======================");
            Logger.Instance.Debug("");

            Logger.Instance.Debug(Graphics.GraphicsDevice.Adapter.Description);
            Logger.Instance.Debug(Graphics.GraphicsDevice.Adapter.DeviceName);
            Logger.Instance.Debug(Graphics.GraphicsDevice.Adapter.VendorId.ToString());
            Logger.Instance.Debug((Graphics.GraphicsDevice.Adapter.IsDefaultAdapter) ? "Default adapter: True" : "Default adapter: False");



            Logger.Instance.Debug("");
            Logger.Instance.Debug("======================");

#endif

            Graphics.PreferMultiSampling = true;
            Graphics.ApplyChanges();
            this.IsFixedTimeStep = true;

        }

        protected override void Initialize()
        {


            //LoadingWindow = new LoadingWindow();
            //LoadingWindow.Show();
            //System.Windows.Forms.Application.DoEvents();
            base.Initialize();
            loadEnviroment();

           

            VolDlg = new VolumeDlg();

           
            Player = new NPlayer();

            
            //Loads Beatmaps
            



            //LoadingWindow.Close();
            



            //Loads main Screen


            hideGameWindow();
            GeneralVolume = Settings1.Default.Volume;
        }

        public void ToggleVSync(bool b)
        {
            //
            Graphics.SynchronizeWithVerticalRetrace = b;

            if (b)
                ChangeFrameRate(60f);
            else
                ChangeFrameRate(Settings1.Default.FrameRate);

        }

       

        void mainWindow_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (Player.soundOut != null)
                Player.soundOut.Dispose();
            this.Exit();
        }

        void hideGameWindow()
        {
            if (!Settings1.Default.QuestionVideo)
            {
                System.Windows.Forms.DialogResult drs = System.Windows.Forms.MessageBox.Show("Warning!\r\n\r\nLa presentacion que viene a continuacion contiene video, si no tienes un equipo 'potente', desactiva esa opcion, en serio, el video tiende a fallar e interfiere con el juego en equipos lentos.\r\n\r\n¿deseas activar el video?", "",System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                if(drs == System.Windows.Forms.DialogResult.No)
                {
                    Settings1.Default.Video = false;
                    Settings1.Default.QuestionVideo = true;
                    Settings1.Default.Save();
                    System.Windows.Forms.MessageBox.Show("ubeat se reiniciará.");
                    InstanceManager.Instance.Reload();
                    return;
                }
                Settings1.Default.QuestionVideo = true;
                Settings1.Default.Save();
            }

            ScreenManager.ChangeTo(new LoadScreen());
          
        }

        void showMain()
        {

            Logger.Instance.Info("");
            Logger.Instance.Info("Kansei shimashita (/^-^)/!");
            Logger.Instance.Info("");
            Logger.Instance.Info("----------------");

        }

        #region TEXTURES
        //public Texture2D buttonDefault;
        //public Texture2D buttonHolder;
        //public Texture2D buttonDefault_0;
        //public Texture2D buttonHolder_0;

        //public Texture2D waitDefault;
        //public Texture2D waitDefault_0;

        //public Texture2D HolderFillDeff;
        //public Texture2D HolderFillDeff_0;

        //public Texture2D radiance;
        //public Texture2D PauseSplash;
        //public Texture2D PerfectTx;
        //public Texture2D ExcellentTx;
        //public Texture2D GoodTx;
        //public Texture2D MissTx;
        //public SoundEffect HitHolderFilling;
        //// public SoundEffect HolderTick;
        //public SoundEffect HolderHit;
        ////public SoundEffect ComboBreak;
        ////public SoundEffect ButtonHit;
        //public Texture2D FailSplash;
        //public Texture2D Push;
        //public Texture2D Hold;
        //public Texture2D StartButton;
        //public Texture2D ExitButton;
        //public Texture2D ConfigButton;
        //public Texture2D AutoModeButton;
        //public Texture2D AutoModeButtonSel;
        //public Texture2D Logo;
        //public Texture2D SpaceSkip;
        //public Texture2D TopEffect;
        //public Texture2D DefaultBackground;


        ////public SoundEffect ButtonOver;
        //public SpriteFont GeneralBig;
        //public SpriteFont ListboxFont;
        //public SpriteFont SettingsFont;
        //public SpriteFont TitleFont;
        ///*update NAudio*/

        //public CachedSound ButtonHit;
        //public CachedSound ComboBreak;
        //public CachedSound ButtonOver;
        //public CachedSound HitButton;
        //public CachedSound HitHolder;
        //public CachedSound HolderFilling;
        //public CachedSound HolderTick;
        //public CachedSound SelectorHit;
        //public CachedSound ScrollHit;
        //public CachedSound SeeyaOsu;


        public CachedSound WelcomeToOsuXd;

        List<CachedSound> SoundsEff = new List<CachedSound>();
        List<Texture2D> Textures = new List<Texture2D>();
        public TouchHandler touchHandler;

        protected override void LoadContent()
        {

            

            Screen.ScreenMode actmode = Screen.ScreenModeManager.GetActualMode();

            ScreenManager.Start();

            Logger.Instance.Info("Loading textures");

            SpriteBatch = new SpriteBatch(GraphicsDevice);


            //buttonDefault = Content.Load<Texture2D>("button_0");
            //buttonHolder = Content.Load<Texture2D>("holder_0");
            //waitDefault = Content.Load<Texture2D>("approachv2");
            //HolderFillDeff = Content.Load<Texture2D>("HolderFill");


            //buttonDefault_0 = Content.Load<Texture2D>("button_0.5");
            //buttonHolder_0 = Content.Load<Texture2D>("holder_0.5");
            //waitDefault_0 = Content.Load<Texture2D>("approach0.5");
            //HolderFillDeff_0 = Content.Load<Texture2D>("HolderFill0.5");

            //TopEffect = Content.Load<Texture2D>("BackgroundEffect" + (((float)actmode.Width / (float)actmode.Height == 1.3f)?"@1-3":""));



            //radiance = Content.Load<Texture2D>("radiance");
            //PauseSplash = Content.Load<Texture2D>("pausesplash");
            //PerfectTx = Content.Load<Texture2D>("Perfect");
            //ExcellentTx = Content.Load<Texture2D>("Excellent");
            //GoodTx = Content.Load<Texture2D>("Good");
            //MissTx = Content.Load<Texture2D>("Miss");
            //FailSplash = Content.Load<Texture2D>("failsplash");
            //Push = Content.Load<Texture2D>("push");
            //Hold = Content.Load<Texture2D>("hold");
            //Logo = Content.Load<Texture2D>("logo");
            //AutoModeButton = Content.Load<Texture2D>("autoBtn");
            //AutoModeButtonSel = Content.Load<Texture2D>("autoBtnSel");
            //SpaceSkip = Content.Load<Texture2D>("SpaceSkip");

            //using (var fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\Assets\bg.png", FileMode.Open, FileAccess.Read))
            //{
            //    DefaultBackground = Texture2D.FromStream(GraphicsDevice, fs);
            //}

            ////test
            //StartButton = Content.Load<Texture2D>("PlayMain");
            //ExitButton = Content.Load<Texture2D>("ExitMain");
            //ConfigButton = Content.Load<Texture2D>("ConfigMain");

            //defaultFont = Content.Load<SpriteFont>("SpriteFont1");
            //GeneralBig = Content.Load<SpriteFont>("General");
            //ListboxFont = Content.Load<SpriteFont>("Listbox");
            //SettingsFont = Content.Load<SpriteFont>("SettingsDisplayFont");
            //TitleFont = Content.Load<SpriteFont>("TitleFont");

            //Textures.Add(StartButton);
            //Textures.Add(ExitButton);
            //Textures.Add(ConfigButton);

            //Textures.Add(buttonDefault);
            //Textures.Add(buttonHolder);
            //Textures.Add(waitDefault);
            //Textures.Add(HolderFillDeff);
            //Textures.Add(buttonDefault_0);
            //Textures.Add(buttonHolder_0);
            //Textures.Add(waitDefault_0);
            //Textures.Add(HolderFillDeff_0);
            //Textures.Add(radiance);
            //Textures.Add(PauseSplash);
            //Textures.Add(PerfectTx);
            //Textures.Add(ExcellentTx);
            //Textures.Add(GoodTx);
            //Textures.Add(MissTx);
            //Textures.Add(FailSplash);
            //Textures.Add(Push);
            //Textures.Add(Hold);
            //Textures.Add(Logo);
            //Textures.Add(AutoModeButton);
            //Textures.Add(AutoModeButtonSel);
            //Textures.Add(buttonDefault);
            //Textures.Add(SpaceSkip);


            //HolderFilling = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\HolderFilling.wav");
            //HitButton = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\HitButton.wav");
            //HitHolder = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\HitHolder.wav");
            //HolderTick = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\HolderTick.wav");
            //ComboBreak = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\ComboBreak.wav");
            //ButtonOver = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\ButtonOver.wav");
            //ButtonHit = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\ButtonHit.wav");
            //SelectorHit = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\SelectorHit.wav");
            //ScrollHit = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\Scroll.wav");
            //WelcomeToOsuXd = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\welcome.mp3");
            //SeeyaOsu = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\Effects\\seeya.mp3");


            //SoundsEff.Add(HolderFilling);
            //SoundsEff.Add(HitButton);
            //SoundsEff.Add(HitHolder);
            //SoundsEff.Add(HolderTick);
            //SoundsEff.Add(ComboBreak);
            //SoundsEff.Add(ButtonOver);
            //SoundsEff.Add(ButtonHit);
            //SoundsEff.Add(SelectorHit);
            //SoundsEff.Add(ScrollHit);

            SpritesContent.Instance.LoadContent(SpriteBatch, GraphicsDevice);

            touchHandler = new TouchHandler(System.Windows.Forms.Control.FromHandle(Window.Handle));
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
            try
            {
                ((ScreenBase)BeatmapScreen.Instance).Dispose();
            }
            catch
            {

            }
            finally
            {
                BeatmapScreen.Instance = null;
            }
            */

            //AudioPlaybackEngine.Instance.Dispose();


            //AllBeatmaps.Clear();

            //AllBeatmaps = null;

            SoundsEff.ForEach(new Action<CachedSound>((cnd) =>
            {
                cnd.Dispose();
            }));

            SoundsEff.Clear();



            Textures.ForEach(new Action<Texture2D>((tx) =>
            {
                tx.Dispose();
            }));

            Textures.Clear();

            Player?.WaveOut?.Dispose();

            

            Logger.Instance.Info("Bye!");
        }

        #endregion

        #region GameUpdates
        protected override void Update(GameTime gameTime)
        {

            touchHandler?.Update();

            System.Windows.Forms.Form FormGame = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Window.Handle);

            if (Player.PlayState == NAudio.Wave.PlaybackState.Playing)
            {
               
                if(SelectedBeatmap != null)
                {
                    FormGame.Text = "ubeat - Playing: " + SelectedBeatmap.Artist + " - " + SelectedBeatmap.Title;
                }
                
            }
            else
            {
                FormGame.Text = "ubeat";
            }

            //Update Gametime FIRST
            GameTimeP = gameTime;

            if (Keyboard.GetState().IsKeyDown(Keys.Add))
                GeneralVolume = GeneralVolume + (gameTime.ElapsedGameTime.Milliseconds)*.0005f;

            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
                GeneralVolume = GeneralVolume -(gameTime.ElapsedGameTime.Milliseconds) * .0005f; 

            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyBoardManager.Update(gameTime);
            ScreenManager.Update(gameTime);


            base.Update(gameTime);
            frameCounter.Update(gameTime);

            
            

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, DepthStencilState.None, RasterizerState.CullCounterClockwise);



            ScreenManager.Render();


            touchHandler?.Render();
            //spriteBatch.Draw(TopEffect, new Rectangle(0, 0, TopEffect.Width, TopEffect.Height), Color.White*.145f);


            SpriteBatch.End();

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

            this.Graphics.PreferredBackBufferWidth = (int)wSize.X;
            this.Graphics.PreferredBackBufferHeight = (int)wSize.Y;


            this.Graphics.ApplyChanges();

        }

        public void ToggleFullscreen(bool enabled = false)
        {
            this.Graphics.IsFullScreen = enabled;
            this.Graphics.ApplyChanges();

            System.Windows.Forms.Form FormGame = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
            if (enabled)
            {
                FormGame.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                FormGame.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }
            else
            {
                Screen.ScreenMode modd = Screen.ScreenModeManager.GetActualMode();

                if (modd.WindowMode == Screen.WindowDisposition.Windowed)
                {
                    FormGame.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                    FormGame.WindowState = System.Windows.Forms.FormWindowState.Normal;
                }
                else
                {
                    FormGame.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    FormGame.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                }
            }

            
        }



        public void ChangeFrameRate(float fps)
        {
          
               
            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / fps);
  
        }

        public VolumeDlg VolDlg { get; set; }
    }
}
