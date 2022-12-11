using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using kyun.GameScreen.UI;
using kyun.Utils;
using kyun.game.GameScreen.UI;
using Microsoft.Xna.Framework.Graphics;
using static kyun.GameScreen.ScreenBase;

namespace kyun.GameScreen
{
    public class UIObjectBase : UIObjs.IUIObject, IDisposable
    {
        public bool Disposing { get; set; }

        public float Opacity { get; set; }

        public Color TextureColor { get; set; }


        public float Scale = 1;

        public float RenderScale
        {
            get
            {
                return Scale;
            }
        }
        
        public Vector2 Position { get; set; }

        public Microsoft.Xna.Framework.Graphics.Texture2D Texture { get; set; }

        public bool IsActive { get; set; }

        public bool Died { get; set; }

        public delegate void ScrollEventHandler(object sender, bool Up);

        public bool Visible { get; set; }

        public float AngleRotation { get; set; }

        public Rectangle SourceRectangle = new Rectangle();

        public Vector2 OriginRender = Vector2.Zero;

        private Vector2 size = Vector2.Zero;

        private AnimationType animationType { get; set; }
        private AnimationEffect animationEffect { get; set; }

        private int animationDuration = 400;
        private int animationElapsed = 0;

        private Vector2 toPosition = Vector2.Zero;
        private Vector2 initialPosition = Vector2.Zero;

        public bool animating = false;

        public virtual Vector2 Size
        {
            get
            {
                if (size == Vector2.Zero)
                {
                    if (Texture != null)
                        return new Vector2(Texture.Width, Texture.Height);
                    else
                        return Vector2.Zero;
                }

                return size;
            }
            set
            {
                size = value;
            }
        }

        public TimeSpan Elapsed { get; set; }

        public Screen.ScreenMode ScreenMode { get; set; }

        public Effect Effect { get; set; }
        public EffectParametersBase EffectParameters;

        public event EventHandler Click;
        public event EventHandler Over;
        public event EventHandler Leave;
        public event EventHandler MouseDown;
        public event EventHandler MouseUp;
        public event EventHandler MouseDoubleClick;
        public event ScrollEventHandler OnScroll;
        public event Utils.TouchHandler.TouchEventHandler OnTouch;

        // Animation Events

        public event EventHandler OnFadeOut;
        public event EventHandler OnFadeIn;
        public event EventHandler OnMoveEnd;

        private bool hasOver = false;

        public Tooltip Tooltip { get; set; }

        int lastScrollVal = 0;
        bool alredyPressed;
        int dClick = 2000;
        int clickC = 0;
        int clickCount = 0;
        private int lastScrollTouchValue;
        private bool alredyTouched;
        private bool mouseEventsCancelled;
        private bool scrollInvoked;
        internal KeyboardState keyboardOldState;

        public UIObjectBase()
        {
            ScreenMode = Screen.ScreenModeManager.GetActualMode();
            Elapsed = KyunGame.Instance.GameTimeP.ElapsedGameTime;
            TextureColor = Color.White;
            Visible = true;
            Opacity = 1;
            this.MouseDown += (e, ar) =>
            {
                //Console.WriteLine($"MouseDown over: {e}");
            };

            Over += (e, arg) =>
            {
                if (Tooltip != null)
                    Tooltip.Visible = true;
            };

            Leave += (e, arg) =>
            {

                if (Tooltip != null)
                    Tooltip.Visible = false;
            };
        }


        public virtual void Update()
        {
            if (!Visible)
                return;


            Elapsed = KyunGame.Instance.GameTimeP.ElapsedGameTime;

            //No Texture, no input update
            if (Texture == null)
            {
                if(this is ProgressBar)
                {
                    updateAnimation();
                }
                return;

            }

            updateAnimation();

            MouseEvent mouseState = MouseHandler.GetState();
            Rectangle rg = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)(this.Texture.Width * Scale), (int)(this.Texture.Height * Scale));
            Rectangle cursor = new Rectangle((int)mouseState.X, (int)mouseState.Y, 1, 1);
            Rectangle tcursor = new Rectangle((int)KyunGame.Instance.touchHandler.LastPosition.X, (int)KyunGame.Instance.touchHandler.LastPosition.Y, 1, 1);

            UpdateTouchEvents(rg);


            if (!KyunGame.Instance.isMainWindowActive) return;

            if (Tooltip != null)
                Tooltip?.Update();

            EffectParameters?.Update();

            //if (!KyunGame.Instance.IsActive) return; //Fix events

