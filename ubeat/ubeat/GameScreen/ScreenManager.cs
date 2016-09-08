using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubeat.GameScreen
{
    public class ScreenManager
    {
        public static IScreen ActualScreen{get;set;}

        public static void ChangeTo(IScreen ToScreen)
        {
            ToScreen.Visible = true;
            if (ActualScreen != null)
            {
                ActualScreen.Visible = false;                
            }
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
    }
}
