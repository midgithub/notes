using XLua;
﻿using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System;

namespace SG
{

[Hotfix]
    public class MainRole
    {


        public long serverID = 0;

        public uint firstlogintime;     // 第一次登陆游戏时间
        public int actorid;            // 角色ID
        public string name;             // 名字
        public ulong handle;            // 实体句柄
        public int sex;                 // 性别(0:男 1:女)
        public int yuanbao;             // 元宝           属性id:36
        public int money;               // 金钱           属性id:37
        public int discountstores;      // 折扣商店可购买次数 属性id:35
        public int tili;                // 体力           属性id:41
        public int tili_max;            // 体力上限     
        public int huoli;               // 活力           属性id:38
        public int huoli_max;           // 活力上限
        public int level;               // 等级           属性id:7
        public int exp;                 // 经验           属性id:32
        public int viplevel;            // VIP等级        属性id:44
        public int firstrechargecount;  // 首次充值元宝   属性id:45    
        public int totalrechargecount;  // 总充值元宝     属性id:46    
        public int campid;              // 国家id         属性id:56
        public int xiyubi;              // 西域币         属性id:39 
        public int xuezhanbi;           // 血战币         属性id:58
        public int gongchengbi;         // 攻城币         属性id:40
        public int gongchengpoint;      // 攻城积分       属性id:33
        public int juntuanbi;           // 军团币         属性id:34 
        public int guildid;             // 公会id         属性id:42
        private int m_mapid;            // 地图ID
        public int toppower;            // 最高战力
        public int reserved3;           // 保留字段3 石灵
        public int reserved4;           // 购买状态 第1位是开服基金1状态  2首充领取状态


        public int icon;                // 形象ID
        public int skyladderFame;       // 擂台声望
        public int skyladderCurrentRank;//add by Alex 20150331 我觉得当前排名应该记录在这里
        public int skyladderTopRank;    // 擂台历史最高排名
        public int skyladderLastTopRank; //

        public int huoli_times;         // 已购买活力次数
        public int tili_times;          // 已购买体力次数     
        public int Gold_times;          // 已购买金币次数  

        public uint curtime;            // 服务器当前时间，短日期时间值, 2010年1月1日0点到当前时间所经过的秒数
        public DateTime localtime;//与服务器同步的本地时间 
        public uint serveropentime;     // 开服时间

        public int heropoints;          //英雄点

        //public List<TeamData> teamData = new List<TeamData>();
        public List<int> teambossTeam = new List<int>();

        //public MsgData_sFuBenPassData m_fubenpassdata = null;

        public int gongxun = 0;             //功勋值

        public int currServerid;//当前服务器id
                                /* public ServerInfo currServerInfo = new ServerInfo();//当前服务器信息*/
                                //阵营信息
        public int faction; //
                            // 地图ID
        public int mapid
        {
            get { return m_mapid; }
            set
            {
                m_mapid = value;
            }
        }
        public DateTime ServerRelativeTime = new DateTime(2010, 1, 1, 0, 0, 0);
        public DateTime GetServerCurTime()
        {
            //if (localtime == null)        //DateTime是struct值类型，不可能为null
            //    return DateTime.Now;
            TimeSpan span = System.DateTime.Now - localtime;
            DateTime severtime = ServerRelativeTime.AddSeconds(MainRole.Instance.curtime + span.TotalSeconds);
            return severtime;
        }

        // 返回副本完成目标数量,现在使用副本grade字段来记录三个目标的完成情况,按位记录
        public static int getFubenCompleteCnt(byte grade)
        {
            int cnt = 0;
            if ((grade & 0x0001) > 0) cnt++;
            if ((grade & 0x0002) > 0) cnt++;
            if ((grade & 0x0004) > 0) cnt++;
            return cnt;
        }
        // 返回第no个目标购买状态,0开服基金
        public bool isReserved4Status(int no)
        {
            return (reserved4 & (1 << no)) > 0;
        }

        // 返回第no个目标是否完成, no:0,1,2
        public bool isCompleteTarget(byte grade, int no)
        {
            return (grade & (1 << no)) > 0;
        }
        // 计算主角下一个等级
        public int calcActorNextLevel(int level, int exp)
        {

            return 0;
        }

        // 计算卡牌下一个等级
        public int calcCardNextLevel(int level, int exp)
        {
            return 0;
        }

