using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using kyun.Beatmap;
using kyun.GameScreen;
using System.Threading;
using kyun.Utils;
using kyun.Audio;
using Microsoft.Xna.Framework.Input;
using kyun.GameScreen.UI.Particles;
using kyun.Overlay;
using kyun.Score;
using kyun.GameScreen.UI;
using System.Threading.Tasks;
using kyun.GameScreen.UI.Buttons;
using osuBMParser;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using kyun.game.GameScreen.UI;
using kyun.game.Beatmap;
using System.IO;

namespace kyun.GameModes.OsuMode
{
    public class OsuMode : GameModeScreenBase
    {

        int lastIndex = 0;
        HashSet<HitBase> hitbaseObjects = new HashSet<HitBase>();
        bool End = false;

        public new static OsuMode Instance;
        private int timeToLeave;
        private bool mousePressedLeft;
        private bool mousePressedRight;
        private bool keypressedZ;
        private bool keypressedX;
        public HealthBar _healthBar;
        private ButtonStandard skipButton;
        private bool skipped;
        internal List<Beatmap.IHitObj> OriginalHitObjects = new List<Beatmap.IHitObj>();
        public ParticleEngine _particleEngine { get; private set; }
        public float FailsCount { get; set; }
        public Image AutoCursor { get; private set; }

        public List<KeyValuePair<long, Vector2>> RecordedMousePositions { get; set; }
        public TimingPoint ActualTimingPoint { get; set; }
        public TimingPoint NextTimingPoint { get; private set; }
        public int cursorFrame { get; private set; }
        public bool allowTrail { get; private set; }
        public Image followPointStart { get; set; }
        public Image followPointEnd { get; set; }
        public Image followPointTail { get; set; }

        public CustomSampleSet sampleSet;
        float elapsedMsToMouse = 0;
        static float MOUSE_FRAMERATE_MS = 1000 / 60;
        internal ProgressBar timeProgressBar;
        int lastAutoId = 0;
        public Replay replay;
        public bool showingFail;
        int replayMouseIndex = 0;

        public bool MoveEnabled;

        public OsuGameMode _osuMode = OsuGameMode.Standard;

        public int comboNumber = 1;

        private Color[] comboColors = new Color[] {
            Color.FromNonPremultiplied(164, 100, 240, 255),
            Color.FromNonPremultiplied(230, 110, 150, 255),
            Color.FromNonPremultiplied(100, 200, 200, 255),
            Color.FromNonPremultiplied(130, 120, 255, 255),
        };
        private Color comboColor = Color.Red;
        private int comboColorIndex = 0;

        public Vector2 comboTxSize1;
        public Vector2 comboTxSize2;
        public Vector2 comboTxSize3;
        public Vector2 comboTxSize4;
        public static OsuMode GetInstance()
        {
            if (Instance == null)
                Instance = new OsuMode();
            return Instance;
        }


