﻿using System;
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
        public float elapsed = 0;
        public bool VideoEnabled { get; set; }
        public Vector2 wSize = new Vector2(800, 600);
        public GameTime GameTimeP { get; set; }
        private float _vol = Settings1.Default.Volume;
        public FrameCounter frameCounter;
        public static bool RunningOverWine;
        public static bool xmas = false;
        public float maxPeak { get; private set; }
        public NikuClientApi server { get; set; }

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

        public KyunGame(bool softwareRendering = false)
        {

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
            Instance = this;
            Graphics = new GraphicsDeviceManager(this);

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

            if (RunningOverWine)
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

            System.Windows.Forms.Form FormGame = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Window.Handle);

            if (srcm[Settings1.Default.ScreenMode].WindowMode != Screen.WindowDisposition.Windowed)
            {
                FormGame.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                FormGame.WindowState = System.Windows.Forms.FormWindowState.Maximized;
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
            /*
            if (!true)
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
                Settings1.Default.Video = !false;
                Settings1.Default.QuestionVideo = true;
                Settings1.Default.Save();
            }*/

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

            uLabelMsgVer.Centered = true;
            uLabelMsgVer.Position = new Vector2(acmode.Width / 2, acmode.Height - 25);
            uLabelMsgVer.Font = SpritesContent.Instance.StandardButtonsFont;
            uLabelMsgVer.Scale = .75f;
            uLabelMsgVer.ForegroundColor = Color.WhiteSmoke * .75f;

            FrameDisplay = new Label()
            {
                Font = SpritesContent.Instance.GeneralBig,
                Position = new Vector2(0, acmode.Height - SpritesContent.Instance.GeneralBig.MeasureString("123").Y - 10),
                Text = ""
            };

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

            touchHandler = new TouchHandler(System.Windows.Forms.Control.FromHandle(Window.Handle));
            Logger.Instance.Info("");
            Logger.Instance.Info("Done.");
            Logger.Instance.Info("");
            Logger.Instance.Info("----------------");

            discordHandler.SetState("Awesome!", "Loading...");
        }


        protected override void UnloadContent()
        {
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
            }

        }

        #region GameUpdates
        protected override void Update(GameTime gameTime)
        {
            //Update Gametime FIRST
            GameTimeP = gameTime;

            touchHandler?.Update();
            updatePeak(gameTime);

            System.Windows.Forms.Form FormGame = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Window.Handle);

            if (Player.PlayState == BassPlayState.Playing)
            {

                if (SelectedBeatmap != null)
                {
                    FormGame.Text = "kyun! - Playing: " + SelectedBeatmap.Artist + " - " + SelectedBeatmap.Title;
                }

            }
            else
            {
                FormGame.Text = "kyun!";
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
            frameCounter.Update(gameTime);

            FrameDisplay.Text = frameCounter.AverageFramesPerSecond.ToString("0", CultureInfo.InvariantCulture) + " FPS";
            FrameDisplay?.Update();

            if (System.Windows.Forms.Form.ActiveForm == FormGame)
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
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            ScreenManager.Render();

#if !BETA
            uLabelMsgVer.Render();
#endif
            touchHandler?.Render();

            FrameDisplay?.Render();

            VolumeControl.Instance.Render();

            SpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        public void ChangeResolution(Screen.ScreenMode screenMode)
        {
            SuppressDraw();

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
            FrameDisplay.Position = new Vector2(0, wSize.Y - SpritesContent.Instance.GeneralBig.MeasureString("123").Y - 10);
            uLabelMsgVer.Position = new Vector2(wSize.X / 2, wSize.Y - 25);
        }

        public static void SetSoftwareRendering()
        {
            if (Settings1.Default.WindowsRender)
            {
                GraphicsAdapter.UseReferenceDevice = true; //A REALLY BAD IDEA
            }
        }

        public void ToggleFullscreen(bool enabled = false)
        {
            this.Graphics.IsFullScreen = enabled;
            this.Graphics.ApplyChanges();

            System.Windows.Forms.Form FormGame = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Window.Handle);
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
    }
}
