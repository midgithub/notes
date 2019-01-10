using XLua;

namespace SG
{

[Hotfix]
    public class AddNewBuff : EventBuffEffect
{

    public override void Init(LuaTable  desc, ActorObj _owner, ActorObj _giver)
    {
        base.Init(desc, _owner, _giver);

    
    
    }

	public override void OnEnter(Buff _ownerBuff)
	{
		base.OnEnter(_ownerBuff);

    }

}


}

