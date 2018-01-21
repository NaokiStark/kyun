using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace kyunMono
{
#if WINDOWS || XBOX || LINUX
    internal static class Program
    {
        public static bool SoftwareRendering = false;
        public static bool repair;

        public static bool LauncherEnd { get; set; }

        [STAThread]
        static void Main(string[] args)
        {


            if (args.Length > 0)
                if (args[0] == "soft")
                    SoftwareRendering = true;
                else if (args[0].ToLower() == "repair")
                    repair = true;


            if (SoftwareRendering)
            {
                Console.WriteLine("== THE GAME WILL START WITH SOFTWARE RENDERING ==");
            }

            Console.WriteLine("Starting Launcher");

            try
            {
                Application.Run(new Launcher());
                LaunchInstance();
            }
            catch (Exception exx)
            {
                Console.WriteLine($"Failed to get update {exx.Message}\\r\\n{exx.StackTrace}");
                LaunchInstance();
            }

            //ln.Show();

            Environment.Exit(0); //Kill all

        }

        
        public static void LaunchInstance()
        {
            new LoadHelper().LaunchKyun();
        }

        public static string GetKyunVersion()
        {
            if (repair)
                Console.WriteLine("kyun has launched with \"repair\" command, and I try to fix, please, make you sure if you have an Internet connection.");

            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "kyun.game.dll")) || repair)
                return null;

            DateTime lastComp = File.GetLastWriteTime(Path.Combine(Environment.CurrentDirectory, "kyun.game.dll"));

            return lastComp.ToString("yyMMddHHmm");
        }

        public static void DoReload()
        {

            Process.Start(Application.ExecutablePath); // to start new instance of application
            Environment.Exit(0);
        }

    }
#endif
}

