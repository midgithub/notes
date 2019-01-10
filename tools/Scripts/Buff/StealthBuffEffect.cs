using XLua;
﻿using UnityEngine;
using System.Collections;


namespace SG
{

[Hotfix]
public class StealthBuffEffect : BuffEffect
{
    protected bool bAlwayHide;

 //   public override void Init(buffConfig  desc, ActorObj _owner, ActorObj _giver)
	//{
 //       base.Init(desc, _owner, _giver);
 //       bAlwayHide = (effectDesc.fParam[0]==0)?false:true;
 //   }

	//public override void OnEnter(Buff _ownerBuff)
	//{
 //       base.OnEnter(_ownerBuff);
 //       owner.OnStealthState(true, bAlwayHide);

 //   }
	//public override void OnExit()
	//{
 //       base.OnExit();
 //       owner.OnStealthState(false, bAlwayHide);
	//}
}

}

