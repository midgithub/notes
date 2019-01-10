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

using System.Xml;
namespace SG
{ 

    // S->C boss状态列表
    [LuaCallCSharp]
    [Hotfix]
    public class TreasurebossInfo
    {
        public int bossState; // 0、已刷新 1、未刷新
        private int mremainTimer;
        public int remainTimer // 未刷新需要剩余时间
        {
            get { return mremainTimer; }
            set { mremainTimer = value; date = DateTime.Now; }
        }
        public int id; // BossID

        public DateTime date = DateTime.Now;

        public int GetRemainTime()
        {
            TimeSpan span = DateTime.Now - date;
            int time = remainTimer - (int)span.TotalSeconds;
            if (time < 0)
                time = 0;
            return time;
        }

    }
    
    [LuaCallCSharp]
    [Hotfix]
    public class TreasureFuBenDataMgr
    {
        //private static TreasureFuBenDataMgr _instance = null;
        public static TreasureFuBenDataMgr Instance
        {
            get
            {
                return PlayerData.Instance.TreasureFuBenDataMgr;
            }
        }

        public int EnterId = 0;
        public int addedTimer = 0;
        public int remainTime = 0;
        public DateTime  date = DateTime.Now;

        public int GetRemainTime()
        {
            //TimeSpan span = DateTime.Now - date;
            int time = remainTime;// -(int)span.TotalSeconds;
            if (time < 0)
                time = 0;
            return time;
        }


        public TreasurebossInfo BossInfo = new TreasurebossInfo();

#region msg
        public void ResisterMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EnterTreasureDupl, onEnterTreasureDupl);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_TreasureTodayAddedTimer, onTreasureTodayAddedTimer);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_TreasureRemainTime, onTreasureRemainTime);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_TreasureUpdateBoss, onTreasureUpdateBoss);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_QueryMonsterPostion_Start, onQueryMonsterPostion_Start);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_QueryMonsterPostion_End, onQueryMonsterPostion_End);

        }
        public void onQueryMonsterPostion_Start(GameEvent ge, EventParameter param)
        {
        }
        public void onQueryMonsterPostion_End(GameEvent ge, EventParameter param)
        {
            //EventParameter p = param;
        }
        public void onEnterTreasureDupl(GameEvent ge, EventParameter param)
        {
            MsgData_sEnterTreasureDupl data = param.msgParameter as MsgData_sEnterTreasureDupl;
            EnterId = data.id; 
        }
        public void onTreasureTodayAddedTimer(GameEvent ge, EventParameter param)
        {
            MsgData_sTreasureTodayAddedTimer data = param.msgParameter as MsgData_sTreasureTodayAddedTimer;
            addedTimer = data.addedTimer;
        }
        public void onTreasureRemainTime(GameEvent ge, EventParameter param)
        {
            MsgData_sTreasureRemainTime data = param.msgParameter as MsgData_sTreasureRemainTime;
            remainTime = data.remainTime;
        }
        public void onTreasureUpdateBoss(GameEvent ge, EventParameter param)
        {
            MsgData_sTreasureUpdateBoss data = param.msgParameter as MsgData_sTreasureUpdateBoss;
            if (data.count > 0)
            {
                BossInfo.bossState = data.items[0].bossState;
                BossInfo.id = data.items[0].id;
                BossInfo.remainTimer = data.items[0].remainTimer;
            }
        }

      
#endregion 

        public Dictionary<int, Vector3> monsterPostion = new Dictionary<int, Vector3>();
        public void MoveToMonster(int monsterID)
        {
            Vector3 pos = Vector3.zero;
            if (monsterPostion.ContainsKey(monsterID))
            {
                pos = monsterPostion[monsterID];
                //AutoAIRunner.StopAll();
                TaskMgr.Instance.MoveToPos(pos);
                return;
            }
            
            string filePath = Application.dataPath + "/ResData/Data/MapData/" + @"" + MapMgr.Instance.EnterMapId + ".xml";

            //---------
            //XmlData xmldata = new XmlData();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            if (xmlDoc == null)
            {
                return;
            }
            XmlNodeList monstersList = xmlDoc.SelectSingleNode("root").ChildNodes;
            if (monstersList == null || monstersList.Count == 0)
            {
                return;
            }
            List<string> enumStrList = new List<string>();
            enumStrList.Add(MonsterType.monster.ToString());
            bool bFindMonster = false;
            foreach (XmlNode item in monstersList)
            {
                XmlElement xe = (XmlElement)item;
                XmlNodeList subList = xe.ChildNodes;
                if ("First".Equals(item.Name))
                {
                    foreach (XmlElement tt in subList)
                    {
                        if (enumStrList.Contains(tt.Name))
                        {
                            foreach (XmlElement subitem in tt)
                            {
                                if (subitem != null)
                                {
                                    int id = Convert.ToInt32(subitem.GetAttribute("id"));
                                    if (id == monsterID)
                                    {
                                        bFindMonster = true;
                                        pos = new Vector3(float.Parse(subitem.GetAttribute("x")), float.Parse(subitem.GetAttribute("y")), float.Parse(subitem.GetAttribute("z")));
                                    }
                                }
                            }                            
                        }
                    }
                }
            }
            //---------
            if (bFindMonster)
            {
                monsterPostion.Add(monsterID, pos);
                //AutoAIRunner.StopAll();
                TaskMgr.Instance.MoveToPos(pos);
            }
            else
            {
                LogMgr.UnityLog(string.Format("没有找到怪物id坐标: {0}",monsterID));
            }
    
        }


    }
}