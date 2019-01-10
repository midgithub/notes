using XLua;
using System;

namespace SG
{
[Hotfix]
    public class NetMsgDef
    {
        //gm command
        public const Int16 C_GMCOMMAND = 2910;		// gm命令


        public const Int16 C_LOGIN = 1001;		// 登录
        public const Int16 C_CREATEROLE = 1002;  //创建角色
        public const Int16 C_MAINPLAYERENTERSCEN = 3003;    //主角进入场景
        public const Int16 C_ENTERGAME = 2001;   //进入游戏
        public const Int16 C_LEAVE_GAME = 2906;//离开游戏，回到选择角色界面
        public const Int16 C_LOGOUT = 1903;//登出，回到登录
        public const Int16 C_LOGIN_LY = 1904;//请求登录(麟游) msgId:1904;
        public const Int16 C_LOGIN_XY = 1905;//请求登录(西游) msgId:1905;
        public const Int16 C_GETSERVERLIST = 1906;//请求最近服务器列表(西游) msgId:1906;
        public const Int16 C_GETRECHARGEORDER = 2915;//请求创建支付订单 msgId:2915;
        public const Int16 C_CREATECHARGEORDER_DYB = 2925; //（第一拨）创建支付订单 msgId:2925;
        public const Int16 S_GETRECHARGEORDER = 7913;//返回:创建订单 msgId:7913;
        public const Int16 S_RechargeRet = 7960; //订单支付通知
        public const Int16 S_GETRECHARGEORDER_DYB = 7930;  //(第一拨)返回创建订单  msgId:7930;
        public const Int16 S_VERIFYACCOUNT = 6007;//返回西游数据检验数据 msgId:6007;

        //////////////////////////////////////心跳////////////////////////////////////
        public const Int16 C_HEART_BEAT = 2136;     //请求-心跳
        public const Int16 S_HEART_BEAT = 7136;     //回复-心跳

        /////////////////////////////////////重连////////////////////////////////////
        public const Int16 C_RECONNECT = 1902;//请求重连

        public const Int16 S_RECONNECT = 6006;//返回重连消息
        public const Int16 S_COOKIE_UPDATE = 6005;//cookie更新
       ////////////////////////////////////////////////////////////////////////////////
        public const Int16 S_LOGIN = 6001;		// 登录
        public const Int16 S_ROLEINFO = 6003;    //角色信息
        public const Int16 S_CREATEROLE = 6002;  //创建角色
        public const Int16 S_ENTERGAME = 7001;   //进入游戏返回
        public const Int16 S_MEINFO = 8002;      //当前角色信息
        public const Int16 S_WORLDLEVEL = 8740;  //当前玩家世界等级
        public const Int16 S_ENTERSCENE = 8001;  //通知进入场景
        public const Int16 S_MAINPLAYERENTERSCENE = 8003;    //主角进入返回
        public const Int16 S_MODLE_FIGHT_CHANGE = 9938;    //模块战斗力变化

        public const Int16 S_LEAVE_GAME = 7906;//服务器返回离开游戏，回到选择角色界面

        public const Int16 C_CASTSKILL = 3050;    //释放技能

        public const Int16 C_BREAKSKILL = 3056;    //打断技能

        /// ///////////////
        public const Int16 S_CASTSKILL_FAILD = 8099;    //释放技能失败
        public const Int16 S_CASTBEGIN = 8050;    // 普通施法开始
        public const Int16 S_CASTEND = 8051;      // 普通施法结束
        public const Int16 S_CASTEFFECT = 8057;    //技能效果
        public const Int16 S_MAGICCOOLDOW = 8058; //法术冷却信息
        public const Int16 S_CASTPREPBEGAN = 8052; //施法蓄力开始
        public const Int16 S_CASTPREPENDED = 8053; //施法蓄力结束
        public const Int16 S_CASTCHANBEGAN = 8054; //施法引导开始
        public const Int16 S_CASTCHANENDED = 8055; //施法引导结束
        public const Int16 S_INTERRUPTCAST = 8056; //施法中断通知
        public const Int16 S_SKILL_TARGET_LIST = 9932;//技能目标列表
        public const Int16 S_MOVE_EFFECT = 8089;//服务器通知移动效果

        public const Int16 S_KNOCKBACK = 8081;    //击退效果
        public const Int16 S_ADDBUFF = 8082;    //增加BUFF效果
        public const Int16 S_UPDATEBUFF = 8083; //更新BUFF效果
        public const Int16 S_DELBUFF = 8084;    //删除BUFF效果
        public const Int16 S_ADDBUFFLIST = 8364; // 增加BUFF列表 msgId:8364

        public const Int16 S_OBJDEAD = 8075;    //目标死亡

        /////////////////////////////功能开放/////////////////////////////////////////////
        public const Int16 S_FUNCTION_OPEN = 8110;
        public const Int16 S_FUNCTION_LIST = 8111;


        ////////////////////////////////////移动消息////////////////////////////////////
        public const Int16 C_MOVETO = 3004;         //主角移动
        public const Int16 S_OTHERMOVETO = 8007;    //其它玩家移动
        public const Int16 C_CHANGEDIR = 3036;         //主角改变方向
        public const Int16 S_OTHERCHANGEDIR = 8036;     //其它玩家改变朝向
        public const Int16 C_MOVESTOP = 3006;           //主角停止移动
        public const Int16 S_OTHERMOVESTOP = 8008;      //其它玩家，怪物停止移动
        public const Int16 S_MONSTERMOVETO = 8551;      //怪物移动
        public const Int16 C_TELEPORT = 3200;      //传送
        public const Int16 S_TELEPORT_RESULT = 8200;      //传送结果
        public const Int16 S_CHANGE_POS = 8096;      //改变位置
        public const Int16 S_TELEPORT_FREE_TIME = 8412;      //免费传送次数更新

        ////////////////////////////////////对象进入和离开场景消息////////////////////////////////////
        public const Int16 S_ROLEATTRINFO = 8011;      //对象进入场景
        public const Int16 S_OBJENTERSCENE = 8012;      //对象进入场景
        public const Int16 S_OBJLEAVESCENE = 8013;      //对象离开场景
        public const Int16 S_OBJDISAPPEAR = 11193;      //对象消失

        ////////////////////////////////////状态位////////////////////////////////////
        public const Int16 S_STATECHANGED = 8085;       //服务端通知:状态位改变
        public const Int16 S_PLAYER_SHOW_CHANGED = 8027;       //服务端通知:玩家外观改变
        public const Int16 C_SET_PK_RULE = 3172;                            //发送设置PK状态
        public const Int16 S_SET_PK_RULE = 8172;                            //返回设置PK状态

        ////////////////////////////////////聊天////////////////////////////////////
        public const Int16 S_CHAT_MESSAGE = 7002;                           //收到聊天消息
        public const Int16 S_CHAT_PRIVATE_MESSAGE = 7003;                   //收到私聊消息
        public const Int16 S_CHAT_SYSTEM_NOTICE = 7005;                     //收到系统公告
        public const Int16 S_NOTICE = 7006;                                 //收到公告消息
        public const Int16 C_CHAT_SEND_MESSAGE = 2002;                      //发送聊天消息
        public const Int16 C_CHAT_SET_PRIVATE_CHAT_STATE = 2004;            //发送设置私聊状态

        ////////////////////////////////////坐骑////////////////////////////////////
        public const Int16 S_RIDE_INFO = 8117;                              //坐骑信息初始化
        public const Int16 S_CHANGE_RIDE_STATE = 8122;                      //更改坐骑状态回复
        public const Int16 S_USE_ATTR_DAN = 8120;                           //使用属性丹回复
        public const Int16 S_RIDE_LEVEL_UP = 8118;                          //坐骑升级回复
        public const Int16 S_CHANGE_RIDE = 8121;                            //更改坐骑回复
        public const Int16 C_CHANGE_RIDE_STATE = 3122;                      //发送更改坐骑状态请求
        public const Int16 C_USE_ATTR_DAN = 3120;                           //发送使用属性丹请求
        public const Int16 C_RIDE_LEVEL_UP = 3118;                          //发送坐骑升级请求
        public const Int16 C_CHANGE_RIDE = 3121;                            //发送更改坐骑请求

