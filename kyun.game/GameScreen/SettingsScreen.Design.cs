using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using kyun.game;
using kyun.Screen;
using kyun.GameScreen.UI;
using kyun.Notifications;
using kyun.Utils;
using kyun.GameScreen.UI.Buttons;
using kyun.game.Utils;
using kyun.game.GameScreen.UI;
using System.IO;

namespace kyun.GameScreen
{
    public partial class SettingsScreen : ScreenBase
    {

        int globalMarginBottom = 15;

        public void LoadInterface()
        {
            //Controls = new HashSet<UIObjectBase>();

            //ñam ñam
            ScreenMode actualMode = ScreenModeManager.GetActualMode();

            var center = new Vector2(actualMode.Width / 2, actualMode.Height / 2);


            //Combobox xdd

            comboLang = new ComboBox(new Vector2(
                center.X, globalMarginBottom * 2 + 0),
                250,
                SpritesContent.Instance.SettingsFont);


            comboLang.Text = "English";
            comboLang.Items.Add("English");
            comboLang.Tooltip = new Tooltip
            {
                Text = "Your language.",
                
            };
            //comboLang.Items.Add("Español");

            //Label Lang

            lcomboLang = new Label();
            lcomboLang.Text = "Language";
            lcomboLang.Font = SpritesContent.Instance.SettingsFont;
            lcomboLang.Size = SpritesContent.Instance.SettingsFont.MeasureString(lcomboLang.Text) + new Vector2(20, 10);
            lcomboLang.Position = new Vector2(center.X - lcomboLang.Size.X - 10, comboLang.Position.Y);


            //Combo display

            combodisplayMode = new ComboBox(
                new Vector2(
                    center.X,
                    comboLang.Position.Y + globalMarginBottom + SpritesContent.Instance.SettingsFont.MeasureString("a").Y + 5),
                250,
                SpritesContent.Instance.SettingsFont);

            combodisplayMode.Tooltip = new Tooltip
            {
                Text = "Screen resolution"
            };

            combodisplayMode.IndexChaged += CombodisplayMode_IndexChaged;
            fillComboDisplay();

            //Label display

            lcombodisplayMode = new Label();
            lcombodisplayMode.Text = "Display Mode";
            lcombodisplayMode.Font = SpritesContent.Instance.SettingsFont;
            lcombodisplayMode.Size = SpritesContent.Instance.SettingsFont.MeasureString(lcombodisplayMode.Text) + new Vector2(20, 10);
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
                string refreshRate = ScreenModeManager.GetMonitorFreq();
                comboFrameRate.Text = refreshRate;
                comboFrameRate.Items.Add(refreshRate);
            }
            else
            {
                comboFrameRate.Items.Add("60");
                comboFrameRate.Items.Add("120");
                comboFrameRate.Items.Add("240");
                comboFrameRate.Items.Add("500");
                comboFrameRate.Items.Add("1000");
            }

            comboFrameRate.Tooltip = new Tooltip
            {
                Text = "This changes framerate, this means how much frames per second will be processed, this feature is limited by your processor."
            };

            comboFrameRate.IndexChaged += ComboFrameRate_IndexChaged;

            //Label Framerate

            lcomboFrameRate = new Label();
            lcomboFrameRate.Text = "Framerate (FPS)";
            lcomboFrameRate.Font = SpritesContent.Instance.SettingsFont;
            lcomboFrameRate.Size = SpritesContent.Instance.SettingsFont.MeasureString(lcomboFrameRate.Text) + new Vector2(20, 10);
            lcomboFrameRate.Position = new Vector2(center.X - lcomboFrameRate.Size.X - 10, comboFrameRate.Position.Y);

            // Check fullscreen

            checkFullScr = new CheckBox();
            checkFullScr.Position = new Vector2(
                    center.X,
                    comboFrameRate.Position.Y + globalMarginBottom + SpritesContent.Instance.SettingsFont.MeasureString("a").Y + 5);

