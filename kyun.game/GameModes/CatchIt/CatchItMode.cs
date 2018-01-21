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

namespace kyun.game.GameModes.CatchIt
{
    public class CatchItMode : GameModeScreenBase
    {
        static CatchItMode Instance;
        private int lastIndex;

        public Image Player;
        public Vector2 PlayerSize;
        public int playerLinePosition = 1;
        public int fieldSlots = 4;

        public Vector2 FieldSize = new Vector2();
        public Vector2 FieldPosition = new Vector2();
        private FilledRectangle fg;
        public int Margin = 2;

        List<FilledRectangle> rectangles = new List<FilledRectangle>();
        List<HitObject> HitObjectsRemain = new List<HitObject>();
        private ComboDisplay comboDisplay;
        private long endToTime;
        private long countToScores;
        public ParticleEngine particleEngine;
        public HealthBar _healthbar;

        public bool End { get; private set; }
        public float FailsCount { get { return 1; } }

        public static CatchItMode GetInstance()
        {
            if (Instance == null)
                Instance = new CatchItMode();
            return Instance;
        }

        public CatchItMode() : base("CatchIt")
        {
            AllowVideo = true;

            HitObjects = new List<HitBase>();
            BackgroundDim = 0.45f;

            Player = new Image(SpritesContent.Instance.Catcher);
            Player.BeatReact = true;
            //Player.TextureColor = Color.ForestGreen;

            PlayerSize = new Vector2(Math.Max(Math.Min(Player.Texture.Width, 100), 50), Math.Max(Math.Min(Player.Texture.Height, 100), 50));

            FieldSize = new Vector2(ActualScreenMode.Width, (PlayerSize.Y + Margin) * fieldSlots);
            //FieldPosition = new Vector2(0, (ActualScreenMode.Height / 2) - (FieldSize.Y / 2) - PlayerSize.Y);
            FieldPosition = new Vector2(0, 0);

            Player.Size = PlayerSize;

            for(int a = 0; a < fieldSlots; a++)
            {
                var rgnl = new FilledRectangle(new Vector2(ActualScreenMode.Width, PlayerSize.Y), Color.Black * .8f);
                rgnl.Position = new Vector2(0, FieldPosition.Y + (PlayerSize.Y + Margin) * (a + 1));
                rgnl.Opacity = .9f;
                rectangles.Add(rgnl);
                Controls.Add(rectangles[a]);
            }

            comboDisplay = new ComboDisplay()
            {
                Position = new Vector2(0, FieldSize.Y)
            };

            _scoreDisplay = new ScoreDisplay((ActualScreenMode.Height < 700 && ActualScreenMode.Width < 1000) ? 1.1f : 1.2f);
           

            particleEngine = new ParticleEngine();

            _healthbar = new HealthBar(this, SpritesContent.Instance.Healthbar.Width - 5, 25);
            _healthbar.Position = new Vector2(0, FieldPosition.Y + FieldSize.Y + 2 + PlayerSize.Y);
            _scoreDisplay.Position = new Vector2(0, _healthbar.Position.Y + 30);

            _healthbar.OnFail += _healthbar_OnFail;

            Controls.Add(Player);
            Controls.Add(comboDisplay);
            Controls.Add(_scoreDisplay);
            Controls.Add(_healthbar);

            Controls.Add(particleEngine);

            onKeyPress += CatchItMode_onKeyPress;
            OnScroll += CatchItMode_OnScroll;
        }

        private void _healthbar_OnFail()
        {
            KyunGame.Instance.Player.Stop();
            InGame = false;
            PauseOverlay.ShowFailed(this);
            ScreenManager.ShowOverlay(PauseOverlay.Instance);

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

        }

