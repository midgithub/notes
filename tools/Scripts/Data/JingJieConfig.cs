using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{
    /// <summary>
    /// 境界配置。
    /// </summary>
[Hotfix]
    public class JingJieConfig
    {
        private Dictionary<int, LuaTable> mNodeCfg = null;
        private static Dictionary<int, LuaTable> mLvCfg = null;

        /// <summary>
        /// 构造函数。
        /// </summary>
        public JingJieConfig()
        {
            InitConfig();
        }

        /// <summary>
        /// 初始化配置。
        /// </summary>
        private void InitConfig()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            mNodeCfg = G.Get<Dictionary<int, LuaTable>>("t_dianfengnode");
            mLvCfg = G.Get<Dictionary<int, LuaTable>>("t_dianfenglvup");
        }

        public LuaTable GetNodeConfig(int id)
        {
            LuaTable t;
            mNodeCfg.TryGetValue(id, out t);
            return t;
        }

        public LuaTable GetLevelConfig(int id)
        {
            LuaTable t;
            mLvCfg.TryGetValue(id, out t);
            return t;
        }
    }
}

