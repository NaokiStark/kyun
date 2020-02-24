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
using kyun.game.GameScreen;
using System.Threading.Tasks;
using kyun.Notifications;
using kyun.game.Winforms;
using kyun.game.Database;
using System.Threading;
using kyun.Screen;

namespace kyun
{
    public class KyunGame : Game, IDisposable
    {
        //Puto

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public BPlayer Player;
        public static KyunGame Instance = null;
        public KeyboardManager KeyBoardManager;
        private IKeyboardMouseEvents m_GlobalHook;
        public IntPtr windHandle;
        public static System.Windows.Forms.Form WinForm = null;

        static DateTime updated = DateTime.Now;

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
        public float maxPeak { get; set; }
        public NikuClientApi server { get; set; }
        public bool isMainWindowActive = true;

        public static int Attemps = 0;

        public static int AttempsLag = 0;
        public static double timeToClear = 0;
        public static long lastSongPos = 0;

        float peak = 0;
        float magic = 0;
        float dPeak = 0;

        delegate void shit(GameTime tm);
        event shit updateEvent;
        public System.Threading.SynchronizationContext syncContext;

        bool gameIsRunning;
        private TimeSpan timeStart;

        public DateTime gameStart { get; private set; }

        private Thread tk;
        public bool stopping;
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

            syncContext = System.Threading.SynchronizationContext.Current;

            if (Settings1.Default.Token != "" && Settings1.Default.User != "")
                server = new NikuClientApi(Settings1.Default.User, Settings1.Default.Token);
            else
                Logger.Instance.Debug("[NikuServer] Offline");

            xmas = (DateTime.Now.Month == 1 && (DateTime.Now.Day >= 1 && DateTime.Now.Day <= 3) || DateTime.Now.Month == 12 && (DateTime.Now.Day >= 8 && DateTime.Now.Day <= 31)) ? true : false;

            Logger.Instance.Debug("Loading database");
            var db = DatabaseInterface.Instance;

            WinForm = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Window.Handle);
            WinForm?.Focus();
            windHandle = WinForm.Handle;

            System.Drawing.Bitmap bmcursor = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/cursor.png"));

            WinForm.Cursor = CreateCursor(bmcursor, bmcursor.Width / 2, bmcursor.Height / 2);

            RunningOverWine = false;
            if (Process.GetProcessesByName("winlogon").Count() < 1)
                RunningOverWine = true; //A cup of wine

            DateTime lastComp = File.GetLastWriteTime(Path.Combine(Environment.CurrentDirectory, "kyun.game.dll"));

            CompilationVersion = CompilationStatus + " | " + lastComp.ToString("ddMMyy");

            frameCounter = new FrameCounter();
            VideoCounter = new FrameCounter();
            Instance = this;
            Graphics = new GraphicsDeviceManager(this);

            if (Settings1.Default.Shaders)
            {
                if (GraphicsAdapter.DefaultAdapter.IsProfileSupported(GraphicsProfile.HiDef))
                    Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            }


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

            if (false)
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

            wSize.X = srcm[Settings1.Default.ScreenMode].ScaledWidth;
            wSize.Y = srcm[Settings1.Default.ScreenMode].ScaledHeight;

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

            stopwatch = new Stopwatch();
            stopwatch.Start();

            timeStart = stopwatch.Elapsed;

            gameStart =  DateTime.Now;
            updateEvent = cUpdate;


