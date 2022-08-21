using System;
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
using kyun.GameScreen.UI;
using kyun.GameScreen.UI.Particles;

namespace kyun.GameModes.OsuMode
{
    public class HitHolder : HitSingle
    {

        public bool holding;
        private long leaveTime;
        private long position;
        private float porcent;
        private float length;

        public Vector2 SliderFollowCirclePos = Vector2.Zero;
        public List<Vector2> SliderPath;
        List<Vector2> points = new List<Vector2>();
        public bool Missed = false;

        private bool replayhasmissed = false;

        bool issuedKeypress = false;

        MouseEvent lastMouse;

        Image end;
        Image sliderTx;

        Texture2D sliderrg;
        private int repeatCount = 0;
        private int actualSliderPoint;
        private int lastSliderIndex;

        public HitHolder(IHitObj hitObject, IBeatmap beatmap, OsuMode Instance, bool shared = false)
            : base(hitObject, beatmap, Instance)
        {

            lastMouse = MouseHandler.GetState();
            Texture = SpritesContent.Instance.osu_circle;
            end = new Image(Texture);
            SliderPath = new List<Vector2>();


            generateSliderTx();

            approachObj.Texture = SpritesContent.Instance.ApproachCircle;
            // approachObj.Scale = Scale = Math.Min(Math.Max(Math.Abs(_beatmap.OverallDifficulty - 10), 1), 2) / 2;

            float PlayfieldWidth = (int)((512f / 384f) * ((float)Screen.ScreenModeManager.GetActualMode().Height * .95f));
            float CircleRadius = (PlayfieldWidth / 16f) * (1f - (0.7f * (_beatmap.CircleSize - 5f) / 5f));
            float scaledCircle = (CircleRadius * 2) / 160;

            approachObj.Scale = Scale = scaledCircle;
            Size = new Vector2(150);

            
                


            SliderFollowCirclePos = Position;
            if (_hitButton.osuHitObject is osuBMParser.HitSpinner)
            {
                Position = CalculatePosition(new Vector2(512f / 2f, 384f / 2f));
            }
        }

