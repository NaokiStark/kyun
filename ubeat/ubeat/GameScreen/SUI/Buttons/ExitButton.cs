using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubeat.GameScreen.UI.Buttons
{
    public class ExitButton:Button
    {
        public ExitButton()
            : base(UbeatGame.Instance.ExitButton)
        {
        }
    }
}
