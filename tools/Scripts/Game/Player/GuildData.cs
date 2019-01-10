/**
* @file     : GuildData.cs
* @brief    : 公会数据
* @details  : 
* @author   : XuXiang
* @date     : 2017-09-27 12:01
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;

namespace SG
{
    /// <summary>
    /// 队伍数据。
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public class GuildData
    {
        /// <summary>
        /// 公会数据。
        /// </summary>
        private MsgData_sQueryMyGuildInfo mGuildInfo = null;

        /// <summary>
        /// 获取公会基本数据。
        /// </summary>
        public MsgData_sQueryMyGuildInfo GuildInfo
        {
            get { return mGuildInfo; }
        }

        /// <summary>
        /// 获取资源数量。
        /// </summary>
        /// <param name="id">资源编号。</param>
        /// <returns>资源数量。</returns>
        public int GetResNumber(int id)
        {
            if (mGuildInfo == null)
            {
                return 0;
            }

            for (int i = 0; i < mGuildInfo.GuildResList.Length; ++i)
            {
                if (mGuildInfo.GuildResList[i].ItemId == id)
                {
                    return mGuildInfo.GuildResList[i].Count;
                }
            }
            return 0;
        }

        /// <summary>
        /// 获取所在公会的ID，0表示无公会。
        /// </summary>
        public long GuildID
        {
            get { return mGuildInfo != null ? mGuildInfo.GuidId : 0; }
        }

        /// <summary>
        /// 获取公会技能。
        /// </summary>
        /// <param name="gid">技能组编号</param>
        /// <returns>技能编号。</returns>
        public int GetGuildSkill(int gid)
        {
            if (mGuildInfo == null || gid <= 0 || gid > mGuildInfo.GuildSkillList.Length)
            {
                return 0;
            }

            return mGuildInfo.GuildSkillList[gid - 1].SkillId;
        }

        /// <summary>
        /// 公会名称。
        /// </summary>
        private string mGuildName = string.Empty;

        /// <summary>
        /// 获取帮主名称。
        /// </summary>
        public string GuildName
        {
            get { return mGuildName; }
        }

        /// <summary>
        /// 获取帮主ID。
        /// </summary>
        public long MasterID
        {
            get
            {
                MsgData_sRespGuildMemsVo vo = MasterInfo;
                return vo == null ? 0 : vo.Gid;
            }
        }

        /// <summary>
        /// 获取帮主信息。
        /// </summary>
        public MsgData_sRespGuildMemsVo MasterInfo
        {
            get
            {
                for (int i = 0; i < mMemberList.Count; ++i)
                {
                    if (mMemberList[i].Pos == GuildPos.GUILD_POS_MASTER)
                    {
                        return mMemberList[i];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 获取帮主名称。
        /// </summary>
        public string MasterName
        {
            get
            {
                for (int i = 0; i < mMemberList.Count; ++i)
                {
                    if (mMemberList[i].Pos == GuildPos.GUILD_POS_MASTER)
                    {
                        int server;
                        string name = PlayerData.GetPlayerName(UiUtil.GetNetString(mMemberList[i].Name), out server);
                        return name;
                    }
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 公会公告。
        /// </summary>
        private string mNotice = string.Empty;

        /// <summary>
        /// 获取公会公告。
        /// </summary>
        public string Notice
        {
            get { return mNotice; }
        }

        /// <summary>
        /// 成员列表。
        /// </summary>
        private List<MsgData_sRespGuildMemsVo> mMemberList = new List<MsgData_sRespGuildMemsVo>();

        /// <summary>
        /// 获取成员列表。
        /// </summary>
        public List<MsgData_sRespGuildMemsVo> MemberList
        {
            get { return mMemberList; }
        }
        /// <summary>
        /// 公会总人数
        /// </summary>
        /// <returns></returns>
        public int GetGuildAllMembersCount()
        {
            return mMemberList.Count;
        }
        //公会总在线人数
        public int GetGuildMemberOnlines()
        {
            int online = 0;
            for (int i = 0; i < mMemberList.Count; i++)
            {
                if (mMemberList[i].Online == 1)
                {
                    online += 1;
                }
            }
            return online;
        }


        /// <summary>
        /// 判断某个玩家是否为公会成员。
        /// </summary>
        /// <param name="id">玩家编号。</param>
        /// <returns>是否为公会成员。</returns>
        public bool IsInGuild(long id)
        {
            if (GuildID == 0)
            {
                return false;
            }

            for (int i = 0; i < mMemberList.Count; ++i)
            {
                if (mMemberList[i].Gid == id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 成员数据更新的时间。
        /// </summary>
        private float mUpdateMemberTime = 0;

        /// <summary>
        /// 获取成员数据更新的时间。
        /// </summary>
        public float UpdateMemberTime
        {
            get { return mUpdateMemberTime; }
        }

        /// <summary>
        /// 对成员列表进行排序比较。
        /// </summary>
        public static int CompareMember(MsgData_sRespGuildMemsVo a, MsgData_sRespGuildMemsVo b)
        {
            if (a.Online != b.Online)
            {
                return a.Online == 1 ? -1 : 1;      //在线的排前面
            }

            if (a.Pos != b.Pos)
            {
                return b.Pos - a.Pos;               //职位大的排前面
            }

            return a.Level - b.Level;               //最后按等级
        }

        /// <summary>
        /// 获取公会成员信息。
        /// </summary>
        /// <param name="id">公会编号。</param>
        /// <returns>公会成员。</returns>
        public MsgData_sRespGuildMemsVo GetMemberInfo(long id)
        {
            for (int i = 0; i < mMemberList.Count; ++i)
            {
                if (mMemberList[i].Gid == id)
                {
                    return mMemberList[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 设置帮众职位。
        /// </summary>
        /// <param name="id">帮众ID。</param>
        /// <param name="pos">职位。</param>
        public void SetMemberPos(long id, sbyte pos)
        {
            for (int i = 0; i < mMemberList.Count; ++i)
            {
                if (mMemberList[i].Gid == id)
                {
                    mMemberList[i].Pos = pos;
                    break;
                }
            }

            //判断是否自己职位被改
            if (PlayerData.Instance.RoleID == id)
            {
                mGuildInfo.Pos = pos;
            }
        }

        /// <summary>
        /// 事件列表。
        /// </summary>
        private List<MsgData_sRespGuildEventVo> mEventList = new List<MsgData_sRespGuildEventVo>();

        /// <summary>
        /// 获取事件列表。
        /// </summary>
        public List<MsgData_sRespGuildEventVo> EventList
        {
            get { return mEventList; }
        }

        /// <summary>
        /// 申请列表。
        /// </summary>
        private List<MsgData_sRespGuildApplysVo> mApplysList = new List<MsgData_sRespGuildApplysVo>();

        /// <summary>
        /// 获取入帮申请列表。
        /// </summary>
        public List<MsgData_sRespGuildApplysVo> ApplysList
        {
            get { return mApplysList; }
        }

        /// <summary>
        /// 移除申请信息。
        /// </summary>
        /// <param name="id">角色编号。</param>
        public void RemoveApplyInfo(long id)
        {
            for (int i = 0; i < mApplysList.Count; ++i)
            {
                if (mApplysList[i].Gid == id)
                {
                    mApplysList.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// 公会加持信息。
        /// </summary>
        private MsgData_sBackAidInfo mAidInfo = null;

        /// <summary>
        /// 获取公会加持信息。
        /// </summary>
        public MsgData_sBackAidInfo AidInfo
        {
            get { return mAidInfo; }
        }

        /// <summary>
        /// 获取临时洗练属性。
        /// </summary>
        public MsgData_sBackAidBapInfo TempAidAttr
        {
            get { return mTempAidAttr; }
        }

        /// <summary>
        /// 临时洗练属性。
        /// </summary>
        private MsgData_sBackAidBapInfo mTempAidAttr = null;

        /// <summary>
        /// 三种祈福状态。
        /// </summary>
        private int[] mPrayState = new int[3];

        /// <summary>
        /// 获取祈福状态。
        /// </summary>
        public int[] PrayState
        {
            get { return mPrayState; }
        }

        /// <summary>
        /// 祈福信息。
        /// </summary>
        private List<MsgData_sPrayVo> mPrayInfos = new List<MsgData_sPrayVo>();

        /// <summary>
        /// 获取祈福信息。
        /// </summary>
        public List<MsgData_sPrayVo> PrayInfos
        {
            get { return mPrayInfos; }
        }

        /// <summary>
        /// 同盟加入申请列表。
        /// </summary>
        private List<MsgData_sRespGuildAllianceApplysVo> mAllianceApplysList = new List<MsgData_sRespGuildAllianceApplysVo>();

        /// <summary>
        /// 获取同盟申请列表。
        /// </summary>
        public List<MsgData_sRespGuildAllianceApplysVo> AllianceApplysList
        {
            get { return mAllianceApplysList; }
        }

        /// <summary>
        /// 同盟信息。
        /// </summary>
        private MsgData_sQueryAllianceGuildInfo mAllianceInfo = null;

        /// <summary>
        /// 获取同盟信息。
        /// </summary>
        private MsgData_sQueryAllianceGuildInfo AllianceInfo
        {
            get { return mAllianceInfo; }
        }

        /// <summary>
        /// 移除申请信息。
        /// </summary>
        /// <param name="id">公会编号。</param>
        public void RemoveAllianceApplyInfo(long id)
        {
            for (int i = 0; i < mAllianceApplysList.Count; ++i)
            {
                if (mAllianceApplysList[i].Id == id)
                {
                    mAllianceApplysList.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// 清除数据。
        /// </summary>
        public void ClearData()
        {
            mGuildInfo = null;
            mGuildName = string.Empty;
            mNotice = string.Empty;
            mMemberList.Clear();
            mEventList.Clear();
            mApplysList.Clear();
            mAllianceApplysList.Clear();
            mAllianceInfo = null;
            mPrayState[0] = 0;
            mPrayState[1] = 0;
            mPrayState[2] = 0;
            mTempAidAttr = null;
        }

        /// <summary>
        /// 注册网络消息。
        /// </summary>
        public void RegisterNetMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, OnCleanupUserData);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_LIST, OnGuildList);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_INFO, OnGuildInfo);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_MEMBER_LIST, OnMemberList);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_EVENT_LIST, OnEventList);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_CREATE, OnGuildCreate);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_UPDATE, OnGuildUpdate);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_UPDATE_MASTER, OnUpdateMaster);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_UPDATE_NOTICE, OnUpdateNotice);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_BE_INVITED, OnBeInvited);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_OTHER_INFO, OnOtherInfo);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_APPLY_LIST, OnApplyList);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_APPLY, OnApplyGuild);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_APPLY_NUM, OnApplyNumber);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_VERIFY_APPLY, OnVerifyApply);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_QUIT, OnQuitGuild);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_DISMISS, OnDismissGuild);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_LEVEL_UP, OnLevelUpGuild);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_LEVEL_UP_SKILL, OnLevelUpGuildSkill);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_CHANGE_POS, OnChangePos);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_KICK_MEMBER, OnKickMember);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_CHANGE_LEADER, OnChangeLeader);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_CONTRIBUTE, OnContribute);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_LEVEL_UP_MY_SKILL, OnLevelUpMyGuildSkill);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_AID_INFO, OnAidInfo);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_AID_BAP, OnAidBap);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_AID_LEVEL_UP, OnAidLevelUp);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_UPDATE_MY_INFO, OnUpDateMyInfo);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_PRAY_INFO, OnPrayInfo);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_PRAY, OnPray);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_CREATE_ALLIANCE, OnCreateAlliance);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_DISMISS_ALLIANCE, OnDismissAlliance);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_ALLIANCE_APPLY_LIST, OnAllianceApplyList);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_ALLIANCE_INFO, OnAllianceInfo);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_ALLIANCE_VERIFY_APPLY, OnAllianceVerifyApply);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_MILITARY, OnMilitary);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GUILD_REWARD, OnReward);
        }

        /// <summary>
        ///  切换账号清除数据
        /// </summary>
        private void OnCleanupUserData(GameEvent ge, EventParameter parameter)
        {
            ClearData();
        }

        /// <summary>
        /// 公会列表。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnGuildList(MsgData data)
        {
            MsgData_sGuildList info = data as MsgData_sGuildList;

            EventParameter ep = EventParameter.Get();
            ep.longParameter = info.RecomendGuid;
            ep.intParameter = info.Page;
            ep.objParameter = info.GuildAllianceApplyList;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_LIST, ep);
        }

        /// <summary>
        /// 公会信息。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnGuildInfo(MsgData data)
        {
            MsgData_sQueryMyGuildInfo info = data as MsgData_sQueryMyGuildInfo;
            int server;
            mGuildInfo = info;
            //Debug.LogError(info.RewardTime);
            //Debug.LogError(info.AverageLevel);
            mGuildName = PlayerData.GetPlayerName(UiUtil.GetNetString(info.GuildName), out server);
            mNotice = UiUtil.GetNetString(info.GuildNotice);

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_INFO, ep);
            if (GuildID != 0)
            {
                SendQueryGuildMemberRequest();
                SendAidInfoRequest();
                SendPrayInfoRequest();
            }
        }

        /// <summary>
        /// 公会成员列表。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnMemberList(MsgData data)
        {
            MsgData_sQueryMyGuildMems info = data as MsgData_sQueryMyGuildMems;
            mMemberList.Clear();
            mMemberList.AddRange(info.GuildMemList);
            mMemberList.Sort(CompareMember);
            mUpdateMemberTime = Time.realtimeSinceStartup;

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_MEMBER_LIST, ep);
        }

        /// <summary>
        /// 公会事件列表。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnEventList(MsgData data)
        {
            MsgData_sQueryMyGuildEvent info = data as MsgData_sQueryMyGuildEvent;
            mEventList.Clear();
            mEventList.AddRange(info.GuildEventList);

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_EVENT_LIST, ep);
        }

        /// <summary>
        /// 公会创建返回。(创建成功后需要自己主动请求公会数据SendQueryGuildInfoRequest(0))
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnGuildCreate(MsgData data)
        {
            MsgData_sCreateGuildRet info = data as MsgData_sCreateGuildRet;
            if (info.Result == 0)
            {
                SendQueryGuildInfoRequest(0);
            }

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_CREATE, ep);
        }

        /// <summary>
        /// 公会属性更新。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnGuildUpdate(MsgData data)
        {
            MsgData_sUpdateGuildInfo info = data as MsgData_sUpdateGuildInfo;
            if (mGuildInfo == null)
            {
                return;
            }
            mGuildInfo.GuidId = info.GuildId;
            mGuildInfo.AllianceGuildId = info.AllianceGuildId;
            mGuildInfo.Rank = info.Rank;
            mGuildInfo.Level = info.Level;
            mGuildInfo.MemCnt = info.MemCnt;
            mGuildInfo.ExtendNum = info.ExtendNum;
            mGuildInfo.Captial = info.Captial;
            mGuildInfo.Liveness = info.Liveness;
            mGuildInfo.Power = info.Power;
            mGuildInfo.GuildResList = info.GuildResList;

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_UPDATE, ep);
        }

        /// <summary>
        /// 公会更改帮主。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnUpdateMaster(MsgData data)
        {
            MsgData_sUpdateGuildMasterName info = data as MsgData_sUpdateGuildMasterName;
            int server;
            string mastername = PlayerData.GetPlayerName(UiUtil.GetNetString(info.GuildMasterName), out server);

            EventParameter ep = EventParameter.Get();
            ep.stringParameter = mastername;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_UPDATE_MASTER, ep);
        }

        /// <summary>
        /// 公会更新公告。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnUpdateNotice(MsgData data)
        {
            MsgData_sUpdateGuildNotice info = data as MsgData_sUpdateGuildNotice;
            mNotice = UiUtil.GetNetString(info.GuildNotice);

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_UPDATE_NOTICE, ep);
        }

        /// <summary>
        /// 通知入帮邀请。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnBeInvited(MsgData data)
        {
            MsgData_sNotifyBeInvitedGuild info = data as MsgData_sNotifyBeInvitedGuild;
            int server;
            string rolename = PlayerData.GetPlayerName(UiUtil.GetNetString(info.Name), out server);
            string guildname = PlayerData.GetPlayerName(UiUtil.GetNetString(info.GuildName), out server);

            EventParameter ep = EventParameter.Get();
            ep.longParameter = info.InviterId;
            ep.stringParameter = string.Format("{0},{1}", rolename, guildname);
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_BE_INVITED, ep);
        }

        /// <summary>
        /// 其它公会信息。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnOtherInfo(MsgData data)
        {
            MsgData_sQueryOtherGuildInfo info = data as MsgData_sQueryOtherGuildInfo;

            EventParameter ep = EventParameter.Get();
            ep.objParameter = info;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_OTHER_INFO, ep);
        }

        /// <summary>
        /// 入帮申请列表。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnApplyList(MsgData data)
        {
            MsgData_sQueryMyGuildApplys info = data as MsgData_sQueryMyGuildApplys;
            mApplysList.Clear();
            mApplysList.AddRange(info.GuildApplysList);
            mGuildInfo.ApplyNum = mApplysList.Count;

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_APPLY_LIST, ep);
        }

        /// <summary>
        /// 入帮申请回复。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnApplyGuild(MsgData data)
        {
            MsgData_sApplyGuild info = data as MsgData_sApplyGuild;

            EventParameter ep = EventParameter.Get();
            ep.longParameter = info.GuildId;
            ep.intParameter = info.Apply;
            ep.intParameter1 = info.ApplyFlag;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_APPLY, ep);
        }

        /// <summary>
        /// 申请人数变化。。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnApplyNumber(MsgData data)
        {
            MsgData_sGuildReplyCountTip info = data as MsgData_sGuildReplyCountTip;
            if (mGuildInfo != null)
            {
                mGuildInfo.ApplyNum = info.ReplyNum;
            }

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.ReplyNum;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_APPLY_NUMBER, ep);
        }

        /// <summary>
        /// 入帮申请审核结果。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnVerifyApply(MsgData data)
        {
            //从申请列表里移除这批人
            MsgData_sVerifyGuildApply info = data as MsgData_sVerifyGuildApply;
            bool needupdate = false;
            for (int i = 0; i < info.GuildApplyList.Count; ++i)
            {
                RemoveApplyInfo(info.GuildApplyList[i].MemGid);
                if (info.GuildApplyList[i].Result == 0)
                {
                    needupdate = true;
                }
            }
            mGuildInfo.ApplyNum = mApplysList.Count;

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Verify;
            ep.objParameter = info.GuildApplyList;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_VERIFY_APPLY, ep);
            if (info.Verify == 0 && needupdate)
            {
                SendQueryGuildMemberRequest();      //有批准成功的则刷新成员
            }
        }

        /// <summary>
        /// 退出公会。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnQuitGuild(MsgData data)
        {
            MsgData_sQuitGuild info = data as MsgData_sQuitGuild;
            if (info.Result == 0)
            {
                ClearData();
            }

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;      // 0-成功，-1失败
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_QUIT, ep);
        }

        /// <summary>
        /// 解散公会。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnDismissGuild(MsgData data)
        {
            MsgData_sDismissGuild info = data as MsgData_sDismissGuild;
            if (info.Result == 0)
            {
                ClearData();
            }

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;      // 0-成功，-1失败
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_DISMISS, ep);
        }

        /// <summary>
        /// 升级公会。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnLevelUpGuild(MsgData data)
        {
            MsgData_sLevelUpGuild info = data as MsgData_sLevelUpGuild;
            if (info.Result == 0 && mGuildInfo != null)
            {
                //mGuildInfo.Level = info.Level;            //服务器那边有毒，等级字段没赋值，但已经通过Update协议提前更新了，所以客户端也不会出问题
                SendQueryGuildInfoRequest();
            }

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_LEVEL_UP, ep);
        }

        /// <summary>
        /// 升级公会技能。(废弃)
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnLevelUpGuildSkill(MsgData data)
        {
            MsgData_sLvUpGuildSkill info = data as MsgData_sLvUpGuildSkill;

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            ep.intParameter1 = info.GroupId;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_LEVEL_UP_SKILL, ep);
        }

        /// <summary>
        /// 改变职位。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnChangePos(MsgData data)
        {
            MsgData_sChangeGuildPos info = data as MsgData_sChangeGuildPos;
            if (info.Result == 0)
            {
                SetMemberPos(info.MemGid, info.Pos);
            }

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            ep.intParameter1 = info.Pos;
            ep.longParameter = info.MemGid;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_CHANGE_POS, ep);
        }

        /// <summary>
        /// 踢出成员。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnKickMember(MsgData data)
        {
            MsgData_sKickGuildMemeber info = data as MsgData_sKickGuildMemeber;
            MsgData_sRespGuildMemsVo vo = null;
            if (info.Result == 0)
            {
                if (info.MemGid == PlayerData.Instance.RoleID)
                {
                    ClearData();
                }
                else
                {
                    for (int i = 0; i < mMemberList.Count; ++i)
                    {
                        if (mMemberList[i].Gid == info.MemGid)
                        {
                            vo = mMemberList[i];
                            mMemberList.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            ep.longParameter = info.MemGid;
            ep.objParameter = vo;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_KICK_MEMBER, ep);
        }

        /// <summary>
        /// 禅让帮主。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnChangeLeader(MsgData data)
        {
            MsgData_sChangeLeader info = data as MsgData_sChangeLeader;
            if (info.Result == 0)
            {
                SetMemberPos(info.OldId, info.Pos);
                SetMemberPos(info.NewId, (sbyte)GuildPos.GUILD_POS_MASTER);
            }

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_CHANGE_LEADER, ep);
        }

        /// <summary>
        /// 公会贡献。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnContribute(MsgData data)
        {
            MsgData_sGuildContribute info = data as MsgData_sGuildContribute;
            mGuildInfo.Captial = info.Captial;
            //mGuildInfo.TotalContribution += info.Contribute - mGuildInfo.Contribution;      //总帮贡也增加
            //mGuildInfo.Contribution = info.Contribute;
            int n = Math.Min(mGuildInfo.GuildResList.Length, info.GuildResList.Length);        //理论上者两个值应该都是3
            for (int i = 0; i < n; ++i)
            {
                mGuildInfo.GuildResList[i] = info.GuildResList[i];
            }

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_CONTRIBUTE, ep);
        }

        /// <summary>
        /// 升级玩家的公会技能。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnLevelUpMyGuildSkill(MsgData data)
        {
            MsgData_sLevelUpMyGuildSkill info = data as MsgData_sLevelUpMyGuildSkill;
            if (info.Result == 0)
            {
                mGuildInfo.GuildSkillList[info.Id - 1].SkillId = mGuildInfo.GuildSkillList[info.Id - 1].SkillId + 1;
            }

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            ep.intParameter1 = info.Id;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_LEVEL_UP_MY_SKIL, ep);
        }

        /// <summary>
        /// 洗练数据。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnAidInfo(MsgData data)
        {
            MsgData_sBackAidInfo info = data as MsgData_sBackAidInfo;
            mAidInfo = info;

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_AID_INFO, ep);
        }

        /// <summary>
        /// 洗练结果。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnAidBap(MsgData data)
        {
            MsgData_sBackAidBapInfo info = data as MsgData_sBackAidBapInfo;
            mTempAidAttr = info;

            EventParameter ep = EventParameter.Get();
            ep.objParameter = info;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_AID_BAP, ep);
        }

        /// <summary>
        /// 升级玩家的公会技能。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnAidLevelUp(MsgData data)
        {
            MsgData_sBackAidUpLevelInfo info = data as MsgData_sBackAidUpLevelInfo;
            if (info.Result == 0)
            {
                mAidInfo.AidLevel = mAidInfo.AidLevel + 1;
            }

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_AID_LEVEL_UP, ep);
        }

        /// <summary>
        /// 更新玩家的公会数据。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnUpDateMyInfo(MsgData data)
        {
            MsgData_sUpdateMyGuildMemInfo info = data as MsgData_sUpdateMyGuildMemInfo;
            if (mGuildInfo != null)
            {
                mGuildInfo.Pos = info.Pos;
                mGuildInfo.Contribution = info.Con;
                mGuildInfo.TotalContribution = info.TotalCon;
                mGuildInfo.Loyalty = info.Loyalty;
            }

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_UPDATE_MY_INFO, ep);
        }

        /// <summary>
        /// 升级玩家的公会技能。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnPrayInfo(MsgData data)
        {
            MsgData_sGetUnionPray info = data as MsgData_sGetUnionPray;
            mPrayState[0] = info.Pray1;
            mPrayState[1] = info.Pray2;
            mPrayState[2] = info.Pray3;
            mPrayInfos.Clear();
            mPrayInfos.AddRange(info.PrayList);

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_PRAY_INFO, ep);
        }

        /// <summary>
        /// 公会祈福结果。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnPray(MsgData data)
        {
            MsgData_sUnionPray info = data as MsgData_sUnionPray;
            if (info.Result == 0)
            {
                //int con = CachePrayConfig[info.PrayID].Get<int>("banggong");
                //mGuildInfo.TotalContribution += con;      //总帮贡也增加
                //mGuildInfo.Contribution += con;
                PrayState[info.PrayID - 1] = 1;

                //添加记录
                MsgData_sPrayVo vo = new MsgData_sPrayVo();
                NetLogicGame.str2Bytes(PlayerData.Instance.Name, vo.Name);
                vo.PrayID = info.PrayID;
                vo.Time = UiUtil.GetNowTimeStamp();
                mPrayInfos.Add(vo);
            }

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            ep.intParameter1 = info.PrayID;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_PRAY, ep);
        }

        /// <summary>
        /// 创建同盟。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnCreateAlliance(MsgData data)
        {
            MsgData_sCreateAlliance info = data as MsgData_sCreateAlliance;
            if (info.Result == 0)
            {
                mGuildInfo.AllianceGuildId = info.GuildId;
            }

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_CREATE_ALLIANCE, ep);
        }

        /// <summary>
        /// 解散同盟。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnDismissAlliance(MsgData data)
        {
            MsgData_sDismissGuildAlliance info = data as MsgData_sDismissGuildAlliance;
            mGuildInfo.AllianceGuildId = 0;
            LogMgr.Log("解散同盟 id:{0}", info.GuildId);

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_DISMISS_ALLIANCE, ep);
        }

        /// <summary>
        /// 同盟申请列表。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnAllianceApplyList(MsgData data)
        {
            MsgData_sQueryGuildAllianceApplys info = data as MsgData_sQueryGuildAllianceApplys;
            mAllianceApplysList.Clear();
            mAllianceApplysList.AddRange(info.GuildAllianceApplysList);

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_ALLIANCE_APPLY_LIST, ep);
        }

        /// <summary>
        /// 同盟信息。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnAllianceInfo(MsgData data)
        {
            MsgData_sQueryAllianceGuildInfo info = data as MsgData_sQueryAllianceGuildInfo;
            mAllianceInfo = info;

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_ALLIANCE_INFO, ep);
        }

        /// <summary>
        /// 同盟审核回复。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnAllianceVerifyApply(MsgData data)
        {
            //从申请列表里移除这批公会
            MsgData_sGuildAllianceVerify info = data as MsgData_sGuildAllianceVerify;
            for (int i = 0; i < info.GuildAllianceApplyList.Count; ++i)
            {
                RemoveAllianceApplyInfo(info.GuildAllianceApplyList[i].GuildId);
            }

            EventParameter ep = EventParameter.Get();
            ep.objParameter = info.GuildAllianceApplyList;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_ALLIANCE_VERIFY_APPLY, ep);
        }
        /// <summary>
        /// 同盟战力回复。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnMilitary(MsgData data)
        {
            //同盟战力
            MsgData_sGuildMilitary info = data as MsgData_sGuildMilitary;
            // Debug.LogError("/同盟战力=="+info.m_i8power);
            mGuildInfo.Power = info.m_i8power;
            EventParameter ep = EventParameter.Get();
            ep.objParameter = info.m_i8power;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_Military, ep);
        }

        /// <summary>
        /// 领取福利奖励
        /// </summary>
        /// <param name="data"></param>
        private void OnReward(MsgData data)
        {
            //Debug.LogError("=================OnReward==============================");
            MsgData_sGuildReward info = data as MsgData_sGuildReward;
            EventParameter ep = EventParameter.Get();
            ep.objParameter = info.result;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GUILD_Reward, ep);
        }

        /// <summary>
        /// 发送公会列表请求。
        /// </summary>
        /// <param name="page">页数。</param>
        /// <param name="pagesize">每页数量。</param>
        /// <param name="onlyautoagree">是否只要自动同意的。</param>
        public void SendGuildListRequest(int page, int pagesize, bool onlyautoagree)
        {
            MsgData_cQueryGuildList data = new MsgData_cQueryGuildList();
            data.Page = page;
            data.PageSize = pagesize;
            data.OnlyAutoAgree = onlyautoagree ? 1 : 0;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_QUERY_LIST, data);
        }

        /// <summary>
        /// 发送公会信息请求。
        /// </summary>
        /// <param name="id">公会编号，0表示自己所在的公会。</param>
        public void SendQueryGuildInfoRequest(long id = 0)
        {
            if (id == 0 || id == 1)
            {
                MsgData_cQueryMyGuildInfo data = new MsgData_cQueryMyGuildInfo();
                data.m_i4type = (int)id;
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_QUERY_MY_INFO, data);
            }
            else
            {
                MsgData_cQueryOtherGuildInfo data = new MsgData_cQueryOtherGuildInfo();
                data.GuildId = id;
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_QUERY_OTHER_INFO, data);
            }            
        }

        /// <summary>
        /// 发送公会成员请求。
        /// </summary>
        public void SendQueryGuildMemberRequest()
        {
            MsgData_cQueryMyGuildMems data = new MsgData_cQueryMyGuildMems();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_QUERY_MY_MEMBER, data);            
        }

        /// <summary>
        /// 发送公会事件请求。
        /// </summary>
        public void SendQueryGuildEventRequest()
        {
            MsgData_cQueryMyGuildEvent data = new MsgData_cQueryMyGuildEvent();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_QUERY_MY_EVENT, data);
        }

        /// <summary>
        /// 发送创建公会请求。
        /// </summary>
        /// <param name="name">公会名称。</param>
        /// <param name="name">公会公告。</param>
        public void SendCreateGuildRequest(string name, string notice)
        {
            MsgData_cCreateGuild data = new MsgData_cCreateGuild();
            NetLogicGame.str2Bytes(name, data.Name);
            NetLogicGame.str2Bytes(notice, data.Notice);
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_CREATE, data);
        }

        /// <summary>
        /// 发送退出公会请求。
        /// </summary>
        public void SendQuitGuildRequest()
        {
            MsgData_cQuitGuild data = new MsgData_cQuitGuild();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_QUIT, data);
        }

        /// <summary>
        /// 发送解散公会请求。
        /// </summary>
        public void SendDismissGuildRequest()
        {
            MsgData_cDismissGuild data = new MsgData_cDismissGuild();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_DISMISS, data);
        }

        /// <summary>
        /// 发送升级公会请求。
        /// </summary>
        public void SendLevelUpGuildRequest()
        {
            MsgData_cLvUpGuild data = new MsgData_cLvUpGuild();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_LEVEL_UP, data);
        }

        /// <summary>
        /// 发送升级公会技能请求。(废弃)
        /// </summary>
        /// <param name="group">技能组。</param>
        public void SendLevelUpGuildSkillRequest(int group)
        {
            MsgData_cLvUpGuildSkill data = new MsgData_cLvUpGuildSkill();
            data.GroupId = group;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_LEVEL_UP_SKILL, data);
        }

        /// <summary>
        /// 发送改变职位请求。
        /// </summary>
        /// <param name="id">玩家ID。</param>
        /// <param name="pos">公会职位。</param>
        public void SendChangePosRequest(long id, int pos)
        {
            MsgData_cChangeGuildPos data = new MsgData_cChangeGuildPos();
            data.MemGid = id;
            data.Pos = pos;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_CHANGE_POS, data);
        }

        /// <summary>
        /// 发送改变公告请求。
        /// </summary>
        /// <param name="notice">公告。</param>
        public void SendChangeNoticeRequest(string notice)
        {
            MsgData_cChangeGuildNotice data = new MsgData_cChangeGuildNotice();
            NetLogicGame.str2Bytes(notice, data.Notice);
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_CHANGE_NOTICE, data);
        }

        public void SendGuildReward()
        {
            MsgData_cGuildReward data = new MsgData_cGuildReward();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_REWARD, data);
        }
        /// <summary>
        /// 发送审核结果请求。
        /// </summary>
        /// <param name="op">是否同意0 - 同意，1 - 拒绝</param>
        /// <param name="id">角色编号。</param>
        public void SendVerifyOneApplyRequest(int op, long id)
        {
            List<long> ids = new List<long>(0);
            ids.Add(id);
            SendVerifyApplyRequest(op, ids);
        }

        /// <summary>
        /// 发送审核结果请求。
        /// </summary>
        /// <param name="op">是否同意0 - 同意，1 - 拒绝</param>
        /// <param name="ids">角色编号列表。</param>
        public void SendVerifyApplyRequest(int op, List<long> ids)
        {
            MsgData_cVerifyGuildApply data = new MsgData_cVerifyGuildApply();
            data.Verify = op;
            data.Size = ids.Count;
            data.GuildApplyList = new List<MsgData_cReqGuildApplyVo>();
            for (int i=0; i<ids.Count; ++i)
            {
                MsgData_cReqGuildApplyVo vo = new MsgData_cReqGuildApplyVo();
                vo.MemGid = ids[i];
                data.GuildApplyList.Add(vo);
            }
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_VERIFY_APPLY, data);
        }

        /// <summary>
        /// 发送审核结果请求。
        /// </summary>
        /// <param name="op">是否同意0 - 同意，1 - 拒绝</param>
        /// <param name="t">角色编号列表。</param>
        public void SendVerifyApplyRequestForLua(int op, LuaTable t)
        {
            SendVerifyApplyRequest(op, t.Cast<List<long>>());
        }

        /// <summary>
        /// 发送踢出成员请求。
        /// </summary>
        /// <param name="id">成员编号。</param>
        public void SendKickMemeberRequest(long id)
        {
            MsgData_cKickGuildMem data = new MsgData_cKickGuildMem();
            data.MemGid = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_KICK_MEMEBER, data);
        }

        /// <summary>
        /// 发送申请加入公会请求。
        /// </summary>
        /// <param name="id">公会编号。</param>
        /// <param name="op">0-取消， 1-申请。</param>
        public void SendApplyGuildRequest(long id, int op)
        {
            MsgData_cApplyGuild data = new MsgData_cApplyGuild();
            data.GuildId = id;
            data.Apply = op;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_APPLY, data);
        }

        /// <summary>
        /// 发送邀请加入公会请求。
        /// </summary>
        /// <param name="id">角色编号。</param>
        public void SendInviteRequest(long id)
        {
            MsgData_cInviteToGuild data = new MsgData_cInviteToGuild();
            data.RoleId = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_INVITE, data);
        }

        /// <summary>
        /// 发送邀请回复请求。
        /// </summary>
        /// <param name="id">邀请人编号。</param>
        /// <param name="op">结果 0 - 同意，1 - 不同意。</param>
        public void SendInviteResultRequest(long id, int op)
        {
            MsgData_cInviteToGuildResult data = new MsgData_cInviteToGuildResult();
            data.InviterId = id;
            data.Result = op;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_INVITE_RES, data);
        }

        /// <summary>
        /// 发送设置自动申请审核请求。
        /// </summary>
        /// <param name="auto">结果 0 - 不自动 1 - 自动。</param>
        /// <param name="level">档数。</param>
        public void SendSetAutoVerifyRequest(sbyte auto, int level)
        {
            MsgData_cSetAutoVerify data = new MsgData_cSetAutoVerify();
            data.Auto = auto;
            data.Level = level;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_SET_AUTO_VERIFY, data);
            mGuildInfo.Autoagree = auto;
        }

        /// <summary>
        /// 发送申请列表请求。
        /// </summary>
        public void SendQueryApplyListRequest()
        {
            MsgData_cQueryMyGuildApplys data = new MsgData_cQueryMyGuildApplys();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_QUERY_MY_APPLYS, data);
            mApplysList.Clear();
        }

        /// <summary>
        /// 发送申请列表请求。
        /// </summary>
        public void SendChangeLeaderRequest(long id)
        {
            MsgData_cChangeLeader data = new MsgData_cChangeLeader();
            data.MemGid = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_CHANGE_LEADER, data);
        }

        /// <summary>
        /// 发送捐献请求。
        /// </summary>
        /// <param name="item">捐献物品。</param>
        /// <param name="count">捐献数量。</param>
        public void SendContributeRequest(int item, int count)
        {
            MsgData_cGuildContribute data = new MsgData_cGuildContribute();
            data.ItemId = item;
            data.Count = count;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_CONTRIBUTE, data);
        }

        /// <summary>
        /// 发送升级自身公会技能请求。
        /// </summary>
        /// <param name="group">技能组。</param>
        public void SendLevelUpMySkillRequest(int group)
        {
            MsgData_cLevelUpMyGuildSkill data = new MsgData_cLevelUpMyGuildSkill();
            data.GroupId = group;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_LEVEL_UP_MY_SKILL, data);
        }

        /// <summary>
        /// 发送查找公会请求。
        /// </summary>
        /// <param name="type">查找类型, (0 -公会名， 1- 帮主名)。</param>
        /// <param name="pagesize">每页数量。</param>
        /// <param name="name">名称。</param>
        public void SendSearchRequest(int type, int pagesize, string name)
        {
            MsgData_cSearchGuild data = new MsgData_cSearchGuild();
            data.Type = type;
            data.PageSize = pagesize;
            NetLogicGame.str2Bytes(name, data.Name);
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_SEARCH, data);
        }

        /// <summary>
        /// 发送加持属性请求。
        /// </summary>
        public void SendAidInfoRequest()
        {
            MsgData_cReqAidInfo data = new MsgData_cReqAidInfo();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_AID_INFO, data);
        }

        /// <summary>
        /// 发送加持洗炼请求。
        /// </summary>
        public void SendAidBapRequest()
        {
            MsgData_cReqUnionBapAid data = new MsgData_cReqUnionBapAid();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_AID_BAP, data);
        }

        /// <summary>
        /// 发送加持升级请求。
        /// </summary>
        public void SendAidLevelUpRequest()
        {
            MsgData_cReqAidUpLevel data = new MsgData_cReqAidUpLevel();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_AID_LEVEL_UP, data);
        }

        /// <summary>
        /// 发送洗炼操作请求。
        /// </summary>
        /// <param name="op">0 清除， 1 保存</param>
        public void SendAidOPRequest(int op)
        {
            MsgData_cReqClearBapAidInfo data = new MsgData_cReqClearBapAidInfo();
            data.State = op;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_AID_BAP_OP, data);
            mTempAidAttr = null;
        }

        /// <summary>
        /// 发送祈福信息请求。
        /// </summary>
        public void SendPrayInfoRequest()
        {
            MsgData_cReqGetUnionPray data = new MsgData_cReqGetUnionPray();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_PRAY_INFO, data);
        }

        /// <summary>
        /// 发送祈福操作请求。
        /// </summary>
        /// <param name="id">祈福类型</param>
        public void SendPrayOPRequest(int id)
        {
            MsgData_cReqUnionPray data = new MsgData_cReqUnionPray();
            data.PrayID = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_PRAY, data);
        }

        /// <summary>
        /// 发送公会同盟请求。
        /// </summary>
        /// <param name="id">id公会编号，0表示自己公会。</param>
        public void SendGuildAllianceRequest(long id = 0)
        {
            MsgData_cCreateAlliance data = new MsgData_cCreateAlliance();
            data.GuildId = id == 0 ? GuildID : id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_CREATE_ALLIANCE, data);
        }

        /// <summary>
        /// 发送公会同盟申请列表请求。
        /// </summary>
        public void SendQueryAllianceApplysRequest()
        {
            MsgData_cQueryGuildAllianceApplys data = new MsgData_cQueryGuildAllianceApplys();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_QUERY_ALLIANCE_APPLYS, data);
            mAllianceApplysList.Clear();
        }

        /// <summary>
        /// 发送公会同盟申请回复请求。
        /// </summary>
        /// <param name="op">是否同意0 - 同意，1 - 拒绝。</param>
        /// <param name="ids">编号列表。</param>
        public void SendAllianceApplysVerifyRequest(int op, List<long> ids)
        {
            MsgData_cGuildAllianceVerify data = new MsgData_cGuildAllianceVerify();
            data.Verify = op;
            data.Size = ids.Count;
            data.GuildAllianceApplyList = new List<MsgData_cReqGuildAllianceApplyVo>();
            for (int i=0; i<ids.Count; ++i)
            {
                MsgData_cReqGuildAllianceApplyVo vo = new MsgData_cReqGuildAllianceApplyVo();
                vo.GuildId = ids[i];
                data.GuildAllianceApplyList.Add(vo);
            }
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GUILD_ALLIANCE_VERIFY, data);
        }
    }
}