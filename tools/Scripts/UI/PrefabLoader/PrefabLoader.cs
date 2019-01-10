using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;
using Game.uGUI;

[Hotfix]
public class PrefabLoader : MonoBehaviour
{
    public string m_Path;
    /// <summary>
    /// LoadedObject 为特效时，调整粒子层
    /// </summary>
    public int order = 0;

    [HideInInspector]
    public GameObject LoadedObject;

    void Awake()
    {
        if (LoadedObject != null)
        {
            return;
        }
        GameObject prefab = (GameObject)CoreEntry.gResLoader.LoadResource(m_Path);
        if (prefab == null)
        {
            return;
        }
        LoadedObject = NGUITools.AddChild(gameObject, prefab);
        if(order > 0)
        {
            if(LoadedObject.GetComponent<UISortingOrder>() != null)
            {
                LoadedObject.GetComponent<UISortingOrder>().SetOrder(order, false);
            }
        }
    }
}

