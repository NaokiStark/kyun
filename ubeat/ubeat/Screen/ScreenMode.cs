using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubeat.Screen
{
    public class ScreenMode
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public WindowDisposition WindowMode { get; set; }

    }
    public enum WindowDisposition
    {
        Borderless = 0,
        Windowed = 1
    }
}
