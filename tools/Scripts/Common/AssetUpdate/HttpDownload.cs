/**
* @file     : #FileName#
* @brief    : 
* @details  : 
* @author   : #Author#
* @date     : #Date#
*/
using XLua;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using System.IO;
using SG;

namespace Bundle
{
    public delegate void HttpDownloadFinish(bool success);
    public delegate void HttpDownloadCallback(bool success);

[Hotfix]
    public class HttpDownloadManager : MonoBehaviour
    {
        private System.Object mLockObject = new System.Object();
        private HttpDownloadTask mCurTask = null;
        private bool mIsDownload = false;
        private List<HttpDownloadTask> mDownloadTask = new List<HttpDownloadTask>();

        public bool mIsPause = false;
        private HttpDownload mDownloader = null;

        //private ABProcessor mABProcessorNum0 = new ABProcessor(0);
        //private ABProcessor mABProcessorNum1 = new ABProcessor(1);   
        
        void Update()
        {
            lock (mLockObject)
            {
                if (!mIsDownload && null != mCurTask)
                {
                    ABFileMgr.Instance.SetABFileState(mCurTask.config.RelativePath, ABFileState.DOWNLOADED);
                    if (null != mCurTask.callback)
                    {
                        mCurTask.callback(mCurTask.success);
                    }
                    mCurTask = null;
                }

                Download();
            }
        }

        public void PushDownloadFile(string url, string saveFile, AssetBundleConfig config, HttpDownloadFinish finishCallback = null)
        {
            int index = -1;
            for (int i = 0; i < mDownloadTask.Count; i++)
            {
                if (mDownloadTask[i].url.Equals(url))
                {
                    index = i;

                    break;
                }
            }
            if (-1 != index)
            {
                if (mDownloadTask.Count > 1)
                {
                    HttpDownloadTask tmp = mDownloadTask[index];
                    mDownloadTask.RemoveAt(index);
                    mDownloadTask.Add(tmp);
                }
            }
            else if (null == mCurTask || !mCurTask.url.Equals(url))
            {
                HttpDownloadTask task = new HttpDownloadTask(url, saveFile, config, finishCallback);
                mDownloadTask.Add(task);
            }
        }

        public void SetPause(bool isPause)
        {
            if (mIsPause == isPause)
            {
                return;
            }

            mIsPause = isPause;

            if (mIsPause && null != mCurTask)
            {
                mDownloadTask.Add(mCurTask);
                mCurTask = null;

                lock (mLockObject)
                {
                    mIsDownload = false;
                }
            }

            if (null != mDownloader)
            {
                mDownloader.SetPause(isPause);
            }
        }

        public void SetExit()
        {
            if (null != mDownloader)
            {
                mDownloader.SetExit();
            }
                        
        }

        public void SetABProcessorExit()
        {
            /*
            if (null != mABProcessorNum0)
            {
                mABProcessorNum0.SetExit();
            }

            if (null != mABProcessorNum1)
            {
                mABProcessorNum1.SetExit();
            }
            */
        }

        private void Download()
        {
            if (!mIsPause && mDownloadTask.Count > 0 && !mIsDownload)
            {
                if (null == mDownloader || (!mDownloader.IsProcess() && !mDownloader.HasTask()))
                {
                    mCurTask = mDownloadTask[mDownloadTask.Count - 1];
                    HttpDownloadCallback callback = (success) =>
                    {
                        lock (mLockObject)
                        {
                            DownloadCallback(success, mCurTask);
                        }
                    };
                    mDownloader = HttpDownload.StartDownloadFile(mCurTask.url, mCurTask.saveFile, mCurTask.config, callback);
                    mDownloadTask.RemoveAt(mDownloadTask.Count - 1);
                    mIsDownload = true;

                    LogMgr.UnityLog("http download:" + mCurTask.url);
                }
            }
        }

        private void DownloadCallback(bool success, HttpDownloadTask task)
        {
            if (null != task)
            {
                task.success = success;

                mIsDownload = false;
            }
        }
    }

[Hotfix]
    class HttpDownloadTask
    {
        public static string configPath;
        
