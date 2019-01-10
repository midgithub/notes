using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class MonoInstance : MonoBehaviour {


    static MonoInstance instance;
    public static MonoInstance Instance { get{
        if(instance==null)
        {
            instance = new GameObject("MonoInstance").AddComponent<MonoInstance>();
        }
        return instance;
    }}

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        instance = null;
    }

}

