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

namespace kyun.game.GameScreen
{
    //This shit loads hitobjects of beatmap if is not loaded
    public class GameLoader : ScreenBase
    {
        private static GameLoader instance;

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
        BeatmapScreen BmScr;

        bool loading { get; set; }

        public GameLoader() : base("GameLoader")
        {
            BmScr = (BeatmapScreen)BeatmapScreen.Instance;
            Background = SpritesContent.Instance.DefaultBackground;
            BackgroundDim = .9f;
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
                Scale = .5f
            };

            coverimg = new Image(SpritesContent.Instance.DefaultBackground)
            {
                Position = new Vector2(ActualScreenMode.Width / 2 - BmScr.coverSize / 2, loadingLabel.Position.Y - BmScr.coverSize / 2 ),
                Texture = BmScr.coverimg.Texture,
                BeatReact = false
            };

            Controls.Add(loadingLabel);
            Controls.Add(beatmapDisplayLabel);
            Controls.Add(detailsLabel);
            Controls.Add(coverimg);

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

        /// <summary>
        /// Loads and execute game
        /// </summary>
        /// <param name="beatmap"></param>
        /// <param name="gameScreen"></param>
        public void LoadBeatmapAndRun(IBeatmap beatmap, GameModeScreenBase gameScreen, GameMod mods = GameMod.None)
        {
            
            loading = false;
            countToRun = 0;
            _beatmap = beatmap;
            _gameScreen = gameScreen;
            _mods = mods;

            beatmapDisplayLabel.Text = $"{_beatmap.Artist} - {_beatmap.Title}";
            detailsLabel.Text = $"Difficulty: {_beatmap.Version}\r\nBy {_beatmap.Creator}";
            coverimg.Texture = BmScr.coverimg.Texture;
            //Start update
            KyunGame.Instance.KeyBoardManager.Enabled = false;
            ScreenManager.ChangeTo(this);
        }

        public override void Update(GameTime tm)
        {
            base.Update(tm);

            if (!Visible)
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
                //Corrupted?
                ScreenManager.ChangeTo(BeatmapScreen.Instance);
            }
        }
    }
}
