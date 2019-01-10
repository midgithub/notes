/**
* @file     : AssetDecompress
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-12-26
*/

using XLua;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using SG;
using System.Threading;

namespace Bundle
{
    public delegate void AssetCopyCallback(bool sucess, byte[] content, string file);

[Hotfix]
    public class AssetDecompress
    {
        private int mCurCopyNum;
        private int mMaxCopyNum;
        private int mTotalCopyNum;
        private int mFinishCopyNum;
        private List<string> mStreamFiles = new List<string>();
        private AssetUpdateMgr mParent;
        private Action mFinishCallback;

        public void DoStreamAssetDecompress(AssetUpdateMgr parent, Action finishCallback)
        {
            LogMgr.UnityError(Time.realtimeSinceStartup.ToString());

            mParent = parent;
            mFinishCallback = finishCallback;

            mParent.UpdateProgress("正从本地解压缩资源（此过程不消耗流量）", 0f);

            mStreamFiles.Clear();
            string abtoFile = string.Format("{0}/{1}/Inner/{2}", Application.persistentDataPath, BundleCommon.ResVersion, BundleCommon.AssetBundleManifest);
            if (!File.Exists(abtoFile))
            {
                mStreamFiles.Add("/" + BundleCommon.AssetBundleManifest);
            }

            List<AssetBundleConfig> streamConfigs = XMLTool.LoadAllABConfig(BundleCommon.StreamUserVersionPath, true);
            if (null != streamConfigs)
            {
                for (int i = 0; i < streamConfigs.Count; i++)
                {
                    string toFilePath = Application.persistentDataPath + "/" + BundleCommon.ResVersion + "/Inner" + streamConfigs[i].RelativePath;
                    string toFileDir = toFilePath.Remove(toFilePath.LastIndexOf("/"));
                    if (!Directory.Exists(toFileDir))
                    {
                        Directory.CreateDirectory(toFileDir);
                    }

                    if (!File.Exists(toFilePath))
                    {
                        mStreamFiles.Add(streamConfigs[i].RelativePath);
                    }
                }
            }
            mCurCopyNum = 0;
            mMaxCopyNum = 5;
            mFinishCopyNum = 0;
            mTotalCopyNum = mStreamFiles.Count;
            if (mTotalCopyNum != 0)
            {
                CopyAsset();
            }
            else
            {
                mParent.UpdateProgress("正从本地解压缩资源（此过程不消耗流量）", 1f);
                if (null != mFinishCallback)
                {
                    mFinishCallback();
                }
            }
        }

        private void CopyAsset()
        {
            for (int i = mStreamFiles.Count - 1; i >= 0; i--)
            {
                if(mCurCopyNum >= mMaxCopyNum)
                {
                    break;
                }

                mCurCopyNum++;
                string file = mStreamFiles[i];
                DownLoadBytesFileCallBack callback = (success, data) =>
                {
                    OnCoypAsset(success, data, file);
                };
                new WWWDownloadTask(string.Format("{0}{1}_Compress", BundleCommon.StreamPath, file), callback);

                mStreamFiles.RemoveAt(i);
            }
        }

        private void OnCoypAsset(bool success, byte[] data, string file)
        {
            if (success)
            {
#if UNITY_EDITOR
                DoDecompress(data, file);
                FinishCopy();
#else
                DecompressThread.StartDecompress(data, file, FinishCopy);
#endif
            }
            else
            {
                SG.LogMgr.UnityError(file + ":copy stream asset error!!!");

                FinishCopy();
            }
        }

        private void DoDecompress(byte[] data, string file)
        {
            string streamFile = string.Format("{0}/{1}/Inner{2}", Application.persistentDataPath, BundleCommon.ResVersion, file);
            string streamTmp = streamFile + "_Compress";
            if (File.Exists(streamTmp))
            {
                File.Delete(streamTmp);
            }

            File.WriteAllBytes(streamTmp, data);
            LZMATool.DecompressFileLZMA(streamTmp, streamFile);
            File.Delete(streamTmp);
        }

        private void FinishCopy()
        {
            mFinishCopyNum++;
            mParent.UpdateProgress("正从本地解压缩资源（此过程不消耗流量）", mFinishCopyNum * 1.0f / mTotalCopyNum);

            mCurCopyNum--;
            CopyAsset();

            if (mCurCopyNum == 0 && mStreamFiles.Count == 0)
            {
                mParent.UpdateProgress("正从本地解压缩资源（此过程不消耗流量）", 1f);
                if (null != mFinishCallback)
                {
                    LogMgr.UnityError(Time.realtimeSinceStartup.ToString());
                    mFinishCallback();
                }
            }
        }
    }

[Hotfix]
    class DecompressThread
    {
        private byte[] data;
        private string file;
        private Action finishCallback;
        private MainThreadDispatcher mDispatcher;

        private DecompressThread(byte[] data, string file, Action callback)
        {
            this.data = data;
            this.file = string.Format("{0}/{1}/Inner{2}", Application.persistentDataPath, BundleCommon.ResVersion, file);
            this.finishCallback = callback;
            mDispatcher = CoreEntry.gMainThreadDispatcher;

            Thread th = new Thread(DoDecompress);
            th.Start();
        }

        private void DoDecompress()
        {
            string streamTmp = file + "_Compress";
            if (File.Exists(streamTmp))
            {
                File.Delete(streamTmp);
            }

            File.WriteAllBytes(streamTmp, data);
            LZMATool.DecompressFileLZMA(streamTmp, file);
            File.Delete(streamTmp);

            if (null != finishCallback)
            {
                mDispatcher.AddAction(finishCallback);
            }
        }

        public static DecompressThread StartDecompress(byte[] data, string file, Action action)
        {
            return new DecompressThread(data, file, action);
        }
    }
}

