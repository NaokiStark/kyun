using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ubeat.GameScreen;
using ubeat.UIObjs;
using ubeat.Utils;

namespace ubeat.Score
{
    public class ComboDisplay : UIObjectBase
    {

        Vector2 MeasuredString { get; set; }

        int oldLength = 0;

        int textLength = 0;

        int lastCD = 0;

        public ComboDisplay()
        {

            Scale = 1;
            this.Texture = new Texture2D(UbeatGame.Instance.GraphicsDevice, 50, 50);
            Color[] data = new Color[50 * 50];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
            this.Texture.SetData(data);
        }

        public override void Update()
        {
            if (!IsActive) return;
            
            reduce();

            if (lastCD < Combo.Instance.ActualMultiplier)
            {
                textLength = Combo.Instance.ActualMultiplier.ToString().Length;
                Scale = 1.4f;
                
            }
            lastCD = (int)Combo.Instance.ActualMultiplier;
        }

        void reduce()
        {
           
            if(Scale - (float)UbeatGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * .001f > 1)
            {
                Scale -= ((float)UbeatGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * .001f);
            }

        }

        public override void Render()
        {
            if (!IsActive) return;
            if (lastCD < 2) return;

            Screen.ScreenMode actualScreenMode = Screen.ScreenModeManager.GetActualMode();

            Vector2 textSize = getMeasuredText() * Scale;

            Rectangle rg = new Rectangle(actualScreenMode.Width - (int)textSize.X - 10,
                actualScreenMode.Height - (int)textSize.Y - 8,
                (int)textSize.X + 35,
                (int)textSize.Y + 8);

            Rectangle rgbox = new Rectangle(rg.X - 3,
                rg.Y + 5,
                rg.X,
                rg.Y);

            UbeatGame.Instance.SpriteBatch.Draw(this.Texture, rgbox, null, Color.White * .75f, 0, Vector2.Zero, SpriteEffects.None, 0);
            UbeatGame.Instance.SpriteBatch.DrawString(SpritesContent.Instance.TitleFont, lastCD.ToString() + "x", new Vector2(rg.X + 3, rg.Y + 5), Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }

        private Vector2 getMeasuredText()
        {
            if(oldLength != textLength)
            {
                MeasuredString = SpritesContent.Instance.TitleFont.MeasureString(lastCD.ToString()+"x");
                oldLength = textLength = lastCD.ToString().Length;
            }

            if(MeasuredString == null)
            {
                MeasuredString = SpritesContent.Instance.TitleFont.MeasureString("1x");
            }
            return MeasuredString;
        }
    }
}
