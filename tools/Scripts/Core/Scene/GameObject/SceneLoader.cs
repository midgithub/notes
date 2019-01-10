/**
* @file     : SceneLoader.cs
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-07-04
*/
using UnityEngine;
using System.Collections;
using XLua;

namespace SG
{
[Hotfix]
    public class SceneLoader : MonoBehaviour
    {

        public void LoadMagicKey(ActorObj master ,int magickeyID ,Vector3 position ,Vector3 eularAngles ,Vector3 scale)
        {
            GameObject petTest = CoreEntry.gSceneLoader.CreateGameObject(magickeyID, position, eularAngles ,scale);
            FollowPlayer magicKeyControl = petTest.GetComponent<FollowPlayer>();
            if (magicKeyControl == null)
            {
                magicKeyControl = petTest.AddComponent<FollowPlayer>();
                LuaTable modelCfg = ConfigManager.Instance.Common.GetModelConfig(magickeyID);
                magicKeyControl.idle = modelCfg.Get<string>("san_idle");
                magicKeyControl.leisure = modelCfg.Get<string>("san_leisure");
            }
            magicKeyControl.Master = master;
            master.MagicKeyModel = petTest;
        }

        /// <summary>
        /// 根据模型加载主玩家
        /// </summary>
        /// <param name="modelID"></param>
        /// <returns></returns>
        public GameObject LoadPlayer(int modelID)
        {
            GameObject obj = CreateGameObject(modelID);
            if (obj == null)
            {
                Object oj = CoreEntry.gResLoader.LoadResource("Animation/player/zhanshi/zhanshi_M_1/zhanshi_M_1_pre");
                if(oj!=null)
                obj = (GameObject)Instantiate(oj);
            }

            ActorObj actorObject = obj.GetComponent<ActorObj>();
            if (null != actorObject)
            {
                Object.DestroyImmediate(actorObject);
            }
            actorObject = obj.AddComponent<PlayerObj>();
            if (actorObject != null)
            {
                //actorObject.SetMainPlayer(true);
                actorObject.Init(modelID, modelID, MainRole.Instance.serverID);
                actorObject.mBaseAttr.Name = PlayerData.Instance.Name;
                actorObject.SetShadow(obj, modelID);
                CoreEntry.gActorMgr.MainPlayer = actorObject as PlayerObj;
            }

            CoreEntry.gActorMgr.MainPlayer.Faction = MainRole.Instance.faction; //阵营信息
            CoreEntry.gActorMgr.MainPlayer.setMainPlayer(true);
            PlayerAgent playerAgent = obj.GetComponent<PlayerAgent>();
            if (null != playerAgent)
            {
                playerAgent.enabled = false;
            }
            if(actorObject!=null)
            {
                //移动相机
                if (null != CoreEntry.gCameraMgr.MainCamera)
                {
                    CameraFollow cameraFollow = CoreEntry.gCameraMgr.MainCamera.GetComponent<CameraFollow>();
                    if (null != cameraFollow)
                    {
                        cameraFollow.jumpToTarget(actorObject.transform);
                    }
                }

                CoreEntry.gActorMgr.AddActorObj(actorObject);
                StartCoroutine(ChangePlayerShow(actorObject as PlayerObj));
            }


            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PLAYER_LOADING_OVER, null);

            return obj;
        }

        /// <summary>
        /// 加载主玩家
        /// </summary>
        public GameObject LoadPlayer()
        {
            int modelID = PlayerData.Instance.GetDressModelID();
            if (CoreEntry.gMorphMgr.IsMorphing)
            {
                modelID = CoreEntry.gMorphMgr.MorphModelID;
            }

            GameObject obj = LoadPlayer(modelID);

            Vector3 position = new Vector3(MapMgr.Instance.PlayerInitPos.x, 0f, MapMgr.Instance.PlayerInitPos.y);
            position.y = CommonTools.GetTerrainHeight(new Vector2(position.x, position.z));
            obj.transform.position = position;
            obj.transform.eulerAngles = MapMgr.Instance.PlayerInitDir;
            obj.transform.localScale = new Vector3(CoreEntry.gMorphMgr.Scale, CoreEntry.gMorphMgr.Scale, CoreEntry.gMorphMgr.Scale);

            //消息通知主角加载完
            NetLogicGame.Instance.SendReqMainPlayerEnterScene(MapMgr.Instance.EnterType);
            MapMgr.Instance.EnterType = 1;

            return obj;
        }

