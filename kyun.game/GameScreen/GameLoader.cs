using kyun.Beatmap;
using kyun.GameModes;
using kyun.GameScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using kyun.Utils;
using kyun.GameScreen.UI;
using Redux.Utilities.Managers;
using System.IO;
//using FreqData;
using kyun.game.Winforms;

namespace kyun.game.GameScreen
{
    //This shit loads hitobjects of beatmap if is not loaded
    public class GameLoader : ScreenBase
    {
        public static GameLoader instance;

        public static GameLoader GetInstance()
        {
            if (instance == null)
                instance = new GameLoader();

            return instance;
        }

        IBeatmap _beatmap;
        GameModeScreenBase _gameScreen;
        GameMod _mods;

        int countToRun = 0;
        private Label loadingLabel;
        private Label beatmapDisplayLabel;
        private Label detailsLabel;
        private Image coverimg;

        public Image ImgTip { get; }

        BeatmapScreen BmScr;
        private bool generatingBm;

        bool loading { get; set; }

        public GameLoader() : base("GameLoader")
        {
            BmScr = (BeatmapScreen)BeatmapScreen.Instance;
            Background = SpritesContent.Instance.DefaultBackground;
            BackgroundDim = .5f;
            AllowVideo = true;

            loadingLabel = new Label(0)
            {
                Text = "Loading beatmap",
                Centered = true,
                Position = new Vector2(ActualScreenMode.Width / 2, ActualScreenMode.Height / 2 - SpritesContent.Instance.ListboxFont.MeasureString("ewe").Y * 3),
                Font = SpritesContent.Instance.ListboxFont
            };

            beatmapDisplayLabel = new Label(0)
            {
                Text = "",
                Centered = true,
                Position = new Vector2(ActualScreenMode.Width / 2, loadingLabel.Position.Y + loadingLabel.Font.MeasureString(":3").Y + 5),
                Font = SpritesContent.Instance.TitleFont
            };

            detailsLabel = new Label(0)
            {
                Text = "",
                Centered = true,
                Position = new Vector2(ActualScreenMode.Width / 2, beatmapDisplayLabel.Position.Y + beatmapDisplayLabel.Font.MeasureString("uwu").Y + 5),
                Font = SpritesContent.Instance.ListboxFont,
                Scale = .7f
            };

            coverimg = new Image(SpritesContent.Instance.DefaultBackground)
            {
                Position = new Vector2(ActualScreenMode.Width / 2 - BmScr.coverSize / 2, loadingLabel.Position.Y - BmScr.coverSize / 2 ),
                //Texture = BmScr.coverimg.Texture,
                BeatReact = false
            };

            ImgTip = new Image(SpritesContent.Instance.CatchTip);

            ImgTip.Position = new Vector2(ActualScreenMode.Width/2 - SpritesContent.Instance.CatchTip.Width / 2, detailsLabel.Position.Y + detailsLabel.Font.MeasureString("a").Y * .5f * 2 + 10);
            ImgTip.BeatReact = false;
            ImgTip.Scale = .7f;


            Controls.Add(loadingLabel);
            Controls.Add(beatmapDisplayLabel);
            Controls.Add(detailsLabel);
            Controls.Add(coverimg);
            Controls.Add(ImgTip);

            onKeyPress += (e, args) =>
            {
                if(args.Key == Microsoft.Xna.Framework.Input.Keys.Escape)
                {
                    //STOP 

                    ScreenManager.ChangeTo(BeatmapScreen.Instance);
                    //reset
                    loading = false;
                    countToRun = 0;
                }

            };
        }

        private void ChangeImgDisplay(string image)
        {
            int coverSize = 300;

            coverimg.Position = new Vector2(ActualScreenMode.Width / 2 - coverSize / 2, loadingLabel.Position.Y - coverSize / 2);

            System.Drawing.Image cimg = null;

            if (!File.Exists(image))
            {
                if (SpritesContent.Instance.CroppedBg == null)
                {
                    using (FileStream ff = File.Open(SpritesContent.Instance.defaultbg, FileMode.Open))
                    {
                        cimg = System.Drawing.Image.FromStream(ff);
                        SpritesContent.Instance.CroppedBg = cimg;
                    }

                }

            }
            else if (File.GetAttributes(image) == FileAttributes.Directory)
            {
                if (SpritesContent.Instance.CroppedBg == null)
                {
                    using (FileStream ff = File.Open(SpritesContent.Instance.defaultbg, FileMode.Open))
                    {
                        cimg = System.Drawing.Image.FromStream(ff);
                        SpritesContent.Instance.CroppedBg = cimg;
                    }

                }
            }
            else
            {
                cimg = System.Drawing.Image.FromFile(image);
            }

            if (cimg == null)
            {
                cimg = SpritesContent.Instance.CroppedBg;
            }


            System.Drawing.Bitmap cbimg = SpritesContent.ResizeImage(cimg, (int)(((float)cimg.Width / (float)cimg.Height) * coverSize), (int)coverSize);

            System.Drawing.Bitmap ccbimg;
            MemoryStream istream;
            if (true)
            {
                ccbimg = SpritesContent.cropAtRect(cbimg, new System.Drawing.Rectangle((int)((cbimg.Width - coverSize) / 2), (int)((cbimg.Height - coverSize / 2.2f) / 2f), (int)coverSize, (int)(coverSize / 2.2f)));
                istream = SpritesContent.BitmapToStream(ccbimg);
            }
            else
            {
                istream = SpritesContent.BitmapToStream(cbimg);
            }

            coverimg.Texture = SpritesContent.RoundCorners(ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, istream), 5);

        }

