using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{    
    //玩家移动
[Hotfix]
public class PlayerMove : IMove {
    protected CharacterController m_ch;
    protected Transform m_transform; 

    //默认速度
    protected float m_speed = 6f;

    //private Vector3 m_curTouchDir = Vector3.zero;
    protected Vector3 m_dstPos = Vector3.zero;
    protected Vector3 m_dstDir = Vector3.zero;

    protected EventMgr m_eventMgr = null;
    static protected GameObject fx_renwuzhixiang = null;

    void Awake()
    {
        m_ch = this.GetComponent<CharacterController>();
        m_transform = this.transform;

        if (fx_renwuzhixiang == null)
        {
			Object obj = CoreEntry.gResLoader.Load ("Effect/skill/remain/fx_jiaodiguanghuan");
			if (obj == null) {
                    LogMgr.LogError ("找不到 prefab: " + "Effect/skill/remain/fx_jiaodiguanghuan");
				return;
			}

            fx_renwuzhixiang = GameObject.Instantiate(obj) as GameObject;
            fx_renwuzhixiang.transform.parent = gameObject.transform;
            fx_renwuzhixiang.transform.localPosition = new Vector3(0.0f,0.1f,0.0f);
            fx_renwuzhixiang.transform.localRotation = Quaternion.identity;
            fx_renwuzhixiang.SetActive(true);
        }
    }

    // Use this for initialization
    void Start()
    {
        m_eventMgr = CoreEntry.gEventMgr;        
    }

    void OnEnable()
    {
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_CHANGE_MAIN_PLAYER, OnGE_CHANGE_MAIN_PLAYER);
    }
    void OnDisable()
    {
        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_CHANGE_MAIN_PLAYER, OnGE_CHANGE_MAIN_PLAYER);

    }

    // Update is called once per frame
    void Update()
    {
        if (m_dstPos != Vector3.zero)
        {
            float distance = Vector3.Distance(m_transform.position, m_dstPos);

            Vector3 curDir = m_dstPos - m_transform.position;
            curDir.Normalize();

            //方向点乘，防止走过头
            float dot = Vector3.Dot(curDir, m_dstDir);

            if (distance <= 0.1f || dot < 0)
            {
                m_dstPos = Vector3.zero;

                //todo:移动到目的地 
                EventParameter param = EventParameter.Get();
                param.goParameter = this.gameObject;

                m_eventMgr.TriggerEvent(GameEvent.GE_PLAYER_MOVE_END, param);
                //renwuzhixiangSetActive(false);
                return;
            }

            //移动                                              
            MoveToDir(m_dstDir);

            Debug.DrawLine(m_transform.position, m_dstPos, Color.red);
        }
        else
        {
            Debug.DrawLine(m_transform.position, m_dstPos, Color.red);

        }
    }

    public override void MovePos(Vector3 dstPos)
    {
        
        //add by lzp 
        if (!MapMgr.Instance.InMainCity)
        {
            if (!CoreEntry.GameStart || !CoreEntry.InFightScene)
                return;
        }
 
  
        //todo:开始移动
        m_dstPos = dstPos;

        m_dstDir = dstPos - m_transform.position;
        m_dstDir.Normalize();

        EventParameter param = EventParameter.Get();
        param.goParameter = this.gameObject;
        
        m_eventMgr.TriggerEvent(GameEvent.GE_PLAYER_MOVE_BEGIN, param);
        renwuzhixiangSetActive(true);

    }

    public override void MoveDir(Vector3 dir)
    {
        //保存当前的朝向                                             
        //m_curTouchDir = dir;
        //m_curTouchDir.Normalize();

        OnMove(dir);             
    }

    public override void Stop(bool isSendEvent)
    {
       if (m_eventMgr == null) return;
        m_dstPos = Vector3.zero;        
        
        if (isSendEvent)
        {
            //todo:通知，移动停止        
            EventParameter param = EventParameter.Get();
            param.goParameter = this.gameObject;

            m_eventMgr.TriggerEvent(GameEvent.GE_PLAYER_MOVE_STOP, param);
            //renwuzhixiangSetActive(false);
        }

        EventParameter param1 = EventParameter.Get();
        param1.objParameter = transform.position;

        ActorObj actor = gameObject.GetComponent<ActorObj>();

        if (actor && actor.IsMainPlayer())
        {
            if (m_eventMgr != null)
            {
                if (MapMgr.Instance.InMainCity)
                {
                    m_eventMgr.TriggerEvent(GameEvent.GE_PLAYER_MOVE_Arrive, param1);
                }
            }
        }
    }

    public override bool IsMoveDir()
    {
        return !m_curTouchDir.Equals(Vector3.zero);        
    }

    public override void LookAtDir()
    {
        if (!m_curTouchDir.Equals(Vector3.zero))
        {
            m_transform.rotation = Quaternion.LookRotation(m_curTouchDir.normalized);
        }                        
    }

    //移动，给定方向
    public bool OnMove(Vector3 dir)
    {    
        //todo:通知，进入移动状态
        EventParameter param = EventParameter.Get();
        param.goParameter = this.gameObject;
      
        m_eventMgr.TriggerEvent(GameEvent.GE_PLAYER_MOVE_BEGIN, param);
        renwuzhixiangSetActive(true);

        //移动
        MoveToDir(dir);
        return true; 
    }

    //移动，给定一个方向，每帧调用
    public void MoveToDir(Vector3 dir)
    {
        dir.Normalize();
        //m_transform.rotation = Quaternion.LookRotation(dir);
        RotateToDir(dir);
        m_ch.SimpleMove(dir * m_speed);        
    }

    //当进入触发器销毁所有物体
    public void OnTriggerEnter(Collider other)
    {
        //Destroy(other.gameObject);          m_ch.OnTriggerEnter = OnTriggerEnter;
        //LogMgr.UnityLog(other.gameObject.layer);     
    }

    public override void RotateToDir(Vector3 dir)
    {                
        //目标方向
        Vector3 dstPos = m_transform.position + dir.normalized * 3;

        Vector3 oldAngle = m_transform.eulerAngles;

        m_transform.LookAt(dstPos);

        float targetY = m_transform.eulerAngles.y;
        Mathf.MoveTowardsAngle(oldAngle.y, targetY, 600 * Time.deltaTime);

        //测试主角瞬间转身的效果
        m_transform.eulerAngles = new Vector3(oldAngle.x, targetY, oldAngle.z);
    }

    public override void SetSpeed(float speed) 
    { 
        m_speed = speed;
        if (m_agent != null)
        {
            m_agent.speed = speed;
        }
    }

    public override float GetSpeed()
    {
        return m_speed;
    }

    protected void renwuzhixiangSetActive(bool b)
    {
			//if (CoreEntry.gTeamMgr.MainPlayer == gameObject && fx_renwuzhixiang != null)
   //     {
			//	if (fx_renwuzhixiang != null && b==true&&fx_renwuzhixiang.transform.parent != gameObject.transform)
   //         {
   //             fx_renwuzhixiang.transform.parent = gameObject.transform;
   //             fx_renwuzhixiang.transform.localPosition = new Vector3(0.0f, 0.1f, 0.0f);
   //             fx_renwuzhixiang.transform.localRotation = Quaternion.identity;
   //         }
   //         fx_renwuzhixiang.SetActive(b);
   //     }
    }


    void OnGE_CHANGE_MAIN_PLAYER(GameEvent ge, EventParameter para)
    {
        renwuzhixiangSetActive(true); 
    }

}

};  //end SG

