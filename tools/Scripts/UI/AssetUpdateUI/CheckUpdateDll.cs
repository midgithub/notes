using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Text;
using SG;
using XLua;

[LuaCallCSharp]
[Hotfix]
public class CheckUpdateDll : MonoBehaviour
{
    static public string _version = "1.0.1";
    private AndroidJavaClass _helper = null;
    private string DllVersion = "DllVersion.txt";
    private string appDllverPatn = "";
    private string netDllVersionPath = "";
    private string netDllPath = "";
    private string dllName = "Assembly-CSharp.dll";

    private bool isUpdate = false;
    private float progress = 0.0f;

    public Slider _slider;
    public Text  _text;
    void Start()
    {
		//Debug.LogError ("==Bundle.AssetBundleLoadManager.IsTestMode=="+Bundle.AssetBundleLoadManager.IsTestMode);
		if (Bundle.AssetBundleLoadManager.IsTestMode) {
			MapMgr.Instance.EnterUpdateScene();
			return;
		}
        string url = Bundle.BundleCommon.RootUrl;
        netDllVersionPath = url + "dll/DllVersion.txt";

        appDllverPatn = "jar:file://" + Application.dataPath + "!/assets/DllVersion.txt";


#if UNITY_EDITOR
        MapMgr.Instance.EnterUpdateScene();
#elif UNITY_ANDROID && !UNITY_EDITOR
            _helper = new AndroidJavaClass("com.cytx.tools.helper");
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                object jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                _helper.CallStatic("init", jo);
            }
            StartCoroutine(CheckDllVersion());
#endif

    }
    IEnumerator CheckDllVersion()
    {
        string path = GetPath();
        string dllpath = path + DllVersion;
        Debug.LogError(dllpath);
        string dllVer;
        if (File.Exists(dllpath))
        {
            Debug.Log("File Exists=" + dllpath);
            StreamReader sr = new StreamReader(dllpath, Encoding.Default);
            dllVer = sr.ReadLine();
            Debug.Log("dllVer=" + dllVer);
            sr.Dispose();
            sr.Close();

        }
        else
        {
            WWW tmpWWW = new WWW(appDllverPatn);
            isUpdate = true;
            _text.text = "检查本地ver版本号";
            while (!tmpWWW.isDone)
            {
                progress = tmpWWW.progress;
                yield return new WaitForEndOfFrame();

            }
            isUpdate = false;
            if (!string.IsNullOrEmpty(tmpWWW.error))
            {
                Debug.LogError("网络下载dll dllappPatn 出错！" + tmpWWW.error);
                yield break;
            }

            Debug.LogError(Directory.Exists(path));
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            using (FileStream fs = new FileStream(dllpath, FileMode.CreateNew))
            {
                Debug.LogError("==tmpWWW.text===" + tmpWWW.text);
                fs.Write(tmpWWW.bytes, 0, tmpWWW.bytes.Length);
                dllVer = tmpWWW.text;
                fs.Flush();
                fs.Close();
            }
        }
        Debug.Log("==dllVer==" + dllVer);
        string[] spVer = dllVer.Split('.');
        WWW netDllVerWWW = new WWW(netDllVersionPath);
        isUpdate = true;
        _text.text = "检查网络ver版本号";
        while (!netDllVerWWW.isDone)
        {
            progress = netDllVerWWW.progress;
            yield return new WaitForEndOfFrame();

        }
        isUpdate = false;

       // yield return netDllVerWWW;
        if (!string.IsNullOrEmpty(netDllVerWWW.error))
        {
            Debug.LogError("网络下载dll ver 出错！" + netDllVerWWW.error + "==netDllVersionPath="+ netDllVersionPath);
            MapMgr.Instance.EnterUpdateScene();
            yield break;
        }
        Debug.Log("netDllVersionPath =" + netDllVerWWW.text);
        string[] netVer = netDllVerWWW.text.Split('.');
        if (spVer.Length != netVer.Length)
        {
            StartCoroutine(DownLoadDll(netDllVerWWW.text));
            yield break;
        }
        for (int i = 0; i < spVer.Length; ++i)
        {
            if (spVer[i] != netVer[i])
            {
                StartCoroutine(DownLoadDll(netDllVerWWW.text));
                yield break;
            }
        }
        _version = netDllVerWWW.text;
        MapMgr.Instance.EnterUpdateScene();
        yield break;
    }
    void Update()
    {
        if (!isUpdate) return;
        if (_slider == null) return;
        _slider.value = progress;
        if (progress >= 1.0f) isUpdate = false;
    }
    IEnumerator DownLoadDll(string netVer)
    {
        netDllPath =  Bundle.BundleCommon.RootUrl + "dll/"+netVer +"/Assembly-CSharp.dll";

        Debug.Log("DownLoadDll==" + netDllPath);
        WWW tmpWWW = new WWW(netDllPath);
        isUpdate = true;
        _text.text = "下载最新的dll文件";
        while (!tmpWWW.isDone)
        {
            progress = tmpWWW.progress;
            yield return new WaitForEndOfFrame();

        }
        isUpdate = false;
        // yield return tmpWWW;
        if (!string.IsNullOrEmpty(tmpWWW.error))
        {
            Debug.LogError("download dll 出错");
            MapMgr.Instance.EnterUpdateScene();
            yield break;
        }
        string path = GetPath();
        string dllpath = path + dllName;

        if (File.Exists(dllpath))
        {
            Debug.Log("File Exists");
            File.Delete(dllpath);
        }
        Debug.Log(Directory.Exists(path));
        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }
        using (FileStream fs = new FileStream(dllpath, FileMode.CreateNew))
        {
            fs.Write(tmpWWW.bytes, 0, tmpWWW.bytes.Length);
            fs.Flush();
            fs.Close();
        }
        string ver = path + DllVersion;
        //ver
        if (File.Exists(ver))
        {
            File.Delete(ver);
        }
        StreamWriter sw = File.CreateText(ver);
        sw.Write(netVer);
        sw.Flush();
        sw.Close();


        yield return new WaitForEndOfFrame();// WaitForSeconds(1f) ;
         _helper.CallStatic("restartApplication");
        yield break;
    }

    string GetPath()
    {
        string datapath = Application.dataPath;
        int start = datapath.IndexOf("com.");
        int end = datapath.IndexOf("-");
        string packagename = datapath.Substring(start, end - start);
        string path = "/data/data/" + packagename + "/files/";
        return path;
    }
}

