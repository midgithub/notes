using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;
using XLua;
using LitJson;

/**
* @file     : DYBAnysis
* @brief    : 第一波SDK解析器
* @details  : SDK接口子类，调用DYBSDK进行操作
* @author   : 
* @date     : 2018.10.10
*/

[LuaCallCSharp]
[Hotfix]
public class DYBAnysis : MonoBehaviour, IThirdPartySDK {

    //初始化字段
    private string sdkLoginData = "";
    private string sdkPayOrderId = "";
    private string sdkPayExtension = "";
    private string sdkServerList = "";
    private string sdkDefalutList = ""; //历史登录服务器
    private string sdkGameObject;
    public static bool isGetServerInfo = false;
    public static bool isRes = false;
    private string userID = "";                        //登录返回的用ID

    public void Start()
    {
        SG.CoreEntry.gEventMgr.AddListener(SG.GameEvent.GE_DYB_RECHARGE_MSG, GE_DYB_RECHARGE_MSG);
      
    }

    public void OnDestroy()
    {
        SG.CoreEntry.gEventMgr.RemoveListener(SG.GameEvent.GE_DYB_RECHARGE_MSG, GE_DYB_RECHARGE_MSG);
    }

    /// <summary>
    /// 初始化 主要传入Unity 对象和各回调函数名称
    /// </summary>
    /// <param name="gameObject"></param>
    public void Init(string gameObject)
    {
        Debug.Log("初始化第一波SDK");
        sdkGameObject = gameObject;
        DYBSDK.Instance.init(sdkGameObject, "initCallback", "loginCallback", "logoutCallback", "payCallBackMethod", "exitCallback");
    }

    public void Login()
    {
        Debug.Log("第一波SDK登录");
        DYBSDK.Instance.login();
    }

    public void Logout()
    {
        Debug.Log("第一波SDK登出");
        DYBSDK.Instance.logout();
    }

    /// <summary>
    /// 支付
    /// </summary>
    /// <param name="configID"></param>
    public void Pay(int configID)
    {
        Debug.Log("第一波SDK支付");
        XLua.LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
        Dictionary<int, XLua.LuaTable> diamond = G.Get<Dictionary<int, XLua.LuaTable>>("t_diamonddyb");
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
        NetLogicGame.Instance.SendReqGetRechargeorder_DYB(configID, 1, Account.Instance.ServerId, DYBSDK.Instance.GetCurrPID(), Account.Instance.ServerName);
        //这里向游戏服发起充值 
    
    }

    public void Exit()
    {
        //DYBSDK.Instance.exit();
    }

    //设置角色信息
    public void SetExtData(Dictionary<string, string> data)
    {
        //角色创建时间
        data["createTime"] = Account.Instance.GetCurrRoleCreateTime().ToString();
        Debug.Log("setData : " + data["dataType"]);
#if UNITY_EDITOR
#elif UNITY_IOS
        //ios平台 走这里
       DYBSDK.Instance.SetUseData(JsonMapper.ToJson(data));
#elif UNITY_ANDROID
        DYBSDK.Instance.SetUseData(JsonMapper.ToJson(data));
#endif
    }

    public void ReleaseResource()
    {
    
    }
    /// <summary>
    /// 进入用户中心
    /// </summary>
    public void EnterUserCenter()
    {
        DYBSDK.Instance.userCenter();
    }

    //是否已登录
    public bool IsLogin()
    {
        return DYBSDK.isLogin;
    }

    /// <summary>
    /// 服务器充值订单回复
    /// </summary>
    /// <param name="ge"></param>
    /// <param name="parameter"></param>
    private void GE_DYB_RECHARGE_MSG(SG.GameEvent ge, SG.EventParameter parameter)
    {       
        MsgData_sGetRechargeorder_DYB data = parameter.msgParameter as MsgData_sGetRechargeorder_DYB;
        Debug.Log("订单号 : " + data.data.ToString());
        Debug.Log("大小  : " + data.dataSize.ToString());
        Debug.Log("ID : " + data.ItemID.ToString());

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
        CallBackPay(configid);
    }


