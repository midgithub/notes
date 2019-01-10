using UnityEngine;
using SenLib;
using XLua;

[Hotfix]
public class AssetLoader : Loader
{
	Object m_data = null;
	System.Type m_assetType = null;		//资源类型

	public AssetLoader()
		: base(Loader.LoaderType.ASSET)
	{

	}

	public void Init (string path, System.Type type, LoadedCallback onLoaded, bool async = true)
	{
		base.Init (path, onLoaded, async);
		m_assetType = type;
	}

	public override void Load()
	{
		base.Load();

#if UNITY_EDITOR
		if (m_assetType == null)
		{
			m_assetType = typeof(Object);
		}

        Util.Log("LoadAsset: " + m_path);
        m_data = UnityEditor.AssetDatabase.LoadAssetAtPath(m_path, m_assetType);
        if (!m_async)
        {
            OnLoadCompleted(m_data);
        }
#else
		if(!m_async)
		{
			OnLoadCompleted(null);
		}
#endif
    }

	public override void Update()
	{
		if (m_state == LoaderState.LOADING)
		{
			OnLoadCompleted(m_data);
			m_data = null;
		}
	}
}