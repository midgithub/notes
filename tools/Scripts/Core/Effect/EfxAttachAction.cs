using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
public class EfxAttachAction : MonoBehaviour {
    //动作对应的特效
    private Transform m_attachObject;
    private Transform m_transform;        
    private bool m_isSyncTransform = false;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (m_attachObject == null)
        {
            return;
        }        

        if (m_isSyncTransform)
        {
            //位置方向一致	        
            m_transform.position = m_attachObject.position;
            // 被yuxj注释  解决BUG：甄姬技能特效方向错误
            // m_transform.rotation = Quaternion.LookRotation(m_attachObject.forward);   
            m_transform.rotation = m_attachObject.rotation;   
        }                    
	}

    public void Init(Transform attachObj, float destoryTime, bool isSyncTransform=true)
    {
        m_attachObject = attachObj;
        m_isSyncTransform = isSyncTransform;   

	    m_transform = this.transform;

        // 解决BUG：甄姬技能特效方向错误
        m_transform.localPosition = new Vector3(0, 0, 0);
        m_transform.localRotation = new Quaternion(0, 0, 0, 1f);

        //位置方向一致	
        m_transform.position = m_attachObject.position;
        if (isSyncTransform)
            m_transform.rotation = m_attachObject.rotation;
        else
            m_transform.rotation = new Quaternion(0, 0, 0, 1f);
     
        Invoke("AutoDestoryObject", destoryTime);
    }

    //脱离Obj，原地播放
    public void DetachObject()
    {
        m_attachObject = null;        
    }

    public void DestoryObject()
    {
        Destroy(this.gameObject);                    
    }        

    void AutoDestoryObject()
    {
        CancelInvoke("AutoDestoryObject");
                          
        Destroy(this.gameObject);
    }
}

}

