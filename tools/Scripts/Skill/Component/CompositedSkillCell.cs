/**
* @file     : CompositedSkillCell.cs
* @brief    : 
* @details  : 复合技能
* @author   : 
* @date     : 2014-12-9
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{

[Hotfix]
    public class CompositedSkillCell : ISkillCell
    {
        SkillBase m_skillBase = null;
        CompositedSkillCellDesc cellDesc = null;
        public List<SubSkillDesc> SkillList = new List<SubSkillDesc>();
        float timer = 0;

        public override void Init(ISkillCellData cellData, SkillBase skillBase)
        {
           
            m_skillBase = skillBase;
            cellDesc = (CompositedSkillCellDesc)cellData;

            SkillList.Clear();
            timer = 0;

            for (int i = 0; i < cellDesc.SkillList.Count; ++i)
            {
                SkillList.Add(cellDesc.SkillList[i]);
            }
          
        }

        public override void Preload(ISkillCellData cellData, SkillBase skillBase)
        {
            StateParameter stateParm = new StateParameter();
            stateParm.state = ACTOR_STATE.AS_DEATH;
            CompositedSkillCellDesc tmpCellDesc = (CompositedSkillCellDesc)cellData;
            for (int i = 0; i < tmpCellDesc.SkillList.Count; ++i)
            {
                int skillID = tmpCellDesc.SkillList[i].SkillId;
                LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(skillID);
                if (skillDesc == null)
                    continue;
                SkillClassDisplayDesc skillClass = CoreEntry.gGameDBMgr.GetSkillClassDisplayDesc(skillDesc.Get<int>("skillDisplayID"));
                if (skillClass == null)
                    continue;
                GameObject obj = CoreEntry.gGameObjPoolMgr.InstantiateSkillBase(skillClass.prefabPath);
                SkillBase tmpSkillBase = obj.GetComponent<SkillBase>();
                tmpSkillBase.Preload(null, skillID);
                stateParm.skillID = skillID;
                tmpSkillBase.BreakSkill(stateParm);
            }
        }

        List<SubSkillDesc> removedList = new List<SubSkillDesc>();
        void Update()
        {
            //只是显示则不放子技能
            if (SkillList.Count > 0 && m_skillBase.m_onlyShowSkillScope == false)
            {
                removedList.Clear();
                for (int i = 0; i < SkillList.Count; ++i)
                {
                    if (timer >= SkillList[i].DelayTime)
                    {
                        m_skillBase.m_actor.OnCastSkill(SkillList[i].SkillId);
                        //当前技能ID始终为组合技能ID
                        m_skillBase.m_actor.curCastSkillID = m_skillBase.m_skillID;
                        AttackState attackState = m_skillBase.m_actor.GetActorState(ACTOR_STATE.AS_ATTACK) as AttackState;
                        if (attackState != null)
                        {
                            attackState.m_curSkillBase.SubSkill = true;
                            attackState.m_curSkillBase = m_skillBase;
                            attackState.m_curSkillID = m_skillBase.m_skillID;
                        }

                        removedList.Add(SkillList[i]);
                        //break;
                    }
                }

                timer += Time.deltaTime;

                for (int i = 0; i < removedList.Count; ++i)
                {
                    SkillList.Remove(removedList[i]);
                }
            }
        }

       
    }

};  //end SG

