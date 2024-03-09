using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using JohnUtilities.Services.Adapters;
using JohnUtilities.Interfaces;
using JohnUtilities.Classes;

namespace JohnUtilities.UnitTests
{
    [TestClass]
    public class SystemConfigurationAccessorUnitTests
    {
        [TestMethod]
        public void GetCodeDroplevelFromProfileVars()
        {
            SystemConfigurationAccessor SystemAccessor;

            var FileManager_Mock = new Mock<IFileManager>();
            FileManager_Mock.Setup(x => x.GetTextFromFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<char>(), It.IsAny<char>(), It.IsAny<int>())).Returns("Test");
            var ConfigManager = new ConfigurationManager(new Mock<IConfigLoading>().Object, FileManager_Mock.Object);
            SystemAccessor = new SystemConfigurationAccessor(ConfigManager, FileManager_Mock.Object);
            ConfigManager.AddConfigurationSetting("ProfileVars", "TestValue");

            SystemAccessor.GetCodeDropLevelFromProfileVars();

            FileManager_Mock.Verify(x => x.GetTextFromFile(It.IsAny<string>(), "CUSTOM_CODE_ROOT=", '/', ';', 3));
        }
    }
}
