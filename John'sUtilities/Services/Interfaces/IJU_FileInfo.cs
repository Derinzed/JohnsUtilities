using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JohnUtilities.Services.Interfaces
{
    public interface INNS_FileInfo
    {
        
        bool Exists();
        DateTime LastWriteTime();

    }
}
