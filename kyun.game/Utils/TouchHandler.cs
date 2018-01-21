using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using kyun.GameScreen;
using kyun.GameScreen.UI;
using Windows7.Multitouch;

namespace kyun.Utils
{
    public class TouchHandler : UIObjectBase
    {

        public delegate void TouchEventHandler(object sender, ubeatTouchEventArgs e);
        public event TouchEventHandler onTouchScreen;

        static TouchHandler instance = null;

        List<TouchPoint> points = new List<TouchPoint>();

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

            if (KyunGame.RunningOverWine)
                return;

            touchDown = false;
            touchUp = true;

            this.handle = handle;

            touchHandler = Windows7.Multitouch.WinForms.Factory.CreateHandler<Windows7.Multitouch.TouchHandler>(handle);
            touchHandler.TouchDown += TouchHandler_TouchDown;
            touchHandler.TouchUp += TouchHandler_TouchUp;
            touchHandler.TouchMove += TouchHandler_TouchMove;
            
           
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
            removePoint(new TouchPoint
            {
                Id = e.Id,
                Location = new Vector2(e.Location.X * screenScaling, e.Location.Y * screenScaling)
            });
        }

        private void TouchHandler_TouchMove(object sender, Windows7.Multitouch.TouchEventArgs e)
        {
            movePoint(new TouchPoint
            {
                Id = e.Id,
                Location = new Vector2(e.Location.X * screenScaling, e.Location.Y * screenScaling)
            });
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

            TouchPoint tcp = new TouchPoint
            {
                Location = LastPosition,
                Id = e.Id
            };

            addPoint(tcp);

            onTouchScreen?.Invoke(this, new ubeatTouchEventArgs {
                Point = tcp
            });
            
        }

        private void addPoint(TouchPoint tcp)
        {
            bool hasPoint = false;
            for (int a = 0; a < points.Count; a++)
            {
                if (points[a].Id == tcp.Id)
                {
                    hasPoint = true;

                    movePoint(tcp);

                }
            }

            if (!hasPoint){
                points.Add(tcp);
            }
        }

        private void movePoint(TouchPoint tcp)
        {
            for (int a = 0; a < points.Count; a++)
            {
                if(points[a].Id == tcp.Id)
                {
                    points[a].Location = tcp.Location;
                }
            }
        }

        private void removePoint(TouchPoint tcp)
        {
            for (int a = 0; a < points.Count; a++)
            {
                if(points[a].Id == tcp.Id)
                {
                    points.Remove(points[a]);
                }
            }
        }

        public bool TouchIntersecs(Rectangle rg)
        {
            if (KyunGame.RunningOverWine)
                return false;
            for(int a = 0; a < points.Count; a++)
            {
                Rectangle touchBox = new Rectangle((int)points[a].Location.X, (int)points[a].Location.Y, 10, 10);

                if (touchBox.Intersects(rg))
                {
                    return true;
                }

            }

            return false;
        }

        public int GetPointsCount()
        {
            return points.Count;
        }

        public List<TouchPoint> GetAllPointsIntersecs(Rectangle rg)
        {
            if (KyunGame.RunningOverWine)
                return new List<TouchPoint>();

            var touchpnt = new List<TouchPoint>();

            for (int a = 0; a < points.Count; a++)
            {
                Rectangle touchBox = new Rectangle((int)points[a].Location.X, (int)points[a].Location.Y, 10, 10);

                if (touchBox.Intersects(rg))
                {
                    touchpnt.Add(points[a]);
                }
            }
            return touchpnt;
        }

        public TouchPoint GetTouchIntersecs(Rectangle rg)
        {
            if (KyunGame.RunningOverWine)
                return new TouchPoint();

            for (int a = 0; a < points.Count; a++)
            {
                Rectangle touchBox = new Rectangle((int)points[a].Location.X, (int)points[a].Location.Y, 10, 10);

                if (touchBox.Intersects(rg))
                {
                    return points[a];
                }

            }

            return null;
        }

        private float getScalingFactor()
        {
            float ScreenScalingFactor = 1;
            ScreenScalingFactor = touchHandler.DpiX / 96;
            return ScreenScalingFactor;
        }
    }

    public class TouchPoint
    {
        public Vector2 Location { get; set; }
        public int Id { get; set; }
    }

    public class ubeatTouchEventArgs
    {
        public TouchPoint Point { get; set; }
    }
}
