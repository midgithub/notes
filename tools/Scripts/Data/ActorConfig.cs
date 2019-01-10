using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{
    /// <summary>
    /// 角色配置。
    /// </summary>
[Hotfix]
    public class ActorConfig
    {
        /// <summary>
        /// 缓存怪物配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheMonsterConfig = null;

        /// <summary>
        /// 获取怪物配置信息。
        /// </summary>
        public Dictionary<int, LuaTable> MonsterConfig
        {
            get { return m_CacheMonsterConfig; }
        }

        /// <summary>
        /// 缓存NPC配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheNPCConfig = null;

        /// <summary>
        /// 获取NPC配置信息。
        /// </summary>
        public Dictionary<int, LuaTable> NPCConfig
        {
            get { return m_CacheNPCConfig; }
        }

        /// <summary>
        /// 缓存变身配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheChangeConfig = null;

        /// <summary>
        /// 缓存玩家信息配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CachePlayerInfoConfig = null;

        /// <summary>
        /// 缓存升级配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheLevelUpConfig = null;

        /// <summary>
        /// 缓存巅峰(境界)升级配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheDianFengConfig = null;

        /// <summary>
        /// 缓存世界Boss配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheWorlBossConfig = null;

        /// <summary>
        /// 构造函数。
        /// </summary>
        public ActorConfig()
        {
            InitConfig();
        }

        /// <summary>
        /// 初始化配置。
        /// </summary>
        public void InitConfig()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            m_CacheMonsterConfig = G.Get<Dictionary<int, LuaTable>>("t_monster");
            m_CacheNPCConfig = G.Get<Dictionary<int, LuaTable>>("t_npc");
            m_CacheChangeConfig = G.Get<Dictionary<int, LuaTable>>("t_change");
            m_CachePlayerInfoConfig = G.Get<Dictionary<int, LuaTable>>("t_playerinfo");
            m_CacheLevelUpConfig = G.Get<Dictionary<int, LuaTable>>("t_lvup");
            m_CacheDianFengConfig = G.Get<Dictionary<int, LuaTable>>("t_dianfenglvup");
            m_CacheWorlBossConfig = G.Get<Dictionary<int, LuaTable>>("t_worldboss");
        }
        
        /// <summary>
        /// 获取怪物配置。
        /// </summary>
        /// <param name="id">怪物ID。</param>
        /// <returns>怪物配置。</returns>
        public LuaTable GetMonsterConfig(int id)
        {
            LuaTable t;
            m_CacheMonsterConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取NPC配置。
        /// </summary>
        /// <param name="id">怪物ID。</param>
        /// <returns>怪物配置。</returns>
        public LuaTable GetNPCConfig(int id)
        {
            LuaTable t;
            m_CacheNPCConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取变身配置。
        /// </summary>
        /// <param name="id">变身ID。</param>
        /// <returns>变身配置。</returns>
        public LuaTable GetChangeConfig(int id)
        {
            LuaTable t;
            m_CacheChangeConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取玩家信息配置。
        /// </summary>
        /// <param name="id">玩家信息ID。</param>
        /// <returns>玩家信息配置。</returns>
        public LuaTable GetPlayerInfoConfig(int id)
        {
            LuaTable t;
            m_CachePlayerInfoConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取升级配置。
        /// </summary>
        /// <param name="id">升级ID。</param>
        /// <returns>升级配置。</returns>
        public LuaTable GetLevelUpConfig(int id)
        {
            LuaTable t;
            m_CacheLevelUpConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取巅峰配置。
        /// </summary>
        /// <param name="id">巅峰ID。</param>
        /// <returns>巅峰配置。</returns>
        public LuaTable GetDianFengConfig(int id)
        {
            LuaTable t;
            m_CacheDianFengConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取世界BOSS名称。
        /// </summary>
        /// <param name="id">BOSS的ID</param>
        /// <returns>BOSS名称。</returns>
        public string GetWorldBossName(int id)
        {
            LuaTable t;
            if (m_CacheWorlBossConfig.TryGetValue(id, out t))
            {
                int mid = t.Get<int>("monster");
                LuaTable mcfg = GetMonsterConfig(mid);
                if (mcfg != null)
                {
                    return mcfg.Get<string>("name");
                }
            }
            return string.Empty;
        }
    }
}

