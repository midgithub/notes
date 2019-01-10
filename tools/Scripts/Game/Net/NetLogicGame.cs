
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using XLua;
using System.Text;

namespace SG
{

    [LuaCallCSharp]
[Hotfix]
    public class NetLogicGame
    {

        static NetLogicGame mInstance;

        public static NetLogicGame Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new NetLogicGame();
                }

                return mInstance;
            }
        }

        public NetLogicGame()
        {
        }

        public bool IsCrossLinking = false;

        public static void str2Bytes(string strInput, byte[] output)
        {
            if (strInput.Length <= output.Length)
                System.Text.Encoding.UTF8.GetBytes(strInput).CopyTo(output, 0);
            else
                LogMgr.LogError("协议参数超过长度");
        }

        /// <summary>
        /// 返回创建角色结果
        /// </summary>
        /// <param name="msg"></param>
        public void OnCreateRole(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_CREATE_ROLE, EventParameter.Get(msg));
        }

        /// <summary>
        /// 返回角色信息
        /// </summary>
        /// <param name="msg"></param>
        public void OnRoleInfo(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ROLE_INFO, EventParameter.Get(msg));
        }

        /// <summary>
        /// 请求登录账号
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="platform"></param>
        /// <param name="game_name"></param>
        /// <param name="i4server_id"></param>
        /// <param name="i4time"></param>
        /// <param name="i4is_adult"></param>
        /// <param name="exts"></param>
        /// <param name="sign"></param>
        /// <param name="mac"></param>
        /// <param name="version"></param>
        /// <param name="pf"></param>
        public void SendReqcLogin(string accountID, // 玩家ID
            string platform, // 平台
            string game_name, // 游戏名
            uint i4server_id, // 区服ID
            uint i4time, // 时间
            uint i4is_adult, // 防沉迷标记
            string exts, // 扩展信息
            string sign, // 签名
            string mac, // 物理地址
            string version, // 协议版本
            string pf) // 渠道)
        {
            MsgData_cLogin msgdata = new MsgData_cLogin();
            str2Bytes(accountID, msgdata.Account);
            str2Bytes(platform, msgdata.Platform);
            str2Bytes(game_name, msgdata.GameName);
            str2Bytes(exts, msgdata.Exts);
            str2Bytes(sign, msgdata.Sign);
            str2Bytes(mac, msgdata.Mac);
            str2Bytes(version, msgdata.Version);
            str2Bytes(pf, msgdata.Channel);

            msgdata.ServerID = i4server_id;
            msgdata.ClientTime = i4time;
            msgdata.IsAdult = i4is_adult;

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_LOGIN, msgdata);

        }
        public void SendReqcLoginLY(
              int i4server_id, // 区服ID
              string data, // 游戏名
              string  Mac, // 本地mac地址
              string Version // 协议版本号
              )  
        {
            MsgData_cLogin_LY msgdata = new MsgData_cLogin_LY(); 
            msgdata.data = new List<byte>(System.Text.Encoding.UTF8.GetBytes(data));
            msgdata.serverid = i4server_id;
            msgdata.dataInfo = (uint)msgdata.data.Count;
            str2Bytes(Mac, msgdata.Mac);
            str2Bytes(Version, msgdata.Version);
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_LOGIN_LY, msgdata);

        }
         
        public void SendGetServerList(
          int i4server_id, // 区服ID
          string ipadd,//ip地址
          string data  // token字符串
          )
        {
            MsgData_cLogin_XYList msgdata = new MsgData_cLogin_XYList();
            str2Bytes(ipadd, msgdata.ipadd);
#if UNITY_ANDROID
            msgdata.Platform = 1;
#elif UNITY_IOS
            msgdata.Platform = 2;
#endif  
            str2Bytes(data, msgdata.data);
            msgdata.serverid = i4server_id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GETSERVERLIST, msgdata);

        }

        public void SendReqGetRechargeorder(
            int ItemID,int ItemNum,int serverID,string pid,string serverName
           )
        {
            LogMgr.Log("ItemID:" + ItemID + " ItemNum:" + ItemNum + " serverID:" + serverID + " pid:" + pid + "  serverName: " + serverName);
            MsgData_cGetRechargeorder msgdata = new MsgData_cGetRechargeorder();
            msgdata.ItemID = ItemID;
            msgdata.ItemNum = ItemNum;
            msgdata.ServerID = serverID;
            msgdata.PidSize = (sbyte)System.Text.Encoding.UTF8.GetBytes(pid).Length;
            msgdata.ServerNameSize = (sbyte)System.Text.Encoding.UTF8.GetBytes(serverName).Length;
            str2Bytes(pid, msgdata.Pid);
            str2Bytes(serverName, msgdata.ServerName);
             
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GETRECHARGEORDER, msgdata); 
        }

        /// <summary>
        /// DYB创建支付订单申请
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="ItemNum"></param>
        /// <param name="serverID"></param>
        /// <param name="pid"></param>
        /// <param name="serverName"></param>
        public void SendReqGetRechargeorder_DYB(
           int ItemID, int ItemNum, int serverID, string pid, string serverName
          )
        {
            LogMgr.Log("ItemID:" + ItemID + " ItemNum:" + ItemNum + " serverID:" + serverID + " pid:" + pid + "  serverName: " + serverName);
            MsgData_cGetRechargeorder_DYB msgdata = new MsgData_cGetRechargeorder_DYB();
            msgdata.ItemID = ItemID;
            msgdata.ItemNum = ItemNum;
            msgdata.ServerID = serverID;
            msgdata.PidSize = (sbyte)System.Text.Encoding.UTF8.GetBytes(pid).Length;
            msgdata.ServerNameSize = (sbyte)System.Text.Encoding.UTF8.GetBytes(serverName).Length;
            str2Bytes(pid, msgdata.Pid);
            str2Bytes(serverName, msgdata.ServerName);
            Debug.Log("发送第一拨订单申请");
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_CREATECHARGEORDER_DYB, msgdata);
        }

        /// <summary>
        /// 登录返回
        /// </summary>
        /// <param name="msg"></param>
        public void OnLogin(MsgData msg)
        {
            //MsgData_sLogin data = msg as MsgData_sLogin;

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_LOGIN_MSG, EventParameter.Get(msg));
        }

        /// <summary>
        /// 进入游戏返回
        /// </summary>
        /// <param name="msg"></param>
        public void OnEnterGame(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ENTER_GAME, EventParameter.Get(msg));
        }

        /// <summary>
        /// 服务器通知进入场景
        /// </summary>
        /// <param name="msg"></param>
        public void OnEnterScene(MsgData msg)
        {
            MsgData_sEnterScene data = msg as MsgData_sEnterScene;
            LogMgr.UnityLog(string.Format("OnEnterScene :  Result :{0},PosX:{1},PosY:{2},Dir:{3},MapID:{4},FubenID:{5},EnterType:{6},ServerTime:{7},MegerServerTime:{8}", data.Result, data.PosX, data.PosY, data.Dir, data.MapID, data.FubenID, data.EnterType, data.ServerTime, data.MegerServerTime));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ENTER_SCENE, EventParameter.Get(msg));
        }        


        /// <summary>
        /// 服务器通知对象死亡
        /// </summary>
        /// <param name="msg"></param>
        public void OnObjectDead(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_OBJ_DEAD, EventParameter.Get(msg));
        }

        /// <summary>
        /// 主角进入场景
        /// </summary>
        /// <param name="enterType"></param>
        public void SendReqMainPlayerEnterScene(int enterType)
        {
            MsgData_cMainPlayerEnterScene data = new MsgData_cMainPlayerEnterScene();
            data.InitGame = enterType;

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_MAINPLAYERENTERSCEN, data);
        }

        /// <summary>
        /// 请求离开游戏，回到角色选择界面
        /// </summary>
        public void SendReqLeaveGame()
        {
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_LEAVE_GAME, new MsgData_cLeaveGame());
        }

        /// <summary>
        /// 返回离开游戏消息
        /// </summary>
        /// <param name="msg"></param>
        public void OnLeaveGame(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_LEAVE_GAME, EventParameter.Get(msg));
        }

        /// <summary>
        /// 请求退出登录
        /// </summary>
        public void SendReqLogout()
        {
            MsgData_cLogout data = new MsgData_cLogout();
            data.AccountGUID = Account.Instance.AccountGuid;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_LOGOUT, data);
        }

        ///////////////////////////////////
        //移动
        //////////////////////////////////

        /// <summary>
        /// 主角请求移动
        /// </summary>
        /// <param name="srcX"></param>
        /// <param name="srcY"></param>
        /// <param name="dirX"></param>
        /// <param name="dirY"></param>
        public void SendReqMoveTo(int srcX, int srcY, int dirX, int dirY)
        {
            MsgData_cMoveTo data = new MsgData_cMoveTo();
            data.SrcX = srcX;
            data.SrcY = srcY;
            data.DestX = dirX;
            data.DestY = dirY;
            SDKMgr.Instance.TrackGameUser(dirX, dirY);
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_MOVETO, data);
        }

        /// <summary>
        /// 收到其它玩家的移动消息
        /// </summary>
        /// <param name="msg"></param>
        public void OnOtherMoveTo(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OTHER_MOVE_TO, EventParameter.Get(msg));
        }

        /// <summary>
        /// 主角请求改变方向
        /// </summary>
        /// <param name="dir"></param>
        public void SendReqChangeDir(double dir)
        {
            MsgData_cChangeDir data = new MsgData_cChangeDir();
            data.Dir = dir;

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_CHANGEDIR, data);
        }

        /// <summary>
        /// 其它玩家改变方向
        /// </summary>
        /// <param name="msg"></param>
        public void OnOtherChangeDir(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OTHER_CHANGE_DIR, EventParameter.Get(msg));
        }

        /// <summary>
        /// 主角请求停止移动
        /// </summary>
        /// <param name="stopX"></param>
        /// <param name="stopY"></param>
        /// <param name="dir"></param>
        public void SendReqMoveStop(int stopX, int stopY, int dir)
        {
            MsgData_cMoveStop data = new MsgData_cMoveStop();
            data.StopX = stopX;
            data.StopY = stopY;
            data.Dir = dir;

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_MOVESTOP, data);
        }

        /// <summary>
        /// 发送传送请求。
        /// </summary>
        /// <param name="type">传送类型。</param>
        /// <param name="map">地图编号。</param>
        /// <param name="x">地图X坐标</param>
        /// <param name="y">地图Y坐标</param>
        public void SendTeleportRequest(int type, int map, int x, int y)
        {
            //Debug.LogError("==== 发送传送请求===type=" + type);
            if (PlayerData.Instance.BaseAttr.GetUnitBit((int)UnitBit.UNIT_BIT_IN_PK))
            {
                UITips.ShowTips("PK状态中，无法传送");
                return;
            }
            if(map == MapMgr.Instance.EnterMapId)   //同一地图
            {
                Vector3 myPos = CoreEntry.gActorMgr.MainPlayer.transform.position;
                float tt = Math.Abs((myPos.x - x) * (myPos.x - x)) + Math.Abs((myPos.z - y) * (myPos.z - y));
                if (tt < 16)  //距离大于 30*30  
                {
                    UITips.ShowTips("目标就在附近，无需使用小飞鞋");
                    return;
                }
            }
            if(UISwitchScene.IS_REVIEW == 1)
            {
                UISwitchScene.InitUITip(0,0,type, map,x,y);
                UISwitchScene.TeleportRequest += DoTeleport;
            }
            else
            {
                DoTeleport(type, map, x, y);
            }

        }

        public void DoTeleport(int type, int map, int x, int y)
        {
            CoreEntry.gAutoAIMgr.AutoFight = false;  //传送前停止打怪
            CoreEntry.gActorMgr.MainPlayer.StopMove(false);  //传送前先停止移动
            MsgData_cTeleport data = new MsgData_cTeleport();
            data.Type = type;
            data.MapID = map;
            data.X = x;
            data.Y = y;
            MoveDispatcher.bFly = true;   //设置角色状态。传送中
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TELEPORT, data);
        }

        /// <summary>
        /// 收到其它玩家，怪物停止消息
        /// </summary>
        /// <param name="msg"></param>
        public void OnOtherMoveStop(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OTHER_MOVE_STOP, EventParameter.Get(msg));
        }

        /// <summary>
        /// 收到怪物移动消息
        /// </summary>
        /// <param name="msg"></param>
        public void OnMonsterMoveTo(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_MONSTER_MOVE_TO, EventParameter.Get(msg));
        }

        /// <summary>
        /// 收到传送结果消息
        /// </summary>
        public void OnTeleportResult(MsgData msg)
        {
            ////移动一点点
            //if (CoreEntry.gActorMgr.MainPlayer != null)
            //{
            //    Vector3 position = CoreEntry.gActorMgr.MainPlayer.transform.position;
            //    Vector3 srcServerPos = ServerPositionConverter.ConvertToServerPosition(position - new Vector3(1, 0, 1));
            //    Vector3 destServerPos = ServerPositionConverter.ConvertToServerPosition(position);
            //    NetLogicGame.Instance.SendReqMoveTo((int)srcServerPos.x, (int)srcServerPos.y, (int)destServerPos.x, (int)destServerPos.y);
            //}

            MoveDispatcher.bFly = false;
            ActorObj actor = CoreEntry.gActorMgr.MainPlayer;
            if (null != actor)
            {
                actor.AutoPathFind = false;
            }

            MsgData_sTeleportResult data = msg as MsgData_sTeleportResult;
            EventParameter ep = EventParameter.Get();
            ep.intParameter = data.Type;
            ep.intParameter1 = data.Result;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TELEPORT, ep);

            MulScenesPathFinder.Instance.CancelMove();
        }

        /// <summary>
        /// 收到改变位置消息
        /// </summary>
        public void OnChangePos(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CHANGE_POS, EventParameter.Get(msg));
        }

        ///////////////////////////////////
        //对象进入和离开场景
        //////////////////////////////////

        public void OnAttrInfo(MsgData msg)
        {
            MsgData_sRoleAttrInfoNotify data = msg as MsgData_sRoleAttrInfoNotify;
            if (data.RoleID == PlayerData.Instance.RoleID)
            {
                //玩家属性更新
                PlayerData.Instance.OnUpdateAttr(data.AttrList);
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ATTRINFO, EventParameter.Get(msg));
        }

        public void OnObjEnterScene(NetReadBuffer buffer)
        {
            EventParameter ep = EventParameter.Get();
            ep.objParameter = buffer;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OBJ_ENTER_SCENE, ep);
        }

        public void OnObjLeaveScene(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OBJ_LEAVE_SCENE, EventParameter.Get(msg));
        }
        public void OnObjDisappear(MsgData msg)
        { 
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OBJ_Disapper, EventParameter.Get(msg));

        }

        /////////////////////////////////// 
        //技能
        /////////////////////////////////// 

        /// <summary>
        /// 技能
        /// </summary>
        /// <param name="nSkillID"></param>
        /// <param name="nTargetID"></param>
        /// <param name="PosX"></param>
        /// <param name="PosY"></param>
        public void SendReqCastSkill(int nSkillID, long nTargetID, double PosX, double PosY ,float selfPosX ,float selfPosY ,float selfDir ,List<long> targets)
        {
            MsgData_cCastMagic data = new MsgData_cCastMagic();
           
            //LogMgr.UnityLog(string.Format("SendReqCastSkill nSkillID:{0} nTargetID:{1} PosX:{2} PosY:{3}" ,nSkillID,nTargetID,PosX,PosY));

            data.SkillID = nSkillID;
            data.TargetID = nTargetID;
            data.PosX = PosX;
            data.PosY = PosY;
            data.SelfPosX = selfPosX;
            data.SelfPosY = selfPosY;
            data.Dir = selfDir;

            if (targets == null || targets.Count == 0)
            {
                data.TargetCount = 0;
            }
            else
            {
                data.TargetCount = (sbyte)targets.Count;
                data.TargetList = targets;
            }

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_CASTSKILL, data);
        }

        //释放技能 不太需要处理这个
        public void OnCastSkill(MsgData msg)
        {
            MsgData_sCastMagic data = msg as MsgData_sCastMagic;
       
            LogMgr.UnityLog(string.Format("CastMagicResult SkillID:{0} ResultCode:{1}", data.SkillID, data.ResultCode));

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_CASTSKILL, EventParameter.Get(msg));
        }

        //打断技能
        public void SendReqBreakSkill(int nSkillID, long nTargetID, double PosX, double PosY)
        {
            MsgData_cInterruptCast data = new MsgData_cInterruptCast();
            data.SkillID = nSkillID;
            LogMgr.UnityLog(string.Format("SendReqBreakSkill SkillID:{0}", data.SkillID));

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_BREAKSKILL, data);
        }



        //其他玩家技能释放开始
        public void OnCastBegin(MsgData msg)
        {
            MsgData_sCastBegan data = msg as MsgData_sCastBegan;
            if (null != CoreEntry.gActorMgr.MainPlayer)
            {
                if (data.CasterID == CoreEntry.gActorMgr.MainPlayer.ServerID)
                {
                    LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(data.SkillID);
                    int showtype = skillDesc.Get<int>("showtype");
                    if (showtype == (int)SkillShowType.ST_EQUIP_SKILL)
                    {
                        int actionID = 0;
                        int.TryParse(skillDesc.Get<string>("skill_action"), out actionID);
                        LuaTable skill_action = ConfigManager.Instance.Skill.GetSkillActionConfig(actionID);
                        if (skill_action != null)
                        {
                            string skilleft = skill_action.Get<string>("skilleffect");
                            if (skilleft.Length > 0)
                            {
                                GameObject objEfx = CoreEntry.gGameObjPoolMgr.InstantiateSkillBase(skilleft);
                                if(objEfx != null)
                                {
                                    EfxAttachActionPool efxObj = objEfx.GetComponent<EfxAttachActionPool>();
                                    if (efxObj == null)
                                    {
                                        efxObj = objEfx.AddComponent<EfxAttachActionPool>();
                                    }
                                    Transform tran = CoreEntry.gActorMgr.GetPlayerActorByServerID(data.CasterID).transform;
                                    efxObj.Init(tran, skill_action.Get<int>("skillEfxLength"));
                                    efxObj.DetachObject();
                                }
                                else
                                {
                                    Debug.LogError(data.SkillID+"  装备技能  特效为空 " + skilleft);
                                }
                                //GameObject objEfx = GameObject.Instantiate(CoreEntry.gResLoader.Load(skilleft)) as GameObject;
                            }
                        }
                    }
                    return;
                }
                    
            }
            //LogMgr.UnityLog(string.Format("OnCastBegin nSkillID:{0} CasterID:{1} nTargetID:{2} PosX:{3} PosY:{4}", data.SkillID, data.CasterID, data.TargetID, data.PosX, data.PosY));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_OTHER_CASTSKILL_BEGIN, EventParameter.Get(msg));
        }


        //其他玩家技能释放开始
        public void OnCastEnd(MsgData msg)
        {
            MsgData_sCastEnd data = msg as MsgData_sCastEnd;

            if (data.CasterID == CoreEntry.gActorMgr.MainPlayer.ServerID)
                return;

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_OTHER_CASTSKILL_END, EventParameter.Get(msg));
        }

        ////法术冷却信息
        public void  OnSkillMagicCooldown(MsgData msg)
        {
            MsgData_sCooldown data = msg as MsgData_sCooldown;

            if (data.CasterID != CoreEntry.gActorMgr.MainPlayer.ServerID)
                return;

           // LogMgr.UnityLog(string.Format("OnSkillMagicCooldown  CasterID:{0} SkillID:{1} CD:{2} GroupCD:{3} GroupID:{4}", data.CasterID, data.SkillID, data.CD, data.GroupCD, data.GroupID));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SKILL_MAGICCOLLDOWN, EventParameter.Get(msg));
        }
        public void  OnCastPrepBegan(MsgData msg)
        {
            MsgData_sCastPrepBegan data = msg as MsgData_sCastPrepBegan;

            if (data.CasterID == CoreEntry.gActorMgr.MainPlayer.ServerID)
                return;

            //LogMgr.UnityLog(string.Format("OnCastPrepBegan CasterID:{0} SkillID:{1} PrepTime:{2} TargetID:{3} PosX:{4} PosY:{5}", data.CasterID, data.SkillID, data.PrepTime, data.TargetID, data.PosX, data.PosY));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SKILL_CASTPREPBEGAN, EventParameter.Get(msg));
        }
        public void OnCastPrepEnded(MsgData msg)
        {
            MsgData_sCastPrepEnd data = msg as MsgData_sCastPrepEnd;

            if (data.CasterID == CoreEntry.gActorMgr.MainPlayer.ServerID)
                return;

            //LogMgr.UnityLog(string.Format("OnCastPrepEnded CasterID:{0} SkillID:{1} IsEnd:{2}}", data.CasterID, data.SkillID, data.IsEnd));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SKILL_CASTPREPENDED, EventParameter.Get(msg));
        }
        public void OnCastChangeBegan(MsgData msg)
        {
            MsgData_sCastChanBegan data = msg as MsgData_sCastChanBegan;

            if (data.CasterID == CoreEntry.gActorMgr.MainPlayer.ServerID)
                return;

            //LogMgr.UnityLog(string.Format("OnCastChangeBegan CasterID:{0} SkillID:{1} TargetID:{2} PosX:{3} PosY:{4}", data.CasterID, data.SkillID, data.TargetID, data.PosX, data.PosY));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SKILL_CASTCHANGEBEGAN, EventParameter.Get(msg));
        }
        public void OnCastChangeEnd(MsgData msg)
        {
            MsgData_sCastChanEnd data = msg as MsgData_sCastChanEnd;

            if (data.CasterID == CoreEntry.gActorMgr.MainPlayer.ServerID)
                return;

            //LogMgr.UnityLog(string.Format("OnCastChangeEnd CasterID:{0} SkillID:{1} ", data.CasterID, data.SkillID));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SKILL_CASTCHANGEEND, EventParameter.Get(msg));
        }
        public void OnInterpuptCast(MsgData msg)
        {
            //MsgData_sInterruptCast data = msg as MsgData_sInterruptCast;
            //LogMgr.UnityLog(string.Format("OnInterpuptCast CasterID:{0} SkillID:{1} ", data.CasterID, data.SkillID));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SKILL_INTERPUPTCAST, EventParameter.Get(msg));
        }

        //技能伤害
        public void OnSkillEffect(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_SKILLEFFECT, EventParameter.Get(msg));
        }

        //击退
        public void OnHitBack(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_KNOCKBACK, EventParameter.Get(msg));
        }

        //目标列表
        public void OnGetSkillTarget(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SKILL_TARGETLIST, EventParameter.Get(msg));
        }

        public void OnCastMoveEffect(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_CAST_MOVE_EFFECRT, EventParameter.Get(msg));
        }

        public void OnAddBuff(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ADDBUFF, EventParameter.Get(msg));
        }
        public void OnAddBuffList(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ADDBUFFList, EventParameter.Get(msg));
        }

        public void OnUpdateBuff(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_UPDATEBUFF, EventParameter.Get(msg));
        }

        public void OnDelBuff(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_DELBUFF, EventParameter.Get(msg));
        }



        public void OnMainPlayerEnterScene(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_MAINPLAYER_ENTER_SCENE, EventParameter.Get(data));
        }

        //////////////////////////////////
        //状态位改变
        /////////////////////////////////
        public void OnStateChanged(MsgData msg)
        {
            MsgData_sStateChanged data = msg as MsgData_sStateChanged;
            if (data.RoleID == PlayerData.Instance.RoleID)
            {
                //玩家属性更新
                PlayerData.Instance.BaseAttr.SetUnitBit(data.State, data.IsSet != 0);
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_STATE_CHANGED, EventParameter.Get(data));
        }

        /// <summary>
        /// 玩家外观改变。
        /// </summary>
        /// <param name="data"></param>
        public void OnPlayerShowChanged(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_PLAYER_SHOW_CHANGED, EventParameter.Get(data));
        }

        /// <summary>
        /// 复活请求
        /// </summary>
        /// <param name="reviveType">复活类型,1原地，2复活点</param>
        /// <param name="moneyType">物品不足时使用的元宝类型,1元宝,2绑定元宝</param>
        public void SendReqRevive(int reviveType, int moneyType)
        {
            MsgData_cRevive data = new MsgData_cRevive();
            data.ReviveType = reviveType;
            data.MoneyType = moneyType;

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_REVIVE, data);
        }

        public void OnRevive(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_REVIVE, EventParameter.Get(data));
        }

        /////////////////////////////////// 
        //拾取
        /////////////////////////////////// 

        /// <summary>
        /// 请求拾取物品
        /// </summary>
        /// <param name="itemServerID">物品ServerID</param>
        public void SendReqPickUpItem(List<long> ids)
        {
            MsgData_cPickUpItem data = new MsgData_cPickUpItem();
            data.DataSize = (uint)ids.Count;
            data.IDs = ids;

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_PICKUPITEM, data);
        }

        public void OnPickUpItem(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ITEM_PICKUP, EventParameter.Get(data));
        }

        /////////////////////////////////// 
        //境界
        /////////////////////////////////// 

        public void SendReqDianfengInfo()
        {
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DianfengInfo, new MsgData_cDianfengInfo());
        }

        public void OnDianfengInfo(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_JINGJIE_INFO, EventParameter.Get(data));
        }

        public void SendReqDianfengSave(List<DianfengInfo_Attr> list)
        {
            MsgData_cDianfengSave data = new MsgData_cDianfengSave();
            data.ListSize = (uint)list.Count;
            data.Data = list;

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DianfengSave, data);
        }

        public void OnDianfengSave(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_JINGJIE_SAVE, EventParameter.Get(data));
        }

        public void SendReqDianfengReset(int type)
        {
            MsgData_cDianfengReset data = new MsgData_cDianfengReset();
            data.Type = type;

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DianfengReset, data);
        }

        public void OnDianfengReset(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_JINGJIE_RESET, EventParameter.Get(data));
        }

        //法宝分解返回
        public void OnReqMagicKeyDecompose(MsgData msg)
        {
            MsgData_sResMagicKeyDecompose data = msg as MsgData_sResMagicKeyDecompose;
            LogMgr.UnityLog(string.Format("OnReqMagicKeyDecompose result : {0}", data.result));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_REQMAGICKEYDECOMPOSE, EventParameter.Get(msg));
        }
        
        //更新法宝信息 
        public void OnUpdateMagicKeyInfo(MsgData msg)
        {
            MsgData_sUpdateMagicKeyInfo data = msg as MsgData_sUpdateMagicKeyInfo;
            for (int i = 0; i < data.count; i++)
            { 
                var item=  data.items[i];

                LogMgr.UnityLog(string.Format(" OnUpdateMagicKeyInfo guid:{0} :magickeyID:{1} wuxing:{2} level:{3} totalExp:{4} starCount:{5} passiveskill1:{6}", item.guid, item.magickeyID, item.wuxing, item.level, item.totalExp, item.starCount, item.passiveskill1));
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_UPDATEMAGICKEYINFO, EventParameter.Get(msg));
        }
        //返回法宝培养结果 
        public void OnTrainMagicKey(MsgData msg)
        {
            MsgData_sTrainMagicKey data = msg as MsgData_sTrainMagicKey;
            LogMgr.UnityLog(string.Format("OnTrainMagicKey result : {0} ", data.result));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_TRAINMAGICKEY, EventParameter.Get(msg));
        }
        //返回法宝祝福值信息 msgId:8818
        public void OnMagicKeyWashInfo(MsgData msg)
        {
            MsgData_sMagicKeyWashInfo data = msg as MsgData_sMagicKeyWashInfo;
            for (int i = 0; i < data.items.Count; i++)
            {
                LogMgr.UnityLog(string.Format("OnMagicKeyWashInfo MakeOrder: {0} WashValue:{1} MaxWashValue: {2}", data.items[i].MagicKeyMakeOrder, data.items[i].MagicKeyWashValue, data.items[i].MagicKeyMaxWashValue));
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_MagicKeyWashInfo, EventParameter.Get(msg));
        }
        //返回法宝打造结果 
        public void OnMakeMagicKey(MsgData msg)
        {
            MsgData_sMakeMagicKey data = msg as MsgData_sMakeMagicKey;
            LogMgr.UnityLog(string.Format("OnMakeMagicKey result : {0} count:{1} ", data.result,data.count));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_MAKEMAGICKEY, EventParameter.Get(msg));
        }

         //返回法宝仙灵列表
        public void OnReturnMagicKeyGodInfos(MsgData msg)
        {
            MsgData_sReturnMagicKeyGodInfos data = msg as MsgData_sReturnMagicKeyGodInfos;

            for (int i = 0; i < data.count; i++)
            {
                var item = data.items[i];
                LogMgr.UnityLog(string.Format("OnReturnMagicKeyGodInfos guid:{0} wuxing:{1} id:{2} magicGuid:{3}", item.guid, item.wuxing, item.id, item.magicGuid));

            }

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_RETURNMAGICKEYGODINFOS, EventParameter.Get(msg));
        }
        //返回仙灵穿戴结果 
        public void OnResMaigcKeyGodInset(MsgData msg)
        {
            MsgData_sResMaigcKeyGodInset data = msg as MsgData_sResMaigcKeyGodInset;
            LogMgr.UnityLog(string.Format("OnResMaigcKeyGodInset guid : {0} magicGuid:{1} ", data.guid, data.magicGuid));

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_RESMAIGCKEYGODINSET, EventParameter.Get(msg));
        }
        // 返回法宝技能 
        public void OnMagicKeyInsetSkill(MsgData msg)
        {
            MsgData_sMagicKeyInsetSkill data = msg as MsgData_sMagicKeyInsetSkill;
            LogMgr.UnityLog(string.Format("OnMagicKeyInsetSkill guid : {0} count:{1} ", data.guid, data.count));            
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_MAGICKEYINSETSKILL, EventParameter.Get(msg));
        }
        //返回:法宝飞升升经验
        public void OnMagicKeyFeiSheng(MsgData msg)
        {
            //MsgData_sMagicKeyFeiSheng data = msg as MsgData_sMagicKeyFeiSheng;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_MAGICKEYFEISHENG, EventParameter.Get(msg));
        }
        // 返回法宝被动技能领悟
        public void OnReturnMagickeySKillLingwu(MsgData msg)
        {
            MsgData_sReturnMagickeySKillLingwu data = msg as MsgData_sReturnMagickeySKillLingwu;
            LogMgr.UnityLog(string.Format("OnReturnMagickeySKillLingwu result : {0} chengeIndex:{1} ", data.result, data.chengeIndex));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_RETURNMAGICKEYSKILLLINGWU, EventParameter.Get(msg));
        }
        //法宝升星 
        public void OnMagicKeyStarLevelUp(MsgData msg)
        {
            MsgData_sMagicKeyStarLevelUp data = msg as MsgData_sMagicKeyStarLevelUp;
            LogMgr.UnityLog(string.Format("OnMagicKeyStarLevelUp result : {0} progressNum:{1} ", data.result, data.progressNum));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_MAGICKEYSTARLEVELUP, EventParameter.Get(msg));
        }

        //法宝分解
        public void SendReqMagicKeyDecompose(LuaTable t)
        {
            List<ItemInfo> itemUidList = t.Cast<List<ItemInfo>>();
            List<MsgData_cMagicKey> list = new List<MsgData_cMagicKey>();
            for (int i = 0; i < itemUidList.Count; i++)
            {
                MsgData_cMagicKey item = new MsgData_cMagicKey();
                item.id = (long)itemUidList[i].UID;
                list.Add(item);
            }
            MsgData_cResMagicKeyDecompose data = new MsgData_cResMagicKeyDecompose();
            data.items = list;
            data.count = (uint)list.Count;

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_REQMAGICKEYDECOMPOSE, data);
        }
        //法宝升星 
        public void SendMagkicKeyStarLevelUp(long guid)
        {
            MsgData_cMagicKeyStarLevelUp data = new MsgData_cMagicKeyStarLevelUp();
            LogMgr.UnityLog(string.Format("SendMagkicKeyStarLevelUp : {0}", guid));
            data.guid = guid;

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_MAGICKEYSTARLEVELUP, data);
        }

        //请求法宝合成
        public void SendMakeMagicKey(int makeorder,int makecount,int bindFlag,int isvip)
        {
            MsgData_cMakeMagicKey data = new MsgData_cMakeMagicKey();
            LogMgr.UnityLog(string.Format("SendMakeMagicKey : makeorder {0} makecount {1}  bindFlag {2} isvip {3}", makeorder, makecount, bindFlag, isvip));
            data.MakeOrder = makeorder; // 法宝打造品阶
            data.MakeCount = makecount; // 法宝打造数量
            //绑定 1 非绑定 2
            data.bindFlag = bindFlag; // 材料绑定标志 1 只使用绑定 2 只使用非绑定 3 两者都用，优先绑定
            data.IsVIP = isvip; // 1VIP打造, 0普通打造 2 领取法宝
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_MAKEMAGICKEY, data);
        }
        // 请求法宝培养 msgId:3626;
        public void SendTrainMagicKey(long guid,long TrainDanguid)
        {
            MsgData_cTrainMagicKey data = new MsgData_cTrainMagicKey();
            data.guid = guid;
            data.count = 1;
            data.items = new List<MsgData_cTrainMagicKeyItem>();
            MsgData_cTrainMagicKeyItem item = new MsgData_cTrainMagicKeyItem();
            item.TrainDanguid = TrainDanguid;
            item.TrainDanNum = 1;
            data.items.Add(item);
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_TRAINMAGICKEY, data);
        }
        //public const Int16 C_REQUESTMAGICKEYSKILLLINGWU = 3879; // 请求法宝被动技能领悟 msgId:3879;
        public void SendRequestMagicKeySkillLingWu(long magickId, long itemID,LuaTable t)
        {
            MsgData_cRequestMagickeySkillLingwu data = new MsgData_cRequestMagickeySkillLingwu();
            List<int> ListId = t.Cast<List<int>>();
            LogMgr.UnityLog(string.Format("SendRequestMagicKeySkillLingWu magickId: {0} itemID:{1}  Count:{2}", magickId, itemID, ListId.Count));

            data.magickId = magickId;
            data.itemID = itemID;
            data.count = (uint)ListId.Count;  
            data.items = new List<MsgData_cSuoList>(); // 1VIP打造, 0普通打造 2 领取法宝
            for (int i = 0; i < ListId.Count; i++)
            {
                MsgData_cSuoList suo = new MsgData_cSuoList();
                suo.indexNum = ListId[i];
                data.items.Add(suo);
            }

           CoreEntry.netMgr.send((Int16)NetMsgDef.C_REQUESTMAGICKEYSKILLLINGWU, data);
        }
        // public const Int16 C_REQMAIGCKEYGODINSET = 3708;// 客户端请求：法宝仙灵穿戴 msgId:3708;
        public void SendReqMagicKeyGodInset(long guid, long magicGuid,int type)
        {
            MsgData_cReqMaigcKeyGodInset data = new MsgData_cReqMaigcKeyGodInset();
            data.guid = guid; // 实例ID
            data.magicGuid = magicGuid; // 法宝实例ID
            data.type = type; //  0、穿上  1、卸下
            LogMgr.UnityLog(string.Format("SendReqMagicKeyGodInset guid: {0} magicGuid:{1}  type:{2}", guid, magicGuid, type));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_REQMAIGCKEYGODINSET, data);
        }

        /////////////////////////////////// 
        //宝石
        ///////////////////////////////////
        public void SendReqStoneInfo()
        {
            MsgData_cReqGemOpenInfo data = new MsgData_cReqGemOpenInfo();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GEMSLOTINFO, data);
        }

        public void SendReqStoneSlotOpen(int pos)
        {
            MsgData_cReqGemSlotOpen data = new MsgData_cReqGemSlotOpen();
            data.Pos = pos;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GEMSLOTOPEN, data);
        }

        public void SendReqStoneInset(int pos, long guid, int type)
        {
            MsgData_cReqEquipGemInset data = new MsgData_cReqEquipGemInset();
            data.Pos = pos;
            data.Guid = guid;
            data.Type = type;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GEMINSET, data);
        }

        public void SendReqStoneLvUp(int id)
        {
            MsgData_cEquipGemUpLevel data = new MsgData_cEquipGemUpLevel();
            data.ID = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GEMLEVELUP, data);
        }

        /////////////////////////////////// 
        //合成分解
        ///////////////////////////////////
        public void SendReqItemCompose(int id, int type, int count)
        {
            MsgData_cItemCompose data = new MsgData_cItemCompose();
            data.ID = id;
            data.Type = type;
            data.Count = count;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ITEMCOMPOSE, data);
        }

        public void OnItemCompose(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ITEM_COMPOSE_RESULT, EventParameter.Get(msg));
        }

        public void OnHONGYANACT(MsgData msg)
        {
            MsgData_sHongyanAct data = msg as MsgData_sHongyanAct;
             
            LogMgr.UnityLog(string.Format("OnHONGYANACT : id:{0} Result:{1}  State:{2} Jiedian:{3} JiedianLevel:{4}", data.ID,data.Result,data.State,data.Jiedian,data.JiedianLevel));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_HONGYANACT, EventParameter.Get(msg));
        }
        public void OnHONGYANFIGHT(MsgData msg)
        {
            MsgData_sHongyanFight data = msg as MsgData_sHongyanFight;
            LogMgr.UnityLog(string.Format("OnHONGYANFIGHT : id:{0} Result:{1}  State:{2}", data.ID, data.Result, data.State));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_HONGYANFIGHT, EventParameter.Get(msg));
        }
       public void OnBeautyWomanLevelUp(MsgData msg)
        {
            MsgData_sBeautyWomanLevelUp data = msg as MsgData_sBeautyWomanLevelUp;
            LogMgr.UnityLog(string.Format("OnBeautyWomanLevelUp : id:{0} Result:{1}  State:{2} starNum:{3} currentExp:{4} :gradeNum{5} radExp:{6}", data.ID, data.result, data.State, data.starNum, data.currentExp, data.gradeNum, data.radExp));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BeautyWomanLevelUp, EventParameter.Get(msg));
        }
       public void OnBeautyWomanInfo(MsgData msg)
        {
            MsgData_sBeautyWomanInfo data = msg as MsgData_sBeautyWomanInfo;
            LogMgr.UnityLog(string.Format("OnBeautyWomanInfo : outoid:{0}", data.ID));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BeautyWomanInfo, EventParameter.Get(msg));
        }
 
       public void OnBeautyWomanUseAtt(MsgData msg)
        {
            MsgData_sBeautyWomanUseAtt data = msg as MsgData_sBeautyWomanUseAtt;
            LogMgr.UnityLog(string.Format("OnBeautyWomanUseAtt : id:{0} result;{1} count:{2}", data.ID,data.result,data.count));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BeautyWomanUseAtt, EventParameter.Get(msg));
        }
         
       public void OnBeautyWomanFightingUpdate(MsgData msg)
       {
           MsgData_sBeautyWomanFightingUpdate data = msg as MsgData_sBeautyWomanFightingUpdate;
           LogMgr.UnityLog(string.Format("OnBeautyWomanFightingUpdate : id:{0} fighting;{1}", data.ID, data.fighting));
           CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BeautyWomanFightingUpdate, EventParameter.Get(msg));
       }
        /////////////////////////////////// 
        //红颜
        ///////////////////////////////////
        public void SendReqHongyanAct(int id, int type)
        {
            MsgData_cHongyanAct data = new MsgData_cHongyanAct();
            data.ID = id;
            data.Type = type;
            LogMgr.UnityLog(string.Format("SendReqHongyanAct : id:{0} type:{1}", id,type));

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_HONGYANACT, data);
        }

        public void SendReqHongyanFight(int id, int state)
        {
            MsgData_cHongyanFight data = new MsgData_cHongyanFight();
            data.ID = id;
            data.State = state;
            LogMgr.UnityLog(string.Format("SendReqHongyanFight : id:{0} state:{1}", id, state));

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_HONGYANFIGHT, data);
        }
        //请求红颜使用属性丹
        public void SendReqBeautyWomanUseAtt(int id)
        {
            MsgData_cBeautyWomanUseAtt data = new MsgData_cBeautyWomanUseAtt();
            data.ID = id; 
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_BeautyWomanUseAtt, data);
        }


        //等级副本
        public void OnDominateRouteData(MsgData msg)
        {
            MsgData_sDominateRouteData data = msg as MsgData_sDominateRouteData;
            LogMgr.UnityLog(string.Format("OnDominateRouteData : enterNum:{0} count:{1}", data.enterNum, data.count));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_DominateRouteData, EventParameter.Get(msg));
        }

        public void OnDominateRouteUpDate(MsgData msg)
        {
            MsgData_sDominateRouteUpDate data = msg as MsgData_sDominateRouteUpDate;
            LogMgr.UnityLog(string.Format("OnDominateRouteUpDate : num:{0} state:{1} time:{2}  id:{3} rewardType:{4}", data.num, data.state, data.time, data.id, data.rewardType));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_DominateRouteUpDate, EventParameter.Get(msg));
        }

        public void OnBackDominateRouteChallenge(MsgData msg)
        {
            MsgData_sBackDominateRouteChallenge data = msg as MsgData_sBackDominateRouteChallenge;
            LogMgr.UnityLog(string.Format("OnBackDominateRouteChallenge : result:{0} id:{1}", data.result, data.id));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_BackDominateRouteChallenge, EventParameter.Get(msg));
        }
        public void OnBackDominateRouteQuit(MsgData msg)
        {
            MsgData_sBackDominateRouteQuit data = msg as MsgData_sBackDominateRouteQuit;
            LogMgr.UnityLog(string.Format("OnBackDominateRouteQuit : result:{0}", data.result));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_BackDominateRouteQuit, EventParameter.Get(msg));
        }
        public void OnBackDominateRouteInfo(MsgData msg)
        {
            MsgData_sBackDominateRouteInfo data = msg as MsgData_sBackDominateRouteInfo;
            LogMgr.UnityLog(string.Format("OnBackDominateRouteInfo : num:{0}", data.num));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_BackDominateRouteInfo, EventParameter.Get(msg));
        }
        public void OnBackDominateRouteWipe(MsgData msg)
        {
            MsgData_sBackDominateRouteWipe data = msg as MsgData_sBackDominateRouteWipe;
            LogMgr.UnityLog(string.Format("OnBackDominateRouteWipe : result:{0} id:{1} num:{2}", data.result, data.id, data.num));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_BackDominateRouteWipe, EventParameter.Get(msg));
        }
        public void OnBackDominateRouteVigor(MsgData msg)
        {
            MsgData_sBackDominateRouteVigor data = msg as MsgData_sBackDominateRouteVigor;
            LogMgr.UnityLog(string.Format("OnBackDominateRouteVigor : result:{0} ", data.result));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_BackDominateRouteVigor, EventParameter.Get(msg));
        }
        public void OnBackDominateRouteBoxReward(MsgData msg)
        {
            MsgData_sBackDominateRouteBoxReward data = msg as MsgData_sBackDominateRouteBoxReward;
            LogMgr.UnityLog(string.Format("OnBackDominateRouteBoxReward : result:{0} id:{1}", data.result, data.id));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_BackDominateRouteBoxReward, EventParameter.Get(msg));
        }
        public void OnBackDominateRouteEnd(MsgData msg)
        {
            MsgData_sBackDominateRouteEnd data = msg as MsgData_sBackDominateRouteEnd;
            LogMgr.UnityLog(string.Format("OnBackDominateRouteEnd : result:{0} level:{1}", data.result, data.level));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_BackDominateRouteEnd, EventParameter.Get(msg));
        }

        public void OnBackDominateRouteMopupEnd(MsgData msg)
        {
            MsgData_sBackDominateRouteMopupEnd data = msg as MsgData_sBackDominateRouteMopupEnd;
            LogMgr.UnityLog(string.Format("OnBackDominateRouteMopupEnd : result:{0} id:{1} num:{2}", data.result, data.id,data.num));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_BackDominateRouteMopupEnd, EventParameter.Get(msg));
        }
        //请求UI信息 msgId:3390;  未使用
        public void SendReqDominateRoute()
        {
            LogMgr.UnityLog("SendReqDominateRoute");
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DominateRoute);
        }
        // 请求挑战 msgId:3392;
        public void SendReqDominateRouteChallenge(int id)
        {
            LogMgr.UnityLog(string.Format("SendReqDominateRouteChallenge : id:{0}", id));
            MsgData_cDominateRouteChallenge data = new MsgData_cDominateRouteChallenge();
            data.id = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DominateRouteChallenge, data);
        }
        // 请求退出 msgId:3393;
        public void SendReqDominateRouteQuit()
        {
            LogMgr.UnityLog("SendReqDominateRouteQuit");
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DominateRouteQuit);
        }

        // 领取宝箱 msgId:3394;
        public void SendReqDominateRouteRequestReward(int id)
        {
            LogMgr.UnityLog(string.Format("SendReqDominateRouteRequestReward : id:{0}", id));
            MsgData_cDominateRouteRequestReward data = new MsgData_cDominateRouteRequestReward();
            data.id = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DominateRouteRequestReward, data);
        }

        public void SendReqDominateRouteWipe(int id, int num)
        {
            LogMgr.UnityLog(string.Format("SendReqDominateRouteWipe : id:{0} num:{1}", id, num));
            MsgData_cDominateRouteWipe data = new MsgData_cDominateRouteWipe();
            data.id = id;
            data.num = num;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DominateRouteWipe, data);
        }

