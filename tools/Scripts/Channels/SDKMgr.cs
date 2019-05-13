/**
* @file     : SDKMgr
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-09-21
*/
using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;
using LitJson;
namespace SG
{
    [LuaCallCSharp]
    [Hotfix]
    public enum SDK_TYPE : int
    {
        SG = 0,
        XY = 1,
        DYB = 2,
        SQW = 3,
    }

    [LuaCallCSharp]
    [Hotfix]
    public class SDKMgr
    {
        /// <summary>
        /// 是否登录过
        /// </summary>
        public bool bLoginStatus = false;
        private static SDKMgr _instance = null;
        public static SDKMgr Instance
        {
            get
            {
                if (null == _instance)
                    _instance = new SDKMgr();

                return _instance;
            }
        }

        public IThirdPartySDK thirdPartySDK = null;
        private int mSKDType = -1;
        public bool bSDK = false;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="gameObjName">SDK事件监听对象名</param>
        public void Init(GameObject obj)
        {
            Debug.Log("xy rechargeData: ==SDK事件监听对象名========启动游戏=======");


            SDKMgr.Instance.addBatchDataEvent(1, 0,"10010", "启动游戏");
            bSDK = ClientSetting.Instance.GetBoolValue("useThirdPartyLogin");
            if (bSDK)
            {
                mSKDType = ClientSetting.Instance.GetIntValue("thirdPartyComponent");
                Debug.Log("SDk Type ：" + mSKDType);
                if (mSKDType == (int)SDK_TYPE.SG)
                {
                    Debug.Log("三国 SDK 初始化");
                    thirdPartySDK = obj.AddComponent<SGAnalyser>();
                }
                else if(mSKDType == (int)SDK_TYPE.XY)
                {
                    Debug.Log("西游 SDK 初始化");
                    SDKMgr.Instance.TrackGameLog("1000", "游戏启动");
                    thirdPartySDK = obj.AddComponent<XYAnalyser>();
                }
                //第一波SDK初始化
                else if (mSKDType == (int)SDK_TYPE.DYB)
                {
                    Debug.Log("第一波 SDK 初始化");
                    SDKMgr.Instance.TrackGameLog("1000", "游戏启动");
                    thirdPartySDK = obj.AddComponent<DYBAnysis>();
                }
                //37玩SDK初始化
                else if (mSKDType == (int)SDK_TYPE.SQW)
                {
                    Debug.Log("37玩 SDK 初始化");
                    SDKMgr.Instance.TrackGameLog("1000", "游戏启动");
                    thirdPartySDK = obj.AddComponent<SQWAnysis>();
                }

                if (null != thirdPartySDK)
                {
                    thirdPartySDK.Init(obj.name);
                }
            }
            else
            {
                mSKDType = -1;
            }
        }

        public bool IsLogin()
        {

            if (thirdPartySDK != null)
            {
                Debug.Log("SDk 是登录");
                return thirdPartySDK.IsLogin();
            }
            else
            {
                Debug.Log("SDk 否登录");
                return false;
            }
        }


