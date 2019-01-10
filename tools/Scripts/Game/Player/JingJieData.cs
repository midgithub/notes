/**
* @file     : JingJieData
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-07-18
*/
using UnityEngine;
using System.Collections.Generic;
using XLua;

namespace SG
{
    [LuaCallCSharp]
    public enum JingJieType
    {
        ATTACK = 1,
        Defense = 2,
        SUPPORT = 3,
        MAX = 4,
    }

    [LuaCallCSharp]
    [Hotfix]
    public class JingJieData
    {
        private Dictionary<int, int> mData = new Dictionary<int, int>();
        public Dictionary<int, int> Data
        {
            get { return mData; }
        }

        private Dictionary<JingJieType, int> mUsedPoint = new Dictionary<JingJieType, int>();
        private Dictionary<JingJieType, int> mTotalPoint = new Dictionary<JingJieType, int>();

        public void RegisterNetMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_JINGJIE_INFO, OnJingJieInfo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_JINGJIE_SAVE, OnJingJieSave);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_JINGJIE_RESET, OnJingJieReset);
        }

        private void OnJingJieInfo(GameEvent ge, EventParameter parameter)
        {
            MsgData_sDianfengInfo data = parameter.msgParameter as MsgData_sDianfengInfo; 
            mData.Clear();
            for (int i = 0; i < data.Data.Count; i++)
            {
                mData[data.Data[i].ID] = data.Data[i].Num;
            }
            RefreshData();

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_JINGJIE_INFO, null);
        }

        private void OnJingJieSave(GameEvent ge, EventParameter parameter)
        {
            MsgData_sDianfengSave data = parameter.msgParameter as MsgData_sDianfengSave;
            
            NetLogicGame.Instance.SendReqDianfengInfo();

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_JINGJIE_SAVE, EventParameter.Get(data.Result));
        }

        private void OnJingJieReset(GameEvent ge, EventParameter parameter)
        {
            MsgData_sDianfengReset data = parameter.msgParameter as MsgData_sDianfengReset;
            
            NetLogicGame.Instance.SendReqDianfengInfo();

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_JINGJIE_RESET, EventParameter.Get(data.Result));
        }

        private void RefreshData()
        {
            mUsedPoint.Clear();
            mTotalPoint.Clear();
            for (int i = 0; i < (int)JingJieType.MAX; i++)
            {
                mUsedPoint.Add((JingJieType)i, 0);
                mTotalPoint.Add((JingJieType)i, 0);
            }
            foreach (var data in mData)
            {
                LuaTable t = ConfigManager.Instance.Jingjie.GetNodeConfig(data.Key);
                if(t != null)
                {
                    JingJieType type = (JingJieType)t.Get<int>("nodeType");
                    int count = 0;
                    if (mUsedPoint.TryGetValue(type, out count))
                    {
                        count += data.Value;
                    }

                    mUsedPoint[type] = count;
                }
            }

            int lv = PlayerData.Instance.BaseAttr.JingJieLevel;
            for (int i = 0; i <= lv; i++)
            {
                LuaTable t = ConfigManager.Instance.Jingjie.GetLevelConfig(i);
                if (t != null)
                {
                    JingJieType type = (JingJieType)t.Get<int>("nodeType");
                    int count = 0;
                    if (mTotalPoint.TryGetValue(type, out count))
                    {
                        count += t.Get<int>("point");
                    }

                    mTotalPoint[type] = count;
                }
            }
        }

        public void SendSavePoint(int ID, int num)
        {
            List<DianfengInfo_Attr> list = new List<DianfengInfo_Attr>();
            DianfengInfo_Attr attr = new DianfengInfo_Attr();
            attr.ID = ID;
            attr.Num = num;
            list.Add(attr);

            NetLogicGame.Instance.SendReqDianfengSave(list);
        }

        public void SendSavePoint(LuaTable tbl)
        {
            List<DianfengInfo_Attr> list = new List<DianfengInfo_Attr>();
            tbl.ForEach<int, int>((id, num) =>
            {
                DianfengInfo_Attr attr = new DianfengInfo_Attr();
                attr.ID = id;
                attr.Num = num;

                list.Add(attr);
            });

            NetLogicGame.Instance.SendReqDianfengSave(list);
        }

        public void SendResetPoint(JingJieType type)
        {
            NetLogicGame.Instance.SendReqDianfengReset((int)type);
        }

        public int GetPoint(int id)
        {
            int count = 0;
            if (mData.TryGetValue(id, out count))
            {
                return count;
            }

            return 0;
        }

        public int GetUsedPoint(JingJieType type)
        {
            int count = 0;
            if (mUsedPoint.TryGetValue(type, out count))
            {
                return count;
            }

            return 0;
        }

        public int GetAllPoint(JingJieType type)
        {
            int count = 0;
            if (mTotalPoint.TryGetValue(type, out count))
            {
                return count;
            }

            return 0;
        }
    }
}
