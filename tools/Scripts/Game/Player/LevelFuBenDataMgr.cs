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
    // S->C 副本信息
     [LuaCallCSharp]
    [Hotfix]
    public class Stagevo  
    {
        public int Num; // 剩余挑战次数
        public int ID; // ID
        public int State; // 0 没扫荡 1在扫荡
        public long TimeNum; // 扫荡剩余时间0 待领取倒计时
        public int MaxNum; // 总次数
        public int Evaluate; // 评价1-3星
        public int RewardType; // 0:未开启1：可领取2：已领取
    }
    [LuaCallCSharp]
    [Hotfix]
    public class LevelFuBenDataMgr
    { 
        //private static LevelFuBenDataMgr _instance = null;
        public static LevelFuBenDataMgr Instance
        {
            get
            {
                return PlayerData.Instance.LevelFuBenDataMgr;
            }
        }

        Dictionary<int, Stagevo> mStage = new Dictionary<int, Stagevo>();
        public Dictionary<int, Stagevo> Stage
        {
            get { return mStage; }
            set { mStage = value; }
        }

        public int EnterId = 0;
        public int enterNum = 0;//剩余总次数
         
#region msg
        public void ResisterMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_DominateRouteData, OnDominateRouteData);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_DominateRouteUpDate, OnDominateRouteUpDate);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_BackDominateRouteChallenge, OnBackDominateRouteChallenge);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_BackDominateRouteQuit, OnBackDominateRouteQuit);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_BackDominateRouteInfo, OnBackDominateRouteInfo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_BackDominateRouteWipe, OnBackDominateRouteWipe);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_BackDominateRouteVigor, OnBackDominateRouteVigor);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_BackDominateRouteBoxReward, OnBackDominateRouteBoxReward);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_BackDominateRouteEnd, OnBackDominateRouteEnd); 
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_BackDominateRouteMopupEnd, OnBackDominateRouteMopupEnd); 
        }

        public void OnDominateRouteData(GameEvent ge, EventParameter param)
        {
            MsgData_sDominateRouteData data = param.msgParameter as MsgData_sDominateRouteData;
            enterNum = data.enterNum;
            for (int i = 0; i < data.items.Count; i++)
            {
                UpdateStagevo(data.items[i]);
            }
        }
        public void OnDominateRouteUpDate(GameEvent ge, EventParameter param)
        {
            MsgData_sDominateRouteUpDate data = param.msgParameter as MsgData_sDominateRouteUpDate;
            Stagevo v = GetStagevo(data.id);
            if (v != null)
            {
                v.Num = data.num;
                v.State = data.state;
                v.TimeNum = data.time;
                v.RewardType = data.rewardType;
                //计算总次数
                int total = 0;
                foreach (KeyValuePair<int,Stagevo> item in mStage)
                {
                    total = total + item.Value.Num;
                }
                enterNum = total;
            }
              
        }
        public void OnBackDominateRouteChallenge(GameEvent ge, EventParameter param)
        {
            MsgData_sBackDominateRouteChallenge data = param.msgParameter as MsgData_sBackDominateRouteChallenge;
            EnterId = data.id;

        }
        public void OnBackDominateRouteQuit(GameEvent ge, EventParameter param)
        {
        }
        public void OnBackDominateRouteInfo(GameEvent ge, EventParameter param)
        {
            //MsgData_sBackDominateRouteInfo data = param.msgParameter as MsgData_sBackDominateRouteInfo;
        }
        public void OnBackDominateRouteWipe(GameEvent ge, EventParameter param)
        {
        }
        public void OnBackDominateRouteVigor(GameEvent ge, EventParameter param)
        {
        }
        public void OnBackDominateRouteBoxReward(GameEvent ge, EventParameter param)
        {
        }
        public void OnBackDominateRouteEnd(GameEvent ge, EventParameter param)
        {
            MsgData_sBackDominateRouteEnd data = param.msgParameter as MsgData_sBackDominateRouteEnd;
            Stagevo stage = GetStagevo(EnterId);
            if (stage != null)
            {
                stage.Evaluate = data.level;
            }
        } 
        public void OnBackDominateRouteMopupEnd(GameEvent ge, EventParameter param)
        {
            //MsgData_sBackDominateRouteMopupEnd data = param.msgParameter as MsgData_sBackDominateRouteMopupEnd;
         
        }
        
        
#endregion

        //副本信息
        public void UpdateStagevo(MsgData_sStagevo item)
        {
            Stagevo v = new Stagevo();
            v.Num = item.Num;
            v.ID = item.ID;
            v.State = item.State;
            v.TimeNum = item.TimeNum;
            v.MaxNum = item.MaxNum;
            v.Evaluate = item.Evaluate;
            v.RewardType = item.RewardType; 
            if (mStage.ContainsKey(v.ID))
            {
                mStage[v.ID] = v;
            }
            else
            {
                mStage.Add(v.ID, v);
            }
        }

        public Stagevo GetStagevo(int id)
        {
            Stagevo v = null;
            if (mStage.ContainsKey(id))
            {
                v = mStage[id];
            }
            else
            {
                v = new Stagevo();
                v.Num = 0;
                v.ID = id;
                v.State = 0;
                v.TimeNum = 0;
                v.MaxNum = 0;
                v.Evaluate = 0;
                v.RewardType = 0;
                mStage[id] = v;
            }
            return v;

        } 
    }
}