using XLua;
﻿using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using SG;

namespace Bundle
{
[Hotfix]
    public class XMLTool
    {
        public static void CreateXMLRoot(string filePath)
        {
            if(!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
            if (!File.Exists(filePath))
            {
                string[] xmlRootData = new string[3];
                xmlRootData[0] = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                xmlRootData[1] = "<Root>";
                xmlRootData[2] = "</Root>";

                File.WriteAllLines(filePath, xmlRootData);
            }
        }

        public static List<AssetBundleConfig> LoadAllABConfig(string filePath, bool isStreamPath = false)
        {
            if (isStreamPath)
            {
                if (!File.Exists(filePath))
                {
                    return null;
                }
            }
            else
            {
                CreateXMLRoot(filePath);
            }

            List<AssetBundleConfig> abCfgList = new List<AssetBundleConfig>();

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filePath);
            }
            catch(System.Exception e)
            {
                LogMgr.UnityLog("Read xml error: recreate" + filePath);
                File.Delete(filePath);
                CreateXMLRoot(filePath);
                xmlDoc.Load(filePath);
            }
            XmlNode rootNode = xmlDoc.SelectSingleNode("Root");
            XmlNodeList nodeList = rootNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                XmlElement element = node as XmlElement;
                AssetBundleConfig dataCfg = new AssetBundleConfig();
                dataCfg.ABName = element.GetAttribute("ABName");
                dataCfg.RelativePath = element.GetAttribute("RelativePath");
                dataCfg.MD5Value = element.GetAttribute("MD5");
                dataCfg.FileSize = long.Parse(element.GetAttribute("FileSize"));
                dataCfg.Build = int.Parse(element.GetAttribute("Build"));

                abCfgList.Add(dataCfg);
            }

            return abCfgList;
        }

        public static void SaveABConfig(string filePath, List<AssetBundleConfig> list)
        {
            CreateXMLRoot(filePath);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlNode rootNode = xmlDoc.SelectSingleNode("Root");
            for (int i = 0; i < list.Count; i++)
            {
                XmlNode node = rootNode.SelectSingleNode(list[i].ABName);
                XmlElement xe;
                if (node == null)
                {
                    xe = xmlDoc.CreateElement(list[i].ABName);
                }
                else
                {
                    xe = node as XmlElement;
                }

                xe.SetAttribute("ABName", list[i].ABName);
                xe.SetAttribute("RelativePath", list[i].RelativePath);
                xe.SetAttribute("MD5", list[i].MD5Value);
                xe.SetAttribute("FileSize", list[i].FileSize.ToString());
                xe.SetAttribute("Build", list[i].Build.ToString());

                rootNode.AppendChild(xe);
            }
            try
            {
                xmlDoc.Save(filePath);
            }
            catch(System.Exception e)
            {
                LogMgr.UnityLog("save xml error: recreate" + filePath);
                File.Delete(filePath);
                CreateXMLRoot(filePath);
            }
        }
    }
}

