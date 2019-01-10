using System.IO;
using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using SG;

public class RecordeBundles
{
    //单例模式. 
    #region 
    private static RecordeBundles _instance;
    public static RecordeBundles Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = new RecordeBundles();
            }

            return _instance;
        }
    }
    #endregion

    private bool _recordeBundle = false;

    private Thread _thread;
    static readonly object _lockObj = new object();
    private string _recordFile = string.Empty;
    private Queue<string> _subBundles = new Queue<string>();
    private DateTime startTime;
    private List<string> _bundles = new List<string>();

    private RecordeBundles()
    {
//#if UNITY_EDITOR
        _recordeBundle = ClientSetting.Instance.GetBoolValue("RecordeBundle");
        if (_recordeBundle)
        {
            string recordDir = Util.DataPath + "FirstResTool/Config";
            if (!Directory.Exists(recordDir))
            {
                Directory.CreateDirectory(recordDir);
            }

            string time = DateTime.Now.ToString("MMdd_HHmm");
            _recordFile = string.Concat(recordDir, "/", time, "_log.log");
            if (File.Exists(_recordFile))
            {
                File.Delete(_recordFile);
            }

            Recorde("UI_Bundles/ui/prefabs/versionupdate.unity3d");

            startTime = DateTime.UtcNow;
            _thread = new Thread(Recorde2Text);
            _thread.Start();
        }
//#endif
    }

    public void Recorde(string path)
    {
        if (_recordeBundle)
        {
//#if UNITY_EDITOR
            lock (_lockObj)
            {
                string file = path.Replace(Util.DataPath, "").Replace(Util.AppContentPath(), "");
                LogMgr.Log("RecordeBundle:" + file);
                if (!_bundles.Contains(file))
                {
                    _bundles.Add(file);
                    _subBundles.Enqueue(file);
                }
            }
//#endif
        }
    }

    private void Recorde2Text()
    {
        while (true)
        {
            lock (_lockObj)
            {
                if (_subBundles.Count > 0)
                {
                    double minu = DateTime.UtcNow.Subtract(startTime).TotalMinutes;
                    StreamWriter sw = new StreamWriter(_recordFile, true);
                    for (int i = 0; i < _subBundles.Count; i++)
                    {
                        sw.WriteLine(string.Format("{0}minute:{1}", Mathf.FloorToInt((float)minu) + 1, _subBundles.Dequeue()));
                    }
                   
                    sw.Close();
                }
            }
            Thread.Sleep(1000);
        }
    }
}
