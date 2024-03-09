using System;
using System.Collections.Generic;
using System.Text;

namespace JohnUtilities.Classes
{
    public class SettingsProcessor
    {

        public SettingsProcessor(ConfigurationManager manager)
        {
            ConfigManager = manager;
        }

        public void ToggleSettingTrueFalse(string setting)
        {
            var old = Convert.ToBoolean(ConfigManager.GetConfigurationSetting(setting));
            var newVal = old ? false : true;
            ApplyChange(setting, "value", newVal.ToString(), ConfigurationCategories.LauncherConfiguration);
        }
        public void SetSettingTrueFalse(string setting, bool value)
        {
            var old = Convert.ToBoolean(ConfigManager.GetConfigurationSetting(setting));
            var newVal = value;
            if (old != newVal)
            {
                ApplyChange(setting, "value", newVal.ToString(), ConfigurationCategories.LauncherConfiguration);
            }
        }
        public void SetSetting(string setting, string value, string attribute = "value")
        {
            var old = ConfigManager.GetConfigurationSetting(setting);
            var newVal = value;
            if (old != newVal)
            {
                ApplyChange(setting, attribute, newVal.ToString(), ConfigurationCategories.LauncherConfiguration);
            }
        }
        public void ApplyChange(string nodeName, string attributeName, string attributeValue, ConfigurationCategories category)
        {
            var NewChange = new ConfigurationChange(nodeName, attributeName, attributeValue, category);

            ConfigManager.StageChange(NewChange);
            ConfigManager.ApplyChanges();
            ConfigManager.SaveChangesToFile();
        }

        ConfigurationManager ConfigManager;
    }
}
