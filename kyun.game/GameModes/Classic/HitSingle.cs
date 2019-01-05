using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.Beatmap;
using kyun.Screen;
using kyun.UIObjs;
using kyun.Utils;
using Microsoft.Xna.Framework.Input;
using kyun.Score;
using kyun.GameScreen.UI.Particles;
using kyun.Audio;

namespace kyun.GameModes.Classic
{
    public class HitSingle : HitBase
    {
        internal IHitObj _hitButton;
        internal IBeatmap _beatmap;

        internal ApproachObj approachObj;

        internal ClassicModeScreen screenInstance;
        public int HitSound {
            get
            {
                return _hitButton.HitSound;
            }
        }

        private bool startShow;

        public int GridPosition { get; private set; }

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

        internal long length
        {
            get
            {
                return EndTime - Time;
            }
        }

        public int ReplayId = 0;

        public int TimeToDie = 100;
        public bool first;
        public bool shared;

        internal bool pressed;
        internal long pressedTime;
        internal bool willDie;
        

        /// <summary>
        /// New Instance of HitSingle
        /// </summary>
        /// <param name="hitObject">HitObject</param>
        public HitSingle(IHitObj hitObject, IBeatmap beatmap, ClassicModeScreen Instance, int gridPosition, bool _shared = false)
            : base(SpritesContent.Instance.ButtonDefault)
        {

            shared = _shared;
            _hitButton = hitObject;
            _beatmap = beatmap;
            screenInstance = Instance;
            approachObj = new ApproachObj(Position, _beatmap.ApproachRate, Time, Instance);
            
            if (shared)
                approachObj.TextureColor = Color.Yellow;
            else
                approachObj.TextureColor = Color.FromNonPremultiplied(14, 201, 255, 255);

            GridPosition = gridPosition;
            

            Screen.ScreenMode scm = Screen.ScreenModeManager.GetActualMode();

            //Vector2 cent = new Vector2(OsuUtils.OsuBeatMap.rnd.Next(Texture.Width / 2, scm.Width - Texture.Width), OsuUtils.OsuBeatMap.rnd.Next(Texture.Height / 2, scm.Height - Texture.Height));


            Position = GetPositionFor(GridPosition);

            
            approachObj.Position = Position;
            approachObj.Opacity = 0;

            kbstatelast = new KeyboardState();
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

        internal KeyboardState kbstatelast = Keyboard.GetState();
        public KeyboardState kbActualState;

        internal bool lastIntersects;
        internal virtual void updateLogic()
        {
            

            //ModAuto
            if((screenInstance.gameMod & GameMod.Auto) == GameMod.Auto)
            {
                if (screenInstance.GamePosition >= Time)
                {
                    pressedTime = screenInstance.GamePosition;
                    calculateScore();
                }

                return;
            }

            if ((screenInstance.gameMod & GameMod.Replay) == GameMod.Replay)
            {
                var pressed = screenInstance.replay.Hits[ReplayId].PressedAt;

                if (screenInstance.GamePosition >= pressed)
                {
                    pressedTime = pressed;
                    calculateScore();
                }

                return;
            }

            bool intersecs = KyunGame.Instance.touchHandler.TouchIntersecs(new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y));

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



            //Touch
            

            bool actualPressed = (kbActualState.IsKeyDown((Keys)(GridPosition + 96)) || intersecs);


            if ((actualPressed && !pressed && (kbstatelast.IsKeyUp((Keys)(GridPosition + 96)) || !lastIntersects)) && first)
            {
                pressed = true;
                pressedTime = screenInstance.GamePosition;
            }
            /*
            if (!pressed && kbstatelast.IsKeyUp((Keys)(GridPosition + 96)) && kbActualState.IsKeyDown((Keys)(GridPosition + 96)))
            {
                pressed = true;
                pressedTime = screenInstance.GamePosition;
            }

            if (screenInstance.GamePosition > Time + _beatmap.Timing50 && !pressed)
            {
                calculateScore();
            }
            */
            

            if (pressed)
            {
                calculateScore();
                
                /*
                if(pressedTime > Time - _beatmap.Timing50)
                {
                    willDie = true;
                    calculateScore();
                    return;
                }*/
            }

            lastIntersects = intersecs;
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

                    screenInstance._healthBar.Substract((2 * _beatmap.OverallDifficulty) * screenInstance.FailsCount);
                    screenInstance.FailsCount++;
                    Combo.Instance.Miss();

                    

                    particle = SpritesContent.Instance.MissTx;
                    break;
                case Score.ScoreType.Good:
                    screenInstance.FailsCount = 0;
                    screenInstance._scoreDisplay.Add((int)Score.ScoreValue.Good);
                    Combo.Instance.Add();
                    particle = SpritesContent.Instance.GoodTx;
                    break;
                case Score.ScoreType.Excellent:
                    screenInstance.FailsCount = 0;
                    screenInstance._healthBar.Add(2);
                    screenInstance._scoreDisplay.Add((int)Score.ScoreValue.Excellent);
                    Combo.Instance.Add();
                    particle = SpritesContent.Instance.ExcellentTx;
                    break;
                case Score.ScoreType.Perfect:
                    screenInstance.FailsCount = 0;
                    screenInstance._healthBar.Add(4);
                    screenInstance._scoreDisplay.Add((int)Score.ScoreValue.Perfect);
                    Combo.Instance.Add();
                    particle = SpritesContent.Instance.PerfectTx;
                    break;
            }


