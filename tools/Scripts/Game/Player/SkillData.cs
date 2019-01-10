/**
* @file     : SkillData.cs
* @brief    : 玩家技能数据。
* @details  : 
* @author   : XuXiang
* @date     : 2017-07-11 14:18
*/

using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;
using System;

namespace SG
{ 
    /// <summary>
    /// 技能数据。
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public class SkillData
    {
        /// <summary>
        /// 战斗力。
        /// </summary>
        private long mPower;

        /// <summary>
        /// 获取战斗力。
        /// </summary>
        public long Power
        {
            get { return mPower; }
            set { mPower = value; }
        }

        /// <summary>
        /// 技能集合。
        /// </summary>
        public Dictionary<int, int> mSkills = new Dictionary<int, int>();
        public Dictionary<int, int> Skills
        {
            get { return mSkills; }
            set { mSkills = value; }
        }
     
        /// <summary>
        /// 获取技能编号。
        /// </summary>
        /// <param name="gid">技能组编号。</param>
        /// <returns>技能编号。</returns>
        public int GetSkill(int gid)
        {
            int id;
            mSkills.TryGetValue(gid, out id);
            return id;
        }

        /// <summary>
        /// 获取普攻列表。
        /// </summary>
        /// <returns>普攻列表。(已按释放顺序进行排序)</returns>
        public List<int> GetNormalAttack()
        {
            int prof = PlayerData.Instance.BaseAttr.Prof;
            return ConfigManager.Instance.Skill.GetNormalAttack(prof);
        }

        /// <summary>
        /// 获取技能列表。
        /// </summary>
        /// <returns>技能列表。(下标对应技能位，为0表示未解锁)</returns>
        public List<int> GetSkillList()
        {
            int prof = PlayerData.Instance.BaseAttr.Prof;
            List<int> glist = ConfigManager.Instance.Skill.GetSkillList(prof);
            List<int> list = new List<int>();
            for (int i=0; i<glist.Count; ++i)
            {
                list.Add(GetSkill(glist[i]));
            }
            return list;
        }

        /// <summary>
        /// 注册网络消息。
        /// </summary>
        public void RegisterNetMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, OnCleanupUserData);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SKILL_LIST_RESULT, OnSkillList);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SKILL_LEARN_RESULT, OnSkillLearn);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SKILL_LEVEL_UP_RESULT, OnSkillLevelUp);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SKILL_ADD, OnSkillAdd);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SKILL_REMOVE, OnSkillRemove);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SKILL_CDLIST, OnSkillCDList);
        }

        /// <summary>
        ///  切换账号清除数据
        /// </summary>
        private void OnCleanupUserData(GameEvent ge, EventParameter parameter)
        {
            mPower = 0;
            mSkills.Clear();
        }

        /// <summary>
        /// 触发技能信息更新事件。
        /// </summary>
        public static void TriggerEventSkillInfo()
        {
            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SKILL_LIST, ep);
        }

        /// <summary>
        /// 触发技能学习事件。
        /// </summary>
        /// <param name="id">技能编号。</param>
        public static void TriggerEventSkillLean(int re, int id)
        {
            EventParameter ep = EventParameter.Get();
            ep.intParameter = re;
            ep.intParameter1 = id;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SKILL_LEARN, ep);
        }

        /// <summary>
        /// 触发技能升级事件。 
        /// </summary>
        /// <param name="oid">原技能编号。</param>
        /// <param name="nid">新技能编号。</param>
        public static void TriggerEventSkillLevelUp(int re, int oid, int nid)
        {
            EventParameter ep = EventParameter.Get();
            ep.intParameter = re;
            ep.intParameter1 = oid;
            ep.intParameter2 = nid;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SKILL_LEVEL_UP, ep);
        }

        /// <summary>
        /// 触发技能添加事件。
        /// </summary>
        /// <param name="id">技能编号。</param>
        public static void TriggerEventSkillAdd(int id)
        {
            EventParameter ep = EventParameter.Get();
            ep.intParameter = id;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SKILL_ADD, ep);
        }

        /// <summary>
        /// 触发技能移除事件。
        /// </summary>
        /// <param name="id">技能编号。</param>
        public static void TriggerEventSkillRemove(int id)
        {
            EventParameter ep = EventParameter.Get();
            ep.intParameter = id;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SKILL_REMOVE, ep);
        }

        /// <summary>
        /// 技能列表刷新。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnSkillList(MsgData data)
        {
            MsgData_sSkillListResult info = data as MsgData_sSkillListResult;
            mSkills.Clear();
            for (int i=0; i<info.Skills.Count; ++i)
            {
                int id = info.Skills[i].ID;
                int gid = ConfigManager.Instance.Skill.GetSkillGroup(id);
                if (!mSkills.ContainsKey(gid))
                {
                    mSkills.Add(gid, id);
                }
                else
                {
                    LogMgr.ErrorLog("技能组重复 id:{0} gid:{1}", id, gid);
                }
            }
            TriggerEventSkillInfo();
        }

        /// <summary>
        /// 技能学习。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnSkillLearn(MsgData data)
        {
            MsgData_sSkillLearnResult info = data as MsgData_sSkillLearnResult;
            if (info.Result == 0)
            {
                int id = info.ID;
                int gid = ConfigManager.Instance.Skill.GetSkillGroup(id);
                if (!mSkills.ContainsKey(gid))
                {
                    mSkills.Add(gid, id);
                }
                else
                {
                    LogMgr.ErrorLog("技能组重复 id:{0} gid:{1}", id, gid);
                }
            }            
            TriggerEventSkillLean(info.Result, info.ID);
        }

        /// <summary>
        /// 技能升级。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnSkillLevelUp(MsgData data)
        {
            MsgData_sSkillLevelUp info = data as MsgData_sSkillLevelUp;
            if (info.Result == 0)
            {
                int oldid = info.OldID;
                int oldgid = ConfigManager.Instance.Skill.GetSkillGroup(oldid);
                int newid = info.NewID;
                int newgid = ConfigManager.Instance.Skill.GetSkillGroup(newid);
                mSkills.Remove(oldgid);
                if (!mSkills.ContainsKey(newgid))
                {
                    mSkills.Add(newgid, newid);
                }
                else
                {
                    LogMgr.ErrorLog("技能组重复 id:{0} gid:{1}", newid, newgid);
                }
            }
                
            TriggerEventSkillLevelUp(info.Result, info.OldID, info.NewID);
        }

        /// <summary>
        /// 技能添加。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnSkillAdd(MsgData data)
        {
            MsgData_sSkillAdd info = data as MsgData_sSkillAdd;
            int id = info.ID;
            int gid = ConfigManager.Instance.Skill.GetSkillGroup(id);
            if (!mSkills.ContainsKey(gid))
            {
                mSkills.Add(gid, id);
            }
            else
            {
                LogMgr.ErrorLog("技能组重复 id:{0} gid:{1}", id, gid);
            }
            TriggerEventSkillAdd(id);
        }

        /// <summary>
        /// 技能移除。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnSkillRemove(MsgData data)
        {
            MsgData_sSkillRemove info = data as MsgData_sSkillRemove;
            int id = info.ID;
            int gid = ConfigManager.Instance.Skill.GetSkillGroup(id);
            mSkills.Remove(gid);
            TriggerEventSkillRemove(id);
        } 


        public void OnSkillCDList(GameEvent ge, EventParameter param)
        {
            if(ModuleServer.MS.GSkillCastMgr == null)
                ModuleServer.MS.Initialize();
            ModuleServer.MS.GSkillCastMgr.OnSkillCDList(ge, param);
        }

        /// <summary>
        /// 发送学习技能请求。
        /// </summary>
        /// <param name="id">技能编号。</param>
        public void SendLearnRequest(int id)
        {
            MsgData_cSkillLearn data = new MsgData_cSkillLearn();
            data.ID = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_SKILL_LEARN, data);
        }

        /// <summary>
        /// 发送升级技能请求。
        /// </summary>
        /// <param name="id">技能编号。</param>
        public void SendLevelUpRequest(int id)
        {
            MsgData_cSkillLevelUp data = new MsgData_cSkillLevelUp();
            data.ID = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_SKILL_LEVEL_UP, data);
        }

        /// <summary>
        /// 发送一键升级技能请求。
        /// </summary>
        /// <param name="id">技能编号。</param>
        public void SendFastLevelUpRequest(int id)
        {
            MsgData_cSkillFastLevelUp data = new MsgData_cSkillFastLevelUp();
            data.ID = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_SKILL_FAST_LEVEL_UP, data);
        }
    }
}
