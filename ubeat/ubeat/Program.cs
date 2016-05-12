using System;
using System.Reflection;

namespace ubeat
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        [STAThread]
        static void Main(string[] args)
        {
            Version vr = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Logger.Instance.Info("");
            Logger.Instance.Info("");
            Logger.Instance.Info("╔═══════════════════════════════╗");
            Logger.Instance.Info("║                               ║");
            Logger.Instance.Info("║        Build " + string.Format("{0}.{1}.{2}", vr.Major, vr.Minor, vr.Revision.ToString("0000")) + "         ║");
            Logger.Instance.Info("║                               ║");
            Logger.Instance.Info("╚═══════════════════════════════╝");
            Logger.Instance.Info("");
            Logger.Instance.Info("=====================================");
            Logger.Instance.Info("=                                   =");
            Logger.Instance.Info("=          Welcome to ubeat!        =");
            Logger.Instance.Info("=       Developed by Fabi With ♥    =");
            Logger.Instance.Info("=                                   =");
            Logger.Instance.Info("=====================================");
            Logger.Instance.Info("");
            
            try
            {
                using (Game1 game = new Game1())
                {
                    game.Run();
                   
                }
            }
            catch(Exception e)
            {
                Logger.Instance.Severe("System.NiceMeme.Exception");
                Logger.Instance.Severe(e.Message);
                Logger.Instance.Severe(e.StackTrace);
                Console.WriteLine("Press any key to exit");
                Console.Read();
            }
           
        }
    }
#endif
}

