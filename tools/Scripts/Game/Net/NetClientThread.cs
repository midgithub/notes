#define NETMGR_USE_IP_FOR_CON

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections;
using System.IO;
using XLua;

namespace SG
{
    public delegate void delegateNetConnected(bool isSuccessfull, IPEndPoint remote);
    public delegate void delegateNetClose();
    public delegate void delegateNetError(int errorCode);
    public delegate void MsgHandler(MsgData msg);
    public delegate void MsgExHandler(NetReadBuffer buffer);

    [CSharpCallLua]
    public delegate LuaTable DispatchMsgCall(LuaTable tb, short cmd, NetReadBuffer buffer);

[Hotfix]
    public abstract class MsgData
    {
        public virtual void reset() { }

        public virtual void pack(NetWriteBuffer buffer) {}

        public virtual void unpack(NetReadBuffer buffer) { }
    }


[Hotfix]
    public class NetClientThread
    {
        //protected class MsgHandleEvent
        //{
        //    public event MsgHandler handlers;
        //    public void onMsg(MsgData data)
        //    {
        //        handlers(data);
        //    }
        //}

        public const int TcpClientReceiveBufferSize = 1024 * 32;
        public const int TcpClientReceiveTimeout = 10000;

        public const int TcpClientSendBufferSize = 1024 * 32;
        public const int TcpClientSendTimeout = 10000;

        public static CachePool<SendBuffer> cacheSendMsgs;
        public static CachePool<RecvBuffer> cacheRecvMsgs;

        private Queue<RecvBuffer> recvMsgs;        
        private Queue<SendBuffer> sendMsgs;
        private Dictionary<Int16, Type> msgTypes;
        private Dictionary<Int16, MsgHandler> msgHandlers;
        private Dictionary<Int16, MsgExHandler> msgExHandlers;
        public NetEncrypt m_netEncrypt;

        public event delegateNetClose onClose;
        public event delegateNetConnected onConnect;
        public event delegateNetError onError; // add by yuxj

        public string m_serverNetUrl = "";
        public string m_serverIP;
        public int m_serverPort;
        public int m_sessionKey;
        public int m_accountId;

        public bool m_recordNet = false;
        public int m_byteSent = 0;
        public int m_byteRecv = 0;

        public NetClientThread()
        {
            msgExHandlers = new Dictionary<short, MsgExHandler>();
            msgHandlers = new Dictionary<Int16, MsgHandler>();
            msgTypes = new Dictionary<Int16, Type>();
            recvMsgs = new Queue<RecvBuffer>();
            sendMsgs = new Queue<SendBuffer>();
            m_netEncrypt = new NetEncrypt();
            cacheSendMsgs = new CachePool<SendBuffer>();
            cacheRecvMsgs = new CachePool<RecvBuffer>(30);
            Start();
        }

        /// <summary>
        /// 连接建立尝试
        /// </summary>
[Hotfix]
        private class ConnectTry
        {
            //对于使用域名的时候 !NETMGR_USE_IP_FOR_CON
            public Queue<IPAddress> remoteHost = new Queue<IPAddress>();

            //对于使用ip的时候 NETMGR_USE_IP_FOR_CON
            public int tryCount = 3;
        }
        private bool m_prepareConnect = false;      // 是否准备开始连接
        private bool m_isConnectted = false;        // 是否已连接
        public bool IsConnectted()
        {
            return m_isConnectted;
        }

        private ConnectTry m_currentTry = null;     // 当前的连接尝试


        private TcpClient m_tcpClient = null;
        private ReceiverThread m_receiver;
        private SenderThread m_sender;
        private ConnectThread m_connector = null;

        public NetReconnect m_reconnect = null;

        private bool IsTcpClientConnected()
        {
            bool ret = false;
            if (m_tcpClient != null && m_tcpClient.Client !=null) {
                ret = m_tcpClient.Connected;
            }
            return ret;
        }

        void Start()
        {
            LogMgr.UnityLog("Start");
            m_isConnectted = false;
            //m_tcpClient = new TcpClient();

        }

        void OnDestroy()
        {
            Disconnect();
        }

