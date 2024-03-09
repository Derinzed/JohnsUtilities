using System;
using System.Collections.Generic;
using System.Text;

namespace JohnUtilities.Classes
{
    public class CustomActionReturn
    {
        public CustomActionReturn(string actionName, Type returnType, object returnVal)
        {
            ActionName = actionName;
            ReturnType = returnType;
            Return = returnVal;
        }
        public string ActionName { get; set; }
        public Type ReturnType { get; set; }
        public object Return { get; set; }
    }
}
