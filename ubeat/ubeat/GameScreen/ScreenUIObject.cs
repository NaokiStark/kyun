using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using ubeat.GameScreen.SUI;

namespace ubeat.GameScreen
{
    public class ScreenUIObject : UIObjs.IUIObject
    {

        public float Scale = 1;

        public Vector2 Position { get; set; }

        public Microsoft.Xna.Framework.Graphics.Texture2D Texture { get; set; }

        public bool IsActive { get; set; }

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
            Rectangle cursor = new Rectangle((int)Mouse.GetState().X, (int)Mouse.GetState().Y, 1, 1);

            if (System.Windows.Forms.Form.ActiveForm != (System.Windows.Forms.Control.FromHandle(UbeatGame.Instance.Window.Handle) as System.Windows.Forms.Form)) return;

            if (!UbeatGame.Instance.IsActive) return; //Fix events

            if (clickC > dClick)
            {
                clickC = clickCount = 0;
            }

            if (cursor.Intersects(rg))
            {
                Over?.Invoke(this, new EventArgs());

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (!alredyPressed)
                    {
                        //Launch MouseDown
                        MouseDown?.Invoke(this, new EventArgs());


                        alredyPressed = true;
                    }
                }                
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released)
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
                        MouseDoubleClick?.Invoke(this, new EventArgs());
                        clickCount = clickC = 0;
                    }

                    alredyPressed = false;
                }
            }

            if (this is Listbox || this is ListboxDiff)
            {
                int actualScrollVal = Mouse.GetState().ScrollWheelValue;
                if (actualScrollVal > lastScrollVal)
                {
                    if (cursor.Intersects(rg))
                    {
                        OnScroll?.Invoke(this, true);
                    }

                }
                else if (actualScrollVal < lastScrollVal)
                {
                    if (cursor.Intersects(rg))
                    {

                        OnScroll?.Invoke(this, false);

                    }
                }
                lastScrollVal = actualScrollVal;

            }
            clickC += (int)UbeatGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds;
        }

        public virtual void Render()
        {
            Rectangle rg = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)(this.Texture.Width*Scale), (int)(this.Texture.Height*Scale));
            UbeatGame.Instance.spriteBatch.Draw(this.Texture, rg, Color.White);
        }
    }
}
