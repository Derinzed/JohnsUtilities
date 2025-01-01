using JohnUtilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;
using JohnUtilities.Classes;
using System.Threading.Tasks;

namespace JohnUtilities.Interfaces
{
    public interface IConfigLoading
    { 

        Tuple<string, string> GetAttributeAndNode(string _node, string attribute);
        Tuple<string, string> GetAttributeAndNode(XmlNode _node, string attribute);
        XmlNode FindFirstNodeWithName(string path, string name);
        XmlElement GetRoot();
        void LoadDocument(string doc);
        Task<XmlDocument> LoadWebXMLAsDocument(string url);
        void LoadTree(XmlNode _node, List<ConfigurationElement> container, string UUID="NULL");
        void ApplyXMLChanges(List<ConfigurationManagerRegisteredChange> changes);
        void SaveXML(string path = "");
        XmlDocument LoadXMLAsDocument(string xml);
    }
}