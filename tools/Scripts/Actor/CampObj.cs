/**
* @file     : #FileName#
* @brief    : 
* @details  : 
* @author   : #Author#
* @date     : #Date#
*/
using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
    public enum CampType
    {
        NONE = 0,
        ME = 1,
        FRIEND = 2,
        ENEMY = 3,
        NEUTRAL = 4
    }

[Hotfix]
    public class CampObj : Entity
    {
        private static string effect_mine = "Effect/scence/sf_effect/xianzhi_lv";
        private static string effect_enmy = "Effect/scence/sf_effect/xianzhi_hong";

        private CampType m_CampType;
        public CampType CampObjType
        {
            get{ return m_CampType; }
        }

        private Vector3 m_EffectSize = Vector3.one;
        public Vector3 EffectSize
        {
            set { m_EffectSize = value; }
            get { return m_EffectSize; }
        }

        private GameObject m_CampEffect;
        public GameObject CampEffect
        {
            get { return m_CampEffect; }
        }

        public override void Init(int resID, int ConfigID, long ServerID, string strEnterAction = "", bool isNpc = false)
        {
            m_shadowType = 0;
            base.Init(resID, ConfigID, ServerID, strEnterAction, isNpc);

            m_CampType = CampType.NONE;
        }
        
        public void ChangeType(CampType type)
        {
            if(m_CampType == type)
            {
                return;
            }

            if(null != m_CampEffect)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(m_CampEffect);
            }

            m_CampType = type;
            if (m_CampType == CampType.ME || m_CampType == CampType.FRIEND)
            {
                m_CampEffect = CoreEntry.gGameObjPoolMgr.InstantiateEffect(effect_mine);
            }
            else if (m_CampType == CampType.ENEMY)
            {
                m_CampEffect = CoreEntry.gGameObjPoolMgr.InstantiateEffect(effect_enmy);
            }

            if (null != m_CampEffect)
            {
                m_CampEffect.transform.parent = transform;
                m_CampEffect.transform.localPosition = Vector3.zero;
                m_CampEffect.transform.localEulerAngles = new Vector3(-90f, 0f, 90f);
                m_CampEffect.transform.localScale = new Vector3(m_EffectSize.x, m_EffectSize.y,1);
            }
        }

        public override void RecycleObj()
        {
            if (null != m_CampEffect)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(m_CampEffect);
            }

            Destroy(gameObject);
        }
    }
}

