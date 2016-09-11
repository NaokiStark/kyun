using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ubeat.Audio;
using ubeat.Beatmap;

namespace ubeat.GameScreen.SUI
{
    public class Listbox : ScreenUIObject
    {
        public Rectangle BoundBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)width, (int)height);
                //return base.BoundBox;
            }
        }

        SpriteFont textFont;


        public int vertOffset = 0;
        private int horizontalOffset = 0;

        
        List<string> objects = new List<string>();

        [Obsolete]
        public List<string> dataSource
        {
            get
            {
                return objects;
            }
            set
            {
                objects = value;
                if (selectedIndex >= value.Count)
                    selectedIndex = -1;
            }
        }

        public List<Mapset> Items = new List<Mapset>();

        public int selectedIndex = -1;

        public bool VertScrollBar = true;
        public bool HorizontalScrollBar = true;
        public bool autoAdjust = true;

        public float width = 50;
        public float height = 250;

        public event EventHandler IndexChanged;

        public Listbox(Vector2 position, float width, float height, SpriteFont font)
        {
            this.Position = position;
            this.height = height;
            this.width = width;
            

            this.textFont = font;
            this.Scale = 1;

            this.Click += Listbox_Click;
            this.OnScroll += Listbox_OnScroll;

            int pwidth = (int)width;
            int pheight = (int)height;
            this.Texture = new Texture2D(Game1.Instance.GraphicsDevice, pwidth, pheight);
            Color[] dataBar = new Color[pwidth * pheight];
            for (int i = 0; i < dataBar.Length; ++i) dataBar[i] = Color.Black * .8f;
            this.Texture.SetData(dataBar);
        }

        void Listbox_OnScroll(object sender, bool Up)
        {
            AudioPlaybackEngine.Instance.PlaySound(Game1.Instance.ScrollHit);
            if (!Up)
            {
                vertOffset++;
            }
            else
            {
                if (vertOffset > 0)
                    vertOffset--;
            }
        }

        void Listbox_Click(object sender, EventArgs e)
        {
            computeSelected();
        }

        public void computeSelected()
        {
            Point mousePos = new Point(Mouse.GetState().X, Mouse.GetState().Y);
            if (BoundBox.Contains(mousePos))
            {
                if (mousePos.X - Position.X <= width)
                {
                    float off = mousePos.Y - Position.Y;
                    float which = off / 50;
                    which = (float)Math.Floor(which);
                    int oldSel = selectedIndex;
                    selectedIndex = (int)(which + vertOffset);
                    if (selectedIndex >= Items.Count) selectedIndex = -1;
                }
            }
            else
            {
                selectedIndex = -1;
            }
        }

      
        public int textOverflows()
        {
            int ret = 0;
            //Find the length of a single monospace character
            float length = textFont.MeasureString("a").X;
            //Now we can use this length to find out just HOW many characters we've gone over. Quite quite.
            int numCharsAllowed = (int)(width / length) - 1;
            for (int i = vertOffset; i < Items.Count; i++)
            {
                //oops

                object o = Items[i].Title + " - " + Items[i].Artist;
                float oLength = textFont.MeasureString(o.ToString()).X;
                int numChars = (int)(oLength / length);
                if (numChars > numCharsAllowed)
                {
                    int numOver = numChars - numCharsAllowed;
                    ret = (numOver > ret) ? numOver : ret;
                }
            }
            return ret;
        }

        public override void Render()
        {
            float perEntryHeight = 50;
            int maxCanHold = (int)(height / perEntryHeight);

            //Take the width divided by the width of a monospace character in the current font, minus 1 (for spacing)
            int maxCharsCanHold = (int)(width / textFont.MeasureString("a").X) - 1;

            Vector2 startBoxPos = Position + new Vector2(2, 2);
            Vector2 drawTextPos = startBoxPos + new Vector2(10, -2);
            Vector2 bottomRightBoxPos = new Vector2(startBoxPos.X + (width - 4), startBoxPos.Y + perEntryHeight);

            
            //DrawManager.Draw_Box(Position + new Vector2(width / 2, height / 2), width, height, Color.Black, sb, 0, 200);
            //DrawManager.Draw_Outline(Position + new Vector2(width / 2, height / 2), width, height, Color.Black, sb);
            
             horizontalOffset = 0;

             Rectangle rg = new Rectangle((int)this.Position.X, (int)this.Position.Y, Texture.Width, Texture.Height);
             Game1.Instance.spriteBatch.Draw(this.Texture, rg, Color.White);

            for (int i = 0; i < maxCanHold && i + vertOffset < Items.Count; i++)
            {
                string o2 = Items[i + vertOffset].Artist + " - " + Items[i + vertOffset].Title;
                string o = "";
                for (int a = horizontalOffset; a < o2.Length && a < maxCharsCanHold + horizontalOffset; a++)
                {
                    o += o2[a];
                }
                Color textColor = Color.White;
                Color backColor = Color.Black;
                Color selectedColor = Color.Brown;

                byte alpha = 200;
                if (i % 2 == 0)
                    alpha = 255;
                if (i + vertOffset == selectedIndex)
                {
                    textColor = Color.Yellow;
                }

                Game1.Instance.spriteBatch.DrawString(textFont, o, drawTextPos, textColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0);

                startBoxPos.Y += perEntryHeight;
                drawTextPos.Y += perEntryHeight;
                bottomRightBoxPos.Y += perEntryHeight;
            }
            //base.Draw(sb);
        }
        int oldSel = 1000;

        public void LaunchEvent()
        {
            IndexChanged?.Invoke(this, new EventArgs());
        }

        public override void Update()
        {
            base.Update(); //Events
            if (autoAdjust)
            {
                float maxWidth = 0;
                foreach (Mapset o in Items)
                {
                    string r = o.Artist + " - " + o.Title;
                    Vector2 size = textFont.MeasureString(r);
                    float w = size.X;
                    if (w > maxWidth)
                        maxWidth = w;
                }
                maxWidth += 5;
                if (maxWidth > 5)
                {

                }
            }

            if (oldSel != selectedIndex)
                if (IndexChanged != null)
                {
                    oldSel = selectedIndex;
                    IndexChanged(this, new EventArgs());
                }

            oldSel = selectedIndex;
        }
    }
}