        void PrepareToConnect()
        {
            //   LogMgr.LogError("PrepareToConnect");
            m_prepareConnect = true;
            m_connector = null;
        }

        private float updatetime = 0;
        int checkCounter = 0;
        public void update()
        {
            updatetime += RealTime.deltaTime;
            if (updatetime < 0.1)
            {
                return;
            }
            updatetime = 0;

            if (m_reconnect != null)
                m_reconnect.Update();

            if (m_prepareConnect && m_connector == null)
            {
                m_prepareConnect = false;
#if (NETMGR_USE_IP_FOR_CON)
                _ConnectAsyncWithIp();
#else
                _ConnectAsyncWithUrl();
#endif
            }

            if (m_connector != null)
            {
                float time = Time.time;

                if (m_connector.CheckTimeOut(time))
                {
                    m_connector.Abort();
                    PrepareToConnect();
                    m_connector = null;
                    //   LogMgr.LogError("Connect Time out");
                    return;
                }

                if (m_connector.IsError())
                {
                    m_connector.WaitTermination();
                    string msg = m_connector.ErrorMsg();
                    LogMgr.LogError("Connect error:" + msg);
                    m_connector = null;
                    PrepareToConnect();
                    return;
                }

                if (m_connector.IsConnected() == false)
                {
                    // LogMgr.LogError("Wait for connect");
                    return;
                }

                m_connector.WaitTermination();
                m_connector = null;

                if (!m_tcpClient.Connected)
                {
                    //LogMgr.LogError("connect fail");
                    PrepareToConnect();
                    return;
                }
                else
                {
                    //LogMgr.LogError("connect ok");
                    connectOK();
                }
            }
            else
            {
                if (m_isConnectted)
                {
                    bool pollret = m_tcpClient.Client.Poll(1000, SelectMode.SelectRead);
                    bool available = m_tcpClient.Client.Available == 0;
                    bool isConnecting = !((pollret && available) || !m_tcpClient.Connected);
                    if (isConnecting)
                    {
                        checkCounter = 0;
                    }
                    //bool isConnecting = !m_tcpClient.Connected;
                    if (isConnecting == false)
                    {
                        checkCounter++;
                        //增加3次统计
                        if (checkCounter >= 3)
                        {
                            LogMgr.LogError("m_tcpClient.Connected:" + m_tcpClient.Connected + " pollret:" + pollret + " available:" + available);
                            m_isConnectted = false;
                            onError(10051);
                            //在网络状况发生变化的时候结束当前收发线程
                            LogMgr.UnityLog("SetTerminateFlag!");
                            SetTerminateFlag();

                            LogMgr.UnityLog("onClose!");
                            if (onClose != null)
                            {
                                onClose();
                            }
                        }
                    }
                    else if (m_currentTry != null)
                    {
                        //连接上时,当前连接请求已经完成,不再需要
                        m_currentTry = null;

                        StartSendThread();
                        StartReceiveThread();
                    }
                }

                dispatchMsg();
            }
            //processPendingMsg();
        }
        
        public void Disconnect()
        {
            LogMgr.UnityLog("Disconnect SetTerminateFlag");
            Debug.Log("Disconnect SetTerminateFlag");
            SetTerminateFlag();

            onConnect = null;
            onClose = null;
            m_isConnectted = false;
            recvMsgs.Clear();
            sendMsgs.Clear();
            msgTypes.Clear();
            msgHandlers.Clear();
            msgExHandlers.Clear();

            if (m_tcpClient.Connected)
            {
                m_tcpClient.GetStream().Close();
                m_tcpClient.Close();
            }
        }

        public void ForceClose()
        {
            SetTerminateFlag();
            m_isConnectted = false;

            if(m_tcpClient.Connected)
            {
                m_tcpClient.GetStream().Close();
                m_tcpClient.Close();
            }
        }

        public void ForceContinue()
        {

        }

