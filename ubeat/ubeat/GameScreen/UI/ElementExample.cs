using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kyun.GameScreen.UI
{
    public class ElementExample : UIObjectBase
    {
        public ElementExample(Texture2D texture)
        {
            this.Texture = texture;

            //ScreenUIObject has events

            this.MouseDown += (obj, args) =>
            {

                Logger.Instance.Debug("ElementExample MouseDown event thrown");

            };
        }


        //Update() & Render() are virtual
        //Ands can be overridden

        public override void Update()
        {
            base.Update(); //THIS UPDATES EVENTS (MouseOver, MouseClick, MouseDown...)

            //Do stuff
        }

        public override void Render()
        {

            //base.Render(); //renders this texture by default

            //Do stuff
        }

    }
}
