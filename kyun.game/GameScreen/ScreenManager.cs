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
        static IScreen ToBeChanged = null;
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

        public static void ShowOverlay(IOverlay _overlay)
        {
            Overlay = _overlay;
        }

        public static void RemoveOverlay()
        {
            Overlay = null;
        }

        public static void Start()
        {
            Screen.ScreenMode actmode = Screen.ScreenModeManager.GetActualMode();

            List<Screen.ScreenMode> scrmds = Screen.ScreenModeManager.GetSupportedModes();
            Screen.ScreenMode higestMode = scrmds[scrmds.Count - 1];
           

            TopEffect = new Texture2D(KyunGame.Instance.GraphicsDevice, higestMode.Width, higestMode.Height);

            Color[] txClr = new Color[higestMode.Width * higestMode.Height];
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

            if(Overlay == null)
            {
                ActualScreen?.Update(gm);
            }
            else
            {
                ((OverlayScreen)Overlay).Update(gm);
            }


            if (showing && ToBeChanged != null)
            {


                if (KyunGame.Instance.Player.Volume > (float)(KyunGame.Instance.GeneralVolume/2))
                {
                    KyunGame.Instance.Player.Volume -= (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.003f);
                }

                if (Opacity < 1f)
                {
                    
                    Opacity += (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.003f);

                }
                else
                {
                    if(ActualScreen != null) ActualScreen.Visible = false;
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
                    Opacity -= (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.003f);
                }
                else
                {
                    fadingIn = false;        
                }
                

                if (KyunGame.Instance.Player.Volume < KyunGame.Instance.GeneralVolume)
                {
                    float vol = KyunGame.Instance.Player.Volume + (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.003f);
                    if(vol > KyunGame.Instance.GeneralVolume)
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
                KyunGame.Instance.SpriteBatch.Draw(TopEffect, new Rectangle(0, 0, TopEffect.Width, TopEffect.Height), Color.White * Opacity);
            }

        }

        public static IScreen ActualScreen { get; set; }
    }
}
