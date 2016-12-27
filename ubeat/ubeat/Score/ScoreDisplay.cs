using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ubeat.UIObjs;
using ubeat.Utils;

namespace ubeat.Score
{
    public class ScoreDisplay : IUIObject
    {
        public Vector2 Position { get; set; }

        public Texture2D Texture { get; set; }

        public bool IsActive { get; set; }

        public bool Died { get; set; }

        float opa = 0f;
        ulong score = 0;

        ulong lScore = 0;

        ulong RollingScore = 0;

        Queue<int> lastAdds = new Queue<int>();

        public void Add(long Score)
        {
            this.lScore = this.score;

            this.score += (ulong)Score;
            opa = 1;
        }

        public void Reset()
        {
            score = 0;
            RollingScore = 0;
            lScore = 0;
        }

        private void updateRolling()
        {
            if (RollingScore < score)
            {
                RollingScore += (ulong)((float)(score - lScore) * (float)(UbeatGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds / 100f));
            }

            float minus = (UbeatGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds / 100f)/5;

            if (opa > 0)
            {
                opa -= minus;
            }
            else
            {
                opa = 0;
            }
        }

        public ScoreDisplay()
        {
            int width = 400;
            int height = 60;
            this.Texture = new Texture2D(UbeatGame.Instance.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
            this.Texture.SetData(data);


            int screenWidth = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;

            Vector2 fSize = SpritesContent.Instance.DefaultFont.MeasureString(RollingScore.ToString("00000000")) * 1.5f;

            Position = new Vector2(screenWidth - (int)fSize.X - 30, 0);
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
            if (!IsActive) return;
            updateRolling();
        }

        public void Render()
        {
            if (!IsActive) return;

            int screenWidth = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;

            Vector2 fSize = SpritesContent.Instance.DefaultFont.MeasureString(RollingScore.ToString("00000000")) * 1.5f;

            Rectangle rect = new Rectangle(screenWidth - (int)fSize.X - 30, 0, (int)fSize.X + 30, (int)fSize.Y);
            
            UbeatGame.Instance.SpriteBatch.Draw(this.Texture, rect, Color.White * .75f);

            //Draw score
            UbeatGame.Instance.SpriteBatch.DrawString(SpritesContent.Instance.DefaultFont, RollingScore.ToString("00000000"), new Vector2(rect.X + 15, 0), Color.WhiteSmoke, 0, new Vector2(0), 1.5f, SpriteEffects.None, 0);

            UbeatGame.Instance.SpriteBatch.DrawString(SpritesContent.Instance.DefaultFont, RollingScore.ToString("00000000"), new Vector2(rect.X + 15, 0), Color.Yellow*opa, 0, new Vector2(0), 1.5f, SpriteEffects.None, 0);
        }
    }
}
