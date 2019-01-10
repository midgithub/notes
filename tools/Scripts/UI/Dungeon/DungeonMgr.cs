using UnityEngine;
using System.Collections;
using XLua;
using System;
using SG;
using System.Collections.Generic;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class DungeonMgr
    {
        private static DungeonMgr instance = null;
        public static DungeonMgr Instance
        {
            get
            {
                if (null == instance)
                    instance = new DungeonMgr();
                return instance;
            }
        }

        public DungeonMgr()
        {
            //法宝副本
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_LingShouMuDiInfo, OnLingShouMuDiInfo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_ChallLingShouMuDi, OnChallLingShouMuDi);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_ChallLingShouMuDiResult, OnChallLingShouMuDiResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_LingShouMuDiQuit, OnLingShouMuDiQuit);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_LingShouMuDiGetAward, OnLingShouMuDiGetAward);

            //幻灵仙域副本
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_HunlingXianYuInfo, OnHunlingXianYuInfo);
           // CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_ResetHunlingXianYu, OnResetHunLingXianYu);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_ChallHunlingXianYu, OnChallHunLingXianYu);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_ChallHunlingXianYuResult, OnChallHunLingXianYuResult);
           // CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_SCanHunlingXianYuResult, OnScanHunLingXianYuResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_HunlingXianYuQuit, OnHunLingXianYuQuit);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_HunlingXianYuGetAward, OnHunLingXianYuGetAward);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_HunlingXianYuMonsterInfo, OnHunLingXianYuMonsterInfo);
            //            Send_CS_HunLingXianYuInfo();

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_EnterDungeonResult, OnEnterDungeonResult);
            /*  剧情副本系列协议，还没做，先屏蔽掉
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_EnterDungeonResult, OnEnterDungeonResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_LeaveDungeonResult, OnLeaveDungeonResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_StoryEndResult, OnStoryEndResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_StoryStep, OnStoryStep);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_DungeonGroupUpdate, OnDungeonGroupUpdate);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_DungeonCountDown, OnDungeonCountDown);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_DungeonPassResult, OnDungeonPassResult);
            */
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_DungeonQuestUpdate, OnDungeonQuestUpdate);


        }
        /*   剧情副本系列协议，还没做，先屏蔽掉
        public void OnEnterDungeonResult(GameEvent ge, EventParameter parameter)
        {
        
        }
        public void OnLeaveDungeonResult(GameEvent ge, EventParameter parameter)
        {

        }
        public void OnStoryEndResult(GameEvent ge, EventParameter parameter)
        {

        }
        public void OnStoryStep(GameEvent ge, EventParameter parameter)
        {

        }
        /// <summary>
        /// S->C 副本组列表更新 msgId:8137
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnDungeonGroupUpdate(GameEvent ge, EventParameter parameter)
        {
            MsgData_sDungeonGroupUpdate resp = parameter.msgParameter as MsgData_sDungeonGroupUpdate;
            Debug.Log("副本组列表list数量: " +resp.listCount);
        }
        public void OnDungeonCountDown(GameEvent ge, EventParameter parameter)
        {

        }
        public void OnDungeonPassResult(GameEvent ge, EventParameter parameter)
        {

        }


        */
        /// <summary>
        /// 是否初次加载UI  , 切换场景 创建加载的时候调用会改变  bLoadFirstUI = true
        /// </summary>
        public static bool bLoadFirstUI = false;


        #region 客户端请求

        /// <summary>
        /// 客户端请求 灵兽墓地信息3743
        /// </summary>
        public void Send_CS_HunLingXianYuInfo()
        {
            MsgData_cHunLingXianYuInfo rsp = new MsgData_cHunLingXianYuInfo();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_HunLingXianYuInfo, rsp);
        }
        /// <summary>
        /// 客户端请求 灵兽墓地信息3421
        /// </summary>
        public void Send_CS_LingShouMuDiInfo()
        {
            MsgData_cLingShouMuDiInfo rsp = new MsgData_cLingShouMuDiInfo();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_LingShouMuDiInfo, rsp);
        }


        /// <summary>
        /// 客户端请求 重置灵兽墓地 3744
        /// </summary>
        public void Send_CS_ResetHunLingXianYu()
        {
            MsgData_cResetHunLingXianYu rsp = new MsgData_cResetHunLingXianYu();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ResetHunLingXianYu, rsp);
        }

        /// <summary>
        /// 客户端请求 重置灵兽墓地 3422
        /// </summary>
        public void Send_CS_ResetLingShouMuDi()
        {
            MsgData_cResetLingShouMuDi rsp = new MsgData_cResetLingShouMuDi();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ResetLingShouMuDi, rsp);
        }

        /// <summary>
        /// 客户端请求 挑战灵兽墓地  3745
        /// </summary>
        public void Send_CS_ChallHunLingXianYu(int _layer)
        {
            MsgData_cChallHunLingXianYu rsp = new MsgData_cChallHunLingXianYu();
            rsp.layer = _layer;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ChallHunLingXianYu, rsp);
         //   Debug.Log("发起挑战---幻灵仙域.. " + _layer);
        }

        /// <summary>
        /// 客户端请求 挑战灵兽墓地  3423
        /// </summary>
        public void Send_CS_ChallLingShouMuDi(int _layer)
        {
            MsgData_cChallLingShouMuDi rsp = new MsgData_cChallLingShouMuDi();
            rsp.layer = _layer;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ChallLingShouMuDi, rsp);
      //      Debug.Log("发起挑战---法宝幻境.. " + _layer);
        }
        /// <summary>
        /// 客户端请求 扫荡灵兽墓地  3747
        /// </summary>
        public void Send_CS_ScanHunLingXianYu()
        {
            MsgData_cCanHunLingXianYu rsp = new MsgData_cCanHunLingXianYu();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ScanHunLingXianYu, rsp);
        }

        /// <summary>
        /// 客户端请求 扫荡灵兽墓地  3424
        /// </summary>
        public void Send_CS_ScanLingShouMuDi()
        {
            MsgData_cCanLingShouMuDi rsp = new MsgData_cCanLingShouMuDi();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ScanLingShouMuDi, rsp);
        }


        /// <summary>
        /// 客户端请求 退出灵兽墓地 msgId:3748
        /// </summary>
        public void Send_CS_LingShouMuDiQuit()
        {
            MsgData_cLingShouMuDiQuit rsp = new MsgData_cLingShouMuDiQuit();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_LingShouMuDiQuit, rsp);
        //    Debug.Log("请求退出副本");
        }

        /// <summary>
        /// 客户端请求 退出灵兽墓地 msgId:3748
        /// </summary>
        public void Send_CS_HunLingXianYuQuit()
        {
            MsgData_cHunLingXianYuQuit rsp = new MsgData_cHunLingXianYuQuit();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_HunLingXianYuQuit, rsp);
      //      Debug.Log("请求退出副本");
        }


        /// <summary>
        /// 客户端请求 获得奖励 msgId:3749;
        /// </summary>
        public void Send_CS_HunLingXianYuGetAward()
        {
            MsgData_cLicaiInfoGetAward rsp = new MsgData_cLicaiInfoGetAward();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_HunLingXianYuGetAward, rsp);
        }

        #endregion

        public int FbType = 0;   //当前挑战的副本类型  1.幻灵仙域   2.法宝幻境

        /// <summary>
        /// 进入副本的时间。
        /// </summary>
        private long m_EnterTime;

        /// <summary>
        /// 获取进入副本的时间。
        /// </summary>
        public long EnterTime
        {
            get { return m_EnterTime; }
        }

        /// <summary>
        /// 灵兽墓地信息 -幻灵仙域
        /// </summary>
        public MsgData_sHunLingXianYuInfo rideInfo;

        /// <summary>
        /// 灵兽墓地信息- 法宝
        /// </summary>
        public MsgData_sLingShouMuDiInfo fabaoInfo;

        /// <summary>
        /// 获取排名信息
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MsgData_sLingShouMuDiInfoTop GetFabaoTopInfo(int index)
        {
            if(fabaoInfo.topList.Length >= index)
            {
                return fabaoInfo.topList[index - 1];
            }
            return null;
        }

        #region 服务器返回

        /// <summary>
        /// 服务器通知:灵兽墓地信息 
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnHunlingXianYuInfo(GameEvent ge, EventParameter parameter)
        {
            rideInfo = parameter.msgParameter as MsgData_sHunLingXianYuInfo;
            EventParameter par = EventParameter.Get();
            par.msgParameter = rideInfo;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_HunlingXianYuInfo, par);
        }

        /// <summary>
        /// S->C 返回重置灵兽墓地结果 msgId:8744;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnResetHunLingXianYu(GameEvent ge, EventParameter parameter)
        {
            MsgData_sResetHunLingXianYu resp = parameter.msgParameter as MsgData_sResetHunLingXianYu;
            if(resp.result == 0)
            {
                TaskMgr.Instance.OnDungeonActivyChange(resp.resetNum);
            }
        }

        /// <summary>
        /// S->C 返回进入灵兽墓地结果 msgId:8745;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnChallHunLingXianYu(GameEvent ge, EventParameter parameter)
        {
            MsgData_sChallHunLingXianYu resp = parameter.msgParameter as MsgData_sChallHunLingXianYu;
            if(resp.result == 0)
            {
                FbType = 1;
            }
        }

        /// <summary>
        ///  S->C 挑战灵兽墓地结果 msgId:8746;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnChallHunLingXianYuResult(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sChallHunLingXianYuResult resp = parameter.msgParameter as MsgData_sChallHunLingXianYuResult;
        }
        /// <summary>
        /// 扫荡灵兽墓地结果 msgId:8747;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnScanHunLingXianYuResult(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sCanHunLingXianYuResult resp = parameter.msgParameter as MsgData_sCanHunLingXianYuResult;
        }
        /// <summary>
        ///  S->C 退出灵兽墓地结果 msgId:8748;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnHunLingXianYuQuit(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sHunLingXianYuQuit resp = parameter.msgParameter as MsgData_sHunLingXianYuQuit;
        }
    

        /// <summary>
        /// S->C 获得灵兽墓地结果 msgId:8749;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnHunLingXianYuGetAward(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sHunLingXianYuGetAward resp = parameter.msgParameter as MsgData_sHunLingXianYuGetAward;
        }

        /// <summary>
        /// 服务器通知:灵兽墓地信息 8421
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnLingShouMuDiInfo(GameEvent ge, EventParameter parameter)
        {
            fabaoInfo = parameter.msgParameter as MsgData_sLingShouMuDiInfo;
            EventParameter par = EventParameter.Get();
            par.msgParameter = fabaoInfo;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_LingShouMuDiInfo, par);
        }

        /// <summary>
        /// S->C 返回重置灵兽墓地结果 msgId:8423;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnResetLingShouMuDi(GameEvent ge, EventParameter parameter)
        {
            MsgData_sResetLingShouMuDi resp = parameter.msgParameter as MsgData_sResetLingShouMuDi;
            if(resp.result == 0 )
            {
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_DailyActivy, parameter);   //刷新主界面日常任务栏 魔宠日常活动
            }
        }

        /// <summary>
        /// S->C 返回进入灵兽墓地结果 msgId:8422;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnChallLingShouMuDi(GameEvent ge, EventParameter parameter)
        {
            MsgData_sChallLingShouMuDi resp = parameter.msgParameter as MsgData_sChallLingShouMuDi;
            if(resp.result == 0)
            {
                FbType = 2;
                m_EnterTime = UiUtil.GetNowTimeStamp();
            }
        }

        /// <summary>
        ///  S->C 挑战灵兽墓地结果 msgId:8424;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnChallLingShouMuDiResult(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sChallLingShouMuDiResult resp = parameter.msgParameter as MsgData_sChallLingShouMuDiResult;
        }
        /// <summary>
        /// 扫荡灵兽墓地结果 msgId:8425;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnScanLingShouMuDiResult(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sCanLingShouMuDiResult resp = parameter.msgParameter as MsgData_sCanLingShouMuDiResult;
        }
        /// <summary>
        ///  S->C 退出灵兽墓地结果 msgId:8432;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnLingShouMuDiQuit(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sLingShouMuDiQuit resp = parameter.msgParameter as MsgData_sLingShouMuDiQuit;
        }


        /// <summary>
        /// S->C 获得灵兽墓地奖励结果 msgId:8426;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnLingShouMuDiGetAward(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sLingShouMuDiGetAward resp = parameter.msgParameter as MsgData_sLingShouMuDiGetAward;
        }




        /// <summary>
        ///  S->C 灵兽墓地怪物波数信息 msgId:8750;
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnHunLingXianYuMonsterInfo(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sHunLingXianYuMonsterInfo resp = parameter.msgParameter as MsgData_sHunLingXianYuMonsterInfo;
        }

        public void OnEnterDungeonResult(GameEvent ge, EventParameter parameter)
        {
            MsgData_sEnterDungeonResult resp = parameter.msgParameter as MsgData_sEnterDungeonResult;
            if(resp.result ==0)
            {
                m_EnterTime = UiUtil.GetNowTimeStamp();
            }
        }

        #endregion

        public List<MsgData_sSceneObjectEnterMonster> monsterList = new List<MsgData_sSceneObjectEnterMonster>();

        /// <summary>
        /// 延迟刷怪。 。保存 怪物信息
        /// </summary>
        /// <param name="monster"></param>
        public void AddMonster(MsgData_sSceneObjectEnterMonster monster)
        {
           
            if (DungeonMono.instance == null)
            {
                if(!monsterList.Contains(monster))
                {
                    monsterList.Add(monster);
                }
                return;
            }
            if (!DungeonMono.instance.bStart)       //倒计时未结束时，服务端推的怪物信息 ，先保存
            {
                if (!monsterList.Contains(monster))
                {
                    monsterList.Add(monster);
                }
            }
            else
            {
                DungeonMono.instance.LoadModel(monster);   //倒计时结束后 推的 怪物，直接显示出来
            }
        }
        public int curFbId = 0;
        public DateTime curTime;
        /// <summary>
        /// 客户端请求 进入任务副本 msgId:4933;
        /// </summary>
        public void Send_CS_EnterDungeonQuest(int fbId)
        {
           // Debug.LogError("请求进入副本fbId  "+ fbId);
            curFbId = fbId;
            curTime = DateTime.Now;
            MsgData_cDungeonQuestEnter rsp = new MsgData_cDungeonQuestEnter();
            // CoreEntry.netMgr.send(3747, rsp);
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DungeonQuestEnter, rsp);
            //CoreEntry.netMgr.send(4933, rsp);
            questRsp = null;
            m_EnterTime = UiUtil.GetNowTimeStamp();
        }


        /// <summary>
        /// 客户端请求 退出任务副本 msgId:4934;
        /// </summary>
        public void Send_CS_QuitDungeonQuest()
        {
            MsgData_cDungeonQuestQuit rsp = new MsgData_cDungeonQuestQuit();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DungeonQuestQuit, rsp);
        }
        /// <summary>
        /// 任务副本战斗 msg
        /// </summary>
        public MsgData_sDungeonQuestStateUpdate questRsp = null;
        /// <summary>
        /// 推结算的点的 时间戳
        /// </summary>
        public long resultDateTime = 0;  
        public void OnDungeonQuestUpdate(GameEvent ge, EventParameter parameter)
        {
            questRsp = parameter.msgParameter as MsgData_sDungeonQuestStateUpdate;
            if (questRsp.state == 0)
            {
                resultDateTime = UiUtil.GetNowTimeStamp();
                CoreEntry.gAutoAIMgr.AutoFight = false; 
                TaskMgr.RunTaskType = 1;
            }
            else if(questRsp.state == 1)
            {
                resultDateTime = UiUtil.GetNowTimeStamp();
                CoreEntry.gAutoAIMgr.AutoFight = false;
                TaskMgr.RunTaskType = 1;
            }

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_DungeonQuestUpdate, EventParameter.Get(questRsp));
        }

    }
}


