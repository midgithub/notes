using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SG;

namespace Bundle
{
    public delegate void ResourceAsyncDelegate(bool isBunlde, Object o);
    public delegate void AssetBundleAsyncDelegate(bool isBundle, AssetBundle assetbundle);
[Hotfix]
    public class AssetBundleLoadManager
    {
        static AssetBundleLoadManager instance;
        public static AssetBundleLoadManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new AssetBundleLoadManager();
                return instance;
            }
        }
        public static bool mLoadTestModeValue = false;
        public static bool mIsTestMode = true;
        public static bool IsTestMode
        {
            get
            {
                if(!mLoadTestModeValue)
                {
                    mIsTestMode = ResSetting.Instance.GetBoolValue("IsTestMode");
                    mLoadTestModeValue = true;
                }
                return mIsTestMode;
            }
        }
        public static bool IsUsingNewAssetBundle = true;
        private ABDependsLoad mABDependsLoader;

        public AssetBundleLoadManager()
        {
            mABDependsLoader = new ABDependsLoad();
        }

        public void UpdateBundleConfigData()
        {
            mABDependsLoader.UpdateABManifest();
            ReleaseAllAssetBundle();
        }

        public Object Load(string resPath, System.Type resType, bool isScene = false)
        {
#if UNITY_EDITOR
            ResourceRecorder.Instance.RecordResource(resPath, resType);
#endif

            if (IsTestMode)
            {
                return Resources.Load(resPath, resType);
            }

            string abName = string.Empty;
            if (isScene)
            {
                abName = string.Format("assets/{0}.scene", resPath.ToLower());
            }
            else
            {
                abName = string.Format("assets/resources/{0}{1}", resPath.ToLower(), GetAssetBundleName(resType));
            }
            AssetBundle abData = mABDependsLoader.GetAssetBundle(abName);
            if (null == abData)
            {
                if (isScene)
                {
                    return null;
                }
                return Resources.Load(resPath, resType);
            }
            else
            {
                if (abData.isStreamedSceneAssetBundle)
                {
                    return null;
                }

                string assetName = resPath.Substring(resPath.LastIndexOf("/") + 1).ToLower();
                string[] allAssetNames = abData.GetAllAssetNames();
                for (int i = 0; i < allAssetNames.Length; i++)
                {
                    if (allAssetNames[i].Contains(assetName))
                    {
                        assetName = allAssetNames[i];

                        break;
                    }
                }

                var o = abData.LoadAsset(assetName);
                return o;
            }
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public T Load<T>(string resPath, bool isScene = false) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            ResourceRecorder.Instance.RecordResource(resPath, typeof(T));
#endif

            if (IsTestMode)
            {
                return Resources.Load<T>(resPath);
            }

            System.Type resType = typeof(T);
            string abName = string.Empty;
            if (isScene)
            {
                abName = string.Format("assets/{0}.scene", resPath.ToLower());
            }
            else
            {
                abName = string.Format("assets/resources/{0}{1}", resPath.ToLower(), GetAssetBundleName(resType));
            }
            AssetBundle abData = mABDependsLoader.GetAssetBundle(abName);
            if(null == abData)
            {
                return Resources.Load<T>(resPath);
            }
            else
            {
                if (abData.isStreamedSceneAssetBundle)
                {
                    return default(T);
                }
                string assetName = resPath.Substring(resPath.LastIndexOf("/") + 1).ToLower();
                string[] allAssetNames = abData.GetAllAssetNames();
                for (int i = 0; i < allAssetNames.Length; i++)
                {
                    if (allAssetNames[i].Contains(assetName))
                    {
                        assetName = allAssetNames[i];

                        break;
                    }
                }

                var o = abData.LoadAsset(assetName);

                return o as T;
            }
        }

        public IEnumerator LoadAsync(string resPath, System.Type resType, bool isScene, ResourceAsyncDelegate callback = null)
        {
#if UNITY_EDITOR
            ResourceRecorder.Instance.RecordResource(resPath, resType);
#endif

            if (IsTestMode)
            {
                if (null != callback)
                {
                    callback(false, Resources.Load(resPath, resType));
                }

                yield break;
            }

            string abName = string.Empty;
            if (isScene)
            {
                abName = string.Format("assets/{0}.scene", resPath.ToLower());
            }
            else
            {
                abName = string.Format("assets/resources/{0}{1}", resPath.ToLower(), GetAssetBundleName(resType));
            }

            AssetBundleAsyncDelegate callbackFunc = (isBundle, assetbundle) =>
            {
                if (null == assetbundle)
                {
                    if (null != callback)
                    {
                        callback(false, isScene ? null : Resources.Load(resPath, resType));
                    }
                }
                else
                {
                    if (assetbundle.isStreamedSceneAssetBundle)
                    {
                        if (null != callback)
                        {
                            callback(true, null);
                        }
                    }
                    else if (null != callback)
                    {
                        string assetName = resPath.Substring(resPath.LastIndexOf("/") + 1).ToLower();
                        string[] allAssetNames = assetbundle.GetAllAssetNames();
                        for (int i = 0; i < allAssetNames.Length; i++)
                        {
                            if (allAssetNames[i].Contains(assetName))
                            {
                                assetName = allAssetNames[i];

                                break;
                            }
                        }

                        Object resObj = assetbundle.LoadAsset(assetName);

                        callback(true, resObj);
                    }
                }
            };
            LogMgr.UnityLog("AssetBundleLoadManager.LoadAsync " + abName);
            yield return mABDependsLoader.GetAssetBundleAsync(abName, callbackFunc);
        }

        public IEnumerator LoadAsync<T>(string resPath, bool isScene, ResourceAsyncDelegate callback = null) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            ResourceRecorder.Instance.RecordResource(resPath, typeof(T));
