using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ubeat.GameScreen.SUI;
using ubeat.GameScreen.UI;
using ubeat.Screen;

namespace ubeat.GameScreen
{
    public partial class ScoreScreen:Screen
    {
        public Screen ScreenInstance { get; set; }

        public List<ScreenUIObject> Controls { get; set; }

        public Texture2D Background { get; set; }
        public event EventHandler OnLoad;

        public void LoadInterface()
        {
            Game1.Instance.IsMouseVisible = true;
            Controls = new List<ScreenUIObject>();
            List<ScreenMode> scmL = ScreenModeManager.GetSupportedModes();
            ScreenMode ActualMode = scmL[Settings1.Default.ScreenMode];

            filledRect1 = new FilledRectangle(new Vector2(ActualMode.Width, 4), Color.SpringGreen);
            filledRect1.Position = new Vector2(0, 96);

            lblTitleDesc = new Label(.98f)
            {
                Scale = 1.2f,
                Text = "",
                Position = new Vector2(0, 0),
                Size = new Vector2(ActualMode.Width, 96)
            };

            Vector2 center = new Vector2(ActualMode.Width/2, ActualMode.Height/2);
            int spaced = 50;
            lblScore = new Label()
            {
                Scale = 1.1f,
                Text = "",
                Position = new Vector2(center.X, 120),
                Font = Game1.Instance.GeneralBig,
                Centered = true
            };

            lblPerfect = new Label() {
                Scale = .95f,
                Text = "",
                Position = new Vector2(center.X, (lblScore.Position.Y + spaced) + 5f),
                Font = Game1.Instance.GeneralBig,
                Centered = true
            };

            lblExcellent = new Label() {
                Scale = .95f,
                Text = "",
                Position = new Vector2(center.X, (lblPerfect.Position.Y + spaced) + 5f),
                Font = Game1.Instance.GeneralBig,
                Centered = true
            };

            lblGood = new Label() {
                Scale = .95f,
                Text = "",
                Position = new Vector2(center.X, (lblExcellent.Position.Y + spaced) + 5f),
                Font = Game1.Instance.GeneralBig,
                Centered = true
            };
            lblMiss = new Label() {
                Scale = .95f,
                Text = "",
                Position = new Vector2(center.X, (lblGood.Position.Y + spaced) + 5f),
                Font = Game1.Instance.GeneralBig,
                Centered = true
            };
            lblAccuracy = new Label() {
                Scale = .95f,
                Text = "",
                Position = new Vector2(center.X, (lblMiss.Position.Y + spaced) + 5f),
                Font = Game1.Instance.GeneralBig,
                Centered = true
            };
            lblCombo = new Label() {
                Scale = .95f,
                Text = "",
                Position = new Vector2(center.X, (lblAccuracy.Position.Y + spaced) + 5f),
                Font = Game1.Instance.GeneralBig,
                Centered = true
            };
            
            Controls.Add(filledRect1);
            Controls.Add(lblTitleDesc);
            Controls.Add(lblScore);
            Controls.Add(lblPerfect);
            Controls.Add(lblExcellent);
            Controls.Add(lblGood);
            Controls.Add(lblMiss);
            Controls.Add(lblAccuracy);
            Controls.Add(lblCombo);


            OnLoad += ScoreScreen_OnLoad;
            if (OnLoad != null)
                OnLoad(this, new EventArgs());
        }

        public void Update(Microsoft.Xna.Framework.GameTime tm)
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
                    backPressed();
                }
            }

            foreach (ScreenUIObject ctr in Controls)
                ctr.Update();
        }

        void backPressed()
        {
            ScreenManager.ChangeTo(new BeatmapScreen());
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

            foreach (ScreenUIObject ctr in Controls)
                ctr.Render();
        }

        public void Redraw()
        {

        }

        public bool Visible { get; set; }


        public FilledRectangle filledRect1 { get; set; }

        public Label lblTitleDesc { get; set; }

        public bool EscapeAlredyPressed { get; set; }

        public Label lblPerfect { get; set; }

        public Label lblExcellent { get; set; }

        public Label lblGood { get; set; }

        public Label lblMiss { get; set; }

        public Label lblAccuracy { get; set; }

        public Label lblCombo { get; set; }

        public Label lblScore { get; set; }
    }
}
