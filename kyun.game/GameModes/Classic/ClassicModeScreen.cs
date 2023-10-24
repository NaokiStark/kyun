using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using kyun.Beatmap;
using kyun.GameScreen;
using kyun.Utils;
using kyun.GameScreen.UI.Buttons;
using kyun.GameScreen.UI;
using Microsoft.Xna.Framework.Graphics;
using kyun.Score;
using System.IO;
using kyun.GameScreen.UI.Particles;
using kyun.Audio;
using kyun.UIObjs;
using kyun.Overlay;
using kyun.Video;
using kyun.OsuUtils;
using kyun.game;
using kyun.game.GameScreen.UI;

namespace kyun.GameModes.Classic
{
    public class ClassicModeScreen : GameModeScreenBase
    {

        int lastIndex = 0;
        HashSet<HitBase> hitbaseObjects = new HashSet<HitBase>();
        List<Break> breaks;
        private bool End;
        private int countToScores;
        private ButtonStandard backButton;
        private Grid objGrid;
        public GameScreen.UI.Image imgGridBackground;
        public HealthBar _healthBar;
        private Label tmLabel;
        private int coverSize;
        public FilledRectangle Board { get; private set; }
        public Image CoverImage { get; private set; }
        public FilledRectangle FillCoverImage { get; private set; }
        public Label TitleLabel { get; private set; }
        public ProgressBar tmProgress { get; private set; }
        public FilledRectangle fcRectangle { get; private set; }
        public ComboDisplay _comboDsp { get; private set; }
        public ParticleEngine _particleEngine { get; private set; }
        public int FailsCount { get; set; }
        public bool onBreak { get; private set; }
        public int[] EnphasisColor { get; private set; }        
        private bool failed;
        private int lastBreak;
        private float velocity = 1;
        private long endToTime;
        private bool squareYesNo;
        private List<int[]> ecolors;
        private bool switchParticle;
        private ParticleEngine particleEngine;
        static ClassicModeScreen Instance;
        public Replay replay;
        private ButtonStandard skipButton;
        private bool skipped;

        public static ClassicModeScreen GetInstance()
        {
            if (Instance == null)
                Instance = new ClassicModeScreen();
            return Instance;
        }

