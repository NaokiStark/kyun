using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace ubeat.GameScreen
{
    public class ScreenManager
    {
        public static Texture2D TopEffect = null;
        static float Opacity = 0;
        static bool showing = false;
        static IScreen ToBeChanged = null;
        static bool Changed = false;

        public static void ChangeTo(IScreen ToScreen)
        {
            showing = true;
            ToBeChanged = ToScreen;           
            
        }

        public static void Start()
        {
            Screen.ScreenMode actmode = Screen.ScreenModeManager.GetActualMode();

            TopEffect = new Texture2D(UbeatGame.Instance.GraphicsDevice, actmode.Width, actmode.Height);

            Color[] txClr = new Color[actmode.Width * actmode.Height];
            for (int a = 0; a < txClr.Length; a++)
            {
                txClr[a] = Color.Black;
            }
            TopEffect.SetData(txClr);

        }

        public static void Update(GameTime gm)
        {
            if (((ScreenBase)ActualScreen)?.isDisposing == true) return;


            ActualScreen?.Update(gm);

            if (showing && ToBeChanged != null)
            {
                
                if (Opacity < 1f)
                {
                    
                    Opacity += (float)(UbeatGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.003f);

                }
                else
                {
                    if(ActualScreen != null) ActualScreen.Visible = false;
                    ActualScreen = ToBeChanged;

                    showing = false;
                    ActualScreen.Visible = true;
                    
                }
            }
            else
            {
                if (Opacity > 0f)
                {
                    Opacity -= (float)(UbeatGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.003f);
                }
            }
            
        }

        public static void Render()
        {
            if (((ScreenBase)ActualScreen)?.isDisposing == true) return;


            ActualScreen?.Render();

            if(Opacity > 0.00001)
            {
                UbeatGame.Instance.SpriteBatch.Draw(TopEffect, new Rectangle(0, 0, TopEffect.Width, TopEffect.Height), Color.White * Opacity);
            }

        }

        public static IScreen ActualScreen { get; set; }
    }
}
