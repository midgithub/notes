using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using XLua;
using System.Collections.Generic;
using SG;
using System;

[LuaCallCSharp]
[Hotfix]
public class XYSDK: SDKChannel
{
    private static XYSDK _instance = null;
    public static XYSDK Instance
    {
        get
        {
            if (_instance == null)
                _instance = new XYSDK();

            return _instance;
        }
    }

    // type类型（1-启动，2-退出，3-角色升级，4-创建角色） -5上报数据
    //6 定额充值
    //7非定额充值

    public static bool isLogin = false;
    private XYSDK()
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
       // Debug.LogError("=====初始化接口==========="+ gameObject);
        _init(gameObject);
#elif UNITY_ANDROID
		androidContext().Call("doInit", gameObject, initCallbackMethod, loginCallbackMethod, logoutCallbackMethod, payCallBackMethod,exitCallbackMethod);
#endif
    }

    public void initAddict(string addictCallbackMethod)
    {
#if UNITY_EDITOR
#elif UNITY_IOS
   
#elif UNITY_ANDROID
		androidContext().Call("doInitAddict", addictCallbackMethod);
#endif
    }

    //防沉迷接口
    public void queryAntiAddiction()
    {
        Debug.Log("queryAntiAddiction");
#if UNITY_EDITOR
#elif UNITY_IOS  
#elif UNITY_ANDROID
		androidContext().Call("queryAntiAddiction");
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

    // 登录接口
    public bool hasLogout()
    {
        Debug.Log("hasLogout");
        bool bres = true; 
#if UNITY_EDITOR
#elif UNITY_IOS  
#elif UNITY_ANDROID
        bres =  androidContext().Call<bool>("hasLogout");
#endif
        Debug.Log("hasLogout" + bres.ToString());
        return bres;
    }

    public string getMasterID()
    {
        //Debug.Log("getMasterID");
        string bres = "43";
#if UNITY_EDITOR
#elif UNITY_IOS  

       bres = _getChannelID();
       if(bres ==""||bres == string.Empty)bres = "0";
#elif UNITY_ANDROID
        bres = androidContext().Call<string>("getMasterID");
#endif
        //Debug.Log("getMasterID" + bres.ToString());
        return bres;
    }
    public string getIdfa()
    {
        //Debug.Log("getMasterID");
        string bres = "";
#if UNITY_EDITOR
#elif UNITY_IOS  
       bres = _getIDFA();
#endif
        //Debug.Log("getMasterID" + bres.ToString());
        return bres;
    }

    // 充值接口
    public void pay(int type, string orderID, string extension, int money, string productID, string productName, string productDesc)
    {
        //if (isLogin) return;
#if UNITY_EDITOR
#elif UNITY_IOS  

        _pay(type, orderID, extension, money, productID, productName, productDesc);

        _orderID = orderID;
        _submitZFStart(orderID, money);

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
    public void changeAccount()
    {
        if (XYSDK.isLogin)
        {
#if UNITY_EDITOR
#elif UNITY_IOS  
#elif UNITY_ANDROID
			androidContext().Call("changeAccount");
#endif
        }
    }

    public string getUserID()
    {
#if UNITY_EDITOR
#elif UNITY_IOS  
#elif UNITY_ANDROID
         return androidContext().Call<string>("getUserID");
#endif
        return "";
    }

    public void requestAvailableServer()
    {
        if (XYSDK.isLogin)
        {
#if UNITY_EDITOR
#elif UNITY_IOS  
            _getServerList();
#elif UNITY_ANDROID
			androidContext().Call("requestAvailableServer");
#endif
        }
    }


    public string  GetCurrPID()
    {
        if (XYSDK.isLogin)
        {
#if UNITY_EDITOR
#elif UNITY_IOS  
            return _getCurrPID();
#elif UNITY_ANDROID
            int flag = androidContext().Call<int>("Getflag");
			return flag.ToString();
#endif
        }
        return "";
    }

    public string GetAppID()
    {
#if UNITY_EDITOR
#elif UNITY_IOS  
            return "104226387836";
#elif UNITY_ANDROID
			return "101128378768";
#endif
        return "104226387836";
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
        if (XYSDK.isLogin)
        {
#if UNITY_EDITOR
#elif UNITY_IOS  
            _logout();
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

    public void SubmitExtraData(int type, string serverID, string serverName, string roleID, string roleName, string roleLevel, string money, string roleCTime, string roleLevelIMTime, string vip, string power)
    {
        //if (!XYSDK.isLogin) return;
#if UNITY_EDITOR
#elif UNITY_IOS  

        _submitExtraData(type, serverID, serverName, roleID, roleName, roleLevel, money, roleCTime, roleLevelIMTime, vip, power);
        if (type == 1 || type == 2)
        {
            roleID = SG.Account.Instance.AccountId;
        }
        if (type == 1)
        {
            _submitLogin(roleID);
        }
        else if (type == 2)
        {
            if (SG.Account.Instance.RoleInfoList.Count == 1)
            {
                if(SG.Account.Instance.loggedServers == 0)
                    _submitReg(roleID);
            }
        }
#elif UNITY_ANDROID
#endif

    }

    public void addBatchDataEvent(int childtype, string startid, int serverid, string act, string des, string logid)
    {
        SG.LogMgr.UnityLog("childtype :" + childtype + " startid:" + startid + " serverid:" + serverid + " act:" + act + " des: "+des + " logid:" + logid);
#if UNITY_EDITOR
#elif UNITY_IOS  
        _submitCustomDotData(act, des);
#elif UNITY_ANDROID
        androidContext().Call("addBatchDataEvent", childtype, startid, serverid, act, des, logid);
#endif
    }

    public void SetUseData(string str)
    {
        //if (XYSDK.isLogin)
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

    //头条注册
    public void ToutiaoRegister()
    {
#if UNITY_ANDROID
        try
        {
            androidContext().Call("TouTiaoRegister");
        }
        catch (Exception e) {
            Debug.Log(e.StackTrace);
        }
#endif
    }
    //头条登录
    public void ToutiaoLogin()
    {
#if UNITY_ANDROID
        try
        {
            androidContext().Call("TouTiaoLogin");
        }
        catch (Exception e) {
            Debug.Log(e.StackTrace);
        }
#endif
    }
    //头条支付
    public void ToutiaoPurchase()
    {
        string _type = payToutiaoInfo._type;
        string _name = payToutiaoInfo._name;
        string _id = payToutiaoInfo._id;
        int _num = payToutiaoInfo._num;
        string _channel = payToutiaoInfo._channel;
        string _currency = payToutiaoInfo._currency;
        bool _issuccess = payToutiaoInfo._success;
        int _amount = payToutiaoInfo._amount;
#if UNITY_ANDROID
        try
        {
            androidContext().Call("TouTiaoPurchase", _type, _name, _id, _num, _channel, _currency, _issuccess, _amount);
        }
        catch (Exception e) {
            Debug.Log(e.StackTrace);
        }
#endif
    }

    //注册
    public void GDTRegister()
    {
#if UNITY_ANDROID
        try
        {
            androidContext().Call("GDT_Register");
        }
        catch (Exception e) {
            Debug.Log(e.StackTrace);
        }
#endif
    }
    //启动
    public void GDTStartApp()
    {
#if UNITY_ANDROID
        try
        {
            androidContext().Call("GDT_StartApp");
        }
        catch (Exception e) {
            Debug.Log(e.StackTrace);
        }
#endif
    }
    //支付
    public void GDTPurchase()
    {
#if UNITY_ANDROID
        string _type = payToutiaoInfo._type;
        string _name = payToutiaoInfo._name;
        string _id = payToutiaoInfo._id;
        int _num = payToutiaoInfo._num;
        string _channel = payToutiaoInfo._channel;
        string _currency = payToutiaoInfo._currency;
        bool _issuccess = payToutiaoInfo._success;
        int _amount = payToutiaoInfo._amount;
        try
        {
            androidContext().Call("GDT_Purchase", _type, _name, _id, _num, _channel, _currency, _issuccess, _amount);
        }
        catch (Exception e) {
            Debug.Log(e.StackTrace);
        }
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

    public void TrackingRegister(string accoutId)
    {
#if UNITY_ANDROID
        try
        {
            androidContext().Call("TrackingRegister", accoutId);
        }
        catch (Exception e) {
            Debug.Log(e.StackTrace);
        }
#endif

    }

    public void TrackingLogin(string accoutId)
    {
#if UNITY_ANDROID
        try
        {
            androidContext().Call("TrackingLogin", accoutId);
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
        }
#endif

    }

    public void TrackingOrder(string amount, string orderId, string extension)
    {
#if UNITY_ANDROID
        try
        {
            androidContext().Call("TrackingOrder", amount, orderId, extension);
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
        }
#endif

    }

    public void TrackingPay(string amount, string count, string productId, string productName, string orderId)
    {
#if UNITY_ANDROID
        try
        {
            androidContext().Call("TrackingPay", amount, count, productId, productName, orderId);
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
        }
#endif

    }

}

