using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ubeat.Audio
{
    class AudioPlaybackEngine : IDisposable
    {
        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;
        VolumeSampleProvider vsp;
        public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
        {
            //outputDevice = new WaveOutEvent();
            outputDevice = new WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared, 2);

            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
            vsp = new VolumeSampleProvider(mixer);
            mixer.ReadFully = true;
            outputDevice.Init(vsp);
            outputDevice.Play();
        }

        public void PlaySound(string fileName)
        {
            var input = new AudioFileReader(fileName);
            input.Volume = Game1.Instance.GeneralVolume;
            AddMixerInput(new AutoDisposeFileReader(input));
        }

        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
        {


            if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
            {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2)
            {
                return new MonoToStereoSampleProvider(input);
            }

            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }

        public void PlaySound(AudioFileReader fileName)
        {
            
            AddMixerInput(new NoDisposeFileReader(fileName));
        }

        public void PlaySound(NoDisposeFileReader fileName)
        {

            AddMixerInput(fileName);
        }

        
        public void PlaySound(CachedSound sound)
        {
            CachedSoundSampleProvider so = new CachedSoundSampleProvider(sound);

            AddMixerInput(so);
        }

        
        private void AddMixerInput(ISampleProvider input)
        {
            try
            {
                vsp.Volume = 60f*Game1.Instance.GeneralVolume/100f;
                mixer.AddMixerInput(ConvertToRightChannelCount(input));
                
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            outputDevice.Dispose();
        }

        public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine(44100, 2);
    }
}
