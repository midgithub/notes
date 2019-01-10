using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;



namespace SG
{
[Hotfix]
    public class NetMsgClientWarpper
    {
        static private NetMsgClientWarpper s_netMsgItem = null;
        public static NetMsgClientWarpper Instance
        {
            get
            {
                if (null == s_netMsgItem)
                {
                    s_netMsgItem = new NetMsgClientWarpper();
                }
                return s_netMsgItem; 
            }
        }
        public void Release()
        {
        }
        public void ResisterMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EQUIP_POSLEVELUP, OnEquipPosLevelUp);////服务器返回:装备位升级
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EQUIP_EQUIPDECOMPOSE, OnEquipDecompose);//返回分解结果
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_ITEMTIPS, OnItemTips);//物品获得失去提示 
            //LuaMgr.Instance.DoString("require 'Lua/NetMsgClientWarp.lua'");
        }

        public void OnEquipPosLevelUp(GameEvent ge, EventParameter paramter)
        {
            MsgData_sEquipPosLevelUp data = paramter.msgParameter as MsgData_sEquipPosLevelUp;
            if (data.result != 1)
            {
                EquipPosInfo item = EquipDataMgr.Instance.GetEquipPosInfo(data.pos);
                if (item != null)
                {
                    if(item.level != data.level)
                    {
                        EventParameter ep = EventParameter.Get();
                        ep.intParameter = data.pos;
                        ep.intParameter1 = item.level;
                        ep.intParameter2 = data.level;
                        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EQUIP_POSLEVELUPUI, ep);
                    }
                    item.level = data.level;
                    item.exp = data.exp;
                    item.starLevel = data.starLevel;
                    item.vip1 = data.vip1;
                    item.vip2 = data.vip2;
                    item.vip3 = data.vip3;
                    item.madness = data.madness;

                }

            }
        }

        public void OnEquipDecompose(GameEvent ge, EventParameter paramter)
        {
            //MsgData_sEquipDecompose data = paramter.msgParameter as MsgData_sEquipDecompose;
        }

        public void OnItemTips(GameEvent ge, EventParameter paramter)
        {
            //MsgData_sItemTips data = paramter.msgParameter as MsgData_sItemTips; 
        }
        

    }
}

