﻿using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace ubeat.Audio
{
    public class Player
    {

        //Events
        public delegate void playEvMan(string ev);
        public event playEvMan onEnd;
        public Stopwatch stp;
        IWaveSource soundSource;
        System.Windows.Forms.Timer tmC;
        public ISoundOut soundOut;
        public string ActualSong = "";
        float vol { get; set; }

        public bool Paused
        {
            get
            {
                return (soundOut.PlaybackState == PlaybackState.Paused);
            }
            set
            {

                if (soundOut != null)
                {
                    if (soundOut.PlaybackState == PlaybackState.Paused)
                    {
                        Stopwatch ffs = new Stopwatch();
                        ffs.Start();
                        
                        soundOut.Play();
                        long elapsd = ffs.ElapsedMilliseconds;
                        stp.Start();
                        offset = 0;
                        offset -= elapsd;
                        ffs.Stop();
                    }
                    else if (soundOut.PlaybackState == PlaybackState.Playing)
                    {
                        soundOut.Pause();
                        stp.Stop();
                    }
                }
            }
        }
        public float Volume { get {
            if (soundOut != null)
                return soundOut.Volume;
            else
                return vol;
        }
            set {
                if (soundOut != null)
                {
                    soundOut.Volume = value;
                    vol = value;
                }
                else
                    vol = value;
            }
        }
        public long SoundLength
        {
            get
            {
                if (soundOut != null)
                    if (soundOut.WaveSource != null)
                        return (long)soundOut.WaveSource.GetLength().TotalMilliseconds;
                    else
                        return 0;
                else
                    return 0;
            }
        }
        /// <summary>
        /// Gets or Sets position
        /// </summary>
        public long Position
        {
            get
            {
                if (soundOut != null && stp!=null)
                {
                    return (long)soundOut.WaveSource.GetPosition().TotalMilliseconds-105;
                   
                } 
                else
                    return 0;
            }
            set
            {
                if (soundOut != null)
                    soundOut.WaveSource.Position = value;
            }
        }
        long offset=0;
        public long PositionV2
        {
            get
            {
                if ( stp != null)
                {
                    if (stp.IsRunning)
                    {
                        long vall = ((long)stp.Elapsed.TotalMilliseconds);
                        vall = (vall + (long)soundOut.WaveSource.GetPosition().TotalMilliseconds)/2;
                        return vall + offset;
                    }
                    else
                        return 0;
                }
                else
                    return 0;
            }
            set
            {
                if (soundOut != null)
                {
                    soundOut.WaveSource.Position = value;
                    offset = (long)stp.Elapsed.TotalMilliseconds -(long)value;
                }
            }
        }

        public Player()
        {
            stp = new Stopwatch();
            tmC = new System.Windows.Forms.Timer();
            
            tmC.Tick += tmC_Tick;
            tmC.Interval = 1;
            tmC.Start();
        }

        void tmC_Tick(object sender, EventArgs e)
        {
            if (soundOut == null)
                return;
            if (soundOut.WaveSource == null)
                return;
            if (soundOut.WaveSource.Position >= soundOut.WaveSource.Length - 15)
            {
                if (onEnd != null)
                    onEnd("1");

                //stp.Start();
               // soundOut.Dispose();
            }
        }
        public void Play(string path=null,long leadIn=0)
        {
            //offsets
            Stopwatch ffs = new Stopwatch();


            if (soundOut != null)
            {
                if (soundOut.PlaybackState == PlaybackState.Paused)
                    soundOut.Resume();
                else if (soundOut.PlaybackState == PlaybackState.Playing)
                    soundOut.Pause();
            }
            
            if (path == null && ActualSong == "")
                return;
            ActualSong = path;
            //Contains the sound to play
            soundSource = GetSoundSource(ActualSong);
            if (soundOut == null)
            {
                soundOut = GetSoundOut();
                soundOut.Stopped +=soundOut_Stopped;
            }

            if (soundOut.PlaybackState == PlaybackState.Playing || soundOut.PlaybackState == PlaybackState.Paused)
            {
                
                soundOut.Stop();
            }
            
             soundOut.Initialize(soundSource);
             soundOut.Volume = vol;
             stp.Stop();
             stp.Reset();
             ffs.Start();
             offset = 0;
            soundOut.Play();
            long elapsd = ffs.ElapsedMilliseconds;
            stp.Start();
            offset -= elapsd;
            offset += leadIn;
            ffs.Stop();
        }

        private void soundOut_Stopped(object sender, PlaybackStoppedEventArgs e)
        {
            try
            {
                if (soundOut.WaveSource.GetPosition().TotalMilliseconds >= soundOut.WaveSource.GetLength().TotalMilliseconds - 50)
                {
                    if (onEnd != null)
                        onEnd("1");

                    stp.Reset();
                    stp.Stop();
                    // soundOut.Dispose();
                }
            }
            catch
            {
                //  nothing nothing nothing
                Logger.Instance.Warn("End!!!!!!!!!!!!!!!!!!!");
            }
        }

        public void Stop()
        {
            if (soundOut != null)
            {
                soundOut.Stop();
                stp.Stop();
            }
            
        }
        private ISoundOut GetSoundOut()
        {
            if (WasapiOut.IsSupportedOnCurrentPlatform)
                return new WasapiOut();
            else
                return new DirectSoundOut();
        }

        private IWaveSource GetSoundSource(string path)
        {
            return CodecFactory.Instance.GetCodec(path);
        }
    }
}
