using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kyun.GameScreen.UI
{
    public class ProgressBar : UIObjectBase
    {
        public int MaxWidth { get; set; }

        public float Value { get; set; }

        public Color BarColor { get; set; }

        FilledRectangle Background;
        public ProgressBar(int maxWidth, int height)
        {
            BarColor = Color.WhiteSmoke;
            MaxWidth = maxWidth;
            Background = new FilledRectangle(new Vector2(1, height), Color.White);
        }

        public override void Update()
        {
            //base.Update();
            
        }
        public override void Render()
        {
            if (!Visible)
                return;

            int finalWidth = (int)(Value * MaxWidth / 100f);
            KyunGame.Instance.SpriteBatch.Draw(Background.Texture, new Rectangle((int)Position.X, (int)Position.Y, finalWidth, Background.Texture.Height), BarColor * Opacity);
            
        }
    }
}
