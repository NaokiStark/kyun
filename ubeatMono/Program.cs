using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Security.Principal;
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
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

            Start(args);

        }


        public static void Start(string[] args)
        {
            //First, check XNA 

            bool XNAResult = CheckXNAInstalled();

            if (!XNAResult)
            {
                var xnaDlr = new XNADownloader();
                xnaDlr.ShowDialog();
                //Download and install, when installed, return to game
            }

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

        public static bool CheckXNAInstalled()
        {
            string pFiles = ProgramFilesx86();
            string XNAPath = Path.Combine(pFiles, "Microsoft XNA\\XNA Game Studio\\v4.0");

            if (!new DirectoryInfo(XNAPath).Exists)
            {
                MessageBox.Show("XNA Framework 4 is not installed that is required for kyun!\r\n\r\nI'll try to install it right now.\r\n\r\nIf this fails, try to install manually.", "kyun!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
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

