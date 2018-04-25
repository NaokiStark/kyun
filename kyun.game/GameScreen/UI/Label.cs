using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using kyun.Utils;

namespace kyun.GameScreen.UI
{

    public class Label : UIObjectBase
    {
        public SpriteFont Font
        {
            get
            {
                if (font == null)
                    return SpritesContent.Instance.DefaultFont;

                return font;
            }
            set
            {
                InitialFont = font = value;
            }
        }


        public string Text { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 TotalSize { get; set; }
        public float BackgroundOpacity { get; set; }
        public Color ForegroundColor { get; set; }
        public SpriteFont InitialFont { get; set; }

        SpriteFont font;



        string lastStr = "";

        int lastLength = 0;
        int textLength = 0;

        string lastText = "";

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

            if (lastLength != textLength)
            {
                messStr = getSizeOfText();
                messStr = new Vector2(messStr.X + 15, messStr.Y + 10);
                generateTexture((int)messStr.X, (int)messStr.Y);
                lastLength = Text.Length;
            }

            base.Update(); //Update Events
        }

        Vector2 getSizeOfText() {

            font = InitialFont;
            try
            {
                return Font.MeasureString(Text) * Scale;
            }
            catch
            {

                if (Font == SpritesContent.Instance.DefaultFont || Font == SpritesContent.Instance.ListboxFont || Font == SpritesContent.Instance.StandardButtonsFont)
                {
                    font = SpritesContent.Instance.MSGothic1;
                }
                else
                {
                    font = SpritesContent.Instance.MSGothic2;
                }

                return Font.MeasureString(Text) * Scale;
            }
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

            try
            {
                if (Shadow)
                    KyunGame.Instance.SpriteBatch.DrawString(Font, this.Text, new Vector2(pos.X + 5, pos.Y + 5 + 2), Color.Black * 0.6f * Opacity, 0,
                    Vector2.Zero, this.Scale, SpriteEffects.None, 0);

                KyunGame.Instance.SpriteBatch.DrawString(Font, this.Text, new Vector2(pos.X + 5, pos.Y + 5), ForegroundColor * Opacity, 0,
                    Vector2.Zero, this.Scale, SpriteEffects.None, 0);
            }
            catch
            {
                /*
                if (Font == SpritesContent.Instance.DefaultFont || Font == SpritesContent.Instance.ListboxFont || Font == SpritesContent.Instance.StandardButtonsFont)
                {
                    Font = SpritesContent.Instance.MSGothic1;
                }
                else
                {
                    Font = SpritesContent.Instance.MSGothic2;
                }          */   
            }

            Tooltip?.Render();
        }

        public bool Centered { get; set; }
    }
}
