using Gma.System.MouseKeyHook;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace kyun.Utils
{
    public static class MouseHandler
    {

        static ButtonState MiddleButton = ButtonState.Released;
        static ButtonState LeftButton = ButtonState.Released;
        static ButtonState RightButton = ButtonState.Released;
        static Vector2 Position = Vector2.Zero;
        static int ScrollWheelValue = 0;

        public static void SetMousePosWinFrm(object obj, System.Windows.Forms.MouseEventArgs args)
        {
            /*
            System.Drawing.Point clientPoint = KyunGame.WinForm.PointToClient(args.Location);

            Position.X = clientPoint.X;
            Position.Y = clientPoint.Y;*/
        }

        public static void SetMouseWheelPos(object obj, System.Windows.Forms.MouseEventArgs args)
        {
            ScrollWheelValue += args.Delta / 10;

            if (ScrollWheelValue > 365)
            {
                ScrollWheelValue = 0;
            }
            else if (ScrollWheelValue < -365)
            {
                ScrollWheelValue = 0;
            }
        }

        public static void setMouseDownStateWinform(object obj, System.Windows.Forms.MouseEventArgs args)
        {

            //Logger.Instance.Info($"Mouse pressed keys: {args.Button}");
            if (args.Button == System.Windows.Forms.MouseButtons.Left)
            {
                LeftButton = ButtonState.Pressed;
            }
            else
            {
                LeftButton = ButtonState.Released;
            }

            if (args.Button == System.Windows.Forms.MouseButtons.Right)
            {
                RightButton = ButtonState.Pressed;
            }
            else
            {
                RightButton = ButtonState.Released;
            }

            if (args.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                MiddleButton = ButtonState.Pressed;
            }
            else
            {
                MiddleButton = ButtonState.Released;
            }

            System.Drawing.Point clientPoint = KyunGame.WinForm.PointToClient(args.Location);

            Position.X = clientPoint.X;
            Position.Y = clientPoint.Y;

        }

        public static void setMouseUpStateWinform(object obj, System.Windows.Forms.MouseEventArgs args)
        {

            //Logger.Instance.Info($"Mouse pressed keys: {args.Button}");
            if (args.Button == System.Windows.Forms.MouseButtons.Left)
            {
                LeftButton = ButtonState.Released;
            }

            if (args.Button == System.Windows.Forms.MouseButtons.Right)
            {
                RightButton = ButtonState.Released;
            }


            if (args.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                MiddleButton = ButtonState.Released;
            }

            System.Drawing.Point clientPoint = KyunGame.WinForm.PointToClient(args.Location);

            Position.X = clientPoint.X;
            Position.Y = clientPoint.Y;

        }

       

        
        public static MouseEvent GetState()
        {

            if (KyunGame.RunningOverWine)
            {
                return WineHandler();
            }
#if WINDOWS
            return getWindowsApiPoint();
#else

            MouseState actualState = Mouse.GetState();
            var actualMode = Screen.ScreenModeManager.GetActualMode();

            return new MouseEvent()
            {
                Position = new Vector2(actualState.X * ((float)actualMode.Width / (float)actualMode.ScaledWidth), actualState.Y * ((float)actualMode.Height / (float)actualMode.ScaledHeight)),
                LeftButton = actualState.LeftButton,
                RightButton = actualState.RightButton,
                MiddleButton = actualState.MiddleButton,
                X = actualState.X * ((float)actualMode.Width / (float)actualMode.ScaledWidth),
                Y = actualState.Y * ((float)actualMode.Height / (float)actualMode.ScaledHeight),
                ScrollWheelValue = actualState.ScrollWheelValue
            };
#endif
        }

        public static MouseEvent GetStateNonScaled()
        {
            if (KyunGame.RunningOverWine)
            {
                return WineHandler();
            }

#if WINDOWS
            return getWindowsApiPointNotScaled();
#else

            MouseState actualState = Mouse.GetState();
            var actualMode = Screen.ScreenModeManager.GetActualMode();

            return new MouseEvent()
            {
                Position = new Vector2(actualState.X, actualState.Y),
                LeftButton = actualState.LeftButton,
                RightButton = actualState.RightButton,
                MiddleButton = actualState.MiddleButton,
                X = actualState.X,
                Y = actualState.Y,
                ScrollWheelValue = actualState.ScrollWheelValue
            };
#endif
        }

        private static MouseEvent WineHandler()
        {
            return new MouseEvent()
            {
                MiddleButton = MiddleButton,
                LeftButton = LeftButton,
                RightButton = RightButton,
                Position = Position,
                X = Position.X,
                Y = Position.Y,
                ScrollWheelValue = ScrollWheelValue
            };
        }
#if WINDOWS
        // use of raw input (Windows api)

        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int virtualKeyCode);


        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            // NOTE: If you need error handling
            // bool success = GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }

        public static bool IsMouseDown(int vKey)
        {
            return 0 != (GetAsyncKeyState(vKey) & 0x8000);
        }

        public static void UpdateCursor()
        {

            Point windowPosition = KyunGame.Instance.Window.Position;
            Point curPos = GetCursorPosition();
            Vector2 actualState = curPos.ToVector2() - windowPosition.ToVector2();

            MiddleButton = IsMouseDown(0x04) ? ButtonState.Pressed : ButtonState.Released;
            LeftButton = IsMouseDown(0x01) ? ButtonState.Pressed : ButtonState.Released;
            RightButton = IsMouseDown(0x02) ? ButtonState.Pressed : ButtonState.Released;
            Position = actualState;            
        }

        private static MouseEvent getWindowsApiPoint()
        {
            MouseEvent actualState = WineHandler();
            var actualMode = Screen.ScreenModeManager.GetActualMode();

            return new MouseEvent
            {
                Position = new Vector2(actualState.X * ((float)actualMode.Width / (float)actualMode.ScaledWidth), actualState.Y * ((float)actualMode.Height / (float)actualMode.ScaledHeight)),
                LeftButton = actualState.LeftButton,
                RightButton = actualState.RightButton,
                MiddleButton = actualState.MiddleButton,
                X = actualState.X * ((float)actualMode.Width / (float)actualMode.ScaledWidth),
                Y = actualState.Y * ((float)actualMode.Height / (float)actualMode.ScaledHeight),
                ScrollWheelValue = ScrollWheelValue,
            };
            /*
            Point windowPosition = KyunGame.Instance.Window.Position;
            Point curPos = GetCursorPosition();
            Vector2 actualState = curPos.ToVector2() - windowPosition.ToVector2();
            var actualMode = Screen.ScreenModeManager.GetActualMode();           


            return new MouseEvent
            {
                Position = new Vector2(actualState.X * ((float)actualMode.Width / (float)actualMode.ScaledWidth), actualState.Y * ((float)actualMode.Height / (float)actualMode.ScaledHeight)),
                LeftButton = IsMouseDown(0x01) ? ButtonState.Pressed:ButtonState.Released,
                RightButton = IsMouseDown(0x02) ? ButtonState.Pressed : ButtonState.Released,
                MiddleButton = IsMouseDown(0x04) ? ButtonState.Pressed : ButtonState.Released,
                X = actualState.X * ((float)actualMode.Width / (float)actualMode.ScaledWidth),
                Y = actualState.Y * ((float)actualMode.Height / (float)actualMode.ScaledHeight),
                ScrollWheelValue = ScrollWheelValue,
            };*/
        }

        private static MouseEvent getWindowsApiPointNotScaled()
        {
            return WineHandler();
            /*
            Point windowPosition = KyunGame.Instance.Window.Position;
            Point curPos = GetCursorPosition();
            Vector2 actualState = curPos.ToVector2() - windowPosition.ToVector2();
            

            return new MouseEvent
            {
                Position = actualState,
                LeftButton = IsMouseDown(0x01) ? ButtonState.Pressed : ButtonState.Released,
                RightButton = IsMouseDown(0x02) ? ButtonState.Pressed : ButtonState.Released,
                MiddleButton = IsMouseDown(0x04) ? ButtonState.Pressed : ButtonState.Released,
                X = actualState.X,
                Y = actualState.Y,
                ScrollWheelValue = ScrollWheelValue,
            };*/
        }
#endif
    }



    public class MouseEvent
    {
        public Vector2 Position { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public ButtonState MiddleButton { get; set; }
        public ButtonState LeftButton { get; set; }
        public ButtonState RightButton { get; set; }
        public int ScrollWheelValue { get; set; }

    }
}
