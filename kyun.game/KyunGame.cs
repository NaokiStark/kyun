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
using kyun.Audio;
using kyun.GameScreen;
using kyun.Utils;
using kyun.GameScreen.UI;
using System.Diagnostics;
using System.Globalization;
using Gma.System.MouseKeyHook;
using kyun.GameModes.Classic;
using kyun.Overlay;
using kyun.game.GameScreen.UI;
using kyun.game;
using kyun.game.Utils;
using kyun.game.NikuClient;
using kyun.game.GameScreen;
using System.Threading.Tasks;
using kyun.Notifications;
using kyun.game.Winforms;

namespace kyun
{
    public class KyunGame : Game
    {
        //Puto

        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public BPlayer Player;
        public static KyunGame Instance = null;
        public KeyboardManager KeyBoardManager;
        private IKeyboardMouseEvents m_GlobalHook;

        public static System.Windows.Forms.Form WinForm = null;


#if !BETA
        public static string CompilationStatus = "Unstable";
#else
        public static string CompilationStatus = "Beta";
#endif

        public static string CompilationVersion = "";

        public event EventHandler OnPeak;
        public Discord_Handler discordHandler;
        public float oVol = 0f;
        public SoundEffect soundEffect;
        public Beatmap.ubeatBeatMap SelectedBeatmap { get; set; }
        public Beatmap.Mapset SelectedMapset { get; set; }
        public double elapsed = 0;
        public bool VideoEnabled { get; set; }
        public Vector2 wSize = new Vector2(800, 600);
        public GameTime GameTimeP { get; set; }
        private float _vol = Settings1.Default.Volume;
        public FrameCounter frameCounter;
        public FrameCounter VideoCounter;
        public static bool RunningOverWine;
        public static bool xmas = false;
        public float maxPeak { get; private set; }
        public NikuClientApi server { get; set; }

        public static int Attemps = 0;

        public static int AttempsLag = 0;
        public static double timeToClear = 0;
        public static long lastSongPos = 0;

        float peak = 0;
        float magic = 0;
        float dPeak = 0;

        delegate void shit(GameTime tm);
        event shit updateEvent;
        private System.Threading.SynchronizationContext syncContext;

        bool gameIsRunning;
        double timeToCrash = 0;

        public Notifier Notifications { get; private set; }

        public static int LauncherVersion = 0; //THIS WILL FILL WITH LAUNCHER (EXECUTABLE)

        public static int DesiredLauncher = 2; //THIS IS A LAUNCHER REQUESTED WITH THIS ACTUAL BUILD (NOT EXECUTABLE)

        public static string MainSite = "https://kyun.mokyu.pw/";

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
                    if (Player != null)
                        Player.Volume = val;

                _vol = val;

                Settings1.Default.Volume = val;
                Settings1.Default.Save();
                VolumeControl.Instance.Show();
            }
        }



        public KyunGame(bool softwareRendering = false, bool repair = false)
        {

            if (repair)
            {
                Settings1.Default.Reset();
                Settings1.Default.Save();
            }           
            

            if (Settings1.Default.Token != "" && Settings1.Default.User != "")
                server = new NikuClientApi(Settings1.Default.User, Settings1.Default.Token);
            else
                Logger.Instance.Debug("[NikuServer] Offline");

            xmas = (DateTime.Now.Month == 1 && (DateTime.Now.Day >= 1 && DateTime.Now.Day <= 3) || DateTime.Now.Month == 12 && (DateTime.Now.Day >= 8 && DateTime.Now.Day <= 31)) ? true : false;


            WinForm = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Window.Handle);
            WinForm?.Focus();
            RunningOverWine = false;
            if (Process.GetProcessesByName("winlogon").Count() < 1)
                RunningOverWine = true; //A cup of wine

            DateTime lastComp = File.GetLastWriteTime(Path.Combine(Environment.CurrentDirectory, "kyun.game.dll"));

            CompilationVersion = CompilationStatus + " | " + lastComp.ToString("ddMMyy");

            frameCounter = new FrameCounter();
            VideoCounter = new FrameCounter();
            Instance = this;
            Graphics = new GraphicsDeviceManager(this);

            if(GraphicsAdapter.DefaultAdapter.IsProfileSupported(GraphicsProfile.HiDef))
                Graphics.GraphicsProfile = GraphicsProfile.HiDef;

            if (softwareRendering)
            {
                Settings1.Default.WindowsRender = true;
                SetSoftwareRendering();
            }

            GameTimeP = new GameTime();

            Content.RootDirectory = "Content";
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f);

            KeyBoardManager = new KeyboardManager(this);
            VideoEnabled = Settings1.Default.Video;

            if (true)
            {
                m_GlobalHook = Hook.AppEvents();
                m_GlobalHook.MouseDown += MouseHandler.setMouseDownStateWinform;
                m_GlobalHook.MouseUp += MouseHandler.setMouseUpStateWinform;
                m_GlobalHook.MouseMove += MouseHandler.SetMousePosWinFrm;
                m_GlobalHook.MouseWheel += MouseHandler.SetMouseWheelPos;
            }

            discordHandler = new Discord_Handler();
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


            if (srcm[Settings1.Default.ScreenMode].WindowMode != Screen.WindowDisposition.Windowed)
            {
                WinForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                WinForm.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }
