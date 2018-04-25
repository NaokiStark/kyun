using Microsoft.Xna.Framework;
using System.Collections.Generic;
using kyun.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace kyun.GameScreen.UI {

    public class RoundedRectangle : FilledRectangle
    {
        public RoundedRectangle(
            Vector2 Size,
            Color Clr,
            int borderThickness,
            int borderRadius,
            Color borderColor,
            int borderShadow = 1,
            float initialShadowIntensity = 1,
            float finalShadowIntensity = 1
            ) : base(Size, Clr)
        {
            Texture = SpritesContent.RoundCorners(Texture, borderRadius);
        }
    }
}
