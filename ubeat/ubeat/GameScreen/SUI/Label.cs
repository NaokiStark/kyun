﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace ubeat.GameScreen.UI
{
    
    public class Label:GameScreen.ScreenUIObject
    {
        public SpriteFont Font;
        public string Text { get; set; }
        public Vector2 Size { get; set; }
        public float BackgroundOpacity { get; set; }

        string lastStr ="";
            
        public Label(float backgroundOpacity = 0.8f)
        {
            this.Text = ""; //WTF NULL
            BackgroundOpacity = backgroundOpacity;
            generateTexture(5, 5);
            Scale = 1;
        }

        private void generateTexture(int w, int h)
        {            
            this.Texture = new Texture2D(UbeatGame.Instance.GraphicsDevice, w, h);
            Color[] dataBar = new Color[w * h];
            for (int i = 0; i < dataBar.Length; ++i) dataBar[i] = Color.Black * BackgroundOpacity;
            this.Texture.SetData(dataBar);
        }

        public override void Update()
        {
            
            if(lastStr != Text) {
                Vector2 messStr;
                if (Font == null)
                    messStr = UbeatGame.Instance.defaultFont.MeasureString(this.Text) * Scale;
                else
                    messStr = Font.MeasureString(this.Text) * Scale;

                messStr = new Vector2(messStr.X + 15, messStr.Y + 10);
                generateTexture((int)messStr.X, (int)messStr.Y);
            }        

            lastStr = Text;
            base.Update(); //Update Events
        }

        public override void Render()
        {
            //base.Render(); //Nope
            Vector2 messStr;
            if(Font==null)
                messStr = UbeatGame.Instance.defaultFont.MeasureString(this.Text) * Scale;
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
                UbeatGame.Instance.spriteBatch.Draw(this.Texture, new Rectangle((int)pos.X, (int)pos.Y, (int)(messStr.X), (int)(messStr.Y)), Color.White);
            }
            else
            {
                UbeatGame.Instance.spriteBatch.Draw(this.Texture, new Rectangle((int)pos.X, (int)pos.Y, (int)(Size.X), (int)(Size.Y)), Color.White);
            }

            UbeatGame.Instance.spriteBatch.DrawString((Font == null) ? UbeatGame.Instance.defaultFont : Font, this.Text, new Vector2(pos.X + 5, pos.Y + 5), Color.White, 0,
                Vector2.Zero,this.Scale,SpriteEffects.None,0);
        }

        public bool Centered { get; set; }
    }
}
