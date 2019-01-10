/**
* @file     : CollectionObj.cs
* @brief    : 采集对象
* @details  : 采集对象
* @author   : CW
* @date     : 2017-07-04
*/
using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;

namespace SG
{
    public enum CollectionType
    {
        NONE = 0,
        ME = 1,
        FRIEND = 2,
        ENEMY = 3,
        NEUTRAL = 4
    }

[Hotfix]
    public class CollectionObj : Entity
    {
        private static string effect_mine = "Effect/scence/Banner_vfx_G";
        private static string effect_enmy = "Effect/scence/Banner_vfx_B";

        public static Dictionary<int, LuaTable> CacheConfig;

        private string m_Name;

        public string Name
        {
            get { return m_Name; }
        }

        protected byte m_Flag;
        public byte Flag
        {
            get { return m_Flag; }
            set { m_Flag = value; }
        }

        protected CollectionType m_Type;
        public CollectionType Type
        {
            get { return m_Type; }
        }

        private List<Transform> m_Childs = new List<Transform>();

        public override void Init(int resID, int configID, long ServerID, string strEnterAction = "", bool isNpc = false)
        {
            m_shadowType = 0;
            base.Init(resID, configID, ServerID, strEnterAction, isNpc);

            if (CacheConfig == null)
            {
                LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
                CacheConfig = G.Get<Dictionary<int, LuaTable>>("t_collection");
            }
            LuaTable t;
            if (CacheConfig.TryGetValue(configID, out t))
            {
                m_Name = t.Get<string>("name");
            }

            m_Type = CollectionType.NONE;
            m_Childs.Clear();
            for (int i = 0, imax = transform.childCount; i < imax; ++i)
            {
                Transform child = transform.GetChild(i);
                m_Childs.Add(child);
            }

            EventParameter ep = EventParameter.Get();
            ep.objParameter = this;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_COLLECT_INFO_CREATE, ep);
        }

        private void OnEnable()
        {
            if (ServerID != 0)
            {
                EventParameter ep = EventParameter.Get();
                ep.objParameter = this;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_COLLECT_INFO_CREATE, ep);
            }            
        }

        private GameObject effectObj = null;

        public void Change(CollectionType type)
        {
            if (m_Type == type)
            {
                return;
            }
            /*
            for (int i = 0; i < m_Childs.Count; i++)
            {
                Destroy(m_Childs[i].gameObject);
            }
            */
            if (effectObj != null)
            {
                Destroy(effectObj);
            }
            m_Childs.Clear();

            m_Type = type;
            //GameObject go = null;
            effectObj = null;
            if (m_Type == CollectionType.ME || m_Type == CollectionType.FRIEND)
            {
                effectObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(effect_mine);
            }
            else if (m_Type == CollectionType.ENEMY)
            {
                effectObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(effect_enmy);
            }

            if (null != effectObj)
            {
                effectObj.transform.parent = transform;
                effectObj.transform.localPosition = Vector3.zero;
                effectObj.transform.localEulerAngles = Vector3.zero;
                effectObj.transform.localScale = Vector3.one;

                m_Childs.Add(effectObj.transform);
            }
        }

        private void OnDisable()
        {
            EventParameter ep = EventParameter.Get();
            ep.objParameter = this;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_COLLECT_INFO_DESTORY, ep);
        }

        public override void RecycleObj()
        {
            if (m_Type == CollectionType.NONE)
            {
                base.RecycleObj();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public  void SelectTarget()
        {
            //显示选中圈    
            GameObject obj = CoreEntry.gSkillMgr.GetSelectTag(false);
            Transform t = obj.transform;
            obj.SetActive(true);
            t.SetParent(this.gameObject.transform);
            t.localPosition = new Vector3(0, 0.1f, 0);
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}

