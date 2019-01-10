using XLua;
﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using SG; 

[Hotfix]
public class UpdateHelper : MonoBehaviour
{

    public enum UpdateResult
    {
        NONE,
        Success,                    // 更新成功
        GetVersionFail,             // 获取版本号失败
        GetFileListFail,            // 获取文件列表失败
        DownloadInComplete,         // 下载文件不完全
        DownloadFail,               // 下载文件失败
        CopyDataFileFail,           // 拷贝文件失败
        GenerateVersionFileFail,    // 生成版本号文件失败
        GenerateFileListFail,       // 生成文件列表文件失败
        CleanCacheFail,             // 生成文件列表文件失败
        LoadRomoteFailListError,    // 读取下载的文件列表失败
    }

    public enum UpdateStep
    {
        NONE,
        CheckVersion,
        GetFileList,
        CompareRes,
        AskIsDonwload,               // 询问是否下载文件
        DownloadRes,
        CheckRes,
        CopyRes,
        CleanCache,
        FINISH,
    }

[Hotfix]
    public class FileInfo
    {
        public FileInfo(string _md5, long _size)
        {
            md5 = _md5;
            size = _size;
        }

        public FileInfo()
        {

        }
        public string md5;
        public long size;
    }

    public UpdateStep CurUpdateStep { get { return m_curUpdateStep; } }
    public UpdateResult CurUpdateResult { get { return m_curUpdateResult; } }
    public long DownloadTotalSize { get {return m_totalNeedDownloadSize;} }
    public long CurDownloadSize {get{return (null != m_dataFileDownloader) ? m_dataFileDownloader.AlreadyDownloadSize : 0;}}



    public static string ResCachePath = Application.temporaryCachePath + "/tlbbres";

    public static string AppVersionPath = Application.streamingAssetsPath + "/VersionData";        // APP自带版本资源路径 
    public static string LocalVersionPath = Application.persistentDataPath + "/VersionData";       // 本地保存的版本资源路径
    public static string CacheVersionPath = ResCachePath + "/VersionData";                // 下载的远程版本资源路径
    public static string DownloadDataFolder = "StreamingAssets";

    public static string VersionFileName = "ResVersion.txt";
    public static string ResFileListName = "update.info";

    private static string LocalPathRoot = ""; //BundleManager.LocalPathRoot;
    delegate void DelegateGetResVersion(bool bSuccess);
	delegate void DelegateDownFileList(bool bSuccess);

	private string m_resServerUrl = "http://127.0.0.1:8080/tlbb/res/";
	private string m_cacheDataPath = ResCachePath + "/"+ DownloadDataFolder;
	private string m_remoteDataPath;
	private int m_remoteVersion = -1;
	private Dictionary<string, FileInfo> m_dicLocalFiles = new Dictionary<string, FileInfo>();
    private Dictionary<string, FileInfo> m_dicRemoteFiles = new Dictionary<string, FileInfo>();
    private List<string> m_listUpdateFiles = new List<string>();
    private List<string> m_listUpdateErrorFiles = new List<string>();
    private List<string> m_listDownloadFiles = new List<string>();
    private List<string> m_listDownloadFileUrls = new List<string>();

    private DownloadHelper m_versionFileDownloader = null;
    private DownloadHelper m_dataFileDownloader = null;

    private UpdateStep m_curUpdateStep = UpdateStep.NONE;
    private UpdateResult m_curUpdateResult = UpdateResult.NONE;
    private long m_totalNeedDownloadSize = 0;

    public void StartCheckRes(string resServerUrl)
    {
        m_remoteVersion = -1;
        if (null != m_versionFileDownloader)
        {
            m_versionFileDownloader.Stop();
        }

        if (null != m_dataFileDownloader)
        {
            m_dataFileDownloader.Stop();
        }
		m_resServerUrl = resServerUrl;
		m_remoteDataPath = m_resServerUrl + "/" + DownloadDataFolder;;
		m_remoteVersion = -1;
		m_versionFileDownloader = null;
        m_dataFileDownloader = null;
        m_totalNeedDownloadSize = 0;
        m_curUpdateStep = UpdateStep.CheckVersion;
        StartCoroutine(GetResVersion(OnCheckVersion));
    }

