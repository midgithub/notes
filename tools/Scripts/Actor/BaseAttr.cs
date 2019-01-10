
using UnityEngine;
using System.Collections;
using behaviac;
using System.Collections.Generic;
using System;

using XLua;

namespace SG
{
    /// <summary>
    /// 基础属性类型定义。
    /// </summary>
    [LuaCallCSharp]
    public enum BasicAttrEnum
    {
        Unkonw = 0,                     //未知属性
        Prof,                           //职业
        Gender,                         //性别
        VIPLevel,                       //VIP等级
        Level,                          //等级
        Exp,                            //经验
        PKValue,                        //善恶值
        Power,                          //战力
        Attack,                         //攻击
        Defence,                        //防御
        MaxHP,                          //生命上限
        CurHP,                          //当前生命
        Hit,                            //命中
        Dodge,                          //闪避
        Crit,                           //暴击
        Toughness,                      //韧性
        CritRatio,                      //暴伤害倍率
        CritRatioDecrease,              //暴伤害倍率减免
        DefenceIgnore,                  //破防
        Impale,                         //刺穿
        ParryProb,                      //格挡率
        ParryValue,                     //格挡值
        DamageIncrease,                 //伤害增强
        DamageDecrease,                 //伤害减免
        Palsy,                          //麻痹
        PalsyResist,                    //抗麻痹
        Speed,                          //速度
        ShenWei,                        //神威
        HuiXinRatio,                    //会心几率
        HuiXinDamage,                   //会心伤害
        JingJieLevel,                   //境界等级
        JingJieExp,                     //境界经验
        Max,                            //属性数量
    }

    /// <summary>
    /// 单位状态。
    /// </summary>
    public enum UnitBit
    {
        UNIT_BIT_MOVING = 1,            // 移动中
        UNIT_BIT_DEAD,                  // 死亡
        UNIT_BIT_CASTING,               // 施法中
        UNIT_BIT_GOD,                   // 无敌中
        UNIT_BIT_STEALTH,               // 隐身中
        UNIT_BIT_STIFF,                 // 硬直中
        UNIT_BIT_POISONED,              // 中毒中
        UNIT_BIT_HOLD,                  // 定身中
        UNIT_BIT_PALSY,                 // 麻痹中
        UNIT_BIT_STUN,                  // 眩晕中
        UNIT_BIT_SILENCE,               // 沉默中

        UNIT_BIT_INCOMBAT,              // 战斗中
        UNIT_BIT_INBACK,                // 归位中
        UNTT_BIT_CHANGE_SCENE,          // 切换场景中

        UNIT_BIT_FORBID_USERITEM,       // 禁用物品状态
        UNIT_BIT_FORBID_RIDE,           // 不可骑乘
        UNIT_BIT_FORBID_RECOVER_HP,     // 不可恢复生命
        UNIT_BIT_FORBID_SLOW,           // 免疫减速
        UNIT_BIT_FORBID_RECOVER_SP,     // 不可恢复体力
        UNIT_BIT_CERTAINLY_CRIT,        // 必定暴击
        UNIT_BIT_CERTAINLY_HIT,         // 必定命中

        UNIT_BIT_IN_PK,                 //PK状态;
        UNIT_BIT_IN_BATI,               //霸体-- 免疫控制
        UNIT_BIT_CREATE_SCENE,          //创建场景
        UNIT_BIT_RAMPAGE,               //狂暴
        UNIT_BIT_MIDNIGHT,              //午夜PK状态
        UNIT_BIT_REBIRTTH,              //重生状态
        UNIT_BIT_BEHEADED,              //斩杀
        UNIT_BIT_FOLLOW,                //跟随者
        UNIT_BIT_NOUSE_TILI,            //不消耗体力

        UNIT_BIT_FLY,                   //击飞
        UNIT_BIT_PROTECTED,		        //重生保护状态
        UNIT_BIT_CLOSE_LINK,            //客户端连接断开
        Max,                            //属性数量
    }


