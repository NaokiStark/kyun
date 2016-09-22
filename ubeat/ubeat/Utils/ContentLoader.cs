using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Windows.Forms;

namespace ubeat.Utils
{
    public static class ContentLoader
    {
        public static Texture2D LoadTexture(string path)
        {

            FileInfo fInfo = new FileInfo(path);

            if (!fInfo.Exists)
                throw new FileNotFoundException("The file doesn't exist. lol");

            Texture2D tx = null;

            using (FileStream fs = new FileStream(fInfo.FullName, FileMode.Open))
            {
                tx = Texture2D.FromStream(UbeatGame.Instance.GraphicsDevice, fs);
            }

            return tx;
        }

        public static Texture2D LoadTextureFromAssets(string asset)
        {

            FileInfo fInfo = new FileInfo(Path.Combine(Application.StartupPath, "Assets", asset));

            if (!fInfo.Exists)
                throw new FileNotFoundException(string.Concat("Asset: ", fInfo.Name, " can't be loaded. (is not fucking exist"));

            Texture2D tx = null;

            using (FileStream fs = new FileStream(fInfo.FullName, FileMode.Open))
            {
                tx = Texture2D.FromStream(UbeatGame.Instance.GraphicsDevice, fs);
            }

            return tx;
        }
    }
}
