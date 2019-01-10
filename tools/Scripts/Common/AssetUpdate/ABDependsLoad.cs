/**
* @file     : ABDependsLoad
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-10-14
*/
using XLua;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SG;

namespace Bundle
{
    public delegate void ABDependDelegate(AssetBundleData data);
    
[Hotfix]
    public class ABDependsLoad
    {
        private AssetBundleManifest streamManifest;
        private HashSet<string> streamABSet = new HashSet<string>();
        //private Dictionary<string, AssetBundleData> streamLoadDict = new Dictionary<string, AssetBundleData>();

        private AssetBundleManifest abManifest;
        private HashSet<string> allABSet = new HashSet<string>();
        private Dictionary<string, AssetBundleData> abLoadDict = new Dictionary<string,AssetBundleData>();

        private float releaseTime = 60f;
        private float maxStayTime = 300f;

        public ABDependsLoad()
        {
            UpdateABManifest();

            MonoInstance.Instance.StartCoroutine(ReleaseUnusedAssetBundle());
        }

        public void UpdateABManifest()
        {
            streamABSet.Clear();
            string streamBasePath = Application.persistentDataPath + "/" + BundleCommon.ResVersion + "/Inner/assetbundle";
            AssetBundle streamBundle = null;
            if (File.Exists(streamBasePath))
            {
                streamBundle = AssetBundle.LoadFromFile(streamBasePath);
            }
            if (null == streamBundle)
            {
                //LogMgr.UnityError("load bunle fail at path:" + streamBasePath);
            }
            else
            {
                streamManifest = streamBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
                if (null == streamManifest)
                {
                    //LogMgr.UnityError("load manifest fail at path:" + streamBasePath);
                }
                else
                {
                    string[] streamAbs = streamManifest.GetAllAssetBundles();
                    for (int i = 0; i < streamAbs.Length; i++)
                    {
                        streamABSet.Add(streamAbs[i]);
                    }
                }

                streamBundle.Unload(false);
            }

            allABSet.Clear();
            string abBasePath = Application.persistentDataPath + "/" + BundleCommon.ResVersion + "/Out/assetbundle";
            AssetBundle bundle = null;
            if (!File.Exists(abBasePath))
            {
                //LogMgr.UnityError("no bundle file at path:" + abBasePath);
            }
            else
            {
                bundle = AssetBundle.LoadFromFile(abBasePath);
                if (null == bundle)
                {
                    //LogMgr.UnityError("load bunle fail at path:" + abBasePath);
                }
                else
                {
                    abManifest = bundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
                    if (null == abManifest)
                    {
                        //LogMgr.UnityError("load manifest fail at path:" + abBasePath);
                    }
                    else
                    {
                        string[] abs = abManifest.GetAllAssetBundles();
                        for (int i = 0; i < abs.Length; i++)
                        {
                            allABSet.Add(abs[i]);
                        }
                    }

                    bundle.Unload(false);
                }
            }
        }

        /// <summary>
        /// ��ȡAssetBundle
        /// </summary>
        /// <param name="bundlePath"></param>
        /// <returns></returns>
        public AssetBundle GetAssetBundle(string bundlePath)
        {
            AssetBundleData abData = null;
            if (!abLoadDict.TryGetValue(bundlePath, out abData))
            {
                abData = LoadAssetBundle(bundlePath);
            }

            UpdateUsedTime(bundlePath);

            AssetBundle bundle = null;
            if (null != abData)
            {
                bundle = abData.mAssetBundle;
            }

            return bundle;
        }

