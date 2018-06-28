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
                //Bass.BASS_ChannelPause(stream);
                var pos = Bass.BASS_ChannelSeconds2Bytes(stream, ((double)value / 1000d));
                Bass.BASS_ChannelSetPosition(stream, pos, BASSMode.BASS_POS_BYTE);
                //Bass.BASS_ChannelPlay(stream, false);
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
            if(PlayState == BassPlayState.Stopped)
                Play(ActualSong);
            else
            {
                if(PlayState == BassPlayState.Paused)
                {
                    Bass.BASS_ChannelPlay(stream, false);
                    PlayState = BassPlayState.Playing;
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

        private float GetLevel(int channel)
        {
            int level = Bass.BASS_ChannelGetLevel(channel);
            float left = (float)((float)Un4seen.Bass.Utils.LowWord32(level) / 5f) / 65535f * 10; // the left level
            float right = (float)((float)Un4seen.Bass.Utils.HighWord32(level) / 5f) / 65535f * 10; // the right level
            //left = Math.Min(left, 1); //Limit to 0db
            //right = Math.Min(right, 1); //Limit to 0db
            return Math.Max(left, right)*.9f;
            
        }
    }

    public enum BassPlayState
    {
        Playing = 0,
        Paused = 1,
        Stopped = 2
    }
}
