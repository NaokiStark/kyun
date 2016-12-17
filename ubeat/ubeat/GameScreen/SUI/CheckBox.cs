using Microsoft.Xna.Framework;
using System;


namespace ubeat.GameScreen.SUI
{
    public class CheckBox : ScreenUIObject
    {

        RoundedRectangle box;
        RoundedRectangle checkBoxRect;

        public event EventHandler CheckChanged;

        bool thisChecked;

        public bool Checked {
            get {
                return thisChecked;
            }
            set
            {
                thisChecked = value;
                checkBoxRect.Visible = value;
                CheckChanged?.Invoke(this, new EventArgs());
            }
        }
        
        public CheckBox()
        {
            box = new RoundedRectangle(new Vector2(30,30), Color.Black * .75f, 5, 5, Color.Black * 0f);
            checkBoxRect = new RoundedRectangle(new Vector2(20,20), Color.AliceBlue, 3, 3, Color.White * 0f);

            box.Click += Box_Click;
            checkBoxRect.Visible = Checked;

            Texture = null;
        }

        private void Box_Click(object sender, EventArgs e)
        {
            
            Checked = !Checked;
            _OnClick();
        }

        public override void Update()
        {
            box.Position = this.Position;
            box.Update();

            checkBoxRect.Position = new Vector2(box.Position.X + (box.Texture.Width/2) - checkBoxRect.Texture.Width/2,
                box.Position.Y + (box.Texture.Height / 2) - checkBoxRect.Texture.Height / 2);

            checkBoxRect.Update();
        }

        public override void Render()
        {
            box.Render();
            checkBoxRect.Render();
        }
    }
}
