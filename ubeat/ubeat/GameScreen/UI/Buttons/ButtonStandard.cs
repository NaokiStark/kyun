using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using kyun.Utils;
using Microsoft.Xna.Framework;

namespace kyun.GameScreen.UI.Buttons
{
    public class ButtonStandard : Button
    {
        Color _color;
        Texture2D TextureOverlay;

        private string text;

        public string Caption {
            get
            {
                return text;
            }
            set
            {
                text = value;
                changeMeasure();
            }
        }

        private void changeMeasure()
        {
            mesStr = ((Font == null) ? SpritesContent.Instance.StandardButtonsFont : Font).MeasureString(text);
        }

        public Color ForegroundColor { get; set; }

        public SpriteFont Font { get; set; }

        Vector2 mesStr = Vector2.Zero;

        public ButtonStandard(Color color) : base(SpritesContent.Instance.ButtonStandard)
        {
            _color = color;
            TextureOverlay = new Texture2D(KyunGame.Instance.GraphicsDevice, SpritesContent.Instance.ButtonStandard.Width, SpritesContent.Instance.ButtonStandard.Height);
            Color[] colors = new Color[SpritesContent.Instance.ButtonStandard.Width * SpritesContent.Instance.ButtonStandard.Height];
            for(int a = 0; a < colors.Length; a++)
            {
                colors[a] = _color * .5f;
            }

            TextureOverlay.SetData(colors);
            
        }

        public override void Render()
        {
            base.Render();

            Rectangle rg = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)(TextureOverlay.Width * Scale), (int)(TextureOverlay.Height * Scale));
            KyunGame.Instance.SpriteBatch.Draw(TextureOverlay, rg, Color.White);

            Vector2 _mesStr = mesStr * Scale;

            Vector2 txZ = new Vector2(TextureOverlay.Width * Scale, TextureOverlay.Height * Scale);

            KyunGame.Instance.SpriteBatch.DrawString((Font == null) ? SpritesContent.Instance.StandardButtonsFont : Font,
                this.Caption,
                new Vector2(Position.X + ((txZ.X / 2) - (_mesStr.X / 2)), Position.Y + ((txZ.Y / 2) - (_mesStr.Y / 2)) + 2),
                Color.Black * 0.5f,
                0,
                Vector2.Zero,
                this.Scale,
                SpriteEffects.None,
                0);

            KyunGame.Instance.SpriteBatch.DrawString((Font == null) ? SpritesContent.Instance.StandardButtonsFont : Font,
                this.Caption,
                new Vector2(Position.X + ((txZ.X/2) - (_mesStr.X / 2)), Position.Y + ((txZ.Y/2) - (_mesStr.Y / 2))),
                (ForegroundColor == null)? Color.White : ForegroundColor,
                0,
                Vector2.Zero,
                this.Scale,
                SpriteEffects.None,
                0);

        }
    }
}
