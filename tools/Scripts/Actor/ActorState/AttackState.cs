using UnityEngine;
using System.Collections;
using XLua;

namespace SG
{
    //攻击状态
[Hotfix]
    public class AttackState : IActorState
    {
        public int m_curSkillID = 0;
        public SkillBase m_curSkillBase = null;
        private float m_lastestEndTime = 0;
        private GameDBMgr m_gameDBMgr = null;
        //private GameObject m_actorObject = null;
        private LuaTable m_curSkillDesc = null;

        ////技能是否结束，释放完
        //private bool m_isSkillCastEnd = false;

        //yy 后摇时间
        private bool m_bAfterAttack = false;

        void Awake()
        {
            m_gameDBMgr = CoreEntry.gGameDBMgr;
        }

        public override void OnEnter(ActorObj actor)
        {
            if (actor == null)
                return;

            m_state = ACTOR_STATE.AS_ATTACK;

            m_actor = actor;
            m_curSkillID = m_actor.castSkillID;

            //if (m_actor != null)
            //{
            //    m_actor.m_bIsInAttack = true;
            //}

            LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(m_curSkillID);
            if (skillDesc == null) return;
            SkillClassDisplayDesc skillClass = null;

            if (skillDesc != null)
            {
                //if (skillDesc.e_type_1 == 1)
                {
                    skillClass = m_gameDBMgr.GetSkillClassDisplayDesc(skillDesc.Get<int>("skillDisplayID"));
                }
            }

            m_curSkillDesc = skillDesc;

            GameObject obj = null;
            if (skillClass != null)
            {
                obj = CoreEntry.gGameObjPoolMgr.InstantiateSkillBase("Skill/SkillBase"); ;//(GameObject)Instantiate(CoreEntry.gResLoader.LoadResource("Skill/SkillBase"));
            }
            else
            {
               // LogMgr.Log("Can not found skillClass skillDesc.skillDisplayID: " + skillDesc.Get<int>("skillDisplayID") + " ActorObj: " + m_actor);
                return;
            }

            if (obj == null)
            {
                return;
            }

            //设置脚本组件
            SkillBase skillBase = obj.GetComponent<SkillBase>();

            ActorObj hitObj = null;
            if (CurParam != null)
            {
                hitObj = CurParam.HitActor;
            }

            skillBase.Init(actor, hitObj, m_curSkillID);

            m_curSkillBase = skillBase;


            //霸体时间。不可以转换为其他状态，除了死亡
            if (skillDesc.Get<int>("stiff_time") > 0)
            {
                StartEndure();
            }

            //m_isSkillCastEnd = false;
            if (m_curSkillDesc.Get<int>("stiff_time") > 0)
            {
                CancelInvoke("SkillCastEnd");
                Invoke("SkillCastEnd", m_curSkillDesc.Get<int>("stiff_time") / 1000f);
            }

            //一个人的时候触发关门
            if (m_actor.mActorType == ActorType.AT_BOSS)
            {
                CoreEntry.gSceneMgr.CloseDoor();
            }

            if (null != skillDesc && skillDesc.Get<int>("showtype") == (int)SkillShowType.ST_HUANLING && skillDesc.Get<int>("subtype") != (int)SkillType.SKILL_NORMAL)
            {
                bool isHuanlingSkill = false;
                if (actor.mActorType == ActorType.AT_LOCAL_PLAYER)
                {
                    isHuanlingSkill = true;
                }
                else if (actor.mActorType == ActorType.AT_PET)
                {
                    PetObj pet = actor as PetObj;
                    if (pet != null && null != pet.m_MasterActor && pet.m_MasterActor.mActorType == ActorType.AT_LOCAL_PLAYER)
                    {
                        isHuanlingSkill = true;
                    }
                }

                if (isHuanlingSkill)
                {
                    EventParameter param = EventParameter.Get();
                    param.intParameter = skillDesc.Get<int>("showtype");
                    param.stringParameter = skillDesc.Get<string>("name");
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FLY_SKILL, param);
                }
            }
        }

        public override void FrameUpdate()
        {
            //接受事件，打断施法 
        }


        public void BreakSkill(StateParameter stateParm)
        {
            CancelInvoke("StartEndure");
            EndEndure();

            CancelInvoke("AutoCastNextSkill");
            CancelInvoke("SkillCastEnd");

            CancelInvoke("SkillSetAfterAttack");
            CancelInvoke("EndSkillAfterAttack");

            m_curSkillID = 0;
            m_bAfterAttack = false;

            //技能结束时间
            m_lastestEndTime = Time.time;

            //如果非正常结束
            if (m_curSkillBase != null && m_curSkillBase.CanBeBroke)
            {
                m_curSkillBase.BreakSkill(stateParm);
                m_curSkillBase = null;
            }

            // 删除预警贴花
            if (m_actor != null && m_actor.m_WarningefxObj != null)
            {
                Destroy(m_actor.m_WarningefxObj);
                m_actor.m_WarningefxObj = null;
            }
        }


        public void ExitAttack()
        {
            CancelInvoke("ExitAttack");

            //if (m_actor != null)
            //{
            //    m_actor.m_bIsInAttack = false;
            //}
        }

