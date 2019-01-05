using Microsoft.Xna.Framework;
using System;
using kyun.GameScreen;
using kyun.GameScreen.UI;
using kyun.Utils;

namespace kyun.Notifications
{
    public class NotificationBox : UIObjectBase, IDisposable
    {
        public Rectangle Size { get { return rectng.Texture.Bounds; } }

        bool leaving = false;

        public NotificationBox(string text, int milliseconds = 5000, NotificationType type = NotificationType.Info)
        {
            displayLabel = new Label(0);
            
            displayLabel.Scale = .65f;

            Vector2 MeasuredString = SpritesContent.Instance.DefaultFont.MeasureString(text) * displayLabel.Scale;

            Text = StringHelper.WrapText(SpritesContent.Instance.DefaultFont, text, 200, .65f);

            displayLabel.Text = Text;

            displayLabel.ForegroundColor = Color.FromNonPremultiplied(232, 232, 232, 255);

            Type = type;

            Color notificationColor = Color.Transparent;

            switch(type)
            {
                case NotificationType.Info:
                    notificationColor = Color.CornflowerBlue;
                    break;
                case NotificationType.Warning:
                    notificationColor = Color.LightGoldenrodYellow;
                    break;
                case NotificationType.Critical:
                    notificationColor = Color.Red;
                    break;
            }

            /*
            rectng = 
                new RoundedRectangle(
                    new Vector2(200, (MeasuredString.Y * (Text.Split('\n').Length) + 20)),
                        Color.Black * .75f,
                        4,
                        4,
                        notificationColor
                    );*/

            rectng = new FilledRectangle(new Vector2(200, (MeasuredString.Y * (Text.Split('\n').Length) + 20)), Color.Black * .8f);


            rectBorder = new FilledRectangle(new Vector2(4, rectng.Texture.Height), notificationColor);

           // rectBorder.Texture = SpritesContent.RoundCorners(rectBorder.Texture, 1, 1);
           
            RemainingMilliseconds = milliseconds;

            rectng.Click += Rectng_Click;
            displayLabel.Click += DisplayLabel_Click;

        }

        private void NotificationBox_Click(object sender, System.EventArgs e)
        {
        }

        private void DisplayLabel_Click(object sender, System.EventArgs e)
        {

        }

        private void Rectng_Click(object sender, System.EventArgs e)
        {
            Click?.Invoke(this, e); //Fuck this again
        }

        public new void FadeIn(AnimationEffect effect, int duration, Action complete) {
            rectng.FadeOut(effect, 400, complete);
            rectBorder.FadeOut(effect, 400);
            displayLabel.FadeOut(effect, 400);
        }

        public new void FadeOut(AnimationEffect effect, int duration, Action complete)
        {
            rectng.FadeOut(effect, 400, complete);
            rectBorder.FadeOut(effect, 400);
            displayLabel.FadeOut(effect, 400);
        }

        public override void Update()
        {
            base.Update(); //Events

            if (!Visible) return;

            RemainingMilliseconds -= KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds;
            
            if(RemainingMilliseconds < 0)
            {
                if (!leaving)
                {
                    FadeOut(AnimationEffect.Linear, 400, () => {
                        Dispose();
                    });
                   
                    leaving = true;
                } 
            }
                

            rectng.Position = Position;
            displayLabel.Position = new Vector2(Position.X + 10, Position.Y + 5);
            rectBorder.Position = new Vector2(rectng.Position.X - rectBorder.Texture.Width, rectng.Position.Y);

            rectng.Update();
            rectBorder.Update();
            displayLabel.Update();

        }

        public override void Render()
        {
            if (!Visible) return;
            rectng.Render();
            rectBorder.Render();
            displayLabel.Render();
        }

        public override void Dispose()
        {
            Disposing = true;
            this.Visible = false;
            rectng?.Texture?.Dispose();
            displayLabel?.Texture?.Dispose();
        }

        public int ActualPosition { get; set; }
        public int RemainingMilliseconds { get; set; }
        private string Text { get; }
        public NotificationType Type { get; set; }

        private FilledRectangle rectng;
        private Label displayLabel;
        private FilledRectangle rectBorder;

        public new event EventHandler Click;
    }

    public enum NotificationType
    {
        Info,
        Warning,
        Critical
    }
}