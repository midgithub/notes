/**
* @file     : HPBarManager.cs
* @brief    : 
* @details  : 血条管理器。
* @author   : XuXiang
* @date     : 2017-06-14 11:20
*/

using XLua;
using UnityEngine;
using System.Collections;
using SG;
using System.Collections.Generic;

namespace SG
{
    public enum FightMapLogo
    { 
       None = 0,
       TypeOne = 1,  
       TypeTwo = 2, 
    }
[Hotfix]
    public class HPBarManager
    {
        //血条管理单实例。
        private static HPBarManager _instance = null;

        /// <summary>
        /// 获取血条管理。
        /// </summary>
        public static HPBarManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HPBarManager();
                }
                return _instance;
            }
        }

        public static string HPBarPlayerPrefab = "UI/Prefabs/HPBar/FirstRes/ItemHPBarPlayer";                          //玩家血条原型
        public static string HPBarMonsterPrefab = "UI/Prefabs/HPBar/FirstRes/ItemHPBarMonster";                         //怪物血条原型
        public static string HPBarBossPrefab = "UI/Prefabs/HPBar/FirstRes/ItemHPBarBoss";                             //Boss血条原型
        public static string HPBarNPCPrefab = "UI/Prefabs/HPBar/FirstRes/ItemHPBarNPC";                             //NPC血条原型        
        public static string HPBarPetPrefab = "UI/Prefabs/HPBar/FirstRes/ItemHPBarPet";                             //pet血条原型        

        /// <summary>
        /// 血条缓存。
        /// </summary>
        private Dictionary<ActorType, Stack<HPBar>> m_CacheHPBar = new Dictionary<ActorType, Stack<HPBar>>();

        /// <summary>
        /// 角色对应的血条。
        /// </summary>
        private Dictionary<ActorHealth, HPBar> mActorHPBar = new Dictionary<ActorHealth, HPBar>();

        /// <summary>
        /// 血条挂接点。
        /// </summary>
        private Transform m_HPBarRoot;

        /// <summary>
        /// 获取血条挂接点。
        /// </summary>
        public Transform HPBarRoot
        {
            get
            {
                if (m_HPBarRoot == null)
                {
                    string path = "UI/Prefabs/HPBar/FirstRes/HPBarRoot";
                    GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(path, typeof(GameObject));
                    if(prefab == null)return null;
                    GameObject obj = GameObject.Instantiate(prefab) as GameObject;
                    obj.SetActive(true);
                    obj.GetComponent<Canvas>().worldCamera = MainPanelMgr.Instance.uiCamera;
                    GameObject.DontDestroyOnLoad(obj);
                    m_HPBarRoot = obj.transform;
                }

                return m_HPBarRoot;
            }
        }

        /// <summary>
        /// 获取是否有Boss血条。
        /// </summary>
        public bool IsHaveBossBar
        {
            get
            {
                var a = mActorHPBar.GetEnumerator();
                bool ret = false;
                while (a.MoveNext())
                {
                    var b = a.Current.Key;
                    if (b.ActorType == ActorType.AT_BOSS)
                    {
                        ret = true;
                        break;
                    }
                }
                a.Dispose();
                return ret;
            }
        }

        /// <summary>
        /// 注册游戏事件。
        /// </summary>
        public void RegisterGameEventListener()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_HPBAR_CREATE, OnGameEventCreate);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_HPBAR_DESTORY, OnGameEventDestory);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_HPBAR_UPDATE, OnGameEventUpdate);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_HPBAR_CHANGENODE, OnGameEventChangeNode);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_HPBAR_PKSTATUS, OnGameEventPKStatus);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_COLLECT_INFO_CREATE, OnGameEventCollectInfoCreate);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_COLLECT_INFO_DESTORY, OnGameEventCollectInfoDestory);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_MAGICKEY_INFO_CREATE, OnGameEventMagickeyInfoCreate);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_MAGICKEY_INFO_DESTORY, OnGameEventMagickeyInfoDestroy);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BEGIN_LOADSCENE, OnBeginLoadScen);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CC_ChangeHeroTitle, OnChangeHeroTitle);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CC_ChangeFacion, OnChangeFaction);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, onClearUseData);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_PET_LEVEL_UPDATE, OnGameEventPetUpdate);
            

        }

        private HPBar CreateHPBar(ActorType at)
        {
            string prefabpath = string.Empty;
            switch (at)
            {
                case ActorType.AT_LOCAL_PLAYER:
                case ActorType.AT_REMOTE_PLAYER:
                case ActorType.AT_PVP_PLAYER:
                    prefabpath = HPBarPlayerPrefab;
                    break;
                case ActorType.AT_MONSTER:
                    prefabpath = HPBarMonsterPrefab;
                    break;
                case ActorType.AT_BOSS:
                    prefabpath = HPBarBossPrefab;
                    break;
                case ActorType.AT_NPC:
                    prefabpath = HPBarNPCPrefab;
                    break;
                case ActorType.AT_PET:
                    prefabpath = HPBarPetPrefab;
                    break;
                default:
                    return null;
            }
            
            GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(prefabpath, typeof(GameObject));
            if(prefab == null)return null;

            GameObject obj = GameObject.Instantiate(prefab) as GameObject;            
            RectTransform rt = obj.GetComponent<RectTransform>();
            HPBar ret = obj.GetComponent<HPBar>();
            obj.SetActive(true);
            rt.SetParent(HPBarRoot);
            rt.anchoredPosition3D = Vector3.zero;
            rt.localScale = Vector3.one;
            ret.BarType = at;
            if (ret.BarType == ActorType.AT_BOSS)
            {
                rt.anchoredPosition = new Vector2(40, 260);
            }
            return ret;
        }

        public HPBar GetHPBar(ActorType at)
        {
            Stack<HPBar> cache;
            if (!m_CacheHPBar.TryGetValue(at, out cache))
            {
                cache = new Stack<HPBar>();
                m_CacheHPBar.Add(at, cache);
            }

            if (cache.Count > 0)
            {
                HPBar bar = cache.Pop();
                bar.gameObject.SetActive(true);
                return bar;
            }

            return CreateHPBar(at);
        }

        /// <summary>
        /// 移除血条。
        /// </summary>
        /// <param name="bar">血条对象。</param>
        public void RemoveHPBar(HPBar bar)
        {
            if (bar == null)
            {
                return;
            }

            Stack<HPBar> cache;
            if (!m_CacheHPBar.TryGetValue(bar.BarType, out cache))
            {
                cache = new Stack<HPBar>();
                m_CacheHPBar.Add(bar.BarType, cache);
            }

            bar.gameObject.SetActive(false);
            cache.Push(bar);
        }

        /// <summary>
        /// 血条创建处理。
        /// </summary>
        public void OnGameEventCreate(GameEvent ge, EventParameter parameter)
        {
            ActorHealth health = parameter.objParameter as ActorHealth;
            HPBar bar;
            int num = 1;
            if (health.ActorType == ActorType.AT_BOSS)
            {
                num = (health.Actor as MonsterObj).MonsterConfig.Get<int>("hpCount");
            }
            if (mActorHPBar.TryGetValue(health, out bar))
            {
                bar.Init(health.Actor.gameObject, health.HPProgress, num, true);
                return;
            }
            bar = GetHPBar(health.ActorType);
            if (bar == null)
            {
                return;
            }
            
            bar.Init(health.Actor.gameObject, health.HPProgress, num);
            mActorHPBar.Add(health, bar);
        }

        /// <summary>
        /// 血条销毁处理。
        /// </summary>
        public void OnGameEventDestory(GameEvent ge, EventParameter parameter)
        {
            ActorHealth health = parameter.objParameter as ActorHealth;
            HPBar bar;
            if (mActorHPBar.TryGetValue(health, out bar))
            {
                mActorHPBar.Remove(health);
                RemoveHPBar(bar);
            }
        }

        public void OnGameEventUpdate(GameEvent ge, EventParameter parameter)
        {
            ActorHealth health = parameter.objParameter as ActorHealth;
            HPBar bar;
            if (mActorHPBar.TryGetValue(health, out bar))
            {
                bar.SetValue(health.HPProgress);
            }
        }

        public void OnGameEventPetUpdate(GameEvent ge, EventParameter parameter)
        {
            ActorHealth health = parameter.objParameter as ActorHealth;
            HPBar bar;
            if (mActorHPBar.TryGetValue(health, out bar))
            {
                bar.SetPetLevel(health.Actor);
            }
        }


        public void OnGameEventChangeNode(GameEvent ge, EventParameter parameter)
        {
            ActorHealth health = parameter.objParameter as ActorHealth;
            HPBar bar;
            if (mActorHPBar.TryGetValue(health, out bar))
            {
                bar.SetBindNode(parameter.goParameter, parameter.stringParameter);
            }
        }

        /// <summary>
        /// 血条PK状态改变。
        /// </summary>
        public void OnGameEventPKStatus(GameEvent ge, EventParameter parameter)
        {
            ActorHealth health = parameter.objParameter as ActorHealth;
            if (health.ActorType != ActorType.AT_LOCAL_PLAYER && health.ActorType != ActorType.AT_REMOTE_PLAYER)
            {
                return;
            }

            HPBar bar;
            if (mActorHPBar.TryGetValue(health, out bar))
            {
                if (health.ActorType == ActorType.AT_LOCAL_PLAYER)
                {
                    bar.InitPKStatus(PlayerData.Instance.CurPKState);
                }
                else
                {
                    bar.InitPKStatus(health.Actor as OtherPlayer);
                }                
            }
        }

        public void OnChangeHeroTitle(GameEvent ge, EventParameter parameter)
        {
            ActorHealth health = parameter.objParameter as ActorHealth;
            HPBar bar;
            if (mActorHPBar.TryGetValue(health, out bar))
            {
                bar.ChangeHeroTitle(parameter.stringParameter);
            }
        }

        public void OnChangeFaction(GameEvent ge, EventParameter parameter)
        {
            ActorHealth health = parameter.objParameter as ActorHealth;
            HPBar bar;
            if (mActorHPBar.TryGetValue(health, out bar))
            {
                bar.ChangeFaction(parameter.intParameter);
            }
        }

        /// <summary>
        ///通过roleId获取指定血条
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public HPBar GetCurObjBar(long roleId)
        {
            if(roleId <= 0)
            {
                return null;
            }

            ActorObj _actor =  CoreEntry.gActorMgr.GetActorByServerID(roleId);
            if(mActorHPBar.ContainsKey(_actor.Health))
            {
                return mActorHPBar[_actor.Health];
            }
            return null;
        }

        public static string CollectionInfoPrefab = "UI/Prefabs/HPBar/FirstRes/ItemCollectionInfo";                      //采集物信息条
        public static string MagickeyInfoPrefab = "UI/Prefabs/HPBar/ItemMagickeyInfo";                      //采集物信息条

        private Stack<CollectionInfo> m_CacheCollectionInfo = new Stack<CollectionInfo>();

        private Dictionary<long, CollectionInfo> m_CollectionInfo = new Dictionary<long, CollectionInfo>();

         
        /// <summary>
        /// 获取采集物信息条。
        /// </summary>
        /// <returns>采集物信息条。</returns>
        public CollectionInfo GetCollectionInfo()
        {
            if (m_CacheCollectionInfo.Count > 0)
            {
                CollectionInfo info = m_CacheCollectionInfo.Pop();
                info.gameObject.SetActive(true);
                return info;
            }

            GameObject prefab = null;
            prefab = (GameObject)CoreEntry.gResLoader.Load(CollectionInfoPrefab, typeof(GameObject));
            if(prefab == null)return null;

            GameObject obj = GameObject.Instantiate(prefab) as GameObject;
            RectTransform rt = obj.GetComponent<RectTransform>();
            CollectionInfo ret = obj.GetComponent<CollectionInfo>();
            obj.SetActive(true);
            rt.SetParent(HPBarRoot);
            rt.anchoredPosition3D = Vector3.zero;
            rt.localScale = Vector3.one;
            return ret;
        }

        public void RemoveCollectionInfo(CollectionInfo info)
        {
            if (info == null || info.gameObject == null)
            {
                return;
            }
            info.gameObject.SetActive(false);
            m_CacheCollectionInfo.Push(info);
        }

        /// <summary>
        /// 采集物信息创建。
        /// </summary>
        public void OnGameEventCollectInfoCreate(GameEvent ge, EventParameter parameter)
        {
            CollectionObj obj = parameter.objParameter as CollectionObj;
            if (obj == null || obj.ServerID == 0 || string.IsNullOrEmpty(obj.Name))
            {
                return;
            }

            //创建采集物信息
            CollectionInfo info;
            if (!m_CollectionInfo.TryGetValue(obj.ServerID, out info))
            {
                info = GetCollectionInfo();
                m_CollectionInfo.Add(obj.ServerID, info);
            }
            info.Init(obj.gameObject);
        }

        /// <summary>
        /// 采集物信息销毁。
        /// </summary>
        public void OnGameEventCollectInfoDestory(GameEvent ge, EventParameter parameter)
        {
            CollectionObj obj = parameter.objParameter as CollectionObj;
            if (obj == null || obj.ServerID == 0)
            {
                return;
            }

            //销毁采集物信息
            CollectionInfo info;
            if (m_CollectionInfo.TryGetValue(obj.ServerID, out info))
            {
                m_CollectionInfo.Remove(obj.ServerID);
                RemoveCollectionInfo(info);
            }
        }

        #region 魔宠

        private Stack<CollectionInfo> m_CacheMagicKeyInfo = new Stack<CollectionInfo>();

        private Dictionary<long, CollectionInfo> m_MagicKeyInfo = new Dictionary<long, CollectionInfo>();

        public CollectionInfo GetMagicKeyInfo()
        {
            if (m_CacheMagicKeyInfo.Count > 0)
            {
                CollectionInfo info = m_CacheMagicKeyInfo.Pop();
                info.gameObject.SetActive(true);
                return info;
            }

            GameObject prefab = null;
            prefab = (GameObject)CoreEntry.gResLoader.Load(MagickeyInfoPrefab, typeof(GameObject));
            if(prefab == null)return null;

            GameObject obj = GameObject.Instantiate(prefab) as GameObject;
            RectTransform rt = obj.GetComponent<RectTransform>();
            CollectionInfo ret = obj.GetComponent<CollectionInfo>();
            obj.SetActive(true);
            rt.SetParent(HPBarRoot);
            rt.anchoredPosition3D = Vector3.zero;
            rt.localScale = Vector3.one;
            return ret;
        }

        public void RemoveMagicKeyInfo(CollectionInfo info)
        {
            if (info == null || info.gameObject == null)
            {
                return;
            }
            info.gameObject.SetActive(false);
            m_CacheMagicKeyInfo.Push(info);
        }

        public void OnGameEventMagickeyInfoCreate(GameEvent ge,EventParameter parameter)
        {
            ActorObj obj = parameter.objParameter as ActorObj;
            if (obj == null || obj.ServerID == 0)
            {
                return;
            }
             
            CollectionInfo info;
            if (!m_MagicKeyInfo.TryGetValue(obj.ServerID, out info))
            {
                info = GetMagicKeyInfo();
                m_MagicKeyInfo.Add(obj.ServerID, info);
            }
            info.Init(obj.MagicKeyModel,MagicKeyDataMgr.Instance.GetMagickeyName(obj.mMagicKeyId,obj.mMagicKeyStar));
        }
        public void OnGameEventMagickeyInfoDestroy(GameEvent ge, EventParameter parameter)
        {
            ActorObj obj = parameter.objParameter as ActorObj;
            if (obj == null || obj.ServerID == 0)
            {
                return;
            }
             
            CollectionInfo info;
            if (m_MagicKeyInfo.TryGetValue(obj.ServerID, out info))
            {
                m_MagicKeyInfo.Remove(obj.ServerID);
                RemoveMagicKeyInfo(info);
            }
        }

        public void OnBeginLoadScen(GameEvent ge, EventParameter parameter)
        {
            Dictionary<long, CollectionInfo>.Enumerator iter = m_MagicKeyInfo.GetEnumerator();
            while(iter.MoveNext())
            {
                CollectionInfo v = iter.Current.Value;
                GameObject.Destroy(v.gameObject);
            }
            m_MagicKeyInfo.Clear();
        }

        public void onClearUseData(GameEvent ge,EventParameter parameter)
        {
            OnBeginLoadScen(ge, parameter);
        }
        #endregion
    }
}

