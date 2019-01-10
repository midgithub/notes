using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using SG;
using XLua;

[LuaCallCSharp]
[Hotfix]
public class XYAnalyser : MonoBehaviour, IThirdPartySDK
{
    //支付数据
    Dictionary<string, string> data = new Dictionary<string, string>();

    #region public methods
    private string sdkLoginData = "";
    private string sdkPayOrderId = "";
    private string sdkPayExtension = "";
    private string sdkServerList = "";
    private string sdkGameObject;
    public static bool isGetServerInfo = false;
    public static bool isRes = false;

    static int[] needAdaptsJRTT = { 370, 329, 307, 275, 369, 330, 276, 308, 371, 331, 309, 277, 310, 278, 372, 332, 311, 333, 279, 368 };

    public void Init(string gameObjectName)
    {
        //Debug.Log("xy rechargeData: ==========初始化西游SDK=======");
        SDKMgr.Instance.TrackGameLog("2000", "初始化西游SDK");

        sdkGameObject = gameObjectName;

        XYSDK.Instance.init(sdkGameObject, "initCallback", "loginCallback", "logoutCallback", "payCallBackMethod", "exitCallback");
        XYSDK.Instance.initAddict("addictCallback");

     
    }


    private void GE_XY_RECHARGE_MSG(SG.GameEvent ge, SG.EventParameter parameter)
    {
        MsgData_sGetRechargeorder data = parameter.msgParameter as MsgData_sGetRechargeorder;
        
        string jsonData = data.data.ToArray().BytesToString();
        Debug.Log("xy rechargeData: " + jsonData);
        JsonData json = JsonMapper.ToObject(jsonData);
        var tmp = json["orderNo"];
        if (tmp.IsString)
            sdkPayOrderId = (string)tmp;
        else
            sdkPayOrderId = "";

        tmp = json["extension"];
        if (tmp.IsObject || tmp.IsArray)
            sdkPayExtension = tmp.ToJson();
        else
            sdkPayExtension = (string)tmp;

        Debug.Log("sdkPayOrderId :" + sdkPayOrderId + " sdkPayExtension:" + sdkPayExtension + " itemID:" + data.ItemID);
        int configid = data.ItemID;
        SDKMgr.Instance.TagAdd(604, "SDK生成订单回调成功");

        CallBackPay(configid);
    }

    private void GE_XY_PAY_MSG(SG.GameEvent ge, SG.EventParameter parameter)
    {
        MsgData_WC_RechargeRet msg  = parameter.msgParameter as MsgData_WC_RechargeRet;
        Debug.Log("GE_XY_PAY_MSG " + msg.m_szOrderID + ":" + msg.m_szProductName);

        Debug.Log("TrackingPay: " + data["amount"] + " ： " + data["itemId"] + " : " + data["itemName"] + " : " + data["strPayOrder"]);
        XYSDK.Instance.TrackingPay(data["amount"], "1", data["itemId"], data["itemName"], data["strPayOrder"]);
    }


    public void Start()
    {
        SG.CoreEntry.gEventMgr.AddListener(SG.GameEvent.GE_XY_RECHARGE_MSG, GE_XY_RECHARGE_MSG);
        SG.CoreEntry.gEventMgr.AddListener(SG.GameEvent.GE_XY_PAY_MSG, GE_XY_PAY_MSG);
    }

    public void OnDestroy()
    { 
        SG.CoreEntry.gEventMgr.RemoveListener(SG.GameEvent.GE_XY_RECHARGE_MSG, GE_XY_RECHARGE_MSG);
        SG.CoreEntry.gEventMgr.RemoveListener(SG.GameEvent.GE_XY_PAY_MSG, GE_XY_PAY_MSG);

    }
    public void Login()
    {
        Debug.Log("XYAnalyser Login ");
        //兼容jrtt外发的马甲包SDK数据上报
        int versionCode = CommonTools.GetVersionCode();
        ArrayList list = new ArrayList(needAdaptsJRTT);
        if (list.Contains(versionCode))
        {
            StartCoroutine(ToutiaoInfo());
        }
        else
        {
            StartCoroutine(TrackingInfo());
        }
        SDKMgr.Instance.TrackGameLog("2100", "西游SDK登录");
        XYSDK.Instance.login();
    }

