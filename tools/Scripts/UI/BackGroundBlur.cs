using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;
using System.Collections.Generic;
using UnityEngine.UI;
#if !UNITY_5_3_8
using UnityEngine.Profiling;
#endif

[Hotfix]
public class BackGroundBlur : MonoBehaviour 
{

    Texture2D m_CutImage;
    RenderTexture rtTempA;
    Material m_BlurMaterial;
    RenderTexture m_Rt;

    [Range(0, 5)]
    public int downsample = 4;

    public enum BlurType
    {
        Standard = 0,
        Sgx = 1,
    }

    [Range(0.0f, 50.0f)]
    public float blurSize = 0.0f;

    [Range(1, 8)]
    public int blurIterations = 6;
    public BlurType blurType = BlurType.Standard;

    public Color m_GrayColor = new Color(0.7f, 0.7f, 0.7f, 1f);//灰度颜色值

    void BalckBG()
    { 
        RawImage texture = gameObject.GetComponent<RawImage>();
        if (texture == null)
        {
            return;
        }
        texture.texture = new Texture2D(Screen.width, Screen.height);
        texture.color = new Color(0,0,0,0.7f);
    }

    void Awake()
    {
        m_CutImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        m_BlurMaterial = (Material)CoreEntry.gResLoader.Load("UI/Materials/Gaussian Blur", typeof(Material));
    }

    void OnEnable()
    {
        //BalckBG();
        //return;
        RawImage texture = gameObject.GetComponent<RawImage>();
        if (texture != null)
        {
            texture.transform.SetRenderActive(false);
        }
        Shader blurShader = Shader.Find("Unlit/Transparent Colored Gaussian Blur");
        if (!SystemInfo.supportsImageEffects || blurShader == null || !blurShader.isSupported)
        {
            BalckBG();
        }
        else
        {
            StartCoroutine(CutImage());
        }
    }

    IEnumerator CutImage()
    {
        //Rect rect = new Rect(0, 0, Screen.width, Screen.height);

        yield return new WaitForEndOfFrame();
        m_Rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
      
        Camera[] camlist = new Camera[Camera.allCamerasCount];
       // RenderTexture[] camRenderTex = new RenderTexture[Camera.allCamerasCount];
        int count = Camera.GetAllCameras(camlist);
        List<Camera> tempCameralist = new List<Camera>();

        Profiler.BeginSample("遍历相机渲染到纹理");
        for (int i = 0; i < count; i++)
        {
            //if (camlist[i].name.Equals("FBCamera"))// || camlist[i].name.Equals("UICamera")
            if (camlist[i].targetTexture == null)
            {
                //camRenderTex[i] = camlist[i].targetTexture;
                camlist[i].targetTexture = m_Rt;

                camlist[i].Render();
                tempCameralist.Add(camlist[i]);
            }
        }
        Profiler.EndSample();
       
        /*
        Profiler.BeginSample("纹理渲染到后台缓冲");
        RenderTexture.active = rt;
        Profiler.EndSample();

        Profiler.BeginSample("截屏");
        m_CutImage.ReadPixels(rect, 0, 0, false);//这个作用相当于截全屏
        m_CutImage.Apply();
        Profiler.EndSample();
        */
        RenderTexture.active = null;                                                       
        for (int i = 0; i < tempCameralist.Count; i++)
        {
            if (tempCameralist[i] != null)
            {
                tempCameralist[i].targetTexture = null;
            }
        }

        //ui摄像机目前渲染不了，只渲染FBCamera
        if(tempCameralist.Count == 0)
            yield break;
        RawImage texture = gameObject.GetComponent<RawImage>();
        if (texture == null)
        {
            yield break;
        }

        if (m_BlurMaterial == null)
        {
            yield break;
        }

        int rtW = Screen.width;
        int rtH = Screen.height;
        rtTempA = RenderTexture.GetTemporary(rtW, rtH, 0);//获取一张临时纹理A
        rtTempA.filterMode = FilterMode.Bilinear;
        Graphics.Blit(m_Rt, rtTempA);//拷贝截屏数据到临时渲染目标A

        for (int i = 0; i < blurIterations; i++)
        {
            float iteraionOffs = i * 0.9f;
            m_BlurMaterial.SetFloat("_blurSize", iteraionOffs);
            //vertical blur  
            RenderTexture rtTempB = RenderTexture.GetTemporary(rtW, rtH, 0);
            rtTempB.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rtTempA, rtTempB, m_BlurMaterial, 0);
            RenderTexture.ReleaseTemporary(rtTempA);
            rtTempA = rtTempB;

            //horizontal blur  
            rtTempB = RenderTexture.GetTemporary(rtW, rtH, 0);
            rtTempB.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rtTempA, rtTempB, m_BlurMaterial, 1);
            RenderTexture.ReleaseTemporary(rtTempA);
            rtTempA = rtTempB;
        }
        texture.texture = rtTempA;
        //add by lzp 10:43 2016/9/22  yangqu要求模糊背景压暗
        texture.color = m_GrayColor;
        texture.transform.SetRenderActive(true);

    }

    void OnDistroy()
    {
        Destroy(m_CutImage);
    }

    void OnDisable()
    {
        RenderTexture.ReleaseTemporary(rtTempA);
        RenderTexture.ReleaseTemporary(m_Rt);
    }
}

