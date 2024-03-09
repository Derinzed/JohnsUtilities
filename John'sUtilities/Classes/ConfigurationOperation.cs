using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnUtilities.Classes
{
    public class ConfigurationOperation
    {
        public string OperationName { get; set; }
        public Action<ConfigurationElement> OperationAction { get; set; }

        public void ExecuteOperation(ConfigurationElement element)
        {
            OperationAction(element);
        }
    }
}
