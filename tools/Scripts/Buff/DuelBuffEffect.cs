using UnityEngine;
using System.Collections;

using XLua;

namespace SG
{
    // 决斗Buff
[Hotfix]
public class DuelBuffEffect : BuffEffect
{

    public override void Init(LuaTable  desc, ActorObj _owner, ActorObj _giver)
    {
        base.Init(desc, _owner, _giver);

    }
    
    public override void OnEnter(Buff _ownerBuff)
	{
		base.OnEnter(_ownerBuff);
        if (owner.curActorState == ACTOR_STATE.AS_ATTACK)
        {
            AttackState attackState = owner.GetActorState(ACTOR_STATE.AS_ATTACK) as AttackState;
            if (attackState != null&&attackState.m_curSkillBase!=null && attackState.m_curSkillBase.m_hitActor != null)
            {
                owner.SetBuffDuelActor(attackState.m_curSkillBase.m_hitActor.gameObject, ownerBuff.desc.Get<int>("last_time"));
            }
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        owner.SetBuffDuelActor(null, 0);

    }
}
}

