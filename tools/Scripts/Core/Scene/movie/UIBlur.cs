using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class UIBlur : ImageEffectBase
{
    private UITexture tex = null;

    public UITexture Tex
    {
        get { return tex; }
        set
        {
            tex = value;
            if (tex != null && accumTexture != null)
            {
                tex.mainTexture = accumTexture;
            }
        }
    }
    private RenderTexture accumTexture = null;
    public Color blendColor = Color.blue;
    protected override void Start()
    {
        ShaderPath = "Custom/BlendColor";
    }

    public void OnEnable()
    {
      
        if (accumTexture == null)
        {
          
            accumTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
            GetComponent<Camera>().targetTexture = accumTexture;
            GetComponent<Camera>().Render();
        }
    }


    protected override void OnDisable()
    {
        base.OnDisable();
        if (accumTexture != null)
        {
            RenderTexture.ReleaseTemporary(accumTexture);
        }

    }


   
}

