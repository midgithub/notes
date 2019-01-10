using XLua;
﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[Hotfix]
public class RenderManager : MonoBehaviour {

    public bool bFog = true;
    public Color heightFogColor = new Color(1.0f, 1.0f, 1.0f);
    public float top = 10.0f;
    public float bottom = 0.0f;
    public Color distanceFogColor = new Color(1.0f, 1.0f, 1.0f);
    public float near = 10.0f;
    public float far = 100.0f;
        
    void Awake()
    {
        //添加标记
        this.gameObject.tag = "fogManager";
        Shader.DisableKeyword("_FOG_ON");
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (bFog)
        {
            Shader.EnableKeyword("_FOG_ON");
            Shader.SetGlobalColor("_HeightFogColor", heightFogColor);
            Shader.SetGlobalColor("_DistanceFogColor", distanceFogColor);
            Vector4 fogParam = new Vector4(top, bottom, near, far);
            Shader.SetGlobalVector("_FogParam", fogParam);
        }
        else
        {
            Shader.DisableKeyword("_FOG_ON");
        }     
	}
}

