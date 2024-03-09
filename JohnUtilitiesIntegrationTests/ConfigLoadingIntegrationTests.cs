using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnUtilities.Model;
using JohnUtilities.Model.Classes;
using System.Xml;
using System.IO;
using System.Linq;
using JohnUtilities.Services.Adapters;
using JohnUtilities.Classes;

namespace JohnUtilities.IntegrationTests
{
    [TestClass]
    public class ConfigLoadingIntegrationTests
    {

        [TestMethod]
        public void CanReturnAttribute()
        {
            ConfigLoading loader = new ConfigLoading(new NNS_XMLService());
            loader.LoadDocument("..\\..\\ConfigTest\\Testconfig.xml");

            Tuple<string, string> node;
            node = loader.GetAttributeAndNode("/TestConfig/TestAttribute", "value");

            Assert.AreEqual("TestAttribute", node.Item1);
            Assert.AreEqual("testResult", node.Item2);
        }

        [TestMethod]
        public void GivenXMLFile_CanLoadFileIntoDictionary()
        {
            ConfigLoading loader = new ConfigLoading(new NNS_XMLService());
            loader.LoadDocument("..\\..\\ConfigTest\\Testconfig.xml");

            List<ConfigurationElement> container = new List<ConfigurationElement>();

            var ProcessManager = new ProcessesManager();
            var ConfigManager = new ConfigurationManager(loader, new FileManager(new NNS_FileService(), ProcessManager));

            loader.LoadTree(loader.GetRoot(), container);

            Assert.AreEqual("TestConfig", container.First(x => x.Key == "TestConfig").Key);
            Assert.AreEqual("testResult", container.First(x => x.Key == "TestAttribute").Attributes.First(y => y.Name == "value").Value);
            Assert.AreEqual("testResult2", container.First(x => x.Key == "TestAttribute2").Attributes.First(y => y.Name == "value").Value);
            Assert.AreEqual("testResult2.1", container.First(x => x.Key == "TestAttribute2.1").Attributes.First(y => y.Name == "value").Value);
            Assert.AreEqual("testResult2.2", container.First(x => x.Key == "TestAttribute2.2").Attributes.First(y => y.Name == "value").Value);
            Assert.AreEqual("testResult3", container.First(x => x.Key == "TestAttribute3").Attributes.First(y => y.Name == "value").Value);
        }

        [TestMethod]
        public void GivenXMLFile_CanFindFirstNodeWithName()
        {
            ConfigLoading loader = new ConfigLoading(new NNS_XMLService());
            loader.LoadDocument("..\\..\\ConfigTest\\Testconfig.xml");

            Dictionary<string, string> container = new Dictionary<string, string>();

            var ProcessManager = new ProcessesManager();
            var ConfigManager = new ConfigurationManager(loader, new FileManager(new NNS_FileService(), ProcessManager));


            XmlNode result = loader.FindFirstNodeWithName("//*", "TestAttribute");

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ApplyXMLChanges_SaveFileWithChanges()
        {
            List<ConfigurationElement> container = new List<ConfigurationElement>();

            File.Copy("..\\..\\ConfigTest\\TestConfig.xml", "..\\..\\ConfigTest\\TestConfig2.xml", true);

            ConfigLoading loader = new ConfigLoading(new NNS_XMLService());
            loader.LoadDocument("..\\..\\ConfigTest\\TestConfig2.xml");
            loader.LoadTree(loader.GetRoot(), container);


            List<ConfigurationManagerRegisteredChange> changes = new List<ConfigurationManagerRegisteredChange>();
            changes.Add(new ConfigurationManagerRegisteredChange("..\\..\\ConfigTest\\TestConfig2.xml", new ConfigurationChange("TestAttribute", "value", "NewTestResult", ConfigurationCategories.LauncherConfiguration)));


            loader.ApplyXMLChanges(changes);
        }

        [TestMethod]
        public void LoadXMLTree_UsingConfigurationElements_LoadsElementsPropertly()
        {
            File.Copy("..\\..\\ConfigTest\\TestConfig.xml", "..\\..\\..\\ConfigTest\\TestConfig3.xml", true);

            List<ConfigurationElement> ConfigurationInformation = new List<ConfigurationElement>();

            ConfigLoading loader = new ConfigLoading(new NNS_XMLService());
            loader.LoadDocument("..\\..\\..\\ConfigTest\\TestConfig3.xml");
            loader.LoadTree(loader.GetRoot(), ConfigurationInformation);

            Assert.AreEqual(6, ConfigurationInformation.Count);
        }
    }
}
