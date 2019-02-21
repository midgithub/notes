using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using XLua;

[Hotfix]
public static class FileHelper
{
    static List<string> m_bigBundles = new List<string>();
    public static void AddBigBundleName(string name)
    {
        if (!m_bigBundles.Contains(name))
        {
            m_bigBundles.Add(name);
        }
    }

    private static string CheckInBigBundles(string path)
    {
        for (int i = 0; i < m_bigBundles.Count; i++)
        {
            if (path.Contains(m_bigBundles[i]))
            {
                path = m_bigBundles[i];
                break;
            }
        }

        return path;
    }

    public static string SearchFilePath(string manifest, string subpath)
	{
        string fullpath = string.Empty;

		if (LoadModule.Instance != null) {
			if (LoadModule.Instance.ManifestHasBundleInfo(subpath)) {
                fullpath = string.Format("{0}{1}/{2}/{3}", Util.DataPath, CVersionManager.STR_VERSION_DIR, manifest, subpath);
			} else {
                Util.LogWarning("SearchFilePath: null");
                return string.Empty;
            }
        } else {
            // 没有找到,直接用包里面的
            fullpath = string.Format("{0}{1}/{2}/{3}", Util.DataPath, CVersionManager.STR_VERSION_DIR, manifest, subpath);
        }

        if (!File.Exists(fullpath))
        {
            fullpath = string.Format("{0}{1}/{2}/{3}", Util.DataPath, CVersionManager.STR_SUBPACKAGE_DIR, manifest, subpath);
        }

        if (!File.Exists(fullpath))
        {
            fullpath = string.Format("{0}{1}/{2}", Util.DataPath, manifest, subpath);
        }

        if (!File.Exists(fullpath))
        {
            return string.Format("{0}{1}/{2}", Util.AppContentPath(), manifest, subpath); 
        }

        Util.Log("SearchFilePath: " + fullpath);
        return fullpath;
    }
	
    public static string GetWWWPath(string path)
    {
        bool addFileHead = true;

#if UNITY_ANDROID && !UNITY_EDITOR
        // 如果是读取apk里的资源,不需要加file:///,其它情况都要加
        if (path.Contains (Application.streamingAssetsPath)) {
            addFileHead = false;
        }
#endif

        if (addFileHead) {
            path = string.Format ("file:///{0}", path);
        }

        Util.Log("GetWWWPath: " + path);
        return path;
    }

    /// <summary>
    /// 检测包的扩展名和小写 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
	public static string CheckBundleName(string path)
	{
        path = CheckInBigBundles(path);
        path = path.ToLower();

        if(path.StartsWith("data/") || path.StartsWith("lua/"))
        {
            path = path.Substring(0,path.LastIndexOf('/'));
        }

        if (path.StartsWith("ui/prefabs/"))
        {
            string subPath = path.Replace("ui/prefabs/", "");
            string[] items = subPath.Split('/');
            path = "ui/prefabs/" + subPath.Replace("/" + items[items.Length - 1], "");
        }

        if (path.StartsWith("sound/") && !path.StartsWith("sound/scene/"))
        {
            string subPath = path.Replace("sound/", "");
            string[] items = subPath.Split('/');
            path = "sound/" + subPath.Replace("/" + items[items.Length - 1], "");
        }

        string name = path;

        if (!path.Contains(AppConst.ExtName))
        {
            name = string.Format("{0}{1}", path, AppConst.ExtName);
            Util.Log(string.Format("GenBundlePath,before:{0},  after:{1}", path, name));
        }

        return name.ToLower();
	}

    public static string GetAPKPath(string path)
    {
#if UNITY_IOS
        path = CheckMjPath(path);
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        path = path.Replace (Application.streamingAssetsPath, Application.dataPath + "!assets");
#endif
        Util.Log("GetAPKPath: " + path);
        return path;
    }

    private static string CheckMjPath(string path)
    {
        string suffix = AppConst.BundleFlag;
        if (string.IsNullOrEmpty(suffix) || path.Contains(Util.DataPath))
        {
            return path;
        }
        else
        {
            path = path.Replace(Util.AppContentPath(), "");

            var dirs = path.Split('/');
            string fileName = string.Empty;
            for (int i = 0; i < dirs.Length; i++)
            {
                if (i != dirs.Length - 1)
                {
                    fileName = fileName + string.Format("{0}_{1}/", dirs[i], suffix);
                }
                else
                {
                    if (dirs[dirs.Length - 1].Contains("."))
                    {
                        var arr = dirs[dirs.Length - 1].Split('.');
                        fileName = fileName + string.Format("{0}_{1}.{2}", arr[0], suffix, arr[1]);
                    }
                    else
                    {
                        fileName = fileName + dirs[dirs.Length - 1];
                    }
                }
            }
            return Util.AppContentPath() + fileName;
        }
    }

	public static bool CheckFileExist(string path, bool isFile = true)
	{
		bool exist = false;
		if (isFile)
		{
			exist = File.Exists(path);
		}
		else {
			exist = Directory.Exists(path);
		}

		if (!exist)
		{
			return false;
		}

		return true;
	}

    public static string LoadFile(string path)
    {
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(path);
        }
        catch (Exception e)
        {
            return string.Empty;
        }

        string str = sr.ReadToEnd();
        sr.Close();
        sr.Dispose();

        return str;
    }

    public static byte[] LoadFileBytes(string path)
    {
        byte[] bytes = null;
        try
        {
            bytes = File.ReadAllBytes(path);
        }
        catch (Exception e)
        {
            Util.LogError(e.Message);
        }

        return bytes;
    }
}