        /// <summary>
        /// ����AssetBundle
        /// </summary>
        /// <param name="bundlePath"></param>
        /// <returns></returns>
        private AssetBundleData LoadAssetBundle(string bundlePath)
        {
            AssetBundleData abData = null;
            string allPath = string.Format("{0}/{1}/Out/{2}", Application.persistentDataPath, BundleCommon.ResVersion, bundlePath);
            string streamPath = string.Format("{0}/{1}/Inner/{2}", Application.persistentDataPath, BundleCommon.ResVersion, bundlePath);

            string dictKey = string.Format("/{0}", bundlePath);
            ABFileState state = ABFileMgr.Instance.GetABFileState(dictKey);

            if (allABSet.Contains(bundlePath) && state == ABFileState.DOWNLOADED)
            {
                abData = new AssetBundleData(bundlePath);
                string[] allDepends = abManifest.GetAllDependencies(bundlePath);
                abData.mDependABs = allDepends;

                for (int i = 0; i < allDepends.Length; i++)
                {
                    string dpPath = allDepends[i];
                    if (!abLoadDict.ContainsKey(dpPath))
                    {
                        dictKey = string.Format("/{0}", dpPath);
                        state = ABFileMgr.Instance.GetABFileState(dictKey);
                        string filePath = string.Format("{0}/{1}/Out/{2}", Application.persistentDataPath, BundleCommon.ResVersion, dpPath);
                        AssetBundle dpAB = null;
                        if (state == ABFileState.DOWNLOADED)
                        {
                            dpAB = AssetBundle.LoadFromFile(filePath);
                        }
                        else if (state == ABFileState.STREAMEXIT)
                        {
                            return null;

#pragma warning disable 0162
                            //
                            filePath = string.Format("{0}/{1}/Inner/{2}", Application.persistentDataPath, BundleCommon.ResVersion,dpPath);
                            dpAB = AssetBundle.LoadFromFile(filePath);
                        }
                        else
                        {
                            return null;
                        }

                        AssetBundleData dpData = new AssetBundleData(dpPath);
                        dpData.mAssetBundle = dpAB;

                        abLoadDict[dpPath] = dpData;
                    }

                    UpdateUsedTime(dpPath);
                }

                AssetBundle bundle = AssetBundle.LoadFromFile(allPath);
                abData.mAssetBundle = bundle;

                abLoadDict[bundlePath] = abData;
            }
            else if (streamABSet.Contains(bundlePath) && state == ABFileState.STREAMEXIT)
            {
                return null;

                //
                abData = new AssetBundleData(bundlePath);
                string[] allDepends = streamManifest.GetAllDependencies(bundlePath);
                abData.mDependABs = allDepends;

                for (int i = 0; i < allDepends.Length; i++)
                {
                    string dpPath = allDepends[i];
                    if (!abLoadDict.ContainsKey(dpPath))
                    {
                        dictKey = string.Format("/{0}", dpPath);
                        state = ABFileMgr.Instance.GetABFileState(dictKey);
                        string filePath = string.Format("{0}/{1}/Out/{2}", Application.persistentDataPath, BundleCommon.ResVersion, dpPath);
                        AssetBundle dpAB = null;
                        if (state == ABFileState.DOWNLOADED)
                        {
                            dpAB = AssetBundle.LoadFromFile(filePath);
                        }
                        else if (state == ABFileState.STREAMEXIT)
                        {
                            filePath = string.Format("{0}/{1}/Inner/{2}", Application.persistentDataPath, BundleCommon.ResVersion, dpPath);
                            dpAB = AssetBundle.LoadFromFile(filePath);
                        }
                        else
                        {
                            return null;
                        }

                        AssetBundleData dpData = new AssetBundleData(dpPath);
                        dpData.mAssetBundle = dpAB;

                        abLoadDict[dpPath] = dpData;
                    }

                    UpdateUsedTime(dpPath);
                }

                AssetBundle bundle = AssetBundle.LoadFromFile(streamPath);
                abData.mAssetBundle = bundle;

                abLoadDict[bundlePath] = abData;
            }

            return abData;
        }

        public IEnumerator GetAssetBundleAsync(string bundlePath, AssetBundleAsyncDelegate callback = null)
        {
            AssetBundleData abData = null;
            if (!abLoadDict.TryGetValue(bundlePath, out abData))
            {
                ABDependDelegate dpCallback = (data) =>
                {
                    abData = data;
                };

                yield return LoadAssetBundleAsync(bundlePath, dpCallback);
            }

            UpdateUsedTime(bundlePath);

            AssetBundle bundle = null;
            if (null != abData)
            {
                bundle = abData.mAssetBundle;
            }

            if (null != callback)
            {
                callback(true, bundle);
            }
        }

