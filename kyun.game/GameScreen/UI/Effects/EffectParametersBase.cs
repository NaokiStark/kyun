using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.GameScreen.UI
{
    public class EffectParametersBase
    {
        public List<KeyValuePair<string, dynamic>> Parameters = new List<KeyValuePair<string, dynamic>>();

        public Effect Effect;

        public EffectParametersBase()
        {

        }

        public virtual void Update()
        {
            //bump
        }
    }
}
