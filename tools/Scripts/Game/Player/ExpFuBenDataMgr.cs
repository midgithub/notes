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

using System.Text;

namespace SG
{
    [LuaCallCSharp]
    [Hotfix]
    public class WaterDungeonInfo 
    {
        public int wave; // 我的最佳波数
        public double exp; // 我的最高经验
        public sbyte time; // 已用次数
        public int monster; // 我的最多杀怪
        public int buyCount; //已购买次数
        public double moreExp; // 今天可额外领取的经验
        public sbyte moreReward; // 0不可以额外领取,1可以额外领取

        private  int mRightTime =0;
        private float Elapsed = 0;
        public int rightTime // 任务剩余时间（秒）
        {
            get {
                return Math.Max(0,(mRightTime - Mathf.FloorToInt(Time.time - Elapsed)));
            }
            set {
                Elapsed = Time.time;
                mRightTime = value;
            }
        }

    }

    [LuaCallCSharp]
    [Hotfix]
    public class ExpFuBenDataMgr
    {

        public WaterDungeonInfo mInfo = new WaterDungeonInfo();
        public WaterDungeonInfo Info
        {
            get { return mInfo; }
            set { mInfo = value; }

        }
        //private static ExpFuBenDataMgr _instance = null;
        public static ExpFuBenDataMgr Instance
        {
            get
            {
                return PlayerData.Instance.ExpFuBenDataMgr;
            }
        }


        public int wave = 1; // 当前波数
        public int monster = 0; // 当前波杀怪数
        public double exp = 0; // 累计获得经验

#region msg
        public void ResisterMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_WaterDungeonEnterResult, OnWaterDungeonEnterResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_WaterDungeonExitResult, OnWaterDungeonExitResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_WaterDungeonInfo, OnWaterDungeonInfo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_WaterDungeonProgress, OnWaterDungeonProgress);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_WaterDungeonResult, OnWaterDungeonResult);
   
        }
        public void OnWaterDungeonEnterResult(GameEvent ge, EventParameter param) 
        {
            MsgData_sWaterDungeonEnterResult data = param.msgParameter as MsgData_sWaterDungeonEnterResult;
            mInfo.rightTime = data.rightTime;
            wave = 1; // 当前波数
            monster = 0; // 当前波杀怪数
            exp = 0; // 累计获得经验
        }
        public void OnWaterDungeonExitResult(GameEvent ge, EventParameter param)
        {
            //MsgData_sWaterDungeonExitResult data = param.msgParameter as MsgData_sWaterDungeonExitResult;
        }
        public void OnWaterDungeonInfo(GameEvent ge, EventParameter param)
        {
            MsgData_sWaterDungeonInfo data = param.msgParameter as MsgData_sWaterDungeonInfo;
            mInfo.wave = data.wave;
            mInfo.exp = data.exp;
            mInfo.time = data.time;
            mInfo.buyCount = data.buyCount;
            mInfo.monster = data.monster;
            mInfo.moreExp = data.moreExp;
            mInfo.moreReward = data.moreReward; 
        }
        public void OnWaterDungeonProgress(GameEvent ge, EventParameter param)
        {
            MsgData_sWaterDungeonProgress data = param.msgParameter as MsgData_sWaterDungeonProgress;
            wave = data.wave; // 当前波数
            monster = data.monster; // 当前波杀怪数
            exp = data.exp; // 累计获得经验
        }
        public void OnWaterDungeonResult(GameEvent ge, EventParameter param)
        {
            //MsgData_sWaterDungeonResult data = param.msgParameter as MsgData_sWaterDungeonResult;
        }
#endregion

    }
}