        private void ConnectFailed()
        {
            IPAddress newaddress = IPAddress.Parse(m_serverIP);
            IPEndPoint remote = new IPEndPoint(newaddress, m_serverPort);
            onConnect(false, remote);


            //丢失网络连接同时还有网络任务时，重试连接
            lock (sendMsgs)
            {
                if (sendMsgs.Count != 0)
                {
#if (NETMGR_USE_IP_FOR_CON)
                    _ConnectAsyncWithIp();
#else
                    _ConnectAsyncWithUrl();
#endif
                }
            }
        }
        //add by lzp 14:43 2016/10/11 开始重连检测在这个函数里
        private bool ConnectIfNeeded()
        {
            if (!m_isConnectted && m_reconnect != null && 
                MapMgr.Instance.CurMapType != MapMgr.MapType.Map_Login && MapMgr.Instance.CurMapType != MapMgr.MapType.Map_None)
            {
                LogMgr.DebugLog("ConnectIfNeeded : StartReconnect");
                m_reconnect.StartReconnect();
            }

            if (!IsTcpClientConnected() && (null == m_currentTry))
            {
#if (NETMGR_USE_IP_FOR_CON)
                ConnectAsyncWithIp();
#else
                ConnectAsyncWithUrl();
#endif
                return true;
            }
            return false;
        }

        /// <summary>
        /// 非线程安全，必须主线程调用
        /// </summary>
        public void ConnectAsyncWithUrl()
        {
            if (IsTcpClientConnected())
            {
                Disconnect();
                Start();
            }

            ConnectTry conTry = new ConnectTry();
            IPAddress[] addresses = Dns.GetHostAddresses(m_serverNetUrl);
            foreach (IPAddress add in addresses)
            {
                conTry.remoteHost.Enqueue(add);
            }
            m_currentTry = conTry;

            PrepareToConnect();
        }
        private void _ConnectAsyncWithUrl()
        {
            if (m_currentTry.remoteHost.Count != 0)
            {
                IPAddress add = m_currentTry.remoteHost.Dequeue();
                LogMgr.UnityLog(string.Format("HttpMgr._ConnectAsync: try url: {0}", add.ToString()));
                //m_tcpClient.BeginConnect(add, m_serverPort, new AsyncCallback(CallBackMethod), this);
                syncConnect(add, m_serverPort);
            }
            else //所有尝试均失败
            {
                ConnectFailed();
            }
        }

        /// <summary>
        /// 非线程安全，必须主线程调用
        /// </summary>
        public void ConnectAsyncWithIp()
        {
            if (IsTcpClientConnected())
            {
                Disconnect();
                Start();
            }

            m_currentTry = new ConnectTry();

            LogMgr.UnityLog(string.Format("ConnectAsyncWithIp m_prepareConnect = true ip:{0} port:{1}", m_serverIP, m_serverPort));
            PrepareToConnect();
        }
        private void connectOK()
        {
            //设置属性
            m_tcpClient.NoDelay = true;

            m_tcpClient.ReceiveBufferSize = TcpClientReceiveBufferSize;
            m_tcpClient.ReceiveTimeout = TcpClientReceiveTimeout;

            m_tcpClient.SendBufferSize = TcpClientSendBufferSize;
            m_tcpClient.SendTimeout = TcpClientSendTimeout;

            // 设置已连接
            m_isConnectted = true;
            m_PackHeaderIndex = 1;

            if(onConnect != null)
            {
                IPAddress newaddress = IPAddress.Parse(m_serverIP);
                IPEndPoint remote = new IPEndPoint(newaddress, m_serverPort);
                onConnect(true, remote);
            }

            //add by lzp 20170605 先注释，这功能待确定
            //sendAuthReq();

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_CrossConnect, EventParameter.Get());
        }
        private void syncConnect(IPAddress add, int port)
        {
            try
            {
                // 同步连接
                m_tcpClient.Connect(add, port);
                if (!m_tcpClient.Connected)
                {
                    PrepareToConnect();
                }
                else
                {
                    connectOK();
                }
            }
            catch (SocketException ex)
            {
                LogMgr.UnityWarning("_ConnectAsyncWithIp: exception " + ex.Message);
                onError(ex.ErrorCode);
            }
        }
        string newHostname;
        int newPort;
        AddressFamily newAddressFamily;
        private void syncConnect(string hostname, int port)
        {
            newPort = port;
            GetNetType.GetNetTypeByIOS(hostname, port.ToString(), (newHost, newAdd) =>
            {
                newHostname = newHost;
                newAddressFamily = newAdd;
            });
            while (string.IsNullOrEmpty(newHostname))
            {
            }
            BeginConnet();
        }
        void BeginConnet()
        {
            try
            {
                // 同步连接
                //LogMgr.Log("NewAddress:"+newHostname);
                //LogMgr.Log("NewAddress:"+newPort);
                //LogMgr.Log("NewAddressFamily:"+newAddressFamily.ToString());
                if (m_connector != null)
                {
                    m_connector.Abort();
                    m_connector = null;
                }
                m_tcpClient = new TcpClient(newAddressFamily);

                m_connector = new ConnectThread(m_tcpClient, newHostname, newPort, 5f);
                if (m_connector != null)
                {
                    m_connector.Run();
                }
            }
            catch (SocketException ex)
            {
                //LogMgr.LogError("SocketException:"+ex.Message);
                LogMgr.UnityWarning("_ConnectAsyncWithIp: exception " + ex.Message);
                // 异常的网络事件，发送断线重连事件
                onError(ex.ErrorCode);
            }
        }

