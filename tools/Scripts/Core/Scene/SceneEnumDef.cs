/**
* @file     : SceneEnumDef.cs
* @brief    : 常用场景枚举定义
* @details  : 
* @author   : CW
* @date     : #2017-06-17#
*/
using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
    public enum EnEntType
    {
        EnEntType_None = 0,                 
        EnEntType_Item = 1,                 //物品
        EnEntType_Monster = 2,              //怪物
        EnEntType_NPC = 3,                  //NPC
        EnEntType_Player = 4,               //玩家
        EnEntType_StaticObj = 5,            //传送阵
        EnEntType_GatherObj = 6,            //采集点
        EnEntType_Trap = 9,                 //陷阱
        EnEntType_Duke = 11,                //城主
        EnEntType_Pet = 12,                 //美人
        EnEntType_BiaoChe = 13,             //镖车
        EnEntType_Patrol = 14,              //婚车
        EntType_VirtualPlayer = 15,
        EnEntType_Max,                      //
    }

    public enum EUnitBits
    {
        UNIT_BIT_MOVING = 1,                //移动中
        UNIT_BIT_DEAD,                      //死亡
        UNIT_BIT_CASTING,                   //施法中
        UNIT_BIT_GOD,                       //无敌
        UNIT_BIT_STEALTH,                   //隐身
        UNIT_BIT_STIFF,                     //硬直
        UNIT_BIT_POISONED,                  //中毒
        UNIT_BIT_HOLD,                      //定身
        UNIT_BIT_PALSY,                     //麻痹
        UNIT_BIT_STUN,                      //眩晕
        UNIT_BIT_SILENCE,                   //沉默

        UNIT_BIT_INCOMBAT,                  //战斗
        UNIT_BIT_INBACK,                    //归位
        UNIT_BIT_CHANGE_SCENE,              //切换场景

        UNIT_BIT_FORBID_USERITEM,           //禁用物品
        UNIT_BIT_FORBID_RIDE,               //禁止骑乘
        UNIT_BIT_FORBID_RECOVER_HP,         //禁止恢复生命
        UNIT_BIT_FORBID_SLOW,               //不可减速
        UNIT_BIT_FORBID_RECOVER_SP,         //禁止恢复体力
        UNIT_BIT_CERTAINLY_CRIT,            //必定暴击
        UNIT_BIT_CERTAINLY_HIT,             //必定命中

        UNIT_BIT_IN_PK,                     //PK状态
        UNIT_BIT_IN_BATI,                   //霸体状态
        UNIT_BIT_CREATE_SCENE,              //创建场景
        UNIT_BIT_RAMPAGE,                   //狂暴状态
        UNIT_BIT_MIDNIGHT,                  //午夜PK状态
        UNIT_BIT_REBIRTH,                   //重生状态
        UNIT_BIT_BEHEADED,                  //斩杀
        UNIT_BIT_FOLLOW,                    //跟随
        UNIT_BIT_NOUSE_TILI,                //不消耗体力
    }


    public enum ESceneType
    {
        SCENE_TYPE_NONE = 0,
        SCENE_TYPE_WILD,
        SCENE_TYPE_MAJOR_CITY,
        SCENE_TYPE_DUNGEON,
        SCENE_TYPE_ACTIVITY,
        SCENE_TYPE_GUILD_ACTIVITY,
        SCENE_TYPE_BABEL,
        SCENE_TYPE_TIMING,
        SCENE_TYPE_EXTREMITY,			// 限时挑战
        SCENE_TYPE_BEICANGDIAN,			// 北仓殿
        SCENE_TYPE_REALM,
        SCENE_TYPE_QUEST_DUNGEON,		// 任务副本	
        SCENE_TYPE_ZHUZAIROAD,			// 主宰之路
        SCENE_TYPE_LINGSHOUMD,			// 灵兽墓地
        SCENE_TYPE_WATER_DUNGEON,		// 流水副本	
        SCENE_TYPE_ADVENTURE,			// 奇遇副本
        SCENE_TYPE_CROSS_PVP,			// 跨服PVP
        SCENE_TYPE_ZHUANSHENG,			// 转生
        SCENE_TYPE_QINGMEN,
        SCENE_TYPE_GUILD_HELL,			// 地宫炼狱
        SCENE_TYPE_PERSONBOSS,          // 个人boss			
        SCENE_TYPE_RIDEBATTLEFB,		// 骑战副本；	
        SCENE_TYPE_SECERETFB,			// 深渊魔域(秘境副本)
        SCENE_TYPE_FORTUITOUS,			//大唐奇遇
        SCENE_TYPE_GUILDTEAM,			//帮派组队活动
        SCENE_TYPE_GUILD_WAR_ACTIVITY,	//帮派战活动		
        SCENE_TYPE_CROSS_BOSS,			// 跨服BOSS
        SCENE_TYPE_CROSS_WAR,			//跨服战场
        SCENE_TYPE_CROSS_TAOTAI,		//跨服淘汰	
        SCENE_TYPE_CROSS_QUEST,			//跨服任务
        SCENE_TYPE_TREASUREDULP,		//打宝秘境	
        SCENE_TYPE_HUNLINGXIANYU,		//幻灵仙域	
        SCENE_TYPE_ZHIYUANFB,			//资源副本
        SCENE_TYPE_CROSS_PRE_ARENA,		// 跨服擂台预选赛	
        SCENE_TYPE_CROSS_ARENA,			// 跨服擂台淘汰赛
        SCENE_TYPE_CROSS_TASK,			// 跨服任务
        SCENT_TYPE_CROSS_FLAG,			// 跨服夺旗
        SCENE_TYPE_CROSS_CITYWAR,		// 跨服王城战	
        SCENE_TYPE_CROSS_PVP3,			// 跨服3V3
        SCENE_TYPE_SHENWU,
        SCENE_TYPE_CROSS_TEAM_DUPL,		// 跨服组队副本	
        SCENE_TYPE_DUNGEON_QUEST,		// 任务副本	
        SCENE_TYPE_ARENA,               //竞技场副本
		SCENE_TYPE_CRWON,               //王冠副本
	    SCENE_TYPE_ELITE_BOSS,          //精英BOSS
        SCENE_TYPE_YUANZHENG,           //远征

        SCENE_TYPE_PERSONVIP_BOSS = 49,          // 个人VIPboss		
        SCENE_TYPE_CROSS_ARTIFACT = 50,      //边境神器争夺
    }

    public enum ESceneMonsterType
    {
        MONSTER_TYPE_NONE = 0,
        MONSTER_TYPE_COMMON,
        MONSTER_TYPE_HUANLINGXIANYU,
        MONSTER_TYPE_DUNGEON_QUEST,
        MONSTER_TYPE_RESOURCES_FB,
        MONSTER_TYPE_LINGSHOUMUDI,
        MONSTER_TYPE_LEVEL_FB,
        MONSTER_TYPE_EXP_FB,
        MONSTER_TYPE__PERSONAL_BOSS,
        MONSTER_TYPE_SECRECT_FB,
        MONSTER_TYPE_GOLD_BOSS,
        MONSTER_TYPE_TREASURE_FB,
        MONSTER_TYPE_ARENA,
        MONSTER_TYPE_WORLD_BOSS,
        MONSTER_TYPE_SECRECT_TEAM_FB,
        MONSTER_TYPE_CROWN_BOSS,
    }
}