        /// <summary>
        /// 登录
        /// </summary>
        public void Login()
        {
            Debug.Log("SDk 登录");
            if (null != thirdPartySDK)
            {
                thirdPartySDK.Login();
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        public void Logout()
        {
            if (null != thirdPartySDK)
            {
                thirdPartySDK.Logout();
            }
        }

        /// <param name="data"></param>
        public void SetExtUpData(int nType)
        {
            if (null != thirdPartySDK)
            {
                Debug.Log("SetExtUpData nType ::" + nType);
                thirdPartySDK.SetExtUpData(nType);
            }
        }

        /// <summary>
        /// 上报数据
        /// </summary>
        /// <param name="data"></param>
        public void SetExtData(System.Collections.Generic.Dictionary<string, string> data)
        {
            if (null != thirdPartySDK)
            {
                thirdPartySDK.SetExtData(data);
            }
        }
        /// <summary>
        /// 用户中心
        /// </summary>
        public void EnterUserCenter()
        {
            if (null != thirdPartySDK)
            {
                thirdPartySDK.EnterUserCenter();
            }
        }


        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="configID"></param>
        /// 
        //按钮点击时间
        public float clickTime = 0;
        public void Pay(int configID)
        {
            Debug.Log("SDk Pay 支付");
            if (Time.time - clickTime < 1.5f)
            {
                return;
            }
            clickTime = Time.time;
            if (null != thirdPartySDK)
            {
                thirdPartySDK.Pay(configID);
            }
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void Exit()
        {
            if (null != thirdPartySDK)
            {
                thirdPartySDK.Exit();
               
            }
        }

        public string GetExtName(ThirdParty_ExtNameType type)
        {
            if (null != thirdPartySDK)
            {
               return  thirdPartySDK.GetExtName(type);
            }
            return "";
        }

        private SDKMgr()
        {
            //升级
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_PLAYER_LV, GE_PLAYER_LV);
            //创建角色
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_CREATE_ROLE, GE_SC_CREATE_ROLE);
            //进入游戏
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_AFTER_LOADSCENE, GE_AFTER_LOADSCENE);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, GE_CLEANUP_USER_DATA);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_PLAYER_INFO, GE_PLAYER_INFO);
            
            
        }
        public void GE_PLAYER_LV(GameEvent ge,EventParameter param)
        {
            //Debug.LogError("GE_PLAYER_LV :" + Account.Instance.GetCurrRoleCreateTime());
            if (bSDK && mSKDType == 1 && !bFirstEnter)
            {
                System.Collections.Generic.Dictionary<string, string> data = new System.Collections.Generic.Dictionary<string, string>();
                data["dataType"] = "3";
                data["roleID"] = PlayerData.Instance.RoleID.ToString();
                data["roleName"] = PlayerData.Instance.Name;
                data["roleLevel"] = PlayerData.Instance.BaseAttr.Level.ToString();
                data["zoneId"] = Account.Instance.ZoneID.ToString();
                data["zoneName"] = Account.Instance.ServerName;
                data["MoneyNum"] = PlayerData.Instance.UnbindMoney.ToString();

                this.SetExtData(data);
                
            }
            //第一波SDK 升级发送数据
            else if (bSDK && mSKDType == 2 && !bFirstEnter)
            {
                SendDYBData("3");

            }

        }

        public bool bFirstEnter = true; //true 第一次进入游戏场景角色属性初始完成  false 非第一次进入游戏场景角色属性初始完成
        public void GE_SC_CREATE_ROLE(GameEvent ge, EventParameter param)
        {
            //Debug.LogError("GE_SC_CREATE_ROLE :" + Account.Instance.GetCurrRoleCreateTime());

            MsgData_sCreateRole msgdata = param.msgParameter as MsgData_sCreateRole;
            if (bSDK && mSKDType == 1 && msgdata.Result == 0)
            {
                SDKMgr.Instance.addBatchDataEvent(1, SG.Account.Instance.ServerId, "10250", "创建角色成功");
                //创建角色成功
                System.Collections.Generic.Dictionary<string, string> data = new System.Collections.Generic.Dictionary<string, string>();
                data["dataType"] = "2";
                data["roleID"] = msgdata.ID.ToString();
                string roleName = msgdata.Name.BytesToString();
                string[] serverIdName = roleName.Split(']');
                if (serverIdName.Length == 2)
                {
                    roleName = serverIdName[1];
                }
                
                data["roleName"] = roleName;
                data["roleLevel"] = "1";
                data["zoneId"] = Account.Instance.ZoneID.ToString();
                data["zoneName"] = Account.Instance.ServerName;
                data["MoneyNum"] = "0";

                this.SetExtData(data);
                
            }
            //第一波SDK 创建角色发送数据
            else if (bSDK && mSKDType == 2 &&  msgdata.Result == 0)
            {
                string roleName = msgdata.Name.BytesToString();
                string[] serverIdName = roleName.Split(']');
                if (serverIdName.Length == 2)
                {
                    roleName = serverIdName[1];
                }
                SendDYBData("2", roleName, msgdata.ID);
            }

        }

        public void GE_PLAYER_INFO(GameEvent ge, EventParameter param)
        {
            if (PlayerData.Instance.BaseAttr.Level < 1)
                return;

            if (!bFirstEnter) return;
            if (bFirstEnter)
            {
                bFirstEnter = false;
            }
           
            if (bSDK && mSKDType == 1)
            {
                SDKMgr.Instance.addBatchDataEvent(1, SG.Account.Instance.ServerId, "10240", "获取角色信息成功,进入场景");

                int level = PlayerData.Instance.BaseAttr.Level;

                if (Account.Instance.RoleInfoList != null)
                {
                    MsgData_sLoginRole roleInfo = Account.Instance.RoleInfoList.Find(s => s.ID == PlayerData.Instance.RoleID);
                    if (roleInfo != null)
                    {
                        //level = roleInfo.Level;
                    }
                }
                //进入游戏
                System.Collections.Generic.Dictionary<string, string> data = new System.Collections.Generic.Dictionary<string, string>();
                data["dataType"] = "1";
                data["roleID"] = PlayerData.Instance.RoleID.ToString();
                data["roleName"] = PlayerData.Instance.Name;
                data["roleLevel"] = level.ToString();
                data["zoneId"] = Account.Instance.ZoneID.ToString();
                data["zoneName"] = Account.Instance.ServerName;
                data["MoneyNum"] = PlayerData.Instance.UnbindMoney.ToString();
                this.SetExtData(data);
            }

            else if (bSDK && mSKDType == 2)
            {
                SendDYBData("1");
                //上报登录信息
                reportDYBUserData();
            }
        }

        public string startIDscene = string.Empty;//进入场景唯一跟踪ID
        public void GE_AFTER_LOADSCENE(GameEvent ge, EventParameter param)
        {
            startIDscene = System.Guid.NewGuid().ToString("N"); 
        }

        /// <summary>
        /// DYB 进入游戏上报玩家数据
        /// </summary>
        public void reportDYBUserData()
        {
            
            WWWForm form = new WWWForm();
            string userID = Account.Instance.AccountId.ToString();
            string playerID = PlayerData.Instance.RoleID.ToString();
            string serverID = Account.Instance.ServerId.ToString();
            string key = "a0c6739105a24ce9c2c8a3109d6d6c33";
            
            //md5 key值
            string md5 = Md5Util.MD5Encrypt(serverID + userID + playerID + key);

            form.AddField("serverid", serverID);
            form.AddField("userID", userID);
            form.AddField("playerID", playerID);

            form.AddField("token", md5);

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["userID"] = userID;
            data["playerID"] = playerID;
            data["serverid"] = serverID;
            data["servername"] = Account.Instance.ServerName.ToString();
            string serverInfo=JsonMapper.ToJson(data);
            form.AddField("data", serverInfo);

            Debug.Log("上报信息token: serverID : " + serverID + " userID:" + userID + " playerID:" + playerID + " key:" + key);

            //进入游戏上报
            int sdkType = ClientSetting.Instance.GetIntValue("thirdPartyComponent");
            int subChannel = ClientSetting.Instance.GetIntValue("SubChannel");
            MonoInstance.Instance.StartCoroutine(PostPhp(GetLoggedUrl(sdkType,subChannel), form));
        }

        public void GE_CLEANUP_USER_DATA(GameEvent ge, EventParameter param)
        {
            bFirstEnter = true;
        }

        public void TrackRechargeSuccessEvent(int id)
        {
            if (bSDK && mSKDType == 1)
            {
                XLua.LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
                System.Collections.Generic.Dictionary<int, XLua.LuaTable> diamond = G.Get<System.Collections.Generic.Dictionary<int, XLua.LuaTable>>("t_diamond");
                if (diamond == null)
                {
                    return;
                }
                XLua.LuaTable tbl = null;
                if (diamond.TryGetValue(id, out tbl))
                {
                    int money = tbl.Get<int>("buy_ios");
                    XYSDK.Instance.trackRechargeSuccess(money);
                }
            }
        }

        //xy 自定义埋点
        public string startid = null; 
        public void addBatchDataEvent(int childtype, int serverid, string act, string des)
        {
            if(startid == null)
            {
                startid = System.Guid.NewGuid().ToString();
            }
            if (bSDK && mSKDType == 1)
            {
                XYSDK.Instance.addBatchDataEvent(childtype, startid, serverid, act, des, System.Guid.NewGuid().ToString());
            }
        }

        public void TrackGameLog(string step, string desc)
        {
            if (bSDK && mSKDType == 1)
            {
                string ic = "TrackEvent";
                string cat = "Client";
                string act = "GameStart";
                string channel_id = XYSDK.Instance.getMasterID();
                string app_id = XYSDK.Instance.GetAppID();
                string start_step = step;
                string step_des = WWW.EscapeURL(desc);
                string start_id = XYSDK.Instance.GetStartID();
                System.TimeSpan ts = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                long lt = (long)ts.TotalSeconds;
                string event_time = lt.ToString();
                string log_id = XYSDK.Instance.GetLogID();
                string cost_time = "1";
                string iemi_original = Miscellaneous.GetDeveiceId();
                string mac_original = Miscellaneous.GetMac();
                string android_original = Miscellaneous.GetSecureID();
                string cliend_id = Account.Instance.GetExtranetIPAddress();
                string idfa_original = XYSDK.Instance.getIdfa();

                string url = string.Format("http://analytics.52xiyou.com/mo.json?ic={0}&cat={1}&act={2}&channel_id={3}&app_id={4}&start_step={5}&step_des={6}&start_id={7}&event_time={8}&log_id={9}&cost_time={10}&iemi_original={11}&mac_original={12}&android_original={13}&client_ip={14}&idfa_original={15}",
                    ic, cat, act, channel_id, app_id, start_step, step_des, 
                    start_id, event_time, log_id, cost_time, 
                    iemi_original, mac_original, android_original, cliend_id, idfa_original);

               // Debug.LogError(url);
                MonoInstance.Instance.StartCoroutine(HttpGet(url));
            }
        }
        //西游网用户轨迹日志 20181023
        public void TrackGameUser(float x, float y)
        {
            return;
            if (bSDK && mSKDType == 1)
            {
                PlayerObj obj = CoreEntry.gActorMgr.MainPlayer;
                if(obj == null)return; 
                //if (!obj.isJoystickMove) return;

                string ic = "TrackEvent";
                string cat = "User";
                string act = "Track";
                string channel_id = XYSDK.Instance.getMasterID(); // 渠道ID
                string app_id = XYSDK.Instance.GetAppID();//应用ID
                string server_id = SG.Account.Instance.ServerId.ToString();//服务器ID
                string cserver_id = SG.Account.Instance.ServerId.ToString();//当前服务器ID
                LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;  
                string user_id = "" + G.GetInPath<int>("ModelManager.LoginModel.userID");//用户ID
                string role_id = PlayerData.Instance.RoleID.ToString();//角色ID
                string role_name = WWW.EscapeURL("[" + Account.Instance.ServerId.ToString() + "]" + PlayerData.Instance.Name);//角色名称
                string client_ip = Account.Instance.GetExtranetIPAddress();//客户端IP
                System.TimeSpan ts = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                long lt = (long)ts.TotalSeconds; 
                string event_time = lt.ToString();//事件时间
                string log_id = Bundle.Md5Tool.Md5Sum(channel_id + app_id + server_id + "UserTrack" + event_time + user_id + role_id+ Random.Range(1,1000000));//日志ID
                string map_type = CoreEntry.gGameMgr.GetCurSceneType().ToString();//地图类型
                string map_id = MainRole.Instance.mapid.ToString();//地图ID
                string x_value = x.ToString();//X坐标
                string y_value = "0";//y坐标
                string z_value = y.ToString();//z坐标
                string track_id = startIDscene;//追踪ID

                string url = string.Format("http://analytics.52xiyou.com/mg.json?ic={0}&cat={1}&act={2}&channel_id={3}&app_id={4}&server_id={5}&cserver_id={6}&user_id={7}&role_id={8}&role_name={9}&client_ip={10}&event_time={11}&log_id={12}&map_type={13}&map_id={14}&x_value={15}&y_value={16}&z_value={17}&track_id={18}",
                    ic, cat, act, channel_id, app_id, server_id, cserver_id,
                    user_id, role_id, role_name, client_ip,
                    event_time, log_id, map_type, map_id,x_value,y_value,z_value,track_id);
                //Debug.Log(url);
                MonoInstance.Instance.StartCoroutine(HttpGet(url));
   
            }
        }
        /// <summary>
        /// 本接口采集玩家在游戏内点击各个面板按钮的日志，配置表在MainPanelUIConfig.xlsx表的t_panelindex页签
        /// </summary>
        /// <param name="id">t_panelindex配置id</param>
        /// <param name="buttonIndex">各个面板里面子按钮的自定义编号</param>
        public void TrackGameOptLog(int id, int buttonIndex)
        {
           if (bSDK && mSKDType == 1)
            {
                LuaTable tb = ConfigManager.Instance.Common.GetPanelIndexConfig(id);
                if (null == tb)
                {
                    LogMgr.UnityError("no config found in t_panelindex with id=" + id);

                    return;
                }
                string ic = "TrackEvent";
                string cat = "Client";
                string act = "RolePanelOP";
                string channel_id = XYSDK.Instance.getMasterID();
                string app_id = XYSDK.Instance.GetAppID();
                int server_id = SG.Account.Instance.ServerId;
                int cserver_id = SG.Account.Instance.ServerId;
                string user_id;
                LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
                user_id = "" + G.GetInPath<int>("ModelManager.LoginModel.userID");
                string role_id = PlayerData.Instance.RoleID.ToString();
                string role_name = WWW.EscapeURL("[" + Account.Instance.ServerId.ToString() + "]" + PlayerData.Instance.Name);
                string client_ip = Account.Instance.GetExtranetIPAddress(); //Network.player.externalIP.Equals("UNASSIGNED_SYSTEM_ADDRESS") ? Network.player.ipAddress : Network.player.externalIP;
                System.TimeSpan ts = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                long lt = (long)ts.TotalSeconds;
                string event_time = lt.ToString();
                string log_id = Bundle.Md5Tool.Md5Sum(channel_id + app_id + server_id + "RolePanelOP" + event_time + user_id + role_id);
                int button_tid = tb.Get<int>("index");
                int button_id = button_tid + buttonIndex;

                string url = string.Format("http://analytics.52xiyou.com/mo.json?ic={0}&cat={1}&act={2}&channel_id={3}&app_id={4}&server_id={5}&cserver_id={6}&user_id={7}&role_id={8}&role_name={9}&client_ip={10}&event_time={11}&log_id={12}&button_id={13}&button_tid={14}",
                    ic, cat, act, channel_id, app_id, server_id, cserver_id,
                    user_id, role_id, role_name, client_ip,
                    event_time, log_id, button_id, button_tid);

               // LogMgr.Log("url: " + url);
                MonoInstance.Instance.StartCoroutine(HttpGet(url));
            }
        }

        public IEnumerator HttpGet(string url)
        {
            //Debug.LogError("请求 .....url........"+ url);

            WWW www = new WWW(url);
            yield return www;
            //Debug.LogError("返回状态==" + www.text);
            if (null != www)
            {
                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogError(www.error);
                }

                
            }
        }



        /// <summary>
        /// http表单请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public IEnumerator PostPhp(string url, WWWForm form)
        {
         
            WWW www = new WWW(url,form);
            yield return www;
            //Debug.LogError("返回状态==" + www.text);
            if (null != www)
            {
                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogError("上报访问错误"+www.error);
                }
                else if (www.text != null)
                {
                    Debug.Log("上报请求回复： " + www.text);
                }
            }
            else
            {
                Debug.Log("链接失败");
            }
        }

        public void ReportLogout()
        {
            if (bSDK && mSKDType == 1)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data["dataType"] = "4";
                data["roleID"] = PlayerData.Instance.RoleID.ToString();
                data["roleName"] = PlayerData.Instance.Name;
                data["roleLevel"] = PlayerData.Instance.BaseAttr.Level.ToString();
                data["zoneId"] = Account.Instance.ZoneID.ToString();
                data["zoneName"] = Account.Instance.ServerName;
                data["MoneyNum"] = PlayerData.Instance.UnbindMoney.ToString();

                this.SetExtData(data);
            }
        }
        //西游打点
        public void TagAdd(int code,string desc)
        {
            if (bSDK && mSKDType == 1)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data["dataType"] = "1000";
                data["roleID"] = PlayerData.Instance.RoleID.ToString();
                data["roleName"] = desc.ToString();
                data["roleLevel"] = PlayerData.Instance.BaseAttr.Level.ToString();
                data["zoneId"] = code.ToString();
                data["zoneName"] = Account.Instance.ServerName;
                data["MoneyNum"] = PlayerData.Instance.UnbindMoney.ToString();
#if UNITY_EDITOR
#elif UNITY_IOS   
#elif UNITY_ANDROID
                this.SetExtData(data);
#endif
            }
        }


        public string appversion()
        {
            if(thirdPartySDK != null)
            {
                Debug.Log("SDK Appversion： " + thirdPartySDK.appversion());
                return thirdPartySDK.appversion();
            }
            return "";
        }

        /// <summary>
        /// 第一波SDK数据同步
        /// </summary>
        /// <param name="type">数据类型 1进入游戏 2创建角色 3 升级 4 退出</param>
        public void SendDYBData(string type, string createRoleName = "", long craeteRoleID = 0)
        {
            int level = PlayerData.Instance.BaseAttr.Level;

            if (Account.Instance.RoleInfoList != null)
            {
                MsgData_sLoginRole roleInfo = Account.Instance.RoleInfoList.Find(s => s.ID == PlayerData.Instance.RoleID);
                if (roleInfo != null)
                {
                    if(type == "2")
                    {
                        level = roleInfo.Level;
                    }
                }
            }

            //进入游戏
            System.Collections.Generic.Dictionary<string, string> data = new System.Collections.Generic.Dictionary<string, string>();
            data["dataType"] = type;
            data["roleLevel"] = level.ToString();
            data["roleId"] = type == "2" ? craeteRoleID.ToString() : PlayerData.Instance.RoleID.ToString();
            data["roleName"] = type == "2" ? createRoleName : PlayerData.Instance.Name;
            data["vipLevel"] = PlayerData.Instance.BaseAttr.VIPLevel.ToString();
            //金币
            data["roleBalance"] = PlayerData.Instance.BindGold.ToString();
            data["serverName"] = Account.Instance.ServerName;
            data["serverId"] = Account.Instance.ServerId.ToString();

            Debug.Log("传送当前服务器serverId: " + Account.Instance.ServerId.ToString());
            
            data["society"] = PlayerData.Instance.GuildData.GuildName;
            if (data["society"] == "")
                data["society"] = "无";

            this.SetExtData(data);
        }

        public string GetDefaultServersURL(int nsdkType,int channel)
        {
            string url = string.Empty;
            if (nsdkType == (int)SDK_TYPE.DYB)
            {
                url = "http://init.wqyry.szdiyibo.com/server/loginlist.php";
                if (channel == (int)SUB_CHANNEL.Dyb_QY)
                {
                    url = "http://qiyuinit.wqyry.szdiyibo.com/server/loginlist.php";
                }
                else if (channel == (int)SUB_CHANNEL.Dyb_QF)
                {
                    url = "http://qfinit.wqyry.szdiyibo.com/server/loginlist.php";
                }
                else if (channel == (int)SUB_CHANNEL.Dyb_XY)
                {
                    url = "http://xunyouinit.wqyry.szdiyibo.com/server/loginlist.php";
                }
            }
            else if(nsdkType == (int)SDK_TYPE.SQW)
            {
                url = "http://37init.wqyry.szdiyibo.com/server/loginlist.php";
            }

            Debug.Log("GetDefaultServersURL: " + url);
            return url;
        }

        public string GetServersURL(int nsdkType, int channel)
        {
            string url = string.Empty;
            if (nsdkType == (int)SDK_TYPE.DYB)
            {
                url = "http://init.wqyry.szdiyibo.com/server/serverlist_pack.php";
                if (channel == (int)SUB_CHANNEL.Dyb_QY)
                {
                    url = "http://qiyuinit.wqyry.szdiyibo.com/server/serverlist.php";
                }
                else if (channel == (int)SUB_CHANNEL.Dyb_QF)
                {
                    url = "http://qfinit.wqyry.szdiyibo.com/server/serverlist.php";
                }
                else if (channel == (int)SUB_CHANNEL.Dyb_XY)
                {
                    url = "http://xunyouinit.wqyry.szdiyibo.com/server/serverlist.php";
                }
            }
            else if (nsdkType == (int)SDK_TYPE.SQW)
            {
                url  = "http://37init.wqyry.szdiyibo.com/server/serverlist.php";
            }

            Debug.Log("GetServersURL: " + url);
            return url;
        }

        //登入上报后台
        public string GetLoggedUrl(int nsdkType, int channel)
        {
            string url = string.Empty;
            if (nsdkType == (int)SDK_TYPE.DYB)
            {
                url = "http://init.wqyry.szdiyibo.com/server/logged.php";
                if (channel == (int)SUB_CHANNEL.Dyb_QY)
                {
                    url = "http://qiyuinit.wqyry.szdiyibo.com/server/logged.php";
                }
                else if (channel == (int)SUB_CHANNEL.Dyb_QF)
                {
                    url = "http://qfinit.wqyry.szdiyibo.com/server/logged.php";
                }
                else if (channel == (int)SUB_CHANNEL.Dyb_XY)
                {
                    url = "http://xunyouinit.wqyry.szdiyibo.com/server/logged.php";
                }
            }
            else if (nsdkType == (int)SDK_TYPE.SQW)
            {
                url = "http://37init.wqyry.szdiyibo.com/server/logged.php";
            }

            Debug.Log("GetLoggedUrl: " + url);
            return url;
        }
    }
}

