using XLua;
﻿using SG;
using System;
using UnityEngine;

[Hotfix]
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
{
    private static T instance;

    protected SingletonMonoBehaviour()
    {
    }

    public virtual void Awake()
    {
        if (SingletonMonoBehaviour<T>.instance == null)
        {
            SingletonMonoBehaviour<T>.instance = ((SingletonMonoBehaviour<T>)this) as T;
            this.Init();
        }
        else
        {
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }

    public abstract void Init();
    public virtual void OnApplicationQuit()
    {
        SingletonMonoBehaviour<T>.instance = null;
    }

    protected virtual void OnDestroy()
    {
        if (SingletonMonoBehaviour<T>.instance == this)
        {
            LogMgr.Log("[SingletonMonoBehaviour] destory singleton. type=" + typeof(T).ToString());
            SingletonMonoBehaviour<T>.instance = null;
        }
    }

    public static T Instance
    {
        get
        {
            if (SingletonMonoBehaviour<T>.instance == null)
            {
                SingletonMonoBehaviour<T>.instance = UnityEngine.Object.FindObjectOfType(typeof(T)) as T;
                if (SingletonMonoBehaviour<T>.instance == null)
                {
                    //Debug.Log("Create Instance : " + typeof(T).ToString());
                    SingletonMonoBehaviour<T>.instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                }
            }
            return SingletonMonoBehaviour<T>.instance;
        }
    }

    public static bool IsInit
    {
        get
        {
            return (SingletonMonoBehaviour<T>.instance != null);
        }
    }
}