        public ClassicModeScreen()
            : base("ClassicModeScreen")
        {
            Instance = this;
            HitObjects = new List<HitBase>();
            coverSize = 75;
            AllowVideo = true;
            objGrid = new Grid(10);

            ChangeBackground(KyunGame.Instance.SelectedBeatmap.Background);
            backButton = new ButtonStandard(Color.DarkRed)
            {
                ForegroundColor = Color.White,
                Caption = "Pause",
                Position = new Vector2(15, ActualScreenMode.Height - (SpritesContent.Instance.ButtonStandard.Height / 2) - 30),
            };

            imgGridBackground = new GameScreen.UI.Image(SpritesContent.Instance.ClassicBackground);
            imgGridBackground.BeatReact = false;

            imgGridBackground.Position = new Vector2((ActualScreenMode.Width / 2) - (imgGridBackground.Texture.Width / 2), (ActualScreenMode.Height / 2) - (imgGridBackground.Texture.Height / 2));
            
            onKeyPress += (obj, args) =>
            {

                if (args.Key == Microsoft.Xna.Framework.Input.Keys.Escape)
                {
                    if (End)
                        countToScores = (int)endToTime;
                    else
                        togglePause();
                }

                if (KyunGame.Instance.Player.Paused && args.Key == Microsoft.Xna.Framework.Input.Keys.F2)
                {
                    ScreenManager.ChangeTo(BeatmapScreen.Instance);
                }

            };

            Board = new FilledRectangle(new Vector2(200, imgGridBackground.Texture.Height), Color.Black * .75f);
            Board.Texture = SpritesContent.RoundCorners(Board.Texture, 6);
            Board.Position = new Vector2(-10, imgGridBackground.Position.Y);
            
            coverSize = Board.Texture.Width;
            CoverImage = new Image(Background);
            CoverImage.BeatReact = false;

            changeCoverDisplay(KyunGame.Instance.SelectedBeatmap.Background);
            CoverImage.Position = new Vector2(Board.Position.X, Board.Position.Y);

            FillCoverImage = new FilledRectangle(new Vector2(100, 100), Color.Black);

            TitleLabel = new Label(0);
            TitleLabel.Text = KyunGame.Instance.SelectedBeatmap.Artist + " \n " + KyunGame.Instance.SelectedBeatmap.Title;

            TitleLabel.Size = new Vector2(Board.Texture.Width - 2, 50);

            TitleLabel.Text = StringHelper.WrapText(SpritesContent.Instance.DefaultFont, TitleLabel.Text, TitleLabel.Size.X, .65f);
            TitleLabel.Centered = false;
            
            TitleLabel.Position = new Vector2(Board.Position.X + /*Board.Texture.Width /2*/ 20, coverSize + CoverImage.Position.Y - (TitleLabel.Size.Y / 2));
            TitleLabel.Font = SpritesContent.Instance.SettingsFont;
            TitleLabel.Scale = .8f;

            Board.Texture = new Texture2D(KyunGame.Instance.GraphicsDevice, 200, (int)TitleLabel.Position.Y + (int)TitleLabel.Size.Y + 10);
            Color[] dataBar = new Color[200 * ((int)TitleLabel.Position.Y + (int)TitleLabel.Size.Y + 10)];
            for (int i = 0; i < dataBar.Length; ++i) dataBar[i] = Color.Black * .75f;
            Board.Texture.SetData(dataBar);

            backButton.Position = new Vector2(Board.Position.X + (Board.Texture.Width / 2 - backButton.Texture.Width / 2), TitleLabel.Position.Y + (int)TitleLabel.Size.Y + 50);
            
            _healthBar = new HealthBar(this, imgGridBackground.Texture.Width - 20, 25);

            _healthBar.Position = new Vector2(ActualScreenMode.Width / 2 - _healthBar.BgBar.Width / 2, ActualScreenMode.Height - _healthBar.BgBar.Height - 10);

            _scoreDisplay = new ScoreDisplay((ActualScreenMode.Height < 700 && ActualScreenMode.Width < 1000) ? 1.1f : 1.2f);

            _scoreDisplay.Position = new Vector2(_healthBar.Position.X + ((_healthBar.Texture.Width / 2) - (_scoreDisplay.Texture.Width / 2)) + 10, _healthBar.Position.Y - 10 - _scoreDisplay.Texture.Height);

            int progressBSize = imgGridBackground.Texture.Width - 15;

            fcRectangle = new FilledRectangle(new Vector2(progressBSize + 15, 20), Color.Black * .8f);
            fcRectangle.Position = new Vector2(ActualScreenMode.Width / 2 - ((progressBSize + 15) / 2), 15);

            tmProgress = new ProgressBar(progressBSize, 10);
            tmProgress.Position = new Vector2(ActualScreenMode.Width / 2 - (progressBSize / 2), 20);

            Vector2 mStr = SpritesContent.Instance.TitleFont.MeasureString("00:00") * .8f;

            tmLabel = new Label()
            {
                Font = SpritesContent.Instance.TitleFont,
                Text = "00:00",
                Position = new Vector2(fcRectangle.Position.X + (fcRectangle.Texture.Width / 2) - (mStr.X / 2), fcRectangle.Position.Y + fcRectangle.Texture.Height + 10),
                Scale = .8f

            };

            skipButton = new ButtonStandard(Color.PaleVioletRed)
            {
                Caption = "Skip",
                Position = new Vector2(ActualScreenMode.Width - SpritesContent.Instance.ButtonDefault.Width, ActualScreenMode.Height - SpritesContent.Instance.ButtonDefault.Height),
                Visible = false
            };

            _comboDsp = new ComboDisplay();

            _particleEngine = new ParticleEngine();
            particleEngine = new ParticleEngine();

            ecolors = new List<int[]>();
            ecolors.Add(new int[] { 206, 53, 39 });
            ecolors.Add(new int[] { 237, 245, 8 });
            ecolors.Add(new int[] { 34, 92, 173 });
            ecolors.Add(new int[] { 35, 196, 91 });
            ecolors.Add(new int[] { 145, 35, 196 });

            if (KyunGame.xmas)
            {
                ecolors.Clear();
                ecolors.Add(new int[] { 24, 114, 21 });
                ecolors.Add(new int[] { 206, 59, 59 });
            }

            EnphasisColor = ecolors[OsuBeatMap.rnd.Next(0, ecolors.Count - 1)];

            onKeyPress += ClassicModeScreen_onKeyPress;
            KyunGame.Instance.OnPeak += Instance_OnPeak;
            backButton.Click += BackButton_Click;
            _healthBar.OnFail += _healthBar_OnFail;

            skipButton.Click += SkipButton_Click;

            Controls.Add(particleEngine);
            Controls.Add(imgGridBackground);
            Controls.Add(Board);
            Controls.Add(TitleLabel);
            Controls.Add(CoverImage);
            Controls.Add(backButton);
            Controls.Add(UserBox.GetInstance());
            Controls.Add(fcRectangle);
            Controls.Add(tmProgress);
            Controls.Add(_comboDsp);
            Controls.Add(_healthBar);
            Controls.Add(_scoreDisplay);
            Controls.Add(tmLabel);
            Controls.Add(_particleEngine);
            Controls.Add(replayLabel);
            Controls.Add(skipButton);

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
                avp.audioplayer.Position = (long)Beatmap.HitObjects[0].StartTime - 3000;
                EffectsPlayer.PlayEffect(SpritesContent.Instance.MenuTransition);
            }
        }

