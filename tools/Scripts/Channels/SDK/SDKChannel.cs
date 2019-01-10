using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using XLua;
using System.Collections.Generic;
using SG;
using System;

[LuaCallCSharp]
[Hotfix]
public class SDKChannel  {

    public const string SUCCESS = "0";
    public const string SUCCESS_WITCHACCOUNT = "2";
    public const string SUCCESS_SERVERLIST = "5";
    public const string FAIL_SERVERLIST = "6";
    public const string FAIL = "1";
    public const string STARTUP = "1";
    public const string EXIT_GAME = "2";
    public const string LEVELUP = "3";
    public const string CREATEROLE = "4";
    public const string REPORTDATA = "5";
    public const string FIXPAY = "6";
    public const string UNFIXPAY = "7";


    public const string CHANEELID = "8";
    public const string USERID = "9";


    public static bool isLogin = false;

#if UNITY_ANDROID
    public AndroidJavaObject androidContext()
    {
        return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
    }
#endif

#if UNITY_IOS
    protected string _orderID;

    [DllImport("__Internal")]
    protected static extern void _init(string gameObjectName);
    [DllImport("__Internal")]
    protected static extern void _login();
    [DllImport("__Internal")]
    protected static extern void _logout();
    [DllImport("__Internal")]
    protected static extern void _getServerList();
    [DllImport("__Internal")]
    protected static extern void _submitExtraData(int type, string serverID, string serverName, string roleID, string roleName, string roleLevel, string money, string roleCTime, string roleLevelIMTime, string vip, string power);
    [DllImport("__Internal")]
    protected static extern void _submitCustomData(string act, string des);
    [DllImport("__Internal")]
    protected static extern void _submitCustomDataWithAct(string act, string des);
    [DllImport("__Internal")]
    protected static extern void _submitCustomDotData(string act, string des);
    [DllImport("__Internal")]
    protected static extern bool _isLogin();
    [DllImport("__Internal")]
    protected static extern string _getChannelID();
    [DllImport("__Internal")]
    protected static extern string _getCurrPID();
    [DllImport("__Internal")]
    protected static extern string _getCurrIP();
    [DllImport("__Internal")]
    protected static extern string _getPlistByKey();
    [DllImport("__Internal")]
    protected static extern void _pay(int type, string orderID, string extension, int money, string productID, string productName, string productDesc);
    [DllImport("__Internal")]
    protected static extern void _submitReg(string roleID);
    [DllImport("__Internal")]
    protected static extern void _submitLogin(string roleID);

    [DllImport("__Internal")]
    protected static extern void _submitZFStart(string orderID, int money);
    [DllImport("__Internal")]
    protected static extern void _submitZFFinish(string orderID, int money);
    [DllImport("__Internal")]
    protected static extern void _payOK(int type, string orderID, string extension, int money, string productID, string productName, string productDesc);
    [DllImport("__Internal")]
    protected static extern string _getIDFA();
    [DllImport("__Internal")]
    protected static extern void _activeInitializeIosUI();
    [DllImport("__Internal")]
    protected static extern string _appversion();

#endif

}

