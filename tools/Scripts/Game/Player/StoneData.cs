/**
* @file     : StoneData
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-08-02
*/
using UnityEngine;
using System.Collections.Generic;
using XLua;

namespace SG
{
    [LuaCallCSharp]
    [Hotfix]
    public class StoneData
    {
        private Dictionary<int, int> mStoneInfo = new Dictionary<int, int>();

        public void RegisterNetMsg()
        {
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GEMSLOTINFO, OnStoneSlotInfo);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GEMSLOTOPEN, OnStoneSlotOpen);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GEMINSET, OnStoneInset);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GEMLEVELUP, OnStoneLvUp);
        }

        public void ClearData()
        {
            mStoneInfo.Clear();
        }
        public void ReqInfo()
        {
            NetLogicGame.Instance.SendReqStoneInfo();
        }

        public void ReqInset(int pos, long guid, int type)
        {
            NetLogicGame.Instance.SendReqStoneInset(pos, guid, type);
        }

        public void ReqSlotOpen(int pos)
        {
            NetLogicGame.Instance.SendReqStoneSlotOpen(pos);
        }

        public void ReqStoneLvup(int id)
        {
            NetLogicGame.Instance.SendReqStoneLvUp(id);
        }

        private void OnStoneSlotInfo(MsgData msg)
        {
            MsgData_sGemOpenInfo data = msg as MsgData_sGemOpenInfo;
            for (int i = 0; i < data.Data.Count; i++)
            {
                MsgData_sGemOpenItem item = data.Data[i];
                mStoneInfo[item.Pos] = item.IsOpen;
            }

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_STONE_SLOTINFO, null);
        }

        private void OnStoneSlotOpen(MsgData msg)
        {
            MsgData_sReqGemSlotOpen data = msg as MsgData_sReqGemSlotOpen;
            if (data.Result == 0)
            {
                mStoneInfo[data.Pos] = 1;
            }

            EventParameter param = EventParameter.Get();
            param.intParameter = data.Result;
            param.intParameter1 = data.Pos;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_STONE_OPENSLOT, param);
        }

        private void OnStoneInset(MsgData msg)
        {
            MsgData_sReqEquipGemInset data = msg as MsgData_sReqEquipGemInset;
            for (int i = 0; i < data.Data.Count; i++)
            {

            }

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_STONE_INSET, null);
        }

        private void OnStoneLvUp(MsgData msg)
        {
            MsgData_sEquipGemUpLevel data = msg as MsgData_sEquipGemUpLevel;

            EventParameter param = EventParameter.Get();
            param.intParameter = data.Result;
            param.intParameter1 = data.ID;
            param.intParameter2 = data.Level;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_STONE_LEVELUP, param);
        }

        public bool IsOpen(int pos)
        {
            if (mStoneInfo.ContainsKey(pos))
            {
                return mStoneInfo[pos] == 1;
            }

            return false;
        }

        public int CurTabIndex
        {
            get;
            set;
        }
    }
}
