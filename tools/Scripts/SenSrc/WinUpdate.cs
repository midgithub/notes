using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using XLua;
using SenLib;

[Hotfix]
public class WinUpdate : MonoBehaviour
{
    public static event Action OnSplashOver;

    GameObject _splashUI;
    RawImage _companyImg;

    GameObject _updateUI;
    Slider _proBar;
    Text _updatePro;
    Text _txtTip;
    Text _version;

    GameObject _tipPanle;
    Text _texDesc;
    Button _okBtn;
    Button _cancleBtn;

    string apkUrl = string.Empty;
    
    public static void ShowUI()
    {
        string uiPath = "UI/Prefabs/VersionUpdate/VersionUpdatePanel";
        if (AppConst.UseAssetBundle)
        {
            string file = FileHelper.CheckBundleName(uiPath);
            string path = FileHelper.SearchFilePath("UI_Bundles", file);
            path = FileHelper.GetAPKPath(path);
            AssetBundle bundle = AssetBundle.LoadFromFile(path);
            if (null != bundle)
            {
                var pre = bundle.LoadAsset("VersionUpdatePanel", typeof(GameObject));
                if (null != pre)
                {
                    GameObject go = GameObject.Instantiate(pre) as GameObject;
                    go.SetActive(true);
                }

                bundle.Unload(false);
            }
        }
        else
        {
            GameObject pre = null;
            if (AppConst.UseResources)
            {
                pre = Resources.Load(uiPath, typeof(GameObject)) as GameObject;
            }
            else
            {
#if UNITY_EDITOR
                string fullPath = string.Format("Assets/{0}{1}.prefab", AppConst.ResDataDir, uiPath);
                pre = AssetDatabase.LoadAssetAtPath(fullPath, typeof(GameObject)) as GameObject;
#endif
            }

            if (null != pre)
            {
                GameObject go = GameObject.Instantiate(pre) as GameObject;
                go.SetActive(true);
            }
        }
    }

    void Awake()
    {
        Init();
        CVersionManager.Instance.OnVersionProgressEvent += OnUpdateResEvent;
    }

    private void OnDestroy()
    {
        CVersionManager.Instance.OnVersionProgressEvent -= OnUpdateResEvent;
    }

    void Init () {
        _splashUI = transform.Find("Splash").gameObject;
        _splashUI.SetActive(true);
        RawImage splash = _splashUI.GetComponent<RawImage>();
        if(null != splash)
        {
            StartCoroutine(SetTexture(splash, ClientSetting.Instance.GetStringValue("ThirdPartyBg")));
        }
        
        _companyImg = transform.Find("Splash/companyImg").GetComponent<RawImage>();
        if (null != _companyImg)
        {
            _companyImg.enabled = false;
            //StartCoroutine(SetTexture(_companyImg, ClientSetting.Instance.GetStringValue("ThirdPartyBg")));
        }

        _updateUI = transform.Find("UpdateUI").gameObject;
        _updateUI.SetActive(false);

        _proBar = transform.Find("UpdateUI/Slider").GetComponent<Slider>();
        _proBar.gameObject.SetActive(false);

        _updatePro = transform.Find("UpdateUI/Slider/updatePro").GetComponent<Text>();
        _updatePro.text = string.Empty;

        _txtTip = transform.Find("UpdateUI/Tip").GetComponent<Text>();
        _txtTip.text = string.Empty;

        _version = transform.Find("UpdateUI/Text/lab_version").GetComponent<Text>();
        _version.text = string.Empty;

        _tipPanle = transform.Find("UpdateUI/TipWindow").gameObject;
        _tipPanle.SetActive(false);

        _texDesc = transform.Find("UpdateUI/TipWindow/Frame/Desc").GetComponent<Text>();

        _okBtn = transform.Find("UpdateUI/TipWindow/Frame/Ok").GetComponent<Button>();
        _okBtn.onClick.AddListener(OnOkBtnClick);

        _cancleBtn = transform.Find("UpdateUI/TipWindow/Frame/Cancel").GetComponent<Button>();
        _cancleBtn.onClick.AddListener(OnCancleBtnClick);

        RawImage imageBg = transform.FindChild("UpdateUI").GetComponent<RawImage>();
        if (imageBg)
        {
            StartCoroutine(SetTexture(imageBg, ClientSetting.Instance.GetStringValue("InitBg")));
        }

        StartCoroutine(SplashAct());
    }

    IEnumerator SplashAct()
    {
        yield return new WaitForEndOfFrame();
        CommonTools.LoadImagSets();
        yield return null;

        _splashUI.SetActive(false);

        _version.text = string.Format("{0}.{1}", CVersionManager.Instance.GetAppVersion(), CVersionManager.Instance.GetLocalResVersion()); 
        _updateUI.SetActive(true);

        if(null != OnSplashOver)
        {
            OnSplashOver();
        }
    }

