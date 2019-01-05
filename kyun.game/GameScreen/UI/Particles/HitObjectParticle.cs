using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace kyun.GameScreen.UI.Particles
{
    public class HitObjectParticle : Particle
    {
        Vector2 startPos = Vector2.Zero;

        public HitObjectParticle(Texture2D tx, Vector2 velocity, Vector2 startUpPosition, int timeToDie, float angleVelocity) : base(tx, velocity, startUpPosition, timeToDie, angleVelocity)
        {
            StopAtBottom = false;
            Scale = 1f;
            Opacity = .2f;
            startPos = startUpPosition;
        }

        public override void Update()
        {
            float positionY = 0;
            float positionX = 0;

            Vector2 VelocityP = new Vector2(Velocity.X, Velocity.Y);

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


            
            if(VelocityP.X == 0)
            {
                Opacity -= KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 0.001f;
            }
            else
            {
                Opacity -= KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 0.001f * Math.Max(0.01f, VelocityP.Y);
            }
            

            Scale += KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 0.0005f;

            //Position = new Vector2(startPos.X - ((Texture.Width * Scale) - Texture.Width)/2, startPos.Y - ((Texture.Height * Scale) - Texture.Height)/2);
            
            //OriginRender = new Vector2(Scale);

            //Position -= new Vector2(0.025f, 0);

            if (Position.Y < (-Size.X * Scale) * 2 || Opacity < 0.001)
            {
                TimeToDie = 0;
            }
        }
    }
}
