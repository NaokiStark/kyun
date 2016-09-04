using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubeat.GameScreen
{
    public interface Screen
    {
        Screen ScreenInstance { get; set; }
        List<ScreenUIObject> Controls { get; set; }
        void Update(GameTime tm);
        void Render();
        void Redraw(); //To redraw objects on change resolution
        bool Visible { get; set; }
    }
}
