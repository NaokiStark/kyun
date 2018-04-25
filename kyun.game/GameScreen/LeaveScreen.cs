using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using kyun.GameScreen.UI;
using kyun.Utils;

namespace kyun.GameScreen
{
    public class LeaveScreen : ScreenBase
    {
        int toLeave = 2*1000;
        int count = 0;

        bool vol;
        private Label titleLabel;

        public LeaveScreen()
        {
            rPeak = false;
            titleLabel = new Label(0)
            {
                Text = "Bye!",
                Centered = true,
                Position = new Vector2(ActualScreenMode.Width / 2, (ActualScreenMode.Height / 2) - 5 - (SpritesContent.Instance.ScoreBig.MeasureString(":3").Y / 2) ),
                Font = SpritesContent.Instance.ScoreBig
            };

            Controls.Add(titleLabel);
        }
    

        public override void Update(GameTime tm)
        {

            base.Update(tm);

            KyunGame.Instance.IsMouseVisible = false;

            count += KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;
            
            if (!vol)
            {
                vol = true;
                KyunGame.Instance.Player.Volume = KyunGame.Instance.GeneralVolume / 2;
                ((System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(KyunGame.Instance.Window.Handle)).FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            }
            
            ///base.Update(tm);
            if (!Visible) return;

            if(count > toLeave)
                KyunGame.Instance.Exit();
        }

    }
}