        ////////////////////////////////////背包////////////////////////////////////
        public const Int16 S_ITEM_ADD = 8014;                               //添加物品
        public const Int16 S_ITEM_DEL = 8015;                               //删除物品
        public const Int16 S_ITEM_UPDATE = 8016;                            //更新物品
        public const Int16 S_QUERY_ITEM_RESULT = 8019;                      //下发物品列表
        public const Int16 S_DISCARD_ITEM_RESULT = 8020;                    //丢弃物品回复
        public const Int16 S_PACK_ITEM_RESULT = 8024;                       //整理背包回复
        public const Int16 S_EXPAND_BAG_RESULT = 8025;                      //背包扩充反馈
        public const Int16 S_EXPAND_BAG_TIPS = 8098;                        //背包可扩充通知
        public const Int16 S_SWAP_ITEM_RESULT = 8021;                       //交换物品回复
        public const Int16 S_SELL_ITEM_RESULT = 8023;                       //出售物品回复
        public const Int16 S_USE_ITEM_RESULT = 8022;                        //使用物品回复
        public const Int16 S_SPLIT_ITEM_RESULT = 8026;                      //拆分物品回复
        public const Int16 S_FAST_SELL_ITEM_RESULT = 9931;                  //批量出售物品回复
        public const Int16 C_QUERY_ITEM = 3019;                             //获取物品列表
        public const Int16 C_DISCARD_ITEM = 3020;                           //丢弃物品请求
        public const Int16 C_PACK_ITEM = 3024;                              //整理背包请求
        public const Int16 C_EXPAND_BAG = 3025;                             //背包扩充请求
        public const Int16 C_SWAP_ITEM = 3021;                              //背包交换物品请求
        public const Int16 C_SELL_ITEM = 3023;                              //背包出售物品请求
        public const Int16 C_USE_ITEM = 3022;                               //背包使用物品请求
        public const Int16 C_SPLIT_ITEM = 3026;                             //背包拆分物品请求
        public const Int16 C_FAST_SELL_ITEM = 4931;                         //背包批量出售物品请求

        //////////////////////////////////////商店////////////////////////////////////
        //public const Int16 S_SHOPPING_RESULT = 8116;                        //商店购买回复
        //public const Int16 S_EXCHANGE_SHOPPING_RESULT = 8572;               //商店道具兑换回复
        //public const Int16 S_BUY_BACK = 8125;                               //回购回复
        //public const Int16 S_SHOP_HAS_BUY_LIST = 8304;                      //已购的限购物品列表推送
        //public const Int16 C_SHOPPING = 3116;                               //商店购买请求
        //public const Int16 C_EXCHANGE_SHOPPING = 3572;                      //商店道具兑换请求
        //public const Int16 C_BUY_BACK = 3125;                               //回购请求

        ////////////////////////////////////技能////////////////////////////////////
        public const Int16 S_SKILL_LIST_RESULT = 8028;                      //技能列表回复
        public const Int16 S_SKILL_LEARN_RESULT = 8029;                     //技能学习回复
        public const Int16 S_SKILL_LEVEL_UP_RESULT = 8031;                  //技能升级回复
        public const Int16 S_SKILL_ADD = 8033;                              //技能添加
        public const Int16 S_SKILL_REMOVE = 8034;                           //技能移除
        public const Int16 C_SKILL_LIST = 3028;                             //请求技能列表
        public const Int16 C_SKILL_LEARN = 3029;                            //请求技能学习
        public const Int16 C_SKILL_LEVEL_UP = 3030;                         //请求技能升级
        public const Int16 C_SKILL_FAST_LEVEL_UP = 4960;                    //请求技能一键升级
        public const Int16 S_SKILL_CDLIST = 8451;                           //登录同步技能CD     

        ////////////////////////////////////组队////////////////////////////////////
        public const Int16 S_TEAM_INFO = 7007;                              //回复-队伍信息
        public const Int16 C_TEAM_INFO = 2007;                              //请求-队伍信息
        public const Int16 C_TEAM_CREATE = 2008;                            //请求-创建队伍
        public const Int16 C_TEAM_APPLY = 2009;                             //请求-申请进入队伍
        public const Int16 C_TEAM_INVITE = 2010;                            //请求-邀请进入队伍
        public const Int16 C_TEAM_QUIT = 2011;                              //请求-退出队伍
        public const Int16 S_TEAM_EXIT = 7016;                              //通知-有人退出队伍
        public const Int16 C_TEAM_TRANSFER = 2012;                          //请求-转让队长
        public const Int16 C_TEAM_FIRE = 2013;                              //请求-开除成员
        public const Int16 C_TEAM_JOIN_APPROVE = 2017;                      //请求-入队审批
        public const Int16 S_TEAM_JOIN_REQUEST = 7017;                      //通知-有人申请入队(仅队长)
        public const Int16 C_TEAM_INVITE_APPROVE = 2018;                    //请求-入队邀请反馈
        public const Int16 S_TEAM_INVITE_REQUEST = 7018;                    //通知-入队邀请
        public const Int16 C_TEAM_SETTING = 2019;                           //请求-组队设置
        public const Int16 C_TEAM_NEARBY_TEAM = 2020;                       //请求-附近队伍信息
        public const Int16 S_TEAM_NEARBY_TEAM = 7020;                       //回复-附近队伍信息
        public const Int16 C_TEAM_NEARBY_ROLE = 2021;                       //请求-附近玩家信息
        public const Int16 S_TEAM_NEARBY_ROLE = 7021;                       //回复-附近玩家信息
        public const Int16 S_TEAM_ROLE_JOIN = 7014;                         //通知-成员加入
        public const Int16 S_TEAM_ROLE_UPDATE = 7015;                       //通知-成员更新
        public const Int16 S_TEAM_ROLE_UPDATE_HPMP = 7025;                  //通知-成员HPMP更新
        public const Int16 C_TEAM_REPLY_TEAM_DUNGEON = 2037;                //请求-回复是否同意组队副本
        public const Int16 S_TEAM_DUNGEON_UPDATE = 7037;                    //通知-组队副本成员状态更新
        public const Int16 C_TEAM_ENTER_DUNGEON_PREPARRE = 2246;            //请求-返回请求进入组队活动状态
        public const Int16 S_TEAM_UPDATE_DUNGEON_PREPARE = 7248;            //通知-更新队伍进入组队活动状态
        public const Int16 C_TEAM_SECRE_ZHAOMU = 2247;                      //请求-发布招募
        public const Int16 C_TEAM_TARGET_LIST = 2911;                       //请求-查询目标队伍
        public const Int16 S_TEAM_TARGET_LIST = 7908;                       //回复-查询目标队伍
        public const Int16 C_TEAM_SET_AUTO = 2912;                          //请求-设置队伍自动匹配状态
        public const Int16 C_TEAM_AUTO = 2913;                              //请求-自动匹配队伍
        public const Int16 S_TEAM_AUTO_SETTING = 7910;                      //通知-自动匹配状态更新
        public const Int16 S_TEAM_AUTO_TEAM = 7914;                         //通知-自动匹配队伍成功

        ////////////////////////////////////好友////////////////////////////////////
        public const Int16 C_FRIEND_ASK_RECOMMEND_LIST = 2030;              //请求-好友推荐
        public const Int16 C_FRIEND_FIND = 2025;                            //请求-好友查找
        public const Int16 C_FRIEND_ADD_RECOMMEND = 2029;                   //请求-添加推荐好友
        public const Int16 C_FRIEND_APPROVE = 2026;                         //请求-添加回复
        public const Int16 C_FRIEND_ADD_BLACK = 2027;                       //请求-添加黑名单
        public const Int16 C_FRIEND_REMOVE_RELATION = 2028;                 //请求-删除关系
        public const Int16 C_FRIEND_RELATION_CHANGE_LIST = 2031;            //请求-关系列表改变
        public const Int16 C_FRIEND_GET_REWARD = 2146;                      //请求-领取好友礼包
        public const Int16 S_FRIEND_RECOMMEND_LIST = 7030;                  //回复-好友推荐列表
        public const Int16 S_FRIEND_FIND = 7027;                            //回复-好友查找
        public const Int16 S_FRIEND_ONLINE_STATUS = 7031;                   //通知-更新在线状态
        public const Int16 S_FRIEND_HAVE_REWARD = 7145;                     //通知-可领取好友礼包
        public const Int16 S_FRIEND_GET_REWARD = 7146;                      //回复-领取好友礼包
        public const Int16 S_FRIEND_REMOVE_RELATION = 7029;                 //回复-移除关系
        public const Int16 S_FRIEND_RELATION_LIST  = 7028;                  //回复-关系列表
        public const Int16 S_FRIEND_ADD_APPLY  = 7332;                      //通知-好友添加申请

