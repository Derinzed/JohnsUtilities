using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using JohnUtilities.Services.Adapters;
using JohnUtilities.Services.Interfaces;
using JohnUtilities.Interfaces;
using System.Linq;

namespace JohnUtilities.Classes
{
    public class ConfigLoading : IConfigLoading
    {

        public ConfigLoading(IJU_XMLService service)
        {
            XMLService = service;
        }

        public void LoadDocument(string doc)
        {
            XMLService.LoadDocument(doc);
        }
        public XmlElement GetRoot()
        {
            if (XMLService.Document != null)
            {
                return XMLService.GetRoot();
            }
            return null;
        }
        public XmlDocument LoadXMLAsDocument(string xml)
        {
            return XMLService.LoadXMLAsDocument(xml);
        }
        public Tuple<string, string> GetAttributeAndNode(string _node, string attribute)
        {
            return new Tuple<string, string>(XMLService.SetActiveNode(_node), XMLService.LoadAttribute(attribute));
        }
        public Tuple<string, string> GetAttributeAndNode(XmlNode _node, string attribute)
        {
            return new Tuple<string, string>(XMLService.SetActiveNode(_node), XMLService.LoadAttribute(attribute));
        }
        public XmlAttributeCollection GetAttributesFromNode(XmlNode _node)
        {
            XMLService.SetActiveNode(_node);
            return _node.Attributes;
        }

        private string ObtainFullyQualifiedParentName(XmlNode Node)
        {
            string FullyQualifiedParent = "";
            var ParentNames = new Stack<string>();
            var CurrentNode = Node;
            while (CurrentNode.ParentNode.ParentNode != null)
            {
                ParentNames.Push(CurrentNode.ParentNode.Name);
                CurrentNode = CurrentNode.ParentNode;
            }
            ParentNames.Push(Node.BaseURI);
            while (ParentNames.Count != 0)
            {
                FullyQualifiedParent += ParentNames.Pop();
                if(ParentNames.Count != 0)
                {
                    FullyQualifiedParent += ".";
                }
            }
            return FullyQualifiedParent;
        }

        public void LoadTree(XmlNode _node, List<ConfigurationElement> container, string ParentUUID = "NULL")
        {
            XmlNode Node = _node;
            XmlAttributeCollection result;
            while (Node != null)
            {
                var UUID = Guid.NewGuid().ToString();

                result = GetAttributesFromNode(Node);

                if (Node.Name == "#comment")
                {
                    Logging.WriteLogLine("WARNING: A node under parent: " + Node.ParentNode.Name + " has no valid attribute name (It may be a comment).  Ignoring this node.");
                    Node = Node.NextSibling;
                    continue;
                }
                List<Attribute> AttributeList = new List<Attribute>();
                if (result != null)
                {
                    foreach (XmlAttribute attribute in result)
                    {
                        Attribute NewAttribute = new Attribute(attribute.Name, attribute.Value);
                        AttributeList.Add(NewAttribute);
                    }
                }

                if (Node.HasChildNodes)
                {
                    LoadTree(Node.FirstChild, container, UUID);
                }

                string ParentNodeName = "";
                if(Node.ParentNode != null)
                {
                    ParentNodeName = ObtainFullyQualifiedParentName(Node);
                }
                else
                {
                    ParentNodeName = "ROOT";
                }

                var OwningDocument = Node.OwnerDocument == null ? "NULL" : Node.OwnerDocument.BaseURI;
    
                container.Add(ConfigurationElement.CreateConfigurationElement(Node.Name, AttributeList, ParentNodeName, UUID, ParentUUID, file:OwningDocument));

                Node = Node.NextSibling;
            }
        }

        public void DeserializeXML()
        {

        }
        public void SerializeXML()
        {

        }
        public XmlNode FindFirstNodeWithName(string path, string name)
        {
            XmlNodeList allNodes = XMLService.Document.SelectNodes(path);
            foreach(XmlNode node in allNodes)
            {
                if (node.Name == name)
                {
                    return node;
                }
            }
            return null;
        }
        public void ApplyXMLChanges(List<ConfigurationManagerRegisteredChange> changes)
        {

            string nodeName;
            string attributeName;
            string attributeValue;

            foreach (var change in changes)
            {
                if (XMLService.GetDocPath() != change.FileLoc && change.FileLoc != "")
                {
                    XMLService.LoadDocument(change.FileLoc);
                }

                if (XMLService.Document == null)
                {
                    Logging.WriteLogLine("The file could not be opened, and changes could not be saved.");
                    return;
                }

                nodeName = change.BaseChange.NodeName;
                attributeName = change.BaseChange.AttributeName;
                attributeValue = change.BaseChange.NewAttributeValue;

                XmlNode nodeToEdit = FindFirstNodeWithName("//*", nodeName);
                if (nodeToEdit == null)
                {
                    Logging.WriteLogLine("WARNING: The Node you are attempting to save does not exist.  The change will not be applied.");
                    continue;
                }
                if (nodeToEdit.Attributes[attributeName] != null)
                {
                    nodeToEdit.Attributes[attributeName].Value = attributeValue;
                }
                else
                {
                    Logging.WriteLogLine("WARNING: The Attribute you are attempting to save does not exist.  The change will not be applied.");
                    continue;
                }

                SaveXML();
            }
        }

        public void SaveXML(string path = "")
        {
            if(String.IsNullOrEmpty(path) && String.IsNullOrEmpty(XMLService.GetDocPath()))
            {
                Logging.WriteLogLine("WARNING! There is no valid output path.  The file could not be saved.");
                return;
            }
            if (String.IsNullOrEmpty(path))
            {
                XMLService.SaveDocument(XMLService.GetDocPath());
                return;
            }
            XMLService.SaveDocument(path);
        }

        IJU_XMLService XMLService;
    }
}
