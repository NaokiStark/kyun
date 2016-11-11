using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using ubeat.GameScreen.SUI;
using ubeat.Screen;


namespace ubeat.GameScreen
{
    public partial class SettingsScreen : ScreenBase
    {

        int globalMarginBottom = 30;

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

            comboLang = new ComboBox(new Vector2(
                center.X, Logo.Position.Y + globalMarginBottom + Logo.Texture.Height),
                250,
                UbeatGame.Instance.ListboxFont);


            comboLang.Text = "English";
            comboLang.Items.Add("English");
            comboLang.Items.Add("Español");

            //Combo display

            combodisplayMode = new ComboBox(
                new Vector2(
                    center.X,
                    comboLang.Position.Y + globalMarginBottom + UbeatGame.Instance.ListboxFont.MeasureString("a").Y + 5),
                250,
                UbeatGame.Instance.ListboxFont);

            fillComboDisplay();
            combodisplayMode.IndexChaged += CombodisplayMode_IndexChaged;

            filledRect1 = new FilledRectangle(
                new Vector2(actualMode.Width, actualMode.Height),
                Color.Black * 0.5f);

            filledRect1.Position = Vector2.Zero;

            OnBackSpacePress += (sender, args) =>
             {
                 BackPressed(new MainScreen(false));
             };
            
            Controls.Add(filledRect1);
            Controls.Add(Logo);
            
            Controls.Add(combodisplayMode);
            Controls.Add(comboLang);


            UbeatGame.Instance.IsMouseVisible = true;

            OnLoadScreen();
        }
        
        private void fillComboDisplay()
        {

            List<ScreenMode> scrnm = ScreenModeManager.GetSupportedModes();

            foreach (ScreenMode mode in scrnm)
            {
                combodisplayMode.Items.Add(string.Format("({0}x{1}){2}", mode.Width, mode.Height, (mode.WindowMode == Screen.WindowDisposition.Windowed) ? "[Windowed]" : ""));
            }

            combodisplayMode.Text = combodisplayMode.Items[Settings1.Default.ScreenMode].ToString();
        }

        public override void Redraw()
        {
            var modes = ScreenModeManager.GetSupportedModes();
            var actualMode = modes[Settings1.Default.ScreenMode];

            var center = new Vector2(actualMode.Width / 2, actualMode.Height / 2);

            Logo.Position = new Vector2(
               center.X - (Logo.Texture.Width / 2),
               center.Y - (Logo.Texture.Height / 2) - Logo.Texture.Height + 15);

            filledRect1 = null;///q

            filledRect1 = new FilledRectangle(
               new Vector2(actualMode.Width, actualMode.Height),
               Color.Black * 0.5f);

            filledRect1.Position = Vector2.Zero;

            comboLang.Position = new Vector2(center.X, Logo.Position.Y + globalMarginBottom + Logo.Texture.Height);

            combodisplayMode.Position = new Vector2(
                    center.X,
                    comboLang.Position.Y + globalMarginBottom + UbeatGame.Instance.ListboxFont.MeasureString("a").Y + 5);
        }


        #region UI

        public UI.Image Logo;
        private ComboBox combodisplayMode;
        private FilledRectangle filledRect1;

        public ComboBox comboLang { get; set; }

        #endregion

        #region Events



        #endregion
    }
}
