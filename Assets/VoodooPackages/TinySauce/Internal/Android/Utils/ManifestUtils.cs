using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Voodoo.Sauce.Internal.Utils
{
    public static class ManifestUtils
    {
        public static bool MergeManifestsFiles(string manifestSourcePath, string manifestDestPath)
        {
            XmlDocument resultDocument = Add(LoadFromFile(manifestSourcePath), LoadFromFile(manifestDestPath));
            if (resultDocument == null) {
                return false;
            }

            SaveDocumentInFile(manifestDestPath, resultDocument);
            return true;
        }

        public static string SetApplicationAttributes(string manifestContent, Dictionary<string,string> keysValues)
        {
            XmlDocument xmlDocument = LoadFromString(manifestContent);
            XmlNode manifestNode = FindChildNode(xmlDocument, "manifest");
            XmlNode applicationNode = FindChildNode(manifestNode, "application");
            foreach (KeyValuePair<string, string> pair in keysValues) {
                XmlAttribute xmlAttribute = FindAttribute(applicationNode.Attributes, pair.Key);
                if (xmlAttribute == null) {
                    string[] keys = pair.Key.Split(':');
                    xmlAttribute = xmlDocument.CreateAttribute(keys[0], keys[1], null);
                    applicationNode.Attributes?.Append(xmlAttribute);
                }
                xmlAttribute.Value = pair.Value;
            } 
            return xmlDocument.OuterXml;
        }

        public static bool ReplaceKeys(string manifestPath, Dictionary<string, string> keysValues)
        {
            if (File.Exists(manifestPath)) {
                string manifestContent = File.ReadAllText(manifestPath);
                foreach (KeyValuePair<string, string> pair in keysValues) {
                    manifestContent = manifestContent.Replace(pair.Key, pair.Value);
                }

                File.WriteAllText(manifestPath, manifestContent);
                return true;
            }

            return false;
        }

        private static XmlDocument Add(XmlDocument sourceDocument, XmlDocument destDocument)
        {
            // check if manifest xml is ok
            if (sourceDocument?.DocumentElement == null || destDocument?.DocumentElement == null) {
                return null;
            }

            // add all manifest source declarations on the destination file
            foreach (XmlNode nodeSource in sourceDocument.DocumentElement.ChildNodes) {
                if (nodeSource.HasChildNodes) {
                    XmlNode destNode = FindChildNode(destDocument.DocumentElement, nodeSource);
                    foreach (XmlNode node in nodeSource.ChildNodes) {
                        AddChildNode(destDocument, destNode, node);
                    }
                } else {
                    AddChildNode(destDocument, destDocument, nodeSource);
                }
            }

            return destDocument;
        }

        private static XmlDocument LoadFromFile(string manifestPath)
        {
            XmlDocument document = null;
            if (File.Exists(manifestPath)) {
                document = new XmlDocument();
                document.Load(manifestPath);
            }

            return document;
        }
        
        private static XmlDocument LoadFromString(string manifestContent)
        {
            var document = new XmlDocument();
            document.LoadXml(manifestContent);
            return document;
        }

        private static void AddChildNode(XmlDocument document, XmlNode parent, XmlNode node)
        {
            if (parent != null && node != null && !FindElementWithAndroidName(parent, node)) {
                parent.AppendChild(document.ImportNode(node, true));
            }
        }

        private static XmlNode FindChildNode(XmlNode parent, XmlNode child) => FindChildNode(parent, child.Name);

        private static XmlAttribute FindAttribute(IEnumerable parent, string name)
        {
            foreach (XmlAttribute attribute in parent) {
                if (attribute.Name == name) {
                    return attribute;
                }
            }
            return null;
        }

        private static XmlNode FindChildNode(XmlNode parent, string childName)
            {
                XmlNode node = parent.FirstChild;
                while (node != null) {
                    if (node.Name.Equals(childName)) {
                        return node;
                    }

                    node = node.NextSibling;
                }

                return null;
            }

            private static bool FindElementWithAndroidName(XmlNode parent, XmlNode child)
            {
                string namespaceOfPrefix = parent.GetNamespaceOfPrefix("android");
                string childName = GetAndroidElementName(child, namespaceOfPrefix);
                if (childName != null) {
                    XmlNode node = parent.FirstChild;
                    while (node != null) {
                        if (GetAndroidElementName(node, namespaceOfPrefix) == childName) {
                            return true;
                        }

                        node = node.NextSibling;
                    }
                }

                return false;
            }

            private static string GetAndroidElementName(XmlNode node, string namespaceOfPrefix) =>
                node is XmlElement element ? element.GetAttribute("name", namespaceOfPrefix) : null;

            private static void SaveDocumentInFile(string manifestPath, XmlDocument document)
            {
                var set = new XmlWriterSettings {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace
                };

                using (var xmlWriter = XmlWriter.Create(manifestPath, set)) {
                    document.Save(xmlWriter);
                }
            }
        }
    }