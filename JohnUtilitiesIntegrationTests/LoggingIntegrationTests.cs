using System;
using System.IO;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnUtilities.Classes;
using JohnUtilities.Services.Adapters;

namespace JohnUtilities.IntegrationTests
{
    [TestClass]
    public class LoggingIntegrationTests
    {
        [TestMethod]
        public void CreateLog()
        {
            string filename = "LoggingIntegrationTests.CreateLog.txt";

            Logging.GetLogger().Init(new NNS_StreamWriter(filename, true), null);

            Logging.WriteLogLine("This is a test line.");

        }
    }
}
