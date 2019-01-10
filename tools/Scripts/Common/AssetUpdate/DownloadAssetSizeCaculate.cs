using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bundle
{
[Hotfix]
    public class DownloadAssetSizeCaculate
    {
        public DownLoadType mCurrentType;
        private List<AssetBundleConfig> mAllDataConfig;
        public List<AssetBundleConfig> mVersionDataConfig;
        public float CurrentDownloadSize = 0;
        public float MaxSize = 0;
        public int MaxBuild = 0;
        public int CurBuild;
        public int TotalNum = 0;
        public int TotalDownloadNum = 0;
        public float TotalSize = 0;
        public float TotalDownloadSize = 0;

        public string CurrentDownLoadFileSize { get { return (CurrentDownloadSize < 0.01f ? 0.01f : CurrentDownloadSize).ToString("0.00"); } }
        public string AllFileSize { get { return (MaxSize < 0.01f ? 0.01f : MaxSize).ToString("0.00"); } }


        public string CurrentFakeDownLoadFileSize { get { return (CurrentDownloadSize < 0.01f ? 0.01f : CurrentDownloadSize/2).ToString("0.00"); } }
        public string AllFakeFileSize { get { return (MaxSize < 0.01f ? 0.01f : MaxSize/2).ToString("0.00"); } }

        public string NewVerionMD5;

        public DownloadAssetSizeCaculate(List<AssetBundleConfig> versionDataConfig, string newVersionMD5)
        {
            NewVerionMD5 = newVersionMD5;
            mAllDataConfig = versionDataConfig;
            mAllDataConfig.Sort((a, b) =>
            {
                return a.Build.CompareTo(b.Build);
            });
            mVersionDataConfig = new List<AssetBundleConfig>();
            CurBuild = 0;
            if (mAllDataConfig.Count > 0)
            {
                MaxBuild = mAllDataConfig[mAllDataConfig.Count - 1].Build;
            }
            SG.LogMgr.UnityLog("DownLoadAssetSizeCaculate: MaxBuild " + MaxBuild);
            TotalSize = 0;
            TotalNum = 0;
            for (int i = 0; i < mAllDataConfig.Count; i++)
            {
                if (mAllDataConfig[i].Build != 0 && ABFileMgr.Instance.GetABFileState(mAllDataConfig[i].RelativePath) == ABFileState.NONE)
                {
                    TotalSize += mAllDataConfig[i].FileSize / 1024f / 1024f;
                    TotalNum++;
                }

            }
            TotalDownloadSize = 0;
            TotalDownloadNum = 0;
        }

        public bool SetDownLoadType(DownLoadType type)
        {
            mCurrentType = type;
            mVersionDataConfig.Clear();
            long size = 0;
            if (type == DownLoadType.FIRST_PHASE)
            {
                CurBuild = 0;
            }
            else
            {
                CurBuild += 1;
            }
            SG.LogMgr.UnityLog("DownloadAssetSizeCaculate: SetDownLoadType CurrBuild " + CurBuild);
            if (null != mAllDataConfig)
            {
                for (int i = 0; i < mAllDataConfig.Count; i++)
                {
                    if (mAllDataConfig[i].Build == CurBuild && ABFileMgr.Instance.GetABFileState(mAllDataConfig[i].RelativePath) == ABFileState.NONE)
                    {
                        mVersionDataConfig.Add(mAllDataConfig[i]);
                    }
                }
                mVersionDataConfig.ApplyAllItem(C => size += C.FileSize);
            }
            MaxSize = size / 1024f / 1024f;
            CurrentDownloadSize = 0;

            return CurBuild <= MaxBuild;
        }

        public void OnAssetDownLoad(AssetBundleConfig dataConfig)
        {
            float fileSize = dataConfig.FileSize / 1024f / 1024f;
            CurrentDownloadSize += fileSize;
            if (CurBuild != 0)
            {
                TotalDownloadSize += fileSize;
                TotalDownloadNum++;
            }
        }

    }
}

