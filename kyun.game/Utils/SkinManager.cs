using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using kyun.Utils;
using System.Reflection;
using kyun.GameScreen;
using kyun.GameModes.Classic;
using kyun.Overlay;

namespace kyun.game.Utils
{
    public class SkinManager
    {

        public List<Skin> skins = new List<Skin>();

        DirectoryInfo SkinsDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Skins"));
        
        public SkinManager()
        {
            DirectoryInfo[] dirs = SkinsDir.GetDirectories();

            foreach(DirectoryInfo dir in dirs)
            {
                skins.Add(Skin.FromDirectory(dir.FullName));
            }

        }

        public void SwitchSkin(int skin, bool reload = false)
        {
            if (skin >= skins.Count)
                return;

            foreach(KeyValuePair<string, Texture2D> tx in skins[skin].Textures)
            {
                try
                {
                    var propInfo = typeof(SpritesContent).GetProperty(tx.Key);
                    if (propInfo != null)
                    {
                        propInfo.SetValue(SpritesContent.Instance, tx.Value, null);
                    }

                }
                catch
                {
                    continue;
                }
                
            }

            if (!reload)
                return;

            ((ScreenBase)MainScreen.Instance)?.Dispose();
            ((ScreenBase)BeatmapScreen.Instance)?.Dispose();
            ((ScreenBase)SettingsScreen.Instance)?.Dispose();
            ((ScreenBase)ClassicModeScreen.Instance)?.Dispose();
            try
            {
                ((ScreenBase)ScorePanel.Instance)?.Dispose();
                PauseOverlay.Instance.Dispose();
            }
            catch
            {
                ScorePanel.Instance = null;
            }

            ((ScreenBase)QuestionOverlay.Instance)?.Dispose();
            MainScreen.Instance = null;
            BeatmapScreen.Instance = null;
            SettingsScreen.Instance = null;
            ClassicModeScreen.Instance = null;
            ScorePanel.Instance = null;
            QuestionOverlay.Instance = null;
            PauseOverlay.Instance = null;

            GC.Collect();

            Screen.ScreenModeManager.Change();

            ScreenManager.ChangeTo(SettingsScreen.Instance); //return 
        }
        
    }
}
