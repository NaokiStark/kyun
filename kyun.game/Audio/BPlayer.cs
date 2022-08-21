using kyun.game.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

namespace kyun.Audio
{
    public class BPlayer : IDisposable
    {
        int stream = 0;


        public float PeakVol
        {
            get
            {
                if (PlayState != BassPlayState.Playing)
                    return 0;
                return GetLevel(stream);
            }

        }

        public delegate void PlayerEvents();
        public event PlayerEvents OnStopped;

        private bool paused;
        public bool Paused
        {
            get
            {

                return paused;
            }
            set
            {
                if (paused)
                {
                    paused = value;
                    if (!paused)
                        Play();//plays this shit
                }
                else
                {
                    paused = value;
                    if (paused)
                        Pause();//pauses this shit
                }
            }
        }

        public BassPlayState PlayState;
        public int framepassed = 0;

        public TimeScroller timer = new TimeScroller();

        public string ActualSong { get; private set; }

        public long Length
        {
            get
            {
                //if (_netstream) return 0;
                var len = Bass.BASS_ChannelGetLength(stream);
                return (long)Math.Round((decimal)(Bass.BASS_ChannelBytes2Seconds(stream, len) * 1000));
            }
        }

        public long Position
        {
            get
            {
                var pos = Bass.BASS_ChannelGetPosition(stream);

                return (long)((decimal)Bass.BASS_ChannelBytes2Seconds(stream, pos) * (decimal)1000);
            }
            set
            {
                Bass.BASS_ChannelPause(stream);
                var pos = Bass.BASS_ChannelSeconds2Bytes(stream, (value / 1000f));

                Bass.BASS_ChannelSetPosition(stream, pos, BASSMode.BASS_POS_BYTE);
                Bass.BASS_ChannelPlay(stream, false);
            }
        }

        public double PositionD
        {
            get
            {
                var pos = Bass.BASS_ChannelGetPosition(stream);

                return (Bass.BASS_ChannelBytes2Seconds(stream, pos) * 1000d);
            }
            set
            {
                Bass.BASS_ChannelPause(stream);
                var pos = Bass.BASS_ChannelSeconds2Bytes(stream, (value / 1000f));

                Bass.BASS_ChannelSetPosition(stream, pos, BASSMode.BASS_POS_BYTE);
                Bass.BASS_ChannelPlay(stream, false);
            }
        }

        public long PositionV2
        {
            get
            {
                var pos = timer.Position;

                return (long)pos;
            }
            set
            {
                Bass.BASS_ChannelPause(stream);
                var pos = Bass.BASS_ChannelSeconds2Bytes(stream, (value / 1000f));

                Bass.BASS_ChannelSetPosition(stream, pos, BASSMode.BASS_POS_BYTE);
                timer.SetTime(value);
                Bass.BASS_ChannelPlay(stream, false);
            }
        }

        public float Velocity = 1;

