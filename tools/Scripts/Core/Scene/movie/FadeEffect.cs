using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class FadeEffect : ImageEffectBase{

    public Color blendColor = Color.white; 
	// Use this for initialization
	protected override void Start () {
        ShaderPath = "Custom/BlendColor";
	}

    void OnRenderImage(RenderTexture source, RenderTexture destination)    {
        material.SetColor("_BlendColor", blendColor);
        Graphics.Blit(source, destination, material);
    }
}

