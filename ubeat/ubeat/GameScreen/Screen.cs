using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ubeat.GameScreen
{
    public interface IScreen
    {
        IScreen ScreenInstance { get; set; }
        List<ScreenUIObject> Controls { get; set; }
        void Update(GameTime tm);
        void Render();
        void Redraw(); //To redraw objects on change resolution
        bool Visible { get; set; }
    }
}
