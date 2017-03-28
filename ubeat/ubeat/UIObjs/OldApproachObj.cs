using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ubeat.GameScreen;
using ubeat.Screen;
using ubeat.Utils;
using ubeat.GameModes.Classic;

namespace ubeat.UIObjs
{
    public class OldApproachObj : UIObjectBase
    {
        float width = 0;
        float height = 0;
        float opacity = 1;
        float approachrate = 0;
        decimal starttime;

        Grid GridInstance;
        
        public OldApproachObj(Vector2 position, float approachRate, decimal startTime, Grid Instance)
        {
            GridInstance = Instance;
            ScreenMode mode = ScreenModeManager.GetActualMode();
            bool isSmallRes = mode.Height < 720;

            this.starttime = startTime;
            this.Position = position;
            this.Texture = (isSmallRes)? SpritesContent.Instance.WaitDefault_0 : SpritesContent.Instance.WaitDefault;
            this.approachrate = approachRate;
            this.width = Texture.Bounds.Width;
            this.height = Texture.Bounds.Height;
            
        }

        void tickSize_Tick()
        {

            float twidth = Texture.Bounds.Width * 1.8f;

            int appr = (int)(1950 - approachrate * 150);

            
            float stime = (float)starttime - (float)GridInstance.GameTimeTotal;
           
            float gtime = (stime / appr);
            float percen = gtime * twidth;

            float pcrt = percen / twidth * 100;

            float percentgg = percen;


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
            }
        }

        public void Apdeit(Vector2 position)
        {
            Position = position;
            //            RotationAngle += Game1.Instance.elapsed;
            //float circle = MathHelper.Pi * 2;
            //RotationAngle = RotationAngle % circle;
            Update();
        }
        float RotationAngle=0;
        public override void Render()
        {
            if (Died)
                return;
            //if(width <= Texture.Bounds.Width)
                


            UbeatGame.Instance.SpriteBatch.Draw(this.Texture,
                    new Rectangle((int)Position.X + (int)Texture.Bounds.Width / 2,(int)Position.Y + (int)Texture.Bounds.Height / 2, (int)width, (int)height),
                    null, Color.White * opacity,
                    //RotationAngle,
                    0,
                    new Vector2(Texture.Bounds.Width / 2, Texture.Bounds.Height / 2),
                    Microsoft.Xna.Framework.Graphics.SpriteEffects.None,
                    0);
           // else
              //  Game1.Instance.spriteBatch.Draw(this.Texture, new Rectangle((int)Position.X + (int)Texture.Bounds.Width / 2, (int)Position.Y + (int)Texture.Bounds.Height / 2, (int)Texture.Bounds.Width, (int)Texture.Bounds.Height), null, Color.White * opacity, 0, new Vector2(Texture.Bounds.Width / 2, Texture.Bounds.Height / 2), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
        }

        public override void Update()
        {
            if(!Died)
                tickSize_Tick();
        }
    }
}
