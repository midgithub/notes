/**
* @file     : AttackBreakCell.cs
* @brief    : 
* @details  : 打断吟唱阶段
* @author   : 
* @date     : 2014-12-22
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
public class BreakPrepareCell : ISkillCell{
    SkillBase m_skillBase = null;
    //GameObject m_damageUIObj = null;
    int m_hadDamageValue = 0;
    int m_breakDamageHP = 0;
    BreakPrepareDesc m_attackBreakDesc = null;

    //吟唱时间
    private float m_prepareKeepTime = 0;           
    private float m_updateCount = 0;

    public override void Init(ISkillCellData cellData, SkillBase skillBase) 
    {
        m_skillBase = skillBase;
        m_attackBreakDesc = (BreakPrepareDesc)cellData;
        m_hadDamageValue = 0;
        m_breakDamageHP = 0;
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
        //m_skillBase = this.transform.root.gameObject.GetComponent<SkillBase>();
        if (m_skillBase == null || m_skillBase.m_actor == null)
            return;
  
        m_prepareKeepTime = m_skillBase.prepareKeepTime;

        if (m_attackBreakDesc == null )
        {
            return;
        }       

        if (m_attackBreakDesc.breakDamageHP <= 0)
        {
            return;
        }
        m_breakDamageHP = m_attackBreakDesc.breakDamageHP;

        //开始等待伤害
        m_hadDamageValue = 0;

        //注册事件  
        RegisterEvent();

        if (m_attackBreakDesc.isUsedUI)
        {
            EventToUI.SetArg(UIEventArg.Arg1, 1f);
            EventToUI.SetArg(UIEventArg.Arg2, m_prepareKeepTime);
            EventToUI.SendEvent("EU_DPSBAR_SHOW");     
                  
            m_updateCount = 0;
            InvokeRepeating("UpdateShowTime", 1, 1);     
        }
        

        //加载UI
        //if (m_attackBreakDesc.UIPrefabPath.Length <= 0)        
        //{
        //    return;
        //}
                   
        //m_damageUIObj = Instantiate(
        //        CoreEntry.gResLoader.LoadResource(m_attackBreakDesc.UIPrefabPath)) as GameObject;                               
        
	}

	
	// Update is called once per frame
    //void Update()
    //{
    //}   

    void OnDestroy ()
    {
        UnRegisterEvent();

        if (m_attackBreakDesc.isUsedUI)
        {
            CancelInvoke("UpdateShowTime");
            EventToUI.SendEvent("EU_DPSBAR_HIDE");
        }
    }    

    void UpdateShowTime()
    {
        m_updateCount++;

        float diffTime = m_prepareKeepTime - m_updateCount;
        if (diffTime < 0)
        {
            diffTime = 0;
        }        

        diffTime = Mathf.Floor(diffTime);

        EventToUI.SetArg(UIEventArg.Arg1, diffTime);
        EventToUI.SendEvent("EU_DPSBAR_SET_TIME_VAULE");            
    }

    void RegisterEvent()
    {
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_DAMAGE_CHANGE, EventFunction);
    }

    void UnRegisterEvent()
    {
        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_DAMAGE_CHANGE, EventFunction);
    }

    void EventFunction(GameEvent ge, EventParameter parameter)
    {
        switch (ge)
        {
            case GameEvent.GE_DAMAGE_CHANGE:
                {                   
                    int entityID = parameter.intParameter;
                    if (entityID != m_skillBase.m_actor.entityid)
                    {
                        return;
                    }                                        
            
                    int hp = parameter.intParameter1;
                    m_hadDamageValue += hp;                    
                    //更新UI
                    if (m_attackBreakDesc.isUsedUI)
                    {
                        float ratio = (m_breakDamageHP - m_hadDamageValue)*1.0f / m_breakDamageHP;
                        //LogMgr.UnityLog("ratio=" + ratio);                        
                        if (ratio < 0)
                        {
                            ratio = 0;
                        }                                                   

                        EventToUI.SetArg(UIEventArg.Arg1, ratio);                        
                        EventToUI.SendEvent("EU_DPSBAR_SET_PERCENT_VAULE");   
                    }

                    //是否达到目标伤害
                    if (m_hadDamageValue >= m_breakDamageHP)
                    {
                        UnRegisterEvent();
                                                                                   
                        OnBreakSkill(); 
                    }                                                                 
                }
                break;
            default:
                break;
        }
    }

    //达到伤害值，中断技能
    void OnBreakSkill()
    {
        //打断吟唱
        m_skillBase.BreakPrepareSkill();

        UnRegisterEvent();

        if (m_attackBreakDesc.isUsedUI)
        {
            CancelInvoke("UpdateShowTime");
            EventToUI.SendEvent("EU_DPSBAR_HIDE");
        }

        if (m_attackBreakDesc.behitAction.Length == 0)
        {
            m_skillBase.SkillEnd();
            return;
        }
        
        //播放被击动作
        m_skillBase.m_actor.StopAll();
        m_skillBase.m_actor.PlayAction(m_attackBreakDesc.behitAction);
                                    
        //动作完后结束技能        
        Invoke("SkillEnd", m_skillBase.m_actor.GetActionLength(m_attackBreakDesc.behitAction));
    }

    void SkillEnd()
    {
        CancelInvoke("SkillEnd");

        m_skillBase.SkillEnd();
    }
}

};  //end SG

