using System;
using System.Collections.Generic;
using System.Linq;
using NAudio;
using NAudio.Wave;

namespace ubeat.Audio
{
    public class NPlayer:IDisposable
    {
        IWavePlayer waveOut;
        AudioFileReader audioFile;

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

        public static NPlayer Instance = null;

        public long Position {
            get
            {
                if (waveOut != null)
                {
                    if (audioFile != null)
                    {
                        try
                        {
                            return (long)audioFile.CurrentTime.TotalMilliseconds  - 52;
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
                        audioFile.Seek(value, System.IO.SeekOrigin.Begin);
                    }
                }
            }        
        }

        /// <summary>
        /// Adapt to old code
        /// </summary>
        public long RawPosition
        {
            get
            {
                return this.Position;
            }
        }

        public bool Paused {
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

        public float Volume
        {
            get
            {
                if (audioFile != null)
                {
                    return audioFile.Volume;
                }
                else
                    return 0;
            }
            set
            {
                if (audioFile != null)
                {
                    audioFile.Volume = value;
                }
            }
        }

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


        public NPlayer()
        {
            Instance = this;
        }

        public void Play(string fileName=null)
        {
            if (waveOut != null)
            {
                Stop();
            }

            if (fileName == null && ActualSong == "")
            {
                throw new Exception("No song to play");
                return;
            }

            if (fileName != null)
                ActualSong = fileName;

            waveOut = new WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared,10);      
            audioFile = new AudioFileReader(ActualSong);
            waveOut.Init(audioFile);
            waveOut.Play();
        }

        public void Stop()
        {
            if (waveOut == null)
            {
                throw new Exception("WaveOut is null ???");
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

            if (audioFile != null)
            {
                lock (audioFile)
                {
                    try
                    {
                        audioFile.Dispose();
                    }
                    catch
                    {
                    }
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
            if (audioFile != null)
            {
                lock (audioFile)
                {
                    try
                    {
                        audioFile.Dispose();
                    }
                    catch { }
                }
            }
        }
    }
}
