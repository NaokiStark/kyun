using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using ubeat.Screen;
using ubeat.GameScreen.SUI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ubeat.GameScreen.UI;
using ubeat.GameScreen.SUI.Buttons;

namespace ubeat.GameScreen
{
    public partial class BeatmapScreen : IScreen
    {

        public IScreen ScreenInstance { get; set; }

        public List<ScreenUIObject> Controls { get; set; }
        public event EventHandler OnLoad;
        Listbox lbox;
        ListboxDiff lBDff;
        FilledRectangle filledRect1;
        Label lblTitleDesc = new Label();
        AutoModeButton autoBtn;

        bool AMode = false;

        public Texture2D Background { get; set; }

        public void LoadInterface()
        {
            UbeatGame.Instance.IsMouseVisible = true;
            Controls = new List<ScreenUIObject>();

            ScreenMode actualMode = ScreenModeManager.GetActualMode();

            Vector2 lPos = new Vector2(0, 100);

            lbox = new Listbox(lPos, actualMode.Width / 2, actualMode.Height - 100, UbeatGame.Instance.ListboxFont);
            
            lbox.IndexChanged += lbox_IndexChanged;
            
            lBDff = new ListboxDiff(new Vector2(lbox.width, lbox.Position.Y), 200, 500, UbeatGame.Instance.ListboxFont);

            filledRect1 = new FilledRectangle(new Vector2(actualMode.Width, 4), Color.SpringGreen);
            filledRect1.Position = new Vector2(0, 96);

            lblTitleDesc = new Label(.98f) { 
                Scale=1.2f,
                Text="",
                Position = new Vector2(0,0),
                Size = new Vector2(actualMode.Width, 96)
            };

            lblSearch = new Label(.98f)
            {
                Scale = .8f,
                Text = "",
                Position = new Vector2(actualMode.Width/2, 48), 
                Centered=true
            };

            autoBtn = new AutoModeButton() {
                Position = new Vector2(actualMode.Width - UbeatGame.Instance.AutoModeButton.Width-20, actualMode.Height - UbeatGame.Instance.AutoModeButton.Height-20),
                Scale=.85f,
                PlayHit=true
            };

            autoBtn.Click += autoBtn_Click;

            Controls.Add(lbox);
            Controls.Add(lBDff);
            Controls.Add(filledRect1);
            Controls.Add(lblTitleDesc);
            Controls.Add(autoBtn);
            Controls.Add(lblSearch);

            OnLoad += BeatmapScreen_OnLoad;

            OnLoad?.Invoke(this, new EventArgs());
        }

        void autoBtn_Click(object sender, EventArgs e)
        {
            if (AMode)
            {
                AMode = false;
                autoBtn.Texture = UbeatGame.Instance.AutoModeButton;
            }
            else
            {
                AMode = true;
                autoBtn.Texture = UbeatGame.Instance.AutoModeButtonSel;
            }
        }



        bool EscapeAlredyPressed = false;

        public void Update(GameTime tm)
        {

            if (!Visible) return;

            if(Keyboard.GetState().IsKeyDown(Keys.Escape)){
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

            foreach(ScreenUIObject ctr in Controls)
                ctr.Update();

        }

        void backPressed()
        {
            ScreenManager.ChangeTo(new MainScreen(false));
        }
       
        public void Render()
        {
            if (!Visible) return;

            if (Background != null)
            {
                int screenWidth = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
                int screenHeight = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;

                //Rectangle screenRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, screenWidth, (int)(((float)Background.Height / (float)Background.Width) * (float)screenWidth));
                Rectangle screenRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)Background.Width / (float)Background.Height) * (float)screenHeight), screenHeight);

                UbeatGame.Instance.spriteBatch.Draw(Background, screenRectangle, null, Color.White, 0, new Vector2(Background.Width / 2, Background.Height / 2), SpriteEffects.None, 0);
            }
            
            foreach (ScreenUIObject ctr in Controls)
                ctr.Render();

        }

        public void Redraw()
        {
            //Called when resize
        }

        public bool Visible { get; set; }

        public Label lblSearch { get; set; }
    }
}