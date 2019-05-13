using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SG;
using XLua;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

//客户端配置表
[Hotfix]
public class ClientSetting
{
    private static ClientSetting sInstance = null;
    public static ClientSetting Instance
    {
        get
        {
            if (sInstance == null)
            {
                sInstance = new ClientSetting();
            }
            return sInstance;
        }
    }

    public ClientSetting()
    {
        ReLoadClientSettingData();
    }
    //读取StreamingAssets中的文件   参数 StreamingAssets下的路径
    public byte[] getTextForStreamingAssets(string path)
    {
        string localPath = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            localPath = Application.streamingAssetsPath + path;
        }
        else
        {
            localPath = "file:///" + Application.streamingAssetsPath + path;
        }
        //Debug.Log("localPath =  " + localPath);
        WWW t_WWW = new WWW(localPath);
        while (!t_WWW.isDone)
        {

        }
        if (t_WWW.error != null)
        {
            Debug.Log("error : " + localPath);
            return null;          //读取文件出错
        }
        Debug.Log("t_WWW.text=  " + t_WWW.text);
        return t_WWW.bytes;
    }

    private TextAsset GetTextFromData()
    {
        string dataPath = "Data/ClientSetting";
        if (AppConst.UseAssetBundle)
        {
            string file = FileHelper.CheckBundleName(dataPath);
            string path = FileHelper.SearchFilePath("Lua_Bundles", file);
            path = FileHelper.GetAPKPath(path);
            AssetBundle bundle = FileHelper.GetAssetBundle(path);
            if (null != bundle)
            {
                TextAsset text = bundle.LoadAsset("ClientSetting", typeof(TextAsset)) as TextAsset;
                bundle.Unload(false);
                return text;
            }
        }
        else
        {
            if (AppConst.UseResources)
            {
                return Resources.Load(dataPath, typeof(TextAsset)) as TextAsset;
            }
            else
            {
#if UNITY_EDITOR
                string fullPath = string.Format("Assets/{0}{1}.csv", AppConst.ResDataDir, dataPath);
                return AssetDatabase.LoadAssetAtPath(fullPath, typeof(TextAsset)) as TextAsset;
#endif
            }
        }

        return null;
    }

    public virtual bool ReLoadClientSettingDataRes()
    {
        TextAsset txt = GetTextFromData();
        if (txt != null)
        {
            ByteReader reader = new ByteReader(txt);
            mData = reader.ReadDictionary();
            Debug.Log("Res 文件：" + txt);
            return true;
        }
        return false;
    }

    private void RelodClientByStreamAssets()
    {
        byte[] strStream = getTextForStreamingAssets("/ConfigData/ClientSettingCfg.csv");
        if (strStream != null)
        {
            if(mData.Count <=1)
            {
                Debug.LogError("ResData ClientSetting.csv 读取失败！！！请检查配置文件是否正常");
            }
            StreamReader srReadFile = new StreamReader(new MemoryStream(strStream), System.Text.Encoding.UTF8);
            char[] separator = new char[] { '=' };
            while (!srReadFile.EndOfStream)
            {
                //检索出行
                string line = srReadFile.ReadLine();
                if (line == null) break;
                if (line.StartsWith("//")) continue;
                string[] split = line.Split(separator, 2, System.StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 2)
                {
                    string key = split[0].Trim();
                    string val = split[1].Trim().Replace("\\n", "\n");
                    if (mData.ContainsKey(key))
                    {
                        mData[key] = val;
                        //Debug.Log("Reset mData：key:" + key + "  val:" + val);
                    }
                    else
                    {
                        mData.Add(key, val);
                        //Debug.Log("Add Reset mData：key:" + key + "  val:" + val);
                    }
                }
            }
            // 关闭读取流文件
            srReadFile.Close();
        }
        else
        {
            Debug.Log("文件不存在：");
        }
    }

    public byte[] backStageStream;
    public void ReloadClientBackstage(byte[] strStream)
    {
        backStageStream = strStream;
        if (strStream != null)
        {
            StreamReader srReadFile = new StreamReader(new MemoryStream(strStream), System.Text.Encoding.UTF8);
            Debug.Log("StreamReader 文件：" + srReadFile);
            char[] separator = new char[] { '=' };
            while (!srReadFile.EndOfStream)
            {
                //检索出行
                string line = srReadFile.ReadLine();
                if (line == null) break;
                if (line.StartsWith("//")) continue;
                string[] split = line.Split(separator, 2, System.StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 2)
                {
                    string key = split[0].Trim();
                    string val = split[1].Trim().Replace("\\n", "\n");
                    if (mData.ContainsKey(key))
                    {
                        mData[key] = val;
                        Debug.Log("ReloadClientBackstage：key:" + key + "  val:" + val);
                    }
                    else
                    {
                        mData.Add(key, val);
                        Debug.Log("Add ReloadClientBackstage：key:" + key + "  val:" + val);
                    }
                }
            }
            // 关闭读取流文件
            srReadFile.Close();
        }
        else
        {
            Debug.Log("ReloadClientBackstage 文件不存在：");
        }
    }

    public virtual void ReLoadClientSettingData()
    {
        mData.Clear();
        ReLoadClientSettingDataRes();
        RelodClientByStreamAssets();
        ReloadClientBackstage(backStageStream);
    }

    public virtual bool GetBoolValue(string key)
    {
        string value;
        if (mData != null && mData.TryGetValue(key, out value))
        {
            int result;
            if (int.TryParse(value, out result))
                return (result != 0);
            else
                return false;
        }
        return false;
    }

    public virtual int GetIntValue(string key)
    {
        int result = -1;
        string value;
        if (mData != null && mData.TryGetValue(key, out value))
        {
            int.TryParse(value, out result);
        }
        return result;
    }

    public virtual string GetStringValue(string key)
    {
        string value = string.Empty;
        if (mData != null && mData.TryGetValue(key, out value))
        {
        }
        return value;
    }

    public virtual float GetFloatValue(string key)
    {
        float result = -1;
        string value;
        if (mData != null && mData.TryGetValue(key, out value))
        {
            float.TryParse(value, out result);
        }
        return result;
    }


    public string FileCSV(string _strFileName)
    {
        string path = Application.dataPath + "/ResData/" + _strFileName;
        Debug.Log(path);
        return path;
    }

    public void DataTableToCsvT()
    {
        if (mData == null)   //确保DataTable中有数据
            return;
        string strBufferLine = "";
        StreamWriter strmWriterObj = new StreamWriter(FileCSV("Data/ClientSetting.csv"), false, System.Text.Encoding.Default);
        //写入列头
        foreach (var col in mData)
        {
            strBufferLine += col.Key + "=" + col.Value + "\r\n";
        }
        strBufferLine = strBufferLine.Substring(0, strBufferLine.Length - 1);
        strmWriterObj.WriteLine(strBufferLine);
        strmWriterObj.Close();
    }

    public virtual void SetValueByKeyAndValue(string key, string setValue)
    {
        if (mData != null && mData.ContainsKey(key))
        {
            mData[key] = setValue;
        }
        else
        {
            mData.Add(key, setValue);
        }
        DataTableToCsvT();
    }

    public virtual void SetValueByKeyAndValue(string key, int setValue)
    {
        if (mData != null && mData.ContainsKey(key))
        {
            mData[key] = setValue.ToString();
        }
        else
        {
            mData.Add(key, setValue.ToString());
        }
        DataTableToCsvT();
    }

    protected Dictionary<string, string> mData = new Dictionary<string, string>();

    public Dictionary<string, string>  ConfigData
    {
        get
        {
            return mData;
        }
    }

}

