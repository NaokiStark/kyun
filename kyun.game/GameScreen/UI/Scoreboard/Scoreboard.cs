using kyun.GameScreen;
using kyun.GameScreen.UI;
using kyun.Utils;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.GameScreen.UI.Scoreboard
{
    public class Scoreboard : UIObjectBase
    {
        static Scoreboard instance;

        public Label noScoresLabel;
        public List<ScoreItem> Items;
        public int MaxItems = 6;

        public static int MARGIN = 20;

        public static Scoreboard Instance
        {
            get
            {
                if (instance == null)
                    instance = new Scoreboard();

                return instance;
            }
        }

        public Scoreboard()
        {
            noScoresLabel = new Label(0)
            {
                Text = "No scores",
                Position = new Vector2(Position.X + MARGIN, Position.Y + MARGIN)
            };

            Items = new List<ScoreItem>();
            Texture = SpritesContent.Instance.SongDescBox;
        }

        /// <summary>
        /// Add new item to scoreboad, if is full, then overrides last item
        /// </summary>
        /// <param name="item"></param>
        public void Add(ScoreItem item)
        {

            item.Position = new Vector2(Position.X, Position.Y * ((Items.Count > 0) ? Items.Count : 1));
            item.Row = ((Items.Count > 0) ? Items.Count : 1);

            if (Items.Count >= MaxItems)
            {
                Items[Items.Count - 1] = item;
            }
            else
            {
                Items?.Add(item);
            }

        }

        /// <summary>
        /// Overrides item row with new item 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="row"></param>
        public void Add(ScoreItem item, int row)
        {
            if (row > Items.Count - 1)
            {
                throw new Exception("Row Not Found, please, use \"Add(Scoreitem)\" Method instead");
            }

            Items[row] = item;
            //Items?.Add(item);
        }

        public void AddList(List<ScoreItem> list)
        {
            foreach(ScoreItem it in list)
            {
                Add(it);
            }
        }

        public void Populate(JArray data)
        {
            Items.Clear();
            Items = ScoreItem.Parse(data, this);
        }

        public void Clear()
        {
            Items.Clear();
        }

        public override void Update()
        {
            if (!Visible)
                return;

            base.Update();
            noScoresLabel.Position = new Vector2(Position.X + MARGIN * 2, Position.Y + MARGIN);
            noScoresLabel.Centered = false;
            noScoresLabel.Update();

            for (int r = 0; r < Items.Count; r++)
            {
                
                Items[r].Position = new Vector2(Position.X + MARGIN, (Position.Y + MARGIN) + (Items[r].Avatar.Size.Y + MARGIN / 2) * r); //temp

                Items[r].Update();
            }
        }

        public override void Render()
        {
            if (!Visible)
                return;
            base.Render();
            foreach (ScoreItem sItem in Items)
            {
                sItem.Render();
            }

            if (Items.Count < 1)
            {
                noScoresLabel.Render();
            }

        }
    }
}
