using System.Collections;

using XLua;

/// <summary>
/// 大天使重生技能
/// </summary>
namespace SG
{
[Hotfix]
public class RebornEffect : BuffEffect
{
    //private BattleTeam.TeamEventHandler m_curHandler;
    //private int m_iRecoverHP = 0;

    public override void Init(LuaTable  _desc, ActorObj _owner, ActorObj _giver)
    {
        base.Init(_desc, _owner, _giver);

         // 播放的生命值
         //  m_iRecoverHP = (int)_desc.fParam[1];
         //m_iRecoverHP = 10000;

        }

    public override void OnEnter(Buff _ownerBuff)
    {
        base.OnEnter(_ownerBuff);
        //InitHandler(Reborn);
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_EVENT_DIE, GE_EVENT_DIE);
    }

    //private void InitHandler(BattleTeam.TeamEventHandler handler)
    //{
    //    m_curHandler = handler;
    //    owner.behavior.team.OnMemberDying += m_curHandler;
    //}

    // Update is called once per frame
    public override void Update(float fElapsed)
    {

    }

    public override void OnExit()
    {
        //base.OnExit();
        //if (m_curHandler != null)
        //    owner.behavior.team.ReleaseEventHandler(m_curHandler);
        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_EVENT_DIE, GE_EVENT_DIE);
    }

    public void GE_EVENT_DIE(GameEvent ge, EventParameter paramter)
    {
        //失效后，不能再重生
        if (bFinished)
        {
            return;
        }

        if (paramter == null)
        {
            return;
        }

        if (owner == null)
        {
            return;
        }

        if (paramter.goParameter != owner.gameObject)
        {
            return ;
        }

        if (owner != null)
        {
            owner.ForceToRevive();
        }

        ownerBuff.FinishEffect();
        
    }

//    void Reborn(BattleTeam sender, TeamEventArgs args)
//    {
//        if (sender != null && args != null)
//        {
//            ActorInfo actorInfo = args.relatedActor;
//            args.bReborn = true;
//            args.iRelatedId = ownerBuff.desc.iId; // buff id

//            int angleID = owner.iRole;
//            bool bAngleDie = (actorInfo.iRole == angleID);
//            if (bAngleDie)
//            { // 大天使死亡
//                //1. 令大天使重生
//                // Do it in RebornLS.cs

//                //2. 給其他队友加血
//                for (int i = 0; i < sender.members.Count; ++i)
//                {
//                    if (sender.members[i].iRole != angleID)
//                    {
//                        int curHP = sender.members[i].attrib.GetAttrValue(AttribID.Hp);
//                        int maxHP = sender.members[i].attrib.GetAttrValue(AttribID.MaxHp);
//                        if (curHP < maxHP)
//                        {
//                            curHP += m_iRecoverHP;
//                            if (curHP > maxHP)
//                            {
//                                curHP = maxHP;
//                            }
//                            sender.members[i].attrib.SetAttrValue(AttribID.Hp, curHP);
//#if !MAMI_LOGIC_SERVER
//                            Messenger.Broadcast<object>("RefreshHP", sender.members[i]);
//#endif
//                        }
//                    }
//                }
//            }
//            else
//            { // 队友死亡
//                //1. 令队友重生
//                // Do it in RebornLS.cs

//                //2. 给大天使加血
//                int curHP = owner.attrib.GetAttrValue(AttribID.Hp);
//                int maxHP = owner.attrib.GetAttrValue(AttribID.MaxHp);
//                if (curHP < maxHP)
//                {
//                    curHP += m_iRecoverHP;
//                    if (curHP > maxHP)
//                    {
//                        curHP = maxHP;
//                    }
//                    owner.attrib.SetAttrValue(AttribID.Hp, curHP);
//#if !MAMI_LOGIC_SERVER
//                    Messenger.Broadcast<object>("RefreshHP", owner);
//#endif
//                }
//            }

//            OnExit(); // 技能只使用一次
//        }
//    }
}
}

