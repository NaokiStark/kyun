using kyun.Audio;
using kyun.Beatmap;
using kyun.game.Database;
using kyun.game.GameModes.CatchIt;
using kyun.game.GameScreen;
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
using System.IO;
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

        private Label loadingLabel;
        private Label beatmapDisplayLabel;
        private Label detailsLabel;
        private Image coverimg;
        Vector2 mStr;

        int cmargin = 65;
        private ButtonStandard backButton;
        private ButtonStandard tryAgainBtn;
        private ButtonStandard replayBtn;

        public Image ScoreLetter { get; private set; }

        public Replay rpl;

        private int infoMargin = 300;

        public ScorePanel()
        {
            infoMargin = ActualScreenMode.Width / 2 - infoMargin;
            AllowVideo = true;
            mStr = SpritesContent.Instance.ScoreBig.MeasureString("la puta madre que te pario");

            var aMode = ActualScreenMode;

            var rnkPnlPos = new Vector2(ActualScreenMode.Width /2 + 50,
                aMode.Height / 2 - (SpritesContent.Instance.RankingPanel.Height / 2));
            rankingPanel = new Image(SpritesContent.Instance.RankingPanel)
            {
                //Position = new Vector2(aMode.Width / 2 - (SpritesContent.Instance.RankingPanel.Width / 2), aMode.Height / 2 - (SpritesContent.Instance.RankingPanel.Height / 2)),
                Position = rnkPnlPos,
                BeatReact = false,
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

            ScoreLetter = new Image(SpritesContent.instance.FScore)
            {
                BeatReact = false,
                
            };

            loadingLabel = new Label(0)
            {
                Text = "Beatmap Passed",
                Centered = true,
                Position = new Vector2(infoMargin,
                    ActualScreenMode.Height / 2 - SpritesContent.Instance.ListboxFont.MeasureString("ewe").Y * 3),

                Font = SpritesContent.Instance.ListboxFont
            };

            beatmapDisplayLabel = new Label(0)
            {
                Text = "",
                Centered = true,
                Position = new Vector2(infoMargin, loadingLabel.Position.Y + loadingLabel.Font.MeasureString(":3").Y + 5),
                Font = SpritesContent.Instance.TitleFont
            };

            detailsLabel = new Label(0)
            {
                Text = "",
                Centered = true,
                Position = new Vector2(infoMargin, beatmapDisplayLabel.Position.Y + beatmapDisplayLabel.Font.MeasureString("uwu").Y + 5),
                Font = SpritesContent.Instance.ListboxFont,
                Scale = .5f
            };

            coverimg = new Image(SpritesContent.Instance.DefaultBackground)
            {
                Position = new Vector2(infoMargin - (BeatmapScreen.Instance as BeatmapScreen).coverSize / 2, loadingLabel.Position.Y - (BeatmapScreen.Instance as BeatmapScreen).coverSize / 2),
                //Texture = BmScr.coverimg.Texture,
                BeatReact = false
            };

            Controls.Add(rankingPanel);
            //Controls.Add(mapNameLabel);
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
            
            Controls.Add(loadingLabel);
            Controls.Add(beatmapDisplayLabel);
            Controls.Add(detailsLabel);
            Controls.Add(coverimg);
            Controls.Add(ScoreLetter);
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
                AVPlayer.videoplayer.Play(i.Beatmap.Video);
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
            ChangeImgDisplay(ins.Beatmap.Background);
            beatmapDisplayLabel.Text = StringHelper.WrapText(beatmapDisplayLabel.Font, $"{ins.Beatmap.Artist} - {ins.Beatmap.Title}", ActualScreenMode.Width - 200);
            detailsLabel.Text = $"Difficulty: {ins.Beatmap.Version}\r\nBy {ins.Beatmap.Creator}";

            i = ins;
            AVPlayer.audioplayer.SetVelocity(1);
            EffectsPlayer.PlayEffect(SpritesContent.Instance.Applause);
            Background = GameLoader.GetInstance().Background;

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

            if(((float)acc / (float)i.HitObjects.Count) == 100)
            {
                if((i.gameMod & GameMod.DoubleTime) == GameMod.DoubleTime)
                {
                    ScoreLetter.Texture = SpritesContent.instance.TripleAScore;
                }
                else
                {
                    ScoreLetter.Texture = SpritesContent.instance.DoubleAScore;
                }
            }
            else if(((float)acc / (float)i.HitObjects.Count) >= 90)
            {
                ScoreLetter.Texture = SpritesContent.instance.AScore;
            }
            else if (((float)acc / (float)i.HitObjects.Count) >= 80)
            {
                ScoreLetter.Texture = SpritesContent.instance.BScore;
            }
            else if(((float)acc / (float)i.HitObjects.Count) >= 50)
            {
                ScoreLetter.Texture = SpritesContent.instance.CScore;
            }
            else
            {
                ScoreLetter.Texture = SpritesContent.instance.FScore;
            }


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

        private void ChangeImgDisplay(string image)
        {
            int coverSize = 300;

            coverimg.Position = new Vector2(infoMargin - coverSize / 2, loadingLabel.Position.Y - coverSize / 2);

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

            ScoreLetter.Position = new Vector2(coverimg.Position.X + 160, coverimg.Position.Y);
            ScoreLetter.AngleRotation = 0;
            ScoreLetter.Scale = .5f;
        }
    }
}
