using System;
using System.IO;
using Moq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Linq;
using JohnUtilities.Services.Interfaces;
using JohnUtilities.Classes;
using JohnUtilities.Services.Adapters;
using JohnUtilities.Interfaces;

namespace JohnUtilities.UnitTests
{
    [TestClass]
    public class ConfigLoadingTests {

        Mock<IJU_XMLService> XMLService_Mock;
        ConfigLoading Loader;
        JU_XMLService XMLService;
        

        [TestInitialize]
        public void SetupTests()
        {
            XMLService_Mock = new Mock<IJU_XMLService>();
            XMLService_Mock.SetupAllProperties();
            XMLService_Mock.CallBase = true;
            XMLService = new JU_XMLService();
        }
    
        [TestMethod]
        public void LoadDocument_ArgumentsArePassedCorrectly()
        {
            Loader = new ConfigLoading(XMLService_Mock.Object);
            Loader.LoadDocument("TestDocument");

            XMLService_Mock.Verify(x => x.LoadDocument("TestDocument"), Times.Once);
        }

        [TestMethod]
        public void GetAttributeAndNodeUsingString_ArgumentsArePassedCorrectly()
        {
            Loader = new ConfigLoading(XMLService_Mock.Object);
            Loader.GetAttributeAndNode("TestNode", "TestAttribute");

            XMLService_Mock.Verify(x => x.SetActiveNode("TestNode"), Times.Once);
            XMLService_Mock.Verify(x => x.LoadAttribute("TestAttribute"), Times.Once);
        }

        [TestMethod]
        public void GetAttributeAndNodeUsingXmlNode_ArgumentsArePassedCorrectly()
        {
            XMLService_Mock.Setup(x => x.SetActiveNode(It.IsAny<XmlNode>())).Returns("TestNode");

            
            //Use an XmlElement in place of a XmlNode since it is a more specific implimentation of XmlNode
            //And XmlNode cannot be constructed.
            var testNode_Mock = new Mock<XmlElement>("TestElement", "TestElement", "TestElement", new XmlDocument());

            Loader = new ConfigLoading(XMLService_Mock.Object);
            Loader.GetAttributeAndNode(testNode_Mock.Object, "TestAttribute");

            XMLService_Mock.Verify(x => x.SetActiveNode(testNode_Mock.Object), Times.Once);
            XMLService_Mock.Verify(x => x.LoadAttribute("TestAttribute"), Times.Once);
        }

        [TestMethod]
        public void Loadtree_GivenEmptyXML_ReturnContainerWithOnlyRootAndDocument()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<TestConfig></TestConfig>");

            List<ConfigurationElement> container = new List<ConfigurationElement>();

            XMLService_Mock.Setup(x => x.SetActiveNode(It.IsAny<XmlNode>())).Returns("TestConfig");
            XMLService_Mock.Setup(x => x.LoadAttribute(It.IsAny<string>())).Returns("TestConfig");

            Loader = new ConfigLoading(XMLService_Mock.Object);
            Loader.LoadTree(doc, container);

            Assert.AreEqual(2, container.Count);

        }

