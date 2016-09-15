using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ubeat.GameScreen
{
    public interface IScreen
    {
        Texture2D Background { get; set; }
        void Update(GameTime tm);
        void Render();
        void Redraw();
        IScreen ScreenInstance { get; set; }
        List<ScreenUIObject> Controls { get; set; }
        bool Visible { get; set; }
    }
}
