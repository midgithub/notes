using XLua;
﻿using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using SG;

[Hotfix]
public class SGAnalyser : MonoBehaviour, IThirdPartySDK
{
    #region public methods
    private string sdkLoginData="";
    private string sdkGameObject;
    public void Init(string gameObjectName)
    {
        sdkGameObject = gameObjectName;

        SGSDK.instance.init(sdkGameObject, "initCallback", "loginCallback", "logoutCallback", "payCallBackMethod", "exitCallback");
    }

    private void GE_LOGIN_MSG(SG.GameEvent ge, SG.EventParameter parameter)
    {
        MsgData_sLogin data = parameter.msgParameter as MsgData_sLogin;

        if (0 == data.ResultCode || -1 == data.ResultCode)
        {
            SGSDK.instance.SetUseInfo(SG.Account.Instance.AccountId, "");
        }
    }
    public void Start()
    {
        SG.CoreEntry.gEventMgr.AddListener(SG.GameEvent.GE_LOGIN_MSG, GE_LOGIN_MSG);
    }

    public void OnDestroy()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["dataType"] = "2";
        this.SetExtData(data);
        SG.CoreEntry.gEventMgr.RemoveListener(SG.GameEvent.GE_LOGIN_MSG, GE_LOGIN_MSG);
    }
    public void Login()
    {
        SGSDK.instance.login();
    }

    public void Logout()
    {
        SGSDK.instance.logout();
    }

    public void Pay(int configID)
    {
        XLua.LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
        Dictionary<int, XLua.LuaTable> diamond = G.Get<Dictionary<int, XLua.LuaTable>>("t_diamond");
        if(diamond == null)
        {
            Debug.LogError("payerror:" + configID.ToString());
            return;
        }
        Dictionary<int, XLua.LuaTable> monthcard = G.Get<Dictionary<int, XLua.LuaTable>>("t_monthcard");
        if(monthcard == null)
        {
            Debug.LogError("payerror:" + configID.ToString());
            return;
        }
        int amount = 1;
        string itemName = "错误";
        XLua.LuaTable tbl = null;
        if(diamond.TryGetValue(configID,out tbl))
        {
            int nbuy_android = 0;
            //int nbuy_ios = 0;
            int ndiamond = 0;
            nbuy_android  = tbl.Get<int>("buy_android");
            //nbuy_ios = tbl.Get<int>("buy_ios");
            ndiamond = tbl.Get<int>("diamond");
            amount = nbuy_android;
            itemName = ndiamond.ToString() + uLocalization.Get("钻石");
        }
        else if(monthcard.TryGetValue(configID,out tbl))
        {
            int card_price = tbl.Get<int>("card_price");
            int card_type = tbl.Get<int>("card_type");
            amount = card_price;
            itemName = uLocalization.Get("月卡")+ card_type.ToString();
        }
        else
        {
            Debug.LogError("payerror:"+configID.ToString());
            return;
        }

        System.Collections.Generic.Dictionary<string, string> data = new System.Collections.Generic.Dictionary<string, string>();
        data["dataType"] = "6";
        data["roleID"] = PlayerData.Instance.RoleID.ToString();
        data["roleName"] = PlayerData.Instance.Name;
        data["roleLevel"] = PlayerData.Instance.BaseAttr.Level.ToString();
        data["zoneId"] = Account.Instance.ServerId.ToString();
        data["zoneName"] = Account.Instance.ServerName;
        data["itemName"] = itemName;
        data["count"] = "1";
        data["amount"] = amount.ToString();
        data["itemId"] = configID.ToString();
        data["callBackInfo"] = PlayerData.Instance.RoleID.ToString()+";"+ configID.ToString()+";"+"0";

        this.SetExtData(data);
    }

    public void Exit()
    {
        SGSDK.instance.exit();
    }
    /// <summary>
    /// data 参数
    /// dataType -类型 type类型（1-启动，2-退出，3-角色升级，4-创建角色） -5上报数据 6 定额充值 7非定额充值
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
        SGSDK.instance.SetUseData(JsonMapper.ToJson(data));
    }
    public void ReleaseResource()
    {
    }
    public string GetExtName(ThirdParty_ExtNameType type)
    {
        if(type == ThirdParty_ExtNameType.ACTION_ENTER_SERVER)
        {
            return sdkLoginData;
        }
        return "";
    }
    public void EnterUserCenter()
    {
        SGSDK.instance.userCenter();
    }
    public bool IsLogin()
    {
        return SGSDK.isLogin;
    }
    #endregion

    #region callback methods
    void initCallback(string result)
    {
        Debug.Log("initCallback:" + result); 
        string resultCode = result;
        if (resultCode == SGSDK.LOGIN_SUCCESS)
        {
            SG.CoreEntry.gEventMgr.TriggerEvent(SG.GameEvent.GE_THIRDPARTY_INIT, null);
        }
        else
        {
            SG.CoreEntry.gEventMgr.TriggerEvent(SG.GameEvent.GE_THIRDPARTY_INIT, EventParameter.Get(0));
        }
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["dataType"] = "1";     
        this.SetExtData(data);

    }

    void loginCallback(string result)
    {
        Debug.Log("loginCallback:" + result);
        string resultCode = result.Substring(0, 1);
        string data = result.Substring(1);
        Debug.Log("data:" + data);
        Debug.Log("resultCode:" + resultCode);
        if (resultCode == SGSDK.LOGIN_SUCCESS)
        {
            sdkLoginData = data;
            SGSDK.isLogin = true;
            SGSDK.instance.showFloatMenu();
        }

    }

    void logoutCallback(string result)
    {
        Debug.Log("logoutCallback:" + result);
        string resultCode = result.Substring(0, 1);
        if (resultCode == SGSDK.LOGIN_SUCCESS)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_THIRDPARTY_LOGOUT, null);
            SGSDK.isLogin = false;
        }
    }
    void payCallBackMethod(string result)
    {
        Debug.Log("payCallBackMethod:" + result);
    }
    void exitCallback(string result)
    {
        Debug.Log("exitCallback:" + result);
        SGSDK.isLogin = false;
        int exitCode = 1;
        int.TryParse(result, out exitCode);
        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_THIRDPARTY_EXIT, EventParameter.Get(exitCode));
        
    }
    public string appversion()
    {
        return "";
    }

    #endregion
}

