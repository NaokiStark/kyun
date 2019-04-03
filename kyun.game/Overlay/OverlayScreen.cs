using kyun.game.GameScreen;
using kyun.GameScreen;
using kyun.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kyun.Overlay
{
    public class OverlayScreen : AnimatedScreenBase, IOverlay
    {
        public OverlayType Type { get; set; }

        public OverlayScreen(OverlayType type)
        {

            Background = new Texture2D(KyunGame.Instance.GraphicsDevice, (int)ActualScreenMode.Width, (int)ActualScreenMode.Height);
            Color[] dataBar = new Color[(int)ActualScreenMode.Width * (int)ActualScreenMode.Height];
            for (int i = 0; i < dataBar.Length; ++i) dataBar[i] = Color.Black;
            Background.SetData(dataBar);
            

            Type = type;
            
            BackgroundDim = 0.5f;
        }

        
        
    }
}
