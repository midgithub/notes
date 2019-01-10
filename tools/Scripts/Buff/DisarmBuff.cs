using XLua;

namespace SG
{

[Hotfix]
    public class DisarmParam
    {
        public float keepTime;
        public int iState;

    };


[Hotfix]
    public class DisarmBuffEffect : BuffEffect
    {

        public override void Init(LuaTable  desc, ActorObj _owner, ActorObj _giver)
        {
            base.Init(desc, _owner, _giver);

        }

        public override void OnEnter(Buff _ownerBuff)
        {
            base.OnEnter(_ownerBuff);
            DisarmParam param = new DisarmParam();
            param.keepTime = _ownerBuff.mLife;
            param.iState = 1;
            owner.OnEnterDisarm(param);
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

