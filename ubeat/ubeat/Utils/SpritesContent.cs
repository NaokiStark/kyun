using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ubeat.Audio;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace ubeat.Utils
{
    public class SpritesContent
    {

        #region Local Vars

        private static SpritesContent instance;

        const string ASSETS_PATH = "Assets";
        private string _fullpath;


        private ContentManager Content;
        private Screen.ScreenMode actualMode;

        #endregion

        #region Singleton
        
        public static SpritesContent Instance
        {
            get
            {
                if (instance == null)
                    instance = new SpritesContent();

                return instance;
            }
        }

        #endregion

        #region Constructor

        public SpritesContent()
        {
            _fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ASSETS_PATH);
            Content = UbeatGame.Instance.Content;
            actualMode = Screen.ScreenModeManager.GetActualMode();

        }

        #endregion

        #region Public Methods

        public void LoadContent(SpriteBatch spriteBatch, GraphicsDevice device)
        {
            //Sprites

            Logger.Instance.Info("Loading Sprites");

            LoadSprites(spriteBatch, device);

            //Fonts

            Logger.Instance.Info("Loading Fonts");

            LoadFonts(spriteBatch);

            //Audios

            Logger.Instance.Info("Loading Audios");

            LoadAudios();

        }

        #endregion

        #region LocalFunctions

        private string getPath(string asset)
        {
            return Path.Combine(_fullpath, asset);
        }

        private CachedSound loadSound(string asset)
        {
            return new CachedSound(getPath(asset));
        }
        #endregion

        #region Loader

        private void LoadAudios()
        {
            HolderFilling = loadSound("Effects\\HolderFilling.wav");
            HitButton = loadSound("Effects\\HitButton.wav");
            HitHolder = loadSound("Effects\\HitHolder.wav");
            HolderTick = loadSound("Effects\\HolderTick.wav");
            ComboBreak = loadSound("Effects\\ComboBreak.wav");
            ButtonOver = loadSound("Effects\\ButtonOver.wav");
            ButtonHit = loadSound("Effects\\ButtonHit.wav");
            SelectorHit = loadSound("Effects\\SelectorHit.wav");
            ScrollHit = loadSound("Effects\\Scroll.wav");
            WelcomeToOsuXd = loadSound("Effects\\welcome.mp3");
            SeeyaOsu = loadSound("Effects\\seeya.mp3");
            OsuHit = loadSound("Effects\\OsuModeHit.wav");
        }

        private void LoadFonts(SpriteBatch spriteBatch)
        {
            
            DefaultFont = Content.Load<SpriteFont>("SpriteFont1");
            GeneralBig = Content.Load<SpriteFont>("General");
            ListboxFont = Content.Load<SpriteFont>("Listbox");
            SettingsFont = Content.Load<SpriteFont>("SettingsDisplayFont");
            TitleFont = Content.Load<SpriteFont>("TitleFont");
            StandardButtonsFont = Content.Load<SpriteFont>("StandardButtonFont");
        }

        private void LoadSprites(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            LoadingSpinnerTx = Content.Load<Texture2D>("loadingSpin");
            ButtonDefault = Content.Load<Texture2D>("button_0");
            ButtonHolder = Content.Load<Texture2D>("holder_0");
            WaitDefault = Content.Load<Texture2D>("approachv2");
            HolderFillDeff = Content.Load<Texture2D>("HolderFill");


            ButtonDefault_0 = Content.Load<Texture2D>("button_0.5");
            ButtonHolder_0 = Content.Load<Texture2D>("holder_0.5");
            WaitDefault_0 = Content.Load<Texture2D>("approach0.5");
            HolderFillDeff_0 = Content.Load<Texture2D>("HolderFill0.5");

            TopEffect = Content.Load<Texture2D>("BackgroundEffect" + (((float)actualMode.Width / (float)actualMode.Height == 1.3f) ? "@1-3" : ""));

            Radiance = Content.Load<Texture2D>("radiance");
            PauseSplash = Content.Load<Texture2D>("pausesplash");
            PerfectTx = Content.Load<Texture2D>("Perfect");
            ExcellentTx = Content.Load<Texture2D>("Excellent");
            GoodTx = Content.Load<Texture2D>("Good");
            MissTx = Content.Load<Texture2D>("Miss");
            FailSplash = Content.Load<Texture2D>("failsplash");
            Push = Content.Load<Texture2D>("push");
            Hold = Content.Load<Texture2D>("hold");
            Logo = Content.Load<Texture2D>("logo");
            AutoModeButton = Content.Load<Texture2D>("autoBtn");
            AutoModeButtonSel = Content.Load<Texture2D>("autoBtnSel");
            SpaceSkip = Content.Load<Texture2D>("SpaceSkip");

            using (var fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\Assets\bg.jpg", FileMode.Open, FileAccess.Read))
            {
                DefaultBackground = Texture2D.FromStream(graphics, fs);
            }

            //test
            StartButton = Content.Load<Texture2D>("PlayMain");
            ExitButton = Content.Load<Texture2D>("ExitMain");
            ConfigButton = Content.Load<Texture2D>("ConfigMain");
            ButtonStandard = Content.Load<Texture2D>("ButtonStandard");

        }

        #endregion

        #region SpritesVars
        public Texture2D ButtonDefault { get; set; }
        public Texture2D ButtonHolder { get; set; }
        public Texture2D ButtonDefault_0 { get; set; }
        public Texture2D ButtonHolder_0 { get; set; }
        public Texture2D WaitDefault { get; set; }
        public Texture2D WaitDefault_0 { get; set; }
        public Texture2D HolderFillDeff { get; set; }
        public Texture2D HolderFillDeff_0 { get; set; }
        public Texture2D Radiance { get; set; }
        public Texture2D PauseSplash { get; set; }
        public Texture2D PerfectTx { get; set; }
        public Texture2D ExcellentTx { get; set; }
        public Texture2D GoodTx { get; set; }
        public Texture2D MissTx { get; set; }
        public Texture2D FailSplash { get; set; }
        public Texture2D Push { get; set; }
        public Texture2D Hold { get; set; }
        public Texture2D StartButton { get; set; }
        public Texture2D ExitButton { get; set; }
        public Texture2D ConfigButton { get; set; }
        public Texture2D AutoModeButton { get; set; }
        public Texture2D AutoModeButtonSel { get; set; }
        public Texture2D Logo { get; set; }
        public Texture2D SpaceSkip { get; set; }
        public Texture2D TopEffect { get; set; }
        public Texture2D DefaultBackground { get; set; }
        public Texture2D LoadingSpinnerTx { get; set; }
        public Texture2D ButtonStandard { get; set; }
        #endregion

        #region FontsVars
        public SpriteFont DefaultFont { get; set; }
        public SpriteFont GeneralBig { get; set; }
        public SpriteFont ListboxFont { get; set; }
        public SpriteFont SettingsFont { get; set; }
        public SpriteFont TitleFont { get; set; }
        public SpriteFont StandardButtonsFont { get; set; }
        #endregion

        #region AudiosVars
        public CachedSound ButtonHit { get; set; }
        public CachedSound ComboBreak { get; set; }
        public CachedSound ButtonOver { get; set; }
        public CachedSound HitButton { get; set; }
        public CachedSound HitHolder { get; set; }
        public CachedSound HolderFilling { get; set; }
        public CachedSound HolderTick { get; set; }
        public CachedSound SelectorHit { get; set; }
        public CachedSound ScrollHit { get; set; }
        public CachedSound SeeyaOsu { get; set; }
        public CachedSound OsuHit { get; set; }
        public CachedSound WelcomeToOsuXd { get; set; }
        #endregion


    }
}
