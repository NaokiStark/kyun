using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using kyun.GameScreen;
using kyun.Screen;
using kyun.Audio;
using kyun.Utils;

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


            for (int a = Notifications.Count - 1; a > -1; a--)
            {

                NotificationBox box = Notifications[a];

                int boxposition = mode.Height - box.Size.Height - 5;

                int boxPositionEnd = mode.Width - box.Size.Width - 5;

                if (box.ActualPosition - KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 0.02 > boxPositionEnd)
                {
                    box.ActualPosition -= (int)(KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 1.5);

                }
                else
                {
                    box.ActualPosition = mode.Width - box.Size.Width - 5;
                }

                int lastSize = (a - 1 > -1) ? Notifications[a - 1].Size.Height * a : 0;


                box.Position = new Vector2(box.ActualPosition, boxposition - lastSize - 5 * a);

                box.Update();

            }

            Notifications.RemoveAll(x => x.Disposing);
        }

        public override void Render()
        {
            foreach (NotificationBox box in Notifications)
                box.Render();
        }

        #endregion

        public void ShowDialog(string text, int milliseconds = 5000, NotificationType type = NotificationType.Info, Action callback = null)
        {
            var ntbox = new NotificationBox(text, milliseconds, type);

            ScreenMode mode = ScreenModeManager.GetActualMode();

            ntbox.ActualPosition = mode.Width;

#if DEBUG
            ntbox.Click += (send, args) =>
            {
                Logger.Instance.Debug($"Showing notification: {text}");

                Notifications.Remove(((NotificationBox)send));

                ((NotificationBox)send).Dispose();
            };
#endif
            ntbox.Click += (s, arg) =>
            {
                callback?.Invoke();
            };

            Notifications.Add(ntbox);
            EffectsPlayer.PlayEffect(SpritesContent.Instance.NotificationSound);
        }

        private List<NotificationBox> Notifications;
    }
}