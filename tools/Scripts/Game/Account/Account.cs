/**
* @file     : Account.cs
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-06-09
*/
using UnityEngine;
using System.Collections.Generic;
using XLua;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class Account
    {
        private long expiredTime;
        /// <summary>
        /// 访问时间
        /// </summary>
        public long ExpiredTime
        {
            get { return expiredTime; }
        }

        private byte[] cookie;
        /// <summary>
        /// cookie
        /// </summary>
        public byte[] Cookie
        {
            get { return cookie; }
        }

        private byte[] accountIDBytes;
        public byte[] AccountIDBytes
        {
            get { return accountIDBytes; }
        }

        private string accountId;
        public string AccountId
        {
            get { return accountId; }
        }

        private int isAdult = 1;
        public int IsAdult
        {
            get { return isAdult; }
            set { isAdult = value; }
        }

        private int serverId;
        public int ServerId
        {
            get { return serverId; }
            set { serverId = value; }
        }
        private int mzoneID = 0;
        public int ZoneID
        {
            get { return mzoneID; }
            set { mzoneID = value; }
        }
        private string serverName = "no";
        public string ServerName
        {
            get { return serverName; }
            set { serverName = value; }
        }

        private long serverTime;
        public long ServerTime
        {
            get { return serverTime; }
        }

        private int forbiddenTime;
        public int ForBiddenTime
        {
            get { return forbiddenTime; }
        }

        /// <summary>
        /// 改成帐号支持多角色后已经弃用。
        /// </summary>
        private long guid;
        public long GUID
        {
            get { return guid; }
        }

        private string exts;
        public string Exts
        {
            get { return exts; }
        }

        public int RoleNum
        {
            get { return roleInfoList.Count; }
        }

        public int RoleMaxLevel
        {
            get
            {
                int maxLevel = 0;
                for (int i = 0; i < roleInfoList.Count; i++)
                {
                    maxLevel = Mathf.Max(roleInfoList[i].Level, maxLevel);
                } 
               return maxLevel;
            }
        }

        /// <summary>
        /// 当前选择的角色索引。
        /// </summary>
        private int mSelectIndex;
        
        /// <summary>
        /// 获取或设置当前选择的角色索引。
        /// </summary>
        public int SelectIndex
        {
            get { return mSelectIndex; }
            set { mSelectIndex = value; }
        }

        /// <summary>
        /// 帐号ID。
        /// </summary>
        private long mAccountGuid;

        /// <summary>
        /// 获取帐号ID。
        /// </summary>
        public long AccountGuid
        {
            get { return mAccountGuid; }
        }

        public string Username;
        public string ServerIP = "0";
        public int ServerPort;
        public int loggedServers = 0;
        public bool isRecharging = false; //是否正在充值
        public int sdkUserId = 0;
        public string sdkUserName = "";
        private List<ServerRecord> recordServers = new List<ServerRecord>();
        public List<ServerRecord> RecordServers
        {
            get { return recordServers; }
        }

        private List<MsgData_sLoginRole> roleInfoList = new List<MsgData_sLoginRole>();
        public List<MsgData_sLoginRole> RoleInfoList
        {
            get { return roleInfoList; }
        }

        public MsgData_sLoginRole GetRoleInfo(int index)
        {
            if (index >= 0 && index < roleInfoList.Count)
                return roleInfoList[index];

            return null;
        }

        private static Account instance = null;
        public static Account Instance
        {
            get
            {
                if (null == instance)
                    instance = new Account();

                return instance;
            }
        }
        private Account()
        {

        }

        public void Init()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_LOGIN_MSG, GE_LOGIN_MSG);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_ROLE_INFO, GE_SC_ROLE_INFO);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_CREATE_ROLE, GE_SC_CREATE_ROLE);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_ENTER_SCENE, GE_SC_ENTER_SCENE);

            ReadEverServer();
        }

        public void RegisterNetMsg()
        {
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_COOKIE_UPDATE, OnCookieUpdate);
        }

        private void GE_LOGIN_MSG(GameEvent ge, EventParameter parameter)
        {
            roleInfoList.Clear();
            MsgData_sLogin data = parameter.msgParameter as MsgData_sLogin;
            //Debug.LogError("GE_LOGIN_MSG: ResultCode : " + data.ResultCode + "  guid:" + data.GUID + " mAccountGuid:" + data.AccountGUID + " serverid:" + data.ServerID);
            if (0 == data.ResultCode || -1 == data.ResultCode)
            {
                accountId = data.Account.BytesToString();
                accountIDBytes = data.Account;
                serverTime = data.ServerTime;
                forbiddenTime = data.ForbbidenTime;
                guid = data.GUID;
                serverId = (int)data.ServerID;
                exts = data.Exts.BytesToString();
                mAccountGuid = data.AccountGUID;

                long ctime = UiUtil.ConvertDateTimeToUnix(System.DateTime.Now) / 1000;
                UiUtil.CSTimeOffset = data.ServerTime - ctime;
                SDKMgr.Instance.SetExtUpData(1);
#if UNITY_EDITOR
                LogMgr.Log("Time Server:{0} Client:{1} Gap:{2}", data.ServerTime, ctime, UiUtil.CSTimeOffset);
#endif
            }
        }

        private void GE_SC_ROLE_INFO(GameEvent ge, EventParameter parameter) 
        {
           roleInfoList.Clear();

            MsgData_sRoleInfo info = parameter.msgParameter as MsgData_sRoleInfo;
            roleInfoList.AddRange(info.Roles);
            mSelectIndex = info.SelectIndex;
            LogMgr.Log("GE_SC_ROLE_INFO: count : " + roleInfoList.Count);
        }

        public long GetCurrRoleCreateTime()
        {
            long createTime = UiUtil.GetNowTimeStamp();
            if(roleInfoList.Count > mSelectIndex)
            {
                createTime = roleInfoList[mSelectIndex].CreateTime;
            }
            return createTime;
        }
        private void GE_SC_CREATE_ROLE(GameEvent ge, EventParameter parameter)
        {
            MsgData_sCreateRole data = parameter.msgParameter as MsgData_sCreateRole;
            LogMgr.Log("GE_SC_CREATE_ROLE: Result : " + data.Result);
            if (0 != data.Result)
                return;

            MsgData_sLoginRole info = new MsgData_sLoginRole();
            info.ID = data.ID;
            info.Job = data.Job;
            info.Name = data.Name;
            info.Level = 1;
            info.CreateTime = data.createtime;
            roleInfoList.Add(info);
            mSelectIndex = roleInfoList.Count - 1;

            AddEverServer();
            SDKMgr.Instance.SetExtUpData(2);
        }

        private void GE_SC_ENTER_SCENE(GameEvent ge, EventParameter parameter)
        {
            if (MapMgr.Instance.CurMapType == MapMgr.MapType.Map_SelectRole)
            {
                AddEverServer();
                SDKMgr.Instance.SetExtUpData(4);
            }
        }

        /// <summary>
        /// 请求创建角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="job"></param>
        /// <param name="icon"></param>
        /// <param name="channel"></param>
        /// <param name="exts"></param>
        public void SendReqCreateRole(string roleName, int job, int icon, string channel, string exts)
        {
            LogMgr.Log("SendReqCreateRole: roleName : " + roleName);
            MsgData_cCreateRole data = new MsgData_cCreateRole();
            NetLogicGame.str2Bytes(roleName, data.RoleName);
            NetLogicGame.str2Bytes(channel, data.Channel);
            NetLogicGame.str2Bytes(exts, data.Exts);
            data.Job = job;
            data.Icon = icon;
            data.AccountGUID = AccountGuid;
            data.CurrentRoleID = 0;
            CoreEntry.netMgr.send(NetMsgDef.C_CREATEROLE, data);
        }

        /// <summary>
        /// 请求进入游戏
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="ip"></param>
        /// <param name="mac"></param>
        /// <param name="openKey"></param>
        /// <param name="channel"></param>
        /// <param name="exts"></param>
        /// <param name="serverID"></param>
        /// <param name="loginType"></param>
        /// <param name="activityID"></param>
        /// <param name="pid"></param>
        public void SendReqEnterGame(string accountID, string ip, string mac, string openKey, string channel, string exts, int serverID, int loginType, int activityID, int pid, long rid)
        {
            LogMgr.Log("SendReqEnterGame: accountID : " + accountID);

            MsgData_cEnterGame data = new MsgData_cEnterGame();
            NetLogicGame.str2Bytes(accountID, data.Account);
            NetLogicGame.str2Bytes(ip, data.IP);
            NetLogicGame.str2Bytes(mac, data.MAC);
            NetLogicGame.str2Bytes(openKey, data.OpenKey);
            NetLogicGame.str2Bytes(channel, data.Channel);
            NetLogicGame.str2Bytes(exts, data.Exts);

            //xy channel
            if (ClientSetting.Instance.GetBoolValue("useThirdPartyLogin"))
            {
                int sdkType = ClientSetting.Instance.GetIntValue("thirdPartyComponent");
                if (sdkType == 1)
                {
                    NetLogicGame.str2Bytes(XYSDK.Instance.getMasterID(), data.Channel);
                    Debug.Log("getMasterID:XYSDK" + XYSDK.Instance.getMasterID());
                }
                else if (sdkType == 2)
                {
                    NetLogicGame.str2Bytes(DYBSDK.Instance.getMasterID(), data.Channel);
                    Debug.Log("getMasterID:DYBSDK" + DYBSDK.Instance.getMasterID());
                }
                else if (sdkType == 3)
                {
                    NetLogicGame.str2Bytes(SQWSDK.Instance.getMasterID(), data.Channel);
                    Debug.Log("getMasterID:SQWSDK" + SQWSDK.Instance.getMasterID());
                }
            } 
            
#if UNITY_ANDROID
            data.LoginType = 1;
#elif UNITY_IOS
            data.LoginType = 2;
#endif
            data.ServerID = serverID;
            data.ActivityID = activityID;
            data.PID = pid;
            data.SelectedRole = rid;
            data.AccountGUID = AccountGuid;
            CoreEntry.netMgr.send(NetMsgDef.C_ENTERGAME, data);
        }

        public void OnCookieUpdate(MsgData msg)
        {
            MsgData_sCookieUpdate data = msg as MsgData_sCookieUpdate;
            expiredTime = data.ExpiredTime;
            cookie = data.Cookie;
        }

        private void ReadEverServer()
        {
            recordServers.Clear();
            string file = "";
#if UNITY_EDITOR
            file = Application.dataPath + "/../server.txt";
#else
            file = Application.persistentDataPath + "/server.txt";
#endif
            if (File.Exists(file))
            {
                string content = File.ReadAllText(file);
                string[] items = content.Split('#');
                for (int i = 0; i < items.Length; i++)
                {
                    string[] item = items[i].Split(',');
                    if (item.Length == 3)
                    {
                        ServerRecord server = new ServerRecord();
                        server.username = item[0];
                        server.ip = item[1];
                        int.TryParse(item[2], out server.port);

                        recordServers.Add(server);
                    }
                }
            }
        }

        private void AddEverServer()
        {
            if(string.IsNullOrEmpty(Username))
            {
                return;
            }

            bool has = false;
            bool isChange = false;
            for (int i = 0; i < recordServers.Count; i++)
            {
                if (recordServers[i].username.Equals(Username) && recordServers[i].ip.Equals(ServerIP) && recordServers[i].port == ServerPort)
                {
                    has = true;

                    if (0 != i)
                    {
                        isChange = true;
                        ServerRecord tmp = recordServers[i];
                        recordServers.RemoveAt(i);
                        recordServers.Insert(0, tmp);
                    }

                    break;
                }
            }

            if (!has)
            {
                isChange = true;
                ServerRecord server = new ServerRecord();
                server.username = Username;
                server.ip = ServerIP;
                server.port = ServerPort;

                recordServers.Insert(0, server);
            }

            if (isChange)
            {
                SaveEverServer();
            }
        }

        private void SaveEverServer()
        {
            string content = "";
            for (int i = 0; i < recordServers.Count; i++)
            {
                if (i == 0)
                {
                    content = string.Format("{0},{1},{2}", recordServers[i].username, recordServers[i].ip, recordServers[i].port);
                }
                else
                {
                    content = string.Format("{0}#{1},{2},{3}", content, recordServers[i].username, recordServers[i].ip, recordServers[i].port);
                }
            }

            string file = "";
#if UNITY_EDITOR
            file = Application.dataPath + "/../server.txt";
#else
            file = Application.persistentDataPath + "/server.txt";
#endif

            if (File.Exists(file))
            {
                File.Delete(file);
            }

            File.WriteAllText(file, content);
        }

        /// <summary>
        /// 取得外网 IP Address
        /// </summary>
        /// <returns></returns>
        private string cip = string.Empty;
        public string GetExtranetIPAddress()
        {
            if (cip != string.Empty) return cip;
            string strAccountUrl = ClientSetting.Instance.GetStringValue("AccountUrl");
            if(strAccountUrl == string.Empty || strAccountUrl == null)
            {
                strAccountUrl = "http://center.wqyrygdt.xiyou-g.com:20003/";
            }
            HttpWebRequest request = HttpWebRequest.Create(strAccountUrl) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/5.0";
            WebResponse response = request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string result = reader.ReadToEnd();
                string pattern = @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}";
                cip = Regex.Match(result, pattern).ToString();
                response.Close();
            }
            return cip; 
        }

    }

    [LuaCallCSharp]
[Hotfix]
    public class ServerRecord
    {
        public string username;
        public string ip;
        public int port;
    }
}

