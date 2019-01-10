using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SG
{
// 修改指定数值, 最大生命，最大法力，攻击力，防御力 等等
[Hotfix]
    public class DamageAttrEffect : ChangeAttrEffect
    {
        //public override void ChangeAttribteFortran()
        //{
        //    //ChangeAttrType typeid1 = GetChangeAttrType(effectDesc.stringParam1);
        //    //ChangeAttrType typeid2 = GetChangeAttrType(sBindPos);

        //    //if (typeid1 == ChangeAttrType.Unknown)
        //    //    LogMgr.UnityLog(string.Format("buffConfig  id = {0} 找不到对应的Key={1}", effectDesc.id, effectDesc.stringParam1));
        //    //if (typeid2 == ChangeAttrType.Unknown)
        //    //    LogMgr.UnityLog(string.Format("buffConfig  id = {0} 找不到对应的Key={1}", effectDesc.id, sBindPos));

        //    //int skillAttrib = (sBindPos.IndexOf("魔法攻击") != -1) ? (int) Configs.skillConfig.SkillAttribEnum.MAGDMG : 0;
        //    //if(skillAttrib==0)
        //    //    skillAttrib = (sbyte) Configs.skillConfig.SkillAttribEnum.PHYDMG;

        //    DamageParam damageParam = new DamageParam();
        //    damageParam.attackActor = giver;
        //    damageParam.behitActor  = owner;
             
        //    damageParam.weight    = 0;
        //    damageParam.isNotUseCurveMove = true;

        //    CoreEntry.gSkillMgr.OnBuffDamage(damageParam, effectDesc.fParam[0] * 0.01f, effectDesc.fParam[0] * 0.01f, effectDesc.fParam[1], 1);
        //}

        ////public override void OnDamage(ActorObj actorbase, ChangeAttrType typeid, float newValue)
        ////{
        ////    //纠正被击表现
        ////    DamageParam damageParam = new DamageParam();
        ////    damageParam.skillID = ownerBuff.skillID;
        ////    damageParam.attackObj = giver.gameObject;
        ////    damageParam.behitObj = actorbase.gameObject;

        ////    BehitParam behitParam = new BehitParam();
        ////    behitParam.hp = actorbase.curHp - (int)newValue;
        ////    if (actorbase.curHp - behitParam.hp > actorbase.mBaseAttr.MaxHP)// 检测最大生命值
        ////        behitParam.hp = actorbase.curHp - actorbase.mBaseAttr.MaxHP;

        ////    behitParam.displayType = DamageDisplayType.DDT_NORMAL;
        ////    behitParam.damgageInfo = damageParam;

        ////    if (behitParam.hp != 0)
        ////        actorbase.OnDamage(behitParam.hp, ownerBuff.buffId, TeamMgr.IsMainPlayer(actorbase.gameObject), behitParam);
        ////}




    }
}





