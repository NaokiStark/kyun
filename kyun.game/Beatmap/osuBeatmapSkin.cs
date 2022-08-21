using kyun.Utils;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.Beatmap
{
    public class osuBeatmapSkin
    {
        public Dictionary<string, Texture2D> skinFiles;

        public osuBeatmapSkin()
        {
            skinFiles = new Dictionary<string, Texture2D>();
        }

        public static osuBeatmapSkin FromFile(string location)
        {
            var tmpobj = new osuBeatmapSkin();

            DirectoryInfo di = new DirectoryInfo(location);

            FileInfo[] hitCircleFiles = di.GetFiles("hitcircle.png");
            FileInfo[] hit0Files = di.GetFiles("hit0.png");
            FileInfo[] hit50Files = di.GetFiles("hit50.png");
            FileInfo[] hit100Files = di.GetFiles("hit100.png");
            FileInfo[] hit300Files = di.GetFiles("hit300.png");

            if (hitCircleFiles.Length > 0)
            {
                tmpobj.skinFiles.Add("hitcircle", ContentLoader.LoadTexture(hitCircleFiles[0].FullName));
            }
            else
            {
                tmpobj.skinFiles.Add("hitcircle", null);
            }

            if (hit0Files.Length > 0)
            {
                tmpobj.skinFiles.Add("hit0", ContentLoader.LoadTexture(hit0Files[0].FullName));
            }
            else
            {
                tmpobj.skinFiles.Add("hit0", null);
            }

            if (hit50Files.Length > 0)
            {
                tmpobj.skinFiles.Add("hit50", ContentLoader.LoadTexture(hit50Files[0].FullName));
            }
            else
            {
                tmpobj.skinFiles.Add("hit50", null);
            }

            if (hit100Files.Length > 0)
            {
                tmpobj.skinFiles.Add("hit100", ContentLoader.LoadTexture(hit100Files[0].FullName));
            }
            else
            {
                tmpobj.skinFiles.Add("hit100", null);
            }


            if (hit300Files.Length > 0)
            {
                tmpobj.skinFiles.Add("hit300", ContentLoader.LoadTexture(hit300Files[0].FullName));
            }
            else
            {
                tmpobj.skinFiles.Add("hit300", null);
            }

            return tmpobj;
        }

        public void CleanUp()
        {
            skinFiles["hitcircle"] = null;
            skinFiles["hit0"] = null;
            skinFiles["hit50"] = null;
            skinFiles["hit100"] = null;
            skinFiles["hit300"] = null;
        }
    }
}
