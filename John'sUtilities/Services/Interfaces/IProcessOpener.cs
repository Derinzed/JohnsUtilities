using JohnUtilities.Services.Interfaces;

namespace JohnUtilities.Interfaces
{
    public interface IProcessOpener
    {
        bool IsRunning { get; }
        string OriginalPath { get; }
        string PathOverride { get; set; }
        INNS_Process Process { get; }
        string ProcessName { get; set; }
        int ReturnCode { get; }

        string GetErrorOutput();
        string GetStandardOutput();
        void SetArguments(string arguments);
        void Start();
    }
}