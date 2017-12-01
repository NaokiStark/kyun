using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using kyun.Utils;

namespace kyun.GameScreen.UI
{
    
    public class Label: UIObjectBase
    {
        public SpriteFont Font;
        public string Text { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 TotalSize { get; set; }
        public float BackgroundOpacity { get; set; }
        public Color ForegroundColor { get; set; }

        string lastStr ="";

        int lastLength = 0;
        int textLength = 0;

        public bool Shadow { get; set; }

        Vector2 messStr { get; set; }

        public Label(float backgroundOpacity = 0.8f)
        {
            this.Text = ""; //WTF NULL
            BackgroundOpacity = backgroundOpacity;
            generateTexture(1, 1);
            Scale = 1;
            ForegroundColor = Color.White;
            Shadow = true;
        }

        private void generateTexture(int w, int h)
        {
            
            if (KyunGame.Instance.GraphicsDevice == null) return; //BUG ON CLOSE, WTF M8!!!11!!1!!
            Texture = new Texture2D(KyunGame.Instance.GraphicsDevice, w, h);
            Color[] dataBar = new Color[w * h];
            for (int i = 0; i < dataBar.Length; ++i) dataBar[i] = Color.Black * BackgroundOpacity;
            this.Texture.SetData(dataBar);
           
        }

        public override void Update()
        {


            if (Disposing) return;

            if (!Visible)
                return;

            textLength = Text.Length;

            if (lastLength != textLength) {
                
                if (Font == null)
                    messStr = SpritesContent.Instance.DefaultFont.MeasureString(this.Text) * Scale;
                else
                    messStr = Font.MeasureString(this.Text) * Scale;

                messStr = new Vector2(messStr.X + 15, messStr.Y + 10);
                generateTexture((int)messStr.X, (int)messStr.Y);
                textLength = lastLength = Text.Length;
            }        

            base.Update(); //Update Events
        }

        public override void Render()
        {
            if (!Visible)
                return;
                      
            Vector2 pos = Position;

            if (Centered)
            {
                pos.X = Position.X - messStr.X / 2;
            }

            if (Size == null || Size == Vector2.Zero)
            {
                KyunGame.Instance.SpriteBatch.Draw(this.Texture, new Rectangle((int)pos.X, (int)pos.Y, (int)(messStr.X), (int)(messStr.Y)), Color.White);
                //SpriteBatchExtensions.DrawRoundedRect(UbeatGame.Instance.spriteBatch, new Rectangle((int)pos.X, (int)pos.Y, (int)(messStr.X), (int)(messStr.Y)), Texture, 16, Color.White);
                TotalSize = new Vector2(messStr.X, messStr.Y); //TODO: Make this better
            }
            else
            {
                KyunGame.Instance.SpriteBatch.Draw(this.Texture, new Rectangle((int)pos.X, (int)pos.Y, (int)(Size.X), (int)(Size.Y)), Color.White);
                TotalSize = Size; //TODO: Make this better
            }

            if (Shadow)
                KyunGame.Instance.SpriteBatch.DrawString((Font == null) ? SpritesContent.Instance.DefaultFont : Font, this.Text, new Vector2(pos.X + 5, pos.Y + 5 + 2), Color.Black *0.6f, 0,
                Vector2.Zero, this.Scale, SpriteEffects.None, 0);

            KyunGame.Instance.SpriteBatch.DrawString((Font == null) ? SpritesContent.Instance.DefaultFont : Font, this.Text, new Vector2(pos.X + 5, pos.Y + 5), ForegroundColor, 0,
                Vector2.Zero,this.Scale,SpriteEffects.None,0);
        }

        public bool Centered { get; set; }
    }
}
