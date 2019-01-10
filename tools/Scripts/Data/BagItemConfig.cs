using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{
    /// <summary>
    /// 背包物品配置。
    /// </summary>
[Hotfix]
    public class BagItemConfig
    {
        /// <summary>
        /// 背包初始大小。
        /// </summary>
        private int m_BagInitSize = 0;

        /// <summary>
        /// 仓库初始大小。
        /// </summary>
        private int m_DepotInitSize = 0;

        /// <summary>
        /// 缓存装备消耗配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheEquipConfig = null;

        /// <summary>
        /// 缓存背包扩展消耗配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheBagCostConfig = null;

        /// <summary>
        /// 缓存仓库扩展消耗配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheDepotCostConfig = null;

        /// <summary>
        /// 缓存物品配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheItemConfig = null;

        /// <summary>
        /// 获取物品配置。
        /// </summary>
        public Dictionary<int, LuaTable> Items
        {
            get { return m_CacheItemConfig; }
        }

        /// <summary>
        /// 翅膀配置
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheWingConfig = null;
        /// <summary>
        /// 翅膀皮肤配置
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheWingSkinConfig = null;

        /// <summary>
        /// 套装配置
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheEquipGroupConfig = null;

        /// <summary>
        /// 缓存时装配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheFashionConfig = null;

        /// <summary>
        /// 缓存时装组配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheFashionGroupConfig = null;

        /// <summary>
        /// 缓存神兵配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheShenBingConfig = null;

        /// <summary>
        /// 缓存圣盾(披风)配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheShieldConfig = null;

        /// <summary>
        /// 品质颜色。
        /// </summary>
        private string[] m_QualityColor = new string[] { "FFFFFF", "20EB6D", "34CEEF", "B57FFF", "E8C01F", "FFFC00","FF0000", "FF0000" };

        /// <summary>
        /// 构造函数。
        /// </summary>
        public BagItemConfig()
        {
            InitConfig();
        }

        /// <summary>
        /// 初始化配置。
        /// </summary>
        public void InitConfig()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            m_CacheEquipConfig = G.Get<Dictionary<int, LuaTable>>("t_equip");
            m_CacheBagCostConfig = G.Get<Dictionary<int, LuaTable>>("t_packetcost");
            m_CacheDepotCostConfig = G.Get<Dictionary<int, LuaTable>>("t_storagecost");
            m_CacheWingConfig = G.Get<Dictionary<int, LuaTable>>("t_newwing");
            m_CacheWingSkinConfig = G.Get<Dictionary<int, LuaTable>>("t_newwingskin");
            m_CacheEquipGroupConfig = G.Get<Dictionary<int, LuaTable>>("t_equipgroup");
            m_CacheItemConfig = G.Get<Dictionary<int, LuaTable>>("t_item");
            m_CacheFashionConfig = G.Get<Dictionary<int, LuaTable>>("t_fashions");
            m_CacheFashionGroupConfig = G.Get<Dictionary<int, LuaTable>>("t_fashiongroup");
            m_CacheShenBingConfig = G.Get<Dictionary<int, LuaTable>>("t_shenbing");
            m_CacheShieldConfig = G.Get<Dictionary<int, LuaTable>>("t_pifeng");

            LuaTable itemconfig = G.Get<LuaTable>("ConfigData").Get<LuaTable>("ItemConfig");
            m_BagInitSize = itemconfig.Get<int>("BagInitSize");
            m_DepotInitSize = itemconfig.Get<int>("StorageInitSize");
        }

        /// <summary>
        /// 获取品质颜色。
        /// </summary>
        /// <param name="quality"></param>
        /// <returns></returns>
        public string GetQualityColor(int quality)
        {
            int index = quality;
            if (index < 0 || index >= m_QualityColor.Length)
            {
                index = 0;
            }
            return m_QualityColor[index];
        }
        string[] QualityFrame = new string[] { "Common:back_q0", "Common:back_q1", "Common:back_q2", "Common:back_q3", "Common:back_q4", "Common:back_q5", "Common:back_q6", "Common:back_q7", "Common:back_q7" };
        //获取品质框 qua:配置表对应的品质
        public string GetQualityFrame(int qua)
        {
            return QualityFrame[qua];
        }


        /// <summary>
        /// 获取背包格子开启需要的时间。
        /// </summary>
        /// <param name="bag">背包类型。</param>
        /// <param name="pos">格子位置。</param>
        /// <returns>格子开启需要的时间。</returns>
        public int GetOpenNeedTime(int bag, int pos)
        {
            int ret = 0;
            Dictionary<int, LuaTable> data = null;
            int id = 0;
            if (bag == BagType.ITEM_BAG_TYPE_COMMON)
            {
                data = m_CacheBagCostConfig;
                id = pos + 1 - m_BagInitSize;
            }
            else if (bag == BagType.ITEM_BAG_TYPE_STORAGE || bag == BagType.ITEM_BAG_TYPE_STORAGE_2
                || bag == BagType.ITEM_BAG_TYPE_STORAGE_3 || bag == BagType.ITEM_BAG_TYPE_STORAGE_4)
            {
                data = m_CacheDepotCostConfig;
                id = pos + 1 - m_DepotInitSize;
            }
            LuaTable t;
            if (data != null && data.TryGetValue(id, out t))
            {
                ret = t.Get<int>("autoTime") * 60;          //配的是分钟
            }
            return ret;
        }

        /// <summary>
        /// 获取物品信息。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LuaTable GetItemConfig(int id)
        {
            LuaTable t;
            if (m_CacheItemConfig.TryGetValue(id, out t))
            {
                return t;
            }
            if (m_CacheEquipConfig.TryGetValue(id, out t))
            {
                return t;
            }

            return null;
        }


        public LuaTable GetEquipConfig(int id)
        {
            LuaTable t;

            if (m_CacheEquipConfig.TryGetValue(id, out t))
            {
                return t;
            }

            return null;
        }

        /// <summary>
        /// 获取物品名称。
        /// </summary>
        /// <param name="id">物品编号。</param>
        /// <returns>物品名称。</returns>
        public string GetItemName(int id)
        {
            LuaTable t = GetItemConfig(id);
            if (t == null)
            {
                return string.Empty;
            }
            return t.Get<string>("name");
        }


        /// <summary>
        /// 通过装备套装ID获取对应套装配置。
        /// </summary>
        /// <param name="groupID">套装ID。</param>
        /// <returns></returns>
        public LuaTable GetEquipGroupConfig(int groupID)
        {
            LuaTable t;
            if (m_CacheEquipGroupConfig.TryGetValue(groupID, out t))
            {
                return t;
            }
            return null;
        }

        /// <summary>
        /// 通过时装套装ID获取对应套装配置。
        /// </summary>
        /// <param name="groupID">时装ID。</param>
        /// <returns>套装配置。</returns>
        public LuaTable GetFashionGroupConfig(int groupID)
        {
            LuaTable t;
            m_CacheFashionGroupConfig.TryGetValue(groupID, out t);
            return t;
        }

        /// <summary>
        /// 通过物品获取对应的翅膀ID。
        /// </summary>
        /// <param name="itemId">物品ID。</param>
        /// <returns>翅膀ID。</returns>
        public int GetWingIdFromItem(int itemId)
        {
            LuaTable t;
            if (m_CacheItemConfig.TryGetValue(itemId, out t))
            {
                int mid = t.Get<int>("link_param");
                return mid;
            }
            return 0;
        }

        /// <summary>
        /// 获取翅膀模型ID。
        /// </summary>
        /// <param name="id">翅膀ID。</param>
        /// <returns>翅膀模型ID。</returns>
        public int GetWingModelID(int id)
        {
            LuaTable t;
            if(m_CacheWingSkinConfig.TryGetValue(id, out t))
            {
                int mid = t.Get<int>("modelID");
                return mid;
            }else if (m_CacheWingConfig.TryGetValue(id + 1000, out t))
            {
                int mid = t.Get<int>("modelID");
                return mid;
            }
            return 0;
        }

        /// <summary>
        /// 获取时装模型ID。
        /// </summary>
        /// <param name="id">时装ID。</param>
        /// <param name="job">职业。</param>
        /// <returns>模型ID。</returns>
        public int GetFashionModelID(int id, int job,int equipStarMin = 0)
        {
            LuaTable t;
            if (m_CacheFashionConfig.TryGetValue(id, out t))
            {
                int star = EquipDataMgr.Instance.GetFashionStar(equipStarMin);
                string key = string.Format("vmesh{0}", job);
                if (star > 0)
                {
                    key = string.Format("vmesh{0}_light{1}", job,star);
                }
                int mid = t.Get<int>(key);
                return mid;
            }
            return 0;
        }

        /// <summary>
        /// 获取神兵配置。
        /// </summary>
        /// <param name="id">神兵编号。</param>
        /// <returns>神兵配置。</returns>
        public LuaTable GetShenBingConfig(int id)
        {
            LuaTable t;
            m_CacheShenBingConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取圣盾(披风)配置。
        /// </summary>
        /// <param name="id">圣盾编号。</param>
        /// <returns>圣盾(披风)配置。</returns>
        public LuaTable GetShieldConfig(int id)
        {
            LuaTable t;
            m_CacheShieldConfig.TryGetValue(id, out t);
            return t;
        }
    }
}

