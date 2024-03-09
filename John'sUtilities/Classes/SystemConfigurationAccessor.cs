using JohnUtilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;


namespace JohnUtilities.Classes
{
    public class SystemConfigurationAccessor
    {

        public SystemConfigurationAccessor(IConfigurationManager configMan, IFileManager FileMan)
        {
            ConfigManager = configMan;
            FileManager = FileMan;
        }

        public string GetConfigTCVersion()
        {
            return FileManager.GetTextFromFile(ConfigManager.GetConfigurationSetting("TeamcenterConfig"), "<version value=\"", '\"', 1);
        }
        public string GetVersionlocalTCVersion()
        {
            return FileManager.GetTextFromFile(ConfigManager.GetConfigurationSetting("TeamcenterVersionLocal"), "<key id=\"about_fullVersion\">", '>', '<', 2);
        }
        public string GetTCReq()
        {
            string requirementsFile = ConfigManager.GetConfigurationSetting("TC_EnvVersions");
            string env = ConfigManager.GetConfigurationSetting("TCENV") + ";";
            string tc_Req = FileManager.GetTextFromFile(requirementsFile, env, "Tc=", ';');

            return tc_Req;
        }
        public string GetNXReq()
        {
            string requirementsFile = ConfigManager.GetConfigurationSetting("TC_EnvVersions");
            string env = ConfigManager.GetConfigurationSetting("TCENV") + ";";
            string tc_NX = FileManager.GetTextFromFile(requirementsFile, env, "NX=", ';');

            return tc_NX;
        }
        public string GetTCVisReq()
        {
            string requirementsFile = ConfigManager.GetConfigurationSetting("TC_EnvVersions");
            string env = ConfigManager.GetConfigurationSetting("TCENV") + ";";
            string tcVis_Req = FileManager.GetTextFromFile(requirementsFile, env, "TcVis=", ';');

            return tcVis_Req;
        }
        public string GetNXVersion()
        {
            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = ConfigManager.GetConfigurationSetting("NX_Env_Print");
            p.StartInfo.Arguments = "-m";
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            return p.StandardOutput.ReadToEnd().Trim();
        }

        public string GetCodeDropLevelFromProfileVars()
        {
            return FileManager.GetTextFromFile(ConfigManager.GetConfigurationSetting("ProfileVars"), "CUSTOM_CODE_ROOT=", '/', ';', 3);
        }

        IConfigurationManager ConfigManager;
        IFileManager FileManager;
    }
}
