using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using kyun.Utils;
using Microsoft.Xna.Framework;

namespace kyun.GameScreen.UI.Buttons
{
    public class SquareButton : ButtonStandard
    {
        public SquareButton(Color color) : base(color)
        {
            Texture = SpritesContent.Instance.SquareButton;
            Font = SpritesContent.Instance.GeneralBig;
        }
    }
}
