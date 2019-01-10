using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Hotfix]
public class DARole
{
    public string roleId
    {
        set;
        get;
    }

    public string roleName
    {
        set;
        get;
    }

    public string roleCareer
    {
        set;
        get;
    }

    public string roleRace
    {
        set;
        get;
    }

    public int roleLevel
    {
        set;
        get;
    }

    public string gameServer
    {
        set;
        get;
    }

    public DARole(string roleId, string gameServer)
    {
        this.roleId = roleId;
        this.gameServer = gameServer;
    }
}

public enum DAAccountType
{
    DA_Anonymous = 0, //未知用户来源
    DA_Registered, //游戏自身注册用户
    DA_SianWeibo, //新浪微博用户
    DA_QQ, //QQ用户
    DA_TencentWeibo, //腾讯微博用户
    DA_ND91, //91用户
	DA_Type1,
	DA_Type2,
	DA_Type3,
	DA_Type4,
	DA_Type5,
	DA_Type6,
	DA_Type7,
	DA_Type8,
	DA_Type9,
	DA_Type10
};

public enum DAGender
{
	DA_UNKNOWN = 0,
	DA_MALE,   //man
	DA_FEMALE //woman
};

public enum DATaskType
{
    DA_GuideLine = 1, //新手任务
    DA_MainLine, //主线任务
    DA_BranchLine, //分支任务
    DA_Daily, //日常任务
    DA_Activity, //活动任务
    DA_Other //其他任务,默认值
};

public interface IEventLogger
{
    void Init( string channelId);
    void Quit();

    void CreateAccount(string accountId, string accountType, DAGender gender, int age, string serverId, string roleName);

    void Login(string accountId, DAGender genders, string age, string serverId, int level, string rolename);
    void Logout();

    void SetAccountType(DAAccountType type);
    void SetGender(DAGender gender);
    void SetAge(int age);
    void SetGameServer(string gameServer);
    void SetLevel(int level);
    void AddTag(string tag, string subTag);
    void RemoveTag(string tag, string subTag);


    void Level_Begin(string levelId);
    void Level_Complete(string levelId);
    void Level_Fail(string levelId, string reason);
    //void Level_Complete(string levelId, string levelType, long spendTime, bool result, bool isFirst, string failReason);


    void Task_Begin(string taskId, DATaskType type);
    void Task_Complete(string taskId, DATaskType type);
    void Task_Fail(string taskId, string reason, DATaskType type);
    //void Task_Complete(string taskId, string taskType, long spendTime, bool result, bool isFirst, string failReason);


    void Item_Buy(string itemId, string itemType, int itemCnt, int vituralCurrency, string currencyType, string consumePoint);
    void Item_Get(string itemId, string itemType, int itemCount, string reason);
    //void Item_Get(string itemId, string itemType, int itemCount, string reason, string levelId);
    void Item_Use(string itemId, string itemType, int itemCount, string reason);
    //void Item_Use(string itemId, string itemType, int itemCount, string reason, string levelId);

    void Coin_SetCoinNum(long coinNum, string coinType);
    void Coin_Use(string reason, string coinType, long lost, long left);
    void Coin_Use(string reason, string coinType, long lost, long left, string levelId);
    void Coin_Get(string reason, string coinType, long gain, long left, float itemTotalPrice);
    void Coin_Get(string reason, string coinType, long gain, long left, float itemTotalPrice, string levelId);

    void VirtualCurrency_PaymentStart(string orderId, string iapId, int iapAmount, double currencyAmount, string currencyType, string paymentType);
    void VirtualCurrency_PaymentSuccess(string orderId, string iapId,int iapAmount, double currencyAmount, string currencyType, string paymentType, string levelId);
    
    void Item_BuyInLevel(string itemId, string itemType, int itemCnt,int vituralCurrency,string currencyType,string consumePoint, string levelId);
    void Item_GetInLevel(string itemId, string itemType, int itemCnt,string reason, string levelId);
    void Item_UseInLevel(string itemId, string itemType, int itemCnt, string reason, string levelId);

    void OnEvent(string eventId);
    void OnEvent(string eventId, string label);
    void OnEvent(string eventId, Dictionary<string,string> map);
    void OnEventCount(string eventId, int count);
    void OnEventDuration(string eventId,long duration);
    void OnEventDuration(string eventId, string label, long duration);
    void OnEventDuration(string eventId, Dictionary<string, string> map, long duration);
    void OnEventBegin(string eventId);
    void OnEventBegin(string eventId, Dictionary<string, string> map);
    void OnEventBegin(string eventId, Dictionary<string, string> map, string flag);
    void OnEventEnd(string eventId);
    void OnEventEnd(string eventId, string flag);
    void OnEventBeforeLogin(string eventId,Dictionary<string, string> dic,long duration);
   

    void ConfigParams_SetUpdateCallBack(string obj, string functionName);
    void ConfigParams_Update();
    void ConfigParams_GetParameterString(string key, string defaultValue);
    void ConfigParams_GetParameterInt(string key, int defaultValue);
    void ConfigParams_GetParameterLong(string key, long defaultValue);
    void ConfigParams_GetParameterBoolean(string key, bool defaultValue);

    void ReportError(string title, string error);
    void Agent_SetVersion(string version);
    void Agent_SetUploadInterval(uint second);
    void Agent_UploadNow();
    void Agent_GetUID();
    void Agent_OpenAdTracking();

    void Tracking_SetEffectPoint(string pointId, Dictionary<string, string> dictionary);

    //void Role_Create(string accountId, DARole role);
    //void Role_Enable(DARole role);
    //void Role_Disable();
    //void Role_Delete(string accountId, string roleId);
    //void Role_LevelUp(string roleId, int prevLevel, int nowLevel, long spendTime);
    
}

