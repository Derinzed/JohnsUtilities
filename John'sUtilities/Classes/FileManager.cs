using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using JohnUtilities.Services.Interfaces;
using System.Linq;
using JohnUtilities.Interfaces;


namespace JohnUtilities.Classes
{
    public class FileManager : IFileManager
    {
        public FileManager()
        {

        }
        public FileManager(INNS_FileService service, IProcessesManager procMan)
        {
            FileService = service;
            ProcessManager = procMan;

            ProcessManager.CreateProcess("Robocopy", "robocopy.exe", "", StartMinimized: true, UseShellExecute: true, NoWindow: true, WaitForExit: true);
        }
        public bool Copy(string originalLocation, string destination, bool overwrite = false)
        {
            try
            {
                if (overwrite == true)
                {
                    Logging.WriteLogLine("Copying file from: " + originalLocation + " to: " + destination + ".  Overwrite set to: " + overwrite.ToString(), LoggingLevel.Debug);
                    FileService.CreateDirectory(FileService.GetDirectoryName(destination));
                    FileService.Copy(originalLocation, destination, overwrite);
                    return true;
                }

                INNS_FileInfo file = FileService.GetFileInfo(originalLocation);
                INNS_FileInfo destFile = FileService.GetFileInfo(destination);
                if (destFile.Exists())
                {
                    if (file.LastWriteTime() > destFile.LastWriteTime())
                    {
                        Logging.WriteLogLine("Copying file from: " + originalLocation + " to: " + destination + ".  Overwrite set to: " + overwrite.ToString(), LoggingLevel.Debug);
                        FileService.Copy(originalLocation, destination, overwrite);
                        return true;
                    }
                    return false;
                }
                Logging.WriteLogLine("Copying file from: " + originalLocation + " to: " + destination + ".  Overwrite set to: " + overwrite.ToString(), LoggingLevel.Debug);
                FileService.Copy(originalLocation, destination, overwrite);
                return true;
            }
            catch (SystemException ex)
            {
                Logging.WriteLogLine("An Error was encountered during the copy operation");
                Logging.WriteLogLine(ex.ToString());
                return false;
            }
            return false;
        }
        public int CopyDirectory(string originalLocation, string destination, bool overwrite)
        {
            Logging.WriteLogLine("Copying directory from: " + originalLocation + " to: " + destination + ".  Overwrite set to: " + overwrite.ToString(), LoggingLevel.Debug);
            try
            {
                int copies = 0;
                Directory.CreateDirectory(destination);
                //Now Create all of the directories
                foreach (string dirPath in FileService.GetDirectories(originalLocation, "*", SearchOption.AllDirectories))
                {
                    FileService.CreateDirectory(dirPath.Replace(originalLocation, destination));
                }
                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in FileService.GetFiles(originalLocation, "*.*", SearchOption.AllDirectories))
                {
                    if (Copy(newPath, newPath.Replace(originalLocation, destination), overwrite) == true)
                    {
                        copies++;
                    }
                }
                return copies;
            }
            catch (SystemException ex)
            {
                Logging.WriteLogLine("An Error was encountered during the CopyDirectory operation");
                Logging.WriteLogLine(ex.ToString());
                return 0;
            }
        }

        public string GetTextFromFile(string file, string key)
        {
            var lines = FileService.ReadLines(file);
            foreach (var line in lines)
            {
                if (line.Contains(key))
                {
                    return line.Replace(key, "");
                }
            }
            return "";
        }
        public string GetTextFromFile(string file, string key, char delimiterOne, int substr)
        {
            var lines = FileService.ReadLines(file);
            foreach (var line in lines)
            {
                if (line.Contains(key))
                {
                    return line.Split(delimiterOne)[substr];
                }
            }
            return "";
        }
        public string GetTextFromFile(string file, string key, char delimiterOne, char delimiterTwo, int substr)
        {
            var lines = FileService.ReadLines(file);
            foreach (var line in lines)
            {
                if (line.Contains(key))
                {
                    return line.Split(delimiterOne, delimiterTwo)[substr];
                }
            }
            return "";
        }
        public string GetTextFromFile(string file, string key, string StartKey, char delimiterTwo)
        {
            var lines = FileService.ReadLines(file);
            foreach (var line in lines)
            {
                if (line.Contains(key))
                {
                    string editedLine = line.Remove(0, line.IndexOf(StartKey) + StartKey.Length);
                    return editedLine.Split(delimiterTwo)[0];
                }
            }
            return "";
        }

