using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace XMLDocHelperNamespace
{
    public class XMLDocHelper
    {
        public XMLDocHelper()
        {
            _xmlApp = new XmlDocument();
        }
        public XMLDocHelper(string docName)
        {
            _xmlApp = new XmlDocument();
            xmlDocPathName = docName;
        }
        public XmlDocument _xmlApp;
        public XmlNode _myNode;
        public static string xmlDocPathName { get; set; }
        public void newXmlDoc()
        {
            _xmlApp = new XmlDocument();
            XmlDeclaration decl = _xmlApp.CreateXmlDeclaration("1.0", "utf-8", null);
            _xmlApp.AppendChild(decl);
            // 加入根元素
            var rootElement = _xmlApp.CreateElement("Root");
            _xmlApp.AppendChild(rootElement);
        }
        public void Save()
        {
            _xmlApp.Save(xmlDocPathName);
        }
        private void OpemXmlDoc(string docPathName)
        {
            if (docPathName.Trim().CompareTo(String.Empty) == 0)
                return;
            if (File.Exists(docPathName))
                _xmlApp.Load(docPathName);
            else
                newXmlDoc();
        }
        /// <summary>
        /// 查询结点的值
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public string GetNodeValue(string parentNode, string nodeName)
        {
            try
            {
                _xmlApp.Load(xmlDocPathName);
                XmlNode rootNode = _xmlApp.FirstChild.NextSibling;
                XmlNodeList nodeList = rootNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name.CompareTo(parentNode) == 0)
                    {
                        foreach (XmlNode o in node.ChildNodes)
                        {
                            if (o.Name.CompareTo(nodeName) == 0)
                                return o.InnerText;
                        }
                    }
                }
                return String.Empty;
            }
            catch
            {
                return String.Empty;
            }
        }
        /// <summary>
        /// 查询结点列表
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public XmlNodeList GetNodeValueList(string parentNode, string nodeName)
        {
            try
            {
                _xmlApp.Load(xmlDocPathName);
                XmlNode rootNode = _xmlApp.FirstChild.NextSibling;
                XmlNodeList nodeList = rootNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name.CompareTo(parentNode) == 0)
                    {
                        return node.ChildNodes;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 设置结点的值
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodeName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string SetNodeValue(string parentNode, string nodeName, string value)
        {
            try
            {
                _xmlApp.Load(xmlDocPathName);
                XmlNode rootNode = _xmlApp.FirstChild.NextSibling;
                XmlNodeList nodeList = rootNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name.CompareTo(parentNode) == 0)
                    {
                        foreach (XmlNode o in node.ChildNodes)
                        {
                            if (o.Name.CompareTo(nodeName) == 0)
                            {
                                o.InnerText = value;
                                _xmlApp.Save(xmlDocPathName);
                                break;
                            }
                        }
                    }
                }
                return String.Empty;
            }
            catch
            {
                return String.Empty;
            }
        }
        /// <summary>
        /// 设置结点属性值
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodeName"></param>
        /// <param name="attriName"></param>
        /// <param name="attriValue"></param>
        /// <returns></returns>
        public bool SetNodeAttribute(string parentNode, string nodeName, string attriName, string attriValue)
        {
            try
            {
                _xmlApp.Load(xmlDocPathName);
                XmlNode rootNode = _xmlApp.FirstChild.NextSibling;
                XmlNodeList nodeList = rootNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name.CompareTo(parentNode) == 0)
                    {
                        foreach (XmlNode o in node.ChildNodes)
                        {
                            if (o.Name.CompareTo(nodeName) == 0)
                            {
                                if (o.Attributes[attriName] != null)
                                {
                                    o.Attributes[attriName].Value = attriValue;
                                }
                                else
                                {
                                    XmlAttribute attri = _xmlApp.CreateAttribute(attriName);
                                    attri.Value = attriValue;
                                    o.Attributes.Append(attri);
                                }
                                _xmlApp.Save(xmlDocPathName);
                                break;
                            }
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 删除结点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodeName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool DeleteNode(string parentNode, string nodeName, string value)
        {
            try
            {
                _xmlApp.Load(xmlDocPathName);
                XmlNode rootNode = _xmlApp.FirstChild.NextSibling;
                XmlNodeList nodeList = rootNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name.CompareTo(parentNode) == 0)
                    {
                        foreach (XmlNode o in node.ChildNodes)
                        {
                            if (o.Name.CompareTo(nodeName) == 0 && o.InnerText.CompareTo(value) == 0)
                            {
                                o.ParentNode.RemoveChild(o);
                                _xmlApp.Save(xmlDocPathName);
                            }
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 插入结点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodeName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool InsertNode(string parentNode, string nodeName, string value)
        {
            try
            {
                _xmlApp.Load(xmlDocPathName);
                XmlNode rootNode = _xmlApp.FirstChild.NextSibling;
                XmlNodeList nodeList = rootNode.ChildNodes;
                if (rootNode.Name.CompareTo(parentNode) == 0)
                {
                    XmlElement newChildNode = _xmlApp.CreateElement(nodeName);
                    newChildNode.InnerText = value;
                    rootNode.AppendChild(newChildNode);
                    _xmlApp.Save(xmlDocPathName);
                    return true;
                }
                else
                {
                    foreach (XmlNode node in nodeList)
                    {
                        if (node.Name.CompareTo(parentNode) == 0)
                        {
                            XmlElement newChildNode = _xmlApp.CreateElement(nodeName);
                            newChildNode.InnerText = value;
                            node.AppendChild(newChildNode);
                            _xmlApp.Save(xmlDocPathName);
                            break;
                        }
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        ///// <summary>
        ///// 根结点下插入结点(作为分层根节点)
        ///// </summary>
        ///// <param name="parentNode"></param>
        ///// <param name="nodeName"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public bool InsertNode(string parentNode, string nodeName)
        //{
        //    try
        //    {
        //        _xmlApp.Load(xmlDocPathName);
        //        XmlNode rootNode = _xmlApp.FirstChild.NextSibling;
        //        if (rootNode.Name.CompareTo(parentNode) == 0)
        //        {
        //            XmlNodeList nodeList = rootNode.ChildNodes;
        //            foreach (XmlNode node in nodeList)
        //            {
        //                if (node.Name.CompareTo(nodeName) == 0)
        //                {
        //                    return false;
        //                }
        //            }
        //            XmlElement newChildNode = _xmlApp.CreateElement(nodeName);
        //            rootNode.AppendChild(newChildNode);
        //            _xmlApp.Save(xmlDocPathName);
        //            return true;
        //        }
        //        else
        //        {
        //            XmlNodeList nodeList = rootNode.ChildNodes;
        //            foreach (XmlNode node in nodeList)
        //            {
        //                if (node.Name.CompareTo(parentNode) == 0)
        //                {
        //                    XmlNodeList nodeList1 = node.ChildNodes;
        //                    foreach (XmlNode node1 in nodeList)
        //                    {
        //                        if (node1.Name.CompareTo(nodeName) == 0)
        //                        {
        //                            return false;
        //                        }
        //                    }
        //                    XmlElement newChildNode = _xmlApp.CreateElement(nodeName);
        //                    rootNode.AppendChild(newChildNode);
        //                    _xmlApp.Save(xmlDocPathName);
        //                    return true;
        //                    //return false;
        //                }
        //            }
        //            //return false;
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
    }
}