        // 获取指定副本通关数据，如果返回null，则表示还没通关
        //     public MsgData_sFuBenPassGrade getFubenPassData(int fbid)
        //     {
        //         foreach (MsgData_sFuBenPassGrade item in m_fubenpassdata.data)
        //         {
        //             if (item.fbid == fbid) return item;
        //         }
        //         return null;
        //     }
        private void onFubenPassList(GameEvent ge, EventParameter parameter)
        {
            //         MsgData_sFuBenPassData data = parameter.msgParameter as MsgData_sFuBenPassData;
            //         // 保存副本通关数据 临时
            //        
            //         foreach (MsgData_sFuBenPassGrade item in data.data)
            //         {
            //             MsgData_sFuBenPassGrade grade = new MsgData_sFuBenPassGrade();
            //             grade.fbid = item.fbid;
            //             grade.grade = item.grade;
            //             grade.passtimes = item.passtimes;
            //             grade.resettimes = item.resettimes;
            //             m_fubenpassdata.data.Add(grade);
            //         }
        }
        private void onFubenSynResult(GameEvent ge, EventParameter parameter)
        {
            //         MsgData_sSynFuBenResultAndLevelChange resultAndLevel = parameter.msgParameter as MsgData_sSynFuBenResultAndLevelChange;
            //         LogMgr.UnityLog(string.Format("onFubenSynResult actor old level:{0} new level:{1}",
            //             resultAndLevel.levelchange.actorlevelchange.oldlevel,resultAndLevel.levelchange.actorlevelchange.newlevel));
            // 
            // 
            //         MsgData_sSynFuBenResult data = resultAndLevel.award;
            // 
            //         // 如果此副本没有通关，则添加到通关列表中
            //         MsgData_sFuBenPassGrade item = getFubenPassData(data.fbid);
            //         if (null == item)
            //         {
            //             MsgData_sFuBenPassGrade grade = new MsgData_sFuBenPassGrade();
            //             grade.fbid = data.fbid;
            //             grade.grade = data.grade;
            //             grade.passtimes = 1;
            //             grade.resettimes = 0;
            //             m_fubenpassdata.data.Add(grade);
            //         }
            //         // 否则修改通关最高星级   
            //         else 
            //         {
            //             if (data.grade < item.grade)
            //             {
            //                 item.grade = data.grade;
            //                 item.passtimes++;
            //             }
            //         }
        }
        public void RegisterNetMsg()
        {
            //CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_ME_INFO, EventFunction);
            NetMsgClientWarpper.Instance.ResisterMsg();
            HPBarManager.Instance.RegisterGameEventListener();
            CutSceneManager.Instance.RegisterGameEventListener();
            //FlyAttrManager.Instance.RegisterGameEventListener();
            EquipDataMgr.Instance.ResisterMsg();


            //         m_fubenpassdata = new MsgData_sFuBenPassData();
            //         m_fubenpassdata.data = new List<MsgData_sFuBenPassGrade>();

            //         CoreEntry.gEventMgr.AddListener(GameEvent.GE_MSG_FUBEN_PASSLIST, onFubenPassList);
            //         CoreEntry.gEventMgr.AddListener(GameEvent.GE_MSG_FUBEN_SYNRESULT, onFubenSynResult);
            //         CoreEntry.gEventMgr.AddListener(GameEvent.GE_MSG_TEAM_INFO_INDEX, OnGetTeamInfoByIndex);
            //         CoreEntry.gEventMgr.AddListener(GameEvent.GE_MSG_TEAMBOSS_INFO, OnGetTeamBossInfo);
        }


        //void EventFunction(GameEvent ge, EventParameter parameter)
        //{
        //    switch (ge)
        //    {

        //        case GameEvent.GE_SC_ME_INFO:
        //            {
        //                MsgData_sMeInfo data = parameter.msgParameter as MsgData_sMeInfo;
        //                if (data != null)
        //                {
        //                    serverID = data.RoleID;
        //                        PlayerData.Instance.InitData(data);                   
        //                }

        //            }
        //            break;



        //        default:
        //            break;
        //    }
        //}







        //     public void onEnterScene(MsgData_sEnterScene msg)
        //     {
        //         mapid = msg.mapid;
        //         LogMgr.UnityLog("mapid:" + mapid);
        //         // 触发进入场景事件
        //         //CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_STORY_TIGGER_FightFront, null);
        //     }

