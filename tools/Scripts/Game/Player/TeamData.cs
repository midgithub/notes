/**
* @file     : TeamData.cs
* @brief    : 队伍数据。
* @details  : 
* @author   : XuXiang
* @date     : 2017-09-04
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
    public class TeamData
    {
        /// <summary>
        /// 请求加入信息。
        /// </summary>
        public class RequestJoinInfo
        {
            /// <summary>
            /// 角色编号。
            /// </summary>
            public long RoleID { get; set; }

            /// <summary>
            /// 所在服务器。
            /// </summary>
            public int Server { get; set; }

            /// <summary>
            /// 名称。
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 职业。
            /// </summary>
            public int Prof { get; set; }

            /// <summary>
            /// 等级。
            /// </summary>
            public int Level { get; set; }
            
            /// <summary>
            /// 战力。
            /// </summary>
            public int Power { get; set; }
        }

        /// <summary>
        /// 邀请加入信息。
        /// </summary>
        public class InviteJoinInfo
        {
            /// <summary>
            /// 队伍编号。
            /// </summary>
            public long TeamID { get; set; }

            /// <summary>
            /// 邀请类型。
            /// </summary>
            public int Type { get; set; }

            /// <summary>
            /// 队长名称。
            /// </summary>
            public string LeaderName { get; set; }
        }

        /// <summary>
        /// 队伍成员最大数量。
        /// </summary>
        public const int TEAM_MEMBER_MAX = 4;

        /// <summary>
        /// 当前队伍ID。
        /// </summary>
        private long mTeamID;

        /// <summary>
        /// 获取当前队伍ID，无队伍则为0。
        /// </summary>
        public long TeamID
        {
            get { return mTeamID; }
        }

        /// <summary>
        /// 组队目标
        /// </summary>
        private int mTargetID = 1000;       //默认无目标
        public int TargetID
        {
            get
            {
                return mTargetID;
            }
            set
            {
                mTargetID = value;
                if (mTargetID == 0)
                {
                    mTargetID = 1000;
                }
            }
        }

        /// <summary>
        /// 战力限制。
        /// </summary>
        public int PowerLimit { get; set; }
        
        /// <summary>
        /// 修为等级限制。
        /// </summary>
        public int GloryLevelLimit { get; set; }

        /// <summary>
        /// 队伍成员列表。
        /// </summary>
        private List<MsgData_sTeamRole> mMembers = new List<MsgData_sTeamRole>();

        /// <summary>
        /// 获取队伍成员列表。
        /// </summary>
        public List<MsgData_sTeamRole> Members
        {
            get { return mMembers; }
        }

        /// <summary>
        /// 获取队伍成员信息。
        /// </summary>
        /// <param name="id">角色ID。</param>
        /// <returns>队伍成员信息。</returns>
        public MsgData_sTeamRole GetTeamRole(long id)
        {
            for (int i=0; i< mMembers.Count; ++i)
            {
                if (mMembers[i].RoleID == id)
                {
                    return mMembers[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 组队秘境类型。
        /// </summary>
        public static int SecretTeamType = 2;

        /// <summary>
        /// 是否是秘境组队副本
        /// </summary>
        public bool IsSecretTeam()
        {
            if (mTeamID == 0)
            {
                return false;
            }
            LuaTable cfg = ConfigManager.Instance.Common.GetTeamTargetConfig(TargetID);
            return cfg != null && cfg.Get<int>("TagType") == SecretTeamType;
        }

        /// <summary>
        /// 判断玩家是否在队伍内。
        /// </summary>
        /// <param name="id">角色ID。</param>
        /// <returns>是否在队伍内。</returns>
        public bool IsInTeam(long id)
        {
            if (mTeamID == 0)
            {
                return false;
            }
            for (int i = 0; i < mMembers.Count; ++i)
            {
                if (mMembers[i].RoleID == id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取队长角色ID。
        /// </summary>
        public long CaptainRoleID
        {
            get
            {
                for (int i = 0; i < mMembers.Count; ++i)
                {
                    if (mMembers[i].TeamPos == 1)
                    {
                        return mMembers[i].RoleID;
                    }
                }
                return 0;
            }
        }


        /// <summary>
        /// 附近队伍列表。
        /// </summary>
        private List<MsgData_sTeamCase> mNearbyTeam = new List<MsgData_sTeamCase>();

        /// <summary>
        /// 获取附加队伍列表。
        /// </summary>
        public List<MsgData_sTeamCase> NearbyTeam
        {
            get { return mNearbyTeam; }
        }

        /// <summary>
        /// 附加角色列表。
        /// </summary>
        private List<MsgData_sTeamNearbyRoleInfo> mNearbyRole = new List<MsgData_sTeamNearbyRoleInfo>();

        /// <summary>
        /// 获取附加角色列表。
        /// </summary>
        public List<MsgData_sTeamNearbyRoleInfo> NearbyRoles
        {
            get { return mNearbyRole; }
        }

        /// <summary>
        /// 附近队伍列表。
        /// </summary>
        private List<MsgData_sTargetTeamInfo> mTargetTeam = new List<MsgData_sTargetTeamInfo>();

        /// <summary>
        /// 获取附加队伍列表。
        /// </summary>
        public List<MsgData_sTargetTeamInfo> TargetTeam
        {
            get { return mTargetTeam; }
        }

        /// <summary>
        /// 副本ID。
        /// </summary>
        private int mDungeonID;

        /// <summary>
        /// 获取副本ID。
        /// </summary>
        public int DungeonId
        {
            get { return mDungeonID; }
        }

        /// <summary>
        /// 队长所分线。
        /// </summary>
        private int mCaptionLine;

        /// <summary>
        /// 获取队长所在的分线。(仅组队副本时有效)
        /// </summary>
        public int CaptionLine
        {
            get { return mCaptionLine; }
        }

        /// <summary>
        /// 玩家副本状态列表。
        /// </summary>
        private List<MsgData_sDungeonTeamStatus> mRoleStatusList = new List<MsgData_sDungeonTeamStatus>();

        /// <summary>
        /// 获取玩家副本状态列表。
        /// </summary>
        public List<MsgData_sDungeonTeamStatus> RoleStatusList
        {
            get { return mRoleStatusList; }
        }

        /// <summary>
        /// 获取队伍成员副本状态。
        /// </summary>
        /// <param name="id">角色ID。</param>
        /// <returns>成员副本状态。</returns>
        public MsgData_sDungeonTeamStatus GetTeamStatus(long id)
        {
            for (int i=0; i< mRoleStatusList.Count; ++i)
            {
                if (mRoleStatusList[i].RoleID == id)
                {
                    return mRoleStatusList[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 组队请求列表。
        /// </summary>
        private List<RequestJoinInfo> mJoinRequestInfos = new List<RequestJoinInfo>();


        /// <summary>
        /// 获取组队请求列表。
        /// </summary>
        public List<RequestJoinInfo> JoinRequestInfos
        {
            get { return mJoinRequestInfos; }
        }

        /// <summary>
        /// 获取组队请求信息。
        /// </summary>
        /// <param name="id">角色编号。</param>
        /// <returns>加入请求信息。</returns>
        public RequestJoinInfo GetJoinRequestInfo(long id)
        {
            for (int i=0; i< mJoinRequestInfos.Count; ++i)
            {
                if (mJoinRequestInfos[i].RoleID == id)
                {
                    return mJoinRequestInfos[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 获取组队请求信息。
        /// </summary>
        /// <param name="id">角色编号。</param>
        /// <returns>加入请求信息。</returns>
        public void RemoveJoinRequestInfo(long id)
        {
            for (int i = 0; i < mJoinRequestInfos.Count; ++i)
            {
                if (mJoinRequestInfos[i].RoleID == id)
                {
                    mJoinRequestInfos.RemoveAt(i);

                    //通知更新
                    EventParameter ep = EventParameter.Get();
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_JOIN_REQUEST, ep);
                    break;
                }
            }
        }

        /// <summary>
        /// 邀请加入列表。
        /// </summary>
        private List<InviteJoinInfo> mInviteJoinInfos = new List<InviteJoinInfo>();

        /// <summary>
        /// 获取邀请加入信息。
        /// </summary>
        /// <param name="id">队伍编号。</param>
        /// <returns>邀请加入信息。</returns>
        public InviteJoinInfo GetInviteJoinInfo(long id)
        {
            for (int i = 0; i < mInviteJoinInfos.Count; ++i)
            {
                if (mInviteJoinInfos[i].TeamID == id)
                {
                    return mInviteJoinInfos[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 获取下一条邀请加入信息。
        /// </summary>
        /// <returns>邀请信息。</returns>
        public InviteJoinInfo GetNextInviteJoinInfo()
        {
            return mInviteJoinInfos.Count > 0 ? mInviteJoinInfos[0] : null;
        }

        /// <summary>
        /// 移除首条邀请加入信息。
        /// </summary>
        public void RemoveFirstInviteJoinInfo()
        {
            if (mInviteJoinInfos.Count > 0)
            {
                mInviteJoinInfos.RemoveAt(0);
            }
        }

        /// <summary>
        /// 清除所有邀请信息。
        /// </summary>
        public void ClearInviteJoinInfo()
        {
            mInviteJoinInfos.Clear();
        }

        /// <summary>
        /// 自动同意入队申请。
        /// </summary>
        private bool m_AutoAgree = false;

        /// <summary>
        /// 获取或设置是否自动同意入队申请。
        /// </summary>
        public bool AutoAgree
        {
            get { return m_AutoAgree; }
            set { m_AutoAgree = value; }
        }

        /// <summary>
        /// 自动组队。
        /// </summary>
        private bool m_AutoTeam = false;

        /// <summary>
        /// 获取是否为自动组队状态。
        /// </summary>
        public bool AutoTeam
        {
            get { return m_AutoTeam; }
        }

        /// <summary>
        /// 自动组队时间戳。
        /// </summary>
        private float m_AutoTeamTimeStamp;

        /// <summary>
        /// 获取自动组队时间戳。
        /// </summary>
        public float AutoTeamTimeStamp
        {
            get { return m_AutoTeamTimeStamp; }
        }

        /// <summary>
        /// 邀请玩家的时间。
        /// </summary>
        private Dictionary<long, float> m_InviteTime = new Dictionary<long, float>();

        /// <summary>
        /// 获取邀请时间。
        /// </summary>
        /// <param name="id">角色编号。</param>
        /// <returns>邀请时间。</returns>
        public float GetInviteTime(long id)
        {
            float t;
            m_InviteTime.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 注册网络消息。
        /// </summary>
        public void RegisterNetMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, OnCleanupUserData);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TEAM_INFO, OnTeamInfo);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TEAM_EXIT, OnMemberExit);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TEAM_JOIN_REQUEST, OnJoinRequest);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TEAM_INVITE_REQUEST, OnInviteRequest);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TEAM_NEARBY_TEAM, OnNearbyTeam);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TEAM_NEARBY_ROLE, OnNearbyRole);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TEAM_ROLE_JOIN, OnMemberJoin);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TEAM_ROLE_UPDATE, OnMemberUpdate);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TEAM_ROLE_UPDATE_HPMP, OnMemberUpdateHPMP);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TEAM_DUNGEON_UPDATE, OnDungeonUpdate); 
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TEAM_UPDATE_DUNGEON_PREPARE, OnDungeonUpdatePrepare);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TEAM_TARGET_LIST, OnTargetList);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TEAM_AUTO_SETTING, OnAutoSetting);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TEAM_AUTO_TEAM, OnAutoResult);
        }

        /// <summary>
        ///  切换账号清除数据
        /// </summary>
        private void OnCleanupUserData(GameEvent ge, EventParameter parameter)
        {
            mTeamID = 0;
            mTargetID = 1000;       //默认无目标
            PowerLimit = 0;
            GloryLevelLimit = 0;
            mMembers.Clear();
            mNearbyTeam.Clear();
            mNearbyRole.Clear();
            mTargetTeam.Clear();
            mDungeonID = 0;
            mCaptionLine = 0;
            mRoleStatusList.Clear();
            mJoinRequestInfos.Clear();
            mInviteJoinInfos.Clear();
            m_AutoAgree = false;
            m_AutoTeam = false;
            m_AutoTeamTimeStamp = 0;
            m_InviteTime.Clear();
        }

        /// <summary>
        /// 队伍信息。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnTeamInfo(MsgData data)
        {
            MsgData_sTeamInfo info = data as MsgData_sTeamInfo;
            mTeamID = info.TeamID;
            mMembers.Clear();
            mMembers.AddRange(info.RoleList);
            m_AutoTeamTimeStamp = Time.realtimeSinceStartup;

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_INFO, ep);
                        
            if (m_AutoTeam)
            {
                //如果自己不是队长则停止匹配
                if (CaptainRoleID != PlayerData.Instance.RoleID)
                {
                    m_AutoTeam = false;
                    EventParameter ep2 = EventParameter.Get();
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_AUTO_SETTING, ep2);
                }
            }
        }

        /// <summary>
        /// 成员退出。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnMemberExit(MsgData data)
        {
            MsgData_sTeamRoleExit info = data as MsgData_sTeamRoleExit;
            if (info.RoleID == PlayerData.Instance.RoleID)
            {
                //自己离开了队伍，直接整个刷新
                mTeamID = 0;
                TargetID = 0;
                PowerLimit = 0;
                GloryLevelLimit = 0;
                mMembers.Clear();
                mJoinRequestInfos.Clear();
                m_AutoTeamTimeStamp = Time.realtimeSinceStartup;

                EventParameter ep = EventParameter.Get();
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_INFO, ep);
            }
            else
            {
                string rname = string.Empty;
                for (int i = 0; i < mMembers.Count; ++i)
                {
                    if (mMembers[i].RoleID == info.RoleID)
                    {
                        rname = UiUtil.GetNetString(mMembers[i].RoleName);
                        mMembers.RemoveAt(i);
                        break;
                    }
                }

                EventParameter ep = EventParameter.Get();
                ep.longParameter = info.RoleID;
                ep.stringParameter = rname;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_EXIT, ep);
            }            
        }

        /// <summary>
        /// 玩家请求加入。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnJoinRequest(MsgData data)
        {
            MsgData_sTeamJoinRequest info = data as MsgData_sTeamJoinRequest;
            RequestJoinInfo joininfo = GetJoinRequestInfo(info.RoleID);
            if (joininfo != null)
            {
                //已经在请求列表中
                return;
            }
            string netname = UiUtil.GetNetString(info.RoleName);
            int server;
            joininfo = new RequestJoinInfo();
            joininfo.RoleID = info.RoleID;
            joininfo.Name = PlayerData.GetPlayerName(netname, out server);
            joininfo.Server = server;
            joininfo.Level = info.Level;
            joininfo.Power = info.Power;
            joininfo.Prof = info.Prof;
            mJoinRequestInfos.Add(joininfo);

            if (AutoAgree)
            {
                SendJoinApproveRequest(info.RoleID, 1);
            }
            else
            {
                EventParameter ep = EventParameter.Get();
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_JOIN_REQUEST, ep);
            }            
        }

        /// <summary>
        /// 邀请加入。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnInviteRequest(MsgData data)
        {
            MsgData_sTeamInviteRequest info = data as MsgData_sTeamInviteRequest;
            InviteJoinInfo joininfo = GetInviteJoinInfo(info.TeamID);
            if (joininfo != null)
            {
                //已经在邀请列表中
                return;
            }

            string netname = UiUtil.GetNetString(info.LeaderName);
            int server;
            joininfo = new InviteJoinInfo();
            joininfo.TeamID = info.TeamID;
            joininfo.LeaderName = PlayerData.GetPlayerName(netname, out server);
            joininfo.Type = info.Type;
            mInviteJoinInfos.Add(joininfo);

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_INVITE_REQUEST, ep);
        }

        /// <summary>
        /// 附近队伍。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnNearbyTeam(MsgData data)
        {
            MsgData_sTeamNearbyTeam info = data as MsgData_sTeamNearbyTeam;
            mNearbyTeam.Clear();
            mNearbyTeam.AddRange(info.TeamList);

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_NEARBY_TEAM, ep);
        }

        /// <summary>
        /// 附近玩家。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnNearbyRole(MsgData data)
        {
            MsgData_sTeamNearbyRole info = data as MsgData_sTeamNearbyRole;
            mNearbyRole.Clear();
            for (int i=0; i< info.RoleList.Count; ++i)
            {
                //过滤掉已经在队伍里的玩家
                if (info.RoleList[i].TeamState == 0)
                {
                    mNearbyRole.Add(info.RoleList[i]);
                }
            }

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_NEARBY_ROLE, ep);
        }

        /// <summary>
        /// 成员加入。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnMemberJoin(MsgData data)
        {
            MsgData_sTeamRoleJoin info = data as MsgData_sTeamRoleJoin;
            MsgData_sTeamRole role = new MsgData_sTeamRole();
            role.RoleID = info.RoleID;
            role.RoleName = (byte[])info.RoleName.Clone();
            role.Line = info.Line;
            role.MapID = info.MapID;
            role.Prof = info.Prof;
            role.Level = info.Level;
            role.HP = info.HP;
            role.MaxHP = info.MaxHP;
            role.Mp = info.Mp;
            role.MaxMP = info.MaxMP;
            role.Power = info.Power;
            role.GuildName = (byte[])info.GuildName.Clone();
            role.TeamPos = info.TeamPos;
            role.Online = info.Online;
            role.Icon = info.Icon;
            role.Arms = info.Arms;
            role.Dress = info.Dress;
            role.FashionsHead = info.FashionsHead;
            role.FashionsArms = info.FashionsArms;
            role.FashionsDress = info.FashionsDress;
            role.WuhunID = info.WuhunID;
            role.Wing = info.Wing;
            role.SuitFlag = info.SuitFlag;
            role.MagicWeapon = info.MagicWeapon;
            role.VIPLevel = info.VIPLevel;
            role.RoomType = info.RoomType;
            mMembers.Add(role);

            EventParameter ep = EventParameter.Get();
            ep.longParameter = role.RoleID;
            ep.objParameter = role;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_JOIN, ep);
        }

        /// <summary>
        /// 成员数据更新。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnMemberUpdate(MsgData data)
        {
            MsgData_sTeamRoleUpdate info = data as MsgData_sTeamRoleUpdate;
            MsgData_sTeamRole role = GetTeamRole(info.RoleID);
            if (role == null)
            {
                return;
            }

            //更新信息
            role.RoleName = (byte[])info.RoleName.Clone();
            role.Line = info.Line;
            role.MapID = info.MapID;
            role.Prof = info.Prof;
            role.Level = info.Level;
            role.GuildName = (byte[])info.GuildName.Clone();
            role.TeamPos = info.TeamPos;
            role.Online = info.Online;
            role.Icon = info.Icon;
            role.Arms = info.Arms;
            role.Dress = info.Dress;
            role.FashionsHead = info.FashionsHead;
            role.FashionsArms = info.FashionsArms;
            role.FashionsDress = info.FashionsDress;
            role.WuhunID = info.WuhunID;
            role.Wing = info.Wing;
            role.SuitFlag = info.SuitFlag;
            role.MagicWeapon = info.MagicWeapon;
            role.VIPLevel = info.VIPLevel;
            role.RoomType = info.RoomType;

            EventParameter ep = EventParameter.Get();
            ep.longParameter = role.RoleID;
            ep.objParameter = role;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_ROLE_UPDATE, ep);
        }

        /// <summary>
        /// 成员血量和法力更新。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnMemberUpdateHPMP(MsgData data)
        {
            MsgData_sTeamRoleUpdateInfo info = data as MsgData_sTeamRoleUpdateInfo;
            MsgData_sTeamRole role = GetTeamRole(info.RoleID);
            if (role == null)
            {
                return;
            }

            //更新信息
            role.HP = info.HP;
            role.MaxHP = info.MaxHP;
            role.Mp = info.Mp;
            role.MaxMP = info.MaxMP;
            role.Power = info.Power;

            EventParameter ep = EventParameter.Get();
            ep.longParameter = role.RoleID;
            ep.objParameter = role;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_ROLE_UPDATE_HPMP, ep);
        }
        
        /// <summary>
        /// 队伍副本状态更新。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnDungeonUpdate(MsgData data)
        {
            MsgData_sTeamDungeonUpdate info = data as MsgData_sTeamDungeonUpdate;
            mDungeonID = info.DungeonId;
            mCaptionLine = info.Line;
            mRoleStatusList.Clear();
            mRoleStatusList.AddRange(info.RoleList);

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_DUNGEON_UPDATE, ep);
        }

        /// <summary>
        /// 成员副本状态更新。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnDungeonUpdatePrepare(MsgData data)
        {
            MsgData_sUpdateTeamPreparte info = data as MsgData_sUpdateTeamPreparte;
            MsgData_sDungeonTeamStatus teamstatus = GetTeamStatus(info.RoleID);
            if (teamstatus == null)
            {
                return;
            }
            teamstatus.Status = info.RoomType;

            EventParameter ep = EventParameter.Get();
            ep.longParameter = info.RoleID;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_DUNGEON_UPDATE_PREPARE, ep);
        }

        /// <summary>
        /// 目标队伍列表。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnTargetList(MsgData data)
        {
            MsgData_sTargetTeamList info = data as MsgData_sTargetTeamList;
            mTargetTeam.Clear();
            mTargetTeam.AddRange(info.TeamList);

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_TARGET_LIST, ep);
        }

        /// <summary>
        /// 队伍自动状态改变。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnAutoSetting(MsgData data)
        {
            MsgData_sAutoTeamSetting info = data as MsgData_sAutoTeamSetting;
            bool setting = info.Setting != 0;
            if (setting != m_AutoTeam)
            {
                m_AutoTeam = setting;
                m_AutoTeamTimeStamp = Time.realtimeSinceStartup;

                EventParameter ep = EventParameter.Get();
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_AUTO_SETTING, ep);
            }
        }

        /// <summary>
        /// 队伍自动匹配结果。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnAutoResult(MsgData data)
        {
            MsgData_sAutoTeamResult info = data as MsgData_sAutoTeamResult;
            LogMgr.Log("AutoTeamResult:{0}", info.Result);
            m_AutoTeam = false;
            m_AutoTeamTimeStamp = Time.realtimeSinceStartup;

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_AUTO_SETTING, ep);
        }

        /// <summary>
        /// 发送队伍信息请求。
        /// </summary>
        /// <param name="id">队伍编号。</param>
        public void SendTeamInfoRequest(long id = 0)
        {
            MsgData_cTeamInfo data = new MsgData_cTeamInfo();
            data.TeamID = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_INFO, data);
        }

        /// <summary>
        /// 发送创建队伍请求。
        /// </summary>
        /// <param name="roleid">一起邀请组队的玩家编号，为0则不邀请。</param>
        public void SendCreateTeamRequest(long roleid = 0)
        {
            MsgData_cTeamCreate data = new MsgData_cTeamCreate();
            data.TargetRoleID = roleid;
            data.targetID = 0;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_CREATE, data);
        }

        /// <summary>
        /// 发送创建队伍请求。
        /// </summary>
        /// <param name="roleid">一起邀请组队的玩家编号，为0则不邀请。</param>
        public void SendCreateTeamRequest2(long roleid, int target)
        {
            MsgData_cTeamCreate data = new MsgData_cTeamCreate();
            data.TargetRoleID = roleid;
            data.targetID = target;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_CREATE, data);
        }

        /// <summary>
        /// 发送加入队伍请求。
        /// </summary>
        /// <param name="teamid">队伍编号。</param>
        public void SendApplyRequest(long teamid)
        {
            MsgData_cTeamApply data = new MsgData_cTeamApply();
            data.TeamID = teamid;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_APPLY, data);
        }

        /// <summary>
        /// 发送邀请加入请求。
        /// </summary>
        /// <param name="role">邀请的角色。</param>
        /// <param name="type">组队类型。</param>
        public void SendInviteRequest(long role, int type)
        {
            MsgData_cTeamInvite data = new MsgData_cTeamInvite();
            data.TargetRoleID = role;
            data.Type = type;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_INVITE, data);
            m_InviteTime[role] = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 发送退出队伍请求。
        /// </summary>
        public void SendQuitRequest()
        {
            MsgData_cTeamQuit data = new MsgData_cTeamQuit();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_QUIT, data);
        }
        
        /// <summary>
        /// 发送转让队长请求。
        /// </summary>
        /// <param name="role">目标角色。</param>
        public void SendTransferRequest(long role)
        {
            MsgData_cTeamTransfer data = new MsgData_cTeamTransfer();
            data.TargetRoleID = role;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_TRANSFER, data);
        }

        /// <summary>
        /// 发送开除队员请求。
        /// </summary>
        /// <param name="role">目标角色。</param>
        public void SendFireRequest(long role)
        {
            MsgData_cTeamFire data = new MsgData_cTeamFire();
            data.TargetRoleID = role;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_FIRE, data);
        }

        /// <summary>
        /// 发送入队审批请求。
        /// </summary>
        /// <param name="role">目标角色。</param>
        /// <param name="op">1同意0拒绝。</param>
        public void SendJoinApproveRequest(long role, int op)
        {
            MsgData_cTeamJoinApprove data = new MsgData_cTeamJoinApprove();
            data.TargetRoleID = role;
            data.Operate = op;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_JOIN_APPROVE, data);
            RemoveJoinRequestInfo(role);
        }

        /// <summary>
        /// 发送入队邀请反馈请求。
        /// </summary>
        /// <param name="role">目标角色。</param>
        /// <param name="op">1同意0拒绝。</param>
        public void SendInviteApproveRequest(long team, int op, int type)
        {
            MsgData_cTeamInviteApprove data = new MsgData_cTeamInviteApprove();
            data.TeamID = team;
            data.Operate = op;
            data.Type = type;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_INVITE_APPROVE, data);
        }

        /// <summary>
        /// 发送队伍设置请求。
        /// </summary>
        /// <param name="autoteam">自己,自动组队1, 需询问0。</param>
        /// <param name="autoenter">队长,自动同意进入队伍1, 需询问0。</param>
        public void SendSettingRequest(int autoteam, int autoenter)
        {
            MsgData_cTeamSetting data = new MsgData_cTeamSetting();
            data.AutoTeam = autoteam;
            data.AutoAgreeEnter = autoenter;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_SETTING, data);
        }

        /// <summary>
        /// 发送附近队伍请求。
        /// </summary>
        public void SendNearbyTeamRequest()
        {
            MsgData_cTeamNearbyTeam data = new MsgData_cTeamNearbyTeam();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_NEARBY_TEAM, data);
            mNearbyTeam.Clear();
        }

        /// <summary>
        /// 发送附近玩家请求。
        /// </summary>
        public void SendNearbyRoleRequest()
        {
            MsgData_cTeamNearbyRole data = new MsgData_cTeamNearbyRole();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_NEARBY_ROLE, data);
            mNearbyRole.Clear();
        }

        /// <summary>
        /// 发送是否同意组队副本请求。
        /// </summary>
        /// <param name="reply">答复结果:1同意，0拒绝。</param>
        public void SendReplyTeamDungeonRequest(int reply)
        {
            MsgData_cReplyTeamDungeon data = new MsgData_cReplyTeamDungeon();
            data.Reply = reply;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_REPLY_TEAM_DUNGEON, data);
        }

        /// <summary>
        /// 发送组队活动状态请求。
        /// </summary>
        /// <param name="prepare">准备状态 0 true 1 false。</param>
        /// <param name="type">邀请类型 0:正常组队 1:魔域深渊  2:奇遇 3:帮派组队。</param>
        public void SendEnterTeamDungeonRequest(int prepare, int type)
        {
            MsgData_cEnterDulpPrepare data = new MsgData_cEnterDulpPrepare();
            data.Prepare = prepare;
            data.Type = type;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_ENTER_DUNGEON_PREPARRE, data);
        }

        /// <summary>
        /// 发送招募请求。
        /// </summary>
        public void SendSecreZhaomuRequest()
        {
            MsgData_cTeamSecreZhaoMu data = new MsgData_cTeamSecreZhaoMu();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_SECRE_ZHAOMU, data);
        }

        /// <summary>
        /// 发送目标队伍请求。
        /// </summary>
        public void SendTargetTeamRequest(int target)
        {
            MsgData_cTargetTeamList data = new MsgData_cTargetTeamList();
            data.TargetID = target;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_TARGET_LIST, data);
            mTargetTeam.Clear();
        }

        /// <summary>
        /// 自动匹配成员，队长使用。
        /// </summary>
        public void SendSetAutoTeamRequest()
        {
            m_AutoTeam = !m_AutoTeam;
            m_AutoTeamTimeStamp = Time.realtimeSinceStartup;

            MsgData_cAutoTeam data = new MsgData_cAutoTeam();
            data.Type = m_AutoTeam ? 1 : 2;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_AUTO, data);
        }

        /// <summary>
        /// 自动匹配队伍，无队伍时使用。
        /// </summary>
        public void SendAutoTeamRequest()
        {
            MsgData_cSetAutoTeam data = new MsgData_cSetAutoTeam();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TEAM_SET_AUTO, data);
        }
    }
}