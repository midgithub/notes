using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{
    /// <summary>
    /// 地图配置。
    /// </summary>
[Hotfix]
    public class MapConfig
    {
        /// <summary>
        /// 缓存地图配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheMapConfig = null;

        /// <summary>
        /// 名称到编号的映射。
        /// </summary>
        private Dictionary<string, int> m_NameToID = null;

        /// <summary>
        /// 获取地图配置信息。
        /// </summary>
        public Dictionary<int, LuaTable> Maps
        {
            get { return m_CacheMapConfig; }
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        public MapConfig()
        {
            InitConfig();
        }

        /// <summary>
        /// 初始化配置。
        /// </summary>
        public void InitConfig()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            m_CacheMapConfig = G.Get<Dictionary<int, LuaTable>>("t_map");
            m_CacheCollectionConfig = G.Get<Dictionary<int, LuaTable>>("t_collection");
            m_CachePortalConfig = G.Get<Dictionary<int, LuaTable>>("t_portal");
            m_CachePositionConfig = G.Get<Dictionary<int, LuaTable>>("t_position");
            m_CacheTrapConfig = G.Get<Dictionary<int, LuaTable>>("t_trap");

            m_NameToID = new Dictionary<string, int>();
            var e = m_CacheMapConfig.GetEnumerator();
            while (e.MoveNext())
            {
                var kvp = e.Current;
                string mapname = kvp.Value.Get<string>("name");
                if (!m_NameToID.ContainsKey(mapname))
                {
                    m_NameToID.Add(mapname, kvp.Key);
                }
                else
                {
                    //LogMgr.LogWarning("地图名称重复 id::{0} name:{1}", kvp.Key, mapname);
                }                
            }
        }

        /// <summary>
        /// 获取地图配置。
        /// </summary>
        /// <param name="id">地图ID。</param>
        /// <returns>配置数据。</returns>
        public LuaTable GetMapConfig(int id)
        {
            LuaTable t;
            m_CacheMapConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取地图名称。
        /// </summary>
        /// <param name="id">地图编号。</param>
        /// <returns>地图名称。</returns>
        public string GetMapName(int id)
        {
            LuaTable t;
            if (m_CacheMapConfig.TryGetValue(id, out t))
            {
                return t.Get<string>("name");
            }
            return string.Empty;
        }

        /// <summary>
        /// 通过地图名称获取地图编号。
        /// </summary>
        /// <param name="name">地图名称。</param>
        /// <returns>地图编号。</returns>
        public int GetMapID(string name)
        {
            int id;
            m_NameToID.TryGetValue(name, out id);
            return id;
        }

        #region --------------------采集物--------------------

        /// <summary>
        /// 采集物配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheCollectionConfig;

        /// <summary>
        /// 获取采集物配置。
        /// </summary>
        public Dictionary<int, LuaTable> CacheCollectionConfig
        {
            get { return m_CacheCollectionConfig; }
        }

        /// <summary>
        /// 获取采集物配置。
        /// </summary>
        /// <param name="id">采集物ID。</param>
        /// <returns>采集物配置。</returns>
        public LuaTable GetCollectionConfig(int id)
        {
            LuaTable t;
            m_CacheCollectionConfig.TryGetValue(id, out t);
            return t;
        }

        #endregion

        #region --------------------传送门--------------------

        /// <summary>
        /// 传送门配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CachePortalConfig;

        /// <summary>
        /// 获取传送门配置。
        /// </summary>
        public Dictionary<int, LuaTable> CachePortalConfig
        {
            get { return m_CachePortalConfig; }
        }

        /// <summary>
        /// 获取传送门配置。
        /// </summary>
        /// <param name="id">传送门ID。</param>
        /// <returns>传送门配置。</returns>
        public LuaTable GetPortalConfig(int id)
        {
            LuaTable t;
            m_CachePortalConfig.TryGetValue(id, out t);
            return t;
        }

        #endregion

        #region --------------------陷阱--------------------

        /// <summary>
        /// 陷阱配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheTrapConfig;

        /// <summary>
        /// 获取陷阱配置。
        /// </summary>
        /// <param name="id">陷阱ID。</param>
        /// <returns>陷阱配置。</returns>
        public LuaTable GetTrapConfig(int id)
        {
            LuaTable t;
            m_CacheTrapConfig.TryGetValue(id, out t);
            return t;
        }

        #endregion


        #region --------------------位置--------------------

        /// <summary>
        /// 位置配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CachePositionConfig;

        /// <summary>
        /// 获取位置配置。
        /// </summary>
        /// <param name="id">位置ID。</param>
        /// <returns>位置配置。</returns>
        public LuaTable GetPositionConfig(int id)
        {
            LuaTable t;
            m_CachePositionConfig.TryGetValue(id, out t);
            return t;
        }

        #endregion
    }
}

