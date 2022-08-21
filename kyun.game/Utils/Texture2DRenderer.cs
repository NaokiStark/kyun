using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.Utils
{
    public class Texture2DRenderer
    {
        public static Texture2D RenderToTexture(Texture2D txBase, Texture2D txToAdd, Point locBottom, Point locTop, GraphicsDevice graphicsDevice, float rotation = 0, bool txTop = false)
        {

            Rectangle boundsBottom = new Rectangle(locBottom, txBase.Bounds.Size);
            Rectangle boundsTop = new Rectangle(locTop, txToAdd.Bounds.Size);
            Rectangle rgsize = Rectangle.Intersect(boundsBottom, boundsTop);

            Vector2 vpos = new Vector2(locBottom.X + (txBase.Width / 2),
                           locBottom.Y + (txBase.Height / 2));
            Vector2 vcent = new Vector2(rgsize.Width / 2, rgsize.Height / 2);


            RenderTarget2D renderTarget2D = new RenderTarget2D(graphicsDevice, rgsize.Width, rgsize.Height);

            graphicsDevice.SetRenderTarget(renderTarget2D);

            KyunGame.Instance.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default);
            graphicsDevice.Clear(Color.Transparent);

            KyunGame.Instance.SpriteBatch.Draw(txBase, new Vector2(locBottom.X, locBottom.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            KyunGame.Instance.SpriteBatch.Draw(txToAdd, new Vector2(locTop.X, locTop.Y), null, Color.White, rotation, vcent, 1, SpriteEffects.None, 0);

            KyunGame.Instance.SpriteBatch.End();

            graphicsDevice.SetRenderTarget(null);

            return (Texture2D) renderTarget2D;
        }
    }
}
