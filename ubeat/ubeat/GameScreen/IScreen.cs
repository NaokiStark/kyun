using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ubeat.GameScreen
{
    public interface IScreen
    {
        void Update(GameTime tm);
        void Render();
        void Redraw();
        IScreen ScreenInstance { get; set; }
        List<ScreenUIObject> Controls { get; set; }
        bool Visible { get; set; }
    }
}
