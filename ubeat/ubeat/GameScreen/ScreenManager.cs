using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public static void ChangeTo(IScreen ToScreen)
        {
            showing = true;
            ToBeChanged = ToScreen;           
            
        }

        public static void Start()
        {
            Screen.ScreenMode actmode = Screen.ScreenModeManager.GetActualMode();

            TopEffect = new Texture2D(KyunGame.Instance.GraphicsDevice, actmode.Width, actmode.Height);

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

                    showing = false;
                    ActualScreen.Visible = true;
                    
                }
            }
            else
            {
                if (Opacity > 0f)
                {
                    Opacity -= (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.003f);
                }


                if (KyunGame.Instance.Player.Volume < KyunGame.Instance.GeneralVolume)
                {
                    KyunGame.Instance.Player.Volume += (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.003f);
                }

            }
            
        }

        public static void Render()
        {
            if (((ScreenBase)ActualScreen)?.isDisposing == true) return;


            ActualScreen?.Render();

            if(Opacity > 0.00001)
            {
                KyunGame.Instance.SpriteBatch.Draw(TopEffect, new Rectangle(0, 0, TopEffect.Width, TopEffect.Height), Color.White * Opacity);
            }

        }

        public static IScreen ActualScreen { get; set; }
    }
}
