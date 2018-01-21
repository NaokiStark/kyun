using kyun.Utils;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.Utils
{
    public class Skin
    {
        public string Name { get; set; }

        public List<KeyValuePair<string, Texture2D>> Textures = new List<KeyValuePair<string, Texture2D>>();

        public static Skin FromDirectory(string path)
        {

            Skin tmpSkn = new Skin()
            {
                Name = new DirectoryInfo(path).Name
            };
            FileInfo[] files = new DirectoryInfo(path).GetFiles();

            foreach(FileInfo file in files)
            {
                if (file.Extension != ".png" && file.Extension != ".jpg")
                    continue;

                try
                {
                    tmpSkn.Textures.Add(new KeyValuePair<string, Texture2D>(Path.GetFileNameWithoutExtension(file.Name), ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, File.OpenRead(file.FullName), true)));
                }
                catch
                {
                    continue;
                }
            }

            return tmpSkn;
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
