using UnityEngine;
using System.Collections;

using XLua;

namespace SG
{

[Hotfix]
    public class FearBuffEffect : TimerBasedBuffEffect
    {

        public override void Init(LuaTable  desc, ActorObj _owner, ActorObj _giver)
        {
            base.Init(desc, _owner, _giver);
            RelativeState = ACTOR_STATE.AS_FEAR;
        }

        public override void OnEnter(Buff _ownerBuff)
        {
            base.OnEnter(_ownerBuff);
            FearParam param = new FearParam();
            param.keepTime = _ownerBuff.mLife;
            param.iState = 1;
            owner.OnEnterFear(param);
        }

        public override void OnExit()
        {
            base.OnExit();
            // 退出变形状态
            // FearParam param = new FearParam();
            //param.keepTime = 0;
            // param.iState = 0;
            //owner.OnEnterFear(param);

        }
    }
}

