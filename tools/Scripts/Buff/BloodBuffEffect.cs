using XLua;
﻿using UnityEngine;
using System.Collections;


namespace SG
{

[Hotfix]
public class BloodBuffEffect : BuffEffect
{
    float bloodper;
    float bloodvalue;
    int  iBuffIDOwner;
    float fCoolIntervalTime;
    float fCoolTime;

 //   public override void Init(buffConfig desc, ActorObj _owner, ActorObj _giver)
	//{
 //       base.Init(desc, _owner, _giver);

 //       bloodper   = desc.fParam[0];   // 血量百分比
 //       bloodvalue = desc.fParam[1];   // 血量值
 //       iBuffIDOwner = (int)desc.fParam[2];   // 添加的BuffID
 //       fCoolIntervalTime = desc.fParam[3];   // 间隔时间
 //       if(fCoolIntervalTime==0)
 //           fCoolIntervalTime = float.MaxValue;
 //   }

	//public override void OnEnter(Buff _ownerBuff)
	//{
 //       base.OnEnter(_ownerBuff);

 //   }
	//public override void OnExit()
	//{
 //       base.OnExit();

 //   }

 //   // Update is called once per frame
 //   public virtual void Update(float fElapsed)
 //   {
 //       if (fCoolTime != 0)
 //       {
 //           fCoolTime -= owner.GetBehaviorDeltaTime();
 //           if (fCoolTime <= float.Epsilon)
 //           {
 //               fCoolTime = 0;
 //           }
 //       }
 //       if (fCoolTime != 0)
 //           return;
 //       if (((float)owner.curHp) / ((float)owner.mBaseAttr.MaxHP) < bloodper || ((float)owner.curHp) < bloodvalue)
 //       {
 //          // owner.Addbuff(iBuffIDOwner, giver, ownerBuff.skillID);
 //           fCoolTime = fCoolIntervalTime;
 //       }
 //   }

}

}

