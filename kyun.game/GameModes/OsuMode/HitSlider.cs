﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.Beatmap;
using kyun.Utils;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using kyun.Score;
using Microsoft.Xna.Framework.Input;
using static kyun.GameScreen.ScreenBase;
using kyun.game.Utils;

namespace kyun.GameModes.OsuMode
{
    public class HitSlider : HitSingle
    {

        public bool holding;
        private long leaveTime;
        private long position;
        private float porcent;
        private float length;

        public bool Missed = false;

        private bool replayhasmissed = false;

        bool issuedKeypress = false;

        MouseEvent lastMouse;
        private Texture2D finalTexture;

        public HitSlider(IHitObj hitObject, IBeatmap beatmap, OsuMode Instance, bool shared = false)
            : base(hitObject, beatmap, Instance)
        {
            lastMouse = MouseHandler.GetState();
            Texture = SpritesContent.Instance.CircleNoteHolder;
            approachObj.Texture = SpritesContent.Instance.ApproachCircle;
            // approachObj.Scale = Scale = Math.Min(Math.Max(Math.Abs(_beatmap.OverallDifficulty - 10), 1), 2) / 2;

            float PlayfieldWidth = (int)((512f / 384f) * ((float)Screen.ScreenModeManager.GetActualMode().Height * .95f));
            float CircleRadius = (PlayfieldWidth / 16f) * (1f - (0.7f * (_beatmap.CircleSize - 5f) / 5f));
            float scaledCircle = (CircleRadius * 2) / 160;

            approachObj.Scale = Scale = scaledCircle;
            Size = new Vector2(150);

            if (shared)
                approachObj.TextureColor = Color.Yellow;
            else
                approachObj.TextureColor = Color.FromNonPremultiplied(255, 66, 11, 255);


        }

