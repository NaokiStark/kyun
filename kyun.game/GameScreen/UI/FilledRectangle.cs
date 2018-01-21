using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace kyun.GameScreen.UI
{
    public class FilledRectangle : UIObjectBase
    {
        Color fColor;
        public FilledRectangle(Vector2 Size, Color Colr)
        {
            fColor = Colr;
            this.Texture = new Texture2D(KyunGame.Instance.GraphicsDevice, (int)Size.X, (int)Size.Y);
            Color[] dataBar = new Color[(int)Size.X * (int)Size.Y];
            for (int i = 0; i < dataBar.Length; ++i) dataBar[i] = Colr;
            this.Texture.SetData(dataBar);
            Scale = 1;            
        }
        public void Resize(Vector2 Size)
        {
            this.Texture = new Texture2D(KyunGame.Instance.GraphicsDevice, (int)Size.X, (int)Size.Y);
            Color[] dataBar = new Color[(int)Size.X * (int)Size.Y];
            for (int i = 0; i < dataBar.Length; ++i) dataBar[i] = fColor;
            this.Texture.SetData(dataBar);
        }
    }
}
