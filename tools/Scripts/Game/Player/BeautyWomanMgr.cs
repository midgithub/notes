/**
* @file     :  .cs
* @brief    : 
* @details  :  
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


    // S->C 美人信息
    [LuaCallCSharp]
    [Hotfix]
    public class BeautyWomanVo 
    {
        public int ID; // 红颜ID
        public int actState; // 激活类型 0-未激活 1-已激活 2-已出战
        public int jiedian; // 当前节点
        public int jiedianlevel; // 节点对应的等级
        public int gradeState; // 升阶类型 0-升星 1-升阶
        public int gradeNum; // 等阶
        public int starNum; // 星数
        public int currentExp; // 星数经验
        public int gradExp; // 祝福值
	    public int count; // 已使用的数量
        public int fighting;//战斗力
    }

    [LuaCallCSharp]
    [Hotfix]
    public class BeautyWomanMgr
    {
         
        //private static BeautyWomanMgr _instance = null;
        public static BeautyWomanMgr Instance
        {
            get
            {
                return PlayerData.Instance.BeautyWomanMgr;
            }
        }
        Dictionary<int, BeautyWomanVo> mBeautyWomanInfo = new Dictionary<int, BeautyWomanVo>();
        public Dictionary<int, BeautyWomanVo> BeautyWomanInfo
        {
            get { return mBeautyWomanInfo; }
            set { mBeautyWomanInfo = value; }
        }

        //出战红颜id
        public int mGoOutID = 0;
       
#region msg
        public void ResisterMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BeautyWomanInfo, GE_BeautyWomanInfo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_HONGYANACT, GE_HONGYANACT);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_HONGYANFIGHT, GE_HONGYANFIGHT);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BeautyWomanLevelUp, GE_BeautyWomanLevelUp);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BeautyWomanUseAtt, GE_BeautyWomanUseAtt);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BeautyWomanFightingUpdate, GE_BeautyWomanFightingUpdate);
             
            
          
        }


        public void GE_BeautyWomanInfo(GameEvent ge, EventParameter param)
        {
            MsgData_sBeautyWomanInfo data = param.msgParameter as MsgData_sBeautyWomanInfo;
            mGoOutID = data.ID;
            for (int i = 0; i < data.items.Count; i++)
            {
                UpdateBeautyWomanInfo(data.items[i]);
            }
        }

        public void GE_HONGYANACT(GameEvent ge, EventParameter param)
        {
            MsgData_sHongyanAct data = param.msgParameter as MsgData_sHongyanAct;
            if (data.Result != 0)
                return;
            BeautyWomanVo bw = GetBeautyWoman(data.ID);
            if (bw == null)
            {
                MsgData_sBeautyWomanVo item = new MsgData_sBeautyWomanVo();
                item.ID = data.ID;
                item.actState = data.State;
                item.jiedian = data.Jiedian;
                item.jiedianlevel = data.JiedianLevel;
                item.gradeState = 0; // 升阶类型 0-升星 1-升阶
                item.gradeNum = 0; // 等阶
                item.starNum = 0; // 星数
                item.currentExp = 0;  // 星数经验
                item.gradExp = 0; // 祝福值
                item.count = 0;
                item.fighting = 0;
                UpdateBeautyWomanInfo(item);
            }
            else
            {
                bw.actState = data.State;
                bw.jiedian = data.Jiedian;
                bw.jiedianlevel = data.JiedianLevel;
                if(bw.actState == 1)
                    bw.gradeNum = 1;
            }

        }
        public void GE_HONGYANFIGHT(GameEvent ge, EventParameter param)
        {
            MsgData_sHongyanFight data = param.msgParameter as MsgData_sHongyanFight;
            if (data.Result != 0)
                return;
            if (data.ID == 0)
                return;

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SKILL_UPGRADE, null);

            Dictionary<int, BeautyWomanVo>.Enumerator iter = BeautyWomanInfo.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current.Value.actState == 2)
                {
                    iter.Current.Value.actState = 1;
                    break;
                }
            }

            BeautyWomanVo bw = GetBeautyWoman(data.ID/100);
            if (bw == null)
            {
                MsgData_sBeautyWomanVo item = new MsgData_sBeautyWomanVo();
                item.ID = data.ID / 100;
                item.actState = data.State;
                UpdateBeautyWomanInfo(item);
            }
            else
            {
                bw.actState = data.State;
            }
        }

        public void GE_BeautyWomanLevelUp(GameEvent ge, EventParameter param)
        {
            MsgData_sBeautyWomanLevelUp data = param.msgParameter as MsgData_sBeautyWomanLevelUp;
            if (data.result != 0)
            {
                return;
            }

            BeautyWomanVo bw = GetBeautyWoman(data.ID);
            if (bw != null)
            {
                bw.gradeState = data.State;
                bw.starNum = data.starNum;
                bw.currentExp = data.currentExp;
                bw.gradeNum = data.gradeNum;
                bw.gradExp = data.radExp;
            }

        }
        public void UpdateBeautyWomanInfo(MsgData_sBeautyWomanVo item)
        {
            BeautyWomanVo v = new BeautyWomanVo();
            v.ID = item.ID;
            v.actState = item.actState; // 激活类型 0-未激活 1-已激活 2-已出战
            v.jiedian = item.jiedian; // 当前节点
            v.jiedianlevel = item.jiedianlevel; // 节点对应的等级
            v.gradeState = item.gradeState; // 升阶类型 0-升星 1-升阶
            v.gradeNum = item.gradeNum; // 等阶
            v.starNum = item.starNum; // 星数
            v.currentExp = item.currentExp;  // 星数经验
            v.gradExp = item.gradExp; // 祝福值
            v.count = item.count;
            v.fighting = item.fighting;
            if (BeautyWomanInfo.ContainsKey(v.ID))
            {
                BeautyWomanInfo[v.ID] = v;
            }
            else
            {
                BeautyWomanInfo.Add(v.ID, v);
            } 
        }

        public void GE_BeautyWomanUseAtt(GameEvent ge, EventParameter param)
        {
            MsgData_sBeautyWomanUseAtt data = param.msgParameter as MsgData_sBeautyWomanUseAtt;
            if (data.result != 0)
            {
                return;
            }
             BeautyWomanVo bw = GetBeautyWoman(data.ID);
             if (bw != null)
             {
                 bw.count = data.count;
             } 
        }

        public void GE_BeautyWomanFightingUpdate(GameEvent ge, EventParameter param)
        {
            MsgData_sBeautyWomanFightingUpdate data = param.msgParameter as MsgData_sBeautyWomanFightingUpdate;
            BeautyWomanVo bw = GetBeautyWoman(data.ID);
            if (bw != null)
            {
                bw.fighting = data.fighting;
            } 

        }
#endregion 
         
        public BeautyWomanVo GetBeautyWoman(int id)
        {
            BeautyWomanVo v = null;
            if (BeautyWomanInfo.ContainsKey(id))
            {
                v = BeautyWomanInfo[id];
            }
            return v;
        }
    }
}