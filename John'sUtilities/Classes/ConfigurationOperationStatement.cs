using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnsUtilities.Classes;
using JohnUtilities.Classes;

namespace JohnsUtilities.Classes
{
    //A simple container class used to create and execute a full operation statement
    //Can be used to collect data for the operation and then save it for later use
    public class ConfigurationOperationStatement
    {
        public ConfigurationOperation Operation { get; set; }
        public ConfigurationElement Element { get; set; }

        public void ExecuteStatement()
        {
            Operation.ExecuteOperation(Element);
        }
    }
}
