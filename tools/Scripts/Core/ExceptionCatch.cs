using XLua;
﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

[Hotfix]
public class ExceptionCatch : MonoBehaviour
{



    //private bool badSetup = false;

    string m_logfilePath;
    StreamWriter m_logFile;
    void Awake()
    {
       
        //Application.RegisterLogCallback(OnLog);
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            int pos = Application.dataPath.LastIndexOf("/Assets");
            if (pos > -1)
            {
                string projectPath = Application.dataPath.Substring(0, pos);

                m_logfilePath = projectPath + "/crash" + ".log";
                m_logFile = new StreamWriter(m_logfilePath, false);
                m_logFile.WriteLine("\n\n\n");
            }
        }
    }


    void OnLog(string message, string stacktrace, LogType type)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            if (m_logFile != null)
            {
                m_logFile.WriteLine(type.ToString());

                m_logFile.WriteLine(message);
                m_logFile.WriteLine(stacktrace);

                m_logFile.Flush();

                if (message.IndexOf("UnityException: Input Axis") == 0 ||
                    message.IndexOf("UnityException: Input Button") == 0
                )
                {
                    //处理异常信息
                }
            }
        }
    }

 

}