        public OsuMode()
            : base("OsuMode")
        {
            comboTxSize1 = SpritesContent.instance.ScoreBig.MeasureString("0");
            comboTxSize2 = SpritesContent.instance.ScoreBig.MeasureString("00");
            comboTxSize3 = SpritesContent.instance.ScoreBig.MeasureString("000");
            comboTxSize4 = SpritesContent.instance.ScoreBig.MeasureString("0000");

            renderBeat = false;
            Instance = this;

            ChangeBackground(KyunGame.Instance.SelectedBeatmap.Background);
            onKeyPress += (obj, args) =>
            {

                if (args.Key == Microsoft.Xna.Framework.Input.Keys.Escape)
                {
                    if (showingFail)
                    {
                        return;
                    }
                    togglePause();
                }

                if (KyunGame.Instance.Player.Paused && args.Key == Microsoft.Xna.Framework.Input.Keys.F2)
                {
                    ScreenManager.ChangeTo(BeatmapScreen.Instance);
                }

                if (args.Key == Keys.Space)
                {
                    if (skipButton.Visible)
                    {
                        skip();
                    }
                }


            };

            HitObjects = new List<HitBase>();

            _healthBar = new HealthBar(this, SpritesContent.Instance.Healthbar.Width - 5, 25);

            _healthBar.Position = new Vector2(0);

            _healthBar.OnFail += _healthbar_OnFail;

            _scoreDisplay = new ScoreDisplay(1.1f);

            _scoreDisplay.Position = new Vector2(0, _healthBar.BgBar.Height + 5);

            _particleEngine = new ParticleEngine();

            AutoCursor = new Image(SpritesContent.Instance.GameCursor)
            {
                Visible = false,
                Scale = .9f,
                BeatReact = false
            };

            skipButton = new ButtonStandard(Color.PaleVioletRed)
            {
                Caption = "Skip",
                Position = new Vector2(ActualScreenMode.Width - SpritesContent.Instance.ButtonDefault.Width, ActualScreenMode.Height - SpritesContent.Instance.ButtonDefault.Height),
                Visible = false
            };
            timeProgressBar = new ProgressBar(ActualScreenMode.Width, 20)
            {
                Position = new Vector2(0, ActualScreenMode.Height - 20),
            };
            Microsoft.Xna.Framework.Graphics.Texture2D followPoint = new Microsoft.Xna.Framework.Graphics.Texture2D(KyunGame.Instance.GraphicsDevice, 2, 2);
            followPoint.SetData(new Color[] { Color.White, Color.White, Color.White, Color.White });
            followPointStart = new Image(followPoint);
            followPointEnd = new Image(followPoint);
            followPointStart.Visible = followPointEnd.Visible = false;

            skipButton.Click += SkipButton_Click;

            OnMouseMove += OsuMode_OnMouseMove;
            Controls.Add(_scoreDisplay);

            Controls.Add(_healthBar);
            //Controls.Add(UserBox.GetInstance());
            Controls.Add(_particleEngine);
            Controls.Add(new ComboDisplay());
            Controls.Add(AutoCursor);
            Controls.Add(replayLabel);
            Controls.Add(skipButton);
            Controls.Add(timeProgressBar);
            Controls.Add(followPointStart);
            Controls.Add(followPointEnd);
        }

        private void OsuMode_OnMouseMove(object sender, EventArgs e)
        {
            return;

            if ((gameMod & GameMod.Auto) == GameMod.Auto || (gameMod & GameMod.Replay) == GameMod.Replay)
            {
                return; //We don't need to record mouse move 
            }

            recordMovement(MouseHandler.GetState().Position);
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
                avp.audioplayer.Position = (long)OriginalHitObjects.First().StartTime - 2000;
                EffectsPlayer.PlayEffect(SpritesContent.Instance.MenuTransition);
            }
        }

