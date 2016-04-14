using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ubeat.GameScreen;
namespace ubeat.UIObjs
{
    public class ScoreObj:IUIObject
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public bool isActive { get; set; }
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

            Rectangle tcR = new Rectangle((int)Position.X + (int)(Game1.Instance.MissTx.Bounds.Width / 2), (int)Position.Y + (int)(Game1.Instance.MissTx.Bounds.Height / 2), (int)(Game1.Instance.MissTx.Bounds.Width / 2), (int)(Game1.Instance.MissTx.Bounds.Height / 2));
            switch (this.scoreType)
                {
                    case Score.ScoreType.Miss:
                        Game1.Instance.spriteBatch.Draw(Game1.Instance.MissTx, tcR, Color.White * Opacity);
                        break;
                    case Score.ScoreType.Perfect:
                        //Game1.Instance.spriteBatch.DrawString(Game1.Instance.fontDefault, "Perfect", Position, new Color(255,0,0,Opacity),0f,new Vector2(0),1f,SpriteEffects.None,0f);
                        Game1.Instance.spriteBatch.Draw(Game1.Instance.PerfectTx, tcR, Color.White * Opacity);
                        break;
                    case Score.ScoreType.Excellent:
                        //Game1.Instance.spriteBatch.DrawString(Game1.Instance.fontDefault, "Excellent", Position, new Color(255,0,0,Opacity),0f,new Vector2(0),1f,SpriteEffects.None,0f);
                        Game1.Instance.spriteBatch.Draw(Game1.Instance.ExcellentTx, tcR, Color.White * Opacity);
                        break;
                    case Score.ScoreType.Good:
                        //Game1.Instance.spriteBatch.DrawString(Game1.Instance.fontDefault, "Good", Position, new Color(255,0,0,Opacity),0f,new Vector2(0),1f,SpriteEffects.None,0f);
                        Game1.Instance.spriteBatch.Draw(Game1.Instance.GoodTx, tcR, Color.White * Opacity);
                        break;
                    
                }
        }
    }
}
