using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnUtilities.Services.Interfaces;
using JohnUtilities.Model.Classes;
using System.IO;
using JohnUtilities.Classes;
using JohnUtilities.Interfaces;

namespace JohnUtilities.UnitTests
{
    [TestClass]
    public class FileManagerUnitTests
    {

        [TestMethod]
        public void Copy_ParametersPassedCorrectly()
        {

            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();

            var info_mock = new Mock<INNS_FileInfo>();
            info_mock.Setup(x => x.Exists()).Returns(false);
            info_mock.Setup(x => x.LastWriteTime()).Returns(new DateTime(1, 1, 1, 1, 4, 1, 1));

            FileService_Mock.Setup(x => x.GetFileInfo(It.IsAny<string>())).Returns(info_mock.Object);
            FileService_Mock.Setup(x => x.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Verifiable();

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);
            fileManager.Copy("Orig", "Dest", false);

            FileService_Mock.Verify(x => x.Copy("Orig", "Dest", false), Times.Once);

        }
        [TestMethod]
        public void Copy_GivenOverwriteTrue_CopyFile()
        {

            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            var info1_mock = new Mock<INNS_FileInfo>();
            info1_mock.Setup(x => x.Exists()).Returns(true);
            info1_mock.Setup(x => x.LastWriteTime()).Returns(new DateTime(1, 1, 1, 1, 1, 1, 1));
            var info2_mock = new Mock<INNS_FileInfo>();
            info2_mock.Setup(x => x.LastWriteTime()).Returns(new DateTime(1, 1, 1, 1, 4, 1, 1));

            FileService_Mock.SetupSequence(x => x.GetFileInfo(It.IsAny<string>())).Returns(info2_mock.Object).Returns(info1_mock.Object);

            FileService_Mock.Setup(x => x.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Verifiable();

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);
            fileManager.Copy("Orig", "Dest", true);

            FileService_Mock.Verify(x => x.Copy("Orig", "Dest", true), Times.Once);

        }
        [TestMethod]
        public void Copy_ExceptionThrown()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            SystemException ex = new SystemException("this is a test");

            var info_mock = new Mock<INNS_FileInfo>();
            info_mock.Setup(x => x.Exists()).Returns(false);
            info_mock.Setup(x => x.LastWriteTime()).Returns(new DateTime(1, 1, 1, 1, 4, 1, 1));

            FileService_Mock.Setup(x => x.GetFileInfo(It.IsAny<string>())).Returns(info_mock.Object);
            FileService_Mock.Setup(x => x.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(ex);

            var StreamWriter_Mock = new Mock<INNS_StreamWriter>();
            StreamWriter_Mock.Setup(x => x.WriteLine(It.IsAny<string>())).Verifiable();

            Logging log = Logging.GetLogger().Init(StreamWriter_Mock.Object, null);

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);
            fileManager.Copy("Orig", "Dest", false);


            StreamWriter_Mock.Verify(x => x.WriteLine(DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " - " + ex.ToString()), Times.Once);


        }
        [TestMethod]
        public void CopyDestination_ParametersPassedCorrectly()
        {

            var info_mock = new Mock<INNS_FileInfo>();
            info_mock.Setup(x => x.Exists()).Returns(false);


            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            FileService_Mock.Setup(x => x.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Verifiable();
            FileService_Mock.Setup(x => x.GetDirectories(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new string[] { "Orig", "Orig2" });
            FileService_Mock.Setup(x => x.GetFiles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new string[] { "Test1", "Test2" });
            FileService_Mock.Setup(x => x.CreateDirectory(It.IsAny<string>())).Verifiable();
            FileService_Mock.Setup(x => x.GetFileInfo(It.IsAny<string>())).Returns(info_mock.Object);

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);
            fileManager.CopyDirectory("Orig", "Dest", false);

            FileService_Mock.Verify(x => x.Copy("Test1", "Test1", false), Times.Once);
            FileService_Mock.Verify(x => x.CreateDirectory("Dest"), Times.Once);
        }
        [TestMethod]
        public void Copy_GivenNewerSource_CopyFile()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            var info1_mock = new Mock<INNS_FileInfo>();
            info1_mock.Setup(x => x.Exists()).Returns(true);
            info1_mock.Setup(x => x.LastWriteTime()).Returns(new DateTime(1, 1, 1, 1, 1, 1, 1));
            var info2_mock = new Mock<INNS_FileInfo>();
            info2_mock.Setup(x => x.LastWriteTime()).Returns(new DateTime(1, 1, 1, 1, 4, 1, 1));

            FileService_Mock.SetupSequence(x => x.GetFileInfo(It.IsAny<string>())).Returns(info2_mock.Object).Returns(info1_mock.Object);

            FileService_Mock.Setup(x => x.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Verifiable();

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);
            fileManager.Copy("Orig", "Dest", false);

            FileService_Mock.Verify(x => x.Copy("Orig", "Dest", false), Times.Once);
        }
        [TestMethod]
        public void Copy_GivenSameSource_DontCopyFile()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            var info1_mock = new Mock<INNS_FileInfo>();
            info1_mock.Setup(x => x.LastWriteTime()).Returns(new DateTime(1, 1, 1, 1, 1, 1, 1));
            var info2_mock = new Mock<INNS_FileInfo>();
            info2_mock.Setup(x => x.Exists()).Returns(true);
            info2_mock.Setup(x => x.LastWriteTime()).Returns(new DateTime(1, 1, 1, 1, 1, 1, 1));

