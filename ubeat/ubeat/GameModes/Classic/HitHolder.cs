using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using kyun.Beatmap;
using kyun.Utils;
using kyun.Score;
using Microsoft.Xna.Framework.Input;

namespace kyun.GameModes.Classic
{
    public class HitHolder : HitSingle
    {

        bool holding;
        long position = 0;
        float porcent = 0;
        private long leaveTime;
        private bool hasLeaveKey;

        public HitHolder(IHitObj hitObject, IBeatmap beatmap, ClassicModeScreen Instance, int gridPosition)
            :base(hitObject, beatmap, Instance, gridPosition)
        { 
            Texture = (Screen.ScreenModeManager.GetActualMode().Height < 650) ?
                  SpritesContent.Instance.ButtonHolder_0 :
                  SpritesContent.Instance.ButtonHolder;

        }
        /*
        public override void Update()
        {


            UpdateTime();
            updateOpacity();



            if (screenInstance.GamePosition > Time && !holding)
            {
                holding = true;
                Audio.CachedSound hsound = null;
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

                Audio.AudioPlaybackEngine.Instance.PlaySound(hsound);
            }

            if(holding && screenInstance.GamePosition > EndTime)
            {
                Audio.CachedSound hsound = null;
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

                Audio.AudioPlaybackEngine.Instance.PlaySound(hsound);
                Died = true;
            }

            position = EndTime - screenInstance.GamePosition;

            position = (EndTime - Time) - position;

            porcent = (float)position / (float)(EndTime - Time) * 100f;

            approachObj.Update();
        }
        */

        internal override void calculateScore()
        {
            base.calculateScore();
        }


        internal override void updateLogic()
        {

            if (screenInstance.gameMod == GameMod.Auto)
            {
                if (screenInstance.GamePosition > Time && !holding)
                {
                    holding = true;
                    pressedTime = Time;
                    playHitsound();
                }

                if (holding && screenInstance.GamePosition > EndTime)
                {
                    leaveTime = EndTime;
                    calculateScore();
                    Died = true;
                }

                position = EndTime - screenInstance.GamePosition;

                position = (EndTime - Time) - position;

                porcent = (float)position / (float)(EndTime - Time) * 100f;

                return;
            }


            bool intersecs = KyunGame.Instance.touchHandler.TouchIntersecs(new Rectangle((int)Position.X, (int)Position.Y, Texture.Height, Texture.Height));
            //No, no and no!
            if (screenInstance.GamePosition < Time - _beatmap.Timing50)
            {
                lastIntersects = intersecs; //Shit 
                return;
            }

            //Touch
            bool actualPressed = (kbActualState.IsKeyDown((Keys)(GridPosition + 96)) || intersecs);
            bool lastPressedUp = kbstatelast.IsKeyUp((Keys)(GridPosition + 96)) || !lastIntersects;

            if (screenInstance.GamePosition > Time + _beatmap.Timing50 && !pressed && !holding)
            {
                //calculateScore(); //I will not mark miss now D:
                pressedTime = screenInstance.GamePosition;
                holding = true;
                hasLeaveKey = true;
                playHitsound();
                return;
            }

            if (actualPressed && !pressed && lastPressedUp && first && !holding)
            {
                pressed = holding = true;
                pressedTime = screenInstance.GamePosition;
                playHitsound();
                return;
            }

            position = EndTime - screenInstance.GamePosition;

            position = (EndTime - Time) - position;

            porcent = (float)position / (float)(EndTime - Time) * 100f;
            
            

            if (!actualPressed && holding)
            {


                if (!hasLeaveKey)
                {
                    leaveTime = EndTime;
                    hasLeaveKey = true;
                }
                //calculateScore(); nop :(
                // pressedTime = screenInstance.GamePosition; //Penalize
                pressed = false;
            }
            else if (actualPressed)
            {
                pressed = true;
            }
            else if (!actualPressed)
            {
                pressed = false;
            }

            if (screenInstance.GamePosition > EndTime - _beatmap.Timing50) 
            {

                if (screenInstance.GamePosition >= EndTime) {
                    if (!hasLeaveKey)
                        leaveTime = screenInstance.GamePosition;

                    calculateScore();
                }
                if (!pressed)
                {
                    //leaveTime = screenInstance.GamePosition;
                    calculateScore();
                }                    
                

            }
            lastIntersects = intersecs;
        }

        internal override ScoreType GetScore()
        {
            float fillPerc = (((float)leaveTime - (float)Time) / (float)length) * 100f;

            if (leaveTime > EndTime - _beatmap.Timing50)
            {
                if (pressedTime >= Time - _beatmap.Timing300 && pressedTime <= Time + _beatmap.Timing300)
                {
                    //Perfect
                    return Score.ScoreType.Perfect;
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
            else if (fillPerc <= 30)
            {
                return Score.ScoreType.Miss;
            }
            else
            {
                //rip?
                if (pressedTime >= Time - _beatmap.Timing300 && pressedTime <= Time + _beatmap.Timing300)
                {
                    //ño
                    return Score.ScoreType.Good;
                }
                else if (pressedTime >= Time - _beatmap.Timing50 && pressedTime <= Time + _beatmap.Timing50)
                {
                    //Bad
                    return Score.ScoreType.Miss;
                }
                else
                {
                    return Score.ScoreType.Miss;
                }
            }
        }

        public override void Render()
        {
            float ppeak = (KyunGame.Instance.maxPeak / 4) + 1;

            if (ppeak > 1.4f)
            {
                ppeak = 1.4f;
            }

            if (holding)
            {

                Texture2D fill = null;
                fill = SpritesContent.Instance.Fill_1;

                for(float a = porcent; a > 0; a -= 1)
                {

                    float circle = (float)(Math.PI * 2d);
                    float cp = circle * a / 100;

                    KyunGame.Instance.SpriteBatch.Draw(fill,
                        new Vector2(this.Position.X + (Texture.Width / 2),
                        this.Position.Y + (Texture.Height / 2)),
                        null,
                        (pressed || screenInstance.gameMod == GameMod.Auto)?Color.White:Color.Red,
                        cp,
                        new Vector2(fill.Width / 2, fill.Height / 2),
                        ppeak,
                        SpriteEffects.None, 0);
                }

            }

            base.Render();
        }
    }
}
