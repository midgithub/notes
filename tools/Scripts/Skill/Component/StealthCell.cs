/**
* @file     : StealthCell
* @brief    : ����Ԫ��
* @details  : ����Ԫ��
* @author   : CW
* @date     : 2017-09-14
*/
using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public class StealthCell : ISkillCell
    {
        private SkillBase m_SkillBase;
        private StealthCellDesc m_CellData;

        public override void Init(ISkillCellData cellData, SkillBase skillBase)
        {
            m_CellData = (StealthCellDesc)cellData;

            m_SkillBase = skillBase;
        }

        // PoolManager
        void OnEnable()
        {
            CancelInvoke("Start");
            Invoke("Start", 0.000001f);
        }

        void OnDisable()
        {
            CancelInvoke("Start");
            CancelInvoke("EnterStealth");
            CancelInvoke("ExitStealth");

            ExitStealth();
        }

        void Start()
        {
            CancelInvoke("Start");
            CancelInvoke("EnterStealth");
            CancelInvoke("ExitStealth");

            if (null == m_CellData || null == m_SkillBase || null == m_SkillBase.m_actor)
            {
                return;
            }

            if(!m_SkillBase.m_actor.Visiable)
            {
                return;
            }

            if(m_CellData.startTime > 0)
            {
                Invoke("EnterStealth", m_CellData.startTime);
            }
            else
            {
                EnterStealth();
            }
        }

        void EnterStealth()
        {
            if (null != m_SkillBase.m_actor)
            {
                m_SkillBase.m_actor.HideSelf();
                m_SkillBase.m_actor.HideBlobShadow();
                m_SkillBase.m_actor.HideEffect();
            }

            if (m_CellData.durationTime > 0)
            {
                Invoke("ExitStealth", m_CellData.durationTime);
            }
            else
            {
                ExitStealth();
            }

        }

        void ExitStealth()
        {
            if (null == m_SkillBase || null == m_SkillBase.m_actor)
            {
                return;
            }
            ActorObj actor = m_SkillBase.m_actor;
            if (!actor.Visiable)
            {
                return;
            }
            if (null != actor && actor.mActorState.CanShowSelf())
            {
                m_SkillBase.m_actor.ShowSelf();
                m_SkillBase.m_actor.ShowBlobShadow();
                m_SkillBase.m_actor.ShowEffect();
            }
        }
    }
}

