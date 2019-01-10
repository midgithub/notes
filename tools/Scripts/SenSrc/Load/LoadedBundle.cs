using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[Hotfix]
public class LoadedBundle{
    string m_abName;          // AssetBundle name
    int m_referencedCount;  // 引用计数
    float m_unloadTime;     // 释放时间

    Dictionary<string, Object> m_dicAssetCache = new Dictionary<string, Object>();

    public LoadedBundle(string abName, AssetBundle ab, int refCount)
    {
        m_abName = abName;
        Bundle = ab;
        m_referencedCount = refCount;
    }

    // AssetBundle
    public AssetBundle Bundle
    {
        get;
        private set;
    }

    // 是否常驻
    public bool Persistent
    {
        get;
        set;
    }

    // 引用计数
    public int ReferencedCount
    {
        get
        {
            return m_referencedCount;
        }

        set
        {
            m_referencedCount = value;
            m_unloadTime = Time.realtimeSinceStartup;
            if(m_referencedCount < 0)
            {
                Util.LogError(m_abName + " 引用计数为负   " + m_referencedCount);
            }
        }
    }

    
    /// <summary>
    /// 不常驻且没有被引用的可以被移除
    /// </summary>
    public bool IsCanRemove
    {
        get
        {
            if (!Persistent && ReferencedCount <= 0)
            {
                return true;
            }

            return false;
        }
    }

    // 缓存时间到
    public bool IsTimeOut
    {
        get
        {
            return Time.realtimeSinceStartup - m_unloadTime >= AppConst.AssetCacheTime;
        }
    }

    private int m_iAssetCount = 0;   //包里总的资源数量
    public int AssetCount
    {
        get
        {
            if (m_iAssetCount == 0 && Bundle != null)
            {
                m_iAssetCount = Bundle.GetAllAssetNames().Length;
            }
            return m_iAssetCount;
        }
    }

    //加载资源
    public Object LoadAsset(string name, System.Type type)
    {
        Object asset = null;
        if (!m_dicAssetCache.TryGetValue(name, out asset))
        {
            if (Bundle == null)
            {
                Util.LogError(string.Format("AssetBundle:{0} is null, load asset:{1},type:{2}, fail!!", m_abName, name, type.ToString()));
                return null;
            }

            asset = Bundle.LoadAsset(name, type);
            if (null == asset && (name.EndsWith(".lua") || name.EndsWith(".bin")))
            {
                string realName = name + ".txt";
                asset = Bundle.LoadAsset(realName, type);
            }
            m_dicAssetCache[name] = asset;
        }

        m_unloadTime = Time.realtimeSinceStartup;  //从新计时

        return asset;
    }

    //加载资源
    public T LoadAsset<T>(string name) where T : Object
    {
        Object objasset = null;
        T asset = null;
        if (!m_dicAssetCache.TryGetValue(name, out objasset))
        {
            if(null != Bundle)
            {
                asset = Bundle.LoadAsset<T>(name);
                m_dicAssetCache.Add(name, asset);
            }
        }
        else
        {
            asset = (T)objasset;
        }

        m_unloadTime = Time.realtimeSinceStartup;

        return asset;
    }

    //真正从内存卸载
    public void Unload()
    {
        Util.Log("Unload---- " + m_abName);
        if (Bundle != null)
        {
            Bundle.Unload(false);
            Bundle = null;
            m_iAssetCount = 0;

            UnloadDependencies();
        }else
        {
            Util.LogWarning("Bundle is null: " + m_abName);
            UnloadDependencies();
        }

        m_dicAssetCache.Clear();
    }

    // 卸载依赖
    private void UnloadDependencies()
    {
        string[] dependencies = LoadModule.Instance.GetDependencies(m_abName);
        for (int i = 0; i < dependencies.Length; ++i)
        {
            LoadedBundleCtrl.Instance.UnReferenceLoadedBundle(dependencies[i]);
        }
    }

    //卸载自己，不卸载依赖
    public void UnloadSelf()
    {
        if (Bundle == null || m_dicAssetCache.Count == 0 || Persistent)
        {
            return;
        }

        //资源大于1，并且没有全部载入内存，则不能卸载
        if (AssetCount > 1 && AssetCount > m_dicAssetCache.Count)
        {
            return;
        }

        Bundle.Unload(false);
        Bundle = null;
        m_iAssetCount = 0;

        Util.Log("=============UnloadSelf ab....: " + m_abName);
    }
}