        private IEnumerator LoadAssetBundleAsync(string bundlePath, ABDependDelegate callback = null)
        {
            string dictKey = string.Format("/{0}", bundlePath);
            LogMgr.UnityLog("ABDependsLoad LoadAssetBundleAsync dictKey " + dictKey);
            ABFileState state = ABFileMgr.Instance.GetABFileState(dictKey);
            string[] abDepends = null;
            List<string> unfinishedAB = new List<string>();
            HashSet<string> abSet = null;
            AssetBundleManifest manifest = null;
            Dictionary<string, ABFileState> allABStateDict = new Dictionary<string, ABFileState>();
            if (state == ABFileState.DOWNLOADED)
            {
                abSet = allABSet;
                manifest = abManifest;
                allABStateDict[dictKey] = ABFileState.DOWNLOADED;
            }
            else if (state == ABFileState.NONE)
            {
                abSet = allABSet;
                manifest = abManifest;
                allABStateDict[dictKey] = ABFileState.DOWNLOADED;

                unfinishedAB.Add(dictKey);
            }
            else if (state == ABFileState.STREAMEXIT)
            {
                abSet = streamABSet;
                manifest = streamManifest;
                allABStateDict[dictKey] = ABFileState.STREAMEXIT;
            }
            
            if (null == abSet || !abSet.Contains(bundlePath))
            {
                if (null != callback)
                {
                    callback(null);
                }

                yield break;
            }
            abDepends = manifest.GetAllDependencies(bundlePath);

            if(null == abDepends)
            {
                if (null != callback)
                {
                    callback(null);
                }

                yield break;
            }

            for (int i = 0; i < abDepends.Length; i++)
            {
                string dpPath = abDepends[i];
                dictKey = string.Format("/{0}", dpPath);
                if (!abLoadDict.ContainsKey(dpPath))
                {
                    state = ABFileMgr.Instance.GetABFileState(dictKey);
                    if (state == ABFileState.NONE)
                    {
                        allABStateDict[dictKey] = ABFileState.DOWNLOADED;

                        unfinishedAB.Add(dictKey);
                    }
                    else if (state == ABFileState.DOWNLOADED)
                    {
                        allABStateDict[dictKey] = ABFileState.DOWNLOADED;
                    }
                    else if (state == ABFileState.STREAMEXIT)
                    {
                        allABStateDict[dictKey] = ABFileState.DOWNLOADED;

                        ABFileMgr.Instance.SetABFileState(dictKey, ABFileState.DOWNLOADED);
                    }

                    if (!abSet.Contains(dpPath))
                    {
                        if (null != callback)
                        {
                            callback(null);
                        }

                        yield break;
                    }
                }
                else
                {
                    allABStateDict[dictKey] = ABFileState.DOWNLOADED;

                    UpdateUsedTime(dpPath);
                }
            }

            for (int i = 0; i < unfinishedAB.Count; i++)
            {
                AssetBundleConfig config = ABFileMgr.Instance.GetABConfig(unfinishedAB[i]);
                if(null == config)
                {
                    if (null != callback)
                    {
                        LogMgr.UnityError("no config " + unfinishedAB[i]);

                        callback(null);
                    }

                    yield break;
                }

                string remoteUrl = BundleCommon.RemoteUrl + config.RelativePath + "_Compress";
                string saveFile = Application.persistentDataPath + "/" + BundleCommon.ResVersion + "/Out" + config.RelativePath + "_Compress";
                LogMgr.UnityLog("ABDependsLoad remoteUrl " + remoteUrl + "," + saveFile);
                CoreEntry.gHttpDownloadMgr.PushDownloadFile(remoteUrl, saveFile, config, null);
            }

            while (unfinishedAB.Count > 0)
            {
                yield return new WaitForSeconds(0.2f);

                for (int i = 0; i < unfinishedAB.Count; i++)
                {
                    state = ABFileMgr.Instance.GetABFileState(unfinishedAB[i]);
                    if (state == ABFileState.DOWNLOADED)
                    {
                        unfinishedAB.RemoveAt(i);

                        i--;
                    }
                }
            }

            string bundleFile = null;
            for (int i = 0; i < abDepends.Length; i++)
            {
                string dpPath = abDepends[i];
                dictKey = string.Format("/{0}", dpPath);
                if (allABStateDict.ContainsKey(dictKey))
                {
                    if (!abLoadDict.ContainsKey(dpPath))
                    {
                        state = allABStateDict[dictKey];
                        if (state == ABFileState.DOWNLOADED)
                        {
                            bundleFile = string.Format("{0}/{1}/Out/{2}", Application.persistentDataPath, BundleCommon.ResVersion, dpPath);
                        }
                        else
                        {
                            if (null != callback)
                            {
                                LogMgr.UnityError("state error:" + dpPath);

                                callback(null);
                            }

                            yield break;
                        }

                        AssetBundle dpAB = AssetBundle.LoadFromFile(bundleFile);
                        AssetBundleData dpData = new AssetBundleData(dpPath);
                        dpData.mAssetBundle = dpAB;

                        abLoadDict[dpPath] = dpData;
                    }

                    UpdateUsedTime(dpPath);
                }
                else
                {
                    if (null != callback)
                    {
                        LogMgr.UnityError("no in  allABStateDict:" + dpPath);

                        callback(null);
                    }

                    yield break;
                }
            }

            dictKey = string.Format("/{0}", bundlePath);
            state = allABStateDict[dictKey];
            if (state == ABFileState.DOWNLOADED)
            {
                bundleFile = string.Format("{0}/{1}/Out/{2}", Application.persistentDataPath, BundleCommon.ResVersion, bundlePath);
            }
            else
            {
                if (null != callback)
                {
                    LogMgr.UnityError("state error:" + bundlePath);

                    callback(null);
                }

                yield break;
            }
            AssetBundleData abData = new AssetBundleData(bundlePath);
            AssetBundle bundle = AssetBundle.LoadFromFile(bundleFile);
            abData.mAssetBundle = bundle;

            abLoadDict[bundlePath] = abData;

            if (null != callback)
            {
                callback(abData);
            }
        }