    public void Logout()
    {
        SDKMgr.Instance.TrackGameLog("2200", "西游SDK登出");
        XYSDK.Instance.logout();
    }
    public void Pay(int configID)
    {
        XLua.LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
        Dictionary<int, XLua.LuaTable> diamond = G.Get<Dictionary<int, XLua.LuaTable>>("t_diamond");
        if (diamond == null)
        {
            Debug.LogError("payerror:" + configID.ToString());
            return;
        }
        Dictionary<int, XLua.LuaTable> monthcard = G.Get<Dictionary<int, XLua.LuaTable>>("t_monthcard");
        if (monthcard == null)
        {
            Debug.LogError("payerror:" + configID.ToString());
            return;
        }
        //int amount = 1;
        //string itemName = "错误";
        XLua.LuaTable tbl = null;
        if (diamond.TryGetValue(configID, out tbl))
        {
            //int nbuy_android = tbl.Get<int>("buy_android");
            //int nbuy_ios = tbl.Get<int>("buy_ios");
            //int ndiamond = tbl.Get<int>("diamond");
            //amount = nbuy_android;
            //itemName = ndiamond.ToString() + uLocalization.Get("钻石");
        }
        else if (monthcard.TryGetValue(configID, out tbl))
        {
            //int card_price = tbl.Get<int>("card_price");
            //int card_type = tbl.Get<int>("card_type");
            //amount = card_price;
            //itemName = uLocalization.Get("月卡") + card_type.ToString();
        }
        else
        {
            Debug.LogError("payerror:" + configID.ToString());
            return;
        }
        SDKMgr.Instance.TagAdd(542, "客户端向游戏服发起生成订单请求-开始");
        NetLogicGame.Instance.SendReqGetRechargeorder(configID, 1, Account.Instance.ServerId,XYSDK.Instance.GetCurrPID(),Account.Instance.ServerName);
        SDKMgr.Instance.TagAdd(574, "客户端向游戏服发起生成订单请求-完成");
        //这里向游戏服发起充值 
    }
    public void CallBackPay(int configID)
    {
        XLua.LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
        Dictionary<int, XLua.LuaTable> diamond = G.Get<Dictionary<int, XLua.LuaTable>>("t_diamond");
        if (diamond == null)
        {
            Debug.LogError("payerror:" + configID.ToString());
            return;
        }
        Dictionary<int, XLua.LuaTable> monthcard = G.Get<Dictionary<int, XLua.LuaTable>>("t_monthcard");
        if (monthcard == null)
        {
            Debug.LogError("payerror:" + configID.ToString());
            return;
        }
        int amount = 1;
        string itemName = "错误";
        string itemDesc = "错误";
        XLua.LuaTable tbl = null;
        if (diamond.TryGetValue(configID, out tbl))
        {
            int nbuy_android = 0;
            //int nbuy_ios = 0;
#pragma warning disable 0219
            int ndiamond = 0;
            //nbuy_ios = tbl.Get<int>("buy_ios");
            ndiamond = tbl.Get<int>("diamond");

#if UNITY_EDITOR
            nbuy_android = tbl.Get<int>("buy_android");
#elif UNITY_IOS
            nbuy_android = tbl.Get<int>("buy_ios");
#elif UNITY_ANDROID
            nbuy_android = tbl.Get<int>("buy_android");
#endif 
            amount = nbuy_android;
            itemName = tbl.Get<string>("commodity_name");//ndiamond.ToString() + uLocalization.Get("钻石");
            itemDesc = tbl.Get<string>("description");
        }
        else if (monthcard.TryGetValue(configID, out tbl))
        {
            int card_price = tbl.Get<int>("card_price");
            int card_type = tbl.Get<int>("card_type");
            amount = card_price;
            itemName = uLocalization.Get("月卡") + card_type.ToString();
        }
        else
        {
            Debug.LogError("payerror:" + configID.ToString());
            return;
        }

        data["dataType"] = "6";
        data["roleID"] = PlayerData.Instance.RoleID.ToString();
        data["roleName"] = PlayerData.Instance.Name;
        data["roleLevel"] = PlayerData.Instance.BaseAttr.Level.ToString();
        data["zoneId"] = Account.Instance.ZoneID.ToString();
        data["zoneName"] = Account.Instance.ServerName;
        data["itemName"] = itemName;
        data["itemDesc"] = itemDesc;
        data["count"] = "1";
        data["amount"] = amount.ToString();
        data["itemId"] = configID.ToString();
        data["strPayOrder"] = sdkPayOrderId;
        data["strPayExtension"] = sdkPayExtension;
        data["callBackInfo"] = PlayerData.Instance.RoleID.ToString() + ";" + configID.ToString() + ";" + "0";

        Account.Instance.isRecharging = true;

#if UNITY_EDITOR
#elif UNITY_IOS
        //ios平台 走这里
        XYSDK.Instance.pay(0, data["strPayOrder"], data["strPayExtension"], amount, data["itemId"], data["itemName"], data["itemName"]);
#elif UNITY_ANDROID
        this.SetExtData(data);
#endif
        int versionCode = CommonTools.GetVersionCode();
        ArrayList list = new ArrayList(needAdaptsJRTT);
        if (list.Contains(versionCode))
        {
            //用作今日头条信息
            payToutiaoInfo._type = itemName;
            payToutiaoInfo._name = itemName;
            payToutiaoInfo._id = configID.ToString();
            payToutiaoInfo._num = 1;
            payToutiaoInfo._channel = "支付宝";
            payToutiaoInfo._currency = "人民币";
            payToutiaoInfo._success = true;
            payToutiaoInfo._amount = amount;

            Debug.Log("ToutiaoPurchase: " + amount + " : " + sdkPayOrderId + " : " + sdkPayExtension);
            XYSDK.Instance.ToutiaoPurchase();
            return;
        }



        Debug.Log("TrackingOrder: " + amount + " : " + sdkPayOrderId + " : " + sdkPayExtension);
        XYSDK.Instance.TrackingOrder(amount.ToString(), sdkPayOrderId, sdkPayExtension);
       
        
    }