    //玩家基础数据
    [LuaCallCSharp]
[Hotfix]
    public class BaseAttr
    {
        // <summary>
        /// 基础属性类型对应的名称。
        /// </summary>
        private static Dictionary<int, string> BasicAttrNames = new Dictionary<int, string>()
        {
            {(int)BasicAttrEnum.Unkonw, "未知属性"},
            {(int)BasicAttrEnum.Prof, "职业"},
            {(int)BasicAttrEnum.Gender, "性别"},
            {(int)BasicAttrEnum.VIPLevel, "VIP等级"},
            {(int)BasicAttrEnum.Level, "等级"},
            {(int)BasicAttrEnum.Exp, "经验"},
            {(int)BasicAttrEnum.PKValue, "善恶值"},
            {(int)BasicAttrEnum.Power, "战力"},
            {(int)BasicAttrEnum.Attack, "攻击"},
            {(int)BasicAttrEnum.Defence, "防御"},
            {(int)BasicAttrEnum.MaxHP, "生命"},
            {(int)BasicAttrEnum.CurHP, "当前生命"},
            {(int)BasicAttrEnum.Hit, "命中"},
            {(int)BasicAttrEnum.Dodge, "闪避"},
            {(int)BasicAttrEnum.Crit, "暴击"},
            {(int)BasicAttrEnum.Toughness, "韧性"},
            {(int)BasicAttrEnum.CritRatio, "暴伤害倍率"},
            {(int)BasicAttrEnum.CritRatioDecrease, "暴伤害倍率减免"},
            {(int)BasicAttrEnum.DefenceIgnore, "破防"},
            {(int)BasicAttrEnum.Impale, "刺穿"},
            {(int)BasicAttrEnum.ParryProb, "格挡率"},
            {(int)BasicAttrEnum.ParryValue, "格挡值"},
            {(int)BasicAttrEnum.DamageIncrease, "伤害增强"},
            {(int)BasicAttrEnum.DamageDecrease, "伤害减免"},
            {(int)BasicAttrEnum.Palsy, "麻痹"},
            {(int)BasicAttrEnum.PalsyResist, "抗麻痹"},
            {(int)BasicAttrEnum.Speed, "速度"},
            {(int)BasicAttrEnum.ShenWei, "神威"},
            {(int)BasicAttrEnum.HuiXinRatio, "会心几率"},
            {(int)BasicAttrEnum.HuiXinDamage, "会心伤害"},
        };

        /// <summary>
        /// 获取基础属性名称。
        /// </summary>
        /// <param name="t">属性类型。</param>
        /// <returns>属性名称。</returns>
        public static string GetBasicAttrName(int t)
        {
            string re = null;
            BasicAttrNames.TryGetValue(t, out re);
            if (re == null)
            {
                re = "未知属性:" + t;
            }
            return re;
        }

