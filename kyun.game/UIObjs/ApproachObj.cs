using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using kyun.GameScreen;
using kyun.Screen;
using kyun.Utils;
using kyun.GameModes.Classic;
using kyun.GameModes;

namespace kyun.UIObjs
{
    public class ApproachObj : UIObjectBase
    {
        float width = 0;
        float height = 0;
        float approachrate = 0;
        decimal starttime;


        GameModeScreenBase ClassicInstance;
        
        public ApproachObj(Vector2 position, float approachRate, decimal startTime, GameModeScreenBase Instance)
        {
            ClassicInstance = Instance;
            ScreenMode mode = ScreenModeManager.GetActualMode();
            bool isSmallRes = mode.Height < 600 && mode.Width < 1000;

            this.starttime = startTime;
            this.Position = position;
            this.Texture = /*(isSmallRes)? SpritesContent.Instance.WaitDefault_0 : */SpritesContent.Instance.WaitDefault;
            this.approachrate = approachRate;
            this.width = Size.X;
            this.height = Size.Y;
            
        }

        void tickSize_Tick()
        {

            float twidth = (Size.X * Scale) * 2f;

            int appr = (int)(1950 - approachrate * 150);

            
            float stime = (float)starttime - (float)ClassicInstance.GamePosition;
           
            float gtime = (stime / appr);
            float percen = gtime * twidth;

            float pcrt = percen / twidth * 100;

            float percentgg = percen + (Size.X * Scale);


            float percentg = gtime * (Size.X * Scale)/*/100f*/;
            /*
            if (opacity > -1)
            {
                opacity =  percentg + .2f;
            }
            else
            {
                opacity = 0;
            }*/
            
            width = percentgg;
            height = percentgg;
            if (width < (Size.X * Scale))
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

        public override void Render()
        {
            if (Died)
                return;


            //if(width <= Texture.Bounds.Width)
            
            KyunGame.Instance.SpriteBatch.Draw(this.Texture,
                    new Rectangle((int)Position.X + (int)Size.X  / 2,(int)Position.Y + (int)Size.Y  / 2, (int)width, (int)height),
                    null, base.TextureColor * Opacity,
                    //RotationAngle,
                    0,
                    new Vector2(Size.X / 2, Size.Y / 2),
                    Microsoft.Xna.Framework.Graphics.SpriteEffects.None,
                    0);
            //base.Render();
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
