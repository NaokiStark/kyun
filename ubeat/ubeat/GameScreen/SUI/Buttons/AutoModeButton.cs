using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ubeat.GameScreen.UI;

namespace ubeat.GameScreen.SUI.Buttons
{
    public class AutoModeButton : Button
    {
        public AutoModeButton()
            : base(Game1.Instance.AutoModeButton)
        {
        }
    }
}
