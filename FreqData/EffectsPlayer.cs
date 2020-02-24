using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

namespace FreqData
{
    public static class EffectsPlayer
    {
        public static List<int> EffectChannelList = new List<int>();

        public static void StartEngine()
        {
            System.Timers.Timer tm = new System.Timers.Timer();
            tm.Elapsed += (obj, args) => {

                for (int a = 0; a < EffectChannelList.Count; a++)
                {
                    int chnl = EffectChannelList[a];
                    //First 'll check for channel
                    if (Bass.BASS_ChannelGetPosition(chnl) >= Bass.BASS_ChannelGetLength(chnl))
                    {
                        //Free chnl
                        try
                        {
                            // .-.
                            Bass.BASS_ChannelStop(chnl);
                            Bass.BASS_StreamFree(chnl);
                        }
                        catch { }
                        finally
                        {
                            EffectChannelList.Remove(chnl); //clean
                        }
                    }
                }
            };
            tm.Interval += 1000;
            tm.Start();
        }

        public static int PlayEffect(int sample, float volume = 1f)
        {

            if (sample != 0)
            {

                int channel = Bass.BASS_SampleGetChannel(sample, false);
                EffectChannelList.Add(channel);
                //Bass.BASS_ChannelGetAttribute(channel, BASSAttribute.BASS_ATTRIB_FREQ, ref initialFrequency);


                Bass.BASS_ChannelSetAttribute(channel, BASSAttribute.BASS_ATTRIB_VOL, volume);
                Bass.BASS_ChannelPlay(channel, false);

                Bass.BASS_StreamFree(channel);


            }

            return sample;
        }

        public static int SetVelocity(int chnl, float vl)
        {
            float pbrate = vl * 100;
            Bass.BASS_ChannelSetAttribute(chnl, BASSAttribute.BASS_ATTRIB_TEMPO, pbrate - 100);
            return chnl;
        }

        public static int setEffect(int chnl, BASSFXType eff)
        {

            Bass.BASS_ChannelSetFX(chnl, eff, 1);
            return chnl;
        }

        public static void StopAll()
        {
            foreach (int chnl in EffectChannelList)
            {
                Bass.BASS_ChannelStop(chnl);
                Bass.BASS_StreamFree(chnl);
            }

            EffectChannelList.Clear();
        }
        public static string _fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "");
        public static string getPath(string asset)
        {
            return Path.Combine(_fullpath, asset);
        }

        public static int LoadSoundBass(string asset)
        {
            //int sample = Bass.BASS_StreamCreateFile(getPath(asset), 0, 0, BASSFlag.BASS_DEFAULT | BASSFlag.BASS_ASYNCFILE);

            byte[] data = null;

            int sample = Bass.BASS_SampleCreate(0, 44100, 2, 8, BASSFlag.BASS_SAMPLE_OVER_POS);
            try
            {
                string filepath = getPath(asset);
                using (Stream stream = File.OpenRead(filepath))
                {
                    if (stream != null)
                    {
                        data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                        stream.Close();
                    }

                }

                sample = Bass.BASS_SampleLoad(data, 0, data.Length, 8, BASSFlag.BASS_SAMPLE_OVER_POS);

                if (sample != 0)
                {

                }
                else
                {
                    
                }

                return sample;
            }
            catch
            {
                
                return 0;
            }
        }
    }
}
