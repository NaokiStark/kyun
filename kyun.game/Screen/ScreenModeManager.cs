﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.game;

namespace kyun.Screen
{
    public class ScreenModeManager
    {
        static ScreenMode _actualScreenMode;
        public static List<ScreenMode> GetSupportedModes()
        {
            List<ScreenMode> screenMode = new List<ScreenMode>();
            int screenWidth = KyunGame.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            int screenHeight = KyunGame.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                if (mode.Width < 1024 || mode.Height < 600) continue;
                if (mode.Format != SurfaceFormat.Color) continue;

                screenMode.Add(new ScreenMode() { 
                    Width = mode.Width,
                    Height = mode.Height,
                    WindowMode = (mode.Width<screenWidth)?WindowDisposition.Windowed:WindowDisposition.Borderless
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
            if(_actualScreenMode == null)
            {
                var modes = GetSupportedModes();
                _actualScreenMode = modes[Settings1.Default.ScreenMode];
            }

            return _actualScreenMode;
        }
    }
}