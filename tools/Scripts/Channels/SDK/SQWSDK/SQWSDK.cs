using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using XLua;
using System.Collections.Generic;
using SG;
using System;
/**
* @file     : SQWSDK
* @brief    : 37玩SDK接口文件
* @details  : 用于和android 文件进行交互
* @author   : 
* @date     : 2019.3.12
*/

[LuaCallCSharp]
[Hotfix]
public class SQWSDK : SDKChannel
{
    private static SQWSDK _instance = null;
    public static SQWSDK Instance
    {
        get
        {
            if (_instance == null)
                _instance = new SQWSDK();
            return _instance;
        }
    }

    public string chaneelID = "";
    public string userID = "";
    public  string sdkServerList = "";
    public  string sdkDefalutList = ""; //历史登录服务器
    // 初始化接口
    public void init(string gameObject, string initCallbackMethod, string loginCallbackMethod, string logoutCallbackMethod, string payCallBackMethod, string exitCallbackMethod)
    {
#if UNITY_EDITOR
#elif UNITY_IOS  
         Debug.Log("SQW IOS SDK Init");
        _init(gameObject);
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
        _login();
#elif UNITY_ANDROID
		androidContext().Call("login");
#endif
    }

    // 登出接口
    public bool hasLogout()
    {
        Debug.Log("hasLogout");
        bool bres = true; 
#if UNITY_EDITOR
#elif UNITY_IOS  
#elif UNITY_ANDROID
      
#endif
        Debug.Log("hasLogout" + bres.ToString());
        return bres;
    }


    //未知
    public string getMasterID()
    {
        Debug.Log("getMasterID");
        string bres = "";
#if UNITY_EDITOR
#elif UNITY_IOS  
      
#elif UNITY_ANDROID
        return chaneelID;
#endif
        Debug.Log("getMasterID" + bres.ToString());
        return bres;
    }

    // 充值接口
    public void pay(int type, string orderID, string extension, int money, string productID, string productName, string productDesc)
    {
        //if (isLogin) return;
#if UNITY_EDITOR
#elif UNITY_IOS  
        Debug.Log("SQWSDK :: pay : " + orderID + "  productID:" + productID);
        _pay(type, orderID, extension, money, productID, productName, productDesc);

        _orderID = orderID;
        _submitZFStart(orderID, money);
#elif UNITY_ANDROID
       
#endif
    }

    // 充值接口
    public void payJson(String  strJson)
    {
        //if (isLogin) return;
#if UNITY_EDITOR
#elif UNITY_IOS  
        Debug.Log("SQWSDK :: pay : " + strJson);
        _payJson(strJson);

        //_orderID = orderID;
        //_submitZFStart(orderID, money);
#elif UNITY_ANDROID
       
#endif
    }


    //充值成功服务器返回成功
    public void paySucces()
    {

    }
    //充值成功服务器返回成功
    public void payCallBackMethod(int type, string orderID, string extension, int money, string productID, string productName, string productDesc)
    {
#if UNITY_EDITOR
#elif UNITY_IOS   
        _payOK(type, orderID, extension, money, productID, productName, productDesc);
#elif UNITY_ANDROID
#endif
    }

    /// <summary>
    /// 设置充值数据
    /// </summary>
    /// <param name="data"></param>
    public void setPayData(string data)
    {
#if UNITY_ANDROID
        androidContext().Call("pay",data);
        Debug.Log("调用SDK支付接口： " + data);
#endif
    }


    public void changeAccount()
    {
        if (SQWSDK.isLogin)
        {
#if UNITY_EDITOR
#elif UNITY_IOS  
#elif UNITY_ANDROID
			androidContext().Call("switchAccount");
#endif
        }
    }


    //请求可用服务器，目前第一拨没有
    public void requestAvailableServer()
    {
        if (SQWSDK.isLogin)
        {
#if UNITY_EDITOR
#elif UNITY_IOS  
            _getServerList();
#elif UNITY_ANDROID
			
#endif

        }
    }

    /// <summary>
    /// PID???W
    /// </summary>
    /// <returns></returns>
    public string  GetCurrPID()
    {
        if (SQWSDK.isLogin)
        {
#if UNITY_EDITOR
#elif UNITY_IOS  
            return _getCurrPID();
#elif UNITY_ANDROID
            
			return "";
#endif
        }
        return "";
    }

    public string GetUserId()
    {
        Debug.Log("UserId : " + userID);
        return userID;
    }

