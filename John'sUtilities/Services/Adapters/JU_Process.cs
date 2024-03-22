using System;
using System.Collections.Generic;
using System.Text;
using JohnUtilities.Services.Interfaces;
using System.Diagnostics;
using JohnUtilities.Classes;

namespace JohnUtilities.Services.Adapters
{
    public class JU_Process : IJU_Process
    {
        public Process Process { get; set; } = new Process();

        public void WaitForExit()
        {
            if (Process != null)
            {
                Process.WaitForExit();
            }
        }
        public int GetExitCode()
        {
            try
            {
                if (Process != null)
                {
                    return Process.ExitCode;
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLogLine("ERROR! An error was encountered trying to get process exit code.");
                Logging.WriteLogLine(ex.Message);
            }
            return 0;
        }
        public ProcessStartInfo GetStartInfo()
        {
            if (Process != null)
            {
                return Process.StartInfo;
            }
            return new ProcessStartInfo();
        }
        public bool Start()
        {
            return Process.Start();
        }
    }
}
