using XLua;
﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using SG;
[Hotfix]
public class DownloadHelper
{
    public delegate void DelegateDownloadFinish(bool bSuccess);
    public long AlreadyDownloadSize { get { return m_alreadyDownloadSize; } }


    private long m_alreadyDownloadSize = 0;


    private string[] m_curUrlArray;
    private string[] m_curFilePathArray;

    private int m_curLimitSizePreUnit;     // KB/S
    private DelegateDownloadFinish m_curDelFun;
    private int m_curDownloadSize = 0;

    //private Thread m_curThread = null;
    private bool m_bStop = false;
    public static DownloadHelper StartDownload(string url, string fileSavePath, DelegateDownloadFinish delFun = null, int speed = 0)
    {
        DownloadHelper helper = new DownloadHelper(url, fileSavePath, delFun, speed);
        Thread newThread = new Thread(helper.DownloadFile);
        newThread.Start();
        return helper;
    }

    public static DownloadHelper StartDownload(List<string> urlList, List<string> fileSavePathList, DelegateDownloadFinish delFun = null, int speed = 0)
    {
        DownloadHelper helper = new DownloadHelper(urlList, fileSavePathList, delFun, speed);
        Thread newThread = new Thread(helper.DownloadFile);
        newThread.Start();
        return helper;
    }

    //public void SetThread(Thread curThread)
    //{
    //    //m_curThread = curThread;
    //}

    public void Stop()
    {
        m_bStop = true;
    }
    // speed 0不限速
    private DownloadHelper(string url, string fileSavePath, DelegateDownloadFinish delFun, int speed)
    {
        m_curUrlArray = new string[1];
        m_curFilePathArray = new string[1];
        m_curFilePathArray[0] = fileSavePath;
        m_curUrlArray[0] = url;
        m_curDelFun = delFun;
        m_curLimitSizePreUnit = speed * 10 * 1024 / 1000;
        m_bStop = false;
    }

    private DownloadHelper(List<string> urlList, List<string> fileSavePathList, DelegateDownloadFinish delFun, int speed)
    {
        m_curUrlArray = new string[urlList.Count];
        for (int i = 0; i < urlList.Count; i++)
        {
            m_curUrlArray[i] = urlList[i];
        }

        m_curFilePathArray = new string[fileSavePathList.Count];
        for (int i = 0; i < fileSavePathList.Count; i++)
        {
            m_curFilePathArray[i] = fileSavePathList[i];
        }

        m_curDelFun = delFun;
        m_curLimitSizePreUnit = speed * 10 * 1024 / 1000;
    }
    

	private void DownloadFile()
    {
        if (null == m_curUrlArray || null == m_curFilePathArray)
        {
            LogMgr.UnityLog("url array or file array is null ");
            if (null != m_curDelFun) m_curDelFun(false);
            return;
        }

        if (m_curUrlArray.Length != m_curFilePathArray.Length)
        {
            LogMgr.UnityLog("url array size is not equal file path size");
            if (null != m_curDelFun) m_curDelFun(false);
            return;
        }
        try
        {
            for (int i = 0; i < m_curUrlArray.Length; i++)
            {
                if (m_bStop)
                {
                    break;
                }
                SG.Utils.CheckTargetPath(m_curFilePathArray[i]);
                FileStream fs = new FileStream(m_curFilePathArray[i], FileMode.OpenOrCreate);
                WebRequest request = WebRequest.Create(m_curUrlArray[i]);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                int bufferSize = m_curLimitSizePreUnit;
                if (bufferSize > 1024 || m_curLimitSizePreUnit == 0)
                {
                    bufferSize = 1024;
                }
                if (bufferSize < 128)
                {
                    bufferSize = 128;
                }
                int readCount;
                byte[] buffer = new byte[bufferSize];

                m_curDownloadSize = 0;
                readCount = stream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    if (m_bStop)
                    {
                        break;
                    }
                    fs.Write(buffer, 0, readCount);
                    m_alreadyDownloadSize += readCount;
                    m_curDownloadSize += readCount;
                    if (m_curDownloadSize >= m_curLimitSizePreUnit && m_curLimitSizePreUnit > 0)
                    {
                        m_curDownloadSize = 0;
                        Thread.Sleep(10);
                    }

                    readCount = stream.Read(buffer, 0, bufferSize);
                }

                stream.Close();
                fs.Close();
                response.Close();
            }

            if (null != m_curDelFun) m_curDelFun(!m_bStop);
        }
        catch (System.Exception ex)
        {
            LogMgr.UnityLog("donwload file fail. error: " + ex.ToString());
            if (null != m_curDelFun) m_curDelFun(false);
        }
    }
}

