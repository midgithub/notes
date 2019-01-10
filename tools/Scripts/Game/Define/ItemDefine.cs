/**
* @file     : ItemDefine.cs
* @brief    : 物品相关定义。
* @details  : 
* @author   : XuXiang
* @date     : 2017-06-29 14:18
*/

using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;

namespace SG
{
    /// <summary>
    /// 背包类型。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public static class BagType
    {
        public static int ITEM_BAG_TYPE_COMMON = 0;
        public static int ITEM_BAG_TYPE_EQUIP = 1;
        public static int ITEM_BAG_TYPE_STORAGE = 2;
        public static int ITEM_BAG_TYPE_RIDE = 3;
        public static int ITEM_BAG_TYPE_ROLEITEM = 4;  //翅膀
        public static int ITEM_BAG_TYPE_LINGSHOU = 5;
        public static int ITEM_BAG_TYPE_LSHORSE = 6;
        public static int ITEM_BAG_TYPE_GEM = 7;  //宝石背包
        public static int TIEM_BAG_TYPE_MAGICKEY = 8;//法宝
        public static int ITEM_BAG_TYPE_PIFENG = 9;//披风
        public static int ITEM_BAG_TYPE_LUCKY = 10;//财神赐福
        public static int ITEM_BAG_TYPE_RIDEWAR = 11;//骑战武器
        public static int ITEM_BAG_TYPE_BAOJIA = 12;//宝甲
        public static int ITEM_BAG_TYPE_TIANGANG = 13; //天罡
        public static int ITEM_BAG_TYPE_ZHANNU = 14;   //战弩
        public static int ITEM_BAG_TYPE_MINGLUN = 15;  //命轮
        public static int ITEM_BAG_TYPE_HUNQI = 16;    //魂器
        public static int ITEM_BAG_TYPE_SHENGQI = 17;  //圣器
        public static int ITEM_BAG_TYPE_STORAGE_2 = 18;
        public static int ITEM_BAG_TYPE_STORAGE_3 = 19;
        public static int ITEM_BAG_TYPE_STORAGE_4 = 20;
        public static int ITEM_BAG_TYPE_QILINBI = 21;  //麒麟臂
        public static int ITEM_BAG_TYPE_JIANYU = 22;   //剑域
        public static int ITEM_BAG_TYPE_HERO = 23;   //魔神装备
        public static int ITEM_BAG_TYPE_SEHNQIFIGHT = 24;   //神器争夺
        public static int ITEM_BAG_TYPE_NUM = 25;
    }

    /// <summary>
    /// 货币类型。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public static class CurrencyType
    {
        public static int CURRENCY_TYPE_BIND_GOLD = 0;          //绑定金币
        public static int CURRENCY_TYPE_UNBIND_GOLD = 1;        //非绑定金币
        public static int CURRENCY_TYPE_BIND_MONEY = 2;         //绑定元宝
        public static int CURRENCY_TYPE_UNBIND_MONEY = 3;       //非绑定元宝
        public static int CURRENCY_TYPE_HONOR_MONEY = 4;       //荣誉值;
        public static int CURRENCY_TYPE_LingZhi_MONEY = 5;       //灵值; //侠义值//骑士勋章
        public static int CURRENCY_TYPE_Rongyao_MONEY = 6;       //荣耀币
        public static int CURRENCY_TYPE_NUMBER = 7;             //货币类型数量

        // <summary>
        /// 基础属性类型对应的名称。
        /// </summary>
        private static Dictionary<sbyte, int> StatTypeToCurrencyType = new Dictionary<sbyte, int>()
        {
            {(sbyte)EStatType.eaBindGold , CURRENCY_TYPE_BIND_GOLD},
            {(sbyte)EStatType.eaUnBindGold , CURRENCY_TYPE_UNBIND_GOLD},
            {(sbyte)EStatType.eaBindMoney , CURRENCY_TYPE_BIND_MONEY},
            {(sbyte)EStatType.eaHonor , CURRENCY_TYPE_HONOR_MONEY},
            {(sbyte)EStatType.eaLingZhi , CURRENCY_TYPE_LingZhi_MONEY},
            {(sbyte)EStatType.eaUnBindMoney , CURRENCY_TYPE_UNBIND_MONEY},
            {(sbyte)EStatType.eaRongyao , CURRENCY_TYPE_Rongyao_MONEY},

        };

        /// <summary>
        /// 从服务器的属性类型映射到基础类型。
        /// </summary>
        /// <param name="type">服务器的属性类型。</param>
        /// <returns>基础类型，非基础类型返回Unkonw。</returns>
        public static int GetCurrencyTypeFromStatType(sbyte type)
        {
            int ret;
            if (StatTypeToCurrencyType.TryGetValue(type, out ret))
            {
                return ret;
            }
            return -1;
        }
    }

