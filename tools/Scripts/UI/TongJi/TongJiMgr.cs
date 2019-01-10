using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;
using System;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class TongJiMgr
    {

        public enum TongJiEnum
        {
            info = 1,       //通缉信息
            diffRefresh = 2,  //难度刷新结果
            accept = 3,     //接受任务结果
            noticeReward = 4,  //可领取奖励通知(完成任务通知)
            getReward = 5,  //领取奖励结果
            giveup = 6,   //放弃任务结果
            listRefresh = 7,  //自动刷新通缉列表
            timeReset = 8, //冷却时间重置
            getBoxReward = 9,  //领取宝箱奖励结果
        }

        private static TongJiMgr instance = null;
        public static TongJiMgr Instance
        {
            get
            {
                if (null == instance)
                    instance = new TongJiMgr();

                return instance;
            }
        }
        /// <summary>
        /// 通缉msg
        /// </summary>
        public MsgData_sTongJiInfo info = new MsgData_sTongJiInfo();

        /// <summary>
        /// 已领取的宝箱id
        /// </summary>
        public List<int> boxIds = new List<int>();

        private int curGetBoxId;  //当前请求领取的宝箱id

        public DateTime NowTime;

        /// <summary>
        /// 通缉任务规定的每个任务的冷却时间
        /// </summary>
        private int leftTime = 900;

        public TongJiMgr()
        {
           // leftTime = *******
            NowTime = DateTime.Now;
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_TongJiInfo, OnTongJiInfo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_TongJiLvlRefreshResult, OnTongJiLvlRefreshResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_AcceptTongJiResult, OnAcceptTongJiResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_FinishTongJi, OnFinishTongJi);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_GetTongJiReward, OnGetTongJiReward);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_GiveupTongJiResult, OnGiveupTongJiResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_RefreshTongJiList, OnRefreshTongJiList);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_GetTongJiBoxResult, OnGetTongJiBoxResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_TongJiRefreshState, OnTongJiRefreshState);
        }

        public int GetCurLeftTime()
        {
            double tt = (DateTime.Now - NowTime).TotalSeconds;
            int less = leftTime - Convert.ToInt32(tt);
            return less > 0 ? less : 0;
        }

        public bool IsHaveBoxId(int boxId)
        {
            if(boxIds.Contains(boxId))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// C->S 接受通缉任务 msgId:3166;
        /// </summary>
        /// <param name="id"></param>
        public void Send_CS_AcceptTongJi(int id)
        {
            MsgData_cAcceptTongJi rsp = new MsgData_cAcceptTongJi();
            rsp.id = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_AcceptTongJi, rsp);
        }

        /// <summary>
        /// C->S 难度刷新 msgId:3165;
        ///  刷新类型 0:客户端申请列表 1:银两 2:元宝;
        /// </summary>
        /// <param name="type"></param>
        public void Send_CS_TongJiRefresh(int type)
        {
            LogMgr.UnityLog("请求通缉");
            MsgData_cTongJiLvlRefresh rsp = new MsgData_cTongJiLvlRefresh();
            rsp.type = type;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TongJiLvlRefresh, rsp);
        }

        /// <summary>
        ///  C->S 领取通缉奖励 msgId:3168;  (弹框奖励 领取) 
        /// id  通缉id;   type类型 0、普通，1、银两，2、元宝;
        /// </summary>
        /// <param name="id"></param>
        public void Send_CS_GetTongJiReward(int id,int type)
        {
            MsgData_cGetTongJiReward rsp = new MsgData_cGetTongJiReward();
            rsp.id = id;
            rsp.type = type;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GetTongJiReward, rsp);
        }

        /// <summary>
        /// 获取通缉宝箱奖励 msgId:3171;
        /// </summary>
        /// <param name="level"></param>
        public void Send_CS_GetTongJiBox(int level)
        {
            MsgData_cGetTongJiBox rsp = new MsgData_cGetTongJiBox();
            rsp.boxId = level;
            curGetBoxId = level;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GetTongJiBox, rsp);
        }

        /// <summary>
        /// C->S 放弃通缉 msgId:3169;
        /// </summary>
        /// <param name="id"></param>
        public void Send_CS_GiveUpTongJi(int id)
        {
            MsgData_cGiveupTongJi rsp = new MsgData_cGiveupTongJi();
            rsp.id = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GiveupTongJi, rsp);
        }

        /// <summary>
        /// 刷新-重置时间
        /// </summary>
        public void Send_CS_RefreshState()
        {
            MsgData_cTongJiRefreshState rsp = new MsgData_cTongJiRefreshState();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TongJiRefreshState,rsp);
        }

        void OnTongJiInfo(GameEvent ge, EventParameter parameter)
        {
            info = parameter.msgParameter as MsgData_sTongJiInfo;
          //  Debug.LogError("info.groupid  "+info.Group +"   info.id  "+info.id);
            NowTime = DateTime.Now;
            leftTime = info.coolTime;
            boxIds.Clear();
            for (int i = 0; i < info.boxList.Count; i++)
            {
                if(!boxIds.Contains(info.boxList[i].boxId))
                {
                    boxIds.Add(info.boxList[i].boxId);
                }
            }
            SendMsg(TongJiEnum.info, info);
        }

        void OnTongJiLvlRefreshResult(GameEvent ge, EventParameter parameter)
        {
            MsgData_sTongJiLvlRefreshResult resp = parameter.msgParameter as MsgData_sTongJiLvlRefreshResult;
            string textTip = "";
            if (resp.result == 0)
            {
                if (info.id != resp.id)
                { 
                    textTip = "刷新成功";
                }
                else
                {
                    textTip = "未提品成功，请再次尝试提品！";
                }
                info.id = resp.id;
                info.Group = resp.Group;
                SendMsg(TongJiEnum.diffRefresh, info);
               
            }else
            {
                textTip = "刷新失败";
            }
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            ShowTips fun = G.GetInPath<ShowTips>("Common.ShowTips");
            if (fun != null && !string.IsNullOrEmpty(textTip) && "TongJiPanel" == MainPanelMgr.Instance.CurPanelName)
            {
                fun(textTip);
            }
        }

        void OnAcceptTongJiResult(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sAcceptTongJiResult resp = parameter.msgParameter as MsgData_sAcceptTongJiResult;
            info.state = 1;  //领取任务后，state = 1
            SendMsg(TongJiEnum.accept, info);
        }
        void OnFinishTongJi(GameEvent ge, EventParameter parameter)
        {
            MsgData_sFinishTongJi resp = parameter.msgParameter as MsgData_sFinishTongJi;
            info.id = resp.id;
            info.state = 2;
            CoreEntry.gAutoAIMgr.AutoFight = false;  //任务完成，关闭挂机
            SendMsg(TongJiEnum.noticeReward, info);
        }

        void OnGetTongJiReward(GameEvent ge, EventParameter parameter)
        {
            MsgData_sGetTongJiReward resp = parameter.msgParameter as MsgData_sGetTongJiReward;
            if(resp.result == 0)
            {
                info.state = 0;
                SendMsg(TongJiEnum.getReward, info);
            }          
        }

        void OnGiveupTongJiResult(GameEvent ge, EventParameter parameter)
        {
            MsgData_sGiveupTongJiResult resp = parameter.msgParameter as MsgData_sGiveupTongJiResult;
            if (resp.result == 0)
            {
                if (info.state == 1)
                {
                    info.state = 0;   //放弃任务后，state = 0
                }
                SendMsg(TongJiEnum.giveup, info);
            }
        }     

        void OnRefreshTongJiList(GameEvent ge, EventParameter parameter)
        {
            MsgData_sRefreshTongJiList resp = parameter.msgParameter as MsgData_sRefreshTongJiList;
            info.Group = resp.Group;
            info.id = resp.id;
            NowTime = DateTime.Now;
            SendMsg(TongJiEnum.listRefresh, info);
        }

        void OnGetTongJiBoxResult(GameEvent ge, EventParameter parameter)
        {
            MsgData_sGetTongJiBoxResult resp = parameter.msgParameter as MsgData_sGetTongJiBoxResult;
            if(resp.result == 0)
            {
                info.count += 1;
                MsgData_sScoreBoxVO v = new MsgData_sScoreBoxVO();
                v.boxId = curGetBoxId;
                if (!info.boxList.Contains(v))
                {
                    info.boxList.Add(v);
                }
                if(!boxIds.Contains(curGetBoxId))
                {
                    boxIds.Add(curGetBoxId);
                }
                SendMsg(TongJiEnum.getBoxReward, info);
            }
        }
        
        void OnTongJiRefreshState(GameEvent ge, EventParameter parameter)
        {   
            string textTip = "";
            MsgData_sTongJiRefreshState resp = parameter.msgParameter as MsgData_sTongJiRefreshState;
            //Debug.LogError("resp.result"+resp.result);
            if (resp.result == 0)
            {
                info.coolTime = 0;
                NowTime = DateTime.Now;
                leftTime = info.coolTime;
                SendMsg(TongJiEnum.timeReset, info);
                textTip = "刷新成功";
            }
            else
            {
                return;
                //textTip = "刷新失败";
            }     
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            ShowTips fun = G.GetInPath<ShowTips>("Common.ShowTips");
            if (fun != null && !string.IsNullOrEmpty(textTip))
            {
                fun(textTip);
            }
        }

        void SendMsg(TongJiEnum enumType, MsgData msg, int id = 0)
        { 
            EventParameter par = EventParameter.Get(msg);
            par.intParameter = (int)enumType;
            par.intParameter1 = info.id;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_TongJiUpdate, par);  //通知 通缉UI更新

            OtherTaskData dt = new OtherTaskData(OtherTaskType.TongJi, info.id, info.state);
            EventParameter taskPar = EventParameter.Get();
            taskPar.objParameter = dt;
            if (info.finishCount >= info.buyCount + 5)  //全部完成没有任务了
            {
                taskPar.intParameter = (int)OtherTaskState.remove;
            }        
            else
            {
                taskPar.intParameter = (int)OtherTaskState.add;
            }
            if(enumType != TongJiEnum.getBoxReward)           
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_OtherQuestIdChange, taskPar);  //通知 任务主界面 更新
        }
    }
}

