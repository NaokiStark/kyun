using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.game;
using kyun.game.Screen;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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
                Width = 1792,
                Height = 1008
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

            BaseResolutions.Add(new BaseResolution
            {
                Width = 2560,
                Height = 1080
            });

            List<ScreenMode> screenMode = new List<ScreenMode>();
            int screenWidth = KyunGame.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            int screenHeight = KyunGame.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                if (mode.Width < 800 || mode.Height < 600) continue;
                if (mode.Format != SurfaceFormat.Color) continue;
                //if (mode.AspectRatio != 1024f / 768f && mode.AspectRatio != 1280f / 720f && mode.AspectRatio != 1280f / 768f && mode.AspectRatio != 1366f / 768f && mode.AspectRatio != 1280f / 1024f) continue;

                BaseResolution _base = BaseResolutions.Find(x => x.AspectRatio == mode.AspectRatio);

                if (_base == null)
                    continue;

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

            //BaseResolution c = BaseResolutions.Find(x => x.AspectRatio == 2560f / 1080f);
            //screenMode.Add(new ScreenMode()
            //{
            //    Width = c.Width,
            //    Height = c.Height,
            //    AspectRatio = c.AspectRatio,
            //    WindowMode = WindowDisposition.Windowed,
            //    Base = c,
            //    ScaledWidth = 2560,
            //    ScaledHeight = 1080
            //});

            return screenMode;
        }

        public static void Change()
        {
            var modes = GetSupportedModes();
            _actualScreenMode = null;
            GetActualMode();
        }

        public static ScreenMode GetActualMode()
        {
            if (_actualScreenMode == null)
            {
                var modes = GetSupportedModes();
                try
                {
                    _actualScreenMode = modes[Settings1.Default.ScreenMode];
                }
                catch
                {
                    Settings1.Default.ScreenMode = 0;
                    Settings1.Default.Save();
                    _actualScreenMode = modes[0];
                    
                }               

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

        public static string GetMonitorFreq()
        {

            IntPtr hwnd = KyunGame.Instance.windHandle;

            // 2. Get a monitor handle ("HMONITOR") for the window. 
            //    If the window is straddling more than one monitor, Windows will pick the "best" one.
            IntPtr hmonitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (hmonitor == IntPtr.Zero)
            {
                return "MonitorFromWindow returned NULL ☹";
            }
            

            // 3. Get more information about the monitor.
            MONITORINFOEXW monitorInfo = new MONITORINFOEXW();
            monitorInfo.cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFOEXW));

            bool bResult = GetMonitorInfoW(hmonitor, ref monitorInfo);
            if (!bResult)
            {
                return "GetMonitorInfoW returned FALSE ☹";
            }

            // 4. Get the current display settings for that monitor, which includes the resolution and refresh rate.
            DEVMODEW devMode = new DEVMODEW();
            devMode.dmSize = (ushort)Marshal.SizeOf(typeof(DEVMODEW));

            bResult = EnumDisplaySettingsW(monitorInfo.szDevice, ENUM_CURRENT_SETTINGS, out devMode);
            if (!bResult)
            {
                return "EnumDisplaySettingsW returned FALSE ☹";
            }

            // Done!
            //return string.Format("{0} x {1} @ {2}hz", devMode.dmPelsWidth, devMode.dmPelsHeight, devMode.dmDisplayFrequency);
            return devMode.dmDisplayFrequency.ToString();
        }

        // MonitorFromWindow
        private const uint MONITOR_DEFAULTTONEAREST = 2;

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        // RECT
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        // MONITORINFOEX
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private unsafe struct MONITORINFOEXW
        {
            public uint cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szDevice;
        }

        // GetMonitorInfo
        [DllImport("user32.dll", SetLastError = false, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorInfoW(
            IntPtr hMonitor,
            ref MONITORINFOEXW lpmi);

        // EnumDisplaySettings
        private const uint ENUM_CURRENT_SETTINGS = unchecked((uint)-1);

        [DllImport("user32.dll", SetLastError = false, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumDisplaySettingsW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpszDeviceName,
            uint iModeNum,
            out DEVMODEW lpDevMode);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DEVMODEW
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;

            public ushort dmSpecVersion;
            public ushort dmDriverVersion;
            public ushort dmSize;
            public ushort dmDriverExtra;
            public uint dmFields;

            /*public short dmOrientation;
            public short dmPaperSize;
            public short dmPaperLength;
            public short dmPaperWidth;
            public short dmScale;
            public short dmCopies;
            public short dmDefaultSource;
            public short dmPrintQuality;*/
            // These next 4 int fields are a union with the above 8 shorts, but we don't need them right now
            public int dmPositionX;
            public int dmPositionY;
            public uint dmDisplayOrientation;
            public uint dmDisplayFixedOutput;

            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;

            public short dmLogPixels;
            public uint dmBitsPerPel;
            public uint dmPelsWidth;
            public uint dmPelsHeight;

            public uint dmNupOrDisplayFlags;
            public uint dmDisplayFrequency;

            public uint dmICMMethod;
            public uint dmICMIntent;
            public uint dmMediaType;
            public uint dmDitherType;
            public uint dmReserved1;
            public uint dmReserved2;
            public uint dmPanningWidth;
            public uint dmPanningHeight;
        }
    }
}
