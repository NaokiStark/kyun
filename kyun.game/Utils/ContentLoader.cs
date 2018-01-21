using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace kyun.Utils
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
                if (KyunGame.RunningOverWine)
                    tx = FromStream(KyunGame.Instance.GraphicsDevice, fs); //More shitting import from stream
                else
                    tx = Texture2D.FromStream(KyunGame.Instance.GraphicsDevice, fs);

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
                tx = FromStream(KyunGame.Instance.GraphicsDevice, fs);
            }

            return tx;
        }

        public static Texture2D FromStream(GraphicsDevice device, Stream stream, bool alt = false)
        {
            if (!KyunGame.RunningOverWine && !alt)
            {
                return Texture2D.FromStream(device, stream);
            }


            var image = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(stream);
            if (image == null)
                throw new Exception("Error loading image.");

            
            var result = new Texture2D(device, image.Width, image.Height, false, SurfaceFormat.Color);
            /*
            Color[] clrs = new Color[image.Width * image.Height];
            for(int y = 0; y < image.Height; y++)
            {
                for(int x = 0; x < image.Width; x++)
                {
                    System.Drawing.Color cl = image.GetPixel(x, y);
                    clrs[x + y * image.Width] = Color.FromNonPremultiplied(cl.R, cl.G, cl.B, cl.A);
                }
            }

            result.SetData(clrs);

            return result;*/

            System.Drawing.Imaging.BitmapData bitmapData = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            var data = new byte[bitmapData.Stride * bitmapData.Height];
            Marshal.Copy(bitmapData.Scan0, data, 0, data.Length);

            Video.VideoDecoder.bgraToRgba(data, data.Length);

            result.SetData<byte>(data);

            image.UnlockBits(bitmapData);

            return result;
        }

        /// <summary>
        /// Returns a Texture2D with a rounded rectangle
        /// http://stackoverflow.com/questions/17217411/create-rounded-rectangle-texture2d
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="borderThickness"></param>
        /// <param name="borderRadius"></param>
        /// <param name="borderShadow"></param>
        /// <param name="backgroundColors"></param>
        /// <param name="borderColors"></param>
        /// <param name="initialShadowIntensity"></param>
        /// <param name="finalShadowIntensity"></param>
        /// <returns></returns>
        public static Texture2D CreateRoundedRectangleTexture(GraphicsDevice graphics, int width, int height, int borderThickness, int borderRadius, int borderShadow, List<Color> backgroundColors, List<Color> borderColors, float initialShadowIntensity, float finalShadowIntensity)
        {
            if (backgroundColors == null || backgroundColors.Count == 0) throw new ArgumentException("Must define at least one background color (up to four).");
            if (borderColors == null || borderColors.Count == 0) throw new ArgumentException("Must define at least one border color (up to three).");
            if (borderRadius < 1) throw new ArgumentException("Must define a border radius (rounds off edges).");
            if (borderThickness < 1) throw new ArgumentException("Must define border thikness.");
            if (borderThickness + borderRadius > height / 2 || borderThickness + borderRadius > width / 2) throw new ArgumentException("Border will be too thick and/or rounded to fit on the texture.");
            if (borderShadow > borderRadius) throw new ArgumentException("Border shadow must be lesser in magnitude than the border radius (suggeted: shadow <= 0.25 * radius).");

            Texture2D texture = new Texture2D(graphics, width, height);
            Color[] color = new Color[width * height];

            for (int x = 1; x < texture.Width; x++)
            {
                for (int y = 1; y < texture.Height; y++)
                {
                    switch (backgroundColors.Count)
                    {
                        case 4:
                            Color leftColor0 = Color.Lerp(backgroundColors[0], backgroundColors[1], ((float)y / (width - 1)));
                            Color rightColor0 = Color.Lerp(backgroundColors[2], backgroundColors[3], ((float)y / (height - 1)));
                            color[x + width * y] = Color.Lerp(leftColor0, rightColor0, ((float)x / (width - 1)));
                            break;
                        case 3:
                            Color leftColor1 = Color.Lerp(backgroundColors[0], backgroundColors[1], ((float)y / (width - 1)));
                            Color rightColor1 = Color.Lerp(backgroundColors[1], backgroundColors[2], ((float)y / (height - 1)));
                            color[x + width * y] = Color.Lerp(leftColor1, rightColor1, ((float)x / (width - 1)));
                            break;
                        case 2:
                            color[x + width * y] = Color.Lerp(backgroundColors[0], backgroundColors[1], ((float)x / (width - 1)));
                            break;
                        default:
                            color[x + width * y] = backgroundColors[0];
                            break;
                    }

                    color[x + width * y] = ColorBorder(x, y, width, height, borderThickness, borderRadius, borderShadow, color[x + width * y], borderColors, initialShadowIntensity, finalShadowIntensity);
                }
            }

            texture.SetData<Color>(color);
            return texture;
        }

        private static Color ColorBorder(int x, int y, int width, int height, int borderThickness, int borderRadius, int borderShadow, Color initialColor, List<Color> borderColors, float initialShadowIntensity, float finalShadowIntensity)
        {
            Rectangle internalRectangle = new Rectangle((borderThickness + borderRadius), (borderThickness + borderRadius), width - 2 * (borderThickness + borderRadius), height - 2 * (borderThickness + borderRadius));

            if (internalRectangle.Contains(x, y)) return initialColor;

            Vector2 origin = Vector2.Zero;
            Vector2 point = new Vector2(x, y);

            if (x < borderThickness + borderRadius)
            {
                if (y < borderRadius + borderThickness)
                    origin = new Vector2(borderRadius + borderThickness, borderRadius + borderThickness);
                else if (y > height - (borderRadius + borderThickness))
                    origin = new Vector2(borderRadius + borderThickness, height - (borderRadius + borderThickness));
                else
                    origin = new Vector2(borderRadius + borderThickness, y);
            }
            else if (x > width - (borderRadius + borderThickness))
            {
                if (y < borderRadius + borderThickness)
                    origin = new Vector2(width - (borderRadius + borderThickness), borderRadius + borderThickness);
                else if (y > height - (borderRadius + borderThickness))
                    origin = new Vector2(width - (borderRadius + borderThickness), height - (borderRadius + borderThickness));
                else
                    origin = new Vector2(width - (borderRadius + borderThickness), y);
            }
            else
            {
                if (y < borderRadius + borderThickness)
                    origin = new Vector2(x, borderRadius + borderThickness);
                else if (y > height - (borderRadius + borderThickness))
                    origin = new Vector2(x, height - (borderRadius + borderThickness));
            }

            if (!origin.Equals(Vector2.Zero))
            {
                float distance = Vector2.Distance(point, origin);

                if (distance > borderRadius + borderThickness + 1)
                {
                    return Color.Transparent;
                }
                else if (distance > borderRadius + 1)
                {
                    if (borderColors.Count > 2)
                    {
                        float modNum = distance - borderRadius;

                        if (modNum < borderThickness / 2)
                        {
                            return Color.Lerp(borderColors[2], borderColors[1], (float)((modNum) / (borderThickness / 2.0)));
                        }
                        else
                        {
                            return Color.Lerp(borderColors[1], borderColors[0], (float)((modNum - (borderThickness / 2.0)) / (borderThickness / 2.0)));
                        }
                    }


                    if (borderColors.Count > 0)
                        return borderColors[0];
                }
                else if (distance > borderRadius - borderShadow + 1)
                {
                    float mod = (distance - (borderRadius - borderShadow)) / borderShadow;
                    float shadowDiff = initialShadowIntensity - finalShadowIntensity;
                    return DarkenColor(initialColor, ((shadowDiff * mod) + finalShadowIntensity));
                }
            }

            return initialColor;
        }

        private static Color DarkenColor(Color color, float shadowIntensity)
        {
            return Color.Lerp(color, Color.Black, shadowIntensity);
        }
    }
}
