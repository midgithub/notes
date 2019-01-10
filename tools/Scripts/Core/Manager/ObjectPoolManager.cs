/**
* @file     : a
* @brief    : b
* @details  : d
* @author   : 
* @date     : 2014-xx-xx
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using XLua;

namespace SG
{

[Hotfix]
    public class ObjectPoolManager : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {

        }

        public void initObjectPool()
        {
            m_ObjectPool.Clear();
            m_MonsterPool.Clear();

            initSceneObjPool();
            initPlayerPool();
            initSkillPool();
            initEffectPool();
            initMonsterPool();
        }

        public void ReleaseObjectPool()
        {
            m_ObjectPool.Clear();
            m_MonsterPool.Clear();
        }

        public GameObject ObtainObject(int iResid)
        {
            if (m_ObjectPool.ContainsKey(iResid))
            {
                GameObjectPool objpool = (GameObjectPool)m_ObjectPool[iResid];

                return objpool.ObtainPrefabInstance();
            }

            return null;
        }

        public void RecycleObject(int iResid, GameObject obj)
        {
            if (m_ObjectPool.ContainsKey(iResid))
            {
                GameObjectPool objpool = (GameObjectPool)m_ObjectPool[iResid];
                obj.transform.position = Vector3.zero;

                objpool.RecyclePrefabInstance(obj);
            }
            else//暂时处理，后面重构代码
            {
                Object.Destroy(obj);
            }
        }

        public void PushToPool(int resID, string ext, int maxNum = 20, bool isDelOldOne = false)
        {
            if (m_ObjectPool.ContainsKey(resID))
            {
                if (isDelOldOne)
                {
                    m_ObjectPool.Remove(resID);
                }
                else
                {
                    return;
                }
            }

            GameObject obj = null;
            GameObject root = ObjectPoolRoot;

            //Configs.modelConfig modelConfig = CSVConfigManager.GetmodelConfig(resID);
            LuaTable cfg = ConfigManager.Instance.Common.GetModelConfig(resID);
            if (cfg == null)
            {
                return;
            }
            else
            {
                string path = cfg.Get<string>("skl");
                if (ext != null && ext.Length > 0)
                {
                    path += ext;
                }
				//Debug.LogError ("====PushToPool==path="+path);
                obj = (GameObject)(CoreEntry.gResLoader.LoadResource(path));
            }
            if (obj)
            {
                GameObjectPool objpool = new GameObjectPool(obj, root, 1, 1, maxNum);
                if (!m_ObjectPool.ContainsKey(resID))
                {
                    m_ObjectPool.Add(resID, objpool);
                }
            }
            else
            {
                LogMgr.UnityLog("");
            }
        }

        public void PushToPool(int resID, int maxNum = 20)
        {
            PushToPool(resID, null, maxNum);
        }

        public void PushPortalToPool(int resID)
        {
            GameObject obj = new GameObject("Portal");
            obj.transform.parent = ObjectPoolRoot.transform;
            GameObjectPool objpool = new GameObjectPool(obj, ObjectPoolRoot, 1, 1, 5);
            if (!m_ObjectPool.ContainsKey(resID))
            {
                m_ObjectPool.Add(resID, objpool);
            }
        }

        private void initSceneObjPool()
        {

            List<SceneEntitySet> entities = CoreEntry.gGameDBMgr.GetEnityConfigInfo(MapMgr.Instance.EnterMapId);

			if (null == entities)
            {
                return;
            }
            bool bPreloading = false;
            LuaTable mapConfig = ConfigManager.Instance.Map.GetMapConfig(MapMgr.Instance.EnterMapId);
            if (mapConfig != null)
            {
                int Preloading = mapConfig.Get<int>("Preloading");
                if(Preloading == 1)
                {
                    bPreloading = true;
                }
                //Debug.LogError("================bPreloading :" + bPreloading.ToString() + "  EnterMapId: " + MapMgr.Instance.EnterMapId);
            }
            Vector3 mainplayerPos = new Vector3 (MapMgr.Instance.PlayerInitPos.x,0.0f,MapMgr.Instance.PlayerInitPos.y);

            for (int i = 0; i < entities.Count; i++)
            {
                bool isPortal = entities[i].type == EntityConfigType.ECT_PORTAL;

                for (int j = 0; j < entities[i].entityList.Count; j++)
                {
                    if (isPortal)
                    {
                        LuaTable portalCfg = ConfigManager.Instance.Map.GetPortalConfig(entities[i].entityList[j].configID);
                        if (null != portalCfg)
                        {
                            PushPortalToPool(portalCfg.Get<int>("modelId"));

                            GameObject effectObj = CoreEntry.gGameObjPoolMgr.Instantiate(portalCfg.Get<string>("pfx"));
                            if (null != effectObj)
                            {
                                CoreEntry.gGameObjPoolMgr.Destroy(effectObj);
                            }
                        }
                    }
                    else
                    {

                        int resID = 0;
                        if (entities[i].type == EntityConfigType.ECT_MONSTER)
                        {
                            int cid = entities[i].entityList[j].configID;
                            LuaTable cfg = ConfigManager.Instance.Actor.GetMonsterConfig(cid);
                            //Configs.monsterConfig monsterCfg = CSVConfigManager.GetmonsterConfig();
                            if (null != cfg)
                            {
                                resID = cfg.Get<int>("modelId");
                            }

                            initMonsterSkillPool(entities[i].entityList[j].configID);
                        }
                        else if (entities[i].type == EntityConfigType.ECT_NPC)
                        {
                            LuaTable npcCfg = ConfigManager.Instance.Actor.GetNPCConfig(entities[i].entityList[j].configID);
                            if (null != npcCfg)
                            {
                                resID = npcCfg.Get<int>("look");
                            }
                        }
                        else if (entities[i].type == EntityConfigType.ECT_COLLECTION)
                        {
                            LuaTable collectionCfg = ConfigManager.Instance.Map.GetCollectionConfig(entities[i].entityList[j].configID);
                            if (null != collectionCfg)
                            {
                                resID = collectionCfg.Get<int>("modelId");
                            }
                        }
                        else
                        {
                            continue;
                        }
                        Vector3 pos = entities[i].entityList[j].position;
                        float fDistance = Vector3.Distance(pos, mainplayerPos);

                        //Debug.LogError ("================pos=========fDistance========"+fDistance);

                        if (!bPreloading && fDistance > 50.0f)
                        {
                            continue;
                        }
                        PushToPool(resID);
                    }
                }
            }
        }

        private void initPlayerPool()
        {
            int dressModelID = PlayerData.Instance.GetDressModelID();

            if (0 != dressModelID)
            {
                PushToPool(dressModelID);
            }

            PushToPool(10001);
            PushToPool(700001001);
        }

        private void initSkillPool()
        {
            Dictionary<int, int> skills = PlayerData.Instance.SkillData.Skills;
            foreach (KeyValuePair<int, int> kv in skills)
            {
                PreLoadSkill(kv.Value);
            }
        }

        private void initEffectPool()
        {
            GameObject effectObj = CoreEntry.gGameObjPoolMgr.Instantiate("Effect/skill/buff/fx_buff_shengji");
            if (null != effectObj)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(effectObj);
            }

            effectObj = CoreEntry.gGameObjPoolMgr.Instantiate("Effect/skill/buff/fx_buff_fuhuo");
            if (null != effectObj)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(effectObj);
            }

            effectObj = CoreEntry.gGameObjPoolMgr.Instantiate("Effect/zuoqi/Mount_vfx");
            if (null != effectObj)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(effectObj);
            }

            effectObj = CoreEntry.gGameObjPoolMgr.Instantiate("Effect/scence/sf_effect/sf_Spawn_vfx");
            if (null != effectObj)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(effectObj);
            }

            for (int index = 1; index < 6; index++)
            {
                effectObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(@"Effect/skill/hurt/fodder_blood0" + index);

                if (null != effectObj)
                {
                    CoreEntry.gGameObjPoolMgr.Destroy(effectObj);
                }
            }

            CoreEntry.gMorphMgr.PreLoad();
        }

        public void initMonsterSkillPool(int monsterID)
        {
            LuaTable cfg = ConfigManager.Instance.Actor.GetMonsterConfig(monsterID);
            //Configs.monsterConfig monsterCfg = CSVConfigManager.GetmonsterConfig(monsterID);
            if (null == cfg)
            {
                return;
            }

            List<int> skillIDs = new List<int>();
            skillIDs.Add(cfg.Get<int>("default_skill_id"));

            string[] ids;
            string skill_id = cfg.Get<string>("skill_id");
            if (!string.IsNullOrEmpty(skill_id) && skill_id.Length > 0)
            {
                ids = skill_id.Split(',');
                if (null != ids)
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        int id = 0;
                        int.TryParse(ids[i], out id);
                        if (!skillIDs.Contains(id))
                        {
                            skillIDs.Add(id);
                        }
                    }
                }
            }
            string condition_skills = cfg.Get<string>("condition_skills");
            if (!string.IsNullOrEmpty(condition_skills) && condition_skills.Length > 0)
            {
                ids = condition_skills.Split(',');
                if (null != ids)
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        int id = 0;
                        int.TryParse(ids[i], out id);

                        LuaTable monsterSkillCfg = ConfigManager.Instance.Skill.GetMonsterSkillConfig(id);
                        if (null == monsterSkillCfg)
                        {
                            continue;
                        }

                        string[] ids2 = monsterSkillCfg.Get<string>("skill_id").Split(',');
                        if (null == ids2)
                        {
                            continue;
                        }

                        for (int j = 0; j < ids2.Length; j++)
                        {
                            id = 0;
                            if (int.TryParse(ids2[j], out id))
                            {
                                if (!skillIDs.Contains(id))
                                {
                                    skillIDs.Add(id);
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < skillIDs.Count; i++)
            {
                PreLoadSkill(skillIDs[i]);
            }
        }

        private void PreLoadSkill(int skillID)
        {
            StateParameter sp = new StateParameter();
            sp.state = ACTOR_STATE.AS_DEATH;
            GameObject obj = CoreEntry.gGameObjPoolMgr.InstantiateSkillBase("Skill/SkillBase");
            if (null != obj)
            {
                SkillBase skillBase = obj.GetComponent<SkillBase>();
                skillBase.Preload(null, skillID);
                sp.skillID = skillID;
                skillBase.BreakSkill(sp);
            }
        }

        private void initMonsterPool()
        {
            CoreEntry.gMonsterPoolMgr.InitMonterPool();
        }

        private Hashtable m_ObjectPool = new Hashtable();
        public Hashtable ObjectPool
        {
            get { return m_ObjectPool; }
        }

        private Hashtable m_MonsterPool = new Hashtable();
        public Hashtable MonsterPool
        {
            get { return m_MonsterPool; }
        }

        GameObject m_ObjectPoolRoot;
        public GameObject ObjectPoolRoot
        {
            get
            {
                if (m_ObjectPoolRoot == null)
                {
                    m_ObjectPoolRoot = new GameObject("ObjectPoolRoot");
                }
                return m_ObjectPoolRoot;
            }
        }
    }
}

