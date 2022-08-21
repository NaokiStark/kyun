using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.Audio
{
    public class TimeScroller
    {
        public double Position;

        public float Velocity = 1f;

        public TimeScroller()
        {

        }

        public void Reset()
        {
            Position = 0;
        }

        public void Add(double milliseconds)
        {
            if (Velocity == 1f)
            {
                Position += milliseconds;
            }
            else
            {
                Position += (double)((double)milliseconds * Velocity);
            }
        }

        public void Add(TimeSpan totaltime)
        {
            if (Velocity == 1f)
            {
                Position += totaltime.TotalMilliseconds;
            }
            else
            {
                Position += (double)((double)totaltime.TotalMilliseconds * Velocity);
            }
        }

        public void SetVelocity(float velocity)
        {
            Velocity = velocity;
        }

        public void SetTime(double millisecond)
        {
            Position = millisecond;
        }
    }
}
