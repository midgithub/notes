using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SG;
using XLua;

namespace Bundle
{
    [LuaCallCSharp]
[Hotfix]
    public class AssetUpdateMgr
    {
        private static AssetUpdateMgr instance = null;
        public static AssetUpdateMgr Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new AssetUpdateMgr();
                }

                return instance;
            }
        }

        private GetBundleVersion mGetBundleVersion;
        private GetBundleUrl mGetBundleUrl;
        private AssetVersionCompare mAssetVersionCompare;
        private AssetDownload mAssetDownload;//= new AssetDownload();
        private DownloadAssetSizeCaculate mSizeData;
        private AssetDecompress mAssetDecompress;
        private EventParameter mUpdateEventParam;

        public bool IsRestartGame = false;

        private AssetUpdateMgr()
        {
            mGetBundleVersion = new GetBundleVersion();
            mGetBundleUrl = new GetBundleUrl();
            mAssetVersionCompare = new AssetVersionCompare();
            mAssetDownload = new AssetDownload();
            mAssetDecompress = new AssetDecompress();
            mUpdateEventParam = EventParameter.Get();
            mUpdateEventParam.autoRecycle = false;
        }

        public AssetDownload AssetDownload {
            get {
                return mAssetDownload;
            }
        }
        /// <summary>
        /// Step 1:获取远程更新地址
        /// </summary>
        public void BeginUpdate()
        {
            mGetBundleVersion.GetVersion(OnGetVersionCallback, this);
        }

        private void OnGetVersionCallback(bool success, string content)
        {
            LogMgr.UnityLog("got version:" + success + ";" + content);

            if (success)
            {
                GetRemoteUrl();
            }
            else
            {
                UpdateProgress("无法获取资源版本", 0);
#if UNITY_EDITOR
                MonoInstance.Instance.StartCoroutine(LoadScnesForTime());
#else
                mUpdateEventParam.intParameter = 10001;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_DOWNLOAD_ASSET, mUpdateEventParam);
#endif
            }
        }

        public void GetRemoteUrl()
        {
            mGetBundleUrl.GetRemoteDownloadUrl(OnGetUrlCallback, this);
        }

        /// <summary>
        /// 获取远程更新地址回调
        /// </summary>
        /// <param name="success"></param>
        /// <param name="content"></param>
        private void OnGetUrlCallback(bool success, string content)
        {
            LogMgr.UnityLog("got remote download url:" + success + ";" + content);

            if (success)
            {
                GetNewVersionDataConfig();
            }
            else
            {
                UpdateProgress("无法获取更新资源", 0);
#if UNITY_EDITOR
                MonoInstance.Instance.StartCoroutine(LoadScnesForTime());
#else
                mUpdateEventParam.intParameter = 20000;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_DOWNLOAD_ASSET, mUpdateEventParam);
#endif
            }
        }

        IEnumerator LoadScnesForTime()
        {
            yield return new WaitForSeconds(1);
            mUpdateEventParam.intParameter = 9999;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_DOWNLOAD_ASSET, mUpdateEventParam);
        }

        /// <summary>
        /// Step 2:获取版本数据配置信息
        /// </summary>
        private void GetNewVersionDataConfig()
        {
            mAssetVersionCompare.BeginCompare(this, OnGetNewVersionDataCallBack);
        }

        /// <summary>
        /// 获取版本数据配置信息回调
        /// </summary>
        /// <param name="sucess"></param>
        /// <param name="dataConfig"></param>
        /// <param name="newVersionMD5"></param>
        private void OnGetNewVersionDataCallBack(bool sucess, List<AssetBundleConfig> dataConfig, string newVersionMD5)
        {
            mSizeData = new DownloadAssetSizeCaculate(dataConfig, newVersionMD5);
            mSizeData.SetDownLoadType(DownLoadType.FIRST_PHASE);
            if (sucess)
            {
                if (null != dataConfig && mSizeData.mVersionDataConfig.Count > 0)
                {
                    ShowDownLoadAssetMsg();
                }
                else
                {
                    ConfirmUpdate();
                }
            }
        }

        /// <summary>
        /// Step3:更新
        /// </summary>
        /// <param name="sizeData"></param>
        public void ConfirmUpdate()
        {
            mAssetDownload.BeginDownLoadAsset(mSizeData.mCurrentType, this, mSizeData, OnDownLoadAssetComplete);
        }

        /// <summary>
        /// 更新提示
        /// </summary>
        /// <param name="sizeData"></param>
        private void ShowDownLoadAssetMsg()
        {
            mUpdateEventParam.floatParameter = 0;
            mUpdateEventParam.objParameter = mSizeData;
            mUpdateEventParam.intParameter = 10000;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_DOWNLOAD_ASSET, mUpdateEventParam);
            mUpdateEventParam.intParameter = 0;
        }

        /// <summary>
        /// Step 4:更新完成
        /// </summary>
        /// <param name="isSucess"></param>
        private void OnDownLoadAssetComplete(bool isSucess)
        {
            UpdateProgress("资源加载完毕", 1);
            if (IsRestartGame)
            {
                mUpdateEventParam.intParameter = 40000;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_DOWNLOAD_ASSET, mUpdateEventParam);
            }
            else
            {
                //DoDecompress();
                DecompressCallback();
            }
        }

        private void DoDecompress()
        {
            mAssetDecompress.DoStreamAssetDecompress(this, DecompressCallback);
        }

        private void DecompressCallback()
        {
            mUpdateEventParam.intParameter = 9999;
            mUpdateEventParam.stringParameter = "正在进入游戏";
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_DOWNLOAD_ASSET, mUpdateEventParam);
            mUpdateEventParam.intParameter = 0;

            UpdateInBackground();
        }

        private void UpdateInBackground()
        {
            if (null != mSizeData)
            {
                if (mSizeData.SetDownLoadType(DownLoadType.BACKGROUND_PHASE))
                {
                    if (mSizeData.mVersionDataConfig.Count >= 0)
                    {
                        LogMgr.UnityLog(string.Format("AssetUpdateMgr Build:{0} begin download!!!", mSizeData.CurBuild));
                        mUpdateEventParam.intParameter = mSizeData.CurBuild;
                        mUpdateEventParam.stringParameter = "bundle group " + mSizeData.CurBuild + "begin download";
                        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_Bundle_Group_BeginDowload, mUpdateEventParam);
                        mAssetDownload.BeginDownLoadAsset(mSizeData.mCurrentType, this, mSizeData, OnBackUpdateComplete);
                    }
                    else
                    {
                        OnBackUpdateComplete(true);
                        //CoreEntry.gHttpDownloadMgr.SetExit();
                    }
                }
                else
                {
                    CoreEntry.gHttpDownloadMgr.SetExit();
                }
            }
        }

        private void OnBackUpdateComplete(bool isSucess)
        {
            LogMgr.UnityLog(string.Format("AssetUpdateMgr Build:{0} downloaded completed!!!", mSizeData.CurBuild));
            mUpdateEventParam.intParameter = mSizeData.CurBuild;
            mUpdateEventParam.stringParameter = "扩展包" + mSizeData.CurBuild + "下载完成";

            LogMgr.UnityLog(string.Format("TriggerEvent GE_Bundle_Group_Downloaded", mSizeData.CurBuild));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_Bundle_Group_Downloaded, mUpdateEventParam);

            UpdateInBackground();
        }

        public void UpdateProgress(string desInfo,float progress)
        {
            mUpdateEventParam.intParameter = 0;
            mUpdateEventParam.floatParameter = progress;
            mUpdateEventParam.stringParameter = desInfo;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_DOWNLOAD_ASSET, mUpdateEventParam); 
        }

        public float GetTotalDownloadSize()
        {
            if(null == mSizeData)
            {
                return 0f;
            }

            return mSizeData.TotalSize;
        }

        public float GetCurrentDownloadSize()
        {
            if(null == mSizeData)
            {
                return 0f;
            }

            return mSizeData.TotalDownloadSize;
        }

        public int GetTotalDownloadNum()
        {
            if (null == mSizeData)
            {
                return 0;
            }

            return mSizeData.TotalNum;
        }

        public int GetCurrentDownloadNum()
        {
            if (null == mSizeData)
            {
                return 0;
            }

            return mSizeData.TotalDownloadNum;
        }

        public bool IsPause()
        {
            return CoreEntry.gHttpDownloadMgr.mIsPause;
        }

        public void SetPause()
        {
            CoreEntry.gHttpDownloadMgr.SetPause(true);
        }

        public void SetContinue()
        {
            CoreEntry.gHttpDownloadMgr.SetPause(false);
        }
    }
}