#region boss挑战副本
        public void SendReqEnterPersonalBoss(int id) // 客户端请求进入个人BOSS副本 msgId:3561;
        {
            LogMgr.UnityLog(string.Format("SendReqEnterPersonalBoss : id:{0}", id));
            MsgData_cEnterPersonalBoss data = new MsgData_cEnterPersonalBoss();
            data.id = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_EnterPersonalBoss, data);
        }
        public void SendReqQuitPersonalBoss()// 客户端请求退出个人BOSS副本 msgId:3562;
        {
            LogMgr.UnityLog(string.Format("SendReqQuitPersonalBoss")); 
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_QuitPersonalBoss);
        }

        public void SendReqPersonBossOneKeySweep() // 请求个人BOSS一键扫荡 msgId:4021;
        {
            LogMgr.UnityLog(string.Format("SendReqPersonBossOneKeySweep"));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_PersonBossOneKeySweep);
        }
        public void SendReqPersonalBossLoading()// 客户端请求结束个人副本loading msgId:3568;
        {
            LogMgr.UnityLog(string.Format("SendReqPersonalBossLoading"));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_QuitPersonalBoss);
        }

        public void OnPersonalBossList(MsgData msg)// 服务器返回BOSS挑战列表 msgId:8560;
        {
            MsgData_sPersonalBossList data = msg as MsgData_sPersonalBossList;
            LogMgr.UnityLog(string.Format("OnPersonalBossList : count:{0}", data.count));

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PersonalBossList, EventParameter.Get(msg));
        }
        public void  OnBackEnterResultPersonalBoss(MsgData msg)// 服务器返回进入个人BOSS结果 msgId:8561;
        {
            MsgData_sBackEnterResultPersonalBoss data = msg as MsgData_sBackEnterResultPersonalBoss;
            LogMgr.UnityLog(string.Format("OnBackEnterResultPersonalBoss : result:{0} id:{1} type:{2} num:{3} itemEnterNum:{4}", data.result, data.id, data.type, data.num, data.itemEnterNum));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BackEnterResultPersonalBoss, EventParameter.Get(msg));
        }
        public void  OnBackQuitPersonalBoss(MsgData msg)// 服务器:退出个人BOSS结果 msgId:8562;
        {
            MsgData_sBackQuitPersonalBoss data = msg as MsgData_sBackQuitPersonalBoss;
            LogMgr.UnityLog(string.Format("OnBackQuitPersonalBoss : result:{0}", data.result));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BackQuitPersonalBoss, EventParameter.Get(msg));
        }
        public void OnPersonalBossResult(MsgData msg)// 服务器:挑战个人BOSS结果 msgId:8563;
        {
            MsgData_sPersonalBossResult data = msg as MsgData_sPersonalBossResult;
            LogMgr.UnityLog(string.Format("OnPersonalBossResult : result:{0}  isfirst:{1}", data.result, data.isfirst));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PersonalBossResult, EventParameter.Get(msg));
        }
         
