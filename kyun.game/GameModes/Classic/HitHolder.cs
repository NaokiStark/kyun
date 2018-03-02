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
using System.IO;

namespace kyun.GameModes.Classic
{
    public class HitHolder : HitSingle
    {

        bool holding;
        long position = 0;
        float porcent = 0;
        public long leaveTime;
        private bool hasLeaveKey;
        private int lastPorcentage = 0;

        public int ReplayId = 0;

        private Texture2D fillCache;

        public HitHolder(IHitObj hitObject, IBeatmap beatmap, ClassicModeScreen Instance, int gridPosition, bool _shared = false)
            :base(hitObject, beatmap, Instance, gridPosition)
        {
            shared = _shared;
            Texture = (Screen.ScreenModeManager.GetActualMode().Height < 650 && Screen.ScreenModeManager.GetActualMode().Width < 1000) ?
                  SpritesContent.Instance.ButtonHolder_0 :
                  SpritesContent.Instance.ButtonHolder;

            if (shared)
                approachObj.TextureColor = Color.Yellow;
            else
                approachObj.TextureColor = Color.FromNonPremultiplied(255, 66, 11, 255);
        }

        internal override void calculateScore()
        {
            base.calculateScore();
        }

        
        internal override void updateLogic()
        {
           

            if ((screenInstance.gameMod & GameMod.Auto) == GameMod.Auto)
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

            if((screenInstance.gameMod & GameMod.Replay) == GameMod.Replay)
            {
                var pressedd = screenInstance.replay.Hits[ReplayId].PressedAt;
                var leaved = screenInstance.replay.Hits[ReplayId].LeaveAt;
                if (screenInstance.GamePosition > pressedd && !holding)
                {
                    holding = pressed = true;
                    pressedTime = pressedd;
                    hasLeaveKey = false;
                    playHitsound();
                }

                if (holding && screenInstance.GamePosition > leaved)
                {
                    leaveTime = leaved;
                    hasLeaveKey = true;
                }

                if (screenInstance.GamePosition > EndTime - _beatmap.Timing50)
                {

                    if (screenInstance.GamePosition >= EndTime)
                    {
                        if (!hasLeaveKey)
                            leaveTime = screenInstance.GamePosition;

                        calculateScore();
                    }
                    /*
                     * I think this will make bug
                     * 
                    if (!pressed)
                    {
                        //leaveTime = screenInstance.GamePosition;
                        calculateScore();
                    }   */


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
                /*
                 * I think this will make bug
                 * 
                if (!pressed)
                {
                    //leaveTime = screenInstance.GamePosition;
                    calculateScore();
                }   */                 
                

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

                


                float circlepresition = 1.07f;
                for(float a = 0; a < porcent;)
                {
                    a += circlepresition;
                    float circle = (float)(Math.PI * 2d);
                    float cp = circle * (a - circlepresition) / 100;
                                       
                    if((int)a % 4 != 0)
                    {
                        KyunGame.Instance.SpriteBatch.Draw(fill,
                            new Vector2(this.Position.X + (Texture.Width / 2),
                            this.Position.Y + (Texture.Height / 2)),
                            null,
                            (pressed || (screenInstance.gameMod & GameMod.Auto) == GameMod.Auto) ? Color.White : Color.Red,
                            cp,
                            new Vector2(fill.Width / 2, fill.Height / 2),
                            ppeak,
                            SpriteEffects.None, 0);
                    }

                        
                    if (a - circlepresition <= 0)
                    {

                        KyunGame.Instance.SpriteBatch.Draw(SpritesContent.Instance.FillStartEnd,
                            new Vector2(this.Position.X + (Texture.Width / 2),
                            this.Position.Y + (Texture.Height / 2)),
                            null,
                            Color.White,
                            0,
                            new Vector2(SpritesContent.Instance.FillStartEnd.Width / 2, SpritesContent.Instance.FillStartEnd.Height / 2),
                            ppeak,
                            SpriteEffects.None, 0);
                    }
                    if (porcent <= a && (int) porcent < 96)
                    {
                        float cc = cp + 0.25f;
                        KyunGame.Instance.SpriteBatch.Draw(SpritesContent.Instance.FillStartEnd,
                            new Vector2(this.Position.X + (Texture.Width / 2),
                            this.Position.Y + (Texture.Height / 2)),
                            null,
                            Color.White,
                            cc,
                            new Vector2(SpritesContent.Instance.FillStartEnd.Width / 2, SpritesContent.Instance.FillStartEnd.Height / 2),
                            ppeak,
                            SpriteEffects.None, 0);
                    }
                }

                float cpeak = Math.Max(Math.Min(ppeak, .3f), 0.2f);


                KyunGame.Instance.SpriteBatch.Draw(SpritesContent.Instance.Radiance,
                       new Vector2(this.Position.X - ((Texture.Width * Math.Max(ppeak,.9f)) - Texture.Width) / 2,
                       this.Position.Y - ((Texture.Height * Math.Max(ppeak, .9f)) - Texture.Height) / 2),
                       null,
                       ((pressed || (screenInstance.gameMod & GameMod.Auto) == GameMod.Auto) ? Color.White : Color.Red) * cpeak,
                       0,
                       Vector2.Zero,
                       Math.Max(ppeak, .9f),
                       SpriteEffects.None, 0);
                //Scale = Math.Max(0.99f, Math.Min(ppeak / 1.2f, 1.04f));
            }

           

            base.Render();
        }
    }
}
