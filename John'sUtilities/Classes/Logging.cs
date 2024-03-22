using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using JohnUtilities.Services.Interfaces;
using JohnUtilities.Services.Adapters;
using System.Collections.Specialized;

namespace JohnUtilities.Classes
{
    public class LoggingLevel{
        public const int Standard = (1 << 0);
        public const int Debug = (1 << 1);
        public const int Warning = (1 << 2);
        public const int Error = (1 << 3);
    }
    public class Logging
    {
        //supplying null for _customLogs will disable the custom logs feature.
        private Logging()
        {
            Instance = this;
        }

        public Logging Init(IJU_StreamWriter writer, Dictionary<string, IJU_StreamWriter> _customLogs, int loggingLevel = LoggingLevel.Standard)
        {
            logfile = writer;
            customLogs = _customLogs;
            LoggingFlags = loggingLevel;
            return Instance;
        }

        static public void WriteLogLine(string Message, int Level = LoggingLevel.Standard)
        {
            if (Instance == null)
            {
                return;

            }
            //if we are trying to print debug info, but logging debug info is false, don't.
            if((Instance.LoggingFlags & Level) > 0)
            {
                logfile.WriteLine(DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " - " + Message);
            }

            return;
        }

        static public void RegisterNewLog(string name, string path)
        {
            if (customLogs == null)
            {
                WriteLogLine("WARNING.  RegisterNewLog was called with name: " + name + " and path: " + path + " ; however, cusom logs have been disabled.");
                return;
            }
            customLogs.Add(name, new JU_StreamWriter(path, true));
        }

        static public void WriteCustomLogLine(string Log, string Message, int Level = LoggingLevel.Standard)
        {
            if (Instance == null)
            {
                return;
            }
            //if we are trying to print debug info, but logging debug info is false, don't.
            if ((Instance.LoggingFlags & Level) == 0)
            {
                return;
            }
            if (customLogs == null)
            {
                WriteLogLine("WARNING. WriteCustomLogLine has attempted to call log: " + Log + " with message: " + Message + "; however, custom logs have been disabled.");
                return;
            }
            if (!customLogs.ContainsKey(Log))
            {
                WriteLogLine("WARNING. WriteCustomLogLine has attempted to call log: " + Log + " with message: " + Message + "; however, that custom log has not been registered.");
                return;
            }
            customLogs[Log].WriteLine(DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " - " + Message);
        }

        static public void WriteLine(string Message)
        {
            Console.WriteLine(DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " - " + Message);
        }

        static public Logging GetLogger()
        {
            if(Instance == null)
            {
                return new Logging();
            }

            return Instance;
        }


        public int LoggingFlags = 0;
        private static Logging Instance;
        private static Dictionary<string, IJU_StreamWriter> customLogs;
        private static IJU_StreamWriter logfile;
    }
}
