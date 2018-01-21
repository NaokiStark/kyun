using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace kyun.GameScreen
{
    public interface IScreen
    {
        string Name { get; set; }
        bool Visible { get; set; }
        float Opacity { get; set; }

        Texture2D Background { get; set; }
        IScreen ScreenInstance { get; set; }
        List<UIObjectBase> Controls { get; set; }

        void Update(GameTime tm);
        void Render();
                
        //TODO: useless?
        void BackPressed(IScreen screen);
        void OnLoadScreen();
        
    }
}
