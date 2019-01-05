using kyun.Audio;
using kyun.Beatmap;
using kyun.game.Database;
using kyun.game.GameModes.CatchIt;
using kyun.game.GameScreen.UI;
using kyun.game.GameScreen.UI.Scoreboard;
using kyun.game.NikuClient;
using kyun.GameScreen;
using kyun.GameScreen.UI;
using kyun.GameScreen.UI.Buttons;
using kyun.Score;
using kyun.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kyun.GameModes.Classic
{
    public class ScorePanel : ScreenBase
    {

        public static bool Displaying = false;

        public static ScorePanel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ScorePanel();
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public Label scoreLabel { get; private set; }

        static ScorePanel instance;

        public GameModeScreenBase i;
        private Label mapNameLabel;
        private Image rankingPanel;
        private Label perfectlbl;
        private Label greatlbl;
        private Label badlbl;
        private Label misslbl;
        private Label acclbl;
        private Label combolbl;
        private Image perfectimg;
        private Image greatimg;
        private Image badimg;
        private Image missimg;
        Vector2 mStr;

        int cmargin = 65;
        private ButtonStandard backButton;
        private ButtonStandard tryAgainBtn;
        private ButtonStandard replayBtn;
        public Replay rpl;

        public ScorePanel()
        {
            AllowVideo = true;
            mStr = SpritesContent.Instance.ScoreBig.MeasureString("la puta madre que te pario");

            var aMode = ActualScreenMode;


            rankingPanel = new Image(SpritesContent.Instance.RankingPanel)
            {
                Position = new Vector2(aMode.Width / 2 - (SpritesContent.Instance.RankingPanel.Width / 2), aMode.Height / 2 - (SpritesContent.Instance.RankingPanel.Height / 2)),
                BeatReact = false
            };


            mapNameLabel = new Label()
            {
                Text = $"",
                Font = SpritesContent.Instance.TitleFont,
                //Centered = true,
                Position = new Vector2(rankingPanel.Position.X, rankingPanel.Position.Y - 50)
            };



            scoreLabel = new Label(0)
            {
                Text = "",
                Font = SpritesContent.Instance.ScoreBig,
                //Centered = true,
                Position = new Vector2(rankingPanel.Position.X + 70, rankingPanel.Position.Y + 20),
            };



            perfectimg = new Image(SpritesContent.Instance.PerfectTx)
            {
                Position = scoreLabel.Position + new Vector2(0, mStr.Y + cmargin),
                BeatReact = false

            };

            greatimg = new Image(SpritesContent.Instance.ExcellentTx)
            {
                Position = perfectimg.Position + new Vector2(200, 0),
                BeatReact = false
            };

            badimg = new Image(SpritesContent.Instance.GoodTx)
            {
                Position = greatimg.Position + new Vector2(0, mStr.Y + cmargin),
                BeatReact = false
            };

            missimg = new Image(SpritesContent.Instance.MissTx)
            {
                Position = badimg.Position + new Vector2(0, mStr.Y + cmargin),
                BeatReact = false
            };

            perfectlbl = new Label(0)
            {
                Text = "",
                Font = SpritesContent.Instance.ScoreBig,
                Position = perfectimg.Position + new Vector2(perfectimg.Texture.Height + 100, 0),
            };


            greatlbl = new Label(0)
            {
                Text = "",
                Font = SpritesContent.Instance.ScoreBig,
                Position = greatimg.Position + new Vector2(perfectimg.Texture.Height + 50, 0),
            };


            badlbl = new Label(0)
            {
                Text = "",
                Font = SpritesContent.Instance.ScoreBig,
                Position = badimg.Position + new Vector2(perfectimg.Texture.Height + 100, 0),
            };

            misslbl = new Label(0)
            {
                Text = "",
                Font = SpritesContent.Instance.ScoreBig,
                Position = missimg.Position + new Vector2(perfectimg.Texture.Height + 100, 0),
            };


            combolbl = new Label(0)
            {
                Text = "",
                Font = SpritesContent.Instance.ScoreBig,
                Position = missimg.Position + new Vector2(0, mStr.Y + cmargin),
            };


            acclbl = new Label(0)
            {
                Text = "",
                Font = SpritesContent.Instance.ScoreBig,
                Position = combolbl.Position + new Vector2(300, 0),
            };

            backButton = new ButtonStandard(Color.DarkRed)
            {
                ForegroundColor = Color.White,
                Caption = "Back",
                Position = new Vector2(15, aMode.Height - SpritesContent.Instance.ButtonStandard.Height - 10),
            };


            tryAgainBtn = new ButtonStandard(Color.Aquamarine)
            {
                ForegroundColor = Color.White,
                Caption = "Rematch!",
                Position = new Vector2(backButton.Position.X, backButton.Position.Y - SpritesContent.Instance.ButtonStandard.Height - 10),
            };

            replayBtn = new ButtonStandard(Color.Blue)
            {
                ForegroundColor = Color.White,
                Caption = "Replay",
                Position = new Vector2(tryAgainBtn.Position.X, tryAgainBtn.Position.Y - SpritesContent.Instance.ButtonStandard.Height - 10),
            };

            Controls.Add(rankingPanel);
            Controls.Add(mapNameLabel);
            Controls.Add(scoreLabel);

            Controls.Add(perfectlbl);
            Controls.Add(greatlbl);
            Controls.Add(badlbl);
            Controls.Add(misslbl);
            Controls.Add(acclbl);
            Controls.Add(combolbl);

            Controls.Add(perfectimg);
            Controls.Add(greatimg);
            Controls.Add(badimg);
            Controls.Add(missimg);

            Controls.Add(backButton);
            Controls.Add(tryAgainBtn);
            Controls.Add(replayBtn);



            onKeyPress += (obj, args) =>
            {
                if (args.Key == Microsoft.Xna.Framework.Input.Keys.Escape)
                {
                    Displaying = false;
                    EffectsPlayer.StopAll();
                    AVPlayer.audioplayer.Play();
                    ScreenManager.ChangeTo(BeatmapScreen.Instance);

                }

            };

            backButton.Click += (o, ar) =>
            {
                Displaying = false;
                EffectsPlayer.StopAll();
                AVPlayer.audioplayer.Play();
                ScreenManager.ChangeTo(BeatmapScreen.Instance);
            };

            tryAgainBtn.Click += TryAgainBtn_Click;

            replayBtn.Click += ReplayBtn_Click;

            //BackgroundDim = i.BackgroundDim;

            //EffectsPlayer.PlayEffect(SpritesContent.Instance.Applause);
        }

        private void TryAgainBtn_Click(object sender, EventArgs e)
        {
            Displaying = false;
            EffectsPlayer.StopAll();
            ScreenManager.ChangeTo(i);
            i.Play(i.Beatmap, i.gameMod); //restart game
        }

        private void ReplayBtn_Click(object sender, EventArgs e)
        {
            Displaying = false;
            EffectsPlayer.StopAll();
            ScreenManager.ChangeTo(i);
            i.Play(i.Beatmap, i.gameMod, rpl); //restart replay
        }

        public async void CalcScore(GameModeScreenBase ins)
        {
            if (Displaying)
                return;

            Displaying = true;
            i = ins;
            AVPlayer.audioplayer.SetVelocity(1);
            EffectsPlayer.PlayEffect(SpritesContent.Instance.Applause);
            Background = i.Background;

            GameMod mods = i.gameMod;
            string mmods = "";
            if ((mods & GameMod.Auto) == GameMod.Auto)
            {
                mmods += "Auto ";
            }

            if ((mods & GameMod.DoubleTime) == GameMod.DoubleTime)
            {
                mmods += "DoubleTime ";
            }

            if ((mods & GameMod.NoFail) == GameMod.NoFail)
            {
                mmods += "NoFail ";
            }

            mapNameLabel.Text = $" {i.Beatmap.Artist} - {i.Beatmap.Title} [{i.Beatmap.Version}]";
            scoreLabel.Text = $"{i._scoreDisplay.TotalScore.ToString("00000000")} {mmods}";
            combolbl.Text = $"{Combo.Instance.MaxMultiplier}";

            int missCount = 0;
            int perfectCount = 0;
            int greatCount = 0;
            int badCount = 0;

            int acc = 0;

            foreach (HitBase hb in i.HitObjects)
            {
                ScoreType score = ScoreType.Miss;
                if (hb is HitHolder)
                {
                    HitHolder hbb = (HitHolder)hb;
                    score = hbb.GetScore();
                }
                else if (hb is HitSingle)
                {
                    HitSingle bb = (HitSingle)hb;
                    score = bb.GetScore();
                }
                else if (hb is game.GameModes.CatchIt.HitObject)
                {
                    game.GameModes.CatchIt.HitObject bbh = (game.GameModes.CatchIt.HitObject)hb;
                    score = bbh.GetScore();
                }
                else
                {
                    score = hb.GetScore();
                }

                switch (score)
                {
                    case ScoreType.Miss:
                        missCount++;
                        break;
                    case ScoreType.Good:
                        badCount++;
                        acc += 15;
                        break;
                    case ScoreType.Excellent:
                        greatCount++;
                        acc += 50;
                        break;
                    case ScoreType.Perfect:
                        perfectCount++;
                        acc += 100;
                        break;
                }
            }

            if (i is ClassicModeScreen)
                rpl = Replay.Build(i.HitObjects, i.Beatmap);
            else if (i is CatchItMode)
                rpl = Replay.Build(((CatchItMode)i).movements, i.Beatmap, false);
            else if (i is OsuMode.OsuMode)
            {
                if ((i.gameMod & GameMod.Replay) == GameMod.Replay)
                {
                    rpl = ((OsuMode.OsuMode)i).replay;
                }
                else
                {
                    rpl = Replay.Build(((OsuMode.OsuMode)i).HitObjects, i.Beatmap, ((OsuMode.OsuMode)i).RecordedMousePositions);
                }
            }
            else
                rpl = null;


            misslbl.Text = $"{missCount}";
            badlbl.Text = $"{badCount}";
            greatlbl.Text = $"{greatCount}";
            perfectlbl.Text = $"{perfectCount}";
            acclbl.Text = $"{((float)acc / (float)i.HitObjects.Count).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)}%";

            string rawMovements = await rpl.ToString();

            if ((mods & GameMod.Auto) == GameMod.Auto)
            {

            }
            else if ((i.gameMod & GameMod.Replay) == GameMod.Replay)
            {
                
            }
            else
            {
                DatabaseInterface.Instance.SaveScore(new game.Score.ScoreInfo
                {
                    Score = (int)i._scoreDisplay.TotalScore,
                    Combo = (int)Combo.Instance.MaxMultiplier,
                    Beatmap = (ubeatBeatMap)i.Beatmap,
                    BeatmapArtist = i.Beatmap.Artist,
                    BeatmapDiff = i.Beatmap.Version,
                    BeatmapName = i.Beatmap.Title,
                    RawMovements = rawMovements,
                    UserId = -1,
                    Username = (NikuClientApi.User != null) ? NikuClientApi.User.Username : "Anon"
                });

                Scoreboard.Instance.Items.Clear();
                Scoreboard.Instance.AddList(ScoreItem.GetFromDb((ubeatBeatMap)i.Beatmap));
            }
        }

        public override void Update(GameTime tm)
        {
            base.Update(tm);

            perfectlbl.Position = perfectimg.Position + new Vector2(perfectimg.Texture.Width + 10, -12);
            greatlbl.Position = greatimg.Position + new Vector2(perfectimg.Texture.Width + 50, -12);
            badlbl.Position = badimg.Position + new Vector2(perfectimg.Texture.Height + 100, -12);
            misslbl.Position = missimg.Position + new Vector2(perfectimg.Texture.Width + 50, -12);
            acclbl.Position = missimg.Position + new Vector2(0, (missimg.Texture.Height + cmargin) * 2 - 12);
            combolbl.Position = badimg.Position + new Vector2(0, (badimg.Texture.Height + cmargin) * 2 - 12);

            perfectimg.Position = scoreLabel.Position + new Vector2(-50, mStr.Y + cmargin);
            greatimg.Position = perfectimg.Position + new Vector2(300, 0);
            badimg.Position = perfectimg.Position + new Vector2(0, perfectimg.Texture.Height + cmargin);
            missimg.Position = badimg.Position + new Vector2(300, 0);
        }
    }
}
