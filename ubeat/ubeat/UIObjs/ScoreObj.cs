using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using kyun.GameScreen;
using kyun.Utils;

namespace kyun.UIObjs
{
    public class ScoreObj : UIObjectBase
    {

        public Score.ScoreType scoreType { get; set; }
        private float RotationAngle;

        public ScoreObj(Score.ScoreType scType, Vector2 position)
        {
            this.scoreType = scType;
            this.Position = position;

        }

        public override void Update()
        {
            if (Died)
            {
                Grid.Instance.objs.Remove(this);
            }

            if (scoreType != Score.ScoreType.Miss)
                Position = new Vector2(Position.X, (float)Position.Y - (float)((double)KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 0.5d));
            else
                Position = new Vector2((float)Position.X + (float)((double)KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 0.45d), (float)Position.Y + (float)((double)KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 0.5d));

            float circle = (float)Math.PI * 2;

            float elapsed = (float)KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalSeconds;
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

        public override void Render()
        {
            if (Died)
                return;

            Rectangle tcR = new Rectangle((int)Position.X + (int)(SpritesContent.Instance.MissTx.Bounds.Width / 2), (int)Position.Y + (int)(SpritesContent.Instance.MissTx.Bounds.Height / 2), (int)(SpritesContent.Instance.MissTx.Bounds.Width / 2), (int)(SpritesContent.Instance.MissTx.Bounds.Height / 2));

            Vector2 origin = new Vector2(SpritesContent.Instance.MissTx.Width / 2, SpritesContent.Instance.MissTx.Height / 2);

            switch (this.scoreType)
            {
                case Score.ScoreType.Miss:
                    KyunGame.Instance.SpriteBatch.Draw(SpritesContent.Instance.MissTx, tcR, null, Color.White * Opacity, RotationAngle, origin, SpriteEffects.None, 0);
                    break;
                case Score.ScoreType.Perfect:
                    KyunGame.Instance.SpriteBatch.Draw(SpritesContent.Instance.PerfectTx, tcR, null, Color.White * Opacity, RotationAngle, origin, SpriteEffects.None, 0);
                    break;
                case Score.ScoreType.Excellent:
                    KyunGame.Instance.SpriteBatch.Draw(SpritesContent.Instance.ExcellentTx, tcR, null, Color.White * Opacity, RotationAngle, origin, SpriteEffects.None, 0);
                    break;
                case Score.ScoreType.Good:
                    KyunGame.Instance.SpriteBatch.Draw(SpritesContent.Instance.GoodTx, tcR, null, Color.White * Opacity, RotationAngle, origin, SpriteEffects.None, 0);
                    break;

            }
        }
    }
}
