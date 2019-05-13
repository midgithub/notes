using UnityEngine;
using XLua;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class DataEvent
    {
        public static string DE_LAST_ENTER_SCENE = "请求进入的场景";
         
        public static string DE_COUNT_FICTITIOUS_COIN = "虚拟币购买次数统计";


        public static string DE_YUANBAO = "元宝";
        public static string DE_TILI = "体力";
        public static string DE_JINLI = "精力";
        public static string DE_GOLD = "金币";

        private static System.Collections.Generic.Dictionary<int, string> m_ItemType = null;

        public static System.Collections.Generic.Dictionary<int, string> ItemType
        {
            get
            {
                if (DataEvent.m_ItemType == null)
                {
                    DataEvent.m_ItemType = new System.Collections.Generic.Dictionary<int, string>();
                }
                return DataEvent.m_ItemType;
            }
        }

        public static string DE_KOF_1V1_FINISH = "1V1血战台结束";
        public static string DE_KOF_3V3_FINISH = "3V3血战台结束";
        //DataAnalyser.instance.OnEvent(DE_LAST_ENTER_SCENE,dict);
    }

    [LuaCallCSharp]
    public enum GameEvent
    {
        // 全局事件
        GE_NONE = 0,

        GE_BEGIN_LOADSCENE,
        GE_BEFORE_LOADSCENE,                //开始加载场景
        GE_AFTER_LOADSCENE,                 //场景加载完
        GE_LOADSCENE_PROGRESS,
        GE_LOADSCENE_FINISH,
        GE_BEGIN_LOADSCENE_LOGIN,               //进入登录场景
        GE_CHECK_BACKDOWNLOAD,                  //进入创角场景前检测后台下载是否完成界面
        GE_BEGIN_LOADSCENE_CREATE_ROLE,         //进入创角场景
        GE_CLEANUP_USER_DATA,

        GE_PLAYER_LOADING_OVER,                 //主角加载完
        GE_OTHERPLAYER_LOAD_OVER,

        GE_MAINUI_HIT_UPDATE,
        GE_MAINUI_NEWTIPS_UPDATE,
        GE_MAINUI_PANEL_OPEN,
        GE_MAINUI_ONLEVELUP,
 

        //移动事件
        GE_ACTOR_MOVE_BEGIN,
        GE_ACTOR_MOVE_STOP,
        GE_ACTOR_MOVE_END,

        GE_PLAYER_MOVE_BEGIN,
        GE_PLAYER_MOVE_STOP,
        GE_PLAYER_MOVE_END,
        GE_PLAYER_MOVE_POSITION,

        GE_PLAYER_MOVE_Arrive,

        //移动
        GE_OTHER_MOVE_TO,                       //玩家移动
        GE_OTHER_CHANGE_DIR,                    //玩家改变方向
        GE_OTHER_MOVE_STOP,                     //玩家停止移动
        GE_MONSTER_MOVE_TO,                   //怪物停止移动
        GE_TELEPORT,                        //玩家传送
        GE_CHANGE_POS,                        //玩家传送
        GE_AUTO_PATH_FIND_STATE,            //自动寻路状态

        //对象进入和离开场景
        GE_SC_ATTRINFO,                         //属性变化
        GE_SC_PLAYER_SHOW_CHANGED,                         //外观变化
        GE_OBJ_ENTER_SCENE,                 //对象进入场景
        GE_OBJ_LEAVE_SCENE,                 //对象离开场景
        GE_OBJ_Disapper,                 //对象消息
        GE_OBJ_CLEAR,                       //清除场景对象，断线重连后触发
        GE_ACTOR_REMOVE,                    //移除角色
        GE_OTHERPLAYER_LEAVE,               //其它玩家离场
        GE_ITEM_DROP,                       //物品掉落
        GE_OBJ_HONGYAN_LEVEL,               //红颜星级变化 

        GE_OBJ_ITEM_ENTER,                  //场景中物品掉落
        GE_OBJ_ITEM_LEAVE,                  //场景物品消失
        GE_ITEM_PICKUP,                     //拾取物品
        GE_CAMP_ENER,
        GE_CAMP_LEAVE,
        GE_CAMP_UPDATE,
        GE_COLLECTION_UPDATE,

        //境界
        GE_SC_JINGJIE_INFO,                    //境界信息获取
        GE_SC_JINGJIE_SAVE,                    //保存加点
        GE_SC_JINGJIE_RESET,                   //重置加点
        GE_JINGJIE_INFO,
        GE_JINGJIE_SAVE,
        GE_JINGJIE_RESET,
        GE_PLAYER_JINGJIE_LV,                //玩家境界等级变化
        GE_PALYER_JINGJIE_EXP,              //玩家境界经验变化

        //状态改变
        GE_STATE_CHANGED,                   //状态改变

        GE_CC_STATE_DEAD,                   //死亡状态 -客户端推送
        //复活消息
        GE_SC_REVIVE,                       //复活

        /// <summary>
        /// skill
        /// </summary>
        GE_NOTIFY_CAST_SKILL,                   //请求释放技能      
        GE_NOTIFY_SHOW_SKILL_SCOPE,             //显示技能释放区域  
        GE_NOTIFY_HIDE_SKILL_SCOPE,             //显示技能释放区域  
        GE_RESPOND_CAST_SKILL_SUCCESS,          //技能成功释放
        //GE_CANCEL_SKILL_COMBO,                  //技能连招状态结束 


        GE_SC_CASTSKILL,                     //释放技能
        GE_SC_OTHER_CASTSKILL_BEGIN,               //其他人释放技能
        GE_SC_OTHER_CASTSKILL_END,               //其他人释放技能
        GE_SKILL_MAGICCOLLDOWN,                  //法术冷却信息
        GE_SKILL_CASTPREPBEGAN,                  //施法蓄力开始
        GE_SKILL_CASTPREPENDED,                  //施法蓄力结束
        GE_SKILL_CASTCHANGEBEGAN,                //施法引导开始
        GE_SKILL_CASTCHANGEEND,                  //施法引导结束
        GE_SKILL_INTERPUPTCAST,                  //施法中断通知

        GE_SKILL_TARGETLIST,                    //技能对象列表

        GE_SC_CAST_MOVE_EFFECRT,                //移动效果

        GE_SC_SKILLEFFECT,                      //技能伤害
        GE_SC_KNOCKBACK,   //击退效果
        GE_SC_ADDBUFF,    //增加BUFF效果
        GE_SC_UPDATEBUFF, //更新BUFF效果
        GE_SC_DELBUFF,    //删除BUFF效果
        GE_SC_ADDBUFFList,    //增加BUFF效果
        GE_CC_ADDBUFF,   //客户端之间推送增加BUFF
        GE_CC_DELEBUFF,  //客户端之间推送删除BUFF


        GE_JOYSTICK_DOWN,					        // 摇杆按下事件
        GE_JOYSTICK_UP,								// 摇杆释放事件



        GE_Boss_QiJue,                  //boss进入气绝状态

        GE_RAGE_CHANGE,                         //怒气值改变

        GE_CHANGE_MAIN_PLAYER,                         //怒气值改变

        //GE_NOTIFY_SKILL_DAMAGE,                 //技能伤害


        GE_CAMERA_EVENT_HEIGHT_ACTIVE,                 //相机高度改变
        GE_CAMERA_EVENT_HEIGHT_DISABLE,                 //相机高度改变
        GE_UPDATE_CAMERA_POSITION_IMMEDIATE,            //立刻更新相机位置

        //GE_FBMAP_OVER,                                  // 副本结束        	 
        //GE_FBMAP_WIN,                                   // 副本结束（胜利）
        //GE_FBMAP_LOSE,                                  // 副本结束（失败）
        //GE_FBMAP_FINISH_TIME,                          //征战天下完成时间

        GE_MONSTER_DEATH,                               //怪物        	 

        GE_SUMMON_DEATH,                                //召唤生物        	 

        GE_PLAYER_DEATH,                               //主角        	         


        GE_PLAYER_RECOVER,                            	// 主角复活    

        GE_BOSS_SKILL,                            	   // BOSS预警    
        GE_BOSS_SKILL_END,                             // BOSS预警  

        GE_PVP_PLAYER_RECOVER,

        GE_PLAYER_RESET_HP,								// 主角喝血瓶

        GE_ACTOR_CAN_BEHIT,                            //主角/怪物可被攻击

        GE_PLAYER_CASTSKILL_ENDURE_END,                //主角释放技能霸体结束

        //GE_PLAYER_CASTSKILL_ICONCHANGE,					// 主角技能按钮图标变化


        GE_LOGINGAME,                               // 登入游戏
        GE_CREATEACTOR,                             // 创建新角色
        GE_CREATEACTOR_FAILED,                      // 创建角色失败
        GE_RANDOMNAME,                              // 随机名字
        GE_SELECTFIRSTCARD,                         // 选择初始卡牌

        GE_CONNECTINFO,                             // 连接阶段信息
        GE_CONNECT_CLOSE,                           // 连接断开

        GE_LOGIN_MSG,                               //服务器返回登录数据
        GE_SC_ROLE_INFO,                            //服务器返回的角色信息
        GE_SC_CREATE_ROLE,                          //服务器返回的创建角色信息
        GE_SC_ENTER_GAME,                           //服务器返回的进入游戏消息
        GE_SC_ENTER_SCENE,
        GE_SC_MAINPLAYER_ENTER_SCENE,               //服务器下发主角进入场景
        GE_SC_LEAVE_GAME,                           //服务器下发离开场景消息，选择角色界面
        GE_MONSTER_LOADED,                          //场景中怪物加载完成

        GE_SC_OBJ_DEAD ,                            //对象死亡

        GE_CC_CurMoveState,                         //当前角色移动状态
        GE_CC_TaskUpdate,                           //任务信息
        GE_CS_QueryQuest,                           //请求任务列表
        GE_CS_QuestClick,                           //点击计数
        GE_CS_AcceptQuest,                           //发送接受任务请求
        GE_CS_GiveupQuest,                           //发送放弃任务请求
        GE_CS_FinishQuest,                           //发送完成任务请求
        GE_SC_QueryQuestResult,                     //服务器反馈任务列表信息         
        GE_SC_QuestAdd,                             //服务器增加一个任务   
        GE_SC_QuestUpdate,                          //服务器任务更新         
        GE_SC_AcceptQuestResult,                     //服务器接受任务反馈       
        GE_SC_GiveupQuestResult,                     //服务器放弃任务反馈    
        GE_SC_FinishQuestResult,                     //服务器完成任务反馈             
        GE_SC_QuestDel,                              //服务器通知任务删除
        GE_CS_STRUCT_DEF,                            //请求触发静物
        GE_SC_STRUCT_DEFResult,                      //静物触发反馈

        GE_CS_DailyQuestStar,                  // 日环任务升到5星
        GE_CS_DailyQuestFinish, //日环任务一键完成
        GE_CS_DailyQuestResult, // 请求日环任务结果
        GE_CS_DQDraw,// 日环任务抽奖
        GE_CS_DQDrawConfirm, // 日环任务抽奖确认
        GE_SC_DailyQuestStar, // 日环任务升5星结果
        GE_SC_DailyQuestFinish, // 返回日环任务一键完成奖励信息
        GE_SC_DailyQuestResult, // 返回日环任务结果
        GE_SC_DQDraw, // 返回日环任务抽奖

        GE_CS_AgainstQuestStar, // 讨伐任务升到5星
        GE_CS_AgainstQuestFinish , //讨伐任务一键完成
        GE_CS_AgainstQuestResult, // 请求讨伐任务结果
        GE_CS_AgainstQDraw, // 讨伐任务抽奖
        GE_CS_GetAgainstQSkipReward, // 讨伐跳环领奖
        GE_CS_AgainstQDrawConfirm, // 讨伐任务抽奖确认
        GE_SC_AgainstQuestStar,  // 讨伐任务升5星结果
        GE_SC_AgainstQuestFinish, // 返回讨伐任务一键完成奖励信息
        GE_SC_AgainstQuestResult,// 返回讨伐任务结果
        GE_SC_AgainstQDraw, // 返回讨伐任务抽奖
        GE_SC_GetAgainstQSkipReward, // 服务器通知：讨伐跳环领奖
        GE_SC_AgainstQuestSkipResult, // 服务器返回跳环结果
        GE_SC_AgainstQDrawNotice,// 服务器通知是否抽奖，每环结束都要发
        GE_SC_QuestRewardResult,// 服务端通知：任务下发的扫荡奖励
        GE_CC_QuestRewardResult,  //客户端之间推送： 任务下发的扫荡奖励信息

        GE_CC_OtherQuestIdChange,  //添加其他类型id 到 主任务下, 或删除指定其他类型id
        GE_CC_OtherQuestUpdate,   //客户端之间推送： 其他类型任务状态更新

        GE_CC_OPENTONPCMSG,                         //打开NPC面板
        GE_CC_NPCMSGTOTASK,                         //NPC面板点击完成任务
        GE_CC_SliderTimeOver,                        //进度条结束

        GE_CC_DungeonStartTimeDown,            // 开场倒计时
        GE_CC_HunLingXianYuTimeSecondUpdate,         // 单秒计时推送
        GE_CC_HunLingXianYuTimeSecondUpdateEnd,      // 停止单秒计时推送

        GE_CS_DungeonQuestEnter,              // 任务副本进入
        GE_CS_DungeonQuestQuit,               // 任务副本退出
        GE_SC_DungeonQuestUpdate,             // 任务副本状态更新 .  0成功  1 失败   2 进行中
        GE_CC_DungeonQuestUpdate,            //客户端之间推送： 任务副本状态

        GE_CS_HunlingXianYuInfo,                     //请求 灵兽墓地信息
        GE_CS_ResetHunlingXianYu,                    //重置 灵兽墓地
        GE_CS_ChallHunlingXianYu,                    //挑战 灵兽墓地
        GE_CS_CanHunlingXianYu,                      //扫荡 灵兽墓地
        GE_CS_HunlingXianYuQuit,                     //退出 灵兽墓地
        GE_CS_HunlingXianYuGetAward,                 //获得 奖励

        GE_SC_HunlingXianYuInfo,                      //获得 灵兽墓地信息
        GE_SC_ResetHunlingXianYu,                     //返回重置灵兽墓地结果
        GE_SC_ChallHunlingXianYu,                     //返回进入 灵兽墓地结果
        GE_SC_ChallHunlingXianYuResult,               //返回挑战 灵兽墓地结果
        GE_SC_SCanHunlingXianYuResult,                //返回扫荡 灵兽墓地结果
        GE_SC_HunlingXianYuQuit,                      //返回退出 灵兽墓地结果
        GE_SC_HunlingXianYuGetAward,                  //返回获得奖励结果

        GE_CS_LingShouMuDiInfo,                     //请求 灵兽墓地信息
        GE_CS_ResetLingShouMuDi,                    //重置 灵兽墓地
        GE_CS_ChallLingShouMuDi,                    //挑战 灵兽墓地
        GE_CS_CanLingShouMuDi,                      //扫荡 灵兽墓地
        GE_CS_LingShouMuDiQuit,                     //退出 灵兽墓地
        GE_CS_LingShouMuDiGetAward,                 //获得 奖励

        GE_SC_LingShouMuDiInfo,                      //获得 灵兽墓地信息
        GE_SC_ResetLingShouMuDi,                     //返回重置灵兽墓地结果
        GE_SC_ChallLingShouMuDi,                     //返回进入 灵兽墓地结果
        GE_SC_ChallLingShouMuDiResult,               //返回挑战 灵兽墓地结果
        GE_SC_SCanLingShouMuDiResult,                //返回扫荡 灵兽墓地结果
        GE_SC_LingShouMuDiQuit,                      //返回退出 灵兽墓地结果
        GE_SC_LingShouMuDiGetAward,                  //返回获得奖励结果
        GE_SC_LingShouMuDiMonsterInfo,                 //返回 灵兽墓地怪物波数信息 -法宝
        GE_SC_HunlingXianYuMonsterInfo,               //返回 灵兽墓地怪物波数信息 - 幻灵仙域
        GE_CC_HunlingXianYuInfo,                      //通知UI层，刷新灵兽墓地UI -幻灵仙域
        GE_CC_LingShouMuDiInfo,                       //通知UI层，刷新灵兽墓地UI -法宝

        GE_CS_DungeonNpcTalkEnd,                      // C->S 请求剧情副本NPC对话结束 msgId:3101;
        GE_CS_EnterDungeon,                           // C->S 请求进入剧情副本 msgId:3102;           
        GE_CS_LeaveDungeon,                     	  // C->S 请求退出剧情副本 msgId:3103;
        GE_CS_StoryEnd,                               // C->S 剧情播放完成 msgId:3104;
        GE_CS_DungeonGrup,                            // C->S 请求副本组列表 msgId:3137;
        GE_CS_DungeonAbstain,                         // C->S 放弃副本(副本进行中退出后倒计时,点击确定放弃) msgId:3140; 
        GE_CS_DungeonGetAward,                        // C->S 领取奖励 msgId:3141;
        GE_SC_EnterDungeonResult,                     // S->C 进入副本返回结果 msgId:8102
        GE_SC_LeaveDungeonResult,                     // S->C 离开副本返回结果 msgId:8103
        GE_SC_StoryEndResult,                         // S->C 剧情完成返回结果 msgId:8104
        GE_SC_StoryStep,                              // S->C 副本剧情步骤 msgId:8105            
        GE_SC_DungeonGroupUpdate,                     // S->C 副本组列表更新 msgId:8137
        GE_SC_DungeonCountDown,                       // S->C 开始副本关闭倒计时 msgId:8140
        GE_SC_DungeonPassResult,                      // S->C 副本过关结果 msgId:8141

        GE_CS_WingHeCheng,          // C->S 客户端请求:翅膀合成 msgId:3464;
        GE_CS_UseWingOrginal,       // C->S 客户端请求:使用原始翅膀 msgId:3698;
        GE_CS_WingExtActive,        // C->S 客户端请求:翅膀扩展激活 msgId:3978;
        GE_SC_WingInfo,             // S->C 返回翅膀信息 msgId:8527;
        GE_SC_UseSkin,             // S->C 服务器返回：使用皮肤 msgId:8700;
        GE_SC_WinAndSkinInfo,       // S->C 获取翅膀(皮肤)相关信息 msgId:8701;
        GE_SC_WingHeCheng,          // S->C 返回翅膀合成结果 msgId:8464;
        GE_SC_UseWingOrginal,   	// S->C 使用原始翅膀 msgId:8698;
        GE_SC_ActivateSkin,        	// S->C 激活翅膀(皮肤) msgId:8699;
        GE_SC_WingExtActive,     	// S->C 返回：翅膀扩展激活 msgId:8978;
        GE_SC_WingExtActiveInfo,   	// S->C 返回 翅膀扩展激活列表 msgId:8979;
        GE_SC_WingExtCountInfo,    	// S->C 返回 翅膀扩展计数列表 msgId:8980;
        GE_CC_WingListInfo,       //通知UI层，更新拥有的翅膀列表
        GE_CC_WingExtListInfo,       //通知UI层，更新拥有的成就列表

        GE_SC_BackAchievementInfo,    //服务器返回：成就信息 msgId:8372;
        GE_SC_AchievementUpData,       //服务器返回:刷新成就进度 msgId:8479;

        GE_CS_MagicWeaponLevelUp, //C->S 请求:神兵进阶 msgId:3250;
        GE_CS_MagicWeaponChangeModel,  //C->S 请求:神兵换模型 msgId:3456;
        GE_SC_MagicWeaponInfo, // S->C 服务器通知:神兵信息 msgId:8248
        GE_SC_MagicWeaponProficiency, //S->C 服务器通知:神兵熟练度 msgId:8249
        GE_SC_MagicWeaponLevelUp,  // S->C 服务器通知:神兵进阶 msgId:8250
        GE_SC_MagicWeaponChangeModel,  // S->C 服务器通知:神兵换模型结果 msgId:8456
        GE_CC_MagicWeaponInfo,  //客户端之间推送神兵更新信息

        GE_CS_PiFengLevelUp, //C->S 请求:圣盾进阶 msgId:3724;
        GE_CS_PiFengChangeModel,  //C->S 请求:圣盾换模型 msgId:3725;
        GE_SC_PiFengUpdateInfo, // S->C 服务器通知:圣盾信息 msgId:8723
        GE_SC_PiFengLevelUp,  // S->C 服务器通知:圣盾进阶 msgId:8724
        GE_SC_PiFengChangeModel,  // S->C 服务器通知:圣盾换模型结果 msgId:8725
        GE_CC_PiFengInfo,  //客户端之间推送圣盾更新信息


        GE_SC_USE_ATTR_DAN,    //服务端推送使用属性丹
        GE_CC_USE_ATTR_DAN,    //客户端之间更新已使用的属性丹数量



        // GE_CC_SceneCreatModelUpdate,                //场景创建模型

        GE_EVENT_NET_RECONNECT_START,               // 开始断线重连（连接断开后，下一次发包时会自动触发重连，重连会重新走登陆流程，以免数据不同步） 
        GE_EVENT_NET_RECONNECT_OK,                  // 断线重连成功
        GE_EVENT_NET_DISCONNECTED_FOR_LONGTIME_TO_RELOGIN,  //断线超过指定时间

        GE_ThreadDownload_Complete,         //多线程下载进度通知 , 


        GE_DAMAGE_CHANGE,                               // 伤害改变

        GE_BOSS_RISE_TO_SKY_START,                      //boss死从天降技能升空开始
        GE_BOSS_RISE_TO_SKY_STATED,                     //boss死从天降技能停留
        GE_BOSS_RISE_TO_SKY_END,                        //boss死从天降技能升空结束

        GE_MOVE_COLLIDE_TARGET,                         //位移中碰到目标


        GE_MSG_MAINROLE_PROPERTY_HEROPOINTS,               //英雄点改变

        GE_MSG_MAINROLE_PROPERTY,                           // 主角属性改变
        GE_MSG_MAINROLE_LEVELUP,                           // 主角等级提升
 
        GE_MSG_JUMPNEWHANDPOINT,                            // 跳过新手指引
 

 

        GE_MSG_MAINACTOR_CREATE,                        // 主玩家创建消息
 
 

        GE_MSG_CARD_INITLIST,                           // 消息_初始卡牌列表
        GE_MSG_CARD_ADD,                                // 消息_卡牌添加
        GE_MSG_CARD_EXP,                                // 消息_卡牌经验等级变化
        GE_MSG_CARD_UPSTAR,                             // 消息_卡牌升星
        GE_MSG_CARD_TUPO,                               // 消息_卡牌突破
        GE_MSG_CARD_SKILLUPGRADE,                       // 消息_卡牌技能升级
        GE_MSG_CARD_UPSTAR_Complete,                    // 消息_卡牌升星  卡牌数据处理完成 , 
        GE_MSG_CARD_WAKEUP_LEVEL,                       // 消息_升级卡牌觉醒等级
        GE_MSG_CARD_HANDLE_WAKEUP_STONE,                // 消息_处理觉醒石
        GE_MSG_CARD_REFRESH_WAKEUP,                     // 消息_觉醒洗练
        GE_MSG_CARD_SAVE_REFRESH_WAKEUP,                // 消息_保存觉醒洗练

        GE_MSG_CARD_LEVEL_UP,                                // 消息_卡牌升级

        GE_MSG_CARD_LIANHUA,                            //卡牌炼化
        GE_MSG_CARD_CHANGEHUN,                            //武魂炼化

        GE_MSG_CARD_CHANGETEAM,                         // 消息_卡牌改变小队

        GE_MSG_CARD_SWAP_EQUIP,                         //卡牌交换装备

        GE_MSG_FUBEN_SYNRESULT,                         // 消息_同步副本奖励
        GE_MSG_FUBEN_GETAWARDINFO,                      // 消息_同步副本奖励
        GE_MSG_FUBEN_GETAWARD,                          // 消息_领取奖励结果
        GE_MSG_FUBEN_REQUIREAWARD,                     //申请领取奖励
        GE_MSG_FUBEN_PASSLIST,                          // 消息_通关副本列表
        GE_MSG_CHAPTER_INFO,                            // 消息_章节领取数据
        GE_MSG_CHAPTER_RESULT,                          // 消息_章节领取结果
        GE_MSG_SWEEP_RESULT,
        GE_MSG_SYNSIGNINFO,                             // 消息_同步PVP副本报名信息
        GE_MSG_FUBEN_RESETTIMES,                        // 消息_重置副本次数
        GE_MSG_1V1_TARGETACTORID,                       // 消息_同步实时PVP1v1对手角色ID
        GE_MSG_1V1_FINISH,                              // 消息_实时PVP1v1结果
        GE_MSG_1V1_DATA,                                // 消息_实时PVP1v1数据
        GE_MSG_1V1_Moba_TARGETACTORID,                       // 消息_同步实时PVP1v1对手角色ID
        GE_MSG_1V1_Moba_FINISH,                              // 消息_实时PVP1v1结果
        GE_MSG_1V1_Moba_DATA,                                // 消息_实时PVP1v1数据
        GE_MSG_TEAMBOSS_FINISH,                         // 消息_组队BOSS结果
        GE_MSG_TEAMBOSS_DATA,                           // 消息_组队BOSS数据

        GE_MSG_FUBEN_ON_ENTER_SCENE,                    // 消息_进入副本 服务器回应
 


        GE_MSG_CARD_CHANGETEAM_OPERATE_OVER,            // 消息_卡牌改变小队 事件处理完成, 通知界面刷新

        GE_MSG_GUIDE_RECORDLIST,                        // 消息_引导记录列表

 

        GE_MSG_VIEWACTOR,                               // 消息_查看角色具体数据
  
          
        GE_MSG_OTHERACTOR_APPEAR,                       // 消息_其他玩家出现
        GE_MSG_ENTITY_APPEAR,                           // 消息_非玩家实体出现
        GE_MSG_ENTITY_DISAPPEAR,                        // 消息_实体消失
        GE_MSG_ENTITYMOVE,                              // 消息_移动
        GE_MSG_SWITCHCARD,                              // 消息_切换出战卡牌
        GE_MSG_SKILLRESULT,                             // 消息_技能使用结果
         

        GE_Guide_dodge,                                 //闪避指引
        GE_Guide_heji,                                  //合击指引


        GE_MSG_CAMP_JOIN,                               // 加入国家



        GE_UI_SHOPPE,                                   //刷新商店商品可购买次数
        GE_UI_CARD_INFO,                                //刷新武将信息
        GE_UI_CARD_INFO_CHANGE,                                //改变卡牌
        GE_UI_CARD_REFRESH_OVER,                        //卡牌数据刷新完毕
        
        GE_Guide_OpenNewFunc,                     //副本通关开启功能 
        GE_Guide_Hide_Guide_Panel,                //关闭指引界面-蒙板


        GE_FuBen_loading_end,         //进入副本 loadind界面关闭 时候触发
        GE_MainCity_loading_end,

        GE_FRIST_UI_LOADED,

        GE_FuBen_enter,         //进入副本 时候触发

        GE_FuBen_exit,         //退出副本 时候触发


        GE_FuBen_WaveChange,         //刷怪波数变化

        //副本，boss刷新
        GE_FuBen_BossRefresh,


        //任务
        GE_TASK_Initiaze,           //进入副本初始化任务列表
        GE_TASK_unInitiaze,         //退出副本清除任务列表，重置时间显示
        GE_TASK_Story_start,        //开始播放剧情,通知任务系统停止计时
        GE_TASK_Story_end,          //剧情播放完，通知任务系统开始计时
        GE_TASK_KILLBOSS,           //击杀BOSS
        GE_TASK_KILLNUM, 			//杀怪计数类任务
        GE_TASK_KILLHERO, 			//击杀英雄类
        GE_TASK_MONSTERHP,          //怪物血量类
        GE_TASK_HERONUM, 			//己方死亡英雄数量
        GE_TASK_EXPLORATION, 		//探索类
        GE_TASK_ESCORT_END, 		//护送任务
        GE_TASK_CATCH_HERO, 		//追击任务
        GE_TASK_KILL_MONSTER, 		//杀死怪物
        GE_TASK_ALLMONSTER_DEATH, 	//杀死副本所有怪物
        GE_TASK_STOPTASKTIME,       //普通副本boss死亡后停止计时
        GE_TASK_MONSTER_NUM,        //怪物数量
        GE_TASK_BOSS_REACH_DEST,  //逃亡boss到达目的地
        GE_TASK_DAMAGED_BY_TRAP,  //受到陷阱的伤害

        GE_TASK_Camera_Focus, 	//镜头拉到 目标身上
        GE_TASK_Camera_end, 	//镜头动画结束

        GE_TASK_Camera_FIGHT,   //对战胜利,镜头拉到怪物身上,


        GE_EVENT_ATTACK,            //攻击      	         
        GE_EVENT_BEHIT,             //受击      	         
        GE_EVENT_CURE,              //治疗      	         
        GE_EVENT_DIE,               //死前      	         
        GE_EVENT_KILL,              //死后 击杀
        GE_EVENT_TIME,              //时间触发
        GE_EVENT_SHADER,            //材质触发
        GE_EVENT_REBORN,            //重生、
        GE_EVENT_BUFF_STATCKING, //叠加buff
        GE_EVENT_DOUBLEDAMAGE,   //暴击
        GE_EVENT_MONSTER_TIPS,      //




        GE_EVENT_AFTER_MONSTER_LOADED, //怪物加载完
        GE_EVENT_AFTER_ALL_LOADED_OF_SCENE,//场景中的东西全都加载完

        GE_COMPETE_RESULT,                    // 切磋结果

        GE_UI_JEWEL_UPGRADE_SUCCESS,    //宝石升级成功

        GE_SKILL_JOIN_ATK_LIST,         // 合击技能列表
        GE_SKILL_JOIN_ATK_ACTIVE,       // 合击技能激活
        GE_SKILL_JOIN_ATK_STAR_UP,      // 合击技能升星
        GE_SKILL_JOIN_ATK_ACTIEVE,      // 合击技能星级成就激活
        GE_SKILL_JOIN_ATK_SET,          // 设置当前合击技能

        GE_IS_OPEN,          // 是否开启活动

        GE_BATTLE_KILLED, //对战中的击杀信息
        GE_UPDATE_CHALLENGE_PLAYER_LIST,
        GE_UPDATE_CHALLENGE_TEAM_POWER,
        GE_UPDATE_TEAM_POWER,

        GE_UI_COMMONTOPBAR_ENABLE_RETURN,

        GE_RANK_NEW_RANK_DATA,//新增排行榜data
        GE_RANK_ACTORDATA,//排行榜角色数据

        GE_DOWNLOAD_ASSET,//更新的资源进度

        GE_UI_PANEL_CONNECT_START,//与服务器通讯加载loading
        GE_UI_PANEL_CONNECT_SUCCESS,//与服务器通讯结束隐藏loading

        GE_THIRDPARTY_INIT,
        GE_THIRDPARTY_LOGIN,
        GE_THIRDPARTY_LOGOUT,
        GE_THIRDPARTY_PAY,
        GE_THIRDPARTY_EXIT,
        GE_THIRDPARTY_BINDACCOUNT,

        GE_MSG_CREATE_ROLE,
        GE_EVENT_SECONDHPISOVER, //第二条生命值为空时

        GE_MSG_BEGIN_PAUSE_GAME,
        GE_MSG_END_PAUSE_GAME,
        GE_MSG_1V1_Ready,
        GE_MSG_1V1_Moba_Ready,

        GE_MSG_SERVER_TIME,     // 同步服务器时间到客户端

        GE_MSG_GET_BAOWUHDINFO,//宝物活动信息
        GE_MSG_GET_BAOWUHD,//宝物活动奖励信息
        GE_MSG_GET_BAOWU_FINISHTASK,//宝物活动完成状态
        GE_MSG_GET_BAOWU_BUYPOINT,// 宝物活动购买点数

        GE_MSG_GET_MINERALWARCRAFT,//矿洞争霸信息
        GE_MSG_GET_MINERALISCANENTER,//矿洞是否能够进入
        GE_MSG_GET_HOLDRESULT,//矿洞占领结果
        GE_MSG_GET_SELF_HOLDRESULT,//矿洞占领结果
        GE_MSG_GET_SELF_TEAMSET,//矿洞队伍设置结果
        GE_MSG_GET_SET_UIEFFECTACTIVE,//设置UI特效是否激活

        GE_MSG_UNLOCKNEWITEM_COMPLETE,//主界面解锁动画播放完成

        GE_MSG_GET_TREASURE_FINISH,      //获取完成宝箱
        GE_MSG_AV_FINISH,                //广告弹板弹完消息

        GE_MSG_TEAM_SAVE_RESULT,        //队伍保存结果
        GE_MSG_TEAM_SET_CLOSE,          //关闭公共队伍设置界面

        GE_MSG_LEAGUE_NOADMIRE,         //第一次没有冠军
        GE_MSG_LEAGUE_TEAMSTATE,        //联赛设置队伍状态
        GE_MSG_LEAGUE_FAIL,             //联赛获取信息失败
        GE_MSG_LEAGUE_TRIAL_INFO,       //海选赛信息
        GE_MSG_LEAGUE_CHECKFIGHT,       //检测是否进入战斗的
        GE_MSG_LEAGUE_CHECKFIGHTRESULT, //服务器检查战斗结果
        GE_MSG_LEAGUE_FINAL_INFO,       //决赛信息
        GE_MSG_LEAGUE_NEXT_ROUND,       //下一波
        GE_MSG_LEAGUE_GUESS,            //竞猜
        GE_MSG_LEAGUE_GETFINALINFO,     //请求决赛信息
        GE_MSG_LEAGUE_FIGHT_RESULT,     //联赛战斗结果消息
        GE_MSG_LEAGUE_REPLAY_START,     //联赛回放开始
        GE_MSG_LEAGUE_FIGHT_TIME,       //联赛战斗时间
        GE_MSG_LEAGUE_FIGHT_INFO,       //联赛战斗信息
        GE_MSG_LEAGUE_ADMIRE_INFO,      //联赛膜拜信息
        GE_MSG_LEAGUE_ADMIRE,           //联赛膜拜返回
        GE_MSG_LEAGUE_CHAMPION,         //联赛冠军信息

        GE_MSG_GUILD_WAR_BASE_INFO,     //军团战基本信息
        GE_MSG_GUILD_TROOPS_INFO,       //军团城池占领信息
        GE_MSG_GUILD_WAR_SIGN_UP_RESULT,//申请攻城返回结果
        GE_MSG_GUILD_WAR_SIGN_UP_LIST,  //城池的报名列表
        GE_MSG_GUILD_WAR_BID_RESULT,    //军团战竞价结果
        GE_MSG_GUILD_FOOD_EXCHANGE_RESULT,    //兑换军团粮草
        GE_MSG_GUILD_WAR_JOIN_RESULT,    //申请军团战参战
        GE_MSG_GUILD_KICK_RESULT,           //军团战踢人结果
        GE_MSG_GUILD_WAR_SELF_MEMBER_LIST,    //我方军团战报名参战人员
        GE_MSG_GUILD_WAR_ENEMY_MEMBER_LIST,    //对方军团战参战人员
        GE_MSG_GUILD_WAR_REQUET_CHALLENGE,    //请求军团战挑战
        GE_MSG_GUILD_WAR_CHALLENGE_RESULT,      //军团战结算
        GE_MSG_GUILD_WAR_GET_AWARD,             //领取军团战奖励
        GE_MSG_GUILD_WAR_BATTLE_INFO,               //军团战实时战报
        GE_MSG_GUILD_WAR_CLOSE,      //军团战战役结束
        GE_MSG_GUILD_FOOD_CHANGE,    //军团粮草刷新
        GE_MSG_GUILD_WAR_CLEAR_CD,      //清除军团战cd

        GE_MSG_KILLINFO_RESET_TIME,

        GE_MSG_KILLTOWER_SIMULATED_LOCALLY, //本地击杀塔消息
        GE_MSG_PET_SMRITI,      //上牌传功
        GE_MSG_PET_DATA_UPDATE, //卡牌数据更新
        GE_MSG_BAG_UPDATEITEMS,  //背包数据更新

        GE_MSG_PURCHASE_FINISH, //购买物品完毕
        GE_MSG_CHANGE_LAYER,

        GE_MSG_REFSH_OPERATIONAL_HINTSPRITE,//刷新精彩活动红点
        GE_MSG_RETTONGYONGLOGIN,		// 返回通用登陆活动信息
        GE_MSG_RETGETTONGYONGLOGINREWARD,		// 返回领取通用登陆活动奖励
        GE_MSG_RETTONGYONGTASK,		// 返回通用任务活动信息
        GE_MSG_RETGETTONGYONGTASKREWARD,		// 返回领取通用任务活动奖励
        GE_MSG_RETTONGYONGITEM,		// 返回通用折扣活动信息
        GE_MSG_RETBUYTONGYONGITEM,		// 返回购买通用折扣活动物品
        GE_MSG_RETJIERILOGIN,		// 返回节日登陆活动信息
        GE_MSG_RETGETJIERILOGINREWARD,		// 返回领取节日登陆活动奖励
        GE_MSG_RETJIERITASK,		// 返回节日任务活动信息
        GE_MSG_RETGETJIERITASKREWARD,		// 返回领取节日任务活动奖励
        GE_MSG_RETJIERIITEM,		// 返回节日折扣活动信息
        GE_MSG_RETBUYJIERIITEM,		// 返回购买节日折扣活动物品
        GE_MSG_RETJIERIRECHARGE,		// 返回节日充值活动信息
        GE_MSG_RETGETJIERIRECHARGEREWARD,		// 返回领取节日充值活动奖励
        GE_MSG_RETACTIVITYONOFFCFG,     //返回运营活动开启、关闭时间
        GE_MSG_RETGODCARDBOXCFG,        //神将宝箱数据配置
        GE_MSG_RETPOINTSETCFG,          //点将台数据配置
        GE_MSG_SINGLERECHARGECFG,       // 单笔充值配置
        GE_MSG_TOTALRECHARGECFG,        // 累计充值配置
        GE_MSG_DENGLUPRIZECFG,          // 登陆送礼配置
        GE_MSG_TONGYONGTASKCFG,         // 活跃任务配置
        GE_REALTIMEPVP_DAMAGEINFO,
        GE_REALTIMEPVP_RESTTIMELIMIT,
        GE_REALTIMEPVP_STOPTIMELIMIT,

        GE_MSG_RETPVPSCORERANKINFO,// 返回自己的荣誉积分与排行
        GE_MSG_TEAM_INFO_INDEX,  //返回单个队伍信息（新队伍系统）
        GE_MSG_NOTIFY_CLICK,    //点击操作事件

        GE_MSG_TEAMBOSS_ROOM_LIST,    //组队打老板房间列表
        GE_MSG_TEAMBOSS_ADJUST_TEAM,    //组队打老板调整队伍
        GE_MSG_TEAMBOSS_INFO,           //组队打老板基础信息
        GE_MSG_TEAMBOSS_CREATE_ROOM,    //组队打老板开房
        GE_MSG_TEAMBOSS_ENTER_ROOM,    //组队打老板入房
        GE_MSG_TEAMBOSS_OTHER_ENTER_ROOM,    //组队打老板入房
        GE_MSG_TEAMBOSS_LEAVE_ROOM,
        GE_MSG_TEAMBOSS_OTHER_LEAVE_ROOM,
        GE_MSG_TEAMBOSS_HOST_CHANGE,        //组队捶老板房东改变
        GE_MSG_TEAMBOSS_REVIVE,        //组队捶老板复活
        GE_MSG_TEAMBOSS_BUY_TIMES,        //组队捶老板购买次数

        GE_MSG_WHEEL_INFO, //轮盘基础信息
        GE_MSG_WHEEL_AWARD, //轮盘结果

        GE_MSG_WING_UP,//翅膀升级
        GE_MSG_WING_SKILL_UPDATE,//翅膀技能安装卸下
        GE_MSG_WING_SKILL_REFRESH,//翅膀刷新

        GE_MSG_FUBEN_POWERINFO,//返回副本战力信息
        GE_MSG_DAILYRECHAREINFO,// 返回每日累充礼包信息
        GE_MSG_DAILYRECHARGEPRIZER_RESULT,// 返回每日累充礼包奖励领取结果
        GE_MSG_DAILYTOTALRECHARGECFG,// 每日累计充值配置

        GE_MSG_SET_PARTNER,// 设置小朋友

        GE_MSG_GUILD_SKILL_INFO,
        GE_MSG_GUILD_SKILL_UPGRADE,
        GE_MSG_GUILD_WAR_SCOREAWARD,// 返回军团战积分奖励领取结果

        GE_MSG_HEROADJUSTUI_HINT,//刷新武将上阵红点

        //实时跨服血战台
        GE_MSG_RETKFPVPMATCHRESULT,//跨服pvp匹配结果
        GE_MSG_KFPVPFINISH,//跨服pvp结果
        GE_MSG_SYNKFPVP,//跨服pvp同步

        #region--------------------Add By XuXiang--------------------

        GE_FPS,                                     //FPS发生改变
        GE_RECORD_LOG,                              //有日志记录
        GE_GAME_PAUSE,                              //游戏暂停或启动
        GE_AUTO_FIGHT,                              //自动战斗发生改变
        GE_SELECT_TARGET,                           //选择目标
        GE_JOYSTICK_STATE,                          //摇杆状态

        GE_UI_OPEN,                                 //UI被打开
        GE_UI_CLOSE,                                //UI被关闭
        GE_UI_GUIDE_CLICK,                          //UI引导点击
        GE_UI_GUIDE_ENABLE_CHANGE,                  //UI引导按钮启用状态发生变化

        GE_HPBAR_CREATE,                            //创建血条
        GE_HPBAR_DESTORY,                           //销毁血条
        GE_HPBAR_UPDATE,                            //更新血条
        GE_HPBAR_CHANGENODE,                        //修改血条绑点
        GE_HPBAR_PKSTATUS,                          //PK状态改变
        GE_COLLECT_INFO_CREATE,                     //采集信息条创建
        GE_COLLECT_INFO_DESTORY,                    //采集信息条销毁
        GE_MAGICKEY_INFO_CREATE,                     //法宝名称创建
        GE_MAGICKEY_INFO_DESTORY,                    //法宝名称销毁
        GE_PET_LEVEL_UPDATE,                         //更新宠物等级

        GE_FLYTEXT,                                 //飘字
        GE_FLY_ATTR,                                //飘属性        
        GE_FLY_SKILL,                               //技能飘字

        GE_PLAYER_INFO,                             //玩家信息变化
        GE_PLAYER_CURRENCY,                         //玩家货币发生变化
        GE_PLAYER_POWER,                            //玩家战力变化
        GE_PLAYER_HP,                               //玩家血量或血量上限变化
        GE_PLAYER_LV,                               //玩家等级变化
        GE_PLAYER_EXP,                              //玩家经验值变化
        GE_PLAYER_SHOW_CHANGED,                     //玩家外观变化        
        GE_PLAYER_PK_MODE,                          //玩家PK模式发生变化
        GE_PLAYER_PK_HURT,                          //玩家在PK中受伤
        GE_MODLE_POWER_CHANGE,                      //玩家模块战力变化
        GE_TELEPORT_FREE_TIME,                      //免费传送次数变化
        GE_PLAYER_VIP,                              //vip等级变化

        GE_BAG_INFO,                                //背包信息变化
        GE_BAG_ITEM_ADD,                            //背包获得物品
        GE_BAG_ITEM_DEL,                            //背包删除物品
        GE_BAG_ITEM_UPDATE,                         //背包更新物品
        GE_BAG_SIZE,                                //背包容量变化
        GE_BAG_FAST_SELL,                           //背包一键出售
        GE_BAG_USE_ITEM,                            //背包使用物品
        GE_CC_BAGCHANGEOVER,                        //背包位置更换完成                   
        GE_BAG_SELLITEM_RESULT,                     //返回背包出售物品
        GE_CC_MOSHEN_BAGCHANGEOVER,                 //魔神位置更换完成  
        GE_CHAT_MESSAGE,                            //收到聊天消息
        GE_CHAT_PRIVATE_MESSAGE,                    //收到私聊信息
        GE_CHAT_SYSTEM_NOTICE,                      //收到聊天系统公告
        GE_CHAT_ADD_RECENT,                         //添加最近列表
        GE_NOTICE,                                  //收到公告

        GE_RIDE_INFO,                               //坐骑信息变化
        GE_RIDE_STAGE_CHANGE,                       //坐骑状态变化
        GE_RIDE_USE_ATTR_DAN,                       //坐骑使用属性丹
        GE_RIDE_LEVEL_UP,                           //坐骑升级
        GE_RIDE_CHANGE_RIDE,                        //坐骑变更

        GE_SKILL_LIST,                              //技能列表更新
        GE_SKILL_LEARN,                             //技能学习
        GE_SKILL_LEVEL_UP,                          //技能升级
        GE_SKILL_ADD,                               //技能添加
        GE_SKILL_REMOVE,                            //技能移除

        GE_RESLEVEL_INIT,                           //资源本初始化
        GE_RESLEVEL_FIGHT,                          //资源本挑战返回
        GE_RESLEVEL_END,                            //资源本挑战结束
        GE_RESLEVEL_REFRESH,                        //资源本关卡刷新
        GE_RESLEVEL_SWEEP,                          //资源本扫荡回复
        GE_RESLEVEL_SWEEP_END,                      //资源本扫荡完成
        GE_RESLEVEL_BUY_NUMBER,                     //资源本购买次数

        GE_TEAM_INFO,                               //整个队伍信息变化
        GE_TEAM_JOIN,                               //队伍有人加入
        GE_TEAM_EXIT,                               //队伍有人退出
        GE_TEAM_JOIN_REQUEST,                       //入队请求
        GE_TEAM_INVITE_REQUEST,                     //入队邀请
        GE_TEAM_NEARBY_TEAM,                        //附近队伍
        GE_TEAM_NEARBY_ROLE,                        //附近玩家
        GE_TEAM_ROLE_UPDATE,                        //队员信息更新
        GE_TEAM_ROLE_UPDATE_HPMP,                   //队员血量法力更新
        GE_TEAM_DUNGEON_UPDATE,                     //副本状态更新
        GE_TEAM_DUNGEON_UPDATE_PREPARE,             //成员副本状态改变
        GE_TEAM_CHANGE_TARGET,                      //队伍目标
        GE_TEAM_TARGET_LIST,                        //队伍目标列表
        GE_TEAM_AUTO_SETTING,                       //队伍自动匹配设置

        GE_FASHION_INFO,                            //时装信息
        GE_FASHION_DRESS,                           //时装穿戴
        GE_FASHION_STATE,                           //时装状态变化
        GE_FASHION_TRY,                             //时装试穿

        GE_FRIEND_INFO,                             //好友信息
        GE_FRIEND_REMOVE_RELATION,                  //好友移除关系
        GE_FRIEND_GET_REWARD,                       //好友领取礼包
        GE_FRIEND_HAVE_REWARD,                      //好友可领取礼包
        GE_FRIEND_ONLINE_STATUS,                    //好友更新在线状态
        GE_FRIEND_FIND,                             //好友查找
        GE_FRIEND_RECOMMEND_LIST,                   //好友推荐列表
        GE_FRIEND_ADD_APPLY,                        //添加好友申请

        GE_OTHER_PLAYER_INFO,                       //请求其它玩家信息返回
        GE_OTHER_PLAYER_BASIC,                      //其它玩家基本信息
        GE_OTHER_PLAYER_DETAIL,                     //其它玩家详细信息
        GE_OTHER_PLAYER_MOUNT,                      //其它玩家坐骑信息
        GE_OTHER_PLAYER_BODY_TOOL,                  //其它玩家身上道具信息

        GE_GUILD_LIST,                              //帮派列表
        GE_GUILD_INFO,                              //帮派信息
        GE_GUILD_MEMBER_LIST,                       //帮派成员列表
        GE_GUILD_EVENT_LIST,                        //帮派事件列表
        GE_GUILD_CREATE,                            //创建帮派
        GE_GUILD_UPDATE,                            //更新帮派
        GE_GUILD_UPDATE_MASTER,                     //更新帮派帮主
        GE_GUILD_UPDATE_NOTICE,                     //更新帮派公告
        GE_GUILD_BE_INVITED,                        //帮派邀请
        GE_GUILD_OTHER_INFO,                        //其他帮派信息
        GE_GUILD_APPLY_LIST,                        //申请列表
        GE_GUILD_APPLY,                             //申请加入返回
        GE_GUILD_APPLY_NUMBER,                      //申请人数变化
        GE_GUILD_VERIFY_APPLY,                      //审批返回
        GE_GUILD_QUIT,                              //退出帮派
        GE_GUILD_DISMISS,                           //解散帮派
        GE_GUILD_LEVEL_UP,                          //升级帮派
        GE_GUILD_LEVEL_UP_SKILL,                    //开启某组帮派技能
        GE_GUILD_CHANGE_POS,                        //改变职位
        GE_GUILD_KICK_MEMBER,                       //踢出帮派成员
        GE_GUILD_CHANGE_LEADER,                     //禅让帮主
        GE_GUILD_CONTRIBUTE,                        //帮派捐献
        GE_GUILD_LEVEL_UP_MY_SKIL,                  //升级自身帮派技能
        GE_GUILD_AID_INFO,                          //加持属性
        GE_GUILD_AID_BAP,                           //加持洗炼
        GE_GUILD_AID_LEVEL_UP,                      //加持升级
        GE_GUILD_UPDATE_MY_INFO,                    //更新自身信息
        GE_GUILD_PRAY_INFO,                         //祈福信息
        GE_GUILD_PRAY,                              //祈福操作
        GE_GUILD_CREATE_ALLIANCE,                   //创建帮派同盟
        GE_GUILD_DISMISS_ALLIANCE,                  //解散同盟
        GE_GUILD_ALLIANCE_APPLY_LIST,               //同盟申请列表
        GE_GUILD_ALLIANCE_INFO,                     //同盟信息
        GE_GUILD_ALLIANCE_VERIFY_APPLY,             //审核同盟返回
        GE_GUILD_Military,                          //同盟战力返回
        GE_GUILD_Reward,                            //领取福利奖励返回
        #endregion

        GE_STONE_SLOTINFO,                          //宝石孔信息
        GE_STONE_INSET,                             //宝石操作
        GE_STONE_OPENSLOT,                          //宝石孔开启
        GE_STONE_LEVELUP,                           //宝石升级

        GE_COLORGEM_INFO,
        GE_COLORGEM_OPERATION,
        GE_COLORGEM_OTHER_INFO,

        GE_EQUIP_GRADEUP,                           //装备升阶 
        GE_EQUIP_POSLEVELUPUI,                      //装备升级 
        GE_EQUIP_POSLEVELUP,                        //服务器返回:装备位升级
        GE_EQUIP_POSINFO,                           //服务器返回:装备位信息
        GE_EQUIP_OTHER_POSINFO,                     //返回其实玩家装备位信息
        GE_EQUIP_EQUIPADD,                          //返回添加装备附加信息 msgId:8149;
        GE_EQUIP_EQUIPSUPER,                        //返回装备卓越信息 msgId:8239;
        GE_EQUIP_EQUIPEXTRA,                        //返回装备追加属性信息 msgId:8246;
        GE_EQUIP_EQUIPNEWSUPER,                     // 返回装备新卓越信息 msgId:8447;
        GE_EQUIP_EQUIPINFO,                         //返回装备附加信息 msgId:8131;
        GE_EQUIP_EQUIPDECOMPOSE,                    //返回分解结果
        GE_EQUIP_STARSELECT,                        //服务器返回:选择装备位星级

        GE_ITEMTIPS,                                //物品获得失去提示
        GE_SKILL_CDLIST,                            //登录同步技能CD
        GE_EU_ONSETSKILLID,//设置技能信息
        GE_EU_ONSKILLCOOLDOWN,//技能cd
        GE_EU_ONCANCELSKILLCOOL,//取消技能cd
        GE_EU_ONPRESS_SKILLBUTTON,
        GE_EU_ONPROCESSING_SKILLBUTTON, 


        //法宝
        GE_SC_REQMAGICKEYDECOMPOSE,//法宝分解结果 
        GE_SC_UPDATEMAGICKEYINFO, //更新法宝信息  
        GE_SC_TRAINMAGICKEY,//返回法宝打造结果  
        GE_SC_MAKEMAGICKEY,//返回法宝打造结果  
        GE_SC_MagicKeyWashInfo,//返回法宝祝福值信息
        
        GE_SC_RETURNMAGICKEYGODINFOS, //返回法宝仙灵列表 
        GE_SC_RESMAIGCKEYGODINSET,//返回仙灵穿戴结果  
        GE_SC_MAGICKEYINSETSKILL,// 返回法宝技能 
        GE_SC_MAGICKEYFEISHENG,//返回:法宝飞升升经验 
        GE_SC_RETURNMAGICKEYSKILLLINGWU, // 返回法宝被动技能领悟 
        GE_SC_MAGICKEYSTARLEVELUP,//返回法宝升星结果
        GE_AutoUseMagicKeySkill,//自动释放法宝技能
        GE_UseMagicKeySkill,//放法宝技能
        //合成分解
        GE_ITEM_COMPOSE_RESULT,//物品分解合成结果
        //红颜
        GE_HONGYANACT,//// 服务器返回：激活返回结果 msgId:8756;
        GE_HONGYANFIGHT,// 服务器返回：红颜出战类型变化 msgId:8758;
        GE_BeautyWomanLevelUp,// 服务器返回：升阶返回结果 msgId:8757;
        GE_BeautyWomanInfo,// 服务器通知:红颜信息(玩家登陆;每天更新) msgId:8755;
        GE_BeautyWomanUseAtt,// 返回: 修为等级信息 msgId:8804;
        GE_BeautyWomanFightingUpdate,// 战斗力更新
        
        //红颜合体
        GE_SC_CHANGE_BEGIN,
        GE_SC_CHANGE_END,

        //等级副本
        GE_SC_DominateRouteData,// 返回UI信息 msgId:8390;
        GE_SC_DominateRouteUpDate,// 刷新 msgId:8391;
        GE_SC_BackDominateRouteChallenge,// 返回挑战 msgId:8392;
        GE_SC_BackDominateRouteQuit,// 返回退出 msgId:8393;
        GE_SC_BackDominateRouteInfo, // 返回追踪信息 msgId:8394;
        GE_SC_BackDominateRouteWipe, // 返回扫荡 msgId:8395;
        GE_SC_BackDominateRouteVigor,// 返回购买精力 msgId:8396;
        GE_SC_BackDominateRouteBoxReward,// 返回领取宝箱奖励 msgId:8397;
        GE_SC_BackDominateRouteEnd, // 返回通关 msgId:8398;
        GE_SC_BackDominateRouteMopupEnd,// 返回扫荡结束 msgId:8399;
         
        //boss挑战副本
        GE_PersonalBossList,// 服务器返回BOSS挑战列表 msgId:8560;
        GE_CC_PersonalBossList,// 客户端推送  BOSS挑战列表 msgId:8560;
        GE_BackEnterResultPersonalBoss,// 服务器返回进入个人BOSS结果 msgId:8561;
        GE_BackQuitPersonalBoss,// 服务器:退出个人BOSS结果 msgId:8562;
        GE_PersonalBossResult,// 服务器:挑战个人BOSS结果 msgId:8563;
        
        //
        GE_SKILL_UPGRADE,//刷新技能绑定信息
        //王冠
        GE_SC_CROWNINFO,//返回王冠数据 
        GE_SC_CROWNSKILLUP,//返回王冠技能升级
        GE_SC_CROWNACTIVE,//返回王冠激活
        GE_SC_CROWNRESULT,//返回王冠副本结果
        GE_CC_CROWN_MAINTASKMSG, //客户端之间推送  王冠关联 主线任务状态更新
        //通缉
        GE_CS_TongJiInfo,  //客户端请求通缉列表 msgId:3164
        GE_CS_TongJiLvlRefresh,  //难度刷新 msgId:3165;
        GE_CS_AcceptTongJi,  //接受通缉任务 msgId:3166;
        GE_CS_GetTongJiReward, //领取通缉奖励 msgId:3168;
        GE_CS_GiveupTongJi, //放弃通缉 msgId:3169;
        GE_CS_GetTongJiBox, //获取通缉宝箱奖励 msgId:3171;
        GE_CS_TongJiRefreshState, //刷新悬赏状态 msgId:3522
        GE_SC_TongJiInfo, //服务端通知:返回通缉信息 msgId:8164;
        GE_SC_TongJiLvlRefreshResult, //服务器通知：返回难度刷新 msgId:8165;
        GE_SC_AcceptTongJiResult, //服务器通知：返回接受通缉任务 msgId:8166;
        GE_SC_FinishTongJi, //服务器通知：通缉活动可领奖 msgId:8167;
        GE_SC_GetTongJiReward, //服务器返回结果：返回领取通缉奖励结果 msgId:8168;
        GE_SC_GiveupTongJiResult, //服务器通知：返回放弃通缉结果 msgId:8169;
        GE_SC_RefreshTongJiList, //服务器通知：自动刷新通缉列表 msgId:8170;
        GE_SC_GetTongJiBoxResult, //服务器通知：返回获取通缉宝箱结果 msgId:8171;
        GE_SC_TongJiRefreshState, //服务器通知：刷新悬赏状态结果 msgId:8522
        GE_CC_TongJiUpdate, //客户端之间推送更新通缉信息
        //邮件
        GE_GetMailResult,//返回邮件列表 msgId:7032;
        GE_OpenMailResult,// 返回打开邮件 msgId:7033;
        GE_GetMailItemResult,// 请求领取附件返回 msgId:7034;
        GE_DelMail,// 请求删除邮件返回 msgId:7035;
        GE_NotifyMail,// 邮件提醒 msgId:7036;
        //竞技场
        GE_SC_ARENA_ENTER,//进入战斗
        GE_SC_ARENA_INFO,//竞技场信息
        GE_SC_ARENA_LIST,//挑战列表
        GE_SC_ARENA_CHALLENGE_RESULT,//挑战结果
        GE_SC_ARENA_REWARD_RESULT,//领取奖励结果
        GE_SC_ARENA_RECORD,//战报
        GE_SC_ARENA_BUY_TIMES,//购买次数
        GE_SC_ARENA_BUY_CD,//购买CD
        GE_AREAN_SHOW_FIGHT,//显示战斗UI
        GE_ARENA_HP_REFRESH,//竞技场血条刷新
        GE_SC_ARENA_RANK_CHANGE,//竞技场排名变化 
        //掉落拾取
        GE_Drop_Item,
        //爵位
        GE_CS_GuanZhiInfo, //客户端请求：官职信息 msgId：3689；
        GE_CS_InterServiceContValue, //请求服务器功勋 msgId：3810；
        GE_CS_GuanZhiLevelUp, //客户端请求：官职升级 msgId:3688；
        GE_SC_InterServiceContValue, //返回服务器功勋 msgId：8810
        GE_SC_GuanZhiInfo, //服务器返回：官职信息 msgId：8689；
        GE_SC_GuanZhiLevelUp, //服务器返回：官职升级 msgId：8688；

        GE_CS_HuoYueReward,   //客户端请求：活跃奖励 msgId:3262;
        GE_SC_HuoYueReward,   //返回获取活跃奖励结果 msgId:8262;
        GE_SC_HuoYueDuFinish,  //返回活跃度任务完成一次 msgId:8261;
        GE_SC_HuoYueDu,    //返回活跃度信息 msgId:8260;
        GE_CC_PlayerTitleUpdate,    //客户端之间推送  爵位Panel相关信息
        //经验副本
        GE_WaterDungeonEnterResult, // 服务器返回:进入流水副本结果 msgId:8439;
        GE_WaterDungeonExitResult,// 服务器返回:退出流水副本结果 msgId:8440;
        GE_WaterDungeonInfo,// 服务器返回:流水副本信息 msgId:8434;
        GE_WaterDungeonProgress,// 服务器返回:流水副本进度 msgId:8436;
        GE_WaterDungeonResult,// 服务器返回:流水副本结算 msgId:8437;


        //占星
        GE_NeiGongInfo, // 服务器返回: 经脉信息 msgId:8665;
        GE_OpenNode,// 服务器返回:冲穴结果 msgId:8666;
        //秘境
        GE_ResSimpleSecrectDuplInfo,// 返回单人秘境副本面板信息 msgId:8637;
        GE_ResEnterSimpleSecrectDupl,// 返回进入单人秘境副本 msgId:8638;
        GE_SimpleSecrectDuplTrace,// 秘境副本追踪面饭信息 msgId:8639;
        GE_SimpleSecrectDuplCom,// 单人秘境副本结算 msgId:8640;
        GE_ResExitSimpleSecrectDupl,// 返回退出单人秘境副本 msgId:8641;
        GE_UpdateSecrectDuplTili,// 更新组队或次数 msgId:8642;
        GE_ResJiHuoSecrectDupl, // 返回激活结果 msgId:8643;
        GE_SecretDungeonSweep,// 返回:个人秘境副本扫荡 msgId:8967;
        GE_SecretDungeonSweepReward,// 返回:个人秘境副本扫荡领奖励 msgId:8968;
        GE_SecretDungeonSweepInfo,// 返回:个人秘境副本扫荡 msgId:8969;
        GE_QueryMonsterPostion_Start,       //获取视野外怪物点坐标
        GE_QueryMonsterPostion_End,         //返回视野外怪物点坐标

        //宝塔秘境
        GE_EnterTreasureDupl, // 服务器返回：进入宝塔秘境 msgId:9951;
        GE_QuitTreasureDupl,// 服务器返回：退出宝塔秘境 msgId:9950;
        GE_FindTreasureInfo,// 寻宝信息 msgId:8475;
        GE_FindTreasureResult,// 服务器返回:寻宝任务接取 结果 msgId:8476;
        GE_FindTreasureCancel,// 服务器返回:取消寻宝任务 msgId:8477;
        GE_FindTreasureCollect,// 服务器返回:接取结果 msgId:8478;
        GE_TreasureTodayAddedTimer, // 返回今日已经购买的时间 msgId:8676;
        GE_TreasureRemainTime,// 打宝塔剩余时间 msgId:8677;
        GE_TreasureUpdateBoss,// 更新BOSS状态信息 msgId:8678;

        //查询怪物数量
        GE_QueryMonsterByPosition,
        //功能列表
        GE_FunctionList ,
        GE_FunctionOpen ,

        //活动
        GE_CW_WorldBoss,   //请求世界BOSS列表 msgId:2063
        GE_WC_WorldBoss,   //返回世界BOSS列表(刷新时推单个) msgId:7064
        GE_WC_ActivityState, //返回活动状态(刷新时推单个) msgId:7065
        GE_CS_ActivityEnter,  //请求:进入活动 msgId:3159;
        GE_SC_ActivityEnter, 	// S->C 返回:进入活动 msgId:8159
        GE_CS_ActivityQuit,   // C->S 请求:退出活动 msgId:3160;
        GE_SC_ActivityQuit,  // S->C 返回:退出活动 msgId:8160
        GE_SC_Activity,  //	//  S->C 登录返回活动列表 msgId:8158
        GE_SC_ActivityFinish, // S->C 返回:活动结束(活动内玩家) msgId:8161
        GE_SC_WorldBossDamage, // S->C 返回玩家累计伤害 msgId:8162
        GE_SC_WorldBossHurt, //S->C 返回世界BOSS伤害信息(活动内) msgId:8163
        GE_SC_FieldBoss,  //S->C  返回金币BOSS伤害信息(野外) msgId:8737
        GE_CS_WaBaoList,  //C->S 请求:挖宝列表信息 msgId:3806;
        GE_SC_WaBaoList,  //S->C 返回Boss挖宝信息 msgId:8807
        GE_CS_GetWaBaoReward, //C->S 请求:领取对应BOSS奖励 msgId:3805;
        GE_SC_GetWaBaoReward,  //S->C 领取对应BOSS奖励 msdId:8808

        GE_CC_ActivityUpdate,  //客户端之间推送活动页更新
        GE_CC_WorldBossUpdate,  //客户端之间推送世界BOSS页更新
        GE_CC_WorldBossDamageUpdate, //客户端之间推送世界BOSS地图玩家伤害值及排名更新
        GE_CC_WorldBossGetReward, //客户端之间推送世界BOSS探宝结果更新    

        GE_SC_TimeDungeonRoomInfo,//秘境，组队副本，创建房间结果
        GE_SC_EnterDulpPrepare,//秘境，组队副本，进入组队副本提示准备消息
        GE_SC_TeamTargetData,//组队目标数据

        GS_SC_TRIGGERTRAP,//陷阱触发

        GE_CS_ZhenBaoGe,  //请求珍宝阁数据 msgId：3113；
        GE_CS_ZhenBaoGeSubmit,  //珍宝阁提交道具 msgId：3114；
        GE_CS_ZhenBaoSpeItem,  //珍宝阁提交特殊道具 msgId：3115；
        GE_SC_ZhenBaoGe,   //返回珍宝阁数据，数据刷新时也返回这个 msgId：8113；
        GE_CC_ZhenBaoGeUpdate, //客户端之间推送珍宝阁更新信息

        GE_CS_EquipGroup,   //客户端请求：设置装备套装 msgId:3465;
        GE_SC_EquipGroup,  // 服务器返回:设置装备套装 msgId:8465;
        GE_CS_EquipGroupTwo,   //客户端请求：设置装备套装 msgId:3564;
        GE_SC_EquipGroupTwo,  // 服务器返回:设置装备套装 msgId:8565;
        GE_CS_EquipSmelt, //客户端请求:装备熔炼 msgId:3514
        GE_SC_EquipSmelt, //服务器返回:装备熔炼结果 msgId:8923
        GE_CS_EquipGroupPeel,  //客户端请求：剥离装备套装 msgId:3558
        GE_SC_EquipGroupPeel, //服务器返回:剥离装备套装 msgId:8558
        GE_CS_EquipGroupLvlUp, //客户端请求：套装升级 msgId:3685
        GE_SC_EquipGroupLvlUp, //服务器返回:套装升级 msgId:8685
        GE_CC_MyEquipSelectClick, //客户端之间推送:  装备套装UI页，选中自己穿戴的装备
        GE_CC_EquipGroupUpdate,  //客户端之间推送：装备套装属性变化更新

        GE_EquipInherit,  // 返回装备传承 msgId:8150;
        GE_EquipPeiYang,//返回装备培养
        GE_EquipPeiYangSet, //返回保存装备培养
        GE_SC_GuildQuestSweep, //帮派任务一键扫荡

        GE_SC_DailyActivy, //返回日常活动开启列表 msg:9953
        GE_CC_DailyActivy, //客户端之间刷新日常活动列表
        GE_CC_DailyActivyTaskUpdate, //客户端直接推送任务栏-日常活动类型显示更新

        GE_CC_GameGraphicSetting,

        GE_CC_StoryStart,                   //剧情动画开始
        GE_CC_StoryEnd,                     //剧情动画结束
        GE_CC_StoryBossName,                //剧情动画显示Boss名称

        GE_SHOW_BATTER,                     //显示主战斗模型
        GE_XY_RECHARGE_MSG,                   //西游网平台服务器返回订单创建结果
        GE_XY_PAY_MSG,                        //订单支付回调接口
        GE_XY_SERVERLIST,                   //西游网平台返回服务器列表
        GE_XY_VerifyAccount,      //西游验证返回消息

        GE_DYB_RECHARGE_MSG,                   //第一拨平台服务器返回充值结果
        GE_DYB_SERVERLIST,                    //第一拨平台返回服务器列表


        GE_SQW_RECHARGE_MSG,                   //37玩平台服务器返回充值结果
        GE_SQW_SERVERLIST,                    //37玩平台返回服务器列表

        GE_SETTING_CHANGE,
        GE_SETTING_ACTORDISPLAYNUM,

        GE_CC_ChangeHeroTitle, //改变玩家英雄称号
        GE_CC_ChangeFacion, //阵营变化

        GE_CC_CrossConnect, //跨服连接，1.断开本服，连接跨服，2.活动结束，断开跨服，连接本服

        GE_CC_MyTakeAction, //主动采取行动，比如点击地面，点击角色，控制方向盘，释放技能等

        GE_Bundle_Group_BeginDowload, //ABList里分包Bundle组开始下载
        GE_Bundle_Group_Downloaded,    //ABList里分包Bundle组下载完成
        GE_Bundle_Group_Download_Progress, //ABList里分包Bundle组下载进度
        GE_AddExpInfo, //s->c 经验信息 msgId:11180
        // event数量
        NUM_EVENTS,
    }

    [LuaCallCSharp]
