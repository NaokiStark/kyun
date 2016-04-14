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
        public Player player;
        public static Game1 Instance = null;
        //Beatmaps
        public SoundEffect soundEffect;
        public List<Beatmap.ubeatBeatMap> Beatmaps = new List<Beatmap.ubeatBeatMap>();
        public Game1()
        {
            //AllocConsole();
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 100.0f);

            //this.IsFixedTimeStep = false;
            //graphics.SynchronizeWithVerticalRetrace = false;

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

            base.Initialize();
            player = new Player();

            //Loads Beatmaps
            Logger.Instance.Info("");
            Logger.Instance.Info("----------------");
            Logger.Instance.Info("");
            Logger.Instance.Info("Loading beatmaps.");
            AllBeatmaps = new List<Beatmap.Mapset>();

            DirectoryInfo osuDirPath = new DirectoryInfo(@"C:\Program Files (x86)\osu!\Songs");
            DirectoryInfo[] osuMapsDirs = osuDirPath.GetDirectories();
            int flieCnt = 0;
            foreach (DirectoryInfo odir in osuMapsDirs)
            {
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
                        
                        Debug.WriteLine("File: {0}s",flieCnt);
                    }
                   
                }
                if (bmms != null)
                {
                    AllBeatmaps.Add(bmms);
                }
            }
            Logger.Instance.Info("");
            Logger.Instance.Info("Done.");
            Logger.Instance.Info("");
            Logger.Instance.Info("----------------");
            Logger.Instance.Info("");

            //Loads main Screen
            mainWindow = new MainWindow();
            mainWindow.FormClosed += mainWindow_FormClosed;
            
            hideGameWindow();
        }

        void mainWindow_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if(player.soundOut!=null)
                player.soundOut.Dispose();
            this.Exit();
        }

        void hideGameWindow()
        {            

            //Temp hidding
            this.graphics.PreferredBackBufferWidth = this.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            this.graphics.PreferredBackBufferHeight = this.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            this.graphics.ApplyChanges();

            System.Windows.Forms.Form MyGameForm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);

            MyGameForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            MyGameForm.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            MyGameForm.ShowInTaskbar = false;
            
            //Show main 
            showMain();
            //ToDo: add setting

            mainWindow.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        }
        public float elapsed = 0;
        void showMain()
        {
            mainWindow.Opacity = 0;
            mainWindow.Show();
            mainWindow.Focus();
            int duration = 101;//in milliseconds
            int steps = 100;
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = duration / steps;

            int currentStep = 0;
            timer.Tick += (arg1, arg2) =>
            {
                mainWindow.Opacity = ((double)currentStep) / steps;
                currentStep++;

                if (currentStep >= steps)
                {
                    timer.Stop();
                    timer.Dispose();
                }
            };

            timer.Start();
        }
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

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            buttonDefault = Content.Load<Texture2D>("button");
            buttonHolder = Content.Load<Texture2D>("holder");
            waitDefault = Content.Load<Texture2D>("wait");
            HolderFillDeff = Content.Load<Texture2D>("HolderFill");
            radiance = Content.Load<Texture2D>("radiance");
            PauseSplash = Content.Load<Texture2D>("pausesplash");
            PerfectTx = Content.Load<Texture2D>("Perfect");
            ExcellentTx= Content.Load<Texture2D>("Excellent");
            GoodTx = Content.Load<Texture2D>("Good");
            MissTx = Content.Load<Texture2D>("Miss");
            FailSplash = Content.Load<Texture2D>("failsplash");

            fontDefault = Content.Load<SpriteFont>("SpriteFont1");

            soundEffect = Content.Load<SoundEffect>("normal-hitnormal2");
            HitHolderFilling = Content.Load<SoundEffect>("HolderFilling");
            HolderTick = Content.Load<SoundEffect>("HolderTick");
            // TODO: use this.Content to load your game content here
        }

        
        protected override void UnloadContent()
        {
            if (player.soundOut != null)
                player.soundOut.Dispose();

            Logger.Instance.Info(" Bye! ");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (grid != null)
            {
                grid.Update();
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (grid != null)
            {
                grid.Render();
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public Texture2D FailSplash { get; set; }

        //console
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
    }
}
