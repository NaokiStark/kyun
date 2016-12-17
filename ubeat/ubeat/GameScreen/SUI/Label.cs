using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using ubeat.Utils;

namespace ubeat.GameScreen.UI
{
    
    public class Label: ScreenUIObject
    {
        public SpriteFont Font;
        public string Text { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 TotalSize { get; set; }
        public float BackgroundOpacity { get; set; }

        string lastStr ="";
            
        public Label(float backgroundOpacity = 0.8f)
        {
            this.Text = ""; //WTF NULL
            BackgroundOpacity = backgroundOpacity;
            generateTexture(1, 1);
            Scale = 1;
        }

        private void generateTexture(int w, int h)
        {
            
            if (UbeatGame.Instance.GraphicsDevice == null) return; //BUG ON CLOSE, WTF M8!!!11!!1!!
            Texture = new Texture2D(UbeatGame.Instance.GraphicsDevice, w, h);
            Color[] dataBar = new Color[w * h];
            for (int i = 0; i < dataBar.Length; ++i) dataBar[i] = Color.Black * BackgroundOpacity;
            this.Texture.SetData(dataBar);
           
        }

        public override void Update()
        {
            if (Disposing) return;

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
                //SpriteBatchExtensions.DrawRoundedRect(UbeatGame.Instance.spriteBatch, new Rectangle((int)pos.X, (int)pos.Y, (int)(messStr.X), (int)(messStr.Y)), Texture, 16, Color.White);
                TotalSize = new Vector2(messStr.X, messStr.Y); //TODO: Make this better
            }
            else
            {
                UbeatGame.Instance.spriteBatch.Draw(this.Texture, new Rectangle((int)pos.X, (int)pos.Y, (int)(Size.X), (int)(Size.Y)), Color.White);
                TotalSize = Size; //TODO: Make this better
            }

            UbeatGame.Instance.spriteBatch.DrawString((Font == null) ? UbeatGame.Instance.defaultFont : Font, this.Text, new Vector2(pos.X + 5, pos.Y + 5), Color.White, 0,
                Vector2.Zero,this.Scale,SpriteEffects.None,0);
        }

        public bool Centered { get; set; }
    }
}