[Hotfix]
    public class EventParameter : CacheObject
    {
        public int intParameter;			// 整数参数   
        public int intParameter1;			// 整数参数   
        public int intParameter2;			// 整数参数
        public long longParameter;			// 整数参数
        public long longParameter1;			// 整数参数
        public long longParameter2;			// 整数参数
        public string stringParameter;		// 字符串参数
        public float floatParameter;		// 浮点参数
        public GameObject goParameter;		// 游戏对象参数
        public GameObject goParameter1;		// 游戏对象参数
        public MsgData msgParameter;         // 网络消息参数
        
        public object objParameter;		// 对象参数
        public object objParameter1;		// 对象参数1
        public object objParameter2;    //对象参数2

        public bool autoRecycle = true;        //是否自动回收

        public EventParameter() { }

        public override void OnRecycle()
        {
            Reset();
        }

        public void Reset()
        {
            intParameter = 0;
            intParameter1 = 0;
            intParameter2 = 0;
            longParameter = 0;
            floatParameter = 0;
            stringParameter = string.Empty;
            goParameter = null;
            goParameter1 = null;
            msgParameter = null;
            objParameter = null;
            objParameter1 = null;
            objParameter2 = null;
            autoRecycle = true;
        }

        public static EventParameter Get()
        {
            return _cache.Get();
        }

        public static EventParameter Get(MsgData msg)
        {
            EventParameter ep = _cache.Get();
            ep.msgParameter = msg;
            return ep;
        }

        public static EventParameter Get(int intparam)
        {
            EventParameter ep = _cache.Get();
            ep.intParameter = intparam;
            return ep;
        }

        public static EventParameter Get(int intparam1, int intparam2)
        {
            EventParameter ep = _cache.Get();
            ep.intParameter1 = intparam1;
            ep.intParameter2 = intparam2;
            return ep;
        }

        public static EventParameter Get(long longparam1, long longparam2)
        {
            EventParameter ep = _cache.Get();
            ep.longParameter1 = longparam1;
            ep.longParameter2 = longparam2;
            return ep;
        }

        public static EventParameter Get(float floatparam)
        {
            EventParameter ep = _cache.Get();
            ep.floatParameter = floatparam;
            return ep;
        }

        public static EventParameter Get(string param)
        {
            EventParameter ep = _cache.Get();
            ep.stringParameter = param;
            return ep;
        }

        public static void Cache(EventParameter ep)
        {
            _cache.Cache(ep);
        }

        private static CachePool<EventParameter> _cache = new CachePool<EventParameter>(20);
    }
}