#endregion

        public void OnTongJiInfo(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_TongJiInfo, EventParameter.Get(msg));
        }
        public void OnTongJiLvlRefreshResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_TongJiLvlRefreshResult, EventParameter.Get(msg));
        }
        public void OnAcceptTongJiResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_AcceptTongJiResult, EventParameter.Get(msg));
        }
        public void OnFinishTongJi(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_FinishTongJi, EventParameter.Get(msg));
        }
        public void OnGetTongJiReward(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_GetTongJiReward, EventParameter.Get(msg));
        }
        public void OnGiveupTongJiResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_GiveupTongJiResult, EventParameter.Get(msg));
        }
        public void OnRefreshTongJiList(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_RefreshTongJiList, EventParameter.Get(msg));
        }
        public void OnGetTongJiBoxResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_GetTongJiBoxResult, EventParameter.Get(msg));
        }
        public void OnTongJiRefreshState(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_TongJiRefreshState, EventParameter.Get(msg));
        }
#region 邮件

        public void SendReqGetMailList()// 请求获取邮件列表 msgId:2032;
        {
            LogMgr.UnityLog(string.Format("SendReqGetMailList"));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GetMailList);
        } 
        public void SendReqOpenMail(long id)// 请求打开邮件 msgId:2033;
        {
            MsgData_cOpenMail data = new MsgData_cOpenMail();
            data.id = id;
            LogMgr.UnityLog(string.Format("SendReqOpenMail id:{0}",id));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_OpenMail, data);
        }
        public void SendReqGetMailItem(List<long> list)// 请求领取附件 msgId:2034;
        {
            MsgData_cGetMailItem data = new MsgData_cGetMailItem();
            data.items = new List<MsgData_cMailReqItemVo>();
            data.count = (uint)list.Count;
            for (int i = 0; i < list.Count; i++ )
            {
                MsgData_cMailReqItemVo item = new MsgData_cMailReqItemVo();
                item.id = list[i];
                data.items.Add(item);
            }
            LogMgr.UnityLog(string.Format("SendReqGetMailItem count : {0}",list.Count));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GetMailItem, data);
        }
        public void SendReqDelMail(List<long> list)// 请求删除邮件 msgId:2035;
        {
            MsgData_cDelMail data = new MsgData_cDelMail();
            data.items = new List<MsgData_cReqMailDelVo>();
            data.count = (uint)list.Count;
            for (int i = 0; i < list.Count; i++)
            {
                MsgData_cReqMailDelVo item = new MsgData_cReqMailDelVo();
                item.id = list[i];
                data.items.Add(item);
            }
            LogMgr.UnityLog(string.Format("SendReqDelMail"));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DelMail, data);
        }


        public void OnGetMailResult(MsgData msg)// 返回邮件列表 msgId:7032;
        {
            MsgData_sGetMailResult data = msg as MsgData_sGetMailResult;
            LogMgr.UnityLog(string.Format("OnGetMailResult : count:{0} ", data.count));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GetMailResult, EventParameter.Get(msg));
        }
        public void OnOpenMailResult(MsgData msg)// 返回打开邮件 msgId:7033;
        {
            MsgData_sOpenMailResult data = msg as MsgData_sOpenMailResult;
            LogMgr.UnityLog(string.Format("OnOpenMailResult : id:{0}  item:{1}   contnet:{1}", data.id, data.item, data.contnet));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OpenMailResult, EventParameter.Get(msg));
        }
        public void OnGetMailItemResult(MsgData msg)// 请求领取附件返回 msgId:7034;
        {
            MsgData_sGetMailItemResult data = msg as MsgData_sGetMailItemResult;
            LogMgr.UnityLog(string.Format("OnGetMailItemResult : count:{0} ", data.count));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_GetMailItemResult, EventParameter.Get(msg));

        }
        public void OnDelMail(MsgData msg)// 请求删除邮件返回 msgId:7035;
        {
            MsgData_sDelMail data = msg as MsgData_sDelMail;
            LogMgr.UnityLog(string.Format("OnDelMail : count:{0} ", data.count));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_DelMail, EventParameter.Get(msg));

        }
        public void OnNotifyMail(MsgData msg)// 邮件提醒 msgId:7036;
        {
            MsgData_sNotifyMail data = msg as MsgData_sNotifyMail;
            LogMgr.UnityLog(string.Format("OnNotifyMail : count:{0} ", data.count));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_NotifyMail, EventParameter.Get(msg));

        }

        public void OnGuanZhiInfo(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_GuanZhiInfo, EventParameter.Get(msg));
        }
        public void OnGuanZhiLevelUp(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_GuanZhiLevelUp, EventParameter.Get(msg));
        }
        public void OnInterServiceContValue(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_InterServiceContValue, EventParameter.Get(msg));
        }
        public void OnHuoYueDu(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_HuoYueDu, EventParameter.Get(msg));
        }
        public void OnHuoYueDuFinish(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_HuoYueDuFinish, EventParameter.Get(msg));
        }
        public void OnHuoYueReward(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_HuoYueReward, EventParameter.Get(msg));
        }


        #endregion
 
