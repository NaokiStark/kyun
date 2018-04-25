using ICSharpCode.SharpZipLib.Zip;
using kyun.GameScreen;
using kyun.GameScreen.UI;
using kyun.OsuUtils;
using kyun.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kyun.game.GameScreen
{
    public class BeatmapLoader : ScreenBase
    {
        private static BeatmapLoader instance;
        private Label loadingLabel;
        private Label titleLabel;

        public static new BeatmapLoader GetInstance()
        {
            if (instance == null)
                instance = new BeatmapLoader();

            return instance;
        }

        public BeatmapLoader() : base("Beatmap Loader")
        {
            loadingLabel = new Label
            {
                Text = "Adding your new beatmaps",
                Position = new Vector2(ActualScreenMode.Width / 2, 0),
                Centered = true,
                Font = SpritesContent.Instance.TitleFont,
            };

            titleLabel = new Label
            {
                Text = "...",
                Position = new Vector2(ActualScreenMode.Width / 2, loadingLabel.Position.Y + 100),
                Centered = true,
                Font = SpritesContent.Instance.DefaultFont,
            };
            
            Controls.Add(loadingLabel);
            Controls.Add(titleLabel);
        }

        /// <summary>
        /// Load Beatmap
        /// ToDo: Make this more secure async call
        ///</summary>
        /// <param name="paths">path of packed zip</param>
        public void LoadBeatmaps(string[] paths, ScreenBase returnScreen)
        {
            Background = SpritesContent.Instance.DefaultBackground;
            BackgroundDim = .5f;

            ScreenManager.ChangeTo(this);

            Thread tr = new Thread(new ThreadStart(() =>
            {
                
                FastZip fz = new FastZip();
                foreach (string path in paths)
                {

                    string dlPath = Path.Combine(Settings1.Default.osuBeatmaps, Path.GetFileNameWithoutExtension(new FileInfo(path).Name));
                    if (string.IsNullOrWhiteSpace(Settings1.Default.osuBeatmaps))
                    {
                        //WTF I dunno
                        if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps")))
                            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps"));

                        dlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps", Path.GetFileNameWithoutExtension(new FileInfo(path).Name));
                    }

                    titleLabel.Text = new FileInfo(path).Name;
                    try
                    {
                        fz.ExtractZip(path, dlPath, null);
                    }
                    catch(Exception ex)
                    {
                        titleLabel.Text = "Error!";
                        Thread.Sleep(1000);
                        ScreenManager.ChangeTo(BeatmapScreen.Instance);
                        ((BeatmapScreen)BeatmapScreen.Instance).changeBeatmapAndReorderDisplay();

                        Logger.Instance.Warn(ex.Message + "\r\n"+ ex.StackTrace);
                        return;
                    }
                    

                    FileInfo[] fl = new DirectoryInfo(dlPath).GetFiles();

                    Beatmap.Mapset bmms = null;
                    foreach (FileInfo fff in fl)
                    {
                        if (fff.Extension.ToLower() == ".osu")
                        {
                            OsuBeatMap bmp = OsuUtils.OsuBeatMap.FromFile(fff.FullName);
                            if (bmp != null)
                            {
                                if (bmms == null)
                                    bmms = new Beatmap.Mapset(bmp.Title, bmp.Artist, bmp.Creator, bmp.Tags);
                                bmms.Add(bmp);


                            }
                        }

                    }
                    if (bmms != null)
                    {
                        Beatmap.Mapset mapst = Beatmap.Mapset.OrderByDiff(bmms);

                        InstanceManager.AllBeatmaps.Add(mapst);
                    }

                }
                Thread.Sleep(1000);
                ScreenManager.ChangeTo(BeatmapScreen.Instance);
                ((BeatmapScreen)BeatmapScreen.Instance).changeBeatmapAndReorderDisplay();
            }));

            tr.Start();
        }
    }
}