            checkFullScr.Checked = Settings1.Default.FullScreen;

            checkFullScr.Tooltip = new Tooltip
            {
                Text = "Toggle fullscreen mode."
            };

            checkFullScr.CheckChanged += CheckFullScr_CheckChanged;

            //Label Fullscreen

            lcheckFullScr = new Label();
            lcheckFullScr.Text = "Fullscreen";
            lcheckFullScr.Font = SpritesContent.Instance.SettingsFont;
            lcheckFullScr.Size = SpritesContent.Instance.SettingsFont.MeasureString(lcheckFullScr.Text) + new Vector2(20, 10);
            lcheckFullScr.Position = new Vector2(center.X - lcheckFullScr.Size.X - 10, checkFullScr.Position.Y);

            // Check VSync

            checkVSync = new CheckBox();
            checkVSync.Position = new Vector2(
                    center.X,
                    checkFullScr.Position.Y + globalMarginBottom + SpritesContent.Instance.SettingsFont.MeasureString("a").Y + 5);

            
            checkVSync.Checked = Settings1.Default.VSync;
            checkVSync.Tooltip = new Tooltip
            {
                Text = "Toggle VSync mode: When enabled, the frame rate will be set to the maximum that your graphics card supports in Vertical Sync mode, to avoid famous \"Tearing\". Note: this will reduce precision adding more milliseconds between frames."
            };
            checkVSync.CheckChanged += CheckVSync_CheckChanged;

            //Label VSync

            lcheckVSync = new Label();
            lcheckVSync.Text = "VSync";
            lcheckVSync.Font = SpritesContent.Instance.SettingsFont;
            lcheckVSync.Size = SpritesContent.Instance.SettingsFont.MeasureString(lcheckVSync.Text) + new Vector2(20, 10);
            lcheckVSync.Position = new Vector2(center.X - lcheckVSync.Size.X - 10, checkVSync.Position.Y);

            //Check InGame Video

            checkInGameVideo = new CheckBox();
            checkInGameVideo.Position = new Vector2(
                    center.X,
                    checkVSync.Position.Y + globalMarginBottom + SpritesContent.Instance.SettingsFont.MeasureString("a").Y + 5);
            checkInGameVideo.Checked = Settings1.Default.Video;
            checkInGameVideo.Tooltip = new Tooltip
            {
                Text = "Toggle video in all beatmaps, enable it if you like more nice stuff :)"
            };

            checkInGameVideo.CheckChanged += CheckInGameVideo_CheckChanged;

            //Label VSync

            lcheckInGameVideo = new Label();
            lcheckInGameVideo.Text = "In-Game Video (Warn!)";
            lcheckInGameVideo.Font = SpritesContent.Instance.SettingsFont;
            lcheckInGameVideo.Size = SpritesContent.Instance.SettingsFont.MeasureString(lcheckInGameVideo.Text) + new Vector2(20, 10);
            lcheckInGameVideo.Position = new Vector2(center.X - lcheckInGameVideo.Size.X - 10, checkInGameVideo.Position.Y);

            //Check InGame Shitty pc

            checkMyPCSucks = new CheckBox();
            checkMyPCSucks.Position = new Vector2(
                    center.X,
                    checkInGameVideo.Position.Y + globalMarginBottom + SpritesContent.Instance.SettingsFont.MeasureString("a").Y + 5);
            checkMyPCSucks.Checked = Settings1.Default.MyPCSucks;
            checkMyPCSucks.Tooltip = new Tooltip
            {
                Text = "Toaster mode reduce all particles and nice stuff, getting more perfomance in Toasters pc's©"
            };

            checkMyPCSucks.CheckChanged += CheckMyPCSucks_CheckChanged;

            lcheckMyPCSucks = new Label();
            lcheckMyPCSucks.Text = "Toaster mode";
            lcheckMyPCSucks.Font = SpritesContent.Instance.SettingsFont;
            lcheckMyPCSucks.Size = SpritesContent.Instance.SettingsFont.MeasureString(lcheckMyPCSucks.Text) + new Vector2(20, 10);
            lcheckMyPCSucks.Position = new Vector2(center.X - lcheckMyPCSucks.Size.X - 10, checkMyPCSucks.Position.Y);


