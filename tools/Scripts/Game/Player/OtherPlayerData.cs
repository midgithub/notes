/**
* @file     : OtherPlayerData.cs
* @brief    : 其它玩家数据
* @details  : 
* @author   : XuXiang
* @date     : 2017-09-19 10:05
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;

namespace SG
{
    /// <summary>
    /// 信息类型。
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public static class OtherPlayerInfoType
    {
        public static int INFO_TYPE_BASIC = 0x0001;             //基本信息
        public static int INFO_TYPE_DETAIL = 0x0002;            //详细信息
        public static int INFO_TYPE_MOUNT = 0x0004;             //坐骑
        public static int INFO_TYPE_WUHUN = 0x0008;             //武魂
        public static int INFO_TYPE_EQUIP = 0x0010;             //装备宝石
        public static int INFO_TYPE_SUPER_HOLE = 0x0020;        //卓越孔信息
        public static int INFO_TYPE_BODY_TOOL = 0x0040;         //身上道具
        public static int INFO_TYPE_LZ = 0x0080;                //灵阵
        public static int INFO_TYPE_SB = 0x0100;                //神兵
    }

    /// <summary>
    /// 其它玩家数据。
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public class OtherPlayerData
    {
        private static OtherPlayerData _instance = null;

        /// <summary>
        /// 获取玩家数据。
        /// </summary>
        public static OtherPlayerData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OtherPlayerData();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 当前玩家的编号。
        /// </summary>
        private long mRoleID = 0;

        /// <summary>
        /// 获取角色编号。
        /// </summary>
        public long RoleID
        {
            get { return mRoleID; }
        }

        /// <summary>
        /// 基本信息。
        /// </summary>
        private MsgData_sOtherHumanBSInfoRet mBasicInfo = null;

        private bool[] VIPState = new bool[3];

        /// <summary>
        /// 获取基本信息。
        /// </summary>
        public MsgData_sOtherHumanBSInfoRet BasicInfo
        {
            get { return mBasicInfo; }
        }

        /// <summary>
        /// 获取玩家职业
        /// </summary>
        public int Prof
        {
            get { return mBasicInfo == null ? 0 : mBasicInfo.Prof; }
        }

        /// <summary>
        /// 获取装备编号。
        /// </summary>
        /// <param name="pos">装备位置。</param>
        /// <returns>装备编号，0表示无装备。</returns>
        public int GetEquipID(int pos)
        {
            if (mBasicInfo == null)
            {
                return 0;
            }

            for (int i = 0; i < mBasicInfo.ItemEquipList.Count; ++i)
            {
                int id = mBasicInfo.ItemEquipList[i].TID;
                LuaTable t = ConfigManager.Instance.BagItem.GetItemConfig(id);
                if (t != null)
                {
                    if (t.Get<int>("pos") == pos)
                    {
                        return id;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// 获取衣服模型ID。
        /// </summary>
        /// <returns>衣服模型ID。</returns>
        public int GetDressModelID()
        {
            if (mBasicInfo == null)
            {
                return 0;
            }

            return SceneLoader.GetClothesModelID(mBasicInfo.FashionDress, mBasicInfo.Dress, mBasicInfo.Prof, 0);
        }

        /// <summary>
        /// 获取武器模型ID。
        /// </summary>
        /// <returns>武器模型ID。</returns>
        public int GetWeaponModelID()
        {
            if (mBasicInfo == null)
            {
                return 0;
            }

            return SceneLoader.GetWeaponModelID(mBasicInfo.FashionWeapon, mBasicInfo.ShenBing, GetEquipID(PlayerEquipPos.Weapon), mBasicInfo.Prof);
        }

        /// <summary>
        /// 获取翅膀模型ID。
        /// </summary>
        /// <returns>翅膀模型ID。</returns>
        public int GetWingModelID()
        {
            if (mBasicInfo == null)
            {
                return 0;
            }

            return SceneLoader.GetWingModelID(mBasicInfo.FashionWing, mBasicInfo.Wing, mBasicInfo.Prof);
        }

        /// <summary>
        /// 详细信息。
        /// </summary>
        private MsgData_sOtherHumanXXInfoRet mDetailInfo = null;

        /// <summary>
        /// 获取详细信息。
        /// </summary>
        public MsgData_sOtherHumanXXInfoRet DetailInfo
        {
            get { return mDetailInfo; }
        }

        /// <summary>
        /// 坐骑信息。
        /// </summary>
        private MsgData_sOtherMountInfoRet mMountInfo = null;

        /// <summary>
        /// 获取坐骑信息。
        /// </summary>
        public MsgData_sOtherMountInfoRet MountInfo
        {
            get { return mMountInfo; }
        }

        /// <summary>
        /// 获取坐骑编号。为0表示为获得坐骑。
        /// </summary>
        public int RideID
        {
            get { return mMountInfo == null ? 0 : mMountInfo.RideLevel; }
        }

        /// <summary>
        /// 身上道具信息。
        /// </summary>
        private MsgData_sOtherBodyTool mBodyToolInfo = null;

        /// <summary>
        /// 获取身上道具信息。
        /// </summary>
        public MsgData_sOtherBodyTool BodyToolInfo
        {
            get { return mBodyToolInfo; }
        }

        /// <summary>
        /// 注册网络消息。
        /// </summary>
        public void RegisterNetMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, OnCleanupUserData);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_OTHER_PLAYER_INFO, OnInfoResult);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_OTHER_PLAYER_BASE, OnBasicInfo);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_OTHER_PLAYER_DETAIL, OnDetailInfo);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_OTHER_PLAYER_MOUNT, OnMountInfo);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_OTHER_PLAYER_BODY_TOOL, OnBodyToolInfo);
        }

        /// <summary>
        ///  切换账号清除数据
        /// </summary>
        private void OnCleanupUserData(GameEvent ge, EventParameter parameter)
        {
            mRoleID = 0;
            mBasicInfo = null;
            VIPState[0] = false;
            VIPState[1] = false;
            VIPState[2] = false;
            mDetailInfo = null;
            mMountInfo = null;
            mBodyToolInfo = null;
        }

        /// <summary>
        /// 信息查询结果。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        public void OnInfoResult(MsgData data)
        {
            MsgData_sOtherHumanInfoRet info = data as MsgData_sOtherHumanInfoRet;

            EventParameter parameter = EventParameter.Get();
            parameter.intParameter = info.Result;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OTHER_PLAYER_INFO, parameter);
        }

        /// <summary>
        /// 收到基本信息。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        public void OnBasicInfo(MsgData data)
        {
            MsgData_sOtherHumanBSInfoRet info = data as MsgData_sOtherHumanBSInfoRet;
            mBasicInfo = info;

            //解析VIP
            VIPState[0] = ((mBasicInfo.VIPLevel >> 31) & 1) != 0;
            VIPState[1] = ((mBasicInfo.VIPLevel >> 30) & 1) != 0;
            VIPState[2] = ((mBasicInfo.VIPLevel >> 29) & 1) != 0;
            mBasicInfo.VIPLevel = mBasicInfo.VIPLevel & 0x1FFFFFFF;

            EventParameter parameter = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OTHER_PLAYER_BASIC, parameter);
        }

        /// <summary>
        /// 收到详细信息。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        public void OnDetailInfo(MsgData data)
        {
            MsgData_sOtherHumanXXInfoRet info = data as MsgData_sOtherHumanXXInfoRet;
            mDetailInfo = info;

            EventParameter parameter = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OTHER_PLAYER_DETAIL, parameter);
        }

        /// <summary>
        /// 收到坐骑信息。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        public void OnMountInfo(MsgData data)
        {
            MsgData_sOtherMountInfoRet info = data as MsgData_sOtherMountInfoRet;
            mMountInfo = info;

            EventParameter parameter = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OTHER_PLAYER_MOUNT, parameter);
        }

        /// <summary>
        /// 收到身上道具信息。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        public void OnBodyToolInfo(MsgData data)
        {
            MsgData_sOtherBodyTool info = data as MsgData_sOtherBodyTool;
            mBodyToolInfo = info;

            EventParameter parameter = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OTHER_PLAYER_BODY_TOOL, parameter);
        }

        /// <summary>
        /// 发送玩家新查询请求。 
        /// </summary>
        /// <param name="id">角色编号。</param>
        /// <param name="type">查询数据类型。</param>
        public void SendInfoQueryRequest(long id, int type)
        {
            MsgData_cOtherHumanInfo data = new MsgData_cOtherHumanInfo();
            data.RoleID = id;
            data.Type = type;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_OTHER_PLAYER_INFO, data);

            if (mRoleID != id)
            {
                mRoleID = id;
                mBasicInfo = null;
                mDetailInfo = null;
                mMountInfo = null;
                mBodyToolInfo = null;
            }
        }
    }
}
