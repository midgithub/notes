using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{
    /// <summary>
    /// 技能配置。
    /// </summary>
[Hotfix]
    public class SkillConfig 
    {
        /// <summary>
        /// 缓存技能配置。
        /// </summary>
        public Dictionary<int, LuaTable> m_CacheSkillConfig = null;

        /// <summary>
        /// 缓存被动技能配置。
        /// </summary>
        public Dictionary<int, LuaTable> m_CachePassiveSkillConfig = null;

        /// <summary>
        /// 缓存组索引配置。
        /// </summary>
        public Dictionary<int, LuaTable> m_CacheSkillGroupIndexConfig = null;

        /// <summary>
        /// 各职业的普攻技能。
        /// </summary>
        private Dictionary<int, List<int>> m_NormalAttack = new Dictionary<int, List<int>>();

        /// <summary>
        /// 各职业的技能列表。
        /// </summary>
        private Dictionary<int, List<int>> m_SkillList = new Dictionary<int, List<int>>();

        /// <summary>
        /// 缓存效果配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheEffectConfig = null;

        /// <summary>
        /// 缓存Buff配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheBuffConfig = null;

        /// <summary>
        /// 缓存怪物技能配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheMonsterSkillConfig = null;

        /// <summary>
        /// 缓存技能动作配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheSkillActionConfig = null;

        /// <summary>
        /// 缓存技能限制配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheSkillLimitConfig = null;

        /// <summary>
        /// 缓存内功配置。
        /// </summary>
        private Dictionary<int, LuaTable> m_CacheNeigongConfig = null;

        /// <summary>
        /// 构造函数。
        /// </summary>
        public SkillConfig()
        {
            InitConfig();
        }

        /// <summary>
        /// 初始化配置。
        /// </summary>
        public void InitConfig()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            m_CacheSkillConfig = G.Get<Dictionary<int, LuaTable>>("t_skill");
            m_CachePassiveSkillConfig = G.Get<Dictionary<int, LuaTable>>("t_passiveskill");
            m_CacheEffectConfig = G.Get<Dictionary<int, LuaTable>>("t_effect");
            m_CacheBuffConfig = G.Get<Dictionary<int, LuaTable>>("t_buff");
            m_CacheSkillGroupIndexConfig = G.Get<Dictionary<int, LuaTable>>("t_skillGroupIndex");
            m_CacheMonsterSkillConfig = G.Get<Dictionary<int, LuaTable>>("t_monsterskill");
            m_CacheSkillActionConfig = G.Get<Dictionary<int, LuaTable>>("t_skill_action");
            m_CacheSkillLimitConfig = G.Get<Dictionary<int, LuaTable>>("t_skilllimit");
            m_CacheNeigongConfig = G.Get<Dictionary<int, LuaTable>>("t_neigong");

            //各职业的普通攻击
            LuaTable skilldata = G.Get<LuaTable>("ConfigData").Get<LuaTable>("SkillConfig");
            List<LuaTable> normal = skilldata.Get<List<LuaTable>>("NormalAttack");
            m_NormalAttack.Clear();
            for (int i = 0; i < normal.Count; ++i)
            {
                List<int> list = new List<int>();
                List<LuaTable> temp = normal[i].Cast<List<LuaTable>>();
                for (int j = 0; j < temp.Count; ++j)
                {
                    int id = temp[j].Get<int>("id");
                    list.Add(id);
                }
                m_NormalAttack.Add(i + 1, list);
            }

            //各职业的技能列表
            List<LuaTable> skill = skilldata.Get<List<LuaTable>>("SkillList");
            m_SkillList.Clear();
            for (int i = 0; i < skill.Count; ++i)
            {
                List<int> list = new List<int>();
                List<LuaTable> temp = normal[i].Cast<List<LuaTable>>();
                for (int j = 0; j < temp.Count; ++j)
                {
                    int id = temp[j].Get<int>("group_id");
                    list.Add(id);
                }
                m_SkillList.Add(i + 1, list);
            }
        }

        /// <summary>
        /// 获取技能配置。
        /// </summary>
        /// <param name="id">技能编号。</param>
        /// <returns>技能配置。</returns>
        public LuaTable GetSkillConfig(int id)
        {
            LuaTable t;
            m_CacheSkillConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取效果配置。
        /// </summary>
        /// <param name="id">效果编号。</param>
        /// <returns>效果配置。</returns>
        public LuaTable GetEffectConfig(int id)
        {
            LuaTable t;
            m_CacheEffectConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取Buff配置。
        /// </summary>
        /// <param name="id">Buff编号。</param>
        /// <returns>Buff配置。</returns>
        public LuaTable GetBuffConfig(int id)
        {
            LuaTable t;
            m_CacheBuffConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取组索引配置。
        /// </summary>
        /// <param name="id">组编号。</param>
        /// <returns>组索引配置。</returns>
        public LuaTable GetSkillGroupIndexConfig(int id)
        {
            LuaTable t;
            m_CacheSkillGroupIndexConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取技能组ID。
        /// </summary>
        /// <param name="id">技能ID。</param>
        /// <returns>技能组ID。</returns>
        public int GetSkillGroup(int id)
        {
            int gid = 0;
            LuaTable t;
            if (m_CacheSkillConfig.TryGetValue(id, out t))
            {
                gid = t.Get<int>("group_id");
            }
            if(gid == 0)
            {
                if (m_CachePassiveSkillConfig.TryGetValue(id, out t))
                {
                    gid = t.Get<int>("group_id");
                }
            }
            return gid;
        }

        /// <summary>
        /// 获取普攻列表。
        /// </summary>
        /// <param name="prof">玩家职业。</param>
        /// <returns>普攻列表。(已按释放顺序进行排序)</returns>
        public List<int> GetNormalAttack(int prof)
        {
            List<int> list;
            m_NormalAttack.TryGetValue(prof, out list);
            return list;
        }

        /// <summary>
        /// 获取技能列表。
        /// </summary>
        /// <param name="prof">玩家职业。</param>
        /// <returns>技能列表。(下标对应技能位，为0表示未解锁)</returns>
        public List<int> GetSkillList(int prof)
        {
            List<int> glist;
            m_SkillList.TryGetValue(prof, out glist);
            return glist;
        }

        /// <summary>
        /// 获取怪物技能配置。
        /// </summary>
        /// <param name="id">怪物技能编号。</param>
        /// <returns>怪物技能配置。</returns>
        public LuaTable GetMonsterSkillConfig(int id)
        {
            LuaTable t;
            m_CacheMonsterSkillConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取技能动作配置。
        /// </summary>
        /// <param name="id">技能动作编号。</param>
        /// <returns>技能动作配置。</returns>
        public LuaTable GetSkillActionConfig(int id)
        {
            LuaTable t;
            m_CacheSkillActionConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取技能限制配置。
        /// </summary>
        /// <param name="id">技能限制编号。</param>
        /// <returns>技能限制配置。</returns>
        public LuaTable GetSkillLimitConfig(int id)
        {
            LuaTable t;
            m_CacheSkillLimitConfig.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// 获取内功配置。
        /// </summary>
        /// <param name="id">内功编号。</param>
        /// <returns>内功配置。</returns>
        public LuaTable GetNeigongConfig(int id)
        {
            LuaTable t;
            m_CacheNeigongConfig.TryGetValue(id, out t);
            return t;
        }
    }
}


