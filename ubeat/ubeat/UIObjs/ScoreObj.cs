using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using ubeat.GameScreen;

namespace ubeat.UIObjs
{
    public class ScoreObj : IUIObject
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public bool IsActive { get; set; }
        public bool Died { get; set; }
        float Opacity = 1f;
        public Score.ScoreType scoreType { get; set; }
        private float RotationAngle;

        public ScoreObj(Score.ScoreType scType, Vector2 position)
        {
            this.scoreType = scType;
            this.Position = position;

        }

        public void Update()
        {
            if (Died)
            {
                Grid.Instance.objs.Remove(this);
            }

            if (scoreType != Score.ScoreType.Miss)
                Position = new Vector2(Position.X, (float)Position.Y - (float)((double)UbeatGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 0.5d));
            else
                Position = new Vector2((float)Position.X + (float)((double)UbeatGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 0.45d), (float)Position.Y + (float)((double)UbeatGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 0.5d));

            float circle = (float)Math.PI * 2;

            float elapsed = (float)UbeatGame.Instance.GameTimeP.ElapsedGameTime.TotalSeconds;
            elapsed = elapsed * 2;
            RotationAngle += elapsed;
            RotationAngle = RotationAngle % circle;

            if ((Opacity - (elapsed * .5f)) < 0f)
            {
                Died = true;
                return;
            }

            Opacity = Opacity - (elapsed * .6f);
        }

        public void Render()
        {
            if (Died)
                return;

            Rectangle tcR = new Rectangle((int)Position.X + (int)(UbeatGame.Instance.MissTx.Bounds.Width / 2), (int)Position.Y + (int)(UbeatGame.Instance.MissTx.Bounds.Height / 2), (int)(UbeatGame.Instance.MissTx.Bounds.Width / 2), (int)(UbeatGame.Instance.MissTx.Bounds.Height / 2));

            Vector2 origin = new Vector2(UbeatGame.Instance.MissTx.Width / 2, UbeatGame.Instance.MissTx.Height / 2);

            switch (this.scoreType)
            {
                case Score.ScoreType.Miss:
                    UbeatGame.Instance.spriteBatch.Draw(UbeatGame.Instance.MissTx, tcR, null, Color.White * Opacity, RotationAngle, origin, SpriteEffects.None, 0);
                    break;
                case Score.ScoreType.Perfect:
                    UbeatGame.Instance.spriteBatch.Draw(UbeatGame.Instance.PerfectTx, tcR, null, Color.White * Opacity, RotationAngle, origin, SpriteEffects.None, 0);
                    break;
                case Score.ScoreType.Excellent:
                    UbeatGame.Instance.spriteBatch.Draw(UbeatGame.Instance.ExcellentTx, tcR, null, Color.White * Opacity, RotationAngle, origin, SpriteEffects.None, 0);
                    break;
                case Score.ScoreType.Good:
                    UbeatGame.Instance.spriteBatch.Draw(UbeatGame.Instance.GoodTx, tcR, null, Color.White * Opacity, RotationAngle, origin, SpriteEffects.None, 0);
                    break;

            }
        }
    }
}
