using kyun.Audio;
using kyun.Overlay;
using kyun.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;

namespace kyun.GameScreen
{
    public class ScreenManager
    {
        public static Texture2D TopEffect = null;
        static float Opacity = 0;
        static bool showing = false;
        public static IScreen ToBeChanged = null;
        static bool Changed = false;

        static bool fadingIn = false;

        public static IOverlay Overlay = null;

        public static void ChangeTo(IScreen ToScreen)
        {

            if (!(ToScreen == null))
                if (!(ToScreen is LogoScreen))
                    if (!(ToScreen is LoadScreen))
                        if (!(ToScreen is GameModes.Classic.ScorePanel))
                            EffectsPlayer.PlayEffect(SpritesContent.Instance.MenuTransition);
            showing = true;
            ToBeChanged = ToScreen;

        }

        public static Texture2D ActualBackground
        {
            get
            {
                if (ActualScreen == null)
                    if (ToBeChanged != null)
                        return ((ScreenBase)ToBeChanged).Background;
                    else
                        return null;

                return ((ScreenBase)ActualScreen).Background;
            }
        }

        public static void ShowOverlay(OverlayScreen _overlay)
        {

            _overlay.Opacity = 0;

            Overlay = _overlay;

            _overlay.Visible = true;

            _overlay.FadeIn(AnimationEffect.Linear, 500);
        }

        public static void RemoveOverlay()
        {
            ((OverlayScreen)Overlay).FadeOut(AnimationEffect.Linear, 100, () =>
            {
                ((OverlayScreen)Overlay).Opacity = 1;
                Overlay = null;
            });
        }

        public static void Start()
        {
            Screen.ScreenMode actmode = Screen.ScreenModeManager.GetActualMode();

            List<Screen.ScreenMode> scrmds = Screen.ScreenModeManager.GetSupportedModes();
            Screen.ScreenMode highestMode = scrmds[scrmds.Count - 1];

            int screenw = Math.Min(highestMode.ScaledWidth, 2048);
            int screenh = Math.Min(highestMode.ScaledHeight, 2048);

            TopEffect = new Texture2D(KyunGame.Instance.GraphicsDevice, screenw, screenh);

            Color[] txClr = new Color[screenw * screenh];
            for (int a = 0; a < txClr.Length; a++)
            {
                txClr[a] = Color.Black;
            }
            TopEffect.SetData(txClr);

        }

        public static void Update(GameTime gm)
        {
            if (((ScreenBase)ActualScreen)?.isDisposing == true)
            {
                if (ActualScreen is SettingsScreen)
                    ActualScreen = SettingsScreen.Instance;
            }

            if (Overlay == null)
            {
                ActualScreen?.Update(gm);
            }
            else
            {
                ((OverlayScreen)Overlay).Update(gm);
            }


            if (showing && ToBeChanged != null)
            {


                if (KyunGame.Instance.Player.Volume > (float)(KyunGame.Instance.GeneralVolume / 2))
                {
                    KyunGame.Instance.Player.Volume -= (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.003f);
                }

                if (Opacity < 1f)
                {

                    Opacity = Math.Min(Opacity + (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.003f), 1f);

                }
                else
                {
                    if (ActualScreen != null) ActualScreen.Visible = false;
                    ActualScreen = ToBeChanged;
                    fadingIn = true;
                    showing = false;
                    ActualScreen.Visible = true;


                }
            }
            else
            {

                if (Opacity >= 0f)
                {
                    Opacity = Math.Max(Opacity - (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.003f), 0f);
                }
                else
                {
                    fadingIn = false;
                }


                if (KyunGame.Instance.Player.Volume < KyunGame.Instance.GeneralVolume)
                {
                    float vol = KyunGame.Instance.Player.Volume + (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.003f);
                    if (vol > KyunGame.Instance.GeneralVolume)
                    {
                        KyunGame.Instance.Player.Volume = KyunGame.Instance.GeneralVolume;
                    }
                    else
                    {
                        KyunGame.Instance.Player.Volume = vol;
                    }


                }

            }

        }

        public static void Render()
        {
            if (((ScreenBase)ActualScreen)?.isDisposing == true) return;


            ActualScreen?.Render();

            ((OverlayScreen)Overlay)?.Render();

            if (Opacity > 0.00001)
            {
                var ActualScreenMode = Screen.ScreenModeManager.GetActualMode();
                float screenWidth = ActualScreenMode.Width;
                float screenHeight = ActualScreenMode.Height;

                var screenRectangle = new Rectangle((int)screenWidth / 2, (int)screenHeight / 2, (int)(((float)TopEffect.Width / (float)TopEffect.Height) * (float)screenHeight), (int)screenHeight);

                if (screenRectangle.Width < screenWidth)
                {
                    screenRectangle = new Rectangle(screenRectangle.X, screenRectangle.Y, (int)screenWidth, (int)(((float)TopEffect.Height / (float)TopEffect.Width) * (float)screenWidth));
                }
                KyunGame.Instance.SpriteBatch.Draw(TopEffect, screenRectangle, null, Color.White * Opacity, 0, new Vector2(TopEffect.Width / 2, TopEffect.Height / 2), SpriteEffects.None, 0);

                //KyunGame.Instance.SpriteBatch.Draw(TopEffect, new Rectangle(0, 0, TopEffect.Width, TopEffect.Height), Color.White * Opacity);
            }

        }

        public static IScreen ActualScreen { get; set; }
    }
}
