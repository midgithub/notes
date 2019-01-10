using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XLua;

namespace SG
{

[Hotfix]
    public class RunState : IActorState
    {

        //int nCountFoot = 0;

        //int nLFoot = 0;
        //int nRFoot = 0;


        struct FootPrintData
        {
            public GameObject m_efxObj;
            public float fTime;
        }

        List<FootPrintData> m_FootList = new List<FootPrintData>();
        List<FootPrintData> m_removeFootList = new List<FootPrintData>();


        void Start()
        {

        }

        //float m_lastFootTime = 0;

        public override void OnEnter(ActorObj actorBase)
        {
            if (actorBase.mActorType == ActorType.AT_TRAP || actorBase.mActorType == ActorType.AT_SCENE_BUFF)
            {
                return;
            }
            MonsterObj monsterObj = this.gameObject.GetComponent<MonsterObj>();
            if (monsterObj && monsterObj.m_bIsTower)
            {
                return;
            }

            m_state = ACTOR_STATE.AS_RUN;

            m_actor = actorBase;
            if (m_actor == null) return;
            // m_actor.PlayAction(m_actor.modelInfo.san_walk);

            if (m_actor.curCastSkillID > 0)
            {
                LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(m_actor.curCastSkillID);
                if (skillDesc != null)
                {
                    if (skillDesc.Get<int>("subtype") != (int)SkillType.SKILL_XUANFANZHAN)
                    {
                        if (m_actor != null)
                            m_actor.PlayAction(m_actor.ModelConfig.Get<string>("san_walk"));
                    }
                }

            }
            else
            {
                if (StoryMgr.IsInStory)
                {
                    if (string.IsNullOrEmpty(m_actor.ModelConfig.Get<string>("san_walk")))
                    {
                        m_actor.PlayAction("walk");
                    }
                    else
                    {
                        m_actor.PlayAction(m_actor.ModelConfig.Get<string>("san_walk"));
                    }
                }
                else
                {
                    m_actor.PlayAction(m_actor.ModelConfig.Get<string>("san_walk"));
                }
            }

            //设置跑步速度
            if (m_actor.mBaseAttr != null)
            {
                if (m_actor.mBaseAttr.Speed > 0)
                {
                    m_actor.SetSpeed(m_actor.mBaseAttr.Speed);
                }
            }
            else
            {
                m_actor.SetSpeed(1.0f);
            }
        }

        Object m_jiaoYinPrefab = null;
        Object getJiaoYin()
        {
            if (m_jiaoYinPrefab == null)
            {
                m_jiaoYinPrefab = CoreEntry.gResLoader.LoadResource("Effect/scence/sf_jiaoyin");
            }
            if (m_jiaoYinPrefab == null)
            {
                LogMgr.LogError("找不到 prefab: " + "Effect/scence/sf_jiaoyin");
            }
            return m_jiaoYinPrefab;
        }


        protected float lastUpdate = 0;
        public virtual void Update()
        {
            // 减少处理 频率 lmjedit
            if (Time.time - lastUpdate < 0.1f)
            {
                return;
            }
            lastUpdate = Time.time;

            for (int i = 0; i < m_FootList.Count; i++)
            {
                float fTime = Time.time - m_FootList[i].fTime;
                if (fTime > 0.8f)
                {
                    Destroy(m_FootList[i].m_efxObj);
                    m_removeFootList.Add(m_FootList[i]);
                }
            }

            //清理 m_FootList  不清理会导致性能大量损耗 lmjedit
            if (m_removeFootList.Count > 0)
            {
                for (int i = 0; i < m_removeFootList.Count; i++)
                {
                    m_FootList.Remove(m_removeFootList[i]);
                }
                m_removeFootList.Clear();
                //LogMgr.UnityLog("foot count:" + m_FootList.Count); 
            }



        }



        public override bool CanChangeState(StateParameter stateParm)
        {
            if (m_actor == null)
            {
                return false;
            }

            if (stateParm.state == ACTOR_STATE.AS_RUN)
            {
                return true;
            }


            if (stateParm.state == ACTOR_STATE.AS_BEHIT && m_actor.IsMainPlayer())
            {
                return false;
            }

            return true;
        }


        public override void OnExit(StateParameter stateParm)
        {

            //退出跑步
            if (stateParm.state == ACTOR_STATE.AS_RUN)
            {
                //AS_RUN状态，就不stopmove了   
                return;
            }

            ActorObj actorObj = m_actor.GetOwnObject().GetComponent<ActorObj>();
            actorObj.StopMove(false);
            actorObj.OnExitState(ACTOR_STATE.AS_RUN);
        }
    }
}

