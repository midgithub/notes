/**
* @file     : ActorAgent.cs
* @brief    : AI编辑器使用的接口
* @details  : 生物行为树代理基类，一些相同的操作放到这里
* @author   : 
* @date     : 2014-9-24 16:57
 *           修改YY
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{

    [behaviac.TypeMetaInfo("ActorAgent", "")]
[Hotfix]
    public class ActorAgent : behaviac.Agent
    {
        protected ActorObj m_actorObject = null;
        protected ActorObj m_actor = null;
        //protected GameLogicMgr m_gameMgr = null;
        //protected EventMgr m_eventMgr = null;
        protected Transform m_transform = null;
        protected Vector3 m_movePos = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
        //private BaseTool m_baseTool = null;

        public bool bStartFlee = false;
        //private int nSetFleePos = 0;
        protected Vector3 m_FleeStartPos = Vector3.zero;

        public int m_nFleeCount = 0;
        //private int m_nFleeNum = 4;

        private Vector3 m_AutoNavPos = Vector3.zero;

        public bool m_bHitDown = false;

        public bool m_bNeedRandom = true;
        protected float m_fBattleBeginTime = 0.0f;  // 战斗开始时间

        protected int m_nCurWayPointIndex = 0;

        public ActorObj m_AttackObj = null;        //攻击对象

        public int m_NpcMoveIndex = 0;

        protected float fMonsterMinDist = 2.3f;

        protected int nSkillIndex = 0;

        //protected AIEvent m_AIEventSkill = null;
        protected int m_AIEventObjID = 0;

        //private bool m_bStartEscape = false;
        //public bool m_bTriggerBoss = false;
        //public bool m_bGoHome = false;
        //public bool m_bHasTower = false;

        //public ActorObj m_TowerObj = null;        //攻击对象
        //bool isFirstTime = true;


        //继承的时候，需要调用
        protected override void Init()
        {
            base.Init();
        }

        protected void InitData()
        {
            m_actorObject = this.gameObject.GetComponent<ActorObj>();
            m_actor = m_actorObject;
            //isFirstTime = true;

            //m_gameMgr = CoreEntry.gGameMgr;
            //m_eventMgr = CoreEntry.gEventMgr;
            //m_baseTool = CoreEntry.gBaseTool;

            m_transform = this.transform;
        }

        protected bool BattleBeginTimeIsValid()
        {
            return m_fBattleBeginTime > 0.01f;
        }

        [behaviac.MethodMetaInfo("是否死亡", "is die")]
        public bool IsDeath()
        {
            //对战不能死亡
            if (CoreEntry.IsMobaGamePlay())
            {
                return false;
            }

            if (m_actor == null)
            {
                return true;
            }
            return m_actor.IsDeath();
        }
        
        //判断是否进行攻击
        [behaviac.MethodMetaInfo("是否进行攻击", " ")]
        public behaviac.EBTStatus CanAttack(ActorType aimActorType)
        {
            //   return behaviac.EBTStatus.BT_SUCCESS;

            if (!m_actor.CanChangeAttack() || m_actor.curActorState == ACTOR_STATE.AS_RUN)
            {
                return behaviac.EBTStatus.BT_FAILURE;
            }

            //当前是否硬直状态
            if (m_actor.IsNonControl || m_actor.IsActorEndure())
            {
                return behaviac.EBTStatus.BT_FAILURE;
            }


            //增加攻击对象
            m_AttackObj = m_actor.GetSelTarget();
            if (m_AttackObj)
            {
                return behaviac.EBTStatus.BT_SUCCESS;
            }
            else
            {
                m_AttackObj = m_actor.AutoSelTarget(10);
                if (m_AttackObj)
                {
                    m_actor.SelectTarget(m_AttackObj);
                    return behaviac.EBTStatus.BT_SUCCESS;
                }
            }
            return behaviac.EBTStatus.BT_FAILURE;
        }
        
        //跟随
        [behaviac.MethodMetaInfo("跟随主玩家", " ")]
        public behaviac.EBTStatus FollowPlayer()
        {
            //宝宝这个地方要修改
            Vector3 vNavPos = CoreEntry.gActorMgr.MainPlayer.transform.position;
            Vector3 vDir = CoreEntry.gActorMgr.MainPlayer.transform.forward;
            vDir = -vDir;

            Vector3 vTmp1 = m_transform.position;
            vTmp1.y = 0;
            Vector3 vTmp2 = vNavPos;
            vTmp2.y = 0;
            float distance = Vector3.Distance(vTmp1, vTmp2);

            //是否需要区分BOSS战和普通小怪
            if ((m_actor.curActorState == ACTOR_STATE.AS_ATTACK || m_AttackObj != null) && distance < 8f)
            {
                return behaviac.EBTStatus.BT_SUCCESS;
            }
            else
            {
                if (distance < 4.5f)
                {
                    return behaviac.EBTStatus.BT_SUCCESS;
                }

            }

            //重新计算寻路位置
            Quaternion qRoate = Quaternion.AngleAxis(30, Vector3.up);  // - aiInfo.nAngle
            Vector3 vNewDir = qRoate * vDir * 3.0f;
            vNewDir = vNewDir + vNavPos;

            vNavPos = vNewDir;

            //增加个判断，如果跟班和主角距离太远，则直接拉
            float fDistance = Vector3.Distance(m_transform.position, CoreEntry.gActorMgr.MainPlayer.transform.position);
            if (fDistance > 20 && m_actor.curActorState != ACTOR_STATE.AS_ATTACK)
            {
                m_transform.position = vNavPos;
            }

            if (distance < 2f)
            {
                StateParameter param = new StateParameter();
                param.state = ACTOR_STATE.AS_STAND;

                m_actor.RequestChangeState(param);

                return behaviac.EBTStatus.BT_SUCCESS;
            }

            //目标没改变，且run状态，不处理
            if (m_movePos.Equals(vNavPos) && m_actor.curActorState == ACTOR_STATE.AS_RUN)
            {
                return behaviac.EBTStatus.BT_RUNNING;
            }

            if (m_movePos.Equals(vNavPos) && m_actor.curActorState == ACTOR_STATE.AS_STAND && distance < 0.1f)
            {

                return behaviac.EBTStatus.BT_SUCCESS;
            }

            m_actor.FaceTo(vNavPos);

            if (m_actorObject.MoveToPos(vNavPos))
            {
                m_movePos = vNavPos;
            }

            return behaviac.EBTStatus.BT_RUNNING;
        }


        //移动到攻击位置            
        [behaviac.MethodMetaInfo("主角移动到合适位置", "")]
        public behaviac.EBTStatus PlayerGoToPos()
        {
            return behaviac.EBTStatus.BT_SUCCESS;
        }

        /// <summary>
        /// 攻击超时计时器
        /// </summary>
        float StartAttackTimer = 0;

        [behaviac.MethodMetaInfo("设置攻击目标", "")]
        public behaviac.EBTStatus SetAttackTarget(ActorType aimActorType, int nSkillID)
        {
            StartAttackTimer = 0;

            if (nSkillID == 0)
            {
                return behaviac.EBTStatus.BT_FAILURE;
            }

            if (m_AttackObj == null)
            {
                return behaviac.EBTStatus.BT_SUCCESS;
            }

            //m_actor.SkillIDWillToDo = nSkillID;


            //增加个判断，判断是选择友方还是敌方。
            LuaTable skillDesc = m_actor.GetCurSkillDesc(nSkillID);
            if (skillDesc == null)
                return behaviac.EBTStatus.BT_FAILURE;


            //判断是否要切换目标
            bool bChangeTarget = false;


            //如果为魅惑则强制切换目标
            if (m_actor.m_bCharmState)
            {
                bChangeTarget = true;
            }

            //如果目标隐身则强制切换目标
            if (m_AttackObj.IsInStealthState(m_actor))
            {
                bChangeTarget = true;
            }

            if (bChangeTarget)
            {
                m_AttackObj = m_actor.GetSelTarget();
            }

            //如果存在攻击目标
            if (m_AttackObj != null)
            {
                //目标死亡
                if (m_AttackObj.IsDeath())
                {
                    m_actor.SelectTarget(null);
                    return behaviac.EBTStatus.BT_FAILURE;
                }
                return behaviac.EBTStatus.BT_SUCCESS;
            }

            return behaviac.EBTStatus.BT_SUCCESS;
        }


        Vector3 LastPosition = Vector3.zero;
        int LastSkillID = -1;
        ///// <summary>
        ///// 标记上次是否攻击失败
        ///// </summary>
        //bool IsFailToAttack = false;
        //移动到攻击位置            
        [behaviac.MethodMetaInfo("主角移动到攻击位置", "")]
        public behaviac.EBTStatus PlayerRunToAttackPos(int nSkillID)
        {
            //连招中
            if (nSkillIndex > 1)
                return behaviac.EBTStatus.BT_SUCCESS;


            //攻击目标失效，重新查找
            if (m_AttackObj == null || m_AttackObj.IsDeath() || m_AttackObj.IsInStealthState(m_actor))
            {
                return behaviac.EBTStatus.BT_SUCCESS;
                //return behaviac.EBTStatus.BT_FAILURE;
            }

            Vector3 vNavPos = m_AttackObj.transform.position;

            float fDist = m_actor.GetSkillAttackDist(nSkillID, m_AttackObj);

            float distance = Vector3.Distance(m_transform.position, m_AttackObj.transform.position);

            //bool bAttack = false;



            //增加个判断，判断和攻击对象的距离
            if (distance < fDist)
            {
                //判断和队长的距离
                return behaviac.EBTStatus.BT_SUCCESS;

            }


            //计算位置
            //Vector3 vDir = Vector3.Normalize(m_AttackObj.transform.position - m_actor.transform.position);

            //vDir.y = 0; 　

            Vector3 vTmp1 = m_transform.position;
            vTmp1.y = 0;
            Vector3 vTmp2 = vNavPos;
            vTmp2.y = 0;
            float fCurDist = Vector3.Distance(vTmp1, vTmp2);


            //目标离自己很近。不需要跑        
            if (fCurDist < 0.1f)
            {

                return behaviac.EBTStatus.BT_SUCCESS;
            }


            //攻击超时处理
            //如果在同一个地方几乎不移动,并且想释放的技能也没有变，而没有跳出该状态，累计超时时间
            const float MaxAttackTime = 0.5f;
            Vector3 dis = m_actorObject.transform.position - LastPosition;
            //Debug.LogWarning(dis.sqrMagnitude + " " + Time.time);
            if (dis.sqrMagnitude < 0.00001 && LastSkillID == nSkillID)
            {
                StartAttackTimer += Time.deltaTime;
                //Debug.LogWarning(StartAttackTimer + " " + Time.time);
                if (StartAttackTimer > MaxAttackTime)
                {
                    //IsFailToAttack = true;
                    return behaviac.EBTStatus.BT_FAILURE;
                }
            }
            else
            {
                StartAttackTimer = 0;
            }

            LastPosition = m_actorObject.transform.position;
            LastSkillID = nSkillID;

            //目标没改变，且run状态，不处理
            if (m_movePos.Equals(vNavPos) && m_actor.curActorState == ACTOR_STATE.AS_RUN)
            {
                return behaviac.EBTStatus.BT_RUNNING;
            }

            m_actor.FaceTo(vNavPos);

            if (m_actorObject.MoveToPos(vNavPos))
            {
                m_movePos = vNavPos;
            }


            return behaviac.EBTStatus.BT_RUNNING;
        }

        /// <summary>
        /// 寻路相关超时
        /// </summary>
        float StartRunningTimer = 0f;

        bool IsRunningTimeOut()
        {
            const float MaxRunningTime = 2.0f;
            Vector3 dis = m_actorObject.transform.position - LastPosition;
            if (dis.sqrMagnitude < 0.00001)
            {
                StartRunningTimer += Time.deltaTime;
                if (StartRunningTimer > MaxRunningTime)
                {
                    StartRunningTimer = 0;
                    return true;
                }
            }
            else
            {
                StartRunningTimer = 0;
            }

            LastPosition = m_actorObject.transform.position;

            return false;
        }

        //主角自动寻路        
        [behaviac.MethodMetaInfo("主角自动寻路", "")]
        public behaviac.EBTStatus PlayerAutoMove()
        {

            if (!CoreEntry.GameStart || (CoreEntry.IsMobaGamePlay()))
            {
                return behaviac.EBTStatus.BT_FAILURE;
            }

            //if (CoreEntry.gGameMgr.AutoFight)
            //{
            //    if (CoreEntry.gActorMgr.MainPlayer.m_bUseAI == false)
            //    {
            //        return behaviac.EBTStatus.BT_FAILURE;
            //    }
            //}


            ActorObj playerObj = m_actor;

            float distance = 0.0f;
            m_AttackObj = m_actor.GetAttackObj();
            if (m_AttackObj)
            {
                distance = Vector3.Distance(m_transform.position, m_AttackObj.transform.position);
                if (distance < fMonsterMinDist)
                {
                    playerObj.StopMove(true);

                    return behaviac.EBTStatus.BT_SUCCESS;
                }
            }

            if (!ArenaMgr.Instance.IsArenaFight)
            {
                if (m_AttackObj)
                {
                    playerObj.StopMove(true);

                    nSkillIndex = 0;

                    return behaviac.EBTStatus.BT_FAILURE;
                }
            }
            else
            {
                if (m_AttackObj)
                {
                    m_AutoNavPos = m_AttackObj.transform.position;
                }
            }

            //主角寻路超时设定，防止后面停止不动
            if (IsRunningTimeOut())
            {
                Debug.LogWarning("Main Player is running time out...");
                return behaviac.EBTStatus.BT_FAILURE;

            }

            //目标没改变，且run状态，不处理
            if (m_movePos.Equals(m_AutoNavPos) && m_actor.curActorState == ACTOR_STATE.AS_RUN)
            {
                return behaviac.EBTStatus.BT_RUNNING;
            }

            m_actor.FaceTo(m_AutoNavPos);

            if (m_actorObject.MoveToPos(m_AutoNavPos))
            {
                m_movePos = m_AutoNavPos;
            }

            return behaviac.EBTStatus.BT_RUNNING;


        }



        //private bool bMoveEnd = false;

        /// <summary>
        /// 主控玩家是否释放特殊技能开关
        /// </summary>
        bool ShouldUseSpecialSkillAsMainPlayer = true;


        bool CanCastSpecialSkill()
        {
            return ShouldUseSpecialSkillAsMainPlayer == true || m_actor.IsMainPlayer() == false;
        }

        //攻击技能选择
        [behaviac.MethodMetaInfo("选择攻击技能Old", " ")]
        public int ChooseSkillOld()
        {

            int nSkillID = -1;

            if (ArenaMgr.Instance.IsArenaFight)
            {
                nSkillID = ArenaMgr.Instance.ChooseSkillID(m_actor);
            }
            else
            {
                PlayerObj player = m_actor as PlayerObj;
                int nPriority = 0;
                if (player == null) return nSkillID;
                //优先使用技能
                foreach (KeyValuePair<int, int> e in player.m_skillBindsDict)
                {
                    if (player.IsInCoolDownTime(e.Value))
                    {
                        continue;
                    }

                    //选择优先级最高的
                    LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(e.Value);
                    if (skillDesc != null)
                    {
                        if (skillDesc.Get<int>("priority") > nPriority)
                        {
                            nPriority = skillDesc.Get<int>("priority");
                            nSkillID = e.Value;
                        }
                    }
                }
            }

            //再使用普通技能
            if (nSkillID == -1)
            {
                nSkillID = PickUpANormalAttack();
            }

            return nSkillID;


        }

        //攻击技能选择
        [behaviac.MethodMetaInfo("选择攻击技能", " ")]
        public int ChooseSkill()
        {
            Debug.LogError("===选择攻击技能==");
            return ChooseSkillOld();
        }

        /// <summary>
        /// 新的方法获取一个普通攻击
        /// </summary>
        int normalAttackCounter = 0;
        protected int PickUpANormalAttack()
        {
            int skillID = m_actor.GetNextNormalAttack(ref normalAttackCounter); ;
            return skillID;
        }




        //查询当前还有多久技能CD冷却，单位ms
        [behaviac.MethodMetaInfo("查询技能冷却时间", "")]
        public float QuerySkillCDOverTime(int nSkillID)
        {
            float overTime = m_actor.QuerySkillCDOverTime(nSkillID) + 100f; //增加100ms，防止边界情况
                                                                            //LogMgr.UnityLog("cool down time : " + overTime.ToString());
            return overTime;
        }
        
        //释放技能
        [behaviac.MethodMetaInfo("释放技能", "")]
        public behaviac.EBTStatus CastSkill(int nSkillID)
        {
            if (!m_actor.CanCastSkill(nSkillID))
            {
                return behaviac.EBTStatus.BT_FAILURE;
            }

            EventParameter param = EventParameter.Get();
            param.goParameter = this.gameObject;
            param.intParameter = nSkillID; //m_actor.actorCreatureInfo.iNormalAttID;

            ActorObj actorObject = this.gameObject.GetComponent<ActorObj>();
            actorObject.StopMove(true);

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_NOTIFY_CAST_SKILL, param);

            return behaviac.EBTStatus.BT_SUCCESS;
        }


        //技能是否结束
        [behaviac.MethodMetaInfo("技能结束", "")]
        public bool IsCastSkillOver(int nSkillID)
        {
            if (m_actor.curActorState == ACTOR_STATE.AS_ATTACK)
            {
                if (m_actor.GetComponent<Animation>().isPlaying == false && m_actor.m_bIsTower == false)
                {
                    LogMgr.UnityWarning("IsCastSkillOver.m_actor.animation.isPlaying == false :" + m_actor);
                    //如果在攻击状态没有动作，则视为技能施放完毕
                    return true;
                }
            }
            else
            {
                return true;
            }

            m_bNeedRandom = true;

            //判断连招
            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_ATTACK;
            param.skillID = nSkillID;
            param.isComposeSkill = false;

            //是否组合技能              
            ComposeSkillDesc composeSkillDesc = m_actor.m_gameDBMgr.GetComposeSkillDesc(m_actor.curCastSkillID, nSkillID);
            if (composeSkillDesc != null)
            {
                param.isComposeSkill = true;
                param.composeSkillDesc = composeSkillDesc;
            }

            if (param.isComposeSkill)
            {
                bool bRet = m_actor.ComposeSkillCanCastSkill(param.composeSkillDesc.changeTime);
                return bRet;
            }

            return m_actor.curActorState != ACTOR_STATE.AS_ATTACK;
        }

        [behaviac.MethodMetaInfo("待机动画", "player stand")]
        public behaviac.EBTStatus Stand()
        {

            if (m_actor.IsMainPlayer())
            {
                if (CoreEntry.gActorMgr.MainPlayer.m_bUseAI == false)
                {
                    return behaviac.EBTStatus.BT_FAILURE;
                }
            }

            if (m_actor.IsNonControl || m_actor.IsHitDownState())
            {
                return behaviac.EBTStatus.BT_FAILURE;
            }



            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_STAND;

            m_actor.RequestChangeState(param);

            return behaviac.EBTStatus.BT_SUCCESS;
        }


    }

};  //end SG

