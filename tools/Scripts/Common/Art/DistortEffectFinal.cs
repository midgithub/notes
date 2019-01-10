using XLua;
﻿using UnityEngine;
using System.Collections;

 

[Hotfix]
public class DistortEffectFinal : MonoBehaviour {

    private DistortEffect distortEffect;
	// Use this for initialization
	void Start () {

        if (!SG.CoreEntry.m_bUseDistort)
        {
            return;
        }

        Camera mainCamera = SG.CoreEntry.gCameraMgr.MainCamera;
        if (mainCamera)
        {
            distortEffect = mainCamera.GetComponent<DistortEffect>();
        }
        else
        {
            distortEffect = GameObject.Find("Camera_Battle_main").GetComponent<DistortEffect>();
        }
	}
	
	// Update is called once per frame
    void OnPreRender()
    {
         if (!SG.CoreEntry.m_bUseDistort)
        {
            return;
        }
        Graphics.Blit(distortEffect.cameraRenderTexture, null, distortEffect.compositeMaterial);
    }
}