#region 经验副本
        public void OnWaterDungeonEnterResult(MsgData msg)     // 服务器返回:进入流水副本结果 msgId:8439;
        {
            MsgData_sWaterDungeonEnterResult data = msg as MsgData_sWaterDungeonEnterResult;
            LogMgr.UnityLog(string.Format("WaterDungeonEnterResult : result:{0}  rightTime:{1}", data.result, data.rightTime));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_WaterDungeonEnterResult, EventParameter.Get(msg));
        }
        public void OnWaterDungeonExitResult(MsgData msg)     // 服务器返回:退出流水副本结果 msgId:8440;
        {
            MsgData_sWaterDungeonExitResult data = msg as MsgData_sWaterDungeonExitResult;
            LogMgr.UnityLog(string.Format("WaterDungeonExitResult : result:{0} ", data.result));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_WaterDungeonExitResult, EventParameter.Get(msg));
        }
        public void OnWaterDungeonInfo(MsgData msg)     // 服务器返回:流水副本信息 msgId:8434;
        {
            MsgData_sWaterDungeonInfo data = msg as MsgData_sWaterDungeonInfo;
            LogMgr.UnityLog(string.Format("OnWaterDungeonInfo : wave:{0} exp:{1} time:{2} monster:{3} moreExp:{4} moreReward:{5}", data.wave, data.exp, data.time, data.monster, data.moreExp, data.moreReward));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_WaterDungeonInfo, EventParameter.Get(msg));

        }
        public void OnWaterDungeonProgress(MsgData msg)     // 服务器返回:流水副本进度 msgId:8436;
        {
            MsgData_sWaterDungeonProgress data = msg as MsgData_sWaterDungeonProgress;
            LogMgr.UnityLog(string.Format("OnWaterDungeonProgress : wave:{0} monster:{1} exp: {2}", data.wave, data.monster, data.exp));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_WaterDungeonProgress, EventParameter.Get(msg));
        }
        public void OnWaterDungeonResult(MsgData msg)     // 服务器返回:流水副本结算 msgId:8437;
        {
            MsgData_sWaterDungeonResult data = msg as MsgData_sWaterDungeonResult;
            LogMgr.UnityLog(string.Format("OnWaterDungeonResult : wave:{0} result:{1} exp: {2}", data.wave, data.result, data.exp));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_WaterDungeonResult, EventParameter.Get(msg));
        } 
        public void SendReqWaterDungeonEnter()     // 客户端请求：进入流水副本 msgId:3439;
        {
            LogMgr.UnityLog(string.Format("SendReqWaterDungeonEnter"));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_WaterDungeonEnter);
        }
        public void SendReqWaterDungeonExit()     // 客户端请求：退出流水副本 msgId:3440;
        {
            LogMgr.UnityLog(string.Format("SendReqWaterDungeonExit"));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_WaterDungeonExit);

        }
        public void SendReqWaterDungeonInfo()     // 客户端请求：流水副本信息 msgId:3434;
        {

            LogMgr.UnityLog(string.Format("SendReqWaterDungeonInfo"));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_WaterDungeonInfo);
        }
#endregion

#region 占星
          public void SendReqOpenNode(int id)    
          {
                MsgData_cOpenNode data = new MsgData_cOpenNode();
                data.id = id;
                LogMgr.UnityLog(string.Format("SendReqOpenNode id:{0}",id));
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_OpenNode, data);
           }

            public void OnNeiGongInfo(MsgData msg) // 服务器返回: 经脉信息 msgId:8665;
            {
                MsgData_sNeiGongInfo data = msg as MsgData_sNeiGongInfo;
                LogMgr.UnityLog(string.Format("OnNeiGongInfo : count:{0} ", data.count));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_NeiGongInfo, EventParameter.Get(msg));
            }
            public void OnOpenNode(MsgData msg) // 服务器返回:冲穴结果 msgId:8666;
            {
                MsgData_sOpenNode data = msg as MsgData_sOpenNode;
                LogMgr.UnityLog(string.Format("OnOpenNode : result:{0} id:{1} nodeID :{2} Level:{3}", data.result, data.id, data.NodeId, data.Level));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OpenNode, EventParameter.Get(msg));
            }
#endregion

