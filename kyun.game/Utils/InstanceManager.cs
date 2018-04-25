﻿using kyun.game;
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

        public static bool SoftwareRendering { get; private set; }
        
        public InstanceManager(bool softwareRendering = false)
        {
            Settings1.Default.WindowsRender = softwareRendering;

            //Disable things
            if (softwareRendering)
            {

                Logger.Instance.Debug("|");
                Logger.Instance.Debug("| Notice:");
                Logger.Instance.Debug("|");
                Logger.Instance.Debug("| kyun! was started with \"soft\" argument, and will disable a lot of things to improve performance. ");
                Logger.Instance.Debug("|                                                                                                    ");
                Settings1.Default.VSync = true;
                Settings1.Default.Video = false;
                Settings1.Default.MyPCSucks = true;
            }

            Instance = this;
            SoftwareRendering = softwareRendering;
            StartInstance();
        }

        void StartInstance()
        {
            try
            {
                kyun.Logger.Instance.Debug(System.Threading.Thread.GetDomainID().ToString());
                using (ubeat = new KyunGame(SoftwareRendering))
                    ubeat.Run();
            }
            catch (Microsoft.Xna.Framework.Graphics.NoSuitableGraphicsDeviceException nsgdex)
            {
                MessageBox.Show("Well, your graphics card is too old, but not everything is lost, kyun will start with CPU rendering (VERY SLOWER [30FPS]).", "kyun!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                MessageBox.Show("If you want to load without this message, set 'Software Rendering' in Settings screen.", "IMPORTANT MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                using (ubeat = new KyunGame(true))
                    ubeat.Run();
            }
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