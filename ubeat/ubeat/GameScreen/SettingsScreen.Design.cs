using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using ubeat.Screen;

namespace ubeat.GameScreen
{
    public partial class SettingsScreen : IScreen
    {
        public void LoadInterface()
        {
            Controls = new List<ScreenUIObject>();

            var modes = ScreenModeManager.GetSupportedModes();
            var actualMode = modes[Settings1.Default.ScreenMode];

            var center = new Vector2(actualMode.Width / 2, actualMode.Height / 2);

            Logo = new UI.Image(Game1.Instance.Logo) { BeatReact = true };

            Logo.Position = 
                    new Vector2(
                        center.X - (Logo.Texture.Width / 2),
                        center.Y - (Logo.Texture.Height / 2) - Logo.Texture.Height + 15);

            Controls.Add(Logo);

            Game1.Instance.IsMouseVisible = true;

            OnLoad?.Invoke(this, new EventArgs());
        }

        public void Redraw()
        {
            var modes = ScreenModeManager.GetSupportedModes();
            var actualMode = modes[Settings1.Default.ScreenMode];

            var center = new Vector2(actualMode.Width / 2, actualMode.Height / 2);

            Logo.Position = new Vector2(center.X - (Logo.Texture.Width / 2),
               center.Y - (Logo.Texture.Height / 2) - Logo.Texture.Height + 15);
        }

        public void Render()
        {
            if (!Visible) return;

            if (Background != null)
            {
                int screenWidth = Game1.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
                int screenHeight = Game1.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;

                //Rectangle screenRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, screenWidth, (int)(((float)Background.Height / (float)Background.Width) * (float)screenWidth));
                Rectangle screenRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)Background.Width / (float)Background.Height) * (float)screenHeight), screenHeight);

                Game1.Instance.spriteBatch.Draw(Background, screenRectangle, null, Color.White, 0, new Vector2(Background.Width / 2, Background.Height / 2), SpriteEffects.None, 0);
            }

            foreach (var ctr in Controls)
                ctr.Render();
        }

        public void Update(GameTime tm)
        {
            if (!Visible) return;

            var keyboardState = Keyboard.GetState();
            var newMouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.Back) || keyboardState.IsKeyDown(Keys.Escape))
                onBackspacePressed();

            foreach (var ctr in Controls)
                ctr.Update();
        }

        public IScreen ScreenInstance { get; set; }
        public List<ScreenUIObject> Controls { get; set; }
        public bool Visible { get; set; }

        #region UI

        public Texture2D Background;
        public UI.Image Logo;

        #endregion

        #region Events

        public event EventHandler OnLoad;

        #endregion
    }
}
