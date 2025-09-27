using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnUtilities.Model;
using System.Xml;
using Moq;
using System.IO;
using JohnUtilities.Classes;
using Attribute = JohnUtilities.Classes.Attribute;
using JohnUtilities.Services.Interfaces;
using JohnUtilities.Interfaces;
using JohnUtilities.Services.Adapters;

namespace JohnUtilities.UnitTests
{

    [TestClass]
    public class ConfigurationManagerUnitTests
    {

        ConfigurationManager ConfigManager;
        Mock<IJU_FileService> FileService_Mock;
        List<ConfigurationElement> container = new List<ConfigurationElement>();

        [TestInitialize]
        public void SetupTests()
        {
            FileService_Mock = new Mock<IJU_FileService>();
        }
        [TestMethod]
        public void GetConfigurationSettings_UsingDefaultValueAttribute_ReturnsCorrectValue()
        {
            container.Add(ConfigurationElement.CreateConfigurationElement("TestNode", "TestValue", "NULL", "NULL", "NULL"));

            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            var FileManager_Mock = new Mock<IFileManager>();
            ConfigManager = new ConfigurationManager(Loader_Mock.Object, FileManager_Mock.Object, container);

            Assert.AreEqual("TestValue", ConfigManager.GetConfigurationSetting("TestNode"));
        }
        [TestMethod]
        public void GetConfigurationSettings_UsingDefinedValueAttribute_ReturnsCorrectValue()
        {
            List<Attribute> attributes = new List<Attribute>();
            attributes.Add(new Classes.Attribute("TestAttribute", "TestValue"));

            container.Add(ConfigurationElement.CreateConfigurationElement("TestNode", attributes, "NULL", "NULL", "NULL"));

            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            var FileManager_Mock = new Mock<IFileManager>();
            ConfigManager = new ConfigurationManager(Loader_Mock.Object, FileManager_Mock.Object, container);


            Assert.AreEqual("TestValue", ConfigManager.GetConfigurationSetting("TestNode", "TestAttribute"));
        }

        [TestMethod]
        public void ParseConfig_ValidateProperArgumentPassing()
        {
            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            Loader_Mock.Setup(x => x.LoadDocument(It.IsAny<string>())).Verifiable();
            Loader_Mock.Setup(x => x.LoadTree(It.IsAny<XmlElement>(), It.IsAny<List<ConfigurationElement>>(), "NULL")).Verifiable();

            var ProcessManager = new ProcessesManager();
            ConfigManager = new ConfigurationManager(Loader_Mock.Object, new FileManager(FileService_Mock.Object, ProcessManager));

            ConfigManager.ParseConfig("TestConfig", container);

            Loader_Mock.Verify(x => x.LoadDocument("TestConfig"), Times.Once);
            Loader_Mock.Verify(x => x.LoadTree(It.IsAny<XmlElement>(), container, "NULL"), Times.Once);

        }

    
        [TestMethod]
        public void LoadLauncherconfigurationFile_GivenFileExists_ParseConfig()
        {
            Mock<IJU_FileService> FileService_Mock = new Mock<IJU_FileService>();
            FileService_Mock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);

            var ProcessManager = new ProcessesManager();
            Mock<ConfigurationManager> ConfigManager_Mock = new Mock<ConfigurationManager>(new Mock<IConfigLoading>().Object, new FileManager(FileService_Mock.Object, ProcessManager), new List<ConfigurationElement>());
            ConfigManager_Mock.CallBase = true;

            ConfigManager_Mock.Setup(x => x.ParseConfig(It.IsAny<String>(), It.IsAny<List<ConfigurationElement>>()));

            ConfigManager_Mock.Object.LoadLauncherConfigurationFile();