        ////////////////////////////////////其它玩家////////////////////////////////////
        public const Int16 C_OTHER_PLAYER_INFO = 3217;                      //请求-查看信息
        public const Int16 S_OTHER_PLAYER_INFO = 8217;                      //回复-查看信息
        public const Int16 S_OTHER_PLAYER_BASE = 8218;                      //回复-基本信息
        public const Int16 S_OTHER_PLAYER_DETAIL = 8219;                    //回复-详细信息
        public const Int16 S_OTHER_PLAYER_MOUNT = 8220;                     //回复-坐骑信息
        public const Int16 S_OTHER_PLAYER_BODY_TOOL = 8452;                 //回复-身上道具

        ////////////////////////////////////时装////////////////////////////////////
        public const Int16 C_FASHION_DRESS = 3226;                          //请求-穿戴时装
        public const Int16 C_FASHION_SET_STATE = 4938;                      //请求-设置时装状态
        public const Int16 S_FASHION_DRESS = 8226;                          //回复-穿戴时装
        public const Int16 S_FASHION_INFO = 8225;                           //通知-时装信息

        ////////////////////////////////////资源副本////////////////////////////////////
        public const Int16 C_RESLEVEL_INIT = 3759;                          //请求初始化信息
        public const Int16 C_RESLEVEL_CHALLENGE = 3760;                     //请求挑战
        public const Int16 C_RESLEVEL_REWARD = 3761;                        //请求领取奖励
        public const Int16 C_RESLEVEL_QUIT = 3762;                          //请求退出副本
        public const Int16 C_RESLEVEL_SWEEP = 3764;                         //请求扫荡
        public const Int16 C_RESLEVEL_BUY_ = 3765;                          //请求购买次数
        public const Int16 C_RESLEVEL_REWARD_BOX = 3766;                    //请求领取宝箱奖励
        public const Int16 C_RESLEVEL_SWEEP_ALL = 3769;                     //请求一键扫荡
        public const Int16 S_RESLEVEL_INIT = 8759;                          //返回初始化信息
        public const Int16 S_RESLEVEL_REFRESH = 8760;                       //返回刷新
        public const Int16 S_RESLEVEL_CHALLENGE = 8761;                     //返回挑战
        public const Int16 S_RESLEVEL_QUIT = 8762;                          //返回退出副本
        public const Int16 S_RESLEVEL_INFO_ = 8763;                         //返回追踪信息 ???
        public const Int16 S_RESLEVEL_SWEEP = 8764;                         //返回扫荡
        public const Int16 S_RESLEVEL_BUY_ = 8765;                          //返回购买次数
        public const Int16 S_RESLEVEL_REWARD_BOX = 8766;                    //返回领取宝箱奖励
        public const Int16 S_RESLEVEL_END = 8767;                           //返回副本结束
        public const Int16 S_RESLEVEL_SWEEP_END = 8768;                     //返回扫荡结束
        public const Int16 S_RESLEVEL_FIRST_CHALLENGE = 8769;               //返回挑战首通结果

        ////////////////////////////////////公会////////////////////////////////////
        public const Int16 C_GUILD_QUERY_LIST = 2038;                       //请求-帮派列表
        public const Int16 C_GUILD_QUERY_MY_INFO = 2039;                    //请求-帮派信息
        public const Int16 C_GUILD_QUERY_MY_MEMBER = 2040;                  //请求-帮派成员信息
        public const Int16 C_GUILD_QUERY_MY_EVENT = 2041;                   //请求-帮派事件
        public const Int16 C_GUILD_CREATE = 2042;                           //请求-创建帮派
        public const Int16 C_GUILD_QUIT = 2043;                             //请求-退出帮派
        public const Int16 C_GUILD_DISMISS = 2044;                          //请求-解散帮派
        public const Int16 C_GUILD_LEVEL_UP = 2045;                         //请求-升级帮派
        public const Int16 C_GUILD_LEVEL_UP_SKILL = 2046;                   //请求-开启帮派技能
        public const Int16 C_GUILD_CHANGE_POS = 2047;                       //请求-改变职位
        public const Int16 C_GUILD_CHANGE_NOTICE = 2048;                    //请求-改变帮派公告
        public const Int16 C_GUILD_VERIFY_APPLY = 2049;                     //请求-审批申请
        public const Int16 C_GUILD_KICK_MEMEBER = 2050;                     //请求-踢出成员
        public const Int16 C_GUILD_APPLY = 2051;                            //请求-申请加入
        public const Int16 C_GUILD_INVITE = 2052;                           //请求-邀请加入
        public const Int16 C_GUILD_INVITE_RES = 2053;                       //请求-邀请加入回复
        public const Int16 C_GUILD_SET_AUTO_VERIFY = 2055;                  //请求-设置自动申请审核
        public const Int16 C_GUILD_QUERY_OTHER_INFO = 2056;                 //请求-其他帮派信息
        public const Int16 C_GUILD_QUERY_MY_APPLYS = 2057;                  //请求-帮派申请信息
        public const Int16 C_GUILD_CHANGE_LEADER = 2058;                    //请求-禅让帮主
        public const Int16 C_GUILD_CONTRIBUTE = 2059;                       //请求-捐献
        public const Int16 C_GUILD_LEVEL_UP_MY_SKILL = 2060;                //请求-升级自身帮派技能
        public const Int16 C_GUILD_SEARCH = 2061;                           //请求-查找帮派
        public const Int16 C_GUILD_AID_INFO = 2071;                         //请求-加持属性
        public const Int16 C_GUILD_AID_BAP = 2072;                          //请求-加持洗炼
        public const Int16 C_GUILD_AID_LEVEL_UP = 2073;                     //请求-加持升级
        public const Int16 C_GUILD_AID_BAP_OP = 2069;                       //请求-洗炼操作
        public const Int16 C_GUILD_PRAY_INFO = 2139;                        //请求-祈福信息
        public const Int16 C_GUILD_PRAY = 2141;                             //请求-祈福操作
        public const Int16 C_GUILD_CREATE_ALLIANCE = 2064;                  //请求-创建帮派同盟
        public const Int16 C_GUILD_QUERY_ALLIANCE_APPLYS = 2066;            //请求-同盟申请列表
        public const Int16 C_GUILD_QUERY_ALLIANCE_INFO = 2067;              //请求-帮派同盟信息
        public const Int16 C_GUILD_ALLIANCE_VERIFY = 2068;                  //请求-审核帮派同盟申请
        public const Int16 C_GUILD_REWARD = 2956;                           //请求-工会每日福利领取奖励
        public const Int16 S_GUILD_LIST = 7038;                             //回复-帮派列表
        public const Int16 S_GUILD_INFO = 7039;                             //回复-帮派信息
        public const Int16 S_GUILD_MEMBER_LIST = 7040;                      //回复-帮派成员列表
        public const Int16 S_GUILD_EVENT_LIST = 7041;                       //回复-帮派事件列表
        public const Int16 S_GUILD_CREATE = 7042;                           //回复-创建帮派
        public const Int16 S_GUILD_UPDATE = 7043;                           //通知-更新帮派信息
        public const Int16 S_GUILD_UPDATE_MASTER = 7044;                    //通知-更新帮派帮主
        public const Int16 S_GUILD_UPDATE_NOTICE = 7045;                    //通知-更新帮派公告
        public const Int16 S_GUILD_BE_INVITED = 7046;                       //通知-帮派邀请
        public const Int16 S_GUILD_OTHER_INFO = 7047;                       //回复-其他帮派信息
        public const Int16 S_GUILD_APPLY_LIST = 7048;                       //回复-申请列表
        public const Int16 S_GUILD_APPLY = 7049;                            //回复-申请加入返回
        public const Int16 S_GUILD_APPLY_NUM = 7149;                        //通知-公会申请数量变化
        public const Int16 S_GUILD_VERIFY_APPLY = 7050;                     //回复-审批返回
        public const Int16 S_GUILD_QUIT = 7051;                             //回复-退出帮派
        public const Int16 S_GUILD_DISMISS = 7052;                          //回复-解散帮派
        public const Int16 S_GUILD_LEVEL_UP = 7053;                         //回复-升级帮派
        public const Int16 S_GUILD_LEVEL_UP_SKILL = 7054;                   //回复-开启某组帮派技能
        public const Int16 S_GUILD_CHANGE_POS = 7055;                       //回复-改变职位
        public const Int16 S_GUILD_KICK_MEMBER = 7056;                      //回复-踢出帮派成员
        public const Int16 S_GUILD_CHANGE_LEADER = 7057;                    //回复-禅让帮主
        public const Int16 S_GUILD_CONTRIBUTE = 7058;                       //回复-帮派捐献
        public const Int16 S_GUILD_LEVEL_UP_MY_SKILL = 7059;                //回复-升级自身帮派技能
        public const Int16 S_GUILD_AID_INFO = 7071;                         //回复-加持属性
        public const Int16 S_GUILD_AID_BAP = 7072;                          //回复-加持洗炼
        public const Int16 S_GUILD_AID_LEVEL_UP = 7073;                     //回复-加持升级
        public const Int16 S_GUILD_UPDATE_MY_INFO = 7074;                   //通知-更新自己的帮派信息
        public const Int16 S_GUILD_PRAY_INFO = 7139;                        //回复-祈福信息
        public const Int16 S_GUILD_PRAY = 7141;                             //回复-祈福操作
        public const Int16 S_GUILD_CREATE_ALLIANCE = 7066;                  //回复-创建帮派同盟
        public const Int16 S_GUILD_DISMISS_ALLIANCE = 7067;                 //回复-解散同盟
        public const Int16 S_GUILD_ALLIANCE_APPLY_LIST = 7068;              //回复-同盟申请列表
        public const Int16 S_GUILD_ALLIANCE_INFO = 7069;                    //回复-同盟信息
        public const Int16 S_GUILD_ALLIANCE_VERIFY_APPLY = 7070;            //回复-审核同盟返回
        public const Int16 S_GUILD_MILITARY = 7922;                         //回复-请求公会战力
        public const Int16 S_GUILD_REWARD = 7961;                           //回复-工会每日福利返回奖励领取结果
        ////////////////////////////////////任务消息//////////////////////////////////
        public const Int16 C_QueryQuest = 3059;   //客户端请求:任务列表
        public const Int16 C_QuestClick = 3061;   //点击计数(如功能指引任务)
        public const Int16 C_AcceptQuest = 3062;   //客户端请求:接受任务(接取任务成功推Update)
        public const Int16 C_GiveupQuest = 3063;   //客户端请求:放弃任务
        public const Int16 C_FinishQuest = 3064;   //客户端请求:完成任务