            // Software render

            checkSoftwareRender = new CheckBox();
            checkSoftwareRender.Position = new Vector2(
                    center.X,
                    checkMyPCSucks.Position.Y + globalMarginBottom + SpritesContent.Instance.SettingsFont.MeasureString("a").Y + 5);

            if (GraphicsAdapter.DefaultAdapter.IsProfileSupported(GraphicsProfile.HiDef))
            {
                checkSoftwareRender.Checked = Settings1.Default.Shaders;
                checkSoftwareRender.Tooltip = new Tooltip
                {
                    Text = "Toggles Shaders effects (Chromatic aberration and more stuff related)."
                };

                checkSoftwareRender.CheckChanged += CheckSoftwareRender_CheckChanged; 
            }
            else
            {
                checkSoftwareRender.Checked = false;
                checkSoftwareRender.Tooltip = new Tooltip
                {
                    Text = "Your graphics card doesn't support shaders, sorry."
                };
                
            }
           

            lcheckSoftwareRender = new Label();
            lcheckSoftwareRender.Text = "Shaders";
            lcheckSoftwareRender.Font = SpritesContent.Instance.SettingsFont;
            lcheckSoftwareRender.Size = SpritesContent.Instance.SettingsFont.MeasureString(lcheckSoftwareRender.Text) + new Vector2(20, 10);
            lcheckSoftwareRender.Position = new Vector2(center.X - lcheckSoftwareRender.Size.X - 10, checkSoftwareRender.Position.Y);


            //Skins Selector

            lbSkins = new ComboBox(new Vector2(
                    center.X,
                    checkSoftwareRender.Position.Y + globalMarginBottom + SpritesContent.Instance.SettingsFont.MeasureString("a").Y + 5), 250, SpritesContent.Instance.SettingsFont);



            foreach(Skin skin in SpritesContent.Instance._SkinManager.skins)
                lbSkins.Items.Add(skin);

            lbSkins.Text = lbSkins.Items[Settings1.Default.Skin].ToString();
            lbSkins.Tooltip = new Tooltip
            {
                Text = "To add more skins or create new, copy 'Default' folder into a new folder (don't delete Default) and edit their images as you like."
            };

            lbSkins.IndexChaged += LbSkins_IndexChaged;

            lSkin = new Label();
            lSkin.Text = "Skin";
            lSkin.Font = SpritesContent.Instance.SettingsFont;
            lSkin.Size = SpritesContent.Instance.SettingsFont.MeasureString(lSkin.Text) + new Vector2(20, 10);
            lSkin.Position = new Vector2(center.X - lSkin.Size.X - 10, lbSkins.Position.Y);

            //Notifier

            notifier = KyunGame.Instance.Notifications;

            //Filled Rectangle

            filledRect1 = new FilledRectangle(
                new Vector2(actualMode.Width, actualMode.Height),
                Color.Black * 0.5f);

            filledRect1.Position = Vector2.Zero;

            backButton = new ButtonStandard(Color.DarkRed)
            {
                ForegroundColor = Color.White,
                Caption = "Back",
                Position = new Vector2(15, ((actualMode.Height - 100) + 100 / 2) - (SpritesContent.Instance.ButtonStandard.Height / 2)),
            };
            backButton.Click += BackButton_Click;

            OnBackSpacePress += (sender, args) =>
             {
                 BackPressed(MainScreen.Instance);
             };



            //Beatmaps selection

            selectbm = new Label(1f)
            {
                Text = "[ Add osu! Beatmaps to kyun! ] (just click here)",
                Position = new Vector2(center.X, lbSkins.Position.Y + 20 + SpritesContent.Instance.SettingsFont.MeasureString("a").Y),
                Font = SpritesContent.Instance.SettingsFont,
                Centered = true
            };