            bool disableMultiThread = false;
            if(!disableMultiThread)
                RecallAndNotify(null);
        }

        public void RecallAndNotify(Exception ex)
        {
            if (ex != null)
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
            }

           
            updateEvent = cUpdate;

            gameIsRunning = true;

            bool disableMulticore = false;

            tk = new Thread(() =>
            {
                int thisThreadId = GetCurrentThreadId();

                ProcessThread CurrentThread = (from ProcessThread th in Process.GetCurrentProcess().Threads
                                               where th.Id == thisThreadId
                                               select th).Single();
                // Run on another core as possible
                if (Environment.ProcessorCount > 1 && !disableMulticore)
                {
                    Thread.BeginThreadAffinity();

                   
                    CurrentThread.ProcessorAffinity = (IntPtr)0x0002;


                }
                // rise thread priority
                CurrentThread.PriorityLevel = ThreadPriorityLevel.TimeCritical;

                DateTime startTime;
                startTime = DateTime.Now;
                while (gameIsRunning)
                {
                    try
                    {
                        elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                        startTime = DateTime.Now;
                        TimeSpan startedGT = stopwatch.Elapsed - timeStart;
                        //cUpdate(new GameTime(startedGT, TimeSpan.FromMilliseconds(elapsed), false));
                        syncContext.Send(state =>
                        {
                           cUpdate(new GameTime(startedGT, TimeSpan.FromMilliseconds(elapsed), false));
                        }, null);

                        if (Settings1.Default.VSync)
                        {
                            //Wait for sync
                            Thread.Sleep(1); //much as possible
                        }
                        else
                        {
                            System.Threading.Thread.Sleep((int)(750f / Settings1.Default.FrameRate));
                        }
                        
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
                stopped = true; //Avoid thread abortion ex
                if (Environment.ProcessorCount > 1)
                {
                    Thread.EndThreadAffinity();
                }
            });

            gameIsRunning = true;
            tk.Start();
            
            Attemps++;
        }

        protected override void Update(GameTime tm)
        {
            
            //Double
            //cUpdate(tm);
        }

        private void FormGame_DragLeave(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// This isn't work
        /// </summary>
        public void StopAll()
        {
           // UnloadContent();
            tk.Abort();
            //while (!stopped) { } //Wait for thread end

            SpritesContent.instance = null; //this is a buggy thing
            InstanceManager.Instance.IsRunning = true;
            

            Exit();
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
            syncContext.Send(state =>
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
            }, null);
          
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
            uLabelVer.Position = new Vector2(acmode.ScaledWidth / 2, acmode.ScaledHeight - 55);

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
            uLabelMsgVer.Position = new Vector2(acmode.ScaledWidth / 2, acmode.ScaledHeight - 25);
            uLabelMsgVer.Font = SpritesContent.Instance.StandardButtonsFont;
            uLabelMsgVer.Scale = .75f;
            uLabelMsgVer.ForegroundColor = Color.WhiteSmoke * .75f;

            imageTesting = new Image(SpritesContent.instance.TestingBar)
            {
                Position = Vector2.Zero,
                Repeat = true, 
                BeatReact = false,
                Opacity = .8f
            };


            FrameDisplay = new Label()
            {
                Font = SpritesContent.Instance.GeneralBig,
                Position = new Vector2(0, acmode.Height - SpritesContent.Instance.GeneralBig.MeasureString("123").Y - 10),
                Text = ""
            };

            Cursor = new Image(SpritesContent.Instance.GameCursor)
            {
                Position = new Vector2(MouseHandler.GetState().X - (SpritesContent.Instance.GameCursor.Width / 2), MouseHandler.GetState().Y - (SpritesContent.Instance.GameCursor.Height / 2)),
                BeatReact = false
            };

            Notifications = new Notifier();
            EffectsPlayer.StartEngine();//Clear
            Player.OnStopped += () =>
            {
                maxPeak = 0.1f;
                InstanceManager.MaxPeak = 0.1f;
            };

#if !UITEST
            ScreenManager.ChangeTo(new LogoScreen());

#else
            ScreenManager.ChangeTo(LoadScreen.Instance);
#endif


        }

        #region TEXTURES

        public TouchHandler touchHandler;
        private Label uLabelVer;
        private Label uLabelMsgVer;
        private Image imageTesting;
        private Label FrameDisplay;

        public Image Cursor { get; private set; }
    

        private RenderTarget2D renderTarget2D;
        public bool stopped;
        private DateTime lastCheckedDay;

        private Stopwatch stopwatch;
        public KeyboardState KeyboardOldState;
        public KeyboardState KeyboardActualState;

        protected override void LoadContent()
        {
            Screen.ScreenMode actmode = Screen.ScreenModeManager.GetActualMode();

            renderTarget2D = new RenderTarget2D(GraphicsDevice, actmode.Width, actmode.Height);



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
            updated = DateTime.Now;
            //IsMouseVisible = false;

            //Cursor.Position = new Vector2(MouseHandler.GetState().X - (SpritesContent.Instance.GameCursor.Width / 2), MouseHandler.GetState().Y - (SpritesContent.Instance.GameCursor.Height / 2));
            //Cursor.Update();
            checkHaxOrLag(gameTime);

            //Update Gametime FIRST
            GameTimeP = gameTime;
            frameCounter.Update(gameTime);

            touchHandler?.Update();
            updatePeak(gameTime);

            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

           
            ScreenManager.Update(gameTime);

            uLabelVer.Update();
            uLabelMsgVer.Update();
            imageTesting.Update();

            //base.Update(gameTime);

            string vAv = VideoCounter.AverageFramesPerSecond.ToString("000", CultureInfo.InvariantCulture);
            string lAv = frameCounter.AverageFramesPerSecond.ToString("000", CultureInfo.InvariantCulture);

            FrameDisplay.Text = $"{lAv} UPS | {vAv} FPS | {(1000 / frameCounter.AverageFramesPerSecond).ToString("000", CultureInfo.InvariantCulture)}ms";
            FrameDisplay.ForegroundColor = Color.White;
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
            if (!gameIsRunning)
                return;


            KeyboardActualState = Keyboard.GetState();
            if (ScreenManager.ActualScreen != null)
            {
                (ScreenManager.ActualScreen as ScreenBase).checkKeyboardEvents(KeyboardOldState, KeyboardActualState);
            }
            KeyboardOldState = KeyboardActualState;
            

            lastCheckedDay = DateTime.Now;

            isMainWindowActive = System.Windows.Forms.Form.ActiveForm == WinForm;
            KeyBoardManager.Update(gameTime);
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


            //Help time hack
            //updated == lastCheckedDay
            //if (/*updated + TimeSpan.FromMilliseconds(100) < DateTime.Now || */updated.Millisecond == 0)
            //{
            //    cUpdate(gameTime);
            //    FrameDisplay.Text = $"{FrameDisplay.Text} :: USING RENDER LOOP FOR UPDATE!";
            //    FrameDisplay.ForegroundColor = Color.IndianRed;
            //}

            VideoCounter.Update(gameTime);

            GraphicsDevice.SetRenderTarget(renderTarget2D);

            GraphicsDevice.Clear(Color.Black);

            if (Settings1.Default.Shaders)
            {
                if (ScreenManager.ActualBackground != null)
                {
                    SpritesContent.Instance.RGBShiftEffect.Parameters["xColoredTexture"].SetValue(ScreenManager.ActualBackground);

                    if (peak > 0)
                    {
                        peak = (float)Math.Max(peak - Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.001f, 0);
                    }
                    else if (peak < 0)
                    {
                        peak = (float)Math.Min(peak + Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.001f, 0);
                    }


                    if (peak == 0 || float.IsNaN(peak))
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

                SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);
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
            else
            {
                if (ScreenManager.ActualBackground != null)
                {
                    SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);
                    if (ScreenManager.ActualScreen != null)
                    {
                        ((ScreenBase)ScreenManager.ActualScreen)?.RenderBg();
                    }
                    else if (ScreenManager.ToBeChanged != null)
                    {
                        ((ScreenBase)ScreenManager.ToBeChanged)?.RenderBg();
                    }
                    SpriteBatch.End();
                }
            }


            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);


            ScreenManager.Render();

#if !BETA
            uLabelMsgVer.Render();
#endif
            touchHandler?.Render();

            FrameDisplay?.Render();

            Notifications?.Render();


#if TESTING || UITEST
            imageTesting.Render();
#endif
            VolumeControl.Instance.Render();



            SpriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);


            var actualMode = Screen.ScreenModeManager.GetActualMode();

            SpriteBatch.Draw(renderTarget2D, new Rectangle(0, 0, actualMode.ScaledWidth, actualMode.ScaledHeight), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

            MouseEvent mouseState = MouseHandler.GetStateNonScaled();
            Cursor.Scale = .8f;

            Cursor.Position = new Vector2(mouseState.X - Cursor.Size.X / 2, mouseState.Y - Cursor.Size.Y / 2);
            Cursor.Update();
            Cursor.Render();


            SpriteBatch.End();
            base.Draw(gameTime);
        }

