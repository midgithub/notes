
/**
* @file     : BagData.cs
* @brief    : 玩家背包数据。
* @details  : 
* @author   : XuXiang
* @date     : 2017-06-29 14:10
*/

using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;
using System;


namespace SG
{
    /// <summary>
    /// 物品信息。
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public class ItemInfo
    {
        public static ItemInfo GetItemInfoClass(int bag)
        {
            return new ItemInfo();
        }

        /// <summary>
        /// 初始化物品信息。
        /// </summary>
        /// <param name="id">物品编号。</param>
        public void Init(int id, int bag, int pos)
        {
            mUID = 0;
            mID = id;
            mCount = 1;
            mBag = bag;
            mPos = pos;
        }

        /// <summary>
        /// 初始化物品信息。
        /// </summary>
        /// <param name="result">服务器下发信息。</param>
        public void Init(MsgData_sItemInfo info)
        {
            mUID = info.UID;
            mID = info.ID;
            mCount = info.Count;
            mBag = info.Bag;
            mPos = info.Pos;
            mUseCount = info.UseCount;
            mTodayUse = info.TodayUse;
            mCanUseVal = info.CanUseVal;
            mFlag = info.Flag;
            SetTimeLimit(info.TimeLimt);
        }

        /// <summary>
        /// 初始化物品信息。
        /// </summary>
        /// <param name="result">服务器下发信息。</param>
        public void Init(MsgData_sItemAdd info)
        {
            mUID = info.UID;
            mID = info.ID;
            mCount = info.Count;
            mBag = info.Bag;
            mPos = info.Pos;
            mUseCount = info.UseCount;
            mTodayUse = info.TodayUse;
            mCanUseVal = info.CanUseVal;
            mFlag = info.Flag;
            SetTimeLimit(info.TimeLimt);
        }

        /// <summary>
        /// 初始化物品信息。
        /// </summary>
        /// <param name="result">服务器下发信息。</param>
        public void Init(MsgData_sItemUpdate info)
        {
            mUID = info.UID;
            mID = info.ID;
            mCount = info.Count;
            mBag = info.Bag;
            mPos = info.Pos;
            mUseCount = info.UseCount;
            mTodayUse = info.TodayUse;
            mCanUseVal = info.CanUseVal;
            mFlag = info.Flag;
            SetTimeLimit(info.TimeLimt);
        }

        /// <summary>
        /// 初始化物品信息。
        /// </summary>
        /// <param name="result">其他玩家装备信息</param>
        public void Init(MsgData_sOtherHumanItemEquipVO info,int pos)
        {
            mID = info.TID;
            mCount = 1;
            mBag = BagType.ITEM_BAG_TYPE_EQUIP;
            mPos = pos;
        }
        public ItemInfo Copy()
        {
            ItemInfo item = new ItemInfo();
            MsgData_sItemUpdate info = new MsgData_sItemUpdate();
            info.UID = mUID;
            info.ID = mID;
            info.Count = mCount;
            info.Bag = mBag;
            info.Pos = mPos;
            info.UseCount = mUseCount;
            info.TodayUse = mTodayUse;
            info.CanUseVal = mCanUseVal;
            info.Flag = mFlag;
            info.TimeLimt = mTimeLimt; 
            item.Init(info);
            item.relativeTime = relativeTime;
            return item;
        }

        /// <summary>
        /// 唯一编号。
        /// </summary>
        private long mUID;

        /// <summary>
        /// 获取唯一编号。
        /// </summary>
        public long UID
        {
            get { return mUID; }
            set { mUID = value; }
        }

        /// <summary>
        /// 物品编号。
        /// </summary>
        private int mID;

        /// <summary>
        /// 获取物品编号。
        /// </summary>
        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }

        /// <summary>
        /// CD剩余时间。
        /// </summary>
        public float CD
        {
            get
            {
                float cd = PlayerData.Instance.BagData.GetCDUseTime(mID) - Time.realtimeSinceStartup;
                return Math.Max(cd, 0);
            }
        }

        /// <summary>
        /// 物品数量。
        /// </summary>
        private int mCount;

        /// <summary>
        /// 获取物品数量。
        /// </summary>
        public int Count
        {
            get { return mCount; }
        }

        /// <summary>
        /// 所属背包。
        /// </summary>
        private int mBag;

        /// <summary>
        /// 获取或设置物品所属背包。
        /// </summary>
        public int Bag
        {
            get { return mBag; }
            set { mBag = value; }
        }

        /// <summary>
        /// 所在格子。
        /// </summary>
        private int mPos;

        /// <summary>
        /// 获取或设置物品所在格子。
        /// </summary>
        public int Pos
        {
            get { return mPos; }
            set { mPos = value; }
        }

        /// <summary>
        /// 使用次数。
        /// </summary>
        private int mUseCount;

        /// <summary>
        /// 获取物品已使用次数。
        /// </summary>
        public int UseCount
        {
            get { return mUseCount; }
        }

        /// <summary>
        /// 今日使用次数。
        /// </summary>
        private int mTodayUse;

        /// <summary>
        /// 获取今日使用次数。
        /// </summary>
        public int TodayUse
        {
            get { return mTodayUse; }
        }

        /// <summary>
        /// 可使用的值。
        /// </summary>
        private long mCanUseVal;

