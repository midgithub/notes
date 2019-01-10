using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading;
using SG;

namespace Bundle
{

[Hotfix]
    public class AssetVersionCompare
    {
        delegate void GetCompressConfigCallBack(byte[] bytes);

        private CompareBundleVersionCallBack mFinishCallBack;
        private AssetUpdateMgr mParent;
        private string mRemoteVersionMD5 = "";
        private float mFValue = 0f;
        private float mDValue = 0.5f;

        private ABFileMgr mABFileMgr;

        public void BeginCompare(AssetUpdateMgr parent, CompareBundleVersionCallBack callBack)
        {
            mParent = parent;
            mFinishCallBack = callBack;

            mABFileMgr = ABFileMgr.Instance;

            mFValue = 0f;
            UpdateLoadCompressProgress(0f);
            DownloadManifest();
        }

        private void DownloadManifest()
        {
            string url = BundleCommon.RemoteUrl + "/" + BundleCommon.AssetBundleManifest + "_Compress";
            new BundleDownLoadTask(url, OnDownManifest, UpdateLoadCompressProgress);
        }

        private void OnDownManifest(bool success, byte[] bytes)
        {
            if (success)
            {
                string localFilePath = Application.persistentDataPath + "/" + BundleCommon.ResVersion + "/Out/assetbundle_Compress";
                string localFileDir = localFilePath.Remove(localFilePath.LastIndexOf("/"));

                if (!Directory.Exists(localFileDir))
                {
                    Directory.CreateDirectory(localFileDir);
                }
                if (File.Exists(localFilePath))
                {
                    File.Delete(localFilePath);
                }
                File.WriteAllBytes(localFilePath, bytes);

                string outFilePath = localFilePath.Replace("_Compress", "");
                if (File.Exists(outFilePath))
                    File.Delete(outFilePath);

                LZMATool.DecompressFileLZMA(localFilePath, outFilePath);
                if (File.Exists(localFilePath))
                    File.Delete(localFilePath);

                DownloadConfig();
            }
            else
            {
                DownloadManifest();
            }
        }

        private void DownloadConfig()
        {
            mFValue = 0.5f;
            UpdateLoadCompressProgress(0f);

            string streamPath = BundleCommon.StreamAssetVersionPath;
            string streamUserPath = BundleCommon.StreamUserVersionPath;
            if (!Directory.Exists(Application.persistentDataPath + "/" + BundleCommon.ResVersion + "/Inner"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + BundleCommon.ResVersion + "/Inner");
            }

            if (!File.Exists(streamUserPath))
            {
                new WWWDownloadTask(streamPath, OnGetStreamABList);
            }
            else
            {
                try
                {
                    streamConfigs = XMLTool.LoadAllABConfig(BundleCommon.StreamUserVersionPath, true);
                    GetRemoteVersion();
                }
                catch (Exception e)
                {
                    LogMgr.UnityLog(e.ToString());
                    File.Delete(streamUserPath);
                    new WWWDownloadTask(streamPath, OnGetStreamABList);
                }
            }
        }

        private void OnGetStreamABList(bool success, byte[] data)
        {
            if (success)
            {
                string streamUserPath = BundleCommon.StreamUserVersionPath;
                string streamTmp = streamUserPath + "_Compress";
                if (File.Exists(streamTmp))
                {
                    File.Delete(streamTmp);
                }

                File.WriteAllBytes(streamTmp, data);
                LZMATool.DecompressFileLZMA(streamTmp, streamUserPath);
                File.Delete(streamTmp);
            }

            GetRemoteVersion();
        }

        /// <summary>
        /// Step 1:获取远程版本信息
        /// </summary>
        private void GetRemoteVersion()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            string configUrl = BundleCommon.RemoteUrl + "/" + BundleCommon.MainVersionFile + "_Compress" + BundleCommon.GetRandomUrl();
#else
            string configUrl = Path.Combine(BundleCommon.RemoteUrl, BundleCommon.MainVersionFile + "_Compress") + BundleCommon.GetRandomUrl();
#endif
            new BundleDownLoadTask(configUrl, OnGetRemoteVersionCallback, UpdateLoadCompressProgress);
        }

