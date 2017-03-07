using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ubeat.GameScreen;
using ubeat.UIObjs;
using ubeat.Utils;

namespace ubeat.Score
{
    public class ScoreDisplay : UIObjectBase
    {

        float opa = 0f;
        ulong score = 0;

        ulong lScore = 0;

        ulong RollingScore = 0;

        Queue<int> lastAdds = new Queue<int>();
        private int oldLength;
        private int textLength;

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

            textLength = RollingScore.ToString("00000000").Length;
        }

        public ScoreDisplay()
        {
            int width = 50;
            int height = 50;
            this.Texture = new Texture2D(UbeatGame.Instance.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
            this.Texture.SetData(data);


            //4 other things
            Screen.ScreenMode actualMode = Screen.ScreenModeManager.GetActualMode();

            Vector2 fSize = getMeasuredText() * 1.5f;
            
            Rectangle rect = new Rectangle(actualMode.Width - (int)fSize.X - 5, 0, (int)fSize.X + 5, (int)fSize.Y);
            Position = new Vector2(rect.X, rect.Y);
        }

        public ulong TotalScore
        {
            get
            {
                return this.score;
            }
        }

        public Vector2 MeasuredString { get; private set; }

        public override void Update()
        {
            if (!IsActive) return;
            updateRolling();
        }

        public override void Render()
        {
            if (!IsActive) return;

            Screen.ScreenMode actualMode = Screen.ScreenModeManager.GetActualMode();

            Vector2 fSize = getMeasuredText() * 1.5f;
            

            Rectangle rect = new Rectangle(actualMode.Width - (int)fSize.X - 5, 0, (int)fSize.X + 5, (int)fSize.Y);

            Position = new Vector2(rect.X, rect.Y);

            UbeatGame.Instance.SpriteBatch.Draw(this.Texture, rect, Color.White * .75f);

            //Draw score
            UbeatGame.Instance.SpriteBatch.DrawString(SpritesContent.Instance.TitleFont, RollingScore.ToString("00000000"), new Vector2(rect.X + 15, 0), Color.WhiteSmoke, 0, new Vector2(0), 1.3f, SpriteEffects.None, 0);

            UbeatGame.Instance.SpriteBatch.DrawString(SpritesContent.Instance.TitleFont, RollingScore.ToString("00000000"), new Vector2(rect.X + 15, 0), Color.Yellow*opa, 0, new Vector2(0), 1.3f, SpriteEffects.None, 0);
        }

        public Vector2 getMeasuredText()
        {
            if (oldLength != textLength)
            {
                MeasuredString = SpritesContent.Instance.TitleFont.MeasureString(RollingScore.ToString("00000000"));
                oldLength = textLength = RollingScore.ToString().Length;
            }

            if (MeasuredString == Vector2.Zero)
            {
                MeasuredString = SpritesContent.Instance.TitleFont.MeasureString(RollingScore.ToString("00000000"));
            }
            return MeasuredString;
        }
    }
}