            FileService_Mock.SetupSequence(x => x.GetFileInfo(It.IsAny<string>())).Returns(info2_mock.Object).Returns(info1_mock.Object);

            FileService_Mock.Setup(x => x.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Verifiable();

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);
            fileManager.Copy("Orig", "Dest", false);

            FileService_Mock.Verify(x => x.Copy("Test1", "Test1", false), Times.Never);
        }
        [TestMethod]
        public void CopyDestination_ExceptionThrown()
        {
            var info_mock = new Mock<INNS_FileInfo>();
            info_mock.Setup(x => x.Exists()).Returns(false);

            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            SystemException ex = new SystemException("this is a test");
            FileService_Mock.Setup(x => x.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(ex);
            FileService_Mock.Setup(x => x.GetDirectories(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new string[] { "Orig", "Orig2" });
            FileService_Mock.Setup(x => x.GetFiles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new string[] { "Test1", "Test2" });
            FileService_Mock.Setup(x => x.CreateDirectory(It.IsAny<string>())).Verifiable();
            FileService_Mock.Setup(x => x.GetFileInfo(It.IsAny<string>())).Returns(info_mock.Object);

            var StreamWriter_Mock = new Mock<INNS_StreamWriter>();
            StreamWriter_Mock.Setup(x => x.WriteLine(It.IsAny<string>())).Verifiable();

            Logging log = Logging.GetLogger().Init(StreamWriter_Mock.Object, null);

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);
            fileManager.CopyDirectory("Orig", "Dest", false);

            StreamWriter_Mock.Verify(x => x.WriteLine(DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " - " + ex.ToString()), Times.Exactly(2));
        }
        [TestMethod]
        public void FileManager_GetTextFromFile_NoDelimiter()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            FileService_Mock.Setup(x => x.ReadLines(It.IsAny<string>())).Returns(new List<string> {"Line1", "Line2=realy cool test line."});

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);
            var foundLine = fileManager.GetTextFromFile("randomFile", "Line2");

            Assert.AreEqual("=realy cool test line.", foundLine);

        }
        [TestMethod]
        public void FileManager_GetTextFromFile_SingleDelimiter()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            FileService_Mock.Setup(x => x.ReadLines(It.IsAny<string>())).Returns(new List<string> { "Line1", "Line2=realy cool test line.","CUSTOM_CODE_ROOT=/tc/BPR/CD2128_DEV; export CUSTOM_CODE_ROOT" });
            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);
            var foundLine = fileManager.GetTextFromFile("randomFile", "CUSTOM_CODE_ROOT", '/',3);

            Assert.AreEqual("CD2128_DEV; export CUSTOM_CODE_ROOT", foundLine);

        }
        [TestMethod]
        public void FileManager_GetTextFromFile_StartAndEndDelimiter()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            FileService_Mock.Setup(x => x.ReadLines(It.IsAny<string>())).Returns(new List<string> { "Line1", "Line2=realy cool test line.","CUSTOM_CODE_ROOT=/tc/BPR/CD2128_DEV; export CUSTOM_CODE_ROOT" });

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);
            var foundLine = fileManager.GetTextFromFile("randomFile", "CUSTOM_CODE_ROOT", '/', ';', 3);

            Assert.AreEqual("CD2128_DEV", foundLine);

        }
        [TestMethod]
        public void FileManager_GetTextFromFile_StartKey()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            FileService_Mock.Setup(x => x.ReadLines(It.IsAny<string>())).Returns(new List<string> { "Line1", "Line2=realy cool test line.", "CUSTOM_CODE_ROOT=/tc/BPR/CD2128_DEV; export CUSTOM_CODE_ROOT", "DEV; NX=NX 2212.7001; Tc=13.3.0.10; TcVis=133010.230329; License=27627@TCDEVLS" });

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);
            var foundLine = fileManager.GetTextFromFile("randomFile", "DEV; N", "Tc=", ';');

            Assert.AreEqual("13.3.0.10", foundLine);

        }

        [TestMethod]
        public void FileManager_Delete_CallesFileServiceDelete()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            FileService_Mock.Setup(x => x.Delete(It.IsAny<string>())).Verifiable();

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);

            fileManager.Delete("SomeFile");

            FileService_Mock.Verify(x => x.Delete("SomeFile"));
        }
        [TestMethod]
        public void FileManager_DeleteFilesInDirectory_CallsFileServiceDelete()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            FileService_Mock.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(new string[] { "File1", "File2", "File3.tif" });
            FileService_Mock.Setup(x => x.GetFiles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new string[] { "File3.tif" });
            FileService_Mock.Setup(x => x.Delete(It.IsAny<string>())).Verifiable();

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);

            fileManager.DeleteFilesInDirectory("SomeFile", null, new string[] { "*.tif" });

            FileService_Mock.Verify(x => x.Delete("File1"), Times.Once);
            FileService_Mock.Verify(x => x.Delete("File2"), Times.Once);
            FileService_Mock.Verify(x => x.Delete("File3.tif"), Times.Never);
        }

        [TestMethod]
        public void FileManager_GetTempPath_FileServiceCalledAndRemoveLastSlashes()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            FileService_Mock.Setup(x => x.GetTempPath()).Returns("TempPath\\");

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);

            var path = fileManager.GetTempPath();

            FileService_Mock.Verify(x => x.GetTempPath(), Times.Once);
            Assert.AreEqual("TempPath", path);
        }

        [TestMethod]
        public void FileManager_GetUserProfilePath_FileServiceCalled()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            FileService_Mock.Setup(x => x.GetUserProfilePath()).Returns("TempPath");

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);

            fileManager.GetUserProfilePath();

            FileService_Mock.Verify(x => x.GetUserProfilePath(), Times.Once);
        }
        //might need a revisit
        [TestMethod]
        public void FileManager_DeleteDirectory_FileServiceDeleteDirecotryCalled()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            FileService_Mock.SetupSequence(x => x.GetDirectories(It.IsAny<string>())).Returns(new string[] { "Directory1" }).Returns(new string[] { }).Returns(new string[] { });
            FileService_Mock.SetupSequence(x => x.DirectoryExists(It.IsAny<string>())).Returns(true);
            FileService_Mock.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(new string[] {  });
            FileService_Mock.Setup(x => x.DeleteDirectory(It.IsAny<string>(), It.IsAny<bool>())).Verifiable();

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);

            fileManager.DeleteDirectory("SomeFile", new string[] { "*.tif" });

            FileService_Mock.Verify(x => x.DirectoryExists(It.IsAny<string>()), Times.Exactly(2));
            FileService_Mock.Verify(x => x.DeleteDirectory("Directory1", false), Times.Once);
        }
        [TestMethod]
        public void FileManager_FileExists_ReturnTrue()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            FileService_Mock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);

            Assert.IsTrue(fileManager.FileExists("SomeRandomPath"));
        }
        [TestMethod]
        public void FileManager_FileExists_ReturnFalse()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            FileService_Mock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);

            Assert.IsFalse(fileManager.FileExists("SomeRandomPath"));
        }
        [TestMethod]
        public void FileManager_DirectoryExists_ReturnTrue()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            FileService_Mock.Setup(x => x.DirectoryExists(It.IsAny<string>())).Returns(true);

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);

            Assert.IsTrue(fileManager.DirectoryExists("SomeRandomPath"));
        }
        [TestMethod]
        public void FileManager_DirectoryExists_ReturnFalse()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            FileService_Mock.Setup(x => x.DirectoryExists(It.IsAny<string>())).Returns(false);

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);

            Assert.IsFalse(fileManager.DirectoryExists("SomeRandomPath"));
        }
        [TestMethod]
        public void FileManager_GetSubdirectories_ReturnsProperDirectories()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            FileService_Mock.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(new string[] { "one", "Two", "Three" });

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);
            var directories = fileManager.GetSubDirectories("SomePath");

            Assert.AreEqual("one", directories[0]);
            Assert.AreEqual("Two", directories[1]);
            Assert.AreEqual("Three", directories[2]);
        }
        [TestMethod]
        public void FileManager_CopyDirectories_ReturnsProperNumberOfCopies()
        {
            var info_mock = new Mock<INNS_FileInfo>();
            info_mock.Setup(x => x.Exists()).Returns(false);


            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();

            var fileInfo_mock = new Mock<INNS_FileInfo>();
            fileInfo_mock.Setup(x => x.LastWriteTime()).Returns(new DateTime(1, 1, 1, 1, 4, 1, 1));
            fileInfo_mock.Setup(x => x.Exists()).Returns(false);
            FileService_Mock.SetupSequence(x => x.GetFileInfo(It.IsAny<string>())).Returns(fileInfo_mock.Object);
            FileService_Mock.Setup(x => x.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Verifiable();
            FileService_Mock.Setup(x => x.GetDirectories(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new string[] { "Orig", "Orig2" });
            FileService_Mock.Setup(x => x.GetFiles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new string[] { "Test1", "Test2" });
            FileService_Mock.Setup(x => x.CreateDirectory(It.IsAny<string>())).Verifiable();
            FileService_Mock.Setup(x => x.GetFileInfo(It.IsAny<string>())).Returns(info_mock.Object);

            var ProcessManager = new ProcessesManager();
            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager);
            var copies = fileManager.CopyDirectory("Orig", "Dest", false);

            FileService_Mock.Verify(x => x.Copy("Test1", "Test1", false), Times.Once);
            FileService_Mock.Verify(x => x.CreateDirectory("Dest"), Times.Once);
            Assert.AreEqual(2, copies);
        }


        [TestMethod]
        public void FileManager_RobocopyWithFalseOverride_PassesCorrectArguments()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            var ProcessManager = new Mock<IProcessesManager>();

            ProcessManager.Setup(x => x.SetProcessArguments(It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            ProcessManager.Setup(x => x.StartProcess(It.IsAny<string>())).Verifiable();

            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager.Object);
            fileManager.Robocopy("TestLoc1", "TestLoc2", false, true);

            ProcessManager.Verify(x => x.SetProcessArguments("Robocopy", "TestLoc1 TestLoc2 /MIR /XO /mt"), Times.Once);
            ProcessManager.Verify(x => x.StartProcess("Robocopy"), Times.Once);
        }
        [TestMethod]
        public void FileManager_RobocopyWithTrueOverride_PassesCorrectArguments()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            var ProcessManager = new Mock<IProcessesManager>();

            ProcessManager.Setup(x => x.SetProcessArguments(It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            ProcessManager.Setup(x => x.StartProcess(It.IsAny<string>())).Verifiable();

            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager.Object);
            fileManager.Robocopy("TestLoc1", "TestLoc2", true);

            ProcessManager.Verify(x => x.SetProcessArguments("Robocopy", "TestLoc1 TestLoc2 /e /IS /IT /mt"), Times.Once);
            ProcessManager.Verify(x => x.StartProcess("Robocopy"), Times.Once);
        }
        [TestMethod]
        public void CreateDirectory_DirectoryCreated()
        {
            Mock<INNS_FileService> FileService_Mock = new Mock<INNS_FileService>();
            var ProcessManager = new Mock<IProcessesManager>();

            FileService_Mock.Setup(x => x.CreateDirectory(It.IsAny<string>()));

            FileManager fileManager = new FileManager(FileService_Mock.Object, ProcessManager.Object);

            fileManager.CreateDirectory("TestDirectory");

            FileService_Mock.Verify(x => x.CreateDirectory("TestDirectory"), Times.Once);
        }
    }
}