        /// <summary>
        /// 获取可使用的值。
        /// </summary>
        public long CanUseVal
        {
            get { return mCanUseVal; }
        }

        /// <summary>
        /// 物品标志位。第1位:绑定, 第2位:交易绑定
        /// </summary>
        private long mFlag;

        /// <summary>
        /// 获取物品标志位。
        /// </summary>
        public long Flag
        {
            get { return mFlag; }
        }

        private int mTimeLimt;
        public int TimeLimt
        {
            get { return mTimeLimt; }
            set { mTimeLimt = value; }
        }

        //相对时间
        public long relativeTime = 0;
        public void SetTimeLimit(int v)
        {
            mTimeLimt = v;
            relativeTime = UiUtil.GetNowTimeStamp();
        }

        //剩余时间
        public int GetTimeLimit()
        {
            if (mTimeLimt <= 0)
            {
                mTimeLimt = 0;
                return mTimeLimt;
            }
            int time = mTimeLimt - (int)(UiUtil.GetNowTimeStamp() - relativeTime);
            if (time < 0)
                time = 0;
            return time;
        }


        /// <summary>
        /// 获取物品是否绑定。
        /// </summary>
        public bool IsLock
        {
            get { return (mFlag & 1) > 0; }
        }

        /// <summary>
        /// 获取物品是否交易绑定。
        /// </summary>
        public bool IsTradeLock
        {
            get { return (mFlag & 0x2) > 0; }
        }

        /// <summary>
        /// 将数据写入缓冲区。
        /// </summary>
        /// <param name="buffer">数据缓冲区。</param>
        public void Pack(NetWriteBuffer buffer)
        {
            buffer.WriteInt64(mUID);
            buffer.WriteInt32(mID);
            buffer.WriteInt32(mCount);
            buffer.WriteInt32(mBag);
            buffer.WriteInt32(mPos);
            buffer.WriteInt32(mUseCount);
            buffer.WriteInt32(mTodayUse);
            buffer.WriteInt64(mCanUseVal);
            buffer.WriteInt64(mFlag);
        }

