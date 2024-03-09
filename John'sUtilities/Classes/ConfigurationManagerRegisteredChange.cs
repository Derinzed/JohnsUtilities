using System;
using System.Collections.Generic;
using System.Text;

namespace JohnUtilities.Classes
{
    public class ConfigurationManagerRegisteredChange
    {
        public ConfigurationManagerRegisteredChange(string fileLoc, ConfigurationChange coreChange)
        {
            FileLoc = fileLoc;
            BaseChange = coreChange;
        }

        public ConfigurationChange BaseChange { get; private set; }
        public string FileLoc { get; private set; }
    }
}
