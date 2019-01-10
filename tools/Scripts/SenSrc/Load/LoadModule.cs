using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SenLib;
using XLua;

[Hotfix]
public class LoadModule : MonoBehaviour
{
    public static LoadModule Instance {
        private set;
        get;
    }

    public enum AssetType
    {
        Asset = 0,
        Csv = 1,
        Txt = 2,
        Mat = 3,
        Xml = 4,
        AudioMp3 = 5,
        AudioWav = 6,
        GameObject = 7,
        Shader,
    }

    private enum ManifestType
    {
        UI_Bundles,
        Lua_Bundles,
        World_Bundles,
    }

    Dictionary<ManifestType, AssetBundleManifest> m_manifests = new Dictionary<ManifestType, AssetBundleManifest>();
    Dictionary<ManifestType, string> m_bundleHeads = new Dictionary<ManifestType, string>();
    List<string> m_bundleNames = new List<string>();
    List<string> m_manifestNames = new List<string>();

    // 加载队列
    List<Loader> m_loaders = new List<Loader>();      
    List<Loader> m_loaderQueue = new List<Loader>();  

    List<string> m_handLoad = new List<string>();   

    float m_lastClear = 0;	// 上一次清除时间

    void Awake()
    {
        Instance = this;
    }

    public void Init(LoadedCallback onInit)
	{
        if (AppConst.UseAssetBundle)
		{
            m_bundleNames.Clear();

            LoadManifest(ManifestType.Lua_Bundles);
            LoadManifest(ManifestType.UI_Bundles);
            LoadManifest(ManifestType.World_Bundles);

            Util.Log("LoadManifest manifest over---------");
        }

        FileHelper.AddBigBundleName("UI/Materials");
        LoadedBundleCtrl.Instance.SetPersistentBundles(new string[] { "shaders", "ui/atlas/common" });

        if (null != onInit)
        {
            onInit(null);
        }
    }

    // 加载资源清单
    void LoadManifest(ManifestType mType)
    {
        if (m_manifests.ContainsKey(mType) && m_manifests[mType] != null)
        {
            DestroyImmediate(m_manifests[mType], true);
            m_manifests.Remove(mType);
        }

        string manifestName = mType.ToString();

        Util.Log("LoadManifest manifestName " + manifestName);

        m_manifestNames.Add(manifestName);
        string strFullPath = FileHelper.SearchFilePath(GetManifestType(manifestName).ToString(), manifestName);
        Util.Log("strFullPath: " + strFullPath);

        BundleLoader bLoader = LoaderPool.Instance.GetLoader<BundleLoader>();
        bLoader.Init(strFullPath, manifestName, delegate (object data) {
            LoadedBundle ab = data as LoadedBundle;
            if (ab != null)
            {
                m_manifests[mType] = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                m_bundleHeads[mType] = mType.ToString().ToLower().Split('_')[0];

                Util.Log("manifest ab in not null over----");
            }
            else
            {
                Util.LogError("manifest ab is null---------");
            }

            LoadedBundleCtrl.Instance.UnReferenceLoadedBundle(manifestName);  // 不走统一接口是因为manifest文件没有后缀
            LoadedBundleCtrl.Instance.HandRemoveBundle(manifestName);

            if (m_manifests[mType] != null)
            {
                string[] bundles = m_manifests[mType].GetAllAssetBundles();

                for (int i = 0; i < bundles.Length; ++i)
                {
                    if (m_bundleNames.Contains(bundles[i]))
                    {
                        Util.LogError(string.Format("别的清单已同名的包名，{0} {1}", manifestName, bundles[i]));
                    }
                    m_bundleNames.Add(bundles[i]);
                }
            }
        }, false, null, true);
    }

    void Update()
	{
		for(int i=m_loaders.Count-1; i>=0; i--)
		{
			Loader loader = m_loaders [i];
			loader.Update();
			if(loader.IsFinish)
			{
				m_loaders.RemoveAt (i);
				LoaderPool.Instance.RecycleLoader (loader);
			}
		}

		int remain = AppConst.SyncCount - m_loaders.Count;
		if(remain>m_loaderQueue.Count)
		{
			remain = m_loaderQueue.Count;
		}
		for(int i=0; i<remain; i++)
		{
			Loader loader = m_loaderQueue [0];
			m_loaderQueue.RemoveAt (0);

			m_loaders.Add (loader);
			loader.Load ();
			loader.Update();
		}

		UpdateAssetBundleCache ();
	}

