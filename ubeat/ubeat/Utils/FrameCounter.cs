using System.Collections.Generic;
using System.Linq;

namespace ubeat.Utils
{
    public class FrameCounter
    {
        public FrameCounter()
        {

        }

        public void Update(float deltaTime)
        {
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
            TotalSeconds += deltaTime;
        }

        #region Constants

        public const int MAXIMUM_SAMPLES = 100;

        #endregion

        #region Private fields

        private Queue<float> _sampleBuffer = new Queue<float>();

        #endregion

        #region Properties

        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }

        #endregion
    }
}
