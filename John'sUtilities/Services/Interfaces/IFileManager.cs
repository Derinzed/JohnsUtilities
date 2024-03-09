using JohnUtilities.Services.Interfaces;

namespace JohnUtilities.Interfaces
{
    public interface IFileManager
    {
        INNS_FileService FileService { get; set; }

        bool Copy(string originalLocation, string destination, bool overwrite = false);
        int CopyDirectory(string originalLocation, string destination, bool overwrite);
        void Delete(string file);
        void DeleteDirectory(string startLocation, string[] searchFiles = null, string[] ignoreFiles = null);
        void DeleteFilesInDirectory(string directory, string[] searchFiles = null, string[] ignoreFiles = null);
        bool DirectoryExists(string path);
        bool FileExists(string path);
        string[] GetSubDirectories(string path);
        string GetTempPath();
        string GetTextFromFile(string file, string key);
        string GetTextFromFile(string file, string key, char delimiterOne, char delimiterTwo, int substr);
        string GetTextFromFile(string file, string key, char delimiterOne, int substr);
        string GetTextFromFile(string file, string key, string StartKey, char delimiterTwo);
        string GetUserProfilePath();
        void CreateDirectory(string path);
        int Robocopy(string source, string dest, bool overwrite, bool mirror = false);
    }
}