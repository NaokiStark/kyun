﻿using kyun.game.GameScreen.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kyun.GameScreen.UI
{
    public class ComboBox : InputControl
    {
        //Well, display label
        UI.Label displayLabel;

        //And listbox (and make a combo (genius))
        ObjectListbox baseListbox;


        public event EventHandler IndexChaged;

        public string Text
        {
            get
            {
                return displayLabel.Text;
            }
            set
            {
                displayLabel.Text = value;
            }
        }

        public bool IsListVisible
        {
            get
            {
                return baseListbox.Visible;
            }

        }

        public int SelectedIndex
        {
            get
            {
                return baseListbox.selectedIndex;
            }
            set
            {
                baseListbox.selectedIndex = value;
            }
        }

        public object SelectedItem
        {
            get
            {

                try
                {
                    if (SelectedIndex < 0)
                        return null;

                    object selectd = baseListbox.Items[SelectedIndex];
                    return selectd;
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<object> Items
        {
            get
            {
                return baseListbox.Items;
            }
            set
            {
                baseListbox.Items = value;
            }
        }

        public new Vector2 Position
        {
            get
            {
                return displayLabel.Position;
            }
            set
            {
                displayLabel.Position = value;
            }
        }

        public int Width;

        public new Tooltip Tooltip
        {
            get
            {
                return baseListbox.Tooltip;
            }
            set
            {
                baseListbox.Tooltip = displayLabel.Tooltip = value;
            }
        }

        public ComboBox(Vector2 position, float width, SpriteFont font)
        {
            Width = (int)width;
            //TODO: hard-coded heigth
            baseListbox = new ObjectListbox(position, width, 200, font);
            baseListbox.Visible = false;

            displayLabel = new UI.Label(0.8f);
            displayLabel.Position = position;
            displayLabel.Font = font;
            //TODO STYLING

            baseListbox.IndexChanged += BaseListbox_IndexChanged;
            displayLabel.Click += DisplayLabel_Click;
        }

        private void DisplayLabel_Click(object sender, EventArgs e)
        {

            baseListbox.Position = new Vector2(displayLabel.Position.X, displayLabel.Position.Y + displayLabel.Texture.Height);
            baseListbox.Visible = !baseListbox.Visible;

            _OnClick();
        }

        private void BaseListbox_IndexChanged(object sender, EventArgs e)
        {
            baseListbox.Visible = false;

            object itm = SelectedItem;
            if (SelectedItem == null) return;

            displayLabel.Text = SelectedItem.ToString();
            IndexChaged?.Invoke(this, new EventArgs());
        }

        public override void Render()
        {
            displayLabel.Render();
            if (baseListbox.Visible)
            {
                baseListbox.Render();
            }

        }

        public override void Update()
        {
            displayLabel.Update();
            if (baseListbox.Visible)
            {
                baseListbox.width = displayLabel.TotalSize.X + 60;

            }
            baseListbox.Update();
        }
    }
}
