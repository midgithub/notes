using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
public class EfxAttachActionPool : MonoBehaviour {
    //动作对应的特效
    private Transform m_attachObject = null;
    private Transform m_transform = null;        
    private bool m_isSyncTransform = false;

    bool m_followRotation = true;

    Vector3 m_offset = Vector3.zero;

    public UnityEngine.Vector3 Offset
    {
        get { return m_offset; }
        set { m_offset = value; }
    }
    public bool FollowRotation
    {
        get { return m_followRotation; }
        set { m_followRotation = value; }
    }

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
            m_transform.position = m_attachObject.position + m_offset;
            // 被yuxj注释  解决BUG：甄姬技能特效方向错误
            // m_transform.rotation = Quaternion.LookRotation(m_attachObject.forward);   
            if (m_followRotation)
            {
                m_transform.rotation = m_attachObject.rotation;   
            }
        }                    
	}

    public void Init(Transform attachObj, float destoryTime, bool isSyncTransform=true)
    {
        if (attachObj != null)
        {
            m_attachObject = attachObj;
            m_isSyncTransform = isSyncTransform;

            m_transform = this.transform;

            // 解决BUG：甄姬技能特效方向错误
            m_transform.localPosition = new Vector3(0, 0, 0);
            m_transform.localRotation = new Quaternion(0, 0, 0, 1f);


         //   Vector3 position = new Vector3(attachObj.position.x, attachObj.position.y + 10.1f, attachObj.position.z);

            //位置方向一致	
            m_transform.position = m_attachObject.position;

           

            if (isSyncTransform)
                m_transform.rotation = m_attachObject.rotation;
            else
                m_transform.rotation = new Quaternion(0, 0, 0, 1f);


        }

        CancelInvoke("AutoDestoryObject");
        if (destoryTime!=100000f)
            Invoke("AutoDestoryObject", destoryTime);
    }



    public void InitRemain(Transform attachObj, float destoryTime, bool isSyncTransform = true)
    {
        if (attachObj != null)
        {
            m_attachObject = attachObj;
            m_isSyncTransform = isSyncTransform;

            m_transform = this.transform;

            // 解决BUG：甄姬技能特效方向错误
            m_transform.localPosition = new Vector3(0, 0, 0);
            m_transform.localRotation = new Quaternion(0, 0, 0, 1f);

            //位置方向一致	
            m_transform.position = m_attachObject.position;

            m_transform.rotation = m_attachObject.rotation;
            
        }

        CancelInvoke("AutoDestoryObject");
        if (destoryTime != 100000f)
            Invoke("AutoDestoryObject", destoryTime);
    }





    //脱离Obj，原地播放
    public void DetachObject()
    {        
        m_attachObject = null;        
    }

    public void DestoryObject()
    {
        //Destroy(this.gameObject);     
        //transform.position = new Vector3(9999, 9999, 9999);
        CoreEntry.gGameObjPoolMgr.Destroy(this.gameObject);
    }

    void AutoDestoryObject()
    {
        CancelInvoke("AutoDestoryObject");

        CoreEntry.gGameObjPoolMgr.Destroy(this.gameObject);
        //Destroy(this.gameObject);
    }
}

}

