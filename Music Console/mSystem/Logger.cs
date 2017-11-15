using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music_Console.mSystem
{
    public class LoggerList<T> : List<T>
    {
        public event EventHandler OnAdd;
        public new void Add(T item)
        {
            OnAdd?.Invoke(this, null);
            base.Add(item);
        }
    }

    public class Logger
    {
        // Directory in which logs are stored
        private static string _loggerPath = "Logs/";
        // Used to check if the Old Logs have already been added to the OldLogs Object
        private static bool _alreadyPulled = false;
        // Previously saved logs before the current use of the console app
        private static readonly List<string> OldLogs = new List<string>();
        // Current logs to be added here
        public static LoggerList<string> CurrentLogs = new LoggerList<string>();

        //EventHandler to Save Logs on add
        public static void OnAdd(object sender, EventArgs e)
        {
            SaveLog();
        }

        /// <summary>
        /// Saves the logs
        /// </summary>
        public static void SaveLog()
        {
            if (!Directory.Exists(_loggerPath))
            {
                Directory.CreateDirectory(_loggerPath);
            }

            string fileName = _loggerPath + DateTime.Now.ToString("MM_dd_yyyy") + "_log_.txt";
            // If the File Exists, contiue with loading
            if (File.Exists(fileName))
            {
                // If the OldLogs have not already been pulled, pull them
                if (!_alreadyPulled)
                {
                    var oldLogs = File.ReadAllLines(fileName).ToList();
                    foreach (var i in oldLogs)
                    {
                        OldLogs.Add(i);
                    }
                    _alreadyPulled = true; // Let know that the OldLogs have already been pulled this instance
                }
                var lines = new List<string>(OldLogs);
                lines.AddRange(CurrentLogs);
                File.Delete(fileName);

                var sr = File.CreateText(fileName);
                foreach (string x in lines)
                {
                    sr.WriteLine(x);
                }
                sr.Flush();
                sr.Close();
            }
            // Only go here if the file doesn't exist
            else
            {
                var sr = File.CreateText(fileName);
                foreach (string x in CurrentLogs)
                {
                    sr.WriteLine(x);
                }
                sr.Flush();
                sr.Close();
            }
        }
    }
}