    /// <summary>
    /// 定时清理未引用的AssetBundle缓存
    /// </summary>
    void UpdateAssetBundleCache()
    {
        if (AppConst.AssetCacheTime == 0 || Time.realtimeSinceStartup - m_lastClear < AppConst.AssetCacheTime)
        {
            return;
        }

        m_lastClear = Time.realtimeSinceStartup;
        LoadedBundleCtrl.Instance.ClearNoneRefBundle(true);
    }

    /// <summary>
    /// 只在场景切换过度的loading场景调用
    /// </summary>
	public void Clear()
	{
        if (AppConst.UseAssetBundle)
        {
            for (int i = 0; i < m_handLoad.Count; i++)
            {
                LoadedBundleCtrl.Instance.UnReferenceLoadedBundle(m_handLoad[i]);
            }
            m_handLoad.Clear();
            LoadedBundleCtrl.Instance.ClearNoneRefBundle(false);
        }
		
		Resources.UnloadUnusedAssets();  
        GC.Collect();  //不调用内存不会立即被释放
    }

    #region 加载资源
    public GameObject LoadPrefab(string strPath)
    {
        if (string.IsNullOrEmpty(strPath))
        {
            Util.LogWarning("资源路径不能为空---");
            return null;
        }
        string[] items = strPath.Split('/');
        string name = items[items.Length - 1];

        GameObject go = null;
        LoadAssetFromBundle(strPath, name, typeof(GameObject), delegate (object data) {
            go = (data as GameObject);

        }, false);

        return go;
    }

    public void LoadPrefab(string strPath, LoadedCallback onLoaded)
    {
        if (string.IsNullOrEmpty(strPath))
        {
            Util.LogWarning("资源路径不能为空---");
            return;
        }

        string[] items = strPath.Split('/');
        string name = items[items.Length - 1];
        LoadAssetFromBundle(strPath, name, typeof(GameObject), onLoaded, true);
    }

	public void LoadScene(string name, LoadedCallback onLoaded, bool async, bool enter = true)
	{
        if (string.IsNullOrEmpty(name))
        {
            Util.LogWarning("资源路径不能为空---");
            return;
        }
        string sceneName = name.Substring(name.LastIndexOf("/") + 1);

        if (AppConst.UseAssetBundle)
		{
			string abPath = "Scenes/" + sceneName;
			LoadAssetBundle (abPath, delegate (object data){
				if(data==null)
				{
                    Util.LogError ("Load Scene Bundle Fail, name: " + sceneName);
                    if (onLoaded != null)
                    {
                        onLoaded(data);
                    }

					return;
				}
                if (enter)
                {
                    __LoadScene(sceneName, onLoaded, async);
                }
                else
                {
                    string abName = FileHelper.CheckBundleName(abPath);
                    if (!m_handLoad.Contains(abName))
                    {
                        m_handLoad.Add(abName);
                    }

                    if (onLoaded != null)
                    {
                        onLoaded(data);
                    }
                }
			}, async);
		}
		else
		{
            if (enter)
            {
                __LoadScene(sceneName, onLoaded, async);
            }else
            {
                if (onLoaded != null)
                {
                    onLoaded(null);
                }
            }
        }
	}

    /// <summary>
    /// 场景包加载完真正异步加载场景
    /// </summary>
    /// <param name="name"></param>
    /// <param name="onLoaded"></param>
	private void __LoadScene(string name, LoadedCallback onLoaded, bool async)
	{
        LevelLoader sLoader = LoaderPool.Instance.GetLoader<LevelLoader> ();
		sLoader.Init (name, onLoaded, async);
		StartLoad (sLoader, async);
	}

