/**
* @file     : ForceFactoryCell.cs
* @brief    : 
* @details  : 力工厂元素
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
    public class ForceFactoryCell : ISkillCell
    {
        //SkillBase m_skillBase = null;
        ForceFactoryCellDesc cellDesc = null;
        float radius = 0f;
        float radiusSQ = 0f;
        float speed = 10;
        //float freq = 0.5f;
        //float timer = 0;
        //float lastTime = 1f;
        //bool isAoe = true;
        //ActorObj target = null;

        public override void Init(ISkillCellData cellData, SkillBase skillBase)
        {

            //m_skillBase = skillBase;
            cellDesc = (ForceFactoryCellDesc)cellData;
            radius = cellDesc.Radius;
            speed = cellDesc.Speed;
            ////isAoe = cellDesc.IsAoe;
            //timer = 0;
            //lastTime = cellDesc.LastTime;
            //target = null;

            radiusSQ = radius * radius;

            // InvokeRepeating("AffectTarget", 0f, freq);

        }

        void Clear()
        {
            // CancelInvoke("AffectTarget");
        }

        void OnDestroy()
        {
            Clear();
        }

        void OnDisable()
        {
            Clear();
        }

        void ApplyForce(ActorObj actor)
        {
            Vector3 diff = (transform.position - actor.transform.position);
            if (actor.CheckIfNotBossOrInQijue())
            {
                if (diff.sqrMagnitude <= radiusSQ)
                {
                    Vector3 distance = diff.normalized * speed * Time.deltaTime;
                    distance.y = 0;
                    Vector3 dest = actor.transform.position + distance;
                    if (BaseTool.instance.CanMoveToPos(actor.transform.position, dest, actor.GetColliderRadius()))
                    {
                        BaseTool.SetPosition(actor.transform, actor.transform.position + distance);
                    }

                }
            }
        }
    }
}

