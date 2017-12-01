using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using kyun.GameScreen;
using kyun.Screen;

namespace kyun.Notifications
{
    public class Notifier : UIObjectBase
    {
        public Notifier()
        {
            Notifications = new List<NotificationBox>();
        }

        #region Events

        public override void Update()
        {
           // base.Update(); //Events

            if (Notifications.Count < 1) return;

            ScreenMode mode = ScreenModeManager.GetActualMode();


            for(int a = 0; a<Notifications.Count; a++)
            {

                NotificationBox box = Notifications[a];

                int boxPositionEnd = mode.Width - box.Size.Width - 5;

                if (box.ActualPosition - KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 0.02 > boxPositionEnd)
                {
                    box.ActualPosition -= (int)(KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 1.5);

                }
                else
                {
                    box.ActualPosition = mode.Width - box.Size.Width - 5;
                }
                

                box.Position = new Vector2(box.ActualPosition, mode.Height - box.Size.Height-5);

                box.Update();
            }
                
        }

        public override void Render()
        {
            foreach (NotificationBox box in Notifications)
                box.Render();
        }
        
        #endregion

        public void ShowDialog(string text, int milliseconds = 5000, NotificationType type = NotificationType.Info)
        {
            var ntbox = new NotificationBox(text, milliseconds, type);

            ScreenMode mode = ScreenModeManager.GetActualMode();

            ntbox.ActualPosition = mode.Width;

            ntbox.Click += (send, args) =>
            {
                Logger.Instance.Debug($"Showing notification: {text}");

                Notifications.Remove(((NotificationBox)send));

                ((NotificationBox)send).Dispose();
            };

            Notifications.Add(ntbox);
        }

        private List<NotificationBox> Notifications;
    }
}