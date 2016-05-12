using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
namespace ubeat
{
    public class Logger
    {
        //Singleton
        static Logger instance = null;
        static public Logger Instance
        {
            get
            {
                if (Logger.instance == null)
                    return instance = new Logger();
                else
                    return instance;
            }
        }
        //eof singleton
        StreamWriter fileStream;
        public string LogFile { get; private set; }
        System.Timers.Timer queuetm;
        List<string> queue = new List<string>();
        bool busy;
        public Logger()
        {
            LogFile = Path.Combine( System.Windows.Forms.Application.StartupPath,"logs\\log-"+(ConvertToTimestamp(DateTime.Now)).ToString()+".log");

            fileStream = new StreamWriter(LogFile);
            fileStream.AutoFlush = true;
            Info("Log File: {0}",LogFile);
            queuetm = new System.Timers.Timer() { Interval = 1 };
            queuetm.Elapsed += queuetm_Tick;
            queuetm.Start();
        }

        void queuetm_Tick(object sender, EventArgs e)
        {
            if (busy)
                return;
            if (queue.Count < 1)
                return;
            if (queue[0] == null)
                return;

            busy = true;
            if (queue[0].Contains("[INFO]"))
            {
                Console.WriteLine(log(queue[0]));
            }
            else if (queue[0].Contains("[WARN]"))
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(log(queue[0]));
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else if (queue[0].Contains(" [SEVERE] "))
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(log(queue[0]));
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            queue.RemoveAt(0);
            busy = false;
        }

        public void Info(string cc, params object[] Str)
        {
            string toLog = string.Format(cc, Str);
            toLog = string.Format("{0} [INFO] {1}", DateTime.Now.ToString("HH:mm:ss"), toLog);
            queue.Add(toLog);
        }

        public void Warn(string cc, params object[] Str)
        {
         
                string toLog = string.Format(cc, Str);
                toLog = string.Format("{0} [WARN] {1}", DateTime.Now.ToString("HH:mm:ss"), toLog);
                queue.Add(toLog);
            

        }

        public void Severe(string cc, params object[] Str)
        {
            
                string toLog = string.Format(cc, Str);
                toLog = string.Format("{0} [SEVERE] {1}", DateTime.Now.ToString("HH:mm:ss"), toLog);

                queue.Add(toLog);
            
        }

        private string log(string cc)
        {
            lock (fileStream)
            {
                fileStream.WriteLine(cc);
            }
            return cc;
        }

        static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ConvertToTimestamp(DateTime value)
        {
            TimeSpan elapsedTime = value - Epoch;
            return (long) elapsedTime.TotalSeconds;
        }
        public void CleanUp()
        {
            fileStream.Close();
        }

    }
}
