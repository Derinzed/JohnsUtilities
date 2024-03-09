using System;
using System.Collections.Generic;
using System.Text;

namespace JohnUtilities.Classes
{

    public enum ConfigurationCategories
    {
        LauncherConfiguration,
        EnvironmentConfiguration,
        InternalConfigurationChange
    }
    public class ConfigurationChange
    {
        public ConfigurationChange(string nodeName, string attributeName, string attributeValue, ConfigurationCategories category)
        {
            NodeName = nodeName;
            AttributeName = attributeName;
            NewAttributeValue = attributeValue;
            Category = category;
        }
        public ConfigurationCategories Category { get; set; }
        public string NodeName { get; set; }
        public string AttributeName { get; set; }
        public string NewAttributeValue {get; set;}
        public string OldAttributeValue { get; set; }
        public bool ChangeApplied { get; set; }
    }
}
