using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.Screen
{
    public class BaseResolution
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float AspectRatio
        {
            get
            {
                return (float)Width / (float)Height;
            }
        }
    }
}
