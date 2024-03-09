using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnUtilities.Services.Interfaces;
using Moq;
using JohnUtilities.Classes;
using JohnUtilities.Enums;

namespace JohnUtilities.UnitTests
{
    [TestClass]
    public class EnvironmentalManagerUnitTests
    {

        EnvironmentalManager EnvManager;
        Mock<INNS_EnvironmentalService> EnvService_Mock;

        [TestInitialize]
        public void SetupTests()
        {
            EnvService_Mock = new Mock<INNS_EnvironmentalService>();

            EnvManager = new EnvironmentalManager();
            EnvManager.EnvironmentalService = EnvService_Mock.Object;
        }

        [TestMethod]
        public void GetEnvironmentalVariable_VerifyArgumentPassing()
        {
            EnvService_Mock.Setup(x => x.GetEnvironmentalVariable(It.IsAny<String>())).Returns("TestVariable");
            EnvManager.GetEnvironmentalVariable("TestVariable");

            EnvService_Mock.Verify(x => x.GetEnvironmentalVariable("TestVariable"), Times.Once);
        }
        [TestMethod]
        public void SetEnvironmentalVariableTask_VerifyArgumentPassing()
        {
            EnvService_Mock.Setup(x => x.SetEnvironmentalVariableTask(It.IsAny<String>(), It.IsAny<String>())).Verifiable();
            EnvManager.SetEnvironmentalVariableTask("TestVariable", "TestValue");

            EnvService_Mock.Verify(x => x.SetEnvironmentalVariableTask("TestVariable", "TestValue"), Times.Once);
        }
        [TestMethod]
        public void SetEnvironmentalVariable_VerifyArgumentPassing()
        {
            EnvService_Mock.Setup(x => x.SetEnvironmentalVariableTask(It.IsAny<String>(), It.IsAny<String>())).Verifiable();
            EnvManager.SetEnvironmentalVariable("TestVariable", "TestValue");

            EnvService_Mock.Verify(x => x.SetEnvironmentalVariable("TestVariable", "TestValue"), Times.Once);
        }
        [TestMethod]
        public void GetRegistryKey_VerifyArgumentPassing()
        {
            EnvService_Mock.Setup(x => x.GetRegistryKey(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<object>())).Returns("TestObject");
            EnvManager.GetRegistryKey("TestKeyName", "TestValueName", "TestObject");

            EnvService_Mock.Verify(x => x.GetRegistryKey("TestKeyName", "TestValueName", "TestObject"), Times.Once);
        }
        [TestMethod]
        public void SetRegistryKey_VerifyArgumentPassing()
        {
            EnvService_Mock.Setup(x => x.SetRegistryKey(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<object>())).Verifiable();
            EnvManager.SetRegistryKey("TestKeyName", "TestValueName", "TestObject");

            EnvService_Mock.Verify(x => x.SetRegistryKey("TestKeyName", "TestValueName", "TestObject"), Times.Once);
        }
        [TestMethod]
        public void DeleteKey_VerifyCorrectServicecall()
        {
            EnvService_Mock.Setup(x => x.DeleteRegistryKey(It.IsAny<string>(), It.IsAny<RegistryPath>())).Verifiable();

            EnvManager.DeleteRegistryKey("HKEY_CURRENT_USER\\EnvironmentLocationTest");
            EnvService_Mock.Verify(x => x.DeleteRegistryKey("EnvironmentLocationTest", RegistryPath.HKEY_CURRENT_USER), Times.Once);

            EnvManager.DeleteRegistryKey("HKEY_CLASSES_ROOT\\EnvironmentLocationTest");
            EnvService_Mock.Verify(x => x.DeleteRegistryKey("EnvironmentLocationTest", RegistryPath.HKEY_CLASSES_ROOT), Times.Once);

            EnvManager.DeleteRegistryKey("HKEY_LOCAL_MACHINE\\EnvironmentLocationTest");
            EnvService_Mock.Verify(x => x.DeleteRegistryKey("EnvironmentLocationTest", RegistryPath.HKEY_LOCAL_MACHINE), Times.Once);

            EnvManager.DeleteRegistryKey("HKEY_USERS\\EnvironmentLocationTest");
            EnvService_Mock.Verify(x => x.DeleteRegistryKey("EnvironmentLocationTest", RegistryPath.HKEY_USERS), Times.Once);

            EnvManager.DeleteRegistryKey("HKEY_CURRENT_CONFIG\\EnvironmentLocationTest");
            EnvService_Mock.Verify(x => x.DeleteRegistryKey("EnvironmentLocationTest", RegistryPath.HKEY_CURRENT_CONFIG), Times.Once);

        }
        [TestMethod]
        public void DeleteKeyTree_VerifyCorrectServicecall()
        {
            EnvService_Mock.Setup(x => x.DeleteRegistryKeyTree(It.IsAny<string>(), It.IsAny<RegistryPath>())).Verifiable();

            EnvManager.DeleteRegistryKeyTree("HKEY_CURRENT_USER\\EnvironmentLocationTest");
            EnvService_Mock.Verify(x => x.DeleteRegistryKeyTree("EnvironmentLocationTest", RegistryPath.HKEY_CURRENT_USER), Times.Once);

            EnvManager.DeleteRegistryKeyTree("HKEY_CLASSES_ROOT\\EnvironmentLocationTest");
            EnvService_Mock.Verify(x => x.DeleteRegistryKeyTree("EnvironmentLocationTest", RegistryPath.HKEY_CLASSES_ROOT), Times.Once);

            EnvManager.DeleteRegistryKeyTree("HKEY_LOCAL_MACHINE\\EnvironmentLocationTest");
            EnvService_Mock.Verify(x => x.DeleteRegistryKeyTree("EnvironmentLocationTest", RegistryPath.HKEY_LOCAL_MACHINE), Times.Once);

            EnvManager.DeleteRegistryKeyTree("HKEY_USERS\\EnvironmentLocationTest");
            EnvService_Mock.Verify(x => x.DeleteRegistryKeyTree("EnvironmentLocationTest", RegistryPath.HKEY_USERS), Times.Once);

            EnvManager.DeleteRegistryKeyTree("HKEY_CURRENT_CONFIG\\EnvironmentLocationTest");
            EnvService_Mock.Verify(x => x.DeleteRegistryKeyTree("EnvironmentLocationTest", RegistryPath.HKEY_CURRENT_CONFIG), Times.Once);

        }
        [TestMethod]
        public void DeleteRegistryName_VerifyCorrectServicecall()
        {
            EnvService_Mock.Setup(x => x.DeleteRegistryValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RegistryPath>())).Verifiable();

