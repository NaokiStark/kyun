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
