using System.Diagnostics;

namespace JohnUtilities.Services.Interfaces
{
    public interface IJU_Process
    {
        Process Process { get; set; }
        void WaitForExit();
        int GetExitCode();
        ProcessStartInfo GetStartInfo();
        bool Start();
    }
}