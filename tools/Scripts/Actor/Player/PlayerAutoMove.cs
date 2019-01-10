using XLua;
﻿using UnityEngine;
using System.Collections;
#if !UNITY_5_3_8
using UnityEngine.AI;
#endif
namespace SG
{
    //
[Hotfix]
    public class PlayerAutoMove : PlayerMove
    {

        private PlayerObj player;

        //private Transform m_transform;

        //private EventMgr m_eventMgr = null;
        //private bool m_isRuning = false;

        //float fRemainPath = 0.0f;
        //public int nCount = 1;

        //private Vector3 m_dstPos = Vector3.zero;


        public System.Action m_arriveCB = null ;
        public System.Action ArriveCB
        {
            get { return m_arriveCB; }
            set { m_arriveCB = value; }
        }


        public override void Init()
        {
            if (m_agent == null)
            {
                m_agent = this.gameObject.AddComponent<NavMeshAgent>();
            }

            m_eventMgr = CoreEntry.gEventMgr;

            m_agent.speed = 6;
            m_agent.acceleration = 3.40282e+038f;   //启动速度
        }


        //移动
        public override void MovePos(Vector3 pos)
        {
            if (m_agent == null)
            {
                LogMgr.LogError("NavMeshAgent is null");
                return;
            }


            m_agent.enabled = true;
            if (!m_agent.gameObject.activeSelf)
                return;
            bool bRet = m_agent.SetDestination(pos);
            
            if (!bRet)
            {
                NavMeshHit hit;

                if (NavMesh.SamplePosition(transform.position, out hit, 100.0f, -1))
                {
                    pos.y = hit.position.y;
                }
            }

            m_dstPos = pos;


            //开始移动
            EventParameter param = EventParameter.Get();
            param.goParameter = this.gameObject;
            m_eventMgr.TriggerEvent(GameEvent.GE_PLAYER_MOVE_BEGIN, param);

            //0.01秒后开始计算距离
            Invoke("BeginRun", 0.01f);
        }

        //ActorObj owner = null;
        void Awake()
        {
            m_agent = this.gameObject.GetComponent<NavMeshAgent>();
            if (m_agent == null)
            {
                m_agent = this.gameObject.AddComponent<NavMeshAgent>();
            }

            //owner = gameObject.GetComponent<ActorObj>();
            m_eventMgr = CoreEntry.gEventMgr;

       
            m_agent.speed = 6;


           m_agent.acceleration = 3.40282e+038f;   //启动速度
           //取消避免障碍的功能，防止自动移动到离障碍近的时候，速度变慢
           //m_agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

            m_transform = this.transform;
        }

        // Use this for initialization
        void Start()
        {
            //m_eventMgr = CoreEntry.gEventMgr;
        }


        public override void SetMoveSpeed(float fSpeed)
        {
            //add by Alex 20160606 判空
            if (m_agent != null)
            {
                if (m_agent.enabled)
                {
                    m_agent.speed = fSpeed;
                    m_agent.acceleration = 100;   //启动速度
                }
            }
        }


        // Update is called once per frame
        void Update()
        {
            ////跑到目的地        
            //if (m_agent.enabled && m_agent.pathStatus == NavMeshPathStatus.PathComplete
            //   && m_isRuning)
            //{
            //    if (owner != null && owner != null)
            //    {
            //        m_agent.speed = owner.m_move.GetSpeed();
            //    }

            //    if (m_agent.remainingDistance <= 0.01)
            //    {
            //        //没有路径了
            //        if (!m_agent.hasPath)
            //         {
            //            //判断距离
            //             float fDist = Vector3.Distance(m_dstPos, m_transform.position);
            //             if (fDist > 0.2f)
            //             {
            //                 m_agent.ResetPath();

            //                 NavMeshPath path = new NavMeshPath();
            //                 bool bRet = m_agent.CalculatePath(m_dstPos, path);
            //                 if (bRet && path.corners.Length > 1)
            //                 {
            //                     m_agent.path = path;
            //                     return;
            //                 }

            //             }                 
                        
            //         }


            //        m_isRuning = false;
            //        m_agent.enabled = false;

            //        //通知移动结束
            //        EventParameter param = EventParameter.Get();
            //        param.goParameter = this.gameObject;

            //        m_eventMgr.TriggerEvent(GameEvent.GE_PLAYER_MOVE_END, param);


            //        if (ArriveCB!=null )
            //        {
            //            ArriveCB(); 
            //        }
            //        if (TeamMgr.IsMainPlayer(gameObject))
            //        {
            //            if (MapMgr.Instance.InMainCity)
            //            {
            //                m_eventMgr.TriggerEvent(GameEvent.GE_PLAYER_MOVE_Arrive, null);
            //            }
            //        }
            //    }

            //    if (m_agent.enabled)
            //        fRemainPath = m_agent.remainingDistance;

                Debug.DrawLine(m_transform.position, m_dstPos, Color.red);
        }
      

        void BeginRun()
        {
            CancelInvoke("BeginRun");

            //m_isRuning = true;

             
        }

        public override void Stop(bool isSendEvent)
        {

            //m_isRuning = false;
            //m_agent.enabled = false;

            //if (isSendEvent)
            //{
            //    //通知移动结束
            //    EventParameter param = EventParameter.Get();
            //    param.goParameter = this.gameObject;

            //    m_eventMgr.TriggerEvent(GameEvent.GE_PLAYER_MOVE_END, param);
            //}
        }

        public override void SetSpeed(float speed)
        {
          //  m_agent.speed = speed;
        }

    }
}

