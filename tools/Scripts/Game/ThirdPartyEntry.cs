using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;
using UnityEngine.UI;

[Hotfix]
public class ThirdPartyEntry : SingletonMonoBehaviour<ThirdPartyEntry>
{
    private GameObject InitFailWnd;

    /// <summary>
    /// 渠道标识
    /// </summary>
    public static string ChannelID = "UNITY_CHANNELID";
    public static string AccountType = "unknow";

    void Start()
    {
        StartGame();
    }

    GameObject _tip;
    void NoNetWorkTip()
    {
        if(_tip == null)
        {
            Object o = CoreEntry.gResLoader.Load("UI/Prefabs/Common/UISwitchScene");
            _tip = GameObject.Instantiate(o) as GameObject;
        }
        _tip.SetActive(true);
        _tip.transform.Find("Frame/Ok/Text").GetComponent<Text>().text = "重 试";
        _tip.transform.Find("Frame/Cancel/Text").GetComponent<Text>().text = "退 出";
        _tip.transform.Find("Frame/desc").GetComponent<Text>().text = "链接网络超时！！！";
        _tip.transform.Find("Frame/Title").GetComponent<Text>().text = "提 示";
        Button btn_ok = _tip.transform.Find("Frame/Ok").GetComponent<Button>();
        btn_ok.onClick.RemoveAllListeners();
        btn_ok.onClick.AddListener(delegate () {
            _tip.SetActive(false);
            StartGame();

        });
        Button btn_cancel = _tip.transform.Find("Frame/Cancel").GetComponent<Button>();
        btn_cancel.onClick.RemoveAllListeners();
        btn_cancel.onClick.AddListener(delegate () {
            _tip.SetActive(false);
            Application.Quit();
        });
    }

    void StartGame()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) //没有连接网络
        {
            NoNetWorkTip();
            return;
        }
        
        SDKMgr.Instance.bLoginStatus = false;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        LogMgr.LoginInit();
       
        PlayerPrefs.SetString("Language", "Chinese");
        int frame = ClientSetting.Instance.GetIntValue("TargetFrame");
        if (frame > 0)
        {
            Application.targetFrameRate = frame;
        }

        bool needLoginFromThirdParty = ClientSetting.Instance.GetBoolValue("useThirdPartyLogin");
        LogMgr.Log("ThirdPartyEntry start : needLoginFromThirdParty " + needLoginFromThirdParty);
        //登录第三方sdk
        SDKMgr.Instance.Init(ThirdPartyObj);
           Debug.Log("needLoginFromThirdParty:"+needLoginFromThirdParty);
        if (needLoginFromThirdParty)
        {
#if UNITY_EDITOR
            SimulatedEvent();
#elif UNITY_STANDALONE
            SimulatedEvent(); 
#endif
        }
        else
        {
            SimulatedEvent();
            //StartCoroutine(DelayEnterDownload());
        }

        Debug.Log("打开版权信息页面:");
        Image imageBgWindow = transform.FindChild("Window").GetComponent<Image>();
        if (imageBgWindow)
        {
            CommonTools.SetLoadImage(imageBgWindow, ClientSetting.Instance.GetStringValue("InitBg"), 1);
        }
    }

    static GameObject ThirdPartyObj;

    public override void Init()
    {

    }

    void SimulatedEvent()
    {
        SG.CoreEntry.gEventMgr.TriggerEvent(SG.GameEvent.GE_THIRDPARTY_INIT, null);
    }

    [ContextMenu("ShowPath")]
    void ShowPath()
    {
        Application.OpenURL(Application.persistentDataPath);
    }

    // Update is called once per frame
    public override void Awake()
    {
        ThirdPartyObj = new GameObject("ThirdPartyObj");
        DontDestroyOnLoad(ThirdPartyObj);
        if(ClientSetting.Instance.GetBoolValue("IS_BUGLY"))
        {
            ThirdPartyObj.AddComponent<LogCatchMgr>();
        }

        Debug.Log("Application.bundeIdentifier: " + Application.bundleIdentifier);
        Debug.Log("Application.version: " + Application.version);
        //int versionCode = CommonTools.GetVersionCode();
        //Debug.Log("Application.versionCode: " + versionCode);
        Account.Instance.Init();

        CoreEntry entryMgr = CoreEntry.Instance;
        LogMgr.Log("CoreEntry.Instance {0}", entryMgr.ToString());     //By XuXiang 去警告

        MapMgr.Instance.Init();
        CoreEntry.gEventMgr.AddListener(SG.GameEvent.GE_THIRDPARTY_INIT, OnThirdPartyInit);
    }

    public void OnThirdPartyInit(GameEvent ge, EventParameter paramter)
    {
        if (paramter != null)
        {
            if (paramter.intParameter == 0)
            {
                InitFailWnd.SetActive(true);

                return;
            }
        }

        //StartCoroutine(DelayEnterDownload());
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

    [ContextMenu("SHowUserPath")]
    void SHowUserPath()
    {
        Application.OpenURL(Application.persistentDataPath);
    }

    public IEnumerator DelayEnterDownload()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("打开版权信息页面:");

        //MapMgr.Instance.EnterInitScene();
    }

    protected override void OnDestroy()
    {
        CoreEntry.gEventMgr.RemoveListener(SG.GameEvent.GE_THIRDPARTY_INIT, OnThirdPartyInit);
    }

    void OnApplicationQuit() 
    { 
    
    }
}
