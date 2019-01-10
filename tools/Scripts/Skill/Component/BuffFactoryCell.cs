/**
* @file     : BuffFactoryCell.cs
* @brief    : 
* @details  : buff工厂元素
* @author   : 
* @date     : 2014-12-9
*/

using XLua;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{

[Hotfix]
    public class BuffFactoryCell : ISkillCell
    {
        //SkillBase m_skillBase = null;
        BuffFactoryCellDesc cellDesc = null;
        //float radius = 0f;
        //float radiusSQ = 0f;

        List<ActorObj> affectList = new List<ActorObj>();

        public override void Init(ISkillCellData cellData, SkillBase skillBase)
        {
            affectList.Clear();

            //m_skillBase = skillBase;
            cellDesc = (BuffFactoryCellDesc)cellData;
            //radius = cellDesc.Radius;
           
            //radiusSQ = radius * radius;

            InvokeRepeating("AddBuffToTarget",cellDesc.Delay,0.5f);
          
        }

        void Clear()
        {
            CancelInvoke("AddBuffToTarget");
        }

        void OnDestroy()
        {
            Clear();
        }

        void OnDisable()
        {
            Clear();
        }

        void AddBuffToTarget()
        {
            //m_skillBase.m_actor.AllAOETargetDoFromSelector(m_skillBase.m_skillDesc, (a) =>
            //{
            //    if ((transform.position - a.transform.position).sqrMagnitude <= radiusSQ)
            //    {
            //        bool isShowTip = false;
            //        //第一次才显示提示
            //        if (affectList.Contains(a) == false)
            //        {
            //            isShowTip = true;
            //            affectList.Add(a);
            //        }
            //     //   a.Addbuff(m_skillBase.m_skillDesc.TargetBuffID, m_skillBase.m_actor, m_skillBase.m_skillID, isShowTip);
            //    }
            //});
        }
    }

};  //end SG

