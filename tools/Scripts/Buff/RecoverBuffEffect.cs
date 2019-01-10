using UnityEngine;
using System.Collections;


using System;
using XLua;

namespace SG
{
    

// 操作限制类Buff 释放技能
[Hotfix]
public class RecoverBuffEffect : BuffEffect
{
    protected float keepTime;

    public override void Init(LuaTable  desc, ActorObj _owner, ActorObj _giver)
    {
        base.Init(desc, _owner, _giver);

            //desc.stringParam1;    
            //desc.stringParam2;    
            //desc.fParam[0];     
            //desc.fParam[1];     
            //desc.fParam[2];     
            //desc.fParam[3];     
            //desc.fParam[4];   
            string cdTime = desc.Get<string>("buff_param");
            if (cdTime == String.Empty) {
                cdTime = "0";
            }
            keepTime = Convert.ToInt32(cdTime) * 0.001f;
    }

    public override void OnEnter(Buff _ownerBuff)
    {
        base.OnEnter(_ownerBuff);

        if(keepTime==0)
            keepTime = ownerBuff.mLife;
        owner.RecoverHealth(keepTime);
    }

    // Update is called once per frame
    public override void Update(float fElapsed)
    {
        //base.Update();

    }

    public override void OnExit()
    {
        //base.OnExit();
        owner.ResetRecoverHealth();
    }

    public override void ReplaceBuff()
    {
        //base.ReplaceBuff();
        string cdTime = effectDesc.Get<string>("buff_param");
        if (cdTime == String.Empty)
        {
            cdTime = "0";
        }
        keepTime = Convert.ToInt32(cdTime) * 0.001f;

        if (keepTime == 0)
            keepTime = ownerBuff.mLife;
        owner.RecoverHealth(keepTime);

    }

}

}