        public const Int16 C_InterSSQuestMyInfo = 3788; //请求我的任务信息
        public const Int16 C_InterSSQuestInfo = 3791; //请求任务信息
        public const Int16 C_InterSSQuestGet = 3792; //请求接取任务信息
        public const Int16 C_InterSSQuestDiscard = 3793; //请求放弃任务信息
        public const Int16 C_InterSSQuestGetReward = 3794; //请求领取任务奖励

        public const Int16 S_QueryQuestResult = 8059;    //服务端通知:任务列表反馈
        public const Int16 S_QuestAdd = 8060;  // 服务端通知:增加一个任务
        public const Int16 S_QuestUpdate = 8061; //服务端通知:任务更新
        //public const Int16 S_AcceptQuestResult = 8062; // 服务端通知:接受任务反馈
        public const Int16 S_GiveupQuestResult = 8063; //服务端通知:放弃任务反馈
        public const Int16 S_FinishQuestResult = 8064; //服务端通知:完成任务反馈
        public const Int16 S_QuestDel = 8065; //服务端通知:任务删除

        public const Int16 S_InterSSQuestMyInfo = 8788;   //服务端通知:返回我的任务信息
        public const Int16 S_InterSSQuestUpdata = 8789; //服务端通知:返回任务状态更新
        public const Int16 S_InterSSQuestRemove = 8790;  //服务端通知:返回任务状态删除
        public const Int16 S_InterSSQuestInfo = 8791;   //服务端通知:返回任务信息
        public const Int16 S_InterSSQuestGet = 8792;  //服务端通知:返回接去任务信息
        public const Int16 S_InterSSQuestDiscard = 8793;  //服务端通知:返回放弃任务信息
        public const Int16 S_InterSSQuestGetReward = 8794;  //服务端通知:返回领取任务奖励

        public const Int16 C_DailyQuestStar = 3152; // 日环任务升到5星
        public const Int16 C_DailyQuestFinish = 3153; //日环任务一键完成
        public const Int16 C_DailyQuestResult = 3154; // 请求日环任务结果
        public const Int16 C_DQDraw = 3155; // 日环任务抽奖
        public const Int16 C_DQDrawConfirm = 3156; // 日环任务抽奖确认
        public const Int16 S_DailyQuestStar = 8152; // 日环任务升5星结果
        public const Int16 S_DailyQuestFinish = 8153; // 返回日环任务一键完成奖励信息
        public const Int16 S_DailyQuestResult = 8154; // 返回日环任务结果
        public const Int16 S_DQDraw = 8155; // 返回日环任务抽奖

        public const Int16 C_AgainstQuestStar = 3702; // 讨伐任务升到5星
        public const Int16 C_AgainstQuestFinish = 3703; //讨伐任务一键完成
        public const Int16 C_AgainstQuestResult = 3704; // 请求讨伐任务结果
        public const Int16 C_AgainstQDraw = 3705; // 讨伐任务抽奖
        public const Int16 C_GetAgainstQSkipReward = 3706; // 讨伐跳环领奖
        public const Int16 C_AgainstQDrawConfirm = 3709; // 讨伐任务抽奖确认
        public const Int16 S_AgainstQuestStar = 8702;  // 讨伐任务升5星结果
        public const Int16 S_AgainstQuestFinish = 8703; // 返回讨伐任务一键完成奖励信息
        public const Int16 S_AgainstQuestResult = 8704; // 返回讨伐任务结果
        public const Int16 S_AgainstQDraw = 8705; // 返回讨伐任务抽奖
        public const Int16 S_GetAgainstQSkipReward = 8706; // 服务器通知：讨伐跳环领奖
        public const Int16 S_AgainstQuestSkipResult = 8707; // 服务器返回跳环结果
        public const Int16 S_AgainstQDrawNotice = 8708; // 服务器通知是否抽奖，每环结束都要发
        public const Int16 S_QuestRewardResult = 9940; // 服务端通知：任务下发的扫荡奖励


        ////////////////////////////////////复活////////////////////////////////////
        public const Int16 C_REVIVE = 3077;       //请求复活
        public const Int16 S_REVIVE = 8077;       //复活信息

        ////////////////////////////////////掉落和拾取////////////////////////////////////
        public const Int16 C_PICKUPITEM = 3076;     //请求拾取
        public const Int16 S_PICKUPITEM = 8076;     //拾取结果

        /////////////////////////////////////触发静物//////////////////////////////////
        public const Int16 C_STRUCT_DEF = 3070;     //请求触发静物
        public const Int16 S_STRUCT_DEF = 8070;     //触发静物反馈

        ////////////////////////////////////装备////////////////////////////////////
        public const Int16 C_EQUIPPOSLEVELUP = 3690;       //客户端请求：装备位升级
        public const Int16 C_EQUIPPOSSTARLEVELUP = 4956;       //客户端请求：装备位星
        public const Int16 S_EQUIPPOSLEVELUP = 8690;       //服务器返回:装备位升级
        public const Int16 S_EQUIPPOSINFO = 8691;       //服务器返回:装备位信息
        public const Int16 S_OTHEREQUIPPOS = 8692;       //返回装备位信息
        public const Int16 C_EQUIPDECOMPOSE = 3388;       //请求分解装备 msgId:3388
        public const Int16 S_EQUIPDECOMPOSE = 8388;       //返回分解结果 msgId:8388; 
        public const Int16 S_EQUIPADD = 8149;       //返回添加装备附加信息 msgId:8149;
        public const Int16 S_EQUIPSUPER = 8239;       //返回装备卓越信息 msgId:8239;
        public const Int16 S_EQUIPEXTRA = 8246;       //返回装备追加属性信息 msgId:8246;
        public const Int16 S_EQUIPNEWSUPER = 8447;       // 返回装备新卓越信息 msgId:8447; 
        public const Int16 S_EQUIPINFO = 8131;       // 返回装备附加信息 msgId:8131;
        public const Int16 C_EQUIPPOSSTARSELECT = 4986;       // 客户端请求：选择装备位星级 msgId:4986
        public const Int16 S_EQUIPPOSSTARSELECT = 11011;       // 服务器返回:选择装备位星级 11011

        public const Int16 S_ITEMTIPS = 8142;       //物品获得失去提示 msgId:8142;

