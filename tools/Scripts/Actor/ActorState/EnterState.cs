/**
* @file     : EnterState.cs
* @brief    : 
* @details  : 入场状态，出生的时候调用
* @author   : 
* @date     : 2014-10-24
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
public class EnterState : IActorState
{
   
    //private GameObject m_cylinderObj = null;   

    public override void OnEnter(ActorObj actorBase)
    {
        m_state = ACTOR_STATE.AS_ENTER;

        m_actor = actorBase;

        string enterAction = m_actor.enterAction;


        ////删除挂点
        //Transform cylinderTransform = this.gameObject.transform.FindChild("Cylinder002");
        //if (cylinderTransform != null)
        //{
        //    m_cylinderObj = cylinderTransform.gameObject;
        //    m_cylinderObj.active = true;
        //}      

        float endTime = m_actor.GetActionLength(enterAction);

        //if (endTime > 0.5f)
        //{
        //    endTime = endTime - 0.2f;
        //}


        if (enterAction == "stand")
        {
            endTime = 0.5f;
        }


        if (endTime == 0)
        {
            endTime = 0.5f;
        }


        m_actor.PlayAction(enterAction, false);

        //播放特效
        string enterEfx = "";// m_actor.actorCreatureDisplayDesc.enterEfx;

        if (CoreEntry.IsMobaGamePlay() == false && enterEfx != "")
        {
            if (m_actor.mActorType == ActorType.AT_MONSTER || m_actor.mActorType == ActorType.AT_MECHANICS)
            {
                enterEfx = "Effect/skill/remain/fx_xiaobingchuxian01";
            }
            if (m_actor.mActorType == ActorType.AT_BOSS)
            {
                enterEfx = "Effect/skill/remain/fx_bosschuxian";
            }

            if (enterEfx.Length > 0)
            {
                GameObject efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(enterEfx);
                //GameObject efxObj = (GameObject)Instantiate(CoreEntry.gResLoader.LoadResource(enterEfx));//CoreEntry.gGameObjPoolMgr.InstantiateEffect(enterEfx);
                if (efxObj != null)
                {
                    SceneEfxPool efx = null;
                    if (efxObj)
                        efx = efxObj.GetComponent<SceneEfxPool>();
                    if (efx == null)
                        efx = efxObj.AddComponent<SceneEfxPool>();
                    efx.Init(m_actor.transform.position, endTime);
                }
            }
        }

       

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


    public void EndEnterStateFromCamera(ActorObj actorbase)
    {
        if (m_actor == null && actorbase != null)
        {
            m_actor = actorbase;
        }

        EndEnterState();
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