        public string url;
        public string saveFile;
        public bool success;
        public AssetBundleConfig config;
        public HttpDownloadFinish callback;

        public HttpDownloadTask(string url, string saveFile, AssetBundleConfig config, HttpDownloadFinish callback)
        {
            this.url = url;
            this.saveFile = saveFile;
            this.callback = callback;
            this.config = config;

            if(string.IsNullOrEmpty(configPath))
            {
                configPath = BundleCommon.UserVersionPath;
            }
        }
    }

[Hotfix]
    public class DownloadedItem
    { 
        public string saveFile;
        public AssetBundleConfig config;
    
    }


[Hotfix]
    public class HttpDownload
    {
        private static System.Object mLockObject = new System.Object();
        public  static System.Object shareObject = new System.Object();
        private string mUrl;
        private string mSaveFile;
        private AssetBundleConfig mConfig;
        private HttpDownloadCallback mFinishCallback;

        private int mBufferSize = 1024;
        private long mDownloadedSize;
        public long DownloadedSize
        {
            get { return mDownloadedSize; }
        }

        private bool mIsPause;
        private bool mIsExit;
        private bool mHasTask;
        private bool mIsProcess;

        public static List<DownloadedItem> downloadedQueue = new List<DownloadedItem>();

        private static HttpDownload downloader = null;
        public static HttpDownload StartDownloadFile(string url, string saveFile, AssetBundleConfig config, HttpDownloadCallback finishCallback = null)
        {
            if (null == downloader)
            {
                downloader = new HttpDownload();
            }

            downloader.StartTask(url, saveFile, config, finishCallback);

            return downloader;
        }

        private HttpDownload()
        {
            mIsPause = false;
            mIsExit = false;
            mHasTask = false;
            mIsProcess = false;

            Thread thread = new Thread(DownLoadFile);
            thread.Start();
        }

        public bool HasTask()
        {
            bool hasTask = false;
            lock (mLockObject)
            {
                hasTask = mHasTask;
            }

            return hasTask;
        }

        public bool IsProcess()
        {
            bool isProcess = false;
            lock (mLockObject)
            {
                isProcess = mIsProcess;
            }

            return isProcess;
        }

        public void SetPause(bool isPause)
        {
            lock(mLockObject)
            {
                mIsPause = isPause;
            }
        }

        public bool GetPause()
        {
            bool isPause = false;
            lock(mLockObject)
            {
                isPause = mIsPause;
            }

            return isPause;
        }

        public void SetExit()
        {
            lock(mLockObject)
            {
                mIsExit = true;
            }
        }

        private void StartTask(string url, string saveFile, AssetBundleConfig config, HttpDownloadCallback finishCallback)
        {
            lock(mLockObject)
            {
                mUrl = url;
                mSaveFile = saveFile;
                mConfig = config;
                mFinishCallback = finishCallback;
                mDownloadedSize = 0;

                mHasTask = true;
            }
        }

