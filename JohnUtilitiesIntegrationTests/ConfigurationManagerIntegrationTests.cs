using System;
using System.Collections.Generic;
using System.Text;
using JohnUtilities.Classes;
using JohnUtilities.Services.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace JohnUtilities.IntegrationTests
{
    [TestClass]
    public class ConfigurationManagerIntegrationTests
    {

        [TestMethod]
        public void GetConfigurationSettings_GetAttributeFromConfigurationElements()
        {
            var XMLService = new JU_XMLService();
            var ProcessManager = new ProcessesManager();
            var FileService = new JU_FileService();
            var FileManager = new FileManager(FileService, ProcessManager);

            FileManager.Copy("..\\..\\ConfigTest\\TestConfig.xml", "..\\..\\..\\ConfigTest\\TestConfig3.xml", true);

            List<ConfigurationElement> ConfigurationInformation = new List<ConfigurationElement>();

            ConfigLoading configLoading = new ConfigLoading(new JU_XMLService());
            configLoading.LoadDocument("..\\..\\ConfigTest\\TestConfig3.xml");
            configLoading.LoadTree(configLoading.GetRoot(), ConfigurationInformation);


            var ConfigManager = new ConfigurationManager(configLoading, FileManager, ConfigurationInformation);


            var result = ConfigManager.GetConfigurationSetting("TestAttribute", "value");
            var result2 = ConfigManager.GetConfigurationSetting("TestAttribute", "SecondaryValue");

            Assert.AreEqual("testResult", result);
            Assert.AreEqual("anotherTestValue", result2);

        }

        [TestMethod]
        public void ApplyChanges_GivenChangeApplyToData()
        {
            var XMLService = new JU_XMLService();
            var ProcessManager = new ProcessesManager();
            var FileService = new JU_FileService();
            var FileManager = new FileManager(FileService, ProcessManager);

            FileManager.Copy("..\\..\\ConfigTest\\TestConfig.xml", "..\\..\\ConfigTest\\TestConfig3.xml", true);

            List<ConfigurationElement> ConfigurationInformation = new List<ConfigurationElement>();

            ConfigLoading configLoading = new ConfigLoading(new JU_XMLService());
            configLoading.LoadDocument("..\\..\\ConfigTest\\TestConfig3.xml");
            configLoading.LoadTree(configLoading.GetRoot(), ConfigurationInformation);


            var ConfigManager = new ConfigurationManager(configLoading, FileManager, ConfigurationInformation);

            var NewChange = new ConfigurationChange("TestAttribute", "value", "NewAttributeValue", ConfigurationCategories.EnvironmentConfiguration);
            var SecondNewChange = new ConfigurationChange("TestAttribute", "SecondaryValue", "NewSecondaryAttributeValue", ConfigurationCategories.EnvironmentConfiguration);

            ConfigManager.StageChange(NewChange);
            ConfigManager.StageChange(SecondNewChange);
            ConfigManager.ApplyChanges();

            var result = ConfigManager.GetConfigurationSetting("TestAttribute", "value");
            var secondResult = ConfigManager.GetConfigurationSetting("TestAttribute", "SecondaryValue");

            Assert.AreEqual("NewAttributeValue", result);
            Assert.AreEqual("NewSecondaryAttributeValue", secondResult);
        }
        [TestMethod]
        public void ApplyChanges_GivenChangeApplyToDataAndSaveToFile()
        {
            var XMLService = new JU_XMLService();
            var ProcessManager = new ProcessesManager();
            var FileService = new JU_FileService();
            var FileManager = new FileManager(FileService, ProcessManager);

            FileManager.Copy("..\\..\\ConfigTest\\TestConfig.xml", "..\\..\\ConfigTest\\TestConfig4.xml", true);

            List<ConfigurationElement> ConfigurationInformation = new List<ConfigurationElement>();
            ConfigurationInformation.Add(new ConfigurationElement("EnvironmentConfigurationFile", "..\\..\\ConfigTest\\TestConfig4.xml", "", "RootNode", "", "NULL", "1"));

            ConfigLoading configLoading = new ConfigLoading(new JU_XMLService());
            configLoading.LoadDocument("..\\..\\ConfigTest\\TestConfig4.xml");
            configLoading.LoadTree(configLoading.GetRoot(), ConfigurationInformation);


            var ConfigManager = new ConfigurationManager(configLoading, FileManager, ConfigurationInformation);

            var NewChange = new ConfigurationChange("TestAttribute", "value", "NewAttributeValue", ConfigurationCategories.EnvironmentConfiguration);
            var SecondNewChange = new ConfigurationChange("TestAttribute", "SecondaryValue", "NewSecondaryAttributeValue", ConfigurationCategories.EnvironmentConfiguration);

            ConfigManager.StageChange(NewChange);
            ConfigManager.StageChange(SecondNewChange);
            ConfigManager.ApplyChanges();
            ConfigManager.SaveChangesToFile();

            ConfigurationInformation = new List<ConfigurationElement>();

            configLoading = new ConfigLoading(new JU_XMLService());
            configLoading.LoadDocument("..\\..\\ConfigTest\\TestConfig4.xml");
            configLoading.LoadTree(configLoading.GetRoot(), ConfigurationInformation);
            ConfigManager = new ConfigurationManager(configLoading, FileManager, ConfigurationInformation);

            var result = ConfigManager.GetConfigurationSetting("TestAttribute", "value");
            var secondResult = ConfigManager.GetConfigurationSetting("TestAttribute", "SecondaryValue");

            Assert.AreEqual("NewAttributeValue", result);
            Assert.AreEqual("NewSecondaryAttributeValue", secondResult);
        }
    }
}