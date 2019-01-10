
using System.Collections;

using XLua;

namespace SG
{

[Hotfix]
    public class RemoveBuff : BuffEffect
{

    public override void Init(LuaTable  desc, ActorObj _owner, ActorObj _giver)
    {
        base.Init(desc, _owner, _giver);

    }
    
    public override void OnEnter(Buff _ownerBuff)
	{
		base.OnEnter(_ownerBuff);

  //      // 这样转格式，不会出现误差
		//int iType	= System.Convert.ToInt32(effectDesc.fParam[0]);
		//int Count	= System.Convert.ToInt32(effectDesc.fParam[1]);

  //      owner.RemoveBuff(iType, Count);
    }

}
}

