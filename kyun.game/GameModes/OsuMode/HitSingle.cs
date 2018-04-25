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

        public List<Keys> keyPressed { get; set; }

        public int HitSound
        {
            get
            {
                return _hitButton.HitSound;
            }
        }

        public long Time
        {
            get
            {
                return (long)_hitButton.StartTime;
            }
        }

        public long EndTime
        {
            get
            {
                return (long)_hitButton.EndTime;
            }
        }

        internal long pressedTime { get; set; }
        public bool IsFirst { get; internal set; }




        /// <summary>
        /// New Instance of HitSingle
        /// </summary>
        /// <param name="hitObject">HitObject</param>
        public HitSingle(IHitObj hitObject, IBeatmap beatmap, OsuMode Instance, bool shared = false)
            : base((Screen.ScreenModeManager.GetActualMode().Height < 650 && Screen.ScreenModeManager.GetActualMode().Width < 1000) ?
                  SpritesContent.Instance.ButtonDefault_0 :
                  SpritesContent.Instance.ButtonDefault)
        {
            
            _hitButton = hitObject;
            _beatmap = beatmap;
            screenInstance = Instance;
            approachObj = new ApproachObj(Position, _beatmap.ApproachRate, Time, Instance);

            Screen.ScreenMode scm = Screen.ScreenModeManager.GetActualMode();

            //Vector2 cent = new Vector2(OsuUtils.OsuBeatMap.rnd.Next(Texture.Width / 2, scm.Width - Texture.Width), OsuUtils.OsuBeatMap.rnd.Next(Texture.Height / 2, scm.Height - Texture.Height));

            Position = CalculatePosition(hitObject.OsuLocation);
            approachObj.Position = Position;
            approachObj.Opacity = 0;

            if (shared)
                approachObj.TextureColor = Color.Yellow;
            else
                approachObj.TextureColor = Color.FromNonPremultiplied(14, 201, 255, 255);
        }

        private Vector2 CalculatePosition(Vector2 pos)
        {
            
            Screen.ScreenMode actualMode = Screen.ScreenModeManager.GetActualMode();

            int heightNoScaled = actualMode.Height - Texture.Height;

            int widthScaled = (int)((512f / 384f) * (float)heightNoScaled);
            int widthScaledForPosition = (int)((512f / 384f) * (float)actualMode.Height);

            int heigthScaled = heightNoScaled;

            float porcx = 100f / 512f * pos.X;
            float porcy = 100f / 384f * pos.Y;

            int posX = (int)(porcx * (float)widthScaled / 100f);
            int posY = (int)(porcy * (float)heigthScaled / 100f);
            posX += ((actualMode.Width / 2) - (widthScaledForPosition / 2));

            return new Vector2(posX, posY);
        }

        public void Show()
        {
            startShow = true;
        }

        internal void updateOpacity()
        {
            if (!startShow) return;

            GameTime gt = KyunGame.Instance.GameTimeP;

            float toAdd = 0.009f * (float)gt.ElapsedGameTime.Milliseconds;

            if(Opacity + toAdd < 1)
            {
                Opacity += toAdd;
                approachObj.Opacity = Opacity;
            }

        }

        public override void Update()
        {


            updateOpacity();
            UpdateTime();
            updateLogic();

            approachObj.Position = Position;
            approachObj.Update();

        }

        internal virtual void updateLogic()
        {


            //ModAuto
            if ((screenInstance.gameMod & GameMod.Auto) == GameMod.Auto)
            {
                if (screenInstance.GamePosition >= Time)
                {
                    pressedTime = screenInstance.GamePosition;
                    calculateScore();
                }

                return;
            }

            if (!IsFirst)
                return;

            bool intersecs = KyunGame.Instance.touchHandler.TouchIntersecs(new Rectangle((int)Position.X, (int)Position.Y, Texture.Height, Texture.Height));

            Rectangle mouseRec = new Rectangle((int)MouseHandler.GetState().Position.X, (int)MouseHandler.GetState().Position.Y, 10, 10);

            bool mouseH = mouseRec.Intersects(new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width * Scale), (int)(Texture.Height * Scale)));
            
            bool kapressed = false;
            bool kbpressed = false;

            if (keyPressed != null)
            {
                kapressed = keyPressed.Exists(x => x == Keys.Z);
                kbpressed = keyPressed.Exists(x => x == Keys.X);
            }
            kapressed = !kapressed && Keyboard.GetState().IsKeyDown(Keys.Z);
            kbpressed = !kbpressed && Keyboard.GetState().IsKeyDown(Keys.X);
            bool kpressed = kapressed || kbpressed;

            bool mousea = MouseHandler.GetState().LeftButton == ButtonState.Pressed;
            bool mouseb = MouseHandler.GetState().RightButton == ButtonState.Pressed;
            bool mousec = mousea || mouseb;

            mouseH = mouseH && (mousec || kpressed);

            intersecs = intersecs || mouseH;

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
                pressedTime = 0;
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

            var finalScore = GetScore();

            if (finalScore != Score.ScoreType.Miss)
            {


            }

            playHitsound();

            Texture2D particle = null; //Using a no assingned var

            switch (finalScore)
            {
                case Score.ScoreType.Miss:

                    //screenInstance._healthBar.Substract((2 * _beatmap.OverallDifficulty) * screenInstance.FailsCount);
                    //screenInstance.FailsCount++;
                    //Combo.Instance.Miss();
                    /*
                    if (Combo.Instance.ActualMultiplier >= 10)
                        EffectsPlayer.PlayEffect(SpritesContent.Instance.ComboBreak);*/

                    particle = SpritesContent.Instance.MissTx;
                    break;
                case Score.ScoreType.Good:
                    /*
                    screenInstance.FailsCount = 0;
                    screenInstance._scoreDisplay.Add((int)Score.ScoreValue.Good);
                    Combo.Instance.Add();*/
                    particle = SpritesContent.Instance.GoodTx;
                    break;
                case Score.ScoreType.Excellent:
                   /* screenInstance.FailsCount = 0;
                    screenInstance._healthBar.Add(2);
                    screenInstance._scoreDisplay.Add((int)Score.ScoreValue.Excellent);
                    Combo.Instance.Add();*/
                    particle = SpritesContent.Instance.ExcellentTx;
                    break;
                case Score.ScoreType.Perfect:
                    /*
                    screenInstance.FailsCount = 0;
                    screenInstance._healthBar.Add(4);
                    screenInstance._scoreDisplay.Add((int)Score.ScoreValue.Perfect);
                    Combo.Instance.Add();*/
                    particle = SpritesContent.Instance.PerfectTx;
                    break;
            }
            
            screenInstance._particleEngine.AddNewScoreParticle(particle,
                new Vector2(.05f),
                new Vector2(Position.X + (Texture.Height / 2) - (particle.Width / 2), Position.Y + (Texture.Height / 2) + (particle.Height + 10) * 1.5f),
                10,
                0,
                Color.White
                );
            
            screenInstance._particleEngine.AddNewHitObjectParticle(SpritesContent.Instance.Radiance,
                       new Vector2(.05f),
                       new Vector2(Position.X, Position.Y),
                       10,
                       0,
                       Color.White
                       );
                       /*
            screenInstance._particleEngine.AddNewHitObjectParticle(Texture,
               new Vector2(.05f),
               new Vector2(Position.X, Position.Y),
               10,
               0,
               Color.White
               );*/

            Died = true;
        }

        internal void playMiss()
        {

        }

        internal virtual void playHitsound()
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

            EffectsPlayer.PlayEffect(hsound);
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


    }
}
