using System;
using System.Collections.Generic;
using System.Linq;
using NAudio;
using NAudio.Wave;
using System.IO;
using NAudio.Wave.SampleProviders;
using kyun.Audio.Effects;
using VarispeedDemo.SoundTouch;

namespace kyun.Audio
{
    public class NPlayer : IDisposable
    {
        IWavePlayer waveOut;
        AudioFileReader audioFile;
        MeteringSampleProvider meterSampleProvider;

        public delegate void PlayerEvents();
        bool playing = false;
        public event PlayerEvents OnStopped;

        public float PeakVol = 0;

        private long pausedTime = 0;
        private DateTime startPause;
        private DateTime stopPause;
        private long lastPaused=0;
        /// <summary>
        /// Adapt to old code
        /// </summary>
        public AudioFileReader soundOut
        {
            get
            {
                return audioFile;
            }
        }

        public IWavePlayer WaveOut { get { return waveOut; } }

        public static NPlayer Instance = null;

        public float Velocity {get; private set;}

        public long Position
        {
            get
            {
                if (waveOut != null)
                {
                    if (audioFile != null)
                    {
                        try
                        {
                            //return (long)audioFile.CurrentTime.TotalMilliseconds - 8;
                            
                            if (Paused)
                            {
                                return lastPaused - pausedTime;
                            }
                            else
                            {/*
                                long cms = (long)(DateTime.Now - startTime).TotalMilliseconds;
                                long ctm = (long)audioFile.CurrentTime.TotalMilliseconds;

                                if (Math.Abs(cms - ctm) > 30){
                                    startTime = DateTime.Now - audioFile.CurrentTime; //Fuck sync?
                                }

                                cms = (long)(DateTime.Now - startTime).TotalMilliseconds;
                                */
                                long cms = (long)(DateTime.Now - startTime).TotalMilliseconds;
                                return (long)((cms) * Velocity) ;
                            }
                            
                        }
                        catch { return 0; }
                    }
                    else
                        return 0;
                }
                else
                    return 0;
            }
            set
            {
                if (waveOut != null)
                {
                    if (audioFile != null)
                    {
                        audioFile.Seek(value, SeekOrigin.Begin);
                    }
                }
            }        
        }

        public bool Paused
        {
            get
            {
                if (waveOut != null)
                {
                    return (waveOut.PlaybackState == PlaybackState.Paused);
                }
                else
                    return false;
            }
            set
            {
                if (waveOut != null)
                {
                    if (value)
                    {
                        if (waveOut.PlaybackState != PlaybackState.Paused && waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            waveOut.Pause();
                            startPause = DateTime.Now;
                            lastPaused = (long)(startPause - startTime).TotalMilliseconds;
                        }
                            
                    }
                    else
                    {
                        if (waveOut.PlaybackState == PlaybackState.Paused)
                        {
                            startTime = DateTime.Now - audioFile.CurrentTime;
                            waveOut.Play();
                            
                            //pausedTime += (long)(DateTime.Now - startPause).TotalMilliseconds;
                        }
                            
                    }
                }
            }
        }

        public long SongLength
        {
            get
            {
                return (long)audioFile.TotalTime.TotalMilliseconds;
            }
        }

        public float Volume
        {
            get
            {
                if (audioFile != null)
                {
                    return audioFile.Volume;
                }
                else
                    return vol;
            }
            set
            {
                if (audioFile != null)
                {
                    audioFile.Volume = value;
                }
                vol = value;
            }
        }

        float vol = 0;
        private DateTime startTime;

        public string ActualSong { get; private set; }

        public PlaybackState PlayState
        {
            get
            {
                if (waveOut != null)
                    return waveOut.PlaybackState;
                else
                    return PlaybackState.Stopped;
            }
        }


        public bool Loop { get; set; }

        public NPlayer()
        {
            Instance = this;
            Velocity = 1;
        }


        public void Play(AudioFileReader fromStream)
        {
            pausedTime = 0;
            fromStream.Position = 0;
            if (waveOut != null)
            {
                playing = true;
                Stop();
            }

            
            waveOut = new WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared, 2);
            audioFile = fromStream;
            meterSampleProvider = new MeteringSampleProvider(audioFile);
            meterSampleProvider.SamplesPerNotification = 1024;
            meterSampleProvider.StreamVolume += MeterSampleProvider_StreamVolume;