        private void ClassicModeScreen_onKeyPress(object sender, GameScreen.InputEvents.KeyPressEventArgs args)
        {
            Vector2 pos = Vector2.Zero;
            switch (args.Key)
            {
                case Microsoft.Xna.Framework.Input.Keys.NumPad1:
                case Microsoft.Xna.Framework.Input.Keys.NumPad2:
                case Microsoft.Xna.Framework.Input.Keys.NumPad3:
                case Microsoft.Xna.Framework.Input.Keys.NumPad4:
                case Microsoft.Xna.Framework.Input.Keys.NumPad5:
                case Microsoft.Xna.Framework.Input.Keys.NumPad6:
                case Microsoft.Xna.Framework.Input.Keys.NumPad7:
                case Microsoft.Xna.Framework.Input.Keys.NumPad8:
                case Microsoft.Xna.Framework.Input.Keys.NumPad9:
                    pos = HitSingle.GetPositionFor(((int)args.Key) - 96);
                    _particleEngine.AddNewHitObjectParticle(SpritesContent.Instance.Radiance,
                       new Vector2(.05f),
                       pos,
                       10,
                       0,
                       Color.White
                       );
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Space:
                    if (skipButton.Visible)
                        skip();
                    break;
            }
        }

        private void _healthBar_OnFail()
        {
            if (!InGame)
                return;

            if (End)
                return;

            if (failed)
                return;

            if (gameMod == GameMod.Auto || gameMod == GameMod.NoFail)
                return;

            failed = true;
            KyunGame.Instance.Player.Pause();
            ScreenManager.ShowOverlay(PauseOverlay.Instance);
            PauseOverlay.ShowFailed(this);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            togglePause();
        }