        private void _ConnectAsyncWithIp()
        {
            LogMgr.UnityLog(string.Format("_ConnectAsyncWithIp m_currentTry.tryCount:", m_currentTry.tryCount));
            if ((--m_currentTry.tryCount) > 0)
            {
                LogMgr.UnityLog("beginconnect");
                syncConnect(m_serverIP, m_serverPort);
            }
            else //所有尝试均失败
            {
                ConnectFailed();
            }
        }

        public void SetTerminateFlag()
        {
            if (m_sender != null)
                m_sender.SetTerminateFlag();

            if (m_receiver != null)
                m_receiver.SetTerminateFlag();

            WaitTermination();
        }

        public void SetContinueFlag()
        {
            if (m_sender != null)
                m_sender.SetContinueFlag();
            if (m_receiver != null)
                m_receiver.SetContinueFlag();

        }

        public void WaitTermination()
        {
            if (m_sender != null)
                m_sender.WaitTermination();
            if (m_receiver != null)
                m_receiver.WaitTermination();
        }

        protected void StartReceiveThread()
        {
            m_receiver = new ReceiverThread(m_tcpClient.GetStream(), this);
            m_receiver.Run();
        }

        protected void StartSendThread()
        {
            m_sender = new SenderThread(m_tcpClient.GetStream(), sendMsgs);
            m_sender.Run();
        }

        public void OnApplicationQuit()
        {
            Disconnect();
        }

        #region 链接线程

[Hotfix]
        public class ConnectThread : BasicThread
        {
            TcpClient m_tcpClient;
            IPEndPoint m_serverAddress;
            string m_ip;
            int m_port;
            bool m_useIpPortMode = true;
            float m_timeout;
            float m_begintime;

            bool m_connected;
            // 链接异常
            bool m_error;
            string m_error_msg;

            public ConnectThread(TcpClient client, IPEndPoint serverAddress, float timeout)
            {
                // TCPCLIENT 可以用引用，外部有一个变量用于保护该指针
                m_tcpClient = client;
                // 这里不要用引用 否则外部修改不一定能检测到
                m_serverAddress = new IPEndPoint(serverAddress.Address, serverAddress.Port);
                m_useIpPortMode = false;
                m_connected = false;
                m_error = false;
                m_timeout = timeout;
                m_begintime = Time.time;
            }

            public ConnectThread(TcpClient client, string ip, int port, float timeout)
            {
                // TCPCLIENT 可以用引用，外部有一个变量用于保护该指针
                m_tcpClient = client;
                // 这里不要用引用 否则外部修改不一定能检测到
                m_ip = ip;
                m_port = port;
                m_useIpPortMode = true;
                m_connected = false;
                m_error = false;
                m_timeout = timeout;
                m_begintime = Time.time;
            }