        // <summary>
        /// 基础属性类型对应的名称。
        /// </summary>
        private static Dictionary<sbyte, int> StatTypeToBasicAttrType = new Dictionary<sbyte, int>()
        {
            {(sbyte)EStatType.eaProf , (int)BasicAttrEnum.Prof},
            {(sbyte)EStatType.eaSex , (int)BasicAttrEnum.Gender},
            {(sbyte)EStatType.eaVipLevel , (int)BasicAttrEnum.VIPLevel},
            {(sbyte)EStatType.eaLevel , (int)BasicAttrEnum.Level},
            {(sbyte)EStatType.eaExp , (int)BasicAttrEnum.Exp},
            {(sbyte)EStatType.eaPKVal , (int)BasicAttrEnum.PKValue},
            {(sbyte)EStatType.eaPower , (int)BasicAttrEnum.Power},
            {(sbyte)EStatType.eaAtk , (int)BasicAttrEnum.Attack},
            {(sbyte)EStatType.eaDef , (int)BasicAttrEnum.Defence},
            {(sbyte)EStatType.eaMaxHp , (int)BasicAttrEnum.MaxHP},
            {(sbyte)EStatType.eaHp , (int)BasicAttrEnum.CurHP},
            {(sbyte)EStatType.eaHit , (int)BasicAttrEnum.Hit},
            {(sbyte)EStatType.eaDodge , (int)BasicAttrEnum.Dodge},
            {(sbyte)EStatType.eaCri , (int)BasicAttrEnum.Crit},
            {(sbyte)EStatType.eaDefCri , (int)BasicAttrEnum.Toughness},
            {(sbyte)EStatType.eaCriValue , (int)BasicAttrEnum.CritRatio},
            {(sbyte)EStatType.eaSubCri , (int)BasicAttrEnum.CritRatioDecrease},
            {(sbyte)EStatType.eaSubDef , (int)BasicAttrEnum.DefenceIgnore},
            {(sbyte)EStatType.eaAbsAtt , (int)BasicAttrEnum.Impale},
            {(sbyte)EStatType.eaParryRate , (int)BasicAttrEnum.ParryProb},
            {(sbyte)EStatType.eaParryValue , (int)BasicAttrEnum.ParryValue},
            {(sbyte)EStatType.eaDmgAdd , (int)BasicAttrEnum.DamageIncrease},
            {(sbyte)EStatType.eaDmgSub , (int)BasicAttrEnum.DamageDecrease},
            {(sbyte)EStatType.eaShenwei , (int)BasicAttrEnum.ShenWei},
            {(sbyte)EStatType.eaVertigo , (int)BasicAttrEnum.Palsy},
            {(sbyte)EStatType.eaDefvertigo , (int)BasicAttrEnum.PalsyResist},
            {(sbyte)EStatType.eaSpeed , (int)BasicAttrEnum.Speed},
            {(sbyte)EStatType.eaSuper , (int)BasicAttrEnum.HuiXinRatio},
            {(sbyte)EStatType.eaSuperValue , (int)BasicAttrEnum.HuiXinDamage},
            {(sbyte)EStatType.eaGloryLevel , (int)BasicAttrEnum.JingJieLevel},
            {(sbyte)EStatType.eaGloryExp , (int)BasicAttrEnum.JingJieExp},
        };

        /// <summary>
        /// 从服务器的属性类型映射到基础类型。
        /// </summary>
        /// <param name="type">服务器的属性类型。</param>
        /// <returns>基础类型，非基础类型返回Unkonw。</returns>
        public static BasicAttrEnum GetBasicAttrTypeFromStatType(sbyte type)
        {
            int ret;
            if (StatTypeToBasicAttrType.TryGetValue(type, out ret))
            {
                return (BasicAttrEnum)ret;
            }
            return BasicAttrEnum.Unkonw;
        }

        /// <summary>
        /// 获取或设置职业。
        /// </summary>
        public int Prof
        {
            get { return (int)mBasicAttrs[(int)BasicAttrEnum.Prof]; }
            set { mBasicAttrs[(int)BasicAttrEnum.Prof] = value; }
        }

        /// <summary>
        /// 获取或设置性别。
        /// </summary>
        public int Gender
        {
            get { return (int)mBasicAttrs[(int)BasicAttrEnum.Gender]; }
            set { mBasicAttrs[(int)BasicAttrEnum.Gender] = value; }
        }

        /// <summary>
        /// 获取或设置VIP等级。
        /// </summary>
        public int VIPLevel
        {
            get { return (int)mBasicAttrs[(int)BasicAttrEnum.VIPLevel]; }
            set { mBasicAttrs[(int)BasicAttrEnum.VIPLevel] = value; }
        }

        /// <summary>
        /// 获取或设置等级。
        /// </summary>
        public int Level
        {
            get { return (int)mBasicAttrs[(int)BasicAttrEnum.Level]; }
            set { mBasicAttrs[(int)BasicAttrEnum.Level] = value; }
        }


