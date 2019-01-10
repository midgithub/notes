using XLua;
﻿using behaviac;
using System.Collections.Generic;
using System;



namespace SG
{
    public enum EStatType
    {
        // 战斗属性
        //eaName = 1,
        eaProf = 2,     //职业;
        eaSex = 3,
        eaVipLevel = 4,     //Vip等级;
        eaZone = 5,     //区服ID;
        eaLevel = 6,        //等级;
        eaExp = 7,      //角色当前经验,可以消耗减少;
        eaLeftPoint = 8,        //角色剩余属性点    角色当前可用来增加4个一级属性的属性点;
        eaTotalPoint = 9,       //角色总属性点      角色累计获得的总属性点;
        eaBindGold = 10,        //绑定金币          角色在游戏内的货币，游戏内产出及消耗，数值可极大,不可交易;
        eaUnBindGold = 11,      //非绑定金币        非绑定金币可替代绑定金币的全部功能,可交易;
        eaUnBindMoney = 12,     //元宝              可通过特殊途径交易，普通交易不可交易;
        eaBindMoney = 13,       //礼金              和绑定金币类似的货币， 可购买绑定的道具，不可交易;
        eaZhenQi = 14,      //灵力              角色在游戏内的另一种代币，数值可极大， 不可交易;

        //战斗属性;
        eaHunLi = 15,       //魂力              影响角色攻击力;
        eaTiPo = 16,        //体魄              主要影响角色生命上限，次要影响角色防御;
        eaShenFa = 17,      //身法              主要影响角色命中和闪避，次要影响爆击和韧性;
        eaJingShen = 18,        //精神              主要影响角色爆击和韧性，次要影响命中和闪避;

        eaHp = 19,      //生命值;
        eaMaxHp = 20,       //生命上限;
        eaHpRe = 21,    //生命恢复速度     暂时没用 角色生命值恢复速度，每30秒恢复一次,;

        eaMp = 22,      //内力值            角色当前内力值，释放技能需要消耗该值;
        eaMaxMp = 23,       //内力上限;
        eaMpRe = 24,    //内力恢复速度      角色内力恢复速度，每30秒恢复一次;os

        eaSp = 25,      //体力值            角色体力值，释放体力值技能需要消耗该值;
        eaMaxSp = 26,       //体力值上限;
        eaSpRe = 27,        //体力恢复速度      角色体力值恢复速度，每30秒恢复一次;

        eaAtk = 28,     //攻击力            角色攻击力，带入伤害公式计算伤害时使用;
        eaDef = 29,     //防御力            角色防御力，带入伤害公式计算伤害时使用;
        eaHit = 30,     //命中              角色命中值，带入命中公式计算是否命中;
        eaDodge = 31,       //闪避              角色闪避值，带入命中公式计算是否命中;
        eaCri = 32,     //爆击              角色爆击值，带入爆击公式计算是否爆击;
        eaDefCri = 33,      //韧性              角色韧性，带入爆击公式计算是否爆击;

        eaAtkSpeed = 34,        //攻击速度         角色攻击速度，影响角色攻击间隔及技能公共CD间隔;
        eaSpeed = 35,       //移动速度           影响角色移动速度;

        eaCriValue = 36,        //爆伤              正整数，显示为百分比，例如：200%;带入伤害公式计算，影响角色爆击后的伤害值;
        eaSubCri = 37,      //免爆            正整数，显示为百分比，例如：200%;带入伤害公式计算，影响角色被爆击后的伤害值;

