using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class SendUIBlur : MonoBehaviour {

	// Use this for initialization
	void Start () {
	

	}

    public UITexture tex = null;
    UIBlur b = null;
    GameObject obj = null;
    Camera cam;

    void OnEnable()
    {
        if (cam == null)
        {
            CreateTmpCam(NGUITools.FindCameraForLayer(gameObject.layer).gameObject);
        }

        if (obj != null)
        {
            b = obj.GetComponent<UIBlur>();
            if (b == null)
            {
                b = obj.AddComponent<UIBlur>();
            }
            b.enabled = true;
            b.Tex = tex;
        }
       
    }

    void CreateTmpCam(GameObject cameraObj)
    {
        obj = new GameObject("tempCam");

        obj.transform.parent = cameraObj.transform;


        cam = obj.AddComponent<Camera>();

        cam.CopyFrom(cameraObj.GetComponent<Camera>());

    }

    void OnDisable()
    {
        if (b != null)
        {
            b.enabled = false;
        }
    }
}