        /// <summary>
        /// 远程配置信息回调
        /// </summary>
        /// <param name="success"></param>
        /// <param name="data"></param>
        private void OnGetRemoteVersionCallback(bool success, byte[] data)
        {
            byte[] deCompressData = null;
            if (null != data)
            {
                deCompressData = LZMATool.DecompressBytesLZMA(data);
            }

            OnGetRemoteVersionBytes(deCompressData);
        }

        /// <summary>
        /// Step 2:记录远程配置数据
        /// </summary>
        /// <param name="data"></param>
        private void OnGetRemoteVersionBytes(byte[] data)
        {
            string tmpFile = Path.Combine(Application.persistentDataPath + "/" + BundleCommon.ResVersion, "Out/TmpABList");
            if (!Directory.Exists(Application.persistentDataPath + "/" + BundleCommon.ResVersion + "/Out"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + BundleCommon.ResVersion + "/Out");
            }

            if (File.Exists(tmpFile))
            {
                File.Delete(tmpFile);
            }

            if (null != data)
            {
                File.WriteAllBytes(tmpFile, data);
            }

            CompareVersion(tmpFile);
        }

        /// <summary>
        /// 下载更新进度
        /// </summary>
        /// <param name="progress"></param>
        private void UpdateLoadCompressProgress(float progress)
        {
            mParent.UpdateProgress("正在读取资源文件", mFValue + progress * mDValue);
        }

        /// <summary>
        /// Step 4:对比本地和服务器配置数据信息 
        /// </summary>
        /// <param name="remoteVersionPath"></param>
        private void CompareVersion(string remoteVersionPath)
        {
            mParent.UpdateProgress("比对更新文件中", 0);

            string remoteABPath = Application.persistentDataPath + "/" + BundleCommon.ResVersion + "/Out/assetbundle";
            AssetBundle remoteAB = null;
            AssetBundleManifest remoteABM = null;
            if (File.Exists(remoteABPath))
            {
                remoteAB = AssetBundle.LoadFromFile(remoteABPath);
            }
            if (null != remoteAB)
            {
                remoteABM = remoteAB.LoadAsset("AssetBundleManifest") as AssetBundleManifest;

                remoteAB.Unload(false);
            }

            remoteConfigs = XMLTool.LoadAllABConfig(remoteVersionPath);
            localConfigs = XMLTool.LoadAllABConfig(BundleCommon.UserVersionPath);
            streamConfigs = XMLTool.LoadAllABConfig(BundleCommon.StreamUserVersionPath, true);

            updateABConfigs = new List<AssetBundleConfig>();

            if (null != remoteConfigs)
            {
                Dictionary<string, AssetBundleConfig> localConfigDict = new Dictionary<string, AssetBundleConfig>();
                Dictionary<string, AssetBundleConfig> remoteConfigDict = new Dictionary<string, AssetBundleConfig>();
                Dictionary<string, AssetBundleConfig> streamConfigDict = new Dictionary<string, AssetBundleConfig>();
                Dictionary<string, string[]> dps = new Dictionary<string, string[]>();

                for (int i = 0; i < remoteConfigs.Count; i++)
                {
                    remoteConfigDict[remoteConfigs[i].RelativePath] = remoteConfigs[i];

                    mABFileMgr.AddABFile(remoteConfigs[i]);
                }
                for (int i = 0; i < localConfigs.Count; i++)
                {
                    localConfigDict[localConfigs[i].ABName] = localConfigs[i];
                }

                if (null != streamConfigs)
                {
                    for (int i = 0; i < streamConfigs.Count; i++)
                    {
                        streamConfigDict[streamConfigs[i].ABName] = streamConfigs[i];
                    }
                }

                int totalSize = remoteConfigs.Count;
                for (int i = 0; i < totalSize; i++)
                {
                    AssetBundleConfig localCfg = null;
                    AssetBundleConfig streamCfg = null;
                    bool isNew = false;
                    if (localConfigDict.TryGetValue(remoteConfigs[i].ABName, out localCfg))
                    {
                        if (localCfg.MD5Value.Equals(remoteConfigs[i].MD5Value))
                        {
                            isNew = false;

                            mABFileMgr.SetABFileState(remoteConfigs[i].RelativePath, ABFileState.DOWNLOADED);
                        }
                        else
                        {
                            isNew = true;
                        }
                    }
                    else
                    {
                        if (streamConfigDict.TryGetValue(remoteConfigs[i].ABName, out streamCfg))
                        {
                            if (streamCfg.MD5Value.Equals(remoteConfigs[i].MD5Value))
                            {
                                isNew = false;

                                mABFileMgr.SetABFileState(remoteConfigs[i].RelativePath, ABFileState.STREAMEXIT);
                            }
                            else
                            {
                                isNew = true;
                            }
                        }
                        else
                        {
                            isNew = true;
                        }
                    }

                    if (isNew)
                    {
                        string relativePath = remoteConfigs[i].RelativePath;
                        string[] dp = remoteABM.GetAllDependencies(relativePath.Substring(1, relativePath.Length - 1));
                        dps[relativePath] = dp;
                    }
                }

                if (dps.Count > 0)
                {
                    CompareThread.StartCompare(remoteConfigs, remoteConfigDict, localConfigDict, streamConfigDict, dps, updateABConfigs, CompareProgress, CompareFinish);
                }
                else
                {
                    CompareFinish();
                }
            }
            else
            {
                CompareFinish();
            }
        }

