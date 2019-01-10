using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{
    /// <summary>
    /// 任务配置。
    /// </summary>
[Hotfix]
    public class TaskConfig
    {
        /// <summary>
        /// 缓存主险配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheMainConfig = null;

        /// <summary>
        /// 缓存日常配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheDailyConfig = null;

        /// <summary>
        /// 缓存日环配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheAgainstConfig = null;

        /// <summary>
        /// 缓存公会配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheGuildConfig = null;

        /// <summary>
        /// 缓存分支配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheBranchConfig = null;

        /// <summary>
        /// 缓存通缉配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheTongjiConfig = null;

        /// <summary>
        /// 构造函数。
        /// </summary>
        public TaskConfig()
        {
            InitConfig();
        }

        /// <summary>
        /// 初始化配置。
        /// </summary>
        public void InitConfig()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            m_CacheMainConfig = G.Get<Dictionary<int, LuaTable>>("t_quest");
            m_CacheDailyConfig = G.Get<Dictionary<int, LuaTable>>("t_dailyquest");
            m_CacheAgainstConfig = G.Get<Dictionary<int, LuaTable>>("t_againstquest");
            m_CacheGuildConfig = G.Get<Dictionary<int, LuaTable>>("t_guildquest");
            m_CacheBranchConfig = G.Get<Dictionary<int, LuaTable>>("t_questlevel");
            m_CacheTongjiConfig = G.Get<Dictionary<int, LuaTable>>("t_fengyao");
        }

        /// <summary>
        /// 获取主线任务配置。
        /// </summary>
        /// <param name="id">主线ID。</param>
        /// <returns>主线配置。</returns>
        public LuaTable GetMainConfig(int id)
        {
            LuaTable t;
            m_CacheMainConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取日常任务配置。
        /// </summary>
        /// <param name="id">日常ID。</param>
        /// <returns>日常配置。</returns>
        public LuaTable GetDailyConfig(int id)
        {
            LuaTable t;
            m_CacheDailyConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取日环任务配置。
        /// </summary>
        /// <param name="id">日环ID。</param>
        /// <returns>日环配置。</returns>
        public LuaTable GetAgainstConfig(int id)
        {
            LuaTable t;
            m_CacheAgainstConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取公会任务配置。
        /// </summary>
        /// <param name="id">公会ID。</param>
        /// <returns>公会配置。</returns>
        public LuaTable GetGuildConfig(int id)
        {
            LuaTable t;
            m_CacheGuildConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取支线任务配置。
        /// </summary>
        /// <param name="id">任务ID。</param>
        /// <returns>支线任务配置。</returns>
        public LuaTable GetBranchConfig(int id)
        {
            LuaTable t;
            m_CacheBranchConfig.TryGetValue(id, out t);
            return t;
        }
                
        /// <summary>
        /// 获取通缉任务配置。
        /// </summary>
        /// <param name="id">任务ID。</param>
        /// <returns>任务配置。</returns>
        public LuaTable GetTongjiConfig(int id)
        {
            LuaTable t;
            m_CacheTongjiConfig.TryGetValue(id, out t);
            return t;
        }
    }
}

