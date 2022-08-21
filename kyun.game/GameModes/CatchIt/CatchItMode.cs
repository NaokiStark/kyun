using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using kyun.Audio;
using kyun.Beatmap;
using kyun.GameModes;
using kyun.GameModes.Classic;
using kyun.GameScreen.UI;
using kyun.Utils;
using kyun.GameScreen;
using kyun.Overlay;
using Microsoft.Xna.Framework.Input;
using kyun.Score;
using kyun.UIObjs;
using kyun.Video;
using kyun.GameScreen.UI.Particles;
using Microsoft.Xna.Framework.Graphics;
using kyun.GameScreen.UI.Buttons;
using osuBMParser;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using kyun.game.GameScreen.UI;
using System.Threading;
using kyun.game.Video;
using kyun.game.Beatmap;
using System.IO;
using kyun.game.GameScreen.UI.Effects;

namespace kyun.game.GameModes.CatchIt
{
    public class CatchItMode : GameModeScreenBase
    {
        public new static CatchItMode Instance;
        internal int lastIndex;

        public Image Player;
        public Vector2 PlayerSize;
        public int playerLinePosition = 1;
        public int fieldSlots = 4;
        public int inFieldPosition = 0;
        public Vector2 FieldSize = new Vector2();
        public Vector2 FieldPosition = new Vector2();
        internal FilledRectangle fg;
        public int Margin = 2;

        internal List<FilledRectangle> rectangles = new List<FilledRectangle>();
        internal List<HitObject> HitObjectsRemain = new List<HitObject>();

        internal List<kyun.Beatmap.IHitObj> OriginalHitObjects = new List<kyun.Beatmap.IHitObj>();

        public OsuGameMode _osuMode { get; private set; }

        public Texture2D bombTx { get; set; }

        internal ComboDisplay comboDisplay;
        internal long endToTime;
        internal long countToScores;
        public ParticleEngine particleEngine;
        public HealthBar _healthbar;
        public Replay replay { get; set; }
        public List<ReplayObject> movements = new List<ReplayObject>();
        internal int mid;
        internal ButtonStandard skipButton;
        internal bool skipped;

        internal ProgressBar timeProgressBar;
        internal Image timeBarEnd;

        internal PlayerTxType txState;
        internal long catcherElapsed = 0;
        internal int catcherState = 0;
        public long catcherToIdle = 0;

        public Texture2D LongTail { get; set; }

        public bool End { get; private set; }
        public float FailsCount { get { return 1; } }

        public CustomSampleSet sampleSet;
        public osuBeatmapSkin _osuBeatmapSkin;

