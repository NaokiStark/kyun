using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubeat.GameScreen.UI
{
    
    public class Label:GameScreen.ScreenUIObject
    {
        public SpriteFont Font;
        public string Text { get; set; }
        public Vector2 Size { get; set; }
        public float BackgroundOpacity { get; set; }
        public Label(float backgroundOpacity = 0.8f)
        {
            BackgroundOpacity = backgroundOpacity;
            int width = 5;
            int height = 5;
            this.Texture = new Texture2D(Game1.Instance.GraphicsDevice, width, height);
            Color[] dataBar = new Color[width * height];
            for (int i = 0; i < dataBar.Length; ++i) dataBar[i] = Color.Black * BackgroundOpacity;
            this.Texture.SetData(dataBar);
            Scale = 1;
        }

        public override void Update()
        {
            base.Update(); //Update Events
           
        }

        public override void Render()
        {
            //base.Render(); //Nope
            Vector2 messStr;
            if(Font==null)
                messStr = Game1.Instance.defaultFont.MeasureString(this.Text) * Scale;
            else
                messStr = Font.MeasureString(this.Text) * Scale;

            messStr = new Vector2(messStr.X + 15, messStr.Y + 10);

            Vector2 pos = Position;

            if (Centered)
            {
                pos.X = Position.X - messStr.X / 2;
            }

            if (Size == null || Size == Vector2.Zero)
            {
                Game1.Instance.spriteBatch.Draw(this.Texture, new Rectangle((int)pos.X, (int)pos.Y, (int)(messStr.X), (int)(messStr.Y)), Color.White);
            }
            else
            {
                Game1.Instance.spriteBatch.Draw(this.Texture, new Rectangle((int)pos.X, (int)pos.Y, (int)(Size.X), (int)(Size.Y)), Color.White);
            }

            Game1.Instance.spriteBatch.DrawString((Font == null) ? Game1.Instance.defaultFont : Font, this.Text, new Vector2(pos.X + 5, pos.Y + 5), Color.White, 0,
                Vector2.Zero,this.Scale,SpriteEffects.None,0);
        }

        public bool Centered { get; set; }
    }
}
