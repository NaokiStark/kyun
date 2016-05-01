using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ubeat.UIObjs;
namespace ubeat.Score
{
    public class ScoreDisplay:IUIObject
    {
        public Microsoft.Xna.Framework.Vector2 Position { get; set; }

        public Microsoft.Xna.Framework.Graphics.Texture2D Texture { get; set; }

        public bool isActive { get; set; }

        public bool Died { get; set; }

        ulong score=0;

        public void Add(long Score)
        {
            this.score += (ulong)Score;
        }

        public void Reset()
        {
            this.score = 0;
        }

        public ScoreDisplay()
        {
            int width = 400;
            int height = 60;
            this.Texture = new Texture2D(Game1.Instance.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
            this.Texture.SetData(data);
        }

        public ulong TotalScore
        {
            get
            {
                return this.score;
            }
        }
        public void Update()
        {
            if (!isActive) return;
        }

        public void Render()
        {
            if (!isActive) return;

            int screenWidth = Game1.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;

            Vector2 fSize = Game1.Instance.fontDefault.MeasureString(score.ToString("00000000")) * 1.5f;

            Rectangle rect = new Rectangle(screenWidth - (int)fSize.X-30, 0, (int)fSize.X+30, (int)fSize.Y);
            Game1.Instance.spriteBatch.Draw(this.Texture, rect, Color.White * .75f);

            //Draw score
            Game1.Instance.spriteBatch.DrawString(Game1.Instance.fontDefault, score.ToString("00000000"), new Vector2(rect.X + 15, 0), Color.WhiteSmoke, 0, new Vector2(0), 1.5f, SpriteEffects.None, 0);
        }
    }
}
