using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ubeat.GameScreen
{
    public interface IScreen
    {
        Texture2D Background { get; set; }
        void Update(GameTime tm);
        void Render();
        void Redraw();
        IScreen ScreenInstance { get; set; }
        List<UIObjectBase> Controls { get; set; }
        bool Visible { get; set; }
        float Opacity { get; set; }
        void BackPressed(IScreen screen);
        void OnLoadScreen();
        string Name { get; set; }
    }
}
