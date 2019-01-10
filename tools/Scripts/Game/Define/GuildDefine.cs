/**
* @file     : GuildDefine.cs
* @brief    : 公会定义。
* @details  : 
* @author   : XuXiang
* @date     : 2017-09-29 19:29
*/

using UnityEngine;
using System.Collections;
using XLua;

namespace SG
{
    /// <summary>
    /// 公会权限。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public static class GuildAuthority
    {
		public static int GUILD_AUTHORITY_APPOINTMENTL = 1;             /* 职务任免 */
        public static int GUILD_AUTHORITY_INVITATIONL = 2;              /* 帮派邀请 */
        public static int GUILD_AUTHORITY_INVITATION_VERIFYL = 3;       /* 申请审核 */
        public static int GUILD_AUTHORITY_EXPELL = 4;                   /* 逐出帮派 */
        public static int GUILD_AUTHORITY_MOD_NOTICEL = 5;              /* 修改公告 */
        public static int GUILD_AUTHORITY_WARL = 6;                     /* 帮派战报名 */
        public static int GUILD_AUTHORITY_REWARDL = 7;                  /* 奖励分发 */
        public static int GUILD_AUTHORITY_SKILL_LVL = 8;                    /* 帮派技能升级 */
        public static int GUILD_AUTHORITY_DISMISSL = 9;                 /* 帮派解散 */
        public static int GUILD_AUTHORITY_ACTIVITYL = 10;                    /* 开启帮派活动 */
        public static int GUILD_AUTHORITY_AUTO_VERIFYL = 11;             /* 自动同意申请 */
        public static int GUILD_AUTHORITY_LEVELUPL = 12;                 /* 帮派升级 */
        public static int GUILD_AUTHORITY_IMPEACHMENTL = 13;             /* 弹劾权限 */
        public static int GUILD_AUTHORITY_BANKAPPROVEL = 14;				/* 仓库审批 */
        public static int GUILD_AUTHORITY_BOSSL = 15;                     /* 帮派Boss活动 */
    }

    /// <summary>
    /// 公会事件。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public static class GuildEvent
    {
        public static int GUILD_EVENT_ADDMEM = 1;  // 增加公会成员   1
        public static int GUILD_EVENT_DELMEM = 2;      // 删除公会成员   2
        public static int GUILD_EVENT_CHANGE_POS = 3;  // 公会职位改变   3
        public static int GUILD_EVENT_LEVELUP = 4;     // 公会升级       4
        public static int GUILD_EVENT_LEARN_SKILL = 5; // 公会技能升级   5
        public static int GUILD_EVENT_CONTRIBUTE = 6;  // 公会捐献       6
        public static int GUILD_EVENT_KICK = 7;        // 公会踢人       7
    }

    /// <summary>
    /// 公会职位。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public static class GuildPos
    {
        public static int GUILD_POS_MEM = 1;                    /* 帮众 */
        public static int GUILD_POS_ELITE = 2;              /* 精英 */
        public static int GUILD_POS_ELDER = 3;              /* 长老 */
        public static int GUILD_POS_DEPUTE_MASTER = 4;      /* 副帮主 */
        public static int GUILD_POS_MASTER = 5;				/* 帮主 */
        public static int GUILD_POS_All = 6;					/* 所有帮会成员 */
    }
}

