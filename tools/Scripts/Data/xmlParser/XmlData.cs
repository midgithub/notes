using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;

/*
    解析xml文件
*/

namespace SG
{

[Hotfix]
    public class XmlData
    {
        private string m_filePath = null;
        private XmlDocument m_xmlDoc = null;

        //创建一个空的xml
        public bool CreateXml(string filePath)
        {
            LogMgr.UnityLog("filepath=" + filePath);

            if (File.Exists(filePath))
            {
                LogMgr.UnityLog("filPath is exist!");
                return false;
            }

            //创建xml文档
            XmlDocument xmlDoc = new XmlDocument();

            XmlDeclaration declare = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");

            xmlDoc.AppendChild(declare);

            //创建root文档            
            XmlElement root = xmlDoc.CreateElement("root");
            xmlDoc.AppendChild(root);

            //保存
            xmlDoc.Save(filePath);

            return true;
        }

        //打开文件
        public bool OpenXml(string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();

            //Object fileByte  = Resources.Load(filePath); 
            TextAsset fileByte = CoreEntry.gResLoader.LoadTextAsset(filePath, LoadModule.AssetType.Xml); //(TextAsset)CoreEntry.gResLoader.Load(filePath, typeof(TextAsset), 11);

            if (fileByte == null)
            {
                LogMgr.UnityLog("Error  加载XML 失败: " + filePath);
                return false;
            }
            Encoding utf8NoBom = new UTF8Encoding(false);

            //StringBuilder builder = new StringBuilder();
            //builder.Append(utf8NoBom.GetString(fileByte.bytes, 0, fileByte.bytes.Length ));
            //builder.Append("\n\n"); 
            string strXml = utf8NoBom.GetString(fileByte.bytes, 0, fileByte.bytes.Length);

            //string strXml =builder.ToString();


            //Encoding encoding = Encoding.UTF8; //Encoding.ASCII;//
            //StreamReader sr = new StreamReader(new MemoryStream(fileByte.bytes), encoding);

            try
            {
                xmlDoc.LoadXml(strXml);

            }
            catch (System.Exception e)
            {
                LogMgr.LogError("error!  load XML   path:" + filePath + "     \n" + e);
                return false;
            }

            //调整filepath，绝对路径                                           
            m_filePath = Application.dataPath + @"/ResData/" + filePath + @".xml";

            m_xmlDoc = xmlDoc;

            return true;
        }

        //add by Alex 20150423 直接解析字符串
        public bool OpenString(string text)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(text);
            m_xmlDoc = xmlDoc;
            return true;
        }

        //打开文件
        public bool OpenXmlInEditor(string filePath)
        {
            //调整filepath，绝对路径                                           
            m_filePath = Application.dataPath + @"/ResData/" + filePath + @".xml";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(m_filePath);

            m_xmlDoc = xmlDoc;

            return true;
        }

        //保留根节点删除
        public void RemoveAllChild()
        {
            //找到跟节点下面的所有节点          
            XmlNodeList nodeList = m_xmlDoc.SelectSingleNode("root").ChildNodes;

            XmlNode root = m_xmlDoc.SelectSingleNode("root");
            for (int i = nodeList.Count - 1; i >= 0; --i)
            {
                root.RemoveChild(nodeList.Item(i));
            }

        }

        public XmlElement GetEmptyElement(string name)
        {
            return m_xmlDoc.CreateElement(name);
        }

        //添加xml
        public void InsertElement(XmlElement node)
        {
            //找到需要插入的node                           
            XmlNode root = m_xmlDoc.SelectSingleNode("root");

            root.AppendChild(node);
        }

        //保存文件
        public void SaveXml()
        {
            m_xmlDoc.Save(m_filePath);
        }

        //删除所有节点，保留根节点
        public bool RemoveAll(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            XmlDocument xmlDoc = new XmlDocument();

            //加载
            xmlDoc.Load(filePath);

            //找到跟节点下面的所有节点          
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("root").ChildNodes;

            XmlNode root = xmlDoc.SelectSingleNode("root");
            for (int i = nodeList.Count - 1; i >= 0; --i)
            {
                root.RemoveChild(nodeList.Item(i));
            }

            xmlDoc.Save(filePath);

            return true;
        }

        //删除满足指定属性条件的所有节点
        public void RemoveElementsByAttrib(string attribName, string attribValue)
        {
            //找到跟节点下面的所有节点          
            XmlNode root = m_xmlDoc.SelectSingleNode("root");
            XmlNodeList nodeList = root.ChildNodes;

            for (int i = nodeList.Count - 1; i >= 0; --i)
            {
                XmlElement xe = (XmlElement)nodeList.Item(i);
                if (xe.GetAttribute(attribName) == attribValue)
                {
                    root.RemoveChild(xe);
                }
            }
        }

        //添加xml
        public void InsertElement(XmlElement parent, XmlElement node)
        {
            //找到需要插入的node
            parent.AppendChild(node);
        }

        //读取数据    
        public XmlNodeList ReadAll()
        {
            return m_xmlDoc.SelectSingleNode("root").ChildNodes;
        }

        public XmlNode ReadNode(string nodeName)
        {
            return m_xmlDoc.SelectSingleNode("root").SelectSingleNode(nodeName);
        }

    }
}