/**
* @file     : FubenPreloader
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-09-29
*/
using UnityEngine;
using System.Collections.Generic;
using XLua;

namespace SG
{
[Hotfix]
    public class FubenPreloader : BasePreloader
    {
        protected string mConfigName;
        protected int mConfigID = 0;
        protected List<string> mKeyName = new List<string>();
        protected int mAddType = 0;

        private const int MAX_POOL_NUM = 20;

        public override void Init(ESceneMonsterType type)
        {
            mAddType = 0;
            mKeyName.Clear();
            switch (type)
            {
                case ESceneMonsterType.MONSTER_TYPE_HUANLINGXIANYU:
                    mConfigName = "t_hunlingxianyu";
                    mConfigID = -1;
                    mKeyName.Add("monster");
                    break;
                case ESceneMonsterType.MONSTER_TYPE_DUNGEON_QUEST:
                    mConfigName = "t_dungeon_quest";
                    mConfigID = DungeonMgr.Instance.curFbId;
                    mKeyName.Add("monster");
                    break;
                case ESceneMonsterType.MONSTER_TYPE_RESOURCES_FB:
                    mConfigName = "t_zhiyuanfb";
                    mConfigID = PlayerData.Instance.ResLevelData.CurID;
                    mKeyName.Add("monster");
                    mKeyName.Add("boss");
                    break;
                case ESceneMonsterType.MONSTER_TYPE_LINGSHOUMUDI:
                    mConfigName = "t_lingshoumudi";
                    mConfigID = -1;
                    mKeyName.Add("monster");
                    break;
                case ESceneMonsterType.MONSTER_TYPE_LEVEL_FB:
                    mConfigName = "t_zhuzairoad";
                    mConfigID = PlayerData.Instance.LevelFuBenDataMgr.EnterId;
                    mKeyName.Add("monster");
                    mKeyName.Add("boss");
                    break;
                case ESceneMonsterType.MONSTER_TYPE_EXP_FB:
                    mConfigName = "t_liushuifuben";
                    mConfigID = PlayerData.Instance.BaseAttr.Level;
                    for (int i = 1; i < 6; i++)
                    {
                        mKeyName.Add(string.Format("water_monster{0}", i));
                        mKeyName.Add(string.Format("water_boss{0}", i));
                    }
                    break;
                case ESceneMonsterType.MONSTER_TYPE__PERSONAL_BOSS:
                    mAddType = 1;
                    mConfigName = "t_personalboss";
                    mConfigID = BossFuBenDataMgr.Instance.EnterId;
                    mKeyName.Add("boosId");
                    break;
                case ESceneMonsterType.MONSTER_TYPE_WORLD_BOSS:
                    mAddType = 1;
                    mConfigName = "t_worldboss";
                    mConfigID = ActivityMgr.Instance.CurActiveId;
                    mKeyName.Add("monster");
                    break;
                case ESceneMonsterType.MONSTER_TYPE_SECRECT_FB:
                    mAddType = 1;
                    mConfigName = "t_secretAreaDupl";
                    mConfigID = SecretDuplDataMgr.Instance.EnterID;
                    mKeyName.Add("normolMonsterIDList");
                    mKeyName.Add("eliteMonsterIDList");
                    mKeyName.Add("bossIDList");
                    break;
                case ESceneMonsterType.MONSTER_TYPE_SECRECT_TEAM_FB:
                    mAddType = 1;
                    mConfigName = "t_secretAreaTeamDupl";
                    mConfigID = SecretDuplDataMgr.Instance.EnterTeamID;
                    mKeyName.Add("normolMonsterIDList");
                    mKeyName.Add("eliteMonsterIDList");
                    mKeyName.Add("bossIDList");
                    break;
                case ESceneMonsterType.MONSTER_TYPE_TREASURE_FB:
                    mAddType = 1;
                    mConfigName = "t_treasureDulp";
                    mConfigID = SecretDuplDataMgr.Instance.EnterID;
                    mKeyName.Add("bossId");
                    break;
                case ESceneMonsterType.MONSTER_TYPE_GOLD_BOSS:
                    mAddType = 1;
                    mConfigName = "t_fieldboss";
                    mConfigID = ActivityMgr.Instance.CurActiveId;
                    mKeyName.Add("bossId");
                    break;
                case ESceneMonsterType.MONSTER_TYPE_CROWN_BOSS:
                    mAddType = 1;
                    mConfigName = "t_crownboss";
                    mConfigID = PlayerData.Instance.CrownData.CurCrownMapID;
                    mKeyName.Add("bossId");
                    break;
                default:
                    mConfigName = "";
                    break;
            }
        }

        public override void PreloadData()
        {
            base.PreloadData();

            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            Dictionary<int, LuaTable> cfg = G.Get<Dictionary<int, LuaTable>>(mConfigName);
            if (null == cfg)
            {
                return;
            }

            if (-1 == mConfigID)
            {
                if (1 == mKeyName.Count)
                {
                    foreach (KeyValuePair<int, LuaTable> data in cfg)
                    {
                        string monsters = data.Value.Get<string>(mKeyName[0]);
                        if (null == monsters)
                        {
                            continue;
                        }

                        if (0 == mAddType)
                        {
                            AddMonsters(monsters, MAX_POOL_NUM);
                        }
                        else
                        {
                            AddMonstersExtra(monsters, MAX_POOL_NUM);
                        }
                    }
                }
            }
            else
            {
                if (cfg.ContainsKey(mConfigID))
                {
                    for (int i = 0; i < mKeyName.Count; i++)
                    {
                        string monsters = cfg[mConfigID].Get<string>(mKeyName[i]);
                        if (null != monsters)
                        {
                            if (0 == mAddType)
                            {
                                AddMonsters(monsters, MAX_POOL_NUM);
                            }
                            else
                            {
                                AddMonstersExtra(monsters, MAX_POOL_NUM);
                            }
                        }
                    }
                }
            }
        }
    }
}

