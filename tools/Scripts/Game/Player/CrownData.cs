/**
* @file     : CrownData
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-08-28
*/
using UnityEngine;
using System;
using System.Collections.Generic;
using XLua;

namespace SG
{
    [LuaCallCSharp]
    public enum CROWNSTATE
    {
        CLOSE = 1,//未开启
        FIGHT = 2,//挑战
        GAINED = 3,//已获得
        PROCESS =4,//获取进行中
    }

    [LuaCallCSharp]
    [Hotfix]
    public class CrownData
    {
        private Dictionary<int, int> mCrowns = new Dictionary<int,int>();
        /// <summary>
        /// 王冠数据
        /// </summary>
        public Dictionary<int, int> Crowns
        {
            get{ return mCrowns; }
        }

        /// <summary>
        /// 注册网络消息事件
        /// </summary>
        public void RegisterNetMsg()
        {
            CurTabIndex = 1;

            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CROWNINFO, OnCrownInfo);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CROWNSKILLUP, OnCrownSkillUp);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CROWNACTIVE, OnCrownActive);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CROWNRESULT, OnCrownResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_EnterDungeonResult, OnEnterFuben);
        }

        private void OnCrownInfo(MsgData msg)
        {
            MsgData_sResCrownInfo data = msg as MsgData_sResCrownInfo;
            mCrowns.Clear();
            for(int i = 0;i < data.List.Count;i++)
            {
                mCrowns[data.List[i].ID] = data.List[i].SkillID;
            }

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_CROWNINFO, null);
        }

        private void OnCrownSkillUp(MsgData msg)
        {
            MsgData_sResCrownSkillUp data = msg as MsgData_sResCrownSkillUp;
            if(0 == data.Result)
            {
                mCrowns[data.ID] = data.SkillID;
            }

            EventParameter param = EventParameter.Get();
            param.intParameter = data.Result;
            param.intParameter1 = data.ID;
            param.intParameter2 = data.SkillID;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_CROWNSKILLUP, param);
        }

        private void OnCrownActive(MsgData msg)
        {
            MsgData_sResCrownActive data = msg as MsgData_sResCrownActive;

            mCrowns[data.ID] = data.SkillID;

            EventParameter param = EventParameter.Get();
            param.intParameter = data.ID;
            param.intParameter1 = data.SkillID;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_CROWNACTIVE, param);

            if (data.SkillID != 0)
            {
                CurShowCrown = data.ID;
                //MainPanelMgr.Instance.ShowDialog("UICrownGain");
            }
        }

        private void OnCrownResult(MsgData msg)
        {
            MsgData_sResCrownFightResult data = msg as MsgData_sResCrownFightResult;

            EventParameter param = EventParameter.Get();
            param.intParameter = data.Result;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_CROWNRESULT, param);
        }

        private void OnEnterFuben(GameEvent ge, EventParameter parameter)
        {
            MsgData_sEnterDungeonResult resp = parameter.msgParameter as MsgData_sEnterDungeonResult;
            if (resp.result == 0)
            {
                LuaTable mapConfig = ConfigManager.Instance.Map.GetMapConfig(resp.id);
                if (mapConfig == null)
                {
                    return;
                }
                int mapType = mapConfig.Get<int>("type");
                if (mapType == 43)
                {
                    mEnterTime = UiUtil.GetNowTimeStamp();
                }
            }
        }

        /// <summary>
        /// 请求王冠数据
        /// </summary>
        public void ReqCrownInfo()
        {
            MsgData_cReqCrownInfo msg = new MsgData_cReqCrownInfo();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_CROWNINFO, msg);
        }

        /// <summary>
        /// 请求王冠技能
        /// </summary>
        /// <param name="id"></param>
        /// <param name="skillID"></param>
        public void ReqwCrownSkillUp(int id, int skillID)
        {
            MsgData_cReqCrownSkillUp msg = new MsgData_cReqCrownSkillUp();
            msg.ID = id;
            msg.SkillID = skillID;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_SKILLUP, msg);
        }

        public void ReqCrownFightEnter(int id)
        {
            MsgData_cReqCrownFight msg = new MsgData_cReqCrownFight();
            msg.id = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_CROWNFIGHT, msg);
        }

        public void ReqCrownFightLeave()
        {
            MsgData_cReqCrownFightLeave msg = new MsgData_cReqCrownFightLeave();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_CROWNFIGHTLEAVE, msg);
        }

        /// <summary>
        /// 获取王冠状态
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public CROWNSTATE GetCrwonState(int id)
        {
            LuaTable t = ConfigManager.Instance.Common.GetCrownConfig(id);
            if(t == null)
            {
                return CROWNSTATE.CLOSE;
            }

            if(mCrowns.ContainsKey(id))
            {
                if (mCrowns[id] == 0)
                {
                    return CROWNSTATE.FIGHT;
                }

                return CROWNSTATE.GAINED;
            }

            int preID = t.Get<int>("FrontID");
            if(preID != 0)
            {
                if (!mCrowns.ContainsKey(preID))
                {
                    return CROWNSTATE.CLOSE;
                }
            }

            return CROWNSTATE.PROCESS;
        }

        /// <summary>
        /// 当前王冠ID
        /// </summary>
        public int CurShowCrown
        {
            set;
            get;
        }

        /// <summary>
        /// 更加王冠ID获取对应技能
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetCrownSkill(int id)
        {
            if (mCrowns.ContainsKey(id))
            {
                return mCrowns[id];
            }

            return 0;
        }

        /// <summary>
        /// 当前选择王冠标签页
        /// </summary>
        public int CurTabIndex
        {
            set;
            get;
        }

        public int CurCrownMapID
        {
            get
            {
                int curMapID = MapMgr.Instance.EnterMapId;
                LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
                Dictionary<int, LuaTable> cfg = G.Get<Dictionary<int, LuaTable>>("t_crownboss");
                if (null == cfg)
                {
                    return 0 ;
                }

                foreach (KeyValuePair<int, LuaTable> data in cfg)
                {
                    int mapID = data.Value.Get<int>("mapId");
                    if(mapID == curMapID)
                    {
                        return data.Key;
                    }
                }

                return 0;
            }
        }

        private long mEnterTime = 0;
        public long EnterFightTime
        {
            get
            {
                return mEnterTime;
            }
        }
    }
}