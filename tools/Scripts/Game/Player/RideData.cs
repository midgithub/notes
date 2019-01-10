/**
* @file     : RideData.cs
* @brief    : 
* @details  : 文件用途说明
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;
using System;


namespace SG
{
    /// <summary>
    /// 特色坐骑信息。
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public class FeatureRideInfo
    {
        /// <summary>
        /// 坐骑编号。
        /// </summary>
        public int RideID;

        /// <summary>
        /// 坐骑限时。
        /// </summary>
        public long Time;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="id">坐骑编号。</param>
        /// <param name="time">坐骑时限。</param>
        public FeatureRideInfo(int id, long time)
        {
            RideID = id;
            Time = time;
        }
    }

    /// <summary>
    /// 保存玩家坐骑相关数据。
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public class RideData
    {
        /// <summary>
        /// 坐骑阶位。
        /// </summary>
        private int mStage;

        /// <summary>
        /// 获取坐骑阶位。
        /// </summary>
        public int Stage
        {
            get { return mStage; }
        }

        /// <summary>
        /// 星级进度进度。
        /// </summary>
        private int mStarProgress;

        /// <summary>
        /// 获取坐骑星级进度。(乘以10除以WishMax等于当前星级，余数就是进度)
        /// </summary>
        public int StarProgress
        {
            get { return mStarProgress; }
        }

        /// <summary>
        /// 属性丹数量。
        /// </summary>
        private int mPillNum;

        /// <summary>
        /// 获取属性丹数量。
        /// </summary>
        public int PillNum
        {
            get { return mPillNum; }
        }

        /// <summary>
        /// 资质丹数量。
        /// </summary>
        private int mPillNumPercent;

        /// <summary>
        /// 获取资质丹数量。
        /// </summary>
        public int PillNumPercent
        {
            get { return mPillNumPercent; }
        }


        /// <summary>
        /// 当前坐骑ID。
        /// </summary>
        private int mRideID;

        /// <summary>
        /// 获取当前坐骑ID。
        /// </summary>
        public int RideID
        {
            get { return mRideID; }
        }

        /// <summary>
        /// 骑乘状态 0下马 1上马。
        /// </summary>
        private int mRideState;

        /// <summary>
        /// 获取或设置当前坐骑状态。0下马 1上马
        /// </summary>
        public int RideState
        {
            get { return mRideState; }
            set { mRideState = value; }
        }

        /// <summary>
        /// 疲劳值 PS:好奇怪的字段，有可能指坐骑经验。
        /// </summary>
        private int mUpLevelNum;

        /// <summary>
        /// 获取今日升级的次数。
        /// </summary>
        public int UpLevelNum
        {
            get { return mUpLevelNum; }
        }

        /// <summary>
        /// 坐骑战斗力。
        /// </summary>
        private long mPower;

        /// <summary>
        /// 获取坐骑提供的战力。
        /// </summary>
        public long Power
        {
            get { return mPower; }
            set { mPower = value; }
        }

        /// <summary>
        /// 特色坐骑列表。
        /// </summary>
        private Dictionary<int, FeatureRideInfo> mFeatureRides = new Dictionary<int, FeatureRideInfo>();

        /// <summary>
        /// 获取特色坐骑列表。
        /// </summary>
        public Dictionary<int, FeatureRideInfo> FeatureRides
        {
            get { return mFeatureRides; }
        }

        /// <summary>
        /// 获取特色坐骑信息。
        /// </summary>
        /// <param name="id">坐骑编号。</param>
        /// <returns>特色坐骑信息。</returns>
        public FeatureRideInfo GetFeatureRideInfo(int id)
        {
            FeatureRideInfo info;
            mFeatureRides.TryGetValue(id, out info);
            return info;
        }

        /// <summary>
        /// 获取是否可以喂养坐骑。
        /// </summary>
        public bool IsCanAdvance
        {
            get
            {
                LuaTable t = ConfigManager.Instance.Ride.GetRideConfig(mStage);
                LuaTable nt = ConfigManager.Instance.Ride.GetRideConfig(mStage+1);
                if (t != null && nt != null)
                {
                    List<int> consume = t.Get<List<int>>("consume_item");
                    int have = PlayerData.Instance.BagData.GetItemNumber(BagType.ITEM_BAG_TYPE_COMMON, consume[0]);
                    return have >= consume[1];
                }
                return false;
            }
        }

        /// <summary>
        /// 注册网络消息。
        /// </summary>
        public void RegisterNetMsg()
        {
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_RIDE_INFO, OnInitData);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CHANGE_RIDE_STATE, OnChangeRideState);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_RIDE_LEVEL_UP, OnRideLevelUp);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CHANGE_RIDE, OnChangeRideID);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_NOTIFY_CAST_SKILL, OnCastSkill);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, OnCleanupUserData);
        }

        /// <summary>
        ///  切换账号清除数据
        /// </summary>
        private void OnCleanupUserData(GameEvent ge, EventParameter parameter)
        {
            mStage = 0;
            mStarProgress = 0;
            mPillNum = 0;
            mPillNumPercent = 0;
            mRideID = 0;
            mRideState = 0;
            mUpLevelNum = 0;
            mPower = 0;
            mFeatureRides.Clear();
        }

        /// <summary>
        /// 判断是否需要上坐骑。
        /// </summary>
        /// <param name="map">目标地图。</param>
        /// <param name="pos">目标位置。</param>
        /// <returns>是否需要上坐骑。</returns>
        public static bool CheckMountDistance(int map, Vector3 pos)
        {
            //不在同一地图的直接上马
            if (map != 0 && map != MapMgr.Instance.EnterMapId)
            {
                return true;
            }
            float dis = ConfigManager.Instance.Ride.AutoMountDistance;
            Vector3 mypos = PlayerData.Instance.GetPlayer().position;
            float x = mypos.x - pos.x;
            float z = mypos.z - pos.z;
            return x * x + z * z >= dis * dis;
        }
        
        /// <summary>
        /// 触发坐骑信息更新事件。
        /// </summary>
        public static void TriggerEventRideInfo()
        {
            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_RIDE_INFO, ep);
        }

        /// <summary>
        /// 触发坐骑乘骑状态改变事件。
        /// </summary>
        public static void TriggerEventRideStageChange(int state)
        {
            EventParameter ep = EventParameter.Get();
            ep.intParameter = state;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_RIDE_STAGE_CHANGE, ep);
        }

        /// <summary>
        /// 触发坐骑使用属性丹事件。
        /// </summary>
        public static void TriggerEventRideUseAttrDan(int type)
        {
            EventParameter ep = EventParameter.Get();
            ep.intParameter = type;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_RIDE_USE_ATTR_DAN, ep);
        }

        /// <summary>
        /// 触发坐骑升级事件。
        /// </summary>
        /// <param name="result">升级结果。</param>
        /// <param name="type">升级类型。</param>
        /// <param name="old">原来的stage#start。</param>
        public static void TriggerEventLevelUp(int result, int type, string old)
        {
            EventParameter ep = EventParameter.Get();
            ep.intParameter = result;
            ep.intParameter1 = type;
            ep.stringParameter = old;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_RIDE_LEVEL_UP, ep);
        }

        /// <summary>
        /// 触发坐骑修改事件。
        /// </summary>
        public static void TriggerEventChangeID(int id, int state)
        {
            EventParameter ep = EventParameter.Get();
            ep.intParameter = id;
            ep.intParameter1 = state;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_RIDE_CHANGE_RIDE, ep);
        }

        /// <summary>
        /// 初始化数据。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        public void OnInitData(MsgData data)
        {
            MsgData_sRideInfo info = data as MsgData_sRideInfo;
            mStage = info.Stage;
            mStarProgress = info.StarProgress;
            mPillNum = info.PillNum;
            mPillNumPercent = info.PillNumPercent;
            mRideID = info.RideID;
            mRideState = info.RideState;
            mUpLevelNum = info.UpLevelNum;
            mFeatureRides.Clear();
            for (int i=0; i<info.FeatureRideCount; ++i)
            {
                MsgData_sFeatureRide fr = info.FeatureRideList[i];
                FeatureRideInfo frinfo = new FeatureRideInfo(fr.RideID, fr.Time);
                mFeatureRides.Add(frinfo.RideID, frinfo);
            }
            TriggerEventRideInfo();

            //临时缓存坐骑模型            
            if (mStage == 0)
            {
                LuaTable cfg = ConfigManager.Instance.Common.GetHorseConfig(1);
                int mid = cfg.Get<int>(string.Format("model{0}", PlayerData.Instance.Job));
                MagicKeyDrag.AddCache(mid);
            }
        }

        /// <summary>
        /// 更改坐骑状态。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnChangeRideState(MsgData data)
        {
            MsgData_sChangeRideState info = data as MsgData_sChangeRideState;
            if (RideState != info.RideState)
            {
                RideState = info.RideState;
                TriggerEventRideStageChange(RideState);
                if(RideState == 1)
                {
                    CoreEntry.gAudioMgr.PlayUISound(900009);
                }
                else
                {
                    CoreEntry.gAudioMgr.PlayUISound(900010);
                }
                LogMgr.Log(string.Format("OnChangeRideState state:{0}", RideState));
            }
        }

        /// <summary>
        /// 使用属性丹。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnUseAttrDan(MsgData data)
        {
            MsgData_sUseAttrDan info = data as MsgData_sUseAttrDan;
            LogMgr.LogError(string.Format("OnUseAttrDan type:{0} result:{1} pillnumber:{2}", info.Type, info.Result, info.PillNumber));
            if (info.Result == 0)
            {
                if (info.Type == AttrDanType.Ride)
                {
                    mPillNum = info.PillNumber;
                    TriggerEventRideUseAttrDan(info.Type);
                }
                else if (info.Type == AttrDanType.RidePer)
                {
                    mPillNumPercent = info.PillNumber;
                    TriggerEventRideUseAttrDan(info.Type);
                }
            }
        }

        /// <summary>
        /// 坐骑升级。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnRideLevelUp(MsgData data)
        {
            MsgData_sRideLvlUpInfo info = data as MsgData_sRideLvlUpInfo;
            LogMgr.Log(string.Format("OnRideLevelUp result:{0} uptype:{1}", info.Result, info.UpType));
            string oldparm = string.Format("{0}#{1}", mStage, mStarProgress);
            if (info.Result == 0)
            {
                int old = mStage;
                //int oldstar = mStarProgress;                
                mStage = info.RideLevel;
                mStarProgress = info.StarProgress;
                mUpLevelNum = info.UpLevelNum;

                //坐骑出现升阶时，服务器会自动把使用的坐骑改成新坐骑
                if (mStage > old)
                {
                    mRideID = mStage;
                }
            }
            TriggerEventLevelUp(info.Result, info.UpType, oldparm);
        }

        /// <summary>
        /// 更改坐骑。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnChangeRideID(MsgData data)
        {
            MsgData_sChangeRideId info = data as MsgData_sChangeRideId;
            mRideID = info.ID;
            mRideState = info.State;
            TriggerEventChangeID(info.ID, info.State);
        }

        /// <summary>
        /// 玩家释放技能。
        /// </summary>
        private void OnCastSkill(GameEvent ge, EventParameter parameter)
        {
            //如果在马上则下马
            if (mRideState == 1)
            {
                SendChangeRideStateRequest(0);
                CoreEntry.gActorMgr.MainPlayer.GetDownHorse();
            }            
        }

        /// <summary>
        /// 发送改变乘骑状态的请求。
        /// </summary>
        public void SendChangeRideStateRequest(int state)
        {
            if (PlayerData.Instance.BaseAttr.GetUnitBit((int)UnitBit.UNIT_BIT_IN_PK) && state != 0)
            {
                UITips.ShowTips("PK状态中，无法上坐骑");
                return;
            }
            if (mRideState == state)
            {
                return;
            }

            MsgData_cChangeRideState data = new MsgData_cChangeRideState();
            data.RideState = state;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_CHANGE_RIDE_STATE, data);
        }

        /// <summary>
        /// 发送升级请求。
        /// </summary>
        /// <param name="type">0 进阶石，1 灵力</param>
        /// <param name="auto">0 自动购买道具,1 不自动购买</param>
        public void SendLevelUpRequest(int type, int auto)
        {
            MsgData_cRideLvlUp data = new MsgData_cRideLvlUp();
            data.Type = type;
            data.AutoBuy = auto;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_RIDE_LEVEL_UP, data);
        }

        /// <summary>
        /// 发送更改坐骑请求。
        /// </summary>
        /// <param name="id">坐骑编号。</param>
        public void SendChangeRideRequest(int id)
        {
            MsgData_cChangeRide data = new MsgData_cChangeRide();
            data.ID = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_CHANGE_RIDE, data);
        }
    }
}