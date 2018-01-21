using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kyun.GameScreen.UI.Particles
{
    public class Particle : UIObjectBase
    {
        
        /// <summary>
        /// Particle Velocity (Readonly)
        /// </summary>
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// Time to particle dies
        /// </summary>
        public float TimeToDie { get; set; }

        public float AngleVel { get; set; }

        public bool StopAtBottom { get; set; }

        /// <summary>
        /// New instance of Particle
        /// </summary>
        public Particle(Texture2D tx, Vector2 velocity, Vector2 startUpPosition, int timeToDie, float angleVelocity)
        {
            Texture = tx;
            Position = startUpPosition;
            Velocity = velocity;
            Opacity = 0.75f;
            TimeToDie = timeToDie;
            AngleVel = angleVelocity;
        }

        public override void Update()
        {
            //Events?
            base.Update();


            TimeToDie -= KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;

            Opacity -= KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 0.00015f;

            float positionY = 0;
            float positionX = 0;

            Vector2 VelocityP = new Vector2(Velocity.X * KyunGame.Instance.Player.PeakVol, Velocity.Y * KyunGame.Instance.Player.PeakVol);

            if (StopAtBottom)
            {
                if ((KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * VelocityP.Y) + Position.Y > Screen.ScreenModeManager.GetActualMode().Height - (Texture.Height * Scale))
                {
                    positionY = 0;
                    positionX = 0;
                }
                else
                {
                    positionY = KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * VelocityP.Y;
                    positionX = KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * VelocityP.X;
                }
            }
            else
            {
                positionY = KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * VelocityP.Y;
                positionX = KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * VelocityP.X;
            }

            
            Position += new Vector2(positionX, positionY);
            if(StopAtBottom)
            {
                if(positionY != 0)
                {
                    AngleRotation += (KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * AngleVel);
                }
                else
                {
                    AngleRotation = 0;
                }
                
            }
            else
            {
                AngleRotation += (KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * AngleVel);
            }
            

//            SourceRectangle = new Rectangle(0, 0, (int)(Texture.Width * Scale), (int)(Texture.Height * Scale));
            OriginRender = new Vector2((Texture.Width * Scale) / 2, (Texture.Height * Scale) / 2);
        }

    }
}
