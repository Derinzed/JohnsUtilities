using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using JohnUtilities.Services.Interfaces;
using JohnUtilities.Services.Adapters;
using JohnUtilities.Enums;
using System.Threading.Tasks;


namespace JohnUtilities.Classes
{
    public class EnvironmentalManager
    {
        public EnvironmentalManager()
        {
            EnvironmentalService = new JU_EnvironmentalService();
        }
        public string GetEnvironmentalVariable(string variable)
        {
            return EnvironmentalService.GetEnvironmentalVariable(variable);
        }
        public string GetEnvironmentalVariableTarget(string variable, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {
            return EnvironmentalService.GetEnvironmentalVariableTarget(variable, target);
        }
        public Task SetEnvironmentalVariableTask(string variable, string value)
        {
           return EnvironmentalService.SetEnvironmentalVariableTask(variable, value);
        }
        public void SetEnvironmentalVariable(string variable, string value)
        {
            EnvironmentalService.SetEnvironmentalVariable(variable, value);
        }
        public object GetRegistryKey(string keyName, string valueName, object defaultValue)
        {
            return EnvironmentalService.GetRegistryKey(keyName, valueName, defaultValue);
        }

        public void SetRegistryKey(string keyName, string valueName, object value)
        {
            EnvironmentalService.SetRegistryKey(keyName, valueName, value);
        }
        public void SetRegistryKey(string keyName, string valueName, object value, RegistryValueKind kind)
        {
            EnvironmentalService.SetRegistryKey(keyName, valueName, value, kind);
        }
    private RegistryPath? GetRegistryPath(string key)
        {
            string[] sep = { "\\" };
            string[] Ret = key.Split(sep, 2, StringSplitOptions.None);
            var RegName = Ret[0];
            RegistryPath RegPath = RegistryPath.HKEY_CURRENT_USER;

            switch (RegName)
            {
                case "HKEY_CLASSES_ROOT":
                    RegPath = RegistryPath.HKEY_CLASSES_ROOT;
                    break;
                case "HKEY_CURRENT_USER":
                    RegPath = RegistryPath.HKEY_CURRENT_USER;
                    break;
                case "HKEY_LOCAL_MACHINE":
                    RegPath = RegistryPath.HKEY_LOCAL_MACHINE;
                    break;
                case "HKEY_USERS":
                    RegPath = RegistryPath.HKEY_USERS;
                    break;
                case "HKEY_CURRENT_CONFIG":
                    RegPath = RegistryPath.HKEY_CURRENT_CONFIG;
                    break;
                default:
                    return null;
                    break;
            }

            return RegPath;
        }

        private string GetRegKeyString(string key)
        {
            string[] sep = { "\\" };
            string[] Ret = key.Split(sep, 2, StringSplitOptions.None);
            return Ret[1];
        }

        public void DeleteRegistryKey(string key)
        {
            var RegKey = GetRegKeyString(key);

            var RegPath = GetRegistryPath(key);
            if (RegPath == null)
            {
                Logging.WriteLogLine("DeleteRegistryKey operation could not be performed due to an incorrect RegistryPath.");
                return;
            }

            var Path = RegPath ?? default;
            EnvironmentalService.DeleteRegistryKey(RegKey, Path);
        }
        public void DeleteRegistryKeyTree(string key)
        {
            var RegKey = GetRegKeyString(key);

            var RegPath = GetRegistryPath(key);
            if (RegPath == null)
            {
                Logging.WriteLogLine("DeleteRegistryKeyTree operation could not be performed due to an incorrect RegistryPath.");
                return;
            }

            var Path = RegPath ?? default;
            EnvironmentalService.DeleteRegistryKeyTree(RegKey, Path);
        }
        public void DeleteRegistryName(string key, string value)
        {
            var RegKey = GetRegKeyString(key);

            var RegPath = GetRegistryPath(key);
            if (RegPath == null)
            {
                Logging.WriteLogLine("DeleteRegistryValue operation could not be performed due to an incorrect RegistryPath.");
                return;
            }

            var Path = RegPath ?? default;
            EnvironmentalService.DeleteRegistryValue(RegKey, value, Path);
        }
        public IJU_EnvironmentalService EnvironmentalService { get; set; }
    }
}