#region 秘境
            public void SendReqSimpleSecrectDuplInfo() // 请求单人秘境副本面板信息 msgId:3627;
            {
                LogMgr.UnityLog(string.Format("SendReqReqSimpleSecrectDuplInfo"));
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_ReqSimpleSecrectDuplInfo);
            }
            public void SendReqEnterSimpleSecrectDupl(
		        int id, // 副本ID
		        int itemId, // 钥匙ID
		        int enterType) // 请求进入单人秘境副本 msgId:3628;
           {
                MsgData_cReqEnterSimpleSecrectDupl data = new MsgData_cReqEnterSimpleSecrectDupl();
                data.id = id;
                data.itemId = itemId;
                data.enterType = enterType;
                LogMgr.UnityLog(string.Format("SendReqReqEnterSimpleSecrectDupl id:{0} itemId:{1} enterType:{2}", id, itemId, enterType));
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_ReqEnterSimpleSecrectDupl, data);
            }
            public void SendReqExitSimpleSecrectDupl() // 请求退出单人秘境副本 msgId:3641;
            {
                LogMgr.UnityLog(string.Format("SendReqReqExitSimpleSecrectDupl"));
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_ReqExitSimpleSecrectDupl);
            }
            public void SendReqBuySecrectDuplTili(int type) // 请求购买组队或次数 msgId:3642;
            {
                MsgData_cReqBuySecrectDuplTili data = new MsgData_cReqBuySecrectDuplTili();
                data.type = type;
                LogMgr.UnityLog(string.Format("SendReqBuySecrectDuplTili type:{0}", type));
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_ReqBuySecrectDuplTili, data);

            }
            public void SendReqJiHuoSecrectDupl(int stepid, int id) // 请求激活 msgId:3643;
            {
                MsgData_cReqJiHuoSecrectDupl data = new MsgData_cReqJiHuoSecrectDupl();
                data.stepid = stepid;
                data.id = id;
                LogMgr.UnityLog(string.Format("SendReqJiHuoSecrectDupl stepid:{0}, id:{1}", data.stepid, data.id));
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_ReqJiHuoSecrectDupl, data);

            }
            public void SendReqSecretDungeonSweep(
                	int id, // 层数id
		            int num // 扫荡次数
                    ) // 请求:个人秘境副本扫荡 msgId:3967;
            {

                MsgData_cSecretDungeonSweep data = new MsgData_cSecretDungeonSweep();
                data.id = id;
                data.num = num;
                LogMgr.UnityLog(string.Format("SendReqSecretDungeonSweep num:{0}, id:{1}", data.num, data.id));
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_SecretDungeonSweep, data);
            }
            public void SendReqSecretDungeonSweepReward(
                		int id // 层数id
                    ) // 请求:个人秘境副本扫荡领奖励 msgId:3968;
            {
                MsgData_cSecretDungeonSweepReward data = new MsgData_cSecretDungeonSweepReward();
                data.id = id;
                LogMgr.UnityLog(string.Format("SendReqBuySecrectDuplTili id:{0}", id));
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_SecretDungeonSweepReward, data); 
            }


            public void onResSimpleSecrectDuplInfo(MsgData msg) // 返回单人秘境副本面板信息 msgId:8637;
            { 
                MsgData_sResSimpleSecrectDuplInfo data = msg as MsgData_sResSimpleSecrectDuplInfo;
                LogMgr.UnityLog(string.Format("onResSimpleSecrectDuplInfo : tili:{0} counts:{1} vlaTag :{2} vlaTagTeam:{3} simpleMaxLayerCount:{4}", data.tili, data.counts, data.vlaTag, data.vlaTagTeam, data.simpleMaxLayerCount));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ResSimpleSecrectDuplInfo, EventParameter.Get(msg));
            }
            public void onResEnterSimpleSecrectDupl(MsgData msg) // 返回进入单人秘境副本 msgId:8638;
            {
                MsgData_sResEnterSimpleSecrectDupl data = msg as MsgData_sResEnterSimpleSecrectDupl;
                LogMgr.UnityLog(string.Format("onResEnterSimpleSecrectDupl : result:{0} id:{1} type:{2} count:{3}", data.result, data.id, data.type, data.count));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ResEnterSimpleSecrectDupl, EventParameter.Get(msg));
            }
            public void onSimpleSecrectDuplTrace(MsgData msg) // 秘境副本追踪面饭信息 msgId:8639;
            {
                MsgData_sSimpleSecrectDuplTrace data = msg as MsgData_sSimpleSecrectDuplTrace;
                LogMgr.UnityLog(string.Format("onSimpleSecrectDuplTrace : num:{0} value:{1}id:{2}", data.num, data.value, data.id));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SimpleSecrectDuplTrace, EventParameter.Get(msg));
            }
            public void onSimpleSecrectDuplCom(MsgData msg) // 单人秘境副本结算 msgId:8640;
            {
                MsgData_sSimpleSecrectDuplCom data = msg as MsgData_sSimpleSecrectDuplCom;
                LogMgr.UnityLog(string.Format("onSimpleSecrectDuplCom : result:{0}", data.result));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SimpleSecrectDuplCom, EventParameter.Get(msg));
            }
            public void onResExitSimpleSecrectDupl(MsgData msg) // 返回退出单人秘境副本 msgId:8641;
            {
                //MsgData_sResExitSimpleSecrectDupl data = msg as MsgData_sResExitSimpleSecrectDupl;
                LogMgr.UnityLog(string.Format("onResExitSimpleSecrectDupl"));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ResExitSimpleSecrectDupl, EventParameter.Get(msg));
            }
            public void onUpdateSecrectDuplTili(MsgData msg) // 更新组队或次数 msgId:8642;
            {
                MsgData_sUpdateSecrectDuplTili data = msg as MsgData_sUpdateSecrectDuplTili;
                LogMgr.UnityLog(string.Format("onUpdateSecrectDuplTili : type:{0} param:{1} vlatag:{2}", data.type, data.param, data.vlaTag));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_UpdateSecrectDuplTili, EventParameter.Get(msg));

            }
            public void onResJiHuoSecrectDupl(MsgData msg) // 返回激活结果 msgId:8643;
            {
                MsgData_sResJiHuoSecrectDupl data = msg as MsgData_sResJiHuoSecrectDupl;
                LogMgr.UnityLog(string.Format("onResJiHuoSecrectDupl : count:{0}", data.count));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ResJiHuoSecrectDupl, EventParameter.Get(msg));

            }
            public void onSecretDungeonSweep(MsgData msg) // 返回:个人秘境副本扫荡 msgId:8967;
            {
                MsgData_sSecretDungeonSweep data = msg as MsgData_sSecretDungeonSweep;
                LogMgr.UnityLog(string.Format("onSecretDungeonSweep : result:{0} id :{1} num:{2} second:{3}", data.result, data.id, data.num, data.second));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SecretDungeonSweep, EventParameter.Get(msg));

            }
            public void onSecretDungeonSweepReward(MsgData msg) // 返回:个人秘境副本扫荡领奖励 msgId:8968;
            {
                MsgData_sSecretDungeonSweepReward data = msg as MsgData_sSecretDungeonSweepReward;
                LogMgr.UnityLog(string.Format("onSecretDungeonSweepReward : result:{0} id :{1} num:{2}", data.result, data.id, data.num));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SecretDungeonSweepReward, EventParameter.Get(msg));
            }
            public void onSecretDungeonSweepInfo(MsgData msg)// 返回:个人秘境副本扫荡 msgId:8969;
            {
                MsgData_sSecretDungeonSweepInfo data = msg as MsgData_sSecretDungeonSweepInfo;
                LogMgr.UnityLog(string.Format("onSecretDungeonSweepInfo : second:{0} id :{1} num:{2}", data.second, data.id, data.num));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SecretDungeonSweepInfo, EventParameter.Get(msg));
            }

        //秘境组队
        public void SendCreateRoom(int layer) // 创建房间
        {
            LogMgr.UnityLog(string.Format("SendCreateRoom"));
            MsgData_cCreateRoom data = new MsgData_cCreateRoom();
            data.dungeonIndex = layer;
            data.autoStart = 1;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_CreateRoom, data);
        }

        public void SendReqRoomStart() // 房间队伍开始战斗 msgId:2163;
        {
            LogMgr.UnityLog(string.Format("SendReqRoomStart"));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ReqRoomStart);
        }

        public void SendSecretTeamHeadNoticePrepare() // 组队秘境，请求准备提醒 msgId : 2289
        {
            LogMgr.UnityLog(string.Format("SendSecretTeamHeadNoticePrepare"));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_SecretTeamHeadNoticePrepare);
        }

        public void SendSecretTeamStart(int id) // 试炼秘境队伍请求进入组队活动 msgId : 2907
        {
            LogMgr.UnityLog(string.Format("SendSecretTeamStart id:{0}", id));
            SecretDuplDataMgr.Instance.EnterTeamID = id;
            MsgData_cSecretTeamStart data = new MsgData_cSecretTeamStart();
            data.dungeonID = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_SecretTeamStart, data);            
        }

        public void SendUpdateTeamTarget(int targetID)
        {
            LogMgr.UnityLog(string.Format("SendUpdateTeamTarget id:{0}", targetID));
            MsgData_cUpdateTeamTarget data = new MsgData_cUpdateTeamTarget();
            data.teamTargetID = targetID;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_UpdateTeamTarget, data);
        }

        public void SendUpdateTeamLimit(int power, int gloryLevel)
        {
            LogMgr.UnityLog(string.Format("SendUpdateTeamLimit"));
            MsgData_cUpdateTeamLimit data = new MsgData_cUpdateTeamLimit();
            data.teamTargetID = PlayerData.Instance.TeamData.TargetID;
            data.powerLimit = power;
            data.gloryLevelLimit = gloryLevel;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_UpdateTeamLimit, data);
        }

        /// <summary>
        /// 发送组队活动状态请求。
        /// </summary>
        /// <param name="prepare">准备状态 0 true 1 false。</param>
        /// <param name="type">邀请类型 0:正常组队 1:魔域深渊  2:奇遇 3:帮派组队。</param>
        public void SendEnterTeamDungeonRequest(int prepare, int type)
        {
            PlayerData.Instance.TeamData.SendEnterTeamDungeonRequest(prepare, type);
        }

        #endregion

        #region 宝塔秘境
        public void SendEnterTreasureDupl(int id)    // 客户端请求：进入宝塔秘境 msgId:4940;
        {
            MsgData_cEnterTreasureDupl data = new MsgData_cEnterTreasureDupl();
            data.id = id;
            LogMgr.UnityLog(string.Format("SendEnterTreasureDupl id:{0}", id));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_EnterTreasureDupl, data);
        }
        public void SendQuitTreasureDupl()   // 客户端请求：退出宝塔秘境 msgId:4941;
        {
            LogMgr.UnityLog(string.Format("SendQuitTreasureDupl"));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_QuitTreasureDupl);
        }
        public void onEnterTreasureDupl(MsgData msg)    // 服务器返回：进入宝塔秘境 msgId:9951;
        {
            MsgData_sEnterTreasureDupl data = msg as MsgData_sEnterTreasureDupl;
            LogMgr.UnityLog(string.Format("onEnterTreasureDupl id : {0} result:{1}",data.id,data.result));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EnterTreasureDupl, EventParameter.Get(msg));
        }
        public void onQuitTreasureDupl(MsgData msg)// 服务器返回：退出宝塔秘境 msgId:9950;
        {
            MsgData_sQuitTreasureDupl data = msg as MsgData_sQuitTreasureDupl;
            LogMgr.UnityLog(string.Format("onQuitTreasureDupl result:{0}",data.result));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_QuitTreasureDupl, EventParameter.Get(msg));
        }


        public void onFindTreasureInfo(MsgData msg)    // 寻宝信息 msgId:8475;
            {
                MsgData_sFindTreasureInfo data = msg as MsgData_sFindTreasureInfo;
                LogMgr.UnityLog(string.Format("onFindTreasureInfo : mapid:{0}  mapid2: {1} wabaoid:{2} getlvl:{3} lastnum:{4} lookpoint :{5}",data.mapid,data.mapid2,data.wabaoId,data.getlvl,data.lastNum,data.lookPoint));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FindTreasureInfo, EventParameter.Get(msg));
            }
            public void onFindTreasureResult(MsgData msg)    // 服务器返回:寻宝任务接取 结果 msgId:8476;
            {
                MsgData_sFindTreasureResult data = msg as MsgData_sFindTreasureResult;
                LogMgr.UnityLog(string.Format("onFindTreasureResult : mapid:{0}  mapid2: {1} wabaoid:{2} getlvl:{3} lastnum:{4} result :{5}", data.mapid, data.mapid2, data.wabaoId, data.getlvl, data.lastNum, data.result));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FindTreasureResult, EventParameter.Get(msg));

            }
            public void onFindTreasureCancel(MsgData msg)    // 服务器返回:取消寻宝任务 msgId:8477;
            {
                MsgData_sFindTreasureCancel data = msg as MsgData_sFindTreasureCancel;
                LogMgr.UnityLog(string.Format("onFindTreasureCancel : result:{0} ",data.result));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FindTreasureCancel, EventParameter.Get(msg));
            }
            public void onFindTreasureCollect(MsgData msg)    // 服务器返回:接取结果 msgId:8478;
            {
                MsgData_sFindTreasureCollect data = msg as MsgData_sFindTreasureCollect;
                LogMgr.UnityLog(string.Format("onFindTreasureCollect : result:{0} mapid:{1} restype:{2} resid:{3}", data.result,data.mapid,data.resType,data.resId));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FindTreasureCollect, EventParameter.Get(msg));
            }
            public void onTreasureTodayAddedTimer(MsgData msg)    // 返回今日已经购买的时间 msgId:8676;
            {
                MsgData_sTreasureTodayAddedTimer data = msg as MsgData_sTreasureTodayAddedTimer;
                LogMgr.UnityLog(string.Format("onTreasureTodayAddedTimer : addedTimer:{0} ", data.addedTimer));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TreasureTodayAddedTimer, EventParameter.Get(msg));

            }
            public void onTreasureRemainTime(MsgData msg)    // 打宝塔剩余时间 msgId:8677;
            {
                MsgData_sTreasureRemainTime data = msg as MsgData_sTreasureRemainTime;
                LogMgr.UnityLog(string.Format("onTreasureRemainTime : remainTime:{0} ", data.remainTime));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TreasureRemainTime, EventParameter.Get(msg));
            }
            public void onTreasureUpdateBoss(MsgData msg)    // 更新BOSS状态信息 msgId:8678;
            {
                MsgData_sTreasureUpdateBoss data = msg as MsgData_sTreasureUpdateBoss;
                LogMgr.UnityLog(string.Format("onTreasureUpdateBoss : count:{0} ", data.count));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TreasureUpdateBoss, EventParameter.Get(msg));
            }
  

            public void SendReqFindTreasure(int quality)    // 客户端请求：寻宝任务接取 msgId:3476;
            {
                MsgData_cFindTreasure data = new MsgData_cFindTreasure();
                data.quality = quality;
                LogMgr.UnityLog(string.Format("SendReqFindTreasure quality:{0}",quality));
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_FindTreasure, data);
            }
            public void SendReqFindTreasureCancel()    //  客户端请求：取消寻宝任务 msgId:3477;
            {
                LogMgr.UnityLog(string.Format("SendReqFindTreasureCancel"));
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_FindTreasureCancel);
            }
            public void SendReqFindTreasureCollect()    //  客户端请求：接取 msgId:3478;
            {
                LogMgr.UnityLog(string.Format("SendReqFindTreasureCollect"));
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_FindTreasureCollect);
            }
            public void SendReqTreasureAddTime(int id,int count)    // 请求打宝塔增加时间 msgId:3676;
            {
                MsgData_cReqTreasureAddTime data = new MsgData_cReqTreasureAddTime();
                data.id = id;
                data.count = count;
                LogMgr.UnityLog(string.Format("SendReqReqTreasureAddTime id: {0} count: {1}",id,count));
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_ReqTreasureAddTime, data);
            }


#endregion

#region 查询怪物数量
            public void onQueryMonsterByPosition(MsgData msg)
            {
                MsgData_sQueryMonsterByPosition data = msg as MsgData_sQueryMonsterByPosition;
                //LogMgr.LogError(string.Format("onQueryMonsterByPosition : x:{0} y:{1} num:{2}", data.x,data.y,data.num));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_QueryMonsterByPosition, EventParameter.Get(msg));
            }

            public void SendQueryMonsterByPosition(int x, int y)    
            {
                MsgData_cQueryMonsterByPosition data = new MsgData_cQueryMonsterByPosition();
                data.x = x;
                data.y = y;
            LogMgr.UnityLog(string.Format("SendQueryMonsterByPosition x: {0} y: {1}", x, y));
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_QueryMonsterByPosition, data);
            }
#endregion

