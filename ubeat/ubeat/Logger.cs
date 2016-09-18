using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ubeat
{
    public class Logger
    {
        public static Logger Instance
        {
            get
            {
                if (instance == null)
                    return instance = new Logger();
                else
                    return instance;
            }
        }

        public Logger()
        {
            string logsDirectory = Path.Combine(Application.StartupPath, "logs");

            if (!Directory.Exists(logsDirectory))
                Directory.CreateDirectory(logsDirectory);

            LogFile = Path.Combine(logsDirectory, string.Concat("log-", ConvertToTimestamp(DateTime.Now), ".log"));

            try
            {
                FileStream = new StreamWriter(LogFile);
                FileStream.AutoFlush = true;
                Info("Log File: {0}", LogFile);
                QueueTimer = new System.Timers.Timer() { Interval = 1 };
                QueueTimer.Elapsed += queuetm_Tick;
                QueueTimer.Start();
            }
            catch (Exception)
            {
                /* idk, no logger available, terminate program using Application.Exit() ?
                   seems weird since its a logger, it should be terminated from the main app
                   also a logger shouldn't be able to use the System.Windows.Forms namespace imo, i'd use an argument in the constructor
                */
                throw;
            }
        }

        void queuetm_Tick(object sender, EventArgs e)
        {
            if (Busy)
                return;
            if (Queue.Count < 1)
                return;
            if (Queue[0] == null)
                return;

            Busy = true;

            switch(Queue[0].MessageType)
            {
                case MessageType.Info:
                    break;
                case MessageType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case MessageType.Severe:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case MessageType.Debug:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
            }

            Console.WriteLine(Log(Queue[0].ToString()));
            Console.ForegroundColor = ConsoleColor.Gray;

            Queue.RemoveAt(0);

            Busy = false;
        }

        /// <summary>
        /// Writes the message in the log file
        /// </summary>
        /// <param name="msg">The message to log</param>
        /// <returns></returns>
        private string Log(string msg)
        {
            lock (FileStream)
            {
                FileStream.WriteLine(msg);
            }
            return msg;
        }

        private void AddToQueue(string msg, MessageType msgType, params object[] args)
        {
            Queue.Add(new Message(msg, msgType, args));
        }

        public void Info(string msg, params object[] args)
        {
            AddToQueue(msg, MessageType.Info, args);
        }

        public void Warn(string msg, params object[] args)
        {
            AddToQueue(msg, MessageType.Warning, args);
        }

        public void Severe(string msg, params object[] args)
        {
            AddToQueue(msg, MessageType.Severe, args);
        }

        public void Debug(string msg, params object[] args)
        {
            AddToQueue(msg, MessageType.Debug, args);
        }

        public static long ConvertToTimestamp(DateTime value)
        {
            TimeSpan elapsedTime = value - Epoch;
            return (long)elapsedTime.TotalSeconds;
        }

        public void CleanUp()
        {
            FileStream.Close();
        }

        #region Properties

        private StreamWriter FileStream;
        public string LogFile { get; private set; }
        private System.Timers.Timer QueueTimer;
        private List<Message> Queue = new List<Message>();
        private bool Busy;
        private static Logger instance = null;
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Message class

        protected class Message
        {
            public Message(string text, MessageType messageType, params object[] args)
            {
                Text = string.Format(text, args);
                MessageType = messageType;
            }

            public override string ToString()
            {
                return string.Format("{0} [{1}] {2}", DateTime.Now.ToString("HH:mm:ss"), MessageType.ToString().ToUpper(), Text);
            }

            private string _text;
            public string Text
            {
                get { return _text; }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                        _text = value;
                }
            }

            public MessageType MessageType { get; set; }
        }

        #endregion

        protected enum MessageType
        {
            Info,
            Warning,
            Severe,
            Debug
        }
    }
}