            var offsetSampleProv = new OffsetSampleProvider(meterSampleProvider);

            audioFile.Volume = vol;
            //waveOut.Init(meterSampleProvider);
            waveOut.Init(offsetSampleProv);

            /*if (Loop)
                waveOut.PlaybackStopped += waveOut_PlaybackStopped;*/
            waveOut.Play();
            playing = false;
        }

        private void MeterSampleProvider_StreamVolume(object sender, StreamVolumeEventArgs e)
        {
            PeakVol = Normalize(e.MaxSampleValues[0]);
        }



        public void Play(string fileName = null, float velocity = 1, float pitch = 1, bool fadeIn = false)
        {
            pausedTime = 0;
            if (waveOut != null)
            {
                playing = true;
                Stop();
            }

            
            if (fileName == null && ActualSong == "")
            {
                Logger.Instance.Severe("No song to play");
                return;
            }

            if (fileName != null)
                ActualSong = fileName;

            //waveOut = new WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared, 2);

            waveOut = new DirectSoundOut(50);

            try
            {
                audioFile = new AudioFileReader(ActualSong);
                

                meterSampleProvider = new MeteringSampleProvider(audioFile);
                meterSampleProvider.SamplesPerNotification = 1024;

                //meterSampleProvider.StreamVolume += MeterSampleProvider_StreamVolume;
                meterSampleProvider.StreamVolume += MeterSampleProvider_StreamVolume;

                var offsetSampleProv = new OffsetSampleProvider(meterSampleProvider);

                //offsetSampleProv.SkipOverSamples = 1000;
                TimePitchEff TimePitchEffProv;
                Effects.FadeInOutSampleProvider cc = null;
                if (velocity != 1)
                {
                    var ct = new VarispeedSampleProvider(offsetSampleProv, 10, new SoundTouchProfile(true, true));
                    ct.PlaybackRate = velocity;
                    this.Velocity = velocity;
                   
                    if (fadeIn)
                    {
                        cc = new Effects.FadeInOutSampleProvider(ct);
                        TimePitchEffProv = new TimePitchEff(cc);
                    }
                    else
                    {
                        TimePitchEffProv = new TimePitchEff(ct);
                    }                    

                    TimePitchEffProv.PitchFactor = pitch;

                }
                else
                {

                    if (fadeIn)
                    {
                        cc = new Effects.FadeInOutSampleProvider(offsetSampleProv);
                        TimePitchEffProv = new TimePitchEff(cc);
                    }
                    else
                    {
                        TimePitchEffProv = new TimePitchEff(offsetSampleProv);
                    }

                    TimePitchEffProv.PitchFactor = pitch;
                }

                               
                audioFile.Volume = vol;
                                               
                if(pitch != 1)
                {
                    pausedTime = 200;
                }
                //waveOut.Init(meterSampleProvider);
                waveOut.Init(TimePitchEffProv);
                waveOut.PlaybackStopped += waveOut_PlaybackStopped;
                waveOut.Play();
                cc?.BeginFadeIn(5000);

                startTime = DateTime.Now - audioFile.CurrentTime;

            }
            catch
            {
                //
            }
            finally
            {
                playing = false;
            }
        }

        public float Normalize(float value)
        {
            if (Volume == 0) return 0;
            return ((value)*(1/Volume));
        }

        void waveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if(!playing)
                OnStopped?.Invoke();
        }

        public void Stop()
        {
            Loop = false;

            if (waveOut == null)
            {
#if DEBUG
                Logger.Instance.Warn("Waveout is null");
#endif
                return;
            }
            else
            {
                
                lock (waveOut)
                {

                    waveOut.Stop();                    

                    waveOut.Dispose();
                 }
                 
            }         
        }

        public void Dispose()
        {
            if (waveOut != null)
            {
                lock (waveOut)
                {
                    try
                    {
                        waveOut.Dispose();
                    }
                    catch { }
                }
            }
        }
    }
}
