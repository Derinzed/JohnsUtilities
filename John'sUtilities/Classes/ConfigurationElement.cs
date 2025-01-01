using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace JohnUtilities.Classes
{
    public class Attribute
    {
        public Attribute(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public string Name {get; set;}
        public string Value {get; set;}
    }

    public class ConfigurationElement
    {
        public ConfigurationElement(string key, IList<Attribute> attributes, string innerText, string parent, string parentID, string file, string uuid)
        {
            Key = key;
            Attributes = attributes;
            ParentName = parent;
            InnerText = innerText;
            UUID = uuid;
            ContainingFile = file;
            ParentUUID = parentID;
        }
        public ConfigurationElement(string key, string value, string innerText, string parent, string parentID, string uuid, string file, string attributeName = "value")
        {
            Key = key;
            Attributes.Add(new Attribute(attributeName, value));
            ParentName = parent;
            InnerText = innerText; 
            UUID = uuid;
            ContainingFile = file;
            ParentUUID = parentID;
        }
        public static ConfigurationElement CreateConfigurationElement(string key, IList<Attribute> attributes, string innerText, string parent, string UUID, string parentID = "NULL", string file = "NULL", string attributeName = "value")
        {
            return new ConfigurationElement(key, attributes, innerText, parent, parentID, file, UUID);
        }
        public static ConfigurationElement CreateConfigurationElement(string key, string value, string innerText, string parent,  string UUID, string parentID = "NULL", string file = "NULL", string attributeName = "value")
        {
            return new ConfigurationElement(key, value, innerText, parent, parentID, file, UUID, attributeName);
        }
        public ConfigurationElement(string key, string[] value, string parent, string parentID, string[] attributeName)
        {
            if(value.Length != attributeName.Length)
            {
                Logging.WriteLogLine("Error: Creating a configuration element with an unequal number of attributes and elements", LoggingLevel.Error);
            }
            Key = key;
            for (var i = 0; i != attributeName.Length; i++)
            {
                Attributes.Add(new Attribute(attributeName[i], value[i]));
            }
            ParentName = parent;
            ParentUUID = parentID;
        }
        public void AddAttribute(string attributeName, string attributeValue)
        {
            Attributes.Add(new Attribute(attributeName, attributeValue));
        }

        public string GetAttribute(string attribute)
        {
            if (Attributes.Count != 0)
            {
                var Attribute = Attributes.DefaultIfEmpty(null).FirstOrDefault(x => x.Name == attribute);
                if (Attribute != null)
                {
                    return Attribute.Value;
                }
            }
            return "";
        }
        public string UUID;
        public string Key { get; set; }
        public string ContainingFile { get; private set; }
        public IList<Attribute> Attributes = new List<Attribute>();
        public string InnerText { get; set; }
        public string ParentName { get; set; }
        public string ParentUUID { get; set; }
    }
}
