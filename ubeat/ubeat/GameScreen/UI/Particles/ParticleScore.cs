using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kyun.GameScreen.UI.Particles
{
    public class ParticleScore : Particle
    {
        public ParticleScore(Texture2D tx, Vector2 velocity, Vector2 startUpPosition, int timeToDie, float angleVelocity, Color pColor) : base(tx, velocity, startUpPosition, timeToDie, angleVelocity)
        {
            StopAtBottom = false;
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


          

            Opacity -= KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 0.001f;

            Scale += KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 0.0001f;

            Position -= new Vector2(0.025f, positionY);

            if (Position.Y < (-Texture.Height * Scale) * 2 || Opacity < 0.001)
            {
                TimeToDie = 0;
            }
        }

    }
}