        IEnumerator ChangePlayerShow(PlayerObj player)
        {
            yield return null;
            if (null == player)
            {
                yield break;
            }

            if (!CoreEntry.gMorphMgr.IsMorphing)
            {
                if (PlayerData.Instance.RideData.RideState == 1)
                {
                    player.FuckHorse(PlayerData.Instance.RideData.RideID);
                }

                yield return null;
                if (null == player)
                {
                    yield break;
                }
                //BagInfo baginfo = PlayerData.Instance.BagData.GetBagInfo(BagType.ITEM_BAG_TYPE_EQUIP);
                player.ChangeWeapon(PlayerData.Instance.GetWeaponModelID());

                yield return null;
                if (null == player)
                {
                    yield break;
                }
                //player.ChangeWing(BagData.GetWingModelID(WingMgr.Instance.GetHasEquipWingId()));  //获取翅膀表id);  
                player.ChangeWing(PlayerData.Instance.GetWingModelID());  //获取翅膀表id);  
                yield return null;
            }
            //法宝
            if (null == player)
            {
                yield break;
            }
            player.ShowMagicKey(PlayerData.Instance.MagicKeyDataMgr.GetMasterMagicKey());
            player.ShowMagicKeyByStar(PlayerData.Instance.MagicKeyDataMgr.GetMasterMagicKeyStar());

            //阵法
            if (null == player)
            {
                yield break;
            }
            player.ChangeZhenFa(PlayerData.Instance.BaseAttr.ZhenFa);
        }

        public GameObject LoadOtherPlayer(int modelID)
        {
            GameObject obj = CreateGameObject(modelID);
            if (null == obj)
            {
                Object oj = CoreEntry.gResLoader.LoadResource("Animation/player/zhanshi/zhanshi_M_1/zhanshi_M_1_pre");
                if(oj != null)
                obj = (GameObject)Instantiate(oj);
            }

            ActorObj actorObject = null;
            actorObject = obj.GetComponent<ActorObj>();
            if (null != actorObject)
            {
                Object.DestroyImmediate(actorObject);
            }

            PlayerAgent agent = obj.GetComponent<PlayerAgent>();
            if (null != agent)
            {
                agent.enabled = false;
            }

            return obj;
        }

        public GameObject LoadPet(MsgData_sSceneObjectEnterPet petStruct)
        {
       
            LuaTable petCfg = ConfigManager.Instance.HeroConfig.GetHeroConfig(petStruct.ConfigID);
            if (null == petCfg)
            {
                return null;
            }

            float s = petCfg.Get<float>("scale");
            Vector3 position = CommonTools.ServerPosToClient(petStruct.PosX, petStruct.PosY);
            Vector3 eulerAngle = CommonTools.ServerDirToClient(petStruct.Dir);
            Vector3 scale = new Vector3(s, s, s);

            if (petStruct.Job <= 0)
                return null;
            int modelid = petCfg.Get<int>("model" + petStruct.Job);
            if(petStruct.Level > 0 )
            {
                LuaTable petShengjieCfg = ConfigManager.Instance.HeroConfig.GetHeroShengjieConfig(petStruct.ConfigID * 100 + petStruct.Level);
                if (petShengjieCfg != null)
                {
                    modelid = petShengjieCfg.Get<int>("model" + petStruct.Job);
                }
            }
            GameObject obj = CreateGameObject(modelid, position, eulerAngle, scale);
            if (obj == null)
            {
                return null;
            }

            //obj.transform.localScale = Vector3.one;

            ActorObj actorObject = null;
            actorObject = obj.GetComponent<ActorObj>();
            if (null != actorObject)
            {
                Object.DestroyImmediate(actorObject);
            }

            PlayerAgent agent = obj.GetComponent<PlayerAgent>();
            if (null != agent)
            {
                agent.enabled = false;
            }

            PetObj pet = obj.AddComponent<PetObj>();
            pet.Starlevel = petStruct.Level;
            pet.Qua = petStruct.Qua;
            pet.OwnName = UiUtil.GetNetString(petStruct.RoleName);

            pet.SetMaster(petStruct.Owner);
            //Debug.LogError("pet :" + petStruct.Owner + " id:" + petStruct.ConfigID);
            pet.Init(modelid, petStruct.ConfigID, petStruct.Guid);
            pet.ServerID = petStruct.Guid;
            pet.InitSpeed(CommonTools.ServerValueToClient(petStruct.Speed));
            CoreEntry.gActorMgr.AddActorObj(pet);

            //宠物创建时，需要检查主人的显示隐藏属性
            if(pet.m_MasterActor && !pet.m_MasterActor.Visiable)
            {
                pet.HideSelf();
            }

            //ActorObj owner = CoreEntry.gActorMgr.GetPlayerActorByServerID(petStruct.Owner);
            //if (null != owner)
            //{
            //    if (owner.mActorType == ActorType.AT_LOCAL_PLAYER)
            //    {
            //        pet.ChangeWeapon(PlayerData.Instance.GetWeaponModelID());
            //    }
            //    else
            //    {
            //        MsgData_sSceneObjectEnterHuman ownerData = CoreEntry.gSceneObjMgr.GetEntityData(petStruct.Owner) as MsgData_sSceneObjectEnterHuman;
            //        if (null != ownerData)
            //        {
            //            pet.ChangeWeapon(GetWeaponModelID(ownerData.FashionState == 0 ? ownerData.FashionWeapon : 0, ownerData.ShenBin, ownerData.Weapon, ownerData.Prof));
            //        }
            //    }
            //}

            return obj;
        }