        [TestMethod]
        public void Loadtree_GivenSingleHeightXML_ReturnCorrectContainer()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<TestConfig><TestAttribute value=\"testResult\"/><TestAttribute2 value = \"testResult2\" /></TestConfig>");

            List<ConfigurationElement> container = new List<ConfigurationElement>();

            XMLService_Mock.SetupSequence(x => x.SetActiveNode(It.IsAny<XmlNode>())).Returns("TestConfig").Returns("TestAttribute").Returns("TestAttribute2");
            XMLService_Mock.SetupSequence(x => x.LoadAttribute(It.IsAny<string>())).Returns("TestConfig").Returns("testResult").Returns("testResult2");

            Loader = new ConfigLoading(XMLService_Mock.Object);
            Loader.LoadTree(doc.DocumentElement, container);

            Assert.AreEqual(3, container.Count);

        }

        [TestMethod]
        public void Loadtree_GivenMultilevelXML_ReturnCorrectContainer()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<TestConfig><TestAttribute value = \"testResult\" /><TestAttribute2 value = \"testResult2\"><TestAttribute2.1 value = \"testResult2.1\" /> <TestAttribute2.2 value = \"testResult2.2\" /> </TestAttribute2><TestAttribute3 value = \"testResult3\" /> </TestConfig>");

            List<ConfigurationElement> container = new List<ConfigurationElement>();

            Loader = new ConfigLoading(XMLService_Mock.Object);
            Loader.LoadTree(doc.DocumentElement, container);

            Assert.AreEqual(6, container.Count);

            var result = container.First(x => x.Key == "TestAttribute").Attributes.First(y => y.Value == "testResult").Value;
            var result2 = container.First(x => x.Key == "TestAttribute2").Attributes.First(y => y.Value == "testResult2").Value;
            var result21 = container.First(x => x.Key == "TestAttribute2.1").Attributes.First(y => y.Value == "testResult2.1").Value;
            var result22 = container.First(x => x.Key == "TestAttribute2.2").Attributes.First(y => y.Value == "testResult2.2").Value;
            var result3 = container.First(x => x.Key == "TestAttribute3").Attributes.First(y => y.Value == "testResult3").Value;

            Assert.AreEqual("testResult", result);
            Assert.AreEqual("testResult2", result2);
            Assert.AreEqual("testResult2.1", result21);
            Assert.AreEqual("testResult2.2", result22);
            Assert.AreEqual("testResult3", result3);

            List<ConfigurationElement> children = container.Where(x => x.ParentName == ".TestConfig.TestAttribute2").ToList();

            Assert.AreEqual("TestAttribute2.1", children[0].Key);
            Assert.AreEqual("TestAttribute2.2", children[1].Key);

        }
        [TestMethod]
        public void Loadtree_GivenEmptyNode_ReturnCorrectContainerWithWarning()
        {
            var StreamWriter_Mock = new Mock<IJU_StreamWriter>();
            StreamWriter_Mock.Setup(x => x.WriteLine(It.IsAny<string>())).Verifiable();
            var logger = Logging.GetLogger().Init(StreamWriter_Mock.Object, null);


            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<TestConfig><!--<ThisIsAComment/>--></TestConfig>");

            List<ConfigurationElement> container = new List<ConfigurationElement>();

            Loader = new ConfigLoading(XMLService_Mock.Object);
            Loader.LoadTree(doc.DocumentElement, container);

            StreamWriter_Mock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(1, container.Count);
        }

        [TestMethod]
        public void GetRoot_GivenDocument_ReturnsRootElement()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<TestConfig><TestAttribute value=\"testResult\"/><TestAttribute value = \"testResult2\" /></TestConfig>");

            XMLService_Mock.Setup(x => x.GetRoot()).Returns(doc.DocumentElement);
            XMLService_Mock.Object.Document = doc;
            Loader = new ConfigLoading(XMLService_Mock.Object);

            XmlElement root = Loader.GetRoot();

            Assert.AreEqual(doc.DocumentElement, root);
            XMLService_Mock.Verify(x=>x.GetRoot(),Times.Once);

        }

        [TestMethod]
        public void GetRoot_NotGivenDocument_ReturnsNull()
        {
            XmlDocument doc = new XmlDocument();
            XMLService_Mock.Object.Document = null;
            Loader = new ConfigLoading(XMLService_Mock.Object);

            XmlElement root = Loader.GetRoot();

            Assert.IsNull(root);

        }
        [TestMethod]
        public void LoadXMLAsDocument_GivenXML_ReturnsValidDocument()
        {
            string xml = "<TestConfig><TestAttribute value=\"testResult\"/><TestAttribute value = \"testResult2\" /></TestConfig>";
            XmlDocument doc = new XmlDocument();
            XmlDocument realDoc = XMLService.LoadXMLAsDocument(xml);

            XMLService_Mock.Setup(x => x.LoadXMLAsDocument(It.IsAny<string>())).Returns(realDoc);

            Loader = new ConfigLoading(XMLService_Mock.Object);
            doc = Loader.LoadXMLAsDocument(xml);

            Assert.IsNotNull(doc);

        }
        [TestMethod]
        public void ApplyXMLChanges_GivenNULLDocumentElement_NoChanges()
        {
            Dictionary<string, string> container = new Dictionary<string, string>();

            Mock<IConfigLoading> ConfigLoading_Mock = new Mock<IConfigLoading>();
            ConfigLoading_Mock.CallBase = true;


            XMLService_Mock.SetupAllProperties();
            XMLService_Mock.CallBase = true;

            List<ConfigurationManagerRegisteredChange> changes = new List<ConfigurationManagerRegisteredChange>();
            changes.Add(new ConfigurationManagerRegisteredChange("TestLoc", new ConfigurationChange("TestAttribute", "value", "NewTestResult", ConfigurationCategories.LauncherConfiguration)));

            Loader = new ConfigLoading(XMLService_Mock.Object);

            Loader.ApplyXMLChanges(changes);

            ConfigLoading_Mock.Verify(x => x.FindFirstNodeWithName(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        }
        [TestMethod]
        public void ApplyXMLChanges_GivenNULLDocument_NoChanges()
        {
            Dictionary<string, string> container = new Dictionary<string, string>();

            Mock<IConfigLoading> ConfigLoading_Mock = new Mock<IConfigLoading>();
            ConfigLoading_Mock.CallBase = true;

            XMLService_Mock.SetupAllProperties();
            XMLService_Mock.CallBase = true;

            XMLService_Mock.Object.Document = null;
            Loader = new ConfigLoading(XMLService_Mock.Object);

            List<ConfigurationManagerRegisteredChange> changes = new List<ConfigurationManagerRegisteredChange>();
            changes.Add(new ConfigurationManagerRegisteredChange("TestLoc", new ConfigurationChange("TestAttribute", "value", "NewTestResult", ConfigurationCategories.LauncherConfiguration)));

            Loader.ApplyXMLChanges(changes);

            ConfigLoading_Mock.Verify(x => x.FindFirstNodeWithName(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        }

        [TestMethod]
        public void ApplyXMLChanges_GivenValidChanges_ApplyChanges()
        {
            List<ConfigurationElement> container = new List<ConfigurationElement>();

            string xml = "<TestConfig><TestAttribute value=\"testResult\"/><TestAttribute2 value = \"testResult2\" /></TestConfig>";
            XmlDocument doc = new XmlDocument();


            doc = XMLService.LoadXMLAsDocument(xml);

            XMLService_Mock = new Mock<IJU_XMLService>();
            XMLService_Mock.SetupAllProperties();
            XMLService_Mock.CallBase = true;

            XMLService_Mock.Object.Document = doc;

            Loader = new ConfigLoading(XMLService_Mock.Object);
            Loader.LoadTree(Loader.GetRoot(), container);

            string node = "TestAttribute";
            string attributeName = "value";
            string attributeValue = "NewTestResult";

            List<ConfigurationManagerRegisteredChange> changes = new List<ConfigurationManagerRegisteredChange>();
            changes.Add(new ConfigurationManagerRegisteredChange("TestLoc", new ConfigurationChange("TestAttribute", "value", "NewTestResult", ConfigurationCategories.LauncherConfiguration)));

            Loader.ApplyXMLChanges(changes);

            string result = doc.DocumentElement.SelectSingleNode(node).Attributes[attributeName].InnerText;

            Assert.AreEqual(result, "NewTestResult");

        }
        [TestMethod]
        public void ApplyXMLChanges_GivenInvalidNode_NoChanges()
        {
            List<ConfigurationElement> container = new List<ConfigurationElement>();

            string xml = "<TestConfig><TestAttribute value=\"testResult\"/><TestAttribute2 value = \"testResult2\" /></TestConfig>";
            XmlDocument doc = new XmlDocument();

            var mockWriter = new Mock<IJU_StreamWriter>();
            mockWriter.Setup(x => x.WriteLine(It.IsAny<String>())).Verifiable();

            Logging.GetLogger().Init(mockWriter.Object, null);

            doc = XMLService.LoadXMLAsDocument(xml);

            XMLService_Mock = new Mock<IJU_XMLService>();
            XMLService_Mock.SetupAllProperties();
            XMLService_Mock.CallBase = true;

            XMLService_Mock.Object.Document = doc;

            Loader = new ConfigLoading(XMLService_Mock.Object);
            Loader.LoadTree(Loader.GetRoot(), container);


            List<ConfigurationManagerRegisteredChange> changes = new List<ConfigurationManagerRegisteredChange>();
            changes.Add(new ConfigurationManagerRegisteredChange("TestLoc", new ConfigurationChange("TestAttribute", "value", "NewTestResult", ConfigurationCategories.LauncherConfiguration)));

            Loader.ApplyXMLChanges(changes);

            mockWriter.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once);

        }
        [TestMethod]
        public void ApplyXMLChanges_GivenInvalidAttribute_NoChanges()
        {
            List<ConfigurationElement> container = new List<ConfigurationElement>();

            string xml = "<TestConfig><TestAttribute value=\"testResult\"/><TestAttribute2 value = \"testResult2\" /></TestConfig>";
            XmlDocument doc = new XmlDocument();

            var mockWriter = new Mock<IJU_StreamWriter>();
            mockWriter.Setup(x => x.WriteLine(It.IsAny<String>())).Verifiable();

            Logging.GetLogger().Init(mockWriter.Object, null);
 
            doc = XMLService.LoadXMLAsDocument(xml);

            XMLService_Mock = new Mock<IJU_XMLService>();
            XMLService_Mock.SetupAllProperties();
            XMLService_Mock.CallBase = true;

            XMLService_Mock.Object.Document = doc;

            Loader = new ConfigLoading(XMLService_Mock.Object);

            Loader.LoadTree(Loader.GetRoot(), container);


            List<ConfigurationManagerRegisteredChange> changes = new List<ConfigurationManagerRegisteredChange>();
            changes.Add(new ConfigurationManagerRegisteredChange("TestLoc", new ConfigurationChange("TestAttribute", "value", "NewTestResult", ConfigurationCategories.LauncherConfiguration)));

            Loader.ApplyXMLChanges(changes);

            mockWriter.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once);

        }
        [TestMethod]
        public void ApplyXMLChanges_GivenDifferentFile_OpenFile()
        {
            List<ConfigurationElement> container = new List<ConfigurationElement>();

            string xml = "<TestConfig><TestAttribute value=\"testResult\"/><TestAttribute2 value = \"testResult2\" /></TestConfig>";
            XmlDocument doc = new XmlDocument();

            var mockWriter = new Mock<IJU_StreamWriter>();
            mockWriter.Setup(x => x.WriteLine(It.IsAny<String>())).Verifiable();

            Logging.GetLogger().Init(mockWriter.Object, null);


            doc = XMLService.LoadXMLAsDocument(xml);

            XMLService_Mock.SetupAllProperties();
            XMLService_Mock.CallBase = true;
            XMLService_Mock.Setup(x => x.LoadDocument(It.IsAny<string>())).Verifiable();


            Loader = new ConfigLoading(XMLService_Mock.Object);

            Loader.LoadTree(Loader.GetRoot(), container);


            List<ConfigurationManagerRegisteredChange> changes = new List<ConfigurationManagerRegisteredChange>();
            changes.Add(new ConfigurationManagerRegisteredChange("TestLoc", new ConfigurationChange("TestAttribute", "value", "NewTestResult", ConfigurationCategories.LauncherConfiguration)));

            Loader.ApplyXMLChanges(changes);

            XMLService_Mock.Verify(x => x.LoadDocument(It.IsAny<string>()), Times.Once);

        }
        [TestMethod]
        public void SaveXML_GivenBaseURI_ExportWithDefinedPath()
        {
            XMLService_Mock.Setup(x => x.SaveDocument(It.IsAny<string>())).Verifiable();
            XMLService_Mock.Setup(x => x.GetDocPath()).Returns("TestBaseURI");

            Loader = new ConfigLoading(XMLService_Mock.Object);
            Loader.SaveXML("TestBaseURI");

            XMLService_Mock.Verify(x => x.SaveDocument("TestBaseURI"), Times.Once);
        }
        [TestMethod]
        public void SaveXML_GivenPath_ExportWithDefinedPath()
        {
            XMLService_Mock.Setup(x => x.SaveDocument(It.IsAny<string>())).Verifiable();
            XMLService_Mock.Setup(x => x.GetDocPath()).Returns("TestBaseURI");

            Loader = new ConfigLoading(XMLService_Mock.Object);
            Loader.SaveXML("ThisIsANewPath");

            XMLService_Mock.Verify(x => x.SaveDocument("ThisIsANewPath"), Times.Once);
        }
        [TestMethod]
        public void SaveXML_GivenNoBaseURIOrPath_ExportWithDefaultPath()
        {
            XMLService_Mock.Setup(x => x.SaveDocument(It.IsAny<string>())).Verifiable();
            XMLService_Mock.Setup(x => x.GetDocPath()).Returns("");

            Loader = new ConfigLoading(XMLService_Mock.Object);
            Loader.SaveXML("RandomString");


            XMLService_Mock.Verify(x => x.SaveDocument("RandomString"), Times.Once);

        }
    }
}