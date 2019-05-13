using XLua;
﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace SG  
{
	// 断线重连
[Hotfix]
    public class NetReconnect
    {
        private const int MaxAutoReconnectTime = 6;//最大自动重连次数
        private const int ReconnectTimeGap = 3; //秒
        public NetClientThread netClientThread = null;

        private float passTime = 0;
        public bool bReconnect = false;//是否重连
        private float ReconnectTime = 0;                    // 重连时间
        private float DisconnectionTime = 0;                // 断线时间
        private float PressButtonTime = 0f;  //按按钮的时间
        public int AutoReconnectCount = 0;               // 自动重连次数
#pragma warning disable 0414

        private bool IsShowReconnectedDialog = false;
        //private bool IsCountinueToReconnecting = false;
        private int tryDialogTimes = 0;

        public bool needShowNotice = false;

        public NetReconnect()
        {
            BindConnect();
        }

        public void BindConnect()
        {
            CoreEntry.netMgr.bindConnectClose(onConnectedClose);
            CoreEntry.netMgr.bindConnectError(onConnectedError);
        }

        private bool isLoadScene = false;
        private bool disconnectInLoad = false;
        public void RegisterNetMsg()
        {
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_RECONNECT, RespReconnect);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BEGIN_LOADSCENE, BeginLoadScene);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_LOADSCENE_FINISH, EndLoadScene);
        }

        private void BeginLoadScene(GameEvent ge, EventParameter parameter)
        {
            isLoadScene = true;
        }

        private void EndLoadScene(GameEvent ge, EventParameter parameter)
        {
            isLoadScene = false;
            if(disconnectInLoad)
            {
                disconnectInLoad = false;

                //IsShowReconnectedDialog = true;
                UITips.ShowTipsDialog("提示", "您已经断开连接,请重新登录", "确定", RequestToLogin, null, RequestToLogin);
            }
        }

        /// <summary>
        /// 是否在重连状态下
        /// </summary>
        /// <returns></returns>
        public bool IsStartReconnect()
        {
            return bReconnect;
        }

        /// <summary>
        /// 开始重连
        /// </summary>
        public void StartReconnect()
        {
            if (!bReconnect && !IsTipsDialogShow())
            {
                MainPanelMgr.Instance.HideDialog("UICommonTk");
                //MainPanelMgr.Instance.ShowDialog("UIWaitDialog");
                bReconnect = true;
                ReconnectTime = 0;
                DisconnectionTime = 0;
                AutoReconnectCount = MaxAutoReconnectTime; // 重连次数
                passTime = Time.time;

                // 开始断线重连的消息
                EventParameter parameter = EventParameter.Get();
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EVENT_NET_RECONNECT_START, parameter);
            }
        }

        private void Clear()
        {
            //IsCountinueToReconnecting = false;
            IsShowReconnectedDialog = false;
            tryDialogTimes = 0;
            PressButtonTime = 0;
            needShowNotice = false;
        }

        public void Update()
        {
            if (!bReconnect)
                return;

            // 连接上了
            if (netClientThread != null && netClientThread.IsConnectted())
            {
                bReconnect = false;
                Clear();

                ReqReconnect();

                return;
            }

            SpecialCondition();


            //if (AutoReconnectCount > 0)//自动重连自次数内
            //{
            //    if (ReconnectTime > ReconnectTimeGap)//超过单次重连时间
            //    {
            //        ReconnectTime = 0;
            //        AutoReconnectCount--;
            //        if (AutoReconnectCount > 0)
            //        {
            //            LogMgr.UnityError(string.Format("auto reconnect time:{0}th", AutoReconnectCount));
            //            NetLogicGame.Instance.connectGame(netClientThread.m_serverIP, (short)netClientThread.m_serverPort);
            //        }
            //    }
            //    else if (ReconnectTime == 0f)
            //    {
            //        LogMgr.UnityError(string.Format("auto reconnect time:{0}th", AutoReconnectCount));
            //        ReconnectTime = 0;
            //        NetLogicGame.Instance.connectGame(netClientThread.m_serverIP, (short)netClientThread.m_serverPort);
            //    }
            //}
            //else if (!IsShowReconnectedDialog)
            //{
            //    IsShowReconnectedDialog = true;
            //    MainPanelMgr.Instance.HideDialog("UIWaitDialog");
            //    UITips.ShowTipsDialog("重新连接", "您已经掉线，是否重新连接?", "重新连接", onBtnReconnect, "返回登录", RequestToLogin);
            //}
            if (isLoadScene)
            {
                disconnectInLoad = true;
            }
            else if (!IsTipsDialogShow() && !NetLogicGame.Instance.IsCrossLinking)
            {
                if (needShowNotice)
                {
                    IsShowReconnectedDialog = true;
                    UITips.ShowTipSinginDlg("提示", "您已经断开连接,请重新登录", RequestToLogin);
                }
                else {
                    RequestToLogin();
                }
            }

            float deltaTime = Time.time - passTime;
            ReconnectTime += deltaTime;
            DisconnectionTime += deltaTime;
            PressButtonTime += deltaTime;
            passTime = Time.time;
        }

        private bool IsTipsDialogShow()
        {
            PanelBase pb = MainPanelMgr.Instance.GetDialog("UICommonTk");
            if (pb != null)
            {
                return pb.IsShown;
            }

            return false;
        }

        /// <summary>
        /// 某些模式下只能直接退出到登录框dui
        /// </summary>
        void SpecialCondition()
        {
        }

        public void onConnectedClose()
        {
            ;
        }

        public void onConnectedError(int errorCode)
        {
            if (!bReconnect)//重连状态下
            {
                if (errorCode == 10051 && MapMgr.Instance.CurMapType != MapMgr.MapType.Map_Login && MapMgr.Instance.CurMapType != MapMgr.MapType.Map_None)
                {
                    LogMgr.LogError("onConnectedError : StartReconnect");
                    StartReconnect();
                    IsShowReconnectedDialog = false;
                }
            }
        }

        public void OnTestReconnect()
        {
            IsShowReconnectedDialog = true;
        }

        // 重连
        private void onBtnReconnect()
        {
            ReconnectTime = 0;
            AutoReconnectCount = MaxAutoReconnectTime;
            bReconnect = true;
            tryDialogTimes++;
            IsShowReconnectedDialog = false;
            PressButtonTime = 0f;

            MainPanelMgr.Instance.HideDialog("UICommonTk");
            MainPanelMgr.Instance.ShowDialog("UIWaitDialog");
        }

        void RequestToLogin()
        {
            //if (ClientSetting.Instance.GetBoolValue("useThirdPartyLogin") && ClientSetting.Instance.GetIntValue("thirdPartyComponent") == 1
            //    && XYSDK.Instance.hasLogout())
            //{
            //    SDKMgr.Instance.Logout();
            //}
            //else
            {
                onBtnGotoLogin();
            }
        }

        // 返回登陆界面
        public void onBtnGotoLogin()
        {
            //Debug.Log("onBtnGotoLogin 1");
            MainPanelMgr.Instance.DestroyPanel("UIVip");
            MainPanelMgr.Instance.HideAllDialog();
            MainPanelMgr.Instance.HideAllPanel(true);
            //Debug.Log("onBtnGotoLogin 2");

            // 返回到登陆界面就不需要重连了
            bReconnect = false;
            Clear();
            //Debug.Log("onBtnGotoLogin 3");

            ClearData();
            //Debug.Log("onBtnGotoLogin 4");

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CLEANUP_USER_DATA, null);
            //Debug.Log("onBtnGotoLogin 5");

            CoreEntry.gGameMgr.StartCoroutine("DelayExit");
            //MapMgr.Instance.EnterLoginScene();
        }

        void ClearData()
        {
            MainPanelMgr.Instance.Release();
            MainRole.Instance.ClearData();
            //退出时清空object缓存
            if (CoreEntry.gObjPoolMgr != null)
            {
                CoreEntry.gObjPoolMgr.ReleaseObjectPool();
            }

            if (CoreEntry.netMgr != null)
            {
                CoreEntry.netMgr.ResetLoginStaus();
            }
        }

        /// <summary>
        /// 请求重连消息
        /// </summary>
        private void ReqReconnect()
        {
            byte state = (byte)MapMgr.Instance.CurMapType;
            if(state < 0 || state > 2)
            {
                LogMgr.UnityError(string.Format("filtered map state:{0}", state));

                return;
            }

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OBJ_CLEAR, null);

            return;
            /*
            MsgData_cReconnect data = new MsgData_cReconnect();
            data.Account = Account.Instance.AccountIDBytes;
            data.Cookie = Account.Instance.Cookie;
            data.Status = (sbyte)state;
            data.ServerID = Account.Instance.ServerId;

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_RECONNECT, data);
            */
        }

        /// <summary>
        /// 收到重连消息
        /// </summary>
        /// <param name="msg"></param>
        private void RespReconnect(MsgData msg)
        {
            MainPanelMgr.Instance.HideDialog("UIWaitDialog");
            MsgData_sReconnect data = msg as MsgData_sReconnect;
            if (null == data)
            {
                return;
            }

            if (data.Result == 0)//重连成功
            {
                UITips.ReconnectSwitchLine();
            }
            else if (data.Result == 1)//认证失败
            {
                UITips.ShowTipsDialog("提示", "认证失败,请重新登录", "确定", RequestToLogin, null, null);
            }
            else if (data.Result == 2)//重连失败
            {
                UITips.ShowTipsDialog("提示", "您已经断开连接,请重新登录", "确定", RequestToLogin, null, null);
            }
            else if (data.Result == 3)
            {
                UITips.ShowTipsDialog("提示", "连接失败,请重新登录", "确定", RequestToLogin, null, null);
            }
        }
    }
}

