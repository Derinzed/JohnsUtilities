using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnUtilities.Classes;
using Moq;
using Attribute = JohnUtilities.Classes.Attribute;

namespace JohnUtilities.UnitTests
{
    [TestClass]
    public class ConfigurationElementUnitTests
    {

        [TestMethod]
        public void GetAttribute_GivenValidAttribute_ReturnsValue()
        {
            var AttributeList = new List<Attribute>();
            AttributeList.Add(new Attribute("TestAttribute", "TestValue"));

            var element = ConfigurationElement.CreateConfigurationElement("TestKey", AttributeList, "", "TestParent", "NULL");

            var result = element.GetAttribute("TestAttribute");

            Assert.AreEqual("TestValue", result);
        }
        [TestMethod]
        public void GetAttribute_GivenInvalidAttribute_ReturnsEmptyString()
        {
            var AttributeList = new List<Attribute>();
            AttributeList.Add(new Attribute("TestAttribute2", "TestValue"));

            var element = ConfigurationElement.CreateConfigurationElement("TestKey", AttributeList, "", "TestParent", "NULL");

            var result = element.GetAttribute("TestAttribute");

            Assert.AreEqual("", result);
        }
        [TestMethod]
        public void GetAttribute_GivenEmptyList_ReturnsEmptyString()
        {
            var AttributeList = new Mock<IList<Attribute>>();

            var element = ConfigurationElement.CreateConfigurationElement("TestKey", AttributeList.Object, "", "TestParent", "NULL");

            var result = element.GetAttribute("TestAttribute");

            Assert.AreEqual("", result);
        }
    }
}
