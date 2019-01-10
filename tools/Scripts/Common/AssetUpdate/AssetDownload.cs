using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using SG;

namespace Bundle
{
    public enum DownLoadType
    {
        FIRST_PHASE,
        BACKGROUND_PHASE,
    }

[Hotfix]
    public class AssetDownload
    {
        public delegate void FinishDelegate(bool sucess);

        public DownLoadType mDownloadType;
        private AssetUpdateMgr mParent;
        private DownloadAssetSizeCaculate mCurrentDownloadAssetSizeCaculate;
        private FinishDelegate mFinishCallBack;

        private List<AssetBundleConfig> mNewVersionDataConfig = new List<AssetBundleConfig>();
        private List<AssetBundleConfig> mDownLoadErrorVersionDataConfig = new List<AssetBundleConfig>();

        private Dictionary<string, AssetBundleConfig> mAllVersionDataDic = new Dictionary<string, AssetBundleConfig>();
        private long mTempDownLoadFileSize = 0;

        private int mCurrentDownLoadCount = 0;
        protected int mMaxDownLoadCount = 8;          //去警告 by XuXiang

        protected int mDownLoadAssetCount;          //去警告 by XuXiang


        public DownloadAssetSizeCaculate DownloadAssetSizeCaculate {
            get {
                return mCurrentDownloadAssetSizeCaculate;
            }
        }


        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="sizeData"></param>
        /// <param name="finishCallBack"></param>
        public void BeginDownLoadAsset(DownLoadType type, AssetUpdateMgr parent, DownloadAssetSizeCaculate sizeData, FinishDelegate finishCallBack)
        {
            mDownloadType = type;
            mParent = parent;
            mCurrentDownloadAssetSizeCaculate = sizeData;
            mFinishCallBack = finishCallBack;

            if (null == sizeData.mVersionDataConfig || 0 == sizeData.mVersionDataConfig.Count)
            {
                CallBackDownLoadSucess();

                return;
            }

            mNewVersionDataConfig = sizeData.mVersionDataConfig;
            mAllVersionDataDic.Clear();

            if (mDownloadType == DownLoadType.FIRST_PHASE && null != mParent)
            {
                mParent.UpdateProgress("开始下载更新资源文件", 0);
            }

            mMaxDownLoadCount = mDownloadType == DownLoadType.FIRST_PHASE ? 8 : 3;
            DownLoadAsset();
            mDownLoadAssetCount = mNewVersionDataConfig.Count;
        }

        /// <summary>
        /// 下载资源
        /// </summary>
        private void DownLoadAsset()
        {
            mCurrentDownLoadCount = mNewVersionDataConfig.Count;
            for (int i = 0; i < mNewVersionDataConfig.Count; i++)
            {
                AssetBundleConfig child = mNewVersionDataConfig[i];
                string remoteUrl = BundleCommon.RemoteUrl + child.RelativePath + "_Compress";
                string saveFile = Application.persistentDataPath + "/" + BundleCommon.ResVersion + "/Out" + child.RelativePath + "_Compress";

                HttpDownloadFinish downLoadAssetComplete = (success) =>
                {
                    OnDownLoadAsset(success, child);
                };
                SG.CoreEntry.gHttpDownloadMgr.PushDownloadFile(remoteUrl, saveFile, child, downLoadAssetComplete);
            }

            mNewVersionDataConfig.Clear();
        }

        /// <summary>
        /// 获取远程更新地址
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string GetRemoteUrl(AssetBundleConfig data)
		{
                string getUrl = BundleCommon.RemoteUrl + data.RelativePath + "_Compress";
#if UNITY_EDITOR
                getUrl = BundleCommon.RemoteUrl + data.RelativePath.Replace("/", "\\") + "_Compress";
#else
                getUrl = BundleCommon.RemoteUrl + data.RelativePath + "_Compress";
#endif
			return getUrl;
		}

        /// <summary>
        /// 下载回调
        /// </summary>
        /// <param name="success"></param>
        /// <param name="resVersionData"></param>
        private void OnDownLoadAsset(bool success, AssetBundleConfig resVersionData)
        {
            if (success)
            {
                mCurrentDownloadAssetSizeCaculate.OnAssetDownLoad(resVersionData);
                float progress = mCurrentDownloadAssetSizeCaculate.CurrentDownloadSize / mCurrentDownloadAssetSizeCaculate.MaxSize;

                if (mDownloadType == DownLoadType.FIRST_PHASE && null != mParent)
                {
                    mParent.UpdateProgress(string.Format("正在下载更新{0}M/{1}M", mCurrentDownloadAssetSizeCaculate.CurrentDownLoadFileSize, mCurrentDownloadAssetSizeCaculate.AllFileSize), progress);
                }
                else {
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_Bundle_Group_Download_Progress, EventParameter.Get(progress));
                    LogMgr.UnityLog("Bundle group " + mCurrentDownloadAssetSizeCaculate.CurBuild + " progress, " + progress);
                }
            }
            else
            {
                mDownLoadErrorVersionDataConfig.Add(resVersionData);
            }

            mCurrentDownLoadCount--;
            if (mNewVersionDataConfig.Count == 0 && mCurrentDownLoadCount == 0)
            {
                if (mDownLoadErrorVersionDataConfig.Count <= 0)
                {
                    CallBackDownLoadSucess();
                }
                else
                {
                    OnDownLoadError();
                }
            }
        }

        private void OnDownLoadError()
        {
            System.Action callBack = delegate()
            {
                for (int i = 0; i < mDownLoadErrorVersionDataConfig.Count; i++)
                {
                    mNewVersionDataConfig.Add(mDownLoadErrorVersionDataConfig[i]);
                }
                mDownLoadErrorVersionDataConfig.Clear();
                DownLoadAsset();
            };
            if (mDownloadType == DownLoadType.FIRST_PHASE)
            {
                SG.EventParameter ep = SG.EventParameter.Get(20001);
                ep.objParameter = callBack;
                SG.CoreEntry.gEventMgr.TriggerEvent(SG.GameEvent.GE_DOWNLOAD_ASSET, ep);
            }
            else if (mDownloadType == DownLoadType.BACKGROUND_PHASE)
            {
                callBack();
            }
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="versionData"></param>
        private void SaveDownloadBundle(AssetBundleConfig versionData)
        {
            mAllVersionDataDic[versionData.ABName] = versionData;
            mTempDownLoadFileSize += versionData.FileSize;
            WriteVersionDataToDB();
        }

        private void WriteVersionDataToDB()
        {
            List<AssetBundleConfig> bundleConfigGroup = new List<AssetBundleConfig>();
            foreach (AssetBundleConfig child in mAllVersionDataDic.Values)
            {
                bundleConfigGroup.Add(child);
            }
            mAllVersionDataDic.Clear();
            mTempDownLoadFileSize = 0;
            XMLTool.SaveABConfig(BundleCommon.UserVersionPath, bundleConfigGroup);
        }

        private void CallBackDownLoadSucess()
        {
            if (mFinishCallBack != null)
            {
                mFinishCallBack(true);
            }
        }
    }
}