        /// <summary>
        /// 从缓冲区中读取数据。
        /// </summary>
        /// <param name="buffer">数据缓冲区。</param>
        public void Uppack(NetReadBuffer buffer)
        {
            mUID = buffer.ReadInt64();
            mID = buffer.ReadInt32();
            mCount = buffer.ReadInt32();
            mBag = buffer.ReadInt32();
            mPos = buffer.ReadInt32();
            mUseCount = buffer.ReadInt32();
            mTodayUse = buffer.ReadInt32();
            mCanUseVal = buffer.ReadInt64();
            mFlag = buffer.ReadInt64();
        }
    }

    /// <summary>
    /// 背包信息。
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public class BagInfo
    { 
        /// <summary>
        /// 初始化背包。
        /// </summary>
        /// <param name="result">服务器下发信息。</param>
        public void Init(MsgData_sQueryItemResult result)
        {
            mID = result.ID;
            mBagType = result.Bag;
            mSize = result.Size;
            OpenLastTime = result.OpenLastTime;
            mItemInfos.Clear();
            for (int i=0; i<result.ItemInfoList.Count; ++i)
            {
                ItemInfo info = ItemInfo.GetItemInfoClass(mBagType);
                info.Init(result.ItemInfoList[i]);
                if (ConfigManager.Instance.BagItem.GetItemConfig(info.ID) != null)
                {
                    if (!mItemInfos.ContainsKey(info.Pos))
                    {
                        mItemInfos.Add(info.Pos, info);
                    }
                    else
                    {
                        LogMgr.LogError("格子已有物品 bag:{0} pos:{1}", info.Bag, info.Pos);
                    }
                }
                else
                {
                    LogMgr.LogError("物品配置不存在 bag:{0} pos:{1} id:{2}", info.Bag, info.Pos, info.ID);
                }
            }
        }

        /// <summary>
        /// 清空背包。
        /// </summary>
        public void Clear()
        {
            mID = 0;
            mBagType = 0;
            mSize = 0;
            mOpenLastTime = 0;
            mRecordTime = 0;
            mItemInfos.Clear();
        }

        /// <summary>
        /// 添加物品。
        /// </summary>
        /// <param name="result">服务器下发信息。</param>
        public void AddItem(MsgData_sItemAdd result)
        {
            ItemInfo info = GetItemInfo(result.Pos);
            if (info != null)
            {
                LogMgr.ErrorLog("物品添加失败，该格子已有物品 bag:{0} pos:{1}", info.Bag, info.Pos);
                return;
            }

            //加入物品
            info = ItemInfo.GetItemInfoClass(result.Bag);
            mItemInfos.Add(result.Pos, info);
            info.Init(result);
        }

        /// <summary>
        /// 添加物品。
        /// </summary>
        /// <param name="item">物品信息。</param>
        public void AddItem(ItemInfo item)
        {
            ItemInfo info = GetItemInfo(item.Pos);
            if (info != null)
            {
                LogMgr.ErrorLog("物品添加失败，该格子已有物品 bag:{0} pos:{1}", info.Bag, info.Pos);
                return;
            }

            //加入物品
            mItemInfos.Add(item.Pos, item);
        }

        /// <summary>
        /// 删除物品。
        /// </summary>
        /// <param name="pos">物品格子位置。</param>
        public void DeleteItem(int pos)
        {
            mItemInfos.Remove(pos);
        }

        /// <summary>
        /// 更新物品。
        /// </summary>
        /// <param name="result">服务器下发信息。</param>
        public void UpdateItem(MsgData_sItemUpdate result)
        {
            ItemInfo info = GetItemInfo(result.Pos);
            if (info == null)
            {
                LogMgr.WarningLog("该格子原本并没有物品 bag:{0} pos:{1}", result.Bag, result.Pos);
            }

            //加入物品
            if (info == null)
            {
                info = ItemInfo.GetItemInfoClass(result.Bag);
                mItemInfos.Add(result.Pos, info);
            }
            info.Init(result);
            if (info.ID == 0 || info.Count == 0)
            {
                mItemInfos.Remove(info.Pos);
            }
        }

        /// <summary>
        /// 物品编号。
        /// </summary>
        private long mID;

        /// <summary>
        /// 物品编号。
        /// </summary>
        public long ID
        {
            get { return mID; }
        }

        /// <summary>
        /// 背包类型。
        /// </summary>
        private int mBagType;

        /// <summary>
        /// 获取背包类型。
        /// </summary>
        public int BagType
        {
            get { return mBagType; }
        }

        /// <summary>
        /// 背包格子数。
        /// </summary>
        private int mSize;

        /// <summary>
        /// 获取或设置背包格子数。
        /// </summary>
        public int Size
        {
            get { return mSize; }
            set { mSize = value; }
        }

        /// <summary>
        /// 开启下一个格子的剩余时间。
        /// </summary>
        private int mOpenLastTime;

        /// <summary>
        /// 记录mOpenLastTime时的时间戳
        /// </summary>
        private float mRecordTime;

        /// <summary>
        /// 获取开启下一个格子的剩余时间。
        /// </summary>
        public int OpenLastTime
        {
            get
            {
                int gap = (int)(Time.realtimeSinceStartup - mRecordTime);
                int t = Math.Max(0, mOpenLastTime - gap);
                return t;
            }
            set
            {
                mOpenLastTime = value;
                mRecordTime = Time.realtimeSinceStartup;
            }
        }

        /// <summary>
        /// 格子对应的物品信息。
        /// </summary>
        private Dictionary<int, ItemInfo> mItemInfos = new Dictionary<int, ItemInfo>();

        /// <summary>
        /// 获取格子对应的物品信息。
        /// </summary>
        public Dictionary<int, ItemInfo> ItemInfos
        {
            get { return mItemInfos; }
        }

        /// <summary>
        /// 通过格子序号获取物品信息。
        /// </summary>
        /// <param name="pos">格子位置，从0开始。</param>
        /// <returns>物品信息。</returns>
        public ItemInfo GetItemInfo(int pos)
        {
            ItemInfo info; 
            mItemInfos.TryGetValue(pos, out info);
            return info;
        }

        // <summary>
        /// 通过格子序号获取物品编号。
        /// </summary>
        /// <param name="pos">格子位置，从1开始。</param>
        /// <returns>物品编号。</returns>
        public int GetItemID(int pos)
        {
            ItemInfo info;
            if (mItemInfos.TryGetValue(pos, out info))
            {
                return info.ID;
            }
            return 0;
        }

        /// <summary>
        /// 获取指定物品数量。
        /// </summary>
        /// <param name="id">物品编号。</param>
        /// <returns>物品数量。</returns>
        public int GetItemNumber(int id)
        {
            int num = 0;
            foreach (KeyValuePair<int, ItemInfo> key in mItemInfos)
            {
                if (key.Value.ID == id)
                {
                    num += key.Value.Count;
                }
            }
            return num;
        }
        /// <summary>
        /// 获取指定物品
        /// </summary>
        /// <param name="id">物品编号。</param>
        /// <returns>物品数量。</returns>
        /// 
        public ItemInfo GetItemInfoById(int itemid)
        {
            ItemInfo item = null;
            foreach (KeyValuePair<int, ItemInfo> key in mItemInfos)
            {
                if (key.Value.ID == itemid)
                {
                    item = key.Value;
                }
            }
            return item;
        }

        /// <summary>
        /// 通过物品ID，获取物品列表
        /// </summary>
        public List<ItemInfo> GetItemListByID(int iItemID)
        {
            List<ItemInfo> list = new List<ItemInfo>();
            foreach(var v in mItemInfos.Values)
            {
                if (v.ID == iItemID) list.Add(v);
            }
            return list;
        }


        /// <summary>
        /// 判断背包是否还有空格子。
        /// </summary>
        /// <returns>是否还有空格子。</returns>
        public bool IsHaveEmptyGrid()
        {
            return ItemInfos.Count < mSize;
        }

        /// <summary>
        /// 获取空格子数量。
        /// </summary>
        /// <returns>空格子数量。</returns>
        public int GetEmptyNumber()
        {
            return mSize - ItemInfos.Count;
        }

        /// <summary>
        /// 获取指定类型的物品列表。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        /// <returns>物品列表。</returns>
        public List<ItemInfo> GetItemByType(int type)
        {
            List<ItemInfo> iteminfos = new List<ItemInfo>();
            foreach (var kvp in ItemInfos)
            {
                if (GetItemType(kvp.Value.ID) == type)
                {
                    iteminfos.Add(kvp.Value);
                }
            }
            return null;
        }

        public int GetItemType(int id)
        {
            return 0;
        }

        /// <summary>
        /// 判断背包是否可放入物品。
        /// </summary>
        /// <param name="id">物品编号。</param>
        /// <param name="num">物品数量。</param>
        /// <returns>是否可放入。</returns>
        public bool IsCanPut(int id, int num)
        {
            LuaTable cfg = ConfigManager.Instance.BagItem.GetItemConfig(id);
            if (cfg == null)
            {
                return true;
            }

            //先判断空格子是否足够
            int repeats = cfg.Get<int>("main") == 5 ? 1 : Math.Max(1, cfg.Get<int>("repeats"));
            int empty = GetEmptyNumber();
            int last = num - empty * repeats;
            if (last <= 0)
            {
                return true;
            }

            //如果是可堆叠物品再判断现有格子是否还塞得下
            if (repeats > 1)
            {
                var e = mItemInfos.GetEnumerator();
                while (e.MoveNext() && last > 0)
                {
                    var cur = e.Current;
                    ItemInfo item = cur.Value;
                    if (item.ID == id)
                    {
                        last -= repeats - item.Count;
                    }
                }
                e.Dispose();
            }

            return last <= 0;
        }
    }

    /// <summary>
    /// 背包数据。
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public class BagData
    {
        public static BagData Instance
        {
            get
            {
                return PlayerData.Instance.BagData;
            }
        }


        /// <summary>
        /// 背包列表。
        /// </summary>
        private BagInfo[] mBags = new BagInfo[BagType.ITEM_BAG_TYPE_NUM];

        /// <summary>
        /// 物品集合。
        /// </summary>
        private Dictionary<long, ItemInfo> mItems = new Dictionary<long, ItemInfo>();

        /// <summary>
        /// 物品移动背包时，走先Add再Del，在处理Add时会出现UID重复，为了保证mItems正确，先缓存UID，在Del时再取消。
        /// </summary>
        private long mCacheAddUID = 0;

        /// <summary>
        /// 物品最后使用到时间。
        /// </summary>
        private Dictionary<int, float> m_UseTime = new Dictionary<int, float>();

        /// <summary>
        /// CD组物品最后使用到时间。
        /// </summary>
        private Dictionary<int, float> m_CDGroupUseTime = new Dictionary<int, float>();
        
        //获取所有物品
        public Dictionary<long, ItemInfo> GetItems()
        {
            return mItems;
        }
        
        /// <summary>
        /// 构造函数。
        /// </summary>
        public BagData()
        {
            for (int i=0; i< mBags.Length; ++i)
            {
                mBags[i] = new BagInfo();
            }
        }

        /// <summary>
        /// 注册网络消息。
        /// </summary>
        public void RegisterNetMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, OnCleanupUserData);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ITEM_ADD, OnItemAdd);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ITEM_DEL, OnItemDel);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ITEM_UPDATE, OnItemUpdate);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_QUERY_ITEM_RESULT, OnBagInfo);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DISCARD_ITEM_RESULT, OnDiscardItem);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_PACK_ITEM_RESULT, OnPackBag);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EXPAND_BAG_RESULT, OnExpandBag);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EXPAND_BAG_TIPS, OnExpandBagTips);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SWAP_ITEM_RESULT, OnSwapItem);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SELL_ITEM_RESULT, OnSellItem);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_USE_ITEM_RESULT, OnUseItem);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SPLIT_ITEM_RESULT, OnSplitItem);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FAST_SELL_ITEM_RESULT, OnFastSellItem);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_MoShenItemResult, OnMoShenItemResult);
        }

        /// <summary>
        ///  切换账号清除数据
        /// </summary>
        private void OnCleanupUserData(GameEvent ge, EventParameter parameter)
        {
            for (int i=0; i< mBags.Length; ++i)
            {
                mBags[i].Clear();
            }
            mItems.Clear();
            mCacheAddUID = 0;
            m_UseTime.Clear();
            m_CDGroupUseTime.Clear();
        }

        /// <summary>
        /// 触发背包信息更新事件。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        public static void TriggerEventBagInfo(int bag)
        {
            EventParameter ep = EventParameter.Get();
            ep.intParameter = bag;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BAG_INFO, ep);
        }

        /// <summary>
        /// 触发添加物品事件。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        /// <param name="pos">物品位置。</param>
        public static void TriggerEventItemAdd(int bag, int pos)
        {
            EventParameter ep = EventParameter.Get();
            ep.intParameter = bag;
            ep.intParameter1 = pos;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BAG_ITEM_ADD, ep);
        }

        /// <summary>
        /// 触发删除物品事件。
        /// </summary>
        /// <param name="type">事件类型 1.删除，  2.物品交换</param>
        /// <param name="bag">背包类型。</param>
        /// <param name="pos">物品位置。</param>
        public static void TriggerEventItemDel(int type,int bag, int pos, long uid)
        {
            EventParameter ep = EventParameter.Get();
            ep.intParameter = bag;
            ep.intParameter1 = pos;
            ep.intParameter2 = type;
            ep.objParameter = uid;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BAG_ITEM_DEL, ep);
        }

        /// <summary>
        /// 触发更新物品事件。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        /// <param name="pos">物品位置。</param>
        public static void TriggerEventItemUpdate(int bag, int pos)
        {
            EventParameter ep = EventParameter.Get();
            ep.intParameter = bag;
            ep.intParameter1 = pos;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BAG_ITEM_UPDATE, ep);
        }

        /// <summary>
        /// 触发更背包扩充事件。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        /// <param name="osize">原来大小。</param>
        /// <param name="nsize">新的大小。</param>
        public static void TriggerEventBagSize(int bag, int osize, int nsize)
        {
            EventParameter ep = EventParameter.Get();
            ep.intParameter = bag;
            ep.intParameter1 = osize;
            ep.intParameter2 = nsize;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BAG_SIZE, ep);
        }

        /// <summary>
        /// 获取背包信息。
        /// </summary>
        /// <param name="bag">背包类型，参考枚举BagType。</param>
        /// <returns>背包信息。</returns>
        public BagInfo GetBagInfo(int bag)
        {
            if (bag < 0 || bag >= mBags.Length)
            {
                return null;
            }
            return mBags[bag];
        }

        /// <summary>
        /// 获取物品信息。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        /// <param name="pos">物品位置。</param>
        /// <returns>物品信息。</returns>
        public ItemInfo GetItemInfo(int bag, int pos)
        {
            BagInfo baginfo = GetBagInfo(bag);
            return baginfo != null ? baginfo.GetItemInfo(pos) : null;
        }


        /// <summary>
        /// 获取某个背包内物品数量。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        /// <param name="id">物品编号。</param>
        /// <returns>物品数量。</returns>
        public int GetItemNumber(int bag, int id)
        {
            BagInfo baginfo = GetBagInfo(bag);
            return baginfo != null ? baginfo.GetItemNumber(id) : 0;
        }

        public ItemInfo GetItemInfoById(int bag, int itemid)
        {
            BagInfo baginfo = GetBagInfo(bag);
            return baginfo != null ? baginfo.GetItemInfoById(itemid) : null;
        }

        public List<ItemInfo> GetItemListByID(int iBagType, int itemid)
        {
            BagInfo bag = GetBagInfo(iBagType);
            return bag != null ? bag.GetItemListByID(itemid) : null;
        }

        /// <summary>
        /// 通过UI获取物品数据。
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ItemInfo GetItemInfoByUID(long uid)
        {
            ItemInfo info;
            mItems.TryGetValue(uid, out info);
            return info;
        }

        /// <summary>
        /// 获取指定类型的物品列表。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        /// <param name="type">物品类型。</param>
        /// <returns>物品列表。</returns>
        public List<ItemInfo> GetItemByType(int bag, int type)
        {
            BagInfo baginfo = GetBagInfo(bag);
            return baginfo != null ? baginfo.GetItemByType(type) : null;
        }

        /// <summary>
        /// 获取物品CD的使用时间。
        /// </summary>
        /// <param name="id">物品编号。</param>
        /// <returns>最近使用时间。</returns>
        public float GetCDUseTime(int id)
        {
            float time = 0;
            m_UseTime.TryGetValue(id, out time);
            LuaTable t = ConfigManager.Instance.BagItem.GetItemConfig(id);
            if (t != null)
            {
                int groupcd = t.Get<int>("groupcd");
                float gtime = 0;
                m_CDGroupUseTime.TryGetValue(groupcd, out gtime);
                time = Math.Max(time, gtime);
            }
            return time;
        }

        /// <summary>
        /// 获取装备名称。
        /// </summary>
        /// <param name="pos">装备部位。</param>
        /// <returns>装备名称。</returns>
        public string GetEquipName(int pos)
        {
            ItemInfo iteminfo = GetItemInfo(BagType.ITEM_BAG_TYPE_EQUIP, pos);
            if (iteminfo == null)
            {
                return string.Empty;
            }

            return ConfigManager.Instance.BagItem.GetItemName(iteminfo.ID);
        }

        /// <summary>
        /// 显示装备改变效果。
        /// </summary>
        /// <param name="id">装备ID。</param>
        public void ShowEquipChange(int id)
        {
            LuaTable cfg = ConfigManager.Instance.BagItem.GetItemConfig(id);
            if (cfg == null)
            {
                return;
            }

            //判断装备位置
            int pos = cfg.Get<int>("pos");
            if (pos <= 10)
            {
                PlayerObj player = CoreEntry.gActorMgr.MainPlayer;
                if (player != null)
                {
                    player.ShowEquipChange();
                }
            }
        }

        /// <summary>
        /// 判断背包是否可放入物品。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        /// <param name="id">物品编号。</param>
        /// <param name="num">物品数量。</param>
        /// <returns>是否可放入。</returns>
        public bool IsCanPut(int bag, int id, int num)
        {
            BagInfo baginfo = GetBagInfo(bag);
            return baginfo != null && baginfo.IsCanPut(id, num);
        }

        /// <summary>
        /// 添加物品。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnItemAdd(MsgData data)
        {
            MsgData_sItemAdd info = data as MsgData_sItemAdd;
            
            BagInfo bag = GetBagInfo(info.Bag);
            if (bag != null)
            {
                bag.AddItem(info);
                if (mItems.ContainsKey(info.UID))
                {
                    //额外多Add了一次，下次Del时抵消
                    mCacheAddUID = info.UID;
                    mItems.Remove(mCacheAddUID);
                }
                mItems.Add(info.UID, bag.GetItemInfo(info.Pos));
                TriggerEventItemAdd(info.Bag, info.Pos);

                if (info.Bag == BagType.ITEM_BAG_TYPE_EQUIP)
                {
                    ShowEquipChange(info.ID);
                }
            }
            else
            {
                LogMgr.ErrorLog("未知背包编号 id:{0}", info.Bag);
            }
        }

        /// <summary>
        /// 删除物品。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnItemDel(MsgData data)
        {
            MsgData_sItemDel info = data as MsgData_sItemDel;
            BagInfo bag = GetBagInfo(info.Bag);
            if (bag != null)
            {
                ItemInfo iteminfo = bag.GetItemInfo(info.Pos);
                if (mCacheAddUID == iteminfo.UID)
                {
                    //抵消前一个Add
                    mCacheAddUID = 0;
                }
                else
                {
                    mItems.Remove(iteminfo.UID);
                }                
                bag.DeleteItem(info.Pos);
                TriggerEventItemDel(1,info.Bag, info.Pos, iteminfo.UID);
            }
            else
            {
                LogMgr.ErrorLog("未知背包编号 id:{0}", info.Bag);
            }
        }

        /// <summary>
        /// 更新物品。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnItemUpdate(MsgData data)
        {
            MsgData_sItemUpdate info = data as MsgData_sItemUpdate;
            BagInfo bag = GetBagInfo(info.Bag);
            if (bag != null)
            {
                ItemInfo iteminfo = bag.GetItemInfo(info.Pos);
				if (iteminfo == null)
                    return;
                bool newuid = iteminfo.UID != info.UID;
                if (newuid)
                {
                    mItems.Remove(iteminfo.UID);
                }
                bag.UpdateItem(info);
                if (newuid)
                {
                    mItems.Add(info.UID, bag.GetItemInfo(info.Pos));
                }                
                TriggerEventItemUpdate(info.Bag, info.Pos);

                if (info.Bag == BagType.ITEM_BAG_TYPE_EQUIP)
                {
                    ShowEquipChange(info.ID);
                }
            }
            else
            {
                LogMgr.ErrorLog("未知背包编号 id:{0}", info.Bag);
            }
        }

        /// <summary>
        /// 初始化背包信息。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnBagInfo(MsgData data)
        {
            MsgData_sQueryItemResult info = data as MsgData_sQueryItemResult;
            BagInfo bag = GetBagInfo(info.Bag);
            if (bag != null)
            {
                //从mItems删除原来背包的东西
                foreach (var kvp in bag.ItemInfos)
                {
                    mItems.Remove(kvp.Value.UID);
                }
                bag.Init(info);
                //重新添加
                foreach (var kvp in bag.ItemInfos)
                {
                    mItems.Add(kvp.Value.UID, kvp.Value);
                }
                TriggerEventBagInfo(bag.BagType);
            }
            else
            {
                LogMgr.ErrorLog("未知背包编号 id:{0}", info.Bag);
            }
        }

        /// <summary>
        /// 背包整理回复。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnPackBag(MsgData data)
        {
            MsgData_sPackItemResult info = data as MsgData_sPackItemResult;
            LogMgr.Log("背包整理回复 bag:{0} result:{1}", info.BagType, info.Result);
            if (info.Result == 0)
            {
                //背包整理成功
            }
        }

        /// <summary>
        /// 丢弃物品回复。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnDiscardItem(MsgData data)
        {
            MsgData_sDiscardItemResult info = data as MsgData_sDiscardItemResult;
            LogMgr.Log("丢弃回复 bag:{0} pos:{1} result:{2}", info.Bag, info.Pos, info.Result);
            if (info.Result == 0)
            {
                //丢弃物品成功
            }
        }

        /// <summary>
        /// 背包扩充提示。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnExpandBagTips(MsgData data)
        {
            MsgData_sExpandBagTips info = data as MsgData_sExpandBagTips;
            LogMgr.Log("背包扩充提示 bag:{0}", info.Bag);
            BagInfo bag = GetBagInfo(info.Bag);
            if(bag!=null)
            SendExpandBagRequest(info.Bag, bag.Size + 1, 0);
        }

        /// <summary>
        /// 背包扩充回复。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnExpandBag(MsgData data)
        {
            MsgData_sExpandBagResult info = data as MsgData_sExpandBagResult;
            LogMgr.Log("背包扩充回复 bag:{0} size:{1} result:{2}", info.Bag, info.NewSize, info.Result);
            if (info.Result == 0)
            {
                //背包扩充成功                
                BagInfo bag = GetBagInfo(info.Bag);
                if(bag!=null)
                {
                    int oldsize = bag.Size;
                    bag.Size = info.NewSize;
                    bag.OpenLastTime = ConfigManager.Instance.BagItem.GetOpenNeedTime(info.Bag, info.NewSize);
                    TriggerEventBagSize(bag.BagType, oldsize, info.NewSize);
                    //只提示通用背包开启
                    if (info.Bag == BagType.ITEM_BAG_TYPE_COMMON)
                    {
                        string msg = string.Format("成功开启{0}格背包！", info.NewSize - oldsize);
                        UITips.ShowTips(msg);
                    }
                }
            }
        }
         
        private void OnMoShenItemResult(MsgData data)
        {
            MsgData_sMoShenItemResult info = data as MsgData_sMoShenItemResult;
            if (info.Result == 0)
            {
                for(int i = 0; i < info.ResultList.Count;i++)
                { 
                    //交换物品成功 先将物品从背包移除，再放入对方背包
                    BagInfo srcbag = GetBagInfo(info.ResultList[i].SrcBag);
                    BagInfo dstbag = GetBagInfo(info.ResultList[i].DstBag);
                    if (dstbag == null)
                    {
                        return;
                    }
                    if (srcbag == null)
                    {
                        return;
                    }
                    ItemInfo srciteminfo = srcbag.GetItemInfo(info.ResultList[i].SrcPos);
                    ItemInfo dstiteminfo = dstbag.GetItemInfo(info.ResultList[i].DstPos);

                    srcbag.DeleteItem(info.ResultList[i].SrcPos);
                    dstbag.DeleteItem(info.ResultList[i].DstPos);
                    if (srciteminfo != null)
                    {
                        srciteminfo.Bag = info.ResultList[i].DstBag;
                        srciteminfo.Pos = info.ResultList[i].DstPos;
                        dstbag.AddItem(srciteminfo); 
                    }
                    if (dstiteminfo != null)
                    {
                        dstiteminfo.Bag = info.ResultList[i].SrcBag;
                        dstiteminfo.Pos = info.ResultList[i].SrcPos;
                        srcbag.AddItem(dstiteminfo);
                    }
                }
            }
            EventParameter par = EventParameter.Get();
            par.intParameter = info.Result;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_MOSHEN_BAGCHANGEOVER, par);
        }
        /// <summary>
        /// 交换物品回复。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnSwapItem(MsgData data)
        {
            MsgData_sSwapItemResult info = data as MsgData_sSwapItemResult;
            LogMgr.Log("交换物品回复 srcbag:{0} desbag:{1} srcpos:{2} dstpos:{3}", info.SrcBag, info.DstBag, info.SrcPos, info.DstPos);
            if (info.Result == 0)
            {
                //交换物品成功 先将物品从背包移除，再放入对方背包
                BagInfo srcbag = GetBagInfo(info.SrcBag);                
                BagInfo dstbag = GetBagInfo(info.DstBag);
                if(dstbag==null)
                {
                    return;
                }
                if (srcbag == null)
                {
                    return;
                }
                ItemInfo srciteminfo = srcbag.GetItemInfo(info.SrcPos);                
                ItemInfo dstiteminfo = dstbag.GetItemInfo(info.DstPos);

                srcbag.DeleteItem(info.SrcPos);
                dstbag.DeleteItem(info.DstPos);

                if((info.DstBag == (int)BagType.ITEM_BAG_TYPE_EQUIP  || info.DstBag == (int)BagType.ITEM_BAG_TYPE_RIDE) && srciteminfo != null)
                {
                    SG.CoreEntry.gAudioMgr.PlayUISound(900011);
                }
                if (srciteminfo != null)
                {
                    srciteminfo.Bag = info.DstBag;
                    srciteminfo.Pos = info.DstPos;
                    dstbag.AddItem(srciteminfo);
                    if (dstbag.BagType == BagType.ITEM_BAG_TYPE_EQUIP)
                    {
                        ShowEquipChange(srciteminfo.ID);
                    }
                }
                if (dstiteminfo != null)
                {
                    dstiteminfo.Bag = info.SrcBag;
                    dstiteminfo.Pos = info.SrcPos;
                    srcbag.AddItem(dstiteminfo);
                }

                if (srciteminfo != null && dstiteminfo != null)
                {
                    //两者不为空，两边更新
                    TriggerEventItemUpdate(info.SrcBag, info.SrcPos);
                    TriggerEventItemUpdate(info.DstBag, info.DstPos);
                }
                else if (srciteminfo != null)
                {
                    //源背包删除 目标背包添加
                    TriggerEventItemDel(2,info.SrcBag, info.SrcPos, srciteminfo.UID);
                    TriggerEventItemAdd(info.DstBag, info.DstPos);
                }
                else if (dstiteminfo != null)
                {
                    //源背包添加 目标背包删除
                    TriggerEventItemAdd(info.SrcBag, info.SrcPos);
                    TriggerEventItemDel(2,info.DstBag, info.DstPos, dstiteminfo.UID);
                }                      
            }
            EventParameter par = EventParameter.Get(data);
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_BAGCHANGEOVER, par);
        }

        /// <summary>
        /// 出售物品回复。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnSellItem(MsgData data)
        {
            MsgData_sSellItemResult info = data as MsgData_sSellItemResult;
            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BAG_SELLITEM_RESULT, ep);
            LogMgr.Log("出售物品回复 result:{0} uid:{1} id:{2} count:{3} flag:{4}", info.Result, info.UID, info.ID, info.Count, info.Flag);
            if (info.Result == 0)
            {
                //出售物品成功
            }
        }

        /// <summary>
        /// 使用物品回复。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnUseItem(MsgData data)
        {
            MsgData_sUseItemResult info = data as MsgData_sUseItemResult;
            if (info.Result == 0)
            {
                //LogMgr.LogError("物品使用成功 id:{0}", info.ID);
                LuaTable t = ConfigManager.Instance.BagItem.GetItemConfig(info.ID);
                if (t != null)
                {
                    float cd = t.Get<int>("cd") * 0.001f;
                    int groupcd = t.Get<int>("groupcd");
                    if (cd > 0)
                    {
                        m_UseTime[info.ID] = Time.realtimeSinceStartup + cd;
                    }
                    if (groupcd > 0)
                    {
                        m_CDGroupUseTime[groupcd] = Time.realtimeSinceStartup + cd;
                    }                    
                }
            }

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            ep.intParameter1 = info.ID;
            ep.longParameter = info.Count;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BAG_USE_ITEM, ep);
        }

        /// <summary>
        /// 拆分物品回复。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnSplitItem(MsgData data)
        {
            MsgData_sSplitItemResult info = data as MsgData_sSplitItemResult;
            LogMgr.Log("拆分物品回复 result:{0}", info.Result);
            if (info.Result == 0)
            {
                //使用物品成功
            }
        }

        /// <summary>
        /// 批量出售物品回复。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnFastSellItem(MsgData data)
        {
            MsgData_sSellItemListResult info = data as MsgData_sSellItemListResult;

            EventParameter ep = EventParameter.Get();
            ep.objParameter = info;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BAG_FAST_SELL, ep);
        }
        
        /// <summary>
        /// 发送背包物品查询请求。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        public void SendQueryItemRequest(int bag)
        {
            MsgData_cQueryItem data = new MsgData_cQueryItem();
            data.RoleID = PlayerData.Instance.RoleID;
            data.BagType = bag;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_QUERY_ITEM, data);
        }

        /// <summary>
        /// 发送整理背包请求。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        public void SendPackItemRequest(int bag)
        {
            MsgData_cPackItem data = new MsgData_cPackItem();
            data.BagType = bag;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_PACK_ITEM, data);
        }

        /// <summary>
        /// 发送丢弃物品请求。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        /// <param name=")">格子序号。</param>
        public void SendDiscardItemRequest(int bag, int pos)
        {
            MsgData_cDiscardItem data = new MsgData_cDiscardItem();
            data.Bag = bag;
            data.Pos = bag;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DISCARD_ITEM, data);
        }

        /// <summary>
        /// 发送背包扩展请求。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        /// <param name="size">新格子数。</param>
        /// <param name="mt">物品不足时:0不扩展 1消耗元宝(优先绑定)</param>
        public void SendExpandBagRequest(int bag, int size, int mt)
        {
            BagInfo baginfo = GetBagInfo(bag);
            if(baginfo!=null)
            {
                MsgData_cExpandBag data = new MsgData_cExpandBag();
                data.Bag = bag;
                data.NewSize = size - baginfo.Size;
                data.MoneyType = mt;
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_EXPAND_BAG, data);
            }
        }

        /// <summary>
        /// 发送交换物品请求。
        /// </summary>
        /// <param name="srcbag">源背包。</param>
        /// <param name="srcpos">源位置。</param>
        /// <param name="dstbag">目标背包。</param>
        /// <param name="dstpos">目标位置<。/param>
        public void SendSwapItemRequest(int srcbag, int srcpos, int dstbag, int dstpos)
        {
            MsgData_cSwapItem data = new MsgData_cSwapItem();
            data.SrcBag = srcbag;
            data.SrcPos = srcpos;
            data.DstBag = dstbag;
            data.DstPos = dstpos;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_SWAP_ITEM, data);
        }

        /// <summary>
        /// 发送出售物品请求。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        /// <param name=")">格子序号。</param>
        public void SendSellItemRequest(int bag, int pos)
        {
            MsgData_cSellItem data = new MsgData_cSellItem();
            data.Bag = bag;
            data.Pos = pos;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_SELL_ITEM, data);
        }

        /// <summary>
        /// 发送使用物品请求。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        /// <param name="pos">格子序号。</param>
        /// <param name="num">使用数量。</param>
        /// <param name="xzid">礼包中物品ID。</param>
        public void SendUseItemRequest(int bag, int pos, int num, int xzid=0)
        {
            MsgData_cUseItem data = new MsgData_cUseItem();
            data.Bag = bag;
            data.Pos = pos;
            data.Count = num;
            data.XuanZiID = xzid;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_USE_ITEM, data);
        }

        /// <summary>
        /// 发送拆分物品请求。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        /// <param name="pos">格子序号。</param>
        /// <param name="num">使用数量。</param>
        public void SendSplitItemRequest(int bag, int pos, int num)
        {
            MsgData_cSplitItem data = new MsgData_cSplitItem();
            data.Bag = bag;
            data.Pos = pos;
            data.Count = num;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_SPLIT_ITEM, data);
        }

        /// <summary>
        /// 发送一键出售请求。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        /// <param name="pos">格子索引列表</param>
        public void SendFastSellRequest(int bag, List<int> pos)
        {
            if (pos.Count <= 0)
            {
                return;
            }

            MsgData_cSellItemList data = new MsgData_cSellItemList();
            data.Bag = bag;
            data.Count = pos.Count;
            data.PosList = new List<int>();
            data.PosList.AddRange(pos);
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_FAST_SELL_ITEM, data);
        }

        /// <summary>
        /// 发送一键出售请求。(给Lua调用)
        /// </summary>
        /// <param name="bag">背包类型。</param>
        /// <param name="pos">格子索引列表</param>
        public void SendFastSellRequestForLua(int bag, LuaTable tb)
        {
            List<int> pos = tb.Cast<List<int>>();
            SendFastSellRequest(bag, pos);
        }
    }
}