        void generateSliderTx()
        {
            sliderrg = SpritesContent.instance.Slider;

            if (_hitButton.osuHitObject is osuBMParser.HitSpinner)
            {
                return;
            }

            var sld = _hitButton.osuHitObject as osuBMParser.HitSlider;
            SpriteBatch sp = KyunGame.Instance.SpriteBatch;

            Vector2 cpos = Position;
            if (sld.Type == osuBMParser.HitSlider.SliderType.BREZIER || sld.Type == osuBMParser.HitSlider.SliderType.CATMULL)
            {

                points.Add(cpos);
                foreach (osuBMParser.HitSliderSegment segm in sld.HitSliderSegments)
                {
                    points.Add(CalculatePosition(new Vector2(segm.position.x, segm.position.y)));
                }

                end.Position = points.Last();
                end.Opacity = Opacity;
                end.Scale = Scale;
                if(sld.Type == osuBMParser.HitSlider.SliderType.CATMULL)
                {
                    SliderPath = MonoGame.Primitives2D.CreateCatmull(points);
                }
                else
                {
                    SliderPath = MonoGame.Primitives2D.CreateBezierLine(points);
                }
            }
            else if (sld.Type == osuBMParser.HitSlider.SliderType.LINEAR)
            {
                /*
                osuBMParser.HitSliderSegment sgm = sld.HitSliderSegments.Last();
                MonoGame.Primitives2D.DrawLine(sp, cpos, CalculatePosition(new Vector2(sgm.position.x, sgm.position.y)), Color.Green * Opacity, Size.X);*/

                points.Add(cpos);
                if (sld.HitSliderSegments.Count < 3)
                {
                    points.Add(Vector2.LerpPrecise(cpos, CalculatePosition(new Vector2(sld.HitSliderSegments.Last().position.x, sld.HitSliderSegments.Last().position.y)), .5f));
                }
                foreach (osuBMParser.HitSliderSegment segm in sld.HitSliderSegments)
                {
                    points.Add(CalculatePosition(new Vector2(segm.position.x, segm.position.y)));
                }
                SliderPath = MonoGame.Primitives2D.CreateLinear(points);
                end.Position = points.Last();
                end.Opacity = Opacity;
                end.Scale = Scale;

            }
            else if (sld.Type == osuBMParser.HitSlider.SliderType.PASSTHROUGH)
            {


                points.Add(cpos);
                foreach (osuBMParser.HitSliderSegment segm in sld.HitSliderSegments)
                {
                    points.Add(CalculatePosition(new Vector2(segm.position.x, segm.position.y)));
                }
                end.Position = points.Last();
                end.Opacity = Opacity;
                end.Scale = Scale;
                SliderPath = MonoGame.Primitives2D.CreateOsuArc(points);
            }
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


        internal override void calculateScore()
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
            Vector2 partPos = end.Position;
            if (_hitButton.osuHitObject is osuBMParser.HitSpinner)
            {
                partPos = Position;
            }
            screenInstance._particleEngine.AddNewScoreParticle(particle,
            new Vector2(.05f),
            new Vector2(partPos.X + (Texture.Height / 2) - (particle.Width / 2), partPos.Y + (Texture.Height / 2) + (particle.Height + 10) * 1.5f),
            10,
            0,
            Color.White
            ).Scale = Scale;

            Particle pr = screenInstance._particleEngine.AddNewHitObjectParticle(SpritesContent.Instance.osu_circle_top,
                       new Vector2(2),
                       new Vector2(partPos.X, partPos.Y),
                       10,
                       0,
                       Color.White
                       );

            pr.Opacity = .6f;
            pr.Scale = Scale;
            if (finalScore == ScoreType.Miss)
            {
                pr.Opacity = .8f;
                pr.MoveTo(GameScreen.AnimationEffect.Linear, 5000, new Vector2(partPos.X, partPos.Y + 50));
                pr.TextureColor = Color.Violet;
            }


            Died = true;
            Position = partPos;
        }/*
        internal override void calculateScore()
        {
            Died = true;
            base.calculateScore();
        }*/

        public override void Update()
        {
            base.Update();
            kbLast = KyunGame.Instance.KeyboardOldState;
            lastMouse = MouseHandler.GetState();

            moveSliderCircle();

            if(end != null)
            {
                end.TextureColor = ComboColor;
            }
        }

        internal void moveSliderCircle()
        {
            if (!Visible)
            {
                return;
            }
            if (_hitButton.osuHitObject is osuBMParser.HitSpinner || SliderPath.Count < 1)
            {
                return;
            }
            var sld = _hitButton.osuHitObject as osuBMParser.HitSlider;
            if (holding)
            {
                int repeat = sld.Repeat;

                decimal totalLength = (EndTime - Time) / repeat;

                decimal t = getT(screenInstance.GamePosition, Time, totalLength);

                if (repeat > 1 && repeatCount > 0 && t >= 1)
                {
                    t = Math.Abs((Decimal)MathHelper.Clamp((float)t, 0, repeat) - repeatCount);
                }

                int index = (int)Math.Floor((decimal)(SliderPath.Count) * t);
                index = MathHelper.Clamp(index, 0, SliderPath.Count - 1);
                decimal sliderStep = totalLength / (decimal)SliderPath.Count;

                decimal sliderToTotal = (sliderStep * index) + Time;

                float sliderT = (float)getT(screenInstance.GamePosition, sliderToTotal, sliderStep);
                if (index >= SliderPath.Count - 1)
                {
                    if (repeat > 1 && t > repeatCount + 1)
                    {
                        playHitsound(false);
                        SliderPath.Reverse();
                        repeatCount++;
                    }
                    SliderFollowCirclePos = SliderPath[index];
                }
                else
                {/*
                    if(lastSliderIndex == index)
                    {*/

                    SliderFollowCirclePos = getTPointAt(SliderPath[index].X, SliderPath[index].Y, SliderPath[index + 1].X, SliderPath[index + 1].Y, sliderT);
                    /*}
                    else
                    {
                        SliderFollowCirclePos = SliderPath[index];                        
                    }*/
                }
                lastSliderIndex = index;
            }
        }

