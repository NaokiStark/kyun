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

namespace kyun.game.GameModes.CatchIt
{
    public class CatchItMode : GameModeScreenBase
    {
        static CatchItMode Instance;
        internal int lastIndex;

        public Image Player;
        public Vector2 PlayerSize;
        public int playerLinePosition = 1;
        public int fieldSlots = 4;

        public Vector2 FieldSize = new Vector2();
        public Vector2 FieldPosition = new Vector2();
        internal FilledRectangle fg;
        public int Margin = 2;

        internal List<FilledRectangle> rectangles = new List<FilledRectangle>();
        internal List<HitObject> HitObjectsRemain = new List<HitObject>();

        internal List<kyun.Beatmap.IHitObj> OriginalHitObjects = new List<kyun.Beatmap.IHitObj>();

        public OsuGameMode _osuMode { get; private set; }

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

        internal PlayerTxType txState;
        internal long catcherElapsed = 0;
        internal int catcherState = 0;
        public long catcherToIdle = 0;

        public Texture2D LongTail { get; set; }

        public bool End { get; private set; }
        public float FailsCount { get { return 1; } }

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

        internal long lastTime = 0;

        public static CatchItMode GetInstance()
        {
            if (Instance == null)
                Instance = new CatchItMode();
            return Instance;
        }

