using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using ubeat.Beatmap;

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
            ChangeBeatmapDisplay(Game1.Instance.SelectedBeatmap);
        }

        private void onBackspacePressed()
        {
            ScreenManager.ChangeTo(new MainScreen(false));
        }


        // design
        void ChangeBeatmapDisplay(ubeatBeatMap bm)
        {
            if (Game1.Instance.SelectedBeatmap.SongPath != bm.SongPath)
            {
                Game1.Instance.player.Play(bm.SongPath);
                Game1.Instance.player.soundOut.Volume = Game1.Instance.GeneralVolume;
            }

            Game1.Instance.SelectedBeatmap = bm;

            try
            {
                FileStream bgFstr = new FileStream(bm.Background, FileMode.Open);
                Background = Texture2D.FromStream(Game1.Instance.GraphicsDevice, bgFstr);
                bgFstr.Close();
            }
            catch
            {
                Logger.Instance.Warn("BACKGROUND NOT FOUND!!");
            }
        }
    }
}
