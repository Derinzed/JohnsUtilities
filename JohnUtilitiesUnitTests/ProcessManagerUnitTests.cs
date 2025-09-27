using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnUtilities.Services.Interfaces;
using JohnUtilities.Classes;
using JohnUtilities.Interfaces;
using JohnUtilities.Services.Adapters;
using Moq;
using System.Diagnostics;
using System.Linq;

namespace JohnUtilities.UnitTests
{
    [TestClass]
    public class ProcessManagerUnitTests
    {
        [TestMethod]
        public void CreateProcess_GivenProcessNotInList_ProcessCreated()
        {
            var processesList = new Mock<IList<IProcessOpener>>();
            var mockEnumerator = new Mock<IEnumerator<ProcessOpener>>();

            var StreamWriter_Mock = new Mock<IJU_StreamWriter>();
            StreamWriter_Mock.Setup(x => x.WriteLine(It.IsAny<string>())).Verifiable();
            var logger = Logging.GetLogger().Init(StreamWriter_Mock.Object, null);

            var holdingList = new List<ProcessOpener>();

            processesList.Setup(x => x.Add(Capture.In(holdingList)));
            processesList.Setup(x => x.GetEnumerator()).Returns(mockEnumerator.Object);
            mockEnumerator.Setup(x => x.MoveNext()).Returns(false);
            mockEnumerator.Setup(x => x.Dispose());

            var ProcMan = new ProcessesManager(processesList.Object);

            ProcMan.CreateProcess("TestProc", "TestLoc", "TestArgs");

            Assert.AreEqual("TestProc", holdingList[0].ProcessName);
            Assert.AreEqual("TestLoc", holdingList[0].OriginalPath);
            Assert.AreEqual("TestArgs", holdingList[0].Process.Process.StartInfo.Arguments);

            StreamWriter_Mock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Never);
        }
        [TestMethod]
        public void CreateProcess_GivenProcessAlreadyExists_WarningLogged()
        {
            var processesList = new List<IProcessOpener>();

            var StreamWriter_Mock = new Mock<IJU_StreamWriter>();
            StreamWriter_Mock.Setup(x => x.WriteLine(It.IsAny<string>())).Verifiable();
            var logger = Logging.GetLogger().Init(StreamWriter_Mock.Object, null);

            processesList.Add(new ProcessOpener("TestProc"));

            var ProcMan = new ProcessesManager(processesList);

            ProcMan.CreateProcess("TestProc", "TestLoc", "TestArgs");

            StreamWriter_Mock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void StartProcess_GivenValidProcess_ProcessStarted()
        {
            var processesList = new List<IProcessOpener>();

            var process = new Mock<IProcessOpener>();
            process.SetupAllProperties();
            process.Setup(x => x.Start()).Verifiable();
            process.Object.ProcessName = "TestProc";

            processesList.Add(process.Object);

            var ProcMan = new ProcessesManager(processesList);

            ProcMan.StartProcess("TestProc");

            process.Verify(x => x.Start(), Times.Once);

        }

        [TestMethod]
        public void StartProcess_GivenInvalidProcess_ProcessStartNotCalled()
        {
            var processesList = new List<IProcessOpener>();

            var StreamWriter_Mock = new Mock<IJU_StreamWriter>();
            StreamWriter_Mock.Setup(x => x.WriteLine(It.IsAny<string>())).Verifiable();
            var logger = Logging.GetLogger().Init(StreamWriter_Mock.Object, null);

            var ProcMan = new ProcessesManager(processesList);

            ProcMan.StartProcess("TestProc");

            StreamWriter_Mock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once);

        }
        [TestMethod]
        public void GetProcessOverridePath_GivenValidProcess_ReturnsOverridePathString()
        {
            var processesList = new List<IProcessOpener>();

            var process = new ProcessOpener("TestProc");
            process.PathOverride = "TestOverridePath";

            processesList.Add(process);

            var ProcMan = new ProcessesManager(processesList);

            var result = ProcMan.GetProcessOverridePath("TestProc");

            Assert.AreEqual("TestOverridePath", result);

        }
        [TestMethod]
        public void GetProcessOverridePath_GivenNoProcesses_ReturnsBlankString()
        {
            var processesList = new List<IProcessOpener>();

            var ProcMan = new ProcessesManager(processesList);

            var result = ProcMan.GetProcessOverridePath("TestProc");

            Assert.AreEqual("", result);

        }
        [TestMethod]
        public void GetProcessOverridePath_GivenInvalidProcesses_ReturnsBlankString()
        {
            var processesList = new List<IProcessOpener>();

            var process = new ProcessOpener("TestProc2");
            process.PathOverride = "TestOverridePath";

            processesList.Add(process);

            var ProcMan = new ProcessesManager(processesList);

            var result = ProcMan.GetProcessOverridePath("TestProc");

            Assert.AreEqual("", result);

        }
        [TestMethod]
        public void SetProcessOverridePath_GivenValidProcess_OverridePathSet()
        {
            var processesList = new List<IProcessOpener>();

            var process = new ProcessOpener("TestProc");

            processesList.Add(process);

            var ProcMan = new ProcessesManager(processesList);

            ProcMan.SetProcessOverridePath("TestProc", "TestOverridePath");

            Assert.AreEqual("TestOverridePath", ProcMan.GetProcessOverridePath("TestProc"));
        }

