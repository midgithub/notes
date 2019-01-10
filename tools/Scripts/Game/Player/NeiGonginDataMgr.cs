/**
* @file     :  
* @brief    : 占星
* @details  :  
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;
using System;

using System.Text;

namespace SG
{
    [LuaCallCSharp]
    [Hotfix]
    public class NeiGonginset 
    {
        public int id; // 经脉的ID； 
        public int NodeId; // 最后冲穴成功的穴位ID 
        public int Level; // 穴位当前的等级
    }

    [LuaCallCSharp]
    [Hotfix]
    public class NeiGonginDataMgr
    { 
        //private static NeiGonginDataMgr _instance = null;
        public static NeiGonginDataMgr Instance
        {
            get
            {
                return PlayerData.Instance.NeiGonginDataMgr;
            }
        }
        Dictionary<int, NeiGonginset> mInfo = new Dictionary<int, NeiGonginset>();
        public Dictionary<int, NeiGonginset> Info
        {
            get { return mInfo; }
            set { mInfo = value; }
        }
        public int Power;
#region msg
        public void ResisterMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_NeiGongInfo, OnNeiGongInfo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_OpenNode, OnOpenNode);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, OnCleanUpUserData);
        }

        public void OnCleanUpUserData(GameEvent ge, EventParameter param)
        {
            mInfo.Clear();
            Power = 0;
        }

        public void OnNeiGongInfo(GameEvent ge, EventParameter param) 
        {
            MsgData_sNeiGongInfo data = param.msgParameter as MsgData_sNeiGongInfo;
            for (int i = 0; i < data.Items.Count; i++)
            {
                UpdateInfo(data.Items[i]);
            } 
        }


        public void OnOpenNode(GameEvent ge, EventParameter param)
        {
            MsgData_sOpenNode data = param.msgParameter as MsgData_sOpenNode;
            if (data.result != 0) return;
            NeiGonginset v = GetInfo(data.id);
            if (v != null)
            {
                v.NodeId = data.NodeId;
                v.Level = data.Level;
            }
            else
            {
                v = new NeiGonginset();
                v.id = data.id;
                v.NodeId = data.NodeId; // 最后冲穴成功的穴位ID 
                v.Level = data.Level; // 穴位当前的等级 
                mInfo[v.id] = v;
            }
        }
#endregion

        //副本信息
        public void UpdateInfo(MsgData_sNeiGonginset item)
        {
            NeiGonginset v = new NeiGonginset(); 
            v.id = item.id; // 经脉的ID； 
            v.NodeId = item.NodeId; // 最后冲穴成功的穴位ID 
            v.Level = item.Level; // 穴位当前的等级 
            if (mInfo.ContainsKey(v.id))
            {
                mInfo[v.id] = v;
            }
            else
            {
                mInfo.Add(v.id, v);
            }
        }
        
        public NeiGonginset GetInfo(int id)
        {
            NeiGonginset v = null;
            if (mInfo.ContainsKey(id))
            {
                v = mInfo[id];
            }
//             else
//             {
//                 v = new NeiGonginset();
//                 v.id = id;
//                 v.NodeId = 0; // 最后冲穴成功的穴位ID 
//                 v.Level = 0; // 穴位当前的等级 
//                 mInfo[id] = v;
//             }
            return v; 
        }

        public int GetID()
        {
            if (mInfo.Count == 0)
            {
                return 0;
            }
            List<int> keys = new List<int>(mInfo.Keys);
//             for (int i = keys.Count; i > 0; i--)
//             {
//                 if (mInfo[keys[i - 1]].NodeId != 0)
//                     return mInfo[keys[i - 1]].id;
//             }

            keys.Sort();
            return mInfo[keys[keys.Count - 1]].id;
        }

    }
}