﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubeat.Screen
{
    public class ScreenModeManager
    {
        public static List<ScreenMode> GetSupportedModes()
        {
            List<ScreenMode> screenMode = new List<ScreenMode>();
            int screenWidth = UbeatGame.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            int screenHeight = UbeatGame.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                if (mode.Width < 800) continue;
                if (mode.Format != SurfaceFormat.Color) continue;

                screenMode.Add(new ScreenMode() { 
                    Width = mode.Width,
                    Height = mode.Height,
                    WindowMode = (mode.Width<screenWidth)?WindowDisposition.Windowed:WindowDisposition.Borderless
                });
            }
            return screenMode;
        }

        public static ScreenMode GetActualMode()
        {
            var modes = GetSupportedModes();
            return modes[Settings1.Default.ScreenMode];
        }
    }
}