            protected override void Main()
            {
                try
                {
                    if (m_useIpPortMode)
                    {
                        m_tcpClient.Connect(m_ip, m_port);
                    }
                    else
                    {
                        m_tcpClient.Connect(m_serverAddress);
                    }
                    // 标记成功，返回调用者
                    m_connected = true;
                    //LogMgr.LogError("Connect successful");
                }
                catch (SocketException ex)
                {
                    //Log.LogError( "OnConnect: " + ex.Message );
                    m_error = true;
                    m_error_msg = ex.Message;
                }
            }

            // 检测是否链接超时
            public bool CheckTimeOut(float time)
            {
                // 如果已经连接上了就不存在超时的问题
                if (m_connected == true)
                {
                    return false;
                }
                if (time > m_begintime + m_timeout)
                {
                    return true;
                }
                return false;
            }

            // 判断是否链接结束
            public bool IsConnected()
            {
                return m_connected;
            }

            // 判断是否存在链接错误
            public bool IsError()
            {
                return m_error;
            }

            public string ErrorMsg()
            {
                return m_error_msg;
            }
        }

        #endregion

        #region 接收线程

[Hotfix]
        class ReceiverThread : NetThread
        {
            const uint MaxPacketSize = 1024 * 512;
            const uint MinPacketSize = 16;
            
            private byte[] m_recBuf;
            private int m_recBufOffset;
            private NetClientThread m_netMgr = null;
            //private List<RecvBuff> framelist = new List<RecvBuff>();                      //合包解出来的消息列表
            
            public ReceiverThread(NetworkStream stream, NetClientThread netMgr)
                : base(stream)
            {
                m_recBuf = new byte[2 * MaxPacketSize];
                m_recBufOffset = 0;
                m_netMgr = netMgr;
            }

            protected override void Main()
            {
                LogMgr.UnityLog("NetMgr.ReceiverThread.Main: Begin");

                while (!IsTerminateFlagSet())
                {
                    try
                    {
                        ReadFromStream();
                        ScanPackets();
                    }
                    catch (Exception e)
                    {
                        LogMgr.LogError("rcv thread error:" + e);
                    }
                }

                LogMgr.UnityLog("NetMgr.ReceiverThread.Main: End");
            }

            protected void ReadFromStream()
            {
                if (NetStream.DataAvailable)
                {
                    int size = NetStream.Read(m_recBuf, m_recBufOffset, m_recBuf.Length - m_recBufOffset);
                    //add by lzp 测试接收数据长度
                    //LogMgr.Log("NetClient Recv: size : " + size.ToString());
                    m_recBufOffset += size;
#if UNITY_EDITOR
                    lock(m_netMgr)
                    {
                        if (m_netMgr.m_recordNet)
                        {
                            m_netMgr.m_byteRecv += size;
                        }
                    }                    
#endif
                }
                else
                {
                    Thread.Sleep(16);
                }
            }

