/**
* @file     : NetMgr.cs
* @brief    : login page connect to session gate(session server)
* 			  actor page connect to db gate(db server)
* 			  game page connect to logic gate(logic server)
* @details  : client net mgr
* @author   : liweibin
* @date     : 2015-1-21 20:50
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using XLua;
#if UNITY_EDITOR
using System.IO;
#endif

namespace SG
{
[Hotfix]
    [LuaCallCSharp]
    public class NetMgr : MonoBehaviour
    {

        public enum ConnectInfo
        {
            connectlogin = 0,
            loginok,
            loginfailed,
            cannotlogin,
            connectactor,
            actorok,
            connectgame,
            gameok,
            connectedlogin_Successfull,
            connecttimeout,
        }

        public enum Page
        {
            login = 0,
            actor = 1,
            game = 2,
        }

        private NetClientThread m_net = null;
        public int m_page = 0;  // 0:login 1:actor 2:game
        private int m_actorId = 0;

        public static float HEAR_BEAT_GAP = 5;                          //心跳发送间隔
        public static float HEAR_BEAT_TIME_OUT = 10;                    //心跳超时
        public static MsgData_cHeartBeat CacheSendHeartBeat = new MsgData_cHeartBeat();         //缓存心跳发送数据对象
        public static MsgData_sHeartBeat CacheRecvHeartBeat = new MsgData_sHeartBeat();         //缓存心跳接收数据对象

        private List<int> m_HearBeatDelay = new List<int>();            //最近几次的心跳延时。
        private int m_HearBeatState = 0;                                //0心跳间隔计时 1等待心跳回复
        private int m_HearBeatTimeOutCount = 0;                         //心跳连续超时统计
        private float m_HearBeatGapCount = 0;                           //心跳间隔计时，小于等于0后发送心跳
        private float m_HearBeatSendTimeStamp = 0;                      //发送心跳的时间戳
        private long m_HearBeatSendTimeTicks = 0;                       //发送心跳的时间戳
        private long m_HearBeatLastTime = 0;                            //最后一次发送心跳的时间

        public static float RECORD_GAP = 40;                            //记录间隔
        private float m_StartRecordTime = 0;                            //开始记录网络流量的时间

        /// <summary>
        /// 获取延时列表。
        /// </summary>
        public List<int> HearBeatDelay
        {
            get { return m_HearBeatDelay; }
        }

        /// <summary>
        /// 添加心跳延时记录。
        /// </summary>
        /// <param name="delay">延时。</param>
        public void AddHearBeatDelay(int delay)
        {
            m_HearBeatDelay.Add(delay);
            while (m_HearBeatDelay.Count > 5)       //存最近几次心跳延时
            {
                m_HearBeatDelay.RemoveAt(0);
            }
        }

        /// <summary>
        /// 收到心跳包。
        /// </summary>
        /// <param name="tick">接收时间。</param>
        /// <param name="info">数据协议。</param>
        public void OnReceiveHearBeat(long tick, MsgData_sHeartBeat info)
        {
            //超时的心跳
            if (info.Time < m_HearBeatLastTime)
            {
                return;
            }
            
            long delay = tick - m_HearBeatSendTimeTicks;
            m_HearBeatState = 0;
            m_HearBeatGapCount = HEAR_BEAT_GAP;
            AddHearBeatDelay((int)(delay / 10000));
            m_HearBeatTimeOutCount = 0;
        }

        public void setActorId(int id)
        {
            m_actorId = id;
        }
        public int getActorId()
        {
            return m_actorId;
        }
        public void setAccountId(int id)
        {
            m_net.m_accountId = id;
        }
        public int getAccountId()
        {
            return m_net.m_accountId;
        }

        private NetReconnect m_reconnect = null;

        public NetReconnect Reconnect
        {
            get
            {
                return m_reconnect;
            }
        }

        public List<Action> MainThreadFun = new List<Action>();

        // Use this for initialization
        void Start()
        {
            LogMgr.UnityLog("===== NetMgr start !!! ");

            m_reconnect = new NetReconnect();
            m_reconnect.netClientThread = null;
            // 各系统注册网路消息
            //MainRole.Instance.RegisterNetMsg();
        }
        void OnDestroy()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                disconnect();
            }
        }
        void Heartbeat()
        {
            // NetLogicGame.Instance.sendHeartbeat();
            Invoke("Heartbeat", 10);
        }

        // Update is called once per frame
        void Update()
        {

            //Debug.LogError("===== NetMgr update !!! ");
            if (m_net != null)
            {
                m_net.update();

                if (MainThreadFun.Count > 0)
                {
                    for (int i = 0; i < MainThreadFun.Count; i++)
                    {
                        var child = MainThreadFun[i];
                        child();
                    }
                    MainThreadFun.Clear();
                }
                
                if (m_page == 2)
                {
                    if (m_HearBeatState == 0)
                    {
                        m_HearBeatGapCount -= Time.deltaTime;
                        if (m_HearBeatGapCount <= 0)
                        {
                            m_HearBeatState = 1;
                            m_HearBeatSendTimeStamp = Time.realtimeSinceStartup;
                            m_HearBeatSendTimeTicks = DateTime.Now.Ticks;
                            m_HearBeatLastTime = UiUtil.GetNowTimeStamp();

                            CacheSendHeartBeat.Time = m_HearBeatLastTime;
                            CoreEntry.netMgr.send((Int16)NetMsgDef.C_HEART_BEAT, CacheSendHeartBeat);
                        }
                    }
                    else if (m_HearBeatState == 1)
                    {
                        if (Time.realtimeSinceStartup - m_HearBeatSendTimeStamp > HEAR_BEAT_TIME_OUT)
                        {
                            m_HearBeatState = 0;                            
                            m_HearBeatGapCount = 0;     //超时后直接重发
                            //AddHearBeatDelay((int)(HEAR_BEAT_TIME_OUT * 1000));
                            ++m_HearBeatTimeOutCount;
                            //LogMgr.LogError("心跳超时 n:{0}", m_HearBeatTimeOutCount);
                        }
                    }
                }

#if UNITY_EDITOR
                if (m_net.m_recordNet && Time.realtimeSinceStartup - m_StartRecordTime >= RECORD_GAP)
                {
                    int sent, recv;
                    lock (m_net)
                    {
                        sent = m_net.m_byteSent;
                        recv = m_net.m_byteRecv;
                        m_net.m_byteRecv = 0;
                        m_net.m_byteSent = 0;
                    }
                    
                    m_StartRecordTime = Time.realtimeSinceStartup;
                    SaveNetFlow(sent, recv);
                }
#endif
            }
        }

        /// <summary>
        /// 将线程添加到主线程执行
        /// </summary>
        public void AddToMainThreadFun(Action fun)
        {
            MainThreadFun.Add(fun);
        }

        //
        public void onConnected(bool isSuccessfull, IPEndPoint remote)
        {
            if (isSuccessfull == false)
            {
                LogMgr.LogError("===== connect server failed !!! " + m_page + " address:" + remote.ToString());
            }
            else
            {
                LogMgr.UnityLog("===== connect server success !!! " + m_page + " address:" + remote.ToString());
            }
        }

        public void bindConnect(delegateNetConnected cb)
        {
            if (m_net != null)
            {
                m_net.onConnect -= cb;
                m_net.onConnect += cb;
            }
        }
        public void bindConnectClose(delegateNetClose cb)
        {
            if (m_net != null)
            {
                m_net.onClose -= cb;
                m_net.onClose += cb;
            }
        }
        public void bindConnectError(delegateNetError cb) // add by yuxj
        {
            if (m_net != null)
            {
                m_net.onError -= cb;
                m_net.onError += cb;
            }
        }

        public void removeConnectCB(delegateNetConnected cb)
        {
            if (m_net != null) m_net.onConnect -= cb;
        }
        public void removeConnectCloseCB(delegateNetClose cb)
        {
            if (m_net != null) m_net.onClose -= cb;
        }
        public void removeConnectErrorCB(delegateNetError cb)
        {
            if (m_net != null) m_net.onError -= cb;
        }
        public void addConnectCB(delegateNetConnected cb)
        {
            if (m_net != null) m_net.onConnect += cb;
        }
        public void addConnectCloseCB(delegateNetClose cb)
        {
            if (m_net != null) m_net.onClose += cb;
        }
        public void addConnectErrorCB(delegateNetError cb)
        {
            if (m_net != null) m_net.onError += cb;
        }

        public bool IsLogin()
        {
            return (m_page == (int)Page.game);
        }

        public void ResetLoginStaus()
        {
            m_page = 0;
        }

        public void disconnect()
        {
            if (m_net == null)
            {
                return;
            }
            m_net.Disconnect();
        }

        public string getSettedHost() { return m_net.m_serverIP; }
        public int getSettedPort() { return m_net.m_serverPort; }
        public void forceClose() { m_net.ForceClose(); }

        //
        public void connect(int page, string serverip, int port)
        {
            //if(m_net!=null)
            //{
            //    if(m_net.IsConnectted() == false)
            //    {
            //        m_net.Disconnect();
            //        m_net = null;
            //    }
            //}
            if (m_net == null)
            {
                m_net = new NetClientThread();                
                m_net.onConnect += onConnected;
                StartRecordingNet();
            }
            
            m_page = page;
            m_net.m_serverIP = Dns.GetHostAddresses(serverip)[0].ToString();
            m_net.m_serverPort = port;
            m_net.ConnectAsyncWithIp();

            switch (page)
            {
                case 0:
                    registerMsgType_login.init();
                    break;
                case 1:
                    registerMsgType_actor.init();
                    m_net.m_reconnect = m_reconnect;
                    m_net.m_reconnect.netClientThread = m_net;
                    break;
                case 2:
                    registerMsgType_game.init();
                    m_net.m_reconnect = m_reconnect;
                    m_net.m_reconnect.netClientThread = m_net;
                    m_HearBeatDelay.Clear();
                    break;
            }
        }

        public void send(Int16 cmd, MsgData data = null)
        {
            if (m_net != null)
            {
                NetWriteBuffer buff = NetWriteBuffer.CacheBuff;
                if (data != null)
                {
                    data.pack(buff);
                }
                
                m_net.send((Int16)(cmd), buff);
            }
        }

        [CSharpCallLua]
        public delegate void PackCall(LuaTable tb, NetWriteBuffer buff);

        [LuaCallCSharp]
        public void send(Int16 cmd, LuaTable table)
        {
            if (m_net != null)
            {
                NetWriteBuffer buff = NetWriteBuffer.CacheBuff;
                PackCall fun = table.Get<PackCall>("Pack");
                fun(table, buff);
                m_net.send((Int16)(cmd), buff);
            }
        }

        public bool registerMsgType(Int16 cmd, Type tp)
        {
            if (m_net != null)
            {
                return m_net.registerMsgType((Int16)(cmd), tp);
            }
            return false;
        }
        public void bindMsgHandler(Int16 cmd, MsgHandler handler)
        {
            if (m_net != null)
            {
                m_net.bindMsgHandler((Int16)(cmd), handler);
            }
        }

        public void unbindMsgHandler(Int16 cmd)
        {
            if (m_net != null)
            {
                m_net.unbindMsgHandler((Int16)(cmd));
            }
        }

        public void bindMsgExHandler(Int16 cmd, MsgExHandler handler)
        {
            if (m_net != null)
            {
                m_net.bindMsgExHandler((Int16)(cmd), handler);
            }
        }

        public void unbindMsgExHandler(Int16 cmd)
        {
            if (m_net != null)
            {
                m_net.unbindMsgExHandler((Int16)(cmd));
            }
        }

        public void StartRecordingNet()
        {
            if (m_net != null && !m_net.m_recordNet)
            {
                m_StartRecordTime = Time.realtimeSinceStartup;
                m_net.m_recordNet = true;
                m_net.m_byteRecv = 0;
                m_net.m_byteSent = 0;
            }
        }

        public void StopRecordingNet()
        {
            if (m_net != null && m_net.m_recordNet)
            {
                m_net.m_recordNet = false;
            }
        }

        public void SaveNetFlow(int sent, int recv)
        {
#if UNITY_EDITOR
            float sentflow = sent / RECORD_GAP;
            float recvflow = recv / RECORD_GAP;
            string t = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string msg = string.Format("{2} down:{0}B/s      up:{1}B/s", UiUtil.NumberToString((int)recvflow, 6), UiUtil.NumberToString((int)sentflow, 6), t);
            LogMgr.Log(msg);

            //追加到文件
            string file = Application.dataPath + "/../Net.log";
            FileStream stream = new FileStream(file, FileMode.OpenOrCreate);
            stream.Position = stream.Length;            //跳到末尾
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(msg);
            writer.WriteLine();
            writer.Flush();
            writer.Close();
            writer.Dispose();
            writer = null;
            stream = null;
#endif
        }        
    }
}

