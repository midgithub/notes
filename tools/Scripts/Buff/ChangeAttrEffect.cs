using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SG
{


    public enum ChangeAttrType
    {
        Unknown,

        mCurHP,                         //当前生命

        mSecondHP,                      //第二生命

        mMaxHP,					        //最大生命

        mPAtk,					        //物攻

        mPDef,					        //物防

        mMAtk,					        //法攻

        mMDef,					        //法防

        mSpeed,					        //移动速度

        mSpeedPercent,					//移动速度百分比
        
        mCRIRate,					    //暴击
        
        mFreeCRIRate,					//抗暴

        mCRIDmgRate,					//暴伤加成
        
        mdmgRatePercent,				//伤害加成
        
        mdmgResistRatePercent,		    //伤害减免
                
        mAtkSpeed,					    //攻击速度值
        
        mAtkSpeedPercent,				//攻击速度百分比

        mSuckPercent,					//吸血

        mPBouncePercent,				//物攻反弹

        mMBouncePercent,				//法攻反弹

        mheal,                          //治疗效果

        mhit,                           //命中

        mEvade,                         //闪避

        mEvadeRecover,                  //闪避回血

        mIgnore,                        //忽视

        mCreatureBody,                  //体型

        mMAtkAndmheal,                  //mMAtk*mheal

        SpeedScale,                //速度变化值

        DamageScaleOfBeHitByMale,        //被男性角色攻击伤害缩放

        DamageScaleOfBeHitByFemale,     //被女性角色攻击伤害缩放

        DamageScaleOfHitMaleScale,               //攻击男性角色伤害缩放

        DamageScaleOfHitFemaleScale,            //攻击女性角色伤害缩放

        SkillCDScale,           //技能CD缩放

        BuffTempParameter, //当前buff的临时参数
        MaxType, 
    };

// 修改指定数值, 最大生命，最大法力，攻击力，防御力 等等
[Hotfix]
    public class ChangeAttrEffect : BuffEffect
    {
        //static Dictionary<string, ChangeAttrType> typehash = null;
        //public static ChangeAttrType GetChangeAttrType(string descstring)
        //{
        //    if (typehash == null)
        //    {
        //        // 重要提示：如果下面的代码执行时异常跳出了，请将此文件用VS打开 菜单->高级保存选项 编码改为 UTF-8带签名
        //        typehash = new Dictionary<string, ChangeAttrType>();

        //        typehash.Add("当前生命", ChangeAttrType.mCurHP);

        //        typehash.Add("第二生命", ChangeAttrType.mSecondHP);

        //        typehash.Add("生命", ChangeAttrType.mMaxHP);

        //        typehash.Add("物攻", ChangeAttrType.mPAtk);

        //        typehash.Add("物防", ChangeAttrType.mPDef);

        //        typehash.Add("法攻", ChangeAttrType.mMAtk);

        //        typehash.Add("法防", ChangeAttrType.mMDef);

        //        typehash.Add("移动速度", ChangeAttrType.mSpeed);

        //        typehash.Add("移动速度百分比", ChangeAttrType.SpeedScale);

        //        typehash.Add("暴击", ChangeAttrType.mCRIRate);

        //        typehash.Add("抗暴", ChangeAttrType.mFreeCRIRate);

        //        typehash.Add("暴伤加成", ChangeAttrType.mCRIDmgRate);

        //        typehash.Add("伤害加成", ChangeAttrType.mdmgRatePercent);

        //        typehash.Add("伤害减免", ChangeAttrType.mdmgResistRatePercent);

        //        typehash.Add("攻击速度值", ChangeAttrType.mAtkSpeed);

        //        typehash.Add("攻击速度百分比", ChangeAttrType.mAtkSpeedPercent);

        //        typehash.Add("吸血", ChangeAttrType.mSuckPercent);

        //        typehash.Add("物攻反弹", ChangeAttrType.mPBouncePercent);

        //        typehash.Add("法攻反弹", ChangeAttrType.mMBouncePercent);

        //        typehash.Add("治疗效果", ChangeAttrType.mheal);

        //        typehash.Add("命中", ChangeAttrType.mhit);

        //        typehash.Add("闪避", ChangeAttrType.mEvade);

        //        typehash.Add("闪避回血", ChangeAttrType.mEvadeRecover);

        //        typehash.Add("忽视", ChangeAttrType.mIgnore);

        //        typehash.Add("体型", ChangeAttrType.mCreatureBody);

        //        typehash.Add("法攻*治疗效果", ChangeAttrType.mMAtkAndmheal);

        //        typehash.Add("受到男性伤害缩放", ChangeAttrType.DamageScaleOfBeHitByMale);
        //        typehash.Add("受到女性伤害缩放", ChangeAttrType.DamageScaleOfBeHitByFemale);
        //        typehash.Add("攻击男性伤害缩放", ChangeAttrType.DamageScaleOfHitMaleScale);
        //        typehash.Add("攻击女性伤害缩放", ChangeAttrType.DamageScaleOfHitFemaleScale);
        //        typehash.Add("技能CD缩放", ChangeAttrType.SkillCDScale);
        //        typehash.Add("临时参数", ChangeAttrType.BuffTempParameter);

        //    }
        //    //解决字典找不到引起的报错 add by lzp 19:19 2015/6/30
        //    ChangeAttrType ctype = ChangeAttrType.Unknown;
        //    typehash.TryGetValue(descstring, out ctype);
        //    return ctype;
        //}

        //public bool bDamage = false; // true 为持续伤害Buff
        //public bool bRestore;
        //protected float oldValue;
        //protected float timecur;
        //protected float timeRate;

        //public override void Init(buffConfig desc, ActorObj _owner, ActorObj _giver)
        //{
        //    base.Init(desc, _owner, _giver);
        //    timecur = 0;
        //    timeRate = desc.fParam[2];
        //}

        //public override void OnEnter(Buff _ownerBuff)
        //{
        //    base.OnEnter(_ownerBuff);
        //    ChangeAttrType typeid1 = GetChangeAttrType(effectDesc.stringParam1);
        //    oldValue = GetAttribteValue(owner, typeid1);

        //    if (typeid1 == ChangeAttrType.mSecondHP)
        //    {
        //        CoreEntry.gEventMgr.AddListener(GameEvent.GE_EVENT_SECONDHPISOVER, GE_EVENT_SECONDHPISOVER);
        //    }
        //    ChangeAttribteFortran();
        //}

        //// Update is called once per frame
        //public override void Update(float fElapsed)
        //{
        //    timecur += fElapsed;
        //    if (timecur > timeRate && timeRate != 0)
        //    {
        //        ChangeAttribteFortran();
        //        timecur -= timeRate;
        //    }
        //}

        //public override void OnExit()
        //{
        //    ChangeAttrType typeid1 = GetChangeAttrType(effectDesc.stringParam1);
        //    if (typeid1 == ChangeAttrType.mSecondHP)
        //    {
        //        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_EVENT_SECONDHPISOVER, GE_EVENT_SECONDHPISOVER);
        //    }
        //    // 回复原值
        //    if (bRestore) // 如果不是回复类Buf
        //    {

        //        SetAttribteValue(owner, typeid1, oldValue);
        //    }
        //}

        //public override void ReplaceBuff()
        //{
        //    base.ReplaceBuff();
        //}

        //public override void Stacking()
        //{
        //    base.Stacking();
        //    //if (ownerBuff.desc.Stacking!=0)
        //    {
        //        ChangeAttrType typeid1 = GetChangeAttrType(effectDesc.stringParam1);
        //        //新的叠加算法
        //        SetAttribteValue(owner, typeid1, oldValue);
        //        ChangeAttribteFortran();
        //    }
        //}

        //public virtual void ChangeAttribteFortran()
        //{
        //    ChangeAttrType typeid1 = GetChangeAttrType(effectDesc.stringParam1);
        //    ChangeAttrType typeid2 = GetChangeAttrType(sBindPos);

        //    if (typeid1 == ChangeAttrType.Unknown)
        //        LogMgr.UnityLog(string.Format("buffConfig  id = {0} 找不到对应的Key={1}", effectDesc.id, effectDesc.stringParam1));
        //    if (typeid2 == ChangeAttrType.Unknown)
        //        LogMgr.UnityLog(string.Format("buffConfig  id = {0} 找不到对应的Key={1}", effectDesc.id, sBindPos));

        //    float curValue1 = GetAttribteValue(owner, typeid1);
        //    float curValue2 = GetAttribteValue(owner, typeid2);
        //    float newValue = 0;


        //    float hitValue1 = GetAttribteValue(giver, typeid1);
        //    float hitValue2 = GetAttribteValue(giver, typeid2);



         

        //    if (typeid1 == ChangeAttrType.SpeedScale)
        //    {
        //        newValue = effectDesc.fParam[3];
        //        oldValue = 1 / newValue;
        //    }
        //    else
        //    {
        //        //float stacking = 1f;
        //        //if (ownerBuff.iStacking > 1)
        //        //{
        //        //    stacking = ownerBuff.iStacking;
        //        //}

        //        // 公式
        //        //   newValue = curValue1 + (curValue2 * (((float)effectDesc.fParam[0]) * 0.01f * stacking) + (float)(effectDesc.fParam[1] * stacking));

        //    }

        //    SetAttribteValue(owner, typeid1, newValue);

        //}

        //public virtual void OnDamage(ActorObj actorbase, ChangeAttrType typeid, float newValue)
        //{
        //    //纠正被击表现
        //    //DamageParam damageParam = new DamageParam();
        //    //damageParam.skillID = ownerBuff.skillID;
        //    //damageParam.attackActor = giver;
        //    //damageParam.behitActor = actorbase;

        //    //BehitParam behitParam = new BehitParam();
        //    //behitParam.hp = actorbase.curHp - (int)newValue;
        //    //if (actorbase.curHp - behitParam.hp > actorbase.mBaseAttr.MaxHP)// 检测最大生命值
        //    //    behitParam.hp = actorbase.curHp - actorbase.mBaseAttr.MaxHP;

        //    //behitParam.displayType = DamageDisplayType.DDT_NORMAL;
        //    //behitParam.damgageInfo = damageParam;

        //    //if (behitParam.hp != 0)
        //    //    actorbase.OnDamage(behitParam.hp, ownerBuff.buffId, TeamMgr.IsMainPlayer(actorbase.gameObject), behitParam);
        //}

        //public void SetAttribteValue(ActorObj actorbase, ChangeAttrType typeid, float newValue)
        //{
        //    if (actorbase == null)
        //        return;

        //    // 死亡状态不能加血,否则AI什么的会出错
        //    if (typeid == ChangeAttrType.mCurHP && actorbase.IsDeath())
        //        return;

        //    switch (typeid)
        //    {
        //        case ChangeAttrType.Unknown:
        //            break;

        //        case ChangeAttrType.mCurHP:
        //            {
        //                OnDamage(actorbase, typeid, newValue);
        //            }
        //            break;

        //        case ChangeAttrType.mSecondHP:
        //            {
        //                actorbase.secondHP = (int)newValue;
        //                //if (actorbase.curHp + actorbase.secondHP > actorbase.mBaseAttr.MaxHP)// 检测最大生命值
        //                //actorbase.secondHP = actorbase.mBaseAttr.MaxHP - actorbase.curHp;

        //                //PlayerHeath playerHeath = actorbase.GetComponent<PlayerHeath>() as PlayerHeath;
        //                //if (playerHeath)
        //                //{
        //                //    // 更新第二生命值
        //                //    EventToUI.SetArg(UIEventArg.Arg1, (float)(actorbase.curHp + actorbase.secondHP) / actorbase.mBaseAttr.MaxHP);
        //                //    EventToUI.SetArg(UIEventArg.Arg2, playerHeath.gameObject);
        //                //    EventToUI.SetArg(UIEventArg.Arg3, (object)actorbase);
        //                //    EventToUI.SendEvent("EU_SETPLAYERBLOODVALUE2");
        //                //}

        //            }
        //            break;

        //        case ChangeAttrType.mMaxHP:
        //            actorbase.mBaseAttr.MaxHP = (int)newValue;
        //            break;

        //        //case ChangeAttrType.mPAtk:
        //        //    actorbase.mBaseAttr.PAtk = (int)newValue;
        //        //    break;

        //        //case ChangeAttrType.mPDef:
        //        //    actorbase.mBaseAttr.PDef = (int)newValue;
        //        //    break;

        //        //case ChangeAttrType.mMAtk:
        //        //    actorbase.mBaseAttr.MAtk = (int)newValue;
        //        //    break;

        //        //case ChangeAttrType.mMDef:
        //        //    actorbase.mBaseAttr.MDef = (int)newValue;
        //        //    break;

        //        case ChangeAttrType.mSpeed:
        //            actorbase.mBaseAttr.Speed = (int)newValue;
        //            //actorbase.actorCreatureInfo.battleSpeed = (int)newValue; // 不可以修改这个读表的数据
        //            actorbase.SetSpeed(newValue);
        //            break;

        //        //case ChangeAttrType.mSpeedPercent:
        //        //    actorbase.mBaseAttr.SpeedPercent *= newValue;
        //        //    actorbase.SetSpeed(actorbase.mBaseAttr.Speed);
        //        //    break;

        //        //case ChangeAttrType.mCRIRate:
        //        //    actorbase.mBaseAttr.CRIRate = newValue;
        //        //    break;

        //        //case ChangeAttrType.mFreeCRIRate:
        //        //    actorbase.mBaseAttr.FreeCRIRate = newValue;
        //        //    break;

        //        //case ChangeAttrType.mCRIDmgRate:
        //        //    actorbase.mBaseAttr.CRIDmgRate = newValue;
        //        //    break;

        //        //case ChangeAttrType.mdmgRatePercent:
        //        //    actorbase.mBaseAttr.DmgRatePercent = newValue;
        //        //    break;

        //        //case ChangeAttrType.mdmgResistRatePercent:
        //        //    actorbase.mBaseAttr.DmgResistRatePercent = newValue;
        //        //    break;

        //        //case ChangeAttrType.mAtkSpeed:
        //        //    actorbase.mBaseAttr.AtkSpeed = (int)newValue;
        //        //    break;

        //        //case ChangeAttrType.mAtkSpeedPercent:
        //        //    actorbase.mBaseAttr.AtkSpeedPercent = newValue;
        //        //    break;


        //        //case ChangeAttrType.mPBouncePercent:
        //        //    actorbase.mBaseAttr.PBouncePercent = newValue;
        //        //    break;

        //        //case ChangeAttrType.mMBouncePercent:
        //        //    actorbase.mBaseAttr.MBouncePercent = newValue;
        //        //    break;

        //        //case ChangeAttrType.mheal://治疗效果
        //        //    actorbase.mBaseAttr.Heal = newValue;
        //        //    break;

        //        case ChangeAttrType.mhit://命中
        //            actorbase.mBaseAttr.Hit = newValue;
        //            break;

        //        case ChangeAttrType.mEvade://闪避
        //            actorbase.mBaseAttr.Dodge = newValue;
        //            break;

        //        //case ChangeAttrType.mEvadeRecover://闪避回血
        //        //    actorbase.mBaseAttr.EvadeRecover = newValue;
        //        //    break;

        //        //case ChangeAttrType.mIgnore://忽视
        //        //    actorbase.mBaseAttr.Ignore = newValue;
        //        //    break;

        //        case ChangeAttrType.mCreatureBody:
        //            actorbase.ChangeCreatureBody((int)newValue);
        //            break;

        //        case ChangeAttrType.mMAtkAndmheal:

        //            break;

        //        //case ChangeAttrType.mSuckPercent:
        //        //    owner.mBaseAttr.SuckPercent = newValue;
        //        //    break;

        //        case ChangeAttrType.SpeedScale:
        //            actorbase.mSpeedScale *= newValue;
        //            actorbase.SetSpeed(actorbase.mBaseAttr.Speed);
        //            break;

        //        //case ChangeAttrType.DamageScaleOfBeHitByFemale:
        //        //    actorbase.DamageScaleOfBeHitByFemale = newValue;
        //        //    break;

        //        //case ChangeAttrType.DamageScaleOfBeHitByMale:
        //        //    actorbase.DamageScaleOfBeHitByMale = newValue;
        //        //    break;

        //        //case ChangeAttrType.DamageScaleOfHitFemaleScale:
        //        //    actorbase.DamageScaleOfHitFemaleScale = newValue;
        //        //    break;

        //        //case ChangeAttrType.DamageScaleOfHitMaleScale:
        //        //    actorbase.DamageScaleOfHitMaleScale = newValue;
        //        //    break;

        //        case ChangeAttrType.SkillCDScale:
        //            actorbase.SkillCDScale = newValue;
        //            break;
        //        case ChangeAttrType.BuffTempParameter:
        //            ownerBuff.TempParameter = (int)newValue;
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //public float GetAttribteValue(ActorObj actorbase, ChangeAttrType typeid)
        //{
        //    float rel = 0;
        //    if (actorbase == null)
        //        return rel;
        //    switch (typeid)
        //    {
        //        case ChangeAttrType.Unknown:
        //            break;

        //        case ChangeAttrType.mCurHP:
        //            rel = actorbase.curHp;
        //            break;

        //        case ChangeAttrType.mMaxHP:
        //            rel = actorbase.mBaseAttr.MaxHP;
        //            break;

        //        //case ChangeAttrType.mPAtk:
        //        //    rel = actorbase.mBaseAttr.PAtk;
        //        //    break;

        //        //case ChangeAttrType.mPDef:
        //        //    rel = actorbase.mBaseAttr.PDef;
        //        //    break;

        //        //case ChangeAttrType.mMAtk:
        //        //    rel = actorbase.mBaseAttr.MAtk;
        //        //    break;

        //        //case ChangeAttrType.mMDef:
        //        //    rel = actorbase.mBaseAttr.MDef;
        //        //    break;

        //        case ChangeAttrType.mSpeed:
        //            rel = actorbase.mBaseAttr.Speed;
        //            //rel = actorbase.actorCreatureInfo.battleSpeed;
        //            break;

        //        //case ChangeAttrType.mSpeedPercent:
        //        //    rel = actorbase.mBaseAttr.SpeedPercent;
        //        //    break;

        //        //case ChangeAttrType.mCRIRate:
        //        //    rel = actorbase.mBaseAttr.CRIRate;
        //        //    break;

        //        //case ChangeAttrType.mFreeCRIRate:
        //        //    rel = actorbase.mBaseAttr.FreeCRIRate;
        //        //    break;

        //        //case ChangeAttrType.mCRIDmgRate:
        //        //    rel = actorbase.mBaseAttr.CRIDmgRate;
        //        //    break;

        //        //case ChangeAttrType.mdmgRatePercent:
        //        //    rel = actorbase.mBaseAttr.PhydmgRatePercent;
        //        //    break;

        //        //case ChangeAttrType.mdmgResistRatePercent:
        //        //    rel = actorbase.mBaseAttr.PhydmgResistRatePercent;
        //        //    break;

        //        //case ChangeAttrType.mAtkSpeed:
        //        //    rel = actorbase.mBaseAttr.AtkSpeed;
        //        //    break;

        //        //case ChangeAttrType.mAtkSpeedPercent:
        //        //    rel = actorbase.mBaseAttr.AtkSpeedPercent;
        //        //    break;

        //        //case ChangeAttrType.mPBouncePercent:
        //        //    rel = actorbase.mBaseAttr.PBouncePercent;
        //        //    break;

        //        //case ChangeAttrType.mMBouncePercent:
        //        //    rel = actorbase.mBaseAttr.MBouncePercent;
        //        //    break;

        //        //case ChangeAttrType.mheal://治疗效果
        //        //    rel = actorbase.mBaseAttr.Heal;
        //        //    break;

        //        case ChangeAttrType.mhit://命中
        //            rel = actorbase.mBaseAttr.Hit;
        //            break;

        //        case ChangeAttrType.mEvade://闪避
        //            rel = actorbase.mBaseAttr.Dodge;
        //            break;

        //        //case ChangeAttrType.mEvadeRecover://闪避回血
        //        //    rel = actorbase.mBaseAttr.EvadeRecover;
        //        //    break;

        //        //case ChangeAttrType.mIgnore://忽视
        //        //    rel = actorbase.mBaseAttr.Ignore;
        //        //    break;

        //        case ChangeAttrType.mCreatureBody:
        //            rel = actorbase.BodyType;
        //            break;

        //        //case ChangeAttrType.mMAtkAndmheal:
        //        //    rel = actorbase.mBaseAttr.MAtk * (1f+actorbase.mBaseAttr.Heal);
        //        //    break;

        //        case ChangeAttrType.SpeedScale:
        //            rel = actorbase.mSpeedScale;
        //            break;

        //        //case ChangeAttrType.DamageScaleOfBeHitByFemale:
        //        //    rel = actorbase.DamageScaleOfBeHitByFemale;
        //        //    break;

        //        //case ChangeAttrType.DamageScaleOfBeHitByMale:
        //        //    rel = actorbase.DamageScaleOfBeHitByMale ;
        //        //    break;

        //        //case ChangeAttrType.DamageScaleOfHitFemaleScale:
        //        //    rel = actorbase.DamageScaleOfHitFemaleScale ;
        //        //    break;

        //        //case ChangeAttrType.DamageScaleOfHitMaleScale:
        //        //    rel = actorbase.DamageScaleOfHitMaleScale ;
        //        //    break;

        //        case ChangeAttrType.SkillCDScale:
        //            rel = actorbase.SkillCDScale;
        //            break;

        //        case ChangeAttrType.BuffTempParameter:
        //            rel = ownerBuff.TempParameter;
        //            break;
        //        default:
        //            break;
        //    }
        //    return rel;
        //}

        //void GE_EVENT_SECONDHPISOVER(GameEvent ge, EventParameter parameter)
        //{
        //    ChangeAttrType typeid1 = GetChangeAttrType(effectDesc.stringParam1);
        //    if (typeid1 == ChangeAttrType.mSecondHP)
        //    {
        //        if (parameter != null)
        //        {
        //            if (owner.gameObject == parameter.goParameter)
        //            {
        //                //第二条生命值为空，清空该buff
        //                ownerBuff.FinishEffect();
        //            }

        //        }
        //    }
        //}
    }
}


//当前生命,第二生命,生命,物攻,物防,法攻,法防,移动速度,移动速度百分比,暴击,抗暴,暴伤加成,伤害加成,伤害减免,攻击速度值,攻击速度百分比,吸血,物攻反弹,法攻反弹,治疗效果,命中,闪避,闪避回血,忽视,体型,法攻*治疗效果
//被男性角色攻击伤害缩放 被女性角色攻击伤害缩放 攻击男性角色伤害缩放 攻击女性角色伤害缩放












