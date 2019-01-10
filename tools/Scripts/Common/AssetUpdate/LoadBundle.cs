using XLua;
﻿using UnityEngine;
using System.Collections;
using System.IO;
using Bundle;
using SG;

[Hotfix]
public class LoadBundle : MonoBehaviour {

    //Bundle.AssetUpdateMgr mAssetUpdateMgr = new Bundle.AssetUpdateMgr();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void OnGUI()
    {

        //if (GUILayout.Button("Test", GUILayout.Height(50)))
        //{
        //    string filePath = Application.streamingAssetsPath + "/GameConfig/GameConfig.android_Zip";
        //    string outFilePath = filePath + "_UnZip";
        //    ZipCompressTool.DecompressFileLZMA(filePath, outFilePath);
        //    Debug.Log(outFilePath);
        //    AssetBundle bundle = AssetBundle.CreateFromFile(outFilePath);
        //     Debug.Log(bundle.LoadAll().Length);
        //}

        //if (GUILayout.Button("Log", GUILayout.Height(50)))
        //{
        //    Debug.Log(Application.persistentDataPath);
        //}
        //if (GUILayout.Button("DownLoadBundle",GUILayout.Height(50)))
        //{
        //    mAssetUpdateMgr.BeginUpdate();
        //    //StartCoroutine(download());
        //}
        //if (GUILayout.Button("LoadPrefabBundle", GUILayout.Height(50)))
        //{
        //    //AssetBundle bundle = AssetBundleLoadManager.Instance.LoadAsset("H0001_zhaoyun_pre", BundleAssetType.Prefab);
        //    //Debug.Log(bundle.mainAsset.name);
        //    //GameObject.Instantiate(bundle.mainAsset);
        //    //AssetBundleLoadManager.Instance.LoadAsset("model_npc_function_mugong", AssetType.Prefab, OnLoadPrefabCallback);
        //}
        //if (GUILayout.Button("LoadScenes_B", GUILayout.Height(50)))
        //{
        //    AssetBundle bundle = AssetBundleLoadManager.Instance.LoadScene("LoginUI");
        //    Application.LoadLevel("LoginUI");
        //}

        //if (GUILayout.Button("LoadCsvFile", GUILayout.Height(50)))
        //{
        //    //var bundle = AssetBundleLoadManager.Instance.LoadAsset("Achieve", BundleAssetType.Csv);
        //    //Debug.Log(bundle.mainAsset);
        //}
    }

    IEnumerator CopyFile(string res,string target,System.Action callBack)
    {
        WWW www = new WWW(res);// ("File://" + res.Replace("/", "\\"));
        yield return www;
        LogMgr.LogError(www.error + "," + res);
        File.WriteAllBytes(target,www.bytes);
        callBack();
    }

    IEnumerator download()
    {
        string Url = System.IO.Path.Combine(Bundle.BundleCommon.RemoteUrl, Bundle.BundleCommon.MainVersionFile);
        LogMgr.Log("LoadUrl:" + Url);
        WWW www = new WWW(Url);
        yield return www;
        LogMgr.LogError(www.error);
        LogMgr.Log(www.text);
    }

    IEnumerator Load()
    {
        //string path = "file://" + Application.streamingAssetsPath + "/GameConfig.android";
        string path = "file://" + (Application.persistentDataPath + "/Prefab/Depends/H0006_zhenji_png").Replace("/", "\\");
        
        Debug.Log(path);
        WWW www = new WWW(path);
        
        yield return www;
        LogMgr.LogError(www.error);
        var allassets = www.assetBundle.LoadAllAssets();
        //var allassets = www.assetBundle.LoadAll();
        LogMgr.Log("allassets.Length {0}", allassets.Length);
    }
}