        [TestMethod]
        public void IsProcessRunning_GivenValidProcess_ReturnsRunningValueTrue()
        {
            var processesList = new List<IProcessOpener>();

            var JUProc = new Mock<IJU_Process>();
            JUProc.SetupAllProperties();
            JUProc.Setup(x => x.Start()).Returns(true);
            var realProcess = new Process();
            realProcess.StartInfo.FileName = "TestLoc";
            JUProc.Object.Process = realProcess;
            var process = new ProcessOpener("TestProc", process: JUProc.Object);

            processesList.Add(process);

            var ProcMan = new ProcessesManager(processesList);

            ProcMan.StartProcess("TestProc");

            var result = ProcMan.IsProcessRunning("TestProc");

            Assert.AreEqual(true, result);

        }
        [TestMethod]
        public void IsProcessRunning_GivenInvalidProcess_ReturnsFalse()
        {
            var processesList = new List<IProcessOpener>();

            var JUProc = new Mock<IJU_Process>();
            JUProc.SetupAllProperties();
            JUProc.Setup(x => x.Start()).Returns(true);
            var realProcess = new Process();
            realProcess.StartInfo.FileName = "TestLoc";
            JUProc.Object.Process = realProcess;
            var process = new ProcessOpener("TestProc2", process: JUProc.Object);

            processesList.Add(process);

            var ProcMan = new ProcessesManager(processesList);

            ProcMan.StartProcess("TestProc");

            var result = ProcMan.IsProcessRunning("TestProc");

            Assert.AreEqual(false, result);

        }
        [TestMethod]
        public void IsProcessRunning_GivenNoProcesses_ReturnsFalse()
        {
            var processesList = new List<IProcessOpener>();

            var ProcMan = new ProcessesManager(processesList);

            ProcMan.StartProcess("TestProc");

            var result = ProcMan.IsProcessRunning("TestProc");

            Assert.AreEqual(false, result);

        }
        [TestMethod]
        public void SetProcessArguments_GivenValidProcess_ArgumentsSet()
        {
            string args = "TestArgs";
            var processesList = new List<IProcessOpener>();

            var ProcessOpener = new Mock<IProcessOpener>();
            ProcessOpener.SetupAllProperties();
            ProcessOpener.Object.ProcessName = "TestProc";
            ProcessOpener.Setup(x => x.SetArguments(It.IsAny<string>())).Verifiable();

            processesList.Add(ProcessOpener.Object);

            var ProcMan = new ProcessesManager(processesList);

            ProcMan.SetProcessArguments("TestProc", args);

            ProcessOpener.Verify(x => x.SetArguments(args), Times.Once);
        }
        [TestMethod]
        public void SetProcessArguments_GivenNoProcesses_ArgumentsNotSet()
        {
            string args = "TestArgs";
            var processesList = new Mock<IList<IProcessOpener>>();
            var mockEnumerator = new Mock<IEnumerator<ProcessOpener>>();

            processesList.Setup(x => x.GetEnumerator()).Returns(mockEnumerator.Object);

            var ProcMan = new ProcessesManager(processesList.Object);

            ProcMan.SetProcessArguments("TestProc", args);

            processesList.Verify(x => x.GetEnumerator(), Times.Never);
        }
        [TestMethod]
        public void SetProcessArguments_GivenInvalidProcess_ArgumentsNotSet()
        {
            string args = "TestArgs";
            var processesList = new List<IProcessOpener>();

            var ProcessOpener = new Mock<IProcessOpener>();
            ProcessOpener.SetupAllProperties();
            ProcessOpener.Object.ProcessName = "FakeTestProc";
            ProcessOpener.Setup(x => x.SetArguments(It.IsAny<string>())).Verifiable();

            processesList.Add(ProcessOpener.Object);

            var ProcMan = new ProcessesManager(processesList);

            ProcMan.SetProcessArguments("TestProc", args);

            ProcessOpener.Verify(x => x.SetArguments(It.IsAny<string>()), Times.Never);
        }
        [TestMethod]
        public void GetProcessStandardOutput_GivenValidProcess_OutputObtained()
        {
            var processesList = new List<IProcessOpener>();

            var ProcessOpener = new Mock<IProcessOpener>();
            ProcessOpener.SetupAllProperties();
            ProcessOpener.Object.ProcessName = "TestProc";
            ProcessOpener.Setup(x => x.GetStandardOutput()).Returns("Fake Output");

            processesList.Add(ProcessOpener.Object);

            var ProcMan = new ProcessesManager(processesList);

            var result = ProcMan.GetProcessStandardOutput("TestProc");

            ProcessOpener.Verify(x => x.GetStandardOutput(), Times.Once);
            Assert.AreEqual("Fake Output", result);
        }
        [TestMethod]
        public void GetProcessStandardOutput_GivenInvalidProcess_ReturnEmptyString()
        {
            var processesList = new List<IProcessOpener>();

            var ProcessOpener = new Mock<IProcessOpener>();
            ProcessOpener.SetupAllProperties();
            ProcessOpener.Object.ProcessName = "TestProc";
            ProcessOpener.Setup(x => x.GetStandardOutput()).Returns("Fake Output");

            processesList.Add(ProcessOpener.Object);

            var ProcMan = new ProcessesManager(processesList);

            var result = ProcMan.GetProcessStandardOutput("TestProc2");

            ProcessOpener.Verify(x => x.GetStandardOutput(), Times.Never);
            Assert.AreEqual("", result);
        }
        [TestMethod]
        public void GetProcessStandardOutput_GivenNoProcesses_ReturnEmptyString()
        {
            var processesList = new Mock<IList<IProcessOpener>>();
            var mockEnumerator = new Mock<IEnumerator<ProcessOpener>>();

            processesList.Setup(x => x.GetEnumerator()).Returns(mockEnumerator.Object);

            var ProcMan = new ProcessesManager(processesList.Object);

            var result = ProcMan.GetProcessStandardOutput("TestProc");

            Assert.AreEqual("", result);
        }
        [TestMethod]
        public void GetProcessErrorOutput_GivenValidProcess_OutputObtained()
        {
            var processesList = new List<IProcessOpener>();

            var ProcessOpener = new Mock<IProcessOpener>();
            ProcessOpener.SetupAllProperties();
            ProcessOpener.Object.ProcessName = "TestProc";
            ProcessOpener.Setup(x => x.GetErrorOutput()).Returns("Fake Output");

            processesList.Add(ProcessOpener.Object);

            var ProcMan = new ProcessesManager(processesList);

            var result = ProcMan.GetProcessErrorOutput("TestProc");

            ProcessOpener.Verify(x => x.GetErrorOutput(), Times.Once);
            Assert.AreEqual("Fake Output", result);
        }
        [TestMethod]
        public void GetProcessErrorOutput_GivenInvalidProcess_ReturnEmptyString()
        {
            var processesList = new List<IProcessOpener>();

            var ProcessOpener = new Mock<IProcessOpener>();
            ProcessOpener.SetupAllProperties();
            ProcessOpener.Object.ProcessName = "TestProc";
            ProcessOpener.Setup(x => x.GetErrorOutput()).Returns("Fake Output");

            processesList.Add(ProcessOpener.Object);

            var ProcMan = new ProcessesManager(processesList);

            var result = ProcMan.GetProcessErrorOutput("TestProc2");

            ProcessOpener.Verify(x => x.GetErrorOutput(), Times.Never);
            Assert.AreEqual("", result);
        }
        [TestMethod]
        public void GetProcessErrorOutput_GivenNoProcesses_ReturnEmptyString()
        {
            var processesList = new Mock<IList<IProcessOpener>>();

            var ProcMan = new ProcessesManager(processesList.Object);

            var result = ProcMan.GetProcessErrorOutput("TestProc");

            Assert.AreEqual("", result);
        }
        [TestMethod]
        public void GetExitCode_GivenCode_ReturnCode()
        {
            var processesList = new List<IProcessOpener>();
            var realProcess = new Process();
            realProcess.StartInfo.FileName = "TestLoc";
            var process = new Mock<IJU_Process>();
            process.SetupProperty(x => x.Process, realProcess);
            process.CallBase = true;
            process.Setup(x => x.GetExitCode()).Returns(37);
            var ProcessOpener = new ProcessOpener("TestProc", process.Object);
            ProcessOpener.WaitForExit = true;
            processesList.Add(ProcessOpener);

            var ProcMan = new ProcessesManager(processesList);

            ProcMan.StartProcess("TestProc");


            Assert.AreEqual(37, ProcMan.GetExitCode("TestProc"));
        }
    }
}
