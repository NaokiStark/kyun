using System;
using System.Collections.Generic;
using System.Linq;
using NAudio;
using NAudio.Wave;
using System.IO;
using NAudio.Wave.SampleProviders;

namespace ubeat.Audio
{
    public class NPlayer : IDisposable
    {
        IWavePlayer waveOut;
        AudioFileReader audioFile;
        MeteringSampleProvider meterSampleProvider;

        public delegate void PlayerEvents();
        public event PlayerEvents OnStopped;

        public float PeakVol = 0;


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
                            return (long)audioFile.CurrentTime.TotalMilliseconds;
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
                            waveOut.Pause();
                    }
                    else
                    {
                        if (waveOut.PlaybackState == PlaybackState.Paused)
                            waveOut.Play();
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
        }


        public void Play(AudioFileReader fromStream)
        {
            fromStream.Position = 0;
            if (waveOut != null)
            {
                Stop();
            }            

            waveOut = new WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared, 2);
            audioFile = fromStream;
            meterSampleProvider = new MeteringSampleProvider(audioFile);
            meterSampleProvider.SamplesPerNotification = 60;
            meterSampleProvider.StreamVolume += MeterSampleProvider_StreamVolume;

            audioFile.Volume = vol;
            waveOut.Init(meterSampleProvider);
            /*if (Loop)
                waveOut.PlaybackStopped += waveOut_PlaybackStopped;*/
            waveOut.Play();
        }

        private void MeterSampleProvider_StreamVolume(object sender, StreamVolumeEventArgs e)
        {
            PeakVol = Normalize(e.MaxSampleValues[0]);
        }

        public void Play(string fileName = null)
        {
            if (waveOut != null)
                Stop();

            if (fileName == null && ActualSong == "")
            {
                Logger.Instance.Severe("No song to play");
                return;
            }

            if (fileName != null)
                ActualSong = fileName;

            waveOut = new WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared, 2);
            
            audioFile = new AudioFileReader(ActualSong);

            meterSampleProvider = new MeteringSampleProvider(audioFile);
            meterSampleProvider.SamplesPerNotification = 120;
            
            //meterSampleProvider.StreamVolume += MeterSampleProvider_StreamVolume;
            meterSampleProvider.StreamVolume += MeterSampleProvider_StreamVolume;

            audioFile.Volume = vol;
            waveOut.Init(meterSampleProvider);
            waveOut.PlaybackStopped += waveOut_PlaybackStopped;
            waveOut.Play();
        }

        public float Normalize(float value)
        {
            if (Volume == 0) return 0;
            return ((value)*(1/Volume));
        }

        void waveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
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
