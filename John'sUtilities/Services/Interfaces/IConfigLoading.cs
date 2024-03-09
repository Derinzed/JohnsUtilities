using JohnUtilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;
using JohnUtilities.Classes;

namespace JohnUtilities.Interfaces
{
    public interface IConfigLoading
    { 

        Tuple<string, string> GetAttributeAndNode(string _node, string attribute);
        Tuple<string, string> GetAttributeAndNode(XmlNode _node, string attribute);
        XmlNode FindFirstNodeWithName(string path, string name);
        XmlElement GetRoot();
        void LoadDocument(string doc);
        void LoadTree(XmlNode _node, List<ConfigurationElement> container);
        void ApplyXMLChanges(List<ConfigurationManagerRegisteredChange> changes);
        void SaveXML(string path = "");
        XmlDocument LoadXMLAsDocument(string xml);
    }
}