            selectbm.Tooltip = new Tooltip
            {
                Text = "kyun! can load osu!Beatmaps, select 'osu!\\Songs' folder. Note: when you try to add new osu!Beatmaps here, this will installed in your osu! Songs folder."
            };

            selectbm.Click += async(o, e) =>
            {
                var isFullScreen = KyunGame.Instance.Graphics.IsFullScreen;

                if (isFullScreen)
                    KyunGame.Instance.ToggleFullscreen(false);
                /*
                var msgboxResult = Microsoft.Xna.Framework.Input.MessageBox.Show("kyun!", "Do you want to import osu! beatmaps now?\r\n\r\nThis can take time if you have too many beatmaps", new[] { "Forget", "Yes!" });
                var r = await msgboxResult;
                if(r == 0)
                {
                    return;
                }*/

                var path = Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\osu!\Songs");
                if(!(new DirectoryInfo(path).Exists))
                {
                    //await MessageBox.Show("kyun!", "osu! Songs folder not found, please select osu!'s Songs folder", new[] { "Ok" });
                    var fdlg = new System.Windows.Forms.FolderBrowserDialog();

                    System.Windows.Forms.DialogResult rst = fdlg.ShowDialog();

                    if (rst == System.Windows.Forms.DialogResult.Abort || rst == System.Windows.Forms.DialogResult.Cancel)
                        return;

                    if (fdlg.SelectedPath.Length < 1)
                        return;

                    if (!fdlg.SelectedPath.ToLower().EndsWith("songs\\") && !fdlg.SelectedPath.ToLower().EndsWith("songs"))
                    {
                        notifier.ShowDialog("Hmm, This doesn't seem to be osu!'s songs folder. (osu!/Songs)", 10000, NotificationType.Critical);
                        return;
                    }

                    Settings1.Default.osuBeatmaps = fdlg.SelectedPath;
                }
                else
                {
                    Settings1.Default.osuBeatmaps = path;
                }

                
                Settings1.Default.Save();

                if (isFullScreen)
                    KyunGame.Instance.ToggleFullscreen(true);

                //notifier.ShowDialog("kyun! needs to restart to add osu beatmaps, please, just do it.", 10000, NotificationType.Critical);
                LoadScreen.Instance = null;
                ScreenManager.ChangeTo(new LoadScreen(true));
                notifier.ShowDialog("Reloading database, please wait!", 10000, NotificationType.Critical);
            };



            Controls.Add(filledRect1);

            //Add controls 
            Controls.Add(lcheckMyPCSucks);
            Controls.Add(checkMyPCSucks);
            Controls.Add(lcheckSoftwareRender);
            Controls.Add(checkSoftwareRender);
            

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
            Controls.Add(backButton);
            
            Controls.Add(selectbm);

            Controls.Add(lbSkins);
            Controls.Add(lSkin);



            OnLoadScreen();
        }

        private void LbSkins_IndexChaged(object sender, EventArgs e)
        {
            ComboBox cm = (ComboBox)sender;

            //KyunGame.Instance.SuppressDraw();
            SpritesContent.Instance._SkinManager.SwitchSkin(cm.SelectedIndex, true);

            Settings1.Default.Skin = cm.SelectedIndex;
            Settings1.Default.Save();
            //cm.Text = cm.Items[cm.SelectedIndex].ToString();
        }

        private void CheckSoftwareRender_CheckChanged(object sender, EventArgs e)
        {
            bool actualValue = ((CheckBox)sender).Checked;
            
            Settings1.Default.Shaders = actualValue;
            Settings1.Default.Save();
            notifier.ShowDialog("kyun! needs restart to apply this change.");
        }

        private void CheckMyPCSucks_CheckChanged(object sender, EventArgs e)
        {
            bool actualValue = ((CheckBox)sender).Checked;

            Settings1.Default.MyPCSucks = actualValue;
            Settings1.Default.Save();

            if (actualValue)
                checkInGameVideo.Checked = false;

            if(actualValue)
                notifier.ShowDialog("I'm so sorry :(", 5000, NotificationType.Info);

        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            BackPressed(MainScreen.Instance);
        }

