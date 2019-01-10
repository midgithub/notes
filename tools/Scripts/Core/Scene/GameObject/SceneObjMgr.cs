/**
* @file     : SceneObjMgr.cs
* @brief    : 场景NPC，Monster等管理
* @details  : 场景NPC，Monster等管理
* @author   : CW
* @date     : 2017-06-14
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{
[Hotfix]
    public class SceneObjMgr : MonoBehaviour
    {
        /// <summary>
        /// 最大玩家显示数量，超过这个数量的玩家不再加载模型显示。
        /// </summary>
        public static int ShowMaxPlayer = 1;

        /// <summary>
        /// 最大玩家创建数量，超过这个数量的玩家不再创建。
        /// </summary>
        public static int CreateMaxPlayer = 5;
        
        private List<SceneObj> mSceneDataList = new List<SceneObj>();
        private List<SceneObj> mSceneCache = new List<SceneObj>();
        private List<MsgData_sObjDeadInfo> mDeathCache = new List<MsgData_sObjDeadInfo>();
        private List<ActorObj> mLeaveCache = new List<ActorObj>();
        private Dictionary<long, ScenePlayerData> mBackgroundPlayer = new Dictionary<long, ScenePlayerData>();      //在后台未创建Actor的玩家数据
        private bool bSceneLoaded = false;
        private float mMaxPrcessingTime = .2f;
        
        void Start()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BEGIN_LOADSCENE, OnSceneBeforeLoading);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_AFTER_LOADSCENE, OnSceneLoaded);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_ATTRINFO, OnRoleAttrInfo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_PLAYER_SHOW_CHANGED, OnPlayerShowChanged);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_OBJ_ENTER_SCENE, OnEnterScene);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_OBJ_LEAVE_SCENE, OnLeaveScene);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_OBJ_Disapper, OnLeaveDisapper);
             
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_OBJ_CLEAR, OnClearObj);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_STATE_CHANGED, OnStateChanged);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_REVIVE, OnRevive);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_PLAYER_DEATH, OnPlayerDeath);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_PLAYER_PK_MODE, OnPlayerPKMode);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_TEAM_JOIN, OnTeamJoin);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_TEAM_EXIT, OnTeamExit);
            CoreEntry.gEventMgr.AddListener(GameEvent.GS_SC_TRIGGERTRAP, OnTriggerTrap);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CAMP_ENER, OnCampEnter);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CAMP_LEAVE, OnCampLeave);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CAMP_UPDATE, OnCampUpdate);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_COLLECTION_UPDATE, OnCollectionUpdate);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_OBJ_DEAD, OnObjectDead);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_OBJ_HONGYAN_LEVEL, OnSceneObjHongyanLevel);

            
        }

        void Update()
        {
            if (!bSceneLoaded)
            {
                return;
            }

            float time = 0.0f;
            float lastTime = Time.realtimeSinceStartup;
            while (mSceneCache.Count > 0)
            {
                if (time > mMaxPrcessingTime)
                {
                    return;
                }

                SceneObj sceneObj = mSceneCache[0];
                switch (sceneObj.ObjType)
                {
                    case EnEntType.EnEntType_Player:
                        MsgData_sSceneObjectEnterHuman humanStruct = sceneObj.ObjData as MsgData_sSceneObjectEnterHuman;
                        CoreEntry.gSceneLoader.LoadOtherPlayer(humanStruct);
                        if (sceneObj.PlayerData != null)
                        {
                            sceneObj.PlayerData = null;
                        }
                        break;
                    case EnEntType.EnEntType_Monster:
                        MsgData_sSceneObjectEnterMonster monsterStruct = sceneObj.ObjData as MsgData_sSceneObjectEnterMonster;
                        CoreEntry.gSceneLoader.LoadMonster(monsterStruct);
                        break;
                    case EnEntType.EnEntType_NPC:
                        MsgData_sSceneObjectEnterNPC npcStruct = sceneObj.ObjData as MsgData_sSceneObjectEnterNPC;
                        CoreEntry.gSceneLoader.LoadNPC(npcStruct);
                        break;
                    case EnEntType.EnEntType_StaticObj:
                        MsgData_sSceneObjectEnterStaticObj sstaticStruct = sceneObj.ObjData as MsgData_sSceneObjectEnterStaticObj;
                        CoreEntry.gSceneLoader.LoadStaticObj(sstaticStruct);
                        break;
                    case EnEntType.EnEntType_Item:
                        MsgData_sSceneObjectEnterItem itemStruct = sceneObj.ObjData as MsgData_sSceneObjectEnterItem;
                        EventParameter param = EventParameter.Get();
                        param.objParameter = itemStruct;
                        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OBJ_ITEM_ENTER, param);
                        break;
                    case EnEntType.EnEntType_GatherObj:
                        MsgData_sSceneObjectEnterCollection collectionStruct = sceneObj.ObjData as MsgData_sSceneObjectEnterCollection;
                        CoreEntry.gSceneLoader.LoadCollection(collectionStruct);
                        break;
                    case EnEntType.EnEntType_Pet:
                        MsgData_sSceneObjectEnterPet petStruct = sceneObj.ObjData as MsgData_sSceneObjectEnterPet;
                        CoreEntry.gSceneLoader.LoadPet(petStruct);
                        break;
                    case EnEntType.EnEntType_BiaoChe:
                        MsgData_sSceneObjectEnterBiaoChe cheStruct = sceneObj.ObjData as MsgData_sSceneObjectEnterBiaoChe;
                        CoreEntry.gSceneLoader.LoadBiaoChe(cheStruct);
                        break;
                    case EnEntType.EnEntType_Trap:
                        MsgData_sSceneObjectEnterTrap trapStruct = sceneObj.ObjData as MsgData_sSceneObjectEnterTrap;
                        CoreEntry.gSceneLoader.LoadTrap(trapStruct);
                        break;
                    case EnEntType.EntType_VirtualPlayer:
                        MsgData_sSceneObjectEnterVirtualPlayer vpStruct = sceneObj.ObjData as MsgData_sSceneObjectEnterVirtualPlayer;
                        CoreEntry.gSceneLoader.LoadVirtualPlayer(vpStruct);
                        break;
                    default:
                        break;
                }
                
                mSceneCache.RemoveAt(0);

                time += Time.realtimeSinceStartup - lastTime;
            }

            for (int i = 0; i < mDeathCache.Count; i++)
            {
                MsgData_sObjDeadInfo msg = mDeathCache[i];
                ActorObj behitActor = CoreEntry.gActorMgr.GetActorByServerID(msg.ID);
                if (null == behitActor)
                {
                    continue;
                }

                ActorObj attackActor = CoreEntry.gActorMgr.GetActorByServerID(msg.KillerID);

                BehitParam behitParam = new BehitParam();
                DamageParam damageParam = new DamageParam();
                damageParam.attackActor = attackActor;
                damageParam.behitActor = behitActor;
                damageParam.skillID = msg.KillerSkillID;

                behitParam.damgageInfo = damageParam;
                behitActor.OnDead(msg.KillerSkillID, attackActor, behitParam, EventParameter.Get());

                mDeathCache.RemoveAt(i);
                i--;
            }

            for (int i = 0; i < mLeaveCache.Count; i++)
            {
                ActorObj actor = mLeaveCache[i];
                if (null != actor)
                {
                    if (actor.mActorState.IsDeathEnd())
                    {
                        CoreEntry.gActorMgr.RemoveActorByServerID(actor.ServerID);
                        actor.RecycleObj();

                        EventParameter param = EventParameter.Get();
                        param.longParameter = actor.ServerID;
                        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ACTOR_REMOVE, param);

                        if (actor is OtherPlayer)
                        {
                            param = EventParameter.Get();
                            param.longParameter = actor.ServerID;
                            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OTHERPLAYER_LEAVE, param);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                mLeaveCache.RemoveAt(i);
                i--;
            }
        }

        private void OnSceneBeforeLoading(GameEvent ge, EventParameter parameter)
        {
            bSceneLoaded = false;

            mSceneCache.Clear();
            mSceneDataList.Clear();
            mDeathCache.Clear();
            mLeaveCache.Clear();
            mBackgroundPlayer.Clear();
        }

        private void OnSceneLoaded(GameEvent ge, EventParameter parameter)
        {
            bSceneLoaded = true;
        }

        private void OnTriggerTrap(GameEvent ge, EventParameter parameter)
        {
            MsgData_sTriggerTrap data = parameter.msgParameter as MsgData_sTriggerTrap;
            Trap trap = CoreEntry.gEntityMgr.GetTrapByServerID((long)data.TrapRoleID) as Trap;
            if (null != trap)
            {
                trap.OnTrigger();
            }
        }

        private void OnCampEnter(GameEvent ge, EventParameter parameter)
        {
            Vector3 position = CommonTools.ServerPosToClient(parameter.intParameter1, parameter.intParameter2);
            Vector3 scale = new Vector3(parameter.floatParameter, parameter.floatParameter, parameter.floatParameter);
            GameObject campGo = CoreEntry.gSceneLoader.LoadCamp(parameter.longParameter, position, scale);
            if (null != campGo)
            {
                CampObj camp = campGo.GetComponent<CampObj>();
                if (null != camp)
                {
                    camp.EffectSize = scale;
                    camp.ChangeType((CampType)parameter.intParameter);
                }
            }
        }

        private void OnCampLeave(GameEvent ge, EventParameter parameter)
        {
            CoreEntry.gEntityMgr.RemoveCampByServerID(parameter.longParameter);
            CampObj camp = CoreEntry.gEntityMgr.GetCampByServerID(parameter.longParameter) as CampObj;
            if (null != camp)
            {
                camp.RecycleObj();
            }
        }

        private void OnCampUpdate(GameEvent ge, EventParameter parameter)
        {
            CampObj camp = CoreEntry.gEntityMgr.GetCampByServerID(parameter.longParameter) as CampObj;
            if (null != camp)
            {
                camp.ChangeType((CampType)parameter.intParameter);
            }
        }

        private void OnCollectionUpdate(GameEvent ge, EventParameter parameter)
        {
            CollectionObj collection = CoreEntry.gEntityMgr.GetCollectionByServerID(parameter.longParameter) as CollectionObj;
            if (null != collection)
            {
                collection.Change((CollectionType)parameter.intParameter);
            }
        }

        private void OnPlayerPKMode(GameEvent ge, EventParameter parameter)
        {
            //玩家PK模式发生改变，需要刷新所有其它玩家的血条显示
            List<ActorObj> players = CoreEntry.gActorMgr.GetAllPlayerActors();
            for (int i=0; i< players.Count; ++i)
            {
                players[i].Health.OnPKModeStatus();
            }
        }

        /// <summary>
        /// 队伍成员加入。
        /// </summary>
        private void OnTeamJoin(GameEvent ge, EventParameter parameter)
        {
            ActorObj act = CoreEntry.gActorMgr.GetActorByServerID(parameter.longParameter);
            if (act != null)
            {
                act.Health.OnPKModeStatus();
            }
        }

        /// <summary>
        /// 队伍成员退出。
        /// </summary>
        private void OnTeamExit(GameEvent ge, EventParameter parameter)
        {
            ActorObj act = CoreEntry.gActorMgr.GetActorByServerID(parameter.longParameter);
            if (act != null)
            {
                act.Health.OnPKModeStatus();
            }
        }
        public void OnSceneObjHongyanLevel(GameEvent ge, EventParameter parameter)
        {
            MsgData_sSceneObjHoneYanLevel data = parameter.msgParameter as MsgData_sSceneObjHoneYanLevel;
            ActorObj obj = CoreEntry.gActorMgr.GetActorByServerID(data.RoleID);
            if (obj != null)
            {
                obj.UpdatePetAttr(data);
            }
        }

        private void OnRoleAttrInfo(GameEvent ge, EventParameter parameter)
        {
            MsgData_sRoleAttrInfoNotify data = parameter.msgParameter as MsgData_sRoleAttrInfoNotify;
            ActorObj obj = CoreEntry.gActorMgr.GetActorByServerID(data.RoleID);
            if (obj != null)
            {
                if (ArenaMgr.Instance.IsArena)//如果是竞技场 更新血量
                {
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ARENA_HP_REFRESH, null);
                }
                obj.UpdateAttr(data.AttrList);
            }else //查看缓存中的数据
            {
                SceneObj so = mSceneCache.Find(o => { return o.ObjGuid == data.RoleID; });
                if(so != null && so.ObjType == EnEntType.EnEntType_Monster)
                {
                    var msg = so.ObjData as MsgData_sSceneObjectEnterMonster;
                    foreach(var attr in data.AttrList)
                    {
                        var type = BaseAttr.GetBasicAttrTypeFromStatType(attr.AttrType);
                        switch(type)
                        {
                            case BasicAttrEnum.CurHP: msg.CurHp = attr.AttrValue; break;
                            case BasicAttrEnum.MaxHP: msg.MaxHp = attr.AttrValue; break;
                        }
                    }
                }
            }
        }

        private void OnPlayerShowChanged(GameEvent ge, EventParameter parameter)
        {
            MsgData_sPlayerShowChanged data = parameter.msgParameter as MsgData_sPlayerShowChanged;

            //玩家自身数据层的更新
            if (data.RoleID == PlayerData.Instance.RoleID)
            {
                PlayerData.Instance.OnShowChanged(data.Type, data.Value);
            }

            //角色更新
            ActorObj obj = CoreEntry.gActorMgr.GetActorByServerID(data.RoleID);
            if (obj != null)
            {
                BaseAttr attr = obj.mBaseAttr;
                switch ((EModelChange)data.Type)
                {
                    case EModelChange.MODEL_CHANGE_WING:
                        attr.Wing = data.Value;
                        obj.ChangeWing(SceneLoader.GetWingModelID(attr.FashionState == 0 ? attr.FashionWing : 0, attr.Wing, attr.Prof));
                        break;
                    case EModelChange.MODEL_CHANGE_WEAPON:
                        attr.Weapon = data.Value;
                        obj.ChangeWeapon(SceneLoader.GetWeaponModelID(attr.FashionState == 0 ? attr.FashionWeapon : 0, attr.ShenBing, attr.Weapon, attr.Prof));
                        break;
                    case EModelChange.MODEL_CHANGE_DRESS:
                        attr.Dress = data.Value;
                        obj.ChangeClothes(SceneLoader.GetClothesModelID(attr.FashionState == 0 ? attr.FashionDress : 0, attr.Dress, attr.Prof,attr.EquipStarMin));
                        break;
                    case EModelChange.MODEL_CHANGE_EQUIPSTAR:
                        attr.EquipStarMin = data.Value;
                        obj.ChangeClothes(SceneLoader.GetClothesModelID(attr.FashionState == 0 ? attr.FashionDress : 0, attr.Dress, attr.Prof,attr.EquipStarMin));
                        break;
                    case EModelChange.MODEL_CHANGE_FASHION_WING:
                        attr.FashionWing = data.Value;
                        obj.ChangeWing(SceneLoader.GetWingModelID(attr.FashionState == 0 ? attr.FashionWing : 0, attr.Wing, attr.Prof));
                        break;
                    case EModelChange.MODEL_CHANGE_ZHENFA:
                        attr.ZhenFa = data.Value;
                        obj.ChangeZhenFa(data.Value);
                        break;
                    case EModelChange.MODEL_CHANGE_FACTION:
                        attr.Faction = data.Value;
                        obj.ChangeFaction(data.Value);
                        break;
                    case EModelChange.MODEL_CHANGE_FASHION_DRESS:
                        attr.FashionDress = data.Value;
                        obj.ChangeClothes(SceneLoader.GetClothesModelID(attr.FashionState == 0 ? attr.FashionDress : 0, attr.Dress, attr.Prof,attr.EquipStarMin));
                        break;
                    case EModelChange.MODEL_CHANGE_FASHION_WEAPON:
                        attr.FashionWeapon = data.Value;
                        obj.ChangeWeapon(SceneLoader.GetWeaponModelID(attr.FashionState == 0 ? attr.FashionWeapon : 0, attr.ShenBing, attr.Weapon, attr.Prof));
                        break;
                    case EModelChange.MODEL_CHANGE_SHENBING:
                        attr.ShenBing = data.Value;
                        obj.ChangeWeapon(SceneLoader.GetWeaponModelID(attr.FashionWeapon, attr.ShenBing, attr.Weapon, attr.Prof));
                        break;
                    case EModelChange.MODEL_CHANGE_RIDE:
                        obj.FuckHorse(data.Value);
                        break;
                    case EModelChange.MODEL_CHANGE_MAGICKEY:
                        obj.ShowMagicKey(data.Value);
                        break;
                    case EModelChange.MODEL_CHANGE_MAGICKEY_STAR: // 法宝星级
                        obj.ShowMagicKeyByStar(data.Value);
                        break;
                    case EModelChange.MODEL_CHANGE_PK_STATUS:
                        obj.ChangePKStatus(data.Value);
                        break;
                    case EModelChange.MODEL_CHANGE_GUANZHI:
                        attr.Lord = data.Value;                     
                        obj.Health.OnCreateHPBar();
                        break;
                    case EModelChange.MODEL_CHANGE_FASHION:
                        attr.FashionState = data.Value;
                        obj.ChangeWing(SceneLoader.GetWingModelID(attr.FashionState == 0 ? attr.FashionWing : 0, attr.Wing, attr.Prof));
                        obj.ChangeClothes(SceneLoader.GetClothesModelID(attr.FashionState == 0 ? attr.FashionDress : 0, attr.Dress, attr.Prof,attr.EquipStarMin));
                        obj.ChangeWeapon(SceneLoader.GetWeaponModelID(attr.FashionState == 0 ? attr.FashionWeapon : 0, attr.ShenBing, attr.Weapon, attr.Prof));
                        break;
                    case EModelChange.MODEL_CHANGE_EQUIP_TITLE:
                        obj.ChangeHeroTitle(data.Value);
                        break;
                    case EModelChange.MODEL_CHANGE_UNEQUIP_TITLE:
                        obj.ChangeHeroTitle(0);
                        break;
                    default:
                        break;
                }
            }
        }

        private static MsgData_sSceneObjectEnterHead CacheEnterHead = new MsgData_sSceneObjectEnterHead();

        private void OnEnterScene(GameEvent ge, EventParameter parameter)
        {
            NetReadBuffer buffer = parameter.objParameter as NetReadBuffer;
            CacheEnterHead.unpack(buffer);
            buffer.pos = 0;     //重新给具体类型的消息读取
            if (CacheEnterHead.ObjType == (sbyte)EnEntType.EnEntType_Player)//其它玩家
            {
                MsgData_sSceneObjectEnterHuman humanStruct = new MsgData_sSceneObjectEnterHuman();
                humanStruct.unpack(buffer);

                //队友一定创建
                bool inteam = PlayerData.Instance.TeamData.IsInTeam(humanStruct.Guid);
                int num = inteam ? 0 : (CoreEntry.gActorMgr.GetAllPlayerActors().Count - 1 + GetCacheNumber(EnEntType.EnEntType_Player));
                if (num < CreateMaxPlayer)
                {
                    CacheSceneObj(EnEntType.EnEntType_Player, humanStruct.Guid, humanStruct);
                }
                else
                {
                    ScenePlayerData playerdata = new ScenePlayerData();
                    playerdata.EnterData = humanStruct;
                    if (!mBackgroundPlayer.ContainsKey(playerdata.Guid))
                    {
                        mBackgroundPlayer.Add(playerdata.Guid, playerdata);
                    }
                    else
                    {
                        LogMgr.LogError("玩家已经在缓存中 guid:{0}", playerdata.Guid);
                    }                    
                }                
            }
            else if (CacheEnterHead.ObjType == (sbyte)EnEntType.EnEntType_Monster)//怪物
            {
                MsgData_sSceneObjectEnterMonster monsterStruct = new MsgData_sSceneObjectEnterMonster();
                monsterStruct.unpack(buffer);
                if (CoreEntry.gCurrentMapDesc.Get<int>("type") == 31)   //幻灵副本 ，延时刷怪
                {
                    DungeonMgr.Instance.AddMonster(monsterStruct);
                }
                else
                {
                    CacheSceneObj(EnEntType.EnEntType_Monster, monsterStruct.Guid, monsterStruct);
                }
            }
            else if (CacheEnterHead.ObjType == (sbyte)EnEntType.EnEntType_NPC)//NPC
            {
                MsgData_sSceneObjectEnterNPC npcStruct = new MsgData_sSceneObjectEnterNPC();
                npcStruct.unpack(buffer);
                CacheSceneObj(EnEntType.EnEntType_NPC, npcStruct.Guid, npcStruct);
            }
            else if (CacheEnterHead.ObjType == (sbyte)EnEntType.EnEntType_Item)//物品
            {
                MsgData_sSceneObjectEnterItem itemStruct = new MsgData_sSceneObjectEnterItem();
                itemStruct.unpack(buffer);

                if (itemStruct.Owner == PlayerData.Instance.RoleID)
                {
                    CacheSceneObj(EnEntType.EnEntType_Item, itemStruct.Guid, itemStruct);

                    EventParameter ep = EventParameter.Get();
                    ep.intParameter = itemStruct.ConfigID;
                    ep.intParameter1 = itemStruct.Count;
                    ep.longParameter = itemStruct.Source;
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ITEM_DROP, ep);
                }                
            }
            else if (CacheEnterHead.ObjType == (sbyte)EnEntType.EnEntType_GatherObj)
            {
                MsgData_sSceneObjectEnterCollection collectionStruct = new MsgData_sSceneObjectEnterCollection();
                collectionStruct.unpack(buffer);
                CacheSceneObj(EnEntType.EnEntType_GatherObj, collectionStruct.Guid, collectionStruct);
            }
            else if (CacheEnterHead.ObjType == (sbyte)EnEntType.EnEntType_Trap)//陷阱
            {
                MsgData_sSceneObjectEnterTrap trapStruct = new MsgData_sSceneObjectEnterTrap();
                trapStruct.unpack(buffer);
                CacheSceneObj(EnEntType.EnEntType_Trap, trapStruct.Guid, trapStruct);
            }
            else if (CacheEnterHead.ObjType == (sbyte)EnEntType.EnEntType_Pet)
            {
                MsgData_sSceneObjectEnterPet petStruct = new MsgData_sSceneObjectEnterPet();
                petStruct.unpack(buffer);
                CacheSceneObj(EnEntType.EnEntType_Pet, petStruct.Guid, petStruct);
            }
            else if (CacheEnterHead.ObjType == (sbyte)EnEntType.EnEntType_BiaoChe)
            {
                MsgData_sSceneObjectEnterBiaoChe cheStruct = new MsgData_sSceneObjectEnterBiaoChe();
                cheStruct.unpack(buffer);
                CacheSceneObj(EnEntType.EnEntType_BiaoChe, cheStruct.Guid, cheStruct);
            }
            else if (CacheEnterHead.ObjType == (sbyte)EnEntType.EnEntType_StaticObj)//Portal
            {
                MsgData_sSceneObjectEnterStaticObj sstaticStruct = new MsgData_sSceneObjectEnterStaticObj();
                sstaticStruct.unpack(buffer);
                CacheSceneObj(EnEntType.EnEntType_StaticObj, sstaticStruct.Guid, sstaticStruct);
            }
            else if (CacheEnterHead.ObjType == (sbyte)EnEntType.EntType_VirtualPlayer)//EntType_VirtualPlayer
            {
                MsgData_sSceneObjectEnterVirtualPlayer sstaticStruct = new MsgData_sSceneObjectEnterVirtualPlayer();
                sstaticStruct.unpack(buffer);
                //Debug.LogError(sstaticStruct.Guid+"   " + sstaticStruct.HP+"  " +sstaticStruct.Level + "  "+sstaticStruct.Power+"  " +sstaticStruct.Wing);
                CacheSceneObj(EnEntType.EntType_VirtualPlayer, sstaticStruct.Guid, sstaticStruct);
            }


            //下方的游戏中还没用到，就不改了，减少被没必要的GC，以后需要可参照上方定义协议数据后使用
            //else if (baseStruct.ObjType == (sbyte)EnEntType.EnEntType_Patrol)
            //{
            //    PatrolEnterStruct patrolStruct = new PatrolEnterStruct();
            //    PacketUtil.Unpack<PatrolEnterStruct>(data.Data, out patrolStruct);
            //}
            //else if (baseStruct.ObjType == (sbyte)EnEntType.EnEntType_Duke)
            //{
            //    DukeEnterStruct dukeStruct = new DukeEnterStruct();
            //    PacketUtil.Unpack<DukeEnterStruct>(data.Data, out dukeStruct);
            //}

            //else if (baseStruct.ObjType == (sbyte)EnEntType.EnEntType_BiaoChe)
            //{
            //    BiaoCheEnterStruct biaocheStruct = new BiaoCheEnterStruct();
            //    PacketUtil.Unpack<BiaoCheEnterStruct>(data.Data, out biaocheStruct);
            //}
        }

        private void OnLeaveScene(GameEvent ge, EventParameter parameter)
        {
            MsgData_sSceneObjectLeaveNotify data = parameter.msgParameter as MsgData_sSceneObjectLeaveNotify;
            long serverID = (long)data.ObjectID;
            
            if (mBackgroundPlayer.ContainsKey(serverID))
            {
                mBackgroundPlayer.Remove(serverID);
                return;
            }

            for (int i = 0; i < mSceneDataList.Count; i++)
            {
                if (mSceneDataList[i].ObjGuid == serverID)
                {
                    mSceneDataList.RemoveAt(i);

                    break;
                }
            }

            for(int i = 0;i < mDeathCache.Count;i++)
            {
                if(serverID == mDeathCache[i].ID)
                {
                    mDeathCache.RemoveAt(i);

                    break;
                }
            }

            for (int i = 0; i < mSceneCache.Count; i++)
            {
                if (mSceneCache[i].ObjGuid == serverID)
                {
                    mSceneCache.RemoveAt(i);

                    return;
                }
            }

            if (!bSceneLoaded)
            {
                return;
            }

            if (data.ObjectType == (sbyte)EnEntType.EnEntType_Item)
            {
                EventParameter param = EventParameter.Get();
                param.longParameter = serverID;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OBJ_ITEM_LEAVE, param);

                return;
            }
            else if (data.ObjectType == (sbyte)EnEntType.EnEntType_GatherObj)
            {
                CollectionObj collection = CoreEntry.gEntityMgr.GetCollectionByServerID(serverID) as CollectionObj;
                if (null != collection)
                {
                    CoreEntry.gEntityMgr.RemoveCollectionByServerID(serverID);
                    collection.RecycleObj();
                }

                return;
            }
            else if (data.ObjectType == (sbyte)EnEntType.EnEntType_StaticObj)
            {
                PortalObj portal = CoreEntry.gEntityMgr.GetPortalByServerID(serverID) as PortalObj;
                if (null != portal)
                {
                    CoreEntry.gEntityMgr.RemovePortalByServerID(serverID);
                    portal.RecycleObj();
                }

                return;
            }
            else if (data.ObjectType == (sbyte)EnEntType.EnEntType_Trap)
            {
                Trap trap = CoreEntry.gEntityMgr.GetTrapByServerID(serverID) as Trap;
                if (null != trap)
                {
                    CoreEntry.gEntityMgr.RemoveTrapByServerID(serverID);
                    trap.RecycleObj();
                }

                return;
            }

            ActorObj actor = CoreEntry.gActorMgr.GetActorByServerID(serverID);
            if (null != actor)
            {
                if (!actor.mActorState.IsDeathEnd())
                {
                    if (!mLeaveCache.Contains(actor))
                    {
                        mLeaveCache.Add(actor);
                    }

                    return;
                }

                CoreEntry.gActorMgr.RemoveActorByServerID(serverID);
                actor.RecycleObj();

                EventParameter param = EventParameter.Get();
                param.longParameter = serverID;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ACTOR_REMOVE, param);

                if (actor is OtherPlayer)
                {
                    param = EventParameter.Get();
                    param.longParameter = serverID;
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OTHERPLAYER_LEAVE, param);
                }

                //有后台玩家则切换进来
                if (mBackgroundPlayer.Count > 0)
                {
                    int num = CoreEntry.gActorMgr.GetAllPlayerActors().Count - 1 + GetCacheNumber(EnEntType.EnEntType_Player);
                    if (num < CreateMaxPlayer)
                    {
                        var e = mBackgroundPlayer.GetEnumerator();
                        if (e.MoveNext())
                        {
                            ScenePlayerData value = e.Current.Value;
                            SceneObj sceneobj = CacheSceneObj(EnEntType.EnEntType_Player, value.Guid, value.EnterData);
                            sceneobj.PlayerData = value;
                            mBackgroundPlayer.Remove(value.Guid);
                        }
                        e.Dispose();
                    }                    
                }
            }
        }

        private void OnLeaveDisapper(GameEvent ge, EventParameter parameter)
        {
            MsgData_sSceneObjectDISAPPEA data = parameter.msgParameter as MsgData_sSceneObjectDISAPPEA;
            for (int i = 0; i < data.ObjectID.Count; i++)
            {
                long serverID = (long)data.ObjectID[i];
                ActorObj actor = CoreEntry.gActorMgr.GetActorByServerID(serverID);
                if (null != actor)
                {
                    if (!actor.mActorState.IsDeathEnd())
                    {
                        if (!mLeaveCache.Contains(actor))
                        {
                            mLeaveCache.Add(actor);
                        }
                        return;
                    }
                    if (actor as MonsterObj)
                    {
                        CoreEntry.gActorMgr.RemoveActorByServerID(serverID);
                        actor.RecycleObj();

                        EventParameter param = EventParameter.Get();
                        param.longParameter = serverID;
                        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ACTOR_REMOVE, param);

                    }
                }
            }

        }
        
        private void OnClearObj(GameEvent ge, EventParameter parameter)
        {
            //清除缓存数据
            mSceneDataList.Clear();
            mSceneCache.Clear();
            mBackgroundPlayer.Clear();

            //回收场景对象,本地不用回收
            List<ActorObj> actors = CoreEntry.gActorMgr.GetAllActors();
            for (int i = 0; i < actors.Count; i++)
            {
                if (actors[i] != CoreEntry.gActorMgr.MainPlayer)
                {
                    if (null != actors[i])
                    {
                        actors[i].RecycleObj();
                    }
                }
            }
            CoreEntry.gActorMgr.ClearActor();
            CoreEntry.gActorMgr.AddActorObj(CoreEntry.gActorMgr.MainPlayer);

            //其它实体对象
            List<Entity> entities = CoreEntry.gEntityMgr.GetAllEntities();
            for (int i = 0; i < entities.Count; i++)
            {
                if (null != entities[i])
                {
                    entities[i].RecycleObj();
                }
            }
            CoreEntry.gEntityMgr.ClearObjs();
        }

        //private void OnStateChanged(GameEvent ge, EventParameter parameter)
        //{
        //    MsgData_sStateChanged data = parameter.msgParameter as MsgData_sStateChanged;
        //    if (data == null)
        //        return;

        //    ActorObj actor = CoreEntry.gActorMgr.GetActorByServerID(data.RoleID);
        //    if (null == actor)
        //        return;

        //    StateParameter param = new StateParameter();
        //    if ((sbyte)EUnitBits.UNIT_BIT_DEAD == data.State)//死亡
        //    {
        //        param = new StateParameter();
        //        param.state = ACTOR_STATE.AS_DEATH;
        //        actor.RequestChangeState(param);
        //        EventParameter par = EventParameter.Get(data);
        //        par.intParameter = actor.ConfigID;
        //       // int _modelId = actor.actorCreatureInfo.ID;
        //        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_STATE_DEAD, par);
        //    }
        //}

        private void OnStateChanged(GameEvent ge, EventParameter parameter)
        {
            MsgData_sStateChanged data = parameter.msgParameter as MsgData_sStateChanged;
            ActorObj obj = CoreEntry.gActorMgr.GetActorByServerID(data.RoleID);
            if (obj != null)
            {
                obj.mBaseAttr.SetUnitBit(data.State, data.IsSet != 0);
            }
        }

        private void OnObjectDead(GameEvent ge, EventParameter parameter)
        {
            MsgData_sObjDeadInfo msg = parameter.msgParameter as MsgData_sObjDeadInfo;
            if (null == msg)
                return;

            for (int i = 0; i < mSceneDataList.Count; i++)
            {
                if (mSceneDataList[i].ObjGuid == msg.ID)
                {
                    mSceneDataList.RemoveAt(i);

                    break;
                }
            }
            for (int i = 0; i < mSceneCache.Count; i++)
            {
                if (mSceneCache[i].ObjGuid == msg.ID)
                {
                    mSceneCache.RemoveAt(i);

                    return;
                }
            }

            ActorObj behitActor = CoreEntry.gActorMgr.GetActorByServerID(msg.ID);
            if (null == behitActor)
            {
                if (!mDeathCache.Contains(msg))
                {
                    mDeathCache.Add(msg);
                }

                return;
            }
            //Debug.LogError("---------------------------------"+ msg.KillerID);
            ActorObj attackActor = CoreEntry.gActorMgr.GetActorByServerID(msg.KillerID);

            BehitParam behitParam = new BehitParam();
            DamageParam damageParam = new DamageParam();
            damageParam.attackActor = attackActor;
            damageParam.behitActor = behitActor;
            damageParam.skillID = msg.KillerSkillID;

            behitParam.damgageInfo = damageParam;
            behitActor.OnDead(msg.KillerSkillID, attackActor, behitParam, EventParameter.Get());
        }

        IEnumerator DoDead(MsgData_sObjDeadInfo msg)
        {
            yield return new WaitForSeconds(0.3f);

            ActorObj behitActor = CoreEntry.gActorMgr.GetActorByServerID(msg.ID);
            if (null == behitActor)
                yield break;

            ActorObj attackActor = CoreEntry.gActorMgr.GetActorByServerID(msg.KillerID);

            BehitParam behitParam = new BehitParam();
            DamageParam damageParam = new DamageParam();
            damageParam.attackActor = attackActor;
            damageParam.behitActor = behitActor;
            damageParam.skillID = msg.KillerSkillID;

            behitParam.damgageInfo = damageParam;
            behitActor.OnDead(msg.KillerSkillID, attackActor, behitParam, EventParameter.Get());
        }

        private void OnRevive(GameEvent ge, EventParameter parameter)
        {
            MsgData_sRevive data = parameter.msgParameter as MsgData_sRevive;
            if (null == data)
                return;

            if (0 == data.Result)
            {
                ActorObj actor = CoreEntry.gActorMgr.GetActorByServerID(data.RoleID);
                if (null == actor)
                    return;
                if (PlayerData.Instance != null)
                {
                    PlayerData.Instance.ReviceHideLessTime = 0;
                }
                actor.SetPosition((float)data.PosX, (float)data.PosY);
                actor.ForceToRevive();
                //StartCoroutine(DoRevive(actor));
            }
            else if (-1 == data.Result)
            {
                UITips.ShowTips("5s限制时间已过");
            }
            else if (-2 == data.Result)
            {
                //UITips.ShowTips("非死亡不能复活");
            }
            else if (-3 == data.Result)
            {
                UITips.ShowTips("道具不足");
            }
            else if (-4 == data.Result)
            {
                UITips.ShowTips("钻石不足");
            }
            else if (-5 == data.Result)
            {
                UITips.ShowTips("不可原地复活");
            }
        }

        private IEnumerator DoRevive(ActorObj actor)
        {
            actor.ForceToRebirth();
            yield return new WaitForSeconds(actor.GetActionLength("rebirth"));
        }

        private void OnPlayerDeath(GameEvent ge, EventParameter parameter)
        {
            if (ArenaMgr.Instance.IsArenaFight)
            {
                return;
            }

            ActorObj actor = parameter.goParameter.GetComponent<ActorObj>();
            if (MainRole.Instance.serverID == actor.ServerID)
            {

                LuaTable mapCfg = ConfigManager.Instance.Map.GetMapConfig(MapMgr.Instance.EnterMapId);
                int maptype = mapCfg != null ? mapCfg.Get<int>("type") : 0;
#if false
                if (maptype == (int)ESceneType.SCENE_TYPE_SECERETFB)
                {
                    int rtime = ConfigManager.Instance.Consts.GetValue<int>("secretAreaTeamRelivetime", "val1");
                    TipAutoClose.Show(rtime, "你已死亡！", (int sec) => { if (sec == 0) { NetLogicGame.Instance.SendReqRevive(1, 1); } return string.Format("{0}秒后自动复活......", sec); });
                    return;
                }
                else
#endif 
                if (maptype == (int)ESceneType.SCENE_TYPE_PERSONBOSS)
                {
                    MsgData_sPersonalBossResult pBossRes = new MsgData_sPersonalBossResult();
                    pBossRes.result = 1;//fail
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PersonalBossResult, EventParameter.Get(pBossRes));
                    return;
                }
                else if (maptype == (int)ESceneType.SCENE_TYPE_PERSONVIP_BOSS)
                {
                    int relivetime = ConfigManager.Instance.Consts.GetValue<int>("zhiyuanfb", "val1");
                    TipAutoClose.Show(relivetime, "你已死亡！", (int sec) => { return string.Format("{0}秒后自动复活......", sec); });

                }
                else if (maptype == (int)ESceneType.SCENE_TYPE_ZHIYUANFB )
                {
                    int relivetime = ConfigManager.Instance.Consts.GetValue<int>("zhiyuanfb", "val1");
                    TipAutoClose.Show(relivetime, "你已死亡！", (int sec) => { return string.Format("{0}秒后自动复活......", sec); });
                }
                else if (maptype == (int)ESceneType.SCENE_TYPE_WATER_DUNGEON || maptype == (int)ESceneType.SCENE_TYPE_ZHUZAIROAD)
                {
                    int relivetime = ConfigManager.Instance.Consts.GetValue<int>("zhiyuanfb", "val1");
                    TipAutoClose.Show(relivetime, "你已死亡！", (int sec) => { if (sec == 0) { NetLogicGame.Instance.SendReqRevive(1, 1); } return string.Format("{0}秒后自动复活......", sec); });
                }
                else if (maptype == (int)ESceneType.SCENE_TYPE_GUILD_HELL)
                {

                }
                else if (maptype == (int)ESceneType.SCENE_TYPE_ARENA)
                {

                }
                else if (maptype == (int)ESceneType.SCENE_TYPE_CRWON)
                {
                }
                else if (maptype == (int)ESceneType.SCENE_TYPE_YUANZHENG)
                {
                }
                else if(maptype == (int)ESceneType.SCENE_TYPE_GUILD_WAR_ACTIVITY)
                {
                    int relivetime = ConfigManager.Instance.Consts.GetValue<int>("factfightpowerInfo", "fval");
                    TipAutoClose.Show(relivetime-1, "你已死亡！", (int sec) => { return string.Format("{0}秒后自动复活......", sec); });
                }
                else
                {
                    if (CoreEntry.gAutoAIMgr.AutoFight && CoreEntry.gAutoAIMgr.Config.Relive)
                    {
                        //自动复活
                        System.Action fun = LuaMgr.Instance.GetLuaEnv().Global.Get<System.Action>("CommonRevive");
                        if (fun != null)
                        {
                            fun();
                        }
                    }
                    else
                    {
                        OtherPlayer oo = actor.hitActorObject as OtherPlayer;
                        if(oo != null)
                        {

                            UnityEngine.Events.UnityAction<string> fun = LuaMgr.Instance.GetLuaEnv().Global.Get<UnityEngine.Events.UnityAction<string>>("CommonPKReviveSimple");
                            if (fun != null)
                            {
                                fun(oo.mBaseAttr.Name);
                            }
                        }
                        else
                        {
                            System.Action fun = LuaMgr.Instance.GetLuaEnv().Global.Get<System.Action>("CommonReviveSimple");
                            if (fun != null)
                            {
                                fun();
                            }

                        }
                    }
                }
            }
        }

        private SceneObj CacheSceneObj(EnEntType type, long guid, object data)
        {
            SceneObj sceneObj = new SceneObj();
            sceneObj.ObjType = type;
            sceneObj.ObjGuid = guid;
            sceneObj.ObjData = data;

            mSceneCache.Add(sceneObj);
            mSceneDataList.Add(sceneObj);
            return sceneObj;
        }

        /// <summary>
        /// 根据serverID获取场景实体数据
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public object GetEntityData(long serverID)
        {
            for (int i = 0; i < mSceneDataList.Count; i++)
            {
                if (mSceneDataList[i].ObjGuid == serverID)
                {
                    return mSceneDataList[i].ObjData;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取缓存的对象数量。
        /// </summary>
        /// <param name="type">对象类型。</param>
        /// <returns>对象数量。</returns>
        public int GetCacheNumber(EnEntType type)
        {
            int num = 0;
            for (int i=0; i< mSceneCache.Count; ++i)
            {
                if (mSceneCache[i].ObjType == type)
                {
                    ++num;
                }
            }
            return num;
        }

        /// <summary>
        /// 更新后台玩家的移动数据位置。
        /// </summary>
        public void UpdateObjMove(long guid, int x, int y, int dir)
        {
            ScenePlayerData data;
            if (mBackgroundPlayer.TryGetValue(guid, out data))
            {
                MsgData_sSceneObjectEnterHuman edata = data.EnterData;
                edata.PosX = x;
                edata.PosY = y;
                edata.Dir = dir;
            }
        }
    }

[Hotfix]
    class SceneObj
    {
        public EnEntType ObjType;
        public long ObjGuid;
        public object ObjData;
        public ScenePlayerData PlayerData;          //当玩家从后台切到前台时使用
    }
}

