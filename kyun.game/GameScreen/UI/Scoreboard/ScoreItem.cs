using kyun.Beatmap;
using kyun.GameScreen;
using kyun.GameScreen.UI;
using kyun.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.GameScreen.UI.Scoreboard
{
    public class ScoreItem : UIObjectBase
    {

        public Label UserCaption { get; set; }
        public Label Score { get; set; }
        public Label Combo { get; set; }
        public Image Avatar { get; set; }
        public int ScoreInt { get; set; }
        public int ComboInt { get; set; }
        public int Row { get; set; }

        List<UIObjectBase> controls { get; set; }

        public ScoreItem()
        {
            controls = new List<UIObjectBase>();
            UserCaption = new Label(0)
            {
                Text = "",
                Position = new Vector2()
            };

            Score = new Label(0)
            {
                Text = "0",
                Position = new Vector2(),
                Font = SpritesContent.Instance.ListboxFont
            };

            Combo = new Label(0)
            {
                Text = "0x",
                Position = new Vector2(),
                Font = SpritesContent.Instance.GeneralBig
            };

            Avatar = new Image(SpritesContent.Instance.Catcher);
            Avatar.Size = new Vector2(40);
            Avatar.Texture = SpritesContent.RoundCorners(Avatar.Texture, 5);
            Avatar.BeatReact = false;

            controls.Add(Combo);
            controls.Add(Score);
            controls.Add(UserCaption);
            controls.Add(Avatar);
        }

        public override void Update()
        {
            if (!Visible)
                return;

            base.Update();

            Score.Text = ScoreInt.ToString("#,##0");
            Combo.Text = ComboInt.ToString("#,##0") + "x";

            Avatar.Size = new Vector2(50);

            UserCaption.Scale = .8f;
            Score.Scale = .6f;

            Avatar.Position = new Vector2(Position.X, Position.Y);
            UserCaption.Position = new Vector2(Position.X + Avatar.Size.X + 5, Position.Y - 8);

            Score.Position = new Vector2(Position.X + Avatar.Size.X + 5, Position.Y + Avatar.Size.Y - Score.Texture.Height + 2);
            Combo.Position = new Vector2(Position.X + Scoreboard.Instance.Texture.Width - Combo.Texture.Width - 30, Position.Y + Score.Texture.Height + 2);

            foreach (UIObjectBase ctrl in controls)
            {
                ctrl?.Update();
            }
        }

        public override void Render()
        {
            if (!Visible)
                return;

            base.Render();
            foreach (UIObjectBase ctrl in controls)
            {
                ctrl?.Render();
            }
        }

        public static ScoreItem BuildNew(Texture2D avatar, string caption, int scoreInt, int comboInt)
        {
            var tmp = new ScoreItem
            {
                ScoreInt = scoreInt,
                ComboInt = comboInt,
                Visible = false
            };

            tmp.Avatar.Texture = SpritesContent.RoundCorners(avatar, 5);
            tmp.UserCaption.Text = caption;
            tmp.Visible = true;
            return tmp;
        }

        public static List<ScoreItem> GetFromDb(ubeatBeatMap beatmap)
        {
            var scors = Database.DatabaseInterface.Instance.GetScoresFor(beatmap);
            scors = scors.OrderByDescending(x => x.Score).ToList();

            var scoresList = new List<ScoreItem>();
            if (scors == null)
            {
                return scoresList;
            }

            if (scors.Count < 1)
            {
                return scoresList;
            }

            var userBox = UserBox.GetInstance();
            foreach (Score.ScoreInfo inf in scors)
            {
                scoresList.Add(BuildNew(userBox.userAvatar.Texture, inf.Username, inf.Score, inf.Combo));
            }

            return scoresList;
        }

        public static List<ScoreItem> Parse(JArray data, Scoreboard scoreboard, int row = 1)
        {
            var tmp = new List<ScoreItem>();



            foreach (JObject obj in data)
            {

                var tmpItem = new ScoreItem
                {
                    ComboInt = (int)obj["combo"],
                    ScoreInt = (int)obj["score"]

                };

                tmpItem.Row = row;

                var firstPos = new Vector2(scoreboard.Position.X, scoreboard.Position.Y);

                var finalPos = new Vector2(firstPos.X, firstPos.Y * row);

                tmpItem.Avatar = new Image(SpritesContent.RoundCorners(UserBox.GetInstance().userAvatar.Texture, 5))
                {
                    Position = finalPos
                };


                tmp.Add(tmpItem);
                row++;
            }

            return tmp;
        }
    }
}