        private void CheckInGameVideo_CheckChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked && Settings1.Default.MyPCSucks)
            {
                ((CheckBox)sender).Checked = false;
            }
            if (((CheckBox)sender).Checked)
            {
                
                notifier.ShowDialog("Great!, your pc is a master race!", 5000, NotificationType.Warning);
                Logger.Instance.Info("Setting Video: " + ((CheckBox)sender).Checked.ToString());
                KyunGame.Instance.VideoEnabled = ((CheckBox)sender).Checked;
                Settings1.Default.Video = ((CheckBox)sender).Checked;
                Settings1.Default.Save();
            }
            else
            {
                Logger.Instance.Info("Setting Video: " + ((CheckBox)sender).Checked.ToString());
                KyunGame.Instance.VideoEnabled = ((CheckBox)sender).Checked;
                Settings1.Default.Video = ((CheckBox)sender).Checked;
                Settings1.Default.Save();
            }

        }

        private void CheckVSync_CheckChanged(object sender, EventArgs e)
        {
            KyunGame.Instance.ToggleVSync(((CheckBox)sender).Checked);
            Logger.Instance.Info("Setting VSync: " + ((CheckBox)sender).Checked.ToString());
            Settings1.Default.VSync = ((CheckBox)sender).Checked;
            Settings1.Default.Save();

            if (((CheckBox)sender).Checked)
            {
                string refreshRate = ScreenModeManager.GetMonitorFreq();
                comboFrameRate.Text = refreshRate;
                
                comboFrameRate.Items.Clear();
                comboFrameRate.Items.Add(refreshRate);
            }
            else
            {
                comboFrameRate.Items.Clear();
                comboFrameRate.Items.Add("60");
                comboFrameRate.Items.Add("120");
                comboFrameRate.Items.Add("240");
                comboFrameRate.Items.Add("500");
                comboFrameRate.Items.Add("1000");
                comboFrameRate.Text = Settings1.Default.FrameRate.ToString();
            }
        }

        private void CheckFullScr_CheckChanged(object sender, EventArgs e)
        {
            KyunGame.Instance.ToggleFullscreen(((CheckBox)sender).Checked);
            Settings1.Default.FullScreen = ((CheckBox)sender).Checked;
            Settings1.Default.Save();
        }

        private void ComboFrameRate_IndexChaged(object sender, EventArgs e)
        {
            Logger.Instance.Info("Setting Frame Rate: {0}", ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString());
            Settings1.Default.FrameRate = float.Parse(((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString());
            Settings1.Default.Save();
            KyunGame.Instance.ChangeFrameRate(float.Parse(((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString()));
        }

        private void fillComboDisplay()
        {

            List<ScreenMode> scrnm = ScreenModeManager.GetSupportedModes();

            foreach (ScreenMode mode in scrnm)
            {
                combodisplayMode.Items.Add(
                    string.Format("({0}x{1}){2}",
                    mode.ScaledWidth,
                    mode.ScaledHeight,
                    (mode.WindowMode == WindowDisposition.Borderless) ? "[Borderless]" : "[Windowed]")
                );
            }

            combodisplayMode.Text = combodisplayMode.Items[Settings1.Default.ScreenMode].ToString();
        }

        internal override void UpdateControls()
        {
            base.UpdateControls();
        }


        #region UI

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
        private ButtonStandard backButton;
        private Label selectbm;
        private CheckBox checkMyPCSucks;
        private CheckBox checkSoftwareRender;
        private Label lcheckSoftwareRender;
        private ComboBox lbSkins;
        private Label lSkin;

        public ComboBox comboLang { get; set; }
        public Label lcheckMyPCSucks { get; private set; }

        #endregion

        #region Events



        #endregion
    }
}
