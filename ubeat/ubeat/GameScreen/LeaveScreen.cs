using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ubeat.GameScreen
{
    public class LeaveScreen : ScreenBase
    {
        public override void Update(GameTime tm)
        {
            ///base.Update(tm);
            if (!Visible) return;
            UbeatGame.Instance.Exit();
        }

    }
}
