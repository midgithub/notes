using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class EncryptRes
{
    static string resDir = string.Empty;
    static string targetDir = string.Empty;
    static string identifier = string.Empty;

    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();

    [MenuItem("BuildAppEditeTools/EncryptRes", false, 710)]
    static public void EncryptStreamRes()
    {
        resDir = Application.dataPath + "/StreamingAssets";
        if (!Directory.Exists(resDir))
        {
            Debug.LogError("资源目录不存在---");
            return;
        }

        identifier = PlayerSettings.bundleIdentifier;
        if (string.IsNullOrEmpty(identifier))
        {
            identifier = "sen";
        }

        targetDir = Directory.GetCurrentDirectory();
        targetDir = targetDir.Replace('\\', '/');
        targetDir = targetDir + "/EncryptRes/" + identifier;
        DirectoryInfo dirOutDir = new DirectoryInfo(targetDir);
        if (dirOutDir.Exists)
        {
            Directory.Delete(targetDir, true);
        }
        Directory.CreateDirectory(targetDir);

        paths.Clear();
        files.Clear();
        Recursive(resDir);

        int n = 0;
        foreach (string f in files)
        {
            EncryptFile(f);
            UpdateProgress(n++, files.Count, f);
        }
        EditorUtility.ClearProgressBar();

        Debug.Log("资源加密完成---");
    }

    private static void EncryptFile(string file)
    {
        if (!File.Exists(file))
        {
            Debug.LogError("加密文件不存在---");
            return;
        }

        string subPath = file.Replace(resDir, string.Empty);
        string targetFile = targetDir + file.Replace(resDir, string.Empty);
        if (File.Exists(targetFile)) File.Delete(targetFile);

        string dir = Path.GetDirectoryName(targetFile);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
        byte[] buff = new byte[fs.Length];
        fs.Read(buff, 0, (int)fs.Length);
        fs.Close();

        byte[] encryptBuff = AES.AESEncrypt(buff, identifier);
        FileStream newFile = new FileStream(targetFile, FileMode.Create);
        newFile.Write(encryptBuff, 0, encryptBuff.Length);
        newFile.Close();

        buff = null;
        encryptBuff = null;
    }

    static void UpdateProgress(int progress, int progressMax, string desc)
    {
        string title = "Processing...[" + progress + " - " + progressMax + "]";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }

    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);

        string ext = string.Empty;
        foreach (string filename in names)
        {
            ext = Path.GetExtension(filename);
            if (ext.Equals(AppConst.ExtName) || string.IsNullOrEmpty(ext)) 
            {
                files.Add(filename.Replace('\\', '/'));
            }
        }

        foreach (string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir.Replace('\\', '/'));
        }
    }
}
