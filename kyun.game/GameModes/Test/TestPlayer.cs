using kyun.GameScreen;
using kyun.GameScreen.UI;
using kyun.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

//Ignore hardcoded values
namespace kyun.game.GameModes.Test
{
    public class TestPlayer : UIObjectBase
    {
        public Image playerObj;

        public bool IsJumping;

        private bool jdown;

        private int jumpRandom = 50;

        private int maxLimit = 300;

        public float Qa {
            get
            {
                return qa;
            }
            set
            {
                qa = Math.Max(Math.Min(value, 2), .5f);
            }
        }

        float qa = 0;


        public new Vector2 Position
        {
            get
            {
                return playerObj.Position;
            }
            set
            {
                playerObj.Position = value;
            }
        }

        public Vector2 playerSize = new Vector2(100);
        private bool pressedInThisframe;

        public TestPlayer()
        {
            //playerObj = new FilledRectangle(playerSize, Color.White);
            playerObj = new Image(SpritesContent.Instance.Catcher);
            playerObj.Size = playerSize;
            playerObj.Position = new Vector2(10, ScreenMode.Height - playerSize.Y - 30);
        }

        public override void Update()
        {
            base.Update(); //
            updatePhysics();
            playerObj.Update();
        }

        private void updatePhysics()
        {
            pressedInThisframe = false;
            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.Left))
                move();
            else if (kb.IsKeyDown(Keys.Right))
                move(true);

            if (kb.IsKeyDown(Keys.Up))
            {
                /*
                IsJumping = true;
                if (Position.Y >= Position.Y - maxLimit)
                {
                    playerObj.OriginRender = playerSize / 2;
                    jumpRandom += (int)(.5f * Elapsed.Milliseconds);
                    jumpRandom = Math.Min(jumpRandom, maxLimit);
                }*/
                pressedInThisframe = true;
                Qa += Elapsed.Milliseconds * .01f;
                Position = new Vector2(Position.X, Math.Max(Position.Y - .5f * Elapsed.Milliseconds * Qa, 0));

            }
            else if (kb.IsKeyDown(Keys.Down))
            {
                pressedInThisframe = true;
                Qa += Elapsed.Milliseconds * .01f;
                Position = new Vector2(Position.X, Math.Min(Position.Y + .5f * Elapsed.Milliseconds * Qa, ScreenMode.Height - playerSize.Y));
            }

            if (!pressedInThisframe)
            {
                Qa = 0;
            }

            //updateJump();
        }

        private void updateJump()
        {
            if (/*!IsJumping*/true)
                return;

            playerObj.AngleRotation += (float)Math.PI / 2 * (.01f * Elapsed.Milliseconds);

            if (jdown)
            {
                Position = new Vector2(Position.X, Math.Min(Position.Y + .5f * Elapsed.Milliseconds, ScreenMode.Height - 30));
                if (Position.Y >= ScreenMode.Height - playerSize.Y - 30)
                {
                    IsJumping = false;
                    jdown = false;
                    jumpRandom = 50;
                    playerObj.AngleRotation = 0;
                    playerObj.OriginRender = Vector2.Zero;
                }
            }
            else
            {
                Position = new Vector2(Position.X, Math.Max(Position.Y - .5f * Elapsed.Milliseconds, ScreenMode.Height - jumpRandom - playerSize.Y));
                if (Position.Y <= ScreenMode.Height - jumpRandom - playerSize.Y)
                {
                    jdown = true;
                }
            }
        }

        public override void Render()
        {
            playerObj.Render();
        }

        private void move(bool rigth = false)
        {
            pressedInThisframe = true;
            Qa += Elapsed.Milliseconds * .01f;
            Position = new Vector2((rigth) ? Math.Min(Position.X + .5f * Elapsed.Milliseconds * Qa, ScreenMode.Width - playerSize.X) : Math.Max(Position.X - .5f * Elapsed.Milliseconds * Qa, 0), Position.Y);
        }
    }
}
