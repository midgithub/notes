using XLua;
﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SG;

[Hotfix]
public class AssetCopy : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // IEnumerator LoadLevel()
    //{
    //    string url = "file://" + Application.dataPath + "/2.Android.unity3d";
    //    Debug.Log(url);
    //    WWW www = WWW.LoadFromCacheOrDownload(url, 3);
    //    yield return www;
    //    if (www.error != null) yield return null;
    //    AssetBundle b = www.assetBundle;


    //    Application.LoadLevel("2");
    //}


     IEnumerator LoadFrompersister(string fileName)
     {

         FileStream fs = File.Create(Application.persistentDataPath + "/1.txt");
         fs.Close();
         //fs = File.Create(Application.persistentDataPath + "/1.txt");
         //fs.Close();



         string des = Application.persistentDataPath + "/AssetsBundle/" + fileName;
         string src = "";

         if (Application.platform == RuntimePlatform.WindowsEditor)
         {
             src = "file:///" + Application.streamingAssetsPath + "/" + fileName;
             src = src.Replace('/', '\\');
         }
         else if (Application.platform == RuntimePlatform.Android)
         {
             src = "jar:file://" + Application.dataPath + "!/assets/" + fileName;
         }
         else if (Application.platform == RuntimePlatform.IPhonePlayer)
         {
             src = "file:///" + Application.streamingAssetsPath + "/" + fileName;
         }


        //#if UNITY_EDITOR
        // "file:///" + Application.streamingAssetsPath + "/" + fileName;
        //         des = des.Replace('/', '\\');
        //#elif UNITY_ANDROID
        //        "jar:file://" + Application.dataPath + "!/assets/"+  fileName;
        //#endif
        LogMgr.Log("des:" + des);
        LogMgr.Log("src:" + src);


         //lab.text = Directory.GetFiles(Application.streamingAssetsPath)[0];
         //string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "1.txt");
         using (WWW w = new WWW(src))
         {
             yield return w;
             if (string.IsNullOrEmpty(w.error))
             {
                 while (w.isDone == false) yield return null;


                 if (File.Exists(des))
                     File.Delete(des);
                 FileStream fs1 = File.Create(des);


                 BinaryFormatter bf = new BinaryFormatter();
                 bf.Serialize(fs1, w.bytes);
                 fs1.Close();
             }
             else
             {
                LogMgr.LogError(w.error);
             }

         }


         //string downpath = "file:///" + Application.persistentDataPath + "/2.Android.unity";
         //Debug.Log("down path:" + downpath);
         //using (WWW www = WWW.LoadFromCacheOrDownload(downpath, 7))
         //{
         //    yield return www;
         //    AssetBundle b = www.assetBundle;


         //    Application.LoadLevel("2");
         //    //lab.text = downpath;
         //}


     }
}

