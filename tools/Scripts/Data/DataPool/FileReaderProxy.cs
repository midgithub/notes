using XLua;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
namespace SG
{
  public delegate byte[] delegate_ReadFile(string path);

[Hotfix]
  public static class FileReaderProxy
  {
    private static delegate_ReadFile handlerReadFile;

    public static MemoryStream ReadFileAsMemoryStream(string filePath)
    {
      try {
        byte[] buffer = ReadFileAsArray(filePath);
        if (buffer == null)
        {
            string info = string.Format("Err ReadFileAsMemoryStream failed:{0}\n", filePath);
            LogMgr.UnityLog(info);
            //CoreEntry.gLogMgr.Log(LogLevel.ERROR, "Excepton", info);

            return null;
        }
        return new MemoryStream(buffer);
      } catch (Exception e) {

        LogMgr.UnityLog("Excepton" +  e.Message);
       //CoreEntry.gLogMgr.Log(LogLevel.INFO, "Excepton", e.Message);

        Helper.LogCallStack();
        return null;
      }
    }

    public static byte [] ReadFileAsArray(string filePath)
    {
      byte[] buffer = null;
      try {
        if (handlerReadFile != null) {
          buffer = handlerReadFile(filePath);
        } else {
            LogMgr.UnityLog("ReadFileByEngine handler have not register: " +  filePath);
          //LogSystem.Debug("ReadFileByEngine handler have not register: {0}", filePath);
        }
      } catch (Exception e) {
        LogMgr.UnityLog("Excepton" +  e.Message);
        //CoreEntry.gLogMgr.Log(LogLevel.INFO, "Excepton", e.Message);
        Helper.LogCallStack();
        return null;
      }
      return buffer;
    }

    public static bool Exists(string filePath)
    {
      return File.Exists(filePath);
    }

    public static void RegisterReadFileHandler(delegate_ReadFile handler)
    {
      handlerReadFile = handler;
    }

  }
}

