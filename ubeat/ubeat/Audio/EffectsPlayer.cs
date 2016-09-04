using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ubeat.Audio
{
    public class EffectsPlayer
    {
        public static void PlayEffect(AudioFileReader file, float volume)
        {
            NPlayer plyr = new NPlayer();

            plyr.Volume = volume;
            plyr.Play(file);
                
        }

        public static void Loop(object sender, NAudio.Wave.StoppedEventArgs e)
        {
           
        }

        public static NPlayer PlayEffectLooped(AudioFileReader file, float volume)
        {
            NPlayer plyr = new NPlayer();
            
            /*
            plyr.WaveOut.PlaybackStopped += new EventHandler<NAudio.Wave.StoppedEventArgs>((cc,cca) => {
                
            });*/
            plyr.Loop = true;

            plyr.Play(file);
            plyr.Volume = volume;
           
            return plyr;
        }

        
    }
}
