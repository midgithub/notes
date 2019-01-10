using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{
    /// <summary>
    /// 公会配置。
    /// </summary>
[Hotfix]
    public class GuildConfig
    {
        /// <summary>
        /// 缓存祈福配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CachePrayConfig = null;

        /// <summary>
        /// 缓存职位配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheTitleConfig = null;

        /// <summary>
        /// 构造函数。
        /// </summary>
        public GuildConfig()
        {
            InitConfig();
        }

        /// <summary>
        /// 初始化配置。
        /// </summary>
        public void InitConfig()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            m_CachePrayConfig = G.Get<Dictionary<int, LuaTable>>("t_guildpray");
            m_CacheTitleConfig = G.Get<Dictionary<int, LuaTable>>("t_guildtitle");
        }

        /// <summary>
        /// 获取祈福配置。
        /// </summary>
        /// <param name="id">祈福编号。</param>
        /// <returns>祈福配置。</returns>
        public LuaTable GetPrayConfig(int id)
        {
            LuaTable t;
            m_CachePrayConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取公会职位名称。
        /// </summary>
        /// <param name="pos">公会职位。</param>
        /// <returns>职位名称。</returns>
        public string GetTitleName(int pos)
        {
            LuaTable t;
            if (!m_CacheTitleConfig.TryGetValue(pos, out t))
            {
                return string.Empty;
            }
            return t.Get<string>("posname");
        }
    }
}