        /// <summary>
        /// Loads and execute game
        /// </summary>
        /// <param name="beatmap"></param>
        /// <param name="gameScreen"></param>
        public void LoadBeatmapAndRun(IBeatmap beatmap, GameModeScreenBase gameScreen, GameMod mods = GameMod.None)
        {

            if(gameScreen is GameModes.CatchIt.CatchItMode)
            {
                ImgTip.Texture = SpritesContent.Instance.CatchTip;
                ImgTip.Visible = true;
            }
            else if(gameScreen is kyun.GameModes.OsuMode.OsuMode)
            {
                ImgTip.Texture = SpritesContent.Instance.OsuTip;
                ImgTip.Visible = true;
            }
            else
            {
                ImgTip.Visible = false; 
            }

            ChangeImgDisplay(beatmap.Background);

            if (File.Exists(beatmap.Background))
                Background = SpritesContent.ToGaussianBlur(System.Drawing.Bitmap.FromFile(beatmap.Background) as System.Drawing.Bitmap, 10);
            else
                Background = SpritesContent.Instance.DefaultBackground;


            loading = false;
            countToRun = 0;
            _beatmap = beatmap;
            _gameScreen = gameScreen;
            _mods = mods;

            beatmapDisplayLabel.Text = StringHelper.WrapText(beatmapDisplayLabel.Font, $"{_beatmap.Artist} - {_beatmap.Title}", ActualScreenMode.Width - 200);
            detailsLabel.Text = $"Difficulty: {_beatmap.Version}\r\nBy {_beatmap.Creator}";
            //coverimg.Texture = BmScr.coverimg.Texture;
            //Start update
            KyunGame.Instance.KeyBoardManager.Enabled = false;
            ScreenManager.ChangeTo(this);
        }

        /// <summary>
        /// Loads and execute game with mp3 song
        /// </summary>
        /// <param name="beatmap"></param>
        /// <param name="gameScreen"></param>
        public async void LoadBeatmapAndRun(string song, GameModeScreenBase gameScreen, GameMod mods = GameMod.None)
        {
            if (gameScreen is GameModes.CatchIt.CatchItMode)
            {
                ImgTip.Texture = SpritesContent.Instance.CatchTip;
                ImgTip.Visible = true;
            }
            else if (gameScreen is kyun.GameModes.OsuMode.OsuMode)
            {
                ImgTip.Texture = SpritesContent.Instance.OsuTip;
                ImgTip.Visible = true;
            }
            else
            {
                ImgTip.Visible = false;
            }

            Background = SpritesContent.Instance.DefaultBackground;
            ChangeImgDisplay("");
            loading = false;
            countToRun = 0;
            //_beatmap = beatmap;
            _gameScreen = gameScreen;
            _mods = mods;

            beatmapDisplayLabel.Text = StringHelper.WrapText(beatmapDisplayLabel.Font, $"Loading...", ActualScreenMode.Width - 200);
            detailsLabel.Text = $"Generating beatmap, please wait";
            //coverimg.Texture = BmScr.coverimg.Texture;
            //Start update
            KyunGame.Instance.KeyBoardManager.Enabled = false;
            ScreenManager.ChangeTo(this);

            generatingBm = loading = true;
            AVPlayer.Play(song);

            new bmDiff().ShowDialog();


            //string newbeatmap = await TestGenerator.GenNew(song, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps"),
            //    bmDiff.BmDiff,
            //    new string[] { bmDiff.Artist, bmDiff.CName },
            //    new TestGenerator.StageChangedHandler((obj, args)=> {
            //    beatmapDisplayLabel.Text = $"{(args.porcentage * 100f).ToString("0")}%";
            //    detailsLabel.Text = $"Status: {args.stageName}";
            //}));

            //if ((BeatmapScreen.Instance as BeatmapScreen).AMode)
            //{
            //    _mods |= GameMod.Auto;
            //}

            //if ((BeatmapScreen.Instance as BeatmapScreen).doubleTime)
            //{
            //    _mods |= GameMod.DoubleTime;
            //}
            //_beatmap = OsuUtils.OsuBeatMap.FromFile(newbeatmap, true);
            generatingBm = loading = false;
        }

        public override void Update(GameTime tm)
        {
            base.Update(tm);

            if (!Visible)
                return;

            if (generatingBm)
                return;

            countToRun += tm.ElapsedGameTime.Milliseconds;

            if (countToRun > 3000 && !loading)
                issueLoad();
        }

        private void issueLoad()
        {
            KyunGame.Instance.KeyBoardManager.Enabled = false;
            loading = true;

            if (_beatmap.HitObjects.Count > 0)
            {
                //Better than "goto"
                _gameScreen.Play(_beatmap, _mods);
                KyunGame.Instance.KeyBoardManager.Enabled = false;
                ScreenManager.ChangeTo(_gameScreen);
                return;
            }

            try
            {
                string path = _beatmap.FilePath;
                _beatmap = OsuUtils.OsuBeatMap.FromFile(path, true);

                _gameScreen.Play(_beatmap, _mods);
                KyunGame.Instance.KeyBoardManager.Enabled = false;
                ScreenManager.ChangeTo(_gameScreen);
            }
            catch (Exception ex)
            {
                //Log error
                Logger.Instance.Warn($"{ex.Message}, \r\n\r\nStackTrace: \r\n\r\n{ex.StackTrace}");

                //Corrupted?
                ScreenManager.ChangeTo(BeatmapScreen.Instance);
                KyunGame.Instance.Notifications.ShowDialog("Ups, beatmap corrupted, check log for more info.");

            }
        }
    }
}
