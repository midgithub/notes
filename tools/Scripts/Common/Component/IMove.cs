using XLua;
﻿using UnityEngine;
using System.Collections;
#if !UNITY_5_3_8
using UnityEngine.AI;
#endif
namespace SG
{
            
//移动组件接口
[Hotfix]
public class IMove : MonoBehaviour {
    public Vector3 m_curTouchDir = Vector3.zero;
    protected NavMeshAgent m_agent = null;


  	public virtual void Init(){}
	public virtual bool CanArrived(Vector3 pos){return false;}

    public virtual void SetAgentEnable(bool bAgent)
    {
        //这里有时候会报错，先加个判空吧 add by Alex 20160606
        if (m_agent != null)
        {
            m_agent.enabled = bAgent;
        }
    }

    public virtual void SetMoveSpeed(float fSpeed) { }

    public virtual void MovePos(Vector3 pos){}
    public virtual void MoveDir(Vector3 dir){}
    public virtual void Stop(bool isSendEvent) {}
    public virtual bool IsMoveDir() {return false;}         //是否按方向移动(摇杆)
    public virtual void LookAtDir() {}                      //转动朝向    
    public virtual void SetSpeed(float speed) {}              //设置当前的速度   
    public virtual float GetSpeed() { return 0; }
    public virtual void RotateToDir(Vector3 dir) { }
    
    public virtual NavMeshAgent GetNavMeshAgent() { return m_agent; }

    public bool m_bPaused = false;
    public virtual void SetPaused(bool bPaused)
    {
        m_bPaused = bPaused;
    }

}

};  //end SG

