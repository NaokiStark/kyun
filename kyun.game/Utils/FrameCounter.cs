using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace kyun.Utils
{
    public class FrameCounter
    {
        public FrameCounter()
        {

        }

        public void Update(GameTime deltaTime)
        {
            /*
            CurrentFramesPerSecond = 1.0f / deltaTime;

            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += deltaTime;*/
            
            AverageFramesPerSecond = (double)1 / deltaTime.ElapsedGameTime.TotalSeconds;
            
            //frameCounter = 0;
        }

        #region Constants

        public const int MAXIMUM_SAMPLES = 100;

        #endregion

        #region Private fields

        private Queue<float> _sampleBuffer = new Queue<float>();
        private float frameCounter;

        #endregion

        #region Properties

        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public double AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }

        #endregion
    }
}
