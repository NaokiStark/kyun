using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using ubeat.GameScreen.SUI;
using ubeat.Screen;
using ubeat.GameScreen.UI;
using ubeat.Notifications;
using ubeat.Utils;

namespace ubeat.GameScreen
{
    public partial class SettingsScreen : ScreenBase
    {

        int globalMarginBottom = 15;

        public void LoadInterface()
        {
            Controls = new List<UIObjectBase>();

            //ñam ñam
            ScreenMode actualMode = ScreenModeManager.GetActualMode();

            var center = new Vector2(actualMode.Width / 2, actualMode.Height / 2);

            Logo = new UI.Image(SpritesContent.Instance.Logo) { BeatReact = true };

            Logo.Position = 
                    new Vector2(
                        center.X - (Logo.Texture.Width / 2),
                        center.Y - (Logo.Texture.Height / 2) - Logo.Texture.Height + 15);


            //Combobox xdd

            comboLang = new ComboBox(new Vector2(
                center.X, Logo.Position.Y + globalMarginBottom + Logo.Texture.Height),
                250,
                SpritesContent.Instance.SettingsFont);


            comboLang.Text = "English";
            comboLang.Items.Add("English");
            comboLang.Items.Add("Español");

            //Label Lang

            lcomboLang = new Label();
            lcomboLang.Text = "Language";
            lcomboLang.Font = SpritesContent.Instance.SettingsFont;
            lcomboLang.Size = SpritesContent.Instance.SettingsFont.MeasureString(lcomboLang.Text) + new Vector2(20,10);
            lcomboLang.Position = new Vector2(center.X - lcomboLang.Size.X - 10, comboLang.Position.Y);


            //Combo display

            combodisplayMode = new ComboBox(
                new Vector2(
                    center.X,
                    comboLang.Position.Y + globalMarginBottom + SpritesContent.Instance.SettingsFont.MeasureString("a").Y + 5),
                250,
                SpritesContent.Instance.SettingsFont);

            combodisplayMode.IndexChaged += CombodisplayMode_IndexChaged;
            fillComboDisplay();

            //Label display

            lcombodisplayMode = new Label();
            lcombodisplayMode.Text = "Display Mode";
            lcombodisplayMode.Font = SpritesContent.Instance.SettingsFont;
            lcombodisplayMode.Size = SpritesContent.Instance.SettingsFont.MeasureString(lcombodisplayMode.Text) + new Vector2(20,10);
            lcombodisplayMode.Position = new Vector2(center.X - lcombodisplayMode.Size.X - 10, combodisplayMode.Position.Y);


            // Combo Framerate

            comboFrameRate = new ComboBox(
                new Vector2(
                    center.X,
                    combodisplayMode.Position.Y + globalMarginBottom + SpritesContent.Instance.SettingsFont.MeasureString("a").Y + 5),
                250,
                SpritesContent.Instance.SettingsFont
                );

            comboFrameRate.Text = Settings1.Default.FrameRate.ToString();
            if (Settings1.Default.VSync)
            {
                comboFrameRate.Text = "60";
                comboFrameRate.Items.Add("60");
            }
            else
            {
                comboFrameRate.Items.Add("60");
                comboFrameRate.Items.Add("120");
                comboFrameRate.Items.Add("240");
            }            

            comboFrameRate.IndexChaged += ComboFrameRate_IndexChaged;

            //Label Framerate

            lcomboFrameRate = new Label();
            lcomboFrameRate.Text = "Framerate (FPS)";
            lcomboFrameRate.Font = SpritesContent.Instance.SettingsFont;
            lcomboFrameRate.Size = SpritesContent.Instance.SettingsFont.MeasureString(lcomboFrameRate.Text) + new Vector2(20,10);
            lcomboFrameRate.Position = new Vector2(center.X - lcomboFrameRate.Size.X - 10, comboFrameRate.Position.Y);

            // Check fullscreen

            checkFullScr = new CheckBox();
            checkFullScr.Position = new Vector2(
                    center.X,
                    comboFrameRate.Position.Y + globalMarginBottom + SpritesContent.Instance.SettingsFont.MeasureString("a").Y + 5);

            checkFullScr.Checked = Settings1.Default.FullScreen;
            checkFullScr.CheckChanged += CheckFullScr_CheckChanged;

            //Label Fullscreen

            lcheckFullScr = new Label();
            lcheckFullScr.Text = "Fullscreen";
            lcheckFullScr.Font = SpritesContent.Instance.SettingsFont;
            lcheckFullScr.Size = SpritesContent.Instance.SettingsFont.MeasureString(lcheckFullScr.Text) + new Vector2(20,10);
            lcheckFullScr.Position = new Vector2(center.X - lcheckFullScr.Size.X - 10, checkFullScr.Position.Y);

            // Check VSync

            checkVSync = new CheckBox();
            checkVSync.Position = new Vector2(
                    center.X,
                    checkFullScr.Position.Y + globalMarginBottom + SpritesContent.Instance.SettingsFont.MeasureString("a").Y + 5);

            checkVSync.Checked = Settings1.Default.VSync;
            checkVSync.CheckChanged += CheckVSync_CheckChanged;

            //Label VSync

            lcheckVSync = new Label();
            lcheckVSync.Text = "VSync (Fix framerate to 60FPS)";
            lcheckVSync.Font = SpritesContent.Instance.SettingsFont;
            lcheckVSync.Size = SpritesContent.Instance.SettingsFont.MeasureString(lcheckVSync.Text) + new Vector2(20,10);
            lcheckVSync.Position = new Vector2(center.X - lcheckVSync.Size.X - 10, checkVSync.Position.Y);

            //Check InGame Video

            checkInGameVideo = new CheckBox();
            checkInGameVideo.Position = new Vector2(
                    center.X,
                    checkVSync.Position.Y + globalMarginBottom + SpritesContent.Instance.SettingsFont.MeasureString("a").Y + 5);
            checkInGameVideo.Checked = Settings1.Default.Video;

            checkInGameVideo.CheckChanged += CheckInGameVideo_CheckChanged;

            //Label VSync

            lcheckInGameVideo = new Label();
            lcheckInGameVideo.Text = "In-Game Video (Warn!)";
            lcheckInGameVideo.Font = SpritesContent.Instance.SettingsFont;
            lcheckInGameVideo.Size = SpritesContent.Instance.SettingsFont.MeasureString(lcheckInGameVideo.Text) + new Vector2(20,10);
            lcheckInGameVideo.Position = new Vector2(center.X - lcheckInGameVideo.Size.X - 10, checkInGameVideo.Position.Y);

            notifier = new Notifier();

            //Filled Rectangle

            filledRect1 = new FilledRectangle(
                new Vector2(actualMode.Width, actualMode.Height),
                Color.Black * 0.5f);

            filledRect1.Position = Vector2.Zero;

            OnBackSpacePress += (sender, args) =>
             {
                 //BackPressed(new MainScreen(false));
                 BackPressed(MainScreen.Instance);
             };      



            Controls.Add(filledRect1);
            Controls.Add(Logo);

            //Add controls 
            Controls.Add(lcheckInGameVideo);
            Controls.Add(checkInGameVideo);
            Controls.Add(lcheckVSync);
            Controls.Add(checkVSync);
            Controls.Add(lcheckFullScr);
            Controls.Add(checkFullScr);
            Controls.Add(lcomboFrameRate);
            Controls.Add(comboFrameRate);
            Controls.Add(lcombodisplayMode);
            Controls.Add(combodisplayMode);
            Controls.Add(lcomboLang);
            Controls.Add(comboLang);
            Controls.Add(notifier);

            UbeatGame.Instance.IsMouseVisible = true;

            OnLoadScreen();
        }

