/// <summary>
/// Author: chenqingsen
/// Date: 2017-12-25
/// 输出日志到本地文件
/// </summary>

using System.IO;
using System;
using LuaInterface;
using System.Threading;
using System.Collections.Generic;

public class WriteLog : ILogger
{
    private Thread _thread;
    static readonly object _lockObj = new object();
    private string _logDir = string.Empty;
    private Queue<string> _logs = new Queue<string>();

    public WriteLog()
    {
        _logDir = Directory.GetCurrentDirectory();
        _logDir = _logDir.Substring(0, _logDir.LastIndexOf("\\"));
        _logDir = _logDir + "\\AL-Log";
        if (!Directory.Exists(_logDir))
        {
            Directory.CreateDirectory(_logDir);
        }

        _thread = new Thread(Log2Text);
        _thread.Start();
    }

    public void Log(string msg, string stack, UnityEngine.LogType type)
    {
        lock (_lockObj)
        {
            _logs.Enqueue(msg);
        }
    }

    private void Log2Text()
    {
        while (true)
        {
            lock (_lockObj)
            {
                if(_logs.Count > 0)
                {
                    string logFile = string.Concat(_logDir, "\\", DateTime.Now.ToString("d").Replace("/", "-"), "_log.log");
                    StreamWriter sw = new StreamWriter(logFile, true);
                    sw.WriteLine(string.Format("[{0}]: {1}", DateTime.Now.ToString(), _logs.Dequeue()));
                    sw.Close();
                }
                Thread.Sleep(1);
            }
        }
    }
}
