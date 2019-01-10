/**
* @file     : ResLevelData.cs
* @brief    : 玩家资源副本数据
* @details  : 
* @author   : XuXiang
* @date     : 2017-08-14 11:17
*/

using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;
using System;

namespace SG
{
    /// <summary>
    /// 资源副本数据。
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public class ResLevelData
    {
        /// <summary>
        /// 是否已经初始数据。
        /// </summary>
        private bool mIsInit = false;

        /// <summary>
        /// 获取是否已经初始化数据。
        /// </summary>
        public bool IsInit
        {
            get { return mIsInit; }
        }

        /// <summary>
        /// 初始化后是否显示UI。
        /// </summary>
        private bool mInitShowUI;

        /// <summary>
        /// 获取或设置初始化后是否显示UI。
        /// </summary>
        public bool InitShowUI
        {
            get { return mInitShowUI; }
            set { mInitShowUI = value; }
        }

        /// <summary>
        /// 挑战次数。
        /// </summary>
        private List<int> mEnterNum = new List<int>();

        /// <summary>
        /// 购买次数。
        /// </summary>
        private List<int> mBuyNum = new List<int>();

        /// <summary>
        /// 获取章节挑战次数。
        /// </summary>
        /// <param name="cid">章节编号。</param>
        /// <returns>剩余挑战次数。</returns>
        public int GetChapterNumber(int cid)
        {
            int index = cid - 1;
            if (index < 0 || index >= mEnterNum.Count)
            {
                return 0;
            }
            return mEnterNum[index];
        }

        /// <summary>
        /// 资源副本配置。
        /// </summary>
        public Dictionary<int, LuaTable> m_CacheConfig = null;
        public void InitConfig()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            m_CacheConfig = G.Get<Dictionary<int, LuaTable>>("t_zhiyuanfb");
        }