        /// <summary>
        /// Start game
        /// </summary>
        /// <param name="beatmap"></param>
        public override void Play(IBeatmap beatmap, GameMod GameMods = GameMod.None)
        {

            InGame = false;
            //Break[] bks = new Break[beatmap.Breaks.Count];
            breaks = new List<Break>(beatmap.Breaks);
            lastBreak = 0;
            failed = false;
            End = false;
            skipped = false;
            HitObjects.Clear();
            FailsCount = 0;
            _healthBar.Reset();
            _healthBar.IsActive = false;
            _scoreDisplay.Reset();
            Combo.Instance.ResetAll();
            countToScores = 0;
            clearObjects();
            base.gameMod = GameMods;
            KyunGame.Instance.Player.Stop();

            Beatmap = beatmap;
            imgGridBackground.Visible = false;
            GamePosition = 0;

            lastIndex = 0;
            ChangeBackground(KyunGame.Instance.SelectedBeatmap.Background);
            changeCoverDisplay(KyunGame.Instance.SelectedBeatmap.Background);

            TitleLabel.Text = KyunGame.Instance.SelectedBeatmap.Artist + " \n " + KyunGame.Instance.SelectedBeatmap.Title;
            TitleLabel.Text = StringHelper.WrapText(SpritesContent.Instance.DefaultFont, TitleLabel.Text, TitleLabel.Size.X, .65f);
            _particleEngine.Clear();
            BackgroundDim = 0.45f;

            if (System.IO.File.Exists(beatmap.Video))
            {
                if (System.IO.File.GetAttributes(beatmap.Video) != System.IO.FileAttributes.Directory)
                {
                    if (Settings1.Default.Video)
                    {
                        if (Settings1.Default.Tutorial)
                        {
                            VideoPlayer.Instance.Play(beatmap.Video);
                            Settings1.Default.Tutorial = false;
                            Settings1.Default.Save();
                        }
                        else
                        {
                            //ScreenBase.AVPlayer.SeekTime(1);
                            AVPlayer.videoplayer.Stop();
                            AVPlayer.videoplayer.Play(Beatmap.Video);
                        }
                    }

                }
                else
                {
                    AVPlayer.Stop();
                }
            }
            _healthBar.Start(beatmap.HPDrainRate);
            _comboDsp.IsActive = true;

            if ((gameMod & GameMod.DoubleTime) == GameMod.DoubleTime)
            {
                velocity = 1.5f;
            }

            if ((gameMod & GameMod.Replay) == GameMod.Replay)
            {
                replayLabel.Visible = true;
                if ((GameMods & GameMod.Auto) == GameMod.Auto)
                    replayLabel.Text = "AUTO - REPLAY";
                else
                    replayLabel.Text = "REPLAY";
            }
            else
            {
                replayLabel.Visible = false;
                if ((GameMods & GameMod.Auto) == GameMod.Auto)
                    replayLabel.Text = "AUTO - REPLAY";
                else
                    replayLabel.Text = "REPLAY";
            }

            if ((GameMods & GameMod.Auto) == GameMod.Auto)
            {
                replayLabel.Text = "AUTO";
                replayLabel.Visible = true;
            }
            else
            {
                replayLabel.Visible = false;
            }

            EnphasisColor = ecolors[OsuUtils.OsuBeatMap.rnd.Next(0, ecolors.Count - 1)];
            KyunGame.Instance.discordHandler.SetState("Playing Classic", $"{Beatmap.Artist} - {Beatmap.Title}", "idle_large", "classic_small");
            InGame = true;
        }

        public override void Play(IBeatmap beatmap, GameMod GameMods, Replay _replay)
        {
            replay = _replay;
            GameMods |= GameMod.Replay;
            Play(Beatmap, GameMods);
        }

        private void clearObjects()
        {
            Controls.RemoveAll(isHitBase);
            objGrid.CleanUp();
        }

        private void togglePause()
        {
            if (KyunGame.Instance.Player.PlayState == BassPlayState.Paused)
                return;

            KyunGame.Instance.Player.Pause();
            ScreenManager.ShowOverlay(PauseOverlay.Instance);
            PauseOverlay.ShowAlert(this);
        }

