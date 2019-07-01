using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.Audio;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Microsoft.Xna.Framework;
using Un4seen.Bass;
using kyun.game.Utils;
using kyun.game;

namespace kyun.Utils
{
    public class SpritesContent
    {

        #region Local Vars

        public static SpritesContent instance;

        const string ASSETS_PATH = "Assets";
        private string _fullpath;


        private ContentManager Content;
        private Screen.ScreenMode actualMode;

        public SkinManager _SkinManager { get; set; }
        public List<KeyValuePair<string, Texture2D>> TextureList = new List<KeyValuePair<string, Texture2D>>();
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
            Un4seen.Bass.BassNet.Registration("dymmy@dummy.com", "thecode");

#if LINUX
            bool bassLoaded = Bass.LoadMe(AppDomain.CurrentDomain.BaseDirectory + "x86/bass.dll");
            if (!bassLoaded)
            {
                bassLoaded = Bass.LoadMe(AppDomain.CurrentDomain.BaseDirectory + "x86/libbass.so");
                if (!bassLoaded)
                {
                    throw new BadImageFormatException("Lib is not found or invalid");
                }
                    
            }
                
#endif
            Bass.BASS_PluginLoad(AppDomain.CurrentDomain.BaseDirectory + "bass_fx.dll");
            Bass.BASS_Init(1, 44100, BASSInit.BASS_DEVICE_STEREO, IntPtr.Zero);


            _fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ASSETS_PATH);
            Content = KyunGame.Instance.Content;
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

            Logger.Instance.Info("Loading User Skins");

