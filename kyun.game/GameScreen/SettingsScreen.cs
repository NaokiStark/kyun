using System;
using kyun.Screen;
using System.Collections.Generic;
using System.Windows;
using kyun.Utils;
using kyun.game;
using kyun.Notifications;
using kyun.GameScreen.UI;
using kyun.Overlay;
using System.Threading;

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
                
                ((ScreenBase)instance).Dispose();
                Reset();             
            }
        }

        public static void Reset()
        {
            instance = null;
        }

        public SettingsScreen()
            : base("SettingsScreen")
        {
            ScreenInstance = this;

            LoadInterface();

            OnLoad += SettingsScreen_OnLoad;

            combodisplayMode.Click += (obj, args) =>
            {
                /*
                if (combodisplayMode.IsListVisible)
                    ScreenManager.ShowOverlay(QuestionOverlay.ShowAlert((e,a)=> { }, "This will restart kyun!", "This shit needs to restart to resize elements."));
                    //notifier.ShowDialog("This will restart ubeat", 5000, NotificationType.Warning);*/
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

            //notifier.ShowDialog("Ubeat will restart now", 5000, NotificationType.Warning);


            

            KyunGame.Instance.ChangeResolution(Screen.ScreenModeManager.GetSupportedModes()[Settings1.Default.ScreenMode]);

            //InstanceManager.Instance.Reload();
        }
    }
}
