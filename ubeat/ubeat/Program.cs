using System;
using System.Reflection;

namespace ubeat
{
#if WINDOWS || XBOX
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
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
            Logger.Instance.Info("=          Welcome to ubeat!        =");
            Logger.Instance.Info("=       Developed by Fabi With ♥    =");
            Logger.Instance.Info("=                                   =");
            Logger.Instance.Info("=====================================");
            Logger.Instance.Info("");

            try
            {
                using (var game = new Game1())
                {
                    game.Run();
                }
            }
            catch(Exception e)
            {
                Logger.Instance.Severe(e.Message);
                Logger.Instance.Severe(e.StackTrace);
                Logger.Instance.Info("Press any key to exit");
                Console.Read();
                return;
            }
           
        }
    }
#endif
}

