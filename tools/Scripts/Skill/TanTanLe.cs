using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    // 弹弹乐
[Hotfix]
    public class TanTanLe : Bullet
    {
        int hitCount;
        //float LastTime = 0;

        void OnEnable()
        {
            m_eventMgr.RemoveListener(GameEvent.GE_EVENT_KILL, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_EVENT_KILL, EventFunction);
        }

        void OnDisable()
        {
            m_eventMgr.RemoveListener(GameEvent.GE_EVENT_KILL, EventFunction);
        }

        void EventFunction(GameEvent ge, EventParameter parameter)
        {
            switch (ge)
            {
                case GameEvent.GE_EVENT_KILL:
                    if (m_aimTransform != null)
                    {
                        // 目标死亡时的处理
                        if (parameter.goParameter1 == m_aimTransform.gameObject)
                        {
                            m_aimTransform = null;
                            CancelInvoke("NextAim");
                            NextAim();
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override void Init(BulletParam param)
        {
            m_param = param;
            m_aimTransform = m_param.aimTransform;
            hitCount = m_param.TanTanLeCount;

            if (m_aimTransform == null) // 弹弹乐必须要有目标
            {
                CancelInvoke("AutoEnd");
                Destroy(this.gameObject, 0.0001f);
                return;
            }
            distanceToTarget = Vector3.Distance(this.transform.position, m_aimTransform.position);

        }

        //触发器
        public override void OnTriggerEnter(Collider other)
        {
            if (other.transform != m_aimTransform)
                return;

            bool bIsMonster = false;

            ActorObj actorBase = other.transform.root.gameObject.GetComponent<ActorObj>();
            if (actorBase == null)
            {
                actorBase = other.transform.gameObject.GetComponent<ActorObj>();
                bIsMonster = true;
            }

            //纠正被击表现
            DamageParam damageParam = new DamageParam();
            damageParam.skillID = m_param.skillID;
            damageParam.attackActor = m_param.castObj.GetComponent<ActorObj>();

            if (bIsMonster)
            {
                damageParam.behitActor = actorBase;
            }
            else
            {
                damageParam.behitActor = actorBase;
            }

            CoreEntry.gSkillMgr.OnSkillDamage(damageParam);

            //是否有眩晕效果
            //if (m_param.dizzyTime != 0)
            //{
            //    DizzyParam param = new DizzyParam();
            //    param.keepTime = m_param.dizzyTime;

            //    actorBase.OnEnterDizzy(param);
            //}

            if (objEfx != null)
                objEfx.SetActive(false);

            Invoke("NextAim", 0f);
        }

        protected void NextAim()
        {
            hitCount--;
            if (hitCount <= 0)
            {
                AutoEnd();
                return;
            }

            ActorObj aimActorBase = null;

            List<ActorObj> AIMList = new List<ActorObj>();
            List<ActorObj> noDIZZYList = new List<ActorObj>();

            // 下一个目标
            //计算伤害  群体
            List<ActorObj> actors = CoreEntry.gActorMgr.GetAllMonsterActors();
            for (int i = 0; i < actors.Count; i++)
            {
                //对IOS出现怪物不动 报错的异常  进行错误处理
                if (GameLogicMgr.checkValid(actors[i].gameObject) == false)
                {
                    continue;
                }
                GameObject obj = actors[i].gameObject;
                ActorObj actorBase = actors[i];

                //临时判断
                ActorObj castBase = m_param.castObj.GetComponent<ActorObj>();
                if (actorBase.mActorType == castBase.mActorType)
                    continue;

                if (actorBase.IsDeath())
                    continue;

                if (!CoreEntry.gGameMgr.IsPvpState() && !castBase.IsAimActorType(actorBase))//IsAimActorType(actorBase.mActorType))
                    continue;

                if (obj.transform == m_aimTransform) // 跳过自己
                    continue;

                // 栅栏怪等建筑物不可以被击退，击飞，聚集,眩晕
                //if (actorBase.actorCreatureInfo.behitMoveBase == 0 && actorBase.actorCreatureInfo.behitBackBase == 0 && actorBase.actorCreatureInfo.behitFlyBase == 0)
                //{
                //    continue;
                //}

                // 距离
                if (m_aimTransform == null)
                {
                    if (!CoreEntry.gBaseTool.IsPointInCircleXZ(m_param.castObj.transform.position, actorBase.transform.position,
                        m_param.TanTanLeDis, actorBase.GetColliderRadius()))
                    {
                        continue;
                    }
                }
                else
                if (!CoreEntry.gBaseTool.IsPointInCircleXZ(m_aimTransform.transform.position, actorBase.transform.position,
                    m_param.TanTanLeDis, actorBase.GetColliderRadius()))
                {
                    continue;
                }

                AIMList.Add(actorBase);

                if (actorBase.curActorState != ACTOR_STATE.AS_DIZZY)
                    noDIZZYList.Add(actorBase);

                if (aimActorBase == null)
                {
                    aimActorBase = actorBase;
                    continue;
                }
            }

            // 在没有眩晕的怪之间随机
            if (noDIZZYList.Count > 0)
                aimActorBase = noDIZZYList[Random.Range(0, noDIZZYList.Count)];
            else
            // 在眩晕的怪之间随机    
            if (AIMList.Count > 0)
                aimActorBase = AIMList[Random.Range(0, AIMList.Count)];

            if (aimActorBase != null)
            {
                m_aimTransform = aimActorBase.transform;

                if (objEfx != null)
                    objEfx.SetActive(true);

                if (GetComponent<Collider>() != null)
                {
                    GetComponent<Collider>().enabled = false;
                    GetComponent<Collider>().enabled = true;
                }
            }
            else
            {
                AutoEnd(); // 没有目标就消失了
            }
        }

        protected override void Update()
        {
            if (bAutoEnd)
                return;

            //目标死亡切换
            if (m_aimTransform != null)
            {
                ActorObj actorBase = m_aimTransform.GetComponent<ActorObj>();
                if (actorBase != null)
                {
                    //在这里判断，通过事件发送可能已经被丢失，所以在这里判断
                    if (actorBase.IsDeath())
                    {
                        //Debug.Log("1");
                        m_aimTransform = null;
                        CancelInvoke("NextAim");
                        NextAim();
                    }
                }
            }

            base.Update();
        }
    }
}

