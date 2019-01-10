/**
* @file     : MagicKeyDataMgr.cs
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
    // S->C 副本信息
    [LuaCallCSharp]
    [Hotfix]

    // S->C BOSS信息
    public class PersonalBossVO
    {
        public int id; // BOSSID
        public int num; // 已进入次数
        public int isfirst; // 是否已每日首通 0已首通
        public int remainTime; // 大于0剩余秒数 等于0到期 -1错误
        public int passFlag; // 0 未通关，1已通关
        public int leftNum; //剩余次数
        public int m_nBuyNum;//购买次数
        public long m_i8NextTimeStamp; //下次进入时间戳
        public DateTime date = DateTime.Now;
        public int GetRemainTime()
        {
            TimeSpan  span = DateTime.Now - date;
            int time = remainTime - (int)span.TotalSeconds;
            if (time < 0)
                time = 0;
            return time;
        }
    }

    [LuaCallCSharp]
    [Hotfix]
    public class BossDropItem
    {
        public int ItemID;
        public int Count;
    }

    [LuaCallCSharp]
    [Hotfix]
    public class BossFuBenDataMgr
    {
        //private static BossFuBenDataMgr _instance = null;
        public static BossFuBenDataMgr Instance
        {
            get
            {
                return PlayerData.Instance.BossFuBenDataMgr;
            }
        }

        Dictionary<int, PersonalBossVO> mStage = new Dictionary<int, PersonalBossVO>();
        public Dictionary<int, PersonalBossVO> Stage
        {
            get { return mStage; }
            set { mStage = value; }
        }

        public int EnterId = 0;
         
#region msg
        public void ResisterMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_PersonalBossList, OnPersonalBossList);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BackEnterResultPersonalBoss, OnBackEnterResultPersonalBoss);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BackQuitPersonalBoss, OnBackQuitPersonalBoss);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_PersonalBossResult, OnPersonalBossResult);
            //CoreEntry.gEventMgr.AddListener(GameEvent.GE_Drop_Item, OnDropItem);

        }

        public void OnPersonalBossList(GameEvent ge,EventParameter param)// 服务器返回BOSS挑战列表 msgId:8560;
        {
            MsgData_sPersonalBossList data = param.msgParameter as MsgData_sPersonalBossList;

            for (int i = 0; i < data.items.Count; i++)
            {
                UpdateStagevo(data.items[i]);
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_PersonalBossList,EventParameter.Get());
        }

        public List<BossDropItem> DropItem = new List<BossDropItem>();

        //1001,1#1002,2
        public string DropItemToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < DropItem.Count; i++)
            {
                sb.Append(DropItem[i].ItemID);
                sb.Append(",");
                sb.Append(DropItem[i].Count);
                if (i != DropItem.Count - 1)
                    sb.Append("#");
            }
            LogMgr.UnityLog(sb.ToString());
            return sb.ToString();
        }
        public void OnBackEnterResultPersonalBoss(GameEvent ge, EventParameter param)// 服务器返回进入个人BOSS结果 msgId:8561;
        {
            MsgData_sBackEnterResultPersonalBoss data = param.msgParameter as MsgData_sBackEnterResultPersonalBoss;
            if(data.result == 0)
                EnterId = data.id;
            DropItem.Clear();
        }
        public void OnBackQuitPersonalBoss(GameEvent ge, EventParameter param)// 服务器:退出个人BOSS结果 msgId:8562; 
        {
            //MsgData_sBackQuitPersonalBoss data = param.msgParameter as MsgData_sBackQuitPersonalBoss;
            EnterId = 0;
        }
        public void OnPersonalBossResult(GameEvent ge, EventParameter param)// 服务器:挑战个人BOSS结果 msgId:8563;
        {
            //MsgData_sPersonalBossResult data = param.msgParameter as MsgData_sPersonalBossResult;
        }
        public void OnDropItem(GameEvent ge, EventParameter param)
        {
            if (EnterId == 0)return;

            BossDropItem dropItem = DropItem.Find(s => s.ItemID == param.intParameter);
            if (dropItem == null)
            {
                dropItem = new BossDropItem();
                dropItem.ItemID = param.intParameter;
                dropItem.Count = param.intParameter1;
                DropItem.Add(dropItem); 
            }
            else
            {
                dropItem.Count = dropItem.Count + param.intParameter1;
            }
        }
        
        
#endregion

        //副本信息
        public void UpdateStagevo(MsgData_sPersonalBossVO item)
        {
            PersonalBossVO v = new PersonalBossVO();
            v.num = item.num;
            v.id = item.id;
            v.isfirst = item.isfirst;
            v.remainTime = item.remainTime;
            v.passFlag = item.passFlag;
            v.leftNum = item.leftNum;
            v.m_nBuyNum = item.m_nBuyNum;
            v.date = DateTime.Now;
            v.m_i8NextTimeStamp = item.m_i8NextTimeStamp;
            if (mStage.ContainsKey(v.id))
            {
                mStage[v.id] = v;
            }
            else
            {
                mStage.Add(v.id, v);
            }
        }

        public PersonalBossVO GetStagevo(int id)
        {
            PersonalBossVO v = null;
            if (mStage.ContainsKey(id))
            {
                v = mStage[id];
            }
            else
            {
                v = new PersonalBossVO();
                v.num = 0;
                v.id = id;
                v.isfirst = 0;
                v.remainTime = 0;
                v.passFlag = 0;
                v.leftNum = -1;
                v.m_nBuyNum = 0;
                v.date = DateTime.Now;
                v.m_i8NextTimeStamp = 0;
            }
            return v;

        } 

        //重置信息
        public void Reset()
        {
            mStage.Clear();
        }
    }
}