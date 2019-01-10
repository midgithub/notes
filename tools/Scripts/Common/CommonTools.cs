using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.IO;
using SG;

[LuaCallCSharp]
[Hotfix]
public static class CommonTools {

    public static string CONFIG_DIR = "Data/";

    public static void SetText(this UILabel label, string tex)
    {
        label.text = tex;
    }
     
    public static T FindComponent<T>(Transform transform, string name) where T : Component
    {
        Transform tr = transform.Find(name);
        if (tr != null)
        {
            return tr.GetComponent<T>();
        }

        return null;
    }
    /// <summary>
    /// 在子节点内搜查
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FindComponentDeepSearch<T>(Transform transform, string name) where T : Component
    {
        T[] tr = transform.GetComponentsInChildren<T>();
        for (int i = 0; i < tr.Length; ++i)
        {
            if (tr[i].name.CompareTo(name) == 0)
            {
                return tr[i];
            }
        }

        return null;
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform DeepFindChild(this Transform transform, string name)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).name.Equals(name))
                return transform.GetChild(i);

            Transform findTrans = DeepFindChild(transform.GetChild(i), name);

            if (findTrans != null)
                return findTrans;
        }

        return null;
    }


    /// <summary>
    /// 在父节点内搜查
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindComponentInParentDeepSearch(Transform transform, string name)
    { 
        if (transform.parent != null)
        {
            if (transform.parent.name.Equals(name))
                return transform.parent;
            Transform findTrans = FindComponentInParentDeepSearch(transform.parent, name);
            if (findTrans != null)
                return findTrans;

        }
        return null;        
    }

    public static T GetARequiredComponent<T>(GameObject obj) where T : Component
    {
        T t = null;
        if (obj != null)
        {
            t = obj.GetComponent<T>();
            if (t == null)
            {
                t = obj.AddComponent<T>();
            }
        }
        
        return t;
    }

    public static void CleanUpChild(Transform parent)
    {
        foreach (Transform child in parent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }


    public static void HideChild(Transform parent)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.SetActive(false);
        }
    }

    public static void ApplyAllItem<T>(this IEnumerable<T> sourceList, System.Action<T> Action)
    {
        using (var e = sourceList.GetEnumerator())
        {
            while (e.MoveNext())
            {
                var child = e.Current;
                Action(child);
            }
        }
    }

    static Vector3 TempPos = Vector3.zero;
    public static void SetRenderActive(this Transform tran, bool flag)
    {
     
        TempPos = tran.localPosition;
        TempPos.z = flag ? 0 : -100000;
        tran.localPosition = TempPos;
       // Debug.LogError(tran.parent.name + "==flag==" + flag.ToString() + "= tran.localPosition=" + tran.localPosition);
    }


    public static void SetSubRenderActive(this Transform tran, bool flag)
    {
        CanvasGroup rend = tran.GetComponent<CanvasGroup>();
        if (rend)
        {
            rend.alpha = (flag ? 1 : 0);
        }
        TempPos = tran.localPosition;
        TempPos.z = flag ? 0 : -100000;
        tran.localPosition = TempPos;
    }


    public static bool IsRenderActive(this Transform tran)
    {
        Vector3 pos = tran.localPosition;
        return (bool)(pos.z != -100000);
    }

    public static void MoveFromTo(this Transform tran, Vector3 from, Vector3 to, float time)
    {
        tran.position = from;
        tran.DOMove(to, time);
    }

    //ui是否在显示状态,true显示，false不显示
    public static bool IsUIActive(this Transform tran)
    {
       int outlayer = LayerMask.NameToLayer("outui");
       int uilayer = tran.gameObject.layer;
       return (uilayer != outlayer);
    }

    public static T LoadResouce<T>(string path) where T : Object
    {
        return CoreEntry.gResLoader.Load(path,typeof(T)) as T;  //Bundle.AssetBundleLoadManager.Instance.Load<T>(path);
    }
    public static bool IsEventMsgNull(this SG.EventParameter msg)
    {
        if(msg == null)
            return true;
        if(msg.msgParameter == null)
            return true;
        return false;
    }

    public static string BytesToString(this byte[] bytes)
    {
        if (null == bytes)
            return string.Empty;

        int len = bytes.Length;
        for (int i = 0; i < bytes.Length; ++i)
        {
            if (bytes[i] == 0)
            {
                len = i;
                break;
            }
        }
        return len <= 0 ? string.Empty : System.Text.Encoding.UTF8.GetString(bytes, 0, len);
    }

    public static string BytesToString2(this byte[] bytes,int pos,int maxLen)
    {
        if (null == bytes)
            return string.Empty;

        int len = 0;
        for (int i = 0; i < maxLen; ++i)
        {
            if (bytes[i + pos] == 0)
            {
                len = i;
                break;
            }
        }
        return len <= 0 ? string.Empty : System.Text.Encoding.UTF8.GetString(bytes, pos, len);
    }

    public static byte[] StringToBytes(this string srcStr)
    {
        if (string.IsNullOrEmpty(srcStr))
            return null;

        return System.Text.Encoding.UTF8.GetBytes(srcStr);
    }

    public static float GetTerrainHeight(Vector2 xzPosition)
    {
        Ray ray = new Ray();
        ray.origin = new Vector3(xzPosition.x, 10000.0f, xzPosition.y);
        ray.direction = new Vector3(0, -1, 0);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("ground")))
        {
            return hitInfo.point.y;
        }

        Debug.Log("no ground--------");
        return 0;
    }

    public static float GetTerrainHeight(Vector3 position)
    {
        return GetTerrainHeight(new Vector2(position.x, position.z));
    }

    private static float scaleRatio = 0.001f;
    public static Vector3 ServerPosToClient(int posX, int posY)
    {
        return new Vector3(posX * scaleRatio, 0f, posY * scaleRatio);
    }

    public static Vector3 ServerDirToClient(int dir)
    {
        return new Vector3(0f, Mathf.Rad2Deg * (dir * scaleRatio), 0f);
    }

    public static float ServerValueToClient(float value)
    {
        return value * scaleRatio;
    }

    public static GameObject AddSubChild(GameObject parent ,string path)
    {
       GameObject obj = SG.CoreEntry.gResLoader.Load(path) as GameObject;
       if (obj != null)
       {
           GameObject objclone = GameObject.Instantiate(obj);
           if (objclone == null)
               return null;
           objclone.transform.SetParent(parent != null ? parent.transform : null);

           objclone.name = objclone.name.Replace("(Clone)", "");
           objclone.transform.localScale = Vector3.one;
           objclone.gameObject.SetActive(true);
            if (objclone.transform is RectTransform)
            {
                (objclone.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            }
            else
            {
                objclone.transform.localPosition = Vector3.zero;
            }
            
           return objclone;
       }
        return null;
    }
    public static void SetlayerUI(this Transform tran, bool v)
    {
        int tmpLayer = LayerMask.NameToLayer("UI");
        if (!v)
        {
           tmpLayer = LayerMask.NameToLayer("outui");
        }
        CommonTools.SetLayer(tran.gameObject, tmpLayer);

    }
    public static void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;

        Transform t = go.transform;

        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            SetLayer(child.gameObject, layer);
        }
    }

    public static void SetLayer(GameObject go, int layer, HashSet<string> nodes)
    {
        if (null == nodes || nodes.Count == 0)
        {
            return;
        }

        if (nodes.Contains(go.name))
        {
            go.layer = layer;
        }

        if (go.name.Contains("Effect_"))
        {
            go.layer = LayerMask.NameToLayer("effect");
        }

        Transform t = go.transform;

        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            SetLayer(child.gameObject, layer, nodes);
        }
    }

    public static void SetMainLayer(GameObject go, int modelID)
    {
        LuaTable cfg = SG.ConfigManager.Instance.Common.GetModelConfig(modelID);
        if (null == cfg)
        {
            return;
        }

        HashSet<string> nodes = new HashSet<string>();
        string[] nodeNames = cfg.Get<string>("castShadow").Split('#');
        for (int i = 0; i < nodeNames.Length; i++)
        {
            nodes.Add(nodeNames[i]);
        }

        CommonTools.SetLayer(go, LayerMask.NameToLayer("mainplayer"), nodes);
    }

    public static UnityEngine.Material LoadMaterial(string path)
    {
        return (UnityEngine.Material)SG.CoreEntry.gResLoader.Load(path, typeof(UnityEngine.Material));
    }
    public static bool CheckGuiRaycastObjects()
    {
        if (Application.isMobilePlatform)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return true;
            }
        }
        else
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                //过滤穿透
                return true;
            }
        }
        return false;
    }

    public static void LoadImagSets()
    {
        var configData = ClientSetting.Instance.ConfigData;
        foreach(var val in configData)
        {
            string strImage = val.Value;
            if (strImage.Contains(".png"))
            {
                string urlImage = Application.streamingAssetsPath + "/" + strImage;
                if (Application.platform == RuntimePlatform.Android)
                {
                    urlImage = Application.streamingAssetsPath + "/" + strImage;
                }
                else
                {
                    urlImage = "file://" + Application.streamingAssetsPath + "/" + strImage;
                }

                //Debug.Log("LoadImage:::" + urlImage);
                WWW t_WWW = new WWW(urlImage);
                while (!t_WWW.isDone)
                {

                }
                if (t_WWW.error != null)
                {
                    Debug.Log("error : " + urlImage);
                }
                else
                {
                    if (t_WWW.texture != null)
                    {
                        MapMgr.Instance.AddTexture2D(strImage, t_WWW.texture);
                    }
                }
            }
        }

    }

    public static object SetLoadImage(Component pCompont, string strImage, int nType = 0)
    {
        //bool bChange = ClientSetting.Instance.GetBoolValue("Is_ChangeImage");
        //if (bChange)
        {
            Texture2D texImage = MapMgr.Instance.GetTexture2D(strImage);
            Debug.Log("SetLoadImage ::: Start::" + strImage);
            if (texImage)
            {
                if (nType == 0)
                {
                    var imageBg = pCompont as RawImage;
                    imageBg.texture = texImage;
                    Debug.Log("SetLoadImage ::: RawImage!!!" + strImage);
                }
                else if (nType == 1)
                {
                    var imageBg = pCompont as UnityEngine.UI.Image;
                    imageBg.sprite = Sprite.Create(texImage, new Rect(0, 0, texImage.width, texImage.height), new Vector2(0, 0));
                    Debug.Log("SetLoadImage ::: Sprite!!!" + strImage);
                }
            }
        }
        return null;
    }

        public static IEnumerator  LoadImage(Component pCompont, string strImage, int nType = 0)
    {
        yield return SetLoadImage(pCompont, strImage, nType);
        //string urlImage = Application.streamingAssetsPath + "/" + strImage; ;
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    urlImage = Application.streamingAssetsPath + "/" + strImage;
        //}
        //else
        //{
        //    urlImage = "file://" + Application.streamingAssetsPath + "/" + strImage;
        //}

        //Debug.Log("LoadImage:::" + urlImage);
        //WWW tWwwInfo = new WWW(urlImage);
        //yield return tWwwInfo;
        //if (nType == 0)
        //{
        //    var imageBg = pCompont as RawImage;
        //    if (tWwwInfo.texture != null)
        //    {
        //        imageBg.texture = tWwwInfo.texture;
        //    }
        //    imageBg.enabled = true;
        //}
        //else if (nType == 1)
        //{
        //    var imageBg = pCompont as UnityEngine.UI.Image;
        //    if (tWwwInfo.texture != null)
        //    {
        //        Texture2D tex = tWwwInfo.texture;
        //        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        //        imageBg.sprite = temp;
        //    }
        //    imageBg.enabled = true;
        //}
        Debug.Log("加载完成！");
    }

    public static void LoadImageIO(Component pCompont, string strImage, Vector2 imageSize, int nType = 0)
    {
        string url = "file://" + Application.streamingAssetsPath + "/" + strImage;
        double startTime = (double)Time.time;

        using (FileStream fileStream = new FileStream(url, FileMode.Open, FileAccess.Read))
        {
            fileStream.Seek(0, SeekOrigin.Begin);

            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, (int)fileStream.Length);

            Texture2D texture = new Texture2D((int)imageSize.x, (int)imageSize.y);
            texture.LoadImage(bytes);
            if (nType == 0)
            {
                var imageBg = pCompont as RawImage;
                imageBg.texture = texture;
                imageBg.enabled = true;
            }
            else if (nType == 1)
            {
                var imageBg = pCompont as UnityEngine.UI.Image;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                imageBg.sprite = sprite;                
            }
            startTime = (double)Time.time - startTime;
            Debug.Log("IO加载用时：" + startTime);
        }
        Debug.Log("IO 加载完成！");
    }


    public static void Append2File(string fileNmae,string desc)
    {
        string path = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            path = Application.persistentDataPath;
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            path = Application.dataPath;
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            path = Application.dataPath;
        }

        System.IO.StreamWriter sw;
        System.IO.FileInfo fileInfo = new System.IO.FileInfo(path.Replace("/Assets","") + "/" + fileNmae);
        if(!fileInfo.Exists)
        {
            sw = fileInfo.CreateText();
        }
        else
        {
            sw = fileInfo.AppendText();
        }
        sw.WriteLine(desc);
        sw.Close();
        sw.Dispose();


    }

    public static int GetVersionCode()
    {
        int ret = 0;
#if !UNITY_EDITOR
#if UNITY_ANDROID
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (AndroidJavaObject assetManager = activity.Call<AndroidJavaObject>("getApplicationContext"))
                {
                    string text = assetManager.Call<string>("getPackageName");
                    
                    using (AndroidJavaObject pm = assetManager.Call<AndroidJavaObject>("getPackageManager"))
                    {
                        using (AndroidJavaObject pkgInfo = pm.Call<AndroidJavaObject>("getPackageInfo", new object[] { text, 0 }))
                        {
                            ret = pkgInfo.Get<int>("versionCode");
                        }
                    }
                }

               
            }

        }
#endif
#endif
        return ret;
    }

}
