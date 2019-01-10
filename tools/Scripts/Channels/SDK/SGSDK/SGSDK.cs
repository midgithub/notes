using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

[Hotfix]
public class SGSDK
{
    private static SGSDK _instance = null;
    public static SGSDK instance
    {
        get
        {
            if (_instance == null)
                _instance = new SGSDK();

            return _instance;
        }
    }

    public const string LOGIN_SUCCESS = "0";
    public const string STARTUP = "1";
    public const string EXIT_GAME = "2";
    public const string LEVELUP = "3";
    public const string CREATEROLE = "4";
    public const string REPORTDATA = "5";
    public const string FIXPAY = "6";
    public const string UNFIXPAY = "7";

    // type类型（1-启动，2-退出，3-角色升级，4-创建角色） -5上报数据
    //6 定额充值
    //7非定额充值

    public static bool isLogin = false;
    private SGSDK()
    {
    }

#if UNITY_ANDROID
    public AndroidJavaObject androidContext()
    {
        return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
    }
#endif

    // 初始化接口
    public void init(string gameObject, string initCallbackMethod, string loginCallbackMethod, string logoutCallbackMethod, string payCallBackMethod, string exitCallbackMethod)
    {
#if UNITY_EDITOR
#elif UNITY_IOS
#elif UNITY_ANDROID
		androidContext().Call("doInit", gameObject, initCallbackMethod, loginCallbackMethod, logoutCallbackMethod, payCallBackMethod,exitCallbackMethod);
#endif
    }

    // 登录接口
    public void login()
    {
        Debug.Log("sg login");
        if (isLogin) return;
#if UNITY_EDITOR
#elif UNITY_IOS
#elif UNITY_ANDROID
		androidContext().Call("login");
#endif
    } 
    public void changeAccount()
    {
        if (SGSDK.isLogin)
        {
#if UNITY_EDITOR
#elif UNITY_IOS
#elif UNITY_ANDROID
			androidContext().Call("changeAccount");
#endif
        }
    }
    // 登出接口
    public void logout()
    {
        if (SGSDK.isLogin)
        {
#if UNITY_EDITOR
#elif UNITY_IOS
#elif UNITY_ANDROID
			androidContext().Call("logout");
#endif
        }
    }
    public void SetUseInfo(string uid, string name)
    {
#if UNITY_EDITOR
#elif UNITY_IOS
#elif UNITY_ANDROID
			androidContext().Call("SetUseInfo",uid,name);
#endif
    }
    public void SetUseData(string str)
    {
        if (SGSDK.isLogin)
        {
#if UNITY_EDITOR
#elif UNITY_IOS
#elif UNITY_ANDROID
			androidContext().Call("SetUseData",str);
#endif
        }
    }
    public void userCenter()
    {
#if UNITY_EDITOR
#elif UNITY_IOS
#elif UNITY_ANDROID
			androidContext().Call("userCenter");
#endif
    }
    //退出接口 
    public void exit()
    {
#if UNITY_EDITOR
#elif UNITY_IOS
#elif UNITY_ANDROID
		androidContext().Call("exit");
#endif
    }

    //显示浮标
    public void showFloatMenu()
    {
#if UNITY_EDITOR
#elif UNITY_IOS
#elif UNITY_ANDROID
		androidContext().Call("showFloatMenu");
#endif
    }
    public void hideFloatMenu()
    {
#if UNITY_EDITOR
#elif UNITY_IOS
#elif UNITY_ANDROID
		androidContext().Call("hideFloatMenu");
#endif
    }

#if UNITY_IOS
#endif
}

