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
    public class BeautyWomanConfig
    {
        /// <summary>
        /// 缓存红颜配置。
        /// </summary>
        public Dictionary<int, LuaTable> m_CacheBeautyWomanConfig = null;

        /// <summary>
        /// 缓存红颜升阶配置。
        /// </summary>
        public Dictionary<int, LuaTable> m_CacheBeautyWomanShengJieConfig = null;

        /// <summary>
        /// 构造函数。
        /// </summary>
        public BeautyWomanConfig()
        {
            InitConfig();
        }

        /// <summary>
        /// 初始化配置。
        /// </summary>
        public void InitConfig()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            m_CacheBeautyWomanConfig = G.Get<Dictionary<int, LuaTable>>("t_hongyan");
            m_CacheBeautyWomanShengJieConfig = G.Get<Dictionary<int, LuaTable>>("t_hongyanshengjie");
        }

        /// <summary>
        /// 获取红颜配置。
        /// </summary>
        /// <param name="id">红颜编号。</param>
        /// <returns>红颜配置。</returns>
        public LuaTable GetBeautyWomanConfig(int id)
        {
            LuaTable t;
            m_CacheBeautyWomanConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取红颜升阶配置。
        /// </summary>
        /// <param name="id">红颜升阶编号。</param>
        /// <returns>红颜升阶配置。</returns>
        public LuaTable GetBeautyWomanShengJieConfig(int id)
        {
            LuaTable t;
            m_CacheBeautyWomanShengJieConfig.TryGetValue(id, out t);
            return t;
        }
    }
}

