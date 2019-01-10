using UnityEngine;
using System.Collections;

using System;
using XLua;

namespace SG
{
// 在目标身上绑定一个特效
[Hotfix]
public class ScaleBuffEffect : BuffEffect
{
    float fRateTime;
    Vector3 fBuffScale;
    Vector3 fOldScale = new Vector3(1f,1f,1f);
    Transform refNode = null;

    public override void Init(LuaTable  desc, ActorObj _owner, ActorObj _giver)
    {
        base.Init(desc, _owner, _giver);

        refNode    = _owner.transform;

        float fScale = Convert.ToSingle(desc.Get<string>("buff_param"));

        fBuffScale = new Vector3(fScale, fScale, fScale);
        fRateTime  = 0.55f;
       
        //if (_owner.actorCreatureDisplayDesc.sacle != 0)
        //{
        //    fBuffScale *= _owner.actorCreatureDisplayDesc.sacle;
        //    fOldScale   = new Vector3(_owner.actorCreatureDisplayDesc.sacle, _owner.actorCreatureDisplayDesc.sacle, _owner.actorCreatureDisplayDesc.sacle);
        //}

        //if (desc.fParam[3] < 0)
       // {
            refNode.transform.localScale = fBuffScale;
       // }
    }

    public override void OnEnter(Buff _ownerBuff)
    {
        base.OnEnter(_ownerBuff);

    }

    // Update is called once per frame
    public override void Update(float fElapsed)
    {
        if (ownerBuff.mLife > fRateTime)
        {
            if (refNode.transform.localScale != fBuffScale)
                refNode.transform.localScale = Vector3.Lerp(refNode.transform.localScale, fBuffScale, fElapsed*8);
        }
        else
        {
            if (refNode.transform.localScale != fOldScale)
                refNode.transform.localScale = Vector3.Lerp(refNode.transform.localScale, fOldScale, fElapsed*8);
        }
    }

    public override void OnExit()
    {
        refNode.transform.localScale = fOldScale;

    }

    public override void OnDie()
    {
        base.OnDie();

    }
}

}