        /// <summary>
        /// 世界等级
        /// </summary>
        public int WorldLevel;
       
        /// <summary>
        /// 获取或设置经验。
        /// </summary>
        public long Exp
        {
            get { return (long)mBasicAttrs[(int)BasicAttrEnum.Exp]; }
            set { mBasicAttrs[(int)BasicAttrEnum.Exp] = value; }
        }

        /// <summary>
        /// 获取或设置善恶值。
        /// </summary>
        public int PKValue
        {
            get { return (int)mBasicAttrs[(int)BasicAttrEnum.PKValue]; }
            set { mBasicAttrs[(int)BasicAttrEnum.PKValue] = value; }
        }

        /// <summary>
        /// 获取或设置战力。
        /// </summary>
        public long Power
        {
            get { return (long)mBasicAttrs[(int)BasicAttrEnum.Power]; }
            set { mBasicAttrs[(int)BasicAttrEnum.Power] = value; }
        }

        /// <summary>
        /// 获取或设置攻击。
        /// </summary>
        public int Attack
        {
            get { return (int)mBasicAttrs[(int)BasicAttrEnum.Attack]; }
            set { mBasicAttrs[(int)BasicAttrEnum.Attack] = value; }
        }

        /// <summary>
        /// 获取或设置防御。
        /// </summary>
        public int Defence
        {
            get { return (int)mBasicAttrs[(int)BasicAttrEnum.Defence]; }
            set { mBasicAttrs[(int)BasicAttrEnum.Defence] = value; }
        }

        /// <summary>
        /// 获取或设置生命上限。
        /// </summary>
        public long MaxHP
        {
            get { return (long)mBasicAttrs[(int)BasicAttrEnum.MaxHP]; }
            set { mBasicAttrs[(int)BasicAttrEnum.MaxHP] = value; }
        }

        /// <summary>
        /// 获取或设置生命。
        /// </summary>
        public long CurHP
        {
            get { return (long)mBasicAttrs[(int)BasicAttrEnum.CurHP]; }
            set { mBasicAttrs[(int)BasicAttrEnum.CurHP] = value; }
        }

