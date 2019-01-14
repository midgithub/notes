using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using SenLib;
using XLua;

[Hotfix]
public static class ZipHelper
{
#if UNITY_IPHONE && !UNITY_EDITOR // IOS
    [DllImport("__Internal")]
    public static extern bool UnCompress(string strSrcFile, string strDestDire);
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
    [DllImport("zip")]
    public static extern bool UnCompress(string strSrcFile, string strDestDire);
#elif UNITY_ANDROID
    [DllImport("tqzip")]
    public static extern bool UnCompress(string strSrcFile, string strDestDire);
#endif

    public static bool UncompressZip(string strSrcFile, string strDestDire)
    {
        return Helper.UncompressZip(strSrcFile, strDestDire);
    }
}