            ConfigManager_Mock.Verify(x => x.ParseConfig(It.IsAny<String>(), It.IsAny<List<ConfigurationElement>>()), Times.Once);

        }
        [TestMethod]
        public void LoadLauncherconfigurationFile_GivenFileMissing_ReturnError()
        {
            Mock<IJU_FileService> FileService_Mock = new Mock<IJU_FileService>();
            FileService_Mock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

            var mockWriter = new Mock<IJU_StreamWriter>();
            mockWriter.Setup(x => x.WriteLine(It.IsAny<String>())).Verifiable();

            Logging.GetLogger().Init(mockWriter.Object, null);

            var ProcessManager = new ProcessesManager();
            Mock<ConfigurationManager> ConfigManager_Mock = new Mock<ConfigurationManager>(new Mock<IConfigLoading>().Object, new FileManager(FileService_Mock.Object, ProcessManager), new List<ConfigurationElement>());
            ConfigManager_Mock.CallBase = true;

            ConfigManager_Mock.Object.LoadLauncherConfigurationFile();


            mockWriter.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once);

        }

        [TestMethod]
        public void LoadEnvironmentConfigurationFile_GivenFileMissing_ReturnError()
        {
            Mock<IJU_FileService> FileService_Mock = new Mock<IJU_FileService>();
            FileService_Mock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

            var mockWriter = new Mock<IJU_StreamWriter>();
            mockWriter.Setup(x => x.WriteLine(It.IsAny<String>())).Verifiable();

            Logging.GetLogger().Init(mockWriter.Object, null);

            var ProcessManager = new ProcessesManager();
            Mock<ConfigurationManager> ConfigManager_Mock = new Mock<ConfigurationManager>(new Mock<IConfigLoading>().Object, new FileManager(FileService_Mock.Object, ProcessManager), new List<ConfigurationElement>());
            ConfigManager_Mock.CallBase = true;


            ConfigManager_Mock.Object.LoadEnvironmentConfigurationFile();


            mockWriter.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once);

        }

        [TestMethod]
        public void LoadEnvironmentConfigurationFile_GivenFileExists_ParseConfig()
        {
            Mock<IJU_FileService> FileService_Mock = new Mock<IJU_FileService>();
            FileService_Mock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);

            var Dict = new List<ConfigurationElement>();
            Dict.Add(ConfigurationElement.CreateConfigurationElement("EnvironmentalConfigurationFile", "Test", "NULL", "NULL", "NULL"));

            var ProcessManager = new ProcessesManager();
            Mock<ConfigurationManager> ConfigManager_Mock = new Mock<ConfigurationManager>(new Mock<IConfigLoading>().Object, new FileManager(FileService_Mock.Object, ProcessManager), Dict, null);
            ConfigManager_Mock.CallBase = true;
            ConfigManager_Mock.Object.AddConfigurationSetting("EnvironmentConfigurationFile", "TestVal");

            var Loader = new ConfigLoading(new JU_XMLService());
            Loader.LoadXMLAsDocument("<JohnUtilitiesConfigFile><EnvironmentConfigurationFile value = \"TestEntry\" /></JohnUtilitiesConfigFile > ");
            Loader.LoadTree(Loader.GetRoot(), new List<ConfigurationElement>());

            ConfigManager_Mock.Setup(x => x.ParseConfig(It.IsAny<String>(), It.IsAny<List<ConfigurationElement>>())).Verifiable();

            ConfigManager_Mock.Object.LoadEnvironmentConfigurationFile();

            ConfigManager_Mock.Verify(x => x.ParseConfig(It.IsAny<String>(), It.IsAny<List<ConfigurationElement>>()), Times.Once);

        }

        [TestMethod]
        public void LodConfigurationFile_GivenFileExists_ParseConfig()
        {
            Mock<IJU_FileService> FileService_Mock = new Mock<IJU_FileService>();
            FileService_Mock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);

            var ProcessManager = new ProcessesManager();
            Mock<ConfigurationManager> ConfigManager_Mock = new Mock<ConfigurationManager>(new Mock<IConfigLoading>().Object, new FileManager(FileService_Mock.Object, ProcessManager), null, null);
            ConfigManager_Mock.CallBase = true;
            ConfigManager_Mock.Object.AddConfigurationSetting("EnvironmentConfigurationFile", "TestVal");

            ConfigManager_Mock.Setup(x => x.ParseConfig(It.IsAny<String>(), It.IsAny<List<ConfigurationElement>>())).Verifiable();

            ConfigManager_Mock.Object.LoadConfigFile("TestFile");

            ConfigManager_Mock.Verify(x => x.ParseConfig(It.IsAny<String>(), It.IsAny<List<ConfigurationElement>>()), Times.Once);
        }
        [TestMethod]
        public void ResolveReferences_GivenConfigLoaded_ReferencesResolved()
        {
            Mock<IJU_FileService> FileService_Mock = new Mock<IJU_FileService>();
            FileService_Mock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);

            var Dict = new List<ConfigurationElement>();
            Dict.Add(ConfigurationElement.CreateConfigurationElement("TestRef", "Test", "NULL", "Definitions", "NULL", "NULL"));
            Dict.Add(ConfigurationElement.CreateConfigurationElement("TestElement", "$TestRef - Reference Value", "Definitions", "NULL", "NULL"));

            var ProcessManager = new ProcessesManager();
            Mock<ConfigurationManager> ConfigManager_Mock = new Mock<ConfigurationManager>(new Mock<IConfigLoading>().Object, new FileManager(FileService_Mock.Object, ProcessManager), Dict, null);
            ConfigManager_Mock.CallBase = true;
            ConfigManager_Mock.Object.AddConfigurationSetting("EnvironmentConfigurationFile", "TestVal");

            ConfigManager_Mock.Setup(x => x.ParseConfig(It.IsAny<String>(), It.IsAny<List<ConfigurationElement>>())).Verifiable();

            ConfigManager_Mock.Object.LoadConfigFile("TestFile");

            ConfigManager_Mock.Verify(x => x.ParseConfig(It.IsAny<String>(), It.IsAny<List<ConfigurationElement>>()), Times.Once);

            Assert.AreEqual("Test - Reference Value", ConfigManager_Mock.Object.GetConfigurationSetting("TestElement"));
        }

        [TestMethod]
        public void ApplyChanges_GivenChanges_ChangesApplied()
        {
            ConfigurationChange change = new ConfigurationChange("TestNode", "value", "NewTestResult", ConfigurationCategories.EnvironmentConfiguration );

            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            Loader_Mock.Setup(x => x.ApplyXMLChanges(It.IsAny<List<ConfigurationManagerRegisteredChange>>())).Verifiable();

            var ProcessManager = new ProcessesManager();
            ConfigManager = new ConfigurationManager(Loader_Mock.Object, new FileManager(FileService_Mock.Object, ProcessManager));

            ConfigManager.AddConfigurationSetting("TestNode", "TestValue");
            ConfigManager.AddConfigurationSetting("EnvironmentConfigurationFile", "DummyFile");
            ConfigManager.StageChange(change);
            ConfigManager.ApplyChanges();

            Assert.AreEqual(change.NewAttributeValue, ConfigManager.GetConfigurationSetting("TestNode"));

        }
        [TestMethod]
        public void ApplyChanges_GivenChanges_ChangesSavedToXMLFile()
        {

            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            Loader_Mock.Setup(x => x.ApplyXMLChanges(It.IsAny<List<ConfigurationManagerRegisteredChange>>())).Verifiable();

            var ProcessManager = new ProcessesManager();
            ConfigManager = new ConfigurationManager(Loader_Mock.Object, new FileManager(FileService_Mock.Object, ProcessManager));

            ConfigManager.SaveChangesToFile();

            Loader_Mock.Verify(x => x.ApplyXMLChanges(It.IsAny<List<ConfigurationManagerRegisteredChange>>()), Times.Once);

        }
        [TestMethod]
        public void AddConfigurationSettings_GivenNewKeyAndVal_AddedToList()
        {
            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            var FileManager_Mock = new Mock<IFileManager>();
            ConfigManager = new ConfigurationManager(Loader_Mock.Object, FileManager_Mock.Object);

            ConfigManager.AddConfigurationSetting("TestKey", "TestValue");

            Assert.AreEqual("TestValue", ConfigManager.GetConfigurationSetting("TestKey"));
        }
        [TestMethod]
        public void AddConfigurationSettings_GivenNewKeyValAndAttribute_AddedToList()
        {
            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            var FileManager_Mock = new Mock<IFileManager>();
            ConfigManager = new ConfigurationManager(Loader_Mock.Object, FileManager_Mock.Object);

            ConfigManager.AddConfigurationSetting("TestKey", "TestValue", "TestAttribute");

            Assert.AreEqual("TestValue", ConfigManager.GetConfigurationSetting("TestKey", attribute:"TestAttribute"));
        }
        [TestMethod]
        public void AddConfigurationSettings_GivenNewKeyMultipleValAndMultipleAttribute_AddedToList()
        {
            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            var FileManager_Mock = new Mock<IFileManager>();
            ConfigManager = new ConfigurationManager(Loader_Mock.Object, FileManager_Mock.Object);

            ConfigManager.AddConfigurationSetting("TestKey", new[]{"TestValue", "TestValue2"}, new[] { "TestAttribute", "TestAttribute2" });

            Assert.AreEqual("TestValue", ConfigManager.GetConfigurationSetting("TestKey", attribute: "TestAttribute"));
            Assert.AreEqual("TestValue2", ConfigManager.GetConfigurationSetting("TestKey", attribute: "TestAttribute2"));
        }
        [TestMethod]
        public void AddConfigurationSettings_GivenSameKey_NoOverwrite()
        {
            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            var FileManager_Mock = new Mock<IFileManager>();
            ConfigManager = new ConfigurationManager(Loader_Mock.Object, FileManager_Mock.Object);

            ConfigManager.AddConfigurationSetting("TestKey", "theOriginalTestKeyValue");
            ConfigManager.AddConfigurationSetting("TestKey", "TestValue");

            Assert.AreEqual("theOriginalTestKeyValue", ConfigManager.GetConfigurationSetting("TestKey"));
        }
        [TestMethod]
        public void GetItemsWithParent_GivenParent_ReturnsCorrectList()
        {
            List<ConfigurationElement> Container = new List<ConfigurationElement>();
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey", "TestValue", "NULL", "TestParent.Parent",  "NULL"));
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey2", "TestValue2", "NULL", "TestParent.Parent", "NULL"));
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey3", "TestValue3", "NULL",  "TestParent.Parent", "NULL"));

            List<ConfigurationElement> results = new List<ConfigurationElement>();

            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            var FileManager_Mock = new Mock<IFileManager>();

            ConfigManager = new ConfigurationManager(Loader_Mock.Object, FileManager_Mock.Object, Container);

            results = ConfigManager.GetItemsWithParent("TestParent.Parent");

            Assert.AreEqual(3, results.Count);
            Assert.AreEqual("TestKey", results[0].Key);
            Assert.AreEqual("TestValue", results[0].Attributes[0].Value);

            Assert.AreEqual("TestKey2", results[1].Key);
            Assert.AreEqual("TestValue2", results[1].Attributes[0].Value);

            Assert.AreEqual("TestKey3", results[2].Key);
            Assert.AreEqual("TestValue3", results[2].Attributes[0].Value);
        }
        [TestMethod]
        public void GetItemsWithDirectParent_GivenParent_ReturnsCorrectList()
        {
            List<ConfigurationElement> Container = new List<ConfigurationElement>();
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey", "TestValue", "NULL", "TestParent.Parent", "NULL"));
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey2", "TestValue2", "NULL", "TestParent.Parent", "NULL"));
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey3", "TestValue3", "NULL", "TestParent.Parent", "NULL"));

            List<ConfigurationElement> results = new List<ConfigurationElement>();

            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            var FileManager_Mock = new Mock<IFileManager>();

            ConfigManager = new ConfigurationManager(Loader_Mock.Object, FileManager_Mock.Object, Container);

            results = ConfigManager.GetItemsWithPartialParent("Parent");

            Assert.AreEqual(3, results.Count);
            Assert.AreEqual("TestKey", results[0].Key);
            Assert.AreEqual("TestValue", results[0].Attributes[0].Value);

            Assert.AreEqual("TestKey2", results[1].Key);
            Assert.AreEqual("TestValue2", results[1].Attributes[0].Value);

            Assert.AreEqual("TestKey3", results[2].Key);
            Assert.AreEqual("TestValue3", results[2].Attributes[0].Value);
        }
        [TestMethod]
        public void GetItemsWithDirectParentAndAttributeValue_GivenParent_ReturnsCorrectList()
        {
            var attributeList1 = new List<Attribute>(){ new Attribute( "TestAttribute", "TestValue")};
            var attributeList2 = new List<Attribute>() { new Attribute("TestAttribute", "TestValue2") };
            var attributeList3 = new List<Attribute> { new Attribute("TestAttribute", "TestValue3") };


            List<ConfigurationElement> Container = new List<ConfigurationElement>();
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey", attributeList1, "NULL", "TestParent.Parent", "NULL"));
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey2", attributeList2, "NULL", "TestParent.Parent", "NULL"));
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey3", attributeList3, "NULL", "TestParent.Parent", "NULL"));

            List<ConfigurationElement> results = new List<ConfigurationElement>();

            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            var FileManager_Mock = new Mock<IFileManager>();

            ConfigManager = new ConfigurationManager(Loader_Mock.Object, FileManager_Mock.Object, Container);

            results = ConfigManager.GetItemsWithPartialParentAndAttributeValue("Parent", "TestAttribute", "TestValue3");

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("TestKey3", results[0].Key);
            Assert.AreEqual("TestValue3", results[0].GetAttribute("TestAttribute"));

        }
        [TestMethod]
        public void GetItemsWithAttributeValue_GivenAttributeValue_ReturnsCorrectList()
        {
            var attributeList1 = new List<Attribute>() { new Attribute("TestAttribute", "TestValue") };
            var attributeList2 = new List<Attribute>() { new Attribute("TestAttribute", "TestValue2") };
            var attributeList3 = new List<Attribute> { new Attribute("TestAttribute", "TestValue2") };


            List<ConfigurationElement> Container = new List<ConfigurationElement>();
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey", attributeList1, "TestParent.Parent", "NULL", "NULL"));
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey2", attributeList2, "TestParent.Parent", "NULL", "NULL"));
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey3", attributeList3, "TestParent.Parent", "NULL", "NULL"));

            List<ConfigurationElement> results = new List<ConfigurationElement>();

            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            var FileManager_Mock = new Mock<IFileManager>();

            ConfigManager = new ConfigurationManager(Loader_Mock.Object, FileManager_Mock.Object, Container);

            results = ConfigManager.GetItemsWithAttributeValue("TestAttribute", "TestValue2");

            Assert.AreEqual(2, results.Count);

        }
        [TestMethod]
        public void GetItemsWhereAttributeContainsValue_GivenAttributeValue_ReturnsCorrectList()
        {
            var attributeList1 = new List<Attribute>() { new Attribute("TestAttribute", "TestValue") };
            var attributeList2 = new List<Attribute>() { new Attribute("TestAttribute", "TestValue2, TestValue3") };
            var attributeList3 = new List<Attribute> { new Attribute("TestAttribute", "TestValue2, TestValue4") };


            List<ConfigurationElement> Container = new List<ConfigurationElement>();
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey", attributeList1, "TestParent.Parent", "NULL", "NULL"));
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey2", attributeList2, "TestParent.Parent", "NULL", "NULL"));
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey3", attributeList3, "TestParent.Parent", "NULL", "NULL"));

            List<ConfigurationElement> results = new List<ConfigurationElement>();

            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            var FileManager_Mock = new Mock<IFileManager>();

            ConfigManager = new ConfigurationManager(Loader_Mock.Object, FileManager_Mock.Object, Container);

            results = ConfigManager.GetItemsWhereAttributeContainsValue("TestAttribute", "TestValue2");

            Assert.AreEqual(2, results.Count);

        }
        [TestMethod]
        public void GetChildrenElements_GivenChildrenElements_ReturnsCorrectList()
        {
            var attributeList1 = new List<Attribute>() { new Attribute("TestAttribute", "TestValue") };
            var attributeList2 = new List<Attribute>() { new Attribute("TestAttribute", "TestValue2, TestValue3") };
            var attributeList3 = new List<Attribute> { new Attribute("TestAttribute", "TestValue2, TestValue4") };


            List<ConfigurationElement> Container = new List<ConfigurationElement>();
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey", attributeList1, "NULL", "TestParent.Parent", "NULL", "NULL"));
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey2", attributeList2, "NULL", "TestParent.Parent", "NULL", "NULL"));
            Container.Add(ConfigurationElement.CreateConfigurationElement("TestKey3", attributeList3, "NULL", "TestParent.Parent", "NULL", "NULL"));

            List<ConfigurationElement> results = new List<ConfigurationElement>();

            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            var FileManager_Mock = new Mock<IFileManager>();

            ConfigManager = new ConfigurationManager(Loader_Mock.Object, FileManager_Mock.Object, Container);

            results = ConfigManager.GetChildrenElements("TestParent.Parent");

            Assert.AreEqual(3, results.Count);

        }

        [TestMethod]
        public void RegisterOperation_GivenOperationRegistered_CanGetOperation()
        {

            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            var FileManager_Mock = new Mock<IFileManager>();

            ConfigManager = new ConfigurationManager(Loader_Mock.Object, FileManager_Mock.Object);

            ConfigManager.RegisterOperations("TestOperation", (ConfigurationElement) => { return; });

            var Op = ConfigManager.GetOperation("TestOperation");

            Assert.IsNotNull(Op);

        }

        [TestMethod]
        public void GetConfigurationElement_ElementExists_ReturnsConfigurationElement()
        {
            var attributeList1 = new List<Attribute>() { new Attribute("TestAttribute", "TestValue") };
            var attributeList2 = new List<Attribute>() { new Attribute("TestAttribute", "TestValue2, TestValue3") };
            var attributeList3 = new List<Attribute> { new Attribute("TestAttribute", "TestValue2, TestValue4") };


            List<ConfigurationElement> Container = new List<ConfigurationElement>();
            var elm1 = ConfigurationElement.CreateConfigurationElement("TestKey", attributeList1, "TestParent.Parent", "NULL", "NULL");
            var elm2 = ConfigurationElement.CreateConfigurationElement("TestKey2", attributeList2, "TestParent.Parent", "NULL", "NULL");
            var elm3 = ConfigurationElement.CreateConfigurationElement("TestKey3", attributeList3, "TestParent.Parent", "NULL", "NULL");
            Container.Add(elm1);
            Container.Add(elm2);
            Container.Add(elm3);


            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            var FileManager_Mock = new Mock<IFileManager>();

            ConfigManager = new ConfigurationManager(Loader_Mock.Object, FileManager_Mock.Object, Container);

            var result = ConfigManager.GetConfigurationElement(elm3);

            Assert.AreEqual("TestKey3", result.Key);


        }

        [TestMethod]
        public void GetElementOperations_GivenElementExists_ReturnsOperations()
        {
            var attributeList1 = new List<Attribute>() { new Attribute("TestAttribute", "TestValue") };
            var attributeList2 = new List<Attribute>() { new Attribute("TestAttribute", "TestValue2, TestValue3") };
            var attributeList3 = new List<Attribute> { new Attribute("TestAttribute", "TestValue2, TestValue4") };


            List<ConfigurationElement> Container = new List<ConfigurationElement>();
            var elm1 = ConfigurationElement.CreateConfigurationElement("TestKey", attributeList1, "NULL", "TestParent.Parent", "NULL", "NULL");
            var elm2 = ConfigurationElement.CreateConfigurationElement("TestKey2", attributeList2, "NULL", "TestParent.Parent", "NULL", "NULL");
            var elm3 = ConfigurationElement.CreateConfigurationElement("TestKey3", attributeList3, "NULL", "TestParent.Parent.TestOperation.AnotherTestOperation", "NULL", "NULL");
            Container.Add(elm1);
            Container.Add(elm2);
            Container.Add(elm3);


            Mock<IConfigLoading> Loader_Mock = new Mock<IConfigLoading>();
            var FileManager_Mock = new Mock<IFileManager>();

            ConfigManager = new ConfigurationManager(Loader_Mock.Object, FileManager_Mock.Object, Container);

            ConfigManager.RegisterOperations("Parent", (ConfigurationElement) => { return; });
            ConfigManager.RegisterOperations("AnotherTestOperation", (ConfigurationElement) => { return; });

            var result = ConfigManager.GetElementOperations(elm3);

            Assert.AreEqual(2, result.Count);


        }
    }
}