        /// <summary>
        /// 获取或设置命中。
        /// </summary>
        public float Hit
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.Hit]; }
            set { mBasicAttrs[(int)BasicAttrEnum.Hit] = value; }
        }

        /// <summary>
        /// 获取或设置闪避。
        /// </summary>
        public float Dodge
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.Dodge]; }
            set { mBasicAttrs[(int)BasicAttrEnum.Dodge] = value; }
        }

        /// <summary>
        /// 获取或设置暴击率。
        /// </summary>
        public float Crit
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.Crit]; }
            set { mBasicAttrs[(int)BasicAttrEnum.Crit] = value; }
        }

        /// <summary>
        /// 获取或设置韧性。
        /// </summary>
        public float Toughness
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.Toughness]; }
            set { mBasicAttrs[(int)BasicAttrEnum.Toughness] = value; }
        }

        /// <summary>
        /// 获取或设置暴击伤害倍率。
        /// </summary>
        public float CritRatio
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.CritRatio]; }
            set { mBasicAttrs[(int)BasicAttrEnum.CritRatio] = value; }
        }

        /// <summary>
        /// 获取或设置被暴击时伤害倍率减免。
        /// </summary>
        public float CritRatioDecrease
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.CritRatioDecrease]; }
            set { mBasicAttrs[(int)BasicAttrEnum.CritRatioDecrease] = value; }
        }

        /// <summary>
        /// 获取或设置破防。
        /// </summary>
        public float DefenceIgnore
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.DefenceIgnore]; }
            set { mBasicAttrs[(int)BasicAttrEnum.DefenceIgnore] = value; }
        }

        /// <summary>
        /// 获取或设置刺穿。
        /// </summary>
        public float Impale
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.Impale]; }
            set { mBasicAttrs[(int)BasicAttrEnum.Impale] = value; }
        }

        /// <summary>
        /// 获取或设置格挡率。
        /// </summary>
        public float ParryProb
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.ParryProb]; }
            set { mBasicAttrs[(int)BasicAttrEnum.ParryProb] = value; }
        }

        /// <summary>
        /// 获取或设置格挡值。
        /// </summary>
        public float ParryValue
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.ParryValue]; }
            set { mBasicAttrs[(int)BasicAttrEnum.ParryValue] = value; }
        }

        /// <summary>
        /// 获取或设置格伤害增强。
        /// </summary>
        public float DamageIncrease
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.DamageIncrease]; }
            set { mBasicAttrs[(int)BasicAttrEnum.DamageIncrease] = value; }
        }

        /// <summary>
        /// 获取或设置格伤害减免。
        /// </summary>
        public float DamageDecrease
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.DamageDecrease]; }
            set { mBasicAttrs[(int)BasicAttrEnum.DamageDecrease] = value; }
        }

        /// <summary>
        /// 获取或设置格麻痹。
        /// </summary>
        public float Palsy
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.Palsy]; }
            set { mBasicAttrs[(int)BasicAttrEnum.Palsy] = value; }
        }

        /// <summary>
        /// 获取或设置抗麻痹。
        /// </summary>
        public float PalsyResist
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.PalsyResist]; }
            set { mBasicAttrs[(int)BasicAttrEnum.PalsyResist] = value; }
        }

        /// <summary>
        /// 获取或设置速度。
        /// </summary>
        public float Speed
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.Speed]; }
            set { mBasicAttrs[(int)BasicAttrEnum.Speed] = value; }
        }

        /// <summary>
        /// 获取或设置神威。
        /// </summary>
        public float ShenWei
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.ShenWei]; }
            set { mBasicAttrs[(int)BasicAttrEnum.ShenWei] = value; }
        }

        /// <summary>
        /// 获取或设置会心几率。
        /// </summary>
        public float HuiXinRatio
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.HuiXinRatio]; }
            set { mBasicAttrs[(int)BasicAttrEnum.HuiXinRatio] = value; }
        }

        /// <summary>
        /// 境界等级
        /// </summary>
        public int JingJieLevel
        {
            get { return (int)mBasicAttrs[(int)BasicAttrEnum.JingJieLevel]; }
            set { mBasicAttrs[(int)BasicAttrEnum.JingJieLevel] = value; }
        }

        /// <summary>
        /// 境界经验
        /// </summary>
        public long JingJieExp
        {
            get { return (long)mBasicAttrs[(int)BasicAttrEnum.JingJieExp]; }
            set { mBasicAttrs[(int)BasicAttrEnum.JingJieExp] = value; }
        }

        /// <summary>
        /// 获取或设置会心伤害。
        /// </summary>
        public float HuiXinDamage
        {
            get { return (float)mBasicAttrs[(int)BasicAttrEnum.HuiXinDamage]; }
            set { mBasicAttrs[(int)BasicAttrEnum.HuiXinDamage] = value; }
        }
        
        /// <summary>
        /// 获取属性数组。
        /// </summary>
        public double[] Attrs
        {
            get { return mBasicAttrs; }
        }

        /// <summary>
        /// 基础属性值。
        /// </summary>
        protected double[] mBasicAttrs = new double[(int)BasicAttrEnum.Max];
        
        /// <summary>
        /// 清除基础属性。
        /// </summary>
        public void ClearBasicAttr()
        {
            for (int i=0; i< mBasicAttrs.Length; ++i)
            {
                mBasicAttrs[i] = 0;
            }
        }

        /// <summary>
        /// 获取属性值。
        /// </summary>
        /// <param name="t">属性类型。</param>
        /// <returns>属性值，不存在的属性返回0。</returns>
        public double GetBasicAttrValue(int t)
        {
            if (t <= (int)BasicAttrEnum.Unkonw || t >= (int)BasicAttrEnum.Max)
            {
                return 0;
            }

            return mBasicAttrs[t];
        }

        /// <summary>
        /// 设置属性值。
        /// </summary>
        /// <param name="t">属性类型。</param>
        /// <param name="v">属性值。</param>
        public void SetBasicAttrValue(int t, double v)
        {
            if (t <= (int)BasicAttrEnum.Unkonw || t >= (int)BasicAttrEnum.Max)
            {
                return;
            }

            mBasicAttrs[t] = v;
        }

        /// <summary>
        /// 添加属性值。
        /// </summary>
        /// <param name="t">属性类型。</param>
        /// <param name="v">属性值。</param>
        public void AddBasicAttrValue(int t, double v)
        {
            if (t <= (int)BasicAttrEnum.Unkonw || t >= (int)BasicAttrEnum.Max)
            {
                return;
            }

            mBasicAttrs[t] += v;
        }

        /// <summary>
        /// 状态位。
        /// </summary>
        protected bool[] mUnitBits = new bool[(int)UnitBit.Max];

        /// <summary>
        /// 获取状态值。
        /// </summary>
        /// <param name="bit">状态类型。</param>
        /// <returns>状态值。</returns>
        public bool GetUnitBit(int bit)
        {
            if (bit <= 0 || bit >= (int)UnitBit.Max)
            {
                return false;
            }
            return mUnitBits[bit];
        }

        /// <summary>
        /// 设置状态值。
        /// </summary>
        /// <param name="bit">状态类型。</param>
        /// <param name="set">状态值。</param>
        public void SetUnitBit(int bit, bool set)
        {
            if (bit <= 0 || bit >= (int)UnitBit.Max)
            {
                return;
            }
            mUnitBits[bit] = set;
        }

        /// <summary>
        /// 名称。
        /// </summary>
        protected string mName;

        /// <summary>
        /// 获取或设置名称。
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// 玩家爵位id
        /// </summary>
        private int mLordId;

        /// <summary>
        /// 玩家爵位id
        /// </summary>
        public int Lord
        {
            get
            {
                return mLordId;
            }
            set
            {
                mLordId = value;
            }

        }

        /// <summary>
        /// 所在队伍ID。
        /// </summary>
        public long TeamID { get; set; }

        /// <summary>
        /// 所在帮派ID。
        /// </summary>
        public long GuildID { get; set; }

        /// <summary>
        /// 通过怪物进场数据初始化属性。
        /// </summary>
        /// <param name="es">进场数据。</param>
        public void InitMonsterAttr(MsgData_sSceneObjectEnterMonster mes)
        {
            SetBasicAttrValue((int)BasicAttrEnum.Speed, CommonTools.ServerValueToClient(mes.Speed));
            SetBasicAttrValue((int)BasicAttrEnum.MaxHP, mes.MaxHp);
            SetBasicAttrValue((int)BasicAttrEnum.CurHP, mes.CurHp);
        }

        /// <summary>
        /// 通过角色进场数据初始化属性。
        /// </summary>
        /// <param name="es">进场数据。</param>
        public void InitOtherPlayerAttr(MsgData_sSceneObjectEnterHuman hes)
        {
            int server;
            mName = PlayerData.GetPlayerName(UiUtil.GetNetString(hes.RoleName), out server);
            VIPLevel = hes.VipLv;
            mLordId = hes.GuanZhi;
            SetBasicAttrValue((int)BasicAttrEnum.Prof, hes.Prof);
            SetBasicAttrValue((int)BasicAttrEnum.Level, hes.Level);
            SetBasicAttrValue((int)BasicAttrEnum.Gender, hes.Sex);
            SetBasicAttrValue((int)BasicAttrEnum.Speed, CommonTools.ServerValueToClient(hes.Speed));
            SetBasicAttrValue((int)BasicAttrEnum.MaxHP, hes.MaxHp);
            SetBasicAttrValue((int)BasicAttrEnum.CurHP, hes.CurHp);
            Dress = hes.Dress;
            EquipStarMin = hes.EquipStarMin;
            Weapon = hes.Weapon;
            FashionDress = hes.FashionDress;
            FashionWeapon = hes.FashionWeapon;
            ShenBing = hes.ShenBin;
            FashionState = hes.FashionState;
            TeamID = hes.TeamID;
            GuildID = hes.GuildID;
            HeroTitle = hes.Title[0];
        }

        public void InitEnterVirtualAttr(MsgData_sSceneObjectEnterVirtualPlayer hes)
        {
            int server;
            mName = PlayerData.GetPlayerName(UiUtil.GetNetString(hes.Name), out server);
            VIPLevel = 0;
            mLordId = 0;
            SetBasicAttrValue((int)BasicAttrEnum.Prof, hes.Job);
            SetBasicAttrValue((int)BasicAttrEnum.Level, hes.Level);
            SetBasicAttrValue((int)BasicAttrEnum.Gender, hes.Gender);
            //SetBasicAttrValue((int)BasicAttrEnum.Speed, CommonTools.ServerValueToClient(hes.Speed));
            SetBasicAttrValue((int)BasicAttrEnum.MaxHP, hes.HP);
            SetBasicAttrValue((int)BasicAttrEnum.CurHP, hes.HP);
            //SetBasicAttrValue((int)BasicAttrEnum.CurHP, hes.HP);
            Dress = hes.Dress;
            EquipStarMin = 0;
            Weapon = hes.Weapon;
            FashionDress = hes.FashionDress;
            FashionWeapon = hes.FashionWeapon;
            //ShenBing = hes.ShenBin;
            //FashionState = hes.FashionState;
            //TeamID = hes.TeamID;
            GuildID = hes.Guid;
            //HeroTitle = hes.Title[0];
        }

        /// <summary>
        /// 初始化玩家自身数据。
        /// </summary>
        public void InitFromPlayerData()
        {
            BaseAttr playerattr = PlayerData.Instance.BaseAttr;
            double[] attrs = playerattr.Attrs;
            for (int i = 0; i < attrs.Length; ++i)
            {
                mBasicAttrs[i] = attrs[i];
            }
            Dress = playerattr.Dress;
            EquipStarMin = playerattr.EquipStarMin;
            Weapon = playerattr.Weapon;
            ShenBing = playerattr.ShenBing;
            FashionDress = playerattr.FashionDress;
            FashionWeapon = playerattr.FashionWeapon;
            FashionState = playerattr.FashionState;
            ShenBing = playerattr.ShenBing;
            mName = PlayerData.Instance.Name;
            mLordId = playerattr.Lord;
        }

        /// <summary>
        /// 翅膀。
        /// </summary>
        public int Wing { get; set; }

        /// <summary>
        /// 衣服
        /// </summary>
        public int Dress { get; set; }
        /// <summary>
        /// 装备最小星级
        /// </summary>
        public int EquipStarMin { get; set; }
        /// <summary>
        /// 武器
        /// </summary>
        public int Weapon { get; set; }

        /// <summary>
        /// 时装武器
        /// </summary>
        public int FashionWing { get; set; }

        /// <summary>
        /// 时装武器
        /// </summary>
        public int FashionWeapon { get; set; }

        /// <summary>
        /// 时装衣服
        /// </summary>
        public int FashionDress { get; set; }

        /// <summary>
        /// 时装状态
        /// </summary>
        public int FashionState { get; set; }

        /// <summary>
        /// 神兵
        /// </summary>
        public int ShenBing { get; set; }  
        
        /// <summary>
        /// 称号ID
        /// </summary>      
        public int HeroTitle { get; set; }

        /// <summary>
        /// 阵法ID
        /// </summary>
        public int ZhenFa { get; set; }

        /// <summary>
        /// 阵营，用于玩家混战
        /// </summary>
        public int Faction { get; set; }
    }
}

