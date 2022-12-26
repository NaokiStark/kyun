using kyun.GameScreen;
using kyun.GameScreen.UI;
using kyun.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kyun.game.GameScreen.UI
{
    public class Cursor : Image
    {
        private int countToHide;

        MouseEvent lastState;

        Vector2 OriginalSize = Vector2.Zero;        
        
        public Cursor(Texture2D texture) : base(texture)
        {
            BeatReact = false;
            lastState = MouseHandler.GetState();
            
        }

        public override void Update()
        {
            base.Update();

            MouseEvent mouseState = MouseHandler.GetState();


            if (countToHide < 7000)
            {
                countToHide += KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;
            }


            if (countToHide >= 7000)
            {
                Opacity = Math.Max(Opacity - KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * .0005f, 0f);

            }
            
            if (mouseState.Position.Equals(lastState.Position))
            {
                return;
            }
            countToHide = 0;
            Opacity = 1f;
            lastState = mouseState;
        }
    }
}