            protected void ScanPackets()
            {
                bool packetFound = false;
                do
                {
                    packetFound = false;

                    //如果大于包头长度
                    if (m_recBufOffset >= MinPacketSize)
                    {
                        PacketHeader header = new PacketHeader();
                        header.HeaderSign = BitConverter.ToInt16(m_recBuf, 0);
                        header.BodySize = BitConverter.ToInt32(m_recBuf, sizeof(Int16));
                        
                        int dwPkgLen = (int)(MinPacketSize + header.BodySize);
                        if (m_recBufOffset >= dwPkgLen)
                        {
                            //接收到了一个完整的数据包
                            packetFound = true;
                            header.RandSeed = (byte)BitConverter.ToChar(m_recBuf, sizeof(Int16) + sizeof(Int32));
                            header.CheckSum = (byte)BitConverter.ToChar(m_recBuf, sizeof(Int16) + sizeof(Int32) + sizeof(byte));
                            header.Compress = BitConverter.ToInt32(m_recBuf, sizeof(Int16) + sizeof(Int32) + sizeof(byte) * 2);
                            header.CMD = BitConverter.ToInt16(m_recBuf, sizeof(Int16) + sizeof(Int32) * 2 + sizeof(byte) * 2);
                            header.Index = BitConverter.ToInt16(m_recBuf, sizeof(Int16) * 2 + sizeof(Int32) * 2 + sizeof(byte) * 2);
     
                            //将数据表扔到接收队列里
                            RecvBuffer msg = null;
                            lock (cacheRecvMsgs)
                            {
                                msg = cacheRecvMsgs.Get();
                            }
                            msg.tick = DateTime.Now.Ticks;
                            msg.cmd = header.CMD;

                            //解包数据流
                            if (header.BodySize != 0)
                            {
                                PacketUtil.Decrypt(m_recBuf, PacketHeader.Length, header.BodySize);
                                if (header.GetCompressType() == (int)CompressType.LZ4)
                                {
                                    int len = header.GetUnCompressLength();
                                    PacketUtil.uncompress_lz4(m_recBuf, PacketHeader.Length, header.BodySize, msg.data, len);
                                    msg.length = len;
                                }
                                else
                                {
                                    msg.length = header.BodySize;
                                    Utils.CopyTo(m_recBuf, PacketHeader.Length, msg.data, 0, msg.length);
                                }
                            }

                            lock (m_netMgr.recvMsgs)
                            {
                                m_netMgr.recvMsgs.Enqueue(msg);
                            }

                            //接收缓冲区数据往回移
                            int offset = dwPkgLen;
                            if (m_recBufOffset > offset)
                            {
                                for (int i = offset, j = 0; i < m_recBufOffset; i++, j++)
                                {
                                    m_recBuf[j] = m_recBuf[i];
                                }
                                m_recBufOffset -= offset;
                            }
                            else
                            {
                                m_recBufOffset = 0;
                            }
                        }
                    }
                }
                while (packetFound && !IsTerminateFlagSet());
            }
        }

        #endregion

        #region 发送线程


[Hotfix]
        class SenderThread : NetThread
        {
            Queue<SendBuffer> m_sendMsgs;
            public SenderThread(NetworkStream stream, Queue<SendBuffer> _sendMsgs) : base(stream)
            {
                m_sendMsgs = _sendMsgs;
            }

            protected override void Main()
            {
                LogMgr.UnityLog("NetMgr.SenderThread.Main: Begin");

                while (!IsTerminateFlagSet())
                {
                    bool sleep = false;
                    SendBuffer buffer = null;

                    lock (m_sendMsgs)
                    {
                        if (m_sendMsgs.Count > 0)
                        {
                            buffer = m_sendMsgs.Dequeue();
                            
                        }
                        else
                        {
                            sleep = true;
                            //LogMgr.UnityError("======sendMsgs.Count ==0 =====");
                        }
                    }
                    if (buffer != null)
                    {
                        try
                        {
                            NetStream.Write(buffer.msg, 0, buffer.Length);
                            NetStream.Flush();
                        }
                        catch (IOException e)
                        {
                            LogMgr.UnityError("SenderThread, Message: " + e.Message);
                            LogMgr.UnityError("SenderThread, StackTrace: " + e.StackTrace);
                            LogMgr.UnityError("SenderThread, InnerException.Message: " + e.InnerException.Message);
                        }

                        lock (cacheSendMsgs)
                        {
                            cacheSendMsgs.Cache(buffer);
                        }
                    }

                    if (sleep)
                        Thread.Sleep(15);
                }

                LogMgr.UnityLog("NetMgr.SenderThread.Main: End");
            }
        }

        #endregion

        public Int16 m_PackHeaderIndex = 1;//包的序列号，从0 ~9999 ，必须循环递增

        public void send(Int16 cmd, NetWriteBuffer buffer)
        {
            send(cmd, buffer.Data, buffer.Length);
        }

