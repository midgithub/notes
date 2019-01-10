using UnityEngine;
using System.Collections;
using XLua;
using System;
using System.Collections.Generic;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class TitleMgr
    {
        /// <summary>
        /// 服务器返回类型
        /// </summary>
        public enum TitleEnum  
        {
            info = 1,  //爵位信息
            levelUp = 2, //爵位升级
            constValue = 3, //服务器功勋
            huoyueInfo = 4,  //活跃度信息
            huoyueFinish = 5, //活跃度任务完成一次
            huoyueReward = 6, //活跃奖励结果
        }

        private static TitleMgr instance = null;
        public static TitleMgr Instance
        {
            get
            {
                if (null == instance)
                    instance = new TitleMgr();

                return instance;
            }
        }
        /// <summary>
        /// 爵位信息
        /// </summary>
        public MsgData_sGuanZhiInfo info;

        /// <summary>
        /// 请求类型 --1.上报，2 升级
        /// </summary>
        public int SendType = 0;

        /// <summary>
        /// 请求前的已上报功勋值
        /// </summary>
        public int SendStartVital = 0;
        /// <summary>
        /// 请求后的已上报功勋值
        /// </summary>
        public int SendEndVital = 0;


        /// <summary>
        /// 当前剩余功勋值
        /// </summary>
        public int curContValue = 0;
      
        /// <summary>
        /// 爵位战斗力
        /// </summary>
        public long Power;

        /// <summary>
        /// 活跃度进度计数 ,  key -活跃度id , value - 已完成的次数
        /// </summary>
        private Dictionary<int, int> vitalityList = new Dictionary<int, int>();

        /// <summary>
        /// 爵位配置
        /// </summary>
        public static Dictionary<int, LuaTable> CacheTitleConfig = null;

        public TitleMgr()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            CacheTitleConfig = G.Get<Dictionary<int, LuaTable>>("t_guanzhi");
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_GuanZhiInfo, OnGuanZhiInfo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_GuanZhiLevelUp, OnGuanZhiLevelUp);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_InterServiceContValue, OnInterServiceContValue);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_HuoYueDu, OnHuoYueDu);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_HuoYueDuFinish, OnHuoYueDuFinish);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_HuoYueReward, OnHuoYueReward);
            //Send_CS_ContValue();  //默认请求一次服务器功勋
        }

        /// <summary>
        /// 获取当前爵位的config表名字 lordId -爵位id ,   bColor -是否返回 带有品质颜色的名称
        /// </summary>
        /// <param name="guanzhiId"></param>
        /// <returns></returns>
        public string GetCurLordText(int lordId,bool bColor = false)
        {

            LuaTable l;
            string title_name = "";
            int quality = 0;
            if (CacheTitleConfig.TryGetValue(lordId, out l))
            {
                if (bColor)
                {
                    title_name = l.Get<string>("title_name");
                   // string lvl = l.Get<string>("title_levelName");
                    quality = l.Get<int>("quality");
                    if (title_name == "")
                    {
                        return "";
                    }
                    else
                    {
                        string lordName = string.Format("<color=#{0}>[{1}]</color>", UiUtil.GetQualitycolor(quality), title_name);
                        return lordName;
                    }
                   
                }
                else
                {
                    title_name = l.Get<string>("title_name");
                    return title_name;
                }
                
            }
            return "";
        }


        /// <summary>
        /// 获取指定活跃度id进行的次数
        /// </summary>
        /// <param name="vitalId"></param>
        public int GetCurVitalityCount(int vitalId)
        {
            if(vitalityList.ContainsKey(vitalId))
            {
                return vitalityList[vitalId];
            }
            return 0;
        }

        /// <summary>
        /// 请求服务器功勋 msgId:3810
        /// </summary>
        public void Send_CS_ContValue()
        {
            MsgData_cInterServiceContValue rsp = new MsgData_cInterServiceContValue();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_InterServiceContValue, rsp);
        }

        /// <summary>
        /// 客户端请求：官职信息 msgId:3689
        /// </summary>
        public void Send_CS_GuanZhiInfo()
        {
            MsgData_cGuanZhiInfo rsp = new MsgData_cGuanZhiInfo();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GuanZhiInfo, rsp);
        }

        /// <summary>
        /// 客户端请求：官职升级 msgId:3688;1.上报，2 升级
        /// </summary>
        public void Send_CS_GuanZhiLevelUp(int type)
        {
            MsgData_cGuanZhiLevelUp rsp = new MsgData_cGuanZhiLevelUp();
            rsp.type = type;
            SendType = type;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GuanZhiLevelUp, rsp);
        }
        /// <summary>
        /// 客户端请求：活跃奖励 msgId:3262;
        /// </summary>
        public void Send_CS_HuoYueReward()
        {
            MsgData_cHuoYueReward rsp = new MsgData_cHuoYueReward();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_HuoYueReward, rsp);
        }

        /// <summary>
        /// 服务器返回:官职信息 msgId:8689;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        void OnGuanZhiInfo(GameEvent ge, EventParameter parameter)
        {
            info = parameter.msgParameter as MsgData_sGuanZhiInfo;
     //       Debug.LogError("官职id    " +info.id);
     //       Debug.LogError("已上报功勋     " +info.val);
            SendMsg(TitleEnum.info,info);
        }
        /// <summary>
        /// 服务器返回:官职升级 msgId:8688;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        void OnGuanZhiLevelUp(GameEvent ge, EventParameter parameter)
        {
            MsgData_sGuanZhiLevelUp resp = parameter.msgParameter as MsgData_sGuanZhiLevelUp;
         //   Debug.LogError("上报升级处理结果 " + resp.result + "   上报功勋值  " + resp.val);
            if(resp.result == 0)
            {               
                if(SendType ==1)  //上传功勋成功,改变info的 已上传值
                {
                    curContValue -= (resp.val - info.val);
                    SendStartVital = info.val;
                    info.val = resp.val;
                    SendEndVital = info.val;
                }
                else if(SendType == 2) //爵位进阶成功，info 改变
                {
                    curContValue -= resp.val;
                    LuaTable l;
                    int nextId = 0;
                    if (CacheTitleConfig.TryGetValue(info.id, out l))
                    {
                        nextId = l.Get<int>("next_id");
                    }
                    SendStartVital =0;
                    SendEndVital = 0;
                    info = new MsgData_sGuanZhiInfo();
                    info.id = nextId;
                    info.val = 0;
                }
            }
            SendMsg(TitleEnum.levelUp,resp);
        }
        /// <summary>
        /// 返回服务器功勋 msgId:8810;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        void OnInterServiceContValue(GameEvent ge, EventParameter parameter)
        {
            MsgData_sInterServiceContValue resp = parameter.msgParameter as MsgData_sInterServiceContValue;
          //  Debug.LogError("返回服务器功勋值 " + resp.contValue);
            SendMsg(TitleEnum.constValue,resp);
        }

        /// <summary>
        /// 返回活跃度信息 msgId:8260;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        void OnHuoYueDu(GameEvent ge, EventParameter parameter)
        {
            MsgData_sHuoYueDu resp = parameter.msgParameter as MsgData_sHuoYueDu;
            for (int i = 0; i < resp.list.Count; i++)
            {
                MsgData_sHuoYueDuTaskVo tt = resp.list[i];
                vitalityList[tt.id] = tt.num;
            }
            curContValue = resp.vitality;
            SendMsg(TitleEnum.huoyueInfo,resp);
        }
        /// <summary>
        /// 返回活跃度任务完成一次 msgId:8261;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        void OnHuoYueDuFinish(GameEvent ge, EventParameter parameter)
        {
            MsgData_sHuoYueDuFinish resp = parameter.msgParameter as MsgData_sHuoYueDuFinish;
            vitalityList[resp.id] = resp.num;
          //  Debug.LogError("活跃度完成一次: " +resp.id +"  " + resp.vitality);
            curContValue += resp.vitality;
            SendMsg(TitleEnum.huoyueFinish,resp);
        }
        /// <summary>
        /// 返回获取活跃奖励结果 msgId:8262;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        void OnHuoYueReward(GameEvent ge, EventParameter parameter)
        {
            MsgData_sHuoYueReward resp = parameter.msgParameter as MsgData_sHuoYueReward;
          //  Debug.LogError("活跃度奖励: " + resp.id + "  " + resp.result);
            SendMsg(TitleEnum.huoyueReward,resp);
        }

        void SendMsg(TitleEnum enumType, MsgData msg)
        {
            EventParameter par = EventParameter.Get(msg);
            par.intParameter = (int)enumType;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_PlayerTitleUpdate, par);
        }

    }
}