        private void CheckInGameVideo_CheckChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked){
                if(System.Windows.MessageBox.Show("This feature uses a lot of resources, is recommended for multicore processors and up to 2GB of RAM\r\n\r\nAre you sure you want to apply this change?", "ubeat", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.Yes)
                {
                    Logger.Instance.Info("Setting Video: " + ((CheckBox)sender).Checked.ToString());
                    UbeatGame.Instance.VideoEnabled = ((CheckBox)sender).Checked;
                    Settings1.Default.Video = ((CheckBox)sender).Checked;
                    Settings1.Default.Save();
                }
                else
                {
                    ((CheckBox)sender).Checked = false;
                }
            }
            else
            {
                Logger.Instance.Info("Setting Video: " + ((CheckBox)sender).Checked.ToString());
                UbeatGame.Instance.VideoEnabled = ((CheckBox)sender).Checked;
                Settings1.Default.Video = ((CheckBox)sender).Checked;
                Settings1.Default.Save();
            }
            
        }

        private void CheckVSync_CheckChanged(object sender, EventArgs e)
        {
            UbeatGame.Instance.ToggleVSync(((CheckBox)sender).Checked);
            Logger.Instance.Info("Setting VSync: " + ((CheckBox)sender).Checked.ToString());
            Settings1.Default.VSync = ((CheckBox)sender).Checked;
            Settings1.Default.Save();

            if (((CheckBox)sender).Checked)
            {
                comboFrameRate.Text = "60";
                comboFrameRate.Items.Clear();
                comboFrameRate.Items.Add("60");
            }
            else
            {
                comboFrameRate.Items.Clear();
                comboFrameRate.Items.Add("60");
                comboFrameRate.Items.Add("120");
                comboFrameRate.Items.Add("240");
                comboFrameRate.Text = Settings1.Default.FrameRate.ToString();
            }
        }

