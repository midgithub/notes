using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
public class StandState : IActorState {

    float lastTime = 0.0f;
    string standAin = "stand";
    string idleAni = "idle";
    public override void OnEnter(ActorObj actor)
    {
        m_state = ACTOR_STATE.AS_STAND;

        m_actor = actor;

        lastTime = Time.time;

        standAin = "stand";
        idleAni = "idle";
        if (m_actor.ModelConfig != null)
        {
            if (!string.IsNullOrEmpty(m_actor.ModelConfig.Get<string>("san_idle")))
            {
                standAin = m_actor.ModelConfig.Get<string>("san_idle");
            }
            if (!string.IsNullOrEmpty(m_actor.ModelConfig.Get<string>("san_born")))
            {
                idleAni = m_actor.ModelConfig.Get<string>("san_born");
            }
        }
        m_actor.PlayAction(standAin);      
    }

    public override void FrameUpdate()
     {
            if (Time.time - lastTime > 10)
            {
                if (m_actor.IsHadAction(idleAni) && (!m_actor.IsRiding && !m_actor.IsWing))
                    m_actor.PlayAction(idleAni);
                
                lastTime = Time.time;
            }
            else
            {
                if (m_actor.mActorType == ActorType.AT_NPC)
                {
                    if (!m_actor.IsPlayingAction(idleAni))
                    {
                        m_actor.PlayAction(standAin);
                    }
                }
                else
                {
                    m_actor.PlayAction(standAin);
                }
            }

        }

       
    }

};  //end SG

