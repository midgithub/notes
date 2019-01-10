/**
* @file     : FashionData.cs
* @brief    : 时装数据
* @details  : 
* @author   : XuXiang
* @date     : 2017-09-09 15:19
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;

namespace SG
{
    /// <summary>
    /// 时装数据。
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public class FashionData
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
        /// 拥有的时装。
        /// </summary
        private Dictionary<int, MsgData_sFashionVO> mFashions = new Dictionary<int, MsgData_sFashionVO>();

        /// <summary>
        /// 获取说有时装编号列表。
        /// </summary>
        /// <returns></returns>
        public List<int> GetAllFashion()
        {
            List<int> keys = new List<int>();
            keys.AddRange(mFashions.Keys);
            return keys;
        }

        /// <summary>
        /// 获取时装信息。
        /// </summary>
        /// <param name="id">时装编号。</param>
        /// <returns>时装信息。</returns>
        public MsgData_sFashionVO GetFashionInfo(int id)
        {
            MsgData_sFashionVO info;
            mFashions.TryGetValue(id, out info);
            return info;
        }

        /// <summary>
        /// 获取限时时装列表。
        /// </summary>
        /// <returns>时装列表。</returns>
        public List<MsgData_sFashionVO> GetTimeLimitFashion()
        {
            List<MsgData_sFashionVO> fashions = new List<MsgData_sFashionVO>();
            foreach (var kvp in mFashions)
            {
                if (kvp.Value.Time > 0)
                {
                    fashions.Add(kvp.Value);
                }
            }
            return fashions;
        }

        /// <summary>
        /// 注册网络消息。
        /// </summary>
        public void RegisterNetMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, OnCleanupUserData);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FASHION_INFO, OnFashionInfo);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FASHION_DRESS, OnFashionDress);
        }

        /// <summary>
        ///  切换账号清除数据
        /// </summary>
        private void OnCleanupUserData(GameEvent ge, EventParameter parameter)
        {
            mPower = 0;
            mFashions.Clear();
        }

        /// <summary>
        /// 时装信息。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnFashionInfo(MsgData data)
        {
            MsgData_sFashionsInfo info = data as MsgData_sFashionsInfo;
            for (int i=0;i<info.Fashions.Count;++i)
            {
                MsgData_sFashionVO vo = info.Fashions[i];
                mFashions.Remove(vo.ID);
                if (vo.Time != 0)
                {
                    mFashions.Add(vo.ID, vo);
                }
            }
            
            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FASHION_INFO, ep);
        }

        /// <summary>
        /// 时装信息。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnFashionDress(MsgData data)
        {
            MsgData_sDressFashion info = data as MsgData_sDressFashion;

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            ep.intParameter1 = info.ID;
            ep.intParameter2 = info.OP;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FASHION_DRESS, ep);
        }

        /// <summary>
        /// 发送邀请加入请求。
        /// </summary>
        /// <param name="id">时装编号。</param>
        /// <param name="op">操作类型 1:穿 0:脱。</param>
        public void SendDressRequest(int id, int op)
        {
            MsgData_cDressFashion data = new MsgData_cDressFashion();
            data.ID = id;
            data.OP = op;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_FASHION_DRESS, data);
        }

        /// <summary>
        /// 发送邀请加入请求。
        /// </summary>
        /// <param name="id">时装状态，0显示，1隐藏。</param>
        public void SendSetStateRequest(int state)
        {
            MsgData_cUpdateFashionState data = new MsgData_cUpdateFashionState();
            data.State = state;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_FASHION_SET_STATE, data);
        }
    }
}