#if DEBUG
            Logger.Instance.Debug("!!!! GRAPHICS CARD !!!!");
            Logger.Instance.Debug("======================");
            Logger.Instance.Debug("");
            Logger.Instance.Debug(Graphics.GraphicsDevice.Adapter.Description);
            Logger.Instance.Debug(Graphics.GraphicsDevice.Adapter.DeviceName);
            Logger.Instance.Debug(Graphics.GraphicsDevice.Adapter.VendorId.ToString());
            Logger.Instance.Debug((Graphics.GraphicsDevice.Adapter.IsDefaultAdapter) ? "Default adapter: True" : "Default adapter: False");
            Logger.Instance.Debug("");
            Logger.Instance.Debug("======================");

#endif

            Graphics.ApplyChanges();
            WinForm.AllowDrop = true;

            //ToDo: add sprite
            WinForm.DragEnter += FormGame_DragEnter;
            WinForm.DragLeave += FormGame_DragLeave;
            WinForm.DragDrop += FormGame_DragDrop;

            gameIsRunning = true;

            TimeSpan gameStart = DateTime.Now - DateTime.Now;
            updateEvent = cUpdate;

            /*            
            syncContext = System.Threading.SynchronizationContext.Current;
            System.Threading.Thread tr = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                DateTime startTime;
                startTime = DateTime.Now;
                while (gameIsRunning)
                {
                    elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                    startTime = DateTime.Now;
                    syncContext.Send(state =>
                    {
                        updateEvent.Invoke(new GameTime(gameStart, TimeSpan.FromMilliseconds(elapsed), false));
                    }, null);
                    System.Threading.Thread.Sleep(TimeSpan.FromTicks(1000));
                }
            }));
            tr.IsBackground = true;
            tr.Start();*/
            syncContext = System.Threading.SynchronizationContext.Current;

            Task tk = new Task(() =>
            {
                DateTime startTime;
                startTime = DateTime.Now;
                while (gameIsRunning)
                {
                    try
                    {
                        elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                        startTime = DateTime.Now;

                        timeToCrash += elapsed;

                        syncContext.Send(state =>
                        {
                            updateEvent.Invoke(new GameTime(gameStart, TimeSpan.FromMilliseconds(elapsed), false));
                        }, null);

                        System.Threading.Thread.Sleep((int)(750f / Settings1.Default.FrameRate));


                    }
                    catch (Exception pex)
                    {
                        Logger.Instance.Severe($"{pex.Message} \r\n {pex.StackTrace}");
                        gameIsRunning = false;
                        syncContext.Send(state =>
                        {
                            RecallAndNotify(pex);
                        }, null);
                        return;
                    }
                }
            });

            tk.Start();
        }

        public void RecallAndNotify(Exception ex)
        {
            if (Attemps >= 5)
            {
                var frm = new FailForm();
                Player.Stop();
                WinForm.Hide();
                gameIsRunning = false;
                frm.ShowForm(ex);
                return;
            }

            TimeSpan gameStart = DateTime.Now - DateTime.Now;
            updateEvent = cUpdate;
            syncContext = System.Threading.SynchronizationContext.Current;
            gameIsRunning = true;
            try
            {
                Notifications.ShowDialog("Ups, kyun! has experienced an error, this will be notified, sorry about that.", 10000, NotificationType.Critical);
            }
            catch (Exception rex)
            {
                Attemps++;
                RecallAndNotify(rex);
                return;
            }


            Task tk = new Task(() =>
            {
                DateTime startTime;
                startTime = DateTime.Now;
                while (gameIsRunning)
                {
                    try
                    {
                        elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                        startTime = DateTime.Now;

                        syncContext.Send(state =>
                        {
                            updateEvent.Invoke(new GameTime(gameStart, TimeSpan.FromMilliseconds(elapsed), false));
                        }, null);

                        System.Threading.Thread.Sleep((int)(750f / Settings1.Default.FrameRate));
                        //throw new Exception("Test");
                    }
                    catch (Exception pex)
                    {
                        Logger.Instance.Severe($"{pex.Message} \r\n {pex.StackTrace}");
                        gameIsRunning = false;
                        syncContext.Send(state =>
                        {
                            RecallAndNotify(pex);
                        }, null);
                        return;
                    }
                }
            });

            gameIsRunning = true;
            tk.Start();
            Attemps++;
        }

        protected override void Update(GameTime tm)
        {

        }

        private void FormGame_DragLeave(object sender, EventArgs e)
        {
        }

        private void FormGame_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            e.Effect = System.Windows.Forms.DragDropEffects.All;

        }

        private void FormGame_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);
            BeatmapLoader.GetInstance().LoadBeatmaps(files, (ScreenBase)ScreenManager.ActualScreen);
        }

        protected override void Initialize()
        {
            base.Initialize();
            loadEnviroment();
            Player = new BPlayer();
            hideGameWindow();
            GeneralVolume = Settings1.Default.Volume;
        }

        public void ToggleVSync(bool b)
        {
            if (b)
            {
                IsFixedTimeStep = false;
                Graphics.SynchronizeWithVerticalRetrace = b;
            }
            else
            {
                Graphics.SynchronizeWithVerticalRetrace = b;
                IsFixedTimeStep = true;
                ChangeFrameRate(Settings1.Default.FrameRate);
            }

            Graphics.ApplyChanges();
        }

        void mainWindow_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (Player != null)
                Player.Dispose();
            this.Exit();
        }

        void hideGameWindow()
        {
            Screen.ScreenMode acmode = Screen.ScreenModeManager.GetActualMode();
            uLabelVer = new GameScreen.UI.Label(.1f);
            uLabelVer.Text = "ubeat Project alpha";
            uLabelVer.Centered = true;
            uLabelVer.Position = new Vector2(acmode.Width / 2, acmode.Height - 55);

            uLabelVer.Font = SpritesContent.Instance.TitleFont;
            uLabelVer.Scale = .75f;
            uLabelVer.ForegroundColor = Color.WhiteSmoke * .75f;

            uLabelMsgVer = new GameScreen.UI.Label(.1f);
            uLabelMsgVer.Text = CompilationVersion;
            /*
            uLabelMsgVer.Tooltip = new Tooltip
            {
                Text = "Hi",
                BorderColor = Color.Coral
            };
                        uLabelMsgVer.Click += (e, arg) => {
                ScreenManager.ChangeTo(game.GameModes.Test.TestScreen.GetInstance());
            };
             
             */

            uLabelMsgVer.Centered = true;
            uLabelMsgVer.Position = new Vector2(acmode.Width / 2, acmode.Height - 25);
            uLabelMsgVer.Font = SpritesContent.Instance.StandardButtonsFont;
            uLabelMsgVer.Scale = .75f;
            uLabelMsgVer.ForegroundColor = Color.WhiteSmoke * .75f;



            FrameDisplay = new Label()
            {
                Font = SpritesContent.Instance.GeneralBig,
                Position = new Vector2(0, acmode.Height - SpritesContent.Instance.GeneralBig.MeasureString("123").Y * 2 - 10),
                Text = ""
            };

            Notifications = new Notifier();

            ScreenManager.ChangeTo(new LogoScreen());
            EffectsPlayer.StartEngine();//Clear
            Player.OnStopped += () =>
            {
                maxPeak = 0.1f;
                InstanceManager.MaxPeak = 0.1f;
            };

        }

        #region TEXTURES

        public TouchHandler touchHandler;
        private Label uLabelVer;
        private Label uLabelMsgVer;
        private Label FrameDisplay;

        protected override void LoadContent()
        {
            Screen.ScreenMode actmode = Screen.ScreenModeManager.GetActualMode();

            ScreenManager.Start();

            Logger.Instance.Info("Loading textures");

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            SpritesContent.Instance.LoadContent(SpriteBatch, GraphicsDevice);

            touchHandler = new TouchHandler(WinForm);
            Logger.Instance.Info("");
            Logger.Instance.Info("Done.");
            Logger.Instance.Info("");
            Logger.Instance.Info("----------------");

            discordHandler.SetState("Awesome!", "Loading...");
        }


        protected override void UnloadContent()
        {
            gameIsRunning = false;
            if (Player != null)
                Player.Dispose();

            Player?.Dispose();

            Logger.Instance.Info("Shutdown DiscordRPC");
            discordHandler.Shutdown();

            Logger.Instance.Info("Bye!");
        }

        #endregion

        void updatePeak(GameTime gm)
        {
            maxPeak -= (float)(gm.ElapsedGameTime.Milliseconds * 0.0004f);

            float actualPeak = Instance.Player.PeakVol;

            if (actualPeak > 1) actualPeak = 1;

            if (actualPeak > maxPeak)
            {
                if (actualPeak < 0.1) actualPeak = 0.1f;
                maxPeak = actualPeak;

            }
            if (actualPeak > InstanceManager.MaxPeak)
            {
                InstanceManager.MaxPeak = actualPeak;
            }

            if (actualPeak >= InstanceManager.MaxPeak - 0.001)
            {
                OnPeak?.Invoke(this, new EventArgs());
                magic = (float)OsuUtils.OsuBeatMap.rnd.NextDouble(-4d, 4d);
            }

        }


        /// <summary>
        /// This fails
        /// </summary>
        /// <param name="gameTime"></param>
        public void checkHaxOrLag(GameTime gameTime)
        {
            return;

            //Check if time cheating or game is running slow

            long lastPos = Player.Position;

            ///ToDo: Check that
            if (gameTime.ElapsedGameTime.Milliseconds >= 200)
            {
                AttempsLag++;

                if (AttempsLag > 50)
                    throw new Exception("Game is running too slow or lagged.");
            }

            if (Player.PlayState == BassPlayState.Playing)
            {
                long desiredPosition = lastSongPos + gameTime.ElapsedGameTime.Milliseconds;
                if (Math.Abs(desiredPosition - lastPos) >= 100)
                {
                    if (ScreenManager.ActualScreen is GameModes.GameModeScreenBase)
                        AttempsLag++;
                    if (AttempsLag > 15)
                        throw new Exception("Game is running too slow or lagged /Or speedhack/.");
                }
            }

            if (timeToClear > 10000)
                timeToClear = AttempsLag = 0;

            timeToClear++;
            lastSongPos = lastPos;
        }

        #region GameUpdates
        protected void cUpdate(GameTime gameTime)
        {
            checkHaxOrLag(gameTime);

            //Update Gametime FIRST
            GameTimeP = gameTime;
            frameCounter.Update(gameTime);

            touchHandler?.Update();
            updatePeak(gameTime);


            if (Player.PlayState == BassPlayState.Playing)
            {

                if (SelectedBeatmap != null)
                {
                    WinForm.Text = "kyun! - Playing: " + SelectedBeatmap.Artist + " - " + SelectedBeatmap.Title;
                }

            }
            else
            {
                WinForm.Text = "kyun!";
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Add))
                GeneralVolume = GeneralVolume + (gameTime.ElapsedGameTime.Milliseconds) * .0005f;

            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
                GeneralVolume = GeneralVolume - (gameTime.ElapsedGameTime.Milliseconds) * .0005f;

            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyBoardManager.Update(gameTime);
            ScreenManager.Update(gameTime);

            uLabelVer.Update();
            uLabelMsgVer.Update();

            base.Update(gameTime);

            string vAv = VideoCounter.AverageFramesPerSecond.ToString("0", CultureInfo.InvariantCulture);
            string lAv = frameCounter.AverageFramesPerSecond.ToString("0", CultureInfo.InvariantCulture);

            FrameDisplay.Text = $"Logic: {lAv} UPS\nVideo: {vAv} FPS";
            FrameDisplay?.Update();

            Notifications?.Update();
            if (System.Windows.Forms.Form.ActiveForm == WinForm)
            {
                if (oVol != 0f)
                    GeneralVolume = oVol;
                oVol = 0;
            }
            else
            {
                if (oVol == 0f)
                {
                    oVol = GeneralVolume;
                    GeneralVolume = GeneralVolume * 0.50f;
                }
            }

            VolumeControl.Instance.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            VideoCounter.Update(gameTime);

            GraphicsDevice.Clear(Color.Black);

            if(Graphics.GraphicsProfile == GraphicsProfile.HiDef)
            {
                if (ScreenManager.ActualBackground != null)
                {
                    SpritesContent.Instance.RGBShiftEffect.Parameters["xColoredTexture"].SetValue(ScreenManager.ActualBackground);

                    if (peak > 0)
                    {
                        peak = (float)Math.Max(peak - Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.001f, 0);
                    }
                    else if(peak < 0)
                    {
                        peak = (float)Math.Min(peak + Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.001f, 0);
                    }                    


                    if(peak == 0 || float.IsNaN(peak))
                    {

                        if (KyunGame.Instance.Player.PeakVol >= InstanceManager.MaxPeak - 0.11)
                            peak = 2 * (magic / -magic);
                    }

                    dPeak = (float)Math.Max(dPeak - Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.01f, 0);

                    if (KyunGame.Instance.Player.PeakVol >= InstanceManager.MaxPeak - 0.01)
                    {
                        dPeak = KyunGame.Instance.Player.PeakVol;
                    }

                    float distorsion = Math.Max(0, Math.Min(.1f, dPeak / 5));
                    SpritesContent.Instance.RGBShiftEffect.Parameters["DisplacementDist"].SetValue(-distorsion);

                    SpritesContent.Instance.RGBShiftEffect.Parameters["DisplacementScroll"].SetValue((peak / 1000) * magic);
                    SpritesContent.Instance.RGBShiftEffect.CurrentTechnique = SpritesContent.Instance.RGBShiftEffect.Techniques["Technique1"];

                }

                SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
                if (ScreenManager.ActualBackground != null)
                {
                    foreach (EffectPass pass in SpritesContent.Instance.RGBShiftEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                    }
                    if (ScreenManager.ActualScreen != null)
                    {
                        ((ScreenBase)ScreenManager.ActualScreen)?.RenderBg();
                    }
                    else if (ScreenManager.ToBeChanged != null)
                    {
                        ((ScreenBase)ScreenManager.ToBeChanged)?.RenderBg();
                    }

                }

                SpriteBatch.End();
            }
           

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
            

            ScreenManager.Render();