    public TextAsset LoadTextAsset(string strPath, LoadModule.AssetType assetType)
    {
        if (string.IsNullOrEmpty(strPath))
        {
            Util.LogWarning("资源路径不能为空---");
            return null;
        }
        string[] items = strPath.Split('/');
        string name = items[items.Length - 1];

        TextAsset text = null;
        LoadAssetFromBundle(strPath, name, typeof(TextAsset), delegate (object data) {
            text = (data as TextAsset);
        }, false, assetType);

        return text;
    }

    public Material LoadMaterial(string strPath)
    {
        if (string.IsNullOrEmpty(strPath))
        {
            Util.LogWarning("资源路径不能为空---");
            return null;
        }

        string[] items = strPath.Split('/');
        string name = items[items.Length - 1];

        Material mat = null;
        LoadAssetFromBundle(strPath, name, typeof(Material), delegate (object data) {
            mat = (data as Material);
        }, false, LoadModule.AssetType.Mat);

        return mat;
    }

    public Shader LoadShader(string shaderName)
    {
        if (string.IsNullOrEmpty(shaderName))
        {
            Util.LogWarning("资源路径不能为空---");
            return null;
        }

        Shader shader = null;
        LoadAssetFromBundle("shaders", shaderName, typeof(Shader), delegate (object data) {
            shader = (data as Shader);
        }, false, LoadModule.AssetType.Shader);

        return shader;
    }

    public AudioClip LoadAudio(string strPath, LoadModule.AssetType assetType)
    {
        if (string.IsNullOrEmpty(strPath))
        {
            Util.LogWarning("资源路径不能为空---");
            return null;
        }

        string[] items = strPath.Split('/');
        string name = items[items.Length - 1];

        AudioClip audio = null;
        LoadAssetFromBundle(strPath, name, typeof(AudioClip), delegate (object data) {
            audio = (data as AudioClip);
        }, false, assetType);

        return audio;
    }

    #endregion

    /// <summary>
    /// 手动从Bundle中加载资源唯一入口 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="onLoaded"></param>
    /// <param name="async"></param>
    private void LoadAssetFromBundle(string path, string assetName, System.Type type, LoadedCallback onLoaded, bool async, AssetType assetType = AssetType.GameObject)
	{
		if(AppConst.UseAssetBundle)
		{
            Util.Log(string.Format("LoadAssetFromBundle, bundle:{0}, asset:{1}", path, assetName));

            LoadAssetBundle (path, (data)=>{
                LoadedBundle abCache = data as LoadedBundle;
				object asset = null;
				if(abCache != null)
				{
                    string abName = FileHelper.CheckBundleName(path);
                    LoadedBundleCtrl.Instance.UnReferenceLoadedBundle(abName, true);

                    if (!m_handLoad.Contains(abName))
                    {
                        m_handLoad.Add(abName);
                    }

                    if (!string.IsNullOrEmpty (assetName))
					{
						asset = abCache.LoadAsset (assetName, type);
                        if (type != null && type == typeof(GameObject))
                        {
                            abCache.UnloadSelf();
                        }
                    }
					else
					{
						asset = abCache.Bundle;
					}
				}

				if(asset == null)
				{
                    Util.LogError(string.Format("LoadAssetFromBundle, path:{0},name:{1}, asset is null", path, assetName));
				}

				if (onLoaded != null)
				{
					onLoaded(asset);
				}
			}, async);
		}
		else 	
		{
			LoadAsset(path, onLoaded, type, false, assetType);
		}
	}

    public void LoadAsset(string path, LoadedCallback onLoaded, System.Type type = null, bool async = true, LoadModule.AssetType assetType = LoadModule.AssetType.GameObject)
	{
        string ext = GetExtOfAsset(assetType);
        string fullPath = string.Format("Assets/{0}{1}{2}", AppConst.ResDataDir, path, ext);
        if (!FileHelper.CheckFileExist(fullPath))
		{
            SG.LogMgr.UnityError(string.Format("Load Asset, Path:[ {0} ] not exist!  ", fullPath));
			if(onLoaded!=null)
			{
				onLoaded (null);
			}
			return;
		}

		AssetLoader aLoader = LoaderPool.Instance.GetLoader<AssetLoader> ();
		aLoader.Init (fullPath, type, onLoaded, async);
		StartLoad (aLoader, async);
	}

