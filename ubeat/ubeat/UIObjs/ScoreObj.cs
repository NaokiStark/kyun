using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ubeat.GameScreen;

namespace ubeat.UIObjs
{
    public class ScoreObj:IUIObject
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public bool IsActive { get; set; }
        public bool Died { get; set; }
        float Opacity = 1f;
        Timer tickOpacity;
        public Score.ScoreType scoreType { get; set; }

        public ScoreObj(Score.ScoreType scType, Vector2 position)
        {
            this.scoreType = scType;
            this.Position = position;
            tickOpacity = new Timer()
            {
                Interval = 1
            };
            tickOpacity.Tick += tickOpacity_Tick;
            tickOpacity.Start();
        }

        void tickOpacity_Tick(object sender, EventArgs e)
        {
            if ((Opacity - .01f) < 0f)
            {
                Died = true;
                tickOpacity.Stop();
                return;
            }
            this.Opacity = this.Opacity - .02f;
        }
        public void Update()
        {
            if (Died)
            {
                Grid.Instance.objs.Remove(this);
            }
        }

        public void Render()
        {
            if (Died)
                return;

            Rectangle tcR = new Rectangle((int)Position.X + (int)(UbeatGame.Instance.MissTx.Bounds.Width / 2), (int)Position.Y + (int)(UbeatGame.Instance.MissTx.Bounds.Height / 2), (int)(UbeatGame.Instance.MissTx.Bounds.Width / 2), (int)(UbeatGame.Instance.MissTx.Bounds.Height / 2));
            switch (this.scoreType)
                {
                    case Score.ScoreType.Miss:
                        UbeatGame.Instance.spriteBatch.Draw(UbeatGame.Instance.MissTx, tcR, Color.White * Opacity);
                        break;
                    case Score.ScoreType.Perfect:
                        UbeatGame.Instance.spriteBatch.Draw(UbeatGame.Instance.PerfectTx, tcR, Color.White * Opacity);
                        break;
                    case Score.ScoreType.Excellent:
                        UbeatGame.Instance.spriteBatch.Draw(UbeatGame.Instance.ExcellentTx, tcR, Color.White * Opacity);
                        break;
                    case Score.ScoreType.Good:
                        UbeatGame.Instance.spriteBatch.Draw(UbeatGame.Instance.GoodTx, tcR, Color.White * Opacity);
                        break;
                    
                }
        }
    }
}
