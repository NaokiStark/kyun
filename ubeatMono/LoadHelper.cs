/*
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 *
 * 
 * I FUCKING MADE THIS SHIT TO AVOID TO LOAD kyun.game.dll (and another dependences) ON STARTUP
 * TO ALLOW FUCKING REPLACE FILE ON UPDATE
 *
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 *
 *  
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyunMono
{
    public class LoadHelper
    {
       
        public void LaunchKyun(){
            
            try
            {
                kyun.Logger.Instance.Info("");
                kyun.Logger.Instance.Info("");
                kyun.Logger.Instance.Info("======================================");
                kyun.Logger.Instance.Info("=                                    =");
                kyun.Logger.Instance.Info("=          Welcome to kyun!          =");
                kyun.Logger.Instance.Info("=      Developed by Fabi With ♥      =");
                kyun.Logger.Instance.Info("=                                    =");
                kyun.Logger.Instance.Info("=        Powered by MonoGame         =");
                kyun.Logger.Instance.Info("=                                    =");
                kyun.Logger.Instance.Info("======================================");
                kyun.Logger.Instance.Info("");


                kyun.KyunGame.LauncherVersion = 3;
                try
                {
                    using (kyun.Utils.InstanceManager InsManager = new kyun.Utils.InstanceManager(false, Program.repair)) { }

                }
                catch
                {
                    using (kyun.Utils.InstanceManager InsManager = new kyun.Utils.InstanceManager(false)) { }
                }


                Launcher.Instance?.Dispose();
            }
            catch (Exception e)
            {

                kyun.Logger.Instance.Severe("Beep boop, something went wrong.");
                kyun.Logger.Instance.Warn("You can try run kyun! with \"repair\" argument if you can't run kyun! anymore.");
                kyun.Logger.Instance.Severe("");
                kyun.Logger.Instance.Severe("kyun repair");
                kyun.Logger.Instance.Severe("");
                kyun.Logger.Instance.Severe("Some info to developer: ");
                kyun.Logger.Instance.Severe("");
                kyun.Logger.Instance.Severe($"{e.GetType().ToString()}");
                kyun.Logger.Instance.Severe(e.Message);
                kyun.Logger.Instance.Severe(e.StackTrace);
                kyun.Logger.Instance.Info("Press any key to exit");
                new kyun.game.Winforms.FailForm().ShowForm(e);
                //Console.Read();
                return;
            }
        }
    }
}
