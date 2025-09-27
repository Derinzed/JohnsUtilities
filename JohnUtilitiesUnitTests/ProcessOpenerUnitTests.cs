using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnUtilities.Services.Interfaces;
using JohnUtilities.Interfaces;
using JohnUtilities.Services.Adapters;
using Moq;
using System.Diagnostics;
using JohnUtilities.Classes;

namespace JohnUtilities.UnitTests
{
    [TestClass]
    public class ProcessOpenerUnitTests
    {
        [TestMethod]
        public void Start_GivenOverridePath_SetProcessPathToOverride()
        {
            var proc = new Process();
            var JUProcess = new Mock<IJU_Process>();
            JUProcess.Setup(x => x.Start()).Returns(true);
            JUProcess.SetupAllProperties();
            JUProcess.Object.Process = proc;

            var ProcessO = new ProcessOpener("TestProcess",  JUProcess.Object);
            ProcessO.PathOverride = "TestPath";

            ProcessO.Start();

            Assert.AreEqual("TestPath", JUProcess.Object.Process.StartInfo.FileName);
        }
        [TestMethod]
        public void Start_GivenNoOverridePath_SetProcessPathToOriginal()
        {
            var proc = new Process();
            var JUProcess = new Mock<IJU_Process>();
            JUProcess.Setup(x => x.Start()).Returns(true);
            JUProcess.SetupAllProperties();
            JUProcess.Object.Process = proc;

            var ProcessO = new ProcessOpener("TestProcess", JUProcess.Object);
            ProcessO.PathOverride = "TestPath";

            ProcessO.Start();

            Assert.AreEqual("TestPath", JUProcess.Object.Process.StartInfo.FileName);
        }
        [TestMethod]
        public void Start_GivenNoPath_StartNotCalled()
        {
            var proc = new Process();
            var JUProcess = new Mock<IJU_Process>();
            JUProcess.Setup(x => x.Start()).Returns(true);
            JUProcess.SetupAllProperties();
            JUProcess.Object.Process = proc;

            var ProcessO = new ProcessOpener("TestProcess", JUProcess.Object);

            ProcessO.Start();

            JUProcess.Verify(x => x.Start(), Times.Never);
        }
        [TestMethod]
        public void Start_GivenPath_StartCalled()
        {
            var proc = new Process();
            proc.StartInfo.FileName = "TestLoc";
            var JUProcess = new Mock<IJU_Process>();
            JUProcess.Setup(x => x.Start()).Returns(true);
            JUProcess.SetupAllProperties();
            JUProcess.Object.Process = proc;

            var ProcessO = new ProcessOpener("TestProcess", JUProcess.Object);

            ProcessO.Start();

            JUProcess.Verify(x => x.Start(), Times.Once);
        }
        [TestMethod]
        public void Start_ProcessThrowError_ErrorCaught()
        {
            var StreamWriter_Mock = new Mock<IJU_StreamWriter>();
            StreamWriter_Mock.Setup(x => x.WriteLine(It.IsAny<string>())).Verifiable();
            var logger = Logging.GetLogger().Init(StreamWriter_Mock.Object, null);


            var proc = new Process();
            proc.StartInfo.FileName = "TestLoc";
            var JUProcess = new Mock<IJU_Process>();
            JUProcess.Setup(x => x.Start()).Throws(new Exception("This is a test exception"));
            JUProcess.SetupAllProperties();
            JUProcess.Object.Process = proc;

            var ProcessO = new ProcessOpener("TestProcess", JUProcess.Object);

            ProcessO.Start();

            StreamWriter_Mock.Verify(x => x.WriteLine(DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " - An error has occured within the ActiveWorkspaceClientManager."));
        }

    }
}