    public void CallBackPay(int configID)
    {
        Debug.Log("服务器订单回调：：：：" + configID);
        XLua.LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
        Dictionary<int, XLua.LuaTable> diamond = G.Get<Dictionary<int, XLua.LuaTable>>("t_diamonddyb");
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
        string iosshopid = "";
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
        System.Collections.Generic.Dictionary<string, string> data = new System.Collections.Generic.Dictionary<string, string>();
        iosshopid = tbl.Get<string>("iosshopid");

        data["orderId"] = sdkPayOrderId;    //订单号
        data["productId"] = configID.ToString();   //商品ID
        data["productDes"] = itemDesc;            //商品描述
        data["productName"] = itemName;            //商品名字
        data["money"] = (amount/100).ToString();                 //商品金额
        data["count"] = "1";                    //商品数量
        data["coinType"] = "人民币";            //币种
        int nServerId = Account.Instance.ServerId;
        data["serverId"] = Account.Instance.ServerId.ToString();    //区服ID
        data["serverName"] = Account.Instance.ServerName;           //区服名称
        data["roleId"] = PlayerData.Instance.RoleID.ToString();     //角色ID
        data["roleName"] = PlayerData.Instance.Name;                //角色名称
        data["roleLevel"] = PlayerData.Instance.BaseAttr.Level.ToString();  //角色等级
        data["vipLevel"] = PlayerData.Instance.BaseAttr.VIPLevel.ToString();        //vip等级
        data["roleBalance"] = PlayerData.Instance.BindGold.ToString();              //金币
        data["society"] = PlayerData.Instance.GuildData.GuildName;          //公会
        if (data["society"] == "")
            data["society"] = "无";
        data["exchangeRate"] = "";     //兑换率
        data["extraInfo"] = sdkPayExtension;            //额外信息          
        Account.Instance.isRecharging = true;
        int nType = 0;
        //DYBSDK.Instance.pay(nType, sdkPayOrderId, sdkPayExtension, amount, data["productId"], itemName, itemDesc);
#if UNITY_EDITOR
#elif UNITY_IOS
        //ios平台 走这里
        Debug.Log("第一波 ios平台 充值接口:");
        DYBSDK.Instance.pay(nServerId, sdkPayOrderId, sdkPayExtension, amount, iosshopid, itemName, itemDesc);
#elif UNITY_ANDROID
        DYBSDK.Instance.setPayData(JsonMapper.ToJson(data));
#endif
    }


    #region     回调函数

    public void initCallback(string result)
    {
        Debug.Log("initCallback:" + result);
        string resultCode = result;
        //判断是否成功
        if (resultCode == DYBSDK.SUCCESS)
        {
            SG.CoreEntry.gEventMgr.TriggerEvent(SG.GameEvent.GE_THIRDPARTY_INIT, null);      
        }
        else
        {
            SG.CoreEntry.gEventMgr.TriggerEvent(SG.GameEvent.GE_THIRDPARTY_INIT, EventParameter.Get(0));
          
        }
    }

