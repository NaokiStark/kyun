using Microsoft.Xna.Framework;
using System.Collections.Generic;
using kyun.Utils;

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

            var bgcolor = new List<Color>();
            bgcolor.Add(Clr);
            var bordColor = new List<Color>();
            bordColor.Add(borderColor);

            //borderShadow = borderThickness;

            this.Texture = ContentLoader.CreateRoundedRectangleTexture(
                KyunGame.Instance.GraphicsDevice,
                (int)Size.X,
                (int)Size.Y,
                borderThickness,
                borderRadius,
                borderShadow,
                bgcolor,
                bordColor,
                initialShadowIntensity,
                finalShadowIntensity
                );
        }
    }
}
