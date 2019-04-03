using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.Beatmap;
using kyun.UIObjs;
using kyun.Utils;
using Microsoft.Xna.Framework.Graphics;
using kyun.Audio;
using Microsoft.Xna.Framework.Input;
using kyun.GameScreen.UI.Particles;
using kyun.Score;
using kyun.game.Beatmap.Generators;

namespace kyun.GameModes.OsuMode
{
    public class HitSingle : HitBase
    {
        internal IHitObj _hitButton;
        internal IBeatmap _beatmap;

        internal ApproachObj approachObj;

        internal OsuMode screenInstance;
        internal bool startShow;
        internal bool lastIntersects;
        internal bool willDie;
        internal bool pressed;

        public bool MoveEnabled = false;
        MouseEvent lastMouse;
        public long replayTime = 0;

        internal Vector2 velocityAndMovement = Vector2.Zero;

        public List<Keys> keyPressed { get; set; }

        public int HitSound
        {
            get
            {
                return _hitButton.HitSound;
            }
        }

        internal long auxTime = -1;

        public long Time
        {
            get
            {
                if (auxTime < 0)
                    return (long)_hitButton.StartTime;
                else
                    return auxTime;
            }
            set
            {
                auxTime = value;
            }
        }

        long auxEndTime = -1;
        public long EndTime
        {
            get
            {
                if (auxEndTime < 0)
                    return (long)_hitButton.EndTime;
                else
                    return auxEndTime;
            }
            set
            {
                auxEndTime = value;
            }
        }

        internal long pressedTime { get; set; }
        public bool IsFirst { get; internal set; }
        internal bool avaiableToClick { get; set; }
        internal bool mouseClicked { get; set; }

        public int Id = 0;
        internal KeyboardState kbLast;

        /// <summary>
        /// New Instance of HitSingle
        /// </summary>
        /// <param name="hitObject">HitObject</param>
        public HitSingle(IHitObj hitObject, IBeatmap beatmap, OsuMode Instance, bool shared = false)
            : base(SpritesContent.Instance.CircleNote)
        {
            lastMouse = MouseHandler.GetState();

            _hitButton = hitObject;
            _beatmap = beatmap;
            screenInstance = Instance;
            float PlayfieldWidth = (int)((512f / 384f) * ((float)Screen.ScreenModeManager.GetActualMode().Height * .95f));
            float CircleRadius = (PlayfieldWidth / 16f) * (1f - (0.7f * (_beatmap.CircleSize - 5f) / 5f));
            if (_beatmap.ApproachRate == 0)
            {
                _beatmap.ApproachRate = _beatmap.OverallDifficulty;
            }
            
            if(screenInstance._osuMode != OsuGameMode.Standard && screenInstance._osuMode != OsuGameMode.CTB)
            {
                if (_beatmap.OverallDifficulty >= 7)
                {
                    _beatmap.ApproachRate = Math.Min(_beatmap.OverallDifficulty + 2, 10);
                }
                else if(_beatmap.OverallDifficulty >= 5)
                {
                    _beatmap.ApproachRate = Math.Min(_beatmap.OverallDifficulty + 2, 10);
                }
            }

            approachObj = new ApproachObj(Position, _beatmap.ApproachRate, Time, Instance);

            float scaledCircle = (CircleRadius * 2) / 160;

            Size = new Vector2(150);

            approachObj.Scale = Scale = scaledCircle;

            //approachObj.Scale = Scale = Math.Min(Math.Max(Math.Abs(_beatmap.OverallDifficulty - 10), 1), 2) / 2;

            approachObj.Texture = SpritesContent.Instance.ApproachCircle;


            Screen.ScreenMode scm = Screen.ScreenModeManager.GetActualMode();

            //Vector2 cent = new Vector2(OsuUtils.OsuBeatMap.rnd.Next(Texture.Width / 2, scm.Width - Texture.Width), OsuUtils.OsuBeatMap.rnd.Next(Texture.Height / 2, scm.Height - Texture.Height));

            Vector2 finalPos = hitObject.OsuLocation;


            switch (screenInstance._osuMode)
            {
                //In Mania case, make spawn in the middle screen
                case OsuGameMode.Mania:
                    //finalPos = getMiddleScreen();
                    finalPos = RandomGenerator.MakeRandom(_beatmap.OverallDifficulty);
                    break;
                //To test
                case OsuGameMode.CTB:
                    break;
                //Same as Mania
                case OsuGameMode.Taiko:
                    finalPos = RandomGenerator.MakeRandom(_beatmap.OverallDifficulty);
                    //finalPos = getMiddleScreen();
                    break;
            }


            Position = CalculatePosition(finalPos);

            approachObj.Position = Position;
            approachObj.Opacity = 0;

            if (shared)
                approachObj.TextureColor = Color.Yellow;
            else
                approachObj.TextureColor = Color.FromNonPremultiplied(14, 201, 255, 255);



            Click += HitSingle_Click;

            Over += HitSingle_Over;
        }

