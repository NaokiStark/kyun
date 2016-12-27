using System;

namespace Tests
{
#if WINDOWS
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {      
            using (UbeatTests game = new UbeatTests())
            {
                game.Run();
            }
        }
    }
#endif
}