            EnvManager.DeleteRegistryName("HKEY_CURRENT_USER\\EnvironmentLocationTest", "TestName");
            EnvService_Mock.Verify(x => x.DeleteRegistryValue("EnvironmentLocationTest", "TestName", RegistryPath.HKEY_CURRENT_USER), Times.Once);

            EnvManager.DeleteRegistryName("HKEY_CLASSES_ROOT\\EnvironmentLocationTest", "TestName");
            EnvService_Mock.Verify(x => x.DeleteRegistryValue("EnvironmentLocationTest", "TestName", RegistryPath.HKEY_CLASSES_ROOT), Times.Once);

            EnvManager.DeleteRegistryName("HKEY_LOCAL_MACHINE\\EnvironmentLocationTest", "TestName");
            EnvService_Mock.Verify(x => x.DeleteRegistryValue("EnvironmentLocationTest", "TestName", RegistryPath.HKEY_LOCAL_MACHINE), Times.Once);

            EnvManager.DeleteRegistryName("HKEY_USERS\\EnvironmentLocationTest", "TestName");
            EnvService_Mock.Verify(x => x.DeleteRegistryValue("EnvironmentLocationTest", "TestName", RegistryPath.HKEY_USERS), Times.Once);

            EnvManager.DeleteRegistryName("HKEY_CURRENT_CONFIG\\EnvironmentLocationTest", "TestName");
            EnvService_Mock.Verify(x => x.DeleteRegistryValue("EnvironmentLocationTest", "TestName", RegistryPath.HKEY_CURRENT_CONFIG), Times.Once);

        }
    }
}
