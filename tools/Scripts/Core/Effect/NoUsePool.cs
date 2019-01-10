using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{
    // 解决TrailRenderer特效，加到对象池后多次随机播放出现错乱的问题
[Hotfix]
public class NoUsePool : MonoBehaviour
{
    Transform[] tranOld;
    Transform[] tranNew;

    void Awake()
    {
        tranOld = new Transform[transform.childCount];
        tranNew = new Transform[transform.childCount];
        for (int i = 0; i < tranOld.Length; i++)
        {
            tranOld[i] = transform.GetChild(i);
            tranOld[i].gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (tranOld != null && tranOld.Length > 0)
        {
            for (int i = 0; i < tranOld.Length; i++)
            {
                if (tranNew[i] != null)
                {
                    GameObject.DestroyImmediate(tranNew[i].gameObject);
                    tranNew[i] = null;
                }
            }

            for (int i = 0; i < tranOld.Length; i++)
            {
                GameObject obj = Object.Instantiate(tranOld[i].gameObject, tranOld[i].transform.position, tranOld[i].transform.rotation) as GameObject;
                tranNew[i] = obj.transform;
                tranNew[i].transform.parent = tranOld[i].transform.parent;
                tranNew[i].gameObject.SetActive(true);
            }
        }
    }
    
}

}