            if (this is Listbox || this is ListboxDiff || this is ObjectListbox)
            {
                //int actualScrollTouchValue = 0;

                int actualScrollVal = mouseState.ScrollWheelValue;
                TouchHandler touch = KyunGame.Instance.touchHandler;
                List<TouchPoint> touchPoints = touch.GetAllPointsIntersecs(rg);

                if (actualScrollVal > lastScrollVal)
                {
                    if (cursor.Intersects(rg))
                    {
                        OnScroll?.Invoke(this, true);
                    }

                }
                else if (actualScrollVal < lastScrollVal)
                {
                    if (cursor.Intersects(rg))
                    {

                        OnScroll?.Invoke(this, false);

                    }
                }
                else if (touchPoints.Count > 0)
                {


                    TouchPoint first = touchPoints[0];
                    if (!alredyTouched)
                    {
                        lastScrollTouchValue = (int)first.Location.Y;
                        alredyTouched = true;
                        scrollInvoked = false;
                    }
                    else
                    {
                        if ((int)first.Location.Y > lastScrollTouchValue + 50)
                        {
                            lastScrollTouchValue = (int)first.Location.Y;
                            mouseEventsCancelled = true;
                            OnScroll?.Invoke(this, true);
                            scrollInvoked = true;

                        }
                        else if ((int)first.Location.Y < lastScrollTouchValue - 50)
                        {
                            lastScrollTouchValue = (int)first.Location.Y;
                            mouseEventsCancelled = true;
                            OnScroll?.Invoke(this, false);
                            scrollInvoked = true;
                        }

                    }

                }
                else if (touchPoints.Count < 1)
                {

                    alredyTouched = false;
                }

                if (mouseEventsCancelled && scrollInvoked)
                {
                    return;
                }

                lastScrollVal = actualScrollVal;

            }


            clickC += (int)KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds;
            if (clickC > dClick)
            {
                clickC = clickCount = 0;
            }

            if (cursor.Intersects(rg) || tcursor.Intersects(rg))
            {
                hasOver = true;
                Over?.Invoke(this, new EventArgs());

                if (mouseState.LeftButton == ButtonState.Pressed || KyunGame.Instance.touchHandler.TouchDown)
                {


                    if (!alredyPressed)
                    {
                        //Logger.Instance.Debug("MouseDown");
                        //Launch MouseDown
                        MouseDown?.Invoke(this, new EventArgs());


                        alredyPressed = true;
                    }
                }
            }
            else
            {
                if (hasOver)
                {
                    hasOver = false;
                    Leave?.Invoke(this, new EventArgs());
                }
            }

