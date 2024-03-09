using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using JohnUtilities.Interfaces;
using JohnUtilities.Services.Interfaces;
using JohnUtilities.Services.Adapters;
using JohnUtilities.Classes;

namespace JohnUtilities.Classes
{
    public class ProcessOpener : IProcessOpener
    {

        public INNS_Process Process { get; private set; }
        public string ProcessName { get; set; }
        public string PathOverride { get; set; }
        public string OriginalPath { get; private set; } = "";
        public bool WaitForExit { get; set; }
        public bool IsRunning { get; private set; }
        public int ReturnCode { get; private set; }

        public ProcessOpener(string name, INNS_Process process)
        {
            ProcessName = name;
            Process = process;
            PathOverride = "";
            OriginalPath = process.Process.StartInfo.FileName;
            IsRunning = false;
        }
        public ProcessOpener(string name, string path = "", string args = "", bool StartMinimized = false, bool useShellExecute = true, bool NoWindow = true, bool waitForExit = false, bool redirectOutput = false)
        {
            Process = new NNS_Process();
            ProcessName = name;
            PathOverride = "";

            WaitForExit = waitForExit;

            Process.Process.StartInfo.FileName = path;
            OriginalPath = path;
            Process.Process.StartInfo.CreateNoWindow = NoWindow;
            Process.Process.StartInfo.Arguments = args;
            Process.Process.StartInfo.UseShellExecute = useShellExecute;
            Process.Process.EnableRaisingEvents = true;
            Process.Process.Exited += new EventHandler(OnExit);

            OriginalPath = path;

            if (useShellExecute == false && redirectOutput == true)
            {
                Process.Process.StartInfo.RedirectStandardOutput = true;
                Process.Process.StartInfo.RedirectStandardError = true;
            }

            if (StartMinimized == true)
            {
                Process.Process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            }

            IsRunning = false;
        }
        public void Start()
        {
            if (PathOverride != "")
            {
                Process.Process.StartInfo.FileName = PathOverride;
            }
            else
            {
                Process.Process.StartInfo.FileName = OriginalPath;
            }

            if (!String.IsNullOrEmpty(Process.Process.StartInfo.FileName))
            {
                try
                {
                    Process.Start();
                    IsRunning = true;
                    if(WaitForExit == true)
                    {
                        Process.WaitForExit();
                        ReturnCode = Process.GetExitCode();
                    }
                }
                catch (Exception ex)
                {
                    Logging.WriteLogLine("An error has occured within the ActiveWorkspaceClientManager.");
                    Logging.WriteLogLine(ex.Message);
                }
            }
        }

        public void SetArguments(string arguments)
        {
            Process.Process.StartInfo.Arguments = arguments;
        }

        public string GetStandardOutput()
        {
            return Process.Process.StandardOutput.ReadToEnd().Trim();
        }
        public string GetErrorOutput()
        {
            return Process.Process.StandardError.ReadToEnd().Trim();
        }
        private void OnExit(object sender, System.EventArgs e)
        {
            IsRunning = false;
        }

    }
}