            loadSkins();

        }

        #endregion

        #region LocalFunctions

        private string getPath(string asset)
        {
            return Path.Combine(_fullpath, asset);
        }

        private int LoadSoundBass(string asset)
        {
            //int sample = Bass.BASS_StreamCreateFile(getPath(asset), 0, 0, BASSFlag.BASS_DEFAULT | BASSFlag.BASS_ASYNCFILE);

            byte[] data = null;

            int sample = Bass.BASS_SampleCreate(0, 44100, 2, 8, BASSFlag.BASS_SAMPLE_OVER_POS);
            try
            {
                using (Stream stream = File.OpenRead(getPath(asset)))
                {
                    if (stream != null)
                    {
                        data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                        stream.Close();
                    }

                }

                sample = Bass.BASS_SampleLoad(data, 0, data.Length, 8, BASSFlag.BASS_SAMPLE_OVER_POS);

                if (sample != 0)
                {

                }
                else
                {
                    Logger.Instance.Warn($"Error loading: {asset}");
                }

                return sample;
            }
            catch
            {
                Logger.Instance.Warn($"Error loading: {asset}");
                return 0;
            }
        }
        #endregion

        #region Loader

        private void LoadAudios()
        {
            //HolderFilling = LoadSoundBass("Effects\\HolderFilling.wav");
            //HitButton = LoadSoundBass("Effects\\HitButton.wav");
            HitHolder = LoadSoundBass("Effects\\HitHolder.wav");
            HolderTick = LoadSoundBass("Effects\\HolderTick.wav");
            ComboBreak = LoadSoundBass("Effects\\ComboBreak.wav");
            ButtonOver = LoadSoundBass("Effects\\ButtonOver.wav");
            //ButtonHit = LoadSoundBass("Effects\\ButtonHit.wav");
            SelectorHit = LoadSoundBass("Effects\\SelectorHit.wav");
            ScrollHit = ButtonHit = LoadSoundBass("Effects\\Scroll.wav");
            WelcomeToOsuXd = LoadSoundBass("Effects\\welcome.mp3");
            SeeyaOsu = LoadSoundBass("Effects\\seeya.mp3");
            //OsuHit = LoadSoundBass("Effects\\OsuModeHit.wav");

            Hitwhistle = LoadSoundBass("Effects\\Hitwhistle.wav");
            Hitfinish = LoadSoundBass("Effects\\Hitfinish.wav");
            Hitclap = LoadSoundBass("Effects\\Hitclap.wav");


            Applause = LoadSoundBass("Effects\\applause.wav");

            MenuTransition = LoadSoundBass("Effects\\MenuTransition.mp3");

            NotificationSound = LoadSoundBass("Effects\\Notification.mp3");
            FailSound = LoadSoundBass("Effects\\failsound.mp3");
            FailTransition = LoadSoundBass("Effects\\failtrans.wav");
        }

        private void LoadFonts(SpriteBatch spriteBatch)
        {

            DefaultFont = Content.Load<SpriteFont>("SpriteFont1");
            GeneralBig = Content.Load<SpriteFont>("General");
            ListboxFont = Content.Load<SpriteFont>("Listbox");
            SettingsFont = Content.Load<SpriteFont>("SettingsDisplayFont");
            TitleFont = Content.Load<SpriteFont>("TitleFont");
            StandardButtonsFont = Content.Load<SpriteFont>("StandardButtonFont");
            ScoreBig = Content.Load<SpriteFont>("scorebig");

            MSGothic1 = Content.Load<SpriteFont>("MSGothic1");
            MSGothic2 = Content.Load<SpriteFont>("MSGothic2");
        }

        private void LoadSprites(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            LoadingSpinnerTx = Content.Load<Texture2D>("loadingSpin");

            ButtonDefault = Content.Load<Texture2D>("button_0");
            ButtonHolder = Content.Load<Texture2D>("holder_0");
            WaitDefault = Content.Load<Texture2D>("approachv2");
            Fill_1 = Content.Load<Texture2D>("Fill_1");
            FillStartEnd = Content.Load<Texture2D>("fillstart-end");

            ButtonDefault_0 = Content.Load<Texture2D>("button_0.5");
            ButtonHolder_0 = Content.Load<Texture2D>("holder_0.5");
            WaitDefault_0 = Content.Load<Texture2D>("approach0.5");

            TopEffect = Content.Load<Texture2D>("BackgroundEffect" + (((float)actualMode.Width / (float)actualMode.Height == 1.3f) ? "@1-3" : ""));

            Radiance = Content.Load<Texture2D>("radiance");

            PerfectTx = Content.Load<Texture2D>("perfect");
            ExcellentTx = Content.Load<Texture2D>("great");
            GoodTx = Content.Load<Texture2D>("bad");
            MissTx = Content.Load<Texture2D>("miss");

            Logo = Content.Load<Texture2D>("logo");

            LogoAtTwo = Content.Load<Texture2D>("logo@2");

            IsoLogo = Content.Load<Texture2D>("logo");
            IsoLogoAtTwo = Content.Load<Texture2D>("logo@2");

            Healthbar = Content.Load<Texture2D>("heathbar");

            AutoModeButton = Content.Load<Texture2D>("autoBtn");
            AutoModeButtonSel = Content.Load<Texture2D>("autoBtnSel");
            //SpaceSkip = Content.Load<Texture2D>("SpaceSkip");

            ClassicBackground = Content.Load<Texture2D>("ClassicBackground");
            ClassicBackground_0 = Content.Load<Texture2D>("ClassicBackground_0.5");

            defaultbg = AppDomain.CurrentDomain.BaseDirectory + @"\Assets\bg.jpg";

            if (KyunGame.xmas)
            {
                defaultbg = AppDomain.CurrentDomain.BaseDirectory + @"\Assets\xmas2.jpg";
            }

            using (var fs = new FileStream(defaultbg, FileMode.Open, FileAccess.Read))
            {
                DefaultBackground = ContentLoader.FromStream(graphics, fs);
            }

            //test
            StartButton = Content.Load<Texture2D>("PlayMain");
            ExitButton = Content.Load<Texture2D>("ExitMain");
            ConfigButton = Content.Load<Texture2D>("ConfigMain");
            ButtonStandard = Content.Load<Texture2D>("ButtonStandard");

            Catcher = Content.Load<Texture2D>("Catcher");

            CatcherCombo = Content.Load<Texture2D>("CatcherCombo");
            CatcherCombo2 = Content.Load<Texture2D>("CatcherCombo2");

            CatcherFire = Content.Load<Texture2D>("CatcherFire");
            CatcherFire2 = Content.Load<Texture2D>("CatcherFire2");

            CatcherMiss = Content.Load<Texture2D>("CatcherMiss");
            CatcherMiss2 = Content.Load<Texture2D>("CatcherMiss2");

            CatchObject = Content.Load<Texture2D>("CHitObject");

            MenuSnow = Content.Load<Texture2D>("menu-snow");

            ScrollListBeatmap = Content.Load<Texture2D>("ScrollListBeatmap");

            SongDescBox = Content.Load<Texture2D>("SongDescBox");

            ScrollListBeatmap_alt = Content.Load<Texture2D>("ScrollListBeatmap_alt");

            DiffSelector = Content.Load<Texture2D>("DiffSelector");
            /*
            SquareParticle = new Texture2D(UbeatGame.Instance.GraphicsDevice, 500, 500);
            Color[] sColor = new Color[500 * 500];

            for(int a = 0; a < 500 * 500; a++)
            {
                sColor[a] = Color.White;
            }
            SquareParticle.SetData(sColor);          */

            SquareParticle = Content.Load<Texture2D>("ParticleSquare");

            // SPRITE = Content.Load<Texture2D>("SPRITE");

            //SquareParticle = MenuSnow = SPRITE;

            RankingPanel = Content.Load<Texture2D>("ranking-panel");
            SquareButton = Content.Load<Texture2D>("SquareButton");
            EpWarn = Content.Load<Texture2D>("Ewarn");

            using (FileStream ff = File.Open(defaultbg, FileMode.Open))
            {
                CroppedBg = System.Drawing.Image.FromStream(ff);
            }

            RGBShiftEffect = Content.Load<Effect>("RGBShift");

            ApproachCircle = Content.Load<Texture2D>("approachcircle");
            CircleNote = Content.Load<Texture2D>("Note");
            CircleNoteHolder = Content.Load<Texture2D>("Note_holder");

            GameCursor = Content.Load<Texture2D>("cursor");

            BeatmapBarBase = Content.Load<Texture2D>("1beatmaptopbar");

            BeatmapBarTail = Content.Load<Texture2D>("beatmaptopbar");

            Scorebar = Content.Load<Texture2D>("scorebar");

            CatchTip = Content.Load<Texture2D>("CatchTip");
            OsuTip = Content.Load<Texture2D>("OsuTip");

            TestingBar = Content.Load<Texture2D>("cinta_testing");

            TripleAScore = Content.Load<Texture2D>("tascore");
            DoubleAScore = Content.Load<Texture2D>("dascore");
            AScore = Content.Load<Texture2D>("ascore");
            BScore = Content.Load<Texture2D>("bscore");
            CScore = Content.Load<Texture2D>("cscore");
            FScore = Content.Load<Texture2D>("fscore");
        }

        public void AddTexture(string name, Texture2D tex)
        {

        }


        private void loadSkins()
        {
            _SkinManager = new SkinManager();

            //Test

            if (Settings1.Default.Skin > 0)
                _SkinManager.SwitchSkin(Settings1.Default.Skin);
        }

        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {

            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new System.Drawing.Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = System.Drawing.Graphics.FromImage(destImage))
            {

                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }


        public static System.Drawing.Bitmap cropAtRect(System.Drawing.Bitmap b, System.Drawing.Rectangle r)
        {
            // An empty bitmap which will hold the cropped image
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(r.Width, r.Height);

            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);


            System.Drawing.Rectangle rg = new System.Drawing.Rectangle(0, 0, r.Width, r.Height);
            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(b, rg, r, System.Drawing.GraphicsUnit.Pixel);

            return bmp;
        }

        public static MemoryStream BitmapToStream(System.Drawing.Bitmap image)
        {
            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            return memoryStream;
        }

        public static Texture2D RoundCorners(Texture2D tx, float radius, float factor = 1)
        {

            List<Color> cl = new List<Color>();
            cl.Add(Color.Black);

            List<Color> cll = new List<Color>();
            cll.Add(Color.Green * 0f);
            cll.Add(Color.Red);
            cll.Add(Color.Blue * factor);
            Texture2D rConnr = ContentLoader.CreateRoundedRectangleTexture(
                KyunGame.Instance.GraphicsDevice,
                tx.Width,
                tx.Height,
                (int)3,
                (int)radius,
                5,
                cl,
                cll,
                .7f,
                0);

            Color[] rdc = new Color[rConnr.Width * rConnr.Height];
            rConnr.GetData<Color>(rdc);

            Color[] hdc = new Color[tx.Width * tx.Height];
            tx.GetData<Color>(hdc);

            Color[] final = new Color[rConnr.Width * rConnr.Height];

            int a = 0;

            foreach (Color clrr in rdc)
            {
                try
                {
                    hdc[a] = Color.FromNonPremultiplied(hdc[a].R, hdc[a].G, hdc[a].B, clrr.A);
                    final[a] = hdc[a];

                }
                catch
                {
                    break;
                }

                a++;
            }

            rConnr.SetData<Color>(final);

            return rConnr;
        }

        public static Texture2D ToGaussianBlur(Texture2D tx, int radial = 3)
        {

            var image = Texture2DToBitmap(tx);

            var blur = new GaussianBlur(image);

            var result = blur.Process(radial);

            return ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, BitmapToStream(result), false);
        }

        public static Texture2D ToGaussianBlur(System.Drawing.Bitmap bm, int radial = 3)
        {

            bm = cropAtRect(bm, new System.Drawing.Rectangle(bm.Width / 2 - 800 / 2, bm.Height / 2 - 450 / 2, 800, 450));
            var blur = new GaussianBlur(bm);
            
            bm = blur.Process(radial);

            return ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, BitmapToStream(bm));
        }

        public static System.Drawing.Bitmap Texture2DToBitmap(Texture2D tx)
        {
            byte[] txData = new byte[4 * tx.Width * tx.Height];
            tx.GetData<byte>(txData);

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(
               tx.Width, tx.Height
             );

            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(
                           new System.Drawing.Rectangle(0, 0, tx.Width, tx.Height),
                           System.Drawing.Imaging.ImageLockMode.WriteOnly,
                           System.Drawing.Imaging.PixelFormat.Format32bppArgb
                         );

            IntPtr safePtr = bmpData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(txData, 0, safePtr, txData.Length);
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        public static Texture2D RoundCornerss(Texture2D texture, float radius)
        {
            //   return texture;

            Texture2D texturex = new Texture2D(KyunGame.Instance.GraphicsDevice, texture.Width, texture.Height);

            Color[] colorData = new Color[texture.Width * texture.Height];
            Color[] colorDataTx = new Color[texture.Width * texture.Height];

            texture.GetData<Color>(colorDataTx);

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {

                    int index = y * texture.Width + x;

                    Rectangle internalRectangle = new Rectangle(
                        (int)radius, (int)(radius),
                        (int)((float)texture.Width + 1 - 2f * radius),
                        (int)((float)texture.Height + 1 - 2f * radius));

                    Vector2 origin = Vector2.Zero;
                    Vector2 point = new Vector2(x, y);

                    if (x < radius)
                    {
                        if (y < radius)
                            origin = new Vector2(radius, radius);
                        else if (y > texture.Height - (radius))
                            origin = new Vector2(radius, texture.Height - (radius));
                        else
                            origin = new Vector2(radius, y);
                    }
                    else if (x > texture.Width - (radius))
                    {
                        if (y < radius)
                            origin = new Vector2(texture.Width - (radius), radius);
                        else if (y > texture.Height - (radius))
                            origin = new Vector2(texture.Width - (radius), texture.Height - (radius));
                        else
                            origin = new Vector2(texture.Width - (radius), y);
                    }
                    else
                    {
                        if (y < radius)
                            origin = new Vector2(x, radius);
                        else if (y > texture.Height - (radius))
                            origin = new Vector2(x, texture.Height - (radius));
                    }

                    if (!origin.Equals(Vector2.Zero))
                    {
                        float distance = Vector2.Distance(point, origin);

                        if (distance > radius)
                        {
                            colorData[index] = Color.Transparent;
                        }
                        else
                        {
                            colorData[index] = colorDataTx[index];
                        }
                    }

                    if (internalRectangle.Contains(x, y))
                    {
                        // colorData[index] = colorDataTx[index];
                    }


                    //colorData[index] = colorDataTx[index];

                    //colorData[index] = Color.Transparent;
                }
            }

            texturex.SetData(colorData);
            return texturex;
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
        public Texture2D LogoAtTwo { get; set; }
        public Texture2D SpaceSkip { get; set; }
        public Texture2D TopEffect { get; set; }
        public Texture2D DefaultBackground { get; set; }
        public Texture2D LoadingSpinnerTx { get; set; }
        public Texture2D ButtonStandard { get; set; }
        public Texture2D MenuSnow { get; set; }
        public Texture2D ClassicBackground { get; set; }
        public Texture2D ClassicBackground_0 { get; set; }
        public Texture2D Fill_1 { get; set; }
        public Texture2D Fill_2 { get; set; }
        public Texture2D Fill_3 { get; set; }
        public Texture2D Fill_4 { get; set; }
        public Texture2D Fill_5 { get; set; }
        public Texture2D Fill_6 { get; set; }
        public Texture2D ScrollListBeatmap { get; set; }
        public Texture2D ScrollListBeatmap_alt { get; set; }
        public Texture2D SongDescBox { get; set; }
        public Texture2D SquareParticle { get; set; }
        public Texture2D SPRITE { get; set; }
        public Texture2D IsoLogoAtTwo { get; set; }
        public Texture2D IsoLogo { get; set; }
        public Texture2D RankingPanel { get; set; }
        public Texture2D EpWarn { get; set; }
        public System.Drawing.Image CroppedBg { get; set; }
        public System.Drawing.Image CroppedBgCover2 { get; set; }
        public Texture2D Catcher { get; set; }

        public Texture2D CatcherCombo { get; set; }
        public Texture2D CatcherCombo2 { get; set; }

        public Texture2D CatcherFire { get; set; }
        public Texture2D CatcherFire2 { get; set; }

        public Texture2D CatcherMiss { get; set; }
        public Texture2D CatcherMiss2 { get; set; }

        public Texture2D CatchObject { get; set; }

        public Texture2D BeatmapBarBase { get; set; }
        public Texture2D BeatmapBarTail { get; set; }

        public Texture2D Scorebar { get; set; }

        public Texture2D CatchTip { get; set; }

        public Texture2D OsuTip { get; set; }

        public Texture2D TestingBar { get; set; }

        public Texture2D TripleAScore { get; set; }
        public Texture2D DoubleAScore { get; set; }
        public Texture2D AScore { get; set; }
        public Texture2D BScore { get; set; }
        public Texture2D CScore { get; set; }
        public Texture2D FScore { get; set; }

        #endregion

        #region FontsVars
        public SpriteFont DefaultFont { get; set; }
        public SpriteFont GeneralBig { get; set; }
        public SpriteFont ListboxFont { get; set; }
        public SpriteFont SettingsFont { get; set; }
        public SpriteFont TitleFont { get; set; }
        public SpriteFont StandardButtonsFont { get; set; }
        public SpriteFont ScoreBig { get; set; }
        public SpriteFont MSGothic1 { get; set; }
        public SpriteFont MSGothic2 { get; set; }
        #endregion

        #region AudiosVars
        public int ButtonHit { get; set; }
        public int ComboBreak { get; set; }
        public int ButtonOver { get; set; }
        public int HitButton { get; set; }
        public int HitHolder { get; set; }
        public int HolderFilling { get; set; }
        public int HolderTick { get; set; }
        public int SelectorHit { get; set; }
        public int ScrollHit { get; set; }
        public int SeeyaOsu { get; set; }
        public int OsuHit { get; set; }
        public int WelcomeToOsuXd { get; set; }
        public int Hitwhistle { get; set; }
        public int Hitfinish { get; set; }
        public int Hitclap { get; set; }
        public int Applause { get; set; }
        public Texture2D SquareButton { get; set; }
        public Texture2D FillStartEnd { get; set; }
        public Texture2D Healthbar { get; set; }
        public string defaultbg { get; set; }
        public int MenuTransition { get; set; }
        public int NotificationSound { get; set; }
        public Effect RGBShiftEffect { get; set; }
        public Texture2D ApproachCircle { get; set; }
        public Texture2D CircleNote { get; set; }
        public Texture2D CircleNoteHolder { get; set; }
        public int FailSound { get; set; }
        public int FailTransition { get; private set; }
        public Texture2D GameCursor { get; set; }
        public Texture2D DiffSelector { get; set; }

        #endregion


    }
}
