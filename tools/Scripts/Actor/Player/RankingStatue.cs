using XLua;
﻿using UnityEngine;
using System.Collections.Generic;
using SG;

[Hotfix]
public class RankingStatue : MonoBehaviour
{

    //string[] m_Effects = { "Effect/ui/uf_guanjun", "Effect/ui/uf_yajun", "Effect/ui/uf_jijun" };
    /// <summary>
    /// 0：第一名
    /// 1：第二名
    /// 2：第三名
    /// </summary>
    int Ranking = 0;
    List<Texture> mainTexList = new List<Texture>();

    List<string> MatrialPath = new List<string>();

    //Configs.PlayGroundStatue Config = null;

    //string Name = "";

    //float YOffset = 0f;

    //private Camera uiCamera = null;
    //GameObject m_RankBoard;
    //void Awake()
    //{
    //}

    public void Init(int ranking, string name)
    {
        Ranking = ranking;
        //Name = name;

        MatrialPath.Clear();
        MatrialPath.Add("AutoMaterial/player_zc");
        MatrialPath.Add("AutoMaterial/player_zc");
        MatrialPath.Add("AutoMaterial/player_zc");
    }

    // Use this for initialization
    void Start()
    {

        ShowLable();

        bool shouldChangeMaterial = true;
        if (shouldChangeMaterial)
        {
            SkinnedMeshRenderer[] skinnedMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
            mainTexList.Clear();
            //找到主贴图
            for (int i = 0; i < skinnedMeshRenderer.Length; ++i)
            {
                for (int j = 0; j < skinnedMeshRenderer[i].materials.Length; ++j)
                {

                    Texture mainTex = skinnedMeshRenderer[i].materials[j].GetTexture("_MainTex");
                    if (mainTex != null)
                    {
                        mainTexList.Add(mainTex);
                    }
                }
            }
            string path = MatrialPath[0];
            if (Ranking < MatrialPath.Count)
            {
                path = MatrialPath[Ranking];
            }

            //更换材质
            Material mat = CoreEntry.gResLoader.LoadMaterial(path);  //Bundle.AssetBundleLoadManager.Instance.Load<Material>(path);
            Material newMat = Material.Instantiate(mat) as Material;
            for (int i = 0; i < skinnedMeshRenderer.Length; ++i)
            {
                Material[] mats = new Material[skinnedMeshRenderer[i].materials.Length];
                for (int j = 0; j < skinnedMeshRenderer[i].materials.Length; ++j)
                {
                    mats[j] = newMat;
                    if (j < mainTexList.Count)
                    {
                        mats[j].SetTexture("_MainTex", mainTexList[j]);
                    }
                }
                skinnedMeshRenderer[i].materials = mats;
            }
        }

    }

    GameObject m_playerNameObj = null;

    void ShowLable()
    {


    }

    void OnEnable()
    {
        Invoke("DelayStopAnim", 0.2f);
    }

    void DelayStopAnim()
    {
        GetComponent<Animation>().Stop();
    }

    void OnDisable()
    {

    }

    void OnDestroy()
    {
        if (m_playerNameObj != null) GameObject.Destroy(m_playerNameObj);
    }
}
