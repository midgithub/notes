using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XLua;

namespace SG
{

[Hotfix]
    public class RunToAttackState : RunState
    {
        // m_actor.m_curCastSkillID  技能ID
        protected GameDBMgr m_gameDBMgr = null;
        protected GameObject m_actorObject = null;
        protected LuaTable m_curSkillDesc = null;
        public int m_curSkillID = 0;
        protected Vector3 m_vNavPos = new Vector3(0, 0, 0);
        protected float m_Attackdistance = 0;

        void Awake()
        {
            m_gameDBMgr = CoreEntry.gGameDBMgr;
        }

        public void ChooseAttackAim(ActorObj actorObj, int skillID)
        {
            m_actor = actorObj;
            LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(skillID);
            if (skillDesc != null)
            {
                ActorObj selObject = null;
                //旋转到当前的方向        
                if (actorObj.mActorType == ActorType.AT_LOCAL_PLAYER)
                {
                    selObject = actorObj.GetSelTarget();
                    if (selObject == null)
                    {
                        return;
                    }

                    Vector3 aimPos = selObject.transform.position;
                    actorObj.transform.LookAt(new Vector3(aimPos.x, actorObj.transform.position.y, aimPos.z));
                }
            }
        }

        public override void OnEnter(ActorObj actorBase)
        {
            base.OnEnter(actorBase);
            m_state = ACTOR_STATE.AS_RUN_TO_ATTACK;

            m_actor = actorBase;
            m_curSkillID = m_actor.m_RuncastSKillID;

            if (m_curSkillID == 0)
                return;

            m_actorObject = m_actor.thisGameObject;

            ChooseAttackAim(m_actor, m_curSkillID);

            ActorObj selObject = m_actor.GetSelTarget();


            LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(m_curSkillID);

            //SkillClassDisplayDesc skillClass = m_gameDBMgr.GetSkillClassDisplayDesc(skillDesc.skillDisplayID);

            m_curSkillDesc = skillDesc;

            m_Attackdistance = m_curSkillDesc.Get<float>("min_dis");
            m_vNavPos = m_actor.transform.position;
            if (selObject != null)
            {
                float distance = Vector3.Distance(m_actor.transform.position, selObject.transform.position);
                if (distance > m_Attackdistance)
                {
                    Vector3 vDis = selObject.transform.position - m_actor.transform.position;

                    vDis.Normalize();
                    m_vNavPos = m_actor.transform.position + vDis * m_Attackdistance;
                }
            }
            m_actor.MoveToPos(m_vNavPos, false);

            Debug.DrawLine(m_actor.transform.position, m_vNavPos, Color.green);
            m_actor.FaceTo(m_vNavPos);

            m_actor.m_bRunToAttack = true;
        }

        public override void OnExit(StateParameter stateParm)
        {
            //退出跑步
            if (stateParm.state == ACTOR_STATE.AS_RUN || stateParm.state == ACTOR_STATE.AS_RUN_TO_ATTACK)
            {
                return;
            }

            m_actor.StopMove(false);

        }

        public override void Update()
        {
            if (m_actor != null && m_actorObject != null && m_curSkillID != 0 && m_actor.m_bRunToAttack)
            {
                ActorObj selObject = m_actor.GetSelTarget();
                if (selObject != null)
                {
                    float distance = Vector3.Distance(m_actorObject.transform.position, selObject.transform.position);
                    if (distance <= m_Attackdistance * 1.5f)// 适当增加攻击距离
                    {
                        m_actor.OnCastSkill(m_curSkillID);

                        m_actorObject = null;
                        m_curSkillID = 0;
                    }
                    else
                    {
                        if (selObject != null)
                        {
                            float dist = Vector3.Distance(m_vNavPos, selObject.transform.position);
                            if (dist > 0.1f)
                            {
                                Vector3 vDis = selObject.transform.position - m_actor.transform.position;
                                vDis.Normalize();
                                m_vNavPos = m_actor.transform.position + vDis * m_Attackdistance;
                            }
                        }
                        m_actor.MoveToPos(m_vNavPos, false);

                        Debug.DrawLine(m_actor.transform.position, m_vNavPos, Color.red);

                    }
                }
            }
        }

    }
}

