﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubeat.GameScreen.UI.Buttons
{
    public class ConfigButton:Button
    {
        public ConfigButton()
            : base(UbeatGame.Instance.ConfigButton)
        {
        }
    }
}
