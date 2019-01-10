/**
* @file     : FriendData.cs
* @brief    : 好友数据
* @details  : 
* @author   : XuXiang
* @date     : 2017-09-14 10:06
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;

namespace SG
{
    /// <summary>
    /// 好友数据。
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public class FriendData
    {
        /// <summary>
        /// 玩家简单信息。
        /// </summary>
        public class RoleSimpleInfo
        {
            /// <summary>
            /// 编号。
            /// </summary>
            public long ID { get; set; }

            /// <summary>
            /// 服务器。
            /// </summary>
            public int Server { get; set; }

            /// <summary>
            /// 名称。
            /// </summary>
            public String Name { get; set; }

            /// <summary>
            /// 等级。
            /// </summary>
            public int Level { get; set; }
        }

        /// <summary>
        /// 关系信息。
        /// </summary>
        private Dictionary<long, MsgData_sRelationVO> m_RelationInfo = new Dictionary<long, MsgData_sRelationVO>();

        /// <summary>
        /// 获取关系列表。
        /// </summary>
        /// <param name="type">关系类型。</param>
        /// <returns>关系列表。</returns>
        public List<MsgData_sRelationVO> GetRelationList(int type)
        {
            List<MsgData_sRelationVO> ret = new List<MsgData_sRelationVO>();
            foreach (var kvp in m_RelationInfo)
            {
                if (CheckRelation(kvp.Value.RelationFlag, type))
                {
                    ret.Add(kvp.Value);
                }
            }

            return ret;
        }

        /// <summary>
        /// 获取与某个角色之间的关系。
        /// </summary>
        /// <param name="id">角色编号。</param>
        /// <returns>关系信息。</returns>
        public MsgData_sRelationVO GetRelation(long id)
        {
            MsgData_sRelationVO vo;
            m_RelationInfo.TryGetValue(id, out vo);
            return vo;
        }

        /// <summary>
        /// 判断目标是否在线。
        /// </summary>
        /// <param name="id">玩家编号。</param>
        /// <returns>是否在线。</returns>
        public bool IsOnline(long id)
        {
            MsgData_sRelationVO vo = GetRelation(id);
            return vo != null && vo.OnlineStatus == 0;
        }

        /// <summary>
        /// 判断与某个玩家是否为好友。
        /// </summary>
        /// <param name="id">目标玩家编号。</param>
        /// <returns>是否为好友。</returns>
        public bool IsFriend(long id)
        {
            MsgData_sRelationVO vo = GetRelation(id);
            return vo != null && CheckRelation(vo.RelationFlag, RelationType.RELATION_TYPE_FRIEND);
        }

        /// <summary>
        /// 判断与某个玩家是否为仇人。
        /// </summary>
        /// <param name="id">目标玩家编号。</param>
        /// <returns>是否为仇人。</returns>
        public bool IsEnemy(long id)
        {
            MsgData_sRelationVO vo = GetRelation(id);
            return vo != null && CheckRelation(vo.RelationFlag, RelationType.RELATION_TYPE_ENEMY);
        }

        /// <summary>
        /// 判断与某个玩家是否被拉黑。
        /// </summary>
        /// <param name="id">目标玩家编号。</param>
        /// <returns>是否被拉黑。</returns>
        public bool IsInBlackList(long id)
        {
            MsgData_sRelationVO vo = GetRelation(id);
            return vo != null && CheckRelation(vo.RelationFlag, RelationType.RELATION_TYPE_BLACK);
        }

        /// <summary>
        /// 获取在线人数。
        /// </summary>
        /// <param name="list">关系列表。</param>
        /// <returns>在线人数。</returns>
        public static int GetOnlineNumber(List<MsgData_sRelationVO> list)
        {
            int num = 0;
            for (int i=0; i< list.Count; ++i)
            {
                if (list[i].OnlineStatus == 0)
                {
                    ++num;
                }
            }
            return num;
        }

        ///// <summary>
        ///// 设置角色在线状态。
        ///// </summary>
        ///// <param name="id">角色编号。</param>
        ///// <param name="state">1-玩家在线，2-三天未登录。</param>
        //private void SetOnlineStatus(long id, long state)
        //{
        //    for (int i=0; i< m_RelationInfo.Count; ++i)
        //    {
        //        if (m_RelationInfo[i].RoleID == id)
        //        {
        //            m_RelationInfo[i].OnlineStatus = (sbyte)state;
        //        }
        //    }
        //}

        /// <summary>
        /// 获取好友列表。
        /// </summary>
        public List<MsgData_sRelationVO> FriendList
        {
            get
            {
                List<MsgData_sRelationVO> list = GetRelationList(RelationType.RELATION_TYPE_FRIEND);
                list.Sort(CompareFirend);
                return list;
            }
        }

        /// <summary>
        /// 比较好友排序。
        /// </summary>
        public static int CompareFirend(MsgData_sRelationVO a, MsgData_sRelationVO b)
        {
            //在线的排前面
            if (a.OnlineStatus == 1 && b.OnlineStatus != 1)
            {
                return -1;
            }
            if (a.OnlineStatus != 1 && b.OnlineStatus == 1)
            {
                return 1;
            }

            return b.Level - a.Level;       //其次按等级
        }

        /// <summary>
        /// 获取仇人列表。
        /// </summary>
        public List<MsgData_sRelationVO> EnemyList
        {
            get
            {
                //按最近击杀事件排序
                List<MsgData_sRelationVO> list = GetRelationList(RelationType.RELATION_TYPE_ENEMY);
                list.Sort((a, b) => { return (int)(b.KillTime - a.KillTime); });
                return list;
            }
        }

        /// <summary>
        /// 获取黑名单列表。
        /// </summary>
        public List<MsgData_sRelationVO> BlackList
        {
            get { return GetRelationList(RelationType.RELATION_TYPE_BLACK); }
        }

        /// <summary>
        /// 获取最近联系人列表。
        /// </summary>
        public List<MsgData_sRelationVO> RecentList
        {
            get { return GetRelationList(RelationType.RELATION_TYPE_RECENT); }
        }

        /// <summary>
        /// 可领取的好友礼包列表。
        /// </summary>
        private List<MsgData_sFriendReward> mFriendRewards = new List<MsgData_sFriendReward>();

        /// <summary>
        /// 获取可领取的好友礼包列表。
        /// </summary>
        public List<MsgData_sFriendReward> FriendRewards
        {
            get { return mFriendRewards; }
        }

        /// <summary>
        /// 好友推荐列表。
        /// </summary>
        private List<MsgData_sFriendRecommendVO> mRecommendList = new List<MsgData_sFriendRecommendVO>();

        /// <summary>
        /// 获取好友推荐列表。
        /// </summary>
        public List<MsgData_sFriendRecommendVO> RecommendList
        {
            get { return mRecommendList; }
        }

        /// <summary>
        /// 检查推荐列表，移除已经是好友的玩家。
        /// </summary>
        public void CheckRecommendList()
        {
            int i = 0;
            while (i < mRecommendList.Count)
            {
                if (IsFriend(mRecommendList[i].RoleID))
                {
                    mRecommendList.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }
        }

        /// <summary>
        /// 查找角色编号。
        /// </summary>
        private RoleSimpleInfo mFindRole = new RoleSimpleInfo();

        /// <summary>
        /// 获取查找角色。
        /// </summary>
        public RoleSimpleInfo FindRole
        {
            get { return mFindRole; }
        }

        /// <summary>
        /// 奖励列表。
        /// </summary>
        private List<RoleSimpleInfo> mHaveReward = new List<RoleSimpleInfo>();

        /// <summary>
        /// 获取奖励列表。
        /// </summary>
        public List<RoleSimpleInfo> HaveReward
        {
            get { return mHaveReward; }
        }

        /// <summary>
        /// 添加申请列表。
        /// </summary>
        private List<RoleSimpleInfo> mAddApplyList = new List<RoleSimpleInfo>();

        /// <summary>
        /// 获取首个申请列表。
        /// </summary>
        /// <param name="id">角色编号。</param>
        public RoleSimpleInfo GetAddApplyInfo(long id)
        {
            for (int i = 0; i< mAddApplyList.Count; ++i)
            {
                if (mAddApplyList[i].ID == id)
                {
                    return mAddApplyList[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 获取首个申请列表。
        /// </summary>
        public RoleSimpleInfo GetFirstAddApplyInfo()
        {
            return mAddApplyList.Count > 0 ? mAddApplyList[0] : null;
        }

        /// <summary>
        /// 移除首个申请列表。
        /// </summary>
        public void RemoveFirstAddApplyInfo()
        {
            if (mAddApplyList.Count > 0)
            {
                mAddApplyList.RemoveAt(0);
            }
        }
        
        /// <summary>
        /// 注册网络消息。
        /// </summary>
        public void RegisterNetMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, OnCleanupUserData);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FRIEND_RELATION_LIST, OnRelationInfo);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FRIEND_RECOMMEND_LIST, OnRecommendList);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FRIEND_FIND, OnFindResult);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FRIEND_ONLINE_STATUS, OnOnlineStatus);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FRIEND_HAVE_REWARD, OnHaveReward);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FRIEND_GET_REWARD, OnGetReward);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FRIEND_REMOVE_RELATION, OnRemoveRelation);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FRIEND_ADD_APPLY, OnAddApply);
        }

        /// <summary>
        ///  切换账号清除数据
        /// </summary>
        private void OnCleanupUserData(GameEvent ge, EventParameter parameter)
        {
            m_RelationInfo.Clear();
            mRecommendList.Clear();
            mHaveReward.Clear();
            mAddApplyList.Clear();
        }

        /// <summary>
        /// 队伍信息。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnRelationInfo(MsgData data)
        {
            MsgData_sRelationList info = data as MsgData_sRelationList;
            for (int i=0; i<info.RelationList.Count; ++i)
            {
                MsgData_sRelationVO vo = info.RelationList[i];
                MsgData_sRelationVO savevo;
                if (m_RelationInfo.TryGetValue(vo.RoleID, out savevo))
                {
                    m_RelationInfo[vo.RoleID] = vo;     //更新
                }
                else
                {
                    m_RelationInfo.Add(vo.RoleID, vo);
                }            
            }

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FRIEND_INFO, ep);
        }

        /// <summary>
        /// 检查是否包含对应关系。
        /// </summary>
        /// <param name="flag">关系标志。</param>
        /// <param name="type">关系类型。</param>
        /// <returns>是否包含对应关系。</returns>
        public static bool CheckRelation(sbyte flag, int type)
        {
            byte mark = (byte)(1 << (type - 1));
            return (flag & mark) > 0;
        }

        /// <summary>
        /// 移除关系。
        /// </summary>
        /// <param name="flag">关系标志。</param>
        /// <param name="type">关系类型。</param>
        /// <returns>移除关系后的标志。</returns>
        public static sbyte RemoveRelation(sbyte flag, int type)
        {
            byte mark = (byte)(1 << (type - 1));
            return (sbyte)(flag & ~mark);
        }

        /// <summary>
        /// 好友推荐列表。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnRecommendList(MsgData data)
        {
            MsgData_sFriendRecommendList info = data as MsgData_sFriendRecommendList;
            mRecommendList.Clear();
            mRecommendList.AddRange(info.RoleList);

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FRIEND_RECOMMEND_LIST, ep);
        }

        /// <summary>
        /// 好友查找结果。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnFindResult(MsgData data)
        {
            MsgData_sFindFriendTarget info = data as MsgData_sFindFriendTarget;
            int server;
            mFindRole.ID = info.RoleID;
            mFindRole.Name = PlayerData.GetPlayerName(UiUtil.GetNetString(info.RoleName), out server);
            mFindRole.Server = server;
            mFindRole.Level = info.Level;

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FRIEND_FIND, ep);
        }

        /// <summary>
        /// 好友在线状态改变。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnOnlineStatus(MsgData data)
        {
            MsgData_sRelationOnLineStatus info = data as MsgData_sRelationOnLineStatus;
            MsgData_sRelationVO rvo = GetRelation(info.RoleID);
            if (rvo == null)
            {
                return;
            }
            rvo.OnlineStatus = info.LastLoginTime;
            //SetOnlineStatus(info.RoleID, info.LastLoginTime);
            //for (int i=0; i<info.RelationList.Count; ++i)
            //{
            //    MsgData_sRelationOnLineVO vo = info.RelationList[i];
            //    SetOnlineStatus(vo.RoleID, vo.OnlineStatus);
            //}

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FRIEND_ONLINE_STATUS, ep);
        }

        /// <summary>
        /// 有好友礼包通知。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnHaveReward(MsgData data)
        {
            MsgData_sFriendReward info = data as MsgData_sFriendReward;
            RoleSimpleInfo roleinfo = new RoleSimpleInfo();
            int server;
            roleinfo.ID = info.RoleID;
            roleinfo.Name = PlayerData.GetPlayerName(UiUtil.GetNetString(info.RoleName), out server);
            roleinfo.Server = server;
            roleinfo.Level = info.Level;
            mHaveReward.Add(roleinfo);

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FRIEND_HAVE_REWARD, ep);
        }

        /// <summary>
        /// 领取好友礼包返回。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnGetReward(MsgData data)
        {
            MsgData_sFriendRewardGet info = data as MsgData_sFriendRewardGet;
            mHaveReward.Clear();

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FRIEND_GET_REWARD, ep);
        }

        /// <summary>
        /// 移除关系。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnRemoveRelation(MsgData data)
        {
            MsgData_sRemoveRelation info = data as MsgData_sRemoveRelation;
            for (int i = 0; i < info.RemoveList.Count; ++i)
            {
                MsgData_sRemoveRelationVO vo = info.RemoveList[i];
                MsgData_sRelationVO savevo;
                if (m_RelationInfo.TryGetValue(vo.RoleID, out savevo))
                {
                    savevo.RelationFlag = RemoveRelation(savevo.RelationFlag, vo.RelationType);
                }
            }
            
            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FRIEND_REMOVE_RELATION, ep);
        }

        /// <summary>
        /// 好友添加申请。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnAddApply(MsgData data)
        {
            MsgData_sAddFriendApply info = data as MsgData_sAddFriendApply;
            if (IsFriend(info.RoleID) || IsInBlackList(info.RoleID))            //已经时好友或者被拉黑了则屏蔽
            {
                return;
            }

            RoleSimpleInfo roleinfo = GetAddApplyInfo(info.RoleID);
            if (roleinfo == null)
            {
                int server;
                roleinfo = new RoleSimpleInfo();
                roleinfo.ID = info.RoleID;
                roleinfo.Level = info.Level;
                roleinfo.Name = PlayerData.GetPlayerName(UiUtil.GetNetString(info.RoleName), out server);
                roleinfo.Server = server;
                mAddApplyList.Add(roleinfo);
            }

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FRIEND_REMOVE_RELATION, ep);
        }

        /// <summary>
        /// 发送好友信息请求。
        /// </summary>
        public void SendFriendInfoRequest()
        {
            MsgData_cRelationChangeList data = new MsgData_cRelationChangeList();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_FRIEND_RELATION_CHANGE_LIST, data);
        }

        /// <summary>
        /// 发送好友推荐请求。
        /// </summary>
        public void SendAskRecommendListRequest()
        {
            MsgData_cAskRecommendList data = new MsgData_cAskRecommendList();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_FRIEND_ASK_RECOMMEND_LIST, data);
            mRecommendList.Clear();
        }

        /// <summary>
        /// 发送好友查找请求。
        /// </summary>
        /// <param name="name">查找名称。</param>
        public void SendFindRequest(string name)
        {
            MsgData_cFindFriend data = new MsgData_cFindFriend();
            NetLogicGame.str2Bytes(name, data.RoleName);
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_FRIEND_FIND, data);
        }

        /// <summary>
        /// 发送好友请求。
        /// </summary>
        /// <param name="id">角色ID。</param>
        public void SendAddFriendRequest(long id)
        {
            MsgData_cAddFriendRecommend data = new MsgData_cAddFriendRecommend();
            data.AddFriendCount = 1;
            data.AddFriendList = new List<MsgData_cAddFriendVO>();

            MsgData_cAddFriendVO vo = new MsgData_cAddFriendVO();
            vo.RoleID = id;
            data.AddFriendList.Add(vo);
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_FRIEND_ADD_RECOMMEND, data);
        }

        /// <summary>
        /// 发送推荐好友请求。
        /// </summary>
        /// <param name="ids">角色ID列表。</param>
        public void SendAddRecommendRequest(List<long> ids)
        {
            if (ids.Count <= 0)
            {
                return;
            }

            MsgData_cAddFriendRecommend data = new MsgData_cAddFriendRecommend();
            data.AddFriendCount = (uint)ids.Count;
            data.AddFriendList = new List<MsgData_cAddFriendVO>();
            for (int i=0; i<ids.Count; ++i)
            {
                MsgData_cAddFriendVO vo = new MsgData_cAddFriendVO();
                vo.RoleID = ids[i];
                data.AddFriendList.Add(vo);
            }
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_FRIEND_ADD_RECOMMEND, data);
        }

        /// <summary>
        /// 发送推荐好友请求。
        /// </summary>
        public void SendAddRecommendRequestForLua(LuaTable t)
        {
            SendAddRecommendRequest(t.Cast<List<long>>());
        }

        /// <summary>
        /// 发送添加回复请求。
        /// </summary>
        /// <param name="role">角色编号。</param>
        /// <param name="agree">0-同意， 1-不同意。</param>
        public void SendApproveOnRequest(long role, int agree)
        {
            List<MsgData_cFriendApproveVO> list = new List<MsgData_cFriendApproveVO>();
            MsgData_cFriendApproveVO vo = new MsgData_cFriendApproveVO();
            vo.RoleID = role;
            vo.Agree = agree;
            SendApproveRequest(list);
        }

        /// <summary>
        /// 发送添加回复请求。
        /// </summary>
        /// <param name="list">回复列表。</param>
        public void SendApproveRequest(List<MsgData_cFriendApproveVO> list)
        {
            MsgData_cFriendApproveList data = new MsgData_cFriendApproveList();
            data.ApproveCount = (uint)list.Count;
            data.ApproveList = list;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_FRIEND_APPROVE, data);
        }

        /// <summary>
        /// 发送添加回复请求。
        /// </summary>
        /// <param name="list">回复列表。</param>
        public void SendApproveRequestForLua(LuaTable t)
        {
            SendApproveRequest(t.Cast<List<MsgData_cFriendApproveVO>>());
        }

        /// <summary>
        /// 发送添加黑名单请求。
        /// </summary>
        /// <param name="role">角色编号。</param>
        public void SendAddBlackRequest(long role)
        {
            MsgData_cAddBlackList data = new MsgData_cAddBlackList();
            data.RoleID = role;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_FRIEND_ADD_BLACK, data);
        }

        /// <summary>
        /// 发送删除关系请求。
        /// </summary>
        /// <param name="role">角色编号。</param>
        /// <param name="type">关系类型。</param>
        public void SendRemoveRelationRequest(long role, int type)
        {
            MsgData_cRemoveRelation data = new MsgData_cRemoveRelation();
            data.RoleID = role;
            data.RelationType = type;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_FRIEND_REMOVE_RELATION, data);
        }

        /// <summary>
        /// 发送领取好友礼包请求。
        /// </summary>
        public void SendGetRewardRequest()
        {
            MsgData_cFriendRewardGet data = new MsgData_cFriendRewardGet();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_FRIEND_GET_REWARD, data);
        }
    }
}