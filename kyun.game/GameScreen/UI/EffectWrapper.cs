using kyun.GameScreen;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.GameScreen.UI
{
    public class EffectWrapper : UIObjectBase
    {
        public List<UIObjectBase> Controls;

        public EffectWrapper(EffectParametersBase effectParameters)
        {
            EffectParameters = effectParameters;
            Controls = new List<UIObjectBase>();
        }

        public virtual void Update(GameTime gameTime)
        {
            base.Update();
            foreach(UIObjectBase control in Controls)
            {
                control?.Update();
            }
        }

        public virtual void Render(GameTime gameTime)
        {
            base.Render();
            foreach(UIObjectBase control in Controls)
            {
                control?.Render();
            }
        }
    }
}