    /// <summary>
    /// 属性丹类型。
    /// </summary>
[Hotfix]
    public static class AttrDanType
    {
        public static int Ride = 1;
        public static int LingShou = 2;
        public static int ShenBing = 3;
        public static int LingZhen = 4;
        public static int RideWar = 5;
        public static int PiFeng = 6;
        public static int ShengLing = 7;
        public static int PiFengPer = 8;
        public static int BaoJia = 9;
        public static int BaoJiaPer = 10;
        public static int TianGang = 11;
        public static int TianGangPer = 12;
        public static int ZhanNu = 13;
        public static int ZhanNuPer = 14;
        public static int RidePer = 15;
        public static int ShenBingPer = 16;
        public static int RideWarPer = 17;
        public static int WuXing = 18;
        public static int MingLun = 19;
        public static int MingLunPer = 20;
        public static int HunQi = 21;
        public static int HunQiPer = 22;
        public static int WuXingPer = 23;
        public static int ShengQi = 24;
        public static int ShengQiPer = 25;
        public static int ZhenFa = 26;
        public static int ZhenFaPer = 27;
        public static int Qilinbi = 28;
        public static int QilinbiPer = 29;
        public static int JianYu = 30;
        public static int JianYuPer = 31;
    };

    // 玩家外观发生改变类型
    public enum EModelChange
    {
        MODEL_CHANGE_NONE,
        MODEL_CHANGE_FACE,          // 变脸
        MODEL_CHANGE_HAIR,          // 头发
        MODEL_CHANGE_DRESS,         // 衣服
        MODEL_CHANGE_WEAPON,          // 武器
        MODEL_CHANGE_WUHUN,         // 武魂
        MODEL_CHANGE_RIDE,          // 坐骑
        MODEL_CHANGE_ZAZEN,         // 打坐
        MODEL_CHANGE_EQUIP_TITLE,   // 穿称号
        MODEL_CHANGE_UNEQUIP_TITLE, // 卸称号
        MODEL_CHANGE_PK_STATUS,     // PK状态;
        MODEL_CHANGE_FASHION_WING,          // 时装-翅膀
        MODEL_CHANGE_FASHION_DRESS,          // 时装-衣服
        MODEL_CHANGE_FASHION_WEAPON,        // 时装-武器
        MODEL_CHANGE_SHENBING,      // 神兵变化
        MODEL_CHANGE_FACTION,       // 阵营变化
        MODEL_LINGZHI,              // 灵值
        MODEL_CHANGE_REALM,         // 境界变化
        MODEL_CHANGE_ACTPET,        // 萌宠变化
        MODEL_CHANGE_WING,          // 翅膀变化
        MODEL_CHANGE_VPLAN = 27,    // VPlan
        MODEL_CHANGE_GROUP_SUIT,    // 套装变化
        MODEL_CHANGE_FOOTPRINTS,    // 脚印
        MODEL_CHANGE_MAGICKEY = 30, // 法宝
        MODEL_CHANGE_GLORYLEVEL = 31,// 修为等级
        MODEL_CHANGE_MAGICKEYQUALITY = 32, // 法宝品质
        MODEL_CHANGE_PIFENG = 33,       // 披风变化
        MODEL_CHANGE_CROSS_SCORE,   //跨服BOSS积分
        MODEL_CHANGE_GUANZHI = 35,      // 官职变化
        MODEL_CHANGE_GUILDPOS = 36,     // 帮派职位变化
        MODEL_CHANGE_HONGYAN = 37,      //红颜变化
        MODEL_CHANGE_MILITARY_RANK, //军衔
        MODEL_CHANGE_BAOJIA, // 宝甲变化
        MODEL_CHANGE_TIANGANG,      // 天罡变化
        MODEL_CHANGE_ZHANNU,        //战弩
        MODEL_CHANGE_MINGLUN,       //命轮
        MODEL_CHANGE_HUNQI,     //魂器
        MODEL_CHANGE_SHENGQI,           //圣器
        MODEL_CHANGE_MAGICKEY_AWAKE,    //法宝觉醒
        MODEL_CHANGE_ZHUANSHENG,        //转生
        MODEL_CHANGE_ZHENFA,        //阵法
        MODEL_CHANGE_SHENWU,        //神武
        MODEL_CHANGE_MAGICKEY_FEISHENG,        //法宝飞升
        MODEL_CHANGE_JIANYU,    //剑域
        MODEL_CHANGE_FASHION,    // 是否隐藏时装
        MODEL_CHANGE_EQUIPSTAR = 52,    // 装备升星模型
        MODEL_CHANGE_MAGICKEY_STAR = 53, // 法宝星级

    }