        public const Int16 C_DungeonQuestEnter = 4933;  //请求进入任务副本
        public const Int16 C_DungeonQuestQuit = 4934;  //请求退出任务副本
        public const Int16 S_DungeonQuestStateUpdate = 9935;  //任务副本更新

        public const Int16 C_HunLingXianYuInfo = 3743; // 客户端请求：灵兽墓地信息
        public const Int16 C_ResetHunLingXianYu = 3744; //客户端请求：重置灵兽墓地
        public const Int16 C_ChallHunLingXianYu = 3745; // 客户端请求：挑战灵兽墓地
        public const Int16 C_ScanHunLingXianYu = 3747; //客户端请求：扫荡灵兽墓地
        public const Int16 C_HunLingXianYuQuit = 3748; // 客户端请求：退出灵兽墓地
        public const Int16 C_HunLingXianYuGetAward = 3749; //客户端请求：获得奖励

        public const Int16 S_HunLingXianYuInfo = 8743; // 服务器通知:灵兽墓地信息
        public const Int16 S_ResetHunLingXianYu = 8744; //反回重置灵兽墓地结果
        public const Int16 S_ChallHunLingXianYu = 8745; //返回进入灵兽墓地结果
        public const Int16 S_ChallHunLingXianYuResult = 8746; //反回挑战灵兽墓地结果
        public const Int16 S_ScanHunLingXianYuResult = 8747; //反回扫荡灵兽墓地结果
        public const Int16 S_HunLingXianYuQuit = 8748; //反回退出灵兽墓地结果
        public const Int16 S_HunLingXianYuGetAward = 8749; //反回获得奖励结果
        public const Int16 S_HunLingXianYuMonsterInfo = 8750; //反回灵兽墓地怪物波数信息

        public const Int16 C_LingShouMuDiInfo = 3421; // 客户端请求：灵兽墓地信息
        public const Int16 C_ResetLingShouMuDi = 3422; //客户端请求：重置灵兽墓地
        public const Int16 C_ChallLingShouMuDi = 3423; // 客户端请求：挑战灵兽墓地
        public const Int16 C_ScanLingShouMuDi = 3424; //客户端请求：扫荡灵兽墓地
        public const Int16 C_LingShouMuDiQuit = 3432; // 客户端请求：退出灵兽墓地
        public const Int16 C_LingShouMuDiGetAward = 3425; //客户端请求：获得奖励

        public const Int16 S_LingShouMuDiInfo = 8421; // 服务器通知:灵兽墓地信息
        public const Int16 S_ResetLingShouMuDi = 8423; //反回重置灵兽墓地结果
        public const Int16 S_ChallLingShouMuDi = 8422; //返回进入灵兽墓地结果
        public const Int16 S_ChallLingShouMuDiResult = 8424; //反回挑战灵兽墓地结果
        public const Int16 S_ScanLingShouMuDiResult = 8425; //反回扫荡灵兽墓地结果
        public const Int16 S_LingShouMuDiQuit = 8732; //反回退出灵兽墓地结果
        public const Int16 S_LingShouMuDiGetAward = 8426; //反回获得奖励结果
        public const Int16 S_LingShouMuDiMonsterInfo = 8650; //反回灵兽墓地怪物波数信息


        ////////////////////////////////////境界，荣誉，巅峰////////////////////////////////////
        public const Int16 C_DianfengInfo = 3646;//请求境界信息
        public const Int16 C_DianfengSave = 3644;//请求境界加点
        public const Int16 C_DianfengReset = 3645;//请求境界重置
        public const Int16 S_DianfengInfo = 8646;//返回境界信息
        public const Int16 S_DianfengSave = 8644;//返回保存加点结果
        public const Int16 S_DianfengReset = 8645;//返回重置结果

        public const Int16 C_WingHeCheng = 3464;  //客户端请求:翅膀合成 msgId:3464;
        public const Int16 C_UseWingOrginal = 3698;  //客户端请求:使用原始翅膀 msgId:3698;
        public const Int16 C_ActivateSkin = 3699;  //客户端请求：激活皮肤 msgId:3699;
        public const Int16 C_UseSkin = 3700;       //客户端请求：使用皮肤 msgId:3700;
        public const Int16 C_WingExtActive = 3978;  //客户端请求:翅膀扩展激活 msgId:3978;
        public const Int16 S_WingInfo = 8527;  //返回翅膀信息 msgId:8527;
        public const Int16 S_WingHeCheng = 8464;  //返回翅膀合成结果 msgId:8464;
        public const Int16 S_UseWingOrginal = 8698;  //使用原始翅膀 msgId:8698;
        public const Int16 S_ActivateSkin = 8699;  // S->C 激活翅膀(皮肤) msgId:8699;
        public const Int16 S_UseSkin = 8700;    // 服务器返回：使用皮肤 msgId:8700;
        public const Int16 S_WinAndSkinInfo = 8701;  // S->C 获取翅膀(皮肤)相关信息 msgId:8701;
        public const Int16 S_WingExtActive = 8978;  //S->C 返回：翅膀扩展激活 msgId:8978;
        public const Int16 S_WingExtActiveInfo = 8979; //S->C 返回 翅膀扩展激活列表 msgId:8979;
        public const Int16 S_WingExtCountInfo = 8980; //S->C 返回 翅膀扩展计数列表 msgId:8980;
        public const Int16 S_BackAchievementInfo = 8372; //S->C服务器返回：成就信息 msgId:8372;
        public const Int16 S_AchievementUpData = 8479; //S->C服务器返回:刷新成就进度 msgId:8479;

        public const Int16 C_DungeonNpcTalkEnd = 3101;     // C->S 请求剧情副本NPC对话结束 msgId:3101;
        public const Int16 C_EnterDungeon = 3102;          // C->S 请求进入剧情副本 msgId:3102;           
        public const Int16 C_LeaveDungeon = 3103;          // C->S 请求退出剧情副本 msgId:3103;
        public const Int16 C_StoryEnd = 3104;              // C->S 剧情播放完成 msgId:3104;
        public const Int16 C_DungeonGrup = 3137;           // C->S 请求副本组列表 msgId:3137;
        public const Int16 C_DungeonAbstain = 3140;        // C->S 放弃副本(副本进行中退出后倒计时,点击确定放弃) msgId:3140; 
        public const Int16 C_DungeonGetAward = 3141;       // C->S 领取奖励 msgId:3141;
        public const Int16 S_EnterDungeonResult = 8102;    // S->C 进入副本返回结果 msgId:8102
        public const Int16 S_LeaveDungeonResult = 8103;    // S->C 离开副本返回结果 msgId:8103
        public const Int16 S_StoryEndResult = 8104;        // S->C 剧情完成返回结果 msgId:8104
        public const Int16 S_StoryStep = 8105;             // S->C 副本剧情步骤 msgId:8105            
        public const Int16 S_DungeonGroupUpdate = 8137;    // S->C 副本组列表更新 msgId:8137
        public const Int16 S_DungeonCountDown = 8140;      // S->C 开始副本关闭倒计时 msgId:8140
        public const Int16 S_DungeonPassResult = 8141;     // S->C 副本过关结果 msgId:8141

        //法宝
        public const Int16 C_REQMAGICKEYDECOMPOSE = 3655;// 请求法宝分解 msgId:3655;
        public const Int16 C_MAGICKEYSTARLEVELUP = 3735; // 客户端请求：法宝升星 msgId:3735;
        public const Int16 C_MAKEMAGICKEY = 3623; // 请求法宝合成 msgId:3623;
        public const Int16 C_TRAINMAGICKEY = 3626; // 请求法宝培养 msgId:3626;
        public const Int16 C_REQUESTMAGICKEYSKILLLINGWU = 3879; // 请求法宝被动技能领悟 msgId:3879;
        public const Int16 C_REQMAIGCKEYGODINSET = 3708;// 客户端请求：法宝仙灵穿戴 msgId:3708;

