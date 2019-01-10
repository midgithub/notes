using XLua;
﻿using UnityEngine;



[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Blur/Mobile Blur ")]

[Hotfix]
public class MobileBlur : MonoBehaviour
{

    [Range(0, 5)]
    public int downsample = 4;

    public enum BlurType
    {
        Standard = 0,
        Sgx = 1,
    }

    [Range(0.0f, 50.0f)]
    public float blurSize = 0.0f;

    [Range(1, 4)]
    public int blurIterations = 3;
    public BlurType blurType = BlurType.Standard;

    public Shader blurShader;

    public bool IsFirstPage = false;    //是否是一级界面

    private Material m_blurMaterial = null;

    protected Material blurMaterial
    {
        get
        {
            if (m_blurMaterial == null)
            {
                m_blurMaterial = new Material(blurShader);
                m_blurMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_blurMaterial;
        }
    }

    public RenderTexture cameraRenderTexture;

    private GameObject shaderCamera;



    protected void OnDisable()
    {
        if (blurMaterial)
        {
            DestroyImmediate(blurMaterial);
        }
        if (shaderCamera)
        {
            DestroyImmediate(shaderCamera);
        }

        if (cameraRenderTexture)
        {
            RenderTexture.ReleaseTemporary(cameraRenderTexture);
            cameraRenderTexture = null;
        }
    }

    protected void Start()
    {
        // Disable if we don't support image effects
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

        if (!blurShader || !blurShader.isSupported)
        {
            enabled = false;
            return;
        }
    }

    void OnDestroy()
    {
    }

    public bool isOpenBlur = false;
    void BlurImp(bool open)
    {
        isOpenBlur = open;
    }

    public void OnDo()//OnPostRender()
    {

        if (!isOpenBlur) return;

        if (cameraRenderTexture != null)
        {
            RenderTexture.ReleaseTemporary(cameraRenderTexture);
            cameraRenderTexture = null;
        }
        int rtW = (int)GetComponent<Camera>().pixelWidth >> downsample;
        int rtH = (int)GetComponent<Camera>().pixelHeight >> downsample;

        //int rtW = (int)camera.pixelWidth /16;
        //int rtH = (int)camera.pixelHeight/16 ;

        cameraRenderTexture = RenderTexture.GetTemporary(rtW, rtH, 16);



        if (shaderCamera == null)
        {
            shaderCamera = new GameObject("ShaderCamera", typeof(Camera));
            shaderCamera.GetComponent<Camera>().enabled = false;
            shaderCamera.hideFlags = HideFlags.HideAndDontSave;
        }

        Camera cam = shaderCamera.GetComponent<Camera>();
        if (IsFirstPage && null != SG.CoreEntry.gCameraMgr.MainCamera)
        {
            cam.CopyFrom(SG.CoreEntry.gCameraMgr.MainCamera);
        }
        else
        {
            cam.CopyFrom(GetComponent<Camera>());
            cam.cullingMask = 1 << LayerMask.NameToLayer("UI");
        }
        //cam.backgroundColor = new Color(0f, 0f, 0f, 0f);
        //cam.clearFlags = CameraClearFlags.SolidColor;
        cam.targetTexture = cameraRenderTexture;
        cam.Render();


        float widthMod = 1.0f / (1.0f * (1 << downsample));
        blurMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod, -blurSize * widthMod, 0.0f, 0.0f));

        int passOffs = blurType == BlurType.Standard ? 0 : 2;

        for (int i = 0; i < blurIterations; i++)
        {
            float iterationOffs = (i * 1.0f);
            blurMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod + iterationOffs, -blurSize * widthMod - iterationOffs, 0.0f, 0.0f));

            // vertical blur
            RenderTexture rt2 = RenderTexture.GetTemporary(rtW, rtH, 0);
            rt2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(cameraRenderTexture, rt2, blurMaterial, 1 + passOffs);
            RenderTexture.ReleaseTemporary(cameraRenderTexture);
            cameraRenderTexture = rt2;

            // horizontal blur
            rt2 = RenderTexture.GetTemporary(rtW, rtH, 0);
            rt2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(cameraRenderTexture, rt2, blurMaterial, 2 + passOffs);
            RenderTexture.ReleaseTemporary(cameraRenderTexture);
            cameraRenderTexture = rt2;
        }

        Shader.SetGlobalTexture("_blurTexture", cameraRenderTexture);
    }
}

