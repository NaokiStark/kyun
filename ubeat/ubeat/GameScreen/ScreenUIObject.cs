using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using ubeat.GameScreen.UI;

namespace ubeat.GameScreen
{
    public class UIObjectBase : UIObjs.IUIObject, IDisposable
    {
        public bool Disposing { get; set; }

        public float Scale = 1;

        public Vector2 Position { get; set; }

        public Microsoft.Xna.Framework.Graphics.Texture2D Texture { get; set; }

        public bool IsActive { get; set; }

        public bool Died { get; set; }

        public delegate void ScrollEventHandler(object sender, bool Up);

        public bool Visible { get; set; }

        public event EventHandler Click;
        public event EventHandler Over;
        public event EventHandler MouseDown;
        public event EventHandler MouseUp;
        public event EventHandler MouseDoubleClick;
        public event ScrollEventHandler OnScroll;


        int lastScrollVal = 0;
        bool alredyPressed;
        int dClick = 2000;
        int clickC = 0;
        int clickCount = 0;

        public UIObjectBase()
        {
            Visible = true;
        }


        public virtual void Update()
        {
            if (!Visible)
                return;

            Rectangle rg = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)this.Texture.Width, (int)this.Texture.Height);
            Rectangle cursor = new Rectangle((int)Mouse.GetState().X, (int)Mouse.GetState().Y, 1, 1);
            Rectangle tcursor = new Rectangle((int)UbeatGame.Instance.touchHandler.LastPosition.X, (int)UbeatGame.Instance.touchHandler.LastPosition.Y, 1, 1);


            if (System.Windows.Forms.Form.ActiveForm != (System.Windows.Forms.Control.FromHandle(UbeatGame.Instance.Window.Handle) as System.Windows.Forms.Form)) return;

            if (!UbeatGame.Instance.IsActive) return; //Fix events

            clickC += (int)UbeatGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds;
            if (clickC > dClick)
            {
                clickC = clickCount = 0;
            }

            if (cursor.Intersects(rg) || tcursor.Intersects(rg))
            {

                Over?.Invoke(this, new EventArgs());

                if (Mouse.GetState().LeftButton == ButtonState.Pressed || UbeatGame.Instance.touchHandler.TouchDown)
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
                if (UbeatGame.Instance.touchHandler.TouchUp)
                {                               
                    if (alredyPressed)
                    {
                        if(clickCount < 3)
                        {
                            clickCount++;
                            clickC = 0;
                        }
                    
                        //Launch MouseUp
                        if (MouseUp != null)
                            if (cursor.Intersects(rg) || tcursor.Intersects(rg)) //again
                                MouseUp(this, new EventArgs());

                        //MouseClick

                        if (Click != null)
                            if (cursor.Intersects(rg)) //again
                                Click(this, new EventArgs());

                        if (clickCount > 1 && clickC > 10)
                        {
                            MouseDoubleClick?.Invoke(this, new EventArgs());
                            clickCount = clickC = 0;
                        }

                        alredyPressed = false;
                    }
                }
            }

            if (this is Listbox || this is ListboxDiff || this is ObjectListbox)
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
        }

        public virtual void Render()
        {
            if (!Visible)
                return;
            
            Rectangle rg = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)(this.Texture.Width*Scale), (int)(this.Texture.Height*Scale));
            UbeatGame.Instance.SpriteBatch.Draw(this.Texture, rg, Color.White);
        }

        public void _OnClick()
        {
            Click?.Invoke(this, new EventArgs());
        }

        public virtual void Dispose()
        {
            Disposing = true;
        }
    }
}
