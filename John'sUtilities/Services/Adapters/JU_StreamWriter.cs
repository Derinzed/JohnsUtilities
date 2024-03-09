using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using JohnUtilities.Services.Interfaces;

namespace JohnUtilities.Services.Adapters
{
    public class NNS_StreamWriter : INNS_StreamWriter
    {
        public NNS_StreamWriter() { }
        public NNS_StreamWriter(string file, bool _AutoFlush)
        {
            writer = new StreamWriter(file) { AutoFlush = _AutoFlush };
        }
        public void WriteLine(string line)
        {
            writer.WriteLine(line);
        }

        public StreamWriter writer { get; set; }
    }
}
