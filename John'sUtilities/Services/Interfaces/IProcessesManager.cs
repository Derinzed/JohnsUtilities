namespace JohnUtilities.Interfaces
{
    public interface IProcessesManager
    {
        void CreateProcess(string name, string path, string args = "", bool StartMinimized = false, bool UseShellExecute = true, bool NoWindow = true, bool WaitForExit = false, bool redirectOutput = false);
        string GetProcessErrorOutput(string processName);
        string GetProcessOverridePath(string processName);
        string GetProcessStandardOutput(string processName);
        bool IsProcessRunning(string processName);
        void SetProcessArguments(string processName, string arguments);
        void SetProcessOverridePath(string processName, string overridePath);
        void StartProcess(string processName);
        int GetExitCode(string processName);
    }
}