using Microsoft.Xna.Framework;
using System.Collections.Generic;
using ubeat.GameScreen;
using ubeat.Screen;

namespace ubeat.Notifyer
{
    public class Notifyer : ScreenUIObject
    {

        List<NotificationBox> Notifications = new List<NotificationBox>();

        public Notifyer()
        {

        }

        #region Events

        public override void Update()
        {
           // base.Update(); //Events

            if (Notifications.Count < 1) return;

            ScreenMode mode = ScreenModeManager.GetActualMode();


            foreach (NotificationBox box in Notifications)
            {
                box.Position = new Vector2(mode.Width - box.Size.Width-5, mode.Height - box.Size.Height-5);

                box.Update();
            }
                
        }

        public override void Render()
        {
            foreach (NotificationBox box in Notifications)
                box.Render();
        }
        
        #endregion

        public void ShowDialog(string text, int milliseconds = 5000)
        {
            Notifications.Add(new NotificationBox(text, milliseconds));
        }

    }
}