        /// <summary>
        /// 获取指定资源副本配置。
        /// </summary>
        /// <param name="id">主线ID。</param>
        /// <returns>主线配置。</returns>
        public LuaTable GetMainConfig(int id)
        {
            LuaTable t;
            if(m_CacheConfig == null)
            {
                InitConfig();
            }
            m_CacheConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取章节购买次数。
        /// </summary>
        /// <param name="cid">章节编号。</param>
        /// <returns>购买次数。</returns>
        public int GetChapterBuyNumber(int cid)
        {
            int index = cid - 1;
            if (index < 0 || index >= mBuyNum.Count)
            {
                return 0;
            }
            return mBuyNum[index];
        }

        /// <summary>
        /// 总挑战次数
        /// </summary>
        /// <returns></returns>
        public int GetCfgAllNum()
        {
            int num = 0;
            foreach (var item in mStageInfo)
            {
                LuaTable cg1 = GetMainConfig(item.Value.ID);
                if (null != cg1)
                {
                    int daliyNum = cg1.Get<int>("daliyNum");
                    num = num + daliyNum;
                }
            }
            return num;
        }

        /// <summary>
        /// 总挑战次数。(已打掉的次数)  返回int
        /// </summary>
        /// <returns></returns>
        public int GetAllNum()
        {
            long num = 0;
            for (int i = 0; i < mEnterNum.Count; i++)
            {
                num += mEnterNum[i] - mBuyNum[i];        //减去购买的次数
            }
            return (int)num;
        }

        /// <summary>
        /// 减少章节挑战次数。
        /// </summary>
        /// <param name="id">关卡ID。</param>
        /// <param name="num">次数。</param>
        private void SubChapterNumber(int id, int num)
        {
            int cid = id / 10000;
            int index = cid - 1;
            if (index < 0 || index >= mEnterNum.Count)
            {
                return;
            }
            mEnterNum[index] += num;
        }

        /// <summary>
        /// 添加购买次数。
        /// </summary>
        /// <param name="id">章节ID。</param>
        /// <param name="num">次数。</param>
        private void AddChapterBuyNumber(int id, int num)
        {
            int index = id - 1;
            if (index < 0 || index >= mBuyNum.Count)
            {
                return;
            }
            mBuyNum[index] += num;
        }

        /// <summary>
        /// 章节列表。
        /// </summary>
        private List<MsgData_sVeilvo> mVeilList = new List<MsgData_sVeilvo>();

        /// <summary>
        /// 获取章节列表。
        /// </summary>
        public List<MsgData_sVeilvo> VeilList
        {
            get { return mVeilList; }
        }

        /// <summary>
        /// 副本信息集合。
        /// </summary>
        private Dictionary<int, MsgData_sStagevo> mStageInfo = new Dictionary<int, MsgData_sStagevo>();

        /// <summary>
        /// 获取副本信息集合。
        /// </summary>
        public Dictionary<int, MsgData_sStagevo> StageInfo
        {
            get { return mStageInfo; }
        }

        /// <summary>
        /// 获取副本信息。
        /// </summary>
        /// <param name="id">副本编号。</param>
        /// <returns>副本信息。</returns>
        public MsgData_sStagevo GetStageInfo(int id)
        {
            MsgData_sStagevo info;
            mStageInfo.TryGetValue(id, out info);
            return info;
        }
        /// <summary>
        /// 获取关卡剩余挑战次数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetStateNumber(int id)
        {
            int num = 0;
            MsgData_sStagevo info =GetStageInfo(id);
            LuaTable cfg = GetMainConfig(id);
            int totalnum = 0;
            if(cfg != null)
            {
                //int levellow = cfg.Get<int>("levellow");
                //int levelup = cfg.Get<int>("levelup");
                //int level = PlayerData.Instance.BaseAttr.Level;
                //if(levellow <= level && level <= levelup)
                {
                    totalnum = cfg.Get<int>("daliyNum");
                }
            }
            if(info != null)
            {
                num = totalnum - info.Num;
            }
            num = Math.Max(num, 0);
            return num;
        }

        /// <summary>
        /// 当前挑战的副本ID。
        /// </summary>
        private int mCurID;

        /// <summary>
        /// 获取当前挑战的副本ID。
        /// </summary>
        public int CurID
        {
            get { return mCurID; }
        }

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
        /// 注册网络消息。
        /// </summary>
        public void RegisterNetMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, OnCleanupUserData);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_RESLEVEL_INIT, OnInitData);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_RESLEVEL_CHALLENGE, OnFight); 
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_RESLEVEL_END, OnLevelEnd); 
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_RESLEVEL_REFRESH, OnLevelRefresh); 
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_RESLEVEL_SWEEP, OnLevelSweep);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_RESLEVEL_SWEEP_END, OnLevelSweepEnd);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_RESLEVEL_BUY_, OnBuyNumber);
        }

        /// <summary>
        ///  切换账号清除数据
        /// </summary>
        private void OnCleanupUserData(GameEvent ge, EventParameter parameter)
        {
            mIsInit = false;
            mInitShowUI = false;
            mEnterNum.Clear();
            mBuyNum.Clear();
            mVeilList.Clear();
            mStageInfo.Clear();
            mCurID = 0;
            m_EnterTime = 0;
        }

        /// <summary>
        /// 副本信息。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnInitData(MsgData data)
        {
            MsgData_sZhiYuanFbData info = data as MsgData_sZhiYuanFbData;
            mIsInit = true;
            string str = UiUtil.GetNetString(info.EnterNumList);
            string[] numstr = str.Split(',');
            mEnterNum.Clear();
            mBuyNum.Clear();
            for (int i = 0; i < numstr.Length; ++i)
            {
                string nstr = numstr[i].Trim();
                int num;
                if (int.TryParse(nstr, out num))
                {
                    mEnterNum.Add(num % 16);
                    mBuyNum.Add(num / 16);
                }
            }
            mVeilList.Clear();
            mVeilList.AddRange(info.VeilList);
            mStageInfo.Clear();
            for (int i = 0; i < info.StageList.Count; ++i)
            {
                MsgData_sStagevo stage = info.StageList[i];
                mStageInfo.Add(stage.ID, stage);
            }

            //触发事件
            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_RESLEVEL_INIT, ep);

            //显示UI
            if (mInitShowUI)
            {
                mInitShowUI = false;
                MainPanelMgr.Instance.ShowPanel("UIResLevel");
            }
        }

        /// <summary>
        /// 副本挑战。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnFight(MsgData data)
        {
            MsgData_sBackZhiYuanFbChallenge info = data as MsgData_sBackZhiYuanFbChallenge;
            mCurID = info.ID;
            m_EnterTime = UiUtil.GetNowTimeStamp();            

            //挑战通知
            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            ep.intParameter1 = mCurID;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_RESLEVEL_FIGHT, ep);
        }

        /// <summary>
        /// 副本结束。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnLevelEnd(MsgData data)
        {
            MsgData_sBackZhiYuanFbEnd info = data as MsgData_sBackZhiYuanFbEnd;
            LogMgr.LogWarning("OnLevelEnd result:{0} level:{1} mCurID:{2}", info.Result, info.Level, mCurID);
            if (mCurID == 0)
            {
                //会收到多条结束消息
                return;
            }

            //成功才扣次数
            if (info.Result == 1)
            {
                SubChapterNumber(mCurID, 1);
            }

            //修改信息
            int id = mCurID;
            MsgData_sStagevo stage = GetStageInfo(mCurID);
            bool first = stage == null;
            if (first)
            {
                stage = new MsgData_sStagevo();
                stage.ID = mCurID;
                mStageInfo.Add(stage.ID, stage);
            }
            if(stage.Evaluate <= 0)
            {
                first = true;
            }
            stage.Evaluate = info.Level;            
            mCurID = 0;
            CoreEntry.gAutoAIMgr.AutoFight = false;

            //挑战通知
            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            ep.intParameter1 = id;
            ep.intParameter2 = first ? 1 : 0;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_RESLEVEL_END, ep);
        }

        /// <summary>
        /// 副本刷新。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnLevelRefresh(MsgData data)
        {
            MsgData_sZhiYuanFbUpDate info = data as MsgData_sZhiYuanFbUpDate;
            MsgData_sStagevo stage = GetStageInfo(info.ID);
            if (stage == null)
            {
                stage = new MsgData_sStagevo();
                stage.ID = info.ID;
                mStageInfo.Add(stage.ID, stage);
            }
            stage.Num = info.Num;
            stage.State = info.State;
            stage.TimeNum = info.TimeNum;
            stage.RewardType = info.RewardType;

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.ID;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_RESLEVEL_REFRESH, ep);
        }

        /// <summary>
        /// 副本扫荡。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnLevelSweep(MsgData data)
        {
            MsgData_sBackZhiYuanFbWipe info = data as MsgData_sBackZhiYuanFbWipe;
            LogMgr.Log(string.Format("扫荡回复 result:{0} id:{1} num:{2}", info.Result, info.ID, info.Num));
            if (info.Result == 0)
            {
                SubChapterNumber(info.ID, info.Num);
            }

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            ep.intParameter1 = info.ID;
            ep.intParameter2 = info.Num;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_RESLEVEL_SWEEP, ep);
        }

        /// <summary>
        /// 副本扫荡。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnLevelSweepEnd(MsgData data)
        {
            MsgData_sBackZhiYuanFbMopupEnd info = data as MsgData_sBackZhiYuanFbMopupEnd;
            LogMgr.Log(string.Format("扫荡完成 result:{0} id:{1} num:{2}", info.Result, info.ID, info.Num));

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            ep.intParameter1 = info.ID;
            ep.intParameter2 = info.Num;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_RESLEVEL_SWEEP_END, ep);
        }

        /// <summary>
        /// 购买次数。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnBuyNumber(MsgData data)
        {
            MsgData_sBackZhiYuanFbVigor info = data as MsgData_sBackZhiYuanFbVigor;
            //if (info.Result == 0)
            //{
            //    AddChapterBuyNumber(info.CID, 1);
            //}

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.Result;
            ep.intParameter1 = info.CID;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_RESLEVEL_BUY_NUMBER, ep);
        }

        /// <summary>
        /// 发送初始化信息请求。 
        /// </summary>
        public void SendInitRequest()
        {
            MsgData_cZhiYuanFb data = new MsgData_cZhiYuanFb();
            CoreEntry.netMgr.send(NetMsgDef.C_RESLEVEL_INIT, data);
        }

        /// <summary>
        /// 发送挑战请求。
        /// </summary>
        /// <param name="id">副本编号。</param>
        public void SendFightRequest(int id)
        {
            MsgData_cZhiYuanFbChallenge data = new MsgData_cZhiYuanFbChallenge();
            data.ID = id;
            CoreEntry.netMgr.send(NetMsgDef.C_RESLEVEL_CHALLENGE, data);
        }

        /// <summary>
        /// 发送扫荡请求。
        /// </summary>
        /// <param name="id">副本编号。</param>
        /// <param name="num">扫荡次数。</param>
        public void SendSweepRequest(int id, int num)
        {
            MsgData_cZhiYuanFbWipe data = new MsgData_cZhiYuanFbWipe();
            data.ID = id;
            data.Num = num;
            CoreEntry.netMgr.send(NetMsgDef.C_RESLEVEL_SWEEP, data);
        }

        /// <summary>
        /// 发送退出副本请求。
        /// </summary>
        public void SendQuitRequest()
        {
            MsgData_cZhiYuanFbQuit data = new MsgData_cZhiYuanFbQuit();
            CoreEntry.netMgr.send(NetMsgDef.C_RESLEVEL_QUIT, data);
        }

        /// <summary>
        /// 发送购买次数请求。
        /// </summary>
        /// <param name="id">购买的章节编号。</param>
        public void SendBuyNumberRequest(int id)
        {
            MsgData_cZhiYuanFbVigor data = new MsgData_cZhiYuanFbVigor();
            data.CID = id;
            CoreEntry.netMgr.send(NetMsgDef.C_RESLEVEL_BUY_, data);
        }
    }
}