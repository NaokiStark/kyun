using System;
using kyun.Screen;
using System.Collections.Generic;
using System.Windows;
using kyun.Utils;
using kyun.Notifications;
using kyun.GameScreen.UI;

namespace kyun.GameScreen
{
    public partial class SettingsScreen : ScreenBase
    {

        static IScreen instance = null;
        public static IScreen Instance
        {
            get
            {
                if (instance == null)
                    instance = new SettingsScreen();

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public SettingsScreen()
            : base("SettingsScreen")
        {
            ScreenInstance = this;

            LoadInterface();

            OnLoad += SettingsScreen_OnLoad;

            combodisplayMode.Click += (obj, args) =>
            {
                if (combodisplayMode.IsListVisible)
                    notifier.ShowDialog("This will restart ubeat", 5000, NotificationType.Warning);
            };

            Background = SpritesContent.Instance.DefaultBackground;
        }

        private void SettingsScreen_OnLoad(object sender, EventArgs e)
        {
            //base.ChangeBackground(UbeatGame.Instance.SelectedBeatmap.Background);

        }

        private void CombodisplayMode_IndexChaged(object sender, EventArgs e)
        {
            List<ScreenMode> scrnm = ScreenModeManager.GetSupportedModes();
            Logger.Instance.Debug("Setting Display Mode: {0}", ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex]);
            Settings1.Default.ScreenMode = ((ComboBox)sender).SelectedIndex;
            Settings1.Default.Save();

            notifier.ShowDialog("Ubeat will restart now", 5000, NotificationType.Warning);

            InstanceManager.Instance.Reload();
        }
    }
}