        float velocity = 1;
        public float Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = value;
                smoothingVel = true;
            }
        }


        public float SmoothVelocity { get; set; }

        public bool smoothingVel = false;

        internal long timeToPlayerGlow = 0;
        public bool isKeyDownShift = false;

        internal TimingPoint NonInheritedPoint;
        internal TimingPoint InheritedPoint;
        internal TimingPoint ActualTimingPoint;

        public TimingPoint NextTimingPoint { get; private set; }

        internal BPlayer GeneratorPlayer;
        float coverSize = 75;
        internal long lastTime = 0;
        private bool generateNotes;
        private bool generating;
        private bool generatingBg;
        private bool showingFail;
        Label lbpeak;

        internal Image powerup_guide;
        internal int p_guide_timer = 0;
        internal int p_guide_timeout = 10000;
        public static CatchItMode GetInstance()
        {
            if (Instance == null)
                Instance = new CatchItMode();
            return Instance;
        }

        public CatchItMode() : base("CatchIt")
        {
            bombTx = ContentLoader.LoadTextureFromAssets(Path.Combine("TxHelper", "bomb.png"));
            GeneratorPlayer = new BPlayer();

            AllowVideo = true;
            mid = 0;
            HitObjects = new List<HitBase>();
            BackgroundDim = .9f;

            Player = new Image(SpritesContent.Instance.Catcher);
            Player.BeatReact = true;
            //Player.EffectParameters = new RGBShiftEffect();
            //Player.TextureColor = Color.ForestGreen;

            PlayerSize = new Vector2(Math.Max(Math.Min(Player.Texture.Width, 100), 50), Math.Max(Math.Min(Player.Texture.Height, 100), 50));

            FieldSize = new Vector2(ActualScreenMode.Width, (PlayerSize.Y + Margin) * fieldSlots);
            //FieldPosition = new Vector2(0, (ActualScreenMode.Height / 2) - (FieldSize.Y / 2) - PlayerSize.Y);
            FieldPosition = new Vector2(0, 0);

            Player.Size = PlayerSize;

            Color lineColor = Color.FromNonPremultiplied(255, 229, 51, 255);

            for (int a = 0; a < fieldSlots; a++)
            {

                var rgnl = new FilledRectangle(new Vector2(FieldSize.X, PlayerSize.Y), Color.Black * .8f);
                rgnl.Position = new Vector2(FieldPosition.X, FieldPosition.Y + (PlayerSize.Y + Margin) * (a + 1));
                rgnl.Opacity = .9f;
                rectangles.Add(rgnl);
                Controls.Add(rectangles[a]);

                var lineRg = new FilledRectangle(new Vector2(FieldSize.X, Margin), lineColor);
                lineRg.Position = new Vector2(0, rgnl.Position.Y - Margin);
                Controls.Add(lineRg);
            }

            var lineRg2 = new FilledRectangle(new Vector2(FieldSize.X, Margin), lineColor);
            lineRg2.Position = new Vector2(0, rectangles[fieldSlots - 1].Position.Y + PlayerSize.Y);
            Controls.Add(lineRg2);

            timeProgressBar = new ProgressBar((int)FieldSize.X, 20)
            {
                Position = new Vector2(0, FieldPosition.Y + PlayerSize.Y - 20 - Margin),
            };

            int timeBarEndSize = 30;

            timeBarEnd = new Image(SpritesContent.Instance.MenuSnow)
            {
                Position = new Vector2(0, timeProgressBar.Position.Y - (timeBarEndSize / 2f) + (20 / 2f)),
                Size = new Vector2(timeBarEndSize),
                TextureColor = lineColor,
            };


            Controls.Add(timeProgressBar);
            Controls.Add(timeBarEnd);

            comboDisplay = new ComboDisplay()
            {
                Position = new Vector2(0, FieldSize.Y)
            };

            _scoreDisplay = new ScoreDisplay((ActualScreenMode.Height < 700 && ActualScreenMode.Width < 1000) ? 1.1f : 1.2f);


            particleEngine = new ParticleEngine();

            LongTail = new Texture2D(KyunGame.Instance.GraphicsDevice, SpritesContent.Instance.CatchObject.Width, SpritesContent.Instance.CatchObject.Height);
            Color[] dataBar = new Color[SpritesContent.Instance.CatchObject.Width * SpritesContent.Instance.CatchObject.Height];
            for (int i = 0; i < dataBar.Length; ++i) dataBar[i] = Color.White;
            LongTail.SetData(dataBar);


            _healthbar = new HealthBar(this, SpritesContent.Instance.Healthbar.Width - 5, 25);
            _healthbar.Position = new Vector2(0, FieldPosition.Y + FieldSize.Y + 2 + PlayerSize.Y);
            _scoreDisplay.Position = new Vector2(0, _healthbar.Position.Y + 30);

            _healthbar.OnFail += _healthbar_OnFail;

            skipButton = new ButtonStandard(Color.PaleVioletRed)
            {
                Caption = "Skip",
                Position = new Vector2(ActualScreenMode.Width - SpritesContent.Instance.ButtonDefault.Width, ActualScreenMode.Height - SpritesContent.Instance.ButtonDefault.Height),
                Visible = false
            };

            lbpeak = new Label
            {
                Text = "0"
            };

            lbpeak.Visible = false;

            System.Drawing.Image cimg = System.Drawing.Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + @"\Assets\bg.jpg");

            System.Drawing.Bitmap cbimg = SpritesContent.ResizeImage(cimg, (int)(((float)cimg.Width / (float)cimg.Height) * coverSize), (int)coverSize);

            System.Drawing.Bitmap ccbimg;
            MemoryStream istream;
            if (cbimg.Width != cbimg.Height)
            {
                ccbimg = SpritesContent.cropAtRect(cbimg, new System.Drawing.Rectangle((int)((cbimg.Width - coverSize) / 2), 0, (int)coverSize, (int)coverSize));
                istream = SpritesContent.BitmapToStream(ccbimg);
            }
            else
            {
                istream = SpritesContent.BitmapToStream(cbimg);
            }

            coverimg = new Image(ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, (Stream)istream))
            {
                BeatReact = false,
                Position = new Vector2(0, 0),
            };

            coverBox = new FilledRectangle(new Vector2((SpritesContent.Instance.SettingsFont.MeasureString("").X * .8f) + 20, coverSize), Color.Black * .8f)
            {
                Position = new Vector2(coverSize, coverimg.Position.Y)
            };

            coverLabel = new Label(0)
            {
                Text = "",
                Font = SpritesContent.Instance.SettingsFont,
                Position = new Vector2(coverSize + 5, coverimg.Position.Y),
                Scale = .8f
            };

            coverLabelArt = new Label(0)
            {
                Text = "",
                Font = SpritesContent.Instance.SettingsFont,
                Position = new Vector2(coverSize + 15, coverimg.Position.Y + 20),
                Scale = .6f
            };

            coverLabelDiff = new Label(0)
            {
                Text = "",
                Font = SpritesContent.Instance.SettingsFont,
                Position = new Vector2(coverSize + 15, coverimg.Position.Y + 40),
                Scale = .6f
            };

            powerup_guide = new Image(SpritesContent.instance.powerup_guide)
            {
                BeatReact = false,
                Size = new Vector2(ActualScreenMode.Width, PlayerSize.Y),
                Position = Player.Position,
                Visible = true,
                Opacity = 0f,
            };

            Controls.Add(coverBox);
            Controls.Add(coverimg);
            Controls.Add(coverLabel);
            Controls.Add(coverLabelArt);
            Controls.Add(coverLabelDiff);

            //Controls.Add(UserBox.GetInstance());


            Controls.Add(lbpeak);
            Controls.Add(powerup_guide);
            Controls.Add(Player);
            Controls.Add(comboDisplay);
            Controls.Add(_healthbar);
            Controls.Add(_scoreDisplay);

            Controls.Add(particleEngine);
            Controls.Add(replayLabel);
            Controls.Add(skipButton);


            onKeyPress += CatchItMode_onKeyPress;

            OnScroll += CatchItMode_OnScroll;

            skipButton.Click += SkipButton_Click;
        }

        private void SkipButton_Click(object sender, EventArgs e)
        {
            skipButton.Visible = false;
            skip();
        }

        private void skip()
        {
            if (skipped)
                return;

            if (avp.audioplayer.PlayState == BassPlayState.Playing)
            {
                skipped = true;
                skipButton.Visible = false;
                avp.audioplayer.PositionV2 = (long)OriginalHitObjects[0].StartTime - 2000;
                EffectsPlayer.PlayEffect(SpritesContent.Instance.MenuTransition);
            }
        }

        public void showGuide()
        {
            if(powerup_guide.Opacity > 0f) {
                return;
            }
            powerup_guide.Visible = true;
            powerup_guide.Opacity = 0f;
            powerup_guide.FadeIn(AnimationEffect.Linear, 300, () =>
            {
                powerup_guide.FadeOut(AnimationEffect.Linear, p_guide_timer);
            });
        }
        private void _healthbar_OnFail()
        {
            //KyunGame.Instance.Player.Stop();
            //InGame = false;
            //PauseOverlay.ShowFailed(this);
            //ScreenManager.ShowOverlay(PauseOverlay.Instance);

            if (!InGame)
                return;


            InGame = false;
            bool killVel = false;
            KyunGame.Instance.Player.SetVelocity(.5f);
            showingFail = true;
            new Task(() =>
            {
                EffectsPlayer.PlayEffect(SpritesContent.instance.FailTransition);
                return;
                int time = 2000;
                for (int a = time; a > 0; a--)
                {
                    if (killVel)
                    {
                        killVel = false;
                        KyunGame.Instance.Player.SetVelocity(1);
                        break;
                    }


                    if (a == 1950)
                    {

                    }
                    KyunGame.Instance.Player.SetVelocity(((float)a / 1000f / (time / 1000)) / 2f);
                    Thread.Sleep(1);
                }
            }).Start();

            new Task(() =>
            {
                Thread.Sleep(2000);

                killVel = true;
                KyunGame.Instance.Player.SetVelocity(1);

                KyunGame.Instance.Player.Velocity = 1;
                KyunGame.Instance.Player.Stop();



                PauseOverlay.ShowFailed(this);
                ScreenManager.ShowOverlay(PauseOverlay.Instance);
            }).Start();

            for (int c = 0; c < Controls.Count; c++)
            {
                UIObjectBase control = Controls.ElementAt(c);

                if (control is HitBase)
                {
                    control.Died = true;
                    control.Visible = false;

                    HitObjectParticle pr = particleEngine.AddNewHitObjectParticle(control.Texture,
                      new Vector2(2),
                      new Vector2(control.Position.X, control.Position.Y),
                      10,
                      0,
                      Color.White
                      ) as HitObjectParticle;
                    pr.Scale = control.Scale;
                    pr.Velocity = new Vector2(.5f);
                    pr.Opacity = control.Opacity;
                    pr.MoveTo(AnimationEffect.Ease, 2000, new Vector2(control.Position.X + OsuUtils.OsuBeatMap.rnd.Next(-100, 100), ActualScreenMode.Height + pr.Size.Y));
                    pr.TextureColor = Color.Violet;

                    ParticleScore sc = particleEngine.AddNewScoreParticle(SpritesContent.instance.MissTx,
                       new Vector2(.05f),
                       new Vector2(control.Position.X + (control.Texture.Height / 2) - (SpritesContent.instance.MissTx.Width / 2), control.Position.Y + (control.Texture.Height / 2) + (SpritesContent.instance.MissTx.Height + 10) * 1.5f),
                       10,
                       0,
                       Color.White
                       ) as ParticleScore;

                    sc.Scale = control.Scale;
                    sc.Velocity = new Vector2(.5f);
                    sc.Opacity = control.Opacity;
                    sc.MoveTo(AnimationEffect.Ease, 2000, new Vector2(control.Position.X + OsuUtils.OsuBeatMap.rnd.Next(-100, 100), ActualScreenMode.Height + sc.Size.Y));
                    sc.TextureColor = Color.Violet;
                }
            }

        }

        private void CatchItMode_OnScroll(object sender, bool Up, bool touch)
        {
            if (!touch)
            {
                if (Up)
                {
                    if (playerLinePosition > 1)
                    {
                        playerLinePosition--;
                    }
                }
                else
                {
                    if (playerLinePosition < fieldSlots)
                    {
                        playerLinePosition++;
                    }
                }
            }
            else
            {
                if (Up)
                {
                    if (playerLinePosition < fieldSlots)
                    {
                        playerLinePosition++;
                    }
                }
                else
                {
                    if (playerLinePosition > 1)
                    {
                        playerLinePosition--;
                    }

                }
            }
            movements.Add(new ReplayObject { PressedAt = playerLinePosition, LeaveAt = GamePosition });
        }

        private void CatchItMode_onKeyPress(object sender, kyun.GameScreen.InputEvents.KeyPressEventArgs args)
        {

            isKeyDownShift = Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift)
                || GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed
                || GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed
                || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed;

            switch (args.Key)
            {

                case Keys.Up:
                //case Keys.Left:
                case Keys.W:
                case Keys.A:
                    if ((gameMod & GameMod.Auto) != GameMod.Auto)
                    {
                        if (playerLinePosition > 1)
                        {
                            if (isKeyDownShift)
                                playerLinePosition = Math.Max(playerLinePosition - 2, 1);
                            else
                                playerLinePosition--;
                        }
                        movements.Add(new ReplayObject { PressedAt = playerLinePosition, LeaveAt = GamePosition });
                    }

                    break;
                case Keys.Down:
                case Keys.Right:
                case Keys.S:
                case Keys.D:
                    if ((gameMod & GameMod.Auto) != GameMod.Auto)
                    {
                        if (playerLinePosition < fieldSlots)
                        {
                            if (isKeyDownShift)
                                playerLinePosition = Math.Min(playerLinePosition + 2, fieldSlots);
                            else
                                playerLinePosition++;
                        }
                        movements.Add(new ReplayObject { PressedAt = playerLinePosition, LeaveAt = GamePosition });
                    }
                    break;
                case Keys.Escape:
                    togglePause();
                    break;
                case Keys.Space:
                    if (skipButton.Visible)
                        skip();
                    break;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed)
            {
                if (skipButton.Visible)
                    skip();
            }
        }

        private void clearObjects()
        {
            Controls.RemoveAll(isHitBase);
            HitObjectsRemain.Clear();
            HitObjects.Clear();
            particleEngine.Clear();
        }

        public override void Play(IBeatmap beatmap, GameMod GameMods = GameMod.None)
        {
            renderBeat = showingFail = false;
            velocity = SmoothVelocity = 3;
            beatmap.ApproachRate = Math.Min(beatmap.ApproachRate, 10);
            End = false;
            skipped = false;
            GamePosition = -3000;
            lastIndex = objectIndx = 0;
            countToScores = 0;
            _healthbar.Reset();
            _healthbar.Start(0);
            powerup_guide.Opacity = 0f;
            int guide_time = (int)Math.Ceiling(p_guide_timeout * (5f / beatmap.OverallDifficulty));
            p_guide_timer = guide_time > 10000 ? 10000 : guide_time;
            _scoreDisplay.Reset();
            KyunGame.Instance.Player.Stop();
            ChangeBackground(beatmap.Background);
            Beatmap = beatmap;
            OriginalHitObjects = Beatmap.HitObjects.ToArray().ToList();
            _osuMode = beatmap.Osu_Gamemode;
            timeProgressBar.Value = 0;
            timeBarEnd.Position = new Vector2(-timeBarEnd.Size.X, timeBarEnd.Position.Y);
            //Remove

            //OriginalHitObjects = OriginalHitObjects.Distinct(new HitObjectComparer()).ToList();

            //if(beatmap.Creator.ToLower() == "kyun")
            //{
            OriginalHitObjects = checkObjsKyun(OriginalHitObjects, false, false); //Nice
            AVPlayer.videoplayer.Stop();                                              //}

            //



            sampleSet?.CleanUp();
            _osuBeatmapSkin?.CleanUp();
            try
            {
                FileInfo sampleInfo = new FileInfo(KyunGame.Instance.SelectedBeatmap.SongPath);

                sampleSet = CustomSampleSet.LoadFromBeatmap(sampleInfo.DirectoryName);
            }
            catch
            {
                sampleSet = null;
                Logger.Instance.Info("Error getting sampleset");
            }

            try
            {
                FileInfo skinInfo = new FileInfo(KyunGame.Instance.SelectedBeatmap.SongPath);

                _osuBeatmapSkin = osuBeatmapSkin.FromFile(skinInfo.DirectoryName);
            }
            catch
            {
                _osuBeatmapSkin = null;
                Logger.Instance.Info("Error getting skin files");
            }


            playerLinePosition = 1;
            lastIndex = 0;
            _scoreDisplay.IsActive = true;
            changeCatcherTx(PlayerTxType.Idle);
            timeToPlayerGlow = 0;
            lastTime = 0;
            if ((GameMods & GameMod.Replay) != GameMod.Replay)
                movements.Clear();

            mid = 0;
            FieldSize = new Vector2(ActualScreenMode.Width - 20, (PlayerSize.Y + Margin) * fieldSlots);
            //FieldPosition = new Vector2(0, (ActualScreenMode.Height / 2) - (FieldSize.Y / 2) - PlayerSize.Y);
            FieldPosition = new Vector2(10, 0);

            _healthbar.Position = new Vector2(0, FieldPosition.Y + FieldSize.Y + 2 + PlayerSize.Y);
            _scoreDisplay.Position = new Vector2(0, _healthbar.Position.Y + 48);

            _healthbar.IsActive = true;
            gameMod = GameMods;
            Combo.Instance.ResetAll();

            generateNotes = false;

            if (OriginalHitObjects.Count() < 2)
            {
                OriginalHitObjects = new List<IHitObj>();
                generateNotes = true;
            }

            if ((gameMod & GameMod.Replay) == GameMod.Replay && (gameMod & GameMod.Auto) != GameMod.Auto)
            {
                replayLabel.Text = "REPLAY";
                if (!string.IsNullOrWhiteSpace(replay.Username))
                    replayLabel.Text += " - " + replay.Username;
                replayLabel.Visible = true;

            }
            else if ((gameMod & GameMod.Auto) == GameMod.Auto)
            {
                replayLabel.Visible = true;
                if ((gameMod & GameMod.Replay) == GameMod.Replay)
                {
                    replayLabel.Text = "AUTO - REPLAY";
                }
                else
                {
                    replayLabel.Text = "AUTO";
                }
            }
            else
            {
                replayLabel.Visible = false;
            }


            

            KyunGame.Instance.discordHandler.SetState("Catching things", $"{Beatmap.Artist} - {Beatmap.Title}", "idle_large", "classic_small");
            clearObjects();
            generating = false;
            generatingBg = false;
            if (generateNotes)
            {
                startGenerating(Beatmap);
            }

            changeDisplayPanel();
            NonInheritedPoint = beatmap.GetTimingPointFor(0, false);
            InheritedPoint = beatmap.GetTimingPointFor(0, true);
            InGame = true;
        }

        void changeDisplayPanel()
        {

            changeCoverDisplay(Beatmap.Background);
            float titleSize = 50;
            float artSize = 50;
            float diffsize = 50;

            try
            {
                titleSize = SpritesContent.Instance.SettingsFont.MeasureString(Beatmap.Title).X;
                artSize = SpritesContent.Instance.SettingsFont.MeasureString(Beatmap.Artist).X;
                diffsize = SpritesContent.Instance.SettingsFont.MeasureString(Beatmap.Version).X;
            }
            catch
            {
                titleSize = SpritesContent.Instance.MSGothic2.MeasureString(Beatmap.Title).X;
                artSize = SpritesContent.Instance.MSGothic2.MeasureString(Beatmap.Artist).X;
                diffsize = SpritesContent.Instance.MSGothic2.MeasureString(Beatmap.Version).X;

            }


            float maxSize = Math.Max(titleSize, artSize);
            maxSize = Math.Max(maxSize, diffsize);
            coverBox.Resize(new Vector2((maxSize * .8f) + 20, coverSize));
            coverLabel.Text = Beatmap.Title;
            coverLabelArt.Text = Beatmap.Artist;

            coverLabelDiff.Text = "[ " + Beatmap.Version + " ]";
        }

        private void startGenerating(IBeatmap beatmap)
        {

        }

        public void Play(IBeatmap beatmap, GameMod GameMods, int slots)
        {
            fieldSlots = slots;
            Play(beatmap, GameMods);
        }

        public override void Play(IBeatmap beatmap, GameMod GameMods, Replay _replay)
        {
            replay = _replay;
            GameMods |= GameMod.Replay;
            Play(Beatmap, GameMods);
        }

        float lastpeak = 0;
        float max1, max2, max3, max4;
        bool sust1, sust2, sust3, sust4;

        public int objectIndx = 0;
        private int lastPlayerLinePosition;
        private Image coverimg;
        private FilledRectangle coverBox;
        private Label coverLabel;
        private Label coverLabelArt;
        private Label coverLabelDiff;
        private KeyboardState kbShiftState;
        private KeyboardState kbShiftOldState;

        internal virtual void checkObjects()
        {

            if (!InGame) return;

            if (InheritedPoint == null)
            {

            }
            else
            {

                float sliderVelocityInOsuFormat = NonInheritedPoint.MsPerBeat;

                if (NonInheritedPoint.MsPerBeat < -250)
                {
                    NonInheritedPoint.MsPerBeat = -200;
                }

                if (sliderVelocityInOsuFormat >= 0)
                {
                    //sliderVelocityInOsuFormat = -100f; //DEFAUL VALUE FOR OSU
                    if (Velocity == 0)
                        Velocity = Beatmap.SliderMultiplier * 2f;

                    Velocity = Math.Max(Velocity, 6f);
                }
                else
                {
                    Velocity = (float)Math.Abs(100f / sliderVelocityInOsuFormat);
                    //Velocity = (float)Math.Abs(sliderVelocityInOsuFormat / 100f);
                    Velocity *= (Beatmap.SliderMultiplier * 2f);
                    Velocity = Math.Max(Velocity, 6f);

                }
            }


            if (InGame && GamePosition > 0 && KyunGame.Instance.Player.PlayState == BassPlayState.Stopped)
            {
                GamePosition = 0;
                KyunGame.Instance.Player.Play(Beatmap.SongPath, ((gameMod & GameMod.DoubleTime) == GameMod.DoubleTime) ? 1.5f : 1f);
                ActualTimingPoint = Beatmap.TimingPoints[0];
                NextTimingPoint = Beatmap.GetNextTimingPointFor(ActualTimingPoint.Offset + 50);
                KyunGame.Instance.Player.Volume = KyunGame.Instance.GeneralVolume;

                if (System.IO.File.Exists(Beatmap.Video))
                {
                    if (System.IO.File.GetAttributes(Beatmap.Video) != System.IO.FileAttributes.Directory)
                    {
                        if (Settings1.Default.Video)
                        {
                            
                            AVPlayer.VideoOffset = (int)Beatmap.VideoStartUp;
                            AVPlayer.videoplayer.Play(Beatmap.Video);
                        }
                    }
                }

                if ((long)OriginalHitObjects.First().StartTime > 3500)
                    skipButton.Visible = true;
                else
                    skipButton.Visible = false;
            }



            if (lastIndex >= OriginalHitObjects.Count)
            {
                InGame = false;
                return;
            }

            long actualTime = GamePosition;


            IHitObj lastObject = OriginalHitObjects[lastIndex];


            if (!skipped && actualTime > OriginalHitObjects[0].StartTime - 3000)
            {
                skipped = true;

            }
            skipButton.Visible = !skipped;

            long approachStart = (long)(ModeConstants.APPROACH_TIME_BASE - Beatmap.ApproachRate * 150f) + (long)(SmoothVelocity * 1000) + 5000;

            long nextObjStart = (long)lastObject.StartTime - approachStart;


            if (actualTime > nextObjStart)
            {
                if (Math.Abs(lastObject.StartTime - lastTime) > 20)
                {
                    if (lastObject is HitButton)
                    {

                        if (lastObject.isFakeOBj)
                        {
                            var obj = new FakeHitObject(lastObject, Beatmap, this);
                            obj.ReplayId = lastIndex;
                            HitObjects.Add(obj);
                            HitObjectsRemain.Add(obj);
                            Controls.Add(obj);
                        }
                        else
                        {
                            var obj = new HitObject(lastObject, Beatmap, this);
                            obj.ReplayId = lastIndex;
                            HitObjects.Add(obj);
                            HitObjectsRemain.Add(obj);
                            Controls.Add(obj);
                        }

                    }
                    else
                    {

                        if (!lastObject.isFakeOBj)
                        {
                            var obj2 = new HitObject(lastObject, Beatmap, this, false);
                            //obj2.TextureColor = Color.Red;

                            obj2.ReplayId = lastIndex;
                            HitObjects.Add(obj2);
                            HitObjectsRemain.Add(obj2);

                            var obj = new HitObject(lastObject, Beatmap, this, false, true, obj2);
                            obj.ReplayId = lastIndex;

                            HitObjects.Add(obj);
                            HitObjectsRemain.Add(obj);
                            Controls.Add(obj);

                            Controls.Add(obj2);


                            objectIndx++;
                        }
                        else
                        {
                            var obj2 = new FakeHitObject(lastObject, Beatmap, this, false);
                            //obj2.TextureColor = Color.Red;

                            obj2.ReplayId = lastIndex;
                            HitObjects.Add(obj2);
                            HitObjectsRemain.Add(obj2);

                            var obj = new FakeHitObject(lastObject, Beatmap, this, false, false);
                            obj.ReplayId = lastIndex;

                            HitObjects.Add(obj);
                            HitObjectsRemain.Add(obj);
                            Controls.Add(obj);


                            Controls.Add(obj2);
                            objectIndx++;
                        }
                    }

                    objectIndx++;
                }

                lastTime = (long)lastObject.StartTime;
                lastIndex++;
            }


        }

        private void togglePause()
        {
            if (showingFail)
            {
                return;
            }

            if (KyunGame.Instance.Player.PlayState == BassPlayState.Paused)
                return;

            KyunGame.Instance.Player.Pause();
            ScreenManager.ShowOverlay(PauseOverlay.Instance);
            PauseOverlay.ShowAlert(this);
        }

        private void updateCatcher()
        {
            if (Combo.Instance.ActualMultiplier > 200)
            {
                changeCatcherTx(PlayerTxType.Fire);
            }
            else if (Combo.Instance.ActualMultiplier > 80)
            {
                changeCatcherTx(PlayerTxType.Combo);
            }

            if (catcherElapsed > Math.Max(InheritedPoint.MsPerBeat, 1000f / 150f))
            {
                catcherElapsed = 0;
                if (catcherState == 0)
                {
                    catcherState = 1;
                }
                else
                {
                    catcherState = 0;
                }

                changeCatcherTx(txState);
            }

            if (catcherToIdle > 5000 && txState == PlayerTxType.Miss)
            {
                catcherToIdle = 0;
                changeCatcherTx(PlayerTxType.Idle);
            }
        }

        public override void Update(GameTime tm)
        {


            if (NextTimingPoint == null)
            {
                ActualTimingPoint = Beatmap.TimingPoints[0];
                NextTimingPoint = Beatmap.GetNextTimingPointFor(ActualTimingPoint.Offset + 50);
            }
            else
            {
                if (AVPlayer.audioplayer.PositionV2 >= NextTimingPoint.Offset - 50)
                {
                    ActualTimingPoint = NextTimingPoint;/*Beatmap.GetTimingPointForV2(AVPlayer.audioplayer.PositionV2 + 50);*/
                    NextTimingPoint = Beatmap.GetNextTimingPointFor(ActualTimingPoint.Offset + 50);
                }
            }
            renderBeat = ActualTimingPoint.KiaiMode;
            if (!ActualTimingPoint.Inherited)
            {
                NonInheritedPoint = ActualTimingPoint;
            }
            else
            {
                InheritedPoint = ActualTimingPoint;
            }

            if (smoothingVel)
            {
                if (SmoothVelocity != velocity)
                {


                    if (SmoothVelocity > velocity)
                    {
                        SmoothVelocity = Math.Max(velocity, SmoothVelocity - tm.ElapsedGameTime.Milliseconds * (Math.Max(InheritedPoint.MsPerBeat, 150) / 100000f));
                    }
                    else if (SmoothVelocity < velocity)
                    {
                        SmoothVelocity = Math.Min(velocity, SmoothVelocity + tm.ElapsedGameTime.Milliseconds * (Math.Max(InheritedPoint.MsPerBeat, 150) / 100000f));
                    }
                }
                else
                {
                    smoothingVel = false;
                }
            }

            if (!generatingBg)
            {
                if (KyunGame.Instance.Player.PlayState == BassPlayState.Stopped && !generateNotes)
                {
                    GamePosition += (long)KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds;
                }
                else
                {
                    GamePosition = KyunGame.Instance.Player.PositionV2 + Beatmap.SleepTime;
                    timeProgressBar.Value = ((float)KyunGame.Instance.Player.PositionV2 / (float)/*KyunGame.Instance.Player.Length*/Beatmap.HitObjects.Last().EndTime * 100f);
                    timeBarEnd.Position = new Vector2(timeProgressBar.Size.X - 10, timeBarEnd.Position.Y);
                }
            }


            catcherElapsed += tm.ElapsedGameTime.Milliseconds;
            catcherToIdle += tm.ElapsedGameTime.Milliseconds;

            timeToPlayerGlow += tm.ElapsedGameTime.Milliseconds;
            //isKeyDownShift = Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift);
            /*
            if ((gameMod & GameMod.Replay) != GameMod.Replay)
            {
                kbShiftState = Keyboard.GetState();

                if (kbShiftOldState.IsKeyUp(Keys.LeftShift) && kbShiftState.IsKeyDown(Keys.LeftShift))
                {
                    movements.Add(new ReplayObject { PressedAt = -1, LeaveAt = GamePosition });
                }

                if (kbShiftOldState.IsKeyUp(Keys.RightShift) && kbShiftState.IsKeyDown(Keys.RightShift))
                {
                    movements.Add(new ReplayObject { PressedAt = -1, LeaveAt = GamePosition });
                }

                if (kbShiftOldState.IsKeyDown(Keys.LeftShift) && kbShiftState.IsKeyUp(Keys.LeftShift))
                {
                    movements.Add(new ReplayObject { PressedAt = 0, LeaveAt = GamePosition });
                }

                if (kbShiftOldState.IsKeyDown(Keys.RightShift) && kbShiftState.IsKeyUp(Keys.RightShift))
                {
                    movements.Add(new ReplayObject { PressedAt = 0, LeaveAt = GamePosition });
                }

                kbShiftOldState = kbShiftState;
            }*/

            if (timeToPlayerGlow > 12) //12ms
            {
                timeToPlayerGlow = 0;
                if (isKeyDownShift)
                {
                    Particle particle = particleEngine.AddNewHitObjectParticle(Player.Texture, Vector2.Zero, Player.Position, 30, 0, Color.White);
                    particle.Opacity = .5f;
                    //particle.AngleRotation = Player.AngleRotation;
                    particle.TextureColor = Color.Yellow;
                    particle.Size = new Vector2(PlayerSize.X, PlayerSize.Y);
                    particle.Scale = Player.Scale;
                }
                isKeyDownShift = Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift);
            }
            base.Update(tm);

            updatePlayer();
            checkObjects();
            updateCatcher();

            Controls.RemoveAll(isDed);
            HitObjectsRemain.RemoveAll(item => item.Died);

            bool decoding = false;
            if (FFmpegDecoder.Instance != null)
            {
                decoding = FFmpegDecoder.Instance.Decoding;
            }

            if (OriginalHitObjects.Count < 1)
            {
                return;
            }

            IHitObj last = OriginalHitObjects.Last();

            endToTime = 3000;

            int hitCount = HitObjectsRemain.Count;

            if (lastIndex >= OriginalHitObjects.Count && hitCount < 1)
            {
                End = true;

            }

            if (End && countToScores < endToTime)
            {
                countToScores += KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;
            }

            if (End && countToScores >= endToTime)
            {

                ScreenManager.ChangeTo(ScorePanel.Instance);
                ScorePanel.Instance.CalcScore(this);

                //KyunGame.Instance.Player.Play(Beatmap.SongPath);
                End = false;
                countToScores = 0;
            }
        }

        private void updatePlayer()
        {
            //lbpeak.Text = playerLinePosition + " | " + lastPlayerLinePosition;
            if (lastPlayerLinePosition == playerLinePosition && (gameMod & GameMod.Replay) != GameMod.Replay)
            {
                return;
            }

            if ((gameMod & GameMod.Replay) == GameMod.Replay)
            {
                if (replay.Hits.Count > 0)
                {
                    if (mid < replay.Hits.Count)
                    {
                        if (replay.Hits[mid].PressedAt > 0)
                        {
                            if (GamePosition >= replay.Hits[mid].LeaveAt)
                            {

                                playerLinePosition = (int)replay.Hits[mid].PressedAt;
                                mid++;
                            }
                        }
                        else
                        {
                            if (replay.Hits[mid].PressedAt == 0)
                            {
                                isKeyDownShift = false;
                            }
                            else if (replay.Hits[mid].PressedAt == -1)
                            {
                                isKeyDownShift = true;
                            }
                        }
                    }
                }
            }

            inFieldPosition = (int)(FieldSize.Y / fieldSlots);

            inFieldPosition *= playerLinePosition;

            //Player.Position = new Vector2(FieldPosition.X, inFieldPosition + FieldPosition.Y);
            int anDuration = 50;
            if (Beatmap.OverallDifficulty > 5)
            {
                anDuration = 25;
            }

            if (lastPlayerLinePosition > playerLinePosition)
            {
                Player.AngleRotation = 12;
            }
            else if ((gameMod & GameMod.Replay) == GameMod.Replay)
            {
                Player.AngleRotation = 0;
            }
            else
            {
                Player.AngleRotation = -12;
            }
            Player.MoveTo(AnimationEffect.Ease, anDuration, new Vector2(FieldPosition.X, inFieldPosition), () =>
            {
                Player.AngleRotation = 0;
            });
            lastPlayerLinePosition = playerLinePosition;

            //powerup_guide.MoveTo(AnimationEffect.Ease, anDuration, new Vector2(0, inFieldPosition), ()=>{ });
            powerup_guide.Position = new Vector2(0, inFieldPosition);
        }

        public void changeCatcherTx(PlayerTxType type)
        {
            switch (type)
            {
                case PlayerTxType.Idle:
                    txState = type;
                    Player.Texture = SpritesContent.Instance.Catcher;

                    break;
                case PlayerTxType.Combo:
                    txState = type;

                    if (catcherState == 0)
                        Player.Texture = SpritesContent.Instance.CatcherCombo;
                    else
                        Player.Texture = SpritesContent.Instance.CatcherCombo2;

                    break;
                case PlayerTxType.Fire:
                    txState = type;

                    if (catcherState == 0)
                        Player.Texture = SpritesContent.Instance.CatcherFire;
                    else
                        Player.Texture = SpritesContent.Instance.CatcherFire2;

                    break;
                case PlayerTxType.Miss:
                    txState = type;

                    if (catcherState == 0)
                        Player.Texture = SpritesContent.Instance.CatcherMiss;
                    else
                        Player.Texture = SpritesContent.Instance.CatcherMiss2;

                    break;
            }
            Player.Texture = Player.Texture;
        }

        public override void Render()
        {
            base.Render();
            foreach (HitBase hitobj in HitObjects)
            {
                if (hitobj.Texture == SpritesContent.instance.MenuSnow && !hitobj.Died)
                {
                    hitobj?.Render();
                }
            }
        }

        public List<IHitObj> checkObjsKyun(List<IHitObj> itms, bool addFake = false, bool maze = false)
        {
            List<IHitObj> newObs = new List<IHitObj>();

            var length = itms.Count;



            for (int a = 0; a < length; a++)
            {
                try
                {
                    if (a == 0 && !maze)
                    {
                        newObs.Add(itms[a]);
                        continue;
                    }

                    var item = itms[a];
                    var startTime = item.StartTime;
                    var endTime = item.EndTime;
                    if (item is HitButton)
                    {
                        endTime = item.StartTime;
                    }

                    var lastStartTime = item.StartTime;
                    var lastEndTime = item.EndTime;
                    if (newObs.Count > 0)
                    {
                        lastStartTime = newObs.Last().StartTime;
                        lastEndTime = newObs.Last().EndTime;

                        if (newObs.Last() is HitButton)
                        {
                            lastEndTime = newObs.Last().StartTime;
                        }
                    }

                    var objectTooClose = false;

                    if (startTime - lastEndTime < 20)
                    {
                        objectTooClose = true;
                    }

                    if (objectTooClose == true && !maze)
                    {
                        continue;
                    }

                    if (!maze)
                    {
                        if (item is HitButton && addFake && item.StartTime % 6 == 0)
                        {
                            itms[a].isFakeOBj = true;
                        }
                    }
                    else
                    {
                        itms[a].isFakeOBj = true;
                    }

                    newObs.Add(itms[a]);
                }
                catch { continue; }
            }

            return newObs;
        }

        public void changeCoverDisplay(string image)
        {

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
            if (cbimg.Width != cbimg.Height)
            {
                ccbimg = SpritesContent.cropAtRect(cbimg, new System.Drawing.Rectangle((int)((cbimg.Width - coverSize) / 2), 0, (int)coverSize, (int)coverSize));
                istream = SpritesContent.BitmapToStream(ccbimg);
            }
            else
            {
                istream = SpritesContent.BitmapToStream(cbimg);
            }

            coverimg.Texture = ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, istream);
        }
    }

    public enum PlayerTxType
    {
        Idle,
        Combo,
        Fire,
        Miss
    }



    class HitObjectComparer : IEqualityComparer<IHitObj>
    {
        public bool Equals(IHitObj x, IHitObj y)
        {
            if (x is HitSlider)
            {
                if (x.EndTime > x.BeatmapContainer.HitObjects.Last().EndTime)
                {
                    x.EndTime = x.BeatmapContainer.HitObjects.Last().EndTime;
                }
            }

            if (y is HitSlider)
            {
                if (y.EndTime > x.BeatmapContainer.HitObjects.Last().EndTime)
                {
                    y.EndTime = x.BeatmapContainer.HitObjects.Last().EndTime;
                }
            }

            if (x.StartTime == y.StartTime)
            {
                return true;
            }
            else if (Math.Abs(x.StartTime - y.StartTime) < 50) //25
            {
                return true;
            }
            else if (Math.Abs(x.EndTime - y.StartTime) < 50)
            {
                return true;
            }
            else if (Math.Abs(x.StartTime - y.EndTime) < 50)
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(IHitObj obj)
        {
            return obj.StartTime.GetHashCode() ^ obj.EndTime.GetHashCode();
        }

    }
}
