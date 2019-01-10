/**
* @file     : 
* @brief    : 
* @details  : 
* @author   : 
* @date     : 2014-12-25
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
    public class TimerStateParam
    {
        public float keepTime;
        public int iState;
       
    };

[Hotfix]
    public class TimerState : IActorState
    {
        
        protected float deltime = 0;
        /// <summary>
        /// 多长时间可以走动一次
        /// </summary>
        protected float RunFreq = 1.0f;
        public TimerStateParam Param = null;

        public override void OnEnter(ActorObj actorBase)
        {
            deltime = 0;
            m_actor = actorBase;
            ResetTimer();

        }

        public virtual void ResetTimer()
        {
            if (Param != null && Param.keepTime > 0)
            {
                CancelInvoke("ExitingTimer");
                Invoke("ExitingTimer", Param.keepTime * 0.001f);
            }
        }


        protected virtual void PreExiting()
        {
            CancelInvoke("ExitingTimer");
            if (Param != null)
            {
                Param.iState = 0;
            }
        }

        protected void ExitingTimer()
        {
            PreExiting();

            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_STAND;
            m_actor.RequestChangeState(param);
        }

        public override void FrameUpdate()
        {
            base.FrameUpdate();
            deltime += Time.deltaTime;
            if (deltime > RunFreq)
            {
                deltime -= RunFreq;
                OnRun();
            }
        }

        protected virtual void OnRun()
        {

        }

        public override void OnExit(StateParameter stateParm)
        {
            PreExiting();
        }

        public override bool CanChangeState(StateParameter stateParm)
        {
            if (stateParm.state == ACTOR_STATE.AS_DEATH)
                return true;

            //if (stateParm.state == ACTOR_STATE.AS_RUN) // 移动
            //    OnRun();

            if (stateParm.state == ACTOR_STATE.AS_BEHIT) // 受击
                OnBeHit();

            return (Param == null || Param.iState == 0);
        }

        protected virtual void OnBeHit()
        {
            m_actor.PlayAction("hit002");
        }
    }

};  //end SG

