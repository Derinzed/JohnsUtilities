using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using JohnUtilities.Classes;
using JohnUtilities.Enums;
using System.Threading.Tasks;

namespace JohnUtilities.Services.Interfaces
{
    public interface IJU_EnvironmentalService
    {
        string GetEnvironmentalVariable(string variable);
        string GetEnvironmentalVariableTarget(string variable, EnvironmentVariableTarget target = EnvironmentVariableTarget.User);
        Task SetEnvironmentalVariableTask(string variable, string value);
        void SetEnvironmentalVariable(string variable, string value);

        object GetRegistryKey(string keyName, string valueName, object defaultValue);
        void SetRegistryKey(string keyName, string valueName, object value, RegistryValueKind kind);

        void SetRegistryKey(string keyName, string valueName, object value);
        void DeleteRegistryKey(string subKey, RegistryPath path);
        void DeleteRegistryValue(string subKey, string value, RegistryPath path);
        void DeleteRegistryKeyTree(string subKey, RegistryPath path);
    }
}
