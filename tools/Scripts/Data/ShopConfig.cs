using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{
    /// <summary>
    /// 商店配置。
    /// </summary>
[Hotfix]
    public class ShopConfig
    {
        /// <summary>
        /// 缓存商品配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheShopConfig = null;

        /// <summary>
        /// 缓存商品配置。物品ID为Key。
        /// </summary>
        private Dictionary<int, int> m_CacheItemShopConfig = null;

        /// <summary>
        /// 构造函数。
        /// </summary>
        public ShopConfig()
        {
            InitConfig();
        }

        /// <summary>
        /// 初始化配置。
        /// </summary>
        public void InitConfig()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            m_CacheShopConfig = G.Get<Dictionary<int, LuaTable>>("t_shop");
            m_CacheItemShopConfig = new Dictionary<int, int>();
            foreach (var kvp in m_CacheShopConfig)
            {
                LuaTable table = kvp.Value;
                int id = table.Get<int>("id");
                int itemid = table.Get<int>("itemId");
                if (!m_CacheItemShopConfig.ContainsKey(itemid))
                {
                    m_CacheItemShopConfig.Add(itemid, id);
                }
            }
        }

        /// <summary>
        /// 获取商品配置。
        /// </summary>
        /// <param name="id">商品ID。</param>
        /// <returns>商品配置。</returns>
        public LuaTable GetShopConfig(int id)
        {
            LuaTable t;
            m_CacheShopConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取通过物品编号获取出售对应商品ID。
        /// </summary>
        /// <param name="id">物品ID。</param>
        /// <returns>商品ID，0表示不出售此物品。</returns>
        public int GetShopIDFromItem(int id)
        {
            int shopid;
            m_CacheItemShopConfig.TryGetValue(id, out shopid);
            return shopid;
        }

    }
}

