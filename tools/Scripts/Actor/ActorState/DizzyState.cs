/**
* @file     : DizzyState.cs
* @brief    : 
* @details  : 眩晕状态，外部触发
* @author   : zerodeng
* @date     : 2014-12-25
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
public class DizzyParam
{
    public float keepTime;        
};
 
[Hotfix]
public class DizzyState : IActorState {
    
    DizzyParam m_dizzyParam = null;
    GameObject efxObj = null;

    BehitState m_beHitState = null;  // 嵌入受击状态，眩晕时也可以有受击表现
    string dizzyAction;

    public override void OnEnter(ActorObj actorBase)
    {
        m_state = ACTOR_STATE.AS_DIZZY;

        m_actor = actorBase;        
        m_dizzyParam = m_actor.dizzyParam;
    
        //播放动作
        dizzyAction = "hit014"; //m_actor.actorCreatureDisplayDesc.DizzyAction;
        if (dizzyAction.Length <= 0)
        {
            //没有，就使用默认的动作
            dizzyAction = m_actor.ModelConfig.Get<string>("san_walk");
        }

        if (m_actor.IsPlayingAction(dizzyAction) == false)
        {
            m_actor.StopAll(); // 当前动作
            m_actor.PlayAction(dizzyAction);
        }

        string dizzyEfx = "Effect/skill/buff/fx_bf_xuanyun"; //;m_actor.actorCreatureDisplayDesc.DizzyEfx;
        if (dizzyEfx.Length > 0)
        {
            if (efxObj == null )
            {
                Object tmpObj = CoreEntry.gResLoader.LoadResource(dizzyEfx);
                if (tmpObj != null)
                {
                    efxObj = Instantiate(tmpObj) as GameObject;
                }

                SceneEfx sceneEfx = efxObj.AddComponent<SceneEfx>();
                sceneEfx.Init(m_actor.transform.position, m_dizzyParam.keepTime);

                //设置有挂点的特效            
                Transform[] childTransform = efxObj.GetComponentsInChildren<Transform>();
                for (int i = 0; i < childTransform.Length; i++)
                {
                    Transform childTrans = childTransform[i];
                    EfxSetAttachPoint setAttach = childTrans.gameObject.GetComponent<EfxSetAttachPoint>();
                    if (setAttach == null || setAttach.m_attachPointEnum == AttachPoint.E_None)
                    {
                        continue;
                    }

                    setAttach.Init(false);

                    Transform parent = m_actor.GetChildTransform(setAttach.m_attachPointEnum.ToString());
                    if (parent != null)
                    {
                        // 因为 top 绑定会旋转所以这里取了相对位置
                        childTrans.parent = m_actor.transform;
                        childTrans.localPosition = new Vector3(0, parent.localPosition.y, 0);
                        childTrans.localRotation = Quaternion.identity;
                        childTrans.localScale = Vector3.one;

                        SceneEfx sceneEfx1 = childTrans.gameObject.AddComponent<SceneEfx>();
                        sceneEfx1.Init(childTrans.position, m_dizzyParam.keepTime);
                    }
                }
            }
        }
              
        //状态硬直，不可转换其他状态
        m_isNonControl = true;

        CancelInvoke("AutoExitState");
        Invoke("AutoExitState", m_dizzyParam.keepTime);

        //EventToUI.SetArg(UIEventArg.Arg1, gameObject);
        //EventToUI.SetArg(UIEventArg.Arg2, (float)m_dizzyParam.keepTime);
        //EventToUI.SendEvent("EU_ENTER_DIZZY");
    }

    public override void OnExit(StateParameter stateParm)
    {
        CancelInvoke("ExitBehitState");
        CancelInvoke("AutoExitState");

        if (stateParm.state != m_state)
        {
            if (efxObj)
                Destroy(efxObj);
        }
    }

    public override bool CanChangeState(StateParameter stateParm)
    {
        if (stateParm.state == ACTOR_STATE.AS_DEATH)
        {
            return true;
        }
        if (stateParm.state == ACTOR_STATE.AS_BEHIT)
        {
            if (m_beHitState == null)
            {
                m_beHitState = m_actor.m_BehitState;
                m_beHitState.OnEnter(m_actor);
                // 保证受击动作播完
                BehitState.BehitInfo info = m_beHitState.GetBeHitInfo(m_actor.resid, m_beHitState.m_behitParame.damgageInfo.skillID);
                CancelInvoke("ExitBehitState");
                Invoke("ExitBehitState", info.NonControlTime);
            }
        }
        
        return !m_isNonControl;
    }    
        
    void AutoExitState()
    {
        LogMgr.LogError("DizzState, AutoExitState");
        CancelInvoke("ExitBehitState");
        CancelInvoke("AutoExitState");
        m_isNonControl = false;

        StateParameter param = new StateParameter();
        param.state = ACTOR_STATE.AS_STAND;
        m_actor.RequestChangeState(param);           
    }

    public override void FrameUpdate()
    {
        if (m_beHitState!=null)
            m_beHitState.FrameUpdate();
    }

    // 保证受击动作播完
    public void ExitBehitState()
    {
        CancelInvoke("ExitBehitState");
        if (m_beHitState)
        {
            StateParameter stateParm = new StateParameter();
            m_beHitState.OnExit(stateParm);
            m_actor.StopAll(); // 当前动作
            m_actor.PlayAction(dizzyAction);
            m_beHitState = null;
        }
    }
}

};  //end SG

