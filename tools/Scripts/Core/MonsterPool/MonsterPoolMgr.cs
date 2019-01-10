/**
* @file     : MonsterPoolMgr
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-09-28
*/
using UnityEngine;
using System.Collections.Generic;
using XLua;

namespace SG
{
[Hotfix]
    public class MonsterPoolMgr 
    {
        private Dictionary<ESceneMonsterType, BasePreloader> mPreloaders;
        private FubenPreloader mFubenPreloader;

        public MonsterPoolMgr()
        {
            mPreloaders = new Dictionary<ESceneMonsterType, BasePreloader>();
            mFubenPreloader = new FubenPreloader();

            mPreloaders.Add(ESceneMonsterType.MONSTER_TYPE_HUANLINGXIANYU, mFubenPreloader);
            mPreloaders.Add(ESceneMonsterType.MONSTER_TYPE_DUNGEON_QUEST, mFubenPreloader);
            mPreloaders.Add(ESceneMonsterType.MONSTER_TYPE_RESOURCES_FB, mFubenPreloader);
            mPreloaders.Add(ESceneMonsterType.MONSTER_TYPE_LINGSHOUMUDI, mFubenPreloader);
            mPreloaders.Add(ESceneMonsterType.MONSTER_TYPE_LEVEL_FB, mFubenPreloader);
            mPreloaders.Add(ESceneMonsterType.MONSTER_TYPE_EXP_FB, mFubenPreloader);
            mPreloaders.Add(ESceneMonsterType.MONSTER_TYPE__PERSONAL_BOSS, mFubenPreloader);
            mPreloaders.Add(ESceneMonsterType.MONSTER_TYPE_SECRECT_FB, mFubenPreloader);
            mPreloaders.Add(ESceneMonsterType.MONSTER_TYPE_GOLD_BOSS, mFubenPreloader);
            mPreloaders.Add(ESceneMonsterType.MONSTER_TYPE_TREASURE_FB, mFubenPreloader);
            mPreloaders.Add(ESceneMonsterType.MONSTER_TYPE_WORLD_BOSS, mFubenPreloader);
            mPreloaders.Add(ESceneMonsterType.MONSTER_TYPE_SECRECT_TEAM_FB, mFubenPreloader);
            mPreloaders.Add(ESceneMonsterType.MONSTER_TYPE_CROWN_BOSS, mFubenPreloader);
        }

        /// <summary>
        /// ��ʼ������������
        /// </summary>
        public void InitMonterPool()
        {
            LuaTable mapCfg = ConfigManager.Instance.Map.GetMapConfig(MapMgr.Instance.EnterMapId);

            int maptype = mapCfg.Get<int>("type");
            if(maptype == 1 || maptype ==2)
            {
                ESceneMonsterType monsterType = mapCfg != null ? (ESceneMonsterType)mapCfg.Get<int>("monsterType") : ESceneMonsterType.MONSTER_TYPE_NONE;

                if (mPreloaders.ContainsKey(monsterType))
                {
                    BasePreloader loader = mPreloaders[monsterType];
                    loader.Init(monsterType);
                    loader.Preload();
                }
            }
        }
    }
}

