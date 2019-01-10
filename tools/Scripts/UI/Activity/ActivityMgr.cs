using UnityEngine;
using System.Collections;
using XLua;
using System;
using System.Collections.Generic;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class ActivityMgr
    {
        private static ActivityMgr instance = null;
        public static ActivityMgr Instance
        {
            get
            {
                if (null == instance)
                    instance = new ActivityMgr();

                return instance;
            }
        }

        /// <summary>
        /// 当前活动地图的id  .  进入活动地图后，curActiveId 会赋值，。 退出 后CurActiveId =0
        /// </summary>
        public int CurActiveId = 0;

        /// <summary>
        /// 世界地图boss玩家伤害值(前5名数据)  key - 排名
        /// </summary>
        public Dictionary<int, MsgData_sWorldBossHurtList> worldBossHurts = new Dictionary<int, MsgData_sWorldBossHurtList>();

        /// <summary>
        /// 活动配置
        /// </summary>
        public static Dictionary<int, LuaTable> CacheExtConfig = null;


        /// <summary>
        /// 活动状态列表 --前端读配置表
        /// </summary>
        public Dictionary<int, MsgData_sActivityStateList> list = new Dictionary<int, MsgData_sActivityStateList>();

        /// <summary>
        /// 活动状态列表 --前端读配置表  限时
        /// </summary>
        public Dictionary<int, MsgData_sActivityStateList> list1 = new Dictionary<int, MsgData_sActivityStateList>();

        /// <summary>
        /// 我的活动列表????
        /// </summary>
        public Dictionary<int, MsgData_sActivityList> list2 = new Dictionary<int, MsgData_sActivityList>();

        /// <summary>
        /// 世界BOSS列表
        /// </summary>
        public Dictionary<int, MsgData_sWorldBossList> worldBossList = new Dictionary<int, MsgData_sWorldBossList>();

        /// <summary>
        /// 探宝列表(世界BOSS探宝功能)
        /// </summary>
        public Dictionary<int, MsgData_sBossWabaoList> wabaoList = new Dictionary<int, MsgData_sBossWabaoList>();

        public MsgData_sBossWabaoList GetCurWabaoMsg(int bossId)
        {
            if(wabaoList.ContainsKey(bossId))
            {
                return wabaoList[bossId];
            }
            return null;
        }
        /// <summary>
        /// 获取当前活动的server数据
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public MsgData_sActivityStateList GetCurActivyMsg(int activityId)
        {
            if (list.ContainsKey(activityId))
            {
                return list[activityId];
            }
            return null;
        }
        /// <summary>
        /// 获取当前活动地图的activityId
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public int GetCurMapActivyId(int mapId)
        {
            foreach (var item in list)
            {
                if(item.Value.mapId == mapId)
                {
                    return item.Key;
                }
            }
            return 0;
        }

        public ActivityMgr()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            CacheExtConfig = G.Get<Dictionary<int, LuaTable>>("t_activity");
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_WC_ActivityState, OnActivityState);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_WC_WorldBoss, OnWorldBoss);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_ActivityEnter, OnActivityEnter);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_ActivityQuit, OnActivityQuit);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_Activity, OnActivity);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_ActivityFinish, OnActivityFinish);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_WorldBossDamage, OnWorldBossDamage);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_WorldBossHurt, OnWorldBossHurt);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_WaBaoList, OnWaBaoList);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_GetWaBaoReward, OnGetWaBaoReward);
            //GetActivivyList();
        }

        public void Send_CW_WorldBoss()
        {
            MsgData_cWorldBoss rsp = new MsgData_cWorldBoss();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_WorldBoss, rsp);
        }
        public void Send_CS_ActivityEnter(int activityId,int param =0)
        {
            MsgData_cActivityEnter rsp = new MsgData_cActivityEnter();
            rsp.id = activityId;
            rsp.param1 = param;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ActivityEnter, rsp);
        }

        public void Send_CS_ActivityQuit(int activityId)
        {
            MsgData_cActivityQuit rsp = new MsgData_cActivityQuit();
            rsp.id = activityId;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ActivityQuit, rsp);
        }
        /// <summary>
        /// 请求挖宝列表
        /// </summary>
        public void Send_CS_GetWaBaoList()
        {
            MsgData_cWaBaoList rsp = new MsgData_cWaBaoList();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_WaBaoList, rsp);
        }

        /// <summary>
        /// 请求挖宝领取BOSS奖励 , type - 领取类型 0:1次 , 1:多次
        /// </summary>
        public void Send_CS_GetWaBaoReward(int bossId,int type = 0)
        {
            MsgData_cGetWaBaoReward rsp = new MsgData_cGetWaBaoReward();
            rsp.bossId = bossId;
            rsp.index = type;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GetWaBaoReward,rsp);
        }

        public void OnWorldBoss(GameEvent ge, EventParameter parameter)
        {
            MsgData_sWorldBoss resp = parameter.msgParameter as MsgData_sWorldBoss;
            for (int i = 0; i < resp.count; i++)
            {
                MsgData_sWorldBossList tt = resp.items[i];
                worldBossList[tt.id] = tt;
                if (bShow(tt.id))
                {
                    MsgData_sActivityStateList t = new MsgData_sActivityStateList();
                    t.id = tt.id;
                    t.state = 1;
                    list[tt.id] = t;
                }
            }
            EventParameter par = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_WorldBossUpdate, par);
        }

        public void OnActivity(GameEvent ge, EventParameter parameter)
        {
            MsgData_sActivity resp = parameter.msgParameter as MsgData_sActivity;
            for (int i = 0; i < resp.count; i++)
            {
                MsgData_sActivityList tt = resp.list[i];

                list2[tt.id] = tt;
            }
         //   LogMgr.LogError("receive Activity.count  " + resp.count);
        }
        /// <summary>
        /// 读 activity表  字段 show ,是否显示在UI列表中
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool bShow(int id)  
        {
            LuaTable l;
            if (CacheExtConfig.TryGetValue(id, out l))
            {
                bool bShow = l.Get<bool>("show");
                return bShow;
            }
            return false;
        }

        public void OnActivityState(GameEvent ge, EventParameter parameter)
        {
            MsgData_sActivityState resp = parameter.msgParameter as MsgData_sActivityState;

            for (int i = 0; i < resp.count; i++)
            {
                MsgData_sActivityStateList tt = resp.items[i];
                if(bShow(tt.id))
                {
                    list[tt.id] = tt;
                    if (tt.state == 1 )
                    {
                        list1[tt.id] = tt;
                    }
                    else if   (tt.state == 0 && tt.forecast == 1)
                    {
                        list1[tt.id] = tt;
                    }
                    //         LogMgr.LogError("receive ActivityState.state  " + tt.state);
                }
            }
     //       LogMgr.LogError("receive ActivityState.count  " + list.Count);
            EventParameter par = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_ActivityUpdate, par);
        }
        public void OnActivityEnter(GameEvent ge, EventParameter parameter)
        {
            LogMgr.Log("receive Activity enter");
            MsgData_sActivityEnter resp = parameter.msgParameter as MsgData_sActivityEnter;
            if(resp.result == 0)
            {
                CurActiveId = resp.id;
            }
        }
        public void OnActivityQuit(GameEvent ge, EventParameter parameter)
        {
            LogMgr.Log("receive ActivityQuit");
            MsgData_sActivityQuit resp = parameter.msgParameter as MsgData_sActivityQuit;
            if (resp.result == 0)
            {
                CurActiveId = 0;   // 退出活动，id 清0
            }
        }
        public void OnActivityFinish(GameEvent ge, EventParameter parameter)
        {
            LogMgr.LogError("receive ActivityFinish");

        }
        /// <summary>
        /// 玩家累计伤害
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnWorldBossDamage(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sWorldBossDamage resp = parameter.msgParameter as MsgData_sWorldBossDamage;
        }
        /// <summary>
        /// 更新世界BOSS剩余血量,玩家击杀伤害值及排名
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnWorldBossHurt(GameEvent ge, EventParameter parameter)
        {
            MsgData_sWorldBossHurt resp = parameter.msgParameter as MsgData_sWorldBossHurt;
            for (int i = resp.hurtList.Length; i > 0; i--)
            {
                for (int j = 0; j < i - 1; j++)
                {
                    if (resp.hurtList[j].hurt < resp.hurtList[j+1].hurt)
                    {
                        MsgData_sWorldBossHurtList  tt= resp.hurtList[j];
                        resp.hurtList[j] = resp.hurtList[j + 1];
                        resp.hurtList[j + 1] = tt;
                    }
                }
            }
            worldBossHurts.Clear();
            int index = 1;
            for (int i = 0; i < resp.hurtList.Length; i++)
            {
                if(resp.hurtList[i].hurt > 0)  
                {
                    worldBossHurts[index] = resp.hurtList[i];
                    index += 1;
                }
              //  worldBossHurts[i + 1] = resp.hurtList[i];
            }

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_WorldBossDamageUpdate, EventParameter.Get(parameter.msgParameter));
        }
        public void OnWaBaoList(GameEvent ge, EventParameter parameter)
        {
            MsgData_sWaBaoList resp = parameter.msgParameter as MsgData_sWaBaoList;
            for (int i = 0; i < resp.count; i++)
            {
                MsgData_sBossWabaoList tt = resp.list[i];
                wabaoList[tt.bossId] = tt;
            }
          //  LogMgr.LogError("receive WaBaoList.count  " +resp.count);
        }
        public void OnGetWaBaoReward(GameEvent ge, EventParameter parameter)
        {
            LogMgr.LogError("receive GetWaBaoReward");
            MsgData_sGetWaBaoReward resp = parameter.msgParameter as MsgData_sGetWaBaoReward;
            if(resp.result == 0)
            {
                MsgData_sBossWabaoList tt = new MsgData_sBossWabaoList();
                tt.bossId = resp.bossId;
                tt.num = resp.num;
                wabaoList[tt.bossId] = tt;
            }
            else
            {
                LogMgr.LogError("探宝失败");
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_WorldBossGetReward, parameter);
        }
    }
}

