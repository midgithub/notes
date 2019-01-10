using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
    public class StoneState : IActorState
    {
       
        public override void OnEnter(ActorObj actorBase)
        {
            m_state = ACTOR_STATE.AS_STONE;

            m_actor = actorBase;

            //m_actor.PlayAction("stand");
            m_actor.GetComponent<Animation>().Stop();


        }

        //能否切换状态
        public override bool CanChangeState(StateParameter stateParm)
        {
            return false;
        }



        public override void OnExit(StateParameter stateParm)
        {

        }
    }

};  //end SG