        /// <summary>
        /// 加载镖车, 由于镖车有血量和阵营，并且可以被攻击，类似同阵营玩家，先用于玩家代替！！
        /// </summary>
        public GameObject LoadBiaoChe(MsgData_sSceneObjectEnterBiaoChe cheStruct)
        {
            ActorObj ao = CoreEntry.gActorMgr.GetPlayerActorByServerID(cheStruct.Guid);
            if (null != ao) return ao.gameObject;

            LuaTable row = RawLuaConfig.Instance.GetRowData("t_kuafubiaoche", cheStruct.DartID);
            if (null == row) return null;

            int modelid = row.Get<int>("model");
        
            Vector3 position = CommonTools.ServerPosToClient(cheStruct.PosX, cheStruct.PosY);
            Vector3 eulerAngle = CommonTools.ServerDirToClient(cheStruct.Dir);
            Vector3 scale = new Vector3(1, 1, 1);

            GameObject obj = LoadOtherPlayer(modelid);
            obj.transform.position = position;
            obj.transform.eulerAngles = eulerAngle;
            obj.transform.localScale = scale;

            OtherPlayer BiaoChe = obj.AddComponent<OtherPlayer>();

            BiaoChe.Faction = cheStruct.Camp;
            if(BiaoChe.mBaseAttr == null) BiaoChe.mBaseAttr = new BaseAttr();
            BiaoChe.mBaseAttr.MaxHP = cheStruct.HP;
            BiaoChe.mBaseAttr.CurHP = cheStruct.HP;
            BiaoChe.mBaseAttr.Level = cheStruct.Level;
            BiaoChe.mBaseAttr.Name = CommonTools.BytesToString(cheStruct.LeaderName) + "的镖车";
            BiaoChe.Init(modelid, modelid, cheStruct.Guid);
            BiaoChe.mActorType = ActorType.AT_MECHANICS;
            BiaoChe.mBaseAttr.Speed = CommonTools.ServerValueToClient(cheStruct.Speed);
            BiaoChe.SetSpeed(BiaoChe.mBaseAttr.Speed);
            BiaoChe.ServerID = cheStruct.Guid;
            CoreEntry.gActorMgr.AddActorObj(BiaoChe);

            return obj;
        }

        /// <summary>
        /// 加载其它玩家
        /// </summary>
        /// <param name="humanStruct"></param>
        /// <returns></returns>
        public GameObject LoadOtherPlayer(MsgData_sSceneObjectEnterHuman humanStruct)
        {
            int modelID = GetClothesModelID(humanStruct.FashionState == 0 ? humanStruct.FashionDress : 0, humanStruct.Dress, humanStruct.Prof,humanStruct.EquipStarMin);
            
            if (humanStruct.ChangeID != 0)
            {
                CoreEntry.gMorphMgr.SetInMorph(humanStruct.Guid);
                //Configs.changeConfig changeCfg = CSVConfigLoader.GetchangeConfig((int)humanStruct.ChangeID);
                LuaTable changeCfg = ConfigManager.Instance.Actor.GetChangeConfig((int)humanStruct.ChangeID);
                if (changeCfg != null)
                {
                    modelID = changeCfg.Get<int>("model_id");
                }
            }

            GameObject obj = LoadOtherPlayer(modelID);

            Vector3 position = CommonTools.ServerPosToClient(humanStruct.PosX, humanStruct.PosY);
            position.y = CommonTools.GetTerrainHeight(new Vector2(position.x, position.z));
            Vector3 eulerAngle = CommonTools.ServerDirToClient(humanStruct.Dir);

            obj.transform.position = position;
            obj.transform.eulerAngles = eulerAngle;
            obj.transform.localScale = Vector3.one;

            OtherPlayer otherPlayer = obj.AddComponent<OtherPlayer>();

            otherPlayer.Faction = humanStruct.Faction;
                    
            otherPlayer.Init(modelID, modelID, humanStruct.Guid);            
            otherPlayer.ServerID = humanStruct.Guid;
            otherPlayer.InitAttr(humanStruct);
            CoreEntry.gActorMgr.AddActorObj(otherPlayer);
            otherPlayer.Health.OnCreateHPBar();    //名字有更新，重新创建血条
            PetObj petObj = CoreEntry.gActorMgr.GetPetActorByServerID(humanStruct.MoShenID) as PetObj;
            if(petObj != null)
            {
                petObj.SetMaster(otherPlayer);
                //Debug.LogError("找到 moshenid: " + humanStruct.MoShenID + " otherplayer:" + humanStruct.Guid);
            }
            else
            {
                //Debug.LogError("未找到 moshenid: " + humanStruct.MoShenID + " otherplayer:" + humanStruct.Guid);
            }

            //LoadMagicKey(otherPlayer, 620000001, position, eulerAngle);

            StartCoroutine(ChangeOtherPlayerShow(otherPlayer as OtherPlayer, humanStruct));

            EventParameter parameter = new EventParameter();
            parameter.objParameter = otherPlayer;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OTHERPLAYER_LOAD_OVER, parameter);

