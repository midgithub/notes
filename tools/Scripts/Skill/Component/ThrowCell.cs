using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SG
{
[Hotfix]
    public class ThrowCell : ISkillCell
    { 
        public OneDamageInfo m_oneDamageInfo = null;
        private Transform m_transform;
        private SkillBase m_skillBase;

        //实例化的数据
        public int m_dataIndex = 0;

        //private MovePosAttackDesc m_movePosAttackDesc = null;

        public override void Init(ISkillCellData cellData, SkillBase skillBase)
        {
            m_skillBase = skillBase;
            //聚怪技能 配置
            m_oneDamageInfo = (OneDamageInfo)cellData;
            m_dataIndex = 0;
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
        }


        // Use this for initialization
        void Start()
        {
            CancelInvoke("Start");
            if (m_skillBase == null || m_skillBase.m_actor == null)
                return;

            //伤害计算
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
        //private List<objectMoveInfo> m_aimObjectList = new List<objectMoveInfo>();

        //private static GameObject mCenterObj;

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
            foreach (ActorObj actor in actors)
            {
                //对IOS出现怪物不动 报错的异常  进行错误处理
                if (GameLogicMgr.checkValid(actor.gameObject) == false)
                {
                    continue;
                }

                ActorObj actorBase = actor;
                if (!m_skillBase.m_actor.IsAimActorType(actorBase))
                {
                    continue;
                }

                //伤害对象
                if (CoreEntry.gBaseTool.IsPointInCircleXZ(centerPos, actor.transform.position, fun.radius, 1f))
                {
                    //isSkillSuccess = true;

                    Vector3 distance = m_transform.position - actor.transform.position;
                    float len = distance.magnitude;
                    distance = m_transform.position + distance;

                    distance = CoreEntry.gBaseTool.GetGroundPoint(distance);
                    TweenPositionY.Begin(actor.gameObject, 0.6f, distance, len);
                }

            }

            StartCoroutine(destroyBox());
        }

        private IEnumerator destroyBox()
        {
            yield return new WaitForSeconds(1.0f);

        }
    }
}

