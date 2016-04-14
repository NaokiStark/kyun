using System;

namespace ubeat
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
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
                Logger.Instance.Severe("System.NiceMemeException");
                Logger.Instance.Severe(e.Message);
                Logger.Instance.Severe(e.StackTrace);
                Console.Read();
            }
           
        }
    }
#endif
}

