using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using ubeat.GameScreen;
using ubeat.GameScreen.UI;
using Windows7.Multitouch;

namespace ubeat.Utils
{
    public class TouchHandler : UIObjectBase
    {

        static TouchHandler instance = null;

        public static TouchHandler Instance
        {
            get
            {
                return instance;
            }
        }

        public bool TouchDown
        {
            get
            {
                return touchDown;
            }

            set
            {
                touchDown = value;
            }
        }

        public bool TouchUp
        {
            get
            {
                return touchUp;
            }

            set
            {
                touchUp = value;
            }
        }

        private bool touchDown;
        private bool touchUp;
        public Vector2 LastPosition { get; set; }

        private float screenScaling = 1;

        private System.Windows.Forms.Control handle;
        private Windows7.Multitouch.TouchHandler touchHandler;

        private RoundedRectangle roundedRect;

        public TouchHandler(System.Windows.Forms.Control handle)
        {

            roundedRect = new RoundedRectangle(new Vector2(10, 10), Color.Green, 1, 1, Color.Transparent);
            roundedRect.Visible = false;
            roundedRect.IsActive = true;

            touchDown = false;
            touchUp = true;

            this.handle = handle;

            touchHandler = Windows7.Multitouch.WinForms.Factory.CreateHandler<Windows7.Multitouch.TouchHandler>(handle);
            touchHandler.TouchDown += TouchHandler_TouchDown;
            touchHandler.TouchUp += TouchHandler_TouchUp;
            screenScaling = getScalingFactor();

        }

        public override void Update()
        {
#if DEBUG
            roundedRect?.Update();
#endif
        }

        public override void Render()
        {
#if DEBUG
            roundedRect?.Render();
#endif
        }

        private void TouchHandler_TouchUp(object sender, Windows7.Multitouch.TouchEventArgs e)
        {
            touchDown = false;
            touchUp = true;
            roundedRect.Visible = false;
        }

        private void TouchHandler_TouchDown(object sender, Windows7.Multitouch.TouchEventArgs e)
        {
            float pX = e.Location.X;
            float pY = e.Location.Y;

            LastPosition = new Vector2(pX * screenScaling, pY * screenScaling);

            touchDown = true;
            touchUp = false;
            roundedRect.Visible = true;
            roundedRect.Position = LastPosition;
        }

        private float getScalingFactor()
        {
            float ScreenScalingFactor = 1;
            ScreenScalingFactor = touchHandler.DpiX / 96;
            return ScreenScalingFactor;
        }
    }

}
