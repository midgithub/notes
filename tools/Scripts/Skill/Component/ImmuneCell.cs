/**
* @file     : ImmuneCell.cs
* @brief    : 
* @details  : 技能无敌元素
* @author   : 
* @date     : 2014-12-9
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
public class ImmuneCell : ISkillCell {
    private float m_keepTime = 0;
    SkillBase m_skillBase = null;    
    ImmuneCellDesc m_immuneCellDesc = null;

    public override void Init(ISkillCellData cellData, SkillBase skillBase)
    {
        m_skillBase = skillBase;
        m_immuneCellDesc = (ImmuneCellDesc)cellData;

        m_keepTime = m_immuneCellDesc.keepTime;
        Invoke("StartImmune", m_immuneCellDesc.startTime);
    }

	// Use this for initialization
	void Start () 
	{
        //m_skillBase = this.transform.root.gameObject.GetComponent<SkillBase>();        
	}
	
	// Update is called once per frame
	//void Update () {
	
	//}
    

    void StartImmune()
    {
        CancelInvoke("StartImmune");

        if (m_skillBase != null && m_skillBase.m_actor != null )
        {
            m_skillBase.m_actor.RecoverHealth(m_keepTime);
        }                             
    }
}

};  //end SG