            if (mouseState.LeftButton == ButtonState.Released)
            {
                //Fix Wine bug
                if (KyunGame.Instance.touchHandler.TouchUp || KyunGame.RunningOverWine)
                {
                    if (alredyPressed)
                    {
                        if (clickCount < 3)
                        {
                            clickCount++;
                            clickC = 0;
                        }

                        //Launch MouseUp
                        if (MouseUp != null)
                            if (cursor.Intersects(rg) || tcursor.Intersects(rg)) //again
                                MouseUp(this, new EventArgs());

                        //MouseClick

                        if (Click != null)
                            if (cursor.Intersects(rg)) //again
                                Click(this, new EventArgs());

                        if (clickCount > 1 && clickC > 10)
                        {
                            MouseDoubleClick?.Invoke(this, new EventArgs());
                            clickCount = clickC = 0;
                        }

                        alredyPressed = false;
                    }
                }
            }
        }

        internal void UpdateTouchEvents(Rectangle rg)
        {
            if (KyunGame.Instance.touchHandler.TouchIntersecs(rg))
            {
                OnTouch?.Invoke(this, new Utils.ubeatTouchEventArgs
                {
                    Point = KyunGame.Instance.touchHandler.GetTouchIntersecs(rg)
                });
            }
        }

        public virtual void Render()
        {
            if (!Visible)
                return;

            if (Texture == null)
                return;

            if (EffectParameters != null && KyunGame.Instance.Graphics.GraphicsProfile == GraphicsProfile.HiDef)
            {
                KyunGame.Instance.SpriteBatch.End();
                KyunGame.Instance.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

                foreach (KeyValuePair<string, dynamic> parameter in EffectParameters.Parameters)
                {

                    EffectParameters.Effect.Parameters[parameter.Key].SetValue(parameter.Value);

                }

                EffectParameters.Effect.CurrentTechnique = EffectParameters.Effect.Techniques[0];

                foreach (EffectPass pass in EffectParameters.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }
            }

            Rectangle rg = new Rectangle((int)(Position.X), (int)(Position.Y), (int)(Size.X * RenderScale), (int)(Size.Y * RenderScale));

            rg.X = (int)(rg.X - ((Size.X * RenderScale) - Size.X) / 2);
            rg.Y = (int)(rg.Y - ((Size.Y * RenderScale) - Size.Y) / 2);

            SourceRectangle = new Rectangle(SourceRectangle.X, SourceRectangle.Y, (int)(SourceRectangle.Width * RenderScale), (int)(SourceRectangle.Height * RenderScale));

            if (SourceRectangle != Rectangle.Empty)
                KyunGame.Instance.SpriteBatch.Draw(Texture, rg, SourceRectangle, TextureColor * Opacity, AngleRotation, OriginRender, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            else
                KyunGame.Instance.SpriteBatch.Draw(Texture, rg, null, TextureColor * Opacity, AngleRotation, OriginRender, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);

            if (Effect != null && KyunGame.Instance.Graphics.GraphicsProfile == GraphicsProfile.HiDef)
            {
                KyunGame.Instance.SpriteBatch.End();
                KyunGame.Instance.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
            }

            //Render over object
            if (Tooltip != null)
            {
                KyunGame.Instance.tooltips.Add(Tooltip);
            }

        }

        internal void updateAnimation()
        {
            if (animationType == AnimationType.None)
            {
                animationElapsed = 0;
                return;
            }

            animationElapsed += KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;

            switch (animationEffect)
            {
                case AnimationEffect.Linear:
                    updateLinear();
                    break;
                case AnimationEffect.Ease:
                    updateEase();
                    break;
                case AnimationEffect.bounceIn:
                case AnimationEffect.bounceOut:
                    updateBounce();
                    break;
                default: //??
                    updateLinear();
                    break;
            }
        }

        internal void updateLinear()
        {
            switch (animationType)
            {
                case AnimationType.FadeIn:
                    Opacity = linearIn(animationElapsed, animationDuration);
                    if (Opacity >= 1f || animationElapsed > animationDuration)
                    {
                        Opacity = 1f;
                        animationType = AnimationType.None;
                        OnFadeIn?.Invoke(this, new EventArgs());
                    }
                    break;
                case AnimationType.FadeOut:
                    Opacity = Math.Max(linearOut(animationElapsed, animationDuration), 0f);
                    if (Opacity <= 0f || animationElapsed > animationDuration)
                    {
                        Opacity = 0;
                        animationType = AnimationType.None;
                        OnFadeOut?.Invoke(this, new EventArgs());
                    }
                    break;
                case AnimationType.MoveTo:
                    float initialPosX = initialPosition.X;
                    float initialPosY = initialPosition.Y;
                    float destPosX = toPosition.X;
                    float destPosY = toPosition.Y;

                    float scalePosX = (initialPosX > destPosX) ? linearIn(animationElapsed, animationDuration) : linearIn(animationElapsed, animationDuration);
                    float scalePosY = (initialPosY > destPosY) ? linearIn(animationElapsed, animationDuration) : linearIn(animationElapsed, animationDuration);

                    scalePosX = scalePosY = linearIn(animationElapsed, animationDuration);

                    float actualX = (initialPosX - destPosX) * scalePosX;
                    float actualY = (initialPosY - destPosY) * scalePosY;

                    float stepY = Math.Abs(initialPosY - destPosY) / 100f;
                    float stepX = Math.Abs(initialPosX - destPosX) / 100f;


                    if (initialPosX < destPosX)
                    {
                        actualX = initialPosX + stepX * (scalePosX * 100);
                    }
                    else if (initialPosX == destPosX)
                    {
                        actualX = initialPosX;
                    }
                    else
                    {
                        actualX = initialPosX - stepX * (scalePosX * 100);
                    }

                    if (initialPosY < destPosY)
                    {
                        //actualY = Math.Abs((destPosY - initialPosY) * scalePosY);
                        actualY = initialPosY + stepY * (scalePosY * 100);
                    }
                    else if (initialPosY == destPosY)
                    {
                        actualY = initialPosY;
                    }
                    else
                    {
                        //actualY = Math.Abs((destPosY - initialPosY) * scalePosY);
                        actualY = initialPosY - stepY * (scalePosY * 100);
                    }

                    Position = new Vector2(actualX, actualY);
                    if (animationElapsed >= animationDuration)
                    {
                        Position = toPosition;
                        animating = false;
                        animationType = AnimationType.None;
                        OnMoveEnd?.Invoke(this, new EventArgs());
                    }
                    break;
            }
        }

        internal void updateEase()
        {
            switch (animationType)
            {
                case AnimationType.FadeIn:
                    Opacity = bezierBlend(linearIn(animationElapsed, animationDuration));
                    if (Opacity == 1f)
                    {
                        animationType = AnimationType.None;
                        OnFadeIn?.Invoke(this, new EventArgs());
                    }
                    break;
                case AnimationType.FadeOut:
                    Opacity = Math.Max(bezierBlend(linearOut(animationElapsed, animationDuration)), 0f);
                    if (Opacity <= 0f || animationElapsed > animationDuration)
                    {
                        animationType = AnimationType.None;
                        OnFadeOut?.Invoke(this, new EventArgs());
                    }
                    break;
                case AnimationType.MoveTo:
                    float initialPosX = initialPosition.X;
                    float initialPosY = initialPosition.Y;
                    float destPosX = toPosition.X;
                    float destPosY = toPosition.Y;

                    float scalePosX = bezierBlend((initialPosX > destPosX) ? linearOut(animationElapsed, animationDuration) : linearIn(animationElapsed, animationDuration));
                    float scalePosY = bezierBlend((initialPosY > destPosY) ? linearOut(animationElapsed, animationDuration) : linearIn(animationElapsed, animationDuration));

                    float actualX = (initialPosX - destPosX) * scalePosX;
                    float actualY = (initialPosY - destPosY) * scalePosY;

                    /* NEW */

                    scalePosX = scalePosY = bezierBlend(linearIn(animationElapsed, animationDuration));

                    
                    float stepY = Math.Abs(initialPosY - destPosY) / 100f;
                    float stepX = Math.Abs(initialPosX - destPosX) / 100f;


                    if (initialPosX < destPosX)
                    {
                        actualX = initialPosX + stepX * (scalePosX * 100);
                    }
                    else if (initialPosX == destPosX)
                    {
                        actualX = initialPosX;
                    }
                    else
                    {
                        actualX = initialPosX - stepX * (scalePosX * 100);
                    }

                    if (initialPosY < destPosY)
                    {
                        //actualY = Math.Abs((destPosY - initialPosY) * scalePosY);
                        actualY = initialPosY + stepY * (scalePosY * 100);
                    }
                    else if (initialPosY == destPosY)
                    {
                        actualY = initialPosY;
                    }
                    else
                    {
                        //actualY = Math.Abs((destPosY - initialPosY) * scalePosY);
                        actualY = initialPosY - stepY * (scalePosY * 100);
                    }


                    Position = new Vector2(actualX, actualY);
                    if (animationElapsed >= animationDuration)
                    {
                        animating = false;
                        animationType = AnimationType.None;
                        OnMoveEnd?.Invoke(this, new EventArgs());
                    }
                    break;
            }
        }

        internal void updateBounce()
        {
            switch (animationType)
            {
                case AnimationType.FadeIn:
                    float tIn = linearIn(animationElapsed, animationDuration);
                    Opacity = (animationEffect == AnimationEffect.bounceIn) ? bounceIn(tIn) : bounceOut(tIn);

                    if (Opacity == 1f)
                    {
                        animationType = AnimationType.None;
                        OnFadeIn?.Invoke(this, new EventArgs());
                    }
                    break;
                case AnimationType.FadeOut:
                    float tOut = Math.Max(linearOut(animationElapsed, animationDuration), 0f);
                    Opacity = (animationEffect == AnimationEffect.bounceIn) ? bounceIn(tOut) : bounceOut(tOut);

                    if (Opacity <= 0f || animationElapsed > animationDuration)
                    {
                        animationType = AnimationType.None;
                        OnFadeOut?.Invoke(this, new EventArgs());
                    }
                    break;
                case AnimationType.MoveTo:
                    float initialPosX = initialPosition.X;
                    float initialPosY = initialPosition.Y;
                    float destPosX = toPosition.X;
                    float destPosY = toPosition.Y;

                    float mTIn = linearIn(animationElapsed, animationDuration);
                    float mTOut = linearOut(animationElapsed, animationDuration);

                    float scalePosX = (animationEffect == AnimationEffect.bounceIn) ? bounceIn((initialPosX < destPosX) ? mTOut : mTIn) : bounceOut((initialPosX > destPosX) ? mTOut : mTIn);
                    float scalePosY = (animationEffect == AnimationEffect.bounceIn) ? bounceIn((initialPosY < destPosY) ? mTOut : mTIn) : bounceOut((initialPosY > destPosY) ? mTOut : mTIn);

                    float actualX = (initialPosX - destPosX) * scalePosX;
                    float actualY = (initialPosY - destPosY) * scalePosY;

                    if (initialPosX < destPosX)
                    {
                        actualX = (destPosX - initialPosX) * scalePosX;
                    }
                    if (initialPosition.X == destPosX)
                    {
                        actualX = initialPosition.X;
                    }


                    if (initialPosition.Y < destPosY)
                    {
                        actualY = (destPosY - initialPosY) * scalePosY;
                    }

                    if (initialPosition.Y == destPosY)
                    {
                        actualY = initialPosition.Y;
                    }



                    Position = new Vector2(actualX, actualY);
                    if (animationElapsed >= animationDuration)
                    {
                        animating = false;
                        animationType = AnimationType.None;
                        OnMoveEnd?.Invoke(this, new EventArgs());
                    }
                    break;
            }
        }

        public void FadeIn(AnimationEffect effect, int duration)
        {
            animationEffect = effect;
            animationDuration = duration;
            animationType = AnimationType.FadeIn;

        }

        public void FadeIn(AnimationEffect effect, int duration, Action complete)
        {
            OnFadeIn += (e, args) =>
            {
                complete();

                if(OnFadeIn != null)
                {
                    Delegate[] c = OnFadeIn.GetInvocationList();
                    if(c != null)
                    {
                        foreach (Delegate d in c)
                        {
                            OnFadeIn -= (EventHandler)d;
                        }
                    }
                }
                
            };

            FadeIn(effect, duration);
        }

        public void FadeOut(AnimationEffect effect, int duration)
        {
            animationType = AnimationType.FadeOut;
            animationEffect = effect;
            animationDuration = duration;


        }

        public void FadeOut(AnimationEffect effect, int duration, Action complete)
        {
            OnFadeOut += (e, args) =>
            {
                complete();

                if(OnFadeOut != null)
                {
                    Delegate[] c = OnFadeOut.GetInvocationList();
                    if(c != null)
                    {
                        foreach (Delegate d in c)
                        {
                            OnFadeOut -= (EventHandler)d;
                        }
                    }
                }                
            };



            FadeOut(effect, duration);

        }

        public void MoveTo(AnimationEffect effect, int duration, Vector2 to)
        {
            if (animating)
            {
                Position = toPosition; //Skips                
            }

            animating = true;
            animationEffect = effect;
            toPosition = to;
            initialPosition = Position;
            animationDuration = duration;
            animationType = AnimationType.MoveTo;
        }

        public void MoveTo(AnimationEffect effect, int duration, Vector2 to, Action complete)
        {
            OnMoveEnd += (e, args) =>
            {
                complete();
                Delegate[] invListArr = OnMoveEnd?.GetInvocationList();
                if(invListArr != null)
                {
                    if(invListArr.Length > 0)
                    {
                        foreach (Delegate d in invListArr)
                        {
                            OnMoveEnd -= (EventHandler)d;
                        }                        
                    }
                }
            };

            MoveTo(effect, duration, to);
        }

        internal float linearIn(float time, float duration)
        {
            return Math.Max(Math.Min(time / (duration / 100f) / 100f, 1f), 0.1f);
        }

        internal float linearOut(int time, int duration)
        {
            return Math.Max((duration - time) / (duration / 100f) / 100f, 0f);
        }

        internal float bezierBlend(float t)
        {
            return (float)Math.Pow(t, 2f) * (3.0f - 2.0f * t);
        }

        internal float bounceIn(float t)
        {
            return 1 - bounceOut(1 - t);
        }

        internal float bounceOut(float t)
        {
            return (t = +t) < b1 ? b0 * t * t : t < b3 ? b0 * (t -= b2) * t + b4 : t < b6 ? b0 * (t -= b5) * t + b7 : b0 * (t -= b8) * t + b9;
        }

        float b1 = 4f / 11f,
            b2 = 6f / 11f,
            b3 = 8f / 11f,
            b4 = 3f / 4f,
            b5 = 9 / 11f,
            b6 = 10f / 11f,
            b7 = 15f / 16f,
            b8 = 21f / 22f,
            b9 = 63f / 64f;

        float b0 = 1f / (4f / 11f) / (4f / 11f);


        internal Vector2 calculatePosition(Vector2 position, float scale)
        {
            return position * scale;
        }

        public void _OnClick()
        {
            Click?.Invoke(this, new EventArgs());
        }

        public virtual void Dispose()
        {
            Disposing = true;
        }


    }

    public enum AnimationType
    {
        None,
        FadeIn,
        FadeOut,
        MoveTo,
    }

    public enum AnimationEffect
    {
        Linear,
        Ease,
        bounceIn,
        bounceOut
    }
}