    public string GetAppID()
    {
		Debug.Log("SDK_APPID : " + ClientSetting.Instance.GetStringValue("SDK_APPID"));
        return ClientSetting.Instance.GetStringValue("SDK_APPID");
    }

    private string startID = string.Empty;
    public string GetStartID()
    {
        if (string.IsNullOrEmpty(startID))
        {
            startID = System.Guid.NewGuid().ToString("N");
        }
        return startID;
    }

    public string GetLogID()
    {
        return System.Guid.NewGuid().ToString("N");
    }
    // 登出接口
    public void logout()
    {
        //if (SQWSDK.isLogin)
        if (SQWSDK.isLogin)
        {
			isLogin = false;
#if UNITY_EDITOR
#elif UNITY_IOS  
            _logout();
#elif UNITY_ANDROID
			androidContext().Call("switchAccount");
#endif
        }
    }


    //第一拨没有
    public void SetUseInfo(string uid, string name)
    {
#if UNITY_EDITOR
#elif UNITY_IOS  
#elif UNITY_ANDROID
			
#endif
    }
    public void SubmitUpData(int type, string serverID, string serverName)
    {
#if UNITY_EDITOR
#elif UNITY_IOS
            _submitUpData(type, serverID, serverName);
#elif UNITY_ANDROID
			
#endif
    }

    //第一拨没有
    public void SubmitExtraData(int type, string serverID, string serverName, string roleID, string roleName, string roleLevel, string money, string roleCTime, string roleLevelIMTime, string vip, string power)
    {
        //if (!SQWSDK.isLogin) return;
#if UNITY_EDITOR
#elif UNITY_IOS  
        _submitExtraData(type, serverID, serverName, roleID, roleName, roleLevel, money, roleCTime, roleLevelIMTime, vip, power);       
        if (type == 1)
        {
            _submitLogin(roleID);
        }
        else if (type == 2)
        {
            _submitReg(roleID);
        }
#elif UNITY_ANDROID
#endif

    }


    //第一拨没有
    public void addBatchDataEvent(int childtype, string startid, int serverid, string act, string des, string logid)
    {
        SG.LogMgr.UnityLog("childtype :" + childtype + " startid:" + startid + " serverid:" + serverid + " act:" + act + " des: "+des + " logid:" + logid);
#if UNITY_EDITOR
#elif UNITY_IOS  
        _submitCustomDotData(act, des);
#elif UNITY_ANDROID
     
#endif
    }


    //设置数据
    public void SetUseData(string str)
    {
        //if (SQWSDK.isLogin)
        {
#if UNITY_EDITOR
#elif UNITY_IOS
            _submitRoleInfo(str);
#elif UNITY_ANDROID
			androidContext().Call("setData",str);
#endif
        }
    }

    //用户中心
    public void userCenter()
    {
#if UNITY_EDITOR
#elif UNITY_IOS  
#elif UNITY_ANDROID
			
#endif
    }

    //退出接口 
    public void exit()
    {
#if UNITY_EDITOR
#elif UNITY_IOS  
#elif UNITY_ANDROID
		androidContext().Call("exitGame");
#endif
    }

    //显示浮标 第一拨没有
    public void showFloatMenu()
    {
#if UNITY_EDITOR
#elif UNITY_IOS  
#elif UNITY_ANDROID
		
#endif
    }

    //隐藏浮标
    public void hideFloatMenu()
    {
#if UNITY_EDITOR
#elif UNITY_IOS  
#elif UNITY_ANDROID
		
#endif
    }

    //第一拨没有
    public void trackRechargeSuccess(int money)
    {
#if UNITY_EDITOR
#elif UNITY_IOS  
        _submitZFFinish(_orderID, money);
#elif UNITY_ANDROID
#endif
    }

    static public void activeInitializeIosUI()
    {
#if UNITY_EDITOR
#elif UNITY_IOS
        _activeInitializeIosUI();
#endif
    }

    public string appversion()
    {
#if UNITY_EDITOR
#elif UNITY_IOS   
        return _appversion();
#elif UNITY_ANDROID
		 
#endif
        return "";
    }

#if UNITY_IOS   //只针对37玩的接口
    [DllImport("__Internal")]
    protected static extern void _submitRoleInfo(string roleInfoJson);
    [DllImport("__Internal")]
    protected static extern string _getAppID();
#endif
}

