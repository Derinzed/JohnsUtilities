using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using JohnUtilities.Services.Interfaces;

namespace JohnUtilities.Services.Adapters
{
    public class JU_StreamWriter : IJU_StreamWriter
    {
        public JU_StreamWriter() { }
        public JU_StreamWriter(string file, bool _AutoFlush)
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