        /// <summary>
        /// ����ABʹ��ʱ��
        /// </summary>
        /// <param name="bundleName"></param>
        private void UpdateUsedTime(string bundleName)
        {
            AssetBundleData abData = null;
            if (abLoadDict.TryGetValue(bundleName, out abData))
            {
                abData.UpdateUsedTime();
                if (abData.mDependABs != null)
                {
                    for (int i = 0; i < abData.mDependABs.Length; i++)
                    {
                        string dp = abData.mDependABs[i];
                        if (abLoadDict.ContainsKey(dp))
                        {
                            abLoadDict[dp].UpdateUsedTime();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// �ͷ�AssetBundle
        /// </summary>
        /// <param name="bundleName"></param>
        public void ReleaseAssetBundle(string bundleName)
        {
            if (abLoadDict.ContainsKey(bundleName))
            {
                AssetBundleData abData = abLoadDict[bundleName];
                abData.Release();

                abLoadDict.Remove(bundleName);
            }
        }

        /// <summary>
        /// �ͷ����е�AssetBundle
        /// </summary>
        public void ReleaseAllAssetBundle()
        {
            foreach(AssetBundleData abData in abLoadDict.Values)
            {
                abData.Release();
            }
            abLoadDict.Clear();

            Resources.UnloadUnusedAssets();
        }

        private List<AssetBundleData> abReleaseList = new List<AssetBundleData>();
        /// <summary>
        /// ��ʱ����δʹ����Դ
        /// </summary>
        /// <returns></returns>
        IEnumerator ReleaseUnusedAssetBundle()
        {
            while (true)
            {
                yield return new WaitForSeconds(releaseTime);

                abReleaseList.Clear();
                foreach (AssetBundleData abData in abLoadDict.Values)
                {
                    float curTime = Time.realtimeSinceStartup - abData.mLastUsedTime;
                    if (curTime > maxStayTime)
                    {
                        abReleaseList.Add(abData);
                    }
                }

                if (abReleaseList.Count > 0)
                {
                    for (int i = 0; i < abReleaseList.Count; i++)
                    {
                        AssetBundleData abData = abReleaseList[i];
                        abData.Release();

                        abLoadDict.Remove(abData.mPath);
                    }

                    Resources.UnloadUnusedAssets();
                }
            }
        }

        public bool NeedDownload(string bundlePath)
        {


            return true;
        }
    }

[Hotfix]
    public class AssetBundleData
    {
        public string mPath;
        public AssetBundle mAssetBundle;
        public float mLastUsedTime;
        public string[] mDependABs;

        public AssetBundleData(string abPath)
        {
            mPath = abPath;

            UpdateUsedTime();
        }

        public void UpdateUsedTime()
        {
            mLastUsedTime = Time.realtimeSinceStartup;
        }

        public void Release()
        {
            if (null != mAssetBundle)
            {
                mAssetBundle.Unload(false);
            }
            mAssetBundle = null;
            mDependABs = null;
        }
    }
}

#pragma warning disable 0162