        /// <summary>
        /// Start game
        /// </summary>
        /// <param name="beatmap"></param>
        public override void Play(IBeatmap beatmap, GameMod GameMods = GameMod.None)
        {
            if ((GameMods & GameMod.Auto) == GameMod.Auto || (GameMods & GameMod.Replay) == GameMod.Replay)
            {

                AutoCursor.Visible = true;
                lastAutoId = 0;
                replayMouseIndex = 0;
                KyunGame.Instance.discordHandler.SetState("Watching kyun! clicking circles", $"{beatmap.Artist} - {beatmap.Title} [{beatmap.Version}]", "idle_large", "classic_small");
            }
            else
            {
                AutoCursor.Visible = false;
                KyunGame.Instance.discordHandler.SetState("Clicking circles", $"{beatmap.Artist} - {beatmap.Title} [{beatmap.Version}]", "idle_large", "classic_small");

            }

            showingFail = false;
            lastSlider = null;
            RecordedMousePositions = new List<KeyValuePair<long, Vector2>>();
            replayMouseIndex = 0;
            AllowVideo = true;
            End = false;
            skipped = false;
            BackgroundDim = .4f;
            FailsCount = 0;
            clearObjects();
            base.gameMod = GameMods;
            KyunGame.Instance.Player.Stop();
            Beatmap = beatmap;
            GamePosition = -3000;
            InGame = true;
            lastIndex = 0;
            ChangeBackground(beatmap.Background);
            hitbaseObjects.Clear();
            HitObjects.Clear();
            Combo.Instance.ResetAll();
            _particleEngine.Clear();
            _healthBar.Reset();
            _healthBar.Start(0);
            _healthBar.IsActive = true;
            timeToLeave = 0;
            _scoreDisplay.Reset();
            _scoreDisplay.IsActive = true;
            ActualTimingPoint = Beatmap.TimingPoints[0];
            NextTimingPoint = Beatmap.GetNextTimingPointFor(ActualTimingPoint.Offset + 50);
            _osuMode = beatmap.Osu_Gamemode;

            OriginalHitObjects.Clear();

            OriginalHitObjects = Beatmap.HitObjects.ToArray().ToList();

            try
            {
                FileInfo sampleInfo = new FileInfo(beatmap.SongPath);

                sampleSet = CustomSampleSet.LoadFromBeatmap(sampleInfo.DirectoryName);
            }
            catch
            {
                sampleSet = null;
                Logger.Instance.Info("Error getting sampleset");
            }
            //Remove

            if (beatmap.Title.ToLower() == "the empress" || beatmap.Title.ToLower() == "centipede")
            {

            }
            else
            {
                OriginalHitObjects = OriginalHitObjects.Distinct(new HitObjectComparer()).ToList();
            }

            if (game.Settings1.Default.Video)
            {
                if (!AVPlayer.videoplayer.Stopped)
                {
                    avp.videoplayer.vdc?.Dispose();

                    avp.videoplayer.Play(beatmap.Video);
                }
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
        }

        public override void Play(IBeatmap beatmap, GameMod GameMods, Replay _replay)
        {
            replay = _replay;
            GameMods |= GameMod.Replay;
            Play(Beatmap, GameMods);


        }

        private void _healthbar_OnFail()
        {
            //KyunGame.Instance.Player.Stop();
            if (!InGame)
                return;


            InGame = false;
            bool killVel = false;
            KyunGame.Instance.Player.SetVelocity(.5f);
            showingFail = true;
            new Task(() =>
            {
                int time = 2000;
                for (int a = time; a > 0; a--)
                {
                    if (killVel)
                    {
                        killVel = false;
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
                KyunGame.Instance.Player.Pause();
                KyunGame.Instance.Player.SetVelocity(1);
                KyunGame.Instance.Player.Velocity = 1;


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

                    HitObjectParticle pr = _particleEngine.AddNewHitObjectParticle(control.Texture,
                      new Vector2(2),
                      new Vector2(control.Position.X, control.Position.Y),
                      10,
                      0,
                      Color.White
                      ) as HitObjectParticle;
                    pr.Scale = control.Scale;
                    pr.Velocity = new Vector2(.5f);
                    pr.Opacity = control.Opacity;
                    pr.MoveTo(GameScreen.AnimationEffect.Linear, 60000, new Vector2(control.Position.X, control.Position.Y + 50));
                    pr.TextureColor = Color.Violet;

                    ParticleScore sc = _particleEngine.AddNewScoreParticle(SpritesContent.instance.MissTx,
                       new Vector2(.05f),
                       new Vector2(control.Position.X + (control.Texture.Height / 2) - (SpritesContent.instance.MissTx.Width / 2), control.Position.Y + (control.Texture.Height / 2) + (SpritesContent.instance.MissTx.Height + 10) * 1.5f),
                       10,
                       0,
                       Color.White
                       ) as ParticleScore;

                    sc.Scale = control.Scale;
                    sc.Velocity = new Vector2(.5f);
                    sc.Opacity = control.Opacity;
                    sc.MoveTo(GameScreen.AnimationEffect.Linear, 60000, new Vector2(control.Position.X, control.Position.Y + 50));
                    sc.TextureColor = Color.Violet;
                }
            }

        }

        private void clearObjects()
        {
            Controls.RemoveAll(isHitBase);
        }

        private void togglePause()
        {
            //KyunGame.Instance.Player.Paused = !KyunGame.Instance.Player.Paused;
            if (KyunGame.Instance.Player.PlayState == BassPlayState.Paused)
                return;

            KyunGame.Instance.Player.Pause();
            ScreenManager.ShowOverlay(PauseOverlay.Instance);
            PauseOverlay.ShowAlert(this);
        }

        private void checkObjectsInTime()
        {
            if (!skipped && GamePosition > OriginalHitObjects.First().StartTime - 3000)
            {
                skipped = true;

            }
            skipButton.Visible = !skipped;

            if (KyunGame.Instance.Player.PlayState == BassPlayState.Stopped)
                GamePosition += (long)KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds;
            else
                GamePosition = KyunGame.Instance.Player.PositionV2;

            if (!InGame) return;


            if (InGame && GamePosition > Beatmap.SleepTime && KyunGame.Instance.Player.PlayState == BassPlayState.Stopped)
            {
                if (!End)
                {
                    KyunGame.Instance.Player.Play(Beatmap.SongPath);
                    ActualTimingPoint = Beatmap.TimingPoints[0];
                    NextTimingPoint = Beatmap.GetNextTimingPointFor(ActualTimingPoint.Offset + 50);

                    KyunGame.Instance.Player.Volume = KyunGame.Instance.GeneralVolume;
                    if ((gameMod & GameMod.DoubleTime) == GameMod.DoubleTime)
                    {
                        KyunGame.Instance.Player.SetVelocity(1.5f);
                    }
                }

            }


            if (lastIndex >= OriginalHitObjects.Count)
            {
                InGame = false;
                return;
            }

            long actualTime = GamePosition;


            IHitObj lastObject = OriginalHitObjects[lastIndex];
            IHitObj afterOjb = lastIndex == 0 ? null : OriginalHitObjects[lastIndex - 1];

            long approachStart = (long)(ModeConstants.APPROACH_TIME_BASE - Beatmap.ApproachRate * 150f) + 500;

            long nextObjStart = (long)lastObject.StartTime - approachStart;


            if (actualTime > nextObjStart)
            {
                if (lastObject.isNewCombo)
                {
                    comboNumber = 0;
                    comboColorIndex = (comboColorIndex + 1 >= comboColors.Length) ? 0 : comboColorIndex + 1;
                    comboColor = comboColors[comboColorIndex];
                }
                comboNumber++;
                if (lastObject is HitButton)
                {
                    var obj = new HitSingle(lastObject, Beatmap, this);
                    obj.Opacity = 0;
                    hitbaseObjects.Add(obj);
                    HitObjects.Add(obj);
                    obj.Id = HitObjects.Count - 1;
                    obj.MoveEnabled = MoveEnabled;
                    obj.ComboColor = comboColor;
                    obj.ComboNumber = comboNumber;

                    if ((gameMod & GameMod.Replay) == GameMod.Replay)
                    {
                        obj.replayTime = replay.Hits[obj.Id].PressedAt;
                    }

                    Controls.Add(obj);

                    if (afterOjb != null)
                    {
                        if (afterOjb.OsuLocation.Equals(lastObject.OsuLocation))
                        {
                            obj.Position = Vector2.Add(obj.Position, new Vector2(10));
                        }
                    }
                }
                else
                {
                    if (lastObject.isNewCombo)
                    {
                        comboNumber = 1;
                    }
                    var obj = new HitHolder(lastObject, Beatmap, this);
                    obj.Opacity = 0;

                    // Quick hax, error parsing in hyper-fast sliders causes an extra-large HitHolder
                    if (obj.EndTime > AVPlayer.audioplayer.Length)
                    {
                        var tm = Beatmap.GetTimingPointFor(GamePosition, false);
                        obj.EndTime = (int)(obj.Time + tm.MsPerBeat); //just 1 tick (or beat?) in milliseconds
                    }
                    obj.ComboColor = comboColor;
                    obj.ComboNumber = comboNumber;
                    hitbaseObjects.Add(obj);
                    HitObjects.Add(obj);
                    obj.Id = HitObjects.Count - 1;

                    if ((gameMod & GameMod.Replay) == GameMod.Replay)
                    {
                        obj.replayTime = replay.Hits[obj.Id].PressedAt;
                        //obj.Missed = ((((ReplayHitHolder)replay.Hits[obj.Id]).PressedAt) > 0) ? true : false;
                    }

                    Controls.Add(obj);

                    if (afterOjb != null)
                    {
                        if (afterOjb.OsuLocation.Equals(lastObject.OsuLocation))
                        {
                            obj.Position = Vector2.Add(obj.Position, new Vector2(10));
                        }
                    }
                }

                lastIndex++;
            }

        }

        public override void Update(GameTime tm)
        {
            if (!Visible || isDisposing) return;
            checkObjectsInTime();
            base.Update(tm);
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


            cursorFrame += tm.ElapsedGameTime.Milliseconds;
            if (cursorFrame > 16)
            {
                cursorFrame = 0;
                allowTrail = true;
            }

            int leaveTime = 3000;


            if ((gameMod & GameMod.Auto) == GameMod.Auto || (gameMod & GameMod.Replay) == GameMod.Replay)
            {

            }
            else if (!End)
            {
                elapsedMsToMouse += KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;

                if (elapsedMsToMouse >= MOUSE_FRAMERATE_MS)
                {
                    RecordedMousePositions.Add(new KeyValuePair<long, Vector2>(GamePosition, MouseHandler.GetState().Position));
                    elapsedMsToMouse = 0;
                }
            }

            timeProgressBar.Value = ((float)KyunGame.Instance.Player.PositionV2 / (float)Beatmap.HitObjects.Last().EndTime * 100f);
            if (lastIndex >= OriginalHitObjects.Count)
            {
                if (OriginalHitObjects.Last() is kyun.Beatmap.HitHolder)
                {
                    if (GamePosition > ((HitHolder)hitbaseObjects.Last()).EndTime)
                    {
                        End = true;
                    }
                }
                else
                {
                    if (GamePosition > ((HitSingle)hitbaseObjects.Last()).Time)
                    {
                        End = true;
                    }
                }
            }



            if (End)
            {
                timeToLeave += tm.ElapsedGameTime.Milliseconds;
            }

            if (End && timeToLeave > leaveTime)
            {
                /*
                ScreenManager.ChangeTo(BeatmapScreen.Instance);
                KyunGame.Instance.Player.Play(Beatmap.SongPath);*/
                if ((gameMod & GameMod.Auto) == GameMod.Auto || (gameMod & GameMod.Replay) == GameMod.Replay)
                {
                    AutoCursor.Visible = false;
                }

                ScreenManager.ChangeTo(Classic.ScorePanel.Instance);
                Classic.ScorePanel.Instance.CalcScore(this);
                End = false;
            }
        }

        internal override void RenderObjects()
        {
            try
            {
                for (int i = Controls.Count - 1; i >= 0; i--)
                {
                    var obj = Controls[i];
                    if (obj == null)
                        continue; //wtf
                    if (obj.Texture != null)
                    {
                        if (obj.Texture == SpritesContent.Instance.TopEffect)
                        {
                            continue;
                        }
                        else
                        {
                            obj.Render();
                        }
                    }
                    else
                    {
                        obj.Render();
                    }
                }/*
                foreach (UIObjectBase obj in Controls.Reverse<UIObjectBase>())
                {
                    if (obj == null)
                        continue; //wtf
                    if (obj.Texture != null)
                    {
                        if (obj.Texture == SpritesContent.Instance.TopEffect)
                        {
                            continue;
                        }
                        else
                        {
                            obj.Render();
                        }
                    }
                    else
                    {
                        obj.Render();
                    }
                }*/

            }
            catch
            {

            }
        }

        public override void Render()
        {
            base.Render();

        }

        private bool keypressed(int key)
        {
            switch (key)
            {
                case 1:
                    mousePressedLeft = MouseHandler.GetState().LeftButton == ButtonState.Pressed;
                    return mousePressedLeft;
                    break;
                case 2:
                    mousePressedRight = MouseHandler.GetState().RightButton == ButtonState.Pressed;
                    return mousePressedRight;
                    break;
                case 3:
                    keypressedZ = Keyboard.GetState().IsKeyDown(Keys.Z);
                    return keypressedZ;
                    break;
                case 4:
                    keypressedX = Keyboard.GetState().IsKeyDown(Keys.X);
                    return keypressedX;
                    break;
            }

            return false;
        }


        internal override void UpdateControls()
        {
            Controls.RemoveAll(isNull); //wtf
                                        //Controls.RemoveWhere(isDed);
            Controls.RemoveAll(isDed);
            bool first = false;
            bool secnd = false;

            List<Keys> keyspressed = Keyboard.GetState().GetPressedKeys().ToList();

            HitSingle firstEl = null;
            HitSingle secEl = null;

            bool autoCursorMoved = false;
            HitBase lastHitBaseControl = null;
            for (int c = 0; c < Controls.Count; c++)
            {
                UIObjectBase control = Controls.ElementAt(c);

                if (control is HitBase)
                {
                    Rectangle posNow = new Rectangle((int)control.Position.X, (int)control.Position.Y, (int)control.Size.X, (int)control.Size.Y);

                    bool intersects = false;
                    if (lastHitBaseControl != null)
                    {
                        intersects = (new Rectangle(
                            (int)lastHitBaseControl.Position.X,
                            (int)lastHitBaseControl.Position.Y,
                            (int)lastHitBaseControl.Size.X,
                            (int)lastHitBaseControl.Size.Y
                            ).Intersects(posNow));
                    }

                    if (!first || AVPlayer.audioplayer.PositionV2 >= (control as HitSingle).Time - Beatmap.Timing50 && !intersects)
                    {
                        if (!control.Died)
                        {
                            ((HitSingle)control).IsFirst = first = true;

                        }
                    }
                    lastHitBaseControl = (HitBase)control;
                }

                control.Update();

                if (control is HitBase)
                {

                    ((HitSingle)control).keyPressed = keyspressed;
                }
            }

            int lastForSlider = 0;
            if ((gameMod & GameMod.Auto) == GameMod.Auto)
            {
                for (int b = lastAutoId; b < HitObjects.Count; b++)
                {
                    if (!HitObjects[b].Died)
                    {
                        lastForSlider = b;
                        break;
                    }
                }
            }

            if ((gameMod & GameMod.Auto) == GameMod.Auto)
            {
                for (int b = lastAutoId; b < HitObjects.Count; b++)
                {
                    if (!HitObjects[b].Died)
                    {
                        if (HitObjects.Count < 2)
                        {
                            firstEl = (HitSingle)HitObjects[0];

                            break;
                        }
                        if (b - 1 < 0)
                        {
                            firstEl = (HitSingle)HitObjects[0];
                            break;

                        }
                        firstEl = (HitSingle)HitObjects[b - 1];
                        lastAutoId = Math.Max(0, b - 2);
                        if (b < HitObjects.Count)
                        {
                            secEl = (HitSingle)HitObjects[b];
                        }
                        break;
                    }
                }
            }

            if ((gameMod & GameMod.Replay) == GameMod.Replay)
            {

                if (!(replayMouseIndex > replay.MousePositions.Count - 1))
                {
                    if (GamePosition > replay.MousePositions[replayMouseIndex].Key)
                    {
                        if (GamePosition < replay.MousePositions[replayMouseIndex].Key + 100)
                        {

                            var curPos = new Vector2(
                                replay.MousePositions[replayMouseIndex].Value.X - Math.Abs((AutoCursor.Size.X) / 2),
                                replay.MousePositions[replayMouseIndex].Value.Y - Math.Abs((AutoCursor.Size.Y) / 2));

                            Vector2 lastReplayPos = Vector2.Zero;

                            if (true)
                            {
                                if (AutoCursor.Position != curPos)
                                {
                                    autoCursorMoved = true;
                                }

                                AutoCursor.Position = curPos;

                            }
                            else
                            {
                                float startx = replay.MousePositions[replayMouseIndex - 1].Value.X,
                                    starty = replay.MousePositions[replayMouseIndex - 1].Value.Y,
                                    endx = replay.MousePositions[replayMouseIndex].Value.X,
                                    endy = replay.MousePositions[replayMouseIndex].Value.Y
                                    ;

                                var actps = getPointAt(startx, starty, endx, endy, (GamePosition) / clampDiff());
                                AutoCursor.Position = actps;
                            }

                            replayMouseIndex++;
                        }
                        else
                        {
                            for (int t = replayMouseIndex; t < replay.MousePositions.Count; t++)
                            {
                                if (replay.MousePositions[t].Key > GamePosition)
                                {
                                    replayMouseIndex = t;
                                    break;
                                }
                            }
                            var curDestn = new Vector2(
                                replay.MousePositions[replayMouseIndex].Value.X - Math.Abs((AutoCursor.Size.X) / 2),
                                replay.MousePositions[replayMouseIndex].Value.Y - Math.Abs((AutoCursor.Size.Y) / 2));

                            if (AutoCursor.Position != curDestn)
                            {
                                autoCursorMoved = true;
                            }

                            AutoCursor.Position = curDestn;

                        }
                    }
                    else
                    {


                    }
                }

            }

            bool lastSliderDead = (lastSlider == null) ? true : lastSlider.Died;

            if (firstEl != null && (gameMod & GameMod.Auto) == GameMod.Auto && lastSliderDead)
            {

                long timeDiff = 0;
                Vector2 dest = Vector2.Zero;
                if (secEl == null)
                {
                    timeDiff = firstEl.Time - GamePosition;

                    dest = new Vector2(firstEl.Position.X, firstEl.Position.Y);

                }
                else
                {
                    timeDiff = secEl.Time - 20 - GamePosition;

                    dest = new Vector2(secEl.Position.X, secEl.Position.Y);

                }

                float atime = clampDiff();

                if (secEl == null)
                {


                    var ps = getPointAt(AutoCursor.Position.X, AutoCursor.Position.Y,
                   dest.X, dest.Y, 1f - ((float)timeDiff / atime));


                    Vector2 curDest = new Vector2(ps.X + 10, ps.Y + 10);

                    if (AutoCursor.Position != curDest)
                        autoCursorMoved = true;

                    AutoCursor.Position = curDest;
                }
                else
                {
                    var ps2 = getPointAt(firstEl.Position.X, firstEl.Position.Y,
                   dest.X, dest.Y, 1f - ((float)timeDiff / 150));
                    Vector2 curDest = new Vector2(ps2.X + 10, ps2.Y + 10);

                    if (AutoCursor.Position != curDest)
                        autoCursorMoved = true;

                    AutoCursor.Position = curDest;

                } 
            }
            
            if (!autoCursorMoved)
            {
                if (HitObjects.Count < 1)
                {

                }
                else
                {
                    HitBase hitb = HitObjects[lastForSlider];

                    if (hitb != null)
                    {
                        if (!hitb.Died)
                        {
                            if (hitb is HitHolder && (hitb as HitHolder)._hitButton.osuHitObject is osuBMParser.HitSlider)
                            {
                                lastSlider = hitb;
                                if (AutoCursor.Position != (hitb as HitHolder).SliderFollowCirclePos && allowTrail)
                                {
                                    _particleEngine.AddNewHitObjectParticle(AutoCursor.Texture, Vector2.Zero, (hitb as HitHolder).SliderFollowCirclePos, 1, 0, Color.YellowGreen).Opacity = .6f;
                                    allowTrail = false;
                                }

                                AutoCursor.Position = (hitb as HitHolder).SliderFollowCirclePos;                               
                                
                            }
                        }
                    }
                }
            }
            AutoCursor.TextureColor = Color.YellowGreen;/*
            followPointEnd.Position = AutoCursor.Position;
            followPointEnd.Visible = true;*/
            if (autoCursorMoved && allowTrail)
            {
                _particleEngine.AddNewHitObjectParticle(AutoCursor.Texture, Vector2.Zero, AutoCursor.Position, 1, 0, Color.YellowGreen).Opacity = .6f;
                allowTrail = false;
            }
        }

        float clampDiff()
        {
            float atime = 1000;
            float min = 1800;
            float mid = 800;
            float max = 150;
            if (Beatmap.ApproachRate > 5f)
                atime = mid + (max - mid) * (Beatmap.ApproachRate - 5f) / 5f;
            else if (Beatmap.ApproachRate < 5f)
                atime = mid - (mid - min) * (5f - Beatmap.ApproachRate) / 5f;
            else
                atime = mid;

            return atime;
        }

        public void recordMovement(Vector2 pos)
        {
            if (elapsedMsToMouse >= MOUSE_FRAMERATE_MS)
            {


            }

        }

        internal double getT(long time, long startT, long totalT)
        {
            return ((double)time - startT) / totalT;
        }


        internal Vector2 getPointAt(float startX, float startY, float endX, float endY, float t)
        {
            // "autopilot" mod: move quicker between objects

            float te = StringHelper.clamp(t, 0f, 1f);

            bool addNoise = OsuUtils.OsuBeatMap.rnd.NextBoolean() && OsuUtils.OsuBeatMap.rnd.NextBoolean() && OsuUtils.OsuBeatMap.rnd.NextBoolean() && OsuUtils.OsuBeatMap.rnd.NextBoolean();
            float NoiseToAdd = 0;
            float NoiseToAddY = 0;
            if (false)
            {
                if (OsuUtils.OsuBeatMap.rnd.NextBoolean() && OsuUtils.OsuBeatMap.rnd.NextBoolean() && OsuUtils.OsuBeatMap.rnd.NextBoolean())
                {
                    NoiseToAdd = (float)OsuUtils.OsuBeatMap.rnd.Next(-3, 3);
                }
                if (OsuUtils.OsuBeatMap.rnd.NextBoolean() && OsuUtils.OsuBeatMap.rnd.NextBoolean() && OsuUtils.OsuBeatMap.rnd.NextBoolean())
                {
                    NoiseToAddY = (float)OsuUtils.OsuBeatMap.rnd.Next(-3, 3);
                }
            }

            float tee = bezierBlend(te * .95f);
            return new Vector2(startX + (endX - startX) * tee + NoiseToAdd, startY + (endY - startY) * tee + NoiseToAddY);
        }

        internal float bezierBlend(float t)
        {
            //return bounceOut(t);
            return (float)Math.Pow(t, 2f) * (3.0f - 2.0f * t);
        }

        internal float bounceIn(float t)
        {
            return 1 - bounceOut(1 - t);
        }

        internal float bounceOut(float t)
        {
            return (t = +t) < b1 ? b0 * t * t : t < b3 ? b0 * (t -= b2) * t + b4 : t < b6 ? b0 * (t -= b5) * t + b7 : b0 * (t -= b8) * t + b9;
        }

        float b1 = 4f / 11f,
            b2 = 6f / 11f,
            b3 = 8f / 11f,
            b4 = 3f / 4f,
            b5 = 9 / 11f,
            b6 = 10f / 11f,
            b7 = 15f / 16f,
            b8 = 21f / 22f,
            b9 = 63f / 64f;

        float b0 = 1f / (4f / 11f) / (4f / 11f);
        private HitBase lastSlider;
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
            else if (Math.Abs(x.StartTime - y.StartTime) < 3)
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
