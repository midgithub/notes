/**
* @file     : BasePreloader
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-09-28
*/
using UnityEngine;
using System.Collections.Generic;

using XLua;

namespace SG
{
[Hotfix]
    public class BasePreloader
    {
        protected Dictionary<int, int> modelDict = new Dictionary<int,int>();

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="type"></param>
        public virtual void Init(ESceneMonsterType type)
        {

        }

        /// <summary>
        /// Ԥ��������
        /// </summary>
        public virtual void PreloadData()
        {
            modelDict.Clear();
        }

        /// <summary>
        /// Ԥ���ض���
        /// </summary>
        public virtual void PreloadObjects()
        {
            foreach (KeyValuePair<int, int> kv in modelDict)
            {
                PreloadModel(kv.Key, kv.Value);
                PreLoadMonsterSkill(kv.Key);
            }
        }

        /// <summary>
        /// ���Ӷ����������õ�Ԥ����������
        /// </summary>
        /// <param name="monsters"></param>
        protected virtual void AddMonsters(Dictionary<int, int> monsters)
        {
            foreach(KeyValuePair<int, int> kv in monsters)
            {
                AddMonster(kv.Key, kv.Value);
            }
        }

        /// <summary>
        /// ���Ӷ����������õ�Ԥ����������
        /// </summary>
        /// <param name="montersStr">��ʽΪ"id#id"</param>
        /// <param name="maxNum"></param>
        protected virtual void AddMonsters(string montersStr, int maxNum)
        {
            string[] monsters = montersStr.Split('#');
            if (null == monsters)
            {
                return;
            }

            for (int i = 0; i < monsters.Length; i++)
            {
                AddMonster(monsters[i], maxNum);
            }
        }

        /// <summary>
        /// ���Ӷ����������õ�Ԥ����������
        /// </summary>
        /// <param name="monstersStr">��ʽΪ"id,id"</param>
        /// <param name="maxNum"></param>
        protected virtual void AddMonstersExtra(string montersStr, int maxNum)
        {
            string[] monsters = montersStr.Split(',');
            if (null == monsters)
            {
                return;
            }

            for (int i = 0; i < monsters.Length; i++)
            {
                int id = 0;
                int.TryParse(monsters[i], out id);
                AddMonster(id, maxNum);
            }
        }

        /// <summary>
        /// ���ӹ������õ�Ԥ����������
        /// </summary>
        /// <param name="monsterStr">��ʽΪ"id,xx,xx"</param>
        /// <param name="maxNum"></param>
        protected virtual void AddMonster(string monsterStr, int maxNum)
        {
            string[] monster = monsterStr.Split(',');
            if (null == monster)
            {
                return;
            }

            int id = 0;
            int.TryParse(monster[0], out id);

            AddMonster(id, maxNum);
        }

        /// <summary>
        /// ���ӹ��ﵽԤ����������
        /// </summary>
        /// <param name="monsterID"></param>
        /// <param name="maxNum"></param>
        protected virtual void AddMonster(int monsterID, int maxNum)
        {
            LuaTable cfg = ConfigManager.Instance.Actor.GetMonsterConfig(monsterID);
            //monsterConfig monsterCfg = CSVConfigManager.GetmonsterConfig(monsterID);
            if (null == cfg)
            {
                return;
            }

            AddModel(cfg.Get<int>("modelId"), maxNum);
        }

        /// <summary>
        /// ����ģ�͵�Ԥ����������
        /// </summary>
        /// <param name="modelID"></param>
        /// <param name="maxNum"></param>
        protected virtual void AddModel(int modelID, int maxNum)
        {
            if (modelDict.ContainsKey(modelID))
            {
                if (modelDict[modelID] > maxNum)
                {
                    maxNum = modelDict[modelID];
                }
            }

            modelDict[modelID] = maxNum;
        }

        /// <summary>
        /// �ⲿ����Ԥ���ؽӿ�,�������֣�1��Ԥ�������ݣ�2��Ԥ���ض���
        /// </summary>
        public virtual void Preload()
        {
            PreloadData();
            PreloadObjects();
        }

        /// <summary>
        /// Ԥ���ع���
        /// </summary>
        /// <param name="monsterID"></param>
        /// <param name="maxNum"></param>
        protected virtual void PreloadMonster(int monsterID, int maxNum)
        {
            //Configs.monsterConfig monsterCfg = CSVConfigManager.GetmonsterConfig(monsterID);
            LuaTable cfg = ConfigManager.Instance.Actor.GetMonsterConfig(monsterID);
            if (null != cfg)
            {
                PreloadModel(cfg.Get<int>("modelId"), maxNum);
            }
        }

        /// <summary>
        /// Ԥ���ع���ģ��
        /// </summary>
        /// <param name="modelID"></param>
        /// <param name="maxNum"></param>
        protected virtual void PreloadModel(int modelID, int maxNum)
        {
            CoreEntry.gObjPoolMgr.PushToPool(modelID, maxNum);
        }

        /// <summary>
        /// Ԥ���ع��＼��
        /// </summary>
        /// <param name="monster"></param>
        protected virtual void PreLoadMonsterSkill(int monster)
        {
            CoreEntry.gObjPoolMgr.initMonsterSkillPool(monster);
        }
    }
}

