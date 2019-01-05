using Gma.System.MouseKeyHook;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
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
            
            System.Drawing.Point clientPoint = KyunGame.WinForm.PointToClient(args.Location);

            Position.X = clientPoint.X;
            Position.Y = clientPoint.Y;
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
        }

        public static MouseEvent GetStateNonScaled()
        {
            if (KyunGame.RunningOverWine)
            {
                return WineHandler();
            }

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