        private void CatchItMode_onKeyPress(object sender, kyun.GameScreen.InputEvents.KeyPressEventArgs args)
        {
            switch (args.Key)
            {
                case Microsoft.Xna.Framework.Input.Keys.Up:
                case Microsoft.Xna.Framework.Input.Keys.Left:
                    if(playerLinePosition > 1)
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                            playerLinePosition = Math.Max(playerLinePosition - 2, 1);
                        else
                            playerLinePosition--;
                    }
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Down:
                case Microsoft.Xna.Framework.Input.Keys.Right:
                    if (playerLinePosition < fieldSlots)
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                            playerLinePosition = Math.Min(playerLinePosition + 2, fieldSlots);
                        else
                            playerLinePosition++;
                    }
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Escape:
                    togglePause();
                    break;
            }

        }

        private void clearObjects()
        {
            Controls.RemoveAll(item => item is HitBase);
            HitObjectsRemain.Clear();
            HitObjects.Clear();
            particleEngine.Clear();
        }

        public override void Play(IBeatmap beatmap, GameMod GameMods = GameMod.None)
        {            
            End = false;
            GamePosition = 0;
            lastIndex = 0;
            countToScores = 0;
            _healthbar.Reset();
            _healthbar.Start(0);
            _scoreDisplay.Reset();
            KyunGame.Instance.Player.Stop();
            ChangeBackground(KyunGame.Instance.SelectedBeatmap.Background);
            Beatmap = beatmap;
            playerLinePosition = 1;
            lastIndex = 0;
            _scoreDisplay.IsActive = true;

            FieldSize = new Vector2(ActualScreenMode.Width, (PlayerSize.Y + Margin) * fieldSlots);
            //FieldPosition = new Vector2(0, (ActualScreenMode.Height / 2) - (FieldSize.Y / 2) - PlayerSize.Y);
            FieldPosition = new Vector2(0, 0);

            _healthbar.Position = new Vector2(0, FieldPosition.Y + FieldSize.Y + 2 + PlayerSize.Y);
            _scoreDisplay.Position = new Vector2(0, _healthbar.Position.Y + 48);

            _healthbar.IsActive = true;
            gameMod = GameMods;
            Combo.Instance.ResetAll();

            clearObjects();
            
            InGame = true;
        }

        public void Play(IBeatmap beatmap, GameMod GameMods, int slots)
        {
            fieldSlots = slots;
            Play(beatmap, GameMods);
        }

        internal void checkObjects()
        {
            if (KyunGame.Instance.Player.PlayState == BassPlayState.Stopped)
                GamePosition += (long)KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds;
            else
                GamePosition = KyunGame.Instance.Player.Position + Beatmap.SleepTime;

            if (!InGame) return;

            if (InGame && GamePosition > Beatmap.SleepTime && KyunGame.Instance.Player.PlayState == BassPlayState.Stopped)
            {
                KyunGame.Instance.Player.Play(Beatmap.SongPath, ((gameMod & GameMod.DoubleTime) == GameMod.DoubleTime) ? 1.5f : 1f);

                KyunGame.Instance.Player.Volume = KyunGame.Instance.GeneralVolume;

            }

            if (lastIndex >= Beatmap.HitObjects.Count)
            {
                InGame = false;
                return;
            }

            long actualTime = GamePosition;


            IHitObj lastObject = Beatmap.HitObjects[lastIndex];

            long approachStart = (long)(ModeConstants.APPROACH_TIME_BASE - Beatmap.ApproachRate * 150f) + 5000;

            long nextObjStart = (long)lastObject.StartTime - approachStart;


            if (actualTime > nextObjStart)
            {

                if (lastObject is HitButton)
                {
                    
                    var obj = new HitObject(lastObject, Beatmap, this);

                    HitObjects.Add(obj);
                    HitObjectsRemain.Add(obj);
                    Controls.Add(obj);
                }
                else
                {
                    var obj = new HitObject(lastObject, Beatmap, this);
                    
                    HitObjects.Add(obj);
                    HitObjectsRemain.Add(obj);
                    Controls.Add(obj);

                    var obj2 = new HitObject(lastObject, Beatmap, this, false, true);
                    obj2.TextureColor = Color.Red;
                    HitObjects.Add(obj2);
                    HitObjectsRemain.Add(obj2);
                    Controls.Add(obj2);
                }

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

        public override void Update(GameTime tm)
        {
            updatePlayer();
            checkObjects();
            base.Update(tm);
            Controls.RemoveAll(item => item.Died);
            HitObjectsRemain.RemoveAll(item => item.Died);

            endToTime = (Settings1.Default.Video && !VideoPlayer.Instance.Stopped) ? avp.audioplayer.Length - (long)Beatmap.HitObjects.Last().EndTime : 3000;

            int hitCount = HitObjectsRemain.Count;

            if (lastIndex >= Beatmap.HitObjects.Count && hitCount < 1)
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

            int inFieldPosition = (int)(FieldSize.Y / fieldSlots);

            inFieldPosition *= playerLinePosition;

            Player.Position = new Vector2(0, inFieldPosition + FieldPosition.Y);
        }

        public override void Render()
        {
            base.Render();
        }
    }
}
