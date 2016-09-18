using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ubeat.UIObjs;

namespace ubeat.Score
{
    public class ComboDisplay : IUIObject
    {

        public Vector2 Position { get; set; }

        public Texture2D Texture { get; set; }

        public bool IsActive { get; set; }

        public bool Died { get; set; }

        Vector2 size { get; set; }

        static int max_size = 200;

        int lastCD=0;

        public ComboDisplay()
        {
            size = new Vector2(200,60);
            
            this.Texture = new Texture2D(UbeatGame.Instance.GraphicsDevice, (int)size.X, (int)size.Y);
            Color[] data = new Color[((int)size.X * (int)size.Y)];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
            this.Texture.SetData(data);
        }

        public void Update()
        {
            if (!IsActive) return;
            
            reduce();

            if (lastCD < Combo.Instance.ActualMultiplier)
            {
                size = new Vector2(400, (60f / 200f) * 400f);
                
            }
            lastCD = (int)Combo.Instance.ActualMultiplier;
        }

        void reduce()
        {
            //float newWidth = size.X / 1.01f;
            float newWidth = size.X - (float)UbeatGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * .2f;

            if (newWidth > 300)
            {                
                float newHeight = (60f / 200f) * newWidth;
                size = new Vector2(newWidth, newHeight);
            }
        }

        public void Render()
        {
            if (!IsActive) return;
            if (lastCD < 2) return;

            int sWidth = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int sHeight = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;

            Vector2 origin = (UbeatGame.Instance.defaultFont.MeasureString(lastCD.ToString() + "x") * (size.X / 200f)) / 2;

            float posX = sWidth - (UbeatGame.Instance.defaultFont.MeasureString(lastCD.ToString() + "x") * (size.X / 200f)).X;

            Vector2 fsize= UbeatGame.Instance.defaultFont.MeasureString(lastCD.ToString() + "x") * (size.X / 200f);

            Rectangle rg = new Rectangle((int)posX - 20, (int)(sHeight- 20), (int)fsize.X + 30, (int)fsize.Y);

            UbeatGame.Instance.spriteBatch.Draw(this.Texture, rg, null, Color.White * .75f, 0, new Vector2(0, origin.Y), SpriteEffects.None, 0);
            UbeatGame.Instance.spriteBatch.DrawString(UbeatGame.Instance.defaultFont, lastCD.ToString() + "x", new Vector2(posX-10,rg.Y), Color.WhiteSmoke, 0, new Vector2(0,fsize.Y/3), (float)(size.X / 200f), SpriteEffects.None, 0);
        }
    }
}