        private void checkObjectsInTime()
        {
            if (!InGame) return;

            if (InGame && GamePosition > Beatmap.SleepTime && KyunGame.Instance.Player.PlayState == BassPlayState.Stopped)
            {
                KyunGame.Instance.ChangeWindowTitle("kyun! - Playing: " + Beatmap.Artist + " - " + Beatmap.Title);

                KyunGame.Instance.Player.Play(Beatmap.SongPath, ((gameMod & GameModes.GameMod.DoubleTime) == GameMod.DoubleTime) ? 1.5f : 1f);

                KyunGame.Instance.Player.Volume = KyunGame.Instance.GeneralVolume;

                if ((long)Beatmap.HitObjects.First().StartTime > 3500)
                    skipButton.Visible = true;
                else
                    skipButton.Visible = false;
            }

            onBreak = false;
            if (KyunGame.Instance.Player.PlayState == BassPlayState.Playing)
            {
                long songpos = KyunGame.Instance.Player.Position;
                if (lastBreak < breaks.Count)
                {
                    if (songpos >= breaks[lastBreak].Start)
                    {
                        onBreak = true;
                        if (songpos >= breaks[lastBreak].End)
                        {
                            onBreak = false;
                            lastBreak++;
                        }
                    }
                }
                else
                {
                    onBreak = false;
                }
            }

            if (onBreak)
            {
                if (imgGridBackground.Visible)
                {
                    _particleEngine.AddNewHitObjectParticle(imgGridBackground.Texture, new Vector2(0.0001f), imgGridBackground.Position, 9, 0, Color.White);
                }

                imgGridBackground.Visible = false;
            }
            else
            {
                if (!_healthBar.IsActive && imgGridBackground.Visible)
                {
                    _particleEngine.AddNewHitObjectParticle(imgGridBackground.Texture, new Vector2(0.005f), imgGridBackground.Position, 9, 0, Color.White);
                }

                imgGridBackground.Visible = _healthBar.IsActive;

            }

            if (lastIndex > 0)
            {
                _healthBar.IsActive = true;
            }

            if (lastIndex >= Beatmap.HitObjects.Count)
            {
                InGame = false;
                return;
            }

            long actualTime = GamePosition;

            IHitObj lastObject = Beatmap.HitObjects[lastIndex];

            if (!skipped && actualTime > Beatmap.HitObjects[0].StartTime - 3000)
            {
                skipped = true;
               
            }

            skipButton.Visible = !skipped;

            long approachStart = (long)(ModeConstants.APPROACH_TIME_BASE - Beatmap.ApproachRate * 150f) + 10;

            long nextObjStart = (long)lastObject.StartTime - approachStart;

            if (actualTime > nextObjStart)
            {
                int gridPosition = lastObject.Location - 96;

                bool isLastHolder = true;
                int attps = 0;
                //No Overlap all objects with holders
                while (isLastHolder)
                {
                    if (attps > 9)
                    {
                        lastObject = null;
                        break;
                    }
                    attps++;

                    HitBase lastObj = null;
                    foreach (UIObjectBase control in Controls)
                    {
                        if (control is HitBase)
                        {
                            if (gridPosition == ((HitSingle)control).GridPosition)
                                lastObj = (HitBase)control;
                        }
                    }

                    if (lastObj == null)
                    {
                        isLastHolder = false;
                        continue;
                    }

                    if (lastObj is HitHolder)
                    {
                        HitHolder lastHolder = (HitHolder)lastObj;
                        if (lastHolder.EndTime > lastObject.StartTime)
                        {
                            gridPosition = OsuUtils.OsuBeatMap.rnd.Next(8) + 1;
                            continue;
                        }
                        else
                        {
                            isLastHolder = false;
                        }

                    }
                    else
                    {
                        isLastHolder = false;
                    }
                }

                if (lastObject != null)
                {                //New Element
                    if (lastIndex % (Math.Max((int)Beatmap.OverallDifficulty, 1) * 10) == 0)
                        EnphasisColor = ecolors[OsuUtils.OsuBeatMap.rnd.Next(0, ecolors.Count - 1)];

                    bool shared = false;
                    if (lastIndex > 1)
                    {
                        if (Beatmap.HitObjects[lastIndex - 1].StartTime == lastObject.StartTime)
                            shared = true;
                        if (lastIndex < Beatmap.HitObjects.Count - 1)
                        {
                            if (Beatmap.HitObjects[lastIndex + 1].StartTime == lastObject.StartTime)
                                shared = true;
                        }

                    }

                    if (lastObject is HitButton)
                    {
                        var obj = new HitSingle(lastObject, Beatmap, this, gridPosition, shared);
                        obj.Opacity = 0;
                        obj.ReplayId = lastIndex;
                        objGrid.Add(obj, gridPosition - 1);
                        HitObjects.Add(obj);
                    }
                    else
                    {
                        var obj = new HitHolder(lastObject, Beatmap, this, gridPosition, shared);
                        obj.Opacity = 0;
                        obj.ReplayId = lastIndex;
                        objGrid.Add(obj, gridPosition - 1);
                        HitObjects.Add(obj);
                    }
                }
                lastIndex++;
            }
        }

