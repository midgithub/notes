using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class CopyFBCamera : MonoBehaviour {


    Camera m_FBCamera ;

    Camera m_camera; 

	// Use this for initialization

    int m_backCullingMask; 
	void Start () {
        m_FBCamera = transform.parent.GetComponent<Camera>();
        m_backCullingMask = m_FBCamera.cullingMask; 

	}


    void Awake()
    {

    }
    void OnEnable()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;
        StartCoroutine(delayInit());
    }
   

    IEnumerator delayInit()
    {
        yield return new WaitForSeconds(0.01f); 
        m_camera = gameObject.GetComponent<Camera>();
        m_FBCamera = transform.parent.GetComponent<Camera>();
        if (m_FBCamera != null && m_camera != null)
        {
            //m_camera.transform.LookAt()
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            transform.localScale = Vector3.one;
            m_camera.fieldOfView = m_FBCamera.fieldOfView;
            m_camera.nearClipPlane = m_FBCamera.nearClipPlane;
            m_camera.farClipPlane = m_FBCamera.farClipPlane;

            int tmpMask = m_camera.cullingMask;
            int notMask = ~tmpMask;
            m_FBCamera.cullingMask = m_FBCamera.cullingMask & notMask;
            //tmpMask = tmpMask 

        }


    }

    void OnDisable()
    {
        m_FBCamera.cullingMask = m_backCullingMask;

        NsRenderManager Ns = GetComponent<NsRenderManager>();
        if (Ns != null)
        {
            GameObject.Destroy(Ns);
        }
    }

    // Update is called once per frame
    void Update()
    {

	}
}

