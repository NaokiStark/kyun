using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using ubeat.GameScreen.UI.Buttons;
using ubeat.Screen;

namespace ubeat.GameScreen
{
    //Here goes Desing
    public partial class MainScreen : IScreen
    {
        public void LoadInterface()
        {
            Controls = new List<ScreenUIObject>();

            ScreenMode ActualMode = ScreenModeManager.GetActualMode();

            Vector2 center = new Vector2(ActualMode.Width / 2, ActualMode.Height / 2);

            Logo = new UI.Image(Game1.Instance.Logo) { BeatReact = true };
            Logo.Position = new Vector2(center.X - (Logo.Texture.Width / 2),
                center.Y - (Logo.Texture.Height / 2) - Logo.Texture.Height + 15);

            CnfBtn = new ConfigButton();
            CnfBtn.Position = new Vector2(center.X - (CnfBtn.Texture.Width / 2),
                center.Y - (CnfBtn.Texture.Height / 2) + Logo.Texture.Height / 2);

            StrBtn = new StartButton();
            StrBtn.Position = new Vector2(center.X - (StrBtn.Texture.Width / 2),
                (center.Y - (StrBtn.Texture.Height / 2) - StrBtn.Texture.Height - 2) + Logo.Texture.Height / 2);

            ExtBtn = new ExitButton();
            ExtBtn.Position = new Vector2(center.X - (ExtBtn.Texture.Width / 2),
                center.Y - (ExtBtn.Texture.Height / 2) + ExtBtn.Texture.Height + 2 + Logo.Texture.Height / 2);

            Vector2 meas = Game1.Instance.defaultFont.MeasureString("ubeat") * .85f;

            Label1 = new UI.Label();
            Label1.Text = "Hello world";
            Label1.Scale = .85f;
            Label1.Position = new Vector2(0);

            FPSMetter = new UI.Label();
            FPSMetter.Text = "0";
            FPSMetter.Scale = .75f;
            FPSMetter.Position = new Vector2(0, ActualMode.Height - meas.Y);


            Controls.Add(CnfBtn);
            Controls.Add(StrBtn);
            Controls.Add(ExtBtn);
            Controls.Add(Logo);
            Controls.Add(Label1);
            Controls.Add(FPSMetter);

            Game1.Instance.IsMouseVisible = true;
            OnLoad += MainScreen_OnLoad;

            OnLoad?.Invoke(this, new EventArgs());
        }


        public void Update(GameTime tm)
        {

            //FPSMetter.Text = Game1.Instance.frameCounter.AverageFramesPerSecond.ToString("0", CultureInfo.InvariantCulture) +" FPS";

            int fps = (int)Math.Round(Game1.Instance.frameCounter.AverageFramesPerSecond, 0);

            FPSMetter.Text = string.Join(" ", fps.ToString("0", CultureInfo.InvariantCulture), "FPS");

            if (Visible)
                foreach (ScreenUIObject ctr in Controls)
                    ctr.Update();
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

        public void Redraw()
        {
            ScreenMode ActualMode = ScreenModeManager.GetActualMode();

            Vector2 center = new Vector2(ActualMode.Width / 2, ActualMode.Height / 2);

            Logo.Position = new Vector2(center.X - (Logo.Texture.Width / 2),
               center.Y - (Logo.Texture.Height / 2) - Logo.Texture.Height + 15);

            CnfBtn.Position = new Vector2(center.X - (CnfBtn.Texture.Width / 2),
                center.Y - (CnfBtn.Texture.Height / 2) + Logo.Texture.Height / 2);

            StrBtn.Position = new Vector2(center.X - (StrBtn.Texture.Width / 2),
                (center.Y - (StrBtn.Texture.Height / 2) - StrBtn.Texture.Height - 2) + Logo.Texture.Height / 2);

            ExtBtn.Position = new Vector2(center.X - (ExtBtn.Texture.Width / 2),
                center.Y - (ExtBtn.Texture.Height / 2) + ExtBtn.Texture.Height + 2 + Logo.Texture.Height / 2);
        }

        #region Properties

        public bool Visible { get; set; }
        public List<ScreenUIObject> Controls { get; set; }

        #endregion

        #region UI

        public ConfigButton CnfBtn;
        public StartButton StrBtn;
        public ExitButton ExtBtn;
        public Texture2D Background;
        public UI.Image Logo;
        public UI.Label Label1;
        public UI.Label FPSMetter;

        #endregion

        #region Events

        public event EventHandler OnLoad;

        #endregion
    }
}
