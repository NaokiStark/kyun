using kyun.GameScreen;
using kyun.GameScreen.UI;
using kyun.Screen;
using kyun.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.GameScreen.UI
{
    public class Tooltip
    {
        private FilledRectangle rectBorder;

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = StringHelper.WrapText(Font, value, 200, lblcontent.Scale);
                lblcontent.Text = text;
                resize();
            }
        }
       
        string text = "";
        private Label lblcontent;

        public SpriteFont Font {
            get
            {
                return lblcontent.Font;
            }
            set {
                lblcontent.Font = Font;
            }
        }

        public new bool Visible
        {
            get
            {
                return lblcontent.Visible;
            }
            set
            {
                rectBorder.Visible = lblcontent.Visible = value;
            }
        }

        public new Vector2 Position
        {
            get
            {
                return rectBorder.Position;
            }
            set
            {
                rectBorder.Position = value;
                lblcontent.Position = new Vector2(value.X + 4, value.Y);
            }
        }

        public Color BorderColor
        {
            get
            {               
                return clr;
            }
            set
            {
                rectBorder.TextureColor = clr = value;
            }
        }

        Color clr = Color.CornflowerBlue;

        Vector2 boxSize = new Vector2(1);

        public Tooltip()
        {
          
            rectBorder = new FilledRectangle(new Vector2(10), Color.White);
            rectBorder.TextureColor = Color.CornflowerBlue;

            lblcontent = new Label();

            lblcontent.Font = SpritesContent.Instance.GeneralBig;
            lblcontent.Scale = 1;
            Visible = false;
        }

        private void resize()
        {

            boxSize = Font.MeasureString(Text);

            rectBorder?.Dispose();

            rectBorder = new FilledRectangle(new Vector2(4, boxSize.Y + 10), Color.White);
            rectBorder.TextureColor = clr;
        }

        public void Update()
        {
            if (!lblcontent.Visible)
                return;

            
            Position = new Vector2(MouseHandler.GetState().Position.X + 10 / 2, MouseHandler.GetState().Position.Y + 10 / 2);

            if(Position.Y + rectBorder.Texture.Height > ScreenModeManager.GetActualMode().Height)
            {
                Position = new Vector2(Position.X, MouseHandler.GetState().Position.Y - /*ScreenModeManager.GetActualMode().Height*/rectBorder.Texture.Height);
            }

            if(Position.X +  boxSize.X > ScreenModeManager.GetActualMode().Width)
            {
                Position = new Vector2(Position.X - boxSize.X - 10, Position.Y);
            }

            rectBorder?.Update();
            lblcontent?.Update();
            
        }

        public void Render()
        {
            if (Vector2.Zero.Equals(Position))
            {
                return;
            }
            if (!lblcontent.Visible)
                return;
            if (!Visible)
            {
                return;
            }
            //base.Render();

            lblcontent?.Render();
            rectBorder?.Render();
            
        }
    }
}
