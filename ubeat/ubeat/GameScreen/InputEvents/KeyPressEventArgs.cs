using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubeat.GameScreen.InputEvents
{
    public class KeyPressEventArgs : EventArgs
    {
        public Keys Key { get; set; }

    }
}
