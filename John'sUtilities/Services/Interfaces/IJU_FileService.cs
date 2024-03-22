using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using JohnUtilities.Services.Adapters;

namespace JohnUtilities.Services.Interfaces
{
    public interface IJU_FileService
    {
        bool FileExists(string file);
        void Copy(string original, string destination, bool overwrite);
        string[] GetDirectories(string path);
        string[] GetDirectories(string path, string searchPattern, SearchOption searchOption);
        string[] GetFiles(string path);
        string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
        void CreateDirectory(string directory);
        void Delete(string path);
        void DeleteDirectory(string path, bool recursive);
        IEnumerable<string> ReadLines(string file);
        string GetTempPath();
        string GetUserProfilePath();

        void SetAttributes(string path, FileAttributes attributes);
        bool DirectoryExists(string directory);
        IJU_FileInfo GetFileInfo(string path);
        string GetDirectoryName(string directory);


    }
}