    public void DownloadCurFileList()
    {
        m_curUpdateStep = UpdateStep.DownloadRes;
        Debug.Log("have diff files");
        m_dataFileDownloader = DownloadHelper.StartDownload(m_listDownloadFileUrls, m_listDownloadFiles, OnDownloadRes);
    }
    // 获取版本号
    IEnumerator GetResVersion(DelegateGetResVersion delFun)
    {
		LogMgr.UnityLog("ger version form " + m_resServerUrl + "/" + VersionFileName);
		WWW wwwVersionFile = new WWW(m_resServerUrl + "/" + VersionFileName);

        yield return wwwVersionFile;
        if (null != wwwVersionFile.error)
        {
            LogMgr.UnityLog(wwwVersionFile.error);
            delFun(false);
        }
        else
        {
            if (!int.TryParse(wwwVersionFile.text, out m_remoteVersion))
            {
                delFun(false);
            }
            else
            {
				delFun(true);
			}
		}
	}

    // 获取版本号回调
    void OnCheckVersion(bool bSuccess)
	{

		if(bSuccess)
		{
            LogMgr.UnityLog("comparing version");

            // 比较版本号
            int m_localVerion = -1;
            string localVersionFilePath = LocalVersionPath + "/" + VersionFileName;
            string AppVersionFilePath = AppVersionPath + "/" + VersionFileName;
            string curVersionFilePath = null;
            if (File.Exists(localVersionFilePath))
            {
                curVersionFilePath = localVersionFilePath;
            }
            else if (File.Exists(AppVersionFilePath))
            {
                curVersionFilePath = AppVersionFilePath;
            }

            if (null != curVersionFilePath)
            {
                FileStream fs = new FileStream(curVersionFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                StreamReader sr = new StreamReader(fs);
                string localVersion = sr.ReadLine();
                sr.Close();
                fs.Close();
                if (!int.TryParse(localVersion, out m_localVerion))
                {
                    LogMgr.UnityLog("parse version error");
                }
            }

			LogMgr.UnityLog("version --local:"+ m_localVerion.ToString() + " --remote:" + m_remoteVersion);
			if (m_remoteVersion > m_localVerion)
            {
                LogMgr.UnityLog("remote version is big than local, begin update");

                m_curUpdateStep = UpdateStep.GetFileList;

                // 下载文件列表
				m_versionFileDownloader = DownloadHelper.StartDownload(m_resServerUrl + "/" + ResFileListName, CacheVersionPath + "/" + ResFileListName, OnDownloadResFileList);
            }
			else
			{
				UpdateFinish(UpdateResult.Success);
			}
		}
		else
		{
			// error
            LogMgr.UnityLog("check verion fail");
            UpdateFinish(UpdateResult.GetVersionFail);
            // 严重错误，可能网络不通，需要重新请求
		}
		
	}
	
    // 下载文件列表回调
	void OnDownloadResFileList(bool bSuccess)
	{
		if(bSuccess)
		{
            m_curUpdateStep = UpdateStep.CompareRes;

            // 远程文件列表请求成功，开始查找需要下载的资源

            LogMgr.UnityLog("load file success: " + CacheVersionPath + "/" + ResFileListName);
            string localFileListName = LocalVersionPath + "/" + ResFileListName;
            string appFileListName = AppVersionPath + "/" + ResFileListName;
            string cacheFileListName = CacheVersionPath + "/" + ResFileListName;

            // 如果读取目录没有文件，将包体文件拷贝出
            if (!File.Exists(localFileListName))
			{
                // 将版本中的资源文件拷贝到目标位置
                if (File.Exists(appFileListName))
                {
                    SG.Utils.CheckTargetPath(localFileListName);
                    File.Copy(appFileListName, localFileListName);
                }
			}

			m_dicLocalFiles.Clear();
			m_dicRemoteFiles.Clear();
			m_listUpdateFiles.Clear();

			// 将资源列表中所有的文件存入dic

            // 本地文件读取失败不处理，当做空
            ReadVersionFileToDic(localFileListName, m_dicLocalFiles);

            // 远程文件读取失败，需要重新下载
            if (!ReadVersionFileToDic(cacheFileListName, m_dicRemoteFiles))
            {
                UpdateFinish(UpdateResult.LoadRomoteFailListError);
                // 严重错误，可能网络不通，需要重新请求
                // 退出重新下载逻辑
                return;
            }

            // 对比文件列表，找出本地没有的资源
			foreach(KeyValuePair<string, FileInfo> curPair in m_dicRemoteFiles)
			{
                // 如果远程列表包含，本地列表不包含，则放入更新文件列表
				if(m_dicLocalFiles.ContainsKey(curPair.Key))
				{
					if(m_dicLocalFiles[curPair.Key].md5 != curPair.Value.md5)
					{
						m_listUpdateFiles.Add(curPair.Key);
					}
				}
				else
				{
					m_listUpdateFiles.Add(curPair.Key);
				}
			}

			if(m_listUpdateFiles.Count > 0)
			{
                // 下载更新文件

                m_listDownloadFileUrls.Clear();
                m_listDownloadFiles.Clear();
                m_totalNeedDownloadSize = 0;
                foreach (string curFile in m_listUpdateFiles)
                {
                    // 检查缓存中是否已经有下载好的资源，如果有缓存文件，并且MD5值正确，则跳过
                    string localFilePath = m_cacheDataPath + "/" + curFile;
                    if (!File.Exists(localFilePath) || SG.Utils.GetMD5Hash(localFilePath).ToLower() != m_dicRemoteFiles[curFile].md5.ToLower())
                    {
                        m_listDownloadFileUrls.Add(m_remoteDataPath + "/" + curFile);
                        m_listDownloadFiles.Add(localFilePath);
                        m_totalNeedDownloadSize += m_dicRemoteFiles[curFile].size;
                    }
                }

                // 如果下载列表大于0 开始下载
                if (m_listDownloadFiles.Count > 0)
                {
                    m_curUpdateStep = UpdateStep.AskIsDonwload;
                    return;
                }
			}
            // 没有需要下载的文件，开始拷贝
            OnDownloadRes(true);
		}
		else
		{
			// 下载远程文件列表失败，应该重新请求
            UpdateFinish(UpdateResult.GetFileListFail);
		}
	}

    

    // 下载资源回调
    void OnDownloadRes(bool bSuccess)
    {
        if (bSuccess)
        {
            LogMgr.UnityLog("check res md5");
            m_curUpdateStep = UpdateStep.CheckRes;
            // 下载所有文件成功，开始检查文件，将MD5错误或未下载文件放入错误文件列表
            m_listUpdateErrorFiles.Clear();
            foreach (string updateFileName in m_listUpdateFiles)
            {
                string curFilePath = m_cacheDataPath + "/" + updateFileName;
                if (!File.Exists(curFilePath))
                {
                    LogMgr.UnityLog("download file fail " + updateFileName);
                    m_listUpdateErrorFiles.Add(updateFileName);
                }
                else
                {
                    string curFileMD5 = SG.Utils.GetMD5Hash(curFilePath);
                    if (!m_dicRemoteFiles.ContainsKey(updateFileName) || curFileMD5.ToLower() != m_dicRemoteFiles[updateFileName].md5.ToLower())
                    {
                        LogMgr.UnityLog("download file md5 error : romote:" + m_dicRemoteFiles[updateFileName].md5 + " local :" + curFileMD5);
                        m_listUpdateErrorFiles.Add(updateFileName);
                    }
                }
            }

            if (m_listUpdateErrorFiles.Count == 0)
            {
                // 没有错误文件，开始拷贝过程
                LogMgr.UnityLog("check res success");
                CopyResToDataPath();
            }
            else
            {
                // 有文件发生错误，应该重新比对下载
                LogMgr.UnityLog("check res fail");
                UpdateFinish(UpdateResult.DownloadInComplete);
            }
        }
        else
        {
            // 下载资源失败，应该重新比对下载
            LogMgr.UnityLog("download res  error");
            UpdateFinish(UpdateResult.DownloadFail);
        }
    }

    // 拷贝资源
    void CopyResToDataPath()
    {
        try
        {
            LogMgr.UnityLog("copy res");
            m_curUpdateStep = UpdateStep.CopyRes;
            foreach (string updateFileName in m_listUpdateFiles)
            {
                string localFilePath = LocalPathRoot + "/" + updateFileName;
                SG.Utils.CheckTargetPath(localFilePath);
                File.Copy(m_cacheDataPath + "/" + updateFileName, localFilePath, true);
            }
        }
        catch (System.Exception ex)
        {
            // 拷贝文件失败，这该咋办
            LogMgr.UnityLog("copy res error!" + ex.ToString());
            UpdateFinish(UpdateResult.CopyDataFileFail);
            return;
        }

        CopyFileListFils();
    }

    // 拷贝文件列表
    void CopyFileListFils()
    {
        try
        {
            LogMgr.UnityLog("copy filelist");
            string localFileListName = LocalVersionPath + "/" + ResFileListName;
            string cacheFileListName = CacheVersionPath + "/" + ResFileListName;

            SG.Utils.CheckTargetPath(localFileListName);
            File.Copy(cacheFileListName, localFileListName, true);
        }
        catch (System.Exception ex)
        {
            LogMgr.UnityLog("generate version file fail!" + ex.ToString());
            UpdateFinish(UpdateResult.GenerateFileListFail);
            return;
        }

        CopyVersionFile();
    }

    // 生成版本文件
    void CopyVersionFile()
    {
        try
        {
            LogMgr.UnityLog("copy version file");
            string localVersionFilePath = LocalVersionPath + "/" + VersionFileName;
            SG.Utils.CheckTargetPath(localVersionFilePath);
            FileStream fs = new FileStream(localVersionFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(m_remoteVersion.ToString());
            sw.Close();
            fs.Close();
        }
        catch (System.Exception ex)
        {
            LogMgr.UnityLog("generate version file fail!" + ex.ToString());
            UpdateFinish(UpdateResult.GenerateVersionFileFail);
            return;
        }

        CleanCacheFiles();
    }

    // 清理缓存
    void CleanCacheFiles()
    {
        LogMgr.UnityLog("clean cache");
        m_curUpdateStep = UpdateStep.CleanCache;
        try
        {
            SG.Utils.DeleteFolder(m_cacheDataPath);
            SG.Utils.DeleteFolder(CacheVersionPath);
        }
        catch (System.Exception ex)
        {
            LogMgr.UnityLog("clean cache fail!" + ex.ToString());
            UpdateFinish(UpdateResult.CleanCacheFail);
            return;
        }

        m_curUpdateStep = UpdateStep.FINISH;
        UpdateFinish(UpdateResult.Success);
    }
    
    bool ReadVersionFileToDic(string versionPath, Dictionary<string, FileInfo> curDic)
	{
        try
        {
            if (!File.Exists(versionPath))
            {
                return false; ;
            }

            XmlDocument xml = new XmlDocument();

            xml.Load(versionPath);
            XmlNode fileListNode = null;
            foreach (XmlNode elem in xml.ChildNodes)
            {
                LogMgr.UnityLog(elem.Name);
                if (elem.Name == "FileList")
                {
                    fileListNode = elem;
                    break;
                }
            }

            if (null == fileListNode)
            {
                return false;
            }

            foreach (XmlNode elemPath in fileListNode.ChildNodes)
            {
                FileInfo curFileInfo = new FileInfo();
                string curPath = null;
                foreach (XmlNode fileInfo in elemPath.ChildNodes)
                {
                    if (fileInfo.Name == "md5")
                    {
                        curFileInfo.md5 = fileInfo.InnerText;
                    }
                    else if (fileInfo.Name == "size")
                    {
                        long.TryParse(fileInfo.InnerText, out curFileInfo.size);
                    }
                    else if (fileInfo.Name == "path")
                    {
                        curPath = fileInfo.InnerText;
                    }
                }
                if (null != curPath)
                {
                    curDic.Add(curPath, curFileInfo);
                }
                
            }
            return true;
        }
        catch (System.Exception e)
        {
            LogMgr.UnityLog("read version file error :" + versionPath + " e: " + e.ToString());
            return false;
        }
    }

    void UpdateFinish(UpdateResult result)
    {
        m_curUpdateStep = UpdateStep.FINISH;
        m_curUpdateResult = result;
    }
}

