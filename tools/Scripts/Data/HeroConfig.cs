using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{
    /// <summary>
    /// 红颜配置。
    /// </summary>
[Hotfix]
    public class HeroConfig
    {
        /// <summary>
        /// 缓存红颜配置。
        /// </summary>
        public Dictionary<int, LuaTable> m_CacheHeroConfig = null;
        public Dictionary<int, LuaTable> m_CacheHeroShengjieConfig = null;



        /// <summary>
        /// 构造函数。
        /// </summary>
        public HeroConfig()
        {
            InitConfig();
        }

        /// <summary>
        /// 初始化配置。
        /// </summary>
        public void InitConfig()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            m_CacheHeroConfig = G.Get<Dictionary<int, LuaTable>>("t_hero");
            m_CacheHeroShengjieConfig = G.Get<Dictionary<int, LuaTable>>("t_heroshengjie");
        }

        /// <summary>
        /// 获取英雄配置。
        /// </summary>
        /// <param name="id">英雄编号。</param>
        /// <returns>英雄配置。</returns>
        public LuaTable GetHeroConfig(int id)
        {
            LuaTable t;
            m_CacheHeroConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取英雄升阶配置。
        /// </summary>
        /// <param name="id">英雄编号。</param>
        /// <returns>英雄配置。</returns>
        public LuaTable GetHeroShengjieConfig(int id)
        {
            LuaTable t;
            m_CacheHeroShengjieConfig.TryGetValue(id, out t);
            return t;
        }
         
    }
}

