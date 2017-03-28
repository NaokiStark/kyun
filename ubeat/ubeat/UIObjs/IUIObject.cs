﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ubeat.UIObjs
{
    public interface IUIObject
    {
        Vector2 Position { get; set; }
        Texture2D Texture { get; set; }
        float Opacity { get; set; }
        bool IsActive { get; set; }
        bool Died { get; set; }

        //methods

        void Update();
        void Render();
    }
}
