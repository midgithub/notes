/**
* @file     : CollideDamageCell.cs
* @brief    : 
* @details  : 碰撞伤害元素
* @author   : 
* @date     : 2014-12-1
*/

using XLua;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{

[Hotfix]
public class CollideDamageCell : ISkillCell {    
    public CollideDamageDesc m_collideDamage = null;               
    private SkillBase m_skillBase = null;   
    //private bool m_startDamage = false;

    private List<Transform> m_aimTransformList = new List<Transform>();

    public override void Init(ISkillCellData cellData, SkillBase skillBase)
    {
        m_skillBase = skillBase;
        m_collideDamage = (CollideDamageDesc)cellData;
    }
    
    // PoolManager
    void OnEnable()
    {
        CancelInvoke("Start");
        Invoke("Start", 0.000001f);
    }

    void OnDisable()
    {
        CancelInvoke("Start");
    }


	// Use this for initialization
	void Start () 
	{
        CancelInvoke("Start");

        ////伤害计算
        //m_skillBase = this.transform.root.gameObject.GetComponent<SkillBase>();      

        if (m_skillBase == null || m_skillBase.m_actor == null)
            return;
           
        if (m_collideDamage == null)
        {
            return;
        }

        float curActionTime = m_skillBase.GetCurActionTime();
        float diffTime = m_collideDamage.endCarryOffTargetTime - curActionTime;
        if (diffTime > 0)
        {
            Invoke("EndCarryOffTarget", diffTime);
        }        
                
        RegisterEvent();
	}
	
    void RegisterEvent()
    {
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_MOVE_COLLIDE_TARGET, EventFunction);            
    }
    
    void RemoveEvent()
    {
        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_MOVE_COLLIDE_TARGET, EventFunction);
    }

    void EventFunction(GameEvent ge, EventParameter parameter)
    {
        switch (ge)
        {
            case GameEvent.GE_MOVE_COLLIDE_TARGET:
                {
                    GameObject obj = parameter.goParameter;
                    ActorObj actorBase = obj.GetComponent<ActorObj>();

                    //临时判断
                    if (actorBase.mActorType == m_skillBase.m_actor.mActorType)
                    {
                        break;
                    }


                    if (!CoreEntry.gGameMgr.IsPvpState() && !m_skillBase.m_actor.IsAimActorType(actorBase))
                    {
                        break;
                    }

                    if (!IsAimGameObject(obj.transform))
                    {
                        m_aimTransformList.Add(obj.transform);

                        DamageParam damageParam = new DamageParam();
                        damageParam.skillID = m_skillBase.m_skillID;
                        damageParam.attackActor = m_skillBase.m_actor;
                        damageParam.behitActor = actorBase;                        

                        CoreEntry.gSkillMgr.OnSkillDamage(damageParam);
                    }                                         
                }
                break;
            default:
                break;
        }
    }

    void OnDestroy()
    {
        RemoveEvent();

        CancelInvoke("EndCarryOffTarget");        
    }

    void EndCarryOffTarget()
    {
        CancelInvoke("EndCarryOffTarget");

        m_skillBase.SendEvent(SkillCellEventType.SE_STOP_CARRYOFF_TARGET, null);            
    }
   
    //已经伤害了，就不在伤害了
    bool IsAimGameObject(Transform aimTransform)
    {
        for (int i = 0; i < m_aimTransformList.Count; ++i)
        {
            if (m_aimTransformList[i] == aimTransform)
            {
                return true;
            }                                                        
        }
                        
        return false;
    }  

    bool IsAimActorType(ActorType aimActorType)
    {
        for (int i = 0; i < m_collideDamage.aimActorTypeList.Count; ++i)
        {
            if (aimActorType == (ActorType)m_collideDamage.aimActorTypeList[i])
            {
                return true;
            }
        }   

        return false;
    }  
}
};  //end SG

