using System;
using System.Collections.Generic;
using System.Text;
using JohnUtilities.Services.Interfaces;
using JohnUtilities.Services.Adapters;
using JohnUtilities.Interfaces;
using System.Linq;



namespace JohnUtilities.Classes
{
    public class ProcessesManager : IProcessesManager
    {
        public ProcessesManager(IList<IProcessOpener> processesList = null)
        {
            Processes = processesList == null ? new List<IProcessOpener>() : processesList;
        }

        public void CreateProcess(string name, string path, string args = "", bool StartMinimized = false, bool UseShellExecute = true, bool NoWindow = true, bool WaitForExit = false, bool redirectOutput = false)
        {
            if (Processes.Any(x => x.ProcessName == name))
            {
                Logging.WriteLogLine("Warning.  A process with the name: " + name + " already exists.  A new process cannot be registered with the same name.");
                return;
            }
            Processes.Add(new ProcessOpener(name, path, args, StartMinimized, UseShellExecute, NoWindow, WaitForExit, redirectOutput));
        }

        public void StartProcess(string processName)
        {
            if (Processes.Count != 0)
            {
                var Process = Processes.DefaultIfEmpty(null).FirstOrDefault(x => x.ProcessName == processName);
                if (Process != null)
                {
                    Logging.WriteLogLine("Starting process: " + processName);
                    Process.Start();
                    return;
                }
            }
            Logging.WriteLogLine("Warning:  There is no process registered with the name: " + processName + " Are you sure it's defined?");
        }

        public void SetProcessOverridePath(string processName, string overridePath)
        {
            if (Processes.Count == 0)
            {
                return;
            }
            var Process = Processes.DefaultIfEmpty(null).FirstOrDefault(x => x.ProcessName == processName);
            if (Process != null)
            {
                Process.PathOverride = overridePath;
            }
        }

        public string GetProcessOverridePath(string processName)
        {
            if (Processes.Count != 0)
            {
                var Process = Processes.DefaultIfEmpty(null).FirstOrDefault(x => x.ProcessName == processName);
                if (Process != null)
                {
                    return Process.PathOverride;
                }
            }
            return "";
        }
        public bool IsProcessRunning(string processName)
        {
            if (Processes.Count != 0)
            {
                var Process = Processes.DefaultIfEmpty(null).FirstOrDefault(x => x.ProcessName == processName);
                if (Process != null)
                {
                    return Process.IsRunning;
                }
            }
            return false;
        }
        public void SetProcessArguments(string processName, string arguments)
        {
            if (Processes.Count != 0)
            {
                var Process = Processes.DefaultIfEmpty(null).FirstOrDefault(x => x.ProcessName == processName);
                if (Process != null)
                {
                    Process.SetArguments(arguments);
                }
            }
        }
        public string GetProcessStandardOutput(string processName)
        {
            if (Processes.Count != 0)
            {
                var Process = Processes.DefaultIfEmpty(null).FirstOrDefault(x => x.ProcessName == processName);
                if (Process == null)
                {
                    return "";
                }
                return Process.GetStandardOutput(); ;
            }
            return "";
        }
        public string GetProcessErrorOutput(string processName)
        {
            if (Processes.Count != 0)
            {
                var Process = Processes.DefaultIfEmpty(null).FirstOrDefault(x => x.ProcessName == processName);
                if (Process == null)
                {
                    return "";
                }
                return Process.GetErrorOutput();
            }
            return "";
        }
        public int GetExitCode(string processName)
        {
            if (Processes.Count != 0)
            {
                var Process = Processes.DefaultIfEmpty(null).FirstOrDefault(x => x.ProcessName == processName);
                if (Process == null)
                {
                    return -9999;
                }
                return Process.ReturnCode;
            }
            return -9999;
        }
        IList<IProcessOpener> Processes;
    }
}
