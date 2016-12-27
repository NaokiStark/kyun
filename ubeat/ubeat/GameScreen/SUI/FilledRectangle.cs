﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ubeat.GameScreen.SUI
{
    public class FilledRectangle : UIObjectBase
    {
        
        public FilledRectangle(Vector2 Size, Color Colr)
        {
            this.Texture = new Texture2D(UbeatGame.Instance.GraphicsDevice, (int)Size.X, (int)Size.Y);
            Color[] dataBar = new Color[(int)Size.X * (int)Size.Y];
            for (int i = 0; i < dataBar.Length; ++i) dataBar[i] = Colr;
            this.Texture.SetData(dataBar);
            Scale = 1;            
        }
    }
}
