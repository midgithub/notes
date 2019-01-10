/**
* @file     : PlayerData.cs
* @brief    : 
* @details  : 文件用途说明
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;

namespace SG
{
    /// <summary>
    /// 战斗力类型。
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public static class ModleType
    {
        public const int FightChangeHorse = 1;     //坐骑;
        public const int FightChangeMagickey = 2;     //法宝;
        public const int FightChangeWing = 3;     //翅膀;
        public const int FightChangeHongYan = 4;     //红颜;
        public const int FightChangeMagicWeapon = 5;     //神兵;
        public const int FightChangeHolyShield = 6;            //圣盾;
        public const int FightChangeLord = 7;     //爵位;
        public const int FightChangeAstrology = 8;   //占星;
        public const int FightChangeSkill = 9;   //技能;
        public const int FightChangeFashion = 10;   //时装;
        public const int FightChangeTreasure = 11; //珍宝
    }

    [LuaCallCSharp]
    [Hotfix]
    public class PlayerData
    {
        private static PlayerData _instance = null;

        /// <summary>
        /// 获取玩家数据。
        /// </summary>
        public static PlayerData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlayerData();
                }
                return _instance;
            }
        }

        [CSharpCallLua]
        public delegate void MemberFunctionCall(LuaTable tb);

        /// <summary>
        /// 注册网络消息。
        /// </summary>
        public void RegisterNetMsg()
        {
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_TELEPORT_FREE_TIME, OnTeleportFreeTime); 
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_MEINFO, OnInitData);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_WORLDLEVEL, OnInitWorldlevel);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_MODLE_FIGHT_CHANGE, OnPowerChange);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_SET_PK_RULE, OnSetPKRule);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_MAINPLAYER_ENTER_SCENE, OnGameEventEnterScene);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BAG_INFO, OnBagChange);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BAG_ITEM_ADD, OnBagChange);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BAG_ITEM_DEL, OnBagChange);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BAG_ITEM_UPDATE, OnBagChange);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, OnCleanupUserData);

            mRideData.RegisterNetMsg();
            mBagData.RegisterNetMsg();
            mSkillData.RegisterNetMsg();
            //mShopData.RegisterNetMsg();
            mTeamData.RegisterNetMsg();
            mGuildData.RegisterNetMsg();
            mResLevelData.RegisterNetMsg();
            mJingJieData.RegisterNetMsg();
            mChatData.RegisterNetMsg();
            mEquipMgr.ResisterMsg();
            mMagicKeyMgr.ResisterMsg();
            mLevelFuBenDataMgr.ResisterMsg();
            mBossFuBenDataMgr.ResisterMsg();
            ExpFuBenDataMgr.ResisterMsg();
            mNeiGonginDataMgrr.ResisterMsg();
            mStoneData.RegisterNetMsg();
            mBeautyWomanMgr.ResisterMsg();
            mCrownData.RegisterNetMsg();
            mMailDataMgr.ResisterMsg();
            mFashionData.RegisterNetMsg();
            mFriendData.RegisterNetMsg();
            mSecretDuplDataMgr.ResisterMsg();
            mTreasureFuBenDataMgr.ResisterMsg();
            funcData.RegisterNetMsg();

            //Lua层注册
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            LuaTable table = G.Get<LuaTable>("ModelManager");
            MemberFunctionCall fun = table.Get<MemberFunctionCall>("RegisterNetMsg");
            if (fun != null)
            {
                fun(table);
            }
        }
        /// <summary>
        /// 解绑网络消息
        /// </summary>
        public void UnregisterNetMsg()
        {
            funcData.UnregisterNetMsg();
        }

        /// <summary>
        ///  切换账号清除数据
        /// </summary>
        private void OnCleanupUserData(GameEvent ge, EventParameter parameter)
        {
            //Lua层
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            LuaTable table = G.Get<LuaTable>("ModelManager");
            MemberFunctionCall fun = table.Get<MemberFunctionCall>("Reset");
            if (fun != null)
            {
                fun(table);
            }
            //邮件
            mMailDataMgr.ReSet();
            mStoneData.ClearData();
            mBasicAttr.ClearBasicAttr();
            mBasicAttr.Name = " ";
            mRoleID = 0;
            UnbindMoney = 0;
            Account.Instance.ServerId = 0;
            Account.Instance.ServerName = "no";
            BossFuBenDataMgr.Instance.Reset();
        }

        #region----------------------------------------基本信息----------------------------------------

        //职业
        public int Job
        {
            set { mBasicAttr.Prof = value;}
            get { return mBasicAttr.Prof; }
        }

        /// <summary>
        /// 角色ID。
        /// </summary>
        private long mRoleID;

        /// <summary>
        /// 获取角色ID。
        /// </summary>
        public long RoleID
        {
            get { return mRoleID; }
        }

        /// <summary>
        /// 头像。
        /// </summary>
        private int mIcon;

        /// <summary>
        /// 获取角色头像。
        /// </summary>
        public int Icon
        {
            get { return mIcon; }
        }

        /// <summary>
        /// 翅膀
        /// </summary>
        public int Wing
        {
            get { return mBasicAttr.Wing; }
        }


        /// <summary>
        /// 阵法
        /// </summary>
        public int ZhenFa
        {
            get { return mBasicAttr.ZhenFa; }
        }

        /// <summary>
        /// 阵营
        /// </summary>
        public int Faction
        {
            get { return mBasicAttr.Faction; }
        }

        /// <summary>
        /// 衣服
        /// </summary>
        public int Dress
        {
            get { return mBasicAttr.Dress; }
        }

        /// <summary>
        /// 武器
        /// </summary>
        public int Weapon
        {
            get { return mBasicAttr.Weapon; }
        }

        /// <summary>
        /// 时装翅膀
        /// </summary>
        public int FashionWing
        {
            get { return mBasicAttr.FashionWing; }
        }

        /// <summary>
        /// 时装衣服
        /// </summary>
        public int FashionDress
        {
            get { return mBasicAttr.FashionDress; }
        }

        /// <summary>
        /// 时装武器
        /// </summary>
        public int FashionWeapon
        {
            get { return mBasicAttr.FashionWeapon; }
        }

        /// <summary>
        /// 时装状态
        /// </summary>
        public int FashionState
        {
            get { return mBasicAttr.FashionState; }
        }

        /// <summary>
        /// 神兵幻化id
        /// </summary>
        public int ShenBingId;
      

        /// <summary>
        /// 服务器分组。
        /// </summary>
        private int mServerGroup;

        /// <summary>
        /// 获取服务器分组。
        /// </summary>
        public int ServerGroup
        {
            get { return mServerGroup; }
        }

        /// <summary>
        /// 获取角色名称。
        /// </summary>
        public string Name
        {
            get { return mBasicAttr.Name; }
        }

        /// <summary>
        /// 寻路目标点。
        /// </summary>
        private Vector3 mPathTarget;

        /// <summary>
        /// 获取寻路目标点。
        /// </summary>
        public Vector3 PathTarget
        {
            get { return mPathTarget; }
        }

        /// <summary>
        /// 玩家的货币。
        /// </summary>
        private long[] mCurrency = new long[CurrencyType.CURRENCY_TYPE_NUMBER];

        /// <summary>
        /// 获取玩家货币。
        /// </summary>
        /// <param name="type">货币类型。</param>
        /// <returns>货币数量。</returns>
        public long GetCurrency(int type)
        {
            if (type < 0 || type >= mCurrency.Length)
            {
                LogMgr.ErrorLog("Unkown currency type {0}", type);
                return 0;
            }
            return mCurrency[type];
        }

        /// <summary>
        /// 根据服务器类型获取货币
        /// </summary>
        /// <param name="serverType"></param>
        /// <returns></returns>
        public long GetCurrencyByStatType(int serverType)
        {
            int type = CurrencyType.GetCurrencyTypeFromStatType((sbyte)serverType);

            return GetCurrency(type);
        }

        /// <summary>
        /// 设置货币。
        /// </summary>
        /// <param name="type">货币类型。</param>
        /// <param name="num">货币数量。</param>
        public void SetCurrency(int type, long num)
        {
            if (type < 0 || type >= mCurrency.Length)
            {
                LogMgr.ErrorLog("Unkown currency type {0}", type);
                return ;
            }
            mCurrency[type] = num;
        }

        /// <summary>
        /// 获取或设置绑定金币数量。
        /// </summary>
        public long BindGold
        {
            get { return mCurrency[CurrencyType.CURRENCY_TYPE_BIND_GOLD]; }
            set { mCurrency[CurrencyType.CURRENCY_TYPE_BIND_GOLD] = value; }
        }
        //灵值; //侠义值//骑士勋章
        public long LingZhi
        {
            get { return mCurrency[CurrencyType.CURRENCY_TYPE_LingZhi_MONEY]; }
            set { mCurrency[CurrencyType.CURRENCY_TYPE_LingZhi_MONEY] = value; }
        }
        /// <summary>
        /// 获取或设置非绑定金币数量。
        /// </summary>
        public long UnbindGold
        {
            get { return mCurrency[CurrencyType.CURRENCY_TYPE_UNBIND_GOLD]; }
            set { mCurrency[CurrencyType.CURRENCY_TYPE_UNBIND_GOLD] = value; }
        }

        /// <summary>
        /// 荣耀币。
        /// </summary>
        public long Rongyao
        {
            get { return mCurrency[CurrencyType.CURRENCY_TYPE_Rongyao_MONEY]; }
            set { mCurrency[CurrencyType.CURRENCY_TYPE_Rongyao_MONEY] = value; }
        }

        /// <summary>
        /// 获取或设置绑定元宝数量。
        /// </summary>
        public long BindMoney
        {
            get { return mCurrency[CurrencyType.CURRENCY_TYPE_BIND_MONEY]; }
            set { mCurrency[CurrencyType.CURRENCY_TYPE_BIND_MONEY] = value; }
        }

        /// <summary>
        /// 获取或设置非绑定元宝数量。
        /// </summary>
        public long UnbindMoney
        {
            get { return mCurrency[CurrencyType.CURRENCY_TYPE_UNBIND_MONEY]; }
            set { mCurrency[CurrencyType.CURRENCY_TYPE_UNBIND_MONEY] = value; }
        }
        //荣誉币数量
        public long HonorMoney
        {
            get { return mCurrency[CurrencyType.CURRENCY_TYPE_HONOR_MONEY]; }
            set { mCurrency[CurrencyType.CURRENCY_TYPE_HONOR_MONEY] = value; }
        }
        /// <summary>
        /// 发送使用属性丹请求。
        /// </summary>
        public void SendUseAttrDanRequest(int type)
        {
            MsgData_cUseAttrDan data = new MsgData_cUseAttrDan();
            data.Type = type;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_USE_ATTR_DAN, data);
        }

        /// <summary>
        /// 移动到某点。
        /// </summary>
        /// <param name="end">目标点。</param>
        /// <param name="mapId">场景id。</param>
        /// <param name="near">附近容错半径。</param>
        /// <returns>是否可移动。</returns>
        public bool MoveToPos(Vector3 end, int mapId = 0, int near = 1)
        {
			//Debug.LogError ("====移动到某点===="+ mapId + "=="+end );
            ActorObj ag = CoreEntry.gActorMgr.GetActorByServerID(RoleID);
            if (mapId == 0)
            {
                mapId = MapMgr.Instance.EnterMapId;
            }

            Vector3 to;
            if (!SceneDataMgr.Instance.GetNearPoint(mapId, ag.transform.position, end, out to, near))
            {
                return false;
            }
            mPathTarget = to;
            MulScenesPathFinder.Instance.StartPathFinder(mapId, to);
            //ag.ReqRideHorse();          
            return true;
        }

        /// <summary>
        /// 获取玩家对象。
        /// </summary>
        /// <returns>玩家对象。</returns>
        public Transform GetPlayer()
        {
            ActorObj ag = CoreEntry.gActorMgr.GetActorByServerID(RoleID);
            return ag == null ? null : ag.transform;
        }

        /// <summary>
        /// 免费传送次数。
        /// </summary>
        private int mTeleportFreeTime;

        /// <summary>
        /// 获取或设置免费传送次数。
        /// </summary>
        public int TeleportFreeTime
        {
            get { return mTeleportFreeTime; }
            set { mTeleportFreeTime = value; }
        }

        /// <summary>
        /// 更新免费传送次数。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        public void OnTeleportFreeTime(MsgData data)
        {
            MsgData_sTeleportFreeTime info = data as MsgData_sTeleportFreeTime;
            mTeleportFreeTime = info.Time;
            EventParameter parameter = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TELEPORT_FREE_TIME, parameter);
        }

        /// <summary>
        /// 获取显示模型ID
        /// </summary>
        /// <returns></returns>
        public int GetDressModelID()
        {
            return SceneLoader.GetClothesModelID(FashionDress, Dress, Job,EquipMgr.selectstar);
        }

        /// <summary>
        /// 获取武器模型ID
        /// </summary>
        /// <returns>武器模型ID</returns>
        public int GetWeaponModelID()
        {
            return SceneLoader.GetWeaponModelID(FashionWeapon, ShenBingId, Weapon, Job);
        }

        /// <summary>
        /// 获取翅膀模型ID
        /// </summary>
        /// <returns>武器模型ID</returns>
        public int GetWingModelID()
        {
            return SceneLoader.GetWingModelID(FashionState == 0 ? FashionWing : 0, Wing, Job);
        }

        private void OnBagChange(GameEvent ge, EventParameter parameter)
        {
            //装备背包变更
            if (parameter.intParameter == BagType.ITEM_BAG_TYPE_EQUIP)
            {
                BagInfo baginfo = PlayerData.Instance.BagData.GetBagInfo(BagType.ITEM_BAG_TYPE_EQUIP);
                int olddress = Dress;                
                //mBasicAttr.Dress = baginfo.GetItemID(PlayerEquipPos.Cloth);
                if (FashionDress == 0 && olddress != Dress)
                {
                    EventParameter ep = EventParameter.Get();
                    ep.intParameter = (int)EModelChange.MODEL_CHANGE_DRESS;
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PLAYER_SHOW_CHANGED, ep);
                }

                int oldweapon = Weapon;
                if(baginfo!=null)
                {
                    mBasicAttr.Weapon = baginfo.GetItemID(PlayerEquipPos.Weapon);
                }
                if (FashionWeapon == 0 && oldweapon != Weapon)
                {
                    EventParameter ep = EventParameter.Get();
                    ep.intParameter = (int)EModelChange.MODEL_CHANGE_WEAPON;
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PLAYER_SHOW_CHANGED, ep);
                }
            }
            if (parameter.intParameter == BagType.ITEM_BAG_TYPE_ROLEITEM)
            {
            }
        }
        
        #endregion

        #region----------------------------------------战斗属性----------------------------------------

        /// <summary>
        /// 上次提醒切换PK模式的时间。
        /// </summary>
        public static float LastNoteSwitchPKTime = 0;

        /// <summary>
        /// 基础属性。
        /// </summary>
        private BaseAttr mBasicAttr = new BaseAttr();

        /// <summary>
        /// 是否已经初始化。
        /// </summary>
        private bool mInit = false;

        /// <summary>
        /// 获取基础属性。
        /// </summary>
        public BaseAttr BaseAttr
        {
            get { return mBasicAttr; }
        }

        /// <summary>
        /// PK模式。
        /// </summary>
        private int mCurPKMode;

        /// <summary>
        /// 获取PK模式。
        /// </summary>
        public int CurPKMode
        {
            get { return mCurPKMode; }
        }

        /// <summary>
        /// 频道是否可攻击玩家。
        /// </summary>
        /// <param name="player">目标玩家。</param>
        /// <returns>是否可攻击。</returns>
        public bool IsCanAttack(OtherPlayer player)
        {
            //双方都有阵营
            if (Faction != 0 && player.Faction != 0)
            {
                return Faction != player.Faction;
            }

            //新手和保护状态不能被攻击
            if (player.PKStatus == PKStatus.PK_STATUS_NEWBIE || player.PKStatus == PKStatus.PK_STATUS_PROTECT)
            {
                return false;
            }

            if (mCurPKMode == PKMode.PK_MODE_ALL)
            {
                return true;
            }
            else if (mCurPKMode == PKMode.PK_MODE_EVIL)
            {
                return player.PKStatus == PKStatus.PK_STATUS_GRAY || player.PKStatus == PKStatus.PK_STATUS_RED;
            }
            else if (mCurPKMode == PKMode.PK_MODE_TEAM)
            {
                return !mTeamData.IsInTeam(player.ServerID);
            }
            else if (mCurPKMode == PKMode.PK_MODE_ALLY)
            {
             //   LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
             //   long attackid = G.GetInPath<long>("ConfigData.EliteBossConfig.AttackPlayerID");
             //   return player.ServerID == attackid;
                return !mTeamData.IsInTeam(player.ServerID) && !mGuildData.IsInGuild(player.ServerID);
            }

            return false;
        }

        /// <summary>
        /// 自定义PK规则。
        /// </summary>
        private int mSelfPK;

        /// <summary>
        /// 获取自定义PK规则。
        /// </summary>
        public int SelfPK
        {
            get { return mSelfPK; }
        }

        /// <summary>
        /// 当前PK状态。
        /// </summary>
        private int mCurPKState;

        /// <summary>
        /// 获取当前PK状态。
        /// </summary>
        public int CurPKState
        {
            get { return mCurPKState; }
        }

        /// <summary>
        /// 初始化数据。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        public void OnInitData(MsgData data)
        {
            MsgData_sMeInfo info = data as MsgData_sMeInfo;
            MainRole.Instance.serverID = info.RoleID;
            MainRole.Instance.faction = info.ZhenYing;
            mBasicAttr.ClearBasicAttr();
            mRoleID = info.RoleID;
            mBasicAttr.Wing = info.Wing;
            mBasicAttr.ZhenFa = info.ZhenFaID;
            mBasicAttr.Faction = info.ZhenYing;
            mBasicAttr.Dress = info.Dress;
            mBasicAttr.EquipStarMin = info.EquipStarMin;
            mBasicAttr.Weapon = info.Weapon;
            mBasicAttr.FashionWing = info.FashionWing;
            mBasicAttr.FashionDress = info.FashionDress;
            mBasicAttr.FashionWeapon = info.FashionWeapon;
            mBasicAttr.FashionState = info.FashionState;
            mBasicAttr.ShenBing = info.ShenBing;
            ShenBingId = info.ShenBing;
            mBasicAttr.Name = GetPlayerName(UiUtil.GetNetString(info.RoleName), out mServerGroup);
            mBasicAttr.SetBasicAttrValue((int)BasicAttrEnum.Gender, info.Gender);
            mBasicAttr.SetBasicAttrValue((int)BasicAttrEnum.Prof, info.Job);
            mBasicAttr.Lord = info.GuanZhi;
            mIcon = info.Icon;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PLAYER_INFO, EventParameter.Get());
            CoreEntry.gAutoAIMgr.ReloadConfig();
            NetLogicGame.Instance.SendReqDianfengInfo();
            NetLogicGame.Instance.SendReqGetMailList();

            //若已经创建了玩家对象则更新
            if (CoreEntry.gActorMgr.MainPlayer != null)
            {
                CoreEntry.gActorMgr.MainPlayer.mBaseAttr.Name = mBasicAttr.Name;
                CoreEntry.gActorMgr.MainPlayer.mBaseAttr.Lord = mBasicAttr.Lord;
                CoreEntry.gActorMgr.MainPlayer.Faction = info.ZhenYing;
            }
            mTeamData.SendTeamInfoRequest();

            //Lua层初始化
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            LuaTable table = G.Get<LuaTable>("ModelManager");
            MemberFunctionCall fun = table.Get<MemberFunctionCall>("InitPlayerData");
            if (fun != null)
            {
                fun(table);
            }
        }

        /// <summary>
        /// 初始化数据,服务端推送玩家世界等级
        /// </summary>
        /// <param name="data"></param>
        public void OnInitWorldlevel(MsgData data)
        {
            MsgData_sWorldLevel info = data as MsgData_sWorldLevel;
            mBasicAttr.WorldLevel = info.level;
           // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PLAYER_INFO, EventParameter.Get());
            //若已经创建了玩家对象则更新
            if (CoreEntry.gActorMgr.MainPlayer != null)
            {
                CoreEntry.gActorMgr.MainPlayer.mBaseAttr.WorldLevel = mBasicAttr.WorldLevel;
            }
        }

        /// <summary>
        /// 外观改变。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void OnShowChanged(int type, int value)
        {
            switch ((EModelChange)type)
            {
                case EModelChange.MODEL_CHANGE_WING:
                    mBasicAttr.Wing = value;
                    break;
                case EModelChange.MODEL_CHANGE_ZHENFA:
                    mBasicAttr.ZhenFa = value;
                    break;
                case EModelChange.MODEL_CHANGE_FACTION:
                    mBasicAttr.Faction = value;
                    break;
                case EModelChange.MODEL_CHANGE_WEAPON:
                    mBasicAttr.Weapon = value;
                    break;
                case EModelChange.MODEL_CHANGE_DRESS:
                    mBasicAttr.Dress = value;
                    break;
                case EModelChange.MODEL_CHANGE_EQUIPSTAR:
                    mBasicAttr.EquipStarMin = value;
                    break;
                case EModelChange.MODEL_CHANGE_FASHION_WING:
                    mBasicAttr.FashionWing = value;
                    break;
                case EModelChange.MODEL_CHANGE_FASHION_DRESS:
                    mBasicAttr.FashionDress = value;
                    break;
                case EModelChange.MODEL_CHANGE_FASHION_WEAPON:
                    mBasicAttr.FashionWeapon = value;
                    break;
                case EModelChange.MODEL_CHANGE_PK_STATUS:
                    mCurPKState = value;
                    break;
                case EModelChange.MODEL_CHANGE_GUANZHI:
                    mBasicAttr.Lord = value;
                    break;
                case EModelChange.MODEL_CHANGE_FASHION:
                    mBasicAttr.FashionState = value;
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FASHION_STATE, EventParameter.Get());
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 战斗力变化。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        public void OnPowerChange(MsgData data)
        {
            MsgData_sModleFightChange info = data as MsgData_sModleFightChange;
            switch (info.ModleType)
            {
                case ModleType.FightChangeHorse:
                    mRideData.Power = info.Value;
                    break;
                case ModleType.FightChangeMagicWeapon:
                    mShenBingMgr.Power = info.Value;
                    break;
                case ModleType.FightChangeHolyShield:
                    mShieldMgr.Power = info.Value;
                    break;
                case ModleType.FightChangeLord:
                    mTitleMgr.Power = info.Value;
                    break;
                case ModleType.FightChangeAstrology:
                    NeiGonginDataMgr.Power = (int)info.Value;
                    break;
                case ModleType.FightChangeSkill:
                    mSkillData.Power = info.Value;
                    break;
			case ModleType.FightChangeFashion:
				    //Debug.LogError ("===info.Value=="+info.Value);
                    mFashionData.Power = info.Value;
                    break;
                case ModleType.FightChangeTreasure:
                    mTreasureMgr.Power = info.Value;
                    break;
                case ModleType.FightChangeMagickey:
                    mMagicKeyMgr.Power = Mathf.FloorToInt(info.Value);
                    break;
                default:
                    break;
            }
            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.ModleType;
            ep.longParameter = info.Value;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_MODLE_POWER_CHANGE, ep);
        }

        /// <summary>
        /// 设置PK规则。
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        public void OnSetPKRule(MsgData data)
        {
            MsgData_sSetPKRule info = data as MsgData_sSetPKRule;
            mCurPKMode = info.ID;
            mSelfPK = info.SelfPK;
            mCurPKState = info.State;

            EventParameter ep = EventParameter.Get();
            ep.intParameter = info.ID;
            ep.intParameter1 = info.SelfPK;
            ep.intParameter2 = info.State;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PLAYER_PK_MODE, ep);
        }

        /// <summary>
        /// 获取玩家名称。
        /// </summary>
        /// <param name="sname">带有服务器号的名字。</param>
        /// <param name="server">[输出参数]:服务器号。</param>
        /// <returns>玩家名称。</returns>
        public static string GetPlayerName(string sname, out int server)
        {
            int index = sname.IndexOf(']');
            if (index == -1)
            {
                server = 0;
                return sname;
            }
            server = int.Parse(sname.Substring(1, index - 1));
            string name = sname.Substring(index + 1);
            return string.Format("{0}", name);
        }

        /// <summary>
        /// 更新属性。
        /// </summary>
        /// <param name="attrs">属性列表。</param>
        public virtual void OnUpdateAttr(List<MsgData_sClientAttr> attrs)
        {
            //Debug.Log("OnUpdateAttr 0");
            //用于判断血量或战力是否发生变化
            long oldhp = mBasicAttr.CurHP;
            long oldmaxhp = mBasicAttr.MaxHP;
            long oldpower = mBasicAttr.Power;
            double fOldPower = mBasicAttr.GetBasicAttrValue((int)BasicAttrEnum.Power);
            int oldlevel = mBasicAttr.Level;
            long oldExp = mBasicAttr.Exp;
            bool currencychange = false;
            int oldJingJieLv = mBasicAttr.JingJieLevel;
            long oldJingJieExp = mBasicAttr.JingJieExp;
            int oldVip = mBasicAttr.VIPLevel;
            double[] oldattr = mInit ? FlyAttrManager.GetShowAttr(mBasicAttr.Attrs) : null;     //没进场不飘字
            for (int i = 0; i < attrs.Count; ++i)
            {
                MsgData_sClientAttr ca = attrs[i];
                BasicAttrEnum type = BaseAttr.GetBasicAttrTypeFromStatType(ca.AttrType);
                if (type != BasicAttrEnum.Unkonw)
                {
                    mBasicAttr.SetBasicAttrValue((int)type, ca.AttrValue);
                    continue;
                }
                int currencytype = CurrencyType.GetCurrencyTypeFromStatType(ca.AttrType);
                if (currencytype != -1)
                {
                    SetCurrency(currencytype, (long)ca.AttrValue);
                    currencychange = true;
                    if (currencytype == CurrencyType.CURRENCY_TYPE_UNBIND_MONEY)
                    {
                        Account.Instance.isRecharging = false; //充值钻石完成
                    }
                    continue;
                }
            }
            if (oldVip != mBasicAttr.VIPLevel)
            {
                if(CoreEntry.gActorMgr.MainPlayer != null)
                {
                    if(CoreEntry.gActorMgr.MainPlayer.mBaseAttr != null)
                    {
                        CoreEntry.gActorMgr.MainPlayer.mBaseAttr.VIPLevel = mBasicAttr.VIPLevel;
                        CoreEntry.gActorMgr.MainPlayer.Health.OnLordChange();
                    }

                }
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PLAYER_VIP, EventParameter.Get());
                //Debug.Log("OnUpdateAttr 1");
            }
            if (oldhp != mBasicAttr.CurHP || oldmaxhp != mBasicAttr.MaxHP)
            {
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PLAYER_HP, EventParameter.Get());
                //Debug.Log("OnUpdateAttr 2");
            }
            if (oldpower != mBasicAttr.Power)
            {
                int powerDiff =(int)(mBasicAttr.GetBasicAttrValue((int)BasicAttrEnum.Power) - fOldPower);
                long powerDiffInt = mBasicAttr.Power - oldpower;
                oldpower = oldpower -  (powerDiff - powerDiffInt);
                EventParameter ep = EventParameter.Get();
                ep.longParameter = oldpower;
                ep.longParameter1 = mBasicAttr.Power;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PLAYER_POWER, ep);
                Debug.Log("OnUpdateAttr 3");
            }
            if (oldlevel != mBasicAttr.Level)
            {
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PLAYER_INFO, EventParameter.Get());
                //Debug.Log("OnUpdateAttr 4");
                if (oldlevel != 0)
                {
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PLAYER_LV, EventParameter.Get(oldlevel));
                    //Debug.Log("OnUpdateAttr 5");
                }
            }
            if (currencychange)
            {
                //注意，大量丢金币变化通知，卡CPU
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PLAYER_CURRENCY, EventParameter.Get());
                //Debug.Log("OnUpdateAttr 6");
            }
            if (oldlevel != mBasicAttr.Level || oldExp != mBasicAttr.Exp)
            {
                EventParameter parameter = EventParameter.Get();
                parameter.intParameter = oldlevel;
                parameter.longParameter = oldExp;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PLAYER_EXP, parameter);
                //Debug.Log("OnUpdateAttr 7");
            }
            if (oldJingJieLv != mBasicAttr.JingJieLevel)
            {
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PLAYER_JINGJIE_LV, EventParameter.Get(oldJingJieLv));
                //Debug.Log("OnUpdateAttr 8");
            }
            if (oldJingJieLv != mBasicAttr.JingJieLevel || oldJingJieExp != mBasicAttr.JingJieExp)
            {
                EventParameter parameter = EventParameter.Get();
                parameter.intParameter = oldJingJieLv;
                parameter.longParameter = oldJingJieExp;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PALYER_JINGJIE_EXP, parameter);
                //Debug.Log("OnUpdateAttr 9");
            }
            if (oldattr != null)
            {
                double[] newattr = FlyAttrManager.GetShowAttr(mBasicAttr.Attrs);
                EventParameter ep = EventParameter.Get();
                ep.objParameter = oldattr;
                ep.objParameter1 = newattr;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FLY_ATTR, ep);
                //Debug.Log("OnUpdateAttr 10");
            }
        }

        public void OnGameEventEnterScene(GameEvent ge, EventParameter parameter)
        {
            mInit = true;
        }

        /// <summary>
        /// 发送设置PK规则请求。
        /// </summary>
        /// <param name="id">PK规则。</param>
        public void SendSetPKRuleRequest(int id)
        {
            if (mCurPKMode == id)
            {
                return;
            }

            MsgData_cSetPKRule data = new MsgData_cSetPKRule();
            data.ID = id;
            data.SelfPK = mSelfPK;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_SET_PK_RULE, data);
        }

        #endregion

        #region----------------------------------------坐骑----------------------------------------

        /// <summary>
        /// 坐骑数据。
        /// </summary>
        private RideData mRideData = new RideData();

        /// <summary>
        /// 获取坐骑数据。
        /// </summary>
        public RideData RideData
        {
            get { return mRideData; }
        }

        #endregion

        #region----------------------------------------背包----------------------------------------

        /// <summary>
        /// 背包数据。
        /// </summary>
        private BagData mBagData = new BagData();

        /// <summary>
        /// 获取背包数据。
        /// </summary>
        public BagData BagData
        {
            get { return mBagData; }
        }

        #endregion

        #region----------------------------------------技能----------------------------------------

        /// <summary>
        /// 技能数据。
        /// </summary>
        private SkillData mSkillData = new SkillData();

        /// <summary>
        /// 获取技能数据。
        /// </summary>
        public SkillData SkillData
        {
            get { return mSkillData; }
        }

        #endregion

        #region----------------------------------------商店----------------------------------------

        /// <summary>
        /// 聊天数据。
        /// </summary>
        private ChatData mChatData = new ChatData();

        /// <summary>
        /// 获取聊天数据。
        /// </summary>
        public ChatData ChatData
        {
            get { return mChatData; }
        }

        #endregion

        #region----------------------------------------队伍----------------------------------------

        /// <summary>
        /// 队伍数据。
        /// </summary>
        private TeamData mTeamData = new TeamData();

        /// <summary>
        /// 获取队伍数据。
        /// </summary>
        public TeamData TeamData
        {
            get { return mTeamData; }
        }

        #endregion

        #region----------------------------------------公会----------------------------------------

        /// <summary>
        /// 队伍数据。
        /// </summary>
        private GuildData mGuildData = new GuildData();

        /// <summary>
        /// 获取队伍数据。
        /// </summary>
        public GuildData GuildData
        {
            get { return mGuildData; }
        }

        #endregion

        #region----------------------------------------好友----------------------------------------

        /// <summary>
        /// 好友数据。
        /// </summary>
        private FriendData mFriendData = new FriendData();

        /// <summary>
        /// 获取好友数据。
        /// </summary>
        public FriendData FriendData
        {
            get { return mFriendData; }
        }

        #endregion

        #region----------------------------------------时装----------------------------------------

        /// <summary>
        /// 时装数据。
        /// </summary>
        private FashionData mFashionData = new FashionData();

        /// <summary>
        /// 获取时装数据。
        /// </summary>
        public FashionData FashionData
        {
            get { return mFashionData; }
        }

        #endregion

        #region----------------------------------------资源本----------------------------------------

        /// <summary>
        /// 聊天数据。
        /// </summary>
        private ResLevelData mResLevelData = new ResLevelData();

        /// <summary>
        /// 获取聊天数据。
        /// </summary>
        public ResLevelData ResLevelData
        {
            get { return mResLevelData; }
        }

        #endregion

        #region----------------------------------------境界----------------------------------------
        private JingJieData mJingJieData = new JingJieData();

        /// <summary>
        /// 境界数据
        /// </summary>
        public JingJieData JingJieData
        {
            get { return mJingJieData; }
        }

        #endregion

        #region----------------------------------------任务----------------------------------------
        /// <summary>
        /// 任务
        /// </summary>
        public TaskMgr Task = TaskMgr.Instance;
        #endregion

        #region----------------------------------------NPC----------------------------------------
        /// <summary>
        /// NPC
        /// </summary>
        public NpcMgr Npc = NpcMgr.Instance;
        #endregion

        #region----------------------------------------副本(幻灵仙域)----------------------------------------
        /// <summary>
        /// 副本(幻灵仙域)
        /// </summary>
        public DungeonMgr Dungeon = DungeonMgr.Instance;
        #endregion


    #region 装备
        private EquipDataMgr mEquipMgr = new EquipDataMgr();
        public EquipDataMgr EquipMgr
        {
            get
            {
                return mEquipMgr;
            }
        }
    #endregion
    #region 法宝 
        private MagicKeyDataMgr mMagicKeyMgr = new MagicKeyDataMgr();
        public MagicKeyDataMgr MagicKeyDataMgr
        {
            get
            {
                return mMagicKeyMgr;
            }
        }
         
    #endregion

        #region 等级副本
        private LevelFuBenDataMgr mLevelFuBenDataMgr = new LevelFuBenDataMgr();
        public LevelFuBenDataMgr LevelFuBenDataMgr
        {
            get
            {
                return mLevelFuBenDataMgr;
            }
        }

        #endregion
        #region boss副本
        private BossFuBenDataMgr mBossFuBenDataMgr = new BossFuBenDataMgr();
        public BossFuBenDataMgr BossFuBenDataMgr
        {
            get
            {
                return mBossFuBenDataMgr;
            }
        } 
        #endregion


        #region 经验副本
        private ExpFuBenDataMgr mExpFuBenDataMgr = new ExpFuBenDataMgr();
        public ExpFuBenDataMgr ExpFuBenDataMgr
        {
            get
            {
                return mExpFuBenDataMgr;
            }
        }
        #endregion
        #region 占星
        private NeiGonginDataMgr mNeiGonginDataMgrr = new NeiGonginDataMgr();
        public NeiGonginDataMgr NeiGonginDataMgr
        {
            get
            {
                return mNeiGonginDataMgrr;
            }
        }
        #endregion
        #region 邮件
        private MailDataMgr mMailDataMgr = new MailDataMgr();
        public MailDataMgr MailDataMgr
        {
            get
            {
                return mMailDataMgr;
            }
        }
        #endregion
        
        #region 幻灵
        private BeautyWomanMgr mBeautyWomanMgr = new BeautyWomanMgr();
        public BeautyWomanMgr BeautyWomanMgr
        {
            get
            {
                return mBeautyWomanMgr;
            }
        }
         
        #endregion

#region 秘境副本
       private SecretDuplDataMgr mSecretDuplDataMgr = new SecretDuplDataMgr();
        public SecretDuplDataMgr SecretDuplDataMgr
        {
            get
            {
                return mSecretDuplDataMgr;
            }
        }

        #endregion

#region 宝塔副本
        private TreasureFuBenDataMgr mTreasureFuBenDataMgr = new TreasureFuBenDataMgr();
        public TreasureFuBenDataMgr TreasureFuBenDataMgr
        {
            get
            {
                return mTreasureFuBenDataMgr;
            }
        }
#endregion

        #region----------------------------------------宝石----------------------------------------
        private StoneData mStoneData = new StoneData();

        /// <summary>
        /// 宝石数据
        /// </summary>
        public StoneData StoneData
        {
            get { return mStoneData; }
        }

        #endregion


        #region -----------------------------------神兵 ------------------------------------------------
        /// <summary>
        /// 神兵
        /// </summary>
        public ShenBingMgr mShenBingMgr = ShenBingMgr.Instance;
        #endregion

        #region -----------------------------------圣盾 ------------------------------------------------
        /// <summary>
        /// 圣盾
        /// </summary>
        public ShieldMgr mShieldMgr = ShieldMgr.Instance;
        #endregion

        #region -----------------------------------灌注 ------------------------------------------------
        public PerfusionMgr mPerfusionMgr = PerfusionMgr.Instance;
        #endregion

        #region -----------------------------------王冠------------------------------------------------
        private CrownData mCrownData = new CrownData();

        /// <summary>
        /// 获取王冠数据 
        /// </summary>
        public CrownData CrownData
        {
            get { return mCrownData; }
        }
        #endregion

        #region  ------------------------------通缉------------------------------------------
        public TongJiMgr mTongJiMgr = TongJiMgr.Instance;
        #endregion

        #region ---------------------------------------爵位------------------------------------------
        public TitleMgr mTitleMgr = TitleMgr.Instance;
        #endregion

        #region 功能列表
        private FunctionData funcData = new FunctionData();
        public FunctionData FuncData
        {
            get { return funcData; }
        }
        #endregion

        #region -----------------------------------------活动--------------------------------------------
        public ActivityMgr mActivityMgr = ActivityMgr.Instance;
        #endregion

        #region -----------------------------------------珍宝阁--------------------------------------------
        public TreasureMgr mTreasureMgr = TreasureMgr.Instance;
        #endregion

        #region -----------------------------------------日常活动--------------------------------------------
        public DailyMgr mDailyMgr = DailyMgr.Instance;
        #endregion

        /// <summary>
        /// 隐藏复活界面的剩余复活时长,  在返回复活成功的协议里 还原 ReviceHideLessTime = 0
        /// </summary>
        public int ReviceHideLessTime = 0;
    }
}