using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;

[Hotfix]
public class CameraEffect : MonoBehaviour 
{
    public AnimationCurve m_AnimationCurve;
    private CameraBase m_CameraFollow;
    bool m_Shaking = false;
    private Transform m_CameraTransform;
    private float m_BeginTime = 0;
    public float Scale = 1.0f;
	// Use this for initialization
	void Awake () 
    {
        GameObject editorPre = CoreEntry.gResLoader.Load("Scene/Camera/CameraShakeEditor") as GameObject;// (GameObject)Bundle.AssetBundleLoadManager.Instance.Load("Scene/Camera/CameraShakeEditor", typeof(GameObject));
        m_AnimationCurve = editorPre.GetComponent<CameraShakeEditor>().m_AnimationCurve;
        m_CameraTransform = gameObject.transform;
        m_CameraFollow = gameObject.GetComponent<CameraBase>();
	}
	
	// Update is called once per frame
	void LateUpdate () 
    {
        if (m_Shaking)
        {
            if (m_CameraFollow != null)
            {
                m_CameraTransform = m_CameraFollow.UpdateMainCameraTransform();
            }
           
            //相机抖动的值
            float posY = m_AnimationCurve.Evaluate(Time.time - m_BeginTime) * Scale;
            m_CameraTransform.position = m_CameraTransform.position + new Vector3(0, posY, 0);
        }  
	}

    public void ShakeStart()
    {
        if (m_CameraFollow != null)
        {
            m_CameraFollow.m_cameraShake = true;
        }
        m_Shaking = true;
        m_BeginTime = Time.time;
        CancelInvoke("ShakeEnd");
        if (m_AnimationCurve != null)
        {
            float length = m_AnimationCurve[m_AnimationCurve.length - 1].time;
            Invoke("ShakeEnd", length);
        }
    }

    void ShakeEnd()
    {
        if (m_CameraFollow != null)
        {
            m_CameraFollow.m_cameraShake = false;
        }
        m_Shaking = false;
    }
}
