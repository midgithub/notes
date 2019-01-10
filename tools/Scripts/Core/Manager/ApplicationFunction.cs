using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;

[Hotfix]
public class ApplicationFunction : MonoBehaviour {

    float reconnectedTimeForPause = 0f;


    public NetMgr netMgr = null;
    //public int frameRate = 30;

	// Use this for initialization
	void Start () {

        reconnectedTimeForPause = ClientSetting.Instance.GetFloatValue("reconnectedTimeForPause");
        CoreEntry.gEventMgr.AddListener(SG.GameEvent.GE_THIRDPARTY_LOGOUT, ConfirmLogout);
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_THIRDPARTY_EXIT, GE_THIRDPARTY_EXIT);
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_LEAVE_GAME, OnLeaveGame);

        needLoginFromThirdParty = ClientSetting.Instance.GetBoolValue("useThirdPartyLogin");
	}

    bool needLoginFromThirdParty;
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
        {
            if (needLoginFromThirdParty)
            {
                SDKMgr.Instance.Exit();
            }
        }
    }

    /// <summary>
    /// 监听登出
    /// </summary>
    /// <param name="ge"></param>
    /// <param name="parameter"></param>
    void ConfirmLogout(GameEvent ge, EventParameter parameter)
    {
        Debug.Log("ConfirmLogout");
        CoreEntry.netMgr.Reconnect.onBtnGotoLogin();
    }

    void OnLeaveGame(GameEvent ge, EventParameter parameter)
    {
        //返回到角色选择界面
        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CLEANUP_USER_DATA, null);

        MapMgr.Instance.EnterRoleSelectScene();
    }

    void OnEnable()
    {
        //Application.targetFrameRate = frameRate;
    }

    private void OnApplicationPause(bool pause)
    {
        Debug.Log("OnApplicationPause  " + pause);
        EventParameter ep = EventParameter.Get(pause ? 1 : 0);
        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GAME_PAUSE, ep);
        if (pause)
        {
            OnBeginPause();
        }
        else {
            OnEndPause();
        }

    }

    void OnApplicationQuit()
    {
#if UNITY_EDITOR
        ResourceRecorder.Instance.WriteFile();
#endif
        //DataAnalyser.instance.Quit();

        CoreEntry.gHttpDownloadMgr.SetExit();
        CoreEntry.gHttpDownloadMgr.SetABProcessorExit();
    }

   
    
    private System.DateTime beginDateTime;
    void OnBeginPause()
    {
        beginDateTime = System.DateTime.Now;
        Debug.Log("Begin Pause...." + beginDateTime);
    }

    void OnEndPause()
    {
        Debug.Log("OnEndPause 1  " + reconnectedTimeForPause);
        double pausedDeltaSecond = System.DateTime.Now.Subtract(beginDateTime).TotalSeconds;
        if (pausedDeltaSecond > reconnectedTimeForPause)
        {
            Debug.Log("OnEndPause 2");

            if (netMgr != null && netMgr.Reconnect != null)
            {
                Debug.Log("OnEndPause 3");
                if (Account.Instance.isRecharging == false)
                {
                    Debug.Log("OnEndPause 4");
                    //dyb走重登录流程
                    if(ClientSetting.Instance.GetBoolValue("useThirdPartyLogin") && ClientSetting.Instance.GetIntValue("thirdPartyComponent")==2)
                    {
                        SG.CoreEntry.gEventMgr.TriggerEvent(SG.GameEvent.GE_THIRDPARTY_LOGOUT, null);
                        DYBSDK.isLogin = false;
                    }
                    else
                    {
                        CoreEntry.netMgr.Reconnect.needShowNotice = false;
                        CoreEntry.netMgr.disconnect();
                    }
                }
            }
        }

    }

    void GE_THIRDPARTY_EXIT(GameEvent ge, EventParameter parameter)
    {
        if (parameter.intParameter == 0)
        {
            //             TipsDialog.SetTipsText("确认退出游戏？", new TipsDialog.OnDelegateClick(() =>
            //             {
            //                 DataAnalyser.instance.Exit();
            //                 Application.Quit();
            //             }));

            UITips.ShowTipsDialog("提示", "确认退出游戏？", "确定", new System.Action(() => {
                SDKMgr.Instance.Exit();
                Application.Quit();
            }), "取消", new System.Action(() => {
                MainPanelMgr.Instance.HideDialog("UICommonTk");
            }));
        }
        else if (parameter.intParameter == 1)
        {
            SDKMgr.Instance.Exit();
            Application.Quit();
        }
    }
}

