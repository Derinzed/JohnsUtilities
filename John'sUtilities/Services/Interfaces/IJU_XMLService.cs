using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace JohnUtilities.Services.Interfaces
{
    public interface IJU_XMLService
    {

         XmlElement GetRoot();
         string SetActiveNode(string _node);
         string SetActiveNode(XmlNode _node);

         void LoadDocument(string doc);
         XmlDocument LoadXMLAsDocument(string doc);
         string LoadAttribute(string attribute);
         void SaveDocument(string path);
         string GetDocPath();
         XmlNode node { get; }
         XmlDocument Document { get; set; }
    }
}