    public void Exit()
    {
        //XYSDK.Instance.exit();
    }


    /// <summary>
    /// data 参数
    /// dataType -类型 type类型（1-启动，2-退出，3-角色升级，4-创建角色） -5上报数据 6 定额充值 7非定额充值
    //1、玩家进入游戏，即创建角色完成进入游戏
    //2、角色创建时调用该接口。
    //3、角色升级时调用该接口。
    //4、角色退出游戏时调用该接口。（包括用户退出游戏、用户注销角色账号）
    //5、玩家首次进入创角界面，且无角色时，调用一次			
    // type类型调用时机  1：登录(进入游戏区服时)，2，创建角色，3：升级，4退出(包括用户退出游戏、用户注销角色账号)
    //6 定额充值
    //7非定额充值
    //1000 兼容接口：sdk 1.8.8 以后版本 XiYouPlugins.getTag().add(code, desc);
    /// roleID  角色id
    /// roleName 角色名称
    /// roleLevel 角色等级
    /// zoneId 服务器id
    /// zoneName 服务器名称
    /// itemName 充值物品名
    /// itemId  充值物品id
    /// amount 购买价格分
    /// count 数量1
    /// callBackInfo 扩展字段
    /// </summary>
    /// <param name="data"></param>
    ///  
    public void SetExtData(System.Collections.Generic.Dictionary<string, string> data)
    {
        int type = 1;
        string strType = "1";

        string serverID = "";
        string serverName = "";
        string roleID = "";
        string roleName = "";
        string roleLevel = "";
        string money = "";
        string roleCTime = "1";
        string roleLevelIMTime ="1";

        if (data.TryGetValue("dataType", out strType))
        {
            int.TryParse(strType, out type);
        }
        data.TryGetValue("roleID", out roleID);
        if(string.IsNullOrEmpty(roleID))
        {
            roleID = "1";
        }
        data.TryGetValue("roleName", out roleName);
        if(string.IsNullOrEmpty(roleName))
        {
            roleName = "1";
        }
        data.TryGetValue("roleLevel", out roleLevel);
        if(string.IsNullOrEmpty(roleLevel))
        {
            roleLevel = "1";
        }
        data.TryGetValue("zoneId", out serverID);
        if(string.IsNullOrEmpty(serverID))
        {
            serverID = "1";
        }
        data.TryGetValue("zoneName", out serverName);
        if(string.IsNullOrEmpty(serverName))
        {
            serverName = "11";
        }
        data.TryGetValue("MoneyNum", out money);
        if(string.IsNullOrEmpty(money))
        {
            money = "0";
        }
        data["RoleCTime"] = Account.Instance.GetCurrRoleCreateTime().ToString();
        data["RoleLevelIMTime"] = UiUtil.GetNowTimeStamp().ToString();

        data.TryGetValue("RoleCTime", out roleCTime);
        if(string.IsNullOrEmpty(roleCTime))
        {
            roleCTime = UiUtil.GetNowTimeStamp().ToString();
        }
        data.TryGetValue("RoleLevelIMTime", out roleLevelIMTime);
        if(string.IsNullOrEmpty(roleLevelIMTime))
        {
            roleLevelIMTime = UiUtil.GetNowTimeStamp().ToString();
        }
        Debug.Log("SetExtData : " + data["dataType"] + " moneyNum : " + money);
        string vip = PlayerData.Instance.BaseAttr.VIPLevel.ToString();
        string power = PlayerData.Instance.BaseAttr.Power.ToString();
        if(string.IsNullOrEmpty(vip))vip = "0"; 
        if(string.IsNullOrEmpty(power))power = "1"; 
        data["vip"] = vip;
        data["power"] = power;

#if UNITY_EDITOR
#elif UNITY_IOS
        //ios平台 走这里
        //if (type == 1 || type == 2)
        //{
        //    roleID = SG.Account.Instance.AccountId;
        //}
        XYSDK.Instance.SubmitExtraData(type, serverID, serverName, roleID, roleName, roleLevel, money, roleCTime, roleLevelIMTime, vip, power);
#elif UNITY_ANDROID
        XYSDK.Instance.SetUseData(JsonMapper.ToJson(data));
#endif


    }
    public void ReleaseResource()
    {
    }
    public string GetExtName(ThirdParty_ExtNameType type)
    {
        if (type == ThirdParty_ExtNameType.ACTION_ENTER_SERVER)
        {
            return sdkLoginData;
        }
        else if(type == ThirdParty_ExtNameType.APP_SERVERlIST)
        {
            return sdkServerList;
        }
        return "";
    }
    public void EnterUserCenter()
    {
        XYSDK.Instance.userCenter();
    }
    public bool IsLogin()
    {
        return XYSDK.isLogin;
    }
    #endregion

