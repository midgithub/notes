using XLua;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;

using SG; 
[Hotfix]
public class BundleManager 
{

	public static string LocalLoadPath = Application.streamingAssetsPath;
    public static string FolderLoginUI = "Login";
    public static string FolderMainUI = "MainUI";
    public static string FolderCommonUI = "Tips";

	public static string UIFontName = "font.data";
	public static string UIBaseName =  "baseui.data";
	public static string UISoundName = "uisound.data";
    public static string UILoginName = "login.data";
    public static string UIMainName = "main.data";
    public static string UICommonName = "common.data";

	public static string PathUICommon = "/UI/CommonData";
	public static string PathUIPrefab = "/UI/Prefabs";
    public static string PathModelPrefab = "/Model";
    public static string PathEffectPrefab = "/Effect";
    public static string PathSoundPrefab = "/Sound";
    public static string PathAnimationAsset = "/Animation";
    public static string PathTableData = "/Data";

    public static string AppOutputPath = Application.streamingAssetsPath;
    public static string DevelopOutputPath = Application.dataPath + "/BundleAssets";
	public static string UpdateOutputPath = Application.dataPath + "/../Release/ResData/StreamingAssets";

    // 下载更新后存放的路径
    public static string LocalPathRoot
    {
        get
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return Application.persistentDataPath + "/ResData";
            }
            else
            {
                 return Application.dataPath + "/ResData";
            }
        }
    }


	private static string m_loadUrlHead = "file://";

    // 各种常驻内存容器
    private static List<AssetBundle> m_BundleListFont = new List<AssetBundle>();
    private static List<AssetBundle> m_BundleListCommon = new List<AssetBundle>();
    private static List<AssetBundle> m_BundleListLoginUI = new List<AssetBundle>();
    private static List<AssetBundle> m_BundleListMainUI = new List<AssetBundle>();

    private static List<string> m_BundleUIGroupLoadingList = new List<string>();
   
 #region UI Bundle
    // 存储捆绑打包类型，当主资源请求加载时加载Bundle
    private static Dictionary<string, AssetBundle> m_BundleDicUIGroup = new Dictionary<string, AssetBundle>();

    //public delegate void LoadBundleFinish(UIPathData uiData,GameObject retObj, object param1, object param2);

    public static string GetBundleLoadPath(string subFolder, string localName)
    {
        //lmjedit
        //if (PlatformHelper.IsEnableUpdate())
        {
            string localPath = LocalPathRoot + subFolder + "/" + localName;

            if (File.Exists(localPath))
            {
                return localPath;
            }
        }
        
#if UNITY_EDITOR
        return BundleManager.DevelopOutputPath + subFolder + "/" + localName;
#else
        return Application.streamingAssetsPath + subFolder + "/" + localName;
#endif
    }
    public static string GetBundleLoadUrl(string subFolder, string localName)
    {
        //return BundleManager.m_loadUrlHead + GetBundleLoadPath(subFolder, localName);
        return string.Format("{0}{1}", BundleManager.m_loadUrlHead, GetBundleLoadPath(subFolder, localName));
    }

    public static IEnumerator LoadLoginUI()
    {
        if (m_BundleListLoginUI.Count == 0)
        {
            WWW www = new WWW(GetBundleLoadUrl(BundleManager.PathUICommon, BundleManager.UILoginName));
            yield return www;
            if (null != www.assetBundle)
            {
                www.assetBundle.LoadAllAssets();
                m_BundleListLoginUI.Add(www.assetBundle);
            }
        }
    }

    public static IEnumerator LoadFontUI()
    {
        // 加载字体
        if (m_BundleListFont.Count == 0)
        {
            WWW www = new WWW(GetBundleLoadUrl(BundleManager.PathUICommon, BundleManager.UIFontName));
            yield return www;
            if (null != www.assetBundle)
            {
                www.assetBundle.LoadAllAssets();
                m_BundleListFont.Add(www.assetBundle);
            }
        }
    }

    public static IEnumerator LoadCommonUI()
    {
        // 加载commonui
        if (m_BundleListCommon.Count == 0)
        {
            WWW www = new WWW(GetBundleLoadUrl(BundleManager.PathUICommon, BundleManager.UICommonName));
            yield return www;
            if (null != www.assetBundle)
            {
                www.assetBundle.LoadAllAssets();
                m_BundleListCommon.Add(www.assetBundle);
            }
        }
    }

    public static IEnumerator LoadMainUI()
    {
        // 加载mainui
        if (m_BundleListMainUI.Count == 0)
        {
            WWW www = new WWW(GetBundleLoadUrl(BundleManager.PathUICommon, BundleManager.UIMainName));
            yield return www;
            if (null != www.assetBundle)
            {
                www.assetBundle.LoadAllAssets();
                m_BundleListMainUI.Add(www.assetBundle);
            }
        }
    }

    /*
    public static IEnumerator LoadUI(UIPathData uiData, LoadBundleFinish delFinish, object param1, object param2)
	{

        if (m_BundleListFont.Count == 0)
        {
            WWW www = new WWW(GetBundleLoadUrl(BundleManager.PathUICommon, BundleManager.UIFontName));
            yield return www;
            if (null != www.assetBundle)
            {
                www.assetBundle.LoadAll();
                m_BundleListFont.Add(www.assetBundle);
            }
        }

      
        // 加载通用资源
        if (m_BundleListCommon.Count == 0)
        {
            WWW www = new WWW(GetBundleLoadUrl(BundleManager.PathUICommon, BundleManager.UICommonName));
            yield return www;
            if (null != www.assetBundle)
            {
                www.assetBundle.LoadAll();
                m_BundleListCommon.Add(www.assetBundle);
            }
        }

        // 加载Login资源
        if (Application.loadedLevel == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_LOGIN)
        {
            if (m_BundleListLoginUI.Count == 0)
            {
                WWW www = new WWW(GetBundleLoadUrl(BundleManager.PathUICommon, BundleManager.UILoginName));
                yield return www;
                if (null != www.assetBundle)
                {
                    www.assetBundle.LoadAll();
                    m_BundleListLoginUI.Add(www.assetBundle);
                }
            }
        }
        else
        {
            if (m_BundleListMainUI.Count == 0)
            {
                WWW www = new WWW(GetBundleLoadUrl(BundleManager.PathUICommon, BundleManager.UIMainName));
                yield return www;
                if (null != www.assetBundle)
                {
                    www.assetBundle.LoadAll();
                    m_BundleListMainUI.Add(www.assetBundle);
                }
            }
        }
      
        // 加载当前UI

        GameObject retObj = null;
        WWW wwwUI;
        if (uiData.uiGroupName != null)
        {
            while (m_BundleUIGroupLoadingList.Contains(uiData.uiGroupName))
            {
                yield return null;
            }
            m_BundleUIGroupLoadingList.Add(uiData.uiGroupName);

            if (m_BundleDicUIGroup.ContainsKey(uiData.uiGroupName))
            {
                retObj = m_BundleDicUIGroup[uiData.uiGroupName].Load(uiData.name, typeof(GameObject)) as GameObject;
            }
            else
            {
                //LogModule.DebugLog("load asset :" + uiData.name + "UI gropu:" + uiData.uiGroupName);
                wwwUI = new WWW(GetBundleLoadUrl(BundleManager.PathUIPrefab, uiData.uiGroupName + ".data"));
                yield return wwwUI;
                if (null != wwwUI.assetBundle)
                {
                    retObj = wwwUI.assetBundle.Load(uiData.name, typeof(GameObject)) as GameObject;
                    m_BundleDicUIGroup.Add(uiData.uiGroupName, wwwUI.assetBundle);
                }
                else
                {
                    LogMgr.UnityLog("load assetbundle none :" + uiData.uiGroupName);
                }
            }

            m_BundleUIGroupLoadingList.Remove(uiData.uiGroupName);
        }
        else
        {
            wwwUI = new WWW(GetBundleLoadUrl(BundleManager.PathUIPrefab, uiData.name + ".data"));
            yield return wwwUI;

            if (null != wwwUI.assetBundle)
            {
                retObj = wwwUI.assetBundle.Load(uiData.name, typeof(GameObject)) as GameObject;
                wwwUI.assetBundle.Unload(false);
            }
            else
            {
                LogMgr.UnityLog("load assetbundle none :" + uiData.uiGroupName);
            }
        }
       
        if (null != delFinish) delFinish(uiData, retObj, param1, param2);
//          Resources.UnloadUnusedAssets();
//          GC.Collect();
	}
    //*/

    public static void ReleaseLoginBundle()
    {
        foreach (AssetBundle loginBundle in m_BundleListLoginUI)
        {
            loginBundle.Unload(true);
        }
        m_BundleListLoginUI.Clear();
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    //public static GameObject GetGroupUIItem(UIPathData data)
    //{
    //    if (null == data.uiGroupName)
    //    {
    //        return null;
    //    }
    //    if (m_BundleDicUIGroup.ContainsKey(data.uiGroupName))
    //    {
    //        return m_BundleDicUIGroup[data.uiGroupName].Load(data.name) as GameObject;
    //    }

    //    return null;
    //}

    public static void CleanGroupBundle(string groupName)
    {
        if (null == groupName)
        {
            return;
        }
        if (m_BundleDicUIGroup.ContainsKey(groupName))
        {
            m_BundleDicUIGroup[groupName].Unload(false);
            m_BundleDicUIGroup.Remove(groupName);
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }

    public static void ReleaseGroupBundle()
    {
        foreach (KeyValuePair<string, AssetBundle> loginBundle in m_BundleDicUIGroup)
        {
            loginBundle.Value.Unload(true);
        }
        m_BundleUIGroupLoadingList.Clear();
        m_BundleDicUIGroup.Clear();
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

#endregion
    // 单体资源缓存，如果有缓存，则不从文件加载
    private static Dictionary<string, GameObject> m_dicSingleBundleCache = new Dictionary<string, GameObject>();
    // 单体资源引用计数，如果超过1，则将此资源放入缓存
    private static Dictionary<string, int> m_dicSingleBundleRef = new Dictionary<string, int>();

    public static void ReleaseSingleBundle()
    {
        m_dicSingleBundleCache.Clear();
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    static void ChangeShader(Transform obj)
    {

        if (obj.GetComponent<Renderer>() != null && obj.GetComponent<Renderer>().material != null)
        {
            Material sm = obj.GetComponent<Renderer>().material;
            var shaderName = sm.shader.name;
            if (!string.IsNullOrEmpty(shaderName))
            {
                var newShader = CoreEntry.gResLoader.LoadShader(shaderName);
                if (newShader != null)
                {
                    sm.shader = newShader;
                }
                else
                {
                    LogMgr.UnityLog("unable to refresh shader: " + shaderName + " in material " + sm.name);
                }
            }
        }

        for (int i = 0; i < obj.childCount; i++)
        {
            ChangeShader(obj.transform.GetChild(i));
        }
    }

    public delegate void LoadSingleFinish(string modelName, GameObject resObj, object param1, object param2, object param3 = null);

    private static bool LoadFromCache(string bundlePath, string bundleName, LoadSingleFinish delFinish, object param1, object param2, object param3)
    {
        if (m_dicSingleBundleCache.ContainsKey(bundlePath))
        {
            if (null != delFinish) delFinish(bundleName, m_dicSingleBundleCache[bundlePath], param1, param2, param3);
            return true;
        }

        if (m_dicSingleBundleRef.ContainsKey(bundlePath))
        {
            m_dicSingleBundleRef[bundlePath]++;
        }
        else
        {
            m_dicSingleBundleRef.Add(bundlePath, 1);
        }

        return false;
    }

    private static void ProcessLoad(WWW www, string bundlePath, string bundleName, LoadSingleFinish delFinish, object param1, object param2, object param3)
    {
        GameObject retObj = null;
        if (null != www.assetBundle)
        {
            retObj = www.assetBundle.mainAsset as GameObject;
#if UNITY_EDITOR
            ChangeShader(retObj.transform);
#endif
            www.assetBundle.Unload(false);

            if (m_dicSingleBundleRef.ContainsKey(bundlePath) && m_dicSingleBundleRef[bundlePath] > 1)
            {
                m_dicSingleBundleCache.Add(bundlePath, retObj);
                m_dicSingleBundleRef.Remove(bundlePath);
            }
        }
        else
        {
            LogMgr.UnityLog("load single assetbundle none :" + bundleName);
        }


        if (null != delFinish) delFinish(bundleName, retObj, param1, param2, param3);
    }

    // 加载声音，不缓存
    private static void ProcessLoadSound(WWW www, string bundlePath, string bundleName, LoadSoundFinish delFinish, object param1, object param2, object param3)
    {
        AudioClip retObj = null;
        if (null != www.assetBundle)
        {
            retObj = www.assetBundle.mainAsset as AudioClip;
            www.assetBundle.Unload(false);
        }
        else
        {
            LogMgr.UnityLog("load single assetbundle none :" + bundleName);
        }

        if (null != delFinish) delFinish(bundleName, retObj, param1, param2, param3);
    }

    // 加载动作，不缓存
    private static void ProcessLoadAnimation(WWW www, string bundlePath, string bundleName, LoadAnimationFinish delFinish)
    {
        AnimationClip retObj = null;
        if (null != www.assetBundle)
        {
            retObj = www.assetBundle.mainAsset as AnimationClip;
            www.assetBundle.Unload(false);
        }
        else
        {
            LogMgr.UnityLog("load single assetbundle none :" + bundleName);
        }

        if (null != delFinish) delFinish(bundleName, retObj);
    }

    // 加载模型
    public static IEnumerator LoadModel(string modelName, LoadSingleFinish delFinish, object param1, object param2, object param3)
    {
        string loadPath = GetBundleLoadUrl(BundleManager.PathModelPrefab, modelName + ".data");
        if (!LoadFromCache(loadPath, modelName, delFinish, param1, param2, param3))
        {
            WWW www = new WWW(loadPath);
            yield return www;
            ProcessLoad(www, loadPath, modelName, delFinish, param1, param2, param3);
        }
    }
    
    // 加载特效
    public static IEnumerator LoadEffect(string modelName, LoadSingleFinish delFinish, object param1 = null, object param2 = null)
    {

        string loadPath = GetBundleLoadUrl(BundleManager.PathEffectPrefab, modelName + ".data");
        if (!LoadFromCache(loadPath, modelName, delFinish, param1, param2, null))
        {
            WWW www = new WWW(loadPath);
            yield return www;
            ProcessLoad(www, loadPath, modelName, delFinish, param1, param2, null);
        }
        
    }

    public delegate void LoadSoundFinish(string modelName, AudioClip audioClip, object param1, object param2, object param3 = null);
    // 加载声音
    public static IEnumerator LoadSound(string soundPath, LoadSoundFinish delFinish, object param1 = null, object param2 = null, object param3 = null)
    {
        // 表里路径包含了Sounds文件夹名称
        string loadPath = GetBundleLoadUrl("", soundPath + ".data");
        WWW www = new WWW(loadPath);
        yield return www;
        ProcessLoadSound(www, loadPath, soundPath, delFinish, param1, param2, param3);
    }

    public delegate void LoadAnimationFinish(string animPath, AnimationClip animClip);
    public static IEnumerator LoadAnimation(string animPath, LoadAnimationFinish delFinish)
    {
        string loadPath = GetBundleLoadUrl(BundleManager.PathAnimationAsset, animPath + ".data");
        WWW www = new WWW(loadPath);
        yield return www;
        ProcessLoadAnimation(www, loadPath, animPath, delFinish);
    }
}
