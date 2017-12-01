using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace kyun.Audio
{
    public class CachedSound : IDisposable
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }
        public CachedSound(string audioFileName)
        {
            using (var audioFileReader = new AudioFileReader(audioFileName))
            {
                mfr = new WdlResamplingSampleProvider(audioFileReader, 44100);

                this.WaveFormat = mfr.WaveFormat;

                var wholeFile = new List<float>((int)(audioFileReader.Length / 4));

                var readBuffer = new float[mfr.WaveFormat.SampleRate * mfr.WaveFormat.Channels];
                int samplesRead;

                while ((samplesRead = mfr.Read(readBuffer, 0, readBuffer.Length)) > 0)
                {
                    wholeFile.AddRange(readBuffer.Take(samplesRead));
                }

                AudioData = wholeFile.ToArray();
            }
        }

        public WdlResamplingSampleProvider mfr { get; set; }

        public void Dispose()
        {
            AudioData = null;
        }
    }
}
