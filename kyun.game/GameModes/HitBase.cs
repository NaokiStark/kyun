using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.Score;

namespace kyun.GameModes
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

        internal virtual ScoreType GetScore()
        {
            return ScoreType.Miss;
        }
    }
}
