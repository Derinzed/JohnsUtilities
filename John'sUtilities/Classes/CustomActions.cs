using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using System.Data;
using JohnUtilities.Classes;
using JohnUtilities.Interfaces;

namespace JohnUtilities.Classes
{
    public class ActionCall
    {
        public ActionCall(string obj, string method, string[] arguments)
        {
            Object = obj;
            Method = method;
            Arguments = arguments;
        }
        public string Object { get; private set; }
        public string Method { get; private set; }
        public string[] Arguments { get; private set; }
    }

    public class CustomActions
    {
        public CustomActions(FileManager fileman, EnvironmentalManager envMan, IProcessesManager procMan, ConfigurationManager confman)
        {
            FileManager = fileman;
            EnvironmentalManager = envMan;
            ConfigManager = confman;
            ProcessesManager = procMan;
        }
        private CustomActionReturn InvokeCall(string ActionName, ActionCall call)
        {
            try
            {
                object FMObject = this.GetType().GetProperty(call.Object).GetValue(this, null);
                Type FMType = FMObject.GetType();

                MethodInfo theMethod = FMType.GetMethods().Single(x => x.Name == call.Method && x.GetParameters().Length == call.Arguments.Length);

                ParameterInfo[] ParamInfo = theMethod.GetParameters();
                List<object> ParamList = new List<object>();
                for (int i = 0; i < ParamInfo.Length; i++)
                {
                    TypeConverter typeConverter = TypeDescriptor.GetConverter(ParamInfo[i].ParameterType);
                    if (i < call.Arguments.Length)
                    {
                        object value = typeConverter.ConvertFromString(call.Arguments[i]);
                        ParamList.Add(value);
                    }
                }
                var ReturnType = theMethod.ReturnType;
                var ReturnVal = theMethod.Invoke(FMObject, ParamList.ToArray());
                return new CustomActionReturn(ActionName, ReturnType, ReturnVal);
            }
            catch (Exception ex)
            {
                Logging.WriteLogLine("Warning. CustomActions could not invoke command");
                Logging.WriteLogLine(ex.Message);
                return null;
            }
        }
        private ActionCall CreateCall(ConfigurationElement action)
        {
            var argumentString = action.GetAttribute("Arguments");
            string[] arguments = argumentString.Split(',').Select(x => x.Trim()).ToArray();
            return new ActionCall(action.GetAttribute("System"), action.GetAttribute("Operation"), arguments);
        }
        private bool EvaluateConditionalStatement(ConfigurationElement element)
        {
            var Condition = element.GetAttribute("Condition");
            var ConditionalGetters = ConfigManager.GetItemsWithPartialParent(element.Key).ToList();
            var GetterReturns = new List<CustomActionReturn>();

            foreach(var getter in ConditionalGetters)
            {
                var Call = CreateCall(getter);
                CustomActionReturn Result = InvokeCall(getter.Key, Call);

                if (Condition.Contains(getter.Key))
                {
                    if(Result.ReturnType == typeof(string))
                    {
                        Condition = Condition.Replace(getter.Key, (string)Result.Return);
                        continue;
                    }
                    var RetVal = Activator.CreateInstance(Result.ReturnType, Result.Return);
                    Condition.Replace(getter.Key, RetVal.ToString());
                }
            }

            var dt = new DataTable();
            return Convert.ToBoolean(dt.Compute(Condition, null));
        }
        public void CustomStartupAction()
        {
            var actions = ConfigManager.GetItemsWithPartialParent("CustomActions");
            
            foreach(var action in actions)
            {
                //Obtain conditional if present
                var ConditionalStatement = ConfigManager.GetItemsWithPartialParent(action.Key).Where(x => x.Key == "Conditional").ToList();
                var Returnable = ConfigManager.GetItemsWithPartialParent(action.Key).FirstOrDefault(x => x.Key == "Returnable");

                if (ConditionalStatement.Count != 0)
                {
                    if(EvaluateConditionalStatement(ConditionalStatement[0]) == false)
                    {
                        continue;
                    }
                }

                var result = InvokeCall(action.Key, CreateCall(action));

                if(Returnable != null)
                {
                    if (Returnable.GetAttribute("type") == "Reference")
                    {
                        ConfigManager.AddReference('$' + action.Key, (string)result.Return);
                    }
                }
            }
        }

        private List<CustomActionReturn> Returns;
        public FileManager FileManager { get; set; }
        public EnvironmentalManager EnvironmentalManager { get; set; }
        public IProcessesManager ProcessesManager { get; set; }
        public ConfigurationManager ConfigManager { get; }
    }
}