        eaAbsAtt = 38,      //穿刺             无视防御的伤害值，带入伤害公式计算伤害值;
        eaParryValue = 39,       //格挡值           角色格挡生效后减免的伤害值，带入伤害公式计算;
        eaDmgAdd = 40,      //伤害增强         正整数，显示为百分比，例如：50%;角色最终伤害增加的比例，带入伤害公式计算;
        eaDmgSub = 41,      //伤害减免         正整数，显示为百分比，例如：50%;角色最终伤害减免的比例，带入伤害公式计算;
        eaParryRate = 42,       //格挡率           正整数;
        eaWuHunSP = 43,     //武魂豆  角色武魂值，使用武魂技能协议消耗;
        eaMaxWuHunSP = 44,      //武魂豆最大上限;
        eaWuHunSPRe = 45,       //武魂豆恢复速度 角色武魂豆恢复速度，每5s恢复一次;
        eaPower = 46,       //战斗力;
        eaMultiKill = 47,       //连斩数;
        eaSubDef = 48,       //破防;
        eaDropVal = 49,       //活力值;
        eaPKVal = 50,       //PK值(善恶值);
        eaHonor = 51,       //荣誉值;
        eaSuper = 52,       //卓越一击几率
        eaSuperValue = 53,      //卓越以及伤害
        eaLingZhi = 54,     //灵值
        eaJingjieExp = 55,      //境界经验(现在修改为 披风魂力值,披风扩展功能使用)
        eaFatigue = 56,     //疲劳值, 打宝地宫
        eaEnergy = 57,       //主宰之路精力值(现在修改为 骑兵历练值,骑兵扩展功能使用)
        eaEquipVal = 58,        //装备打造活力值;
        eaExtremityVal = 59,        //极限副本积分(现在修改为 兵灵魂力值,神兵扩展功能使用);
        eaChargeMoney = 60,     //玩家充值钱数;
        eaCrossScore = 61,      //跨服积分;
        eaCrossExploit = 62,        //跨服PVP功勋;
        eaCrossZhanyi = 63,     //跨服副本战意值;
        eaPvpLv = 64,       //段位;
        eaZhuanSheng = 65,      //转生类型
        eaHpX = 66,     //最大生命百分比
        eaAtkX = 67,        //攻击百分比
        eaDefX = 68,        //防御百分比
        eaHitX = 69,        //命中百分比
        eaDodgeX = 70,      //闪避百分比
        eaCriX = 71,        //暴击百分比
        eaDefCriX = 72,     //韧性百分比
        eaAbsAttX = 73,     //穿刺百分比
        eaParryRateX = 74,      //格挡率百分比
        eaParryValueX = 75,     //格挡值百分比
        eaSubDefX = 76,     //破防百分比
        eaBossMedia = 77,       //屠魔点数
        eaMagickeyTili = 78,        //法宝体力
        eaSheild = 79,      //护盾

        eaGloryLevel = 80,      //荣耀等级;
        eaGloryExp = 81,        //荣耀经验;
        eaCuMoJiFen = 82,       //除魔积分
        eaGuildCont = 83,       //帮贡ID(101原来在CommonDefine.h)
        eaGuildLoyalty = 84,        //帮派忠诚ID(103原来在CommonDefine.h)	
        eaNengLiang = 85,       //能量值	
        eaLuckyPoint = 86,      //抽奖点数
        eaCrossSceneExploit = 87,   //跨服场景功勋
        eaMilitaryExploit = 88,     //军功
        eaHuoYueDu = 89,            //活跃度
        eaGoldatt = 90, //金攻
        eaGolddef = 91, //金防
        eaWateratt = 92,    //水攻
        eaWaterdef = 93,    //水防
        eaWoodatt = 94, //木攻
        eaWooddef = 95, //木防
        eaSoilatt = 96, //土攻
        eaSoildef = 97, //土防
        eaFireatt = 98, //火攻
        eaFiredef = 99, //火防
        eaCrossScore3 = 100,        //跨服3v3积分
        eaVertigo = 101,        //眩晕
        eaDefvertigo = 102,     //抗眩晕
        eaTalent = 103,     //天赋点数
        eaIntimate = 104,       //情缘值（占位：客户端用）
        eaShenwei = 105,        //神威
        eaDefSuper = 106,       //抗卓越一击几率
        eaDefSuperValue = 107,      //抗卓越以及伤害
        eaRongyao = 111,       //荣耀币;

        Max_Client_Attr,
    }
}



