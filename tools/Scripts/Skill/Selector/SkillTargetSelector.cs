using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public enum AimEffectObjectTypeEnum
    {
        ENEMY,          //敌人

        FRIEND,             //自己人

    }



[Hotfix]
    public class SkillTargetSelector 
    {
        //public ActorObj GetTarget(ActorObj actorBase, Configs.skillConfig skillDesc, DamageCell dmgCell)
        //{
        //    return GetTarget(actorBase, (AimEffectObjectTypeEnum)skillDesc.faction_limit, skillDesc.range, (Configs.skillConfig.OperationTypeEnum)skillDesc.operationType, (Configs.skillConfig.TypefilterEnum)skillDesc.typefilter, (Configs.skillConfig.TargetFilterEnum)skillDesc.TargetFilter, dmgCell);
        //}

        //public ActorObj GetTarget(ActorObj actorBase, AimEffectObjectTypeEnum aimEffectObjectType, float range, Configs.skillConfig.OperationTypeEnum operationType, Configs.skillConfig.TypefilterEnum typeFilter, Configs.skillConfig.TargetFilterEnum targetFilter)
        //{
        //    return GetTarget(actorBase, aimEffectObjectType, range, operationType, typeFilter, targetFilter, null);
        //}

        /// <summary>
        /// 获得唯一目标
        /// </summary>
        /// <param name="actorBase"></param>
        /// <param name="aimEffectObjectType"></param>
        /// <param name="range"></param>
        /// <param name="operationType"></param>
        /// <param name="typeFilter"></param>
        /// <param name="targetFilter"></param>
        /// <param name="dmgCell"></param>
        /// <returns></returns>
        //public ActorObj GetTarget(ActorObj actorBase, Configs.skillConfig.AimEffectObjectTypeEnum aimEffectObjectType, float range, Configs.skillConfig.OperationTypeEnum operationType, Configs.skillConfig.TypefilterEnum typeFilter, Configs.skillConfig.TargetFilterEnum targetFilter, DamageCell dmgCell) 
        //{
        //    //如果target是自己，则直接返回
        //    if (targetFilter == Configs.skillConfig.TargetFilterEnum.SELF)
        //    {
        //        return actorBase;
        //    }

        //    List<ActorObj> norminatedList = GetTargetList(actorBase, aimEffectObjectType, range, operationType, typeFilter, dmgCell);

        //    if (norminatedList.Count > 1)
        //    {
        //        switch (targetFilter)
        //        {
        //            case Configs.skillConfig.TargetFilterEnum.MAXHP:
        //                norminatedList = MaxHpFilter(actorBase,  norminatedList);
        //                break;
        //            case Configs.skillConfig.TargetFilterEnum.MINHP:
        //                norminatedList = MinHpFilter(actorBase,  norminatedList);
        //                break;
        //            case Configs.skillConfig.TargetFilterEnum.MAXHPPERCENT:
        //                norminatedList = MaxHpPercentFilter(actorBase,  norminatedList);
        //                break;
        //            case Configs.skillConfig.TargetFilterEnum.MINHPPERCENT:
        //                norminatedList = MinHpPercentFilter(actorBase,  norminatedList);
        //                break;
        //            case Configs.skillConfig.TargetFilterEnum.MINDISTANCE:
        //                norminatedList = MinDistanceFilter(actorBase,  norminatedList);
        //                break;
        //            case Configs.skillConfig.TargetFilterEnum.MAXDISTANCE:
        //                norminatedList = MaxDistanceFilter(actorBase,  norminatedList);
        //                break;
        //            case Configs.skillConfig.TargetFilterEnum.RANDOM:
        //                norminatedList = RandomFilter(actorBase,  norminatedList);
        //                break;
        //        }
        //    }

        //    if (norminatedList.Count > 0)
        //    {
        //        return norminatedList[0];
        //    }
        //    return null;
        //}

        /// <summary>
        /// 获取一组目标
        /// </summary>
        /// <param name="actorBase"></param>
        /// <param name="aimEffectObjectType"></param>
        /// <param name="range"></param>
        /// <param name="operationType"></param>
        /// <param name="typeFilter"></param>
        /// <param name="dmgCell"></param>
        /// <returns></returns>
    //    public List<ActorObj> GetTargetList(ActorObj actorBase, Configs.skillConfig.AimEffectObjectTypeEnum aimEffectObjectType, float range, Configs.skillConfig.OperationTypeEnum operationType, Configs.skillConfig.TypefilterEnum typeFilter, DamageCell dmgCell)
    //    {
    //        List<ActorObj> norminatedList = GetNorminatedTarget(actorBase, aimEffectObjectType);

    //        if (dmgCell != null)
    //        {
    //            norminatedList = RangeCondition(actorBase, dmgCell, norminatedList);
    //        }
    //        else if (range > 0)
    //        {
    //            norminatedList = RangeCondition(actorBase, range, norminatedList);
    //        }

    //        if (operationType != Configs.skillConfig.OperationTypeEnum.ALL)
    //        {
    //            norminatedList = OperationCondition(actorBase, operationType, norminatedList);
    //        }

    //        if (typeFilter != Configs.skillConfig.TypefilterEnum.NONE)
    //        {
    //            norminatedList = TypeFilterCondition(actorBase, typeFilter, norminatedList);
    //        }

    //        return norminatedList;
    //    }


    //    /// <summary>
    //    /// 获得预选actorBase
    //    /// </summary>
    //    /// <param name="actorBase"></param>
    //    /// <param name="skillDesc"></param>
    //    /// <returns></returns>
    //    protected virtual List<ActorObj> GetNorminatedTarget(ActorObj actorBase, Configs.skillConfig.AimEffectObjectTypeEnum aimEffectObjectType)
    //    {
    //        return null;
    //    }

    //    protected bool CheckValidGameObject(ActorObj actorBase, Configs.skillConfig.AimEffectObjectTypeEnum aimEffectObjectType, ActorObj obj, bool isMonster)
    //    {
    //         ActorObj targetActorBase = null;
    //         return CheckValidGameObject(actorBase, aimEffectObjectType, obj, ref targetActorBase, isMonster);
    //    }

    //    protected bool CheckValidGameObject(ActorObj actorBase, Configs.skillConfig.AimEffectObjectTypeEnum aimEffectObjectType, ActorObj obj, ref ActorObj targetActorBase, bool isMonster)
    //    {

    //        //对IOS出现怪物不动 报错的异常  进行错误处理
    //        //if (GameLogicMgr.checkValid(obj) == false)
    //        //{
    //        //    return false;
    //        //}

    //        ActorObj tmpActorBase = obj;
            
    //        targetActorBase = tmpActorBase;
            

    //        //报错的异常  进行错误处理
    //        if (tmpActorBase == null || ( isMonster== false && tmpActorBase.mBaseAttr == null))
    //        {
    //            return false;

    //        }

    //        if (actorBase.IsSkillAim((sbyte)aimEffectObjectType, tmpActorBase) && !tmpActorBase.IsDeath())
    //        {
    //            return true;
    //        }

    //        return false;
    //    }

    //    protected bool CheckEnemyState(ActorObj targetActorBase,ActorObj ownerActorBase)
    //    {
    //        //除塔以为物体，入场状态也不处理
    //        if (targetActorBase.m_bIsTower == false && targetActorBase.curActorState == ACTOR_STATE.AS_ENTER)
    //        {
    //            return false;
    //        }

    //        //隐身状态 by yuxj
    //        if (targetActorBase.IsInStealthState(ownerActorBase))
    //        {
    //            return false; 
    //        }

    //        return true;
    //    }

    //    /// <summary>
    //    /// 范围处理
    //    /// </summary>
    //    /// <param name="actorBase"></param>
    //    /// <param name="skillDesc"></param>
    //    /// <param name="target"></param>
    //    /// <returns></returns>
    //    protected List<ActorObj> RangeCondition(ActorObj actorBase, DamageCell dmgCell, List<ActorObj> target)
    //    {
    //        List<ActorObj> list = null;
    //        if (target != null)
    //        {
    //            list = new List<ActorObj>();
              
    //            for (int i = 0; i < target.Count; ++i)
    //            {
    //                if (CoreEntry.gSkillMgr.IsSkillDamageRange(dmgCell.GetOneDamageInfo(), actorBase.transform, target[i].transform, target[i].GetColliderRadius()))
    //                {
    //                    list.Add(target[i]);
    //                }
    //            }

    //        }
    //        return list;
    //    }

    //    /// <summary>
    //    /// 范围处理
    //    /// </summary>
    //    /// <param name="actorBase"></param>
    //    /// <param name="skillDesc"></param>
    //    /// <param name="target"></param>
    //    /// <returns></returns>
    //    protected List<ActorObj> RangeCondition(ActorObj actorBase, float range, List<ActorObj> target)
    //    {
    //        List<ActorObj> list = null;
    //        if (target != null)
    //        {
    //            list = new List<ActorObj>();
    //            float rangeSQ = range * range;

    //            for (int i = 0; i < target.Count; ++i)
    //            {
    //                if ((target[i].transform.position - actorBase.transform.position).sqrMagnitude <= rangeSQ)
    //                {
    //                    list.Add(target[i]);
    //                }
    //            }

    //        }
    //        return list;
    //    }

    //    static public bool IsHero(ActorObj actorbase)
    //    {
    //        if (IsBoss(actorbase) || IsPlayer(actorbase) || (actorbase.mActorType == ActorType.AT_AVATAR))
    //        {
    //            return true;
    //        }

    //        return false;
    //    }

    //    static public bool IsBoss(ActorObj actorbase)
    //    {
    //        if (actorbase.mActorType == ActorType.AT_BOSS)
    //        {
    //            return true;
    //        }

    //        return false;
    //    }

    //    /// <summary>
    //    /// 是否是玩家
    //    /// </summary>
    //    /// <param name="actorbase"></param>
    //    /// <returns></returns>
    //    static public bool IsPlayer(ActorObj actorbase)
    //    {
    //        if (actorbase.mActorType == ActorType.AT_LOCAL_PLAYER || actorbase.mActorType == ActorType.AT_PVP_PLAYER || actorbase.mActorType == ActorType.AT_REMOTE_PLAYER)
    //        {
    //            return true;
    //        }

    //        return false;
    //    }

    //    /// <summary>
    //    /// 是否是小怪
    //    /// </summary>
    //    /// <param name="actorbase"></param>
    //    /// <returns></returns>
    //    static public bool IsCommonMonster(ActorObj actorbase)
    //    {
    //        if (actorbase.mActorType == ActorType.AT_MONSTER)
    //        {
    //            return true;
    //        }

    //        return false;
    //    }


    //    static public bool IsLivings(ActorObj actorbase)
    //    {
    //        if (IsHero(actorbase) || IsCommonMonster(actorbase))
    //        {
    //            return true;
    //        }

    //        return false;
    //    }

    //    static public bool IsMechanics(ActorObj actorbase)
    //    {
    //        if (actorbase.mActorType == ActorType.AT_MECHANICS)
    //        {
    //            return true;
    //        }

    //        return false;
    //    }

    //    /// <summary>
    //    /// 移动的类型
    //    /// </summary>
    //    /// <param name="actorbase"></param>
    //    /// <returns></returns>
    //    static public bool IsMovings(ActorObj actorbase)
    //    {
    //        if (IsLivings(actorbase) || IsMechanics(actorbase))
    //        {
    //            return true;
    //        }

    //        return false;
    //    }

    //    /// <summary>
    //    /// 是否为其他类
    //    /// </summary>
    //    /// <param name="actorbase"></param>
    //    /// <returns></returns>
    //    static public bool IsOthers(ActorObj actorbase)
    //    {
    //        if (actorbase.mActorType == ActorType.AT_NPC || actorbase.mActorType == ActorType.AT_TRAP || actorbase.mActorType == ActorType.AT_BROKED)
    //        {
    //            return true;
    //        }

    //        return false;
    //    }

    //    /// <summary>
    //    /// 静态类型，包括可以移动的NPC
    //    /// </summary>
    //    /// <param name="actorbase"></param>
    //    /// <returns></returns>
    //    static public bool IsStatic(ActorObj actorbase)
    //    {
    //        if (actorbase.mActorType == ActorType.AT_BROKED || actorbase.mActorType == ActorType.AT_NPC)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    /// <summary>
    //    /// 可以被攻击的类型
    //    /// </summary>
    //    /// <param name="actorbase"></param>
    //    /// <returns></returns>
    //    static public bool IsAttackable(ActorObj actorbase)
    //    {
    //        if (IsMovings(actorbase) || IsStatic(actorbase))
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    /// <summary>
    //    /// 不能被攻击的类型
    //    /// </summary>
    //    /// <param name="actorbase"></param>
    //    /// <returns></returns>
    //    static public bool IsUnattackable(ActorObj actorbase)
    //    {
    //        if (actorbase.mActorType == ActorType.AT_TRAP || actorbase.mActorType == ActorType.AT_NON_ATTACK)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    /// <summary>
    //    /// OperationType操作
    //    /// </summary>
    //    /// <param name="actorBase"></param>
    //    /// <param name="skillDesc"></param>
    //    /// <param name="target"></param>
    //    /// <returns></returns>
    //    protected List<ActorObj> OperationCondition(ActorObj actorBase, Configs.skillConfig.OperationTypeEnum operationType, List<ActorObj> target)
    //    {
    //        List<ActorObj> list = null;
            
    //        if (target != null)
    //        {
    //            List<ActorObj> heroList = new List<ActorObj>();
    //            List<ActorObj> commonMonsterList = new List<ActorObj>();
    //            List<ActorObj> livingsList = new List<ActorObj>();
    //            List<ActorObj> mechanicsList = new List<ActorObj>();
    //            List<ActorObj> movingsList = new List<ActorObj>();
    //            List<ActorObj> staticList = new List<ActorObj>();
    //            List<ActorObj> attackableList = new List<ActorObj>();
    //            List<ActorObj> unattackableList = new List<ActorObj>(); 
              
    //            for (int i = 0; i < target.Count; ++i)
    //            {
    //                if (IsHero(target[i]))
    //                {
    //                    heroList.Add(target[i]);
    //                    livingsList.Add(target[i]);
    //                    movingsList.Add(target[i]);
    //                    attackableList.Add(target[i]);
    //                }
    //                else if(IsCommonMonster(target[i]))
    //                {
    //                    commonMonsterList.Add(target[i]);
    //                    livingsList.Add(target[i]);
    //                    movingsList.Add(target[i]);
    //                    attackableList.Add(target[i]);
    //                }
    //                else if (IsMechanics(target[i]))
    //                {
    //                    mechanicsList.Add(target[i]);
    //                    movingsList.Add(target[i]);
    //                    attackableList.Add(target[i]);
    //                }
    //                else if (IsStatic(target[i]))
    //                {
    //                    staticList.Add(target[i]);
    //                    attackableList.Add(target[i]);
    //                }
    //                else if (IsUnattackable(target[i]))
    //                {
    //                    unattackableList.Add(target[i]);
    //                }
                   
    //            }

    //            switch (operationType)
    //            {
    //                case Configs.skillConfig.OperationTypeEnum.HEROES:
    //                    list = heroList;
    //                    break;

    //                case Configs.skillConfig.OperationTypeEnum.MONSTER:
    //                    list = commonMonsterList;
    //                    break;

    //                case Configs.skillConfig.OperationTypeEnum.LIVINGS:
    //                    list = livingsList;
    //                    break;

    //                case Configs.skillConfig.OperationTypeEnum.MECHANIC:
    //                    list = mechanicsList;
    //                    break;

    //                case Configs.skillConfig.OperationTypeEnum.MOVINGS:
    //                    list = movingsList;
    //                    break;

    //                case Configs.skillConfig.OperationTypeEnum.STATIC:
    //                    list = staticList;
    //                    break;

    //                case Configs.skillConfig.OperationTypeEnum.ATTACKABLE:
    //                    list = attackableList;
    //                    break;

    //                case Configs.skillConfig.OperationTypeEnum.UNATTACKABLE:
    //                    list = unattackableList;
    //                    break;
    //                default:
    //                    LogMgr.ErrorLog("operationType should be one of HEROES/MONSTER/LIVINGS/MECHANIC/STATIC/ATTACKABLE/UNATTACKABLE/ALL");
    //                    break;
    //            }
    //        }
    //        return list;
    //    }

    //    protected List<ActorObj> TypeFilterCondition(ActorObj actorBase, Configs.skillConfig.TypefilterEnum typeFilter, List<ActorObj> target)
    //    {
    //        //默认是所有target
    //        List<ActorObj> list = target;

    //        if (target != null)
    //        {
    //            List<ActorObj> typeList = new List<ActorObj>();

    //            for (int i = 0; i < target.Count; ++i)
    //            {
    //                switch (typeFilter)
    //                {
    //                    case Configs.skillConfig.TypefilterEnum.HEROESFIRST:
    //                        if (IsHero(target[i]))
    //                        {
    //                            typeList.Add(target[i]);
    //                        }
    //                        break;
    //                    case Configs.skillConfig.TypefilterEnum.MONSTERFIRST:
    //                        if (IsCommonMonster(target[i]))
    //                        {
    //                            typeList.Add(target[i]);
    //                        }
    //                        break;
    //                }
    //            }

    //            //如果存在某一个特定的种类，则用某特定种类
    //            if (typeList.Count > 0)
    //            {
    //                list = typeList;
    //            }

    //        }

    //        return list;
    //    }


    //    public delegate void AllDoHandler(ActorObj obj);

    //    protected void AllDo(List<ActorObj> list, AllDoHandler handle)
    //    {
    //        if (handle != null)
    //        {
    //            for (int i = 0; i < list.Count; ++i)
    //            {

    //                handle(list[i]);
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 最大血量过滤器
    //    /// </summary>
    //    /// <param name="actorBase"></param>
    //    /// <param name="skillDesc"></param>
    //    /// <param name="target"></param>
    //    /// <returns></returns>
    //    protected List<ActorObj> MaxHpFilter(ActorObj actorBase, List<ActorObj> target)
    //    {
    //        List<ActorObj> list = new List<ActorObj>();
    //        int maxhp = int.MinValue;
          
    //        ActorObj final = null;
    //        AllDo(target, (a) =>
    //            {
    //                if (a.mBaseAttr.CurHP > maxhp)
    //                {
    //                    final = a;
    //                    maxhp = a.mBaseAttr.CurHP;
    //                }
    //            });

    //        list.Add(final);
    //        return list;
    //    }

    //    /// <summary>
    //    /// 最小血量过滤器
    //    /// </summary>
    //    /// <param name="actorBase"></param>
    //    /// <param name="skillDesc"></param>
    //    /// <param name="target"></param>
    //    /// <returns></returns>
    //    protected List<ActorObj> MinHpFilter(ActorObj actorBase, List<ActorObj> target)
    //    {
    //        List<ActorObj> list = new List<ActorObj>();
    //        int minhp = int.MaxValue;

    //        ActorObj final = null;
    //        AllDo(target, (a) =>
    //        {
    //            if (a.mBaseAttr.CurHP < minhp)
    //            {
    //                final = a;
    //                minhp = a.mBaseAttr.CurHP;
    //            }
    //        });

    //        list.Add(final);
    //        return list;
    //    }

    //    /// <summary>
    //    /// 最大血量百分比过滤器
    //    /// </summary>
    //    /// <param name="actorBase"></param>
    //    /// <param name="skillDesc"></param>
    //    /// <param name="target"></param>
    //    /// <returns></returns>
    //    protected List<ActorObj> MaxHpPercentFilter(ActorObj actorBase,  List<ActorObj> target)
    //    {
    //        List<ActorObj> list = new List<ActorObj>();
    //        float maxhpPercent = float.MinValue;

    //        ActorObj final = null;
    //        AllDo(target, (a) =>
    //        {
    //            float hpPercent = a.mBaseAttr.CurHP / (float)a.mBaseAttr.MaxHP;
    //            if (hpPercent > maxhpPercent)
    //            {
    //                final = a;
    //                maxhpPercent = hpPercent;
    //            }
    //        });

    //        list.Add(final);
    //        return list;
    //    }

    //    /// <summary>
    //    /// 最小血量百分比过滤器
    //    /// </summary>
    //    /// <param name="actorBase"></param>
    //    /// <param name="skillDesc"></param>
    //    /// <param name="target"></param>
    //    /// <returns></returns>
    //    protected List<ActorObj> MinHpPercentFilter(ActorObj actorBase,  List<ActorObj> target)
    //    {
    //        List<ActorObj> list = new List<ActorObj>();
    //        float minhpPercent = float.MaxValue;

    //        ActorObj final = null;
    //        AllDo(target, (a) =>
    //        {
    //            float hpPercent = a.mBaseAttr.CurHP / (float)a.mBaseAttr.MaxHP;
    //            if (hpPercent < minhpPercent)
    //            {
    //                final = a;
    //                minhpPercent = hpPercent;
    //            }
    //        });

    //        list.Add(final);
    //        return list;
    //    }

    //    /// <summary>
    //    /// 最小距离过滤器
    //    /// </summary>
    //    /// <param name="actorBase"></param>
    //    /// <param name="skillDesc"></param>
    //    /// <param name="target"></param>
    //    /// <returns></returns>
    //    protected List<ActorObj> MinDistanceFilter(ActorObj actorBase,  List<ActorObj> target)
    //    {
    //        List<ActorObj> list = new List<ActorObj>();
    //        float minDistanceSQ = float.MaxValue;

    //        ActorObj final = null;
    //        AllDo(target, (a) =>
    //        {
    //            //最短距离排除掉自己
    //            if (a != actorBase)
    //            {
    //                float distanceSQ = (a.transform.position - actorBase.transform.position).sqrMagnitude;
    //                if (distanceSQ < minDistanceSQ)
    //                {
    //                    final = a;
    //                    minDistanceSQ = distanceSQ;
    //                }
    //            }
    //        });

    //        list.Add(final);
    //        return list;
    //    }

    //    /// <summary>
    //    /// 最大距离过滤器
    //    /// </summary>
    //    /// <param name="actorBase"></param>
    //    /// <param name="skillDesc"></param>
    //    /// <param name="target"></param>
    //    /// <returns></returns>
    //    protected List<ActorObj> MaxDistanceFilter(ActorObj actorBase, List<ActorObj> target)
    //    {
    //        List<ActorObj> list = new List<ActorObj>();
    //        float maxDistanceSQ = float.MinValue;

    //        ActorObj final = null;
    //        AllDo(target, (a) =>
    //        {
    //            float distanceSQ = (a.transform.position - actorBase.transform.position).sqrMagnitude;
    //            if (distanceSQ > maxDistanceSQ)
    //            {
    //                final = a;
    //                maxDistanceSQ = distanceSQ;
    //            }
    //        });

    //        list.Add(final);
    //        return list;
    //    }

    //    /// <summary>
    //    /// 随机筛选器
    //    /// </summary>
    //    /// <param name="actorBase"></param>
    //    /// <param name="skillDesc"></param>
    //    /// <param name="target"></param>
    //    /// <returns></returns>
    //    protected List<ActorObj> RandomFilter(ActorObj actorBase,  List<ActorObj> target)
    //    {
    //        List<ActorObj> list = new List<ActorObj>();

    //        int index = Random.Range(0, target.Count);
    //        ActorObj final = target[index];

    //        list.Add(final);
    //        return list;
    //    }

    //    public bool AOEActorFilter(ActorObj actorBase, Configs.skillConfig skillDesc, ActorObj targetActoBase)
    //    {
    //        if (targetActoBase != null && targetActoBase.mBaseAttr != null)
    //        {
    //            switch (skillDesc.operationType)
    //            {
    //                case (sbyte)Configs.skillConfig.OperationTypeEnum.HEROES:
    //                    {
    //                        if (IsHero(targetActoBase))
    //                        {
    //                            return true;
    //                        }

    //                        return false;
    //                    }
    //                case (sbyte)Configs.skillConfig.OperationTypeEnum.MONSTER:
    //                    {
    //                        if (IsCommonMonster(targetActoBase))
    //                        {
    //                            return true;
    //                        }

    //                        return false;
    //                    }
    //                case (sbyte)Configs.skillConfig.OperationTypeEnum.LIVINGS:
    //                    {
    //                        if (IsLivings(targetActoBase))
    //                        {
    //                            return true;
    //                        }

    //                        return false;
    //                    }
    //                case (sbyte)Configs.skillConfig.OperationTypeEnum.MECHANIC:
    //                    {
    //                        if (IsMechanics(targetActoBase))
    //                        {
    //                            return true;
    //                        }

    //                        return false;
    //                    }
    //                case (sbyte)Configs.skillConfig.OperationTypeEnum.MOVINGS:
    //                    {
    //                        if (IsMovings(targetActoBase))
    //                        {
    //                            return true;
    //                        }

    //                        return false;
    //                    }
    //                case (sbyte)Configs.skillConfig.OperationTypeEnum.STATIC:
    //                    {
    //                        if (IsStatic(targetActoBase))
    //                        {
    //                            return true;
    //                        }

    //                        return false;
    //                    }
    //                case (sbyte)Configs.skillConfig.OperationTypeEnum.ATTACKABLE:
    //                    {
    //                        if (IsAttackable(targetActoBase))
    //                        {
    //                            return true;
    //                        }
    //                        return false;
    //                    }
    //                case (sbyte)Configs.skillConfig.OperationTypeEnum.UNATTACKABLE:
    //                    {
    //                        if (IsUnattackable(targetActoBase))
    //                        {
    //                            return true;
    //                        }
    //                        return false;
    //                    }
    //            }

    //            return true;
    //        }

    //        return false;
    //    }

    //    public void AllAOETargetDo(ActorObj actorBase, Configs.skillConfig skillDesc, AllDoHandler handle)
    //    {
    //        List<ActorObj> norminatedList = GetNorminatedTarget(actorBase, (Configs.skillConfig.AimEffectObjectTypeEnum)skillDesc.aimEffectObjectType);

    //        if (skillDesc.operationType != (sbyte)Configs.skillConfig.OperationTypeEnum.NONE)
    //        {
    //            norminatedList = OperationCondition(actorBase, (Configs.skillConfig.OperationTypeEnum)skillDesc.operationType, norminatedList);
    //        }

    //        if (skillDesc.typefilter != (sbyte)Configs.skillConfig.TypefilterEnum.NONE)
    //        {
    //            norminatedList = TypeFilterCondition(actorBase, (Configs.skillConfig.TypefilterEnum)skillDesc.typefilter, norminatedList);
    //        }
    //        AllDo(norminatedList, handle);
    //    }


    //    public bool BuffActorFilter(ActorObj actorBase, Configs.skillBuff skillBuff, ActorObj targetActoBase)
    //    {
    //        if (targetActoBase != null && targetActoBase.mBaseAttr != null)
    //        {
    //            switch (skillBuff.operationType)
    //            {
    //                case (sbyte)Configs.skillBuff.OperationTypeEnum.HEROES:
    //                    {
    //                        if (IsHero(targetActoBase))
    //                        {
    //                            return true;
    //                        }

    //                        return false;
    //                    }
    //                case (sbyte)Configs.skillBuff.OperationTypeEnum.MONSTER:
    //                    {
    //                        if (IsCommonMonster(targetActoBase))
    //                        {
    //                            return true;
    //                        }

    //                        return false;
    //                    }
    //                case (sbyte)Configs.skillBuff.OperationTypeEnum.LIVINGS:
    //                    {
    //                        if (IsLivings(targetActoBase))
    //                        {
    //                            return true;
    //                        }

    //                        return false;
    //                    }
    //                case (sbyte)Configs.skillBuff.OperationTypeEnum.MECHANIC:
    //                    {
    //                        if (IsMechanics(targetActoBase))
    //                        {
    //                            return true;
    //                        }

    //                        return false;
    //                    }
    //                case (sbyte)Configs.skillBuff.OperationTypeEnum.MOVINGS:
    //                    {
    //                        if (IsMovings(targetActoBase))
    //                        {
    //                            return true;
    //                        }

    //                        return false;
    //                    }
    //                case (sbyte)Configs.skillBuff.OperationTypeEnum.STATIC:
    //                    {
    //                        if (IsStatic(targetActoBase))
    //                        {
    //                            return true;
    //                        }

    //                        return false;
    //                    }
    //                case (sbyte)Configs.skillBuff.OperationTypeEnum.ATTACKABLE:
    //                    {
    //                        if (IsAttackable(targetActoBase))
    //                        {
    //                            return true;
    //                        }
    //                        return false;
    //                    }
    //                case (sbyte)Configs.skillBuff.OperationTypeEnum.UNATTACKABLE:
    //                    {
    //                        if (IsUnattackable(targetActoBase))
    //                        {
    //                            return true;
    //                        }
    //                        return false;
    //                    }
    //            }

    //            return true;
    //        }

    //        return false;
    //    }

    }

    
}

