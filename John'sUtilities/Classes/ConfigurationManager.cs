using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using JohnUtilities.Services.Interfaces;
using JohnUtilities.Services.Adapters;
using JohnUtilities.Classes;
using JohnUtilities.Interfaces;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;


namespace JohnUtilities.Classes
{
    public class Operation
    {

    }
    struct BaseConfigurationOperations
    {
        public static readonly Operation Definitions = new Operation();
        public static readonly Operation CopyFiles = new Operation();
        public static readonly Operation CopyDirectories = new Operation(); 
        public static readonly Operation EnvironmentalVariables = new Operation();
        public static readonly Operation RegistryValues = new Operation();
        public static readonly Operation KillProcesses = new Operation();
        public static readonly Operation Cleanup = new Operation();
        public static readonly Operation CustomActions = new Operation();
    }
    public class ConfigurationManager : IConfigurationManager
    {
        public ConfigurationManager(List<ConfigurationElement> Dict = null, IDictionary<string, string> references = null)
        {
            Loader = new ConfigLoading(new JU_XMLService());
            FileManager = new FileManager();
            ConfigurationItems = Dict == null ? new List<ConfigurationElement>() : Dict;
            ReferenceSubstitutions = references == null ? new Dictionary<string, string>() : references;
        }
        public ConfigurationManager(IConfigLoading config, IFileManager fileManager, List<ConfigurationElement> Dict = null, IDictionary<string, string> references = null)
        {
            Loader = config;
            FileManager = fileManager;
            ConfigurationItems = Dict == null ? new List<ConfigurationElement>() : Dict;
            ReferenceSubstitutions = references == null ? new Dictionary<string, string>() : references;
        }
        public ConfigurationManager(IConfigLoading config, IFileManager fileManager, List<ConfigurationElement> Dict)
        {
            Loader = config;
            FileManager = fileManager;
            ConfigurationItems = Dict;
        }
        private void ResolveReferences()
        {
            foreach (var item in ConfigurationItems.Where(x => x.ParentName.EndsWith("Definitions")).ToList())
            {
                foreach (var attribute in item.Attributes)
                {
                    AddReference('$' + item.Key + "." + attribute.Name, attribute.Value);

                    if (attribute.Name == "value")
                    {
                        AddReference('$' + item.Key, attribute.Value);
                    }
                }
            }
        }

        public virtual void ParseConfig(string file, List<ConfigurationElement> container)
        {
            Loader.LoadDocument(file);
            Loader.LoadTree(Loader.GetRoot(), container);
        }
        public async Task ParseWebConfig(string file, List<ConfigurationElement> container)
        {
            await Loader.LoadWebXMLAsDocument(file);
            Loader.LoadTree(Loader.GetRoot(), container);
        }
        public void LoadEnvironmentConfigurationFile()
        {
            if (!ConfigurationItems.Any(x => x.Key == "EnvironmentConfigurationFile"))
            {
                Logging.WriteLogLine("Warning:  The default configuration file does not exist or the launcher configuration settings have not been loaded.  You can set this manually after the launcher has loaded.");
                return;
            }

            ParseConfig(GetConfigurationSetting("EnvironmentConfigurationFile"), ConfigurationItems);
            ResolveReferences();
        }

        public void LoadLauncherConfigurationFile()
        {
            if (!FileManager.FileService.FileExists(defaultConfigFile))
            {
                Logging.WriteLogLine("Warning:  The default configuration file does not exist.");
                return;
            }

            ParseConfig(defaultConfigFile, ConfigurationItems);
        }

        public void LoadConfigFile(string configFile)
        {
            Logging.WriteLogLine("Loading configuration file: " + configFile, LoggingLevel.Debug);
            if (!FileManager.FileService.FileExists(configFile))
            {
                Logging.WriteLogLine("Warning:  The configuration file does not exist.");
                return;
            }
            ParseConfig(configFile, ConfigurationItems);
            ResolveReferences();
        }
        public async Task LoadWebConfigFile(string configFile)
        {
            Logging.WriteLogLine("Loading configuration file: " + configFile, LoggingLevel.Debug);
            await ParseWebConfig(configFile, ConfigurationItems);
            ResolveReferences();
        }
        public void StageChange(ConfigurationChange change)
        {

            if (!ConfigurationItems.Any(x => x.Key == change.NodeName))
            {
                Logging.WriteLogLine("There is no original value for " + change.NodeName + ". Please use the Add operation.");
                return;
            }
            string fileLoc = "";
            if (change.Category == ConfigurationCategories.LauncherConfiguration)
            {
                fileLoc = defaultConfigFile;
            }
            else if (change.Category == ConfigurationCategories.EnvironmentConfiguration)
            {
                fileLoc = GetConfigurationSetting("EnvironmentConfigurationFile", "value");
            }
            else if (change.Category == ConfigurationCategories.InternalConfigurationChange)
            {
                fileLoc = "";
            }
            else
            {
                Logging.WriteLogLine("Warning.  The Configuration Change Category could not be verrified, change will not be staged");
                return;
            }

            ConfigurationChanges.Add(new ConfigurationManagerRegisteredChange(fileLoc, change));
        }
        public void ApplyChanges()
        {
            foreach (var change in ConfigurationChanges)
            {
                change.BaseChange.OldAttributeValue =
                    ConfigurationItems.First(x => x.Key == change.BaseChange.NodeName).Attributes.First(y => y.Name == change.BaseChange.AttributeName).Value;


                ConfigurationItems.First(x => x.Key == change.BaseChange.NodeName).Attributes.First(y => y.Name == change.BaseChange.AttributeName).Value = change.BaseChange.NewAttributeValue;

                change.BaseChange.ChangeApplied = true;
            }
            CleanChanges();
        }

