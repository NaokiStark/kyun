using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.Niku
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Instance.Info("Starting Server");

            Logger.Instance.Severe("This platform isn't a good idea. \n sorry.");

            Logger.Instance.Info("Press any key to continue...");

            Console.Read();
        }
    }
}
