/**
* @file     : #FileName#
* @brief    : 
* @details  : 
* @author   : #Author#
* @date     : #Date#
*/
using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public class CollectState : IActorState
    {
#pragma warning disable 0108
        private ActorObj m_actor;
        public override void OnEnter(ActorObj actor)
        {
            if (null == actor)
            {
                return;
            }

            m_actor = actor;
            m_actor.GetDownHorse();
            m_actor.PlayAction("open");
            CancelInvoke("AutoExitState");
            float lastTime = m_actor.GetActionLength("open");
            Invoke("AutoExitState", lastTime);
        }

        public override bool CanChangeState(StateParameter stateParm)
        {
            return true;
        }

        private void AutoExitState()
        {
            CancelInvoke("AutoExitState");

            if(null == m_actor)
            {
                return;
            }

            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_STAND;
            m_actor.RequestChangeState(param);  
        }
    }
}

