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

        public void Add(ScoreValue Score)
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

            Rectangle rect = new Rectangle(screenWidth - this.Texture.Width, 0, this.Texture.Width, this.Texture.Height);
            Game1.Instance.spriteBatch.Draw(this.Texture, rect, Color.White * .75f);

            //Draw score
            Game1.Instance.spriteBatch.DrawString(Game1.Instance.fontDefault, score.ToString(), new Vector2(rect.X + 10, 0), Color.White,0,new Vector2(0),1.5f,SpriteEffects.None,0);
        }
    }
}
