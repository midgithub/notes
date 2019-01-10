using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{

[Hotfix]
    public class BehitState : IActorState
    {

        private int m_curSkillID;
        //private GameDBMgr m_GameDataBase;

        //private BaseTool m_baseTool;

        private ActorObj m_attack;

        public ActorObj AttackerActor
        {
            get { return m_attack; }
            //set { m_hitActorBase = value; }
        }
        private Transform m_transform;

        public float exitStateTime = 0;

        //击退参数
        public bool m_bHitBack = false;
        //private Vector3 m_backAimPos;
        private float m_backTime;
        public Vector3 m_backSrcPos = Vector3.zero;
        public float m_backBeginTime = 0;
        public float m_backSpeed = 10;
        public Vector3 m_backDir = Vector3.zero;
        public float m_backDistance = 0;

        private float m_curRadius = 0;

        //当前动作    
        private string m_curPlayAction = null;

        //地表层
        public int m_groundLayerMask;

        //private Vector3 m_wallAimPos = Vector3.zero;

        //生成的动作特效文件
        //private List<EfxAttachAction> m_ActionEfxList;

        public Vector3 m_raypos = Vector3.zero;

[Hotfix]
        public class ActionAndTime
        {
            public string strAction;
            public float needTime;
        };

        //被击信息，动作，硬直时间，击退，击飞距离
[Hotfix]
        public class BehitInfo
        {
            public ActionAndTime[] strActionArry;
            public float NonControlTime;
            public float hitDistance;

            //小退位移
            public float moveDistance;
            public float moveTime;

            //退出被击状态时间
            public float exitStateTime;

            //是否浮空
            public bool isInSky;
        };

        ////动作组
        //private List<ActionAndTime> m_listAction;

        //被击信息
        public BehitParam m_behitParame = null;

        //悬空阶段
        private GravityMotionBase m_gravityMotionBase = null;

        //倒地状态
        public bool m_isHitDownState = false;

        int m_uuid = -1;

        //硬直设置
        public bool isNonControl
        {
            set { m_isNonControl = value; }
        }

        public bool isNonControlProtect
        {
            set { m_isNonControlProtect = value; }
        }

        MonsterObj monsterObj = null;
        public override void OnEnter(ActorObj actor)
        {
            if (actor == null)
                return;

            //塔不没有受击
            bool bChangeColor = true;

            m_state = ACTOR_STATE.AS_BEHIT;

            m_actor = actor;
            m_transform = actor.transform;

            //if (m_actor != null)
            //{
            //    m_actor.m_bIsInAttack = true;
            //}

            m_behitParame = m_actor.damageBebitParam;

            monsterObj = m_actor as MonsterObj;
            if (monsterObj != null)
            {
                if (monsterObj.IsVip)
                {
                    monsterObj.VipIsUnderAttack();
                }
            }

            if (CurParam != null && CurParam != null)
                m_curSkillID = CurParam.skillID;

            //m_GameDataBase = CoreEntry.gGameDBMgr;

            if (CurParam != null && CurParam.AttackActor != null)
            {
                m_attack = CurParam.AttackActor;
            }


            m_groundLayerMask = 1 << LayerMask.NameToLayer("ground");


            

            //m_baseTool = CoreEntry.gBaseTool;

            m_gravityMotionBase = this.gameObject.GetComponent<GravityMotionBase>();

            //m_gravityMotionBase.enabled = true;

            //m_ActionEfxList = new List<EfxAttachAction>();

            //修改shader
            //if (m_actor.ChangeShader("Mobile/BeingHitted"))


            if (monsterObj && monsterObj.m_bIsTower)
            {
                bChangeColor = false;
            }

            if (bChangeColor && m_actor.m_bUseBehitColor && m_actor.mActorType == ActorType.AT_MONSTER 
                && m_attack != null && m_attack is PlayerObj)                                 
            {
                m_actor.SetBrightenShader();

                Invoke("RecoverShader", 0.2f);
            }

            //  if (m_actor.mActorType == ActorType.AT_BOSS && m_actor.AddShader("DZSMobile/Balloon"))
            ////  if (m_actor.mActorType == ActorType.AT_BOSS && m_actor.ChangeShader("DZSMobile/Balloon"))
            //  {
            //      Invoke("RemoveShader", 0.2f);
            //  }

            if (m_actor.mActorType == ActorType.AT_BOSS && m_actor.IsInQiJue())
            {
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_Boss_QiJue, null);

            }


            m_curRadius = m_actor.GetColliderRadius();

            //如果浮空，就给浮空处理受击
            if (m_gravityMotionBase != null && m_gravityMotionBase.isUseGravityState)
            {
                m_gravityMotionBase.DoBehit();
            }
            else
            {
                m_bHitBack = false;
                m_isNonControlProtect = false;
                isNonControl = false;
                m_isHitDownState = false;

                //小怪倒地被击
                if (m_gravityMotionBase != null)
                {
                    m_gravityMotionBase.BreakExitBehitState();
                }

                //面向被击玩家
                // if (m_actor.actorCreatureDisplayDesc.chIsBehitNotLookAtTarget == 0)
                if (m_actor.mActorType != ActorType.AT_LOCAL_PLAYER 
                    && m_actor.mActorType != ActorType.AT_MECHANICS //机械类不转向
                    && m_attack != null
                    && !(m_actor is MonsterObj && !m_actor.bHitTurn))
                {
                    //自动释放法宝技能
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_AutoUseMagicKeySkill, null);
                    //只旋转Y轴
                    Vector3 lookPos = new Vector3(m_attack.transform.position.x,
                        m_transform.position.y, m_attack.transform.position.z);

                    m_transform.LookAt(lookPos);
                }

                DoBehitEx();
            }
        }


        public override void FrameUpdate()
        {
            if(null == m_transform)
            {
                return;
            }
            //被击带有位移
            if (m_bHitBack)
            {
                float curTime = Time.time;
                float distance = m_backSpeed * (curTime - m_backBeginTime);
                if (distance >= m_backDistance)
                {
                    distance = m_backDistance;
                    m_bHitBack = false;
                }

                Vector3 nextPos = m_backSrcPos + m_backDir.normalized * distance;
                Vector3 curPos = m_transform.position;

                Vector3 groundPos = BaseTool.instance.GetGroundPoint(nextPos);
                if (groundPos == Vector3.zero)
                {
                    m_bHitBack = false;
                    return;
                }

                //是否有阻挡
                if (BaseTool.instance.CanMoveToPos(curPos, groundPos, m_curRadius))
                {
                    m_transform.position = groundPos;
                }
                else
                {
                    m_bHitBack = false;
                    return;
                }
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
            m_isNonControlProtect = false;
            isNonControl = false;


            //3秒后退出战斗
            Invoke("ExitAttack", 5);

            if (m_actor == null)
                return;


            //m_actor.StopAll();
            //m_actor.StopAction(m_curPlayAction);    

            //         for (int i = 0; i < m_ActionEfxList.Count; ++i)
            //         {            
            //             if (m_ActionEfxList[i] != null)
            //             {
            //                 m_ActionEfxList[i].DeathObject();
            //             }
            //         }

            CancelInvoke("ExitBehitState");


            //位移曲线
            m_actor.ExitAnimationCurveMove(m_uuid);

            // m_actor.ExitAnimationCurveMove();

            m_actor.StopSound();

            // m_gravityMotionBase.enabled = false;
        }

        public override bool CanChangeState(StateParameter stateParm)
        {
            if (stateParm.state == ACTOR_STATE.AS_DEATH)
            {
                return true;
            }



            //外部状态切换
            if (stateParm.state != ACTOR_STATE.AS_BEHIT)
            {
                return !m_isNonControl;
            }

            //硬直保护
            if (m_isNonControlProtect)
            {
                return false;
            }

            //内部状态切换               
            return true;
        }

        void DoBehitEx()
        {
            BehitInfo info = GetBeHitInfo(m_actor.resid, m_curSkillID);

            if (m_behitParame != null)
            {
                //buff伤害不产生位移效果
                if (m_behitParame.displayType == DamageDisplayType.DDT_BUFF)
                {
                    return;
                }

                //声音
                if (m_behitParame != null)
                {
                    LuaTable desc = ConfigManager.Instance.Skill.GetSkillConfig(m_curSkillID);
                    PlayBehitSound(m_behitParame.damgageInfo.behitActor, m_behitParame.damgageInfo.attackActor, desc);
                }


            }


            //使用动作时间
            exitStateTime = m_actor.GetActionLength("hit001");
            m_actor.StopAll();
            m_actor.PlayAction("hit001", false);

            if (m_actor.mActorType == ActorType.AT_MONSTER)
            {
                m_actor.StopMove(true);
            }
            //      PlayBehitSound(m_behitParame.damgageInfo.behitActor, m_behitParame.damgageInfo.attackActor, desc);

            //没有硬直
            if (exitStateTime <= 0)
            {

                Invoke("ExitBehitState", 0.1f);
                return;
            }

            //2,硬直处理
            isNonControl = false;
            //主角
            //if (info.NonControlTime > 0.001f)
            //{
            //    isNonControl = true;
            //    Invoke("NonControlEnd", info.NonControlTime);
            //}

            //BOSS
            //if (info.NonControlTime == 0f && m_actor.mActorType == ActorType.AT_BOSS && m_actor.IsInQiJue())
            //{
            //    isNonControl = true;
            //    Invoke("NonControlEnd", 0.3f);
            //}



            //3, 位移    
            if (info.hitDistance > 0.01f)
            {

                //曲线位移
                OnBehitCurve(m_curPlayAction, info.hitDistance);
            }
            //else if (info.moveDistance > 0.001f)
            //{
            //    //程序位移
            //    OnBehitMove(info.moveDistance, info.moveTime);
            //}

            //4,退出
            if (exitStateTime > 0.001f)
            {
                Invoke("ExitBehitState", exitStateTime);
            }
            else
            {
                ExitBehitState();
            }

            //5,被击特效
            //OnPlayBehitEfx();      
        }

        //private string m_strHitAction = "";

        //void PauseGame()
        //{
        //    m_actor.SetActionSpeed(m_curPlayAction, 0);
        //    m_strHitAction = m_hitActorBase.GetCurPlayAction();
        //    m_hitActorBase.SetActionSpeed(m_strHitAction, 0);

        //    Invoke("PauseGameEnd", 0.2f);
        //}

        //void PauseGameEnd()
        //{
        //    m_actor.SetActionSpeed(m_curPlayAction, 1);
        //    m_hitActorBase.SetActionSpeed(m_strHitAction, 1);
        //}

        void PlayNextAction()
        {
            CancelInvoke("PlayNextAction");

            //if (m_listAction.Count > 0)
            //{
            //    m_curPlayAction = m_listAction[0].strAction;
            //    float needTime = m_listAction[0].needTime;
            //    m_listAction.RemoveAt(0);

            //    m_actor.SetActionSpeed(m_curPlayAction, 1f);
            //    m_actor.PlayAction(m_curPlayAction, true);
            //    Invoke("PlayNextAction", needTime);
            //}
        }

        void NonControlEnd()
        {
            CancelInvoke("NonControlEnd");

            //
            if (m_actor.mActorType == ActorType.AT_LOCAL_PLAYER)
            {
                isNonControl = false;
            }
        }

        public void ExitBehitState()
        {
            CancelInvoke("ExitBehitState");

            //取消硬直
            isNonControl = false;
            m_isNonControlProtect = false;

            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_STAND;

            if (m_actor != null)
            {
                //要判断是否屏蔽本地死亡
                if ((m_actor.IsUseServerDeath() == false) && m_actor.IsDeath())
                {
                    param.state = ACTOR_STATE.AS_DEATH;
                }

                m_actor.RequestChangeState(param);
            }
        }



        //曲线位移
        void OnBehitCurve(string curPlayAction, float hitDistance)
        {
            //是否飞到空气墙内
            //Vector3 backDir = -m_transform.forward;

            //yy 使用人物朝向
            //Vector3 backDir = m_attack.transform.forward;

            //Vector3 pos = m_transform.position + backDir.normalized * hitDistance + Vector3.up * 5;

            //Vector3 backAimPos = Vector3.zero;
            float backDistance = hitDistance;

            ////垂直向下发射射线，找到对应的目标点
            //RaycastHit beHit;
            //if (Physics.Raycast(pos, -Vector3.up, out beHit, 10, m_groundLayerMask))
            //{
            //    backAimPos = beHit.point;
            //}
            //else
            //{
            //    backAimPos = m_transform.position + backDir.normalized * hitDistance;
            //}

            //m_backAimPos = backAimPos;


            if (backDistance < 0)
            {
                return;
            }


            m_actor.UseCurveData1(curPlayAction, backDistance, true);

            //直接调用曲线位移
            // m_uuid = m_actor.UseCurveData(curPlayAction, Vector3.zero, null);   
        }

        //被击位移
        public float OnBehitMove(float backDistance, float backTime)
        {
            //位移为0，直接退出
            if (backDistance <= 0)
            {
                return 0;
            }

            m_backSrcPos = m_transform.position;

            // Vector3 backDir = -m_transform.forward;

            //yy 使用人物朝向
            Vector3 backDir = m_attack.transform.forward;


            Vector3 pos = m_transform.position + backDir.normalized * backDistance + Vector3.up * 5;

            m_raypos = pos;

            ////垂直向下发射射线，找到对应的目标点
            //RaycastHit beHit;

            //if (Physics.Raycast(pos, -Vector3.up, out beHit, 10, m_groundLayerMask))
            //{
            //    m_backAimPos = beHit.point;
            //}
            //else
            //{
            //    m_backAimPos = m_transform.position + backDir.normalized * backDistance;
            //}


            m_bHitBack = true;
            m_backBeginTime = Time.time;
            m_backTime = backTime;

            return m_backTime;
        }

        public float OnBehitMove(float backDistance, float backTime, Vector3 vDest, Vector3 vDir, int nSpeed)
        {
            //位移为0，直接退出
            if (backDistance <= 0 || m_actor == null)
            {
                return 0;
            }

            m_backDistance = backDistance;

            m_backSrcPos = m_actor.transform.position;
            //m_backAimPos = vDest;

            m_bHitBack = true;
            m_backBeginTime = Time.time;
            m_backTime = backTime;
            m_backDir = vDir;

            m_backSpeed = nSpeed;


            return m_backTime;
        }


        public void UpdateHitMove()
        {
            //被击带有位移
            if (m_bHitBack)
            {
                float curTime = Time.time;
                float distance = m_backSpeed * (curTime - m_backBeginTime);
                if (distance >= m_backDistance)
                {
                    distance = m_backDistance;
                    m_bHitBack = false;
                }

                Vector3 nextPos = m_backSrcPos + m_backDir.normalized * distance;
                Vector3 curPos = m_transform.position;

                Vector3 groundPos = BaseTool.instance.GetGroundPoint(nextPos);
                if (groundPos == Vector3.zero)
                {
                    m_bHitBack = false;
                    return;
                }

                //是否有阻挡
                if (BaseTool.instance.CanMoveToPos(curPos, groundPos, m_curRadius))
                {
                    m_transform.position = groundPos;
                }
                else
                {
                    m_bHitBack = false;
                    return;
                }
            }
        }



        //根据技能力度，怪物体形获取受击动作，以及硬直时间
        public BehitInfo GetBeHitInfo(int resID, int skillID)
        {


            BehitInfo info = new BehitInfo();

            LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(skillID);
            if (skillDesc == null)
            {
                return info;
            }
            //int body = m_actor.GetAttribteValue(RoleAttrType.CS_ID_BODY);        
            int bodyType = m_actor.BodyType;

            int weight = skillDesc.Get<int>("skillweight");




            //技能力度纠正        
            if (m_behitParame.damgageInfo.weight > 0)
            {
                weight = m_behitParame.damgageInfo.weight;
                //LogMgr.UnityLog(skillID+", reset weight=" + weight);
            }

            List<ActionAndTime> listRet = new List<ActionAndTime>();


            //获取技能受击反馈
            SkillBehitDisplayDesc behitDisplay = CoreEntry.gGameDBMgr.GetSkillBehitDisplayDesc(weight, bodyType);
            if (behitDisplay == null)
            {
                if ((m_actor.mActorType == ActorType.AT_BOSS || m_actor.mActorType == ActorType.AT_LOCAL_PLAYER || m_actor.mActorType == ActorType.AT_PVP_PLAYER)
                    && (m_attack.mActorType == ActorType.AT_BOSS || m_attack.mActorType == ActorType.AT_LOCAL_PLAYER || m_attack.mActorType == ActorType.AT_PVP_PLAYER))
                {
                    //普通受击,只有一个
                    ActionAndTime data = new ActionAndTime();
                    data.strAction = "hit001";
                    data.needTime = m_actor.GetActionLength(data.strAction);

                    listRet.Add(data);

                    info.hitDistance = 0;
                    info.moveDistance = 0;
                    //if (creatureDesc != null)
                    //{
                    //    info.moveDistance = creatureDesc.behitMoveBase * skillDesc.hitMoveScale / 100f;
                    //}
                    //else
                    //{
                    //    LogMgr.WarningLog("The creatureDesc is null.");
                    //}
                    info.moveTime = 0.1f;//skillDesc.hitMoveTime / 1000f;


                    //保证动作播放完成
                    info.exitStateTime = data.needTime;
                }

                info.strActionArry = listRet.ToArray();
                return info;
            }

            //BOSS强制不浮空，只击退
            //if (m_actor.mActorType == ActorType.AT_BOSS && m_actor.IsInQiJue())
            //{
            //    if (behitDisplay.behitType == BehitType.BT_HITSKY)
            //    {
            //        behitDisplay.behitType = BehitType.BT_HITBACK;
            //    }

            //}


            //硬直时间
            info.NonControlTime = behitDisplay.nonControlSet.keepTime * 0.001f;

            BehitType behitType = behitDisplay.behitType;
            //动作                
            if (behitDisplay.behitType == BehitType.BT_NORMAL)
            {
                //普通受击,只有一个
                ActionAndTime data = new ActionAndTime();
                data.strAction = behitDisplay.actionList[0];
                data.needTime = m_actor.GetActionLength(data.strAction);

                listRet.Add(data);

                info.hitDistance = 0;

                //   info.moveDistance = skillDesc.hitMoveDistance;            
                //   info.moveTime = 0.1f;
                info.moveDistance = 0;
                //if (creatureDesc != null)
                //{
                //    info.moveDistance = creatureDesc.behitMoveBase * skillDesc.hitMoveScale / 100f;
                //}
                //else
                //{
                //    LogMgr.WarningLog("The creatureDesc is null.");
                //}

                info.moveTime = 0.5f;//skillDesc.hitMoveTime / 1000f;  


                //保证动作播放完成
                info.exitStateTime = data.needTime;
                if (m_actor.mActorType == ActorType.AT_BOSS)
                {
                    StateParameter stateParm = new StateParameter();
                    stateParm.state = ACTOR_STATE.AS_BEHIT;

                    stateParm.skillID = skillID;
                    m_actor.m_AttackState.BreakSkill(stateParm);
                }
            }
            else if (behitDisplay.behitType == BehitType.BT_HITBACK)
            {
                //击退，带有位移            
                ActionAndTime data = new ActionAndTime();
                data.strAction = behitDisplay.actionList[0];
                data.needTime = m_actor.GetActionLength(data.strAction);

                listRet.Add(data);
                info.hitDistance = 0;
                //if(creatureDesc != null)
                //{
                //    info.hitDistance = skillDesc.hitBackScale / 100f * creatureDesc.behitBackBase;
                //}
                //else
                //{
                //    LogMgr.WarningLog("The creatureDesc is null.");
                //}


                //需要打断技能
                if (m_attack.mActorType == ActorType.AT_BOSS)
                {
                    LuaTable pSkillDesc = m_actor.GetCurSkillDesc(m_actor.curCastSkillID);
                    if (pSkillDesc != null)
                    {
                        if (pSkillDesc.Get<int>("showtype") == 2) //大招和合击不能打断
                        {
                            StateParameter stateParm = new StateParameter();
                            stateParm.state = ACTOR_STATE.AS_BEHIT;

                            stateParm.skillID = skillID;
                            m_actor.m_AttackState.BreakSkill(stateParm);
                        }
                    }

                }



                // info.hitDistance = 1;       //保证释放被击位移
                // if (m_behitParame.damgageInfo.isNotUseCurveMove)
                // {
                //     info.hitDistance = 0;
                //  }

                //保证动作播放完成
                info.exitStateTime = data.needTime;
            }
            //else if (behitDisplay.behitType == BehitType.BT_HITDOWN)
            //{
            //    //击到
            //    ActionAndTime data = new ActionAndTime();
            //    data.strAction = behitDisplay.actionList[0];
            //    data.needTime = m_actor.GetActionLength(data.strAction);

            //    listRet.Add(data);            

            //    info.hitDistance = skillDesc.iHitFlyScale / 100f * creatureDesc.fBehitFlyBase;
            //    //保证动作播放完成
            //    info.exitStateTime = data.needTime;  

            //    m_isHitDownState = true;                      
            //}
            //else if (behitDisplay.behitType == BehitType.BT_HITSKY)
            //{
            //    //浮空，接管被击的硬直保护，硬直，退出时间等   
            //    if (m_gravityMotionBase != null)
            //    {
            //        m_gravityMotionBase.StartHitToSky(skillDesc.hitSkyOriginV, skillDesc.hitSkyAngle);

            //        //需要打断技能
            //        StateParameter stateParm = new StateParameter();
            //        stateParm.state = ACTOR_STATE.AS_BEHIT;

            //        stateParm.skillID = skillID;
            //        m_actor.m_AttackState.BreakSkill(stateParm);

            //        info.isInSky = true;
            //        return info;
            //    }

            //    //没有浮空动作，采用hitdown
            //    behitType = BehitType.BT_HITDOWN;                                                                                 
            //}

            //击倒处理
            if (behitType == BehitType.BT_HITDOWN)
            {
                ActionAndTime data = new ActionAndTime();
                data.strAction = behitDisplay.actionList[0];
                data.needTime = m_actor.GetActionLength(data.strAction);

                listRet.Add(data);
                info.hitDistance = 0;
                //if (creatureDesc != null)
                //{
                //    info.hitDistance = skillDesc.hitFlyScale / 100f * creatureDesc.behitFlyBase;
                //}
                //else
                //{
                //    LogMgr.WarningLog("The creatureDesc is null.");
                //}


                // info.hitDistance = 1;       //保证释放被击位移
                //  if (m_behitParame.damgageInfo.isNotUseCurveMove)
                //   {
                //       info.hitDistance = 0;
                //   }

                //保证动作播放完成

                if (m_actor.mActorType == ActorType.AT_BOSS)
                    info.exitStateTime = data.needTime - 0.5f;
                else
                    info.exitStateTime = data.needTime - 0.2f;

                m_isHitDownState = true;
            }


            //硬直保护
            if (//behitDisplay.behitType == BehitType.BT_HITBACK ||
                behitType == BehitType.BT_HITDOWN)
            {
                if (m_actor.mActorType == ActorType.AT_BOSS
                    || m_actor.mActorType == ActorType.AT_LOCAL_PLAYER)
                {
                    m_isNonControlProtect = true;
                }

                //取消目标

                if (m_attack.mActorType == ActorType.AT_LOCAL_PLAYER)
                {

                    //yy修改更换目标的BUG
                    //      actorObj.player.SelectTarget(null);                
                }
            }

            //主角被击后，有位移
            if ((m_actor.mActorType == ActorType.AT_LOCAL_PLAYER || m_actor.mActorType == ActorType.AT_PVP_PLAYER) &&
                (info.hitDistance > 0 || info.moveDistance > 0))
            {
                //只旋转Y轴
                Vector3 lookPos = new Vector3(m_attack.transform.position.x,
                    m_transform.position.y, m_attack.transform.position.z);

                m_transform.LookAt(lookPos);
            }

            //采用动作时间
            if (behitDisplay.nonControlSet.canReset)
            {
                foreach (ActionAndTime action in listRet)
                {
                    info.NonControlTime += action.needTime;
                }
            }


            info.strActionArry = listRet.ToArray();

            return info;
        }

        void RecoverShader()
        {
            CancelInvoke("RecoverShader");

            m_actor.CancelBrightenShader();
        }


        void RemoveShader()
        {
            CancelInvoke("RemoveShader");

            // m_actor.RemoveShader("DZSMobile/Balloon");YY/VNContour

            m_actor.RemoveShader("YY/VNContour");
        }


        //播放声音
        public void PlayBehitSound(ActorObj BeHitActor, ActorObj AttackActor, LuaTable skillDesc)
        {
            if (BeHitActor == null || AttackActor == null || skillDesc == null)
                return;

            if (!BeHitActor.IsMainPlayer() && !AttackActor.IsMainPlayer())
            {
                return;
            }


            m_actor = BeHitActor;
            bool ret = false;
            //if (AttackActor != null)
            //{
            //    //特效声音
            //    string behitSound = "";
            //    ret = AudioCore.GenerateAudio(skillDesc.Get<int>("behit_sound"), ref behitSound);
            //    if (ret && behitSound.Length > 0)
            //    {
            //        m_actor.StopSound();
            //        m_actor.PlaySound(behitSound);
            //    }
            //}

            //生物自己发声
            string behitVoice = "";
            ret = AudioCore.GenerateAudio(skillDesc.Get<int>("behit_sound"), ref behitVoice);
            if (ret && behitVoice.Length > 0)
            {
                m_actor.StopSound2();
                m_actor.PlaySound2(behitVoice);
            }
        }
    }
}

