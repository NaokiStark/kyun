using kyun.GameScreen;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.GameScreen
{
    public class AnimatedScreenBase : ScreenBase
    {

        private AnimationType animationType { get; set; }
        private AnimationEffect animationEffect { get; set; }

        private int animationDuration = 400;
        private int animationElapsed = 0;


        // Animation Events

        public event EventHandler OnFadeOut;
        public event EventHandler OnFadeIn;
        public event EventHandler OnMoveEnd;


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

                foreach (Delegate d in OnFadeIn.GetInvocationList())
                {
                    OnFadeIn -= (EventHandler)d;
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

                foreach (Delegate d in OnFadeOut.GetInvocationList())
                {
                    OnFadeOut -= (EventHandler)d;
                }
            };



            FadeOut(effect, duration);

        }

        internal float linearIn(float time, float duration)
        {
            return Math.Min(time / (duration / 100f) / 100f, 1f);
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
            return (t = +t) < b1 ? b0 * t * t : t < b3 ? b0 * (t -= b2) * t +
                b4 : t < b6 ? b0 * (t -= b5) * t + b7 : b0 * (t -= b8) * t + b9;
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

        public override void Update(GameTime tm)
        {

            base.Update(tm);
            updateAnimation();

        }

        private void updateAnimation()
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
                    if (Opacity == 1f)
                    {
                        animationType = AnimationType.None;
                        OnFadeIn?.Invoke(this, new EventArgs());
                    }
                    break;
                case AnimationType.FadeOut:
                    Opacity = Math.Max(linearOut(animationElapsed, animationDuration), 0f);
                    if (Opacity <= 0f || animationElapsed > animationDuration)
                    {
                        animationType = AnimationType.None;
                        OnFadeOut?.Invoke(this, new EventArgs());
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
                    float lnrOut = linearOut(animationElapsed, animationDuration);
                    float bBlnd = bezierBlend(lnrOut);
                    Opacity = Math.Max(bBlnd, 0f);
                    if (Opacity <= 0f || animationElapsed > animationDuration)
                    {
                        animationType = AnimationType.None;
                        OnFadeOut?.Invoke(this, new EventArgs());
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
            }
        }
    }
}
