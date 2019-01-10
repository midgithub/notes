using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using XLua;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class TreasureMgr
    {
        private static TreasureMgr instance = null;
        public static TreasureMgr Instance
        {
            get
            {
                if (null == instance)
                    instance = new TreasureMgr();

                return instance;
            }
        }
        /// <summary>
        /// 珍宝阁msg , key - 珍宝id,  value - msg
        /// </summary>
        Dictionary<int, MsgData_sZhenBaoGeList> list = new Dictionary<int, MsgData_sZhenBaoGeList>();

        /// <summary>
        /// 珍宝地图对应的珍宝id表
        /// </summary>
        Dictionary<int, List<int>> mapIdsList = new Dictionary<int, List<int>>();

        /// <summary>
        /// 珍宝地图配置表
        /// </summary>
        public static Dictionary<int, LuaTable> CacheExtConfig = null;

        /// <summary>
        /// 珍宝配置表
        /// </summary>
        public static Dictionary<int, LuaTable> CacheZhenbaoExtConfig = null;

        /// <summary>
        /// 获取战斗力。
        /// </summary>
        public long Power;

        /// <summary>
        /// 获取当前珍宝阁的server配置信息
        /// </summary>
        public MsgData_sZhenBaoGeList GetCurMsg(int id)
        {
            if(list.ContainsKey(id))
            {
                return list[id];
            }
            return null;
        }

        /// <summary>
        /// 获取当前珍宝地图的 所有珍宝id
        /// </summary>
        /// <param name="zhenbaoMapId"></param>
        /// <returns></returns>
        public List<int> GetCurMapInfo(int zhenbaoMapId)
        {
            if(mapIdsList.Count == 0) 
            {
                LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
                CacheExtConfig = G.Get<Dictionary<int, LuaTable>>("t_zhenbaoMap");
                foreach (var item in CacheExtConfig)
                {
                    LuaTable tt = item.Value;
                    int id = tt.Get<int>("id");
                    string zhenbaoIdStr = tt.Get<string>("zhenbaoID");
                    List<string> arr = new List<string>(zhenbaoIdStr.Split('#'));
                    List<int> ids = new List<int>();
                    foreach (var arrItem in arr)
                    {
                        int _id = Convert.ToInt32(arrItem);
                        if (!ids.Contains(_id))
                        {
                            ids.Add(_id);
                        }
                    }
                    mapIdsList[id] = ids;
                }
            }
           if(mapIdsList.ContainsKey(zhenbaoMapId))
            {
                return mapIdsList[zhenbaoMapId];
            }
            return null;
        }

        public TreasureMgr()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_ZhenBaoGe, OnZhenBaoGe);
            CacheZhenbaoExtConfig = LuaMgr.Instance.GetLuaEnv().Global.Get<Dictionary<int, LuaTable>>("t_zhenbao");
        }

        /// <summary>
        /// 是否有可提交的道具
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public bool IsHaveSubmitItem(int mapId)
        {
            List<int> treasureIds = GetCurMapInfo(mapId);
            foreach (var item in treasureIds)
            {
                LuaTable l;
                if (CacheZhenbaoExtConfig.TryGetValue(item, out l))
                {
                    // int groupId = t.Get<int>("pskillgroup");
                    int imgId = l.Get<int>("imgUrl");
                    int maxCount = l.Get<int>("maxSubmit");
                    int has = PlayerData.Instance.BagData.GetItemNumber(BagType.ITEM_BAG_TYPE_COMMON, imgId);
                    MsgData_sZhenBaoGeList cur = GetCurMsg(item);
                    if (cur != null)
                    {
                        int sendCount = GetCurMsg(item).submitNum;
                        if (sendCount < maxCount && has > 0)
                        {
                            return true;
                        }
                    }                
                }
            }
            return false;
        }


        /// <summary>
        /// 一键提交当前地图珍宝阁道具
        /// </summary>
        /// <param name="b"></param>
        public void Send_CS_ZhenBaoGeSubmit(int mapId)
        {
            MsgData_cZhenBaoGeSubmit rsp = new MsgData_cZhenBaoGeSubmit();
            List<MsgData_cZhenBaoGeSubmitList> idList = new List<MsgData_cZhenBaoGeSubmitList>();
            List<int> treasureIds = GetCurMapInfo(mapId);
            uint _count = 0;
            if (treasureIds!=null)
            {
                foreach (var item in treasureIds)
                {
                    MsgData_cZhenBaoGeSubmitList tt = new MsgData_cZhenBaoGeSubmitList();
                    tt.id = item;
                    idList.Add(tt);
                }
                _count = (uint)treasureIds.Count;
            }

            rsp.count = _count;
            rsp.list = idList;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ZhenBaoGeSubmit, rsp);
        }

        public void Send_CS_ZhenBaoGeSpeItem(int id,int itemId)
        {
            MsgData_cZhenBaoGeSpeItem rsp = new MsgData_cZhenBaoGeSpeItem();
            rsp.id = id;
            rsp.itemId = itemId;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ZhenBaoGeSpeItem, rsp);
        }

        void OnZhenBaoGe(GameEvent ge, EventParameter parameter)
        {
            MsgData_sZhenBaoGe resp = parameter.msgParameter as MsgData_sZhenBaoGe;
            for (int i = 0; i < resp.list.Count; i++)
            {
                MsgData_sZhenBaoGeList tt = resp.list[i];
                list[tt.id] = tt;
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_ZhenBaoGeUpdate, EventParameter.Get(resp));
        }

    }
}

