using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;
[Hotfix]
public class RandBGImpl : MonoBehaviour {

	// Use this for initialization

    void Awake()
    {
        if (strRandBgResources != null)
        {
            m_bgs = new GameObject[strRandBgResources.Length];
            for (int i = 0; i < strRandBgResources.Length; i++)
            {
                if (strRandBgResources[i] == null) continue;
                GameObject oj = (GameObject)CoreEntry.gResLoader.Load(strRandBgResources[i]);
                if(oj == null)continue;
                GameObject obj=  NGUITools.AddChild(this.gameObject,oj );
                obj.transform.localPosition = Vector3.zero;
                m_bgs[i] = obj;
            }
        }
    }
	void Start () {
	
	}
    public string[] strRandBgResources; 
    public GameObject[] m_bgs;


    static int lastIndex = 0;

    void OnEnable()
    {
        int tempIndex = Random.Range(0, m_bgs.Length);
        if (tempIndex == lastIndex)
        {
            lastIndex++;
        }
        if (lastIndex >= m_bgs.Length)
        {
            lastIndex = 0;
        }
        for (int i = 0; i < m_bgs.Length; i++)
        {
            if (lastIndex == i)
            {
                if(m_bgs[i] != null)
                    m_bgs[i].SetActive(true);
            }
            else
            {
                if (m_bgs[i] != null)
                    m_bgs[i].SetActive(false);

            }
        }

    }
}

