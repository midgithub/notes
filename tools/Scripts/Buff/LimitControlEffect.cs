using UnityEngine;
using System.Collections;

using XLua;

namespace SG
{
    

// 操作限制类Buff 释放技能
[Hotfix]
    public class LimitControlEffect : TimerBasedBuffEffect
{

        protected int m_type;

        public override void Init(LuaTable desc, ActorObj _owner, ActorObj _giver)
        {
            base.Init(desc, _owner, _giver);
            RelativeState = ACTOR_STATE.AS_LIMIT;
            //desc.stringParam1;    
            //desc.stringParam2;    
            //desc.fParam[0];     
            //desc.fParam[1];     
            //desc.fParam[2];     
            //desc.fParam[3];     
            //desc.fParam[4];   

            //m_type = 0;
            //m_type = (desc.Get<int>("limitType") == (int)limitType.Dizzy) ? (int)limitType.Dizzy : 0;
            //m_type = (desc.Get<int>("limitType") == (int)limitType.Frozen) ? (int)limitType.Frozen : 0;
            //m_type = (desc.Get<int>("limitType") == (int)limitType.Move) ? (int)limitType.Move : 0;
            //m_type = (desc.Get<int>("limitType") == (int)limitType.Attack) ? (int)limitType.Attack : 0;
            //m_type = (desc.Get<int>("limitType") == (int)limitType.Sleep) ? (int)limitType.Sleep : 0;
            //m_type = (desc.Get<int>("limitType") == (int)limitType.Skill) ? (int)limitType.Skill : 0;
            //m_type = (desc.Get<int>("limitType") == (int)limitType.Floating) ? (int)limitType.Floating : 0;
            //m_type = (desc.Get<int>("limitType") == (int)limitType.Hitdown) ? (int)limitType.Hitdown : 0;
            m_type = desc.Get<int>("limitType");

        }

        public override void OnEnter(Buff _ownerBuff)
        {
            base.OnEnter(_ownerBuff);

            if (m_type == (int)limitType.Dizzy) 
            {
                // 眩晕
                DizzyParam param = new DizzyParam();
                param.keepTime = ownerBuff.mLife;
                owner.OnEnterDizzy(param);

                //眩晕对应的状态不同
                RelativeState = ACTOR_STATE.AS_DIZZY;
            }
            
            if (m_type == (int)limitType.Sleep || m_type == (int)limitType.Frozen 
                || m_type == (int)limitType.Move || m_type == (int)limitType.Attack || m_type == (int)limitType.Skill
                || m_type == (int)limitType.Floating || m_type == (int)limitType.Hitdown)
            {
                LimitParam param = new LimitParam();
                param.keepTime = ownerBuff.mLife;
                param.bStopActor = false;
                param.iState = m_type;
                if(m_type == (int)limitType.Sleep)
                {
                    // 睡眠
                    param.buffid = ownerBuff.mBuffType;
                }
                if (m_type== (int)limitType.Frozen)
                {
                    // 冰冻
                    param.bStopActor = true;
                }

                owner.OnEnterLimit(param);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (m_type > (int)limitType.Dizzy)
            {
                LimitParam param = new LimitParam();
                param.keepTime = 0;
                param.bStopActor = false;
                param.iState = 0;
                owner.OnEnterLimit(param);
            }
        }

        public override void ReplaceBuff()
        {
            base.OnExit();
            base.ReplaceBuff();
            OnEnter(ownerBuff);
        }

    }

}

