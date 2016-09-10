using System;

namespace ubeat.GameScreen
{
    public partial class SettingsScreen : IScreen
    {
        public SettingsScreen()
        {
            ScreenInstance = this;

            OnLoad += _OnLoad;

            LoadInterface();
        }

        private void _OnLoad(object sender, EventArgs e)
        {
#if DEBUG
            Logger.Instance.Debug("SettingsScreen loaded");
#endif
        }

        private void onBackspacePressed()
        {
            ScreenManager.ChangeTo(new MainScreen(false));
        }
    }
}