        private void CheckFullScr_CheckChanged(object sender, EventArgs e)
        {
            UbeatGame.Instance.ToggleFullscreen(((CheckBox)sender).Checked);
            Settings1.Default.FullScreen = ((CheckBox)sender).Checked;
            Settings1.Default.Save();
        }

        private void ComboFrameRate_IndexChaged(object sender, EventArgs e)
        {
            Logger.Instance.Info("Setting Frame Rate: {0}", ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString());
            Settings1.Default.FrameRate = float.Parse(((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString());
            Settings1.Default.Save();
            UbeatGame.Instance.ChangeFrameRate(float.Parse(((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString()));
        }

        private void fillComboDisplay()
        {

            List<ScreenMode> scrnm = ScreenModeManager.GetSupportedModes();

            foreach (ScreenMode mode in scrnm)
            {
                combodisplayMode.Items.Add(
                    string.Format("({0}x{1}){2}",
                    mode.Width,
                    mode.Height,
                    (mode.WindowMode == WindowDisposition.Windowed) ? "[Windowed]" : "")
                );
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
                    comboLang.Position.Y + globalMarginBottom + SpritesContent.Instance.ListboxFont.MeasureString("a").Y + 5);
        }

       

        public override void UpdateControls()
        {
            bool restrictedUpdate = false;

            ComboBox activeBox=null;

            foreach(UIObjectBase ctr in Controls)
            {
                if(ctr is ComboBox)
                {
                    ComboBox ctrc = (ComboBox)ctr;
                    if (ctrc.IsListVisible)
                    {
                        restrictedUpdate = true;
                        activeBox = ctrc;
                        break;
                    }
                }
            }

            foreach (UIObjectBase ctr in Controls)
            {
                if(activeBox != null) {
                    if (restrictedUpdate && (ctr is InputControl && !activeBox.IsListVisible))
                    {
                        continue;
                    }
                        
                }
                

                ctr.Update();
            }
        }


        #region UI

        public UI.Image Logo;
        private ComboBox combodisplayMode;
        private FilledRectangle filledRect1;
        private ComboBox comboFrameRate;
        private CheckBox checkFullScr;
        private CheckBox checkVSync;
        private CheckBox checkInGameVideo;
        private Label lcomboLang;
        private Label lcombodisplayMode;
        private Label lcomboFrameRate;
        private Label lcheckFullScr;
        private Label lcheckVSync;
        private Label lcheckInGameVideo;
        private Notifier notifier;

        public ComboBox comboLang { get; set; }

        #endregion

        #region Events



        #endregion
    }
}
