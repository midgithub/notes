/**
* @file     : ChatData.cs
* @brief    : 聊天数据。
* @details  : 
* @author   : XuXiang
* @date     : 2017-07-21 11:18
*/

using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace SG
{
    /// <summary>
    /// 一条聊天消息。
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public class ChatMessage
    {
        /// <summary>
        /// 消息内容。
        /// </summary>
        private MsgData_sChat mContent;

        /// <summary>
        /// 获取消息内容。
        /// </summary>
        public MsgData_sChat Content
        {
            get { return mContent; }
        }

        /// <summary>
        /// 显示高度。
        /// </summary>
        private float mShowHeight;

        /// <summary>
        /// 获取或设置显示高度。
        /// </summary>
        public float ShowHeight
        {
            get { return mShowHeight; }
            set { mShowHeight = value; }
        }

        /// <summary>
        /// 显示类型。
        /// </summary>
        private int mShowType;

        /// <summary>
        /// 显示类型。
        /// </summary>
        public int ShowType
        {
            get { return mShowType; }
        }

        /// <summary>
        /// 发送者编号。
        /// </summary>
        private long mSenderID = 0;
        
        /// <summary>
        /// 获取发送者编号。
        /// </summary>
        public long SenderID
        {
            get { return mSenderID; }
        }

        /// <summary>
        /// 接收者编号。
        /// </summary>
        private long mToID = 0;

        /// <summary>
        /// 获取接收者编号。
        /// </summary>
        public long ToID
        {
            get { return mToID; }
        }

        /// <summary>
        /// 发送者名称。
        /// </summary>
        private string mSenderName = string.Empty;

        /// <summary>
        /// 获取发送者名称。
        /// </summary>
        public string SenderName
        {
            get { return mSenderName; }
        }

        /// <summary>
        /// 消息内容。
        /// </summary>
        private string mText;

        /// <summary>
        /// 获取消息内容。
        /// </summary>
        public string Text
        {
            get { return mText; }
        }

        /// <summary>
        /// 频道号。
        /// </summary>
        public int mChannel;

        /// <summary>
        /// 获取频道号。
        /// </summary>
        public int Channel
        {
            get { return mChannel; }
        }

        /// <summary>
        /// 获取发送者VIP。
        /// </summary>
        public int SenderVIP
        {
            get { return mContent != null ? mContent.SenderVIP : 0; }
        }
        
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="chat">聊天网络消息。</param>
        public ChatMessage(MsgData_sChat chat)
        {
            int server;
            mContent = chat;

            mShowType = mContent.SenderID == PlayerData.Instance.RoleID ? 3 : 2;
            //byte[] bytes = mContent.Text.ToArray();
            mText = UiUtil.GetNetString(mContent.Text);
            mSenderID = mContent.SenderID;
            mSenderName = PlayerData.GetPlayerName(UiUtil.GetNetString(mContent.SenderName), out server);
            mChannel = mContent.Channel;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="showtype">显示类型。</param>
        /// <param name="channel">频道号。</param>
        /// <param name="text">消息内容。</param>
        public ChatMessage(int showtype, int channel, string text)
        {
            mShowType = showtype;
            mChannel = channel;
            mText = text;

            if (mChannel == ChatChannel.CHAT_CHANNEL_WHISPER)
            {
                mToID = ChatData.LastPrivateChatID;
            }
        }
    }
    
    /// <summary>
    /// 最近私聊信息
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public class RecentPrivateChatInfo
    {
        /// <summary>
        /// 角色编号。
        /// </summary>
        public long RoleID { get; set; }

        /// <summary>
        /// 职业。
        /// </summary>
        public int Prof { get; set; }

        /// <summary>
        /// 角色名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 添加进列表的时间。
        /// </summary>
        public float AddTime { get; set; }

        /// <summary>
        /// 等级。
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 上次登录时间。(0表示在线)
        /// </summary>
        public long LastLoginTime { get; set; }
    }

    /// <summary>
    /// 聊天信息。
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public class ChatData
    {
        /// <summary>
        /// 消息缓存数量。
        /// </summary>
        public static int MessageCacheSize = 100;

        /// <summary>
        /// 各个频道消息列表，私聊不在此存放。
        /// </summary>
        private Dictionary<int, List<ChatMessage>> mChannelMessage = new Dictionary<int, List<ChatMessage>>();

        /// <summary>
        /// 所有消息。
        /// </summary>
        private List<ChatMessage> mAllMessage = new List<ChatMessage>();

        /// <summary>
        /// 私聊消息。
        /// </summary>
        private Dictionary<long, List<ChatMessage>> mPrivateMessage = new Dictionary<long, List<ChatMessage>>();

        /// <summary>
        /// 最近私聊列表。
        /// </summary>
        private List<RecentPrivateChatInfo> mRecentPrivateChatList = new List<RecentPrivateChatInfo>();

        /// <summary>
        /// 获取最近私聊列表。
        /// </summary>
        public List<RecentPrivateChatInfo> RecentPrivateChatList
        {
            get { return mRecentPrivateChatList; }
        }

        /// <summary>
        /// 最后一次发送私聊编号。
        /// </summary>
        public static long LastPrivateChatID = 0;

        /// <summary>
        /// 快捷语言。
        /// </summary>
        private Dictionary<int, string> m_FastTalk = new Dictionary<int, string>();

        /// <summary>
        /// 特殊公告ID。
        /// </summary>
        private static int SpecialNoticeID = 10000;

        /// <summary>
        /// 获取快捷语言。
        /// </summary>
        /// <param name="index">语言索引。(从1开始)</param>
        /// <returns>语言内容。</returns>
        public string GetFastTalk(int index)
        {
            string ret;
            if (m_FastTalk.TryGetValue(index, out ret))
            {
                return ret;
            }

            string key = string.Format("CHAT_FAST_TALK_{0}", index);
            ret = RoleUtility.GetString(key);
            m_FastTalk.Add(index, ret);
            return ret;
        }

        /// <summary>
        /// 设置快捷语言。
        /// </summary>
        /// <param name="index">语音索引。(从1开始)</param>
        /// <param name="msg">语言内容。(从1开始)</param>
        public void SetFastTalk(int index, string msg)
        {
            string key = string.Format("CHAT_FAST_TALK_{0}", index);
            m_FastTalk[index] = msg;
            RoleUtility.SetString(key, msg);
        }

        /// <summary>
        /// 渠道接收信息开关。
        /// </summary>
        private Dictionary<int, bool> m_ChannelSwitch = new Dictionary<int, bool>();

        /// <summary>
        /// 获取渠道开关。
        /// </summary>
        /// <param name="id">渠道编号。</param>
        /// <returns>是否接收渠道消息。</returns>
        public bool GetChannelSwitch(int id)
        {
            bool ret;
            if (m_ChannelSwitch.TryGetValue(id, out ret))
            {
                return ret;
            }

            string key = string.Format("CHAT_CHANNEL_SWITCH_{0}", id);
            ret = RoleUtility.GetBool(key, true);
            m_ChannelSwitch.Add(id, ret);
            return ret;
        }

        /// <summary>
        /// 设置渠道开关。
        /// </summary>
        /// <param name="id">渠道编号。</param>
        /// <param name="on">是否接收渠道消息。</param>
        public void SetChannelSwitch(int id, bool on)
        {
            string key = string.Format("CHAT_CHANNEL_SWITCH_{0}", id);
            m_ChannelSwitch[id] = on;
            RoleUtility.SetBool(key, on);
        }

        /// <summary>
        /// WIFI下自动播放语音开关。
        /// </summary>
        private Dictionary<int, bool> m_AutoSoundWifi = new Dictionary<int, bool>();

        /// <summary>
        /// 获取WIFI下自动播放语音开关。
        /// </summary>
        /// <param name="id">渠道编号。</param>
        /// <returns>是否WIFI下自动播放语音。</returns>
        public bool GetAutoSoundWifi(int id)
        {
            bool ret;
            if (m_AutoSoundWifi.TryGetValue(id, out ret))
            {
                return ret;
            }

            string key = string.Format("CHAT_AUTO_SOUND_WIFI_{0}", id);
            ret = RoleUtility.GetBool(key, true);
            m_AutoSoundWifi.Add(id, ret);
            return ret;
        }

        /// <summary>
        /// 设置WIFI下自动播放语音开关。
        /// </summary>
        /// <param name="id">渠道编号。</param>
        /// <param name="on">是否WIFI下自动播放语音。</param>
        public void SetAutoSoundWifi(int id, bool on)
        {
            string key = string.Format("CHAT_AUTO_SOUND_WIFI_{0}", id);
            m_AutoSoundWifi[id] = on;
            RoleUtility.SetBool(key, on);
        }

        /// <summary>
        /// 流量下自动播放语音开关。
        /// </summary>
        private Dictionary<int, bool> m_AutoSound4G = new Dictionary<int, bool>();

        /// <summary>
        /// 获取流量下自动播放语音开关。
        /// </summary>
        /// <param name="id">渠道编号。</param>
        /// <returns>是否流量下自动播放语音。</returns>
        public bool GetAutoSound4G(int id)
        {
            bool ret;
            if (m_AutoSound4G.TryGetValue(id, out ret))
            {
                return ret;
            }

            string key = string.Format("CHAT_AUTO_SOUND_4G_{0}", id);
            ret = RoleUtility.GetBool(key, false);
            m_AutoSound4G.Add(id, ret);
            return ret;
        }

        /// <summary>
        /// 设置流量下自动播放语音开关。
        /// </summary>
        /// <param name="id">渠道编号。</param>
        /// <param name="on">是否流量下自动播放语音。</param>
        public void SetAutoSound4G(int id, bool on)
        {
            string key = string.Format("CHAT_AUTO_SOUND_4G_{0}", id);
            m_AutoSound4G[id] = on;
            RoleUtility.SetBool(key, on);
        }
        
        /// <summary>
        /// 添加消息。
        /// </summary>
        /// <param name="chat">消息内容。</param>
        private void AddMessage(ChatMessage chat)
        {
            //插入到频道消息集合
            List<ChatMessage> msgs = null;
            if (chat.Channel == ChatChannel.CHAT_CHANNEL_WHISPER)
            {
                if (chat.Content != null)
                {
                    if (!mPrivateMessage.TryGetValue(chat.SenderID, out msgs))
                    {
                        msgs = new List<ChatMessage>();
                        mPrivateMessage.Add(chat.SenderID, msgs);
                    }

                    MsgData_sChat cdata = chat.Content;
                    RecentPrivateChatInfo info = GetRecentPrivateChat(chat.SenderID);
                    if (info == null)
                    {
                        AddRecentPrivateChat(chat.SenderID, cdata.SenderIcon, chat.SenderName, cdata.SenderLevel);
                    }
                    else
                    {
                        info.Name = chat.SenderName;
                        info.Level = cdata.SenderLevel;
                    }
                }
                else if (chat.ShowType == 1)
                {
                    //MsgData_sChatSysNotice notice = chat.Notice;
                    if (!mPrivateMessage.TryGetValue(LastPrivateChatID, out msgs))
                    {
                        msgs = new List<ChatMessage>();
                        mPrivateMessage.Add(LastPrivateChatID, msgs);
                    }
                }
                
            }
            else
            {
                if (!mChannelMessage.TryGetValue(chat.Channel, out msgs))
                {
                    msgs = new List<ChatMessage>();
                    mChannelMessage.Add(chat.Channel, msgs);
                }
            }
            if (msgs != null)
            {
                msgs.Insert(0, chat);
                if (msgs.Count > MessageCacheSize)
                {
                    msgs.RemoveRange(MessageCacheSize, msgs.Count - MessageCacheSize);
                }
            }

            //所有消息列表
            mAllMessage.Insert(0, chat);
            if (mAllMessage.Count > MessageCacheSize)
            {
                mAllMessage.RemoveRange(MessageCacheSize, mAllMessage.Count - MessageCacheSize);
            }
        }

        /// <summary>
        /// 获取显示高度。
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public float GetShowHeight(int channel)
        {
            float h = 0;
            List<ChatMessage> list = GetMessages(channel);
            if (list != null)
            {
                for (int i=0;i< list.Count; ++i)
                {
                    h += list[i].ShowHeight;
                }
            }
            return h;
        }

        /// <summary>
        /// 获取聊天信息列表。
        /// </summary>
        /// <param name="channel">频道号。</param>
        /// <returns>消息列表。</returns>
        public List<ChatMessage> GetMessages(int channel)
        {
            List<ChatMessage> list = null;
            if (channel == ChatChannel.CHAT_CHANNEL_ALL)
            {
                list = mAllMessage;
            }
            else
            {
                mChannelMessage.TryGetValue(channel, out list);
            }
            return list;
        }

        /// <summary>
        /// 获取所有消息。
        /// </summary>
        /// <param name="num">获取的消息数量，0表示所有。</param>
        /// <returns>被屏蔽列表过滤的消息。</returns>
        public List<ChatMessage> GetAllMessages(int num = 0)
        {
            List<ChatMessage> list = new List<ChatMessage>();
            int n = num == 0 ? mAllMessage.Count : Mathf.Min(mAllMessage.Count, num);
            for (int i=0; i<n; ++i)
            {
                ChatMessage cm = mAllMessage[i];
                if (GetChannelSwitch(cm.Channel))
                {
                    list.Add(cm);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取私聊消息。
        /// </summary>
        /// <param name="id">私聊对象ID。</param>
        /// <returns>消息列表。</returns>
        public List<ChatMessage> GetPrivateMessage(long id)
        {
            List<ChatMessage> msgs;
            mPrivateMessage.TryGetValue(id, out msgs);
            return msgs;
        }

        /// <summary>
        /// 获取最近聊天对象。
        /// </summary>
        /// <param name="id">角色编号。</param>
        /// <returns>最近聊天对象。</returns>
        public RecentPrivateChatInfo GetRecentPrivateChat(long id)
        {
            for (int i=0; i<mRecentPrivateChatList.Count; ++i)
            {
                if (mRecentPrivateChatList[i].RoleID == id)
                {
                    return mRecentPrivateChatList[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 添加最近私聊对象。
        /// </summary>
        /// <param name="id">角色ID。</param>
        /// <param name="name">角色名称。</param>
        /// <param name="level">角色等级。
        /// </param>
        public void AddRecentPrivateChat(long id, int prof, string name, int level)
        {
            RecentPrivateChatInfo info = GetRecentPrivateChat(id);
            bool add = info == null;
            if (add)
            {
                info = new RecentPrivateChatInfo();
                info.RoleID = id;
                info.Prof = prof;
                mRecentPrivateChatList.Add(info);
            }            
            info.Name = name;
            info.Level = level;
            info.AddTime = Time.realtimeSinceStartup;
            mRecentPrivateChatList.Sort(CompareRecentPrivateChat);
            if (mRecentPrivateChatList.Count > 10)
            {
                mRecentPrivateChatList.RemoveRange(10, mRecentPrivateChatList.Count - 10);
            }
            if (add)
            {
                TriggerEventAddRecent();
            }
        }

        /// <summary>
        /// 置顶最近私聊对象。
        /// </summary>
        /// <param name="id">角色ID。</param>
        /// </param>
        public void TopRecentPrivateChat(long id)
        {
            RecentPrivateChatInfo info = GetRecentPrivateChat(id);
            if (info == null)
            {
                return;
            }
            info.AddTime = Time.realtimeSinceStartup;
            mRecentPrivateChatList.Sort(CompareRecentPrivateChat);
        }

        /// <summary>
        /// 比较最近聊天列表(排序用)。
        /// </summary>
        public static int CompareRecentPrivateChat(RecentPrivateChatInfo a, RecentPrivateChatInfo b)
        {
            //在线的排前面
            if ((a.LastLoginTime == 0 && b.LastLoginTime !=0) || (a.LastLoginTime != 0 && b.LastLoginTime == 0))
            {
                return a.LastLoginTime == 0 ? -1 : 1;
            }

            return (int)(b.AddTime - a.AddTime);
        }

        /// <summary>
        /// 注册网络消息。
        /// </summary>
        public void RegisterNetMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, OnCleanupUserData);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CHAT_MESSAGE, OnChatMessage);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CHAT_PRIVATE_MESSAGE, OnPrivateChatMessage);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CHAT_SYSTEM_NOTICE, OnSystemNotice);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_NOTICE, OnNotice);
        }

        /// <summary>
        /// 触发收到聊天消息事件。
        /// </summary>
        /// <param name="msg">消息对象。</param>
        public static void TriggerEventMessage(ChatMessage msg)
        {
            EventParameter ep = EventParameter.Get();
            ep.intParameter = msg.Content.Channel;
            ep.objParameter = msg;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CHAT_MESSAGE, ep);
        }

        /// <summary>
        /// 触发收到私聊消息事件。
        /// </summary>
        /// <param name="sender">发送者编号。</param>
        public static void TriggerEventPrivateMessage(long sender)
        {
            EventParameter ep = EventParameter.Get();
            ep.longParameter = sender;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CHAT_PRIVATE_MESSAGE, ep);
        }

        /// <summary>
        /// 触发添加最近列表事件。
        /// </summary>
        public static void TriggerEventAddRecent()
        {
            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CHAT_ADD_RECENT, ep);
        }

        /// <summary>
        /// 触发系统通知消息事件。
        /// </summary>
        /// <param name="msg">消息对象。</param>
        public static void TriggerEventSystemNotice(ChatMessage msg)
        {
            EventParameter ep = EventParameter.Get();
            ep.intParameter = msg.Channel;
            ep.objParameter = msg;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CHAT_SYSTEM_NOTICE, ep);
        }

        /// <summary>
        /// 触发公告事件。
        /// </summary>
        /// <param name="msg">消息对象。</param>
        public static void TriggerEventNotice(ChatMessage msg)
        {
            EventParameter ep = EventParameter.Get();
            ep.intParameter = msg.Channel;
            ep.objParameter = msg;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_NOTICE, ep);
        }

        /// <summary>
        ///  切换账号清除数据
        /// </summary>
        private void OnCleanupUserData(GameEvent ge, EventParameter parameter)
        {
            mChannelMessage.Clear();
            mAllMessage.Clear();
            mPrivateMessage.Clear();
            mRecentPrivateChatList.Clear();
            LastPrivateChatID = 0;
            m_FastTalk.Clear();
        }

        /// <summary>
        /// 收到聊天消息。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        public void OnChatMessage(MsgData data)
        {
            MsgData_sChat info = data as MsgData_sChat;
            ChatMessage chat = new ChatMessage(info);
            //Debug.LogError("===收到聊天消息==="+ chat.SenderID);
            if (PlayerData.Instance.FriendData.IsInBlackList(chat.SenderID)) return;
            AddMessage(chat);
            TriggerEventMessage(chat);         
        }

        /// <summary>
        /// 收到私聊消息。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        public void OnPrivateChatMessage(MsgData data)
        {
            MsgData_sPrivateChatNotice info = data as MsgData_sPrivateChatNotice;
            SendSetPrivateChatStateRequest(info.SenderID, 1);
            TriggerEventPrivateMessage(info.SenderID);
        }

        //[CSharpCallLua]
        //public delegate string GetLevelNameCall(int id);

        [CSharpCallLua]
        public delegate string GetParamValueCall(string msg);

        [CSharpCallLua]
        public delegate string GetQualityColorCall(string msg, int qua);

        /// <summary>
        /// 获取参数值。
        /// </summary>
        /// <param name="format"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetParamValue(string param)
        {
            //判断参数是否可解析
            string[] paramstr = param.Split(',');
            if (paramstr.Length < 2)
            {
                return param;
            }
            int id;
            if (!int.TryParse(paramstr[0], out id))
            {
                return param;
            }

            //根据类型解析
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            if (id == ChatParamType.CHAT_PARAM_SENDER_NAME || id == ChatParamType.CHAT_PARAM_ROLE_NAME)
            {
                int server;
                return PlayerData.GetPlayerName(paramstr[2], out server);
            }
            else if (id == ChatParamType.CHAT_PARAM_ITEM || id == ChatParamType.CHAT_PARAM_EQUIP)
            {
                int itemid = int.Parse(paramstr[1]);
                LuaTable table = ConfigManager.Instance.BagItem.GetItemConfig(itemid);
                if (table == null)
                {
                    return itemid.ToString();
                }

                GetQualityColorCall gqcfun = G.GetInPath<GetQualityColorCall>("Common.GetQualityColor");
                string itemname = table.Get<string>("name");
                int qua = table.Get<int>("quality");
                return gqcfun != null ? gqcfun(itemname, qua) : itemname;
            }
            else if (id == ChatParamType.CHAT_PARAM_MAP_POS)
            {
                int mapid = int.Parse(paramstr[2]);
                string mapname = ConfigManager.Instance.Map.GetMapName(mapid);
                return string.Format("{0}({1},{2})", mapname, paramstr[3], paramstr[4]);
            }
            else if (id == ChatParamType.CHAT_PARAM_VALUE)
            {
                return paramstr[1];
            }
            else if (id == ChatParamType.CHAT_PARAM_MONSTER)
            {
                int mid = int.Parse(paramstr[1]);
                LuaTable cfg = ConfigManager.Instance.Actor.GetMonsterConfig(mid);
                return cfg.Get<string>("name");
            }
            else if (id == ChatParamType.CHAT_PARAM_WORLD_BOSS)
            {
                int bossid = int.Parse(paramstr[1]);
                return ConfigManager.Instance.Actor.GetWorldBossName(bossid);
            }
            else if (id == ChatParamType.CHAT_PARAM_MAP_ID)
            {
                int mapid = int.Parse(paramstr[1]);
                return ConfigManager.Instance.Map.GetMapName(mapid);
            }
            else if (id == ChatParamType.CHAT_PARAM_POS)
            {
                //int mapid = int.Parse(paramstr[1]);
                //string mapname = ConfigManager.Instance.Map.GetMapName(mapid);
                //return string.Format("{0}({1},{2})", mapname, paramstr[2], paramstr[3]);

                return "";
                //return string.Format("地图坐标({0},{1})", paramstr[1], paramstr[2]);
            }
            else if (id == ChatParamType.CHAT_PARAM_GUILD)
            {
                int server;
                return PlayerData.GetPlayerName(paramstr[2], out server);
            }
            else if (id == ChatParamType.CHAT_PARAM_SHENBING)
            {
                int sbid = int.Parse(paramstr[1]);
                LuaTable table = ConfigManager.Instance.BagItem.GetShenBingConfig(sbid);
                return table.Get<string>("name");
            }
            else if (id == ChatParamType.CHAT_PARAM_HORSE)
            {
                int rideid = int.Parse(paramstr[1]);
                return ConfigManager.Instance.Ride.GetRideName(rideid);
            }
            else if (id == ChatParamType.CHAT_PARAM_GUILD_POS)
            {
                int pos = int.Parse(paramstr[1]);
                return ConfigManager.Instance.Guild.GetTitleName(pos);
            }
            else if (id == ChatParamType.CHAT_PARAM_ACTIVITY)
            {
                int actid = int.Parse(paramstr[1]);
                return ConfigManager.Instance.Common.GetActivityName(actid);
            }
            else if (id == ChatParamType.CHAT_PARAM_NEW_GROUP)
            {
                int gid = int.Parse(paramstr[1]);
                LuaTable table = ConfigManager.Instance.BagItem.GetEquipGroupConfig(gid);
                if(table!=null)
                return table.Get<string>("name");
            }
            else if (id == ChatParamType.CHAT_PARAM_NEIGON)
            {
                int ngid = int.Parse(paramstr[1]);
                LuaTable table = ConfigManager.Instance.Skill.GetNeigongConfig(ngid);
                return table.Get<string>("Name");
            }
            else if (id == ChatParamType.CHAT_PARAM_EQUIP_POS)
            {
                int pos = int.Parse(paramstr[1]);
                return PlayerData.Instance.BagData.GetEquipName(pos);
            }
            else if (id == ChatParamType.CHAT_PARAM_PIFENG)
            {
                int shieldid = int.Parse(paramstr[1]);
                LuaTable table = ConfigManager.Instance.BagItem.GetShieldConfig(shieldid);
                return table.Get<string>("name");
            }

            //剩下的由Lua解析            
            GetParamValueCall fun = G.GetInPath<GetParamValueCall>("Common.GetChatParamValue");
            return fun != null ? fun(param) : param;
        }

        /// <summary>
        /// 解析系统提示。
        /// </summary>
        /// <param name="cfg">公告配置。</param>
        /// <param name="param">被替换参数。</param>
        /// <returns>解析后的提示内容。</returns>
        public static string ParseNotice(LuaTable cfg, string param)
        {
            string text = cfg.Get<string>("text");
            if (!string.IsNullOrEmpty(param))
            {
                //List<string> format = cfg.Get<List<string>>("_param");
                string[] paramstr = param.Split('#');
                for (int i=0; i<paramstr.Length; ++i)
                {
                    string value = GetParamValue(paramstr[i]);
                    text = text.Replace("{" + (i+1) + "}", value);
                }
            }

            //暂时不再后面附带链接
            //string link = cfg.Get<string>("link");
            //string linkstr = string.Empty;
            //if (!string.IsNullOrEmpty(link))
            //{
            //    string clickparam = string.Format("{0},{1},{2}", ChatParamType.CHAT_PARAM_NOTICE, cfg.Get<string>("linkScript"), cfg.Get<string>("linkParam"));
            //    linkstr = string.Format("<href param={1}>{0}</href>", link, clickparam);
            //}

            //前缀后缀拼接
            StringBuilder sb = new StringBuilder();
            sb.Append(cfg.Get<string>("prefix"));
            sb.Append(text);
            //sb.Append(linkstr);

            return sb.ToString();
        }

        /// <summary>
        /// 解析特殊公告。
        /// </summary>
        /// <param name="text">公告参数与内容混合。</param>
        /// <returns>公告参数。</returns>
        public static string ParseSpecialNotice(string text)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            while (index < text.Length)
            {
                //查找{}包裹起来的参数
                int left = text.IndexOf('{', index);
                if (left == -1)
                {
                    break;
                }
                int right = text.IndexOf('}', left + 1);
                if (right == -1)
                {
                    break;
                }

                int leftlen = left - index;
                if (leftlen > 0)
                {
                    sb.Append(text.Substring(index, leftlen));
                }
                int paramlen = right - left - 1;
                if (paramlen > 0)
                {
                    string param = text.Substring(left + 1, paramlen);
                    sb.Append(GetParamValue(param));
                }
                index = right + 1;
            }
            sb.Append(text.Substring(index));
            return sb.ToString();
        }

        /// <summary>
        /// 收到系统公告。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        public void OnSystemNotice(MsgData data)
        {
            MsgData_sChatSysNotice info = data as MsgData_sChatSysNotice;
            string param = UiUtil.GetNetString(info.Param).Trim();
            ShowSystemNotice(info.ID, param);
        }

        /// <summary>
        /// 显示系统公告。
        /// </summary>
        /// <param name="id">公告编号。</param>
        /// <param name="param">公告参数。</param>
        public void ShowSystemNotice(int id, string param)
        {
            LuaTable cfg = ConfigManager.Instance.Common.GetSystemNoticeConfig(id);
            if (cfg == null)
            {
                LogMgr.LogWarning("Unknow system notice id:{0}", id);
                return;
            }

            //解析消息内容            
            string text = ParseNotice(cfg, param);
            int channel = cfg.Get<int>("channel");

            //是否信息漂浮额外提示
            if (cfg.Get<int>("type") == 4)
            {
                UITips.ShowTips(text);
            }

            //系统提示信息0号频道不在聊天信息
            if (channel != 0)
            {
                ChatMessage chat = new ChatMessage(1, channel, text);
                AddMessage(chat);
                TriggerEventSystemNotice(chat);
            }
        }

        /// <summary>
        /// 收到公告。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        public void OnNotice(MsgData data)
        {
            MsgData_sNotice info = data as MsgData_sNotice;
            string param = UiUtil.GetNetString(info.Param).Trim();
            ShowNotice(info.ID, param);
        }

        /// <summary>
        /// 显示公告。
        /// </summary>
        /// <param name="id">公告编号。</param>
        /// <param name="param">公告参数。</param>
        public void ShowNotice(int id, string param)
        {
            LuaTable cfg = ConfigManager.Instance.Common.GetNoticeConfig(id);
            if (cfg == null)
            {
                LogMgr.LogWarning("Unknow notice id:{0}", id);
                return;
            }

            //解析消息内容
            string text = id == SpecialNoticeID ? ParseSpecialNotice(param) : ParseNotice(cfg, param);
            int channel = cfg.Get<int>("channel");

            //是否走马灯显示
            string pos = cfg.Get<string>("pos");
            if (pos.IndexOf("1") != -1 || pos.IndexOf("2") != -1)
            {
                //登录和选角不弹走马灯
                if (MapMgr.Instance.CurMapType != MapMgr.MapType.Map_Login && MapMgr.Instance.CurMapType != MapMgr.MapType.Map_SelectRole)
                {
                    UITips.ShowNotice(text);
                }                
            }

            if (pos.IndexOf("4") != -1)
            {
                ChatMessage chat = new ChatMessage(1, channel, text);
                AddMessage(chat);
                TriggerEventNotice(chat);
            }
            if(pos.IndexOf("5") != -1)
            {
                UITips.ShowTips(text);
            }
        }

        /// <summary>
        /// 物品发送匹配。
        /// </summary>
        private static readonly Regex ItemRegex = new Regex(@"<([0-9]+)-([0-9]+)>", RegexOptions.Singleline);

        /// <summary>
        /// 表情发送匹配。
        /// </summary>
        private static readonly Regex EmojiRegex = new Regex(@"{([0-9]+)}", RegexOptions.Singleline);

        /// <summary>
        /// 位置发送匹配。
        /// </summary>
        private static readonly Regex PositionRegex = new Regex(@"\((.*?),([0-9]+),([0-9]+)\)", RegexOptions.Singleline);

        /// <summary>
        /// 获取点击参数。
        /// </summary>
        /// <param name="info">背包格子信息。</param>
        /// <returns>参数。</returns>
        public static string GetClickParam(ItemInfo info)
        {
            LuaTable cfg = ConfigManager.Instance.BagItem.GetItemConfig(info.ID);
            if (cfg == null) return "";
            int type = (info.Bag == BagType.ITEM_BAG_TYPE_EQUIP || cfg.Get<int>("main") == 5) ? ChatParamType.CHAT_PARAM_EQUIP : ChatParamType.CHAT_PARAM_ITEM;
            if (type == ChatParamType.CHAT_PARAM_ITEM)
            {
                return string.Format("{0},{1}", type, info.ID);
            }
            EquipTipInfo equipinfo = EquipDataMgr.Instance.GetMyTipInfo(info);
            return string.Format("{0},{1},{2}", type, info.ID, equipinfo.Serialize());
        }

        /// <summary>
        /// 解析发送物品信息。
        /// </summary>
        /// <param name="input">输入的消息。</param>
        /// <returns>解析结果。</returns>
        public static string ParseItem(string input)
        {
            StringBuilder sb = new StringBuilder();
            string txt = input;
            var indexText = 0;
            foreach (Match match in ItemRegex.Matches(input))
            {
                sb.Append(txt.Substring(indexText, match.Index - indexText));
                indexText = match.Index + match.Length;

                int bag = int.Parse(match.Groups[1].Value.Trim());
                int pos = int.Parse(match.Groups[2].Value.Trim());
                ItemInfo iteminfo = PlayerData.Instance.BagData.GetItemInfo(bag, pos);
                if (iteminfo != null)
                {
                    LuaTable cfg = ConfigManager.Instance.BagItem.GetItemConfig(iteminfo.ID);
                    if(cfg!=null)
                    {
                        string color = ConfigManager.Instance.BagItem.GetQualityColor(cfg.Get<int>("quality"));
                        string name = cfg.Get<string>("name");
                        string param = GetClickParam(iteminfo);
                        string text = string.Format("<href param={2}><color=#{0}>[{1}]</color></href>", color, name, param);
                        sb.Append(text);
                    }
                }
            }
            sb.Append(txt.Substring(indexText, txt.Length - indexText));
            return sb.ToString();
        }

        /// <summary>
        /// 解析发送表情。
        /// </summary>
        /// <param name="input">输入的消息。</param>
        /// <returns>解析结果。</returns>
        public static string ParseEmoji(string input)
        {
            StringBuilder sb = new StringBuilder();
            string txt = input;
            var indexText = 0;
            foreach (Match match in EmojiRegex.Matches(input))
            {
                sb.Append(txt.Substring(indexText, match.Index - indexText));
                indexText = match.Index + match.Length;
                
                int code = int.Parse(match.Groups[1].Value.Trim());
                string ico = ConfigManager.Instance.Common.GetEmoji(code);
                if (!string.IsNullOrEmpty(ico))
                {
                    string text = string.Format("<image name={0},2.9/>", ico);      //表情包要放大2.9倍
                    sb.Append(text);
                }
                else
                {
                    sb.Append(match.Groups[0]);     //没找到表情则继续发原本内容
                }
            }
            sb.Append(txt.Substring(indexText, txt.Length - indexText));
            return sb.ToString();
        }

        /// <summary>
        /// 解析发送坐标。
        /// </summary>
        /// <param name="input">输入的消息。</param>
        /// <returns>解析结果。</returns>
        public static string ParsePosition(string input)
        {
            StringBuilder sb = new StringBuilder();
            string txt = input;
            var indexText = 0;
            int line = MapMgr.Instance.Line;
            foreach (Match match in PositionRegex.Matches(input))
            {
                sb.Append(txt.Substring(indexText, match.Index - indexText));
                indexText = match.Index + match.Length;

                string mapname = match.Groups[1].Value.Trim();
                int map = ConfigManager.Instance.Map.GetMapID(mapname);
                if (!string.IsNullOrEmpty(mapname))
                {
                    int x = int.Parse(match.Groups[2].Value.Trim());
                    int z = int.Parse(match.Groups[3].Value.Trim());
                    string param = string.Format("{0},{1},{2},{3},{4}", ChatParamType.CHAT_PARAM_MAP_POS, map, line, x, z);         //超链接点击参数
                    string text = string.Format("<href param={3}><color=#00FF00>{0}({1},{2})</color></href>", mapname, x, z, param+",0");       //0表示寻路走过去
                    sb.Append(text);
                    text = string.Format("<href param={0}><color=#00FF00>[传送]</color></href>", param + ",1");       //1表示传送过去
                    sb.Append(text);
                }
                else
                {
                    sb.Append(match.Groups[0]);
                }
            }
            sb.Append(txt.Substring(indexText, txt.Length - indexText));
            return sb.ToString();
        }

        /// <summary>
        /// 获取发送的消息。
        /// </summary>
        /// <param name="input">输入的消息。</param>
        /// <returns>发送出去的消息。</returns>
        public static string GetSendMessage(string input)
        {
            string temp = input;
            temp = ParseEmoji(temp);        //先分析表情，否则物品添加的颜色信息会被解析成表情
            temp = ParseItem(temp);            
            temp = ParsePosition(temp);
            return temp;
        }

        /// <summary>
        /// 发送聊天消息请求。
        /// </summary>
        /// <param name="channel">频道。</param>
        /// <param name="toid">私聊对象id。</param>
        /// <param name="msg">消息内容。</param>
        public void SendChatMessageRequest(int channel, long toid, string msg)
        {
            //物品信息解析
            string sendmsg = GetSendMessage(msg);
            MsgData_cChat data = new MsgData_cChat();
            data.Channel = channel;
            data.ToID = toid;
            data.Text.AddRange(System.Text.Encoding.UTF8.GetBytes(sendmsg));
            data.TextSize = (uint)data.Text.Count;
            CoreEntry.netMgr.send(NetMsgDef.C_CHAT_SEND_MESSAGE, data);
            LastPrivateChatID = data.ToID;

            //私聊要自己把消息添加到列表中
            if (channel == ChatChannel.CHAT_CHANNEL_WHISPER)
            {
                MsgData_sChat info = new MsgData_sChat();
                info.SenderID = PlayerData.Instance.RoleID;
                info.SenderName = PlayerData.Instance.Name.StringToBytes();
                info.SenderLevel = PlayerData.Instance.BaseAttr.Level;
                info.Channel = channel;
                info.Text.AddRange(data.Text);

                ChatMessage chat = new ChatMessage(info);
                List<ChatMessage> msgs = null;
                if (!mPrivateMessage.TryGetValue(toid, out msgs))
                {
                    msgs = new List<ChatMessage>();
                    mPrivateMessage.Add(toid, msgs);
                }
                msgs.Insert(0, chat);
                if (msgs.Count > MessageCacheSize)
                {
                    msgs.RemoveRange(MessageCacheSize, msgs.Count - MessageCacheSize);
                }
            }

            ////测试 自己给自己发消息
            //MsgData_sChatSysNotice info = new MsgData_sChatSysNotice();
            //info.ID = 0;
            //info.Param.AddRange(System.Text.Encoding.UTF8.GetBytes(msg));
            //info.ParamSize = (uint)info.Param.Count;
            //ChatMessage chat = new ChatMessage(info);
            //AddMessage(chat);
            //TriggerEventSystemNotice(chat);

            ////测试
            //for (int i = 0; i < 30; ++i)
            //{
            //    //MsgData_sPrivateChatNotice pc = new MsgData_sPrivateChatNotice();
            //    //pc.SenderID = 0;
            //    //pc.SenderName = System.Text.Encoding.UTF8.GetBytes("哈哈哈");
            //    MsgData_sChat info = new MsgData_sChat();
            //    info.Channel = ChatChannel.CHAT_CHANNEL_WHISPER;
            //    info.SenderName = System.Text.Encoding.UTF8.GetBytes("哈哈哈");
            //    info.Text.AddRange(System.Text.Encoding.UTF8.GetBytes("私聊测试数据" + i));
            //    ChatMessage chat = new ChatMessage(info);
            //    AddMessage(chat);
            //}
        }

        /// <summary>
        /// 发送设置私聊状态请求。
        /// </summary>
        /// <param name="id">目标编号。</param>
        /// <param name="state">私聊状态,1接受对方私聊,0关闭私聊。</param>
        public void SendSetPrivateChatStateRequest(long id, int state)
        {
            MsgData_cSetPrivateChatState data = new MsgData_cSetPrivateChatState();
            data.RoleID = id;
            data.State = state;
            CoreEntry.netMgr.send(NetMsgDef.C_CHAT_SET_PRIVATE_CHAT_STATE, data);
        }
    }
}