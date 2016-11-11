
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ubeat.GameScreen
{
    public class ScreenBase : IScreen
    {
        private bool EscapeAlredyPressed;

        public Texture2D Background { get; set; }

        public List<ScreenUIObject> Controls { get; set; }

        public float Opacity { get; set; }

        public IScreen ScreenInstance { get; set; }

        public bool Visible { get; set; }

        public string Name { get; set; }

        public event EventHandler OnLoad;

        public event EventHandler OnBackSpacePress;

        public ScreenBase(string name = "BaseScreen")
        {
            Name = name;
            OnLoad += _OnLoad;
        }

        public virtual void Redraw()
        {
            
        }

        public virtual void Render()
        {
            if (!Visible) return;

            if (Background != null)
            {
                int screenWidth = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
                int screenHeight = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;

                Rectangle screenRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)Background.Width / (float)Background.Height) * (float)screenHeight), screenHeight);

                UbeatGame.Instance.spriteBatch.Draw(Background, screenRectangle, null, Color.White, 0, new Vector2(Background.Width / 2, Background.Height / 2), SpriteEffects.None, 0);
            }

            foreach (ScreenUIObject obj in Controls)
                obj.Render();
        }

        public virtual void Update(GameTime tm)
        {
            if (!Visible) return;

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                EscapeAlredyPressed = true;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Escape))
            {
                if (EscapeAlredyPressed)
                {
                    EscapeAlredyPressed = false;
                    OnBackSpacePress?.Invoke(this, new EventArgs());
                }
            }

            foreach (ScreenUIObject obj in Controls)
                obj.Update();
        }

        public void BackPressed(IScreen screen)
        {
            ScreenManager.ChangeTo(screen);
        }        

        public void OnLoadScreen()
        {
            OnLoad?.Invoke(this, new EventArgs());
        }

        private void _OnLoad(object sender, EventArgs e)
        {

#if DEBUG
            Logger.Instance.Debug($"{this.Name} loaded");
#endif
        }

        private void _OnUnload(object sender, EventArgs e)
        {
#if DEBUG
            Logger.Instance.Debug($"{this.Name} unloaded");
#endif
        }
    }
}