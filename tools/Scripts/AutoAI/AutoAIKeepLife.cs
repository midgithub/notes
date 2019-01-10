using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;

namespace SG.AutoAI
{
    /// <summary>
    /// 检测血量。
    /// </summary>
[Hotfix]
    public class AutoAICheckLife : Action
    {
        /// <summary>
        /// 是否需要使用物品。
        /// </summary>
        private bool m_IsNeedUseItem = false;

        public override ActionState Update()
        {
            return m_IsNeedUseItem ? ActionState.Succeed : ActionState.Running;
        }

        protected override void OnStart()
        {
            LogMgr.LogAI("AutoAICheckLife.OnStart");
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_PLAYER_HP, OnLifeChange);
            CheckLife();
        }

        protected override void OnStop()
        {
            LogMgr.LogAI("AutoAICheckLife.OnStop");
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_PLAYER_HP, OnLifeChange);
        }

        public override void Reset()
        {
            LogMgr.LogAI("AutoAICheckLife.Reset");
            m_IsNeedUseItem = false;            
        }

        /// <summary>
        /// 玩家血量发生变化。
        /// </summary>
        public void OnLifeChange(GameEvent ge, EventParameter parameter)
        {
            CheckLife();
        }

        /// <summary>
        /// 检测生命值。
        /// </summary>
        private void CheckLife()
        {
            BaseAttr attr = PlayerData.Instance.BaseAttr;
            float rate = attr.CurHP * 1.0f / attr.MaxHP;
            m_IsNeedUseItem = rate < CoreEntry.gAutoAIMgr.Config.KeepLifePercent;
            if (m_IsNeedUseItem)
            {
                LogMgr.LogAI("Life rate is {0}. Need use potion!!!", rate);
            }
        }
    }

    /// <summary>
    /// 检测药水。
    /// </summary>
[Hotfix]
    public class AutoAICheckPotion : Action
    {
        /// <summary>
        /// 药水编号集合。
        /// </summary>
        public static HashSet<int> Potions = new HashSet<int> {110120001, 110120002, 110120003, 110120004, 110120005, 110120006, 110120007, 110120008, 110120009 };

        /// <summary>
        /// 是否有可使用物品。
        /// </summary>
        private bool m_IsHaveItem = false;

        /// <summary>
        /// 是否在请求购买中。
        /// </summary>
        private bool m_IsRequestBuyItem = false;

        /// <summary>
        /// 购买等待计数。
        /// </summary>
        private float m_BuyWaitCount = 0;

        public override void Reset()
        {
            LogMgr.LogAI("AutoAICheckPotion.Reset");
            m_IsHaveItem = false;
            m_IsRequestBuyItem = false;
        }

        protected override void OnStart()
        {
            LogMgr.LogAI("AutoAICheckPotion.OnStart");
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BAG_ITEM_ADD, OnBagChange);
            CheckItem();
        }

        protected override void OnStop()
        {
            LogMgr.LogAI("AutoAICheckPotion.OnStop");
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_BAG_ITEM_ADD, OnBagChange);
        }

        public override ActionState Update()
        {
            if (m_IsRequestBuyItem)
            {
                m_BuyWaitCount -= Time.deltaTime;
                if (m_BuyWaitCount <= 0)
                {
                    m_BuyWaitCount = 0;
                    m_IsRequestBuyItem = false;
                    LogMgr.LogAI("AutoAICheckPotion.Update. Buy potion wait time out!");
                }

                return ActionState.Running;
            }
            return m_IsHaveItem ? ActionState.Succeed : ActionState.Failed;
        }

        private void OnBagChange(GameEvent ge, EventParameter parameter)
        {
            ItemInfo iteminfo = GetPotion();
            m_IsHaveItem = iteminfo != null;
            if (m_IsHaveItem)
            {
                m_IsRequestBuyItem = false;
                m_BuyWaitCount = 0;
                LogMgr.LogAI("AutoAICheckPotion.OnBagChange. Get potion !!!");
            }
        }

        /// <summary>
        /// 检测物品。
        /// </summary>
        private void CheckItem()
        {
            ItemInfo iteminfo = GetPotion();
            m_IsHaveItem = iteminfo != null;
            if (!m_IsHaveItem && CoreEntry.gAutoAIMgr.Config.BuyPotion)
            {
                //购买流程
                int id = GetBuyPotionID();
                int shopid = ConfigManager.Instance.Shop.GetShopIDFromItem(id);
                ShopData.SendShoppingRequest(shopid, 99);
                m_IsRequestBuyItem = true;
                m_BuyWaitCount = 5;             //购买等待5秒
                LogMgr.LogAI("AutoAICheckPotion.CheckItem. Start waiting buy potion. time:{0}s", m_BuyWaitCount);
            }
        }

        /// <summary>
        /// 获取可使用的药水。
        /// </summary>
        /// <returns>可使用的药水所在的背包格子信息。</returns>
        public static ItemInfo GetPotion()
        {
            int level = PlayerData.Instance.BaseAttr.Level;
            BagInfo baginfo = PlayerData.Instance.BagData.GetBagInfo(BagType.ITEM_BAG_TYPE_COMMON);
            if (baginfo == null) return null;
            foreach (var kvp in baginfo.ItemInfos)
            {
                ItemInfo iteminfo = kvp.Value;
                if (Potions.Contains(iteminfo.ID))
                {
                    LuaTable cfg = ConfigManager.Instance.BagItem.GetItemConfig(iteminfo.ID);
                    if (cfg == null) continue;
                    if (level >= cfg.Get<int>("level"))
                    {
                        return iteminfo;
                    }
                }
            }
            LogMgr.LogAI("AutoAICheckPotion.GetPotion. Can not find useful potion!");
            return null;
        }

        /// <summary>
        /// 获取要适合购买的药水物品编号。
        /// </summary>
        /// <returns>物品编号。</returns>
        public static int GetBuyPotionID()
        {
            int level = PlayerData.Instance.BaseAttr.Level;
            int maxlevel = 0;
            int itemid = 0;
            foreach (int id in Potions)
            {
                LuaTable cfg = ConfigManager.Instance.BagItem.GetItemConfig(id);
                if (cfg == null) continue;
                int itemlevel = cfg.Get<int>("level");
                if (level >= itemlevel && itemlevel > maxlevel)
                {
                    itemid = id;
                    maxlevel = itemlevel;
                }
            }
            LogMgr.LogAI("AutoAICheckPotion.GetBuyPotionID id:{0} level:{1}", itemid, maxlevel);
            return itemid;
        }
    }

    /// <summary>
    /// 使用药水。
    /// </summary>