        public void send(Int16 cmd, byte[] data, int len)
        {
            ConnectIfNeeded();

            //没有连上逻辑服，丢掉将要发的消息
            if (!m_isConnectted && m_reconnect != null)
            {
                return;
            }
            SendBuffer msg = null;
            lock(cacheSendMsgs)
            {
                msg = cacheSendMsgs.Get();
            }
            
            msg.Init(cmd, m_PackHeaderIndex, data, len);
            m_PackHeaderIndex += 1;
            if (m_PackHeaderIndex > 9999)
            {
                m_PackHeaderIndex = 1;
            }

            lock (sendMsgs)
            {
#if UNITY_EDITOR
                lock (this)
                {
                    if (m_recordNet)
                    {
                        m_byteSent += msg.Length;
                    }
                }
#endif
                sendMsgs.Enqueue(msg);
            }
        }

        public bool bindMsgHandler(Int16 cmd, MsgHandler handler)
        {
            MsgHandler handlerEvent;
            if (msgHandlers.TryGetValue(cmd, out handlerEvent))
            {
#if UNITY_EDITOR
                LogMgr.LogError("网络消息处理重复注册:{0}", cmd);
#endif
                return false;
            }
            else
            {
                msgHandlers.Add(cmd, handler);

                return true;
            }
        }

        public void unbindMsgHandler(Int16 cmd)
        {
            msgHandlers.Remove(cmd);
        }

        public bool bindMsgExHandler(Int16 cmd, MsgExHandler handler)
        {
            MsgExHandler handlerEvent;
            if (msgExHandlers.TryGetValue(cmd, out handlerEvent))
            {
#if UNITY_EDITOR
                LogMgr.LogError("网络扩展消息处理重复注册:{0}", cmd);
#endif
                return false;
            }
            else
            {
                msgExHandlers.Add(cmd, handler);

                return true;
            }
        }

        public void unbindMsgExHandler(Int16 cmd)
        {
            msgExHandlers.Remove(cmd);
        }
        
        public bool registerMsgType(Int16 cmd, Type tp)
        {
            Type t;
            if (msgTypes.TryGetValue(cmd, out t))
            {
                return false;
            }

            if (!tp.IsSubclassOf(typeof(MsgData)))
            {
                return false;
            }

            msgTypes.Add(cmd, tp);
            return true;
        }
                 
        private RecvBuffer tempBuff = new RecvBuffer();                         //解开合并包时复用

        /// <summary>
        /// 二次解析数据表。
        /// </summary>
        /// <param name="msg">被合并的数据表。</param>
        /// <param name="sendfun">发送消息的回调。</param>
        protected void UnpackFrameMessages(RecvBuffer msg, Action<RecvBuffer> sendfun)
        {
            //int len = BitConverter.ToInt16(msg.data, 0);                //数据长度（没用到）
            int n = BitConverter.ToInt16(msg.data, sizeof(short));      //被合并的包数
            int index = sizeof(short) * 2;                              //开始解包的索引
            for (int i = 0; i < n; ++i)
            {
                tempBuff.OnUse();
                tempBuff.cmd = BitConverter.ToInt16(msg.data, index);
                tempBuff.length = BitConverter.ToUInt16(msg.data, index + 2);
                tempBuff.tick = DateTime.Now.Ticks;
                Utils.CopyTo(msg.data, index + 4, tempBuff.data, 0, tempBuff.length);
                index += 4 + tempBuff.length;
                sendfun(tempBuff);
            }
        }