    IEnumerator SetTexture(RawImage rawImg,string res)
    {
        string path = AppContentPath() + res;
        Debug.Log("load path: " + path);
        WWW www = new WWW(path);
        yield return www;

        if (www != null && string.IsNullOrEmpty(www.error))
        {
            rawImg.texture = www.texture;
        }
        else
        {
            Debug.LogError(www.error);
        }

        if (www.isDone)
        {
            www.Dispose();
        }
    }

    private string AppContentPath()
    {
        string path = string.Empty;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                path = Application.streamingAssetsPath + "/";
                break;
            case RuntimePlatform.IPhonePlayer:
                path = "file://" + Application.dataPath + "/Raw/";
                break;
            default:
                path = "file://" + Application.streamingAssetsPath + "/";
                break;
        }

        return path;
    }

    private void OnUpdateResEvent(VersionProgressEvent data)
    {
        switch (data.state)
        {
            case EVersionState.Extracting:  //解压
                if (data.curPro == 0)
                {
                    _proBar.gameObject.SetActive(true);
                    _proBar.value = 0;

                    _txtTip.text = "首包资源解压中，不耗费流量...";
                }
                else
                {
                    _proBar.value = (float)data.curPro / data.dataLength;
                }
                break;
            case EVersionState.ExtracSuccess:  //解压成功
                _txtTip.text = "首包资源解压完";
                break;
            case EVersionState.NoInternet:  //无信号
                _proBar.gameObject.SetActive(false);
                _txtTip.text = string.Empty;

                apkUrl = string.Empty;
                ShowTipPanel("无可用网络信号,请检查网络!");
                break;
            case EVersionState.ApkUpdate:   //发现大版本
                _proBar.gameObject.SetActive(false);
                _txtTip.text = string.Empty;

                apkUrl = data.info;
                ShowTipPanel("发现大版本更新,是否前往链接!");
                break;
            case EVersionState.ResUpdating:  //资源更新中
                _proBar.gameObject.SetActive(true);
                _proBar.value = (float)data.curPro / data.dataLength;
                _updatePro.text = string.Format("{0} / {1}", Util.GetFileLengthStr(data.curPro), Util.GetFileLengthStr(data.dataLength));

                _txtTip.text = "资源更新中...";
                break;
            //case EVersionState.ResUpdateSuccess:  //资源更新成功
            //    _txtTip.text = "资源更新成功";
            //    break;
            case EVersionState.ResUpdateFail:  //资源更新失败
                _txtTip.text = "资源更新失败";
                _updatePro.text = string.Empty;
                break;
            case EVersionState.ResExtractSuccess:  //资源解压成功
                _txtTip.text = "资源解压成功";
                _updatePro.text = string.Empty;
                break;
            case EVersionState.ResExtractFail:  //资源解压失败
                _txtTip.text = "资源解压失败";
                _updatePro.text = string.Empty;
                break;
            case EVersionState.PackageCfgFail:  //获取分包配置出错
                _txtTip.text = "获取分包配置出错";
                break;
            case EVersionState.PackageUpdating:  //每个分包更新中
                _proBar.gameObject.SetActive(true);
                _proBar.value = (float)data.curPro / data.dataLength;
                _updatePro.text = string.Format("{0} / {1}", Util.GetFileLengthStr(data.curPro), Util.GetFileLengthStr(data.dataLength));

                _txtTip.text = "资源更新中...";
                break;
            case EVersionState.PackageUpdateSuccess:  //登入所有分包更新完
                _txtTip.text = "准备游戏";
                _updatePro.text = string.Empty;
                break;
            case EVersionState.PackageUpdateFail:  //分包更新失败
                _txtTip.text = "资源更新失败";
                _updatePro.text = string.Empty;
                break;
            case EVersionState.PackageExtracting:  //分包解压同步
                _txtTip.text = "资源解压中...";
                _updatePro.text = string.Empty;
                break;
            case EVersionState.PackageExtractSuccess:  //分包解压成功
                _txtTip.text = "资源解压成功";
                break;
            case EVersionState.PackageExtractFail:  //分包解压失败
                _txtTip.text = "解压失败";
                break;
            default:
                break;
        };
    }

    private void ShowTipPanel(string tip)
    {
        _tipPanle.SetActive(true);
        _texDesc.text = tip;
    }

    private void OnOkBtnClick()
    {
        if (string.IsNullOrEmpty(apkUrl))
        {
            Application.Quit();
        }
        else
        {
            Debug.Log("apk更新地址： " + apkUrl);
            Application.OpenURL(apkUrl);
        }  
    }

    private void OnCancleBtnClick()
    {
        if (string.IsNullOrEmpty(apkUrl))
        {
            Application.Quit();
        }
        else
        {
            if (CVersionManager.Instance.ContinueUpdateRes)
            {
                Debug.Log("取消大版本更新继续资源更新---- ");
                _tipPanle.SetActive(false);
                CVersionManager.Instance.ContinueCheckResUpdate();
            }
            else
            {
                Application.Quit();
            }
        }
    }
}