        public void SaveChangesToFile()
        {
            ConfigurationChanges.RemoveAll(x => x.FileLoc == "");
            var OrderedChanges = ConfigurationChanges.OrderBy(x => x.FileLoc).ToList();
            Loader.ApplyXMLChanges(OrderedChanges);
            ConfigurationChanges.Clear();
        }

        private void CleanChanges()
        {
            ConfigurationChanges.RemoveAll(x => x.FileLoc == "");
        }

        private bool DoesKeyExist(string key, string attribute = "value")
        {
            if (!ConfigurationItems.Any(x => x.Key == key))
            {
                Logging.WriteLogLine("Warning.  The configuration does not have the key: " + key + " registered.  Are you sure its defined?", LoggingLevel.Debug);
                return false;
            }
            if (!ConfigurationItems.First(x => x.Key == key).Attributes.Any(y => y.Name == attribute))
            {
                Logging.WriteLogLine("Warning.  The configuration found key: " + key + "; however, the atttribute: " + attribute + " was not associated with it.  Are you sure its defined?", LoggingLevel.Debug);
                return false;
            }
            return true;
        }

        public void AddConfigurationSetting(string key, string val = "", string attribute = "value")
        {
            if (ConfigurationItems.Any(x => x.Key == key))
            {
                return;
            }

            ConfigurationItems.Add(ConfigurationElement.CreateConfigurationElement(key, val, "", "NULL", "NULL", attributeName: attribute));
        }

        public void AddConfigurationSetting(string key, string[] val, string[] attribute)
        {
            if (val.Length != attribute.Length)
            {
                Logging.WriteLogLine("AddConfigurationSetting: Configuration element could not be added. Values and Attributes are not equal", LoggingLevel.Error);
                return;
            }
            if (ConfigurationItems.Any(x => x.Key == key))
            {
                return;
            }

            ConfigurationItems.Add(new ConfigurationElement(key, val, "NULL", "NULL", attributeName: attribute));
        }

        public string GetConfigurationSetting(string key, string attribute = "value")
        {
            if (!DoesKeyExist(key, attribute))
            {
                return "";
            }
            return ConfigurationItems.First(x => x.Key == key).Attributes.First(y => y.Name == attribute).Value;
        }
        public string GetConfigurationSetting(string key, string Parent, string attribute = "value")
        {
            if (!DoesKeyExist(key, attribute))
            {
                Logging.WriteLogLine("Warning.  The configuration does not have the key: " + key + " registered.  Are you sure its defined?", LoggingLevel.Debug);
                return "";
            }
            if (!ConfigurationItems.Any(x => x.Key == key))
            {
                Logging.WriteLogLine("Warning.  The configuration does not have the key: " + key + " registered.  Are you sure its defined?", LoggingLevel.Debug);
                return "";
            }
            return ConfigurationItems.First(x => x.Key == key && x.ParentName == Parent).Attributes.First(y => y.Name == attribute).Value;

        }
        public void AddReference(string reference, string value)
        {
            ReferenceSubstitutions.Add(reference, value);
            ReplaceReferences();
        }
        public ConfigurationElement GetItemFromName(string name)
        {
            return ConfigurationItems.FirstOrDefault(x => x.Key == name);
        }
        public List<ConfigurationElement> GetItemsWithPartialParent(string parent)
        {
            return ConfigurationItems.Where(x => x.ParentName.EndsWith(parent)).ToList();
        }
        public List<ConfigurationElement> GetItemsWithParent(string parent)
        {
            return ConfigurationItems.Where(x => x.ParentName == parent).ToList();
        }
        public List<ConfigurationElement> GetItemsWithPartialParentAndAttributeValue(string parent, string Attribute, string AttributeValue)
        {
            return ConfigurationItems.Where(x => x.ParentName.EndsWith(parent) && x.GetAttribute(Attribute).Contains(AttributeValue)).ToList();
        }
        public List<ConfigurationElement> GetItemsWithAttributeValue(string Attribute, string AttributeValue)
        {
            return ConfigurationItems.Where(x => x.GetAttribute(Attribute) == AttributeValue).ToList();
        }
        public List<ConfigurationElement> GetItemsWhereAttributeContainsValue(string Attribute, string AttributeValue)
        {
            return ConfigurationItems.Where(x => x.GetAttribute(Attribute).Contains(AttributeValue)).ToList();
        }
        public List<ConfigurationElement> GetItemsWhereAttributeContainsExactValue(string Attribute, string AttributeValue)
        {
            var rawItemList = ConfigurationItems.Where(x => x.GetAttribute(Attribute).Contains(AttributeValue)).ToList();
            List<ConfigurationElement> ExactMatchList = new List<ConfigurationElement>();
            foreach (var item in rawItemList)
            {
                var itemAttribute = item.GetAttribute(Attribute);
                var individualAttributeValues = itemAttribute.Split(';').ToList();
                foreach (var value in individualAttributeValues)
                {
                    if (value.Trim() == AttributeValue)
                    {
                        ExactMatchList.Add(item);
                    }
                }
            }
            return ExactMatchList;
        }
        void ReplaceReferences()
        {
            foreach (var data in ConfigurationItems)
            {
                if (!data.Attributes.Any(x => x.Value.Contains('$')))
                {
                    continue;
                }

                foreach (var attribute in data.Attributes)
                {
                    string newValue = attribute.Value;
                    for (int i = 0; i < ReferenceSubstitutions.Count; i++)
                    {
                        if (attribute.Value.Contains(ReferenceSubstitutions.ElementAt(i).Key))
                        {
                            newValue = newValue.Replace(ReferenceSubstitutions.ElementAt(i).Key, ReferenceSubstitutions.ElementAt(i).Value);
                        }
                    }
                    if (newValue != attribute.Value)
                    {
                        StageChange(new ConfigurationChange(data.Key, attribute.Name, newValue, ConfigurationCategories.InternalConfigurationChange));
                    }
                }
            }
            ApplyChanges();
        }

