using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;

[Hotfix]
public class BlobShadow : MonoBehaviour 
{
	private GameObject blob;
    private Transform blobT;
    public GameObject shadowPro;
    private int shadowType = 1;
    private Transform m_transform = null;	

	public float scaleX = 1;
	public float scaleY = 1;    
    private Vector3 m_cachePos = Vector3.zero;
    private Entity m_entity = null;
    public void Init(Entity entity, int type)
    {
        m_entity = entity;
        //shadowType = type;
        shadowType = SettingManager.Instance.PicSetting - 1;
        if (blob == null)
        {
            Object obj = CoreEntry.gResLoader.Load("Effect/common/fx_shadow");

            if (obj == null)
            {
                LogMgr.LogError("找不到 prefab: " + "Effect/common/fx_shadow");
                return;
            }
            blob = GameObject.Instantiate(obj) as GameObject;
            blobT = blob.transform;
            blobT.localScale = new Vector3(3.0f, 3.0f, 3.0f);
            blobT.parent = transform;
            m_transform = this.transform;
            UpdatePostion();
        }
         
        ShadowProCreateOrDestroy(shadowType == 2); 
    }

    //shadowpro true创建对像  false销毁对象
    public void ShadowProCreateOrDestroy(bool v)
    {
        if(v)
        {
            if (m_entity is PlayerObj && shadowPro == null) 
            {
                if (shadowPro == null)
                {
                    Object obj = CoreEntry.gResLoader.Load("Effect/common/fx_shadow_pro");
                    if (obj == null)
                    {
                        LogMgr.LogError("找不到 prefab: " + "Effect/common/fx_shadow_pro");
                        return;
                    }
                    shadowPro = GameObject.Instantiate(obj) as GameObject;
                }
                if (shadowPro != null)
                {
                    shadowPro.transform.localScale = Vector3.one;
                    shadowPro.transform.localPosition = Vector3.zero;
                    shadowPro.transform.localRotation = Quaternion.identity;

                    CameraLock cl = shadowPro.GetComponent<CameraLock>();
                    if (cl != null)
                    {
                        cl.m_TargetObj = m_entity.gameObject;
                    }
                     
                }
            }
        }
        else
        {
            if(shadowPro != null)
            {
                Object.DestroyImmediate(shadowPro);
                shadowPro = null;
            }
        }

    }


	void Start () 
	{       
        m_transform = this.transform; 
	}
//     void Update()
//     {
//         UpdatePostion();
//     }
    // Update is called once per frame
    void UpdatePostion () 
	{
        bool check = blob != null && Vector3.SqrMagnitude(m_cachePos - m_transform.position) > 0.01f;
        if (check)
        {
            blobT.position = BaseTool.instance.GetGroundPoint(m_transform.position); 
            m_cachePos = m_transform.position;
        }
    }

	void OnDestroy()
	{
        if (blob != null)
        {
            GameObject.DestroyObject(blob);
        } 
    }
   
    public void HideShadow()
	{
		if(blob != null)
		{
            //MeshRenderer mr = blob.GetComponent<MeshRenderer>();
            //mr.enabled = false;
            blob.SetActive(false);
           
		}
        if (shadowPro != null)
        {
            shadowPro.SetActive(false);
        }
	}
	public void ShowShadow()
	{
		if(blob != null)
		{
            //MeshRenderer mr = blob.GetComponent<MeshRenderer>();
            //mr.enabled = true;
            if (m_entity is PlayerObj)
            {
                blob.SetActive(shadowType == 1 ? true : false);
            }
            else
            {
                blob.SetActive(shadowType == 2 ? true : false);
            }
        }
        if (shadowPro != null)
        {
            shadowPro.SetActive(shadowType == 2 ? true : false);
        }
	}

    public void ChangeShow(int type)
    {
        shadowType = SettingManager.Instance.PicSetting - 1;
        //shadowType = type;
        if(m_entity is PlayerObj)
        {
            ShadowProCreateOrDestroy(shadowType == 2);
        }

        ShowShadow();
    }
}

