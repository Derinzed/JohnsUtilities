using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace JohnUtilities.Services.Interfaces
{
    public interface IJU_ProcessService
    {
        void Start(ProcessStartInfo info);
        bool IsProcessRunning(string process);
        void Kill(string process);
    }
}