        public float Volume
        {
            get
            {
                float temp = 0.0f;
                Bass.BASS_ChannelGetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, ref temp);
                return temp;
            }
            set
            {
                Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, value);
            }
        }



        public BPlayer()
        {
            PlayState = BassPlayState.Stopped;

            System.Timers.Timer tmm = new System.Timers.Timer();

            tmm.Elapsed += (obj, hnd) =>
            {
                if (PlayState != BassPlayState.Playing)
                    return;

                if (stream == 0)
                    return;
                try
                {
                    if (Bass.BASS_ChannelGetPosition(stream) >= Bass.BASS_ChannelGetLength(stream))
                    {
                        PlayState = BassPlayState.Stopped;
                        OnStopped?.Invoke();
                    }

                }
                catch { }

            };

            tmm.Interval = 10;
            tmm.Start();

        }

        public void Dispose()
        {
            Bass.BASS_StreamFree(stream);
            // free BASS
            Bass.BASS_Free();
        }

        public void SetVelocity(float vl)
        {
            Velocity = vl;
            timer.SetVelocity(vl);
            float pbrate = vl * 100;
            Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_TEMPO, pbrate - 100);

        }

        public void Play(string fileName = null, float velocity = 1f, float pitch = 1, bool fadeIn = false)
        {

            var flags = BASSFlag.BASS_SAMPLE_LOOP | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_FX_TEMPO_ALGO_LINEAR | BASSFlag.BASS_SAMPLE_FX;
            timer.SetVelocity(velocity);
            timer.SetTime(0);
            if (fileName != null)
            {
                if (stream != 0)
                {
                    Stop();
                    Bass.BASS_StreamFree(stream);
                    stream = 0;
                }

                stream = Bass.BASS_StreamCreateFile(fileName, 0, 0, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE);
                stream = BassFx.BASS_FX_TempoCreate(stream, BASSFlag.BASS_DEFAULT);
                float pbrate = velocity * 100;
                if (velocity != 1)
                {
                    // Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_FREQ, vel * 1.5f);
                    Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_TEMPO, pbrate - 100);
                }

                if (stream != 0)
                {
                    // play the stream channel
                    Volume = KyunGame.Instance.GeneralVolume;
                    ActualSong = fileName;
                    Bass.BASS_ChannelPlay(stream, false);
                    timer.SetTime(Position);
                    
                    PlayState = BassPlayState.Playing;
                }
                else
                {
                    // error creating the stream
                    Console.WriteLine("Stream error: {0}", Bass.BASS_ErrorGetCode());
                }


            }
            else
            {
                if (stream == 0)
                    throw new Exception("No audio");
                Volume = KyunGame.Instance.GeneralVolume;
                Bass.BASS_ChannelPlay(stream, false);
                timer.SetTime(Position);
                PlayState = BassPlayState.Playing;
            }

        }

        public void Update(GameTime gameTime)
        {
            
            if (PlayState != BassPlayState.Playing)
            {
                return;
            }
            double pos = PositionD;


            //if ((double)pos - (double)timer.Position > 21d)
            if ((double)Math.Abs((double)timer.Position - (double)pos) > 21d)
            {

                timer.SetTime(pos+5);
                //Console.WriteLine("er");
            }
            else
            {

                timer.Add(gameTime.ElapsedGameTime);

            }

        }

        public void Pause()
        {
            if (PlayState == BassPlayState.Stopped)
            {
                Play(ActualSong);
                timer.SetTime(PositionD);
            }
            else
            {
                if (PlayState == BassPlayState.Paused)
                {
                    Bass.BASS_ChannelPlay(stream, false);
                    PlayState = BassPlayState.Playing;
                    timer.SetTime(PositionD);
                }
                else
                {
                    Bass.BASS_ChannelPause(stream);
                    PlayState = BassPlayState.Paused;
                }
            }
        }

        public void Stop()
        {
            Bass.BASS_ChannelStop(stream);
            PlayState = BassPlayState.Stopped;
        }

        public static int FFTFrequency2Index(int frequency, int length, int samplerate)
        {
            int bin = (int)Math.Round((double)length * (double)frequency / (double)samplerate);
            if (bin > length / 2 - 1)
                bin = length / 2 - 1;
            return bin;
        }

        private float GetLevel(int channel)
        {
            int level = Bass.BASS_ChannelGetLevel(channel);
            float left = (float)((float)Un4seen.Bass.Utils.LowWord32(level) / 5f) / 65535f * 10; // the left level
            float right = (float)((float)Un4seen.Bass.Utils.HighWord32(level) / 5f) / 65535f * 10; // the right level
            //left = Math.Min(left, 1); //Limit to 0db
            //right = Math.Min(right, 1); //Limit to 0db
            return Math.Max(left, right) * .9f;

        }

        float[] fft = new float[2048];

        float Lerp(float a, float b, float t)
        {
            return a * t + b * (1 - t);
            //return (1f - t) * a + t * b;
        }

        public float DetectFrequency(int freq1, int freq2, int size = 1024)
        {
            if (freq1 < 1 || freq2 < 1 || freq1 > freq2 || stream == 0)
                return 0f;

            float sum = 0f;
            int delta = 1;
            try
            {
                int bassdatafft = 1024;
                switch (size)
                {
                    case 256:
                        bassdatafft = (int)BASSData.BASS_DATA_FFT512;
                        break;
                    case 512:
                        bassdatafft = (int)BASSData.BASS_DATA_FFT1024;
                        break;
                    case 1024:
                        bassdatafft = (int)BASSData.BASS_DATA_FFT2048;
                        break;
                }
                // get the fft data
                if (Bass.BASS_ChannelGetData(stream, fft, bassdatafft) > 0)
                {
                    BASS_CHANNELINFO info = new BASS_CHANNELINFO();
                    if (Bass.BASS_ChannelGetInfo(stream, info))
                    {
                        // see, if we got the desired frequency here
                        // we need to calculate the bin for the frequency...
                        int bin1 = Un4seen.Bass.Utils.FFTFrequency2Index(freq1, size, info.freq);

                        int bin2 = Un4seen.Bass.Utils.FFTFrequency2Index(freq2, size, info.freq);

                        float maxP = 0;

                        for (int a = bin1; a <= bin2; a++)
                        {
                            float pval = fft[a];
                            //if (pval < 0.1f)
                            //    continue;

                            //sum += fft[a] * 10f;

                            //sum += pval;

                            if (pval > maxP) maxP = pval;
                        }

                        return maxP;
                    }
                }
            }
            catch
            {
                // error false
                return 0f;
            }
            // interpolate the result
            return Math.Min(sum / delta, 1f);
        }
    }

    public enum BassPlayState
    {
        Playing = 0,
        Paused = 1,
        Stopped = 2
    }
}
