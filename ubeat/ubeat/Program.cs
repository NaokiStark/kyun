using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using kyun.Utils;

namespace kyun
{
#if WINDOWS || XBOX
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            
            //Console.OutputEncoding = System.Text.Encoding.UTF8;
            Version vr = Assembly.GetExecutingAssembly().GetName().Version;

            Logger.Instance.Info("");
            Logger.Instance.Info("");
            Logger.Instance.Info("╔═══════════════════════════════╗");
            Logger.Instance.Info("║                               ║");
            Logger.Instance.Info(
                                $"║        Build {vr.Major}.{vr.Minor}.{vr.Revision.ToString("0000")}         ║");
            Logger.Instance.Info("║                               ║");
            Logger.Instance.Info("╚═══════════════════════════════╝");
            Logger.Instance.Info("");
            Logger.Instance.Info("=====================================");
            Logger.Instance.Info("=                                   =");
            Logger.Instance.Info("=           Welcome to kyun!        =");
            Logger.Instance.Info("=       Developed by Fabi With ♥    =");
            Logger.Instance.Info("=                                   =");
            Logger.Instance.Info("=====================================");
            Logger.Instance.Info("");

            try
            {

                using (InsManager = new InstanceManager()) { }
            }
            catch(Exception e)
            {
                Logger.Instance.Severe("An critical error is broking your computer, go outside, this message will explode in 1 minute.");
                Logger.Instance.Severe(e.Message);
                Logger.Instance.Severe(e.StackTrace);
                Logger.Instance.Info("Press any key to exit");
                //Console.Read();
                return;
            }

            Logger.Instance.Debug("Exited");
            Environment.Exit(0); //Kill all subprocess

        }

        static InstanceManager InsManager = null;

    }
#endif
}

