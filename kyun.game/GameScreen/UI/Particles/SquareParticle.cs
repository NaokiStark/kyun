using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace kyun.GameScreen.UI.Particles
{
    public class SquareParticle : Particle
    {

        public Color squareColor;
        public SquareParticle(Texture2D tx, Vector2 velocity, Vector2 startUpPosition, int timeToDie, float angleVelocity, Color pColor) : base(tx, velocity, startUpPosition, timeToDie, angleVelocity)
        {
            StopAtBottom = false;

            squareColor = pColor;
            
        }

        public override void Update()
        {
            //Events?
            //base.Update();

            //Opacity = 1;
            //TimeToDie -= UbeatGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;

            //Opacity -= UbeatGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 0.00015f;

            float positionY = 0;
            float positionX = 0;

            Vector2 VelocityP = new Vector2(Velocity.X * KyunGame.Instance.maxPeak, Math.Max((Math.Abs(Scale - 1)/1.1f) * KyunGame.Instance.maxPeak, 0.05f));

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


            Position -= new Vector2(positionX, positionY);

            if(Position.Y < (-Texture.Height * Scale) * 2)
            {
                TimeToDie = 0;
            }
            if (StopAtBottom)
            {
                if (positionY != 0)
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

            // NO
            //AngleRotation = 0.785398f;

            //SI C:
            AngleRotation = (float)Math.PI * 0.25f;


            //            SourceRectangle = new Rectangle(0, 0, (int)(Texture.Width * Scale), (int)(Texture.Height * Scale));
            //OriginRender = new Vector2((Texture.Width * Scale) / 2, (Texture.Height * Scale) / 2);
        }
        public override void Render()
        {
            Rectangle rg = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)(this.Texture.Width * Scale), (int)(this.Texture.Height * Scale));
            SourceRectangle = new Rectangle(SourceRectangle.X, SourceRectangle.Y, (int)(SourceRectangle.Width * Scale), (int)(SourceRectangle.Height * Scale));
            if (SourceRectangle != Rectangle.Empty)
                KyunGame.Instance.SpriteBatch.Draw(this.Texture, rg, SourceRectangle, squareColor * Opacity, AngleRotation, OriginRender, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            else
                KyunGame.Instance.SpriteBatch.Draw(this.Texture, rg, null, squareColor * Opacity, AngleRotation, OriginRender, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
        }
    }
}
