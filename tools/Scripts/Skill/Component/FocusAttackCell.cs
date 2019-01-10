using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SG
{
[Hotfix]
    public class FocusAttackCell : ISkillCell
    {
        public OneDamageInfo m_oneDamageInfo = null;
        private Transform m_transform;
        private SkillBase m_skillBase;
        //private FrameStopCellDesc m_frameStopCellDesc = null;

        //顿帧，屏震一次伤害只处理一次
        //private bool m_isHadLoadFrameStop = false;


        public override void Init(ISkillCellData cellData, SkillBase skillBase)
        {
            m_skillBase = skillBase;
            //聚怪技能 配置
            m_oneDamageInfo = (OneDamageInfo)cellData;
        }

        // PoolManager
        void OnDisable()
        {
            CancelInvoke("CalculateDamage");
            CancelInvoke("Start");
        }

        // PoolManager
        void OnEnable()
        {
            CancelInvoke("CalculateDamage");
            CancelInvoke("Start");
            Invoke("Start", 0.000001f);
        }

        // Use this for initialization
        void Start()
        {
            CancelInvoke("Start");

            if (m_skillBase == null || m_skillBase.m_actor == null)
                return;

            //m_isHadLoadFrameStop = false;

            //伤害计算
            //m_skillBase = this.transform.root.gameObject.GetComponent<SkillBase>();
            m_transform = m_skillBase.transform;



            float hitTime = m_oneDamageInfo.hitTime / m_skillBase.m_speed;

            float delayTime = m_oneDamageInfo.hitTime;

            float curTime = m_skillBase.GetCurActionTime();

            if (curTime >= hitTime)
            {
                CalculateDamage();
            }
            else
            {
                //delayTime = hitTime - curTime;
                Invoke("CalculateDamage", delayTime);
            }

            if (m_oneDamageInfo.isRepeatedly)
            {
                //Invoke("EndDamage", endTime);
            }
        }

        void EndDamage()
        {
            CancelInvoke("CalculateDamage");
        }

        void CalculateDamage()
        {
            CancelInvoke("CalculateDamage");
            DoFocusAction();
        }

        //class objectMoveInfo
        //{
        //    public GameObject obj;
        //    public Vector3 srcPos;
        //};

        void DoFocusAction()
        {
            //计算位移距离
            FunDamageNode fun = m_oneDamageInfo.damageNode.funDamage;

            //聚怪中心点
            Vector3 centerPos = m_transform.position + m_transform.forward.normalized * fun.offDistance;

            //中间是否有空气墙
            Vector3 wallPos = CoreEntry.gBaseTool.GetWallHitPoint(m_transform.position, centerPos);
            if (!wallPos.Equals(centerPos))
            {
                //存在空气墙 
                Vector3 dist = wallPos - m_transform.position;
                dist = dist * 0.9f;
                //Vector3 wallGroundPos = CoreEntry.gBaseTool.GetGroundPoint(m_transform.position + dist);
                centerPos = CoreEntry.gBaseTool.GetGroundPoint(centerPos);

            }

            if (centerPos.Equals(Vector3.zero))
            {
                centerPos = m_transform.position;
            }

            //需要聚集的怪物    
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


                //塔不没有受击
                MonsterObj monsterObj = obj.GetComponent<MonsterObj>();
                if (monsterObj && monsterObj.m_bIsTower)
                {
                    continue;
                }

                //bool isSkillSuccess = false;

                if (!m_skillBase.m_actor.IsAimActorType(actorBase))
                {
                    continue;
                }

                // 有霸体的不能被拉动
                if (actorBase.IsActorEndure())
                {
                    continue;
                }

                //伤害对象
                if (CoreEntry.gBaseTool.IsPointInCircleXZ(centerPos, obj.transform.position, fun.radius, 1f))
                {
                    //isSkillSuccess = true;
                    Go.to(obj.transform, 0.4f, new GoTweenConfig().position(centerPos));
                }

            }

            StartCoroutine(destroyBox());
        }

        private IEnumerator destroyBox()
        {
            yield return new WaitForSeconds(1.0f);

        }

        static bool IsAimActorType(ActorType mActorType, List<ActorType> aimActorTypeList)
        {
            return true;
        }
    }
}