    #region callback methods
    void initCallback(string result)
    {
        SDKMgr.Instance.TrackGameLog("2001", "西游SDK初始化回调");

        Debug.Log("initCallback:" + result);
        string resultCode = result;
        if (resultCode == XYSDK.SUCCESS)
        {
            SG.CoreEntry.gEventMgr.TriggerEvent(SG.GameEvent.GE_THIRDPARTY_INIT, null);
        }
        else
        {
            SG.CoreEntry.gEventMgr.TriggerEvent(SG.GameEvent.GE_THIRDPARTY_INIT, EventParameter.Get(0));
        } 
    }

    void loginCallback(string result)
    {
        Debug.Log("loginCallback:" + result);
        string resultCode = result.Substring(0, 1);
        if (!SDKMgr.Instance.bLoginStatus)
        {
            Debug.Log(" init sdk !!!!");
            return;
        }

        string data = result.Substring(1);
        Debug.Log("data:" + data);
        Debug.Log("resultCode:" + resultCode);
        if (resultCode == XYSDK.SUCCESS)
        {
            SDKMgr.Instance.TrackGameLog("2101", "西游SDK登录回调");
            sdkLoginData = data;
            XYSDK.isLogin = true;
            XYSDK.Instance.showFloatMenu();
            //if (string.IsNullOrEmpty(sdkServerList))
            {
                SDKMgr.Instance.TrackGameLog("2300", "请求获取西游SDK服务器列表");
                XYSDK.Instance.requestAvailableServer();
            }
//             else
//             {
//                 EventParameter ev = EventParameter.Get();
//                 ev.stringParameter = sdkServerList;
//                 ev.intParameter = 0;//获取成功
//                 CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_XY_SERVERLIST, ev); 
//             }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_THIRDPARTY_LOGIN, null);

        }
        else if (resultCode == XYSDK.SUCCESS_WITCHACCOUNT)
        {
            sdkLoginData = data;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_THIRDPARTY_LOGIN, null);
            XYSDK.isLogin = true;
            //if(string.IsNullOrEmpty(sdkServerList))
            {
                SDKMgr.Instance.TrackGameLog("2300", "请求获取西游SDK服务器列表");
                XYSDK.Instance.requestAvailableServer();
            }
//             else
//             {
//                 EventParameter ev = EventParameter.Get();
//                 ev.stringParameter = sdkServerList;
//                 ev.intParameter = 0;//获取成功
//                 CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_XY_SERVERLIST, ev); 
//             }

            Dictionary<string, string> datasm = new Dictionary<string, string>();
            datasm["dataType"] = "4";
            datasm["roleID"] = PlayerData.Instance.RoleID.ToString();
            datasm["roleName"] = PlayerData.Instance.Name;
            datasm["roleLevel"] = PlayerData.Instance.BaseAttr.Level.ToString();
            datasm["zoneId"] = Account.Instance.ZoneID.ToString();
            datasm["zoneName"] = Account.Instance.ServerName;
            datasm["MoneyNum"] = PlayerData.Instance.UnbindMoney.ToString();

            this.SetExtData(datasm);

        }
        else if(resultCode == XYSDK.FAIL)
        {
            SDKMgr.Instance.TrackGameLog("2101", "西游SDK登录回调");
            XYSDK.isLogin = false;
            sdkLoginData = data;
            //XYSDK.Instance.login();
        } 
        else if(resultCode == XYSDK.SUCCESS_SERVERLIST)
        {
            SDKMgr.Instance.TrackGameLog("2301", "获取西游SDK服务器列表回调成功");
            sdkServerList = data;
            EventParameter ev = EventParameter.Get();
            ev.stringParameter = data;
            ev.intParameter = 0;//获取成功
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_XY_SERVERLIST, ev);
        }
        else if(resultCode == XYSDK.FAIL_SERVERLIST)
        {
            SDKMgr.Instance.TrackGameLog("2302", "获取西游SDK服务器列表回调失败");
            EventParameter ev = EventParameter.Get();
            ev.stringParameter = data;
            ev.intParameter = 1;//失败
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_XY_SERVERLIST, ev);

        }
    }

    void logoutCallback(string result)
    {
        Debug.Log("logoutCallback:" + result);
        if (!SDKMgr.Instance.bLoginStatus)
        {
            Debug.Log(" init sdk !!!!");
            return;
        }

        SDKMgr.Instance.TrackGameLog("2201", "西游SDK登出回调");
        string resultCode = result.Substring(0, 1);
        if (resultCode == XYSDK.SUCCESS)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["dataType"] = "4";
            data["roleID"] = PlayerData.Instance.RoleID.ToString();
            data["roleName"] = PlayerData.Instance.Name;
            data["roleLevel"] = PlayerData.Instance.BaseAttr.Level.ToString();
            data["zoneId"] = Account.Instance.ZoneID.ToString();
            data["zoneName"] = Account.Instance.ServerName;
            data["MoneyNum"] = PlayerData.Instance.UnbindMoney.ToString();

            this.SetExtData(data);

            XYSDK.isLogin = false;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_THIRDPARTY_LOGOUT, null);
        }
    }


    void payCallBackMethod(string result)
    {
        Debug.Log("payCallBackMethod:" + result);

        int versionCode = CommonTools.GetVersionCode();
        ArrayList list = new ArrayList(needAdaptsJRTT);
        if (list.Contains(versionCode))
        {
            XYSDK.Instance.ToutiaoPurchase();
            return;
        }

        {
            if (int.Parse(result) >= 8000)
            {
                XYSDK.Instance.payCallBackMethod(0, data["strPayOrder"], data["strPayExtension"], int.Parse(data["amount"]), data["itemId"], data["itemName"], data["itemName"]);
#if UNITY_ANDROID
                //Debug.Log("TrackingPay: " + data["amount"] + " ： " + data["itemId"] + " : " + data["itemName"] + " : " + data["strPayOrder"]);
                //XYSDK.Instance.TrackingPay(data["amount"], "1", data["itemId"] , data["itemName"], data["strPayOrder"]);
#endif
            }
        }
    }

    void exitCallback(string result)
    {
        Debug.Log("exitCallback:" + result);
        int exitCode = 1;

        Dictionary<string, string> data = new Dictionary<string, string>();
        data["dataType"] = "4";
        if (Account.Instance.RoleNum > 0)
        {
            data["roleID"] = PlayerData.Instance.RoleID.ToString();
            data["roleName"] = PlayerData.Instance.Name;
            data["roleLevel"] = PlayerData.Instance.BaseAttr.Level.ToString();
            data["zoneId"] = Account.Instance.ZoneID.ToString();
            data["zoneName"] = Account.Instance.ServerName;
            data["MoneyNum"] = PlayerData.Instance.UnbindMoney.ToString();        
        }
        else
        {
            data["roleID"] = "0";
            data["roleName"] = " ";
            data["roleLevel"] = "0";
            data["zoneId"] = "0";
            data["zoneName"] = "no";
            data["MoneyNum"] = "0"; 
        }
        SDKMgr.Instance.SetExtData(data);
        //int.TryParse(result, out exitCode);
        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_THIRDPARTY_EXIT, EventParameter.Get(exitCode));
        XYSDK.isLogin = false;
         
    }

    void addictCallback(string result)
    {
        Debug.Log("addictCallback:" + result);

        if(result == "0")
        {
            Account.Instance.IsAdult = 0;
        }
        else
        {
            Account.Instance.IsAdult = 1;
        }
    }
    #endregion

    IEnumerator ToutiaoInfo()
    {
        Debug.Log("头条========ToutiaoInfo============");
        yield return new WaitForSeconds(2f);
        if (!isGetServerInfo)
        {
            StartCoroutine(ToutiaoInfo());
            yield break;
        }
        if (isRes)
        {
            Debug.Log("头条========注册============");
            XYSDK.Instance.ToutiaoRegister();
        }
        yield return new WaitForSeconds(1f);
        XYSDK.Instance.ToutiaoLogin();
        Debug.Log("头条========登录============");

        isGetServerInfo = false;
        isRes = false;
        yield break;
    }

    IEnumerator GDTInfo()
    {
        // Debug.Log("广点通========GDTInfo============");
        yield return new WaitForSeconds(2f);
        if (!isGetServerInfo)
        {
            StartCoroutine(GDTInfo());
            yield break;
        }
        if (isRes)
        {
            Debug.Log("头条========注册============");
            XYSDK.Instance.GDTRegister();
        }
        yield return new WaitForSeconds(1f);
        //XYSDK.Instance.GDTStartApp();
        //  Debug.Log("头条========登录============");

        isGetServerInfo = false;
        isRes = false;
        yield break;
    }
    public string appversion()
    { 
       return XYSDK.Instance.appversion();
    }

    IEnumerator TrackingInfo()
    {
        Debug.Log("========TrackingInfo============");
        yield return new WaitForSeconds(2f);
        if (!isGetServerInfo)
        {
            StartCoroutine(TrackingInfo());
            yield break;
        }
        if (isRes)
        {
            Debug.Log("========注册============" + Account.Instance.sdkUserId);
            XYSDK.Instance.TrackingRegister(Account.Instance.sdkUserId.ToString());
        }
        yield return new WaitForSeconds(1f);
        XYSDK.Instance.TrackingLogin(Account.Instance.sdkUserId.ToString());
        Debug.Log("========登录============" + Account.Instance.sdkUserId);

        isGetServerInfo = false;
        isRes = false;
        yield break;
    }
}

