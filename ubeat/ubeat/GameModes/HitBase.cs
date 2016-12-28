using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubeat.GameModes
{
    /// <summary>
    /// Hit Control Base
    /// </summary>
    public class HitBase : GameScreen.UIObjectBase
    {
        
        public HitBase(Texture2D texture)
        {
            Texture = texture;
            Died = false;
        }
    }
}
