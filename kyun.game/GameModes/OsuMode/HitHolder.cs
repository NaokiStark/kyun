using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.Beatmap;
using kyun.Utils;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using kyun.Score;

namespace kyun.GameModes.OsuMode
{
    public class HitHolder : HitSingle
    {

        bool holding;
        private long leaveTime;
        private long position;
        private float porcent;
        private float length;

        public HitHolder(IHitObj hitObject, IBeatmap beatmap, OsuMode Instance, bool shared = false)
            :base(hitObject, beatmap, Instance)
        {
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
                for (float a = 0; a < porcent;)
                {
                    a += circlepresition;
                    float circle = (float)(Math.PI * 2d);
                    float cp = circle * (a - circlepresition) / 100;

                    if ((int)a % 4 != 0)
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
                    if (porcent <= a && (int)porcent < 96)
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
                       new Vector2(this.Position.X - ((Texture.Width * Math.Max(ppeak, .9f)) - Texture.Width) / 2,
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
