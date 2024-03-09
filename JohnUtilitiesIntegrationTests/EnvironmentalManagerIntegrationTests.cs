using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnUtilities.Classes;
using System.Xml;
using System.IO;

namespace JohnUtilities.IntegrationTests
{
    [TestClass]
    public class EnvironmentalManagerIntegrationTests
    {

        [TestMethod]
        public void GetEnvironmentalVariable_GivenVariable_ReturnsCorrectValue()
        {
            var EnvironmentManager = new EnvironmentalManager();
            EnvironmentManager.SetEnvironmentalVariable("TestVariable", "TestValue");

            Assert.AreEqual("TestValue", EnvironmentManager.GetEnvironmentalVariable("TestVariable"));

            EnvironmentManager.SetEnvironmentalVariable("TestVariable", null);
            Assert.AreEqual(null, EnvironmentManager.GetEnvironmentalVariable("TestVariable"));
        }
        [TestMethod]
        public void DeleteRegistryKey_GivenValidKey_DeletesKey()
        {

            var EnvironmentManager = new EnvironmentalManager();
            EnvironmentManager.SetRegistryKey("HKEY_CURRENT_USER\\System\\TestKey", "TestName", "TestData");

            EnvironmentManager.DeleteRegistryKey("HKEY_CURRENT_USER\\System\\TestKey");

            Assert.AreEqual(null, EnvironmentManager.GetRegistryKey("HKEY_CURRENT_USER\\System\\TestKey", "TestName", "Failure"));
        }
        [TestMethod]
        public void DeleteRegistryName_GivenValidName_DeletesName()
        {

            var EnvironmentManager = new EnvironmentalManager();
            EnvironmentManager.SetRegistryKey("HKEY_CURRENT_USER\\System\\TestKey2", "TestName", "TestData");

            EnvironmentManager.DeleteRegistryName("HKEY_CURRENT_USER\\System\\TestKey2", "TestName");

            Assert.AreEqual("Failure", EnvironmentManager.GetRegistryKey("HKEY_CURRENT_USER\\System\\TestKey2", "TestName", "Failure"));

            EnvironmentManager.DeleteRegistryKey("HKEY_CURRENT_USER\\System\\TestKey2");
        }
    }
}
