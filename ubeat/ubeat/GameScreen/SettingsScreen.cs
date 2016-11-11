using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using ubeat.Beatmap;
using ubeat.Extensions;
using ubeat.Screen;
using ubeat.GameScreen.SUI;
using System.Collections.Generic;

namespace ubeat.GameScreen
{
    public partial class SettingsScreen : ScreenBase
    {
        public SettingsScreen() 
            : base("SettingsScreen")
        {
            ScreenInstance = this;

            LoadInterface();
        }

           
        private void CombodisplayMode_IndexChaged(object sender, EventArgs e)
        {
            List<ScreenMode> scrnm = ScreenModeManager.GetSupportedModes();
            Logger.Instance.Debug("Setting Display Mode: {0}", ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex]);
            Settings1.Default.ScreenMode = ((ComboBox)sender).SelectedIndex;
            Settings1.Default.Save();
            UbeatGame.Instance.ChangeResolution(scrnm[((ComboBox)sender).SelectedIndex]);
        }

    }
}