    public void StartLoad(Loader loader, bool async)
    {
        if (async)
        {
            m_loaderQueue.Add(loader);
        }
        else
        {
            m_loaders.Add(loader);
            loader.Load();
        }
    }

    public BundleLoader.BundleState LoadAssetBundle(string path, LoadedCallback onLoaded, bool async = true, BundleLoader parent = null)
	{
		string abName = FileHelper.CheckBundleName(path);
        if (!ManifestHasBundleInfo(abName))  
        {
            if (onLoaded != null)
            {
                onLoaded(null);
            }
            return BundleLoader.BundleState.NotExist;
        }

        LoadedBundle loadedCache = LoadedBundleCtrl.Instance.ReferenceLoadedBundle(abName);
        if (loadedCache != null)
        {
            if (onLoaded != null)
            {
                onLoaded(loadedCache);
            }
            return BundleLoader.BundleState.Loaded;
        }

        BundleLoader loadingCache = BundleLoaderCtrl.Instance.GetBundleLoader(abName);
        if (loadingCache != null)
        {
            loadingCache.AddHandleCallBack(onLoaded);
            loadingCache.AddParent(parent);
            return BundleLoader.BundleState.Loading;
        }

		string fullpath = FileHelper.SearchFilePath(GetManifestType(abName).ToString(), abName);   //包最终加载路径

        BundleLoader bLoader = LoaderPool.Instance.GetLoader<BundleLoader>();
        bLoader.Init(fullpath, abName, onLoaded, async, parent);

        return BundleLoader.BundleState.JustLoad;
    }

	public bool ManifestHasBundleInfo(string abName)
	{
        string name = abName.ToLower();
        if(m_bundleNames.Contains(name) || m_manifestNames.Contains(abName))
        {
            return true;
        }
        Util.LogError("清单中无该bundle: " + name);
        return false;
	}

	private string GetExtOfAsset(LoadModule.AssetType assetType)
    {
        if (assetType == LoadModule.AssetType.Asset)
        {
            return ".asset";
        }
        else if (assetType == LoadModule.AssetType.Csv)
        {
            return ".csv";
        }
        else if (assetType == LoadModule.AssetType.Txt)
        {
            return ".txt";
        }
        else if (assetType == LoadModule.AssetType.Mat)
        {
            return ".mat";
        }
        else if (assetType == LoadModule.AssetType.Xml)
        {
            return ".xml";
        }
        else if (assetType == LoadModule.AssetType.AudioMp3)
        {
            return ".mp3";
        }
        else if (assetType == LoadModule.AssetType.AudioWav)
        {
            return ".wav";
        }
        else if (assetType == LoadModule.AssetType.GameObject)
        {
            return ".prefab";
        }

        return "";
	}

    private ManifestType GetManifestType(string abName) 
    {
        if (abName.Equals(ManifestType.UI_Bundles.ToString()))
        {
            return ManifestType.UI_Bundles;
        }
        else if (abName.Equals(ManifestType.Lua_Bundles.ToString()))
        {
            return ManifestType.Lua_Bundles;
        }
        else if (abName.Equals(ManifestType.World_Bundles.ToString()))
        {
            return ManifestType.World_Bundles;
        }
        else
        {
            string[] items = abName.Split('/');
            string manifestName = items[0];

            if (manifestName.StartsWith("data") || manifestName.StartsWith("lua"))
            {
                manifestName = "lua";
            }

            foreach (var item in m_bundleHeads)
            {
                if (manifestName.Equals(item.Value))
                {
                    return item.Key;
                }
            }

            return ManifestType.World_Bundles;
        }
    }

    public string[] GetDependencies(string abName)
    {
        ManifestType type = GetManifestType(abName);

        AssetBundleManifest manifest;
        m_manifests.TryGetValue(type, out manifest);
        if (null == manifest)
        {
            Util.LogError("获取依赖包名失败，清单为空 : " + abName);
        }

        return manifest.GetDirectDependencies(abName);
    }
}