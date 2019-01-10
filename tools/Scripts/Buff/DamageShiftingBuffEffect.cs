using XLua;
﻿using UnityEngine;
using System.Collections;


namespace SG
{
    /// <summary>
    /// 伤害转置buff
    /// </summary>
[Hotfix]
    public class DamageShiftingBuffEffect : TimerBasedBuffEffect
    {
    //    float POwnerFactor= 0f;
    //    float PTargetFactor = 0f;
    //    float MOwnerFactor = 0f;
    //    float MTargetFactor = 0f;
    //    int MBaseDamage = 0;
    //    int PBaseDamage = 0;
    //    public override void Init(buffConfig  desc, ActorObj _owner, ActorObj _giver)
    //    {
    //        base.Init(desc, _owner, _giver);

    //        POwnerFactor = desc.fParam[0];
    //        PTargetFactor = desc.fParam[1];
    //        MOwnerFactor = desc.fParam[2];
    //        MTargetFactor = desc.fParam[3];

            
    //    }

    //    public override void OnEnter(Buff _ownerBuff)
    //    {
    //        base.OnEnter(_ownerBuff);

          
    //        CoreEntry.gEventMgr.AddListener(GameEvent.GE_DAMAGE_CHANGE, OnWorking);
    //    }
    //    public override void OnExit()
    //    {
    //        base.OnExit();
    //        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_DAMAGE_CHANGE, OnWorking);
    //    }

    //    public void OnWorking(GameEvent ge, EventParameter paramter)
    //    {
    //        float damage = paramter.intParameter2;
    //        ActorObj buffOwner = paramter.objParameter as ActorObj;
    //        ActorObj target = paramter.objParameter1 as ActorObj;

    //        BehitParam curBehitParam = paramter.objParameter2 as BehitParam;
            
    //        //反弹的伤害不能再反弹
    //        if(curBehitParam.displayType == DamageDisplayType.DDT_BONCE )
    //        {
    //            return;
    //        }
            
    //        //是伤害才反弹，不反弹自己的伤害
    //        if (damage > 0 && target != null && buffOwner != null && buffOwner == owner && target != buffOwner)
    //        {
    //            //XuXiang 2017-06-20 16:16 属性取消物理伤害和魔法伤害区分(临时用PFactor)
    //            //float finalPDamage = PBaseDamage + POwnerFactor * owner.mBaseAttr.PDef + PTargetFactor * target.mBaseAttr.PAtk;
    //            //float finalMDamage = MBaseDamage + MOwnerFactor * owner.mBaseAttr.MDef + MTargetFactor * target.mBaseAttr.MAtk;
    //            //float finalDamage = finalPDamage + finalMDamage;
    //            float factorDamage = (float)(POwnerFactor * owner.mBaseAttr.Defence + PTargetFactor * target.mBaseAttr.Attack);
    //            float finalDamage = PBaseDamage + MBaseDamage + damage;

    //            DamageParam damageParam = new DamageParam();
    //           // damageParam.skillID = ownerBuff.skillID;
    //            damageParam.attackActor = buffOwner;
    //            damageParam.behitActor = target;

    //            BehitParam behitParam = new BehitParam();
    //            behitParam.hp = (int)finalDamage;
    //            behitParam.displayType = DamageDisplayType.DDT_BONCE;
    //            behitParam.damgageInfo = damageParam;
    //            target.OnDamage((int)finalDamage, 0, target.IsMainPlayer(), behitParam);
    //        }
    //    }
    }

}

