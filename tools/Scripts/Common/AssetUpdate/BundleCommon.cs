using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SG;
using XLua;

namespace Bundle
{

    public delegate void CompareBundleVersionCallBack(bool sucess, List<AssetBundleConfig> newVersion, string newVersionMD5);
    public delegate void DownLoadBundleFileProgressCallBack(float progress);
    public delegate void SaveBundleVersionFileProgressCallBack(string tips,float progress);
    public delegate void LoadBundleObjectFileCallBack(bool sucess, Object callbackObj);
    //public delegate void LoadBundleFileCallBack(bool sucess, AssetBundle callbackObj);
    public delegate void DownLoadTxtFileCallBack(bool sucess, string content);
    public delegate void DownLoadBytesFileCallBack(bool sucess, byte[] content);

    [LuaCallCSharp]
    public class BundleCommon
    {
        private static string mRootUrl = null; 
        public static string RootUrl
        {
            //120.92.35.56
			get
            {
                if(mRootUrl == null)
                {
                    mRootUrl = ResSetting.Instance.GetStringValue("RootUrl");
                }
                return mRootUrl;
//#if UNITY_EDITOR
//                //return "http://192.168.8.109/";
//#else
//                //return "http://res.wqyry.linygame.com/";
//#endif
            }
        }

        public static string RootUrlFileName
        {
            get
            {
                return string.Format("RootUrl{0}.txt", RootUrlVersion);
            }
        }

        private static string mResVersion = string.Empty;
        public static string ResVersion
        {
            get
            {
                if (string.IsNullOrEmpty(mResVersion))
                {
                    mResVersion = ResSetting.Instance.GetStringValue("resversion");

                    if (string.IsNullOrEmpty(mResVersion))
                    {
                        LogMgr.UnityLog("ReSetting contains no key:resversion");
                        mResVersion = "0.0.0";
                    }
                    LogMgr.UnityLog("res version:" + mResVersion);
                }

                return mResVersion;
            }
        }

        public static string RootUrlVersion = "1.0.0";
        public static string RootUrlVersionFileName = "Version.txt";
        public static string BaseUrl;

        public static string RemoteUrl { get {
            string RelativePath = "";
#if UNITY_ANDROID
            RelativePath =  "Android";
#elif UNITY_IPHONE
            RelativePath =  "iOS";
#else
            RelativePath = "Windows";
#endif
            return Path.Combine(BaseUrl, RelativePath);
        } }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        public static string StreamPath = "file://" + Application.streamingAssetsPath;
#elif UNITY_IPHONE
        public static string StreamPath = "file://" + Application.streamingAssetsPath;
#else
        public static string StreamPath = Application.streamingAssetsPath;
#endif

        public const string AssetBundleManifest = "assetbundle";
        public const string BundleVersionFile = "BundleVersionFile";
        public const string MainVersionFile = "ABList";
        public const string VersionMD5File = "VersionMd5File.txt";
        public const string CsvFileRelativePath = "/GameConfig";//配置文件相对路径
        public static string UserVersionPath { get { return string.Format("{0}/{1}/Out/{2}", Application.persistentDataPath, BundleCommon.ResVersion, MainVersionFile); } }
        public static string StreamAssetVersionPath { get { return string.Format("{0}/{1}_Compress", StreamPath, MainVersionFile); } }
        public static string StreamUserVersionPath { get { return string.Format("{0}/{1}/Inner/{2}", Application.persistentDataPath, BundleCommon.ResVersion, MainVersionFile); } }
        public static string UserVersionMd5FilePath { get { return string.Format("{0}/{1}/{2}", Application.persistentDataPath, BundleCommon.ResVersion, VersionMD5File); } }

        public static string GetRandomUrl()
        {
            string getUrl = string.Empty;
            if (Application.platform != RuntimePlatform.WindowsEditor)
            {
                System.DateTime dt = System.DateTime.Now;
                // Default implementation of UNIX time of the current UTC time  
                System.TimeSpan ts = dt.ToUniversalTime() - new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                getUrl = "?" + System.Convert.ToInt64(ts.TotalSeconds).ToString();
            }
            return getUrl;
        }

    }


    public class DownLoadTask
    {
        protected string mUrl;

        protected float mDownLoadTimeOut = 30f;//超时设置
        protected float mLastProgress = 0f;
        protected float mLastTime = 0f;
        /// <summary>
        /// 是否下载超时
        /// </summary>
        /// <param name="curProgress"></param>
        /// <returns></returns>
        protected virtual bool IsTimeout(float curProgress)
        {
            float nowTime = Time.realtimeSinceStartup;

            if (nowTime - mLastTime > mDownLoadTimeOut)
            {
                return true;
            }

            if (mLastProgress != curProgress)
            {
                mLastTime = nowTime;
                mLastProgress = curProgress;
            }

            return false;
        }
    }


    public class TextDownLoadTask : DownLoadTask
    {
        private DownLoadTxtFileCallBack mDownLoadTxtFileCallBack;
        private DownLoadBundleFileProgressCallBack mProgressCallBack;