        internal override void HitSingle_Over(object sender, EventArgs e)
        {
            if (kbLast.IsKeyDown(Keys.X) && kbLast.IsKeyDown(Keys.Z) &&
                lastMouse.LeftButton == ButtonState.Pressed && lastMouse.RightButton == ButtonState.Pressed && holding)
            {
                return;
            }

            KeyboardState kbstate = KyunGame.Instance.KeyboardActualState;
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
            else
            {
                if (holding)
                {
                    mouseClicked = false;
                }
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

        internal override void calculateScore()
        {
            Died = true;
            base.calculateScore();
        }

        public override void Update()
        {
            base.Update();
            kbLast = KyunGame.Instance.KeyboardOldState;
            lastMouse = MouseHandler.GetState();
            /*
            if (holding)
            {
                float circlepresition = 1f;
                float circle = (float)(Math.PI * 2d);
                float cp = 0;
                for (float a = 0; a < porcent;)
                {
                    a += circlepresition;
                    cp = circle * (a - circlepresition) / 100;
                }
                finalTexture = getMergedTx(cp);
            }*/
        }
        internal override void updateLogic()
        {
            if (EndTime - Time < 0)
            {
                EndTime = (long)(Time + (60000f / _beatmap.BPM));
            }

            if ((screenInstance.gameMod & GameMod.Auto) == GameMod.Auto)
            {


                if (screenInstance.GamePosition > Time && !holding)
                {
                    holding = true;
                    pressedTime = Time;
                    playHitsound();
                }

                if (holding && screenInstance.GamePosition >= EndTime)
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

            if ((screenInstance.gameMod & GameMod.Replay) == GameMod.Replay)
            {
                if (screenInstance.GamePosition > Time && !holding)
                {
                    holding = true;
                    pressedTime = replayTime;
                    playHitsound();
                }

                if (Missed && !replayhasmissed)
                {
                    replayhasmissed = true;
                    Combo.Instance.Miss();
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



            position = EndTime - screenInstance.GamePosition;

            position = (EndTime - Time) - position;

            porcent = (float)position / (float)(EndTime - Time) * 100f;


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

            if (screenInstance.GamePosition > Time && !holding)
            {

                holding = true;
            }

            if (intersecs && !pressed && screenInstance.GamePosition > Time - _beatmap.Timing50)
            {

                playHitsound();
                pressed = true;

                pressedTime = screenInstance.GamePosition;

            }

            if (screenInstance.GamePosition > EndTime)
            {
                leaveTime = EndTime;
                calculateScore();
            }

            if (pressed && holding && !intersecs && !Missed)
            {
                Missed = true;
                Combo.Instance.Miss();
            }

            if (!holding && screenInstance.GamePosition > Time + _beatmap.Timing100)
            {
                pressedTime = screenInstance.GamePosition;
                Missed = true;
                holding = true;
            }


        }

        internal override ScoreType GetScore()
        {


            if (pressedTime >= Time - _beatmap.Timing300 && pressedTime <= Time + _beatmap.Timing300 && !Missed)
            {
                //Perfect
                if (Missed)
                    return ScoreType.Excellent; //punish
                else
                    return Score.ScoreType.Perfect;
            }
            else if (pressedTime >= Time - _beatmap.Timing100 && pressedTime <= Time + _beatmap.Timing100)
            {
                //Excellent
                if (Missed)
                    return ScoreType.Good;
                else
                    return ScoreType.Excellent;
            }

            else if (pressedTime >= Time - _beatmap.Timing50 && pressedTime <= Time + _beatmap.Timing50)
            {
                if (Missed)
                    return ScoreType.Miss;
                //Bad
                return Score.ScoreType.Good;
            }
            else
            {
                return Score.ScoreType.Miss;
            }

        }


        Texture2D getMergedTx(float rotation)
        {
            return Texture2DRenderer.RenderToTexture(SpritesContent.Instance.Fill_1, SpritesContent.Instance.Fill_1, Point.Zero, Point.Zero, KyunGame.Instance.GraphicsDevice, rotation);
        }

        /*
        public override void Render()
        {
            base.Render();
            if (Died || !Visible)
                return;
            float ppeak = (KyunGame.Instance.maxPeak / 4) + 1;

            if (ppeak > 1.4f)
            {
                ppeak = 1.4f;
            }

            if (holding)
            {
                Color c = (pressed || (screenInstance.gameMod & GameMod.Auto) == GameMod.Auto || (screenInstance.gameMod & GameMod.Replay) == GameMod.Replay) ? Color.White : Color.Red;

                Vector2 vpos = new Vector2(this.Position.X + (Texture.Width / 2),
                           this.Position.Y + (Texture.Height / 2));
                Vector2 vcent = new Vector2(finalTexture.Width / 2, finalTexture.Height / 2);
                KyunGame.Instance.SpriteBatch.Draw(finalTexture,
                            vpos,
                            null,
                            c,
                            0,
                            vcent,
                            ppeak * Scale * 1.1f,
                            SpriteEffects.None, 0);
            }
        }*/

        public override void Render()
        {
            if (Died || !Visible)
                return;
            float ppeak = (KyunGame.Instance.maxPeak / 4) + 1;

            if (ppeak > 1.4f)
            {
                ppeak = 1.4f;
            }



            if (holding)
            {
                Texture2D fill = SpritesContent.Instance.Fill_1;

                Color c = (pressed || (screenInstance.gameMod & GameMod.Auto) == GameMod.Auto || (screenInstance.gameMod & GameMod.Replay) == GameMod.Replay) ? Color.White : Color.Red;

                float circlepresition = 1.07f;
                Vector2 vpos = new Vector2(this.Position.X + (Texture.Width / 2),
                            this.Position.Y + (Texture.Height / 2));
                Vector2 vcent = new Vector2(fill.Width / 2, fill.Height / 2);

                for (float a = 0; a < porcent;)
                {
                    a += circlepresition;
                    float circle = (float)(Math.PI * 2d);
                    float cp = circle * (a - circlepresition) / 100;

                    //if ((int)a % 2 != 0)
                    //if(true)
                    if (true)
                    {
                        KyunGame.Instance.SpriteBatch.Draw(fill,
                            vpos,
                            null,
                            c,
                            cp,
                            vcent,
                            ppeak * Scale * 1.1f,
                            SpriteEffects.None, 0);
                    }

                    /*
                    if (a - circlepresition <= 0)
                    {

                        KyunGame.Instance.SpriteBatch.Draw(SpritesContent.Instance.FillStartEnd,
                            new Vector2(this.Position.X + (Texture.Width / 2),
                            this.Position.Y + (Texture.Height / 2)),
                            null,
                            Color.White,
                            0,
                            new Vector2(SpritesContent.Instance.FillStartEnd.Width / 2, SpritesContent.Instance.FillStartEnd.Height / 2),
                            ppeak * Scale,
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
                            ppeak * Scale,
                            SpriteEffects.None, 0);
                    }*/
                }

                float cpeak = Math.Max(Math.Min(ppeak, .3f), 0.2f);



            }

            base.Render();

            //drawCombo();
        }


    }
}
