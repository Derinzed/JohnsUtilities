using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using JohnUtilities.Services.Interfaces;

namespace JohnUtilities.Services.Adapters
{
    public class JU_FileService : IJU_FileService
    {
        public bool FileExists(string file)
        {
            return File.Exists(file);
        }
        public void Copy(string original, string destination, bool overwrite)
        {
            File.Copy(original, destination, overwrite);
        }
        public string[] GetDirectories(string path)
        {
            if (!DirectoryExists(path))
            {
                return new string[] { };
            }
            return Directory.GetDirectories(path);
        }
        public string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetDirectories(path, searchPattern, searchOption);
        }
        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }
        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetFiles(path, searchPattern, searchOption);
        }
        public void CreateDirectory(string directory)
        {
            Directory.CreateDirectory(directory);
        }
        public IEnumerable<string> ReadLines(string file)
        {
            return File.ReadLines(file);
        }
        public IJU_FileInfo GetFileInfo(string path)
        {
            return new JU_FileInfo(new FileInfo(path));
        }

        public void Delete(string path)
        {
            File.Delete(path);
        }
        public void DeleteDirectory(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }
        public string GetTempPath()
        {
            return Path.GetTempPath();
        }

        public string GetUserProfilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
        public void SetAttributes(string path, FileAttributes attributes)
        {
            File.SetAttributes(path, attributes);
        }
        public bool DirectoryExists(string directory)
        {
            return Directory.Exists(directory);
        }
        public string GetDirectoryName(string directory)
        {
            return Path.GetDirectoryName(directory);
        }
    }
}