        public TextDownLoadTask(string url, DownLoadTxtFileCallBack callBack, DownLoadBundleFileProgressCallBack progressCallBack)
        {
#if UNITY_EDITOR
            mUrl = url;
#else
            mUrl = url.Replace("\\", "/");
#endif
            mDownLoadTxtFileCallBack = callBack;
            mProgressCallBack = progressCallBack;

            MonoInstance.Instance.StartCoroutine(DownLoadFile());
        }

        private IEnumerator DownLoadFile()
        {
            LogMgr.UnityLog(mUrl + " is being downloaded");

            mLastTime = Time.realtimeSinceStartup;
            mLastProgress = 0.0f;

            using (WWW www = new WWW(mUrl + "?" + UnityEngine.Random.Range(11, 999999)))
            {
                while (!www.isDone)
                {
                    if (null != mProgressCallBack)
                    {
                        mProgressCallBack(www.progress);
                    }

                    if (IsTimeout(www.progress))
                    {
                        LogMgr.UnityError(mUrl + ":download timeout!!!");
                        break;
                    }
                    else
                    {
                        yield return null;
                    }
                }

                bool isSucess = www.isDone && string.IsNullOrEmpty(www.error);

                if (!isSucess)
                {
                    LogMgr.UnityError(mUrl + ":download fail!!!");
                    LogMgr.UnityError("error info:" + www.error);
                }

                if (null != mDownLoadTxtFileCallBack)
                {
                    mDownLoadTxtFileCallBack(isSucess, isSucess ? www.text : "");
                }
            }
        }

    }


    public class BundleDownLoadTask : DownLoadTask
    {
        private DownLoadBundleFileProgressCallBack mDownLoadProgress;
        private DownLoadBytesFileCallBack mFinishCallBack;

        //private int mReconnetCount = 0;

        public BundleDownLoadTask(string url, DownLoadBytesFileCallBack finishCallBack, DownLoadBundleFileProgressCallBack progress)
        {
#if UNITY_EDITOR
            mUrl = url;
#else
            mUrl = url.Replace("\\", "/");
#endif
            mFinishCallBack = finishCallBack;
            mDownLoadProgress = progress;
			if (IsIOSCopyFile(url))
				ReadFile ();
			else
                MonoInstance.Instance.StartCoroutine(DownLoadFile());
        }
        private IEnumerator DownLoadFile()
        {
            LogMgr.UnityLog(mUrl + " is being downloaded");

            mLastTime = Time.realtimeSinceStartup;
            mLastProgress = 0.0f;

            using (WWW www = new WWW(mUrl))
            {
                while (!www.isDone)
                {
                    if (null != mDownLoadProgress)
                    {
                        mDownLoadProgress(www.progress);
                    }

                    if (IsTimeout(www.progress))
                    {
                        LogMgr.UnityError(mUrl + ":download timeout!!!");
                        break;
                    }
                    else
                    {
                        yield return null;
                    }
                }
                bool isSucess = www.isDone && string.IsNullOrEmpty(www.error);
                //if (!isSucess && ReconnetCount < 3)
                //{
                //    MonoInstance.Instance.StartCoroutine(BeginDownLoad());
                //    ReconnetCount++;
                //}
                //else
                //{
                if (!isSucess)
                {
                    LogMgr.UnityError(mUrl + ":download fail!!!");
                    LogMgr.UnityError("error info:"+www.error);
                }
                if (null != mFinishCallBack)
                {
                    mFinishCallBack(isSucess, (isSucess ? www.bytes : null));
                }
                //}
            }
        }

		void ReadFile()
		{
			bool isSucess = false;
			byte[] getBytes=null;
			if (File.Exists (mUrl)) {
				getBytes = File.ReadAllBytes(mUrl);
				isSucess = true;
			}
            LogMgr.LogError("ReadIOSFile:"+mUrl+",IsSucess:"+isSucess.ToString());
            mFinishCallBack(isSucess, getBytes);
		}
		bool IsIOSCopyFile(string url)
		{
			return Application.platform == RuntimePlatform.IPhonePlayer && !url.StartsWith ("http:");
		}
    }


    public class WWWDownloadTask
    {
        private string mUrl;
        private DownLoadBytesFileCallBack mFinishCallback;

        public WWWDownloadTask(string url, DownLoadBytesFileCallBack callback)
        {
            mUrl = url;
            mFinishCallback = callback;

            MonoInstance.Instance.StartCoroutine(DownLoadFile());
        }

        private IEnumerator DownLoadFile()
        {
            using (WWW www = new WWW(mUrl))
            {
                yield return www;

                bool isSucess = www.isDone && string.IsNullOrEmpty(www.error);

                if (!isSucess)
                {
                    LogMgr.UnityError(mUrl + ":download fail!!!");
                    LogMgr.UnityError("error info:" + www.error);
                }

                if (null != mFinishCallback)
                {
                    mFinishCallback(isSucess, (isSucess ? www.bytes : null));
                }
            }
        }
    }


    public class AssetBundleConfig
    {
        public string ABName;
        public string RelativePath;
        public string MD5Value;
        public long FileSize;
        public int Build;
    }
}

