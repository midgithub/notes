using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SG
{
    // 天赋属性
[Hotfix]
    public class GiftsAttrEffect : BuffEffect
    {
        //BaseAttr mBaseAttr = null;
        //// 重新计算二级属性时调用
        //public override void RefreshAttrs(BaseAttr attr)
        //{
        //    mBaseAttr = attr;
        //    ChangeAttribteFortran();
        //}

        //public void ChangeAttribteFortran() // 天赋属性
        //{
        //    ChangeAttrType typeid1 = ChangeAttrEffect.GetChangeAttrType(effectDesc.stringParam1);
        //    ChangeAttrType typeid2 = ChangeAttrEffect.GetChangeAttrType(sBindPos);

        //    if (typeid1 == ChangeAttrType.Unknown)
        //        LogMgr.UnityLog(string.Format("buffConfig  id = {0} 找不到对应的Key={1}", effectDesc.id, effectDesc.stringParam1));
        //    if (typeid2 == ChangeAttrType.Unknown)
        //        LogMgr.UnityLog(string.Format("buffConfig  id = {0} 找不到对应的Key={1}", effectDesc.id, sBindPos));

        //    float curValue1 = GetAttribteValue(typeid1);
        //    float curValue2 = GetAttribteValue(typeid2);
        //    float newValue = 0;

        //    // 公式
        //    newValue = curValue1 + (curValue2 * (((float)effectDesc.fParam[0]) / 100) + (float)effectDesc.fParam[1]);

        //    SetAttribteValue(typeid1, newValue);
        //}

        //public void SetAttribteValue(ChangeAttrType typeid, float newValue) // 天赋属性
        //{
        //    if (mBaseAttr == null)
        //        return;

        //    switch(typeid)
        //    {
        //        case ChangeAttrType.Unknown:
        //            break;

        //        case ChangeAttrType.mCurHP:
        //            break;

        //        case ChangeAttrType.mSecondHP:
        //            break;

        //        case ChangeAttrType.mMaxHP:
        //            mBaseAttr.MaxHP = (int)newValue;
        //            break;

        //        //case ChangeAttrType.mPAtk:
        //        //    mBaseAttr.PAtk = (int)newValue;
        //        //    break;

        //        //case ChangeAttrType.mPDef:
        //        //    mBaseAttr.PDef = (int)newValue;
        //        //    break;

        //        //case ChangeAttrType.mMAtk:
        //        //    mBaseAttr.MAtk = (int)newValue;
        //        //    break;

        //        //case ChangeAttrType.mMDef:
        //        //    mBaseAttr.MDef = (int)newValue;
        //        //    break;

        //        case ChangeAttrType.mSpeed:
        //            mBaseAttr.Speed = (int)newValue;
        //            break;

        //        //case ChangeAttrType.mSpeedPercent:
        //        //    mBaseAttr.SpeedPercent = newValue;
        //        //    break;

        //        //case ChangeAttrType.mCRIRate:
        //        //    mBaseAttr.CRIRate = newValue;
        //        //    break;

        //        //case ChangeAttrType.mFreeCRIRate:
        //        //    mBaseAttr.FreeCRIRate = newValue;
        //        //    break;

        //        //case ChangeAttrType.mCRIDmgRate:
        //        //    mBaseAttr.CRIDmgRate = newValue;
        //        //    break;

        //        //case ChangeAttrType.mdmgRatePercent:
        //        //    mBaseAttr.DmgRatePercent = newValue;
        //        //    break;

        //        //case ChangeAttrType.mdmgResistRatePercent:
        //        //    mBaseAttr.DmgResistRatePercent = newValue;
        //        //    break;

        //        //case ChangeAttrType.mAtkSpeed:
        //        //    mBaseAttr.AtkSpeed = (int)newValue;
        //        //    break;

        //        //case ChangeAttrType.mAtkSpeedPercent:
        //        //    mBaseAttr.AtkSpeedPercent = newValue;
        //        //    break;

        //        //case ChangeAttrType.mPBouncePercent:
        //        //    mBaseAttr.PBouncePercent = newValue;
        //        //    break;

        //        //case ChangeAttrType.mMBouncePercent:
        //        //    mBaseAttr.MBouncePercent = newValue;
        //        //    break;

        //        //case ChangeAttrType.mheal://治疗效果
        //        //    mBaseAttr.Heal = newValue;
        //        //    break;

        //        case ChangeAttrType.mhit://命中
        //            mBaseAttr.Hit = newValue;
        //            break;

        //        case ChangeAttrType.mEvade://闪避
        //            mBaseAttr.Dodge = newValue;
        //            break;

        //        //case ChangeAttrType.mEvadeRecover://闪避回血
        //        //    mBaseAttr.EvadeRecover = newValue;
        //        //    break;

        //        //case ChangeAttrType.mIgnore://忽视
        //        //    mBaseAttr.Ignore = newValue;
        //        //    break;

        //        default:
        //            break;
        //    }
        //}

        //public float GetAttribteValue(ChangeAttrType typeid) // 天赋属性
        //{
        //    float rel = 0;
        //    if (mBaseAttr == null)
        //        return rel;
        //    switch (typeid)
        //    {
        //        case ChangeAttrType.Unknown:
        //            break;

        //        case ChangeAttrType.mCurHP:
        //            break;

        //        case ChangeAttrType.mMaxHP:
        //            rel = mBaseAttr.MaxHP;
        //            break;

        //        //case ChangeAttrType.mPAtk:
        //        //    rel = mBaseAttr.PAtk;
        //        //    break;

        //        //case ChangeAttrType.mPDef:
        //        //    rel = mBaseAttr.PDef;
        //        //    break;

        //        //case ChangeAttrType.mMAtk:
        //        //    rel = mBaseAttr.MAtk;
        //        //    break;

        //        //case ChangeAttrType.mMDef:
        //        //    rel = mBaseAttr.MDef;
        //        //    break;

        //        case ChangeAttrType.mSpeed:
        //            rel = mBaseAttr.Speed;
        //            break;

        //        //case ChangeAttrType.mSpeedPercent:
        //        //    rel = mBaseAttr.SpeedPercent;
        //        //    break;

        //        //case ChangeAttrType.mCRIRate:
        //        //    rel = mBaseAttr.CRIRate;
        //        //    break;

        //        //case ChangeAttrType.mFreeCRIRate:
        //        //    rel = mBaseAttr.FreeCRIRate;
        //        //    break;

        //        //case ChangeAttrType.mCRIDmgRate:
        //        //    rel = mBaseAttr.CRIDmgRate;
        //        //    break;

        //        //case ChangeAttrType.mdmgRatePercent:
        //        //    rel = mBaseAttr.DmgRatePercent;
        //        //    break;

        //        //case ChangeAttrType.mdmgResistRatePercent:
        //        //    rel = mBaseAttr.DmgResistRatePercent;
        //        //    break;

        //        //case ChangeAttrType.mAtkSpeed:
        //        //    rel = mBaseAttr.AtkSpeed;
        //        //    break;

        //        //case ChangeAttrType.mAtkSpeedPercent:
        //        //    rel = mBaseAttr.AtkSpeedPercent;
        //        //    break;

        //        //case ChangeAttrType.mPBouncePercent:
        //        //    rel = mBaseAttr.PBouncePercent;
        //        //    break;

        //        //case ChangeAttrType.mMBouncePercent:
        //        //    rel = mBaseAttr.MBouncePercent;
        //        //    break;

        //        //case ChangeAttrType.mheal://治疗效果
        //        //    rel = mBaseAttr.Heal;
        //        //    break;

        //        case ChangeAttrType.mhit://命中
        //            rel = mBaseAttr.Hit;
        //            break;

        //        case ChangeAttrType.mEvade://闪避
        //            rel = mBaseAttr.Dodge;
        //            break;

        //        //case ChangeAttrType.mEvadeRecover://闪避回血
        //        //    rel = mBaseAttr.EvadeRecover;
        //        //    break;

        //        //case ChangeAttrType.mIgnore://忽视
        //        //    rel = mBaseAttr.Ignore;
        //        //    break;

        //        //case ChangeAttrType.mMAtkAndmheal:
        //        //    rel = mBaseAttr.MAtk * mBaseAttr.Heal;
        //        //    break;

        //        default:
        //            break;
        //    }
        //    return rel;
        //}
    }
}