        public List<ConfigurationElement> GetChildrenElements(string element)
        {
            return ConfigurationItems.Where(x => x.ParentName.Contains(element)).ToList();
        }
        public List<ConfigurationElement> GetChildrenElements(ConfigurationElement element)
        {
            return ConfigurationItems.Where(x => x.ParentUUID == element.UUID).ToList();
        }

        public void RegisterOperations(string operationType, Action<ConfigurationElement> action)
        {
            if (Operations.Exists(x => x.OperationName == operationType))
            {
                return;
            }

            Operations.Add(new ConfigurationOperation() { OperationName = operationType, OperationAction = action });
        }
        public ConfigurationOperation GetOperation(string operationType)
        {
            var Op = Operations.FirstOrDefault(x => x.OperationName == operationType);
            if (Op == null)
            {
                Logging.WriteLogLine("Operation of type: " + operationType + " was not registered by the ConfigurationManager, and thus could not be located.", LoggingLevel.Debug);
            }
            return Op;
        }

        public ConfigurationElement GetConfigurationElement(ConfigurationElement element)
        {
            return ConfigurationItems.FirstOrDefault(x => x.Key == element.Key && x.ParentName == element.ParentName);
        }
        public void PerformOperation(string operation, ConfigurationElement element)
        {
            Logging.WriteLogLine("Performing operation: " + operation + " on element: " + element.Key, LoggingLevel.Debug);
            GetOperation(operation).ExecuteOperation(element);
        }

        public List<ConfigurationOperation> GetElementOperations(ConfigurationElement element)
        {
            List<ConfigurationOperation> ElementOps = new List<ConfigurationOperation>();

            //List of Registered Operations
            var Ops = Operations.Select(x => x.OperationName).ToList();

            var Element = GetConfigurationElement(element);

            //Get parents
            List<string> Parents = new List<string>();
            Parents = Element.ParentName.Split('.').ToList();

            //Filter to be only valid operations
            Parents = Parents.Intersect(Ops).ToList();

            foreach (var parent in Parents)
            {
                var Op = GetOperation(parent);
                if (Op != null)
                {
                    ElementOps.Add(Op);
                }
            }

            return ElementOps;
        }

        public List<ConfigurationElement> QueryConfig(Func<ConfigurationElement, bool> query)
        {
            return ConfigurationItems.Where(query).ToList();
        }

        const string defaultConfigFile = "LauncherConfiguration.xml";

        IDictionary<string, string> ReferenceSubstitutions;

        //private Dictionary<string, string> ConfigurationSettings;

        private List<ConfigurationElement> ConfigurationItems;

        private List<ConfigurationOperation> Operations = new List<ConfigurationOperation>();

        List<ConfigurationManagerRegisteredChange> ConfigurationChanges = new List<ConfigurationManagerRegisteredChange>();

        IConfigLoading Loader;
        IFileManager FileManager;

    }
}
