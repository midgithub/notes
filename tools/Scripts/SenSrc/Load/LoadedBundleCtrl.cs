using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[Hotfix]
public class LoadedBundleCtrl
{
    //单例模式.  
    #region
    private static LoadedBundleCtrl _instance;
    public static LoadedBundleCtrl Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = new LoadedBundleCtrl();
            }

            return _instance;
        }
    }
    private LoadedBundleCtrl() { }
    #endregion

    Dictionary<string, LoadedBundle> m_loadedBundles = new Dictionary<string, LoadedBundle>();  // 缓存队列
    HashSet<string> m_persistentBundles = new HashSet<string>();   //常驻包

    /// <summary>
    /// 设置需要常驻内存的包
    /// </summary>
    /// <param name="arrAB"></param>
    public void SetPersistentBundles(string[] arrAB)
    {
        m_persistentBundles.Clear();
        for (int i = 0; i < arrAB.Length; i++)
        {
            string strAB = FileHelper.CheckBundleName(arrAB[i]);
            m_persistentBundles.Add(strAB);
            if (m_loadedBundles.ContainsKey(strAB))
            {
                m_loadedBundles[strAB].Persistent = true;
            }
        }
    }

    /// <summary>
    /// 记录加载完包
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="bundle"></param>
    /// <param name="refCount"></param>
    /// <returns></returns>
    public LoadedBundle AddLoadedBundle(string abName, AssetBundle bundle, int refCount)
    {
        if (m_loadedBundles.ContainsKey(abName))
        {
            Util.LogWarning(string.Format("已经有同名的资源包加载完了: {0}，不应该出现该情况，将被新的覆盖", abName));
        }

        LoadedBundle cache = new LoadedBundle(abName, bundle, refCount);
        m_loadedBundles[abName] = cache;

        if (m_persistentBundles.Contains(abName))
        {
            cache.Persistent = true;
        }

        return cache;
    }

    /// <summary>
    /// 真正从内存卸载包（已判定不是常驻的并且为引用）
    /// </summary>
    /// <param name="abName"></param>
    private void RemoveLoadedBundle(string abName)
    {
        LoadedBundle cache = null;
        m_loadedBundles.TryGetValue(abName, out cache);
        if (cache != null)
        {
            cache.Unload();
            m_loadedBundles.Remove(abName);
        }
        else
        {
            Util.LogWarning(string.Format("移除加载完的包失败，不存在该包：{0}", abName));
        }
    }

    /// <summary>
    /// 引用包
    /// </summary>
    /// <param name="abName"></param>
    /// <returns></returns>
    public LoadedBundle ReferenceLoadedBundle(string abName)
    {
        LoadedBundle cache = null;
        m_loadedBundles.TryGetValue(abName, out cache);
        if (cache != null)
        {
            ++cache.ReferencedCount;
        }

        return cache;
    }

    /// <summary>
    /// 返回不为null 说明 包还没从内存清理（可能未被引用但是清理缓存时间还没到）
    /// </summary>
    /// <param name="abName"></param>
    /// <returns></returns>
    public LoadedBundle UnReferenceLoadedBundle(string abName, bool handLoad = false)
    {
        LoadedBundle cache = null;
        if (!m_loadedBundles.TryGetValue(abName, out cache))
        {
            Util.LogWarning("卸载包失败，不存在该包： " + abName);
            return null;
        }

        --cache.ReferencedCount;

        if(handLoad && 0 == cache.ReferencedCount)   
        {
            cache.ReferencedCount = 1;    
        }

        if ((AppConst.AssetCacheTime == 0) && cache.IsCanRemove)  //无缓存时间的直接删
        {
            RemoveLoadedBundle(abName);
            return null;
        }

        return cache;
    }

    /// <summary>
    ///  定时或切换场景时（loading场景）清除无引用的AssetBundle缓存 
    /// </summary>
    /// <param name="onlyTimeout"></param>
    public void ClearNoneRefBundle(bool onlyTimeout)
    {
        string[] abNames = new string[m_loadedBundles.Count];
        m_loadedBundles.Keys.CopyTo(abNames, 0);

        string abName = "";
        LoadedBundle item = null;
        for (int i = 0; i < abNames.Length; ++i)
        {
            abName = abNames[i];
            item = m_loadedBundles[abName];
            // 只清除引用计数为0的
            if (item.IsCanRemove && (!onlyTimeout || item.IsTimeOut))  //onlyTimeout：false不用自己缓存时间直接删除
            {
                RemoveLoadedBundle(abName);
            }
        }

        abNames = null;
    }

    /// <summary>
    /// 手动移除没被引用的包（等不及定时清理了:场景包等）
    /// </summary>
    /// <param name="abName"></param>
    public void HandRemoveBundle(string abName)
    {
        LoadedBundle cache = null;
        m_loadedBundles.TryGetValue(abName, out cache);
        if (null != cache)
        {
            if (cache.IsCanRemove)
            {
                cache.Unload();
                m_loadedBundles.Remove(abName);
            }
            else
            {
                Util.LogWarning("手动移除包失败，该包还被引用中： " + abName);
            }
        }
        else
        {
            Util.LogWarning("移除加载完的包失败，不存在该包： " + abName);
        }
    }
}
