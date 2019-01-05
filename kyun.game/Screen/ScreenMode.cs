using kyun.game.Screen;

namespace kyun.Screen
{
    public class ScreenMode
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float AspectRatio { get; set; }
        public WindowDisposition WindowMode { get; set; }
        public BaseResolution Base { get; set; }
        public int ScaledWidth { get; set; }        
        public int ScaledHeight { get; set; }
    }

    public enum WindowDisposition
    {
        Borderless = 0,
        Windowed = 1
    }
}
