using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
public class AimingState : IActorState {
   

    public override void OnEnter(ActorObj actor)
    {
        m_state = ACTOR_STATE.AS_AIMING;

        m_actor = actor;

        m_actor.PlayAction("stand");    

        if( CurParam != null  && CurParam.skillID > 0 )
        {//sss
         //   m_actor.OnShowSkillScope(CurParam.skillID);              
        }
    }

    //能否切换状态
    public override bool CanChangeState(StateParameter stateParm)
    {
        if (stateParm.state == ACTOR_STATE.AS_BEHIT)
        {
            return true;
        }
        else if (stateParm.state == ACTOR_STATE.AS_ATTACK)
        {
            return true;
        }

        return false;
    }

    //public override void FrameUpdate()
    //{
        
    //}

    public override void OnExit(StateParameter stateParm)
    {
        if (CurParam != null && CurParam.skillID > 0)
        {
            ModuleServer.MS.GSkillCastMgr.HideSkillScope(CurParam.skillID);
        }
    }
}

};  //end SG

