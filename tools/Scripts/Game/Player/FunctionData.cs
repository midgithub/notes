using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;
using System;


namespace SG
{
    [LuaCallCSharp]
    public enum FunctionType
    {
        None = 0,
        Skill = 1, //技能
        Team = 2, //队伍
        Frindes = 3, //好友        
        Ride = 5,   //坐骑
        Guild = 6,  //帮派
        Riding = 7, //骑乘 
        Zhenbao = 10, //珍宝
        Stren = 12,   //强化
        Dupl = 13,  //副本
        Fengyao = 14,
        WorldBoss = 15, //世界boss
        Activity = 16, //活动
        Arena = 17,     //竞技场
        Cave = 18, //仙缘洞府
        Babel = 19, //斗破苍穹
        Timing = 20,   //灵光界
        ShenBing = 21,  //神兵
        WelfareHall = 22, //福利大厅
        Vitality = 23,    //官职
        Extremity = 24,    //极限挑战
        Jingjie = 25,
        ResourceRecovery = 29, //资源找回
        Ascendant = 30, //升星
        Baojia = 36,    //宝甲
        Fashion = 37,  //时装
        PropsSynthesis = 38,  //道具合成
        WaterDup = 39, //经验副本
        HuiZhang = 40,
        LianTi = 41,
        ZhuZaiRoad = 44, //主宰之路
        EquipmentToStrengthen = 47, //装备强化（现为铸炼）
        EquipCreate = 48,
        LingZhen = 50,
        ActPet = 52,
        SevenDaysLogin = 53, //七日登录
        Consignment = 56,
        XunBao = 57,
        Qiyu = 58,
        Homeland = 59,
        ZhuoYueYinDao = 60,    //卓越引导
        BingHun = 62,  //兵魂
        VIP = 63,  //VIP
        Smelt = 65,
        LingShouHorse = 67,
        Exchange = 70,
        NewGroup = 71,//套装
        PersonBoss = 72, //个人boss
        RideWar = 73,  //骑战兵器
        RideBattleFB = 74,
        BossMedia = 75,
        Palace = 76,   //帮派地宫
        ZhuoyueWash = 77, //卓越重铸
        WingDupl = 78, //翅膀副本
        EquipStepUp = 79,//装备升阶
        PreciousStones = 80, //宝石镶嵌
        MagicKey = 81, //魔宠
        MagicKeyMake = 82,//法宝打造
        MagicKeyTrain = 83,//法宝培养
        SecretDupl = 88,   //深渊魔域(秘境副本)
        Wash = 90,     //装备洗练
        Book = 91,     //深造（经书）
        LingshouMD = 93,   // 灵兽墓地;（现为：法宝灵台）
        Func_ZhuZaiRoad = 94, //名将挑战
        Beauty = 96,  //美人坊
        FabaoDecompose = 97, //法宝分解
        NeiGong = 98, //占星
        Fuwen = 99,        //技能符文
        Role = 100,     //人物
        Bag = 101,      //背包
        Shop = 102,     //商城
        ColorStone = 103,  //七彩灵石
        EquipColorStone = 104,  // 七彩灵石镶嵌
        LvUpColorStone = 105,   // 七彩灵石升级
        SlayerQuset = 106,      // 除魔任务
        TreasureDulp = 107,       // 打宝塔
        DailyQuest = 108,       //日环任务
        NewGroupLvl = 109, //新套装升级	
        GroupPos = 110, //装备位套装
        DaTang = 111, // 大唐奇遇 （现为：组队秘境）
        ExchangeShop = 112, //兑换商店
        shuxingboli = 113, // 稀有属性剥离
        shuxingmingke = 114, // 装备铭刻
        shuxingjinglian = 115, // 装备精炼
        activateshenbing = 117, //激活神兵
        GuildQuest = 118,          //帮派任务
        AgainstQuest = 119,       //讨伐任务
        HongYan = 120,         //红颜（现为：幻灵）
        PiFeng = 122, //披风（现为：圣盾）
        MoYuDiLao = 123, //魔域地牢
        NewCrossBoss = 124,    //新跨服boss  
        ActivateActPet = 126, // 宠物     
        Draw = 128,        //天官赐福 （现为：大唐宝库）
        HunLingXianYu = 129,   // 幻灵仙域; （现为：披风幻境）
        LiCai = 130,           // 投资理财;
        ZhiYuanFb = 131, //资源副本
        PersonalBoss = 132, //个人Boss副本
        ZhuoQiFb = 133, //坐骑副本
        HongYanFb = 134, //红颜副本
        ShengBinFb = 135, //神兵副本
        CrossBoss = 136, //跨服BOSS
        CrossArena = 137, //跨服擂台
        CrossTask = 138,//跨服战场
        TianGangFb = 139, //天罡副本
        TianGang = 140,//天罡
        CrossFlag = 141,   //跨服夺旗
        MagicKey_Up = 142, //法宝升级
        MagicKey_UpStar = 143, //法宝升星
        MagicKey_Gop = 144, //法宝仙灵
        ZhanNu = 145, //战弩
        MagicKey_ZuHe = 149,   //法宝组合
        PeiYang = 150,     //装备培养
        ChuanCheng = 151,  //装备传承	
        MagicKey_LingWu = 155, //法宝技能领悟
        Marry = 156, //结婚
        LunPan = 157, //轮盘
        WuXing = 158,  //五行
        MagicFix = 159,    //法宝修复
        MingLun = 160, //命轮
        HunQi = 161,//魂器
        Sbbingling = 164, //神兵兵灵
        TaoZhuang_JiHuo = 165, //套装激活
        HourseGem = 166,       //坐骑宝石
        CrossPvp3 = 167, //跨服3V3
        PiFengExt = 168, //披风扩展	
        ShengQi = 169,     //圣器
        MagicKey_ChuanCheng = 172, //法宝传承
        Star = 173,    //星图
        EquipRed = 174, //装备变红
        EquipRedUp = 175, //装备变红升级
        EquipRedInherit = 176, //装备变红传承
        WingExt = 177, //翅膀扩展
        ZhanNuGem = 178,       //战弩扩展
        RideWarExt = 179,      //骑兵扩展
        Magickey_Awake = 181,  //法宝觉醒
        CrossPvp = 182, //跨服1V1
        TianGangExt = 183, //天罡扩展
        ZhuanSheng = 184,  //转职
        TianFu = 185,  //天赋
        ErZhuan = 186, //二转
        MarryQuest = 187,       //结婚日环任务
        ZhenFa = 188,  //阵法
        ShenWu = 191, //神武
        XueTong = 192, //血统
        Sbjianyi = 193, //神兵剑意
        FengYaoTa = 194,   //封妖塔
        Shenwuext = 195, //神武淬炼
        Baojiaext = 196, //宝甲扩展
        FeiSheng = 197, //法宝飞升
        Wuxingext = 198, //灵阵扩展
        CrossDupl = 199,   //跨服组队副本
        Qilinbi = 200, //麒麟臂
        Take = 201,     //拾取
        Hook = 202,     //挂机

