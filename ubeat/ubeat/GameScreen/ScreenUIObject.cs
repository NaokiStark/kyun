using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ubeat.GameScreen.SUI;



namespace ubeat.GameScreen
{
    public class ScreenUIObject:UIObjs.IUIObject
    {

        public float Scale = 1;

        public Microsoft.Xna.Framework.Vector2 Position { get; set; }

        public Microsoft.Xna.Framework.Graphics.Texture2D Texture { get; set; }

        public bool isActive { get; set; }

        public bool Died { get; set; }

        public delegate void ScrollEventHandler(object sender, bool Up);
 

        public event EventHandler Click;
        public event EventHandler Over;
        public event EventHandler MouseDown;
        public event EventHandler MouseUp;
        public event EventHandler MouseDoubleClick;
        public event ScrollEventHandler OnScroll;


        int lastScrollVal = 0;
        bool alredyPressed;
        int dClick = 1500;
        int clickC = 0;
        int clickCount = 0;
        public virtual void Update()
        {

            Rectangle rg = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)this.Texture.Width, (int)this.Texture.Height);
            Rectangle cursor = new Rectangle((int)Microsoft.Xna.Framework.Input.Mouse.GetState().X, (int)Microsoft.Xna.Framework.Input.Mouse.GetState().Y, 1, 1);

            if (System.Windows.Forms.Form.ActiveForm != (System.Windows.Forms.Control.FromHandle(Game1.Instance.Window.Handle) as System.Windows.Forms.Form)) return;

            if (!Game1.Instance.IsActive) return; //Fix events

            if (clickC > dClick)
            {
                clickC = clickCount = 0;
            }

            if (cursor.Intersects(rg))
            {
                if (Over != null)
                    Over(this, new EventArgs());

                if (Microsoft.Xna.Framework.Input.Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (!alredyPressed)
                    {
                        //Launch MouseDown
                        if (MouseDown != null)
                            MouseDown(this, new EventArgs());

                        
                        alredyPressed = true;
                    }
                }                
            }

            if (Microsoft.Xna.Framework.Input.Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (alredyPressed)
                {
                    clickCount++;
                    //Launch MouseUp
                    if (MouseUp != null)
                        if (cursor.Intersects(rg)) //again
                            MouseUp(this, new EventArgs());

                    //MouseClick

                    if (Click != null)
                        if (cursor.Intersects(rg)) //again
                            Click(this, new EventArgs());

                    if (clickCount > 1)
                    {
                        if (MouseDoubleClick != null)
                            MouseDoubleClick(this, new EventArgs());
                        clickCount = clickC = 0;
                    }

                    alredyPressed = false;
                }
            }

            if (this is Listbox || this is ListboxDiff)
            {
                int actualScrollVal = Microsoft.Xna.Framework.Input.Mouse.GetState().ScrollWheelValue;
                if (actualScrollVal > lastScrollVal)
                {
                    if (cursor.Intersects(rg))
                    {
                        if (OnScroll != null)
                        {
                            OnScroll(this, true);
                        }
                    }

                }
                else if (actualScrollVal < lastScrollVal)
                {
                    if (cursor.Intersects(rg))
                    {
                        
                        OnScroll(this, false);

                    }
                }
                lastScrollVal = actualScrollVal;

            }
            clickC += (int)Game1.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds;
        }

        public virtual void Render()
        {
            Rectangle rg = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)(this.Texture.Width*Scale), (int)(this.Texture.Height*Scale));
            Game1.Instance.spriteBatch.Draw(this.Texture,rg,Color.White);
        }
    }
}