        /// <summary>
        /// 分发收到的网络消息（主线程中调用）。
        /// </summary>
        protected void dispatchMsg()
        {
            //先将消息缓冲到临时列表，然后清空临界区列表，再通过临时列表派发，防止消息在派发时报错跳出，导致无法从列表里移除一直报错     
            int n = 2;     //每帧最多处理2个消息
            //Debug.Log("recvMsgs.Count " + recvMsgs.Count);
            for (int i=0; i<n; ++i)
            {
                RecvBuffer m = null;
                lock (recvMsgs)
                {
                    if (recvMsgs.Count > 0)
                    {
                        m = recvMsgs.Dequeue();
                    }
                }
                if (m == null)
                {
                    return;
                }

                //消息派发
                if (m.cmd == NetMsgDef.S_HEART_BEAT)
                {
                    //心跳
                    NetReadBuffer buf = NetReadBuffer.CacheBuff;
                    buf.Init(m.data, 0, m.length);
                    NetMgr.CacheRecvHeartBeat.unpack(buf);
                    CoreEntry.netMgr.OnReceiveHearBeat(m.tick, NetMgr.CacheRecvHeartBeat);
                }
                else if (m.cmd == NetMsgDef.S_FRAME_MSG)
                {
                    //合并包
                    UnpackFrameMessages(m, SendMessage);
                }
                else
                {
                    SendMessage(m);
                }
                cacheRecvMsgs.Cache(m);
            }
        }

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="msg"></param>
        private void SendMessage(RecvBuffer msg)
        {
            //try
            //{
                NetReadBuffer buffer = NetReadBuffer.CacheBuff;
                buffer.Init(msg.data, 0, msg.length);
                //LogMgr.Log("msg.cmd " + msg.cmd);
                MsgExHandler exhandler;
                if (msgExHandlers.TryGetValue(msg.cmd, out exhandler))
                {
                    exhandler(buffer);
                    return;
                }

                Type tp;
                if (msgTypes.TryGetValue(msg.cmd, out tp))
                {
                    //走C#层 解包消息
                    MsgData msgdata = Activator.CreateInstance(tp) as MsgData;
                    msgdata.unpack(buffer);

                    //接收调用
                    MsgHandler handlerEvent;
                    if (msgHandlers.TryGetValue(msg.cmd, out handlerEvent))
                    {
                        handlerEvent(msgdata);
                    }
                    else
                    {
#if UNITY_EDITOR
                        LogMgr.Log("NetClient.dispatchMsg Failed ,Unknow NetMessage Recevied cmd:{0}", msg.cmd);
#endif
                    }
                }
                else
                {
                    //走Lua层
                    OnLuaDispatchMsg(msg.cmd, buffer);
                }
            //}
            //catch (Exception e)
            //{
            //    LogMgr.LogError(e.Message);
            //}
        }

        /// <summary>
        /// 清理Lua相关。
        /// </summary>
        public static void ClearLua()
        {
            m_LuaNet = null;
            m_LuaDispatchCall = null;
        }

        private static DispatchMsgCall m_LuaDispatchCall;
        private static LuaTable m_LuaNet;

        public static LuaTable OnLuaDispatchMsg(short cmd, NetReadBuffer buffer)
        {
            if (m_LuaDispatchCall == null)
            {
                LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
                m_LuaNet = G.Get<LuaTable>("Net");
                m_LuaDispatchCall = m_LuaNet.Get<DispatchMsgCall>("OnDispatchMsg");
            }
            return m_LuaDispatchCall(m_LuaNet, cmd, buffer);
        }
    }

[Hotfix]
    public abstract class BasicThread
    {
        public BasicThread()
        {
            m_thread = new Thread(ThreadProc);
            m_terminateFlag = false;
            m_terminateFlagMutex = new System.Object();
        }

        public void Run()
        {
            m_thread.Start(this);
        }

        protected static void ThreadProc(object obj)
        {
            BasicThread me = (BasicThread)obj;
            me.Main();
        }

        protected abstract void Main();

        public void WaitTermination()
        {
            m_thread.Join();
        }

        public void SetTerminateFlag()
        {
            lock (m_terminateFlagMutex)
            {
                m_terminateFlag = true;
            }
        }

        public void SetContinueFlag()
        {
            lock (m_terminateFlagMutex)
            {
                m_terminateFlag = false;
            }
        }

        protected bool IsTerminateFlagSet()
        {
            lock (m_terminateFlagMutex)
            {
                return m_terminateFlag;
            }
        }

        public void Abort()
        {
            m_thread.Abort();
        }

        private Thread m_thread;
        private bool m_terminateFlag;
        private System.Object m_terminateFlagMutex;
    }

[Hotfix]
    abstract class NetThread : BasicThread
    {
        public NetThread(NetworkStream stream)
            : base()
        {
            m_netStream = stream;
        }

        protected NetworkStream NetStream
        {
            get
            {
                return m_netStream;
            }
        }

        private NetworkStream m_netStream;
    }
}

