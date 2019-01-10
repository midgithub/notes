/**
* @file     : EfxSetAttachPoint.cs
* @brief    : 
* @details  : 特效编辑器使用的设置挂点脚本
* @author   : 
* @date     : 2014-xx-xx
*/
using XLua;
using UnityEngine;
using System.Collections;


namespace SG
{

    public enum AttachPoint
    {
        E_None = 0,
        E_Foot_L,
        E_Foot_R,
        E_Hand_L,
        E_Hand_R,
        E_Hand_R01,
        E_Hand_R02,
        E_head,
        E_Shoulder_L,
        E_Shoulder_R,
        E_back,
        E_Spine,
        E_top,
        E_Hand_L01,
        E_Hand_L02,        
        E_head01        
    };         
    
    //做个保存
[Hotfix]
    public class EfxSetAttachPoint : MonoBehaviour
    {
        //编辑器设置    
        public AttachPoint m_attachPointEnum = AttachPoint.E_None;
        Transform m_transform = null;                           
        private bool m_isFxMakerEditor = true;        
             
        GameObject m_rootObj = null;                    
        Transform m_aimPoint = null;

        void Awake()
        {
            m_transform = this.transform;                                      
        }

        //主动调用，非fxmaker状态
        public void Init(bool setFxMakerEditor)
        {
            m_isFxMakerEditor = setFxMakerEditor;          
        }

        void Start()
        {
            if (m_isFxMakerEditor)
            {                
                m_rootObj = this.gameObject.transform.root.gameObject;
                m_aimPoint = null;

                Transform[] childs = m_rootObj.GetComponentsInChildren<Transform>();
                foreach (Transform child in childs)
                {
                    if (child.name.Equals(m_attachPointEnum.ToString()))
                    {
                        m_aimPoint = child;
                        break;
                    }
                }
            }            
        }

        void LateUpdate()
        {
            if (m_isFxMakerEditor)
            {                
                if (m_aimPoint != null)
                {
                    m_transform.position = m_aimPoint.position;
                    m_transform.rotation = m_aimPoint.rotation;
                }   
            }                              
        }
    }
  
};  //end SG