#endregion

        public void ChangeResolution(Screen.ScreenMode screenMode)
        {
            SuppressDraw();

            ScreenModeManager.Change();

                                   
            renderTarget2D = new RenderTarget2D(GraphicsDevice, screenMode.Width, screenMode.Height);

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

            this.wSize.X = screenMode.ScaledWidth;
            this.wSize.Y = screenMode.ScaledHeight;

            this.Graphics.PreferredBackBufferWidth = (int)wSize.X;
            this.Graphics.PreferredBackBufferHeight = (int)wSize.Y;

            
            Screen.ScreenModeManager.Change();
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
            MainScreen.instance = null;
            BeatmapScreen.instance = null;
            SettingsScreen.instance = null;
            ClassicModeScreen.SetInstance(null);
            ScorePanel.Instance = null;
            QuestionOverlay.Instance = null;
            PauseOverlay.Instance = null;

            GC.Collect();


            this.Graphics.ApplyChanges();
            
            ScreenManager.ChangeTo(SettingsScreen.Instance); //return 
            FrameDisplay.Position = new Vector2(0, screenMode.Height - SpritesContent.Instance.GeneralBig.MeasureString("123").Y);
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
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / fps);
            //TargetElapsedTime = TimeSpan.FromTicks(5000);
        }

        public new void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            stopped = true;
            // Suppress finalization.
            GC.SuppressFinalize(this);
            
        }

        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        public static System.Windows.Forms.Cursor CreateCursor(System.Drawing.Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            IntPtr ptr = bmp.GetHicon();
            IconInfo tmp = new IconInfo();
            GetIconInfo(ptr, ref tmp);
            tmp.xHotspot = xHotSpot;
            tmp.yHotspot = yHotSpot;
            tmp.fIcon = false;
            ptr = CreateIconIndirect(ref tmp);
            return new System.Windows.Forms.Cursor(ptr);
        }

    }
}
