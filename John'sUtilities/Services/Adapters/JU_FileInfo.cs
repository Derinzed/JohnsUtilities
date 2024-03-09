using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using JohnUtilities.Services.Interfaces;

namespace JohnUtilities.Services.Adapters
{
    public class NNS_FileInfo : INNS_FileInfo
    {

        public NNS_FileInfo(FileInfo fi)
        {
            info = fi;
        }
        public bool Exists()
        {
            return info.Exists;
        }
        public DateTime LastWriteTime()
        {
            return info.LastWriteTime;
        }
        FileInfo info;
    }
}
