using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.GameScreen;
using kyun.UIObjs;
using kyun.Utils;

namespace kyun.Score
{
    public class ScoreDisplay : UIObjectBase
    {

        float opa = 0f;
        ulong score = 0;

        ulong lScore = 0;

        float RollingScore = 0;

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
            IsActive = true;
        }

        private void updateRolling()
        {
            
            if (RollingScore < score)
            {
                RollingScore += ((float)(score - lScore) * (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds / 100f));
            }

            float minus = (KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds / 100f)/5;

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

        public ScoreDisplay(float scale = 1.5f)
        {

            Vector2 fSize = getMeasuredText() * scale;
            Scale = scale;

            int width = (int)fSize.X;
            int height = (int)fSize.Y;
            this.Texture = new Texture2D(KyunGame.Instance.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
            this.Texture.SetData(data);


            //4 other things
            Screen.ScreenMode actualMode = Screen.ScreenModeManager.GetActualMode();

            
            
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

            Vector2 fSize = getMeasuredText() * Scale;
            

            Rectangle rect = new Rectangle((int)Position.X, (int)Position.Y, (int)fSize.X + 5, (int)fSize.Y);

            //Position = new Vector2(rect.X, rect.Y);

            KyunGame.Instance.SpriteBatch.Draw(this.Texture, rect, Color.White * .75f);

            //Draw score
            KyunGame.Instance.SpriteBatch.DrawString(SpritesContent.Instance.TitleFont, RollingScore.ToString("00000000"), new Vector2(Position.X + 15, Position.Y), Color.WhiteSmoke, 0, new Vector2(0), Scale - 0.2f, SpriteEffects.None, 0);

            KyunGame.Instance.SpriteBatch.DrawString(SpritesContent.Instance.TitleFont, RollingScore.ToString("00000000"), new Vector2(Position.X + 15, Position.Y), Color.Yellow*opa, 0, new Vector2(0), Scale - 0.2f, SpriteEffects.None, 0);
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
