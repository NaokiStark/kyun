using kyun.game;
using kyun.game.Winforms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
        public bool IsRunning { get; set; }

        private SynchronizationContext syncContext;
        public static bool Repair = false;

        public delegate void MainThrRel();
        public static MainThrRel deleg;

        //old launcher
        public InstanceManager(bool softwareRendering = false)
        {
            Settings1.Default.WindowsRender = softwareRendering;

            deleg = Reload;

            Logger.Instance.Severe("|");
            Logger.Instance.Severe("| Notice:");
            Logger.Instance.Severe("|");
            Logger.Instance.Severe("| kyun! is running on old launcher, please, update your launcher, it wouldn't work in the future. ");
            Logger.Instance.Severe("|                                                                                                    ");

            MessageBox.Show("Your launcher is old, please update your launcher.");

            Instance = this;
            SoftwareRendering = false;
            StartInstance();
        }


        public InstanceManager(bool softwareRendering = false, bool repair = false)
        {

            syncContext = System.Threading.SynchronizationContext.Current;
            deleg = Reload;


            Repair = repair;
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

                ubeat = new KyunGame(SoftwareRendering, Repair);
                ubeat.Run();

            }
            catch (Exception nsgdex)
            {
                ubeat?.Exit();
                Logger.Instance.Severe(nsgdex.Message);
                Logger.Instance.Severe(nsgdex.StackTrace);
                var frm = new FailForm();
                frm.ShowForm(nsgdex);
            }

            Environment.Exit(0);
        }

        public void Reload()
        {
            IsRunning = true;
            ubeat?.Exit();

            var process = Process.GetCurrentProcess();

            Process.Start(process.MainModule.FileName);
            Environment.Exit(0);
        }

        private void Ubeat_Disposed(object sender, EventArgs e)
        {

        }

        public void Dispose()
        {
            //I'm a fucking stupid programmer
        }
    }
}