        public const Int16 S_REQMAGICKEYDECOMPOSE = 8667;// 返回法宝分解结果 msgId:8667;
        public const Int16 S_RETURNMAGICKEYGODINFOS = 8697;// 返回法宝仙灵列表 msgId:8697;
        public const Int16 S_MAGICKEYINSETSKILL = 8736;// 返回法宝技能 msgId:8736;
        public const Int16 S_MAGICKEYWASHINFO = 8818;// 返回法宝祝福值信息 msgId:8818;
        public const Int16 S_RETURNMAGICKEYSKILLLINGWU = 8879;// 返回法宝被动技能领悟 msgId:8879;
        public const Int16 S_RETURNMAGICKEYSKILLITEMHECHENG = 8900; // 返回法宝技能书合成 msgId:8900;
        public const Int16 S_UPDATEMAGICKEYINFO = 8628;  // 更新法宝信息 msgId:8628;
        public const Int16 S_TRAINMAGICKEY = 8633;  // 返回法宝培养结果 msgId:8633;
        public const Int16 S_MAKEMAGICKEY = 8627;  // 返回法宝打造结果 msgId:8627
        public const Int16 S_RESMAIGCKEYGODINSET = 8709; //返回仙灵穿戴结果 msgId:8709;
        public const Int16 S_MAGICFIXGIVE = 8924;//返回:法宝修复提交 msgId:8924;
        public const Int16 S_MAGICFIXGIVELIST = 8925;//返回:法宝修复提交 msgId:8925;
        public const Int16 S_MagicKeyPassSkillInherit = 8964;//返回:法宝被动技能传承 msgId:8964;
        public const Int16 S_MagicKeyAwake = 8989;//返回:法宝觉醒 msgId:8989;
        public const Int16 S_MagicKeyFeiSheng = 9037;//返回:法宝飞升升经验 msgId:9037;
        public const Int16 S_MagicKeyPassSkillInfos = 9043;//服务器返回：法宝被动技能强化信息 msgId:9043;
        public const Int16 S_MagicKeyPassSkillStrength = 9044;//返回:法宝被动技能强化结果 msgId:9044;
        public const Int16 S_MagicKeyFeiShengInherit = 9061;//返回:法宝启灵传承 msgId:9061;
        public const Int16 S_MAGICKEYSTARLEVELUP = 8735;// 返回升星结果 msgId:8735;

        ////////////////////////////////////宝石////////////////////////////////////
        public const Int16 C_GEMSLOTINFO = 3738;//请求宝石孔信息
        public const Int16 C_GEMSLOTOPEN = 3739;//请求开启宝石孔
        public const Int16 C_GEMINSET = 3610;//请求宝石操作
        public const Int16 C_GEMLEVELUP = 3157;//请宝石升级

        public const Int16 S_GEMSLOTINFO = 8738;//返回宝石孔信息
        public const Int16 S_GEMSLOTOPEN = 8739;//返回宝石孔开启结果
        public const Int16 S_GEMINSET = 8611;//返回宝石操作
        public const Int16 S_GEMLEVELUP = 8157;//返回宝石升级操作

        ////////////////////////////////////合成分解////////////////////////////////////
        public const Int16 C_ITEMCOMPOSE = 3204;//请求物品合成或分解

        public const Int16 S_ITEMCOMPOSE = 8204;//返回合成或分解结果

        ////////////////////////////////////合体////////////////////////////////////
        public const Int16 S_CHANGEBEGIN = 9936;//开始变身
        public const Int16 S_CHANGEEND = 9937;//变身结束

        ////////////////////////////////////红颜////////////////////////////////////
        public const Int16 C_HONGYANACT = 3754;//请求红颜操作
        public const Int16 C_HONGYANFIGHT = 3755;//请求红颜上阵
        public const Int16 C_BeautyWomanUseAtt = 3804;//请求红颜使用属性丹



        public const Int16 S_HONGYANACT = 8756; // 服务器返回：激活返回结果 msgId:8756;
        public const Int16 S_HONGYANFIGHT = 8758;// 服务器返回：红颜出战类型变化 msgId:8758;
        public const Int16 S_BeautyWomanLevelUp = 8757;// 服务器返回：升阶返回结果 msgId:8757;
        public const Int16 S_BeautyWomanInfo = 8755;// 服务器通知:红颜信息(玩家登陆;每天更新) msgId:8755;
        public const Int16 S_BeautyWomanUseAtt = 8804;// 返回: 修为等级信息 msgId:8804;
        public const Int16 s_BeautyWomanFightingUpdate = 9941;  // S->C 服务器通知:更新红颜战斗力 msgId:9941
 
        public const Int16 C_MagicWeaponLevelUp = 3250; // C->S 请求:神兵进阶 msgId:3250;
        public const Int16 C_MagicWeaponChangeModel = 3456; //C->S 请求:神兵换模型 msgId:3456;
        public const Int16 S_MagicWeaponInfo = 8248; // S->C 服务器通知:神兵信息 msgId:8248
        public const Int16 S_MagicWeaponProficiency = 8249; //S->C 服务器通知:神兵熟练度 msgId:8249
        public const Int16 S_MagicWeaponLevelUp = 8250;  // S->C 服务器通知:神兵进阶 msgId:8250
        public const Int16 S_MagicWeaponChangeModel = 8456;  // S->C 服务器通知:神兵换模型结果 msgId:8456

        public const Int16 C_PiFengLevelUp = 3724; //请求:圣盾进阶 msgId:3724
        public const Int16 C_PiFengChangeModel = 3725; //请求:圣盾换模型 msgId:3725
        public const Int16 S_PiFengInfo = 8723;  //服务器通知:圣盾信息 msgId:8723
        public const Int16 S_PiFengLevelUp = 8724; //服务器通知:圣盾进阶 msgId:8724
        public const Int16 S_PiFengChangeModel = 8725; //服务器通知:圣盾换模型结果 msgId:8725


        //等级副本
        //Client->Server
        public const Int16 C_DominateRoute = 3390;//请求UI信息 msgId:3390;  未使用
        public const Int16 C_DominateRouteChallenge = 3392; // 请求挑战 msgId:3392;
        public const Int16 C_DominateRouteQuit =3393; // 请求退出 msgId:3393;
        public const Int16 C_DominateRouteRequestReward =  3394; // 领取宝箱 msgId:3394;
        public const Int16 C_DominateRouteWipe =  3395; // 请求扫荡 msgId:3395;

        //Server->Clinet
        public const Int16 S_DominateRouteData = 8390; // 返回UI信息 msgId:8390;
        public const Int16 S_DominateRouteUpDate = 8391; // 刷新 msgId:8391;
        public const Int16 S_BackDominateRouteChallenge = 8392; // 返回挑战 msgId:8392;
        public const Int16 S_BackDominateRouteQuit= 8393; // 返回退出 msgId:8393;
        public const Int16 S_BackDominateRouteInfo= 8394; // 返回追踪信息 msgId:8394;
        public const Int16 S_BackDominateRouteWipe= 8395; // 返回扫荡 msgId:8395;
        public const Int16 S_BackDominateRouteVigor= 8396; // 返回购买精力 msgId:8396;
        public const Int16 S_BackDominateRouteBoxReward= 8397;// 返回领取宝箱奖励 msgId:8397;
        public const Int16 S_BackDominateRouteEnd= 8398; // 返回通关 msgId:8398;
        public const Int16 S_BackDominateRouteMopupEnd= 8399; // 返回扫荡结束 msgId:8399;
        
        
        //boss挑战副本
        public const Int16  C_EnterPersonalBoss = 3561; // 客户端请求进入个人BOSS副本 msgId:3561;
        public const Int16  C_QuitPersonalBoss = 3562;// 客户端请求退出个人BOSS副本 msgId:3562;
        public const Int16  C_PersonalBossLoading = 3568;// 客户端请求结束个人副本loading msgId:3568;
        public const Int16  C_PersonBossOneKeySweep = 4021; // 请求个人BOSS一键扫荡 msgId:4021;


        public const Int16  S_PersonalBossList = 8560;// 服务器返回BOSS挑战列表 msgId:8560;
        public const Int16  S_BackEnterResultPersonalBoss = 8561;// 服务器返回进入个人BOSS结果 msgId:8561;
        public const Int16  S_BackQuitPersonalBoss = 8562;// 服务器:退出个人BOSS结果 msgId:8562;
        public const Int16  S_PersonalBossResult = 8563;// 服务器:挑战个人BOSS结果 msgId:8563;

        //王冠
        public const Int16 C_CROWNINFO = 4936;//请求王冠数据
        public const Int16 C_SKILLUP = 4937;//请求王冠升级技能
        public const Int16 C_CROWNFIGHT = 4946;//请求进入王冠副本挑战
        public const Int16 C_CROWNFIGHTLEAVE = 4947;//请求退出王冠副本挑战

        public const Int16 S_CROWNINFO = 9942;//返回王冠数据 
        public const Int16 S_CROWNSKILLUP = 9943;//返回王冠升级技能
        public const Int16 S_CROWNACTIVE = 9944;//返回激活王冠
        public const Int16 S_CROWNRESULT = 9984;//返回王冠副本