        internal static void SetInstance(ClassicModeScreen p)
        {
            Instance = p;
        }

        public override void Update(GameTime tm)
        {
            if (!Visible || isDisposing) return;

            if (KyunGame.Instance.Player.PlayState == BassPlayState.Stopped)
                GamePosition += (long)KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds;
            else
                GamePosition = KyunGame.Instance.Player.Position + Beatmap.SleepTime;

            checkObjectsInTime();
            base.Update(tm);

            if (Beatmap.HitObjects.Last() is Beatmap.HitButton)
            {
                endToTime = (Settings1.Default.Video && !VideoPlayer.Instance.Stopped) ? avp.audioplayer.Length - (long)Beatmap.HitObjects.Last().StartTime : 3000;
            }
            else if (Beatmap.HitObjects.Last() is Beatmap.HitHolder)
            {
                endToTime = (Settings1.Default.Video && !VideoPlayer.Instance.Stopped) ? avp.audioplayer.Length - (long)Beatmap.HitObjects.Last().EndTime : 3000;
            }

            int hitCount = 0;

            foreach (List<IUIObject> list in objGrid.objGrid)
            {
                hitCount += list.Count;
            }

            if (lastIndex >= Beatmap.HitObjects.Count && hitCount < 1)
            {
                End = true;
                imgGridBackground.Visible = false;
            }

            if (End && countToScores < endToTime)
            {
                countToScores += KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;
            }

            if (End && countToScores >= endToTime)
            {
                ScreenManager.ChangeTo(ScorePanel.Instance);
                ScorePanel.Instance.CalcScore(this);
                End = false;
                countToScores = 0;
            }

            long timep = GamePosition - (long)Beatmap.HitObjects.First().StartTime;
            tmProgress.Value = Math.Min(Math.Max(0, (int)(((float)(GamePosition - Beatmap.HitObjects.First().StartTime) / (float)Math.Max(Beatmap.HitObjects.Last().StartTime, Beatmap.HitObjects.Last().EndTime)) * 100f)), 100);

            tmLabel.Text = TimeSpan.FromMilliseconds(timep).ToString(@"mm\:ss");
            if (timep < 1)
            {
                tmLabel.ForegroundColor = Color.Red;
            }
            else
            {
                tmLabel.ForegroundColor = Color.White;
            }

        }
        internal override void RenderObjects()
        {
            base.RenderObjects();
            objGrid.Render();
        }

        public override void Render()
        {
            base.Render();
        }

        internal override void UpdateControls()
        {
            objGrid.Update();
            base.UpdateControls();
        }