    /// <summary>
    /// 玩家装备位。
    /// </summary>
[Hotfix]
    public static class PlayerEquipPos
    {
        public static int Weapon = 0;            //武器
        public static int Shoulder = 1;            //护肩
        public static int Cloth = 2;         //衣服
        public static int Belt = 3;          //腰带
        public static int Pant = 4;            //裤子
        public static int Shoes = 5;         //鞋子
        public static int Gloves = 6;            //护手
        public static int Necklace = 7;          //项链
        public static int Accessory = 8;         //饰品
        public static int RingL = 9;			//戒指
    }

[Hotfix]
    public static class RideEquipPos
    {
        public static int Saddlery = 0;           //鞍具
        public static int Rein = 1;           //僵绳
        public static int HeadWear = 2;           //头饰
        public static int Settle = 3;           //磴具
        public static int EnumToPosOffset = -20;     //通用枚举值到装备位偏移
    }
}


/*
 
	eLSXiangQuan = 30,           //灵兽项圈
	eLSKaiJia    = 31,           //灵兽铠甲
	eLSHuWan     = 32,           //灵兽护腕
	eLSTouShi    = 33,           //灵兽头饰

	eLSHorseXQ   = 40,			 //灵兽坐骑项圈
	eLSHorseKJ   = 41,			 //灵兽坐骑铠甲
	eLSHorseHW   = 42,			 //灵兽坐骑护腕
	eLSHorseTS   = 43,			 //灵兽坐骑头饰

	eQilinbi1   = 55,			 //麒麟臂配饰1
	eQilinbi2   = 56,			 //麒麟臂配饰2
	eQilinbi3   = 57,			 //麒麟臂配饰3
	eQilinbi4   = 58,			 //麒麟臂配饰4

	ePiFeng1   = 60,			 //披风配饰1
	ePiFeng2   = 61,			 //披风配饰2
	ePiFeng3   = 62,			 //披风配饰3
	ePiFeng4   = 63,			 //披风配饰4

	eShengQi1   = 65,			 //圣器配饰1
	eShengQi2   = 66,			 //圣器配饰2
	eShengQi3   = 67,			 //圣器配饰3
	eShengQi4   = 68,			 //圣器配饰4

	eRideWarX  = 70,
	eRideWarR  = 71,
	eRideWarB  = 72,
	eRideWarH  = 73,

	eHunQi1   = 75,			 //魂器配饰1
	eHunQi2   = 76,			 //魂器配饰2
	eHunQi3   = 77,			 //魂器配饰3
	eHunQi4   = 78,			 //魂器配饰4

	eBaoJia1   = 80,			 //宝甲配饰1
	eBaoJia2   = 81,			 //宝甲配饰2
	eBaoJia3   = 82,			 //宝甲配饰3
	eBaoJia4   = 83,			 //宝甲配饰4

	eMingLun1   = 85,			 //命轮1
	eMingLun2   = 86,			 //命轮2
	eMingLun3   = 87,			 //命轮3
	eMingLun4   = 88,			 //命轮4

	eTianGang1   = 90,			 //天罡配饰1
	eTianGang2   = 91,			 //天罡配饰2
	eTianGang3   = 92,			 //天罡配饰3
	eTianGang4   = 93,			 //天罡配饰4

	eZhanNu1	 = 95,			 //战弩1
	eZhanNu2     = 96,			 //战弩2
	eZhanNu3	 = 97,			 //战弩3
	eZhanNu4	 = 98,			 //战弩4
     
     
     */

