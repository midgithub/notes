using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{

[Hotfix]
public class SkillGiftsMgr
{
    List<SkillGifts> skillGiftsList = new List<SkillGifts>();
    SkillGifts gift = null;
    InspirationalSkillGifts inspirationalSkillGifts = null;
    public void Init(ActorObj actor,bool isMonster)
    {
        if (gift == null)
        {
            gift = new SkillGifts();
            skillGiftsList.Add(gift);
        }

        if (inspirationalSkillGifts == null)
        {
            inspirationalSkillGifts = new InspirationalSkillGifts();
            skillGiftsList.Add(inspirationalSkillGifts);
        }

        for (int i = 0; i < skillGiftsList.Count; ++i)
        {
            skillGiftsList[i].isMonster = isMonster;
            skillGiftsList[i].Init(actor);
        }
    }

    public void OnEnter(ActorObj actor)
    {
        for (int i = 0; i < skillGiftsList.Count; ++i)
        {
            skillGiftsList[i].OnEnter(actor);
        }
    }

    public void OnExit(ActorObj actor)
    {
        for (int i = 0; i < skillGiftsList.Count; ++i)
        {
            skillGiftsList[i].OnExit(actor);
        }
    }

    public void OnDie(ActorObj actor)
    {
        OnExit(actor);

    }

    public void Recover(ActorObj actor)
    {
        for (int i = 0; i < skillGiftsList.Count; ++i)
        {
            skillGiftsList[i].Recover(actor);
        }

    }



    public void Update(ActorObj actor)
    {
        for (int i = 0; i < skillGiftsList.Count; ++i)
        {
            skillGiftsList[i].Update(actor);
        }
    }

}

};  //end SG