#endif

            if (IsTestMode)
            {
                if(null != callback)
                {
                    callback(false, Resources.Load<T>(resPath));
                }

                yield break;
            }

            System.Type resType = typeof(T);
            string abName = string.Empty;
            if (isScene)
            {
                abName = string.Format("assets/{0}.scene", resPath.ToLower());
            }
            else
            {
                abName = string.Format("assets/resources/{0}{1}", resPath.ToLower(), GetAssetBundleName(resType));
            }

            AssetBundleAsyncDelegate callbackFunc = (isBundle, assetbundle) =>
            {
                if(null == assetbundle)
                {
                    if(null != callback)
                    {
                        callback(false,  isScene ? null : Resources.Load<T>(resPath));
                    }
                }
                else
                {
                    if (assetbundle.isStreamedSceneAssetBundle)
                    {
                        if (null != callback)
                        {
                            callback(true, null);
                        }
                    }
                    else if (null != callback)
                    {
                        string assetName = resPath.Substring(resPath.LastIndexOf("/") + 1).ToLower();
                        string[] allAssetNames = assetbundle.GetAllAssetNames();
                        for (int i = 0; i < allAssetNames.Length; i++)
                        {
                            if (allAssetNames[i].Contains(assetName))
                            {
                                assetName = allAssetNames[i];

                                break;
                            }
                        }

                        var o = assetbundle.LoadAsset(assetName);

                        callback(true, o as T);
                    }
                }
            };

            yield return mABDependsLoader.GetAssetBundleAsync(abName, callbackFunc);
        }

        /// <summary>
        /// 释放所有的AssetBundle，切换场景的时候调用
        /// </summary>
        public void ReleaseAllAssetBundle()
        {
            AtlasSpriteManager.Instance.ClearCache();
            mABDependsLoader.ReleaseAllAssetBundle();
        }

        /// <summary>
        /// 根据类型获取AssetBundle的名字
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetAssetBundleName(System.Type type)
        {
            if (typeof(GameObject) == type)
            {
                return ".gameobject";
            }
            else if(typeof(TextAsset) == type)
            {
                return ".text";
            }
            else if (typeof(Texture2D) == type ||
                typeof(Texture3D) == type ||
                typeof(Texture) == type)
            {
                return ".texture";
            }
            else if (typeof(AudioClip) == type)
            {
                return ".sound";
            }
            else
            {
                return ".asset";
            }
        }
    }
}

