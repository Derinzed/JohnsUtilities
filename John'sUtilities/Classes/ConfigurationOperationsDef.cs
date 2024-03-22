using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnUtilities.Interfaces;

using JohnUtilities.Classes;
using JohnUtilities.Services.Interfaces;

namespace JohnUtilities.Classes
{
    public class ConfigurationOperationsDef
    {
        public ConfigurationOperationsDef(IConfigurationManager configMan, IFileManager fileMan, EnvironmentalManager envMan, IJU_ProcessService procServ)
        {
            ConfigManager = configMan;
            FileManager = fileMan;
            EnvironmentManager = envMan;
            ProcessService = procServ;
        }

        public void PerformCopyFilesOperations(ConfigurationElement item)
        {
            Logging.WriteLogLine("Performing CopyFiles operation: " + item.Key, LoggingLevel.Debug);
            var overwrite = false;
            bool.TryParse(item.GetAttribute("overwrite"), out overwrite);
            FileManager.Robocopy(item.GetAttribute("from"), item.GetAttribute("to"), overwrite);
        }
        public void PerformCopyDirectoriesOperations(ConfigurationElement item)
        {
            Logging.WriteLogLine("Performing CopyDirectories operation: " + item.Key, LoggingLevel.Debug);
            var overwrite = false;
            bool.TryParse(item.GetAttribute("overwrite"), out overwrite);
            FileManager.Robocopy(item.GetAttribute("from"), item.GetAttribute("to"), overwrite);
        }

        public void PerformRegistryValuesOperations(ConfigurationElement item)
        {
            var remove = false;
            if (!bool.TryParse(ConfigManager.GetConfigurationSetting(item.Key, "remove"), out remove))
            {
                remove = false;
            }

            if (remove == true)
            {
                Logging.WriteLogLine("Performing RegistryValues operation: " + item.Key, LoggingLevel.Debug);
                EnvironmentManager.DeleteRegistryName(ConfigManager.GetConfigurationSetting(item.Key, "path"), ConfigManager.GetConfigurationSetting(item.Key, "name"));
                return;
            }

            Logging.WriteLogLine("Performing RegistryValues operation: " + item.Key + " with name: " + ConfigManager.GetConfigurationSetting(item.Key, "name") + " With value: " + ConfigManager.GetConfigurationSetting(item.Key, "value"), LoggingLevel.Debug);
            EnvironmentManager.SetRegistryKey(ConfigManager.GetConfigurationSetting(item.Key, "path"), ConfigManager.GetConfigurationSetting(item.Key, "name"), ConfigManager.GetConfigurationSetting(item.Key, "value"));
           
        }

        public void PerformKillDefinedProcesses(ConfigurationElement element)
        {
            //kill defined processes
            ProcessService.Kill(element.Key);
        }

        public void PerformEnvironmentVariablesOperation(ConfigurationElement element)
        {
            Logging.WriteLogLine("Performing EnvironmentalVariables operation: " + element.Key + " to: " + " " + ConfigManager.GetConfigurationSetting(element.Key), LoggingLevel.Debug);
            EnvironmentManager.SetEnvironmentalVariable(element.Key, ConfigManager.GetConfigurationSetting(element.Key));
        }

        IConfigurationManager ConfigManager;
        IFileManager FileManager;
        EnvironmentalManager EnvironmentManager;
        IJU_ProcessService ProcessService;
    }
}
