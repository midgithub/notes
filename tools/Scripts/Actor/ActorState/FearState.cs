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
    public class FearParam : TimerStateParam
    {
    };

[Hotfix]
    public class FearState : TimerState
    {
        public override void OnEnter(ActorObj actorBase)
        {
            base.OnEnter(actorBase);
            m_state = ACTOR_STATE.AS_FEAR;
        }
       

         protected override void OnRun()
        {
            ////恐惧的时候朝某个方向
            //Vector3 vDir = m_actor.transform.forward; 
            //Vector3 vNavPos = m_actor.transform.position + vDir;

            ////判断这个位置是否是有个有效的点
            //NavMeshHit hit;
            //if (!NavMesh.SamplePosition(m_actor.NavPos, out hit, 100.0f, -1))
            //{
            //    vNavPos = m_actor.transform.position + vDir;
            //}

            Debug.LogError("===============OnRun=================");
            Vector3 pos = new Vector3();
            pos = Random.insideUnitSphere;
            pos.y = 0;
            pos.Normalize();
            pos = m_actor.gameObject.transform.position + pos * Random.Range(2f, 5f);

            m_actor.FaceTo(pos);
            m_actor.m_move.MovePosition(pos);
            m_actor.PlayAction("run");
             
        }
       
    }

};  //end SG

