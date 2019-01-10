using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[Hotfix]
public class BundleLoaderCtrl
{
    //单例模式. 
    #region 
    private static BundleLoaderCtrl _instance;
    public static BundleLoaderCtrl Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = new BundleLoaderCtrl();
            }

            return _instance;
        }
    }
    private BundleLoaderCtrl() { }
    #endregion

    private Dictionary<string, BundleLoader> m_BundleLoaders = new Dictionary<string, BundleLoader>();  //记录所有加载中的包

    public void AddBundleLoader(string abName, BundleLoader loader)
    {
        if (m_BundleLoaders.ContainsKey(abName))
        {
            Util.LogError("包已经在加载中了，你又重复加载了，包名：" + abName);
        }

        m_BundleLoaders.Add(abName, loader);
    }

    public void RemoveBundleLoader(string abName)
    {
        Util.Log("RemoveBundleLoader---- " + abName);
        if (!m_BundleLoaders.ContainsKey(abName))
        {
            Util.LogError("移出失败，包不在在加载队列中，包名：" + abName);
        }

        m_BundleLoaders.Remove(abName);
    }

    public BundleLoader GetBundleLoader(string abName)
    {
        BundleLoader cache = null;
        m_BundleLoaders.TryGetValue(abName, out cache);

        return cache;
    }
}
