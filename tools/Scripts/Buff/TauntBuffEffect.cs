using UnityEngine;
using System.Collections;

using XLua;

namespace SG
{

[Hotfix]
    public class TauntBuffEffect : BuffEffect
    {
        int curSkillID = -1;
        int skillIndex = -1;
        LuaTable skillDesc = null;
        //bool willAttack = false;
        LimitParam tag = null;
        bool hasAI = false;

        public override void Init(LuaTable  desc, ActorObj _owner, ActorObj _giver)
        {
            base.Init(desc, _owner, _giver);
        }

        public override void OnEnter(Buff _ownerBuff)
        {

            base.OnEnter(_ownerBuff);

            skillIndex = -1;

       //     owner.BreakSkillWhenHit(ownerBuff.skillID);

            //玩家不可以操作方向
            owner.ReqirueCanNotBeControlledByInput();
            FroceToNormalAttack();
            tag = owner.ForbidToUseSkill();
            ActorAgent aa = owner.GetComponent<ActorAgent>();
            if (aa != null)
            {
                if (aa.enabled == true)
                {
                    aa.enabled = false;
                    hasAI = true;
                }
                else
                {
                    hasAI = false;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            owner.AllowToUseSkill(tag);

            //恢复玩家操作方向
            owner.ReleaseCanNotBeControlledByInput();

            if (hasAI)
            {
                ActorAgent aa = owner.GetComponent<ActorAgent>();
                if (aa != null)
                {
                    aa.enabled = true;
                }
            }
        }

        public override void Update(float fElapsed)
        {
            if (giver != null)
            {
                 bool forceAttack = false;

                 if (owner.RunForAttack(skillDesc.Get<float>("min_dispk"), giver) == false)
                 {
                     if (owner.curActorState != ACTOR_STATE.AS_ATTACK)
                     {
                         if (owner.CanChangeAttack() && owner.IsNonControl == false)
                         {
                             FroceToNormalAttack();
                             forceAttack = true;
                         }

                     }
                 }

                if (forceAttack == false)
                { 
                    owner.FaceTo(giver.thisGameObject.transform.position);
                }
               
            }
        }

        void FroceToNormalAttack()
        {
            curSkillID = owner.GetNextNormalAttack(ref skillIndex);
            skillDesc = owner.GetCurSkillDesc(curSkillID);
          
            owner.FaceTo(giver.thisGameObject.transform.position);
            PlayerObj playerObject = owner.GetComponent<PlayerObj>();
            if (playerObject != null)
            {
                playerObject.SelectTarget(giver);
            }
            owner.OnRunToAttack(curSkillID);
        }
    }

}