        private void changeCoverDisplay(string image)
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
                ccbimg = SpritesContent.cropAtRect(cbimg, new System.Drawing.Rectangle((int)((cbimg.Width - coverSize) / 2), (int)((cbimg.Height - coverSize / 2.2f) / 2f), (int)coverSize, (int)(coverSize)));
                istream = SpritesContent.BitmapToStream(ccbimg);
            }
            else
            {
                istream = SpritesContent.BitmapToStream(cbimg);
            }

            CoverImage.Texture = ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, istream);
        }

        private void Instance_OnPeak(object sender, EventArgs e)
        {
            if (!Visible || (Settings1.Default.Video && !avp.videoplayer.Stopped)) return;

            if (KyunGame.Instance.Player.PlayState != BassPlayState.Playing) return;

            if (particleEngine.ParticleCount > 40) return;

            Screen.ScreenMode actualMode = Screen.ScreenModeManager.GetActualMode();

            int randomNumber = OsuUtils.OsuBeatMap.GetRnd(1, 10, -1);

            for (int a = 0; a < randomNumber; a++)
            {
                switchParticle = OsuBeatMap.rnd.NextBoolean();
                int startLeft = 0;
                int startTop = 0;
                if (switchParticle)
                {
                    startTop = OsuBeatMap.GetRnd(25, actualMode.Height - 25, -1);
                    startLeft = OsuBeatMap.GetRnd(25, actualMode.Width + 500, -1);

                    Particle particle = particleEngine.AddNewParticle(SpritesContent.Instance.MenuSnow,
                        new Vector2((5f * (float)(OsuUtils.OsuBeatMap.rnd.NextDouble() * 2 - 1)) / 10f, Math.Abs(5f * (float)(OsuUtils.OsuBeatMap.rnd.NextDouble() * 2 - 1)) / 10f),
                        new Vector2(startLeft, 0),
                        (30 + OsuBeatMap.rnd.Next(40)) * 100,
                        0.01f * (float)(OsuBeatMap.rnd.NextDouble() * 2f - 1)
                        );

                    particle.Opacity = 0.6f;
                    particle.Scale = (float)OsuBeatMap.rnd.NextDouble(0.1, 0.6);
                    if (KyunGame.xmas)
                        particle.TextureColor = Color.Yellow;
                    particle.StopAtBottom = true;
                }
                else
                {
                    //int startTop = OsuUtils.OsuBeatMap.GetRnd(25, actualMode.Height - 25, -1);
                    startLeft = OsuBeatMap.GetRnd(-50, actualMode.Width + 500, -1);

                    float vel = (float)OsuBeatMap.rnd.NextDouble(0.2, 1);

                    int black_rand = 20;

                    if (KyunGame.xmas)
                    {
                        black_rand = OsuBeatMap.rnd.Next(250, 255);
                    }
                    else
                    {
                        black_rand = OsuBeatMap.rnd.Next(20, 40);
                    }

                    Color ccolor = (squareYesNo) ?
                                    LoadScreen.getColorRange(EnphasisColor[0], EnphasisColor[1], EnphasisColor[2]) :
                                    Color.FromNonPremultiplied(black_rand, black_rand, black_rand, 255);

                    if (KyunGame.xmas && OsuBeatMap.rnd.NextBoolean())
                    {
                        ccolor = LoadScreen.getColorRange(ecolors[1][0], ecolors[1][1], ecolors[1][2]);
                    }

                    Particle particle = particleEngine.AddNewSquareParticle(SpritesContent.Instance.SquareParticle,
                        new Vector2(0, vel),
                        new Vector2(startLeft, actualMode.Height),
                        (30 + OsuBeatMap.rnd.Next(40)) * 100,
                        0.01f * (float)(OsuBeatMap.rnd.NextDouble() * 2f - 1),
                        ccolor
                        );
                    particle.Scale = (float)OsuBeatMap.rnd.NextDouble(0.4, 0.7);
                    particle.Opacity = /*(float)OsuUtils.OsuBeatMap.rnd.NextDouble(0.4, 0.9)*/0.3f;
                    squareYesNo = !squareYesNo;
                }
            }
        }
    }
}
