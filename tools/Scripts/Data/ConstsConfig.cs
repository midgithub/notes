/**
* @file     : ConstsConfig
* @brief    : ȫ�����ñ���ȡ�ӿ�
* @details  : ȫ�����ñ���ȡ�ӿ�
* @author   : CW
* @date     : 2017-09-14
*/

using UnityEngine;
using System.Collections.Generic;
using XLua;

namespace SG
{
    /// <summary>
    /// ȫ�ֳ������á�
    /// </summary>
[Hotfix]
    public class ConstsConfig
    {
        /// <summary>
        /// ���ŵ�������ӳ�䡣
        /// </summary>
        private Dictionary<int, LuaTable> m_ConstsCfg;

        /// <summary>
        /// ���Ƶ�������ӳ�䡣
        /// </summary>
        private Dictionary<string, LuaTable> m_NameIDDict;

        /// <summary>
        /// ���캯����
        /// </summary>
        public ConstsConfig()
        {
            InitConfig();
        }
        
        /// <summary>
        /// ��ʼ�����á�
        /// </summary>
        public void InitConfig()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            m_ConstsCfg = G.Get<Dictionary<int, LuaTable>>("t_consts");
            m_NameIDDict = new Dictionary<string, LuaTable>();
            foreach (var data in m_ConstsCfg)
            {
                LuaTable tbl = data.Value;
                string configName = tbl.Get<string>("name");
                if (!string.IsNullOrEmpty(configName))
                {
                    if (!m_NameIDDict.ContainsKey(configName))
                    {
                        m_NameIDDict.Add(configName, tbl);
                    }
                    else
                    {
                        LogMgr.UnityError("Config:t_consts: repeated name:" + configName);
                    }
                }
            }
        }

        /// <summary>
        /// ����ID��ȡlua��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LuaTable GetLuaTable(int id)
        {
            LuaTable t;
            m_ConstsCfg.TryGetValue(id, out t);
            return t;
        }

        /// <summary>
        /// �������ֻ�ȡlua��
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public LuaTable GetLuaTable(string name)
        {
            LuaTable t;
            m_NameIDDict.TryGetValue(name, out t);
            return t;
        }

        /// <summary>
        /// ����ID��ֵ���ƻ�ȡֵ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public T GetValue<T>(int id, string valueName)
        {
            LuaTable tbl = GetLuaTable(id);
            if (null == tbl)
            {
                return default(T);
            }

            return tbl.Get<T>(valueName);
        }

        /// <summary>
        /// �������ƺ�ֵ���ƻ�ȡֵ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public T GetValue<T>(string name, string valueName)
        {
            LuaTable tbl = GetLuaTable(name);
            if (null == tbl)
            {
                return default(T);
            }

            return tbl.Get<T>(valueName);
        }
    }
}

