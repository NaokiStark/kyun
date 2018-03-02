using kyun.GameScreen;
using kyun.GameScreen.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

//Ignore hardcoded values
namespace kyun.game.GameModes.Test
{
    public class TestPlayer : UIObjectBase
    {
        private FilledRectangle playerObj;

        public bool IsJumping;

        private bool jdown;

        private int jumpRandom = 50;

        private int maxLimit = 300;

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

        public TestPlayer()
        {
            playerObj = new FilledRectangle(new Vector2(20), Color.White);
            playerObj.Position = new Vector2(10, ScreenMode.Height - 20);
        }

        public override void Update()
        {
            base.Update(); //
            updatePhysics();
            playerObj.Update();
        }

        private void updatePhysics()
        {
            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.Left))
                move();
            else if (kb.IsKeyDown(Keys.Right))
                move(true);

            if (kb.IsKeyDown(Keys.Up))
            {
                IsJumping = true;
                if (Position.Y >= Position.Y - maxLimit)
                {
                    playerObj.OriginRender = new Vector2(10, 10);
                    jumpRandom += (int)(.5f * Elapsed.Milliseconds);
                    jumpRandom = Math.Min(jumpRandom, maxLimit);
                }
            }

            updateJump();
        }

        private void updateJump()
        {
            if (!IsJumping)
                return;

            playerObj.AngleRotation += (float)Math.PI / 2 * (.01f * Elapsed.Milliseconds);

            if (jdown)
            {
                Position = new Vector2(Position.X, Math.Min(Position.Y + .5f * Elapsed.Milliseconds, ScreenMode.Height - 20));
                if (Position.Y >= ScreenMode.Height - 20)
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
                Position = new Vector2(Position.X, Math.Max(Position.Y - .5f * Elapsed.Milliseconds, ScreenMode.Height - jumpRandom - 20));
                if (Position.Y <= ScreenMode.Height - jumpRandom - 20)
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
            Position = new Vector2((rigth) ? Math.Min(Position.X + .5f * Elapsed.Milliseconds, ScreenMode.Width - 20) : Math.Max(Position.X - .5f * Elapsed.Milliseconds, 0), Position.Y);
        }
    }
}
