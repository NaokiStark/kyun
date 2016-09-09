using Microsoft.Xna.Framework;

namespace ubeat.GameScreen
{
    public class ScreenManager
    {
        public static void ChangeTo(IScreen ToScreen)
        {
            ToScreen.Visible = true;

            if (ActualScreen != null)
                ActualScreen.Visible = false;

            ActualScreen = ToScreen;
        }

        public static void Update(GameTime gm)
        {
            if (ActualScreen == null)
                return;

            ActualScreen.Update(gm);
        }

        public static void Render()
        {
            if (ActualScreen == null)
                return;

            ActualScreen.Render();
        }

        public static IScreen ActualScreen { get; set; }
    }
}