            screenInstance._scoreDisplay.CalcAcc(finalScore);

            screenInstance._particleEngine.AddNewScoreParticle(particle,
                new Vector2(.05f),
                new Vector2(screenInstance.imgGridBackground.Position.X + (screenInstance.imgGridBackground.Texture.Width / 2) - (particle.Width * RenderScale / 2), screenInstance.imgGridBackground.Position.Y + (screenInstance.imgGridBackground.Texture.Height / 2) + (particle.Height * RenderScale + 10)*1.5f ),
                10,
                0,
                Color.White
                );
            /*
            screenInstance._particleEngine.AddNewHitObjectParticle(SpritesContent.Instance.Radiance,
                       new Vector2(.05f),
                       new Vector2(Position.X, Position.Y),
                       10,
                       0,
                       Color.White
                       );*/

            screenInstance._particleEngine.AddNewHitObjectParticle(Texture,
               new Vector2(2f),
               new Vector2(Position.X, Position.Y),
               10,
               0,
               Color.White
               ).Opacity = .6f; 

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

        internal void UpdateTime()
        {
            long approachStart = (long)(ModeConstants.APPROACH_TIME_BASE - screenInstance.Beatmap.ApproachRate * 150f);
            long nextObjStart = (long)Time - approachStart;
            if (screenInstance.GamePosition > nextObjStart)
            {
                Show();
            }
        }

        public override void Render()
        {
            if (Died){
                return;
            }

            base.Render();
            approachObj.Render();     

        }

        public static Vector2 GetPositionFor(int index)
        {

            int posYY = 0;
            int posXX = index;

            if (index > 6)
            {
                posYY = 1;
                if (index == 7)
                    posXX = 1;
                else if (index == 8)
                    posXX = 2;
                else if (index == 9)
                    posXX = 3;
            }
            else if (index > 3)
            {
                posYY = 2;
                if (index == 4)
                    posXX = 1;
                else if (index == 5)
                    posXX = 2;
                else if (index == 6)
                    posXX = 3;
            }
            else if (index > 0)
            {
                posYY = 3;
                posXX = index;
            }

            ScreenMode mode = ScreenModeManager.GetActualMode();
            //bool isSmallRes = mode.Height < 600 && mode.Width < 1000;

            Texture2D txbtn = SpritesContent.Instance.ButtonDefault;

            float scaling = ScreenModeManager.ScreenScaling();

            int x = (mode.Width / 2) + (int)(txbtn.Bounds.Width * scaling + 20) * posXX;
            int y = (mode.Height / 2) + (int)(txbtn.Bounds.Height * scaling + 20) * posYY;


            x = x - (int)(txbtn.Bounds.Width * scaling + 20) * 2 - (int)(txbtn.Bounds.Width * scaling / 2);
            y = y - (int)(txbtn.Bounds.Height * scaling + 20) * 2 - (int)(txbtn.Bounds.Height * scaling / 2);
            return new Vector2(x, y);
        }

        internal virtual Score.ScoreType GetScore()
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