[Hotfix]
    public class AutoAIUsePotion : Action
    {
        /// <summary>
        /// 物品要等待的CD。
        /// </summary>
        private float m_CD = 0;

        /// <summary>
        /// 是否在请求物品使用中。
        /// </summary>
        private bool m_IsRequestUseItem = false;

        /// <summary>
        /// 使用的物品编号。
        /// </summary>
        private int m_UseID = 0;

        /// <summary>
        /// 是否使用成功。
        /// </summary>
        private bool m_UseSuccee = false;

        /// <summary>
        /// 使用等待计数。
        /// </summary>
        private float m_UseWaitCount = 0;

        public override void Reset()
        {
            LogMgr.LogAI("AutoAIUsePotion.Reset");
            m_CD = 0;
            m_IsRequestUseItem = false;
            m_UseSuccee = false;
        }

        protected override void OnStart()
        {
            LogMgr.LogAI("AutoAIUsePotion.OnStart");
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BAG_USE_ITEM, OnUseItem);
            TryUseItem();
        }

        protected override void OnStop()
        {
            LogMgr.LogAI("AutoAIUsePotion.OnStop");
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_BAG_USE_ITEM, OnUseItem);
        }

        public override ActionState Update()
        {
            //等待物品CD
            if (m_CD > 0)
            {
                m_CD -= Time.deltaTime;
                if (m_CD <= 0)
                {
                    m_CD = 0;
                    TryUseItem();
                }
                return ActionState.Running;
            }

            if (m_IsRequestUseItem)
            {
                m_UseWaitCount -= Time.deltaTime;
                if (m_UseWaitCount <= 0)
                {
                    m_UseWaitCount = 0;
                    m_IsRequestUseItem = false;
                    m_UseID = 0;
                    m_UseSuccee = false;
                    LogMgr.LogAI("AutoAIUsePotion.Update. Use item wait time out!");
                }
                return ActionState.Running;
            }

            return m_UseSuccee ? ActionState.Succeed : ActionState.Failed;
        }

        private void OnUseItem(GameEvent ge, EventParameter parameter)
        {
            if (m_UseID == parameter.intParameter1)
            {
                m_IsRequestUseItem = false;
                m_UseWaitCount = 0;
                m_UseID = 0;
                m_UseSuccee = parameter.intParameter == 0;
                LogMgr.LogAI("AutoAIUsePotion.OnUseItem. Use item result:{0}.", m_UseSuccee);
            }
        }

        /// <summary>
        /// 尝试使用物品。
        /// </summary>
        private void TryUseItem()
        {
            ItemInfo iteminfo = AutoAICheckPotion.GetPotion();      //物品可能已经被使用了
            if (iteminfo == null)
            {
                return;
            }

            //CD判断
            m_CD = iteminfo.CD;
            if (m_CD <= 0)
            {
                m_IsRequestUseItem = true;
                m_UseWaitCount = 5;
                m_UseID = iteminfo.ID;
                PlayerData.Instance.BagData.SendUseItemRequest(iteminfo.Bag, iteminfo.Pos, 1, 0);
                LogMgr.LogAI("AutoAIUsePotion.TryUseItem. Send use item request.");
            }
        }
    }
}

