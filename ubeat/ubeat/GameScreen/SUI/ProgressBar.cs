using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubeat.GameScreen.SUI
{
    public class ProgressBar : ScreenUIObject
    {
        public int MaxWidth { get; set; }

        public float Value { get; set; }

        FilledRectangle Background;
        public ProgressBar(int maxWidth, int height)
        {
            MaxWidth = maxWidth;
            Background = new FilledRectangle(new Vector2(1, height), Color.WhiteSmoke);
        }

        public override void Update()
        {
            //base.Update();
            
        }
        public override void Render()
        {

            int finalWidth = (int)(Value * MaxWidth / 100f);
            UbeatGame.Instance.SpriteBatch.Draw(Background.Texture, new Rectangle((int)Position.X, (int)Position.Y, finalWidth, Background.Texture.Height), Color.White);
            
        }
    }
}
