using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass;

namespace kyun.Audio
{
    public static class EffectsPlayer
    {
        public static List<int> EffectChannelList = new List<int>();

        public static void StartEngine()
        {
            System.Timers.Timer tm = new System.Timers.Timer();
            tm.Elapsed += (obj, args) => {
                EffectChannelList.Clear();
            };
            tm.Interval += 60*1000;
            tm.Start();
        }

        public static void PlayEffect(int sample)
        {

            if(sample != 0)
            {

                int channel = Bass.BASS_SampleGetChannel(sample, false);
                EffectChannelList.Add(channel);
                //Bass.BASS_ChannelGetAttribute(channel, BASSAttribute.BASS_ATTRIB_FREQ, ref initialFrequency);


                Bass.BASS_ChannelSetAttribute(channel, BASSAttribute.BASS_ATTRIB_VOL, KyunGame.Instance.GeneralVolume);
                Bass.BASS_ChannelPlay(channel, false);

                Bass.BASS_StreamFree(channel);      
                          
            }
            

                
        }

        public static void StopAll()
        {
            foreach(int chnl in EffectChannelList)
            {
                Bass.BASS_ChannelStop(chnl);
                Bass.BASS_StreamFree(chnl);
            }

            EffectChannelList.Clear();
        }
    }
}
