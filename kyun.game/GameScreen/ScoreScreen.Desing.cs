using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using kyun.GameScreen.UI;
using kyun.Screen;
using kyun.Utils;
using kyun.GameScreen.UI.Buttons;

namespace kyun.GameScreen
{
    public partial class ScoreScreen: ScreenBase
    {
        private ButtonStandard backButton;

        public void LoadInterface()
        {

            //Controls = new HashSet<UIObjectBase>();

            ScreenMode ActualMode = ScreenModeManager.GetActualMode();


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
                Font = SpritesContent.Instance.GeneralBig,
                Centered = true
            };

            lblPerfect = new Label() {
                Scale = .95f,
                Text = "",
                Position = new Vector2(center.X, (lblScore.Position.Y + spaced) + 5f),
                Font = SpritesContent.Instance.GeneralBig,
                Centered = true
            };

            lblExcellent = new Label() {
                Scale = .95f,
                Text = "",
                Position = new Vector2(center.X, (lblPerfect.Position.Y + spaced) + 5f),
                Font = SpritesContent.Instance.GeneralBig,
                Centered = true
            };

            lblGood = new Label() {
                Scale = .95f,
                Text = "",
                Position = new Vector2(center.X, (lblExcellent.Position.Y + spaced) + 5f),
                Font = SpritesContent.Instance.GeneralBig,
                Centered = true
            };
            lblMiss = new Label() {
                Scale = .95f,
                Text = "",
                Position = new Vector2(center.X, (lblGood.Position.Y + spaced) + 5f),
                Font = SpritesContent.Instance.GeneralBig,
                Centered = true
            };
            lblAccuracy = new Label() {
                Scale = .95f,
                Text = "",
                Position = new Vector2(center.X, (lblMiss.Position.Y + spaced) + 5f),
                Font = SpritesContent.Instance.GeneralBig,
                Centered = true
            };
            lblCombo = new Label() {
                Scale = .95f,
                Text = "",
                Position = new Vector2(center.X, (lblAccuracy.Position.Y + spaced) + 5f),
                Font = SpritesContent.Instance.GeneralBig,
                Centered = true
            };

            backButton = new ButtonStandard(Color.DarkRed)
            {
                ForegroundColor = Color.White,
                Caption = "Back",
                Position = new Vector2(15, ((ActualMode.Height - 100) + 100 / 2) - (SpritesContent.Instance.ButtonStandard.Height / 2)),
            };

            backButton.Click += BackButton_Click;

            Controls.Add(filledRect1);
            Controls.Add(lblTitleDesc);
            Controls.Add(lblScore);
            Controls.Add(lblPerfect);
            Controls.Add(lblExcellent);
            Controls.Add(lblGood);
            Controls.Add(lblMiss);
            Controls.Add(lblAccuracy);
            Controls.Add(lblCombo);
            Controls.Add(backButton);

            
            OnLoad += ScoreScreen_OnLoad;

            OnBackSpacePress += (sender, args)=>{
                BackPressed(BeatmapScreen.Instance);
            };

            OnLoadScreen();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            BackPressed(BeatmapScreen.Instance);
        }

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
