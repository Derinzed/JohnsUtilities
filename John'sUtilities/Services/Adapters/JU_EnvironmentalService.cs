using System;
using System.Collections.Generic;
using System.Text;
using JohnUtilities.Services.Interfaces;
using Microsoft.Win32;
using JohnUtilities.Classes;
using JohnUtilities.Enums;
using System.Threading.Tasks;

namespace JohnUtilities.Services.Adapters
{
    public class JU_EnvironmentalService : IJU_EnvironmentalService
    {
        public string GetEnvironmentalVariable(string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }
        public string GetEnvironmentalVariableTarget(string variable, EnvironmentVariableTarget target = EnvironmentVariableTarget.User)
        {
            return Environment.GetEnvironmentVariable(variable, target);
        }
        public Task SetEnvironmentalVariableTask(string variable, string value)
        {
            Logging.WriteLogLine("Setting Environmental Variable: " + variable + " " + value);

            Task.Factory.StartNew(() => Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.User));
            return Task.Factory.StartNew(() => Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.Process));
        }
        public void SetEnvironmentalVariable(string variable, string value)
        {
            Logging.WriteLogLine("Setting Environmental Variable: " + variable + " " + value);
            Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.Process);
        }
        public object GetRegistryKey(string keyName, string valueName, object defaultValue)
        {
            return Registry.GetValue(keyName, valueName, defaultValue);
        }

        public void SetRegistryKey(string keyName, string valueName, object value)
        {
            Registry.SetValue(keyName, valueName, value);
        }
        public void SetRegistryKey(string keyName, string valueName, object value, RegistryValueKind kind)
        {
            Registry.SetValue(keyName, valueName, value, kind);
        }
        public void DeleteRegistryKey(string subKey, RegistryPath path)
        {
            switch (path)
            {
                case RegistryPath.HKEY_CLASSES_ROOT:
                    {
                        Registry.ClassesRoot.DeleteSubKey(subKey);
                        break;
                    }
                case RegistryPath.HKEY_CURRENT_USER:
                    {
                        Registry.CurrentUser.DeleteSubKey(subKey);
                        break;
                    }
                case RegistryPath.HKEY_LOCAL_MACHINE:
                    {
                        Registry.LocalMachine.DeleteSubKey(subKey);
                        break;
                    }
                case RegistryPath.HKEY_USERS:
                    {
                        Registry.Users.DeleteSubKey(subKey);
                        break;
                    }
                case RegistryPath.HKEY_CURRENT_CONFIG:
                    {
                        Registry.CurrentConfig.DeleteSubKey(subKey);
                        break;
                    }
                default: break;
            }

        }
        public void DeleteRegistryKeyTree(string subKey, RegistryPath path)
        {
            switch (path)
            {
                case RegistryPath.HKEY_CLASSES_ROOT:
                    {
                        Registry.ClassesRoot.DeleteSubKeyTree(subKey);
                        break;
                    }
                case RegistryPath.HKEY_CURRENT_USER:
                    {
                        Registry.CurrentUser.DeleteSubKeyTree(subKey);
                        break;
                    }
                case RegistryPath.HKEY_LOCAL_MACHINE:
                    {
                        Registry.LocalMachine.DeleteSubKeyTree(subKey);
                        break;
                    }
                case RegistryPath.HKEY_USERS:
                    {
                        Registry.Users.DeleteSubKeyTree(subKey);
                        break;
                    }
                case RegistryPath.HKEY_CURRENT_CONFIG:
                    {
                        Registry.CurrentConfig.DeleteSubKeyTree(subKey);
                        break;
                    }
                default: break;
            }

        }
        public void DeleteRegistryValue(string subKey, string value, RegistryPath path)
        {
            RegistryKey key = null;
            switch (path)
            {
                case RegistryPath.HKEY_CLASSES_ROOT:
                    {
                        key = Registry.ClassesRoot.OpenSubKey(subKey, true);
                        break;
                    }
                case RegistryPath.HKEY_CURRENT_USER:
                    {
                        key = Registry.CurrentUser.OpenSubKey(subKey, true);
                        break;
                    }
                case RegistryPath.HKEY_LOCAL_MACHINE:
                    {
                        key = Registry.LocalMachine.OpenSubKey(subKey, true);
                        break;
                    }
                case RegistryPath.HKEY_USERS:
                    {
                        key = Registry.Users.OpenSubKey(subKey, true);
                        break;
                    }
                case RegistryPath.HKEY_CURRENT_CONFIG:
                    {
                        key = Registry.CurrentConfig.OpenSubKey(subKey, true);
                        break;
                    }
                default: break;
            }

            if (key != null)
            {
                PerformRegistryValueDeletion(key, value);
            }

        }

        private void PerformRegistryValueDeletion(RegistryKey key, string value)
        {
            if (key.GetValue(value) != null)
            {
                key.DeleteValue(value);
            }
        }
    }
}
