using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ubeat.GameScreen;

namespace ubeat.Extensions
{
    public static class IScreenExtensions
    {
        public static void ChangeBackground(this IScreen s, string backgroundPath)
        {
            try
            {
                using (var fs = new FileStream(backgroundPath, FileMode.Open))
                {
                    s.Background = Texture2D.FromStream(Game1.Instance.GraphicsDevice, fs);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Logger.Instance.Warn("There was a problem loading the background: {0}", ex.Message);
                Logger.Instance.Warn("StackTrace: {0}", ex.StackTrace);
#else
                Logger.Instance.Warn("There was a problem loading the background");
#endif
            }
        }

        public static void LoadCurrentGameInstanceBackground(this IScreen s)
        {
            ChangeBackground(s, Game1.Instance.SelectedBeatmap.Background);
        }
    }
}