    /// <summary>
    /// 登录回调，第一波需要重写
    /// </summary>
    /// <param name="result"></param>
    public void loginCallback(string result)
    {     

        Debug.Log("loginCallback:" + result);
        string resultCode = result.Substring(0, 1);
        string data = result.Substring(1);      
        //JsonData dataJson = JsonMapper.ToObject(data);
        //Debug.Log("data:" + dataJson.ToString());
        //Debug.Log("tokens:" + dataJson["tokens"].ToString());
        //Debug.Log("userId:" + dataJson["userId"].ToString());
        //Debug.Log("userName:" + dataJson["userName"].ToString());
        Debug.Log("resultCode:" + resultCode);
        if (resultCode == DYBSDK.SUCCESS)
        {
            Debug.Log("DYBSDK.SUCCESS:" + data.ToString());
            sdkLoginData = data;
            DYBSDK.isLogin = true;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_THIRDPARTY_LOGIN, null);
        }
         //切换账号
        else if (resultCode == DYBSDK.SUCCESS_WITCHACCOUNT)
        {
            sdkLoginData = data;
            DYBSDK.isLogin = true;
            Debug.Log("DYBSDK.SUCCESS_WITCHACCOUNT:" + data.ToString());
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_THIRDPARTY_LOGIN, null);
          
        }
        else if (resultCode == DYBSDK.FAIL)
        {
            DYBSDK.isLogin = false;
            sdkLoginData = data;
            Debug.Log("DYBSDK.FAIL:" + data.ToString());
            //DYBSDK.Instance.login();
        }
        else if (resultCode == DYBSDK.USERID)
        {
            //DYBSDK.Instance.userID = dataJson["userId"].ToString();
            DYBSDK.Instance.userID = data;
            Debug.Log("DYBSDK.USERID:" + DYBSDK.Instance.userID);
            DYBSDK.isLogin = true;
            //获取DYB服务器列表
            startFreshServerList();
        }
        
    }

    public void logoutCallback(string result)
    {
        Debug.Log("logoutCallback:" + result);
        string resultCode = result.Substring(0, 1);
        if (resultCode == DYBSDK.SUCCESS)
        {
            SDKMgr.Instance.SendDYBData("4");
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_THIRDPARTY_LOGOUT, null);
            DYBSDK.isLogin = false;
        }
    }


    public void payCallBackMethod(string result)
    {
        Debug.Log("payCallBackMethod:" + result);
    }
    public void exitCallback(string result)
    {
        Debug.Log("exitCallback:" + result);
        int exitCode = 1;
        int level = PlayerData.Instance.BaseAttr.Level;
        if (Account.Instance.RoleInfoList != null)
        {
            MsgData_sLoginRole roleInfo = Account.Instance.RoleInfoList.Find(s => s.ID == PlayerData.Instance.RoleID);
            if (roleInfo != null)
            {
                level = roleInfo.Level;
            }
        }
        //进入游戏
        System.Collections.Generic.Dictionary<string, string> data = new System.Collections.Generic.Dictionary<string, string>();
        data["dataType"] = "4";
        data["roleLevel"] = level.ToString();
        data["roleId"] = PlayerData.Instance.RoleID.ToString();
        data["roleName"] = PlayerData.Instance.Name;
        data["vipLevel"] = PlayerData.Instance.BaseAttr.VIPLevel.ToString();
        //金币
        data["roleBalance"] = PlayerData.Instance.BindGold.ToString();
        data["serverName"] = Account.Instance.ServerName;
        data["serverId"] = Account.Instance.ServerId.ToString();
        data["society"] = PlayerData.Instance.GuildData.GuildName;
        if (data["society"] == "")
            data["society"] = "无";
        this.SetExtData(data);
        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_THIRDPARTY_EXIT, EventParameter.Get(exitCode));
        DYBSDK.isLogin = false;
    }

    /// <summary>
    /// 返回版本
    /// </summary>
    /// <returns></returns>
    public string appversion()
    {
        return DYBSDK.Instance.appversion();
    }


    /// <summary>
    /// 获取渠道信息
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string GetExtName(ThirdParty_ExtNameType type)
    {
        if (type == ThirdParty_ExtNameType.ACTION_ENTER_SERVER)
        {
            return sdkLoginData;
        }
        else if (type == ThirdParty_ExtNameType.APP_SERVERlIST)
        {
            return DYBSDK.Instance.sdkServerList;
        }
        else if (type == ThirdParty_ExtNameType.APP_DEFAULTLIST)
            return DYBSDK.Instance.sdkDefalutList;
        return "";
    }


    /// <summary>
    /// 获取第一波服务器列表
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetDYBServerList()
    {
        if (DYBSDK.isLogin)
        {        
            int  isfail = 0;
            //获取历史和推荐服务器
            WWWForm form = new WWWForm();
            form.AddField("userID", DYBSDK.Instance.userID);
            //token加密
            string md5=Md5Util.MD5Encrypt(DYBSDK.Instance.userID + "6842c6012614bd474f75d1067f3e805f");
            form.AddField("token", md5);
            Debug.Log("发送参数：" + "userID: " + DYBSDK.Instance.userID + "key; " + "6842c6012614bd474f75d1067f3e805f");
            Debug.Log("加密后：" + form.data.BytesToString());
			
			form.AddField("package", Application.bundleIdentifier);
            Debug.Log("package：" + Application.bundleIdentifier);
			
            WWW history = new WWW("http://init.wqyry.szdiyibo.com/server/loginlist.php", form);
            yield return history;
            if (history.isDone)
            {                
                DYBSDK.Instance.sdkDefalutList = history.text;
                Debug.Log("推荐服务器列表：" + DYBSDK.Instance.sdkDefalutList);
                //获取公共服务器列表
				//WWW ser = new WWW("http://init.wqyry.szdiyibo.com/server/serverlist_pack.php");

                WWWForm formServers = new WWWForm();
                formServers.AddField("userID", DYBSDK.Instance.userID);
                formServers.AddField("package", Application.bundleIdentifier);
                formServers.AddField("version", ClientSetting.Instance.GetIntValue("PackageVersion"));

                string key = "7ada175d3828b656db7eda80dc7d58b3";
                string token = Md5Util.MD5Encrypt(key + DYBSDK.Instance.userID + Application.bundleIdentifier + ClientSetting.Instance.GetIntValue("PackageVersion"));
                formServers.AddField("token", token);

                Debug.Log("发送服务器列表参数：" + "userID: " + DYBSDK.Instance.userID + "package: " + Application.bundleIdentifier + "version: " + ClientSetting.Instance.GetIntValue("PackageVersion"));
				WWW ser = new WWW("http://init.wqyry.szdiyibo.com/server/serverlist_pack.php",formServers);
                yield return ser;
                if (ser.isDone)
                {
                    Debug.Log("服务器列表： " + ser.text);
                    DYBSDK.Instance.sdkServerList = ser.text;
                }
                else if (!string.IsNullOrEmpty(ser.error))
                {
                    isfail = 1;
                    Debug.Log("获取服务器列表失败：" + ser.error);                  
                }                  
            } 
            else if(!string.IsNullOrEmpty(history.error))
            {
               isfail = 1;
                Debug.Log("获取推荐服务器列表失败：" + history.error);            
            }

            //根据状态发送消息
            EventParameter ev = EventParameter.Get();
            ev.intParameter = isfail;//获取状态 0成功 1失败
            Debug.Log("服务器列表：" + isfail);
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_DYB_SERVERLIST, ev);
            if (1 == isfail)
            {                
                UITips.ShowTips("重新刷新服务器列表");
                StartCoroutine(GetDYBServerList());
            }
        }
       
    }

    /// <summary>
    /// 获取服务器列表
    /// </summary>
    public void startFreshServerList()
    {
        StartCoroutine(GetDYBServerList());
    }
    #endregion
}