        internal decimal getT(decimal time, decimal startT, decimal totalT)
        {
            return ((decimal)time - startT) / Math.Max(totalT, 0.1M);
        }

        internal Vector2 getTPointAt(float startX, float startY, float endX, float endY, float t)
        {
            float te = StringHelper.clamp(t, 0f, 1f);

            return new Vector2(startX + (endX - startX) * te, startY + (endY - startY) * te);
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



        public override void Render()
        {
            if (Died || !Visible)
                return;
            if (_hitButton.osuHitObject is osuBMParser.HitSpinner)
            {
                renderHolder();
            }
            else
            {
               
                //renderSliderV2();
                renderSlider();
                end.Opacity = Opacity;
                end.Render();
            }

            base.Render();



            //base.Render();

            //drawCombo();
        }

        private List<Vector2> centerPoints(List<Vector2> p)
        {
            var tmpp = new List<Vector2>();
            foreach(Vector2 pn in p)
            {
                tmpp.Add(pn + ((Size * Scale) / 2f) + new Vector2(11));
            }

            return tmpp;
        }

        private void renderSliderV2()
        {
            if (points.Count < 1)
            {
                return;
            }/*
            var brsh = new LilyPath.TextureBrush(sliderrg);
            brsh.Color = brsh.Color * Opacity;
            var pn = new LilyPath.Pens.PathGradientPen(ComboColor * Opacity, Color.White * Opacity);
            
            pn.Width = (Size.X * Scale) + 10;
            GraphicsPath graphicsPath = new GraphicsPath(pn, centerPoints(SliderPath));
            KyunGame.Instance.SpriteBatch.End();
            KyunGame.drawBatch.Begin(DrawSortMode.Immediate, BlendState.AlphaBlend);
            KyunGame.drawBatch.DrawPath(graphicsPath);
            KyunGame.drawBatch.End();
            KyunGame.Instance.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);
            */
        }
        private void renderSlider()
        {
            var sld = _hitButton.osuHitObject as osuBMParser.HitSlider;
            SpriteBatch sp = KyunGame.Instance.SpriteBatch;

            Vector2 cpos = Position /*+ new Vector2(Size.X / 2f)*/;
            if (SliderPath.Count < 1)
            {
                renderHolder();
                return;
            }
            if (sld.Type == osuBMParser.HitSlider.SliderType.BREZIER || sld.Type == osuBMParser.HitSlider.SliderType.CATMULL)
            {

                end.Position = SliderPath.Last();
                end.Opacity = Opacity;
                end.Scale = Scale;
                MonoGame.Primitives2D.DrawBezierv2(sp, SliderPath, ComboColor * Opacity, Size * Scale, sliderrg, 1);
                //end.Render();
            }
            else if (sld.Type == osuBMParser.HitSlider.SliderType.LINEAR)
            {

                MonoGame.Primitives2D.DrawBezierv2(sp, SliderPath, ComboColor * Opacity, Size, sliderrg, Scale);

                end.Position = SliderPath.Last();
                end.Opacity = Opacity;
                end.Scale = Scale;
                
            }
            else if (sld.Type == osuBMParser.HitSlider.SliderType.PASSTHROUGH)
            {
                end.Position = SliderPath.Last();
                end.Opacity = Opacity;
                end.Scale = Scale;
                MonoGame.Primitives2D.DrawOsuArc(sp, SliderPath, ComboColor * Opacity, Size, sliderrg, Scale);
                //end.Render();
            }
            else
            {
                renderHolder();
            }

        }


        internal void RenderPoints()
        {

        }

        internal void renderHolder()
        {
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
                Vector2 vpos = new Vector2(Position.X + (Texture.Width / 2),
                            Position.Y + (Texture.Height / 2));
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

                }

                float cpeak = Math.Max(Math.Min(ppeak, .3f), 0.2f);

            }
        }
    }
}