        public const Int16 S_FRAME_MSG = 9945;//每帧消息

        //通缉
        public const Int16 C_TongJiInfo = 3164;//客户端请求通缉列表 msgId:3164
        public const Int16 C_TongJiLvlRefresh = 3165;//难度刷新 msgId:3165;
        public const Int16 C_AcceptTongJi = 3166;//接受通缉任务 msgId:3166;
        public const Int16 C_GetTongJiReward = 3168;//领取通缉奖励 msgId:3168;
        public const Int16 C_GiveupTongJi = 3169;//放弃通缉 msgId:3169;
        public const Int16 C_GetTongJiBox = 3171;//获取通缉宝箱奖励 msgId:3171;
        public const Int16 C_TongJiRefreshState = 3522; //刷新悬赏状态 msgId:3522
        public const Int16 S_TongJiInfo = 8164;//服务端通知:返回通缉信息 msgId:8164;
        public const Int16 S_TongJiLvlRefreshResult = 8165;//服务器通知：返回难度刷新 msgId:8165;
        public const Int16 S_AcceptTongJiResult = 8166;//服务器通知：返回接受通缉任务 msgId:8166;
        public const Int16 S_FinishTongJi = 8167;//服务器通知：通缉活动可领奖 msgId:8167;
        public const Int16 S_GetTongJiReward = 8168;//服务器返回结果：返回领取通缉奖励结果 msgId:8168;
        public const Int16 S_GiveupTongJiResult = 8169;//服务器通知：返回放弃通缉结果 msgId:8169;
        public const Int16 S_RefreshTongJiList = 8170;//服务器通知：自动刷新通缉列表 msgId:8170;
        public const Int16 S_GetTongJiBoxResult = 8171;//服务器通知：返回获取通缉宝箱结果 msgId:8171;
        public const Int16 S_TongJiRefreshState = 8522; //服务器通知：刷新悬赏状态结果 msgId:8522

        //邮件
        //c->s
        public const Int16 C_GetMailList = 2032;// 请求获取邮件列表 msgId:2032;
        public const Int16 C_OpenMail = 2033;// 请求打开邮件 msgId:2033;
        public const Int16 C_GetMailItem = 2034;// 请求领取附件 msgId:2034;
        public const Int16 C_DelMail = 2035;// 请求删除邮件 msgId:2035;

        //s->c
        public const Int16 S_GetMailResult = 7032;// 返回邮件列表 msgId:7032;
        public const Int16 S_OpenMailResult = 7033;// 返回打开邮件 msgId:7033;
        public const Int16 S_GetMailItemResult = 7034;// 请求领取附件返回 msgId:7034;
        public const Int16 S_DelMail = 7035;// 请求删除邮件返回 msgId:7035;
        public const Int16 S_NotifyMail = 7036;// 邮件提醒 msgId:7036;

        public const Int16 C_GuanZhiInfo = 3689;  //客户端请求：官职信息 msgId：3689；
        public const Int16 C_InterServiceContValue = 3810; //请求服务器功勋 msgId：3810；
        public const Int16 C_GuanZhiLevelUp = 3688; //客户端请求：官职升级 msgId:3688；
        public const Int16 S_GuanZhiLevelUp = 8688; //服务器返回：官职升级 msgId：8688；
        public const Int16 S_GuanZhiInfo = 8689; //服务器返回：官职信息 msgId：8689；
        public const Int16 S_InterServiceContValue = 8810; //返回服务器功勋 msgId：8810
        public const Int16 C_HuoYueReward = 3262; //客户端请求：活跃奖励 msgId:3262
        public const Int16 S_HuoYueReward = 8262; //返回获取活跃奖励结果 msgId:8262;
        public const Int16 S_HuoYueDuFinish = 8261; //返回活跃度任务完成一次 msgId:8261
        public const Int16 S_HuoYueDu = 8260; //返回活跃度信息 msgId:8260

        //竞技场
        public const Int16 C_ARENAEXIT = 3192;//请求退出竞技场
        public const Int16 C_ARENAINFO = 2100;//请求当前角色竞技场信息
        public const Int16 C_ARENALIST = 2101;//请求竞技场挑战对手列表
        public const Int16 C_ARENACHALLENGE = 2102;//请求挑战
        public const Int16 C_ARENAREWARD = 2103;//请求竞技场领取奖励
        public const Int16 C_ARENARECORD = 2104;//请求竞技场战报
        public const Int16 C_ARENABUYTIMES = 2105;//请求竞技场购买次数
        public const Int16 C_ARENABUYCD = 2106;//请求竞技场购买CD

        public const Int16 S_ARENAENTER = 8192;//服务器通知进入竞技场
        public const Int16 S_ARENAINFO = 7100;//服务器返回当前角色竞技场信息
        public const Int16 S_ARENALIST = 7101;//服务器返回竞技场挑战对手列表
        public const Int16 S_ARENACHALLENGE = 7102;//服务器返回挑战结果
        public const Int16 S_ARENAREWARD = 7103;//服务器返回领取奖励
        public const Int16 S_ARENARECORD = 7104;//服务器返回竞技场战报
        public const Int16 S_ARENABUYTIMES = 7105;//服务器返回竞技场购买次数
        public const Int16 S_ARENABUYCD = 7106;//服务器返回竞技场购买CD
        public const Int16 S_ARENARANKCHANGE = 7909;//服务器返回竞技场排名变化

        //经验副本
        public const Int16 S_WaterDungeonEnterResult = 8439;     // 服务器返回:进入流水副本结果 msgId:8439;
        public const Int16 S_WaterDungeonExitResult = 8440;     // 服务器返回:退出流水副本结果 msgId:8440;
        public const Int16 S_WaterDungeonInfo = 8434;     // 服务器返回:流水副本信息 msgId:8434;
        public const Int16 S_WaterDungeonProgress = 8436;     // 服务器返回:流水副本进度 msgId:8436;
        public const Int16 S_WaterDungeonResult = 8437;     // 服务器返回:流水副本结算 msgId:8437;


        public const Int16 C_WaterDungeonEnter = 3439;     // 客户端请求：进入流水副本 msgId:3439;
        public const Int16 C_WaterDungeonExit = 3440;     // 客户端请求：退出流水副本 msgId:3440;
        public const Int16 C_WaterDungeonInfo = 3434;     // 客户端请求：流水副本信息 msgId:3434;
 
        //占星
        public const Int16 C_OpenNode = 3666; // 客户端请求：冲穴 msgId:3666;
        
        public const Int16 S_NeiGongInfo = 8665; // 服务器返回: 经脉信息 msgId:8665;
        public const Int16 S_OpenNode = 8666; // 服务器返回:冲穴结果 msgId:8666;


        //秘境
        public const Int16 C_ReqSimpleSecrectDuplInfo = 3627; // 请求单人秘境副本面板信息 msgId:3627;
        public const Int16 C_ReqEnterSimpleSecrectDupl = 3628; // 请求进入单人秘境副本 msgId:3628;
        public const Int16 C_ReqExitSimpleSecrectDupl = 3641; // 请求退出单人秘境副本 msgId:3641;
        public const Int16 C_ReqBuySecrectDuplTili = 3642; // 请求购买组队或次数 msgId:3642;
        public const Int16 C_ReqJiHuoSecrectDupl = 3643; // 请求激活 msgId:3643;
        public const Int16 C_SecretDungeonSweep = 3967; // 请求:个人秘境副本扫荡 msgId:3967;
        public const Int16 C_SecretDungeonSweepReward = 3968; // 请求:个人秘境副本扫荡领奖励 msgId:3968;


        public const Int16 S_ResSimpleSecrectDuplInfo = 8637; // 返回单人秘境副本面板信息 msgId:8637;
        public const Int16 S_ResEnterSimpleSecrectDupl = 8638; // 返回进入单人秘境副本 msgId:8638;
        public const Int16 S_SimpleSecrectDuplTrace = 8639; // 秘境副本追踪面饭信息 msgId:8639;
        public const Int16 S_SimpleSecrectDuplCom = 8640; // 单人秘境副本结算 msgId:8640;
        public const Int16 S_ResExitSimpleSecrectDupl = 8641; // 返回退出单人秘境副本 msgId:8641;
        public const Int16 S_UpdateSecrectDuplTili = 8642; // 更新组队或次数 msgId:8642;
        public const Int16 S_ResJiHuoSecrectDupl = 8643; // 返回激活结果 msgId:8643;
        public const Int16 S_SecretDungeonSweep = 8967; // 返回:个人秘境副本扫荡 msgId:8967;
        public const Int16 S_SecretDungeonSweepReward = 8968; // 返回:个人秘境副本扫荡领奖励 msgId:8968;
        public const Int16 S_SecretDungeonSweepInfo = 8969; // 返回:个人秘境副本扫荡 msgId:8969;

