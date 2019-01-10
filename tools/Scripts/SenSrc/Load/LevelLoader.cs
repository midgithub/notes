using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using SenLib;
using XLua;

[Hotfix]
public class LevelLoader : Loader
{
	AsyncOperation m_request = null;
    public static event Action<AsyncOperation,float> OnAsyncLoading;

    public LevelLoader()
		: base(Loader.LoaderType.SCENE)
	{
        
    }

	public override void Load()
	{
		base.Load();

		Util.Log ("LoadScene: " + m_path);

        if (m_async)
		{
			m_request = SceneManager.LoadSceneAsync(m_path);
            if(null != m_request)
            {
                m_request.allowSceneActivation = false;
            }
        }
		else
		{
			SceneManager.LoadScene(m_path);
			OnLoadCompleted(true);
		}
	}

	protected override void OnLoadProgress(float rate)
	{
		base.OnLoadProgress (rate);
        Util.Log("scene pro " + rate);

        if(null != OnAsyncLoading)
        {
            OnAsyncLoading(m_request, rate);
        }
    }

	public override void Update()
	{
		if (m_state == LoaderState.LOADING)
		{
            if (m_request == null)
            {
                OnLoadCompleted(false);
            }

            //if (m_request.isDone)  // stop at 0.9f
            //{
            //  OnLoadCompleted(true);
            //	m_request = null;
            //}

            if (m_request.progress < 0.90f)  
            {
				OnLoadProgress(m_request.progress);
            }
            else
            {
                OnLoadCompleted(true);
            }
		}
	}

	protected override void OnLoadCompleted (object data)
	{
		base.OnLoadCompleted (data);

        if (AppConst.UseAssetBundle)
        {
            string strABPath = "scenes/" + m_path;
            strABPath = FileHelper.CheckBundleName(strABPath);

            LoadedBundleCtrl.Instance.UnReferenceLoadedBundle(strABPath);
            LoadedBundleCtrl.Instance.HandRemoveBundle(strABPath);
        }
    }
}