/**
* @file     : DizzyState.cs
* @brief    : 
* @details  : 限制 移动、技能
* @author   : 
* @date     : 2014-12-25
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
    public class SheepParam : TimerStateParam
    {
        public int iResID;
    };

[Hotfix]
    public class SheepState : TimerState
    {
        public ActorObj m_SheepActor = null;
        GameObject m_SheepObj = null;


        public override void OnEnter(ActorObj actorBase)
        {

            base.OnEnter(actorBase);
            m_state = ACTOR_STATE.AS_SHEEP;

            RunFreq = 2.0f;

            m_actor.HideSelf();
            m_actor.HideEffect();

            //Configs.CreatureDisplayDesc ActorDisplay = CsvMgr.GetRecordCreatureDisplayDesc(m_actor.sheepParam.iResID);//
            //m_SheepObj = CoreEntry.gObjPoolMgr.ObtainObject(m_actor.sheepParam.iResID);
            //if (m_SheepObj == null)
            //{
            //    if (ActorDisplay != null)
            //    {
            //        Object tmpObj = CoreEntry.gResLoader.LoadResource(ActorDisplay.modelPath);
            //        if (tmpObj != null)
            //        {
            //            m_SheepObj = (GameObject)Instantiate(tmpObj);
            //        }
            //    }

            //}
            //if (m_SheepObj)
            //{
            //    ActorObj actorObject = m_SheepObj.GetComponent<ActorObj>();

            //    actorObject.Init(m_actor.sheepParam.iResID, CoreEntry.gBaseTool.GetIntQueueValue(), null);
            //    actorObject.HideBlobShadow();
             
            //    m_SheepActor = actorObject;

            //    m_SheepObj.transform.parent = m_actor.gameObject.transform;
            //    m_SheepObj.transform.localPosition = Vector3.zero;
            //    m_SheepObj.transform.localRotation = Quaternion.identity;

            //    MonsterRangeAgent actorAI2 = m_SheepObj.GetComponent<MonsterRangeAgent>();
            //    if (actorAI2)
            //        actorAI2.enabled = false;// 关掉AI
            //}

        }

          

        public override void FrameUpdate()
        {
 

            base.FrameUpdate();
        }

        protected override void OnRun()
        {
            Debug.LogError("=======设置跑步速度========");
            m_actor.StopMove(false);

            m_actor.PlayAction(m_actor.ModelConfig.Get<string>("san_walk"));

            //设置跑步速度
            if (m_actor.mBaseAttr.Speed > 0)
                m_actor.SetSpeed(m_actor.mBaseAttr.Speed * 0.3f);

            Vector3 pos = new Vector3();
            pos = Random.insideUnitSphere;
            pos.y = 0;
            pos.Normalize();
            pos = m_actor.gameObject.transform.position + pos * Random.Range(2f, 5f);
            m_actor.FaceTo(pos);
            m_actor.m_move.MovePosition(pos);

            // 播放变形模型的动作
            m_SheepActor.PlayAction(m_SheepActor.ModelConfig.Get<string>("san_walk"));
            m_SheepActor.SetSpeed(0);
        }

        protected override void OnBeHit()
        {
            // 如果在站立状态就播放受击动作

            // 播放变形模型的动作
            m_SheepActor.PlayAction("stand");
            m_actor.StopMove(false);
        }

        public override void OnExit(StateParameter stateParm)
        {
            base.OnExit(stateParm);

            m_actor.ShowSelf();
            m_actor.StopMove(false);
            m_actor.ShowEffect();


            //设置跑步速度
            if (m_actor.mBaseAttr.Speed > 0)
                m_actor.SetSpeed(m_actor.mBaseAttr.Speed);

            if (m_SheepObj)
            {
                DestroyObject(m_SheepObj);
                m_SheepObj = null;
            }
        }

        void ShowEffect()
        {
            m_actor.ShowEffect();
        }

        public override bool CanShowSelf() { return false; }

    }

};  //end SG

