using XLua;
﻿using SG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;



//多线程下载  下载完成后  发消息通知 GE_ThreadDownload_Complete
[Hotfix]
public class DownloadThread
{

    public DownloadThread(string url, string savePath, string _curZipMd5)
    {
        System.Net.ServicePointManager.DefaultConnectionLimit = 1000;

        init(url, savePath, _curZipMd5);

    }

    public void init(string url, string savePath, string _curZipMd5)
    {
        curZipMd5 = _curZipMd5;
        ThreadDownload(url, savePath);

    }

    public delegate void DownLoadProgreCB(string fileName, float progress);

    string fileMd5;


    string tUrl;
    string tSavePath;
    //float tProgress = 0;

    //string strProgress;


    string curZipMd5;


    string curDownloadFile;


    Thread m_thread;
    void ThreadDownload(string url, string saveFile)
    {
        if (m_thread != null)
        {
            //bool isAlive = m_thread.IsAlive;
            m_thread.Abort();
            //LogMgr.UnityLog("线程正在下载中 , 别中断"); 

        }
        m_md5RetryDownloadTimes = 0;
        tUrl = url;
        tSavePath = saveFile;
        //tProgress = 0;
        ThreadStart ts = new ThreadStart(ThreadProc);
        m_thread = new Thread(ts);
        m_thread.Start();


    }


    void ThreadProc()
    {
        string saveFile = tSavePath;
        string uri = tUrl;

        HttpWebRequest request = null;
        //打开网络连接

        try
        {

            request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Timeout = 10000;
            request.ReadWriteTimeout = 10000;




            //HttpWebRequest requestGetCount = (HttpWebRequest)HttpWebRequest.Create(uri);
            WebResponse response = request.GetResponse();

            long countLength = 0;
            countLength = response.ContentLength;

            //HttpWebRequest request = null  ;
            //HttpWebRequest requestGetCount =null ;

            //long countLength = 0;
            //while (true)
            //{
            //    try
            //    {
            //        request = (HttpWebRequest)HttpWebRequest.Create(uri);
            //        requestGetCount = (HttpWebRequest)HttpWebRequest.Create(uri);
            //        countLength = requestGetCount.GetResponse().ContentLength;

            //    }catch(Exception  e){
            //        LogMgr.UnityError(e.ToString());
            //        if (request!=null )                request.Abort();
            //        if (requestGetCount != null)                requestGetCount.Abort; 
            //        Thread.Sleep(200);
            //        continue; 
            //    }
            //    break; 
            //}




            //打开上次下载的文件或新建文件
            long lStartPos = 0;
            FileStream fs;
            if (File.Exists(saveFile))
            {
                fs = File.OpenWrite(saveFile);
                lStartPos = fs.Length;
                if (countLength - lStartPos <= 0)
                {
                    fs.Close();
                    request.Abort();
                    //requestGetCount.Abort();

                    //tProgress = 1.1f;
                    OnComplete();
                    return;

                }
                fs.Seek(lStartPos, SeekOrigin.Current);//移动文件流中的当前指针
            }
            else
            {
                fs = new FileStream(saveFile, FileMode.Create);
            }


            if (lStartPos > 0)
            {
                request.AddRange((int)lStartPos);//设置Range值
                //print(lStartPos);
            }

            //向服务器请求，获得服务器回应数据流
            Stream ns = response.GetResponseStream();
            int len = 1024 * 8;

            byte[] nbytes = new byte[len];
            int nReadSize = 0;
            nReadSize = ns.Read(nbytes, 0, len);

            float downloadLength = (float)fs.Length;
            downloadLength += nReadSize;
            while (nReadSize > 0)
            {
                //CoreEntry.logMgr.ThreadLog("多线程下载  开始:" + TimeUtil.unixTime() ); 

                fs.Write(nbytes, 0, nReadSize);
                nReadSize = ns.Read(nbytes, 0, len);
                //t = downloadString + ":" + fs.Length / 1024 + "kb/" + countLength / 1024 + "kb" + "----" + ((double)fs.Length / countLength).ToString () + "%";
                downloadLength += nReadSize;
                //tProgress = downloadLength / countLength;
                //strProgress = string.Format(@"{0:#.##}KB/{1:#.##}KB", downloadLength / 1024, countLength / 1024);


                // CoreEntry.logMgr.ThreadLog("多线程下载  结束:" +TimeUtil.unixTime()); 

            }
            ns.Close();
            fs.Flush();
            fs.Close();
            response.Close();
            request.Abort();
            //requestGetCount.Abort();




            //m_gzQueue.Enqueue(saveFile); 
            string strIn = saveFile;
            string strOut = saveFile.Substring(0, saveFile.Length - 3);


            string zipMd5 = Md5Util.GetFileHash(strIn);




            if (curZipMd5 != zipMd5 || ZipMgr.DecompressFileGz(strIn, strOut) == false) //解压失败,重新下载
            {
                m_md5RetryDownloadTimes++;
                if (m_md5RetryDownloadTimes > 3)
                {
                    //print(" 重下了3次,不能在循环了  ");
                    return;
                }

                CoreEntry.gLogMgr.ThreadLog("****** 重下:" + saveFile);


                File.Delete(saveFile);
                //tProgress = 0;
                ThreadProc();
            }
            else
            {
                string oriMd5 = Md5Util.GetFileHash(strOut);
                //strOut.LastIndexOf('/') ; 
                //int index = strOut.LastIndexOf('/');
                string oriFileName = strOut.Substring(strOut.LastIndexOf('/') + 1);

                //CoreEntry.gLogMgr.ThreadLog("---oriFileName:   删除:" + strIn);
                //string oriFileName = curDownloadFile.Substring(0, curDownloadFile.Length - 3);

                ConfigMgr.Instance.setData(oriFileName, oriMd5);
                //ConfigMgr.Instance.Flush(); 
                File.Delete(strIn);
                //tProgress = 1.1f;
                OnComplete();
            }

        }

        catch (WebException e)
        {
            CoreEntry.gLogMgr.ThreadLog(" 下载进程出错 :" + uri + "   \n" + e.ToString());

            if (request != null)
            {
                request.Abort();
            }

            m_timeoutRetryTimes++;
            if (m_timeoutRetryTimes > 10)
            {
                //(" 重下了3次,不能在循环了  ");
                return;
            }
            ThreadProc();
            return;

        }

    }

    int m_timeoutRetryTimes = 0;
    int m_md5RetryDownloadTimes = 0;

    void OnComplete()
    {
        EventParameter para = EventParameter.Get();
        para.objParameter = tUrl;

        CoreEntry.gEventMgr.TriggerEventThread((int)GameEvent.GE_ThreadDownload_Complete, para);
    }
}


