using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
#if !UNITY_5_3_8
using UnityEngine.AI;
#endif

namespace SG
{

[Hotfix]
    public class DeathState : IActorState
    {

        private float m_beginDeathTime = 0;
        //private GameDBMgr m_GameDataBase;
        private string m_dieAction = "";

        public string DieAction
        {
            get { return m_dieAction; }
            //set { m_dieAction = value; }
        }

        GameObject m_efxObj = null;


        public override void OnEnter(ActorObj actorBase)
        {
            m_state = ACTOR_STATE.AS_DEATH;
            m_isDeathEnd = false;

            m_actor = actorBase;

            // 直接设置当前血量
            m_actor.mBaseAttr.CurHP = 0;
            if (actorBase.Health != null)
            {
                actorBase.Health.OnDead();
            }

            // 停止移动 , 停止当前动作
            m_actor.StopMove(false);
            m_actor.StopAll();


            if (m_actor.m_bNoDieAction)
                m_dieAction = "stand";  //召唤生物不播放死亡动作               
            else
                m_dieAction = "die001";


            LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(CurParam.skillID);

            //使用击飞死亡
            if (m_actor.IsHadAction("die002") && null != skillDesc)
            {
                string knockStr = skillDesc.Get<string>("knockback");
                if (!string.IsNullOrEmpty(knockStr))
                {
                    string[] knockValues = knockStr.Split('#');
                    float pValue = Random.Range(0f, 1.0f);
                    float pro = 0f;
                    float.TryParse(knockValues[0], out pro);
                    if (pValue * 100 <= pro)
                    {
                        float minDistance = 0f;
                        float maxDistance = 0f;
                        float.TryParse(knockValues[1], out minDistance);
                        float.TryParse(knockValues[2], out maxDistance);

                        m_dieAction = "die002";
                        m_actor.DobehitedFly(m_dieAction, Random.Range(minDistance, maxDistance), CurParam.AttackActor);
                    }
                }
            }

            Invoke("PlayDieSound", 1.8f);

            //死亡的时候在空中
            if (m_actor.mActorType == ActorType.AT_LOCAL_PLAYER
                || !m_actor.isHaveRigidbody)
            {
                if (m_actor.m_gravityMotionBase != null && m_actor.m_gravityMotionBase.isUseGravityState)
                {
                    m_actor.SetPosition(BaseTool.instance.GetGroundPoint(m_actor.transform.position));

                }
            }

            //monster10秒后，消失
            if (m_actor.mActorType == ActorType.AT_MONSTER || m_actor.mActorType == ActorType.AT_BOSS
                || m_actor.mActorType == ActorType.AT_NPC || m_actor.mActorType == ActorType.AT_BROKED
                || m_actor.mActorType == ActorType.AT_NON_ATTACK || m_actor.mActorType == ActorType.AT_AVATAR
                || m_actor.mActorType == ActorType.AT_MECHANICS
                || m_actor.m_bNoDieAction)
            {

                EventParameter param = EventParameter.Get();
                param.autoRecycle = false;
                param.intParameter = m_actor.EntityID;
                param.intParameter1 = m_actor.resid;
                param.intParameter2 = m_actor.ConfigID;
                param.goParameter = m_actor.GetOwnObject();
                if (m_actor.m_bSummonMonster)
                {
                    m_actor.gEventMgr.TriggerEvent(GameEvent.GE_SUMMON_DEATH, param);
                }                    
                else
                {
                    m_actor.gEventMgr.TriggerEvent(GameEvent.GE_MONSTER_DEATH, param);
                    if (m_actor.mActorType != ActorType.AT_BOSS)
                    {
                        bool sendTaskMsg = true;
                        if (sendTaskMsg)
                        {
                            m_actor.gEventMgr.TriggerEvent(GameEvent.GE_TASK_KILL_MONSTER, param);
                        }
                    }
                }
                EventParameter.Cache(param);

                if (m_actor.isHaveRigidbody)
                {
                    m_beginDeathTime = Time.time;

                    //自身阻挡去掉    
                    this.gameObject.GetComponent<Collider>().enabled = false;
                }
                else
                {
                    //有可能是死亡特效
                    int nRand = Random.Range(0, 5);
                    int bDieBroken = 0;// m_actor.actorCreatureDisplayDesc.DieBroken;
                    if (m_actor.m_bNoDieAction)
                    {
                        //分身消失特效
                        this.gameObject.SetActive(false);

                        Object efobj = CoreEntry.gResLoader.LoadResource("Effect/skill/remain/fx_fenshen");
                        if (efobj != null)
                        {
                            m_efxObj = Instantiate(efobj) as GameObject;

                            m_efxObj.transform.position = m_actor.transform.position;
                        }
                        m_actor.PlayAction(m_dieAction);
                        m_actor.GetActionLength(m_dieAction);
                        Invoke("AutoDestoryBody", 1.5f);

                    }
                    else
                    if (bDieBroken == 1 && nRand == 1 && m_actor.mActorType != ActorType.AT_BOSS)
                    {
                        //隐藏模型
                        this.gameObject.SetActive(false);
                        m_efxObj = Instantiate(CoreEntry.gResLoader.LoadResource("Effect/skill/hurt/fx_roukuai")) as GameObject;
                        if(m_efxObj!=null)
                        {
                            m_efxObj.transform.position = m_actor.transform.position;
                            m_actor.PlayAction(m_dieAction);
                            Invoke("AutoDestoryBody", 1.5f);
                        }


                    }
                    else
                    {
                        if (m_actor.m_bSummonMonster == false)
                        {
                            m_actor.PlayAction(m_dieAction);

                            float dieTime = m_actor.GetActionLength(m_dieAction);
                            Invoke("AutoDestory", m_actor.BodyKeepTime + dieTime);
                        }
                        else
                        {
                            AutoDestory();
                        }

                    }


                    //去掉碰撞            
                    m_actor.CancelCollider();
                }

                //if (m_actor.mActorType == ActorType.AT_BOSS)
                //{
                //    //boss死亡 by lzp
                //    //CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TASK_KILLBOSS, null);
                //    //boss,慢镜头
                //    //Time.timeScale = 0.1f;
                //    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TASK_STOPTASKTIME, null);

                //    if (CoreEntry.GameEffectMgr.BossDeadEffect_PlayAction != null)
                //    {
                //        CoreEntry.GameEffectMgr.BossDeadEffect_PlayAction.SetBoss(gameObject);
                //    }

                //    CoreEntry.GameEffectMgr.BossDeadEffect.GameObj.SetActive(true);

                //    //if (TimeScaleCore.SetValue(0.2f))
                //    {
                //        Invoke("TimeScaleEnd", CoreEntry.GameEffectMgr.BossDeadEffect.Length);
                //        //由于有个动画时长可能大于原来的单一的死亡动作时长，所以这里重新设置一下，用整个动画的时长
                //        CancelInvoke("AutoDestory");
                //        Invoke("AutoDestory", m_actor.GetDeathDuration());
                //    }

                //}

                //区别敌方阵营
                if (m_actor.TeamType == 3)
                {
                    //怪物死忙 by lzp
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TASK_KILLNUM, null);
                }

                //死亡后，动作阻挡去掉
                //if (m_actor.actorCreatureDisplayDesc.iIsStaticMonster == 1)
                {
                    NavMeshObstacle[] navMeshObstacles = this.gameObject.GetComponentsInChildren<NavMeshObstacle>();
                    for (int i = 0; i < navMeshObstacles.Length; i++)
                    {
                        navMeshObstacles[i].enabled = false;
                    }
                }

                m_actor.HideBlobShadow();

                float aniTime = m_actor.GetActionLength(m_dieAction);
                float downDelayTime = ConfigManager.Instance.Consts.GetValue<float>(428, "fval");
                Invoke("DelayHideModle", aniTime + downDelayTime);
            }
            else if (m_actor.mActorType == ActorType.AT_LOCAL_PLAYER)
            {
                float dieTime = m_actor.GetActionLength(m_dieAction);
                Invoke("PlayerDeathEventDelay", dieTime);
                //主角死亡通知 by lzp
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TASK_HERONUM, null);


