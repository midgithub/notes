/**
* @file     : PKDefine.cs
* @brief    : PK相关定义
* @details  : 
* @author   : XuXiang
* @date     : 2017-08-30 15:25
*/

using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;

namespace SG
{
    /// <summary>
    /// PK状态。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public static class PKStatus
    {
        public static int PK_STATUS_NONE = 0;       //普通
        public static int PK_STATUS_NEWBIE = 1;     //新手
        public static int PK_STATUS_RED = 2;        //红名
        public static int PK_STATUS_GRAY = 3;       //灰名
        public static int PK_STATUS_PROTECT = 4;    //保护
    }

    /// <summary>
    /// PK模式。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public static class PKMode
    {
        public static int PK_MODE_PEACE = 0;        //和平
        public static int PK_MODE_TEAM = 1;         //组队
        public static int PK_MODE_GUILD = 2;        //帮派
        public static int PK_MODE_SERVER = 3;       //本服
        public static int PK_MODE_CAMP = 4;         //阵营
        public static int PK_MODE_EVIL = 5;         //善恶
        public static int PK_MODE_ALL = 6;          //全体
        public static int PK_MODE_CUSTOM = 7;       //自定义
        public static int PK_MODE_ALLY = 8;       //盟友
    }
}

