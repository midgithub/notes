using UnityEngine;
using System.Collections;

using XLua;

namespace SG
{
[Hotfix]
    public class CharmParam
    {
        public float keepTime;
        public int iState;
        public int nTeamType;

    };


[Hotfix]
    public class CharmBuffEffect : BuffEffect
    {

        public override void Init(LuaTable  desc, ActorObj _owner, ActorObj _giver)
        {
            base.Init(desc, _owner, _giver);

        }

        public override void OnEnter(Buff _ownerBuff)
        {
            base.OnEnter(_ownerBuff);
            CharmParam param = new CharmParam();
            param.keepTime = _ownerBuff.mLife;
            param.iState = 1;
            owner.OnEnterCharm(param);
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

