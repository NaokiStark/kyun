using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using ubeat.GameScreen.SUI;
using ubeat.Screen;


namespace ubeat.GameScreen
{
    public partial class SettingsScreen : IScreen
    {
        public void LoadInterface()
        {
            Controls = new List<ScreenUIObject>();

            //ñam ñam
            ScreenMode actualMode = ScreenModeManager.GetActualMode();

            var center = new Vector2(actualMode.Width / 2, actualMode.Height / 2);

            Logo = new UI.Image(UbeatGame.Instance.Logo) { BeatReact = true };

            Logo.Position = 
                    new Vector2(
                        center.X - (Logo.Texture.Width / 2),
                        center.Y - (Logo.Texture.Height / 2) - Logo.Texture.Height + 15);


            //Combobox xdd

            comboLang = new ComboBox(Vector2.Zero, 250, UbeatGame.Instance.defaultFont);
            comboLang.Text = "Engrish";
            comboLang.Items.Add("Engrish");
            comboLang.Items.Add("Espanyol");


            var filledRect1 = new FilledRectangle(new Vector2(actualMode.Width, actualMode.Height), Color.Black * 0.5f);
            filledRect1.Position = Vector2.Zero;

            
            Controls.Add(filledRect1);
            Controls.Add(Logo);
            Controls.Add(comboLang);

            UbeatGame.Instance.IsMouseVisible = true;

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
                int screenWidth = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
                int screenHeight = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;

                //Rectangle screenRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, screenWidth, (int)(((float)Background.Height / (float)Background.Width) * (float)screenWidth));
                var screenRectangle = new Microsoft.Xna.Framework.Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)Background.Width / (float)Background.Height) * (float)screenHeight), screenHeight);

                UbeatGame.Instance.spriteBatch.Draw(Background, screenRectangle, null, Color.White, 0, new Vector2(Background.Width / 2, Background.Height / 2), SpriteEffects.None, 0);
            }

            foreach (var ctr in Controls)
                ctr.Render();
        }

        public void Update(GameTime tm)
        {
            if (!Visible) return;

            var keyboardState = Keyboard.GetState();
            var newMouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Back) || keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                onBackspacePressed();

            foreach (var ctr in Controls)
                ctr.Update();
        }

        public IScreen ScreenInstance { get; set; }
        public List<ScreenUIObject> Controls { get; set; }
        public bool Visible { get; set; }

        #region UI

        public Texture2D Background { get; set; }
        public UI.Image Logo;
        public ComboBox comboLang { get; set; }

        #endregion

        #region Events

        public event EventHandler OnLoad;

        #endregion
    }
}
