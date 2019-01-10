#if UNITY_EDITOR
/**
* @file     : #FileName#
* @brief    : 
* @details  : 
* @author   : #Author#
* @date     : #Date#
*/

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Bundle;
using System.Xml;
using SG;

public class ResourceRecorder
{
    private static ResourceRecorder instance;
    public static ResourceRecorder Instance
    {
        get
        {
            if(null == instance)
            {
                instance = new ResourceRecorder();
            }

            return instance;
        }
    }

    private ResourceRecorder()
    {

        Debug.Log(Application.dataPath);
        mRecordFilePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets")).Replace("\\", "/") + "/Resources Usage/Log.txt";
        mResourcesList = new List<string>();
        Debug.Log(mRecordFilePath);
        ReadFile();
    }

    private string mRecordFilePath = string.Empty;
    private List<string> mResourcesList = null;

    public void RecordResource(string resPath, System.Type type)
    {
        return;
        string item = string.Empty;
        for (int i = 0; i < mResourcesList.Count; i++)
        {
            if (mResourcesList[i].Equals(resPath + "." + type.ToString()))
            {
                item = mResourcesList[i];

                break;
            }
        }

        if(string.IsNullOrEmpty(item))
        {
            item = resPath + "." + type.ToString();

            mResourcesList.Add(item);
        }
    }

    public void ReadFile()
    {
        if (!Directory.Exists(Path.GetDirectoryName(mRecordFilePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(mRecordFilePath));
        }

        mResourcesList.Clear();

        if (File.Exists(mRecordFilePath))
        {
            string[] contents = File.ReadAllLines(mRecordFilePath);
            for(int i = 1; i < contents.Length ;i++)
            {
                mResourcesList.Add(contents[i]);
            }
        }
    }

    public void WriteFile()
    {
        if (File.Exists(mRecordFilePath))
        {
            File.Delete(mRecordFilePath);
        }

        string[] contents = new string[mResourcesList.Count + 1];
        contents[0] = "资源路径";
        for (int i = 0; i < mResourcesList.Count; i++)
        {
            contents[i + 1] = mResourcesList[i];
        }

        File.WriteAllLines(mRecordFilePath, contents);
    }
}

//class ResourceUsage
//{
//    public string path;
//    public int times;
//}
#endif

