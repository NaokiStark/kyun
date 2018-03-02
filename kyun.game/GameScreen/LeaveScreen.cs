using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace kyun.GameScreen
{
    public class LeaveScreen : ScreenBase
    {
        int toLeave = 1*1000;
        int count = 0;

        bool vol;
    

        public override void Update(GameTime tm)
        {
            count += KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;
            if (!vol)
            {
                vol = true;
                KyunGame.Instance.GeneralVolume = KyunGame.Instance.GeneralVolume / 2;
                ((System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(KyunGame.Instance.Window.Handle)).FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            }
            
            ///base.Update(tm);
            if (!Visible) return;

            if(count > toLeave)
                KyunGame.Instance.Exit();
        }

    }
}