        public const Int16 C_WorldBoss = 2063; //请求世界BOSS列表 msgId:2063
        public const Int16 S_WorldBoss = 7064; //返回世界BOSS列表(刷新时推单个) msgId:7064
        public const Int16 S_ActivityState = 7065; //返回活动状态(刷新时推单个) msgId:7065
        public const Int16 C_ActivityEnter = 3159; //请求:进入活动 msgId:3159;
        public const Int16 S_Activity = 8158;  	//  S->C 登录返回活动列表 msgId:8158
        public const Int16 S_ActivityEnter = 8159;  	// S->C 返回:进入活动 msgId:8159
        public const Int16 C_ActivityQuit = 3160;  // C->S 请求:退出活动 msgId:3160;
        public const Int16 S_ActivityQuit = 8160;   // S->C 返回:退出活动 msgId:8160
        public const Int16 S_ActivityFinish = 8161;	// S->C 返回:活动结束(活动内玩家) msgId:8161
        public const Int16 S_WorldBossDamage = 8162;  // S->C 返回玩家累计伤害 msgId:8162
        public const Int16 S_FieldBoss = 8737;   //返回野外BOSS信息
        public const Int16 S_WorldBossHurt = 8163;   // S->C 返回世界BOSS伤害信息(活动内) msgId:8163
        public const Int16 C_WaBaoList = 3806;  // C->S 请求:挖宝列表信息 msgId:3806;
        public const Int16 S_WaBaoList = 8807;  //S->C 返回Boss挖宝信息 msgId:8807
        public const Int16 C_GetWaBaoReward = 3805; // C->S 请求:领取对应BOSS奖励 msgId:3805;
        public const Int16 S_GetWaBaoReward = 8808; // S->C 领取对应BOSS奖励 msdId:8808
        public const Int16 C_ZhenBaoGe = 3113; //请求珍宝阁数据 msgId：3113；
        public const Int16 C_ZhenBaoGeSubmit = 3114; //珍宝阁提交道具 msgId：3114；
        public const Int16 C_ZhenBaoGeSpeItem = 3115; //珍宝阁提交特殊道具 msgId：3115；
        public const Int16 S_ZhenBaoGe = 8113;  //返回珍宝阁数据，数据刷新时也返回这个 msgId：8113；

        //秘境-组队
        public const Int16 C_CreateRoom = 2156;// 创建房间 msgID : 2156
        public const Int16 S_TimeDungeonRoomInfo = 7154;// 创建房间结果 msgID : 7154
        public const Int16 C_ReqRoomStart = 2163; // 房间队伍开始战斗 msgId:2163;
        public const Int16 C_SecretTeamHeadNoticePrepare = 2289; // 组队秘境，请求准备提醒 msgId : 2289
        public const Int16 S_EnterDulpPrepare = 7245; // 进入组队副本提示准备消息 msgId:7245
        public const Int16 C_SecretTeamStart = 2907; // 试炼秘境队伍请求进入组队活动
        public const Int16 S_TeamTargetData = 7907; // 服务器返回: 组队目标数据  msgId:7907
        public const Int16 C_UpdateTeamTarget = 2908; // 客户端请求: 更新组队目标
        public const Int16 C_UpdateTeamLimit = 2909; // 客户端请求: 更新组队限制条件
        //Trap
        public const Int16 S_TRAPTRIGGER = 9947;//陷阱触发

        //宝塔秘境
        public const Int16 C_EnterTreasureDupl = 4940;    // 客户端请求：进入宝塔秘境 msgId:4940;
        public const Int16 C_QuitTreasureDupl = 4941;    // 客户端请求：退出宝塔秘境 msgId:4941;
        public const Int16 S_EnterTreasureDupl = 9951;    // 服务器返回：进入宝塔秘境 msgId:9951;
        public const Int16 S_QuitTreasureDupl = 9950;    // 服务器返回：退出宝塔秘境 msgId:9950;




        public const Int16 C_FindTreasure = 3476;    // 客户端请求：寻宝任务接取 msgId:3476;
        public const Int16 C_FindTreasureCancel = 3477;    //  客户端请求：取消寻宝任务 msgId:3477;
        public const Int16 C_FindTreasureCollect = 3478;    //  客户端请求：接取 msgId:3478;
        public const Int16 C_ReqTreasureAddTime = 3676;    // 请求打宝塔增加时间 msgId:3676;

        public const Int16 S_FindTreasureInfo = 8475;    // 寻宝信息 msgId:8475;
        public const Int16 S_FindTreasureResult = 8476;    // 服务器返回:寻宝任务接取 结果 msgId:8476;
        public const Int16 S_FindTreasureCancel = 8477;    // 服务器返回:取消寻宝任务 msgId:8477;
        public const Int16 S_FindTreasureCollect = 8478;    // 服务器返回:接取结果 msgId:8478;
        public const Int16 S_TreasureTodayAddedTimer = 8676;    // 返回今日已经购买的时间 msgId:8676;
        public const Int16 S_TreasureRemainTime = 8677;    // 打宝塔剩余时间 msgId:8677;
        public const Int16 S_TreasureUpdateBoss = 8678;    // 更新BOSS状态信息 msgId:8678;

        public const Int16 C_QueryMonsterByPosition = 4939;    // 客户端请求：查询指定点是否有怪 msgId:4939

        public const Int16 S_QueryMonsterByPosition = 9948;    // 服务器返回:查询指定点有多少怪 msgId:9948

        public const Int16 C_EquipGroup = 3465;  // 客户端请求：设置装备套装 msgId:3465;
        public const Int16 S_EquipGroup = 8465;   // 服务器返回:设置装备套装 msgId:8465;
        public const Int16 C_EquipGroupTwo = 3564;  // 客户端请求：设置装备套装 msgId:3465;
        public const Int16 S_EquipGroupTwo = 8565;   // 服务器返回:设置装备套装 msgId:8465;
        public const Int16 C_EquipSmelt = 3514;   //客户端请求:装备熔炼 msgId:3514
        public const Int16 S_EquipSmelt = 8923;   //服务器返回:装备熔炼结果 msgId:8923
        public const Int16 C_EquipGroupPeel = 3558;  //客户端请求：剥离装备套装 msgId:3558
        public const Int16 S_EquipGroupPeel = 8558;  //服务器返回:剥离装备套装 msgId:8558
        public const Int16 C_EquipGroupLvlUp = 3685;  //客户端请求：套装升级 msgId:3685
        public const Int16 S_EquipGroupLvlUp = 8685;  //服务器返回:套装升级 msgId:8685


        public const Int16 S_EquipInherit = 8150; // 返回装备传承 msgId:8150;
        public const Int16 C_EquipInherit = 3148; // 装备传承 msgId:3148;

        public const Int16 C_GuildQuestSweep = 3942;  //帮派任务一键完成 msgId:3942; 
        public const Int16 S_GuildQuestSweep = 8942;  //帮派任务一键完成奖励信息 msgId:8942;


        public const Int16 C_EquipPeiYang = 3850; //请求培养 msg :3850;
        public const Int16 S_EquipPeiYang = 8850; //返回培养 msg :8850;
        public const Int16 S_SCENE_OBJ_HONGYAN_LEVEL = 8849; //服务器返回:场景中红颜等阶和星级,（包括自己和别人的红颜)

        public const Int16 C_EquipPeiYangSet = 3851; //请求保存属性 msg :8851;
        public const Int16 S_EquipPeiYangSet = 8851; //返回保存属性 msg :8851;

        public const Int16 S_DailyActivy = 9953;  //返回日常活动开启列表 msg:9953

        public const Int16 C_ColorGemOperation = 3681;
        public const Int16 C_ColorGemLevelup = 3682;

        public const Int16 S_ColorGemInfo = 8671;
        public const Int16 S_ColorGemLevelup = 8672;
        public const Int16 S_OtherColorGemInfo = 8351;

        public const Int16 C_DeviceTokenInfo = 2951; //发送设备token信息 msgID 2951

        public const Int16 S_AddExpInfo = 11180; //s->c 经验信息 msgId:11180
        public const Int16 S_MoShenItemResult = 11181; //服务器推送：一键穿戴魔神装备结果 msgId:11181


    };

};

