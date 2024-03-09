using System;
using System.Collections.Generic;
using System.Text;
using JohnUtilities.Services.Interfaces;
using System.Diagnostics;
using JohnUtilities.Classes;

namespace JohnUtilities.Services.Adapters
{
    public class NNS_ProcessService : INNS_ProcessService
    {
        public void Start(ProcessStartInfo info)
        {
            Process.Start(info);
        }

        public bool IsProcessRunning(string process)
        {
            Process[] list = Process.GetProcessesByName(process);
            if(list.Length > 0)
            {
                return true;
            }
            return false;
        }

        public void Kill(string process)
        {
            Process[] list = Process.GetProcessesByName(process);
            Logging.WriteLogLine("The Process Service is closing " + list.Length + " processes with name: " + process, LoggingLevel.Debug);
            try
            {
                foreach (var item in list)
                {
                    var message = item.CloseMainWindow();
                    item.Close();

                    if (!item.HasExited || message == false)
                    {
                        Logging.WriteLogLine("The Process was unable to close by request. Killing process.", LoggingLevel.Debug);
                        item.Kill();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLogLine("Warning, there was an error trying to kill process.");
                Logging.WriteLogLine(ex.Message);
            }
        }
    }
}
