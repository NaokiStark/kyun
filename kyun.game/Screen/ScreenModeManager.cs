using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.game;
using kyun.game.Screen;

namespace kyun.Screen
{
    public class ScreenModeManager
    {
        static ScreenMode _actualScreenMode;
        static List<BaseResolution> BaseResolutions = new List<BaseResolution>();

        public static List<ScreenMode> GetSupportedModes()
        {
            BaseResolutions.Add(new BaseResolution
            {
                Width = 1024,
                Height = 768
            });
            BaseResolutions.Add(new BaseResolution
            {
                Width = 1536,
                Height = 864
            });
            BaseResolutions.Add(new BaseResolution
            {
                Width = 1280,
                Height = 768
            });
            BaseResolutions.Add(new BaseResolution
            {
                Width = 1366,
                Height = 768
            });
            BaseResolutions.Add(new BaseResolution
            {
                Width = 1280,
                Height = 1024
            });

            List<ScreenMode> screenMode = new List<ScreenMode>();
            int screenWidth = KyunGame.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            int screenHeight = KyunGame.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                if (mode.Width < 800 || mode.Height < 600) continue;
                if (mode.Format != SurfaceFormat.Color) continue;
                if (mode.AspectRatio != 1024f / 768f && mode.AspectRatio != 1280f / 720f && mode.AspectRatio != 1280f / 768f && mode.AspectRatio != 1366f / 768f && mode.AspectRatio != 1280f / 1024f) continue;

                BaseResolution _base = BaseResolutions.Find(x => x.AspectRatio == mode.AspectRatio);

                screenMode.Add(new ScreenMode()
                {
                    Width = _base.Width,
                    Height = _base.Height,
                    AspectRatio = mode.AspectRatio,
                    WindowMode = (mode.Width <= screenWidth && mode.Height < screenHeight) ? WindowDisposition.Windowed : WindowDisposition.Borderless,
                    Base = _base,
                    ScaledWidth = mode.Width,
                    ScaledHeight = mode.Height
                });
            }
            return screenMode;
        }

        public static void Change()
        {
            var modes = GetSupportedModes();
            _actualScreenMode = modes[Settings1.Default.ScreenMode];
        }

        public static ScreenMode GetActualMode()
        {
            if (_actualScreenMode == null)
            {
                var modes = GetSupportedModes();
                _actualScreenMode = modes[Settings1.Default.ScreenMode];

            }

            return _actualScreenMode;
        }

        public static float ScreenScaling(ScreenMode mode)
        {

            if (mode.AspectRatio < 1.7)
            {
                return .99f;
            }

            return 1;
        }

        public static float ScreenScaling()
        {
            ScreenMode mode = GetActualMode();

            if (mode.Height <= 600 && mode.Width <= 800)
            {
                return .99f;
            }

            return 1;
        }
    }
}
