using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;
using SG;
using XLua;


[Hotfix]
public static class Util {

    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    public static string md5file(string file) {
        try {
            FileStream fs = new FileStream(file, FileMode.Open,FileAccess.Read);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++) {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        } catch (Exception ex) {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }

    ///   <summary>
    ///   给一个字符串进行MD5加密
    ///   </summary>
    ///   <param   name="strText">待加密字符串</param>
    ///   <returns>加密后的字符串</returns>
    public static string md5String(string strText)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] result = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(strText));
        StringBuilder strbul = new StringBuilder(40);
        // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串 
        for (int i = 0; i < result.Length; i++)
        {
            strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
        }
        return strbul.ToString();
    }

    /// <summary>
    /// 取得数据存放目录
    /// </summary>
    public static string DataPath {
        get {
            string game = AppConst.AppName.ToLower();
            if (Application.isMobilePlatform) {
                return Application.persistentDataPath + "/" + game + "/";
            }
            if (AppConst.DebugMode) {
                return Application.dataPath + "/" + AppConst.AssetDir + "/";
            }
            if (Application.platform == RuntimePlatform.OSXEditor) {
                int i = Application.dataPath.LastIndexOf('/');
                return Application.dataPath.Substring(0, i + 1) + game + "/";
            }
            return "c:/" + game + "/";
        }
    }

    /// <summary>
    /// 应用程序内容路径
    /// </summary>
    public static string AppContentPath() {
        string path = string.Empty;
        switch (Application.platform) {
            case RuntimePlatform.Android:
                path = "jar:file://" + Application.dataPath + "!/assets/";
            break;
            case RuntimePlatform.IPhonePlayer:
                path = Application.dataPath + "/Raw/";
            break;
            default:
                path = Application.dataPath + "/" + AppConst.AssetDir + "/";
            break;
        }
        return path;
    }

    public static bool IsTexture(this string s)
    {
        return s.EndsWith("tga", true, System.Globalization.CultureInfo.CurrentCulture) ||
            s.EndsWith("png", true, System.Globalization.CultureInfo.CurrentCulture) ||
            s.EndsWith("jpg", true, System.Globalization.CultureInfo.CurrentCulture) ||
            s.EndsWith("bmp", true, System.Globalization.CultureInfo.CurrentCulture);
    }

    public static void Log(string message)
    {
        LogMgr.DebugLog(message);
    }

    public static void LogWarning(string message)
    {
        LogMgr.LogWarning(message);
    }

    public static void LogError(string message)
    {
        UnityEngine.Debug.LogError(message);
    }

    public static string GetUTF8StringWithoutBom(byte[] buffer)
    {
        if (buffer == null) return null;
        if (buffer.Length <= 3)
        {
            return Encoding.UTF8.GetString(buffer);
        }

        if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
        {
            return new UTF8Encoding(false).GetString(buffer, 3, buffer.Length - 3);
        }
        return Encoding.UTF8.GetString(buffer);
    }

    /// <summary>
    /// 尝试将 bytes 解析为 xml
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="root"></param>
    /// <returns></returns>
    public static bool TryParse(this byte[] bytes, out XML root)
    {
        if (bytes == null)
        {
            root = null;
            return false;
        }

        string str = GetUTF8StringWithoutBom(bytes);
        root = new XML(str);
        return true;
    }

    // 获取操作系统
    public static string GetOS()
    {
#if UNITY_ANDROID
        return "Android";
#elif UNITY_IPHONE
        return "IOS";
#else
        return "Windows";
#endif
    }

    /// <summary>
    /// 获取 Text，异常情况下为空字符串
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static string GetText(this XML node)
    {
        if (node == null)
        {
            return string.Empty;
        }

        string vaule = node.text;
        return vaule != null ? vaule.Replace("\\n", "\n") : string.Empty;
    }

    /// <summary>
    /// 获取 Text，异常情况下为空字符串
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static string GetCureText(this XML node)
    {
        string str = GetText(node);

        if (string.IsNullOrEmpty(str))
        {
            return string.Empty;
        }

        str = str.Replace("\r", "");
        str = str.Replace("\n", "");
        str = str.Trim();

        return str;
    }

    /// <summary>
    /// 获取属性，异常情况下为空字符串
    /// </summary>
    /// <param name="node"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetValue(this XML node, string name)
    {
        if (node == null)
        {
            return string.Empty;
        }
        string vaule = node.GetAttribute(name);
        return !string.IsNullOrEmpty(vaule) ? vaule.Replace("\\n", "\n") : string.Empty;
    }

    /// <summary>
    /// 获取属性，异常情况下为传入的默认值 <paramref name="defValue"/> (缺省情况下为 false),
    /// </summary>
    /// <param name="node"></param>
    /// <param name="name"></param>
    /// <param name="defValue"></param>
    /// <returns></returns>
    public static bool GetBool(this XML node, string name, bool defValue = false)
    {
        if (node == null)
        {
            return false;
        }

        return node.GetAttributeBool(name, defValue);
    }


    /// <summary>
    /// 获取属性，异常情况下为 0
    /// </summary>
    /// <param name="node"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static int GetInt(this XML node, string name)
    {
        if (node == null)
        {
            return 0;
        }

        return node.GetAttributeInt(name, 0);
    }

    /// <summary>
    /// 获取属性，异常情况下为 0.0f
    /// </summary>
    /// <param name="node"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static float GetFloat(this XML node, string name)
    {
        if (node == null)
        {
            return 0f;
        }

        return node.GetAttributeFloat(name, 0f);
    }

    // DateTime转换为时间戳
    public static int GetTimeStamp(DateTime date)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        return (int)(date - startTime).TotalSeconds;
    }

    public static IntPtr UTF8StringToIntptr(string text)
    {
        byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(text);
        byte[] ansi = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding("gb18030"), utf8);
        return BytesToIntptr(ansi);
    }

    public static IntPtr BytesToIntptr(byte[] bytes)
    {
        int size = bytes.Length;
        IntPtr buffer = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
        System.Runtime.InteropServices.Marshal.Copy(bytes, 0, buffer, size);
        return buffer;
    }

    public static int GetInt(string vaule)
    {
        int i;
        return int.TryParse(vaule, out i) ? i : 0;
    }

    /// <summary>
    /// C#提供的文件的大小是以B为单位的
    /// 函数说明，
    ///     如果文件大小是0-1024B 以内的   显示以B为单位
    ///     如果文件大小是1KB-1024KB之间的 显示以KB为单位
    ///     如果文件大小是1M-1024M之间的   显示以M为单位
    ///     如果文件大小是1024M以上的      显示以GB为单位
    /// </summary>
    /// <param name="lengthOfDocument"> 文件的大小 单位：B 类型：long</param>
    /// <returns></returns>
    public static string GetFileLengthStr(long lengthOfDocument)
    {
        if (lengthOfDocument < 1024)
            return string.Format(lengthOfDocument.ToString() + 'B');
        else if (lengthOfDocument > 1024 && lengthOfDocument <= Math.Pow(1024, 2))
            return string.Format(Math.Round(lengthOfDocument / 1024.0,2).ToString() + "KB");
        else if (lengthOfDocument > Math.Pow(1024, 2) && lengthOfDocument <= Math.Pow(1024, 3))
            return string.Format(Math.Round(lengthOfDocument / 1024.0 / 1024.0,2).ToString() + "M");
        else
            return string.Format(Math.Round(lengthOfDocument / 1024.0 / 1024.0 / 1024.0,2).ToString() + "GB");
    }
}