            return obj;
        }

        /// <summary>
        /// 逐步改变玩家外观，如果改变过程中玩家被消耗则停止
        /// </summary>
        IEnumerator ChangeOtherPlayerShow(OtherPlayer player, MsgData_sSceneObjectEnterHuman hes)
        {
            yield return null;
            if (player != null)
            {
                player.ChangeWing(GetWingModelID(hes.FashionState == 0 ? hes.FashionWing : 0, hes.Wing, hes.Prof));
            }
            else
            {
                yield break;
            }
            
            yield return null;
            if (player != null)
            {
                player.FuckHorse(hes.Ride);
            }
            else
            {
                yield break;
            }

            yield return null;
            if (player != null)
            {
                player.ChangeWeapon(GetWeaponModelID(hes.FashionState == 0 ? hes.FashionWeapon : 0, hes.ShenBin, hes.Weapon, hes.Prof));
            }
            else
            {
                yield break;
            }

            yield return null; 
            if (player != null)
            {
                player.ShowMagicKey(hes.MagicKey);
                player.ShowMagicKeyByStar(hes.MagicKeyStar);
            }
            else
            {
                yield break;
            }

            yield return null;
            if (player != null)
            {
                player.ChangeZhenFa(hes.ZhenfaId);
            }
            else
            {
                yield break;
            }
        }
        

        /// <summary>
        /// 获取衣服模型。
        /// </summary>
        /// <param name="fashion">时装。</param>
        /// <param name="equip">装备。</param>
        /// <param name="job">职业。</param>
        /// <returns>按优先级获取的模型ID。</returns>
        public static int GetClothesModelID(int fashion, int equip, int job,int equipStarMin)
        {
            //衣服显示优先级 时装>装备>模型默认
            if (fashion != 0)
            {
                return ConfigManager.Instance.BagItem.GetFashionModelID(fashion, job, equipStarMin);
            }

            //读取装备衣服，没有则读取角色默认衣服
            int dress = equip;
            if (dress == 0)
            {
                LuaTable cfg = ConfigManager.Instance.Actor.GetPlayerInfoConfig(job);
                if (cfg != null)
                {
                    dress = cfg.Get<int>("dress");
                }
            }
            if (dress == 0)
            {
                LogMgr.ErrorLog("角色职业默认衣服配置错误，衣服编号为0");
                return 0;
            }

            LuaTable equipCfg = ConfigManager.Instance.BagItem.GetEquipConfig(dress);
            if (null == equipCfg)
            {
                LogMgr.ErrorLog("没有找到编号为{0}的衣服", dress);
                return 0;
            }
             
            int modelid = EquipDataMgr.Instance.GetEquipStarModelID(job, equipStarMin);
            if (modelid == 0)
            {
                modelid = equipCfg.Get<int>(string.Format("vmesh{0}", job));
            }

            return modelid;
        }

        /// <summary>
        /// 获取翅膀模型。
        /// </summary>
        /// <param name="fashion">时装。</param>
        /// <param name="equip">装备。</param>
        /// <param name="job">职业。</param>
        /// <returns>按优先级获取的模型ID。</returns>
        public static int GetWingModelID(int fashion, int equip, int job)
        {
            //翅膀显示优先级 时装>装备>模型默认
            if (fashion != 0)
            {
                return ConfigManager.Instance.BagItem.GetFashionModelID(fashion, job,0);
            }

            return ConfigManager.Instance.BagItem.GetWingModelID(equip);
        }

        

        /// <summary>
        /// 获取武器模型。
        /// </summary>
        /// <param name="fashion">时装。</param>
        /// <param name="sb">神兵。</param>
        /// <param name="equip">武器。</param>
        /// <param name="job">职业。</param>
        /// <returns>按优先级获取的模型ID。</returns>
        public static int GetWeaponModelID(int fashion, int magicweapon, int equip, int job)
        {
            //武器显示优先级 时装>神兵>装备>模型默认
            if (fashion != 0)
            {
                return ConfigManager.Instance.BagItem.GetFashionModelID(fashion, job,0);
            }

            //神兵外观读取
            if(magicweapon != 0)
            {
                int configId = job * 100 + magicweapon;
                int mid = ShenBingMgr.Instance.GetShenBingModelId(configId);
                if (mid > 0)
                {
                    return mid;
                }else
                {
                    LogMgr.DebugLog("模型配置表有误,找不到神兵模型id " + configId);
                }
            }

            //读取装备武器，没有则读取角色默认武器
            int weapon = equip;
            if (weapon == 0)
            {
                LuaTable cfg = ConfigManager.Instance.Actor.GetPlayerInfoConfig(job);
                if (cfg != null)
                {
                    weapon = cfg.Get<int>("arm");
                }
            }
            if (weapon == 0)
            {
                LogMgr.ErrorLog("角色职业默认武器配置错误，武器编号为0");
                return 0;
            }

            LuaTable equipCfg = ConfigManager.Instance.BagItem.GetItemConfig(weapon);
            if (null == equipCfg)
            {
                LogMgr.ErrorLog("没有找到编号为{0}的武器", weapon);
                return 0;
            }

            int modelid = 0;
            switch (job)
            {
                case 1:
                    modelid = equipCfg.Get<int>("vmesh1");
                    break;
                case 2:
                    modelid = equipCfg.Get<int>("vmesh2");
                    break;
                case 3:
                    modelid = equipCfg.Get<int>("vmesh3");
                    break;
                case 4:
                    modelid = equipCfg.Get<int>("vmesh4");
                    break;
                default:
                    break;
            }

            return modelid;
        }

        /// <summary>
        /// 加载怪物
        /// </summary>
        /// <param name="monsterStruct"></param>
        /// <returns></returns>
        public GameObject LoadMonster(MsgData_sSceneObjectEnterMonster monsterStruct)
        {
            if (MainPanelMgr.Instance.bInsertStatus) return null;
            LuaTable cfg = ConfigManager.Instance.Actor.GetMonsterConfig(monsterStruct.ConfigID);
            if (null == cfg)
            {
                return null;
            }

            int modleid = cfg.Get<int>("modelId");
            LuaTable modelCfg = ConfigManager.Instance.Common.GetModelConfig(modleid);

            Vector3 position = CommonTools.ServerPosToClient(monsterStruct.PosX, monsterStruct.PosY);
            Vector3 eulerAngle = CommonTools.ServerDirToClient(monsterStruct.Dir);
            GameObject obj = CreateGameObject(modleid, position, eulerAngle);
            if (obj == null)
            {
                Object oj = CoreEntry.gResLoader.LoadResource("Animation/monster/mon041_shouchengshibing/mon041_shouchengshibing_pre");
                if(oj!=null)
                obj  = (GameObject)Instantiate(oj);
            }
            
            {

                float s = cfg.Get<float>("scale");
                obj.transform.localScale = new Vector3(s, s, s);
                ParticleScaler[] scalers = obj.GetComponentsInChildren<ParticleScaler>(true);
                for (int i = 0; i < scalers.Length; i++)
                {
                    scalers[i].particleScale = s;
                }

                ActorObj actorObject = null;
                actorObject = obj.GetComponent<ActorObj>();
                if (null != actorObject)
                {
                    Object.DestroyImmediate(actorObject);
                }

                MonsterObj monster = obj.AddComponent<MonsterObj>();
                if (monster != null)
                {
                    monster.Init(modleid, monsterStruct.ConfigID, monsterStruct.Guid);
                    monster.ServerID = monsterStruct.Guid;
                    monster.InitAttr(monsterStruct);
                    monster.mBaseAttr.Name = cfg.Get<string>("name");
                    monster.mBaseAttr.Level = cfg.Get<int>("level");
                    monster.Faction = monsterStruct.Faction;
                    CoreEntry.gActorMgr.AddActorObj(monster);
                    StoryTriggerOnBossBorn.CheckRegister();

                }
                else
                {
                    //Debug.LogError("monster null");
                    return obj;
                }

                EventParameter parameter = EventParameter.Get();
                parameter.goParameter = obj;
                parameter.objParameter = monster;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_MONSTER_LOADED, parameter);

                return obj;
            }
        }

        /// <summary>
        /// 加载NPC
        /// </summary>
        /// <param name="npcStruct"></param>
        /// <returns></returns>
        public GameObject LoadNPC(MsgData_sSceneObjectEnterNPC npcStruct)
        {
            LuaTable npcCfg = ConfigManager.Instance.Actor.GetNPCConfig(npcStruct.ConfigID);
            if (null == npcCfg)
            {
                return null;
            }

            Vector3 position = CommonTools.ServerPosToClient(npcStruct.PosX, npcStruct.PosY);
            Vector3 eulerAngle = CommonTools.ServerDirToClient(npcStruct.Dir);
            GameObject obj = CreateGameObject(npcCfg.Get<int>("look"), position, eulerAngle);
            if (obj == null)
            {
                return null;
            }

            float s = npcCfg.Get<float>("scale");
            obj.transform.localScale = new Vector3(s, s, s);
            ParticleScaler[] scalers = obj.GetComponentsInChildren<ParticleScaler>(true);
            for (int i = 0; i < scalers.Length; i++)
            {
                scalers[i].particleScale = s;
            }

            ActorObj actorObject = null;
            actorObject = obj.GetComponent<ActorObj>();
            if (null != actorObject)
            {
                Object.DestroyImmediate(actorObject);
            }

            NpcObj npc = obj.AddComponent<NpcObj>();
            npc.Init(npcCfg.Get<int>("look"), npcStruct.ConfigID, npcStruct.Guid);
            npc.ServerID = npcStruct.Guid;
            npc.mBaseAttr.Name = npcCfg.Get<string>("name");
            npc.mBaseAttr.MaxHP = 1;        //防止除0错误
            npc.Health.OnCreateHPBar();

            CoreEntry.gActorMgr.AddActorObj(npc);

            return obj;
        }

        /// <summary>
        /// 加载采集点
        /// </summary>
        /// <param name="collectionStruct"></param>
        /// <returns></returns>
        public GameObject LoadCollection(MsgData_sSceneObjectEnterCollection collectionStruct)
        {
            LuaTable collectionCfg = ConfigManager.Instance.Map.GetCollectionConfig(collectionStruct.ConfigID);
            if (null == collectionCfg)
            {
                return null;
            }

            Vector3 position = CommonTools.ServerPosToClient(collectionStruct.PosX, collectionStruct.PosY);
            Vector3 eulerAngle = CommonTools.ServerDirToClient(collectionStruct.Dir);
            GameObject obj = CreateGameObject(collectionCfg.Get<int>("modelId"), position, eulerAngle);
            if (obj == null)
            {
                return null;
            }
            obj.name = collectionStruct.Guid.ToString();
            float s = collectionCfg.Get<float>("scale");
            obj.transform.localScale = new Vector3(s, s, s);
            ParticleScaler[] scalers = obj.GetComponentsInChildren<ParticleScaler>(true);
            for (int i = 0; i < scalers.Length; i++)
            {
                scalers[i].particleScale = s;
            }

            Entity actorObject = null;
            actorObject = obj.GetComponent<Entity>();
            if (null != actorObject)
            {
                Object.DestroyImmediate(actorObject);
            }

            CollectionObj collection = obj.AddComponent<CollectionObj>();
            collection.Init(collectionCfg.Get<int>("modelId"), collectionStruct.ConfigID, collectionStruct.Guid);
            collection.ConfigID = collectionStruct.ConfigID;

            CoreEntry.gEntityMgr.AddCollection(collection);

            return obj;
        }

        /// <summary>
        /// 静态物体，传送门加载
        /// </summary>
        /// <param name="staticStruct"></param>
        /// <returns></returns>
        public GameObject LoadStaticObj(MsgData_sSceneObjectEnterStaticObj staticStruct)
        {
            if (null != CoreEntry.gEntityMgr.GetPortalByServerID(staticStruct.Guid))
            {
                return null;
            }

            LuaTable portalCfg = ConfigManager.Instance.Map.GetPortalConfig(staticStruct.ConfigID);
            if (null == portalCfg)
            {
                return null;
            }

            Vector3 position = CommonTools.ServerPosToClient(staticStruct.PosX, staticStruct.PosY);
            Vector3 eulerAngle = CommonTools.ServerDirToClient(staticStruct.Dir);
            GameObject obj = CreateGameObject(portalCfg.Get<int>("modelId"), position, eulerAngle);
            if (obj == null)
            {
                obj = CoreEntry.gObjPoolMgr.ObtainObject(portalCfg.Get<int>("modelId"));
                if (null == obj)
                {
                    obj = new GameObject("Portal");
                }

                position.y = CommonTools.GetTerrainHeight(new Vector2(position.x, position.z));
                obj.transform.position = position;
                obj.transform.eulerAngles = eulerAngle;
            }

            Entity actorObject = null;
            actorObject = obj.GetComponent<Entity>();
            if (null != actorObject)
            {
                Object.DestroyImmediate(actorObject);
            }

            PortalObj portal = obj.AddComponent<PortalObj>();
            portal.Init(portalCfg.Get<int>("modelId"), staticStruct.ConfigID, staticStruct.Guid);
            portal.ConfigID = staticStruct.ConfigID;

            CoreEntry.gEntityMgr.AddPortal(portal);

            return obj;
        }

        /// <summary>
        /// 加载陷阱
        /// </summary>
        /// <param name="trapStruct"></param>
        /// <returns></returns>
        public GameObject LoadTrap(MsgData_sSceneObjectEnterTrap trapStruct)
        {
            LuaTable trapCfg = ConfigManager.Instance.Map.GetTrapConfig(trapStruct.ConfigID);
            if (null == trapCfg)
            {
                return null;
            }

            string effectPath = "";
            if (trapStruct.Owner == CoreEntry.gActorMgr.MainPlayer.ServerID)
            {
                effectPath = trapCfg.Get<string>("caster");
            }
            else
            {
                effectPath = trapCfg.Get<string>("model");
            }
            GameObject obj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(effectPath);
            if (null == obj)
            {
                Object o = CoreEntry.gResLoader.LoadResource(effectPath);
                if (null == o)
                    return null;

                obj = (GameObject)Instantiate(o);
            }

            Vector3 position = CommonTools.ServerPosToClient(trapStruct.PosX, trapStruct.PosY);
            position.y = CommonTools.GetTerrainHeight(new Vector2(position.x, position.z));
            Vector3 eulerAngle = CommonTools.ServerDirToClient(trapStruct.Dir);
            obj.transform.position = position;
            obj.transform.eulerAngles = eulerAngle;
            obj.transform.localScale = Vector3.one;

            Entity actorObject = null;
            actorObject = obj.GetComponent<Entity>();
            if (null != actorObject)
            {
                Object.DestroyImmediate(actorObject);
            }

            Trap trap = obj.AddComponent<Trap>();
            trap.Init(0, trapStruct.ConfigID, trapStruct.Guid);

            CoreEntry.gEntityMgr.AddTrap(trap);

            return obj;
        }

        public GameObject LoadCamp(long serverID, Vector3 pos, Vector3 scale)
        {
            GameObject obj = new GameObject("Camp");

            pos.y = CommonTools.GetTerrainHeight(new Vector2(pos.x, pos.z));
            obj.transform.parent = CoreEntry.gObjPoolMgr.ObjectPoolRoot.transform;
            obj.transform.position = pos;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            CampObj camp = obj.AddComponent<CampObj>();
            camp.Init(0, 0, serverID);
            camp.EffectSize = scale;

            CoreEntry.gEntityMgr.AddCamp(camp);

            return obj;
        }

        public GameObject LoadVirtualPlayer(MsgData_sSceneObjectEnterVirtualPlayer player)
        {
            int modelID = SceneLoader.GetClothesModelID(player.FashionDress, player.Dress, player.Job, 0);

            Vector3 position = CommonTools.ServerPosToClient(player.PosX, player.PosY);
            Vector3 eulerAngle = CommonTools.ServerDirToClient(player.Dir);
            GameObject obj = CreateGameObject(modelID, position, eulerAngle);

            ActorObj actorObject = null;
            actorObject = obj.GetComponent<ActorObj>();
            if (null != actorObject)
            {
                UnityEngine.Object.DestroyImmediate(actorObject);
            }

            PlayerAgent agent = obj.GetComponent<PlayerAgent>();
            if (null != agent)
            {
                agent.enabled = false;
            }
            
            /*Vector3 position = Vector3.zero;
            position.x = player.PosX;
            position.z = player.PosY;
            position.y = CommonTools.GetTerrainHeight(new Vector2(position.x, position.y));

            obj.transform.position = position;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;*/

            OtherPlayer otherPlayer = obj.AddComponent<OtherPlayer>();
            otherPlayer.Init(modelID, modelID, player.Guid, "", true);
            otherPlayer.InitEnterHumanAttr(player);
            CoreEntry.gActorMgr.AddActorObj(otherPlayer);
            otherPlayer.Health.OnCreateHPBar();

            StartCoroutine(ChangeVirtualPlayerShow(otherPlayer as OtherPlayer, player));

            int weaponID = SceneLoader.GetWeaponModelID(player.FashionWeapon, player.ShenBing, player.Weapon, player.Job);
            LoadWeapon(obj.transform, weaponID, player.Job == 4);

            return obj;
        }

        IEnumerator ChangeVirtualPlayerShow(OtherPlayer player, MsgData_sSceneObjectEnterVirtualPlayer hes)
        {
            yield return null;
            if (player != null)
            {
                player.ChangeWing(GetWingModelID(0 , hes.Wing, hes.Job));
            }
            else
            {
                yield break;
            }

            yield return null;
            if (player != null)
            {
                player.FuckHorse(hes.WuHunID);
            }
            else
            {
                yield break;
            }

            yield return null;
            if (player != null)
            {
                player.ChangeWeapon(GetWeaponModelID( hes.FashionWeapon , hes.ShenBing, hes.Weapon, hes.Job));
            }
            else
            {
                yield break;
            }

            yield return null;
            if (player != null)
            {
                player.ShowMagicKey(hes.MagicKey);
                player.ShowMagicKeyByStar(hes.MagicKeyStar);
            }
            else
            {
                yield break;
            }

            yield return null;
            if (player != null)
            {
                player.ChangeZhenFa(hes.ZhenfaId);
            }
            else
            {
                yield break;
            }
        }

        private void LoadWeapon(Transform mainBody, int id, bool bDoubleWeapon)
        {
            Transform rTran = ActorObj.RecursiveFindChild(mainBody, "DM_R_Hand");
            if (null == rTran)
            {
                LogMgr.UnityError("no DM_R_Hand point!!!");

                return;
            }

            GameObject obj = SceneLoader.LoadModelObject(id);
            if (null == obj)
            {
                LogMgr.UnityError("no weapon:id " + id);

                return;
            }

            if (bDoubleWeapon)
            {
                Transform lTran = ActorObj.RecursiveFindChild(mainBody, "DM_L_Hand");
                if (null == lTran)
                {
                    LogMgr.UnityError("no DM_L_Hand point!!!");

                    return;
                }

                Transform lWeapon = obj.transform.Find("DM_L_wuqi01");
                Transform rWeapon = obj.transform.Find("DM_R_wuqi01");

                lWeapon.SetParent(lTran);
                lWeapon.localPosition = Vector3.zero;
                lWeapon.localScale = Vector3.one;
                lWeapon.localRotation = Quaternion.identity;

                rWeapon.SetParent(rTran);
                rWeapon.localPosition = Vector3.zero;
                rWeapon.localScale = Vector3.one;
                rWeapon.localRotation = Quaternion.identity;

                UnityEngine.Object.Destroy(obj);
            }
            else
            {
                obj.transform.SetParent(rTran);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                obj.transform.localRotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// 根据模型ID，位置和角度、缩放来创建对象
        /// </summary>
        /// <param name="modelID"></param>
        /// <param name="position"></param>
        /// <param name="eulerAngle"></param>
        /// <returns></returns>
        private GameObject CreateGameObject(int modelID, Vector3 position, Vector3 eulerAngle,Vector3 scale)
        {
            GameObject obj = CreateGameObject(modelID);

            if (null != obj)
            {
                position.y = CommonTools.GetTerrainHeight(new Vector2(position.x, position.z));
                obj.transform.position = position;
                obj.transform.eulerAngles = eulerAngle;
                obj.transform.localScale = scale;
            }

            return obj;
        }

        /// <summary>
        /// 根据模型ID，位置和角度来创建对象
        /// </summary>
        /// <param name="modelID"></param>
        /// <param name="position"></param>
        /// <param name="eulerAngle"></param>
        /// <returns></returns>
        private GameObject CreateGameObject(int modelID, Vector3 position, Vector3 eulerAngle)
        {
            return CreateGameObject(modelID, position, eulerAngle, Vector3.one);
        }

        /// <summary>
        /// 根据模型ID创建对象
        /// </summary>
        /// <param name="modelID"></param>
        /// <returns></returns>
        public GameObject CreateGameObject(int modelID)
        {
            //modelConfig modelCfg = CSVConfigManager.GetmodelConfig(modelID);
            LuaTable cfg = ConfigManager.Instance.Common.GetModelConfig(modelID);
            if (null == cfg)
            {
                return null;
            }

            GameObject obj = CoreEntry.gObjPoolMgr.ObtainObject(modelID);
            if (null == obj)
            {
                LogMgr.Log("ObjPoolMgr ObtainObject error: 1 " + modelID);
                CoreEntry.gObjPoolMgr.PushToPool(modelID);
                /*
                Object o = CoreEntry.gResLoader.LoadResource(cfg.Get<string>("skl"));
                if (null == o)
                {
                    LogMgr.Log("ResourceLoader LoadResource error: " + modelID);
                    return null;
                }

                obj = (GameObject)Instantiate(o);
                */
                obj = CoreEntry.gObjPoolMgr.ObtainObject(modelID);
                if (null == obj)
                {
                    LogMgr.Log("ObjPoolMgr ObtainObject error: 2 " + modelID);
                }
            }
            return obj;
        }

        /// <summary>
        /// 加载模型对象，不走缓存池。
        /// </summary>
        /// <param name="id">模型ID。</param>
        /// <param name="isprefab">是否加载prefa。</param>
        /// <returns>模型对象</returns>
        public static GameObject LoadModelObject(int id, bool isprefab = false)
        {
            LuaTable cfg = ConfigManager.Instance.Common.GetModelConfig(id);
            if (null == cfg)
            {
                return null;
            }

            GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(cfg.Get<string>("skl"), typeof(GameObject));
            if (prefab == null)
            {
                return null;
            }

            GameObject obj = isprefab ? prefab : (Instantiate(prefab) as GameObject);
            return obj;
        }
    }
}

