using System;
using System.IO;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnUtilities.Model;
using JohnUtilities.Model.Classes;
using JohnUtilities.Services.Interfaces;
using System.Collections.Generic;
using JohnUtilities.Classes;

namespace JohnUtilities.UnitTests
{
    [TestClass]
    public class LoggingUnitTests
    {

        [TestMethod]
        public void WriteLogLine_WriteLineCalled()
        {

            var mockWriter = new Mock<INNS_StreamWriter>();
            mockWriter.Setup(x => x.WriteLine(It.IsAny<String>()));
            Logging.GetLogger().Init(mockWriter.Object, null);

            Logging.WriteLogLine("This is a test line.");

            mockWriter.Verify(x => x.WriteLine(DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " - " + "This is a test line."), Times.Once);
        }

        [TestMethod]
        public void WriteCustomLogLine_CustomLogsDisabled_PrintWarningMessage()
        {
            var mockWriter = new Mock<INNS_StreamWriter>();
            mockWriter.Setup(x => x.WriteLine(It.IsAny<String>()));
            Logging.GetLogger().Init(mockWriter.Object, null);

            var Log = "CustomLog";
            var Message = "This is a test line";

            Logging.WriteCustomLogLine(Log, Message);

            mockWriter.Verify(x => x.WriteLine(DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " - WARNING. WriteCustomLogLine has attempted to call log: " + Log + " with message: "
                + Message + "; however, custom logs have been disabled."), Times.Once);
        }
        [TestMethod]
        public void WriteCustomLogLine_CustomLogNotRegistered_PrintWarningMessage()
        {
            var mockWriter = new Mock<INNS_StreamWriter>();
            mockWriter.Setup(x => x.WriteLine(It.IsAny<String>()));
            var customLogs = new Dictionary<string, INNS_StreamWriter>();
            customLogs.Add("CustomLog", mockWriter.Object);
            Logging.GetLogger().Init(mockWriter.Object, customLogs);


            var Log = "CustomLog2";
            var Message = "This is a test line";

            Logging.WriteCustomLogLine(Log, Message);

            mockWriter.Verify(x => x.WriteLine(DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + " - WARNING. WriteCustomLogLine has attempted to call log: " + Log + " with message: "
                + Message + "; however, that custom log has not been registered."), Times.Once);
        }

        [TestMethod]
        public void WriteCustomLogLine_GivenValidCustomLog_WriteLineCalled()
        {

            var mockWriter = new Mock<INNS_StreamWriter>();
            mockWriter.Setup(x => x.WriteLine(It.IsAny<String>()));

            var customLogs = new Dictionary<string, INNS_StreamWriter>();
            customLogs.Add("CustomLog", mockWriter.Object);

            Logging.GetLogger().Init(mockWriter.Object, customLogs);

            Logging.WriteCustomLogLine("CustomLog", "This is a test line.");

            mockWriter.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void GetLogger_GivenValidInstance_ReturnsValidObject()
        {
            var logger = Logging.GetLogger().Init(null, null);
            var result = Logging.GetLogger();

            Assert.AreEqual(logger, result);
        }

        [TestMethod]
        public void WriteLogLine_GivenLoggingLevelStandard_DontPrintDebug()
        {
            var mockWriter = new Mock<INNS_StreamWriter>();
            mockWriter.Setup(x => x.WriteLine(It.IsAny<String>()));

            int LogLevel = LoggingLevel.Standard;
            LogLevel |= LoggingLevel.Error;
            LogLevel |= LoggingLevel.Warning;

            Logging.GetLogger().Init(mockWriter.Object, null, LogLevel);

            Logging.WriteLogLine("This is a test Line", LoggingLevel.Debug);

            mockWriter.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void WriteLogLine_GivenLoggingLevelDebug_Print()
        {
            var mockWriter = new Mock<INNS_StreamWriter>();
            mockWriter.Setup(x => x.WriteLine(It.IsAny<String>()));

            int LogLevel = LoggingLevel.Standard;
            LogLevel |= LoggingLevel.Debug;


            Logging.GetLogger().Init(mockWriter.Object, null, LogLevel);

            Logging.WriteLogLine("This is a test Line", LoggingLevel.Debug);

            mockWriter.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once);
        }

    }
}


