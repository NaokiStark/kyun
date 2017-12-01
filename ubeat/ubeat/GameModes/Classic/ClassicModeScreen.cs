using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using kyun.Beatmap;
using kyun.GameScreen;
using System.Threading;
using kyun.Utils;
using kyun.GameScreen.UI.Buttons;
using kyun.GameScreen.UI;
using Microsoft.Xna.Framework.Graphics;
using kyun.Score;
using System.IO;
using kyun.GameScreen.UI.Particles;
using kyun.Audio;
using kyun.UIObjs;

namespace kyun.GameModes.Classic
{
    public class ClassicModeScreen : GameModeScreenBase
    {

        int lastIndex = 0;

        HashSet<HitBase> hitbaseObjects = new HashSet<HitBase>();
        private bool End;
        private int countToScores;
        private ButtonStandard backButton;
        private Grid objGrid;
        public GameScreen.UI.Image imgGridBackground;
        public HealthBar _healthBar;
        public ScoreDisplay _scoreDisplay;
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

        public List<HitBase> hitobjects;

        public static ClassicModeScreen GetInstance()
        {
            return (ClassicModeScreen)Instance;
        }

        public ClassicModeScreen()
            : base("ClassicModeScreen")
        {
            hitobjects = new List<HitBase>();
            coverSize = 75;
            AllowVideo = true;
            objGrid = new Grid(10);
            ChangeBackground(KyunGame.Instance.SelectedBeatmap.Background);
            backButton = new ButtonStandard(Color.DarkRed)
            {
                ForegroundColor = Color.White,
                Caption = "Back",
                Position = new Vector2(15, ActualScreenMode.Height - (SpritesContent.Instance.ButtonStandard.Height / 2) - 30),
            };

            backButton.Click += BackButton_Click;
            Controls.Add(backButton);


            imgGridBackground = new GameScreen.UI.Image((Screen.ScreenModeManager.GetActualMode().Height < 650) ?
                  SpritesContent.Instance.ClassicBackground_0 :
                  SpritesContent.Instance.ClassicBackground);
            imgGridBackground.BeatReact = false;


            imgGridBackground.Position = new Vector2((ActualScreenMode.Width / 2) - (imgGridBackground.Texture.Width / 2), (ActualScreenMode.Height / 2) - (imgGridBackground.Texture.Height / 2));

            Controls.Add(imgGridBackground);
            onKeyPress += (obj, args) => {

                if (args.Key == Microsoft.Xna.Framework.Input.Keys.Escape)
                {
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
            
            //CoverImage.Texture = SpritesContent.RoundCorners(CoverImage.Texture, 50);
            
            //CoverImage.Size = new Vector2(100, ((float)CoverImage.Texture.Height / (float)CoverImage.Texture.Width) * 100f);
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


            backButton.Position = new Vector2(Board.Position.X + (Board.Texture.Width /2 - backButton.Texture.Width/2), TitleLabel.Position.Y + (int)TitleLabel.Size.Y + 50);


            _healthBar = new HealthBar(imgGridBackground.Texture.Width - 20, 25);

            _healthBar.Position = new Vector2(ActualScreenMode.Width / 2 - _healthBar.BgBar.Width / 2, ActualScreenMode.Height - _healthBar.BgBar.Height - 10);

            _scoreDisplay = new ScoreDisplay((ActualScreenMode.Height < 700)?1.1f:1.2f);

            _scoreDisplay.Position = new Vector2(_healthBar.Position.X + ((_healthBar.Texture.Width / 2) - (_scoreDisplay.Texture.Width / 2)) + 10, _healthBar.Position.Y - 10 - _scoreDisplay.Texture.Height);

            int progressBSize = imgGridBackground.Texture.Width - 15;

            fcRectangle = new FilledRectangle(new Vector2(progressBSize + 15, 20), Color.Black * .8f);
            fcRectangle.Position = new Vector2(ActualScreenMode.Width / 2 - ((progressBSize + 15) / 2), 15);

            tmProgress = new ProgressBar(progressBSize, 10);
            tmProgress.Position = new Vector2(ActualScreenMode.Width / 2 - (progressBSize / 2), 20);

            Vector2 mStr = SpritesContent.Instance.TitleFont.MeasureString("00:00")*.8f;

            tmLabel = new Label()
            {
                Font = SpritesContent.Instance.TitleFont,
                Text = "00:00",
                Position = new Vector2(fcRectangle.Position.X + (fcRectangle.Texture.Width / 2) - (mStr.X / 2), fcRectangle.Position.Y + fcRectangle.Texture.Height + 10),
                Scale = .8f
                
            };

            _comboDsp = new ComboDisplay();

            _particleEngine = new ParticleEngine();


            Controls.Add(_particleEngine);

            Controls.Add(CoverImage);
            
           // Controls.Add(FillCoverImage);
            Controls.Add(TitleLabel);

            Controls.Add(Board);

            Controls.Add(_comboDsp);

            Controls.Add(_healthBar);
            Controls.Add(_scoreDisplay);

            Controls.Add(tmLabel);           
            Controls.Add(tmProgress);
            Controls.Add(fcRectangle);
            
            
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            ScreenManager.ChangeTo(BeatmapScreen.Instance);
        }

        /// <summary>
        /// Start game
        /// </summary>
        /// <param name="beatmap"></param>
        public override void Play(IBeatmap beatmap, GameMod GameMods = GameMod.None)
        {
            End = false;
            hitobjects.Clear();
            FailsCount = 0;
            _healthBar.Reset();
            _scoreDisplay.Reset();
            Combo.Instance.ResetAll();
            countToScores = 0;
            clearObjects();
            base.gameMod = GameMods;
            KyunGame.Instance.Player.Stop();
            Beatmap = beatmap;
            GamePosition = 0;
            InGame = true;
            lastIndex = 0;
            ChangeBackground(KyunGame.Instance.SelectedBeatmap.Background);
            changeCoverDisplay(KyunGame.Instance.SelectedBeatmap.Background);
            /*
            try
            {
                CoverImage.Texture = SpritesContent.RoundCorners(ContentLoader.LoadTexture(beatmap.Background), 50);
            }
            catch
            {
                CoverImage.Texture = SpritesContent.Instance.DefaultBackground;
            }
            
            
            CoverImage.Size = new Vector2(100, ((float)CoverImage.Texture.Height / (float)CoverImage.Texture.Width) * 100f);
            CoverImage.Position = new Vector2(Board.Position.X + ((Board.Texture.Width / 2) - (CoverImage.Size.X / 2)), Board.Position.Y + 5 + -(CoverImage.Size.Y - CoverImage.Size.X));*/


            TitleLabel.Text = KyunGame.Instance.SelectedBeatmap.Artist + " \n " + KyunGame.Instance.SelectedBeatmap.Title;
            TitleLabel.Text = StringHelper.WrapText(SpritesContent.Instance.DefaultFont, TitleLabel.Text, TitleLabel.Size.X, .65f);

            BackgroundDim = 0.45f;

            if (System.IO.File.Exists(beatmap.Video))
            {
                if (System.IO.File.GetAttributes(beatmap.Video) != System.IO.FileAttributes.Directory)
                {
                    if (Settings1.Default.Video)
                        ScreenBase.AVPlayer.SeekTime(0);
                }
                else
                {
                    AVPlayer.Stop();
                }                    
            }
            _healthBar.Start(beatmap.HPDrainRate);
            _comboDsp.IsActive = true;
        }

        private void clearObjects()
        {
            Controls.RemoveAll(item => item is HitBase);
            objGrid.CleanUp();
        }

        private void togglePause()
        {
            KyunGame.Instance.Player.Paused = !KyunGame.Instance.Player.Paused;
        }

        private void checkObjectsInTime()
        {

            if (KyunGame.Instance.Player.PlayState == BassPlayState.Stopped)
                GamePosition += (long)KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds;
            else
                GamePosition = KyunGame.Instance.Player.Position + Beatmap.SleepTime;

            if (!InGame) return;

            if (InGame && GamePosition > Beatmap.SleepTime && KyunGame.Instance.Player.PlayState == BassPlayState.Stopped)
            {
                KyunGame.Instance.Player.Play(Beatmap.SongPath);
                
                KyunGame.Instance.Player.Volume = KyunGame.Instance.GeneralVolume;
                
            }
            

            if (lastIndex >= Beatmap.HitObjects.Count)
            {
                InGame = false;
                return;
            }

            long actualTime = GamePosition;

            
            IHitObj lastObject = Beatmap.HitObjects[lastIndex];

            long approachStart = (long)(ModeConstants.APPROACH_TIME_BASE - Beatmap.ApproachRate * 150f) + 1000;

            long nextObjStart = (long)lastObject.StartTime - approachStart;


            if (actualTime > nextObjStart)
            {
                int gridPosition = lastObject.Location - 96;

                bool isLastHolder = true;
                int attps = 0;
                //No Overlap all objects with holders
                while (isLastHolder)
                {
                    if(attps > 9)
                    {
                        lastObject = null;
                        break;
                    }
                    attps++;

                    HitBase lastObj = null;
                    foreach(UIObjectBase control in Controls)
                    {
                        if(control is HitBase)
                        {
                            if(gridPosition == ((HitSingle)control).GridPosition)
                                lastObj = (HitBase)control;
                        }
                    }

                    if (lastObj == null)
                    {
                        isLastHolder = false;
                        continue;
                    }
                    
                    if(lastObj is HitHolder)
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
                
                if(lastObject != null)
                {                //New Element
                    if (lastObject is HitButton)
                    {

                        var obj = new HitSingle(lastObject, Beatmap, this, gridPosition);
                        obj.Opacity = 0;
                        //Controls.Add(obj);
                        objGrid.Add(obj, gridPosition - 1);
                        hitobjects.Add(obj);
                    }
                    else
                    {
                        var obj = new HitHolder(lastObject, Beatmap, this, gridPosition);
                        obj.Opacity = 0;
                        //Controls.Add(obj);
                        objGrid.Add(obj, gridPosition - 1);
                        hitobjects.Add(obj);
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

           
            
            int hitCount = 0;
            foreach (List<IUIObject> list in objGrid.objGrid)
            {
                hitCount += list.Count;
            }

            if (lastIndex >= Beatmap.HitObjects.Count && hitCount < 1)
            {
                End = true;
            }

            if (End && countToScores < 3000)
            {
                countToScores += KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;
            }

            if (End && countToScores > 3000)
            {
                
                ScreenManager.ChangeTo(ScorePanel.Instance);
                ScorePanel.Instance.CalcScore();

                //KyunGame.Instance.Player.Play(Beatmap.SongPath);
                End = false;
                countToScores = 0;
            }

            long timep = GamePosition - (long)Beatmap.HitObjects.First().StartTime;
            tmProgress.Value = Math.Min(Math.Max(0,(int)(((float)(GamePosition - Beatmap.HitObjects.First().StartTime) / (float)Math.Max(Beatmap.HitObjects.Last().StartTime, Beatmap.HitObjects.Last().EndTime))*100f)),100);
            
            tmLabel.Text = TimeSpan.FromMilliseconds(timep).ToString(@"mm\:ss");
            if(timep < 1)
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
            try
            {
                imgGridBackground.Render();
                foreach (UIObjectBase obj in Controls.Reverse<UIObjectBase>())
                {
                    if (obj.Texture != null)
                    {
                        if (obj.Texture == SpritesContent.Instance.TopEffect || obj.Texture == SpritesContent.Instance.ClassicBackground || obj.Texture == SpritesContent.Instance.ClassicBackground_0 || obj is ParticleEngine)
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
                }

                objGrid.Render();
                _particleEngine.Render();
            }
            catch
            {

            }


        }

        public override void Render()
        {
            base.Render();
        }


        internal override void UpdateControls()
        {
            
            foreach (UIObjectBase control in Controls)
            {
                control.Update();
            }
            objGrid.Update();
      

        }

        private void changeCoverDisplay(string image)
        {
            
            System.Drawing.Image cimg;

            if (!File.Exists(image))
            {
                MemoryStream mms = new MemoryStream();
                SpritesContent.Instance.DefaultBackground.SaveAsPng(mms, SpritesContent.Instance.DefaultBackground.Width, SpritesContent.Instance.DefaultBackground.Height);
                cimg = System.Drawing.Image.FromStream(mms);
            }
            else if (File.GetAttributes(image) == FileAttributes.Directory)
            {
                MemoryStream mms = new MemoryStream();
                SpritesContent.Instance.DefaultBackground.SaveAsPng(mms, SpritesContent.Instance.DefaultBackground.Width, SpritesContent.Instance.DefaultBackground.Height);
                cimg = System.Drawing.Image.FromStream(mms);
            }
            else
            {
                cimg = System.Drawing.Image.FromFile(image);
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

            CoverImage.Texture = Texture2D.FromStream(KyunGame.Instance.GraphicsDevice, istream);
            
        }


    }
}