        internal Vector2 getMiddleScreen()
        {
            float width = 512f / 2f;
            float height = 384f / 2f;

            return new Vector2(width, height);
        }

        //internal virtual void HitSingle_Over(object sender, EventArgs e)
        //{
        //    if (kbLast.IsKeyDown(Keys.X) && kbLast.IsKeyDown(Keys.Z))
        //    {
        //        return;
        //    }

        //    KeyboardState kbstate = Keyboard.GetState();

        //    if (kbstate.IsKeyDown(Keys.X) || kbstate.IsKeyDown(Keys.Z))
        //    {
        //        if(kbstate.IsKeyDown(Keys.X) && !kbLast.IsKeyDown(Keys.X))
        //            HitSingle_Click(this, new EventArgs());
        //        else if(kbstate.IsKeyDown(Keys.Z) && !kbLast.IsKeyDown(Keys.Z))
        //            HitSingle_Click(this, new EventArgs());
        //    }
        //}

        internal virtual void HitSingle_Over(object sender, EventArgs e)
        {
            if (kbLast.IsKeyDown(Keys.X) && kbLast.IsKeyDown(Keys.Z) &&
                lastMouse.LeftButton == ButtonState.Pressed && lastMouse.RightButton == ButtonState.Pressed)
            {
                return;
            }

            KeyboardState kbstate = Keyboard.GetState();
            MouseEvent mouseState = MouseHandler.GetState();

            if (kbstate.IsKeyDown(Keys.X) || kbstate.IsKeyDown(Keys.Z)
                || mouseState.RightButton == ButtonState.Pressed || mouseState.LeftButton == ButtonState.Pressed)
            {
                if (kbstate.IsKeyDown(Keys.X) && !kbLast.IsKeyDown(Keys.X))
                    makeClick();
                else if (kbstate.IsKeyDown(Keys.Z) && !kbLast.IsKeyDown(Keys.Z))
                    makeClick();
                else if (mouseState.LeftButton == ButtonState.Pressed && lastMouse.LeftButton != ButtonState.Pressed)
                    makeClick();
                else if (mouseState.RightButton == ButtonState.Pressed && lastMouse.RightButton != ButtonState.Pressed)
                    makeClick();
            }

        }

        internal virtual void makeClick()
        {
            if (!avaiableToClick)
                return;

            mouseClicked = true;
        }

        internal virtual void HitSingle_Click(object sender, EventArgs e)
        {

        }

        private Vector2 CalculatePosition(Vector2 pos)
        {

            Screen.ScreenMode actualMode = Screen.ScreenModeManager.GetActualMode();
            int aWidth = (int)((float)actualMode.Width);
            int aHeight = (int)((float)actualMode.Height * .95f);

            int heightNoScaled = aHeight - Texture.Height;

            int widthScaled = (int)((512f / 384f) * (float)heightNoScaled);
            int widthScaledForPosition = (int)((512f / 384f) * (float)aHeight);

            int heigthScaled = heightNoScaled;

            float porcx = 100f / 512f * pos.X;
            float porcy = 100f / 384f * pos.Y;

            int posX = (int)(porcx * (float)widthScaled / 100f);
            int posY = (int)(porcy * (float)heigthScaled / 100f);
            posX += ((aWidth / 2) - (widthScaledForPosition / 2));

            return new Vector2(posX, posY + (actualMode.Height - aHeight));
        }

        public void Show()
        {
            startShow = true;
            Died = false;
        }

        internal void updateOpacity()
        {
            if (!startShow) return;

            GameTime gt = KyunGame.Instance.GameTimeP;

            float toAdd = 0.005f * (float)gt.ElapsedGameTime.Milliseconds;


            Opacity = Math.Min(toAdd + Opacity, 1);
            approachObj.Opacity = Opacity;


            if (Opacity >= 1)
            {
                startShow = false;

            }

        }

