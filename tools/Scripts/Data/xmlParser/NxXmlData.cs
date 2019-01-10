/**
* @file     : a
* @brief    : b
* @details  : d
* @author   : a
* @date     : 2014-xx-xx
*/

using XLua;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;

namespace SG
{

[Hotfix]
	public class NxXmlData {

		private string m_filePath = null;  
		private XmlDocument m_xmlDoc = null;

		//打开文件
		public bool OpenXmlInEditor(string filePath)
		{                    
			//调整filepath，绝对路径                                           
			m_filePath = Application.dataPath + filePath + @".xml";        

			XmlDocument xmlDoc = new XmlDocument(); 
			xmlDoc.Load(m_filePath);
			
			m_xmlDoc = xmlDoc;
			
			return true;
		}

		//读取JointDesc数据    
		public XmlNodeList ReadJointDescs()
		{
			XmlNode parentNode = m_xmlDoc.SelectSingleNode("NXUSTREAM2/NxuPhysicsCollection/NxSceneDesc");
			XmlNodeList jointDescList = parentNode.SelectNodes("NxD6JointDesc");
			//LogMgr.UnityLog("Joint num : " + jointDescList.Count.ToString());
			return jointDescList;
		}

		//读取ActorDesc数据
		public XmlNodeList ReadActorDescs()
		{
			XmlNode parentNode = m_xmlDoc.SelectSingleNode("NXUSTREAM2/NxuPhysicsCollection/NxSceneDesc");
			XmlNodeList jointDescList = parentNode.SelectNodes("NxActorDesc");
			//LogMgr.UnityLog("Actor num : " + jointDescList.Count.ToString());
			return jointDescList;
		}

		//读取SceneDesc节点
		public XmlNode ReadSceneDesc()
		{
			return m_xmlDoc.SelectSingleNode("NXUSTREAM2/NxuPhysicsCollection/NxSceneDesc");
		}

		//读取节点值
		public string GetChildNodeInnerText(XmlNode parentNode, string childName)
		{
			if (parentNode != null && parentNode.HasChildNodes == true && childName != null)
			{
				XmlNode childNode = parentNode.SelectSingleNode(childName);
				if (childNode != null)
				{
					return childNode.InnerText;
				}
			}

			return null;
		}

		//读取子节点元素属性值
		public string GetChildElementAttribValue(XmlNode parentNode, string childElementName, string attribName)
		{
			if (parentNode != null && parentNode.HasChildNodes == true && childElementName != null)
			{
				XmlElement childElement = (XmlElement)parentNode.SelectSingleNode(childElementName);
				if (childElement != null && attribName != null)
				{
					return childElement.GetAttribute(attribName);
				}
			}

			return null;
		}

		//读取当前节点元素属性值
		public string GetAttribValue(XmlNode node, string attribName)
		{
			if (node != null && attribName != null)
			{
				XmlElement elememt = (XmlElement)node;
				return elememt.GetAttribute(attribName);
			}

			return null;
		}

		//获取子节点
		public XmlNode GetSingleChildNode(XmlNode parentNode, string childName)
		{
			if (parentNode != null && childName != null)
			{
				return parentNode.SelectSingleNode(childName);
			}

			return null;
		}

		//通过节点名和属性值获取节点
		public XmlNode GetChildNodeByNameAndAttrib(XmlNode parentNode, string nodeName, string attribName, string attribValue)
		{
			if (parentNode != null && parentNode.HasChildNodes == true &&
			    nodeName != null && attribName != null && attribValue != null)
			{
				XmlNodeList nodeList = parentNode.SelectNodes(nodeName);
				foreach (XmlNode node in nodeList)
				{
					string value = ((XmlElement)node).GetAttribute(attribName);
					if (value != null && value.CompareTo(attribValue) == 0)
					{
						return node;
					}
				}
			}

			return null;
		}
	}

};//End SG

