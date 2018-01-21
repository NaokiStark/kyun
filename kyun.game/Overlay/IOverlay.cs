using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kyun.Overlay
{
    public interface IOverlay
    {
        OverlayType Type { get; set; }
    }

    public enum OverlayType
    {
        Normal = 0,
        Alert = 1,
        Pause = 2, //useles?
        Fail = 3 //useles?
    }
}
