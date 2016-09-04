using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ubeat.GameScreen;

namespace ubeat.UIObjs
{
    public class ApproachObj : IUIObject
    {

        public Microsoft.Xna.Framework.Vector2 Position { get; set; }

        public Microsoft.Xna.Framework.Graphics.Texture2D Texture { get; set; }

        public bool isActive { get; set; }

        public bool Died { get; set; }

        public Timer tickSize { get; set; }

        float width = 0;
        float height = 0;
        float opacity = 1;
        float approachrate = 0;
        decimal starttime;
        
        public ApproachObj(Vector2 position, float approachRate,decimal startTime)
        {
            this.starttime = startTime;
            this.Position = position;
            this.Texture = Game1.Instance.waitDefault;
            this.approachrate = approachRate;
            this.width = Texture.Bounds.Width;
            this.height = Texture.Bounds.Height;
            tickSize = new Timer()
            {
                Interval = 1
            };
            tickSize.Tick += tickSize_Tick;
            tickSize.Start();
        }

        void tickSize_Tick(object sender, EventArgs e)
        {
            
            int appr = (int)(1950 - approachrate * 150);



            float stime = (float)starttime - (float)Grid.Instance.GameTimeTotal;



            float gtime = (stime / appr);
            float percen = gtime*(((float)Texture.Bounds.Width));

            float pcrt = percen / Texture.Bounds.Width * 100;

            float percentgg = percen * 2f;


            float percentg = gtime * 1/*/100f*/;
            
            if (opacity > -1)
            {
                opacity =  percentg + .2f;
            }
            else
            {
                opacity = 0;
            }
            width = percentgg;
            height = percentgg;
            if (width < 0)
            {
                Died = true;
                tickSize.Stop();

            }
        }

        public void Apdeit(Vector2 position)
        {
            Position = position;
//            RotationAngle += Game1.Instance.elapsed;
            //float circle = MathHelper.Pi * 2;
            //RotationAngle = RotationAngle % circle;
        }
        float RotationAngle=0;
        public void Render()
        {
            if (Died)
                return;
            //if(width <= Texture.Bounds.Width)

           

                Game1.Instance.spriteBatch.Draw(this.Texture,
                    new Rectangle((int)Position.X + (int)135 / 2,(int)Position.Y + (int)135 / 2, (int)width, (int)height),
                    null, Color.White * opacity,
                    //RotationAngle,
                    0,
                    new Vector2(Texture.Bounds.Width / 2, Texture.Bounds.Height / 2),
                    Microsoft.Xna.Framework.Graphics.SpriteEffects.None,
                    0);
           // else
              //  Game1.Instance.spriteBatch.Draw(this.Texture, new Rectangle((int)Position.X + (int)Texture.Bounds.Width / 2, (int)Position.Y + (int)Texture.Bounds.Height / 2, (int)Texture.Bounds.Width, (int)Texture.Bounds.Height), null, Color.White * opacity, 0, new Vector2(Texture.Bounds.Width / 2, Texture.Bounds.Height / 2), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