        public void Delete(string file)
        {
            try
            {
                FileService.SetAttributes(file, FileAttributes.Normal);
                FileService.Delete(file);
                Logging.WriteLogLine("Deleted file: " + file, LoggingLevel.Debug);
            }
            catch (IOException ex)
            {
                Logging.WriteLogLine("Warning. File could not be deleted.");
                Logging.WriteLogLine(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                Logging.WriteLogLine("Warning. File could not be deleted.");
                Logging.WriteLogLine(ex.Message);
            }
        }
        public void DeleteFilesInDirectory(string directory, string[] searchFiles = null, string[] ignoreFiles = null)
        {
            List<string> files = new List<string>();
            if (searchFiles != null)
            {
                foreach (var searchtype in searchFiles)
                {
                    var newList = FileService.GetFiles(directory, searchtype, new SearchOption()).ToList();
                    files.AddRange(newList);
                }
            }
            else
            {
                files = FileService.GetFiles(directory).ToList();
            }
            if (ignoreFiles != null)
            {
                foreach (var ignoreType in ignoreFiles)
                {
                    var filesToIgnore = FileService.GetFiles(directory, ignoreType, new SearchOption()).ToList();
                    files = files.Except(filesToIgnore).ToList();
                }
            }
            foreach (var file in files)
            {
                Delete(file);
            }
        }
        public void DeleteDirectory(string startLocation, string[] searchFiles = null, string[] ignoreFiles = null)
        {
            if (!FileService.DirectoryExists(startLocation))
            {
                Logging.WriteLogLine("Warning. The directory: " + startLocation + " does not exist and cannot be deleted");
                return;
            }
            string[] subDirectories = FileService.GetDirectories(startLocation);
            foreach (var directory in subDirectories)
            {
                DeleteDirectory(directory, searchFiles, ignoreFiles);
                if (FileService.GetFiles(directory).Length > 0)
                {
                    DeleteFilesInDirectory(directory, searchFiles, ignoreFiles);
                }
                if (FileService.GetFiles(directory).Length == 0 &&
                    FileService.GetDirectories(directory).Length == 0)
                {
                    FileService.DeleteDirectory(directory, false);
                }
            }
        }
        public int Robocopy(string source, string dest, bool overwrite, bool mirror = false)
        {
            var roboargs = CreateRobocopyArgs(source, dest, overwrite, mirror);
            ProcessManager.SetProcessArguments("Robocopy", roboargs);
            Logging.WriteLogLine("Robocopy with arguments: " + roboargs, LoggingLevel.Debug);
            ProcessManager.StartProcess("Robocopy");

            return ProcessManager.GetExitCode("Robocopy");
        }
        public string GetTempPath()
        {
            var path = FileService.GetTempPath();
            //remove last character to maintain standard format.
            path = path.Substring(0, path.Length - 1);
            return path;
        }
        public string GetUserProfilePath()
        {
            return FileService.GetUserProfilePath();
        }

        public bool FileExists(string path)
        {
            return FileService.FileExists(path);
        }
        public bool DirectoryExists(string path)
        {
            return FileService.DirectoryExists(path);
        }

        public string[] GetSubDirectories(string path)
        {
            return FileService.GetDirectories(path);
        }

        public void CreateDirectory(string path)
        {
            FileService.CreateDirectory(path);
        }

        private string CreateRobocopyArgs(string source, string dest, bool overwrite, bool mirror)
        {
            var args = source + " " + dest;

            if (mirror == true)
            {
                args += " /MIR";
            }
            else
            {
                args += " /e";
            }

            if (overwrite == true)
            {
                args += " /IS /IT /mt";
            }
            else
            {
                args += " /XO /mt";
            }
            return args;
        }

        public INNS_FileService FileService { get; set; }
        private IProcessesManager ProcessManager;
    }
}