        public override void OnExit(StateParameter stateParm)
        {
            //3秒后退出战斗
            Invoke("ExitAttack", 5);

            CancelInvoke("StartEndure");
            if (m_isEndure == true)
            {
                EndEndure();
            }

            CancelInvoke("AutoCastNextSkill");
            CancelInvoke("SkillCastEnd");

            CancelInvoke("SkillSetAfterAttack");
            CancelInvoke("EndSkillAfterAttack");

            m_curSkillID = 0;
            m_bAfterAttack = false;

            //技能结束时间
            m_lastestEndTime = Time.time;

            //如果非正常结束
            if (m_curSkillBase != null && m_curSkillBase.CanBreakSkill(stateParm))
            {
                m_curSkillBase.BreakSkill(stateParm);
                m_curSkillBase = null;
            }

            // 删除预警贴花
            if (m_actor.m_WarningefxObj != null)
            {
                Destroy(m_actor.m_WarningefxObj);
                m_actor.m_WarningefxObj = null;
            }
        }

        //是否能继续攻击
        public bool CanChangeAttack()
        {
            return !m_isEndure;
        }
        
        public override bool CanChangeState(StateParameter stateParm)
        {
            if (m_actor == null) return false;
            if (m_curSkillDesc == null) return false;
            if (stateParm.state == ACTOR_STATE.AS_DEATH)
            {
                return true;
            }

            if (stateParm.state == ACTOR_STATE.AS_RUN)
            {
                //判断是不是旋风斩
                if (m_curSkillDesc.Get<int>("subtype") == (int)SkillType.SKILL_XUANFANZHAN)
                {
                    return true;

                }
            }

            //外部状态切换
            if (stateParm.state != ACTOR_STATE.AS_ATTACK && stateParm.state != ACTOR_STATE.AS_BEHIT)
            {
                return !m_isEndure;
            }

            //被击状态
            if (stateParm.state == ACTOR_STATE.AS_BEHIT)
            {
                if (m_actor.mActorType == ActorType.AT_MONSTER)
                {
                    if (stateParm.IsHitBack)
                    {
                        return true;
                    }

                    if (null != m_curSkillDesc )
                    {
                        if( m_curSkillDesc.Get<bool>("stop_casting"))
                        return true;
                    }

                    return false;
                }

                //前摇时间0.1秒，不可被打断，被神将的520601打断
                float runTime = m_actor.GetCurActionTime(m_actor.GetCurPlayAction());
                if (runTime < 0.1f && m_actor.mActorType != ActorType.AT_MONSTER) // 怪物攻击可以被打断
                {
                    return false;
                }

                //如果是BOSS，并且处于气绝状态
                if (m_actor.mActorType == ActorType.AT_BOSS && m_actor.IsInQiJue())
                {
                    return true;
                }

                //霸体了,且不是破霸体技能
                if (m_isEndure)
                {
                    return false;
                }

                return false;
            }

            //子技能可以切换
            if (m_curSkillBase != null && m_curSkillBase.IsSubSkill(stateParm.skillID))
            {
                return true;
            }

            //yy 如果是后摇状态可以切换
            if (m_bAfterAttack && stateParm.state == ACTOR_STATE.AS_ATTACK)
            {
                return true;
            }

            //内部状态切换
            if (stateParm.isComposeSkill)
            {
                //组合技能                
                return m_actor.ComposeSkillCanCastSkill(stateParm.composeSkillDesc.changeTime);
            }

            //普通技能切换，看霸体时间结束没
            if (null != m_actor && m_actor.mActorType != ActorType.AT_LOCAL_PLAYER)
            {
                return true;
            }
            return !m_isEndure;
        }

        public override bool CanMove()
        {
            //当前技能能接受攻击位移        
            if (m_curSkillBase == null)
            {
                return false;
            }

            return m_curSkillBase.CanMove();
        }

        //是否组合技能间隔
        public bool IsUseComposeSkill()
        {
            return Time.time - m_lastestEndTime < 0.1f;
        }

        //设置后摇时间
        void SkillSetAfterAttack()
        {
            CancelInvoke("SkillSetAfterAttack");
        }

        void EndSkillAfterAttack()
        {
            CancelInvoke("EndSkillAfterAttack");
            m_bAfterAttack = false;
        }
        
        void StartEndure()
        {
            m_isEndure = true;
            float keepTime = m_curSkillDesc.Get<int>("stiff_time") / 1000.0f;
            //不设置霸体，采用动作时间
            if (keepTime <= 0)
            {
                keepTime = m_actor.GetActionLength(CoreEntry.gSkillMgr.GetSkillActon(m_curSkillID).Get<string>("animation")) - 0.05f;
            }

            keepTime /= m_curSkillBase.m_speed;
            Invoke("EndEndure", keepTime);
        }

        void EndEndure()
        {
            CancelInvoke("EndEndure");
            m_isEndure = false;
        }

        void SkillCastEnd()
        {
            CancelInvoke("SkillCastEnd");

            //m_isSkillCastEnd = true;
        }
    }
}

