using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using JohnUtilities.Classes;
using JohnUtilities.Services.Adapters;

namespace JohnUtilities.IntegrationTests
{


    [TestClass]
    public class FileManagerIntegrationTests
    {

        [TestInitialize]
        public void setup()
        {

        }

        [TestMethod]
        public void FileManager_CopyError()
        {
            Logging log = Logging.GetLogger().Init(new NNS_StreamWriter("CopyFileTestLog_Error.txt", true), null);

            NNS_FileService fileService = new NNS_FileService();

            var ProcessManager = new ProcessesManager();
            FileManager manager = new FileManager(fileService, ProcessManager);

            manager.Copy("TestFileError.txt", "NewTestFile.txt", true);
        }

        [TestMethod]
        public void FileManager_Copy()
        {
            Logging log = Logging.GetLogger().Init(new NNS_StreamWriter("CopyFileTestLog.txt", true), null);
            FileStream fs = File.Create("TestFileCopy.txt");
            fs.Close();
            NNS_FileService fileService = new NNS_FileService();

            var ProcessManager = new ProcessesManager();
            FileManager manager = new FileManager(fileService, ProcessManager);

            manager.Copy("TestFileCopy.txt", "NewTestFileCopy.txt", true);
        }
        [TestMethod]
        public void FileManager_CopyDirectory()
        {
            Logging log = Logging.GetLogger().Init(new NNS_StreamWriter("CopyDirectoryFileTestLog.txt", true), null);
            Directory.CreateDirectory(".\\CopyDirectoryIntegrationTestFolder");
            FileStream fs = File.Create(".\\CopyDirectoryIntegrationTestFolder\\TestFileCopy.txt");
            fs.Close();

            NNS_FileService fileService = new NNS_FileService();

            var ProcessManager = new ProcessesManager();
            FileManager manager = new FileManager(fileService, ProcessManager);

            manager.CopyDirectory(".\\CopyDirectoryIntegrationTestFolder", ".\\CopyDirectoryIntegrationTestFolderNew", true);
        }
        [TestMethod]
        public void FileManager_GetTextFromFile()
        {
            Logging log = Logging.GetLogger().Init(new NNS_StreamWriter("GetTextFromFile.txt", true), null);
            NNS_FileService fileService = new NNS_FileService();

            var ProcessManager = new ProcessesManager();
            FileManager manager = new FileManager(fileService, ProcessManager);

            Console.WriteLine(manager.GetTextFromFile("tc_profilevars", "CUSTOM_CODE_ROOT="));
            Console.WriteLine(manager.GetTextFromFile("tc_profilevars", "CUSTOM_CODE_ROOT=", '/', 3));
            Console.WriteLine(manager.GetTextFromFile("tc_profilevars", "CUSTOM_CODE_ROOT=", '/', ';', 3)) ;
        }

    }
}
