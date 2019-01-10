using System;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using UnityEngine;
using XLua;

namespace SG
{
    //登陆阶段 消息类型（只是订阅解包类型，非具体消息处理）.
[Hotfix]
	public class registerMsgType_login
	{
		public static void init()
		{
			//CoreEntry.netMgr.registerMsgType((int)NetMsgLoginDef.LoginSystem,(int)NetMsgLoginDef.enLoginSystem.sUserLogin,typeof(MsgData_sUserLogin)); 
		}

	}
    //角色阶段 消息类型（只是订阅解包类型，非具体消息处理）..
[Hotfix]
	public class registerMsgType_actor
	{
		public static void init()
		{
            //CoreEntry.netMgr.registerMsgType((int)NetMsgLoginDef.LoginSystem, (int)NetMsgLoginDef.enLoginSystem.sActorList, typeof(MsgData_sActorList)); 
		}
		
	}

    [CSharpCallLua]
    public delegate void RegisterMsgTypeCall(LuaTable tb);

    //游戏阶段 消息类型（只是订阅解包类型，非具体消息处理）..
[Hotfix]
    public class registerMsgType_game
	{
		public static void init()
		{
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FRAME_MSG, typeof(MsgData_sFrameMsg));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_HEART_BEAT, typeof(MsgData_sHeartBeat));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FUNCTION_LIST, typeof(MsgData_sFunctionList));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FUNCTION_OPEN, typeof(MsgData_sFunctionOpen));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_LOGIN, typeof(MsgData_sLogin));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ROLEINFO, typeof(MsgData_sRoleInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CREATEROLE, typeof(MsgData_sCreateRole));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ENTERGAME, typeof(MsgData_sEnterGame)); 
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_MEINFO, typeof(MsgData_sMeInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_WORLDLEVEL,typeof(MsgData_sWorldLevel));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_MODLE_FIGHT_CHANGE, typeof(MsgData_sModleFightChange));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ENTERSCENE, typeof(MsgData_sEnterScene));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_MAINPLAYERENTERSCENE, typeof(MsgData_sMainPlayerEnterScene));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_STATECHANGED, typeof(MsgData_sStateChanged));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_PLAYER_SHOW_CHANGED, typeof(MsgData_sPlayerShowChanged));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SET_PK_RULE, typeof(MsgData_sSetPKRule));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_LEAVE_GAME, typeof(MsgData_sLeaveGame));

            //重连
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RECONNECT, typeof(MsgData_sReconnect));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_COOKIE_UPDATE, typeof(MsgData_sCookieUpdate));

            //对象死亡
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_OBJDEAD, typeof(MsgData_sObjDeadInfo));            

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_OTHERMOVETO, typeof(MsgData_sOtherMoveTo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_OTHERCHANGEDIR, typeof(MsgData_sOtherChangeDir));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_OTHERMOVESTOP, typeof(MsgData_sOtherMoveStop));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_MONSTERMOVETO, typeof(MsgData_sMonsterMoveTo)); 
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TELEPORT_RESULT, typeof(MsgData_sTeleportResult)); 
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CHANGE_POS, typeof(MsgData_sChangePos));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TELEPORT_FREE_TIME, typeof(MsgData_sTeleportFreeTime));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ROLEATTRINFO, typeof(MsgData_sRoleAttrInfoNotify));
            //CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_OBJENTERSCENE, typeof(MsgData_sSceneObjectEnterNotify));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_OBJLEAVESCENE, typeof(MsgData_sSceneObjectLeaveNotify));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_OBJDISAPPEAR, typeof(MsgData_sSceneObjectDISAPPEA));
            

            //skill
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CASTSKILL_FAILD, typeof(MsgData_sCastMagic));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CASTBEGIN, typeof(MsgData_sCastBegan));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CASTEND, typeof(MsgData_sCastEnd));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CASTEFFECT, typeof(MsgData_sCastEffect));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_KNOCKBACK, typeof(MsgData_sKnockBack));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ADDBUFF, typeof(MsgData_sAddBuffer));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_UPDATEBUFF, typeof(MsgData_sUpdateBuffer));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DELBUFF, typeof(MsgData_sDelBuffer));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ADDBUFFLIST, typeof(MsgData_sAddBufferList));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SKILL_TARGET_LIST, typeof(MsgData_sSkillTargetList));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_MOVE_EFFECT, typeof(MsgData_sCastMoveEffect));

            //聊天
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CHAT_MESSAGE, typeof(MsgData_sChat));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CHAT_PRIVATE_MESSAGE, typeof(MsgData_sPrivateChatNotice));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CHAT_SYSTEM_NOTICE, typeof(MsgData_sChatSysNotice));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_NOTICE, typeof(MsgData_sNotice));
            
            //坐骑
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RIDE_INFO, typeof(MsgData_sRideInfo));            
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CHANGE_RIDE_STATE, typeof(MsgData_sChangeRideState));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_USE_ATTR_DAN, typeof(MsgData_sUseAttrDan));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RIDE_LEVEL_UP, typeof(MsgData_sRideLvlUpInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CHANGE_RIDE, typeof(MsgData_sChangeRideId));

            //背包
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ITEM_ADD, typeof(MsgData_sItemAdd));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ITEM_DEL, typeof(MsgData_sItemDel));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ITEM_UPDATE, typeof(MsgData_sItemUpdate));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_QUERY_ITEM_RESULT, typeof(MsgData_sQueryItemResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DISCARD_ITEM_RESULT, typeof(MsgData_sDiscardItemResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_PACK_ITEM_RESULT, typeof(MsgData_sPackItemResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EXPAND_BAG_RESULT, typeof(MsgData_sExpandBagResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EXPAND_BAG_TIPS, typeof(MsgData_sExpandBagTips));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SWAP_ITEM_RESULT, typeof(MsgData_sSwapItemResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SELL_ITEM_RESULT, typeof(MsgData_sSellItemResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_USE_ITEM_RESULT, typeof(MsgData_sUseItemResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SPLIT_ITEM_RESULT, typeof(MsgData_sSplitItemResult)); 
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FAST_SELL_ITEM_RESULT, typeof(MsgData_sSellItemListResult));

            //商店
            //CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SHOPPING_RESULT, typeof(MsgData_sShoppingResult));
            //CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EXCHANGE_SHOPPING_RESULT, typeof(MsgData_sExchangeShopResult));
            //CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_BUY_BACK, typeof(MsgData_sBuyBackResult));
            //CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SHOP_HAS_BUY_LIST, typeof(MsgData_sShopHasBuyList));

            //技能
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SKILL_LIST_RESULT, typeof(MsgData_sSkillListResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SKILL_LEARN_RESULT, typeof(MsgData_sSkillLearnResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SKILL_LEVEL_UP_RESULT, typeof(MsgData_sSkillLevelUp));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SKILL_ADD, typeof(MsgData_sSkillAdd));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SKILL_REMOVE, typeof(MsgData_sSkillRemove));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_REVIVE, typeof(MsgData_sRevive));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_QueryQuestResult, typeof(MsgData_sQueryQuestResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_QuestAdd, typeof(MsgData_sQueryAdd));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_QuestDel, typeof(MsgData_sQuestDele));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_QuestUpdate, typeof(MsgData_sQueryUpdate));

            //组队
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TEAM_INFO, typeof(MsgData_sTeamInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TEAM_EXIT, typeof(MsgData_sTeamRoleExit));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TEAM_JOIN_REQUEST, typeof(MsgData_sTeamJoinRequest));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TEAM_INVITE_REQUEST, typeof(MsgData_sTeamInviteRequest));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TEAM_NEARBY_TEAM, typeof(MsgData_sTeamNearbyTeam));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TEAM_NEARBY_ROLE, typeof(MsgData_sTeamNearbyRole));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TEAM_ROLE_JOIN, typeof(MsgData_sTeamRoleJoin));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TEAM_ROLE_UPDATE, typeof(MsgData_sTeamRoleUpdate));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TEAM_ROLE_UPDATE_HPMP, typeof(MsgData_sTeamRoleUpdateInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TEAM_DUNGEON_UPDATE, typeof(MsgData_sTeamDungeonUpdate)); 
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TEAM_UPDATE_DUNGEON_PREPARE, typeof(MsgData_sUpdateTeamPreparte)); 
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TEAM_TARGET_LIST, typeof(MsgData_sTargetTeamList));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TEAM_AUTO_SETTING, typeof(MsgData_sAutoTeamSetting));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TEAM_AUTO_TEAM, typeof(MsgData_sAutoTeamResult));

            //好友
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FRIEND_RECOMMEND_LIST, typeof(MsgData_sFriendRecommendList));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FRIEND_FIND, typeof(MsgData_sFindFriendTarget));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FRIEND_ONLINE_STATUS, typeof(MsgData_sRelationOnLineStatus));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FRIEND_HAVE_REWARD, typeof(MsgData_sFriendReward));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FRIEND_GET_REWARD, typeof(MsgData_sFriendRewardGet));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FRIEND_REMOVE_RELATION, typeof(MsgData_sRemoveRelation));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FRIEND_RELATION_LIST, typeof(MsgData_sRelationList));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FRIEND_ADD_APPLY, typeof(MsgData_sAddFriendApply));

            //其他玩家信息
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_OTHER_PLAYER_INFO, typeof(MsgData_sOtherHumanInfoRet));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_OTHER_PLAYER_BASE, typeof(MsgData_sOtherHumanBSInfoRet));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_OTHER_PLAYER_DETAIL, typeof(MsgData_sOtherHumanXXInfoRet));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_OTHER_PLAYER_MOUNT, typeof(MsgData_sOtherMountInfoRet));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_OTHER_PLAYER_BODY_TOOL, typeof(MsgData_sOtherBodyTool));

            //时装
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FASHION_DRESS, typeof(MsgData_sDressFashion));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FASHION_INFO, typeof(MsgData_sFashionsInfo));

            //资源副本
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RESLEVEL_INIT, typeof(MsgData_sZhiYuanFbData));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RESLEVEL_REFRESH, typeof(MsgData_sZhiYuanFbUpDate));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RESLEVEL_CHALLENGE, typeof(MsgData_sBackZhiYuanFbChallenge));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RESLEVEL_QUIT, typeof(MsgData_sBackZhiYuanFbQuit));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RESLEVEL_INFO_, typeof(MsgData_sBackZhiYuanFbInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RESLEVEL_SWEEP, typeof(MsgData_sBackZhiYuanFbWipe));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RESLEVEL_BUY_, typeof(MsgData_sBackZhiYuanFbVigor));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RESLEVEL_REWARD_BOX, typeof(MsgData_sBackZhiYuanFbBoxReward));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RESLEVEL_END, typeof(MsgData_sBackZhiYuanFbEnd));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RESLEVEL_SWEEP_END, typeof(MsgData_sBackZhiYuanFbMopupEnd));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RESLEVEL_FIRST_CHALLENGE, typeof(MsgData_sZhiYuanFbFirstChallenge));

            //帮派
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_LIST, typeof(MsgData_sGuildList));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_INFO, typeof(MsgData_sQueryMyGuildInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_MEMBER_LIST, typeof(MsgData_sQueryMyGuildMems));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_EVENT_LIST, typeof(MsgData_sQueryMyGuildEvent));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_CREATE, typeof(MsgData_sCreateGuildRet));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_UPDATE, typeof(MsgData_sUpdateGuildInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_UPDATE_MASTER, typeof(MsgData_sUpdateGuildMasterName));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_UPDATE_NOTICE, typeof(MsgData_sUpdateGuildNotice));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_BE_INVITED, typeof(MsgData_sNotifyBeInvitedGuild));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_OTHER_INFO, typeof(MsgData_sQueryOtherGuildInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_APPLY_LIST, typeof(MsgData_sQueryMyGuildApplys));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_APPLY, typeof(MsgData_sApplyGuild));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_APPLY_NUM, typeof(MsgData_sGuildReplyCountTip));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_VERIFY_APPLY, typeof(MsgData_sVerifyGuildApply));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_QUIT, typeof(MsgData_sQuitGuild));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_DISMISS, typeof(MsgData_sDismissGuild));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_LEVEL_UP, typeof(MsgData_sLevelUpGuild));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_LEVEL_UP_SKILL, typeof(MsgData_sLvUpGuildSkill));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_CHANGE_POS, typeof(MsgData_sChangeGuildPos));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_KICK_MEMBER, typeof(MsgData_sKickGuildMemeber));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_CHANGE_LEADER, typeof(MsgData_sChangeLeader));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_CONTRIBUTE, typeof(MsgData_sGuildContribute));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_LEVEL_UP_MY_SKILL, typeof(MsgData_sLevelUpMyGuildSkill));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_AID_INFO, typeof(MsgData_sBackAidInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_AID_BAP, typeof(MsgData_sBackAidBapInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_AID_LEVEL_UP, typeof(MsgData_sBackAidUpLevelInfo)); 
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_UPDATE_MY_INFO, typeof(MsgData_sUpdateMyGuildMemInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_PRAY_INFO, typeof(MsgData_sGetUnionPray));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_PRAY, typeof(MsgData_sUnionPray));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_CREATE_ALLIANCE, typeof(MsgData_sCreateAlliance));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_DISMISS_ALLIANCE, typeof(MsgData_sDismissGuildAlliance));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_ALLIANCE_APPLY_LIST, typeof(MsgData_sQueryGuildAllianceApplys));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_ALLIANCE_INFO, typeof(MsgData_sQueryAllianceGuildInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_ALLIANCE_VERIFY_APPLY, typeof(MsgData_sGuildAllianceVerify));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_MILITARY, typeof(MsgData_sGuildMilitary));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GUILD_REWARD, typeof(MsgData_sGuildReward));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DailyQuestStar, typeof(MsgData_sDailyQuestStar));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DailyQuestFinish, typeof(MsgData_sDailyQuestFinish));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DailyQuestResult, typeof(MsgData_sDailyQuestResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DQDraw, typeof(MsgData_sDailyDraw));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_AgainstQuestStar, typeof(MsgData_sAgainstQuestStar));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_AgainstQuestFinish, typeof(MsgData_sAgainstQuestFinish));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_AgainstQuestResult, typeof(MsgData_sAgainstQuestResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_AgainstQDraw, typeof(MsgData_sAgainstQuestDraw));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GetAgainstQSkipReward, typeof(MsgData_sAgainstQSkipReward));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_AgainstQuestSkipResult, typeof(MsgData_sAgainstQuestResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_AgainstQDrawNotice, typeof(MsgData_sAgainstQDrawNotice));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_QuestRewardResult, typeof(MsgData_sQuestRewardResult));

            //CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_AcceptQuestResult, typeof(MsgData_sAcceptQuestResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GiveupQuestResult, typeof(MsgData_sGiveupQuestResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FinishQuestResult, typeof(MsgData_sFinishQuestResult));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EQUIPPOSLEVELUP, typeof(MsgData_sEquipPosLevelUp));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EQUIPPOSINFO, typeof(MsgData_sEquipPosInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_OTHEREQUIPPOS, typeof(MsgData_sOtherEquipPos));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EQUIPDECOMPOSE, typeof(MsgData_sEquipDecompose));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EQUIPPOSSTARSELECT, typeof(MsgData_sEquipPosStarSelect));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EQUIPADD, typeof(MsgData_sEquipAdd));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EQUIPSUPER, typeof(MsgData_sEquipSuper));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EQUIPEXTRA, typeof(MsgData_sEquipExtra));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EQUIPNEWSUPER, typeof(MsgData_sEquipNewSuper));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EQUIPINFO, typeof(MsgData_sEquipInfo));


            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_STRUCT_DEF, typeof(MsgData_sStructDefResult));

            //拾取
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_PICKUPITEM, typeof(MsgData_sPickUpItem));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ITEMTIPS, typeof(MsgData_sItemTips));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_HunLingXianYuInfo, typeof(MsgData_sHunLingXianYuInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ResetHunLingXianYu, typeof(MsgData_sResetHunLingXianYu));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ChallHunLingXianYu, typeof(MsgData_sChallHunLingXianYu));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ChallHunLingXianYuResult, typeof(MsgData_sChallHunLingXianYuResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ScanHunLingXianYuResult, typeof(MsgData_sCanHunLingXianYuResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_HunLingXianYuQuit, typeof(MsgData_sHunLingXianYuQuit));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_HunLingXianYuGetAward, typeof(MsgData_sHunLingXianYuGetAward));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_HunLingXianYuMonsterInfo, typeof(MsgData_sHunLingXianYuMonsterInfo));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_LingShouMuDiInfo, typeof(MsgData_sLingShouMuDiInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ResetLingShouMuDi, typeof(MsgData_sResetLingShouMuDi));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ChallLingShouMuDi, typeof(MsgData_sChallLingShouMuDi));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ChallLingShouMuDiResult, typeof(MsgData_sChallLingShouMuDiResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ScanLingShouMuDiResult, typeof(MsgData_sCanLingShouMuDiResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_LingShouMuDiQuit, typeof(MsgData_sLingShouMuDiQuit));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_LingShouMuDiGetAward, typeof(MsgData_sLingShouMuDiGetAward));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_LingShouMuDiMonsterInfo, typeof(MsgData_sLingShouMuDiMonsterInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SKILL_CDLIST, typeof(MsgData_sSkillCDList));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_MAGICCOOLDOW, typeof(MsgData_sCooldown));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CASTPREPBEGAN, typeof(MsgData_sCastPrepBegan));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CASTPREPENDED, typeof(MsgData_sCastPrepEnd));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CASTCHANBEGAN, typeof(MsgData_sCastChanBegan));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CASTCHANENDED, typeof(MsgData_sCastChanEnd));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_INTERRUPTCAST, typeof(MsgData_sInterruptCast));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DianfengInfo, typeof(MsgData_sDianfengInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DianfengSave, typeof(MsgData_sDianfengSave));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DianfengReset, typeof(MsgData_sDianfengReset));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_WingInfo, typeof(MsgData_sWingInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_UseSkin, typeof(MsgData_sUseSkin));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_WingHeCheng, typeof(MsgData_sWingHeCheng));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_UseWingOrginal, typeof(MsgData_sUseWingOrginal));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ActivateSkin, typeof(MsgData_sActivateSkin));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_WinAndSkinInfo, typeof(MsgData_sWinAndSkinInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_WingExtActive, typeof(MsgData_sWingExtActive));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_WingExtActiveInfo, typeof(MsgData_sWingExtActiveInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_WingExtCountInfo, typeof(MsgData_sWingExtCountInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_BackAchievementInfo, typeof(MsgData_sBackAchievementInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_AchievementUpData, typeof(MsgData_sAchievementUpData));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_REQMAGICKEYDECOMPOSE, typeof(MsgData_sResMagicKeyDecompose));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_MAGICKEYINSETSKILL, typeof(MsgData_sMagicKeyInsetSkill));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_UPDATEMAGICKEYINFO, typeof(MsgData_sUpdateMagicKeyInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TRAINMAGICKEY, typeof(MsgData_sTrainMagicKey));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_MAKEMAGICKEY, typeof(MsgData_sMakeMagicKey));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RETURNMAGICKEYGODINFOS, typeof(MsgData_sReturnMagicKeyGodInfos));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RESMAIGCKEYGODINSET, typeof(MsgData_sResMaigcKeyGodInset));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_MagicKeyFeiSheng, typeof(MsgData_sMagicKeyFeiSheng));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RETURNMAGICKEYSKILLLINGWU, typeof(MsgData_sReturnMagickeySKillLingwu));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_MAGICKEYSTARLEVELUP, typeof(MsgData_sMagicKeyStarLevelUp));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_MAGICKEYWASHINFO, typeof(MsgData_sMagicKeyWashInfo));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EnterDungeonResult, typeof(MsgData_sEnterDungeonResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_LeaveDungeonResult, typeof(MsgData_sLeaveDungeonResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_StoryEndResult, typeof(MsgData_sStoryEndResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_StoryStep, typeof(MsgData_sStoryStep));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DungeonGroupUpdate, typeof(MsgData_sDungeonGroupUpdate));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DungeonCountDown, typeof(MsgData_sDungeonCountDown));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DungeonPassResult, typeof(MsgData_sDungeonPassResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DungeonQuestStateUpdate, typeof(MsgData_sDungeonQuestStateUpdate));

            //宝石
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GEMSLOTINFO, typeof(MsgData_sGemOpenInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GEMSLOTOPEN, typeof(MsgData_sReqGemSlotOpen));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GEMINSET, typeof(MsgData_sReqEquipGemInset));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GEMLEVELUP, typeof(MsgData_sEquipGemUpLevel));

            //合成分解
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ITEMCOMPOSE, typeof(MsgData_sItemCompose));

            //合体
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CHANGEBEGIN, typeof(MsgData_sChangeBegin));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CHANGEEND, typeof(MsgData_sChangeEnd));

            //红颜
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_HONGYANACT, typeof(MsgData_sHongyanAct));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_HONGYANFIGHT, typeof(MsgData_sHongyanFight));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_BeautyWomanLevelUp, typeof(MsgData_sBeautyWomanLevelUp));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_BeautyWomanInfo, typeof(MsgData_sBeautyWomanInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_BeautyWomanUseAtt, typeof(MsgData_sBeautyWomanUseAtt));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.s_BeautyWomanFightingUpdate, typeof(MsgData_sBeautyWomanFightingUpdate));
            
            //神兵
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_MagicWeaponInfo, typeof(MsgData_sMagicWeaponInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_MagicWeaponProficiency, typeof(MsgData_sMagicWeaponProficiency));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_MagicWeaponLevelUp, typeof(MsgData_sMagicWeaponLevelUp));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_MagicWeaponChangeModel, typeof(MsgData_sMagicWeaponChangeModel));

            //圣盾(披风)
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_PiFengInfo, typeof(MsgData_sPiFengInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_PiFengLevelUp, typeof(MsgData_sPiFengLevelUp));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_PiFengChangeModel, typeof(MsgData_sPiFengChangeModel));

            //等级副本
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DominateRouteData, typeof(MsgData_sDominateRouteData));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DominateRouteUpDate, typeof(MsgData_sDominateRouteUpDate));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_BackDominateRouteChallenge, typeof(MsgData_sBackDominateRouteChallenge));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_BackDominateRouteQuit, typeof(MsgData_sBackDominateRouteQuit));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_BackDominateRouteInfo, typeof(MsgData_sBackDominateRouteInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_BackDominateRouteWipe, typeof(MsgData_sBackDominateRouteWipe));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_BackDominateRouteVigor, typeof(MsgData_sBackDominateRouteVigor));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_BackDominateRouteBoxReward, typeof(MsgData_sBackDominateRouteBoxReward));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_BackDominateRouteEnd, typeof(MsgData_sBackDominateRouteEnd));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_BackDominateRouteMopupEnd, typeof(MsgData_sBackDominateRouteMopupEnd));

            //boss挑战副本
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_PersonalBossList,typeof(MsgData_sPersonalBossList));// 服务器返回BOSS挑战列表 msgId:8560;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_BackEnterResultPersonalBoss, typeof(MsgData_sBackEnterResultPersonalBoss));// 服务器返回进入个人BOSS结果 msgId:8561;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_BackQuitPersonalBoss, typeof(MsgData_sBackQuitPersonalBoss));// 服务器:退出个人BOSS结果 msgId:8562;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_PersonalBossResult, typeof(MsgData_sPersonalBossResult));// 服务器:挑战个人BOSS结果 msgId:8563;

            //王冠
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CROWNINFO, typeof(MsgData_sResCrownInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CROWNSKILLUP, typeof(MsgData_sResCrownSkillUp));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CROWNACTIVE, typeof(MsgData_sResCrownActive));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_CROWNRESULT, typeof(MsgData_sResCrownFightResult));
            //通缉
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TongJiInfo, typeof(MsgData_sTongJiInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TongJiLvlRefreshResult, typeof(MsgData_sTongJiLvlRefreshResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_AcceptTongJiResult, typeof(MsgData_sAcceptTongJiResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FinishTongJi, typeof(MsgData_sFinishTongJi));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GetTongJiReward, typeof(MsgData_sGetTongJiReward));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GiveupTongJiResult, typeof(MsgData_sGiveupTongJiResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RefreshTongJiList, typeof(MsgData_sRefreshTongJiList));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GetTongJiBoxResult, typeof(MsgData_sGetTongJiBoxResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TongJiRefreshState,typeof(MsgData_sTongJiRefreshState));

            //邮件
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GetMailResult, typeof(MsgData_sGetMailResult));// 返回邮件列表 msgId:7032;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_OpenMailResult, typeof(MsgData_sOpenMailResult));// 返回打开邮件 msgId:7033;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GetMailItemResult, typeof(MsgData_sGetMailItemResult));// 请求领取附件返回 msgId:7034;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DelMail, typeof(MsgData_sDelMail));// 请求删除邮件返回 msgId:7035;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_NotifyMail, typeof(MsgData_sNotifyMail));// 邮件提醒 msgId:7036;

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GuanZhiInfo, typeof(MsgData_sGuanZhiInfo));// 服务器返回：官职信息 msgId：8689；
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GuanZhiLevelUp, typeof(MsgData_sGuanZhiLevelUp));// 服务器返回：官职升级 msgId：8688；
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_InterServiceContValue, typeof(MsgData_sInterServiceContValue));//返回服务器功勋 msgId：8810
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_HuoYueDu, typeof(MsgData_sHuoYueDu));//返回活跃度信息 msgId:8260;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_HuoYueReward, typeof(MsgData_sHuoYueReward));//返回获取活跃奖励结果 msgId:8262;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_HuoYueDuFinish, typeof(MsgData_sHuoYueDuFinish));//返回活跃度任务完成一次 msgId:8261;

            //竞技场
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ARENAENTER, typeof(MsgData_sResEnterArena));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ARENAINFO, typeof(MsgData_sResMeArenaInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ARENALIST, typeof(MsgData_sResArenaList));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ARENACHALLENGE, typeof(MsgData_sResArenaChallenge));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ARENAREWARD, typeof(MsgData_sResArenaReward));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ARENARECORD, typeof(MsgData_sResArenaRecord));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ARENABUYTIMES, typeof(MsgData_sResBuyArenaTimes));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ARENABUYCD, typeof(MsgData_sResBuyArenaCD));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ARENARANKCHANGE, typeof(MsgData_sResArenaRankChange));
            
            //经验副本
           CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_WaterDungeonEnterResult, typeof(MsgData_sWaterDungeonEnterResult));     // 服务器返回:进入流水副本结果 msgId:8439;
           CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_WaterDungeonExitResult, typeof(MsgData_sWaterDungeonExitResult ));     // 服务器返回:退出流水副本结果 msgId:8440;
           CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_WaterDungeonInfo, typeof(MsgData_sWaterDungeonInfo ));     // 服务器返回:流水副本信息 msgId:8434;
           CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_WaterDungeonProgress, typeof(MsgData_sWaterDungeonProgress));     // 服务器返回:流水副本进度 msgId:8436;
           CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_WaterDungeonResult, typeof(MsgData_sWaterDungeonResult));     // 服务器返回:流水副本结算 msgId:8437;


           CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_NeiGongInfo, typeof(MsgData_sNeiGongInfo)); // 服务器返回: 经脉信息 msgId:8665;
           CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_OpenNode, typeof(MsgData_sOpenNode)); // 服务器返回:冲穴结果 msgId:8666;


            //秘境
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ResSimpleSecrectDuplInfo, typeof(MsgData_sResSimpleSecrectDuplInfo)); // 返回单人秘境副本面板信息 msgId:8637;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ResEnterSimpleSecrectDupl, typeof(MsgData_sResEnterSimpleSecrectDupl)); // 返回进入单人秘境副本 msgId:8638;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SimpleSecrectDuplTrace, typeof(MsgData_sSimpleSecrectDuplTrace)); // 秘境副本追踪面饭信息 msgId:8639;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SimpleSecrectDuplCom, typeof(MsgData_sSimpleSecrectDuplCom)); // 单人秘境副本结算 msgId:8640;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ResExitSimpleSecrectDupl, typeof(MsgData_sResExitSimpleSecrectDupl)); // 返回退出单人秘境副本 msgId:8641;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_UpdateSecrectDuplTili, typeof(MsgData_sUpdateSecrectDuplTili)); // 更新组队或次数 msgId:8642;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ResJiHuoSecrectDupl, typeof(MsgData_sResJiHuoSecrectDupl)); // 返回激活结果 msgId:8643;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SecretDungeonSweep, typeof(MsgData_sSecretDungeonSweep)); // 返回:个人秘境副本扫荡 msgId:8967;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SecretDungeonSweepReward, typeof(MsgData_sSecretDungeonSweepReward)); // 返回:个人秘境副本扫荡领奖励 msgId:8968;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SecretDungeonSweepInfo, typeof(MsgData_sSecretDungeonSweepInfo)); // 返回:个人秘境副本扫荡 msgId:8969;

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_WorldBoss, typeof(MsgData_sWorldBoss)); //返回世界BOSS列表(刷新时推单个) msgId:7064
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ActivityState, typeof(MsgData_sActivityState));//返回活动状态(刷新时推单个) msgId:7065
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ActivityEnter, typeof(MsgData_sActivityEnter)); // S->C 返回:进入活动 msgId:8159
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ActivityQuit, typeof(MsgData_sActivityQuit));  // S->C 返回:退出活动 msgId:8160

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_Activity, typeof(MsgData_sActivity));  //  S->C 登录返回活动列表 msgId:8158
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ActivityFinish, typeof(MsgData_sActivityFinish));  // S->C 返回:活动结束(活动内玩家) msgId:8161
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_WorldBossDamage, typeof(MsgData_sWorldBossDamage));  //  S->C 返回玩家累计伤害 msgId:8162
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FieldBoss, typeof(MsgData_sFieldBoss));  //  返回野外Boss信息 msgId:8737;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_WorldBossHurt, typeof(MsgData_sWorldBossHurt));  // S->C 返回世界BOSS伤害信息(活动内) msgId:8163
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_WaBaoList, typeof(MsgData_sWaBaoList));  //S->C 返回Boss挖宝信息 msgId:8807
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GetWaBaoReward, typeof(MsgData_sGetWaBaoReward));  // S->C 领取对应BOSS奖励 msdId:8808
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_ZhenBaoGe, typeof(MsgData_sZhenBaoGe)); //返回珍宝阁数据，数据刷新时也返回这个 msgId：8113；

            //秘境-组队副本
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TimeDungeonRoomInfo, typeof(MsgData_sTimeDungeonRoomInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EnterDulpPrepare, typeof(MsgData_sEnterDulpPrepare));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TeamTargetData, typeof(MsgData_sTeamTargetData));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TRAPTRIGGER, typeof(MsgData_sTriggerTrap));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_QueryMonsterByPosition, typeof(MsgData_sQueryMonsterByPosition)); // 服务器返回:查询指定点有多少怪 msgId:9948

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EquipGroup, typeof(MsgData_sEquipGroup)); //服务器返回:设置装备套装 msgId:8465
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EquipGroupTwo, typeof(MsgData_sEquipGroupTwo)); //服务器返回:设置装备套装 msgId:8565
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EquipSmelt, typeof(MsgData_sEquipSmelt)); //服务器返回:装备熔炼结果 msgId:8923
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EquipGroupPeel, typeof(MsgData_sEquipGroupPeel)); //服务器返回:剥离装备套装 msgId:8558
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EquipGroupLvlUp, typeof(MsgData_sEquipGroupLvlUp));  //服务器返回:套装升级 msgId:8685

            //宝塔秘境 
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EnterTreasureDupl, typeof(MsgData_sEnterTreasureDupl)); 
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_QuitTreasureDupl, typeof(MsgData_sQuitTreasureDupl));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FindTreasureInfo, typeof(MsgData_sFindTreasureInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FindTreasureResult, typeof(MsgData_sFindTreasureResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FindTreasureCancel, typeof(MsgData_sFindTreasureCancel));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_FindTreasureCollect, typeof(MsgData_sFindTreasureCollect));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TreasureTodayAddedTimer, typeof(MsgData_sTreasureTodayAddedTimer));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TreasureRemainTime, typeof(MsgData_sTreasureRemainTime));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_TreasureUpdateBoss, typeof(MsgData_sTreasureUpdateBoss));

            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EquipInherit, typeof(MsgData_sEquipInherit)); // 返回装备传承 msgId:8150;
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EquipPeiYang, typeof(MsgData_sEquipPeiYang));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_EquipPeiYangSet, typeof(MsgData_sEquipPeiYangSet));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GuildQuestSweep, typeof(MsgData_sUnionQuestFinish)); //S->C 返回帮派任务一键完成奖励信息 msgId:8942
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_DailyActivy, typeof(MsgData_sDailyActivyList));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GETRECHARGEORDER, typeof(MsgData_sGetRechargeorder));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_GETRECHARGEORDER_DYB, typeof(MsgData_sGetRechargeorder_DYB));   //C->S 第一拨创建支付订单
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_VERIFYACCOUNT, typeof(MsgData_sVerifyAccount));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_SCENE_OBJ_HONGYAN_LEVEL, typeof(MsgData_sSceneObjHoneYanLevel));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_AddExpInfo, typeof(MsgData_sAddExpInfo));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_MoShenItemResult, typeof(MsgData_sMoShenItemResult));
            CoreEntry.netMgr.registerMsgType((Int16)NetMsgDef.S_RechargeRet, typeof(MsgData_WC_RechargeRet));

            //Lua层注册
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            LuaTable table = G.Get<LuaTable>("Net");
            RegisterMsgTypeCall fun = table.Get<RegisterMsgTypeCall>("OnRegisterMsgType");
            if (fun != null)
            {
                fun(table);
            }
        } 
	}
}

