using Microsoft.Xna.Framework;
using ubeat.GameScreen;
using ubeat.GameScreen.SUI;
using ubeat.Utils;

namespace ubeat.Notifyer
{
    public class NotificationBox : ScreenUIObject
    {

        string Text { get; }
        int millisecondsRemain { get; set; }

        public Rectangle Size
        {
            get
            {
                return rectng.Texture.Bounds;
            }
            
        }

        RoundedRectangle rectng;
        GameScreen.UI.Label displayLabel;

        public NotificationBox(string text, int milliseconds = 5000)
        {
            displayLabel = new GameScreen.UI.Label(0);
            
            displayLabel.Scale = .65f;

            Vector2 MeasuredString = UbeatGame.Instance.defaultFont.MeasureString(text) * displayLabel.Scale;

            Text = StringHelper.WrapText(UbeatGame.Instance.defaultFont, text, 200, .65f);

            displayLabel.Text = Text;

            rectng = new RoundedRectangle(
                new Vector2(200, (MeasuredString.Y * (Text.Split('\n').Length) + 20)),
                Color.Black * .75f,
                4,
                4,
                Color.CornflowerBlue
                );

           
            millisecondsRemain = milliseconds;

            rectng.Click += Rectng_Click;
            displayLabel.Click += DisplayLabel_Click;
        }

        private void DisplayLabel_Click(object sender, System.EventArgs e)
        {

        }

        private void Rectng_Click(object sender, System.EventArgs e)
        {

            

        }

        public override void Update()
        {
            //base.Update(); //Events

            millisecondsRemain -= UbeatGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;

            rectng.Position = Position;
            displayLabel.Position = new Vector2(Position.X+10, Position.Y + 5);

            rectng.Update();
            displayLabel.Update();

        }

        public override void Render()
        {
            rectng.Render();
            displayLabel.Render();
        }
    }
}