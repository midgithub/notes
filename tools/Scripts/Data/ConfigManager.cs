using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{
    /// <summary>
    /// 游戏配置管理。
    /// </summary>
[Hotfix]
    public class ConfigManager
    {
        //游戏配置管理单实例。
        private static ConfigManager _instance = null;

        /// <summary>
        /// 获取血条管理。
        /// </summary>
        public static ConfigManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConfigManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 技能配置。
        /// </summary>
        private SkillConfig m_Skill;

        /// <summary>
        /// 获取技能配置。
        /// </summary>
        public SkillConfig Skill
        {
            get
            {
                if (m_Skill == null)
                {
                    m_Skill = new SkillConfig();
                }
                return m_Skill;
            }
        }

        /// <summary>
        /// 背包物品配置。
        /// </summary>
        private BagItemConfig m_BagItem;

        /// <summary>
        /// 获取背包物品配置。
        /// </summary>
        public BagItemConfig BagItem
        {
            get
            {
                if (m_BagItem == null)
                {
                    m_BagItem = new BagItemConfig();
                }
                return m_BagItem;
            }
        }

        /// <summary>
        /// 商店配置。
        /// </summary>
        private ShopConfig m_Shop;

        /// <summary>
        /// 获取商店配置。
        /// </summary>
        public ShopConfig Shop
        {
            get
            {
                if (m_Shop == null)
                {
                    m_Shop = new ShopConfig();
                }
                return m_Shop;
            }
        }

        /// <summary>
        /// 常量配置。
        /// </summary>
        private ConstsConfig m_Consts;

        /// <summary>
        /// 获取常量配置。
        /// </summary>
        public ConstsConfig Consts
        {
            get
            {
                if (m_Consts == null)
                {
                    m_Consts = new ConstsConfig();
                }
                return m_Consts;
            }
        }

        /// <summary>
        /// 通用配置。
        /// </summary>
        private CommonConfig m_Common;

        /// <summary>
        /// 获取通用配置。
        /// </summary>
        public CommonConfig Common
        {
            get
            {
                if (m_Common == null)
                {
                    m_Common = new CommonConfig();
                }
                return m_Common;
            }
        }

        /// <summary>
        /// 公会配置。
        /// </summary>
        private GuildConfig m_Guild;

        /// <summary>
        /// 获取公会配置。
        /// </summary>
        public GuildConfig Guild
        {
            get
            {
                if (m_Guild == null)
                {
                    m_Guild = new GuildConfig();
                }
                return m_Guild;
            }
        }

        /// <summary>
        /// 境界配置。
        /// </summary>
        private JingJieConfig m_Jingjie;

        /// <summary>
        /// 获取境界配置。
        /// </summary>
        public JingJieConfig Jingjie
        {
            get
            {
                if (m_Jingjie == null)
                {
                    m_Jingjie = new JingJieConfig();
                }
                return m_Jingjie;
            }
        }

        /// <summary>
        /// 角色配置。
        /// </summary>
        private ActorConfig m_Actor;

        /// <summary>
        /// 获取角色配置。
        /// </summary>
        public ActorConfig Actor
        {
            get
            {
                if (m_Actor == null)
                {
                    m_Actor = new ActorConfig();
                }
                return m_Actor;
            }
        }

        /// <summary>
        /// 坐骑配置。
        /// </summary>
        private RideConfig m_Ride;

        /// <summary>
        /// 获取坐骑配置。
        /// </summary>
        public RideConfig Ride
        {
            get
            {
                if (m_Ride == null)
                {
                    m_Ride = new RideConfig();
                }
                return m_Ride;
            }
        }

        /// <summary>
        /// 地图配置。
        /// </summary>
        private MapConfig m_Map;

        /// <summary>
        /// 获取地图配置。
        /// </summary>
        public MapConfig Map
        {
            get
            {
                if (m_Map == null)
                {
                    m_Map = new MapConfig();
                }
                return m_Map;
            }
        }

        /// <summary>
        /// 任务配置。
        /// </summary>
        private TaskConfig m_Task;

        /// <summary>
        /// 获取任务配置。
        /// </summary>
        public TaskConfig Task
        {
            get
            {
                if (m_Task == null)
                {
                    m_Task = new TaskConfig();
                }
                return m_Task;
            }
        }

        /// <summary>
        /// 红颜配置。
        /// </summary>
        private BeautyWomanConfig m_BeautyWoman;

        /// <summary>
        /// 获取红颜配置。
        /// </summary>
        public BeautyWomanConfig BeautyWoman
        {
            get
            {
                if (m_BeautyWoman == null)
                {
                    m_BeautyWoman = new BeautyWomanConfig();
                }
                return m_BeautyWoman;
            }
        }

        /// <summary>
        /// 英雄配置。
        /// </summary>
        private HeroConfig m_HeroConfig;

        /// <summary>
        /// 获取英雄配置。
        /// </summary>
        public HeroConfig HeroConfig
        {
            get
            {
                if (m_HeroConfig == null)
                {
                    m_HeroConfig = new HeroConfig();
                }
                return m_HeroConfig;
            }
        }


    }
}