#if !BETA
            uLabelMsgVer.Render();
#endif
            touchHandler?.Render();

            FrameDisplay?.Render();

            Notifications?.Render();

            VolumeControl.Instance.Render();

            SpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        public void ChangeResolution(Screen.ScreenMode screenMode)
        {
            SuppressDraw();


            if (screenMode.WindowMode != Screen.WindowDisposition.Windowed)
            {
                WinForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                WinForm.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }
            else
            {
                WinForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                WinForm.WindowState = System.Windows.Forms.FormWindowState.Normal;

            }

            this.wSize.X = screenMode.Width;
            this.wSize.Y = screenMode.Height;

            this.Graphics.PreferredBackBufferWidth = (int)wSize.X;
            this.Graphics.PreferredBackBufferHeight = (int)wSize.Y;

            this.Graphics.ApplyChanges();

            ((ScreenBase)MainScreen.Instance)?.Dispose();
            ((ScreenBase)BeatmapScreen.Instance)?.Dispose();
            ((ScreenBase)SettingsScreen.Instance)?.Dispose();
            ClassicModeScreen.GetInstance()?.Dispose();
            try
            {
                ((ScreenBase)ScorePanel.Instance)?.Dispose();
                PauseOverlay.Instance.Dispose();
            }
            catch
            {
                ScorePanel.Instance = null;
            }

            ((ScreenBase)QuestionOverlay.Instance)?.Dispose();
            MainScreen.Instance = null;
            BeatmapScreen.Instance = null;
            SettingsScreen.Instance = null;
            ClassicModeScreen.SetInstance(null);
            ScorePanel.Instance = null;
            QuestionOverlay.Instance = null;
            PauseOverlay.Instance = null;

            GC.Collect();

            Screen.ScreenModeManager.Change();

            ScreenManager.ChangeTo(SettingsScreen.Instance); //return 
            FrameDisplay.Position = new Vector2(0, wSize.Y - SpritesContent.Instance.GeneralBig.MeasureString("123").Y * 2 - 10);
            uLabelMsgVer.Position = new Vector2(wSize.X / 2, wSize.Y - 25);
        }

        public static void SetSoftwareRendering()
        {
            return;
            if (Settings1.Default.WindowsRender)
            {
                GraphicsAdapter.UseReferenceDevice = true; //A REALLY BAD IDEA
            }
        }

        public void ToggleFullscreen(bool enabled = false)
        {
            this.Graphics.IsFullScreen = enabled;
            this.Graphics.ApplyChanges();

            if (enabled)
            {
                WinForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                WinForm.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }
            else
            {
                Screen.ScreenMode modd = Screen.ScreenModeManager.GetActualMode();

                if (modd.WindowMode == Screen.WindowDisposition.Windowed)
                {
                    WinForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                    WinForm.WindowState = System.Windows.Forms.FormWindowState.Normal;
                }
                else
                {
                    WinForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    WinForm.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                }
            }
        }

        public void ChangeFrameRate(float fps)
        {
            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / fps);
            //TargetElapsedTime = TimeSpan.FromTicks(5000);
        }

    }
}