        Minglungem = 203,      //命轮宝石
        Magickey_SkillUp = 204,    //法宝被动技能强化
        EquipBless = 206, //装备神佑
        EquipBlessUp = 207, //装备神佑升级
        EquipBlessInherit = 208, //装备神佑传承
        FeiSheng_ChuanCheng = 209, //飞升传承
        JianYu = 210,  //剑域
        PiFengQiXing = 211, //披风七星功能开启
        XianYu = 212, //仙域 
        HolySprit = 213, //圣灵

        DailyEntrance = 214, //日常入口
        Leaderboards = 215, //排行榜   组队
        RideSkin = 216, //坐骑皮肤     排行榜
        RideSkill = 217, //坐骑技能
        RideEquip = 218, //坐骑装备
        WingAchieve = 219, //翅膀翎羽
        RoleTitle = 220,  //人物称号
        HongYanMerry = 221,  //幻灵合体
        ShenBingSkill = 222,  //神兵技能
        ShieldSkill = 223,  //圣盾技能
        ShieldEquip = 224, //圣盾装备
        DailyTime = 225,   //日常限时
        DailyTitle = 226,  //日常爵位
        Recharge = 227, //充值

        BagShop = 228, //背包商店
        Depot = 229, //仓库
        LucyDraw = 230,//魔宠抽卡
        DailyRecharge = 232, //每日充值
        //NewWing = 233, //新翅膀
        Wing = 233, //战翼
        DoubleExp = 236, //双倍经验
        DailyAnswer = 237, //每日答题
        CaiLiaoFb = 247, //秘宝探险
        ZhuangBeiFb = 248, //炽魔战场
        EQUIPVIP = 249, //vip装备
        Madness = 250, //装备狂魔
        Shengyou = 251, //神佑装备
    }