        public CatchItMode() : base("CatchIt")
        {
            AllowVideo = true;
            mid = 0;
            HitObjects = new List<HitBase>();
            BackgroundDim = .9f;

            Player = new Image(SpritesContent.Instance.Catcher);
            Player.BeatReact = true;
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

            Controls.Add(UserBox.GetInstance());
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
                avp.audioplayer.Position = (long)OriginalHitObjects[0].StartTime - 3000;
                EffectsPlayer.PlayEffect(SpritesContent.Instance.MenuTransition);
            }
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
            new Task(() =>
            {
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
                        EffectsPlayer.PlayEffect(SpritesContent.instance.FailTransition);
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
                    pr.MoveTo(AnimationEffect.Linear, 60000, new Vector2(control.Position.X, control.Position.Y + 60));
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
                    sc.MoveTo(AnimationEffect.Linear, 60000, new Vector2(control.Position.X, control.Position.Y + 50));
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

            isKeyDownShift = Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift);

            switch (args.Key)
            {
                case Keys.Up:
                case Keys.Left:
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
            velocity = SmoothVelocity = 3;
            beatmap.ApproachRate = Math.Min(beatmap.ApproachRate, 10);
            End = false;
            skipped = false;
            GamePosition = 0;
            lastIndex = 0;
            countToScores = 0;
            _healthbar.Reset();
            _healthbar.Start(0);
            _scoreDisplay.Reset();
            KyunGame.Instance.Player.Stop();
            ChangeBackground(KyunGame.Instance.SelectedBeatmap.Background);
            Beatmap = beatmap;
            OriginalHitObjects = Beatmap.HitObjects.ToArray().ToList();
            _osuMode = beatmap.Osu_Gamemode;
            //Remove

            OriginalHitObjects = OriginalHitObjects.Distinct(new HitObjectComparer()).ToList();

            //

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


            if (System.IO.File.Exists(beatmap.Video))
            {
                if (System.IO.File.GetAttributes(beatmap.Video) != System.IO.FileAttributes.Directory)
                {
                    if (Settings1.Default.Video)
                    {
                        AVPlayer.videoplayer.Stop();
                        AVPlayer.videoplayer.Play(Beatmap.Video);
                    }
                }
            }
            KyunGame.Instance.discordHandler.SetState("Catching things", $"{Beatmap.Artist} - {Beatmap.Title}", "idle_large", "classic_small");
            clearObjects();

            InGame = true;
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

        internal virtual void checkObjects()
        {

            if (!InGame) return;
            InheritedPoint = Beatmap.GetInheritedPointFor(AVPlayer.audioplayer.Position);
            if (InheritedPoint == null)
            {

            }
            else
            {

                float sliderVelocityInOsuFormat = InheritedPoint.MsPerBeat;

                if (InheritedPoint.MsPerBeat < -250)
                {
                    InheritedPoint.MsPerBeat = -200;
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


            if (InGame && GamePosition > Beatmap.SleepTime && KyunGame.Instance.Player.PlayState == BassPlayState.Stopped)
            {
                KyunGame.Instance.Player.Play(Beatmap.SongPath, ((gameMod & GameMod.DoubleTime) == GameMod.DoubleTime) ? 1.5f : 1f);

                KyunGame.Instance.Player.Volume = KyunGame.Instance.GeneralVolume;

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

                        var obj = new HitObject(lastObject, Beatmap, this);
                        obj.ReplayId = lastIndex;
                        HitObjects.Add(obj);
                        HitObjectsRemain.Add(obj);
                        Controls.Add(obj);
                    }
                    else
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
                    }
                }

                lastTime = (long)lastObject.StartTime;
                lastIndex++;
            }

        }

        private void togglePause()
        {
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

            if (catcherElapsed > Math.Max(NonInheritedPoint.MsPerBeat, 1000f / 150f))
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
            NonInheritedPoint = Beatmap.GetTimingPointFor(AVPlayer.audioplayer.Position, false);
            if (smoothingVel)
            {
                if (SmoothVelocity != velocity)
                {


                    if (SmoothVelocity > velocity)
                    {
                        SmoothVelocity = Math.Max(velocity, SmoothVelocity - tm.ElapsedGameTime.Milliseconds * (Math.Max(NonInheritedPoint.MsPerBeat, 150) / 100000f));
                    }
                    else if (SmoothVelocity < velocity)
                    {
                        SmoothVelocity = Math.Min(velocity, SmoothVelocity + tm.ElapsedGameTime.Milliseconds * (Math.Max(NonInheritedPoint.MsPerBeat, 150) / 100000f));
                    }
                }
                else
                {
                    smoothingVel = false;
                }
            }

            if (KyunGame.Instance.Player.PlayState == BassPlayState.Stopped)
                GamePosition += (long)KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds;
            else
                GamePosition = KyunGame.Instance.Player.Position + Beatmap.SleepTime;

            catcherElapsed += tm.ElapsedGameTime.Milliseconds;
            catcherToIdle += tm.ElapsedGameTime.Milliseconds;

            timeToPlayerGlow += tm.ElapsedGameTime.Milliseconds;
            //isKeyDownShift = Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift);

            if (timeToPlayerGlow > 12) //12ms
            {
                timeToPlayerGlow = 0;
                if (isKeyDownShift)
                {
                    Particle particle = particleEngine.AddNewHitObjectParticle(Player.Texture, Vector2.Zero, Player.Position, 30, 0, Color.White);
                    particle.Opacity = .5f;
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
            if(FFmpegDecoder.Instance != null)
            {
                decoding = FFmpegDecoder.Instance.Decoding;
            }


            endToTime = (Settings1.Default.Video && !decoding) ? avp.audioplayer.Length - (long)OriginalHitObjects.Last().EndTime : 3000;

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
            if ((gameMod & GameMod.Replay) == GameMod.Replay)
            {
                if (replay.Hits.Count > 0)
                {
                    if (mid < replay.Hits.Count)
                    {
                        if (GamePosition >= replay.Hits[mid].LeaveAt)
                        {

                            playerLinePosition = (int)replay.Hits[mid].PressedAt;
                            mid++;
                        }
                    }
                }
            }

            int inFieldPosition = (int)(FieldSize.Y / fieldSlots);

            inFieldPosition *= playerLinePosition;

            Player.Position = new Vector2(FieldPosition.X, inFieldPosition + FieldPosition.Y);
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
                return true;
            else if (Math.Abs(x.StartTime - y.StartTime) < 25)
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(IHitObj obj)
        {
            return obj.StartTime.GetHashCode();
        }

    }
}
