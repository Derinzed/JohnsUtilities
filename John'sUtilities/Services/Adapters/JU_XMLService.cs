using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using JohnUtilities.Services.Interfaces;

namespace JohnUtilities.Services.Adapters
{
    public class NNS_XMLService : INNS_XMLService
    {
        public NNS_XMLService()
        {
            Document = new XmlDocument();
        }
        public XmlElement GetRoot()
        {
            return Document.DocumentElement;
        }
        public string SetActiveNode(string _node)
        {
            node = Document.DocumentElement.SelectSingleNode(_node);
            return node.Name;
        }
        public string SetActiveNode(XmlNode _node)
        {
            node = _node;
            return node.Name;
        }

        public void LoadDocument(string doc)
        {
            Document.Load(doc);
        }
        public XmlDocument LoadXMLAsDocument(string doc)
        {
            Document.LoadXml(doc);
            return Document;
        }
        public string LoadAttribute(string attribute)
        {
            string result = "";
            if (node.Attributes != null && node.Attributes[attribute] != null) {
                result = node.Attributes[attribute].InnerText;
            }
            else {
                result = "";
            }

            return result;
        }

        public string GetDocPath()
        {
            if(Document == null || Document.BaseURI == "")
            {
                return "";
            }
            return Document.BaseURI.Remove(0, 8);
        }

        public void SaveDocument(string path)
        {
            Document.Save(path);
        }

        public XmlNode node { get; private set; }
        public XmlDocument Document { get; set; }
    }
}