        private void CompareProgress(float f)
        {
            mParent.UpdateProgress("比对更新文件中", f);
        }

        private List<AssetBundleConfig> updateABConfigs;
        private List<AssetBundleConfig> remoteConfigs;
        private List<AssetBundleConfig> localConfigs;
        private List<AssetBundleConfig> streamConfigs;
        private void CompareFinish()
        {
            AssetCleaner.StartClean(remoteConfigs, localConfigs, streamConfigs);

            if (!AssetBundleLoadManager.IsUsingNewAssetBundle)
            {
                updateABConfigs.Clear();
            }

            mParent.UpdateProgress("比对更新文件中", 1.0f);

            if (null != mFinishCallBack)
            {
                mFinishCallBack(true, updateABConfigs, mRemoteVersionMD5);
            }
        }
    }

[Hotfix]
    class CompareThread
    {
        private List<AssetBundleConfig> remoteConfigs;
        private Dictionary<string, AssetBundleConfig> remoteConfigDict;
        private Dictionary<string, AssetBundleConfig> localConfigDict;
        private Dictionary<string, AssetBundleConfig> streamConfigDict;
        private Dictionary<string, string[]> dps;
        private List<AssetBundleConfig> updateABConfigs;
        private Action<float> progressCallback;
        private Action finishCallback;
        private MainThreadDispatcher dispatcher;

        private CompareThread(List<AssetBundleConfig> remoteConfigs, Dictionary<string, AssetBundleConfig> remoteConfigDict, Dictionary<string, AssetBundleConfig> localConfigDict, Dictionary<string, AssetBundleConfig> streamConfigDict, Dictionary<string, string[]> dps, List<AssetBundleConfig> updateABConfigs, Action<float> progressCallback, Action finishCallback)
        {
            this.remoteConfigs = remoteConfigs;
            this.remoteConfigDict = remoteConfigDict;
            this.localConfigDict = localConfigDict;
            this.streamConfigDict = streamConfigDict;
            this.dps = dps;
            this.updateABConfigs = updateABConfigs;
            this.progressCallback = progressCallback;
            this.finishCallback = finishCallback;
            dispatcher = CoreEntry.gMainThreadDispatcher;

            Thread thread = new Thread(DoCompare);
            thread.Start();
        }

        private void DoCompare()
        {
            updateABConfigs.Clear();

            Dictionary<string, AssetBundleConfig> tmpDict = new Dictionary<string, AssetBundleConfig>();

            ABFileMgr mABFileMgr = ABFileMgr.Instance;
            if (null != remoteConfigs)
            {
                int totalSize = remoteConfigs.Count;
                for (int i = 0; i < totalSize; i++)
                {
                    dispatcher.AddAction(() =>
                    {
                        progressCallback(i * 1.0f / totalSize);
                    });

                    AssetBundleConfig localCfg = null;
                    AssetBundleConfig streamCfg = null;
                    bool isNew = false;
                    if (localConfigDict.TryGetValue(remoteConfigs[i].ABName, out localCfg))
                    {
                        if (localCfg.MD5Value.Equals(remoteConfigs[i].MD5Value))
                        {
                            isNew = false;
                        }
                        else
                        {
                            isNew = true;
                        }
                    }
                    else
                    {
                        if (streamConfigDict.TryGetValue(remoteConfigs[i].ABName, out streamCfg))
                        {
                            if (streamCfg.MD5Value.Equals(remoteConfigs[i].MD5Value))
                            {
                                isNew = false;

                                if (!tmpDict.ContainsKey(remoteConfigs[i].RelativePath))
                                {
                                    mABFileMgr.SetABFileState(remoteConfigs[i].RelativePath, ABFileState.STREAMEXIT);
                                }
                            }
                            else
                            {
                                isNew = true;
                            }
                        }
                        else
                        {
                            isNew = true;
                        }
                    }

                    if (isNew)
                    {
                        if (!tmpDict.ContainsKey(remoteConfigs[i].RelativePath))
                        {
                            string relativePath = remoteConfigs[i].RelativePath;
                            string[] dp = dps[relativePath];
                            for (int j = 0; j < dp.Length; j++)
                            {
                                if (!tmpDict.ContainsKey("/" + dp[j]))
                                {
                                    AssetBundleConfig reTmpCfg = null;
                                    if (remoteConfigDict.TryGetValue("/" + dp[j], out reTmpCfg))
                                    {
                                        AssetBundleConfig tmpCfg = null;
                                        if (localConfigDict.TryGetValue(reTmpCfg.ABName, out tmpCfg))
                                        {
                                            if (!tmpCfg.MD5Value.Equals(reTmpCfg.MD5Value))
                                            {
                                                mABFileMgr.SetABFileState(reTmpCfg.RelativePath, ABFileState.NONE);
                                                updateABConfigs.Add(reTmpCfg);
                                                tmpDict[reTmpCfg.RelativePath] = reTmpCfg;
                                            }
                                        }
                                        else
                                        {
                                            if (streamConfigDict.TryGetValue(reTmpCfg.ABName, out tmpCfg))
                                            {
                                                if (tmpCfg.MD5Value.Equals(reTmpCfg.MD5Value))
                                                {
                                                    reTmpCfg.Build = remoteConfigs[i].Build;
                                                }
                                            }
                                            else
                                            {
                                                reTmpCfg.Build = remoteConfigs[i].Build;
                                            }

                                            mABFileMgr.SetABFileState(reTmpCfg.RelativePath, ABFileState.NONE);
                                            updateABConfigs.Add(reTmpCfg);
                                            tmpDict[reTmpCfg.RelativePath] = reTmpCfg;
                                        }
                                    }
                                }
                                else
                                {
                                    AssetBundleConfig reTmpCfg = tmpDict["/" + dp[j]];
                                    if (reTmpCfg.Build > remoteConfigs[i].Build)
                                    {
                                        reTmpCfg.Build = remoteConfigs[i].Build;
                                    }
                                }
                            }

                            updateABConfigs.Add(remoteConfigs[i]);
                            tmpDict[remoteConfigs[i].RelativePath] = remoteConfigs[i];
                        }
                        else
                        {
                            AssetBundleConfig reTmpCfg = tmpDict[remoteConfigs[i].RelativePath];
                            if (reTmpCfg.Build > remoteConfigs[i].Build)
                            {
                                reTmpCfg.Build = remoteConfigs[i].Build;
                            }
                        }
                    }
                }
            }

            dispatcher.AddAction(finishCallback);
        }

        public static CompareThread StartCompare(List<AssetBundleConfig> remoteConfigs, Dictionary<string, AssetBundleConfig> remoteConfigDict, Dictionary<string, AssetBundleConfig> localConfigDict, Dictionary<string, AssetBundleConfig> streamConfigDict, Dictionary<string, string[]> dps, List<AssetBundleConfig> updateABConfigs, Action<float> progressCallback, Action finishCallback)
        {
            return new CompareThread(remoteConfigs, remoteConfigDict, localConfigDict, streamConfigDict, dps, updateABConfigs, progressCallback, finishCallback);
        }
    }
}

#pragma warning disable 0168

