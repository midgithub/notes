using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{

[Hotfix]
public class SkillGifts
{
    public bool isMonster = false;
    /// <summary>
    /// 复活后多少帧重置天赋
    /// </summary>
    protected int delayFrameForRecover = 0;
    /// <summary>
    /// 当前是否复活状态
    /// </summary>
    protected bool isRecover = false;

    public List<int> SkillList = new List<int>();

    public virtual void Init(ActorObj m_actor)
    {
        SkillList.Clear();
        return;
        //for (int i = 0; i < m_actor.mBaseAttr.GiftsSkillIdArray.Length; i++)
        //{
        //    int skillID = m_actor.mBaseAttr.GiftsSkillIdArray[i];
        //    SkillList.Add(skillID);
        //}
    }

    public void OnEnter(ActorObj m_actor)
    {
      //  for (int i = 0; i < SkillList.Count; i++)
      //  {
      //      int skillID = SkillList[i];
      //      if (skillID == 0)
      //          continue;
      //      Configs.skillConfig skillDesc = m_actor.GetCurSkillDesc(skillID);
      //      if (skillDesc == null)
      //          continue;

         
         
      //      //if (skillDesc.passive <= m_actor.mBaseAttr.StarLevel || isMonster) // 星级
      //      //{
      //      //    m_actor.Addbuff(skillDesc.OwnerBuffID, m_actor, skillID);
      //      //}
      ////      m_actor.Addbuff(skillDesc.OwnerBuffID, m_actor, skillID);
      //  }

        isRecover = false;
    }

    public void OnExit(ActorObj m_actor)
    {
        //for (int i = 0; i < SkillList.Count; i++)
        //{
        //    int skillID = m_actor.mBaseAttr.GiftsSkillIdArray[i];
        //    if (skillID == 0)
        //        continue;
        //    Configs.skillConfig skillDesc = m_actor.GetCurSkillDesc(skillID);
        //    if (skillDesc == null)
        //        continue;
        //    //if (skillDesc.passive <= m_actor.mBaseAttr.StarLevel || isMonster) // 星级
        //    //{
        //    //    m_actor.RemoveBuff(skillDesc.OwnerBuffID, m_actor);
        //    //}
        //   //     m_actor.RemoveBuff(skillDesc.OwnerBuffID, m_actor);
        //    }
    }

    public void OnDie(ActorObj m_actor)
    {
        OnExit(m_actor);

    }


    public void Recover(ActorObj m_actor)
    {
        OnExit(m_actor);
        OnEnter(m_actor);
        delayFrameForRecover = 2;
        isRecover = true;
      
    }

    public void Update(ActorObj m_actor)
    {
        if (isRecover)
        {
            delayFrameForRecover--;
            if (delayFrameForRecover <= 0)
            {
                OnEnter(m_actor);
            }
        }
    }

     // 重新计算二级属性时调用
    static public void RefreshAttrs(BaseAttr attr, bool bMonster)
    {
        //for (int i = 0; i < attr.GiftsSkillIdArray.Length; i++)
        //{
        //    int skillID = attr.GiftsSkillIdArray[i];
        //    if (skillID == 0)
        //        continue;
        //    Configs.skillConfig skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(skillID);
        //    if (skillDesc == null)
        //        continue;
        //    //if (skillDesc.passive <= attr.StarLevel || bMonster) // 星级
        //    //{
        //    //    Buff.RefreshAttrs(skillDesc.OwnerBuffID, attr);
        //    //}
        //    //    Buff.RefreshAttrs(skillDesc.OwnerBuffID, attr);
        //    }
    }

}

};  //end SG

