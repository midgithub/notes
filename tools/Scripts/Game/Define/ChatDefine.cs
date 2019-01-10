/**
* @file     : ChatDefine.cs
* @brief    : 聊天相关定义。
* @details  : 
* @author   : XuXiang
* @date     : 2017-07-21 12:02
*/

using UnityEngine;
using System.Collections;
using XLua;

namespace SG
{
    /// <summary>
    /// 频道类型。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public static class ChatChannel
    {
        public static int CHAT_CHANNEL_ALL = 0;// 综合频道(客户端用)
        public static int CHAT_CHANNEL_SYSTEM = 1;//0x0001,// 系统频道
        public static int CHAT_CHANNEL_WORLD = 2;//0x0002,// 世界频道
        public static int CHAT_CHANNEL_SCENE = 3;//0x0004,// 场景频道
        public static int CHAT_CHANNEL_CAMP = 4;//0x0004,// 阵营频道
        public static int CHAT_CHANNEL_GUILD = 5;//0x0020,// 公会频道
        public static int CHAT_CHANNEL_TEAM = 6;//0x0040,// 队伍频道
        public static int CHAT_CHANNEL_HORN = 7;//0x0008,// 喇叭频道
        public static int CHAT_CHANNEL_WHISPER = 8;//0x0080,// 私聊频道
        public static int CHAT_CHANNEL_CROSS = 9;// 跨服频道
        public static int CHAT_CHANNEL_CROSS_SCENE = 101;// 跨服区域频道
        public static int CHAT_CHANNEL_CROSS_LOCAL = 102;// 跨服本服频道
    }

    /// <summary>
    /// 关系类型。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public static class RelationType
    {
        public static int RELATION_TYPE_FRIEND = 1;   // 好友
        public static int RELATION_TYPE_ENEMY = 2;    // 仇人
        public static int RELATION_TYPE_BLACK = 3;    // 黑名单
        public static int RELATION_TYPE_RECENT = 4;   // 最近联系人
        public static int RELATION_TYPE_MARRY = 5;    // 婚姻
        public static int RELATION_TYPE_MENTOR = 6;   // 师徒
    };

    /// <summary>
    /// 聊天参数类型。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public static class ChatParamType
    {
        public static int CHAT_PARAM_SENDER_NAME = 0;//聊天发送者;type,roleID,roleName,teamId,guildId,guildPos,vip,lvl,icon
        public static int CHAT_PARAM_ROLE_NAME = 1;//人名;type,roleID,roleName,teamId,guildId,guildPos,vip,lvl,icon
        public static int CHAT_PARAM_ITEM = 2;//物品;type,物品id
        public static int CHAT_PARAM_EQUIP = 3;//装备;type,装备id,强化等级
        public static int CHAT_PARAM_MAP_POS = 4;//地图坐标;type,line,mapId,x,y
        public static int CHAT_PARAM_VALUE = 5;//数值;type,value
        public static int CHAT_PARAM_LINK = 6;//链接;type,脚本参数
        public static int CHAT_PARAM_DUNGEONS = 7;//副本;type,副本id
        public static int CHAT_PARAM_MONSTER = 8;//怪物;type,怪物id
        public static int CHAT_PARAM_TITLE = 9;//称号;type,称号id
        public static int CHAT_PARAM_GUILD = 10;//帮派;type,帮派id,帮派名
        public static int CHAT_PARAM_WORLD_BOSS = 11;//世界BOss;type,活动id
        public static int CHAT_PARAM_WUHUN = 12;//武魂;type,武魂id
        public static int CHAT_PARAM_SHENBING = 13;//神兵;type,神兵id
        public static int CHAT_PARAM_HORSE = 14;//坐骑;type,坐骑id
        public static int CHAT_PARAM_JINGJIE = 15;//境界;type,境界id
        public static int CHAT_PARAM_URL = 16;//链接；{type,url}
        public static int CHAT_PARAM_ACTIVITY = 17;    //活动;type,活动id
        public static int CHAT_PARAM_BAOJIA = 18;//宝甲;type,宝甲id
        public static int CHAT_PARAM_GUILD_POS = 19;//帮派职位:type,职位类型
        public static int CHAT_PARAM_SUPER = 20;//卓越属性:type,id,val1,val2
        public static int CHAT_PARAM_LINGZHEN = 22;//灵阵;type,灵阵id
        public static int CHAT_PARAM_TIMING = 23;//灵光封魔ID
        public static int CHAT_PARAM_LS_HORSE = 24;     //灵兽坐骑;type,灵兽坐骑id
        public static int CHAT_PARAM_VIP_TYPE = 25;//VIP类型;type,vip类型
        public static int CHAT_PARAM_MAP_ID = 26;//地图;type,mapID
        public static int CHAT_PARAM_NEW_GROUP = 27;//套装;type,ID
        public static int CHAT_PARAM_ZHANYIN = 28;//战印;type,ID
        public static int CHAT_PARAM_JUEXUE = 29;      //绝学;type,ID
        public static int CHAT_PARAM_RIDEWAR = 30;     //骑战;type,ID
        public static int CHAT_PARAM_EQUIP_PICK = 31;//获取高阶装备;type,type2(E_EquipDropScene),type2对应id
        public static int CHAT_PARAM_COLOR_STONE = 32;  //灵石;type,ID
        public static int CHAT_PARAM_NEIGON = 33;     // 内功; type, ID
        public static int CHAT_PARAM_SECERET_FB_TEAM = 34; //组队秘境
        public static int CHAT_PARAM_EQUIP_POS = 35; //装备位升级；type,装备位pos
        public static int CHAT_PARAM_PIFENG = 36;//披风;type,披风id
        public static int CHAT_PARAM_MAGIC_KEY = 37;//法宝;id,quailty
        public static int CHAT_PARAM_HONGYAN = 38;//红颜;type,红颜id
        public static int CHAT_PARAM_TIANGANG = 39;////天罡;type,天罡id
        public static int CHAT_PARAM_CROSS_CAMP = 40;//跨服阵营;type,阵营
        public static int CHAT_PARAM_ZHANNU = 41;//战努;type,天罡id
        public static int CHAT_PARAM_BIAOCHE = 42;//镖车;type,镖车id
        public static int CHAT_PARAM_POS = 43;//地图;type,x,y
        public static int CHAT_PARAM_FEIXIE = 44;//飞鞋;type,type,x,y
        public static int CHAT_PARAM_DATA = 45;
        public static int CHAT_PARAM_MINGLUN = 46;//命轮
        public static int CHAT_PARAM_HUNQI = 47;//魂器;type,魂器id
        public static int CHAT_PARAM_LINGMAI = 48; //五行灵脉;type,灵脉阶级
        public static int CHAT_PARAM_CROSS_FIGHT_3V3 = 49;//3v3,type,roomtype,roomid
        public static int CHAT_PARAM_SHENGQI = 50;//圣器
        public static int CHAT_PARAM_ZHUANSHENG_PROF = 51;//转生职业;type, 职业ID，转生等级
        public static int CHAT_PARAM_SHENWU = 52;      //神武;type,ID
        public static int CHAT_PARAM_CROSS_TEAM_DUPL = 53;//跨服组队副本,type,duplid
        public static int CHAT_PARAM_QILINBI = 54;//麒麟臂,type,duplid
        public static int CHAT_PARAM_JIANYU = 55;//剑域
        public static int CHAT_PARAM_TEAM = 56;//剑域
        public static int CHATPARAM_HONGYANSKILL = 57; //红颜技能
        public static int CHAT_PARAM_NOTICE = 101;//公告链接
    }
}

