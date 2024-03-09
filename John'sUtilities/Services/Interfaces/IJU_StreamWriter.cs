using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JohnUtilities.Services.Interfaces
{
    public interface INNS_StreamWriter
    {
        void WriteLine(string line);
        StreamWriter writer { get; set; }
    }
}
