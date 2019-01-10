 

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
public class FitState : IActorState
{
   
    //private GameObject m_cylinderObj = null;   

    public override void OnEnter(ActorObj actorBase)
    {
        m_state = ACTOR_STATE.AS_FIT;

        m_actor = actorBase;
        //策划不配，硬代码
        string enterAction = "fit";


        float endTime = m_actor.GetActionLength(enterAction);

        if (enterAction == "stand")
        {
            endTime = 0.5f;
        }


        if (endTime == 0)
        {
            endTime = 0.5f;
        }


        m_actor.PlayAction(enterAction, false);

         

        //无敌状态，受击没反映
        float immuneTime = endTime;
        //if (m_actor.actorCreatureDisplayDesc.enterImmuneKeepTime > 0)
        //{
        //    immuneTime = m_actor.actorCreatureDisplayDesc.enterImmuneKeepTime;            
        //}
        m_actor.RecoverHealth(immuneTime);

        //硬直状态，自己不能切换状态
        m_isNonControl = true;

        Invoke("EndEnterState", endTime);

		//去掉碰撞
		m_actor.CancelCollider ();


       
    }

    //public override void FrameUpdate()
    //{

    //}

    public override void OnExit(StateParameter stateParm)
    {
        ////入场结束，去掉对应的绳子                
        //if (m_cylinderObj != null)
        //{
        //    m_cylinderObj.active = false;
        //}  
		//去掉碰撞
		m_actor.RecoverCollider ();
    }

    public override bool CanChangeState(StateParameter stateParm)
    {
        if (stateParm.state == ACTOR_STATE.AS_DEATH || stateParm.state == ACTOR_STATE.AS_STONE)
        {
            return true;
        }

        return !m_isNonControl;
    } 
    public void EndEnterState()
    {
        CancelInvoke("EndEnterState");

        m_isNonControl = false;

        StateParameter param = new StateParameter();
        param.state = ACTOR_STATE.AS_STAND;
        m_actor.RequestChangeState(param);

        //发送消息，激活状态
        EventParameter eventParam = EventParameter.Get();
        eventParam.intParameter = m_actor.entityid;
        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ACTOR_CAN_BEHIT, eventParam);    
    }
}

};  //end SG