                m_actor.PlayAction(m_dieAction);
                m_isDeathEnd = true;
            }
            else if (m_actor.mActorType == ActorType.AT_PVP_PLAYER ||
                m_actor.mActorType == ActorType.AT_REMOTE_PLAYER)
            {
                float dieTime = m_actor.GetActionLength(m_dieAction);
                Invoke("PlayerDeathEventDelay", dieTime);

                //主角死亡通知 by lzp
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TASK_HERONUM, null);

                m_actor.PlayAction(m_dieAction);

                m_isDeathEnd = true;
            }

            //Buff by yuxj
            m_actor.BuffOnDie();
            //召唤物杀死
            SummonCell.OnSummonKillAll(m_actor);



        }

        private TweenPosition tp;
        //隐藏尸体
        void DelayHideModle()
        {
            if(null == m_actor)
            {
                return;
            }
            float downTime = 0;
            if (m_actor.mActorType == ActorType.AT_BOSS)
            {
                downTime = ConfigManager.Instance.Consts.GetValue<float>(429, "fval");
            }
            else
            {
                downTime = ConfigManager.Instance.Consts.GetValue<float>(430, "fval");
            }
            float downValue = ConfigManager.Instance.Consts.GetValue<float>(431, "fval");
            Vector3 downPos = transform.position - new Vector3(0, downValue, 0);
            tp = TweenPosition.Begin(gameObject, downTime, downPos);

            Invoke("DeathEnd", downTime);
        }

        void DeathEnd()
        {
            m_isDeathEnd = true;
        }

        void OnDisable()
        {
            if (null != tp)
            {
                Destroy(tp);
            }

            m_isDeathEnd = true;
        }

        //public override void FrameUpdate()
        //{

        //}
        //by lzp 2015/04/24 
        //boss慢动作完后，任务结束
        void TaskKillBoss()
        {
            CancelInvoke("TaskKillBoss");

            EventParameter param = EventParameter.Get();
            param.intParameter = m_actor.EntityID;
            param.intParameter1 = m_actor.resid;
            param.goParameter = m_actor.GetOwnObject();
            m_actor.gEventMgr.TriggerEvent(GameEvent.GE_TASK_KILL_MONSTER, param);

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TASK_KILLBOSS, null);

        }

        public override void OnExit(StateParameter stateParm)
        {
            m_actor.StopAction(m_dieAction);

            //死亡复活，保护时间
            m_actor.RecoverHealth(3);

            CancelInvoke("AutoDestory");
            if (null != tp)
            {
                Destroy(tp);
            }
            CancelInvoke("DeathEnd");
            m_isDeathEnd = true;
        }

        public override bool CanChangeState(StateParameter stateParm)
        {
            //死亡状态，只有复活技能可用
            switch (stateParm.state)
            {
                case ACTOR_STATE.AS_DEATH:
                case ACTOR_STATE.AS_RUN:
                case ACTOR_STATE.AS_BEHIT:
                case ACTOR_STATE.AS_ATTACK:
                    {
                        return false;
                    }
                case ACTOR_STATE.AS_STAND:
                    if (stateParm.skillID == 1)
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }

            return false;
        }


        void AutoDestory()
        {
            CancelInvoke("AutoDestory");

            //Destroy(m_actor.GetOwnObject());
            //CoreEntry.gObjPoolMgr.RecycleObject(m_actor.resid, m_actor.GetOwnObject());
        }


        void AutoDestoryBody()
        {
            CancelInvoke("AutoDestoryBody");

            if (m_efxObj)
                Destroy(m_efxObj);
        }


        //检查是否静止了
        void CheckRigidbodyStop()
        {
            if (m_actor.m_rootRigidbody)
            {
                //15秒后，自动删除吧
                bool forceDestory = false;
                if (Time.time - m_beginDeathTime >= 10f)
                {
                    forceDestory = true;
                }

                if (m_actor.m_rootRigidbody.velocity.sqrMagnitude < 2 || forceDestory)
                {
                    //Invoke("AutoDestory", m_actor.actorCreatureDisplayDesc.fCorpseKeepTime);
                    Invoke("AutoDestory", m_actor.BodyType);
                    CancelInvoke("CheckRigidbodyStop");

                    //int index = Random.Range(1, 4);
                    //m_actor.PlaySound(@"sound/monster/down0" + index);
                }
            }
        }

        void TimeScaleEnd()
        {
            CancelInvoke("TimeScaleEnd");
            //Time.timeScale = 1f;
            TimeScaleCore.ResetValue();


            if (m_actor.mActorType == ActorType.AT_BOSS)
            {
                StartCoroutine(delayShowEffect(transform.position));
                return;
            }
        }

        IEnumerator delayShowEffect(Vector3 position)
        {
            //CoreEntry.GameStart = false; 
            //掉宝箱
            string path = @"Effect/skill/remain/fx_luobaoxiang";
            Object prefab = CoreEntry.gResLoader.Load(path);
            if(prefab == null)yield break;
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.transform.position = new Vector3(-100000, -1000000, -100000);

            yield return new WaitForSeconds(0.9f);
            //CoreEntry.GameStart = false; 

            go.SetActive(false);
            go.SetActive(true);

            go.transform.position = transform.position;

            //宝箱音效
            if (CoreEntry.cfg_bEaxToggle)
            {
                AudioSource[] audios = go.GetComponentsInChildren<AudioSource>();
                if (audios != null)
                {
                    for (int i = 0; i < audios.Length && audios[i] != null; i++)
                        audios[i].Play();
                }
            }

            yield return new WaitForSeconds(1f);
            //CoreEntry.GameStart = false; 

            Invoke("TaskKillBoss", .5f);
            //CoreEntry.GameStart = false; 


        }
        void PlayDieSound()
        {
            string dieVoice = "";
            bool ret = AudioCore.GenerateAudio(m_actor.DieSound, ref dieVoice);
            if (ret && dieVoice.Length > 0)
            {
                m_actor.StopSound2();
                m_actor.PlaySound2(dieVoice);
            }
        }
        void PlayDownSound()
        {
            CancelInvoke("PlayDownSound");
        }

        void PlayerDeathEventDelay()
        {
            EventParameter param = EventParameter.Get();
            param.goParameter = m_actor.GetOwnObject();
            param.intParameter = m_actor.EntityID;
            m_actor.gEventMgr.TriggerEvent(GameEvent.GE_PLAYER_DEATH, param);
        }
    }
}

