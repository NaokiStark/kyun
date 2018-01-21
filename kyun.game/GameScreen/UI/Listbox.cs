using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.Audio;
using kyun.Beatmap;
using kyun.Utils;

namespace kyun.GameScreen.UI
{
    public class Listbox : InputControl
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

        float perEntryHeight = 0;
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

        public int selectedIndex {
            get{
                return sIndex;
            }
            set
            {
                sIndex = value;
               
            }
        }


        private int sIndex = -1;

        public bool VertScrollBar = true;
        public bool HorizontalScrollBar = true;
        public bool autoAdjust = true;

        public float width = 50;
        public float height = 250;

        float measuredTextWidth = 0;

        int stpsrnd = 2;

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
            this.Texture = new Texture2D(KyunGame.Instance.GraphicsDevice, pwidth, pheight);
            Color[] dataBar = new Color[pwidth * pheight];
            for (int i = 0; i < dataBar.Length; ++i) dataBar[i] = Color.Black * .8f;
            this.Texture.SetData(dataBar);

            perEntryHeight = 90;
            maxCanHold = (int)(height / perEntryHeight);

            measuredTextWidth = textFont.MeasureString("a").X;

        }

        void Listbox_OnScroll(object sender, bool Up)
        {
            //AudioPlaybackEngine.Instance.PlaySound(SpritesContent.Instance.ScrollHit);
            EffectsPlayer.PlayEffect(SpritesContent.Instance.ScrollHit);

            if (!Up)
            {
                if (vertOffset + (maxCanHold / 2) > Items.Count) return;

                vertOffset += 1;
            }
            else
            {

                if (vertOffset > 0)
                    if (vertOffset - 1 > 0)
                        vertOffset -= 1;
                    else
                        vertOffset--;
            }

            int rnd = new Random().Next(10, 20);
            if (!Up)
            {
                rnd = -rnd;
                stpsrnd = 5;
            }
            
        }

        public void Select(bool up)
        {
            if (up)
            {
                if(selectedIndex < 1)
                {
                    return;
                }

                selectedIndex--;

                if (selectedIndex < vertOffset)
                {
                    if(selectedIndex < vertOffset + (maxCanHold / 2))
                        vertOffset--;
                }
            }
            else
            {
                if(selectedIndex > Items.Count - 1)
                {
                    return;
                }

                selectedIndex++;
                if(selectedIndex > vertOffset + maxCanHold - 1)
                {
                    vertOffset++;
                }
            }

            
        }
        
        void Listbox_Click(object sender, EventArgs e)
        {
            computeSelected();
        }

        public void computeSelected()
        {
            Point mousePos = new Point((int)MouseHandler.GetState().X, (int)MouseHandler.GetState().Y);
            Rectangle mouseRect = new Rectangle(mousePos.X, mousePos.Y, 5, 5);
            if (BoundBox.Intersects(mouseRect))
            {
                if (mousePos.X - Position.X <= width)
                {
                    float off = mousePos.Y - Position.Y;
                    float which = off / perEntryHeight;
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

                object o = Items[i].Title;
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

        public int maxCanHold;
        
        public override void Render()
        {
             


            //Take the width divided by the width of a monospace character in the current font, minus 1 (for spacing)
            int maxCharsCanHold = (int)(width / measuredTextWidth) - 1;

            Vector2 startBoxPos = Position + new Vector2(2, stpsrnd);
            Vector2 drawTextPos = startBoxPos + new Vector2(0, -2);
            Vector2 bottomRightBoxPos = new Vector2(startBoxPos.X + (width - 4), startBoxPos.Y + perEntryHeight);

            
            //DrawManager.Draw_Box(Position + new Vector2(width / 2, height / 2), width, height, Color.Black, sb, 0, 200);
            //DrawManager.Draw_Outline(Position + new Vector2(width / 2, height / 2), width, height, Color.Black, sb);
            
             horizontalOffset = 0;

             Rectangle rg = new Rectangle((int)this.Position.X, (int)this.Position.Y, Texture.Width, Texture.Height);
            //UbeatGame.Instance.SpriteBatch.Draw(this.Texture, rg, Color.White);
            Rectangle mrg = new Rectangle((int)MouseHandler.GetState().X, (int)MouseHandler.GetState().Y, 10, 10);

            for (int i = 0; i < maxCanHold + 1 && i + vertOffset < Items.Count; i++)
            {
                string o2 = Items[i + vertOffset].Title;


                string oartistDrw = Items[i + vertOffset].Artist;
                string artistDrw = "";
                string omapCreator = Items[i + vertOffset].Creator;
                string mapCreator = "";


                string o = "";
                for (int a = horizontalOffset; a < o2.Length && a < maxCharsCanHold + horizontalOffset; a++)
                {
                    o += o2[a];
                }

                for (int a = horizontalOffset; a < oartistDrw.Length && a < maxCharsCanHold + horizontalOffset; a++)
                {
                    artistDrw += oartistDrw[a];
                }

                for (int a = horizontalOffset; a < omapCreator.Length && a < maxCharsCanHold + horizontalOffset; a++)
                {
                    mapCreator += omapCreator[a];
                }


                Color textColor = Color.FromNonPremultiplied(155, 155, 155, 255);
                Color backColor = Color.Black;
                Color selectedColor = Color.Brown;

                bool drawFront = true;
                Vector2 fBox = drawTextPos;

                byte alpha = 200;
                if (i % 2 == 0)
                    alpha = 255;
                if (i + vertOffset == selectedIndex)
                {
                    textColor = Color.Yellow;
                    //fBox += new Vector2(20, 0);
                    drawFront = false;
                }

                int middle = maxCanHold / 2;
                if(i > middle)
                {
                    fBox += new Vector2(20 * (maxCanHold - i), 0);
                }
                else
                {
                    fBox += new Vector2(20 * i, 0);
                }

                fBox -= new Vector2(10 , 0);

                Vector2 drTxPs = new Vector2(fBox.X + 30, fBox.Y + 10);

                float pscale = 1;

                if(mrg.Intersects(new Rectangle((int)fBox.X, (int)fBox.Y, SpritesContent.Instance.ScrollListBeatmap_alt.Width, SpritesContent.Instance.ScrollListBeatmap_alt.Height))){
                    pscale = 1.01f;
                }

                
                KyunGame.Instance.SpriteBatch.Draw(SpritesContent.Instance.ScrollListBeatmap_alt, fBox, null, Color.White, 0, Vector2.Zero, pscale ,SpriteEffects.None , 0);
                if (drawFront)
                    //KyunGame.Instance.SpriteBatch.Draw(SpritesContent.Instance.ScrollListBeatmap, fBox, Color.White*0.8f);
                    KyunGame.Instance.SpriteBatch.Draw(SpritesContent.Instance.ScrollListBeatmap, fBox, null, Color.White, 0, Vector2.Zero, pscale, SpriteEffects.None, 0);


                KyunGame.Instance.SpriteBatch.DrawString(textFont, o, drTxPs, textColor, 0f, Vector2.Zero, Scale * pscale, SpriteEffects.None, 0);
                KyunGame.Instance.SpriteBatch.DrawString(textFont, artistDrw, drTxPs + new Vector2(10, 25), textColor, 0f, Vector2.Zero, Scale * .85f * pscale, SpriteEffects.None, 0);
                KyunGame.Instance.SpriteBatch.DrawString(textFont, mapCreator, drTxPs + new Vector2(10, 25*2-5), textColor, 0f, Vector2.Zero, Scale * .75f * pscale, SpriteEffects.None, 0);

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
                    //Vector2 size = textFont.MeasureString(r);
                    //float w = size.X;
                    float w = measuredTextWidth;
                    if (w > maxWidth)
                        maxWidth = w;
                }
                maxWidth += 5;
                if (maxWidth > 5)
                {

                }
            }

            if (oldSel != selectedIndex)
            {
                oldSel = selectedIndex;
                if (IndexChanged != null)
                {                    
                    vertOffset = Math.Max(selectedIndex - (maxCanHold / 2), 0);
                    IndexChanged(this, new EventArgs());
                }
            }


            //oldSel = selectedIndex;
            if (vertOffset < -1)
                vertOffset = 0;
        }
    }
}