        public override void Update()
        {

            base.Update();

            updateOpacity();
            UpdateTime();
            updateLogic();

            approachObj.Position = Position;
            approachObj.Update();
            kbLast = Keyboard.GetState();
            lastMouse = MouseHandler.GetState();
        }

        internal virtual void updateLogic()
        {

            if (approachObj.Visible && MoveEnabled)
            {
                //party owo

                // ex: Time = 1234

                // 1 = dumb (1 second offset)
                // 2 = Y coord (pair up, odd down), and velocity
                // 3 = X coord (pair left, odd right), and velocity
                // 4 = multiple of 4 activates movement

                if (Time > 1000)
                {
                    if (Math.Abs(Time % 10) % 4 == 0)
                    {
                        if (velocityAndMovement == Vector2.Zero)
                        {
                            //
                            int vX = (int)(Math.Truncate(Math.Abs(Time % 1000d) / 100d));
                            int vY = (int)Math.Ceiling((Math.Abs(Time % 1000d) - (vX * 100)) / 10d);

                            velocityAndMovement = new Vector2((vX % 2 == 0) ? vX : 0 - vX, (vY % 2 == 0) ? vY : 0 - vY);
                        }
                    }
                }

                if (velocityAndMovement != Vector2.Zero)
                {
                    int delta = KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;
                    Position = new Vector2(Position.X + (velocityAndMovement.X / 8) * delta / 10, Position.Y + (velocityAndMovement.Y / 8) * delta / 10);
                }
            }



            //ModAuto
            if ((screenInstance.gameMod & GameMod.Auto) == GameMod.Auto)
            {
                if (IsFirst)
                {


                }
                if (screenInstance.GamePosition >= Time)
                {
                    pressedTime = Time;
                    calculateScore();
                }

                return;
            }

            if ((screenInstance.gameMod & GameMod.Replay) == GameMod.Replay)
            {
                if (screenInstance.GamePosition >= Math.Abs(replayTime))
                {
                    pressedTime = replayTime;
                    calculateScore();
                }

                if (screenInstance.GamePosition < Time - _beatmap.Timing50)
                {
                    return;
                }

                if (screenInstance.GamePosition > Time + _beatmap.Timing50)
                {
                    pressedTime = -1;
                    calculateScore();

                }



                return;
            }


            if (IsFirst)
            {
                avaiableToClick = true;
            }
            else
            {
                HitSingle fowrw = (HitSingle)screenInstance.HitObjects[Id - 1];
                Rectangle fwRect = new Rectangle((int)fowrw.Position.X, (int)fowrw.Position.Y, (int)(fowrw.Size.X * Scale), (int)(fowrw.Size.Y * Scale));
                Rectangle thisRect = new Rectangle((int)Position.X, (int)Position.Y, (int)(Size.X * Scale), (int)(Size.Y * Scale));

                if (!fwRect.Intersects(thisRect))
                {

                    avaiableToClick = fowrw.Died;

                }
            }



            bool intersecs = mouseClicked;

            if (screenInstance.GamePosition < Time - _beatmap.Timing50)
            {
                lastIntersects = intersecs; //Shit 
                return;
            }

            if (willDie)
            {
                return;
            }

            if (screenInstance.GamePosition > Time + _beatmap.Timing50 && !pressed)
            {
                pressedTime = -screenInstance.GamePosition;
                calculateScore();
                return;
            }

            if (intersecs && screenInstance.GamePosition > Time - _beatmap.Timing50)
            {
                pressed = true;
                pressedTime = screenInstance.GamePosition;
                calculateScore();
            }
            //Touch

        }



        internal void UpdateTime()
        {
            long approachStart = (long)(ModeConstants.APPROACH_TIME_BASE - screenInstance.Beatmap.ApproachRate * 150f);
            long nextObjStart = (long)Time - approachStart;
            if (screenInstance.GamePosition > nextObjStart)
            {
                Show();
            }
        }

