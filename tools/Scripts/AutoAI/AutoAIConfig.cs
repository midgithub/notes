using UnityEngine;
using System.Collections;
using XLua;

namespace SG.AutoAI
{
    /// <summary>
    /// 自动战斗配置。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public class AutoAIConfig
    {
        #region --------------------保持血量--------------------

        /// <summary>
        /// 保持血量比例。
        /// </summary>
        public const string KEY_KEEPLIFE_PERCENT = "AUTOAI_KEEPLIFE_PERCENT";

        /// <summary>
        /// 购买药水。
        /// </summary>
        public const string KEY_BUY_POTION = "AUTOAI_BUY_POTION";

        /// <summary>
        /// 复活。
        /// </summary>
        public const string KEY_RELIVE = "AUTOAI_RELIVE";

        /// <summary>
        /// 保持血量比例。
        /// </summary>
        private float m_KeepLifePercent;
        
        /// <summary>
        /// 获取或设置要保持血量比例。
        /// </summary>
        public float KeepLifePercent
        {
            get
            {
                return m_KeepLifePercent;
            }
            set
            {
                m_KeepLifePercent = value;
                RoleUtility.SetFloat(KEY_KEEPLIFE_PERCENT, m_KeepLifePercent);
            }
        }

        /// <summary>
        /// 是否购买药水。
        /// </summary>
        private bool m_BuyPotion;

        /// <summary>
        /// 获取或设置是否购买药水。
        /// </summary>
        public bool BuyPotion
        {
            get { return m_BuyPotion; }
            set
            {
                m_BuyPotion = value;
                RoleUtility.SetBool(KEY_BUY_POTION, m_BuyPotion);
            }
        }

        /// <summary>
        /// 是否复活。
        /// </summary>
        public bool m_Relive;

        /// <summary>
        /// 获取或设置是否自动复活。
        /// </summary>
        public bool Relive
        {
            get { return m_Relive; }
            set
            {
                m_Relive = value;
                RoleUtility.SetBool(KEY_RELIVE, m_Relive);
            }
        }

        #endregion

        #region --------------------自动战斗--------------------

        /// <summary>
        /// 使用特殊技能。
        /// </summary>
        public const string KEY_USE_SPECIAL_SKILL = "AUTOAI_USE_SPECIAL_SKILL";

        /// <summary>
        /// 是否使用特殊技能。
        /// </summary>
        private bool m_UseSpecialSkill;

        /// <summary>
        /// 获取i或设置是否使用特殊技能。
        /// </summary>
        public bool UseSpecialSkill
        {
            get { return m_UseSpecialSkill; }
            set
            {
                m_UseSpecialSkill = value;
                RoleUtility.SetBool(KEY_USE_SPECIAL_SKILL, m_UseSpecialSkill);
            }
        }

        /// <summary>
        /// 是否自动反击。
        /// </summary>
        public bool AutoStrikeBack
        {
            get { return true; }
        }

        /// <summary>
        /// 无操作时反击间隔。
        /// </summary>
        public float StrikeBackGapWithIdle
        {
            get { return 5; }
        }

        #endregion

        /// <summary>
        /// 加载自动战斗配置。
        /// </summary>
        /// <returns>自动战斗配置数据。</returns>
        public static AutoAIConfig LoadConfig()
        {
            AutoAIConfig cfg = new AutoAIConfig();
            cfg.m_KeepLifePercent = RoleUtility.GetFloat(KEY_KEEPLIFE_PERCENT, 0.5f);
            cfg.m_BuyPotion = RoleUtility.GetBool(KEY_BUY_POTION, false);
            cfg.m_Relive = RoleUtility.GetBool(KEY_RELIVE, false);
            cfg.m_UseSpecialSkill = RoleUtility.GetBool(KEY_USE_SPECIAL_SKILL, false);
            return cfg;
        }
    }
}

