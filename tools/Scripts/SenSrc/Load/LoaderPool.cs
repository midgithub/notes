using System.Collections;
using System.Collections.Generic;
using SenLib;
using XLua;

[Hotfix]
public class LoaderPool
{
    //单例模式.  
    #region
    private static LoaderPool _instance;
    public static LoaderPool Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = new LoaderPool();
            }

            return _instance;
        }
    }
    private LoaderPool() { }
    #endregion

    private List<BundleLoader> m_bundlePool = new List<BundleLoader>();  //不同类型在内存长度是不一样的
    private List<AssetLoader> m_assetPool = new List<AssetLoader>();
    private List<LevelLoader> m_scenePool = new List<LevelLoader>();

    public T GetLoader<T>() where T:Loader,new()
	{
		System.Type type = typeof(T);

		T loader = null;
		if(type == typeof(BundleLoader))
		{
			if(m_bundlePool.Count > 0)
			{
				loader = m_bundlePool[0] as T;
                m_bundlePool.RemoveAt(0);
			}
		}
        else if (type == typeof(AssetLoader))
        {
            if (m_assetPool.Count > 0)
            {
                loader = m_assetPool[0] as T;
                m_assetPool.RemoveAt(0);
            }
        }
        else if (type == typeof(LevelLoader))
        {
            if(m_scenePool.Count > 0)
			{
				loader = m_scenePool[0] as T;
                m_scenePool.RemoveAt(0);
			}
		}

        if(null == loader)
        {
            loader = new T();
        }

		return loader;
	}

	public void RecycleLoader(Loader loader)
	{
        if(null != loader)
        {
            loader.Reset();

            System.Type type = loader.GetType();
            if (type == typeof(BundleLoader))
            {
                m_bundlePool.Add((BundleLoader)loader);
            }
            else if (type == typeof(AssetLoader))
            {
                m_assetPool.Add((AssetLoader)loader);
            }
            else if (type == typeof(LevelLoader))
            {
                m_scenePool.Add((LevelLoader)loader);
            }
        }
    }
}
