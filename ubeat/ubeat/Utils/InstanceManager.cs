using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace kyun.Utils
{
    public class InstanceManager : IDisposable
    {
        KyunGame ubeat;

        public static float MaxPeak = 0.1f;

        public static List<Beatmap.Mapset> AllBeatmaps { get; set; }

        public List<Beatmap.ubeatBeatMap> Beatmaps = new List<Beatmap.ubeatBeatMap>();

        public static InstanceManager Instance = null;

        public bool IntancedBeatmaps;
        
        public InstanceManager()
        {
            Instance = this;
            StartInstance();
        }

        void StartInstance()
        {
            using (ubeat = new KyunGame())
                ubeat.Run();
        }

        public void Reload()
        {
            //ubeat.Dispose();
            Application.Restart();
            Application.Exit();
        }

        public void Dispose()
        {
            //I'm a fucking stupid programmer
        }
    }
}
