using XLua;
﻿using UnityEngine;
using System.Collections;
#if !UNITY_5_3_8
using UnityEngine.AI;
#endif
namespace SG
{

[Hotfix]
public class MonsterMove : IMove {
    private Transform m_transform;

    private ActorObj m_ActorBase;

    private EventMgr m_eventMgr = null;
    private bool m_isRuning = false;

    float fRemainPath = 0.0f;
    public int nCount = 1;

    private Vector3 m_dstPos = Vector3.zero;

    public override void Init()
    {
        if (this.gameObject.GetComponent<NavMeshAgent>() == null)
	    {
		    m_agent = this.gameObject.AddComponent<NavMeshAgent>();
	    }
        else
        {
            m_agent = this.gameObject.GetComponent<NavMeshAgent>();
        }

        m_agent.enabled = false;
        
        m_agent.speed = 6;
        m_agent.acceleration = 20;   //启动速度
        
        m_transform = this.transform;
        m_eventMgr = CoreEntry.gEventMgr;
        m_isRuning = false;
        fRemainPath = 0.0f;
        nCount = 1;

        m_ActorBase = this.gameObject.GetComponent<ActorObj>();

        m_agent.speed = m_ActorBase.mBaseAttr.Speed;
    }

	//是否能到达
	public override bool CanArrived(Vector3 pos)
	{
		NavMeshHit hit;
		if (m_agent.enabled == true && m_agent.Raycast(pos, out hit) == true)
		{
			return false;
		}

		return true;
	}

    //移动
    public override void MovePos(Vector3 pos)
    {
        //add by lzp 
        if (!CoreEntry.GameStart)
            return;
        if (m_agent == null )
        {
           // Debug.LogError("NavMeshAgent is null");
            return;
        }
 
        m_agent.enabled = true;
        if (m_agent.gameObject.activeSelf && m_agent.enabled)//为什么有时候是关闭的？？add by Alex 20160606
        {
            m_agent.SetDestination(pos);
        }

        m_dstPos = pos;
        //设置isRuning的时候，阻止update运行
        m_isRuning = false;
    
        //开始移动
        EventParameter param = EventParameter.Get();
        param.goParameter = this.gameObject;

        m_eventMgr.TriggerEvent(GameEvent.GE_ACTOR_MOVE_BEGIN, param); 

        //0.01秒后开始计算距离
        Invoke("BeginRun", 0.01f);          
    }
      
    void Awake()
    {
        m_agent = this.gameObject.AddComponent<NavMeshAgent>();

        m_agent.speed = 6;

     
        m_agent.acceleration = 20;   //启动速度
        
        m_transform = this.transform;

        m_eventMgr = CoreEntry.gEventMgr;    
    }
    
	// Use this for initialization
	void Start () {
           
	}
	
	// Update is called once per frame
	void Update () {

        if (m_ActorBase.IsDeath())
        {
            m_isRuning = false;
            m_agent.enabled = false;

            //通知移动结束
            EventParameter param = EventParameter.Get();
            param.goParameter = this.gameObject;

            m_eventMgr.TriggerEvent(GameEvent.GE_ACTOR_MOVE_END, param); 
            return;
        }


        //跑到目的地        
        if (m_agent.enabled && m_agent.pathStatus == NavMeshPathStatus.PathComplete
           && m_isRuning && m_ActorBase.m_bFlee == false)
        {
            if (Mathf.Abs(fRemainPath - m_agent.remainingDistance) < 0.0001f)
            {
                 
                int nRandomAngle = 90;
                //nRandomAngle = nRandomAngle * nCount;
                //nCount++;

                //if (nRandomAngle > 180)
                //{
                //    nRandomAngle = 180;
                //    nCount = 0;
                //}


                float fRange = 0.5f;
                Vector3 vDir = Vector3.Normalize(m_agent.destination - m_transform.position);
                Quaternion qRoate = Quaternion.AngleAxis(nRandomAngle, Vector3.up);
                vDir = qRoate * vDir;
                Vector3 vDestPos = vDir * fRange + m_transform.position;
                m_agent.SetDestination(vDestPos);

            }

            if (m_agent.remainingDistance <= 0.01)
            {
                m_isRuning = false;
                m_agent.enabled = false;
                
                //通知移动结束
                EventParameter param = EventParameter.Get();
                param.goParameter = this.gameObject;
               
                m_eventMgr.TriggerEvent(GameEvent.GE_ACTOR_MOVE_END, param); 
            }

             if (m_agent.enabled)
                fRemainPath = m_agent.remainingDistance;

            Debug.DrawLine(m_transform.position, m_dstPos, Color.red);
        }	        
	}

    void BeginRun()
    {
        CancelInvoke("BeginRun");

        m_isRuning = true;        
    }

    public override void Stop(bool isSendEvent)
    {
        m_isRuning = false;
        m_agent.enabled = false;

        if (isSendEvent)
        {
            //通知移动结束
            EventParameter param = EventParameter.Get();
            param.goParameter = this.gameObject;

            m_eventMgr.TriggerEvent(GameEvent.GE_ACTOR_MOVE_END, param); 
        }
    }

    public override void SetSpeed(float speed) 
    { 
        m_agent.speed = speed;
    }

    public override float GetSpeed()
    {
        return m_agent.speed ;
    }
}

};  //end SG

