using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace kyun.Audio
{
    public class NoDisposeFileReader : ISampleProvider, IDisposable
    {
        private readonly AudioFileReader reader;

        WdlResamplingSampleProvider srt;
        WdlResamplingSampleProvider mfr;
        private long position;

        public float[] AudioData { get; private set; }

        public NoDisposeFileReader(AudioFileReader reader)
        {
            mfr = new WdlResamplingSampleProvider(reader, 44100);
            
            this.reader = reader;
            this.WaveFormat = mfr.WaveFormat;           

            var wholeFile = new List<float>((int)(reader.Length / 4));

            var readBuffer = new float[mfr.WaveFormat.SampleRate * mfr.WaveFormat.Channels];
            int samplesRead;
            while ((samplesRead = mfr.Read(readBuffer, 0, readBuffer.Length)) > 0)
            {
                wholeFile.AddRange(readBuffer.Take(samplesRead));
            }
            AudioData = wholeFile.ToArray();
        }


        public int Read(float[] buffer, int offset, int count)
        {
            /*
            int read = mfr.Read(buffer, offset, count);
            if (read == 0)
            {

                reader.Position = 0;
            }
            return read;
             */
            var availableSamples = this.AudioData.Length - position;
            var samplesToCopy = Math.Min(availableSamples, count);
            Array.Copy(this.AudioData, position, buffer, offset, samplesToCopy);
            position += samplesToCopy;

            if (position > availableSamples)
                reset();
            return (int)samplesToCopy;
        }

        private void reset()
        {
           
        }

        public WaveFormat WaveFormat { get; private set; }


        public void Dispose()
        {
            
            reader.Dispose();
        }
    }
}