        internal virtual void calculateScore()
        {
            Died = true;
            var finalScore = GetScore();

            if (finalScore != Score.ScoreType.Miss)
            {


            }

            Texture2D particle = null; //Using a no assingned var

            switch (finalScore)
            {
                case Score.ScoreType.Miss:
                    playHitsound(true);
                    screenInstance._healthBar.Substract((2 * _beatmap.OverallDifficulty) * Math.Max(1, 1));
                    Combo.Instance.Miss();

                    particle = SpritesContent.Instance.MissTx;
                    break;
                case Score.ScoreType.Good:
                    playHitsound();
                    screenInstance._healthBar.Add(1);
                    Combo.Instance.Add();
                    screenInstance.FailsCount = 1;
                    particle = SpritesContent.Instance.GoodTx;
                    break;
                case Score.ScoreType.Excellent:
                    playHitsound();
                    screenInstance._healthBar.Add(2);
                    screenInstance.FailsCount = 1;
                    Combo.Instance.Add();
                    particle = SpritesContent.Instance.ExcellentTx;
                    break;
                case Score.ScoreType.Perfect:
                    playHitsound();
                    screenInstance._healthBar.Add(4);
                    screenInstance.FailsCount = 1;
                    Combo.Instance.Add();
                    particle = SpritesContent.Instance.PerfectTx;
                    break;
            }

            screenInstance._scoreDisplay.Add((((int)finalScore / 50) * Math.Max(Combo.Instance.ActualMultiplier, 1)) / 2);

            screenInstance._scoreDisplay.CalcAcc(finalScore);

            screenInstance._particleEngine.AddNewScoreParticle(particle,
                new Vector2(.05f),
                new Vector2(Position.X + (Texture.Height / 2) - (particle.Width / 2), Position.Y + (Texture.Height / 2) + (particle.Height + 10) * 1.5f),
                10,
                0,
                Color.White
                ).Scale = Scale;

            Particle pr = screenInstance._particleEngine.AddNewHitObjectParticle(Texture,
                       new Vector2(2),
                       new Vector2(Position.X, Position.Y),
                       10,
                       0,
                       Color.White
                       );

            pr.Opacity = .6f;
            pr.Scale = Scale;
            if(finalScore == ScoreType.Miss)
            {
                pr.Opacity = .8f;
                pr.MoveTo(GameScreen.AnimationEffect.Linear, 5000, new Vector2(Position.X, Position.Y + 50));
                pr.TextureColor = Color.Violet;
            }
            

            Died = true;
        }

        internal void playMiss()
        {

        }

        internal virtual void playHitsound(bool miss = false)
        {
            int hsound = 0;
            switch (HitSound)
            {
                case 2:
                    hsound = SpritesContent.Instance.Hitwhistle;
                    break;
                case 4:
                    hsound = SpritesContent.Instance.Hitfinish;
                    break;
                case 8:
                    hsound = SpritesContent.Instance.Hitclap;
                    break;
                default:
                    hsound = SpritesContent.Instance.HitHolder;
                    break;
            }

            EffectsPlayer.PlayEffect(hsound, (miss) ? .2f : 1);
        }


        public override void Render()
        {

            if (Died)
            {
                return;
            }

            base.Render();
            approachObj.Render();

        }

        internal override Score.ScoreType GetScore()
        {

            if (pressedTime >= Time - _beatmap.Timing300 && pressedTime <= Time + _beatmap.Timing300)
            {
                //Perfect
                return Score.ScoreType.Perfect;
            }
            else if (pressedTime >= Time - _beatmap.Timing300 && pressedTime <= Time + _beatmap.Timing100)
            {

                return Score.ScoreType.Excellent;
            }
            else if (pressedTime >= Time - _beatmap.Timing100 && pressedTime <= Time + _beatmap.Timing100)
            {
                //Excellent
                return Score.ScoreType.Excellent;
            }

            else if (pressedTime >= Time - _beatmap.Timing50 && pressedTime <= Time + _beatmap.Timing50)
            {
                //Bad
                return Score.ScoreType.Good;
            }
            else
            {
                return Score.ScoreType.Miss;
            }

        }

        public Score.ScoreValue GetScoreValue()
        {
            Score.ScoreValue sscv = Score.ScoreValue.Miss;
            switch (GetScore())
            {
                case Score.ScoreType.Perfect:
                    sscv = Score.ScoreValue.Perfect;
                    break;
                case Score.ScoreType.Excellent:
                    sscv = Score.ScoreValue.Excellent;
                    break;
                case Score.ScoreType.Good:
                    sscv = Score.ScoreValue.Good;
                    break;

            }
            return sscv;
        }

        internal Vector2 getPointAt(float startX, float startY, float endX, float endY, float t)
        {
            // "autopilot" mod: move quicker between objects

            //t = StringHelper.clamp(t * 2f, 0f, 1f);

            return new Vector2(startX + (endX - startX) * t, startY + (endY - startY) * t);
        }
    }
}