        //     public void onInit(MsgData_sCreateMainActor msg)
        //     {
        //         actorid = msg.entity_data.id;
        //         name = msg.showname;
        //         handle = msg.handle;
        // 
        //         sex = (int)msg.actor_db_data.sex;
        //         money = (int)msg.actor_db_data.gold;
        //         yuanbao = (int)msg.actor_db_data.yuanbao;
        //         xiyubi = (int)msg.actor_db_data.xiyubi;
        //         discountstores = (int)msg.actor_db_data.stopfreetimes;
        //         tili = (int)msg.actor_db_data.physical;
        //         tili_max = 100;     // todo:读客户端全局配表
        //         huoli = (int)msg.actor_db_data.huoli;
        //         huoli_max = 60;     // todo:读客户端全局配表
        //         level = (int)msg.creature_data.level;
        //         exp = (int)msg.actor_db_data.exp;
        //         viplevel = (int)msg.actor_db_data.viplevel;
        //         firstrechargecount = (int)msg.actor_db_data.firstrechargecount;
        //         totalrechargecount = (int)msg.actor_db_data.totalchargecount;
        //         icon = msg.entity_data.icon;
        //         skyladderFame = msg.actor_db_data.skyFame;
        //         skyladderTopRank = msg.actor_db_data.skyTopRank;
        //         skyladderLastTopRank = msg.actor_db_data.skyTopRank;
        //         campid = (int)msg.actor_db_data.campid;
        //         xuezhanbi = (int)msg.actor_db_data.xuezhanbi;
        //         gongchengbi = (int)msg.actor_db_data.gongchengbi;
        //         gongchengpoint = (int)msg.actor_db_data.gongchengpoint;
        //         juntuanbi = (int)msg.actor_db_data.petbattleidx;
        //         guildid = (int)msg.actor_db_data.guildid;
        //         gongxun = (int)msg.actor_db_data.medal;
        // 
        //         firstlogintime = msg.actor_db_data.firstlogintime;
        //         toppower = (int)msg.actor_db_data.toppower;
        //         reserved3 = msg.actor_db_data.reserved3;
        //         reserved4 = msg.actor_db_data.reserved4;
        // 
        // 
        //         heropoints = msg.actor_db_data.heropoints;
        // 
        //         teamData.Clear();
        //         for (int i = 0; i < msg.cnt; i++)
        //         {
        //             TeamData tmData = new TeamData();
        // 
        //             TeamMember member1 = new TeamMember();
        //             member1.ID = msg.teams[i].pos1;
        // 
        //             TeamMember member2 = new TeamMember();
        //             member2.ID = msg.teams[i].pos2;
        // 
        //             TeamMember member3 = new TeamMember();
        //             member3.ID = msg.teams[i].pos3;
        // 
        //             tmData.m_TeamList.Add(member1);
        //             tmData.m_TeamList.Add(member2);
        //             tmData.m_TeamList.Add(member3);
        // 
        //             teamData.Add(tmData);
        //         }
        // 
        //         CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_MSG_MAINROLE_PROPERTY, null);
        // 
        //         Dictionary<string,string> data = new Dictionary<string,string>();
        //         data["actorId"] = actorid.ToString();
        //         data["名字"] = name;
        //         data["等级"] = level.ToString();
        //         data["vip"] = viplevel.ToString();
        //         DataAnalyser.instance.OnEvent("login", data);
        // 
        //         LogMgr.UnityLog(string.Format("=== oninit yuanbao:{0} money:{1} tili:{2} huoli:{3} level:{4} exp:{5} viplevel:{6} firstrechargecount:{7} totalrechargecount:{8} icon:{9} skyladderfame:{10} skyladdertoprank:{11} xiyubi:{12} campid:{13} guildid:{14} juntuanbi:{15}",
        //             yuanbao, money, tili, huoli, level, exp, viplevel, firstrechargecount, totalrechargecount, icon, skyladderFame, skyladderTopRank, xiyubi, campid, guildid, juntuanbi));
        //         NetLogicGame.Instance.sendTeamBossInfo();
        //     }

        void OnGetTeamBossInfo(GameEvent ge, EventParameter parameter)
        {
            //         teambossTeam.Clear();
            //         MsgData_sGetTeamBossInfo data = parameter.msgParameter as MsgData_sGetTeamBossInfo;
            //         for( int i = 0 ; i < data.tbjoins.Count ; ++i )
            //         {
            //             if( data.tbjoins[i] > 0 )
            //             {
            //                 teambossTeam.Add(data.tbjoins[i]);
            //             }
            //         }
        }

        void OnGetTeamInfoByIndex(GameEvent ge, EventParameter parameter)
        {
            //         MsgData_sGetTeamInfoByIndex data = parameter.msgParameter as MsgData_sGetTeamInfoByIndex;
            // 
            //         TeamMember member1 = new TeamMember();
            //         member1.ID = (int)data.pos1;
            //         TeamMember member2 = new TeamMember();
            //         member2.ID = (int)data.pos2;
            //         TeamMember member3 = new TeamMember();
            //         member3.ID = (int)data.pos3;
            // 
            //         teamData[(int)data.nIndex].m_TeamList[0] = member1;
            //         teamData[(int)data.nIndex].m_TeamList[1] = member2;
            //         teamData[(int)data.nIndex].m_TeamList[2] = member3;
        }

