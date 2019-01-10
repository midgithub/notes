using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
[Hotfix]
    public class PlayerSkillTargetSelector:  SkillTargetSelector
    {

        //protected override List<ActorObj> GetNorminatedTarget(ActorObj actorBase, Configs.skillConfig.AimEffectObjectTypeEnum aimEffectObjectType)
        //{
        //    List<ActorObj> list = new List<ActorObj>();
        //    List<ActorObj> actors = CoreEntry.gActorMgr.GetAllActors();

        //    if (aimEffectObjectType == Configs.skillConfig.AimEffectObjectTypeEnum.FRIEND)
        //    {
        //         ActorObj targetActorBase = null;
        //         for (int i = 0; i < actors.Count; ++i)
        //         {
        //             if (CheckValidGameObject(actorBase, aimEffectObjectType, actors[i], ref targetActorBase, false))
        //             {
        //                 list.Add(targetActorBase);
        //             }

        //         }
        //    }
        //    else if (aimEffectObjectType == Configs.skillConfig.AimEffectObjectTypeEnum.ENEMY)
        //    {
        //        ActorObj targetActorBase = null;
        //        for (int i = 0; i < actors.Count; ++i)
        //        {
        //            if (CheckValidGameObject(actorBase, aimEffectObjectType, actors[i], ref targetActorBase, true))
        //            {
        //                if (targetActorBase != null && CheckEnemyState(targetActorBase, actorBase))
        //                {
        //                    list.Add( targetActorBase);
        //                }
        //            }
        //        }
        //    }
            

        //    return list;
        //}


     

       
    }
}

