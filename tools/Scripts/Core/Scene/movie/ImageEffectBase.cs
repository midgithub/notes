using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("")]
[Hotfix]
public class ImageEffectBase : MonoBehaviour
{
    private string shaderPath = "";

    public string ShaderPath
    {
        get { return shaderPath; }
        set 
        {
            shaderPath = value;
            if (shaderPath != "")
            {
                shader = CoreEntry.gResLoader.LoadShader(shaderPath);
            }
        }
    }

    private string materialPath = "";

    public string MaterialPath
    {
        get { return materialPath; }
        set 
        {
            materialPath = value;
            if (materialPath != "")
            {
                Material mat = CoreEntry.gResLoader.LoadMaterial(materialPath); //Bundle.AssetBundleLoadManager.Instance.Load(materialPath,typeof(Material))as Material;
                if (mat != null)
                {
                    m_Material = Instantiate(mat) as Material;
                }
                mat = null;
                //Resources.UnloadUnusedAssets();
            }
        }
    }
    /// Provides a shader property that is set in the inspector
    /// and a material instantiated from the shader
    public Shader shader;
    private Material m_Material;

    protected virtual void Start()
    {
        // Disable if we don't support image effects
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

        // Disable the image effect if the shader can't
        // run on the users graphics card
        if (!shader || !shader.isSupported)
            enabled = false;
    }

    protected Material material
    {
        get
        {
            if (m_Material == null)
            {
                if (shader != null)
                {
                    m_Material = new Material(shader);
                    m_Material.hideFlags = HideFlags.HideAndDontSave;
                }
            }
            return m_Material;
        }
    }

    protected virtual void OnDisable()
    {
        if (m_Material)
        {
            DestroyImmediate(m_Material);
        }
    }
}

