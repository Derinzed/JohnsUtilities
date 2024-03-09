using System.Collections.Generic;
using JohnUtilities.Classes;

namespace JohnUtilities.Interfaces
{
    public interface IConfigurationManager
    {
        void AddConfigurationSetting(string key, string attribute = "value", string val = "");
        void ApplyChanges();
        string GetConfigurationSetting(string key, string attribute = "value");
        void LoadEnvironmentConfigurationFile();
        void LoadLauncherConfigurationFile();
        void ParseConfig(string file, List<ConfigurationElement> container);
        void SaveChangesToFile();
        void StageChange(ConfigurationChange change);
    }
}