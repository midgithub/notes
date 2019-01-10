using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SG
{

// 在目标身上绑定一个特效
[Hotfix]
public class EventBuffEffect : BuffEffect
{
    enum EventBuffType
    {
        MONO_DEL        = 0x0001,
        MONO_ADD        = 0x0002,
        MULT_FRIEND_DEL = 0x0004,
        MULT_FRIEND_ADD = 0x0008,
        MULT_ENEMY_ADD  = 0x0010,
        MULT_ENEMY_DEL  = 0x0020,
        MONO_TARGET_ADD = 0x0040,
        MONO_TARGET_DEL = 0x0080,
        MULT_TEAM_DEL = 0x0100,
        MULT_TEAM_ADD = 0x0200,
        MULT_ENEMYTEAM_DEL = 0x0400,
        MULT_ENEMYTEAM_ADD = 0x0800,
    }

    //static Dictionary<string, GameEvent> typehash = null;
    //public static GameEvent GetGameEvent(string descstring)
    //{
    //    if (typehash == null)
    //    {
    //        // 重要提示：如果下面的代码执行时异常跳出了，请将此文件用VS打开 菜单->高级保存选项 编码改为 UTF-8带签名
    //        typehash = new Dictionary<string, GameEvent>();
    //        typehash.Add("攻击", GameEvent.GE_EVENT_ATTACK);
    //        typehash.Add("受击", GameEvent.GE_EVENT_BEHIT);
    //        typehash.Add("加血", GameEvent.GE_EVENT_CURE);
    //        typehash.Add("死亡", GameEvent.GE_EVENT_DIE);
    //        typehash.Add("击杀", GameEvent.GE_EVENT_KILL);
    //        typehash.Add("时间", GameEvent.GE_EVENT_TIME);
    //        typehash.Add("重生", GameEvent.GE_EVENT_REBORN);
    //        typehash.Add("叠加", GameEvent.GE_EVENT_BUFF_STATCKING);
    //        typehash.Add("暴击", GameEvent.GE_EVENT_DOUBLEDAMAGE);
    //    }
    //    return (GameEvent)typehash[descstring];
    //}

    //private GameLogicMgr m_gameManager;
    //private int type = 0;
    //private GameEvent eventID;
    //private GameObject ownerObj = null;
    //private float mLife;
    //private float intervalTime;
    //private int iBuffIDOperate;
    //private int iBuffIDOwner; // 删除Buff成功后 给自己添加的Buff
    //private int iBuffIDTarget;
    //private float fRadiusLen;
    //private float fCoolIntervalTime;
    //private float fCoolTime;
    //private int times = 0; //buff使用多少次后被删除
    //private float customParam = 0f;
    //private bool isTeamCheck = false;

    //public override void Init(buffConfig  desc, ActorObj _owner, ActorObj _giver)
    //{
    //    base.Init(desc, _owner, _giver);

    //    m_gameManager = CoreEntry.gGameMgr;

    //    //desc.stringParam1;    "攻击"  事件
    //    //desc.stringParam2;    "范围友方删除"  释放范围
    //    iBuffIDOperate = (int)desc.fParam[0];   // 要添加或者删除的Buff ID
    //    iBuffIDOwner   = (int)desc.fParam[1];   // 删除Buff成功后 给自己添加的Buff
    //    iBuffIDTarget  = (int)desc.fParam[2];   // 删除Buff成功后 给范围内目标添加的Buff
    //    fRadiusLen     = desc.fParam[3];        // 范围半径
    //    fCoolIntervalTime = desc.fParam[4];     // 冷却时间
    //    fCoolTime = 0;
    //    times = (int)desc.fParam[5]; //消息通知多少次数后生效
    //    customParam = desc.fParam[6]; //自定义参数


    //    eventID = GetGameEvent(effectDesc.stringParam1);
    //    if (eventID == GameEvent.GE_EVENT_TIME)
    //    {
    //        intervalTime = fCoolIntervalTime;  // 定时器 间隔时间
    //        mLife = intervalTime;
    //        fCoolIntervalTime = 0;
    //    }

    //    // 冷却时间 -1表示使用Buff的冷却时间
    //    if (fCoolIntervalTime == -1f)
    //        fCoolIntervalTime = ownerBuff.desc.last_time;

    //    //type |= (desc.stringParam2.IndexOf("范围友方添加") != -1) ? (int)EventBuffType.MULT_FRIEND_ADD : 0;
    //    //type |= (desc.stringParam2.IndexOf("范围友方删除") != -1) ? (int)EventBuffType.MULT_FRIEND_DEL : 0;
    //    //type |= (desc.stringParam2.IndexOf("范围敌方添加") != -1) ? (int)EventBuffType.MULT_ENEMY_ADD : 0;
    //    //type |= (desc.stringParam2.IndexOf("范围敌方删除") != -1) ? (int)EventBuffType.MULT_ENEMY_DEL : 0;
    //    //type |= (desc.stringParam2.IndexOf("自身添加") != -1)     ? (int)EventBuffType.MONO_ADD : 0;
    //    //type |= (desc.stringParam2.IndexOf("自身删除") != -1)     ? (int)EventBuffType.MONO_DEL : 0;
    //    //type |= (desc.stringParam2.IndexOf("命中对象添加") != -1) ? (int)EventBuffType.MONO_TARGET_ADD : 0;
    //    //type |= (desc.stringParam2.IndexOf("命中对象删除") != -1) ? (int)EventBuffType.MONO_TARGET_DEL : 0;
    //    //type |= (desc.stringParam2.IndexOf("范围友方英雄添加") != -1) ? (int)EventBuffType.MULT_TEAM_ADD : 0;
    //    //type |= (desc.stringParam2.IndexOf("范围友方英雄删除") != -1) ? (int)EventBuffType.MULT_TEAM_DEL : 0;
    //    //type |= (desc.stringParam2.IndexOf("范围敌方英雄添加") != -1) ? (int)EventBuffType.MULT_ENEMYTEAM_ADD : 0;
    //    //type |= (desc.stringParam2.IndexOf("范围敌方英雄删除") != -1) ? (int)EventBuffType.MULT_ENEMYTEAM_DEL : 0;

    //    if(type==0)
    //        type = 1;

    //    ownerObj = owner.gameObject;
    //}

    //public override void OnEnter(Buff _ownerBuff)
    //{
    //    base.OnEnter(_ownerBuff);

    //    // 删除自己的Buff可以不用写Buff ID
    //    if (type == 1 && iBuffIDOperate == 0)
    //        iBuffIDOperate = ownerBuff.mBuffType;

    //    CoreEntry.gEventMgr.AddListener(eventID, OnWorking);

    //    //一上来触发一次时间事件
    //    if (eventID == GameEvent.GE_EVENT_TIME)
    //    {
    //        TiggerTimeEvent();
    //    }

    //}

    //public override void OnExit()
    //{
    //    CoreEntry.gEventMgr.RemoveListener(eventID, OnWorking);
    //}

    //public override void Update(float fElapsed)
    //{
    //    if (eventID == GameEvent.GE_EVENT_TIME && intervalTime != 0)
    //    {
    //        mLife -= owner.GetBehaviorDeltaTime();
    //        if (mLife <= float.Epsilon)
    //        {
    //            mLife = intervalTime;
    //            // ...
    //            TiggerTimeEvent();
    //        }
    //    }
    //    if (fCoolTime != 0)
    //    {
    //        fCoolTime -= owner.GetBehaviorDeltaTime();
    //        if (fCoolTime <= float.Epsilon)
    //        {
    //            fCoolTime = 0;
    //        }
    //    }
    //}

    //void TiggerTimeEvent()
    //{
    //    EventParameter eventBuffParam = EventParameter.Get();
    //    eventBuffParam.goParameter = owner.gameObject;
    //    eventBuffParam.objParameter = owner;
    //    eventBuffParam.intParameter = -1;
    //    OnWorking(GameEvent.GE_EVENT_TIME, eventBuffParam);
    //}

    //public override void Stacking()
    //{
    //    //叠加buff事件
    //    if (eventID == GameEvent.GE_EVENT_BUFF_STATCKING)
    //    {
    //        EventParameter eventBuffParam = EventParameter.Get();
    //        eventBuffParam.goParameter = owner.gameObject;
    //        eventBuffParam.objParameter = owner;
    //        eventBuffParam.intParameter = -1;
    //        OnWorking(GameEvent.GE_EVENT_BUFF_STATCKING, eventBuffParam);
    //    }
    //}
    //// 
    //public void OnWorking(GameEvent ge, EventParameter paramter)
    //{
    //    if (effectDesc != null)
    //    {
    //        if (EventSourceCondition(paramter) == false)
    //        {
    //            return;
    //        }
    //    }
 
    //    //条件判断放到时间判断的前面，以免触发冷却时间
    //    if (CheckCondition(ge) == false)
    //    {
    //        return;
    //    }

    //    // 判断冷却时间
    //    if (fCoolTime != 0)
    //        return;


    //    fCoolTime = fCoolIntervalTime;
        

    //    bool bDel = false;
    //    bool bAddParam1 = false;


    //    times--;
    //    if (times <= 0)
    //    {

    //        int tempParameter = 0;
    //        //读取buff间临时参数
    //        switch (eventID)
    //        {
    //            case GameEvent.GE_EVENT_DOUBLEDAMAGE:
    //                tempParameter = paramter.intParameter2;
    //                break;
    //        }

    //        if (type == (int)EventBuffType.MONO_DEL) // 删除
    //        {
    //            //增加触发概率
    //            if(ReachProbability() == false )
    //            {
    //                return;
    //            }

    //            bDel |= RemoveBuff(owner);
                
    //            if (bDel && iBuffIDOwner != 0)
    //            {
    //              //  owner.AddbuffWithTempParameter(iBuffIDOwner, giver, tempParameter);
    //            }
    //        }
    //        else if (type == (int)EventBuffType.MONO_ADD) // 添加
    //        {
    //            //增加触发概率
    //            if (ReachProbability() == false)
    //            {
    //                return;
    //            }
    //           // owner.AddbuffWithTempParameter(iBuffIDOperate, giver, ownerBuff.skillID, tempParameter);
    //        }
    //        else if (type == (int)EventBuffType.MONO_TARGET_ADD)
    //        {
    //            //增加触发概率
    //            if (ReachProbability() == false)
    //            {
    //                return;
    //            }

    //            GameObject obj = paramter.goParameter1;
    //            if (obj != null)
    //            {
    //                ActorObj enemyTarget = GetActorBaseParameter1(paramter);
    //                if (enemyTarget != null)
    //                {
    //                    //给其他目标添加类buff的fParam_1参数是是否同时给自身加个buff
    //                    if (iBuffIDOwner > 0)
    //                    {
    //                 //       owner.AddbuffWithTempParameter(iBuffIDOwner, giver, 0, tempParameter);
    //                    }
    //                  //  enemyTarget.AddbuffWithTempParameter(iBuffIDOperate, giver, ownerBuff.skillID, tempParameter);
    //                }
    //            }

               
    //        }
    //        else if (type == (int)EventBuffType.MONO_TARGET_DEL)
    //        {
    //            //增加触发概率
    //            if (ReachProbability() == false)
    //            {
    //                return;
    //            }

    //            GameObject obj = paramter.goParameter1;
    //            if (obj != null)
    //            {
    //                ActorObj enemyTarget = GetActorBaseParameter1(paramter);
    //                if (enemyTarget != null)
    //                {
    //                    bDel |= RemoveBuff(enemyTarget);
    //                }

    //                if (bDel && iBuffIDOwner != 0)
    //                {
    //               //     owner.AddbuffWithTempParameter(iBuffIDOwner, giver, ownerBuff.skillID, tempParameter);
    //                }
    //            }
    //        }
    //        else
    //        {
    //            // 范围内的敌人
    //            //GameObject obj = null;
    //            List<ActorObj> objList = CoreEntry.gActorMgr.GetAllActors();
    //            for (int i = 0; i < objList.Count; i++)
    //            {
    //                //对IOS出现怪物不动 报错的异常  进行错误处理
    //                //if (GameLogicMgr.checkValid(objList[i]) == false)
    //                //{
    //                //    continue;
    //                //}
    //                bDel = false;
    //                //obj = objList[i];
    //                ActorObj actorBase = objList[i];

    //                float aimDistance = 0f;
    //                if (actorBase != null && owner != null)
    //                {
    //                    aimDistance = Vector3.Distance(owner.transform.position, actorBase.transform.position) -
    //                       actorBase.GetColliderRadius() - 0.2f;
    //                }

    //                if (aimDistance > effectDesc.fParam[3]) // 在范围外
    //                    continue;

    //                //增加触发概率
    //                if (ReachProbability() == false)
    //                {
    //                    continue;
    //                }

    //                //// 临时判断
    //                //if (owner.IsSkillAim((int)Configs.skillConfig.AimEffectObjectTypeEnum.FRIEND, actorBase))// 友方
    //                //{
    //                //    if (type == (int)EventBuffType.MULT_FRIEND_ADD)
    //                //    {
    //                //    //    actorBase.AddbuffWithTempParameter(iBuffIDOperate, giver, ownerBuff.skillID, tempParameter);
    //                //    }
    //                //    else if (type == (int)EventBuffType.MULT_FRIEND_DEL)
    //                //    {
    //                //        bDel |= RemoveBuff(actorBase);
    //                //    }
    //                //    else if (type == (int)EventBuffType.MULT_TEAM_ADD)
    //                //    {
    //                //        if (SkillTargetSelector.IsPlayer(actorBase))
    //                //        {
    //                //          //  actorBase.Addbuff(iBuffIDOperate, giver, ownerBuff.skillID);
    //                //        }
    //                //    }
    //                //    else if (type == (int)EventBuffType.MULT_TEAM_DEL)
    //                //    {
    //                //        if (SkillTargetSelector.IsPlayer(actorBase))
    //                //        {
    //                //            bDel |= RemoveBuff(actorBase);
    //                //        }
    //                //    }
    //                //}
    //                //else
    //                //    if (owner.IsSkillAim((int)Configs.skillConfig.AimEffectObjectTypeEnum.ENEMY, actorBase)) // 敌方
    //                //    {
    //                //        if (type == (int)EventBuffType.MULT_ENEMY_ADD)
    //                //        {
    //                //         //   actorBase.AddbuffWithTempParameter(iBuffIDOperate, giver, ownerBuff.skillID, tempParameter);
    //                //        }
    //                //        else if (type == (int)EventBuffType.MULT_ENEMY_DEL)
    //                //        {
    //                //            bDel |= RemoveBuff(actorBase);
    //                //        }
    //                //        else if (type == (int)EventBuffType.MULT_ENEMYTEAM_ADD)
    //                //        {
    //                //            if (SkillTargetSelector.IsPlayer(actorBase))
    //                //            {
    //                //          //      actorBase.AddbuffWithTempParameter(iBuffIDOperate, giver, ownerBuff.skillID, tempParameter);
    //                //            }
    //                //        }
    //                //        else if (type == (int)EventBuffType.MULT_ENEMYTEAM_DEL)
    //                //        {
    //                //            if (SkillTargetSelector.IsPlayer(actorBase))
    //                //            {
    //                //                bDel |= RemoveBuff(actorBase);
    //                //            }
    //                //        }
    //                //    }
    //                // 给范围内目标添加的Buff
    //                if (bDel && iBuffIDTarget != 0)
    //                {
    //                  //  actorBase.AddbuffWithTempParameter(iBuffIDTarget, giver, ownerBuff.skillID, tempParameter);
    //                    bAddParam1 = true;
    //                }
    //            }
    //            // 给自己添加的Buff, 只加一次
    //            if (bAddParam1 && iBuffIDOwner != 0)
    //            {
    //             //   owner.AddbuffWithTempParameter(iBuffIDOwner, giver, ownerBuff.skillID, tempParameter);
    //            }
               
    //        }
    //    }
        
    //}

    //bool RemoveBuff(ActorObj actorBase)
    //{
    //    if (actorBase != null)
    //    {
    //        if (iBuffIDOperate == 1)
    //        {
    //            return actorBase.RemoveSpecialTypeBuff(skillBuff.BufftypeEnum.POSITIVE, (int)customParam);
    //        }
    //        else if (iBuffIDOperate == 2)
    //        {
    //            return actorBase.RemoveSpecialTypeBuff(skillBuff.BufftypeEnum.NEGATIVE, (int)customParam);
    //        }
    //        else
    //        {
    //        //    return actorBase.RemoveBuff(iBuffIDOperate, giver);
    //        }
    //    }

    //    return false;
    //}

    //bool ReachProbability()
    //{
    //    if (effectDesc != null && effectDesc.probability < 1)
    //    {
    //        if (UnityEngine.Random.value > effectDesc.probability)
    //        {
    //            return false;
    //        }
    //    }

    //    return true;
    //}

    //bool CheckCondition(GameEvent ge)
    //{
    //    switch (ge)
    //    {
    //        case GameEvent.GE_EVENT_BEHIT:
    //            {
    //                //体力比例大于1时，必定触发
    //                if (customParam >= 1)
    //                {
    //                    return true;
    //                }
                    
    //                float percent = (float)owner.curHp / owner.maxHp;
    //                if (percent <= customParam)
    //                {
    //                    //当前体力比例小于等于设定比例，触发
    //                    return true;
    //                }
                    
    //                //其他情况不触发
    //                return false;
    //            }
                
    //    }
    //    return true;
    //}

    //ActorObj GetActorBase(EventParameter paramter)
    //{
    //    ActorObj source = paramter.objParameter as ActorObj;
    //    if (source == null)
    //    {
    //        source = paramter.goParameter.GetComponent<ActorObj>();
    //    }
    //    return source;
    //}

    //ActorObj GetActorBaseParameter1(EventParameter paramter)
    //{
    //    ActorObj source = paramter.objParameter1 as ActorObj;
    //    if (source == null)
    //    {
    //        source = paramter.goParameter1.GetComponent<ActorObj>();
    //    }
    //    return source;
    //}

    ///// <summary>
    ///// 事件来源判断
    ///// </summary>
    ///// <returns></returns>
    //bool EventSourceCondition(EventParameter paramter)
    //{
    //    //switch (effectDesc.source)
    //    //{
    //    //    case (sbyte)buffConfig .SourceEnum.SELF:
    //    //        {
    //    //            // 判断是否是当前对象的事件
    //    //            if (paramter.goParameter != ownerObj)
    //    //            {
    //    //                return false;
    //    //            }
    //    //        }

    //    //        break;
    //    //    case (sbyte)buffConfig .SourceEnum.TEAM:
    //    //        {
    //    //            ActorObj source = GetActorBase(paramter);

    //    //            if (source != null)
    //    //            {
    //    //                if (owner.m_TeamType == source.m_TeamType && SkillTargetSelector.IsPlayer(source))
    //    //                {
    //    //                    return true;
    //    //                }

    //    //                return false;
    //    //            }

    //    //        }
    //    //        break;
    //    //    case (sbyte)buffConfig .SourceEnum.FRIEND:
    //    //        {
    //    //            ActorObj source = GetActorBase(paramter);

    //    //            if (source != null)
    //    //            {
    //    //                if (owner.IsSkillAim((sbyte)Configs.skillConfig.AimEffectObjectTypeEnum.FRIEND, source) == false)
    //    //                {
    //    //                    return false;
    //    //                }
    //    //            }

    //    //        }
    //    //        break;

    //    //    case (sbyte)buffConfig .SourceEnum.ENEMY:
    //    //        {
    //    //            ActorObj source = GetActorBase(paramter);

    //    //            if (source != null)
    //    //            {
    //    //                if (owner.IsSkillAim((sbyte)Configs.skillConfig.AimEffectObjectTypeEnum.ENEMY, source) == false)
    //    //                {
    //    //                    return false;
    //    //                }
    //    //            }

    //    //        }
    //    //        break;
    //    //    case (sbyte)buffConfig .SourceEnum.ALL:

    //    //        break;
    //    //}

    //    return true;
    //}

}

}



