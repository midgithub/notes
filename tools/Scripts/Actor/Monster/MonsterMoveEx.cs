/**
* @file     : MonsterMoveEx.cs
* @brief    : 
* @details  : 怪物移动扩展版本：使用寻路导航算出路径，但行走采用基本移动
* @author   : 
* @date     : 2014-9-25
*/

using XLua;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if !UNITY_5_3_8
using UnityEngine.AI;
#endif
namespace SG
{

[Hotfix]
public class MonsterMoveEx : IMove{

    private Transform m_transform;
   
    private EventMgr m_eventMgr = null;
    private bool m_isRuning = false;    
    private float m_startTime = 0;    

    private List<Vector3> m_listPath = new List<Vector3>();    

    //移动
    public override void MovePos(Vector3 pos)
    {
        //add by lzp 
        if (!CoreEntry.GameStart)
            return;
        if (m_agent == null)
        {
            //Debug.LogError("NavMeshAgent is null");
            return;
        }

        //计算路径点
        m_agent.enabled = true;
        NavMeshPath path = new NavMeshPath();

        m_listPath.Clear();
        bool ret = m_agent.CalculatePath(pos, path);
        if (ret)
        {
            if (path.corners.Length < 2)
            {
                return;
            }

            for (int i = 0; i < path.corners.Length; ++i)
            {
                m_listPath.Add(path.corners[i]);
            }

            m_isRuning = true;
            m_startTime = Time.time;                               
        }
        else
        {
            //Debug.LogError("CalculatePath() error! pos="+m_transform.position.ToString("f4"));
        }

        m_agent.enabled = false;

        if (m_isRuning)
        {
            //开始移动
            EventParameter param = EventParameter.Get();
            param.goParameter = this.gameObject;

            m_eventMgr.TriggerEvent(GameEvent.GE_ACTOR_MOVE_BEGIN, param); 
        }
            
    }

    void Awake()
    {
        m_agent = this.GetComponent<NavMeshAgent>();
        m_agent.enabled = true;

        m_transform = this.transform;  

        m_eventMgr = CoreEntry.gEventMgr;      
    }
	
	// Update is called once per frame
	void Update () {
        if (m_isRuning)
        {            
            Vector3 srcPos = m_listPath[0];
            Vector3 dstPos = m_listPath[1];

            float needTime = Vector3.Distance(srcPos, dstPos) / 2f;

            float diff = Time.time - m_startTime;
            float ratio = diff / needTime;
            if (ratio > 1f)
            {
                ratio = 1f;
            }

            m_transform.position = Vector3.Lerp(srcPos, dstPos, ratio);

            m_transform.LookAt(new Vector3(dstPos.x, m_transform.position.y, dstPos.z));

            if (Vector3.Distance(m_transform.position, dstPos) <= 0.01f)
            {
                if (m_listPath.Count <= 2)
                {
                    m_listPath.Clear();
                    m_isRuning = false;
                                        
                    //通知移动结束
                    EventParameter param = EventParameter.Get();
                    param.goParameter = this.gameObject;

                    m_eventMgr.TriggerEvent(GameEvent.GE_ACTOR_MOVE_END, param);                         
                }
                else
                {
                    m_startTime = Time.time;
                    m_listPath.RemoveAt(0);
                }
            }
        }

        if (m_listPath.Count > 0)
        {
            for (int i = 0; i < m_listPath.Count - 1; ++i)
            {
                Debug.DrawLine(m_listPath[i], m_listPath[i + 1], Color.red);
            }
        }        	

	}
}

};  //end SG