        //     public void onMainRoleProperty(MsgData_sMainactorPropertyChange msg)
        //     {
        //         int count = msg.props.items.Count;
        //         for (int i = 0; i < count; i++)
        //         {
        //             int propid = msg.props.items[i].id;
        //             int value = (int)msg.props.items[i].val;
        //             LogMgr.UnityLog(string.Format("onMainPlayerProperty propid:{0} value:{1}", propid, value));
        //             switch (propid)
        //             {
        //                 case 36:
        //                     DataAnalyser.instance.MoneyChange(value - yuanbao, "CNY", "元宝", level, actorid.ToString()); 
        //                     yuanbao = value;
        //                     break;
        //                 case 37: money = value; break;
        //                 case 41:
        //                     {
        //                         tili = value;                        
        //                         break;
        //                     }
        //                 case 38:
        //                     {
        //                         huoli = value;
        //                         break;
        //                     }
        //                 case 39: xiyubi = value; break;
        //                 case 35: discountstores = value; break;
        //                 case 7:
        //                     {
        //                         EventParameter param = EventParameter.Get();
        //                         param.intParameter = propid;
        //                         param.intParameter1 = level;    // 旧等级
        //                         param.intParameter2 = value;    // 新等级
        // 
        //                         level = value;
        //                         CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_MSG_MAINROLE_LEVELUP, param);
        //                         Configs.GlobalConfigure gc = CsvMgr.GetGlobalConfig();
        //                         if (level == gc.skyladderMinLevel)
        //                         {
        //                             NetLogicGame.Instance.sendGetTeamInfoByIndex(TeamType.TEAM_ARENA_DEFENDER);
        //                         }
        //                         break;
        //                     }
        //                 case 32: exp = value; break;
        //                 case 44: viplevel = value; break;
        //                 case 45:
        //                     {
        //                         firstrechargecount = value; break;
        //                     }
        //                 case 46: totalrechargecount = value; break;
        //                 case 53: skyladderFame = value; break;
        //                 case 54: skyladderTopRank = value; break;
        //                 case 56: campid = value; break;
        //                 case 58: xuezhanbi = value; break;
        //                 case 40: gongchengbi = value; break;
        //                 case 33: gongchengpoint = value; break;
        //                 case 34: juntuanbi = value; break;
        //                 case 42: guildid = value; break;
        //                 case 62: reserved3 = value; break;
        //                 case 63: reserved4 = value; break;
        //                 case 55: gongxun = value; break;
        // 
        //                 case (int)enPropActor.PROP_ACTOR_HEROPOINTS:
        //                     EventParameter para = EventParameter.Get();
        //                     para.intParameter = value - heropoints; 
        //                     heropoints = value;
        //                     CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_MSG_MAINROLE_PROPERTY_HEROPOINTS, para);
        // 
        //                     break;
        // 
        //             }
        //         }
        // 
        //         CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_MSG_MAINROLE_PROPERTY, null);
        // 
        //         LogMgr.UnityLog(string.Format("=== prop change yuanbao:{0} money:{1} tili:{2} huoli:{3} level:{4} exp:{5} viplevel:{6} firstrechargecount:{7} totalrechargecount:{8} xiyubi:{9}",
        //             yuanbao, money, tili, huoli, level, exp, viplevel, firstrechargecount, totalrechargecount, xiyubi));
        //     }

        public void ClearData()
        {
            //         level = 0;
            //         actorid = 0;
            //         name = "";
            //         yuanbao = 0;
            //         money= 0;
            //         tili = 0;
            //         tili_max = 0;
            //         huoli = 0;
            //         huoli_max = 0;  
            //         exp = 0;
            //         viplevel = 0;
            //         firstrechargecount = 0;  // 首次充值元宝   属性id:45    
            //         totalrechargecount = 0;  // 总充值元宝     属性id:46    
            //         campid = 0;              // 国家id         属性id:56
            // 
            //         DataAnalyser.instance.SetRoleName("");
        }
        static MainRole mInstance;

        public static MainRole Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new MainRole();
                }

                return mInstance;
            }
        }

        ///// <summary>
        ///// 获取队伍信息
        ///// </summary>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //public TeamData GetTeamByType(TeamType type)
        //{
        //    if (teamData == null || teamData.Count == 0)
        //        return null;

        //    int index = (int)type;
        //    if (index > teamData.Count)
        //        return null;

        //    return teamData[index - 1];
        //}


        public string GetActorSprite(int sex)
        {
            string strSprite = "";
            if (sex == 0)
            {
                strSprite = "ZY_tx2";
            }
            else if (sex == 1)
            {
                strSprite = "ZY_tx1";
            }
            else
            {
                strSprite = "ZY_tx3";
            }
            return strSprite;
        }

    }

}