    [LuaCallCSharp]
    [Hotfix]
    public class FunctionData
    {
        public float functionOpenTime = 0;  //系统开启UI显示持续时间

        /// <summary>
        /// 系统开启列表。
        /// </summary>
        private SortedDictionary<int, bool> openList = new SortedDictionary<int, bool>();

        public void OnInit(MsgData msg)
        {
            openList.Clear();
            MsgData_sFunctionList functionListMsg = msg as MsgData_sFunctionList;
            for (int i = 0; i < functionListMsg.functionList.Count; i++)
            {
                int id = functionListMsg.functionList[i];
                if (!openList.ContainsKey(id))
                {
                    openList.Add(id, true);
                }
            }
           // 触发功能列表初始化事件
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FunctionList, EventParameter.Get(msg));
        }
        public void OnFunctionOpen(MsgData msg)
        {
            MsgData_sFunctionOpen funtionOpenMsg = msg as MsgData_sFunctionOpen;
            int id = funtionOpenMsg.functionID;
            if (!openList.ContainsKey(id))
            {
                openList.Add(id, true);
            }

           // 触发功能开启事件
            EventParameter ep = EventParameter.Get();
            ep.intParameter = funtionOpenMsg.functionID;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FunctionOpen, ep);
        }
        public bool IsFunctionOpen(FunctionType functionType)
        {
            return IsFunctionOpen((int)functionType);
        }

        public bool IsFunctionOpen(int fid)
        {
            return fid == (int)FunctionType.None || ConfigManager.Instance.Common.IsDefaultOpen(fid)
                || ConfigManager.Instance.Common.IsLevelOpen(fid) || openList.ContainsKey(fid);
        }

        /// <summary>
        /// 获取下一个开启的功能。
        /// </summary>
        public int NextOpenFunc
        {
            get
            {
                List<int> FuncOpenOrder = ConfigManager.Instance.Common.FuncOpenOrder;
                for (int i = 0; i < FuncOpenOrder.Count; ++i)
                {
                    int id = FuncOpenOrder[i];
                    if (!IsFunctionOpen(id))
                    {
                        return id;
                    }
                }
                return 0;
            }
        }

        public void RegisterNetMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, OnCleanupUserData);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FUNCTION_LIST, OnInit);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_FUNCTION_OPEN, OnFunctionOpen);
        }

        /// <summary>
        ///  切换账号清除数据
        /// </summary>
        private void OnCleanupUserData(GameEvent ge, EventParameter parameter)
        {
            functionOpenTime = 0;
            openList.Clear();
            //圣盾的数据清除 --临时这样写吧坑
            ShieldMgr.Instance.ReSet();
        }

        public void UnregisterNetMsg()
        {
            CoreEntry.netMgr.unbindMsgHandler(NetMsgDef.S_FUNCTION_LIST);
            CoreEntry.netMgr.unbindMsgHandler(NetMsgDef.S_FUNCTION_OPEN);
        }
    }
}