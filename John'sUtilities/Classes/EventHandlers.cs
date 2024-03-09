using JohnUtilities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnUtilities.Model.Classes
{
    public class EventHandlers
    {
        public EventHandlers(ConfigurationManager configMan)
        {
            ConfigManager = configMan;
        }
        public void OnEventConfigOperations(Object o, NotificationEventArgs args)
        {
            var Elements = ConfigManager.GetItemsWhereAttributeContainsExactValue("Event", args.Message);
            var OrderedElements = new List<ConfigurationElement>();
            foreach (var element in Elements)
            {
                //Elements = Elements.Concat(ConfigManager.GetChildrenElements(element.Key)).ToList();
                var childrenElements = ConfigManager.GetChildrenElements(element.Key).ToList();
                if (childrenElements.Count() < 1)
                {
                    OrderedElements.Add(element);
                    continue;
                }
                OrderedElements.AddRange(childrenElements);
            }

            var UniqueElements = OrderedElements.Distinct().ToList();

            foreach (var element in UniqueElements)
            {
                var Op = ConfigManager.GetElementOperations(element);
                foreach (var operation in Op)
                {
                    ConfigManager.PerformOperation(operation.OperationName, element);
                }
            }
        }

        ConfigurationManager ConfigManager;
    }
}