#region 装备传承
        public void OnEquipInherit(MsgData msg)
        {
            MsgData_sEquipInherit data = msg as MsgData_sEquipInherit;
            LogMgr.UnityLog(string.Format("OnEquipInherit {0} ", data.result));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EquipInherit, EventParameter.Get(msg));
        }

        public void SendEquipInherit(long srcid, long tarid, int autoBuy)
        {
            LogMgr.UnityLog("SendEquipInherit");
            MsgData_cEquipInherit data = new MsgData_cEquipInherit();
            data.srcid = srcid;
            data.tarid = tarid;
            data.autoBuy = autoBuy;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_EquipInherit, data);
        }

        public void OnEquipPeiYang(MsgData msg)
        {
            MsgData_sEquipPeiYang data = msg as MsgData_sEquipPeiYang;
            LogMgr.UnityLog(string.Format("OnEquipPeiYang {0} ", data.result));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EquipPeiYang, EventParameter.Get(msg));
        }

        public void OnEquipPeiYangSet(MsgData msg)
        {
            MsgData_sEquipPeiYangSet data = msg as MsgData_sEquipPeiYangSet;
            LogMgr.UnityLog(string.Format("OnEquipPeiYangSet {0} ", data.result));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EquipPeiYangSet, EventParameter.Get(msg));
        }

        public void OnDailyActivy(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_DailyActivy, EventParameter.Get(msg));
        }

        public void OnGetRechargeOrder(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_XY_RECHARGE_MSG, EventParameter.Get(msg));
        }

        public void OnRechargeRet(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_XY_PAY_MSG, EventParameter.Get(msg));
        }
        /// <summary>
        /// 第一拨平台回复订单
        /// </summary>
        /// <param name="msg"></param>
        public void OnGetRechargeOrder_DYB(MsgData msg)
        {
            Debug.Log("第一拨创建订单回复");
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_DYB_RECHARGE_MSG, EventParameter.Get(msg));
        }


        public void OnGetVerifyAccount(MsgData msg)
        {
            EventParameter ev = EventParameter.Get(msg);
            MsgData_sVerifyAccount data = msg as MsgData_sVerifyAccount;
            ev.stringParameter = data.data.ToArray().BytesToString();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_XY_VerifyAccount, ev);
        }

        public void SendEquipPeiYang(long uid )
        {
            LogMgr.UnityLog("SendEquipPeiYang");
            MsgData_cEquipPeiYang data = new MsgData_cEquipPeiYang();
            data.id = uid;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_EquipPeiYang, data);
        }

        public void SendEquipPeiYangSet(long uid)
        {
            LogMgr.UnityLog("SendEquipPeiYangSet");
            MsgData_cEquipPeiYangSet data = new MsgData_cEquipPeiYangSet();
            data.id = uid;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_EquipPeiYangSet, data);
        }
                    
        #endregion
        public void OnGuildQuestSweep(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_GuildQuestSweep, EventParameter.Get(msg));
        }
        public void OnWorldBoss(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_WC_WorldBoss, EventParameter.Get(msg));
        }
        public void OnActivityState(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_WC_ActivityState, EventParameter.Get(msg));
        }
        public void OnActivityEnter(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ActivityEnter, EventParameter.Get(msg));
        }
        public void OnActivityQuit(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ActivityQuit, EventParameter.Get(msg));
        }

        public void OnActivity(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_Activity, EventParameter.Get(msg));
        }

        public void OnActivityFinish(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ActivityFinish, EventParameter.Get(msg));
        }

        public void OnWorldBossDamage(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_WorldBossDamage, EventParameter.Get(msg));
        }

        public void OnFieldBoss(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_FieldBoss, EventParameter.Get(msg));
        }

        public void OnWorldBossHurt(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_WorldBossHurt, EventParameter.Get(msg));
        }

        public void OnWaBaoList(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_WaBaoList, EventParameter.Get(msg));
        }

        public void OnGetWaBaoReward(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_GetWaBaoReward, EventParameter.Get(msg));
        }

        public void OnZhenBaoGe(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ZhenBaoGe, EventParameter.Get(msg));
        }

        public void OnTimeDungeonRoomInfo(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_TimeDungeonRoomInfo, EventParameter.Get(msg));
        }

        public void OnEnterDulpPrepare(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_EnterDulpPrepare, EventParameter.Get(msg));
        }

        public void OnTeamTargetData(MsgData msg)
        {
            var teamData = PlayerData.Instance.TeamData;
            MsgData_sTeamTargetData data = msg as MsgData_sTeamTargetData;
            teamData.TargetID = data.teamTargetID;
            teamData.PowerLimit = data.powerLimit;
            teamData.GloryLevelLimit = data.gloryLevelLimit;

            EventParameter ep = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TEAM_CHANGE_TARGET, ep);
        }

        public void OnTriggerTrap(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GS_SC_TRIGGERTRAP, EventParameter.Get(msg));
        }
        public void OnChangeBegin(MsgData msg)
        {
            CoreEntry.gMorphMgr.OnChangeBegin(msg);
        }
        public void OnChangeEnd(MsgData msg)
        {
            CoreEntry.gMorphMgr.OnChangeEnd(msg); 
        }
        public void OnSceneObjHongyanLevel(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OBJ_HONGYAN_LEVEL, EventParameter.Get(msg));      
        }

        public void OnAddExpInfo(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_AddExpInfo, EventParameter.Get(msg));
        }

        // 连接结果回调，如果成功，则表示已连接并鉴权通过.
        public void onConnectedGame(bool isSuccessfull, IPEndPoint remote)
        {
            if (isSuccessfull == true)
            {
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_LOGIN, OnLogin);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ROLEINFO, OnRoleInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CREATEROLE, OnCreateRole);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ENTERGAME, OnEnterGame);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ENTERSCENE, OnEnterScene);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_MAINPLAYERENTERSCENE, OnMainPlayerEnterScene);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_LEAVE_GAME, OnLeaveGame);

                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_OBJDEAD, OnObjectDead);

                Account.Instance.RegisterNetMsg();
                CoreEntry.netMgr.Reconnect.RegisterNetMsg();

                //技能
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CASTSKILL_FAILD, OnCastSkill);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CASTBEGIN, OnCastBegin);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CASTEND, OnCastEnd);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CASTEFFECT, OnSkillEffect);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_MAGICCOOLDOW, OnSkillMagicCooldown);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CASTPREPBEGAN, OnCastPrepBegan);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CASTPREPENDED, OnCastPrepEnded);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CASTCHANBEGAN, OnCastChangeBegan);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CASTCHANENDED, OnCastChangeEnd);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_INTERRUPTCAST, OnInterpuptCast);

                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SKILL_TARGET_LIST, OnGetSkillTarget);

                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_MOVE_EFFECT, OnCastMoveEffect);


                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_KNOCKBACK, OnHitBack);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ADDBUFF, OnAddBuff);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_UPDATEBUFF, OnUpdateBuff);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DELBUFF, OnDelBuff);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ADDBUFFLIST, OnAddBuffList);

                //移动
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_OTHERMOVETO, OnOtherMoveTo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_OTHERCHANGEDIR, OnOtherChangeDir);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_OTHERMOVESTOP, OnOtherMoveStop);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_MONSTERMOVETO, OnMonsterMoveTo); 
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TELEPORT_RESULT, OnTeleportResult); 
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CHANGE_POS, OnChangePos);

                //场景对象进入和离开
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ROLEATTRINFO, OnAttrInfo);
                CoreEntry.netMgr.bindMsgExHandler(NetMsgDef.S_OBJENTERSCENE, OnObjEnterScene);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_OBJLEAVESCENE, OnObjLeaveScene);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_OBJDISAPPEAR, OnObjDisappear);

                //服务端通知:状态位改变
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_STATECHANGED, OnStateChanged); 
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_PLAYER_SHOW_CHANGED, OnPlayerShowChanged);

                //玩家数据层消息注册
                PlayerData.Instance.RegisterNetMsg();
                OtherPlayerData.Instance.RegisterNetMsg();

                //复活
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_REVIVE, OnRevive);

                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CONNECTINFO, EventParameter.Get((int)NetMgr.ConnectInfo.connectedlogin_Successfull));

                //任务
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_QueryQuestResult, OnQueryQuestResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_QuestAdd, OnQuestAdd);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_QuestUpdate, OnQuestUpdate);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GiveupQuestResult, OnGiveupQuestResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FinishQuestResult, OnFinishQuestResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_QuestDel, OnQuestDel);

                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DailyQuestStar, OnDailyQuestStar);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DailyQuestFinish, OnDailyQuestFinish);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DailyQuestResult, OnDailyQuestResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DQDraw, OnDQDraw);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_AgainstQuestStar, OnAgainstQuestStar);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_AgainstQuestFinish, OnAgainstQuestFinish);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_AgainstQuestResult, OnAgainstQuestResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_AgainstQDraw, OnAgainstQDraw);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GetAgainstQSkipReward, OnGetAgainstQSkipReward);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_AgainstQuestSkipResult, OnAgainstQuestSkipResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_AgainstQDrawNotice, OnAgainstQDrawNotice);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_QuestRewardResult, OnQuestRewardResult);


                //拾取
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_PICKUPITEM, OnPickUpItem);

                //装备 
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EQUIPPOSLEVELUP, OnEquipPosLevelUp);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EQUIPPOSINFO, OnEquipPosInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_OTHEREQUIPPOS, OnOtherEquipPosInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EQUIPDECOMPOSE, OnEquipDecompose);

                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EQUIPADD, OnEquipAdd);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EQUIPSUPER, OnEquipSuper);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EQUIPEXTRA, OnEquipExtra);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EQUIPNEWSUPER, OnEquipNewSuper);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EQUIPINFO, OnEquipInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EQUIPPOSSTARSELECT, OnEquipStarSelect);
                //静物触碰
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_STRUCT_DEF, OnStructDefResult);

                //物品获得失去提示
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ITEMTIPS, OnItemTips);

                //副本战斗-任务副本
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DungeonQuestStateUpdate, OnDungeonQuestStateUpdate);

                //副本战斗- 法宝幻境
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_LingShouMuDiInfo, OnLingShouMuDiInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ResetLingShouMuDi, OnResetLingShouMuDi);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ChallLingShouMuDi, OnChallLingShouMuDi);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ChallLingShouMuDiResult, OnChallLingShouMuDiResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ScanLingShouMuDiResult, OnScanLingShouMuDiResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_LingShouMuDiQuit, OnLingShouMuDiQuit);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_LingShouMuDiGetAward, OnLingShouMuDiGetAward);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_LingShouMuDiMonsterInfo, OnLingShouMuDiMonsterInfo);

                //副本战斗- 魔塔幻境
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_HunLingXianYuInfo, OnHunLingXianYuInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ResetHunLingXianYu, OnResetHunLingXianYu);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ChallHunLingXianYu, OnChallHunLingXianYu);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ChallHunLingXianYuResult, OnChallHunLingXianYuResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ScanHunLingXianYuResult, OnScanHunLingXianYuResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_HunLingXianYuQuit, OnHunLingXianYuQuit);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_HunLingXianYuGetAward, OnHunLingXianYuGetAward);

                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_HunLingXianYuMonsterInfo, OnHunLingXianYuMonsterInfo);

                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SKILL_CDLIST, OnSkillCDList);

                //任务副本
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EnterDungeonResult, OnEnterDungeonResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_LeaveDungeonResult, OnLeaveDungeonResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_StoryEndResult, OnStoryEndResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_StoryStep, OnStoryStep);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DungeonGroupUpdate, OnDungeonGroupUpdate);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DungeonCountDown, OnDungeonCountDown);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DungeonPassResult, OnDungeonPassResult);

                //境界
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DianfengInfo, OnDianfengInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DianfengSave, OnDianfengSave);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DianfengReset, OnDianfengReset);

                //翅膀
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_WingInfo, OnWingInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_UseSkin,OnUseSkin);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_WinAndSkinInfo, OnWinAndSkinInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_WingHeCheng, OnWingHeCheng);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_UseWingOrginal, OnUseWingOrginal);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ActivateSkin, OnActivateSkin);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_WingExtActive, OnWingExtActive);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_WingExtActiveInfo, OnWingExtActiveInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_WingExtCountInfo, OnWingExtCountInfo);
                //成就
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_BackAchievementInfo, OnBackAchievementInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_AchievementUpData, OnAchievementUpData);

                //法宝
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_REQMAGICKEYDECOMPOSE, OnReqMagicKeyDecompose);
                //更新法宝信息 
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_UPDATEMAGICKEYINFO,OnUpdateMagicKeyInfo);
               //返回法宝培养结果 
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TRAINMAGICKEY,OnTrainMagicKey);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_MAGICKEYWASHINFO, OnMagicKeyWashInfo);
                
                //返回法宝打造结果 
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_MAKEMAGICKEY, OnMakeMagicKey);
                //返回法宝仙灵列表
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_RETURNMAGICKEYGODINFOS, OnReturnMagicKeyGodInfos);
                //返回仙灵穿戴结果 
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_RESMAIGCKEYGODINSET, OnResMaigcKeyGodInset);
                // 返回法宝技能 
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_MAGICKEYINSETSKILL, OnMagicKeyInsetSkill);
                //返回:法宝飞升升经验
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_MagicKeyFeiSheng, OnMagicKeyFeiSheng);
                // 返回法宝被动技能领悟
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_RETURNMAGICKEYSKILLLINGWU, OnReturnMagickeySKillLingwu);
                //法宝升星 
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_MAGICKEYSTARLEVELUP, OnMagicKeyStarLevelUp);

                //合成分解
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ITEMCOMPOSE, OnItemCompose);


                //红颜
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_HONGYANACT, OnHONGYANACT);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_HONGYANFIGHT, OnHONGYANFIGHT);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_BeautyWomanLevelUp, OnBeautyWomanLevelUp);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_BeautyWomanInfo, OnBeautyWomanInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_BeautyWomanUseAtt, OnBeautyWomanUseAtt);

                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.s_BeautyWomanFightingUpdate, OnBeautyWomanFightingUpdate);
                //神兵
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_MagicWeaponInfo, OnMagicWeaponInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_MagicWeaponProficiency, OnMagicWeaponProficiency);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_MagicWeaponLevelUp, OnMagicWeaponLevelUp);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_MagicWeaponChangeModel, OnMagicWeaponChangeModel);
                //圣盾
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_PiFengInfo, OnPiFengInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_PiFengLevelUp, OnPiFengLevelUp);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_PiFengChangeModel, OnPiFengChangeModel);

                //灌注
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_USE_ATTR_DAN, OnUSE_ATTR_DAN);

                //等级副本
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DominateRouteData, OnDominateRouteData);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DominateRouteUpDate, OnDominateRouteUpDate);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_BackDominateRouteChallenge, OnBackDominateRouteChallenge);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_BackDominateRouteQuit, OnBackDominateRouteQuit);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_BackDominateRouteInfo, OnBackDominateRouteInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_BackDominateRouteWipe, OnBackDominateRouteWipe);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_BackDominateRouteVigor, OnBackDominateRouteVigor);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_BackDominateRouteBoxReward, OnBackDominateRouteBoxReward);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_BackDominateRouteEnd, OnBackDominateRouteEnd);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_BackDominateRouteMopupEnd, OnBackDominateRouteMopupEnd);


                //boss挑战副本
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_PersonalBossList, OnPersonalBossList);// 服务器返回BOSS挑战列表 msgId:8560;
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_BackEnterResultPersonalBoss, OnBackEnterResultPersonalBoss);// 服务器返回进入个人BOSS结果 msgId:8561;
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_BackQuitPersonalBoss, OnBackQuitPersonalBoss);// 服务器:退出个人BOSS结果 msgId:8562;
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_PersonalBossResult, OnPersonalBossResult);// 服务器:挑战个人BOSS结果 msgId:8563;

                //通缉
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TongJiInfo, OnTongJiInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TongJiLvlRefreshResult, OnTongJiLvlRefreshResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_AcceptTongJiResult, OnAcceptTongJiResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FinishTongJi, OnFinishTongJi);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GetTongJiReward, OnGetTongJiReward);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GiveupTongJiResult, OnGiveupTongJiResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_RefreshTongJiList, OnRefreshTongJiList);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GetTongJiBoxResult, OnGetTongJiBoxResult);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TongJiRefreshState, OnTongJiRefreshState);
                //邮件
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GetMailResult, OnGetMailResult);// 返回邮件列表 msgId:7032;
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_OpenMailResult, OnOpenMailResult);// 返回打开邮件 msgId:7033;
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GetMailItemResult, OnGetMailItemResult);// 请求领取附件返回 msgId:7034;
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DelMail, OnDelMail);// 请求删除邮件返回 msgId:7035;
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_NotifyMail, OnNotifyMail);// 邮件提醒 msgId:7036;

               ArenaMgr.Instance.ResigtNetMsg();

               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GuanZhiInfo, OnGuanZhiInfo);// 服务器返回：官职信息 msgId：8689；
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GuanZhiLevelUp, OnGuanZhiLevelUp);// 服务器返回：官职升级 msgId：8688；
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_InterServiceContValue, OnInterServiceContValue);// 返回服务器功勋 msgId：8810
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_HuoYueDu, OnHuoYueDu);// 服务器返回：官职信息 msgId：8689；
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_HuoYueReward, OnHuoYueReward);// 服务器返回：官职升级 msgId：8688；
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_HuoYueDuFinish, OnHuoYueDuFinish);// 返回服务器功勋 msgId：8810
               
	            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_WaterDungeonEnterResult, OnWaterDungeonEnterResult);     // 服务器返回:进入流水副本结果 msgId:8439;
	            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_WaterDungeonExitResult, OnWaterDungeonExitResult);     // 服务器返回:退出流水副本结果 msgId:8440;
	            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_WaterDungeonInfo, OnWaterDungeonInfo);     // 服务器返回:流水副本信息 msgId:8434;
	            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_WaterDungeonProgress, OnWaterDungeonProgress);     // 服务器返回:流水副本进度 msgId:8436;
	            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_WaterDungeonResult, OnWaterDungeonResult);     // 服务器返回:流水副本结算 msgId:8437;


               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_NeiGongInfo, OnNeiGongInfo); // 服务器返回: 经脉信息 msgId:8665;
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_OpenNode, OnOpenNode); // 服务器返回:冲穴结果 msgId:8666;

                //秘境
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ResSimpleSecrectDuplInfo, onResSimpleSecrectDuplInfo); // 返回单人秘境副本面板信息 msgId:8637;
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ResEnterSimpleSecrectDupl, onResEnterSimpleSecrectDupl); // 返回进入单人秘境副本 msgId:8638;
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SimpleSecrectDuplTrace, onSimpleSecrectDuplTrace); // 秘境副本追踪面饭信息 msgId:8639;
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SimpleSecrectDuplCom, onSimpleSecrectDuplCom); // 单人秘境副本结算 msgId:8640;
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ResExitSimpleSecrectDupl, onResExitSimpleSecrectDupl); // 返回退出单人秘境副本 msgId:8641;
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_UpdateSecrectDuplTili, onUpdateSecrectDuplTili); // 更新组队或次数 msgId:8642;
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ResJiHuoSecrectDupl, onResJiHuoSecrectDupl); // 返回激活结果 msgId:8643;
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SecretDungeonSweep, onSecretDungeonSweep); // 返回:个人秘境副本扫荡 msgId:8967;
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SecretDungeonSweepReward, onSecretDungeonSweepReward); // 返回:个人秘境副本扫荡领奖励 msgId:8968;
               CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SecretDungeonSweepInfo, onSecretDungeonSweepInfo); // 返回:个人秘境副本扫荡 msgId:8969;

                //活动， 世界boss
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_WorldBoss, OnWorldBoss);  //返回世界BOSS列表(刷新时推单个) msgId:7064
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ActivityState, OnActivityState); //返回活动状态(刷新时推单个) msgId:7065
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ActivityEnter, OnActivityEnter); // S->C 返回:进入活动 msgId:8159
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ActivityQuit, OnActivityQuit); // S->C 返回:退出活动 msgId:8160
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_Activity, OnActivity);  //返回世界BOSS列表(刷新时推单个) msgId:7064
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ActivityFinish, OnActivityFinish); //返回活动状态(刷新时推单个) msgId:7065
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_WorldBossDamage, OnWorldBossDamage); // S->C 返回玩家累计伤害 msgId:8162
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FieldBoss, OnFieldBoss); // 返回野外Boss信息 msgId:8737;
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_WorldBossHurt, OnWorldBossHurt); // S->C 返回:退出活动 msgId:8160
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_WaBaoList, OnWaBaoList); // S->C 返回:进入活动 msgId:8159
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GetWaBaoReward, OnGetWaBaoReward); // S->C 返回:退出活动 msgId:8160

                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ZhenBaoGe, OnZhenBaoGe); // 返回珍宝阁数据，数据刷新时也返回这个 msgId：8113；

                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TimeDungeonRoomInfo, OnTimeDungeonRoomInfo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EnterDulpPrepare, OnEnterDulpPrepare);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TeamTargetData, OnTeamTargetData);

                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TRAPTRIGGER, OnTriggerTrap);

                //宝塔秘境
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EnterTreasureDupl, onEnterTreasureDupl);// 服务器返回：进入宝塔秘境 msgId:9951;
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_QuitTreasureDupl,onQuitTreasureDupl);// 服务器返回：退出宝塔秘境 msgId:9950;

                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FindTreasureInfo, onFindTreasureInfo);    // 寻宝信息 msgId:8475;
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FindTreasureResult, onFindTreasureResult);    // 服务器返回:寻宝任务接取 结果 msgId:8476;
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FindTreasureCancel, onFindTreasureCancel);    // 服务器返回:取消寻宝任务 msgId:8477;
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FindTreasureCollect, onFindTreasureCollect);    // 服务器返回:接取结果 msgId:8478;
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TreasureTodayAddedTimer, onTreasureTodayAddedTimer);    // 返回今日已经购买的时间 msgId:8676;
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TreasureRemainTime, onTreasureRemainTime);    // 打宝塔剩余时间 msgId:8677;
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TreasureUpdateBoss, onTreasureUpdateBoss);    // 更新BOSS状态信息 msgId:8678;

                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_QueryMonsterByPosition, onQueryMonsterByPosition);    // 服务器返回:查询指定点有多少怪 msgId:9948

                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EquipGroup, OnEquipGroup);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EquipGroupTwo,OnEquipGroupTwo);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EquipSmelt, OnEquipSmelt);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EquipGroupLvlUp, OnEquipGroupLvlUp);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EquipGroupPeel, OnEquipGroupPeel);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EquipInherit, OnEquipInherit);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EquipPeiYang, OnEquipPeiYang);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_EquipPeiYangSet, OnEquipPeiYangSet);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_DailyActivy, OnDailyActivy);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GETRECHARGEORDER, OnGetRechargeOrder);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_RechargeRet, OnRechargeRet);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GETRECHARGEORDER_DYB, OnGetRechargeOrder_DYB); //回复创建订单
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_VERIFYACCOUNT, OnGetVerifyAccount);

                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_GuildQuestSweep, OnGuildQuestSweep);  //返回帮派任务一键完成奖励信息 msgId:8942

                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CHANGEBEGIN, OnChangeBegin);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_CHANGEEND, OnChangeEnd);
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SCENE_OBJ_HONGYAN_LEVEL, OnSceneObjHongyanLevel);
                
                CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_AddExpInfo, OnAddExpInfo);
            }
            else
            {
                LogMgr.UnityError("connect game server failed!");
            }
        }

        public void OnWingInfo(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_WingInfo, EventParameter.Get(data));
        }
        public void OnUseSkin(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_UseSkin, EventParameter.Get(data));
        }
        public void OnWinAndSkinInfo(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_WinAndSkinInfo, EventParameter.Get(data));
        }
        public void OnWingHeCheng(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_WingHeCheng, EventParameter.Get(data));
        }
        public void OnUseWingOrginal(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_UseWingOrginal, EventParameter.Get(data));
        }
        public void OnActivateSkin(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ActivateSkin, EventParameter.Get(data));
        }
        public void OnWingExtActive(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_WingExtActive, EventParameter.Get(data));
        }
        public void OnWingExtActiveInfo(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_WingExtActiveInfo, EventParameter.Get(data));
        }
        public void OnWingExtCountInfo(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_WingExtCountInfo, EventParameter.Get(data));
        }
        public void OnBackAchievementInfo(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_BackAchievementInfo, EventParameter.Get(data));
        }
        public void OnAchievementUpData(MsgData data)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_AchievementUpData, EventParameter.Get(data));
        }

        public void OnMagicWeaponInfo(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_MagicWeaponInfo, EventParameter.Get(msg));
        }

        public void OnMagicWeaponProficiency(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_MagicWeaponProficiency, EventParameter.Get(msg));
        }

        public void OnMagicWeaponLevelUp(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_MagicWeaponLevelUp, EventParameter.Get(msg));
        }

        public void OnMagicWeaponChangeModel(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_MagicWeaponChangeModel, EventParameter.Get(msg));
        }
        public void OnPiFengInfo(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_PiFengUpdateInfo, EventParameter.Get(msg));
        }
        public void OnPiFengLevelUp(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_PiFengLevelUp, EventParameter.Get(msg));
        }

        public void OnPiFengChangeModel(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_PiFengChangeModel, EventParameter.Get(msg));
        }

        public void OnUSE_ATTR_DAN(MsgData msg)
        {
            EventParameter param = EventParameter.Get(msg);
            param.autoRecycle = false;
            param.objParameter = msg;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_USE_ATTR_DAN, param);
        }

        
        // 连接关闭
        public void onConnectedCloseGame()
        {
            LogMgr.UnityWarning("connection close! please reconnect!");
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CONNECT_CLOSE, null);
        }

        public void onConnectedError(int errorCode)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CONNECT_CLOSE, null);

        }

        // 连接游戏网关
        public void connectGame(string host, short port)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CONNECTINFO, EventParameter.Get((int)NetMgr.ConnectInfo.connectgame));
            Debug.Log("connectGame:" + host + ":" + port);
            LogMgr.UnityLog("connectGame...");

            CoreEntry.netMgr.connect((int)NetMgr.Page.game, host, (int)(port));
            CoreEntry.netMgr.bindConnect(onConnectedGame);
            CoreEntry.netMgr.Reconnect.BindConnect();
            CoreEntry.netMgr.bindConnectClose(onConnectedCloseGame);
            CoreEntry.netMgr.bindConnectError(onConnectedError);


        }

        //移除回调
        public void RemoveConnectCB()
        {
            CoreEntry.netMgr.removeConnectCB(onConnectedGame);
            CoreEntry.netMgr.removeConnectCloseCB(onConnectedCloseGame);
            CoreEntry.netMgr.removeConnectErrorCB(onConnectedError);
        }
        //添加回调
        public void AddConnectCB()
        {
            CoreEntry.netMgr.addConnectCB(onConnectedGame);
            CoreEntry.netMgr.addConnectCloseCB(onConnectedCloseGame);
            CoreEntry.netMgr.addConnectErrorCB(onConnectedError);
        }
        //取得当前设置的Host
        public string GetNowConnectHost()
        {
            return CoreEntry.netMgr.getSettedHost();
        }
        //取得当前设置的IP
        public int GetNowConnectPort()
        {
            return CoreEntry.netMgr.getSettedPort();
        }
        //跨服连接
        public void ConnectCross(string szHost, short port)
        {
            LogMgr.UnityLog("//跨服连接...");

            IsCrossLinking = true;

            RemoveConnectCB();
            CoreEntry.netMgr.forceClose();
            CoreEntry.netMgr.connect((int)NetMgr.Page.game, szHost, (int)(port));
        }
        //跨服战结束后, 本服重新连接
        public void ConnectBackByCrossEnd(string szHost, short port)
        {
            LogMgr.UnityLog("//跨服战结束后, 本服重新连接");

            CoreEntry.netMgr.forceClose();
            CoreEntry.netMgr.connect((int)NetMgr.Page.game, szHost, (int)(port));

            //IsCrossLinking = false;
        }

        /// <summary>
        /// 请求静物触发
        /// </summary>
        /// <param name="serverID"></param>
        public void SendReqStaticTrigger(long serverID)
        {
            if(UISwitchScene.IS_REVIEW == 1)
            {
                UISwitchScene.Alarm += SendCallBackStaticTrigger;
                UISwitchScene.InitUITip(1, serverID,0,0,0,0);
            }
            else
            {
                SendCallBackStaticTrigger(serverID);
            }
        }
        void SendCallBackStaticTrigger(long serverID)
        {
            MsgData_cStructDef data = new MsgData_cStructDef();
            data.cID = serverID;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_STRUCT_DEF, data);
        }
        /// <summary>
        /// 返回静物触碰msg
        /// </summary>
        /// <param name="msg"></param>
        public void OnStructDefResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_STRUCT_DEFResult, EventParameter.Get(msg));
        }

        //物品获得失去提示
        public void OnItemTips(MsgData msg)
        {
            MsgData_sItemTips data = msg as MsgData_sItemTips;

            for (int i = 0; i < data.items.Count; i++)
            {
                MsgData_sItemTipsVO item = data.items[i];

                LogMgr.UnityLog(string.Format("OnItemTips type:{0} id:{1} count:{2}", item.type, item.id, item.count));
            }


            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ITEMTIPS, EventParameter.Get(msg));
        }

        /// <summary>
        /// 返回任务列表
        /// </summary>
        /// <param name="msg"></param>
        public void OnQueryQuestResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_QueryQuestResult, EventParameter.Get(msg));
        }

        /// <summary>
        /// 增加一个任务信息
        /// </summary>
        /// <param name="msg"></param>
        public void OnQuestAdd(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_QuestAdd, EventParameter.Get(msg));
        }
        /// <summary>
        /// 任务更新
        /// </summary>
        /// <param name="msg"></param>
        public void OnQuestUpdate(MsgData msg)
        {

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_QuestUpdate, EventParameter.Get(msg));
        }
        /// <summary>
        /// 返回接受任务处理结果 
        /// </summary>
        /// <param name="msg"></param>
        public void OnAcceptQuestResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_AcceptQuestResult, EventParameter.Get(msg));
        }
        /// <summary>
        /// 返回放弃任务处理结果 
        /// </summary>
        /// <param name="msg"></param>
        public void OnGiveupQuestResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_GiveupQuestResult, EventParameter.Get(msg));
        }
        /// <summary>
        /// 返回完成任务处理结果 
        /// </summary>
        /// <param name="msg"></param>
        public void OnFinishQuestResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_FinishQuestResult, EventParameter.Get(msg));
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        public void OnQuestDel(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_QuestDel, EventParameter.Get(msg));
        }

        public void OnDailyQuestStar(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_DailyQuestStar, EventParameter.Get(msg));
        }

        public void OnDailyQuestFinish(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_DailyQuestFinish, EventParameter.Get(msg));
        }
        public void OnDailyQuestResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_DailyQuestResult, EventParameter.Get(msg));
        }
        public void OnDQDraw(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_DQDraw, EventParameter.Get(msg));
        }
        public void OnAgainstQuestStar(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_AgainstQuestStar, EventParameter.Get(msg));
        }
        public void OnAgainstQuestFinish(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_AgainstQuestFinish, EventParameter.Get(msg));
        }
        public void OnAgainstQuestResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_AgainstQuestResult, EventParameter.Get(msg));
        }
        public void OnAgainstQDraw(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_AgainstQDraw, EventParameter.Get(msg));
        }
        public void OnGetAgainstQSkipReward(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_GetAgainstQSkipReward, EventParameter.Get(msg));
        }
        public void OnAgainstQuestSkipResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_AgainstQuestSkipResult, EventParameter.Get(msg));
        }
        public void OnAgainstQDrawNotice(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_AgainstQDrawNotice, EventParameter.Get(msg));
        }
        public void OnQuestRewardResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_QuestRewardResult, EventParameter.Get(msg));
        }
 
        //服务器返回:装备位升级
        public void OnEquipPosLevelUp(MsgData msg)
        {
            MsgData_sEquipPosLevelUp data = msg as MsgData_sEquipPosLevelUp;
            LogMgr.UnityLog(string.Format("OnEquipPosLevelUp  result: {0} pos:{1}  level:{2} exp :{3}", data.result, data.pos, data.level,data.exp));
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EQUIP_POSLEVELUP, EventParameter.Get(msg)); 
        }

        //服务器返回:装备位信息
        public void OnEquipPosInfo(MsgData msg)
        {
            MsgData_sEquipPosInfo data = msg as MsgData_sEquipPosInfo;

            for (int i = 0; i < data.items.Count; i++)
            {
                MsgData_sEquipData sed = data.items[i];
                LogMgr.UnityLog(string.Format("OnEquipPosInfo  pos: {0} level : {1} exp : {2}",sed.pos,sed.level,sed.exp));
            }
            
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EQUIP_POSINFO, EventParameter.Get(msg));
        }

        //返回其实玩家装备位信息
        public void OnOtherEquipPosInfo(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EQUIP_OTHER_POSINFO, EventParameter.Get(msg));
        }
        //客户端请求：装备位升级
        public void SendEquipPosLevelUp(int pos, int keeplvl, int autobuy)
        {
            MsgData_cEquipPosLevelUp data = new MsgData_cEquipPosLevelUp();
            data.pos = pos;
            data.keepLvl = keeplvl;
            data.autobuy = autobuy;
            LogMgr.UnityLog(string.Format("SendEquipPosLevelUp  pos: {0} keeplvl : {1} autobuy : {2}", pos, keeplvl, autobuy));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_EQUIPPOSLEVELUP, data);
        } 
        //客户端请求：装备位升级
        public void SendEquipPosStarLevelUp(int pos,   int autobuy)
        {
            MsgData_cEquipPosStarLevelUp data = new MsgData_cEquipPosStarLevelUp();
            data.pos = pos;
            data.autobuy = autobuy;
            LogMgr.UnityLog(string.Format("SendEquipPosLevelUp  pos: {0}  autobuy : {1}", pos, autobuy));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_EQUIPPOSSTARLEVELUP, data);
        }
        //返回添加装备附加信息 msgId:8149;
        public void OnEquipAdd(MsgData msg)
        {
            MsgData_sEquipAdd data = msg as MsgData_sEquipAdd;
            LogMgr.UnityLog(string.Format("OnEquipAdd uid:{0} strenLvl:{1} strenVal:{2} attrAddLvl:{3} groupId:{4} groupId2:{5} groupId2Bind:{6} group2Level:{7} superNum:{8} isyuangu:{9} godLevel:{10} blessLevel:{11}", data.uid, data.strenLvl, data.strenVal, data.attrAddLvl, data.groupId,
                data.groupId2, data.groupId2Bind, data.group2Level, data.superNum,
                data.isyuangu, data.godLevel, data.blessLevel));
            for (int i = 0; i < data.Superitems.Length; i++)
            {
                LogMgr.UnityLog(string.Format("Superitems i:{0} uid:{1} id:{2} val:{3} ",i,data.Superitems[i].uid,data.Superitems[i].id,data.Superitems[i].val));
            }
            for (int i = 0; i < data.NewSuperitems.Length; i++)
            {
                LogMgr.UnityLog(string.Format("NewSuperitems i:{0} id:{1} wash:{2}", i, data.NewSuperitems[i].id, data.NewSuperitems[i].wash));
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EQUIP_EQUIPADD, EventParameter.Get(msg));
        }
        //返回装备卓越信息 msgId:8239;
        public void OnEquipSuper(MsgData msg)
        {
            MsgData_sEquipSuper data = msg as MsgData_sEquipSuper;

            for (int i = 0; i < data.items.Count; i++)
            {
                LogMgr.UnityLog(string.Format("OnEquipSuper i:{0} uid:{1} superNum:{2} count:{3}",i, data.items[i].uid, data.items[i].superNum, data.items[i].items.Length));
                for (int j = 0; j < data.items[i].items.Length; j++)
                {
                    LogMgr.UnityLog(string.Format("SuperVO  i:{0} uid:{1} id:{2} val:{3}", i, data.items[i].items[j].uid, data.items[i].items[j].id, data.items[i].items[j].val));
                }
            }

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EQUIP_EQUIPSUPER, EventParameter.Get(msg));
        }
        //返回装备追加属性信息 msgId:8246;
        public void OnEquipExtra(MsgData msg)
        {
            MsgData_sEquipExtra data = msg as MsgData_sEquipExtra;
            for (int i = 0; i < data.items.Count; i++)
            {
                LogMgr.UnityLog(string.Format("OnEquipExtra i:{0} uid:{1} level:{2}", i, data.items[i].uid, data.items[i].level));
            }
            
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EQUIP_EQUIPEXTRA, EventParameter.Get(msg));
        }
        // 返回装备新卓越信息 msgId:8447;
        public void OnEquipNewSuper(MsgData msg)
        {
            MsgData_sEquipNewSuper data = msg as MsgData_sEquipNewSuper;

            for (int i = 0; i < data.items.Count; i++)
            {
                LogMgr.UnityLog(string.Format("OnEquipSuper i:{0} uid:{1}", i, data.items[i].uid));
                for (int j = 0; j < data.items[i].items.Length; j++)
                {
                    LogMgr.UnityLog(string.Format("NewSuperVO  i:{0} id:{1} wash:{2}", i, data.items[i].items[j].id, data.items[i].items[j].wash));
                }
            }
             
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EQUIP_EQUIPNEWSUPER, EventParameter.Get(msg));
        }
        //返回装备附加信息 msgId:8131;
        public void OnEquipInfo(MsgData msg)
        {
            MsgData_sEquipInfo dataNew = msg as MsgData_sEquipInfo;

            for (int i = 0; i < dataNew.items.Count; i++)
            {
                MsgData_sItemEquipVO data = dataNew.items[i];
                LogMgr.UnityLog(string.Format("OnEquipInfo uid:{0} strenLvl:{1} strenVal:{2} groupId:{3} groupId2:{4} groupId2Bind:{5} group2Level:{6} godLevel:{7} blessLevel:{8}", data.uid, data.strenLvl, data.strenVal, data.groupId,
                    data.groupId2, data.groupId2Bind, data.group2Level,
                    data.godLevel, data.blessLevel)); 
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EQUIP_EQUIPINFO, EventParameter.Get(msg));
        }

        public void OnEquipStarSelect(MsgData msg)
        {
            MsgData_sEquipPosStarSelect dataNew = msg as MsgData_sEquipPosStarSelect;
            LogMgr.UnityLog(string.Format("OnEquipStarSelect result:{0} star:{1}", dataNew.result,dataNew.star));

          CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EQUIP_STARSELECT, EventParameter.Get(msg));
        }
        public void SendReqStarSelect(int star)
        {
            MsgData_cEquipPosStarSelect data = new MsgData_cEquipPosStarSelect();
            data.star = star; 
            LogMgr.UnityLog(string.Format("SendReqStarSelect  star: {0}  ", data.star));
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_EQUIPPOSSTARSELECT, data);
        }
        //返回分解结果
        public void OnEquipDecompose(MsgData msg)
        {
            MsgData_sEquipDecompose data = msg as MsgData_sEquipDecompose;
            for (int i = 0; i < data.items.Count; i++)
            {
                LogMgr.UnityLog(string.Format("OnEquipDecompose num:{0} cid :{1}", data.items[i].num, data.items[i].cid));
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EQUIP_EQUIPDECOMPOSE, EventParameter.Get(msg));
        }

        //客户端请求分解装备
        public void SendEquipDecompose(LuaTable tbl)
        {
            List<ItemInfo> itemUidList = tbl.Cast<List<ItemInfo>>();
            List<MsgData_cEquipData> items = new List<MsgData_cEquipData>();
            for (int i = 0; i < itemUidList.Count; i++)
            {
                MsgData_cEquipData newItem = new MsgData_cEquipData();
                newItem.guid = (long)itemUidList[i].UID;
                items.Add(newItem);
            }
            MsgData_cEquipDecompose data = new MsgData_cEquipDecompose();
            data.count = (uint)items.Count;
            data.items = items;

            for (int i = 0; i < data.items.Count; i++)
            {
                MsgData_cEquipData sed = data.items[i];
                LogMgr.UnityLog(string.Format("SendEquipDecompose  posguid :{0}", sed.guid));
            }

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_EQUIPDECOMPOSE, data);
        }
        //客户端请求分解装备
        public void SendEquipDecompose(List<long> list)
        {
            List<MsgData_cEquipData> items = new List<MsgData_cEquipData>();
            for (int i = 0; i < list.Count; i++)
            {
                MsgData_cEquipData newItem = new MsgData_cEquipData();
                newItem.guid = list[i];
                items.Add(newItem);
            }
            MsgData_cEquipDecompose data = new MsgData_cEquipDecompose();
            data.count = (uint)items.Count;
            data.items = items;

            for (int i = 0; i < data.items.Count; i++)
            {
                MsgData_cEquipData sed = data.items[i];
                LogMgr.UnityLog(string.Format("SendEquipDecompose  posguid :{0}", sed.guid));
            }

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_EQUIPDECOMPOSE, data);
        }
        public void OnDungeonQuestStateUpdate(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_DungeonQuestUpdate, EventParameter.Get(msg));
        }

        public void OnHunLingXianYuInfo(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_HunlingXianYuInfo, EventParameter.Get(msg));
        }

        public void OnResetHunLingXianYu(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ResetHunlingXianYu, EventParameter.Get(msg));
        }

        public void OnChallHunLingXianYu(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ChallHunlingXianYu, EventParameter.Get(msg));
        }

        public void OnChallHunLingXianYuResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ChallHunlingXianYuResult, EventParameter.Get(msg));
        }

        public void OnScanHunLingXianYuResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_SCanHunlingXianYuResult, EventParameter.Get(msg));
        }

        public void OnHunLingXianYuQuit(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_HunlingXianYuQuit, EventParameter.Get(msg));
        }


        public void OnHunLingXianYuGetAward(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_HunlingXianYuGetAward, EventParameter.Get(msg));
        }


        public void OnLingShouMuDiInfo(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_LingShouMuDiInfo, EventParameter.Get(msg));
        }

        public void OnResetLingShouMuDi(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ResetLingShouMuDi, EventParameter.Get(msg));
        }

        public void OnChallLingShouMuDi(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ChallLingShouMuDi, EventParameter.Get(msg));
        }

        public void OnChallLingShouMuDiResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ChallHunlingXianYuResult, EventParameter.Get(msg));
        }

        public void OnScanLingShouMuDiResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_SCanLingShouMuDiResult, EventParameter.Get(msg));
        }

        public void OnLingShouMuDiQuit(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_HunlingXianYuQuit, EventParameter.Get(msg));
        }


        public void OnLingShouMuDiGetAward(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_LingShouMuDiGetAward, EventParameter.Get(msg));
        }

        public void OnLingShouMuDiMonsterInfo(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_LingShouMuDiMonsterInfo, EventParameter.Get(msg));
        }

        public void OnHunLingXianYuMonsterInfo(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_HunlingXianYuMonsterInfo, EventParameter.Get(msg));
        }
        public void OnSkillCDList(MsgData msg)
        {
            MsgData_sSkillCDList data = msg as MsgData_sSkillCDList;
            for (int i = 0; i < data.items.Count; i++)
            {
                LogMgr.UnityLog(string.Format("OnSkillCDList skillID:{0} cdTimes:{1}", data.items[i].skillID, data.items[i].cdTime));  
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SKILL_CDLIST, EventParameter.Get(msg));
        }

        public void OnEnterDungeonResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_EnterDungeonResult, EventParameter.Get(msg));
        }
        public void OnLeaveDungeonResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_LeaveDungeonResult, EventParameter.Get(msg));
        }
        public void OnStoryEndResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_StoryEndResult, EventParameter.Get(msg));
        }
        public void OnStoryStep(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_StoryStep, EventParameter.Get(msg));
        }
        public void OnDungeonGroupUpdate(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_DungeonGroupUpdate, EventParameter.Get(msg));
        }
        public void OnDungeonCountDown(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_DungeonCountDown, EventParameter.Get(msg));
        }
        public void OnDungeonPassResult(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_DungeonPassResult, EventParameter.Get(msg));
        }

        public void OnEquipGroup(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_EquipGroup, EventParameter.Get(msg));
        }
        public void OnEquipGroupTwo(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_EquipGroupTwo, EventParameter.Get(msg));
        }

        public void OnEquipSmelt(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_EquipSmelt, EventParameter.Get(msg));
        }
        public void OnEquipGroupLvlUp(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_EquipGroupLvlUp, EventParameter.Get(msg));
        }
        public void OnEquipGroupPeel(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_EquipGroupPeel, EventParameter.Get(msg));
        }

        public void SendDeviceTokenInfo(byte[] token)
        {
            LogMgr.UnityLog("SendDeviceTokenInfo" + token.Length);
            byte[] _by = new byte[64];
            for(int i=0;i< token.Length;++i)
            {
                _by[i] = token[i];
            }
            LogMgr.UnityLog("SendDeviceTokenInfo" + _by.Length);

            MsgData_cDeviceTokenInfo data = new MsgData_cDeviceTokenInfo();
            data.m_szToken = _by;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DeviceTokenInfo, data);
        }

    }

}

