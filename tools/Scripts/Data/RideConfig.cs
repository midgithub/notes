using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{
    /// <summary>
    /// 坐骑配置。
    /// </summary>
[Hotfix]
    public class RideConfig
    {
        /// <summary>
        /// 缓存坐骑配置。
        /// </summary>
        public Dictionary<int, LuaTable> m_CacheHorseConfig = new Dictionary<int, LuaTable>();

        /// <summary>
        /// 缓存坐骑皮肤配置。
        /// </summary>
        public Dictionary<int, LuaTable> m_CacheHorseSkinConfig = new Dictionary<int, LuaTable>();

        /// <summary>
        /// 自动上坐骑距离。
        /// </summary>
        private float m_AutoMountDistance = 1;

        /// <summary>
        /// 获取自动上坐骑距离。
        /// </summary>
        public float AutoMountDistance
        {
            get { return m_AutoMountDistance; }
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        public RideConfig()
        {
            InitConfig();
        }

        /// <summary>
        /// 初始化配置。
        /// </summary>
        public void InitConfig()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            m_CacheHorseConfig = G.Get<Dictionary<int, LuaTable>>("t_horse");
            m_CacheHorseSkinConfig = G.Get<Dictionary<int, LuaTable>>("t_horseskn");
            m_AutoMountDistance = G.GetInPath<int>("ConfigData.ConstsConfig.NameToValue.mountDistance.val1");
        }

        /// <summary>
        /// 获取坐骑配置。
        /// </summary>
        /// <param name="id">坐骑编号。</param>
        /// <returns>坐骑配置。</returns>
        public LuaTable GetRideConfig(int id)
        {
            LuaTable t;
            m_CacheHorseConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取坐骑皮肤路径。
        /// </summary>
        /// <param name="id">坐骑阶段或皮肤编号。</param>
        /// <returns>皮肤路径。</returns>
        public string GetRideSkin(int id,bool bShowUI = false)
        {
            LuaTable t;
            if (!m_CacheHorseConfig.TryGetValue(id, out t))
            {
                if (!m_CacheHorseSkinConfig.TryGetValue(id, out t))
                {
                    return string.Empty;
                }
            }
            string key = "";
            if (bShowUI)
            {
                key = string.Format("modelui{0}", PlayerData.Instance.BaseAttr.Prof);
            }
            else
            {
                key = string.Format("model{0}", PlayerData.Instance.BaseAttr.Prof);
            }
            int mid = t.Get<int>(key);
            LuaTable cfg = ConfigManager.Instance.Common.GetModelConfig(mid);
            if (cfg == null)
            {
                return string.Empty;
            }
            return cfg.Get<string>("skl");
        }

        public int GetModelID(int id)
        {
            LuaTable t;
            if (!m_CacheHorseConfig.TryGetValue(id, out t))
            {
                if (!m_CacheHorseSkinConfig.TryGetValue(id, out t))
                {
                    return 0;
                }
            }

            string key = string.Format("model{0}", PlayerData.Instance.BaseAttr.Prof);
            int modelID = t.Get<int>(key);

            return modelID;
        }

        /// <summary>
        /// 获取坐骑名称。
        /// </summary>
        /// <param name="id">坐骑编号。</param>
        /// <returns>坐骑名称。</returns>
        public string GetRideName(int id)
        {
            LuaTable t;
            if (!m_CacheHorseConfig.TryGetValue(id, out t))
            {
                if (!m_CacheHorseSkinConfig.TryGetValue(id, out t))
                {
                    return string.Empty;
                }
            }

            return t.Get<string>("name");
        }
    }
}

