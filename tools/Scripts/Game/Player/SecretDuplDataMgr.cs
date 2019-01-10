/**
* @file     :  
* @brief    : 
* @details  :  
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;
using System;


namespace SG
{

    [LuaCallCSharp]
    [Hotfix]
    public class SecretDuplDataMgr
    {
        //private static SecretDuplDataMgr _instance = null;
        public static SecretDuplDataMgr Instance
        {
            get
            {
                return PlayerData.Instance.SecretDuplDataMgr;
            }
        }



        public int tili; // 组队剩余进入次数
        public int counts; // 剩余进入次数
        public int vlaTag; // 个人已购买次数
        public int vlaTagTeam; // 组队已购买次数
        public int simpleMaxLayerCount; // 单人秘境最高层数
        public int second; //  剩余秒数


        public DateTime date = DateTime.Now;
        public int GetRemainTime()
        {
            TimeSpan span = DateTime.Now - date;
            int time = second - (int)span.TotalSeconds;
            if (time < 0)
                time = 0;
            return time;
        }

        public int EnterLayer;
        public int EnterID = 0;
        public long EnterTime = 0;
        public int num; // 已击杀数量
        public int value; // 当前产生的值
        public int id; // 区域ID

        public int EnterTeamID;         //进入的组队副本ID

        public bool bSend = true;
        #region msg
        public void ResisterMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_ResSimpleSecrectDuplInfo, OnResSimpleSecrectDuplInfo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SecretDungeonSweep, onSecretDungeonSweep);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SimpleSecrectDuplTrace, onSimpleSecrectDuplTrace);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_ResEnterSimpleSecrectDupl, onResEnterSimpleSecrectDupl);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_UpdateSecrectDuplTili, onUpdateSecrectDuplTili);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SecretDungeonSweepInfo, onSecretDungeonSweepInfo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, onClearData);
        }

        public void onClearData(GameEvent ge, EventParameter param)
        {
            Release();
        }
        public void Release()
        {
            bSend = true;
        }

        public void SendInitDataInfo()
        {
            if (bSend)
            {
                bSend = false;
                SG.NetLogicGame.Instance.SendReqSimpleSecrectDuplInfo();
            }
        }


        public void OnResSimpleSecrectDuplInfo(GameEvent ge, EventParameter param)
        {
            MsgData_sResSimpleSecrectDuplInfo data = param.msgParameter as MsgData_sResSimpleSecrectDuplInfo;
            tili = data.tili; // 组队剩余进入次数
            counts = data.counts; // 剩余进入次数
            vlaTag = data.vlaTag; // 个人已购买次数
            vlaTagTeam = data.vlaTagTeam; // 组队已购买次数
            simpleMaxLayerCount = data.simpleMaxLayerCount; // 单人秘境最高层数
        }
        public void onSecretDungeonSweep(GameEvent ge, EventParameter param)
        {
            MsgData_sSecretDungeonSweep data = param.msgParameter as MsgData_sSecretDungeonSweep;
            if (data.result != 0) return;
            date = DateTime.Now;
            second = data.second;
            EnterLayer = simpleMaxLayerCount;
        }

        public void onSimpleSecrectDuplTrace(GameEvent ge, EventParameter param)
        {
            MsgData_sSimpleSecrectDuplTrace data = param.msgParameter as MsgData_sSimpleSecrectDuplTrace;
            num = data.num; // 已击杀数量
            value = data.value; // 当前产生的值
            id = data.id; // 区域ID
        }

        public void onResEnterSimpleSecrectDupl(GameEvent ge, EventParameter param)
        {
            MsgData_sResEnterSimpleSecrectDupl data = param.msgParameter as MsgData_sResEnterSimpleSecrectDupl;
            EnterID = data.id;
            EnterTime = UiUtil.GetNowTimeStamp();
            num = 0;
            value = 0;
            id = 0;
            EnterLayer = simpleMaxLayerCount;
        }

        public void onUpdateSecrectDuplTili(GameEvent ge, EventParameter param)
        {
            SG.NetLogicGame.Instance.SendReqSimpleSecrectDuplInfo();
        }
        public void onSecretDungeonSweepInfo(GameEvent ge, EventParameter param)
        {
            MsgData_sSecretDungeonSweepInfo data = param.msgParameter as MsgData_sSecretDungeonSweepInfo;
            second = data.second;
            date = DateTime.Now;
            EnterLayer = data.id;
        }

        #endregion
    }
}