        private void DownLoadFile()
        {
            bool bExit = false;
            bool bTask = false;
            bool bPause = false;
            bool bProcess = false;
            lock(mLockObject)
            {
                bExit = mIsExit;
            }
            while(!bExit)
            {
                lock(mLockObject)
                {
                    bTask = mHasTask;
                    bPause = mIsPause;
                    bExit = mIsExit;
                    bProcess = mIsProcess;
                }

                if (!bTask || bPause || bProcess)
                {
                    //LogMgr.UnityLog("DownLoadFile continue: ");
                    Thread.Sleep(10);
                    continue;
                }

                try
                {
                    lock (mLockObject)
                    {
                        bProcess = true;
                    }

                    string localFileDir = mSaveFile.Remove(mSaveFile.LastIndexOf("/"));
                    if (!Directory.Exists(localFileDir))
                    {
                        Directory.CreateDirectory(localFileDir);
                    }

                    HttpWebRequest request = WebRequest.Create(mUrl + BundleCommon.GetRandomUrl()) as HttpWebRequest;
                    WebResponse response = request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    byte[] buffer = new byte[mBufferSize];
                    FileStream fileStream = new FileStream(mSaveFile, FileMode.OpenOrCreate);
                    
                    int readSize = stream.Read(buffer, 0, mBufferSize);
                    while (readSize > 0 && !bPause && !bExit)
                    {
                        fileStream.Write(buffer, 0, readSize);
                        mDownloadedSize += readSize;

                        readSize = stream.Read(buffer, 0, mBufferSize);

                        lock(mLockObject)
                        {
                            bPause = mIsPause;
                            bExit = mIsExit;
                        }
                    }

                    stream.Close();
                    fileStream.Close();
                    response.Close();

                    DownloadedItem item = new DownloadedItem();
                    item.saveFile = mSaveFile;
                    item.config = mConfig;
                    lock (shareObject)
                    {
                        downloadedQueue.Add(item);
                    }

                    if(!bPause && !bExit)
                    {
                        
                        string outFilePath = mSaveFile.Replace("_Compress", "");
                        if (File.Exists(outFilePath))
                            File.Delete(outFilePath);
                        LZMATool.DecompressFileLZMA(mSaveFile, outFilePath);
                        if (File.Exists(mSaveFile))
                            File.Delete(mSaveFile);

                        if (null != mConfig)
                        {
                            List<AssetBundleConfig> bundleConfigGroup = new List<AssetBundleConfig>();
                            bundleConfigGroup.Add(mConfig);
                            lock (mLockObject)
                            {
                                XMLTool.SaveABConfig(HttpDownloadTask.configPath, bundleConfigGroup);
                                LogMgr.UnityLog("HttpDownLoad SaveABConfig: " + mConfig.RelativePath 
                                    + "," + mConfig.ABName + "," + mConfig.Build
                                    );
                            }
                        }
                        
                        if (null != mFinishCallback)
                        {
                            mFinishCallback(true);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogMgr.UnityError("download file error:" + e.ToString());
                    if (null != mFinishCallback && !bPause && !bExit)
                    {
                        mFinishCallback(false);
                    }
                }

                lock (mLockObject)
                {
                    bExit = mIsExit;

                    mIsProcess = false;
                    mHasTask = false;
                }
            }
        }
    }

[Hotfix]
    public class ABProcessor 
    {
        Thread task = null;
        private bool isExit = false;
        private int mIdx = 0;
        public ABProcessor( int idx) 
        {
            mIdx = idx;
            task = new Thread(ABProcess);
            task.Start();
        }

        public void SetExit()
        {
            isExit = true;
        }

        private void ABProcess()
        {
            LogMgr.UnityLog("ABProcess # " + mIdx + " Started .."); 
            while (!isExit)
            {
                int count = 0;
                DownloadedItem item = null;
                string mSaveFile = null;
                AssetBundleConfig mConfig = null;

                lock (HttpDownload.shareObject)
                {
                    count = HttpDownload.downloadedQueue.Count;
                    if (count > 0) 
                    {
                        item = HttpDownload.downloadedQueue[0];
                        HttpDownload.downloadedQueue.RemoveAt(0);
                    }
                }
                LogMgr.UnityLog("ABProcessor mid: " + mIdx);
                if (null != item)
                {
                    mSaveFile = item.saveFile;
                    mConfig = item.config;

                    string outFilePath = mSaveFile.Replace("_Compress", "");
                    if (File.Exists(outFilePath))
                        File.Delete(outFilePath);
                    LZMATool.DecompressFileLZMA(mSaveFile, outFilePath);
                    if (File.Exists(mSaveFile))
                        File.Delete(mSaveFile);

                    if (null != mConfig)
                    {
                        List<AssetBundleConfig> bundleConfigGroup = new List<AssetBundleConfig>();
                        bundleConfigGroup.Add(mConfig);

                        XMLTool.SaveABConfig(HttpDownloadTask.configPath, bundleConfigGroup);
                        LogMgr.UnityLog("HttpDownLoad SaveABConfig: " + count + "," +  mConfig.RelativePath
                            + "," + mConfig.ABName + "," + mConfig.Build + "," + Thread.CurrentThread.ManagedThreadId + ",@" + mIdx
                            );
                    }


                }
                else {
                    LogMgr.UnityLog("ABProcessor continue " + mIdx);
                    Thread.Sleep(10);
                }
            
            }
        }
    
    }
}

