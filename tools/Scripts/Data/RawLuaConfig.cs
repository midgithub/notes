using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XLua;

namespace SG
{
    /// <summary>
    /// 用于载入Lua表, 单例类
    /// </summary>
[Hotfix]
    class RawLuaConfig
    {
        static public RawLuaConfig Instance
        {
            get
            {
                if(null == _inst)
                {
                    _inst = new RawLuaConfig();
                }
                return _inst;
            }
        }

        private RawLuaConfig()
        {
            m_RawConfig = new Dictionary<string, Dictionary<int, LuaTable>>();
        }
        private static RawLuaConfig _inst = null;

        /// <summary>
        /// 载入特定原始Lua表
        /// </summary>
        /// <param name="strRaw"></param>
        /// <returns>返回字段</returns>
        public Dictionary<int, LuaTable> LoadConfig(string strRaw)
        {
            if(m_RawConfig.ContainsKey(strRaw))
            {
                return m_RawConfig[strRaw];
            }
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            Dictionary<int, LuaTable> rawConfig = G.Get<Dictionary<int, LuaTable>>(strRaw);
            if(null != rawConfig)
            {
                m_RawConfig[strRaw] = rawConfig;
            }
            return rawConfig;
        }

        /// <summary>
        /// 取得特定表的某行
        /// </summary>
        public LuaTable GetRowData(string strRaw, int iLine)
        {
            Dictionary<int, LuaTable> cfg = LoadConfig(strRaw);
            if (null == cfg) return null;

            LuaTable row = null;
            cfg.TryGetValue(iLine, out row);
            return row;
        }

        // 所有的原始Lua表记录
        private Dictionary<string, Dictionary<int, LuaTable>> m_RawConfig = null;
    }
}

