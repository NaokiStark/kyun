using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

namespace kyun.Audio
{
    public class BPlayer : IDisposable
    {
        int stream = 0;

        public float PeakVol {
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
                    if(!paused)
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
                
                return (long)Math.Round((decimal)(Bass.BASS_ChannelBytes2Seconds(stream, pos) * 1000));
            }
            set
            {

                var pos = Bass.BASS_ChannelSeconds2Bytes(stream, value);
                Bass.BASS_ChannelSetPosition(stream, pos);
            }
        }

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

            /*
             * We init this before
            Un4seen.Bass.BassNet.Registration("nikumi@hotmail.com", "2X14292918312422");
            Bass.BASS_PluginLoad(AppDomain.CurrentDomain.BaseDirectory + "bass_fx.dll");

            Bass.BASS_Init(1, 44100, BASSInit.BASS_DEVICE_STEREO, IntPtr.Zero);
            */

            System.Timers.Timer tmm = new System.Timers.Timer();

            tmm.Elapsed += (obj, hnd) =>
            {
                if (PlayState != BassPlayState.Playing)
                    return;

                if (stream == 0)
                    return;

                if (Bass.BASS_ChannelGetPosition(stream) >= Bass.BASS_ChannelGetLength(stream))
                {
                    PlayState = BassPlayState.Stopped;
                    OnStopped?.Invoke();
                }

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
            float pbrate = vl * 100;
            Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_TEMPO, pbrate - 100);

        }

        public void Play(string fileName = null, float velocity = 1f, float pitch = 1, bool fadeIn = false)
        {
            var flags = BASSFlag.BASS_SAMPLE_LOOP | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_FX_TEMPO_ALGO_LINEAR | BASSFlag.BASS_SAMPLE_FX;

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
                    Bass.BASS_ChannelPlay(stream, false);
                    ActualSong = fileName;
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
                PlayState = BassPlayState.Playing;
            }

        }

        public void Pause()
        {
            Bass.BASS_ChannelPause(stream);
            PlayState = BassPlayState.Paused;
        }

        public void Stop()
        {
            Bass.BASS_ChannelStop(stream);
            PlayState = BassPlayState.Stopped;
        }

        private float GetLevel(int channel)
        {/*
            float maxL = 0f;
            float maxR = 0f;

            // length of a 20ms window in bytes
            int length20ms = (int)Bass.BASS_ChannelSeconds2Bytes(channel, 0.02);
            // the number of 32-bit floats required (since length is in bytes!)
            int l4 = length20ms / 4; // 32-bit = 4 bytes

            // create a data buffer as needed
            float[] sampleData = new float[l4];

            int length = Bass.BASS_ChannelGetData(channel, sampleData, length20ms);

            // the number of 32-bit floats received
            // as less data might be returned by BASS_ChannelGetData as requested
            l4 = length / 4;

            for (int a = 0; a < l4; a++)
            {
                float absLevel = Math.Abs(sampleData[a]);

                // decide on L/R channel
                if (a % 2 == 0)
                {
                    // Left channel
                    if (absLevel > maxL)
                        maxL = absLevel;
                }
                else
                {
                    // Right channel
                    if (absLevel > maxR)
                        maxR = absLevel;
                }
            }

            // limit the maximum peak levels to +6bB = 65535 = 0xFFFF
            // the peak levels will be int values, where 32767 = 0dB
            // and a float value of 1.0 also represents 0db.
            int peakL = (int)Math.Round(32767f * maxL) & 0xFFFF;
            int peakR = (int)Math.Round(32767f * maxR) & 0xFFFF;
            //int peakL = (float)Math.Round(32767f * maxL);
            //float peakR = (float)Math.Round(32767f * maxR);

            return maxL;*/

            int level = Bass.BASS_ChannelGetLevel(channel);
            float left = (float)((float)Un4seen.Bass.Utils.LowWord32(level) / 5f) / 65535f * 10; // the left level
            float right = (float)((float)Un4seen.Bass.Utils.HighWord32(level) / 5f) / 65535f * 10; // the right level
            left = Math.Min(left, 1); //Limit to 0db
            right = Math.Min(right, 1); //Limit to 0db
            return Math.Max(left, right);
            /*
            int left = Utils.LowWord32(level); // the left level
            int right = Utils.HighWord32(level); // the right level*/
        }
    }

    public enum BassPlayState
    {
        Playing = 0,
        Paused = 1,
        Stopped = 2
    }
}
