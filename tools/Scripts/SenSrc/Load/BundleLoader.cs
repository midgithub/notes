using UnityEngine;
using System.Collections.Generic;
using SenLib;
using XLua;

[Hotfix]
public class BundleLoader : Loader
{
    public enum BundleState
    {
        NotExist,
        Loading,
        Loaded,
        JustLoad,
    }

    private string m_abName;			

    AssetBundleCreateRequest m_abRequest = null;
	WWW m_wwwRequest = null;

	int m_stateCurrent = 0;
	int m_stageCount = 1;

    private List<BundleLoader> m_parents = new List<BundleLoader>(); 
    private List<string> m_childs = new List<string>();  //还在加载依赖的包
    private bool allChildsPassed = false;

    private LoadedCallback m_onRefLoaded;  //主动加载回调

    public BundleLoader ()
        : base (Loader.LoaderType.BUNDLE)
    {

    }

	public override void Reset ()
    {
        base.Reset ();

		m_stateCurrent = 0;
		m_stageCount = 1;

        m_parents.Clear();
        m_childs.Clear();
        allChildsPassed = false;
        m_onRefLoaded = null;

        m_abRequest = null;

		if (m_wwwRequest != null) {
			m_wwwRequest.Dispose ();
			m_wwwRequest = null;
		}
    }
    
	public void Init (string path, string abName, LoadedCallback refCallback, bool async = true, BundleLoader parent = null, bool manifest = false)
	{
		base.Init (path, null, async);  //null不使用父类回调
		Util.Log (string.Format("Init BundleLoader path:{0}, name:{1}", path, abName));

        m_abName = abName;
        AddHandleCallBack(refCallback);
        AddParent(parent);

        BundleLoaderCtrl.Instance.AddBundleLoader(abName, this);

        if (manifest)
        {
            StartLoad();
        }
        else
        {
            string[] dependencies = LoadModule.Instance.GetDependencies(abName);
            for (int i = 0; i < dependencies.Length; ++i)
            {
                m_childs.Add(dependencies[i]);

                if (i == dependencies.Length - 1)
                {
                    allChildsPassed = true;
                }

                BundleState state = LoadModule.Instance.LoadAssetBundle(dependencies[i], null, async, this);
                if (state == BundleState.NotExist || state == BundleState.Loaded)
                {
                    m_childs.Remove(dependencies[i]);
                }
            }

            if (m_childs.Count == 0 && m_state != LoaderState.FINISHED) //无依赖直接加载自己
            {
                StartLoad();
            }
        }
    }

    public void AddParent(BundleLoader parent)
    {
        if (null != parent)
        {
            m_parents.Add(parent);
        }
    }

    public void ChildLoadEnd(string abName)
    {
        if (string.IsNullOrEmpty(abName))
        {
            Util.LogWarning("移出依赖包失败，包名为空");
        }

        if (m_childs.Count > 0 && m_childs.Contains(abName))
        {
            m_childs.Remove(abName);
        }
        else
        {
            Util.LogWarning("移出刚加载完的依赖包失败，因为不存在该依赖包");
        }

        if (m_childs.Count == 0 && allChildsPassed)
        {
            StartLoad();
        }
    }

    /// <summary>
    /// 主动加载时有加载完回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddHandleCallBack(LoadedCallback callback)
	{
		if(callback != null)
		{
			if(m_onRefLoaded==null)
			{
				m_onRefLoaded = callback;
			}
			else
			{
				m_onRefLoaded += callback;
			}
		}
	}

    /// <summary>
    /// 通过依赖自己决定是否准备好可以加载（依赖包加载完或没依赖）
    /// </summary>
    private void StartLoad()
    {
        LoadModule.Instance.StartLoad(this, m_async);
    }

    public override void Load ()
	{
		base.Load ();
        if(string.IsNullOrEmpty(m_path) || m_path.Equals(""))
        {
            OnLoaded(null);
            Util.LogError("路径不能为空--------");
        }

//#if UNITY_EDITOR
        RecordeBundles.Instance.Recorde(m_path);
//#endif

        if (m_async) {
			bool useWWW = false;
#if UNITY_ANDROID && !UNITY_EDITOR
			useWWW = m_path.Contains (AppConst.AssetDir);
#endif
            if (useWWW) {
				string path = FileHelper.GetWWWPath (m_path);
				m_wwwRequest = new WWW (path);
			} else {
                string path = FileHelper.GetAPKPath(m_path);
                m_abRequest = AssetBundle.LoadFromFileAsync(path);
            }
		} else {
			AssetBundle ab = null;
			try {
				if (ab == null) {
                    string path = FileHelper.GetAPKPath(m_path);
                    ab = AssetBundle.LoadFromFile(path);
                }
			} catch (System.Exception e) {
                Util.LogError (e.Message);
			} finally {
				OnLoaded (ab);
			}
		}
	}

    public override void Update ()
	{
		if (m_state == LoaderState.LOADING) {
			if (m_abRequest != null) {
				if (m_abRequest.isDone) {
					++m_stateCurrent;
					OnLoaded (m_abRequest.assetBundle);
				} else {
					DoProgress (m_abRequest.progress);
				}
			}
			else {
				if (m_wwwRequest != null) {
					if (m_wwwRequest.isDone) {
						++m_stateCurrent;
						if (!string.IsNullOrEmpty (m_wwwRequest.error)) {
							Util.LogError (m_wwwRequest.error);
							OnLoaded (null);
						} else {
                            OnLoaded(m_wwwRequest.assetBundle);
                        }
					} else {
						DoProgress (m_wwwRequest.progress);
					}
				}
			}
		}
	}
		
	void DoProgress(float rate)
	{
		OnLoadProgress ((m_stateCurrent + rate) / m_stageCount);
	}

    //**注意代码顺序，不要随便修改流程可引发bug
    void OnLoaded (AssetBundle ab)
    {
        Util.Log("LoadAsset---- " + m_abName);
        if (null == ab)
        {
            Util.LogError("加载包失败 " + m_path);
        }
        Util.Log (string.Format("Load {0} - {1} use {2}ms", m_path, m_async, m_watch.Elapsed.Milliseconds));
    	
        OnLoadCompleted (ab);
        BundleLoaderCtrl.Instance.RemoveBundleLoader(m_abName);
       
        int refCount = 0;
        if (m_onRefLoaded != null)
        {
            refCount = m_onRefLoaded.GetInvocationList().Length;
        }
        refCount += m_parents.Count;

        LoadedBundle cache = null;
        if (null != ab)
        {
            cache = LoadedBundleCtrl.Instance.AddLoadedBundle(m_abName, ab, refCount);
        }

        if(m_onRefLoaded != null)
        {
            Util.Log("OnLoaded  m_onRefLoaded call---- ");
            m_onRefLoaded(cache);
        }

        for (int i = 0; i < m_parents.Count; i++)
        {
            m_parents[i].ChildLoadEnd(m_abName);
        }
        m_parents.Clear();
    }
}