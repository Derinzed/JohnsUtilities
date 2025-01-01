using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JohnUtilities.Classes;
using JohnUtilities.Model.Classes;
using JohnUtilities.Services.Adapters;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using CSharpUtilities.CommandLineParser;

namespace JohnsUtilities.UpdaterLibrary
{
    public class UpdaterLibrary
    {
        public UpdaterLibrary(string assemblyDir, string updateXMLFile)
        {
            assemblyDirectory = assemblyDir;
            updateXMLDirectory = updateXMLFile;

            JU_XMLService XMLService = new JU_XMLService();
            ConfigLoading configLoading = new ConfigLoading(XMLService);
            ConfigMan = new ConfigurationManager(configLoading, new FileManager());
            ProcessesManager = new ProcessesManager();
            LoadConfig().GetAwaiter().GetResult();
        }
        public async Task LoadConfig()
        {
            await ConfigMan.LoadWebConfigFile(updateXMLDirectory);
        }
        public void StartUpdating()
        {
            ProcessesManager.StartProcess("updateScript");
            Environment.Exit(0);
        }
        public void GetUpdates()
        {
            if (UpdateAvailable())
            {
                string startPath = assemblyDirectory.Substring(0, assemblyDirectory.LastIndexOf('\\'));
                string rootProgramPath = Path.GetFullPath(Path.Combine(startPath, @"..\..\"));
                DownloadZipFile(ConfigMan.GetItemFromName("url").InnerText, startPath).GetAwaiter().GetResult();
                //this update method assumes the assembly is placed inside a bin folder, inside the root application directory.
                CreateUpdaterPatchingScript(rootProgramPath, startPath, assemblyDirectory);
            }
        }
        public bool UpdateAvailable()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(assemblyDirectory);
            string assemblyVersion = versionInfo.FileVersion;
            Version currentVersion = Version.Parse(assemblyVersion);
            Version ServerVersion = Version.Parse(ConfigMan.GetItemFromName("version").InnerText);

            if (currentVersion.CompareTo(ServerVersion) < 0)
            {
                return true;
            }
            return false;
        }

        async Task DownloadZipFile(string url, string filePath)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // Save the ZIP file to disk
                    using (FileStream fileStream = new FileStream(filePath + "\\update.zip", FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    throw new Exception($"Failed to download file: {response.StatusCode}");
                }
            }
        }

        public void CreateUpdaterPatchingScript(string updateDir, string updatePackage, string assemblyDir)
        {
            string batchScript = $@"
            @echo off
            timeout /t 2 >nul
            powershell -command ""Expand-Archive -Path '{updatePackage}\update.zip' -DestinationPath '{updateDir}' -Force""
            del /f /q ""{updatePackage}\update.zip"" 
            start """" ""{assemblyDir}""
            del ""%~f0""";

            File.WriteAllText(updateDir + @"\update.bat", batchScript);

            ProcessesManager.CreateProcess("updateScript", updateDir + @"\update.bat");
        }


        ConfigurationManager ConfigMan;
        ProcessesManager ProcessesManager;

        string assemblyDirectory;
        string updateXMLDirectory;
    }
}
