using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{
    /// <summary>
    /// 通用配置。
    /// </summary>
[Hotfix]
    public class CommonConfig
    {
        /// <summary>
        /// 缓存系统公告配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheSystemNoticeConfig = null;

        /// <summary>
        /// 缓存公告配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheNoticeConfig = null;

        /// <summary>
        /// 缓存表情配置。 
        /// </summary>
        private Dictionary<int, string> m_CacheEmojiConfig = null;
        
        /// <summary>
        /// 缓存活动配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheActivityConfig = null;

        /// <summary>
        /// 王冠配置。
        /// </summary>
        private static Dictionary<int, LuaTable> m_CrownCfg = null;

        /// <summary>
        /// 面板编号配置。
        /// </summary>
        private static Dictionary<int, LuaTable> m_PanelIndexCfg = null;

        /// <summary>
        /// 构造函数。
        /// </summary>
        public CommonConfig()
        {
            InitConfig();
        }

        /// <summary>
        /// 初始化配置。
        /// </summary>
        public void InitConfig()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            m_CacheSystemNoticeConfig = G.GetInPath<Dictionary<int, LuaTable>>("ConfigData.CommonConfig.SysNotice");
            m_CacheNoticeConfig = G.GetInPath<Dictionary<int, LuaTable>>("ConfigData.ChatConfig.Notice");
            m_CacheEmojiConfig = G.GetInPath<Dictionary<int, string>>("ConfigData.ChatConfig.Emoji");    
            m_CacheActivityConfig = G.GetInPath<Dictionary<int, LuaTable>>("ConfigData.ActivityConfig.Activity");
            m_CrownCfg = G.GetInPath<Dictionary<int, LuaTable>>("ConfigData.CrownConfig.CrownCfg");
            m_CacheCampConfig = G.GetInPath<Dictionary<int, LuaTable>>("ConfigData.CommonConfig.Camp");
            m_CacheAudioConfig = G.GetInPath<Dictionary<int, LuaTable>>("ConfigData.CommonConfig.AudioDesc");
            m_CacheAnimationConfig = G.GetInPath<Dictionary<int, LuaTable>>("ConfigData.CommonConfig.Animation");
            m_CacheTeamTargetConfig = G.GetInPath<Dictionary<int, LuaTable>>("ConfigData.CommonConfig.TeamTarget");
            m_PanelIndexCfg = G.Get<Dictionary<int, LuaTable>>("t_panelindex");

            //初始化默认开启的功能。
            Dictionary<int, LuaTable> data = G.GetInPath<Dictionary<int, LuaTable>>("ConfigData.CommonConfig.FuncOpen");
            m_FuncOpen = data;
            m_DefaultOpen.Clear();
            foreach (var kvp in data)
            {
                if (kvp.Value.Get<bool>("defaultOpen"))
                {
                    m_DefaultOpen.Add(kvp.Key, true);
                }
            }
            //等级开放功能
            m_levelFuncOpen = G.GetInPath<Dictionary<int, LuaTable>>("ConfigData.CommonConfig.LevelFuncOpen");

            m_FuncOpenOrder = G.GetInPath<List<int>>("ConfigData.CommonConfig.FuncOrder");
            m_CacheModelConfig = G.GetInPath<Dictionary<int, LuaTable>>("ConfigData.NpcConfig.Model");
            m_CacheHorseConfig = G.GetInPath<Dictionary<int, LuaTable>>("ConfigData.HorseConfig.Horse");
        }

        /// <summary>
        /// 获取系统公告配置。
        /// </summary>
        /// <param name="id">系统公告ID。</param>
        /// <returns>配置数据。</returns>
        public LuaTable GetSystemNoticeConfig(int id)
        {
            LuaTable t;
            m_CacheSystemNoticeConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取公告配置。
        /// </summary>
        /// <param name="id">公告ID。</param>
        /// <returns>配置数据。</returns>
        public LuaTable GetNoticeConfig(int id)
        {
            LuaTable t;
            m_CacheNoticeConfig.TryGetValue(id, out t);
            return t;
        }
        
        /// <summary>
        /// 获取活动名称。
        /// </summary>
        /// <param name="id">活动编号。</param>
        /// <returns>活动名称。</returns>
        public string GetActivityName(int id)
        {
            LuaTable t;
            if (m_CacheActivityConfig.TryGetValue(id, out t))
            {
                return t.Get<string>("name");
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取表情图标。
        /// </summary>
        /// <param name="id">表情编号。</param>
        /// <returns>表情图标路径。</returns>
        public string GetEmoji(int id)
        {
            string ico;
            m_CacheEmojiConfig.TryGetValue(id, out ico);
            return ico;
        }

        /// <summary>
        /// 获取王冠配置。
        /// </summary>
        /// <param name="id">王冠id。</param>
        /// <returns>王冠配置。</returns>
        public LuaTable GetCrownConfig(int id)
        {
            LuaTable t;
            m_CrownCfg.TryGetValue(id, out t);
            return t;
        }

        public LuaTable GetPanelIndexConfig(int id)
        {
            LuaTable t;
            m_PanelIndexCfg.TryGetValue(id, out t);
            return t;
        }

        #region --------------------功能开启--------------------

        /// <summary>
        /// 要提示的功能开启顺序。
        /// </summary>
        private List<int> m_FuncOpenOrder = new List<int>();

        /// <summary>
        /// 默认开启的功能列表。
        /// </summary>
        private SortedDictionary<int, bool> m_DefaultOpen = new SortedDictionary<int, bool>();

        public Dictionary<int, LuaTable> m_FuncOpen;

        public Dictionary<int, LuaTable> m_levelFuncOpen;
        /// <summary>
        /// 要显示开启的功能顺序。
        /// </summary>
        public List<int> FuncOpenOrder
        {
            get { return m_FuncOpenOrder; }
        }

        /// <summary>
        /// 判断某个功能是否默认开启。
        /// </summary>
        /// <param name="id">功能编号。</param>
        /// <returns>是否默认开启。</returns>
        public bool IsDefaultOpen(int id)
        {
            return m_DefaultOpen.ContainsKey(id);
        }

        /// <summary>
        /// 判断某个功能是否开启。
        /// </summary>
        /// <param name="id">功能编号。</param>
        /// <returns>是否等级条件满足开启。</returns>
        public bool IsLevelOpen(int id)
        {
            LuaTable t; 
            if(m_levelFuncOpen.TryGetValue(id,out t))
            {
                int open_type = t.Get<int>("open_type");
                int open_level = t.Get<int>("open_level");
                if (open_type == 1 && open_level <= PlayerData.Instance.BaseAttr.Level)
                    return true;
            }
            return false;
        }


        public LuaTable GetLevelOpen(int id)
        {
            LuaTable t = null;
            if(m_FuncOpen.TryGetValue(id,out t))
            {
                return t;
            }
            if(m_levelFuncOpen.TryGetValue(id,out t))
            {
                return t;
            }
            return t;
        }

        #endregion

        #region --------------------模型--------------------

        /// <summary>
        /// 模型配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheModelConfig;

        /// <summary>
        /// 获取模型配置。
        /// </summary>
        /// <param name="id">模型ID。</param>
        /// <returns>模型配置。</returns>
        public LuaTable GetModelConfig(int id)
        {
            LuaTable t;
            m_CacheModelConfig.TryGetValue(id, out t);
            return t;
        }

        #endregion

        #region --------------------坐骑--------------------

        /// <summary>
        /// 坐骑配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheHorseConfig;

        /// <summary>
        /// 获取坐骑配置。
        /// </summary>
        /// <param name="id">模型ID。</param>
        /// <returns>坐骑配置。</returns>
        public LuaTable GetHorseConfig(int id)
        {
            LuaTable t;
            m_CacheHorseConfig.TryGetValue(id, out t);
            return t;
        }

        #endregion

        #region --------------------阵营--------------------

        /// <summary>
        /// 阵营配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheCampConfig;

        /// <summary>
        /// 获取阵营配置。
        /// </summary>
        /// <param name="id">阵营ID。</param>
        /// <returns>阵营配置。</returns>
        public LuaTable GetCampConfig(int id)
        {
            LuaTable t;
            m_CacheCampConfig.TryGetValue(id, out t);
            return t;
        }

        #endregion   

        #region --------------------音效--------------------

        /// <summary>
        /// 音效配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheAudioConfig;

        /// <summary>
        /// 获取音效配置。
        /// </summary>
        /// <param name="id">音效ID。</param>
        /// <returns>音效配置。</returns>
        public LuaTable GetAudioConfig(int id)
        {
            LuaTable t;
            m_CacheAudioConfig.TryGetValue(id, out t);
            return t;
        }

        #endregion

        #region --------------------过场动画--------------------

        /// <summary>
        /// 过场动画配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheAnimationConfig;

        /// <summary>
        /// 过场动画配置。
        /// </summary>
        public Dictionary<int, LuaTable> Animations
        {
            get { return m_CacheAnimationConfig; }
        }

        /// <summary>
        /// 获取过场动画配置。
        /// </summary>
        /// <param name="id">过场动画ID。</param>
        /// <returns>过场动画配置。</returns>
        public LuaTable GetAnimationConfig(int id)
        {
            LuaTable t;
            m_CacheAnimationConfig.TryGetValue(id, out t);
            return t;
        }

        #endregion   

        #region --------------------组队目标--------------------

        /// <summary>
        /// 组队目标配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheTeamTargetConfig;

        /// <summary>
        /// 组队目标配置。
        /// </summary>
        public Dictionary<int, LuaTable> TeamTarget
        {
            get { return m_CacheTeamTargetConfig; }
        }

        /// <summary>
        /// 获取组队目标配置。
        /// </summary>
        /// <param name="id">组队目标ID。</param>
        /// <returns>组队目标配置。</returns>
        public LuaTable GetTeamTargetConfig(int id)
        {
            LuaTable t;
            m_CacheTeamTargetConfig.TryGetValue(id, out t);
            return t;
        }

        #endregion   
    }
}

