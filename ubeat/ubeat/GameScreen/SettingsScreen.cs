using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using ubeat.Beatmap;
using ubeat.Screen;
using ubeat.GameScreen.SUI;
using System.Collections.Generic;
using System.Windows;
using ubeat.Utils;

namespace ubeat.GameScreen
{
    public partial class SettingsScreen : ScreenBase
    {
        public SettingsScreen() 
            : base("SettingsScreen")
        {
            ScreenInstance = this;

            LoadInterface();

            OnLoad += SettingsScreen_OnLoad;
            ChangeBackground(UbeatGame.Instance.SelectedBeatmap.Background);
            combodisplayMode.Click += (obj, args) =>
              {
                  if(combodisplayMode.IsListVisible)
                    MessageBox.Show("Warning, this will restart ubeat.");
              };
        }

        private void SettingsScreen_OnLoad(object sender, EventArgs e)
        {
            ChangeBackground(UbeatGame.Instance.SelectedBeatmap.Background);
        }

        private void CombodisplayMode_IndexChaged(object sender, EventArgs e)
        {
            List<ScreenMode> scrnm = ScreenModeManager.GetSupportedModes();
            Logger.Instance.Debug("Setting Display Mode: {0}", ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex]);
            Settings1.Default.ScreenMode = ((ComboBox)sender).SelectedIndex;
            Settings1.Default.Save();
            //UbeatGame.Instance.ChangeResolution(scrnm[((ComboBox)sender).SelectedIndex]);

            MessageBox.Show("ubeat will reload right now.","ubeat",MessageBoxButton.OK,MessageBoxImage.Exclamation);

            InstanceManager.Instance.Reload();

        }

    }
}
