using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubeat.GameScreen.UI.Buttons
{
    public class StartButton:Button
    {
        public StartButton()
            : base(UbeatGame.Instance.StartButton)
        {
        }

    }
}
