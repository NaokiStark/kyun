namespace kyun_gl
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            try
            {
                using (kyun.Utils.InstanceManager InsManager = new kyun.Utils.InstanceManager(false, false)) { }

            }
            catch(Exception e)
            {
                new kyun.game.Winforms.FailForm().ShowForm(e);
            }

            /*
            Launcher.Instance?.Dispose();
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());*/
        }
    }
}