/**
* @file     : CollideDamageCell.cs
* @brief    : 
* @details  : 召唤其他NPC 元素
* @author   : 
* @date     : 2014-12-1
*/

using XLua;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    //SkillClassDisplayDesc.xml
    //<summonCell stage="1" posType="0" prefabPath="Skill/Cell/summon" desc="召唤幻像">
    //<npc npcID="51001"  对应 CreatureDesc.xlsx 中的ID 
    //count="1"    召唤的数量
    //dis="2.5"    召唤的间隔距离
    //killBefore="1" 新召唤时，杀死上次的召唤物
    //buffID="0"     给召唤物添加一个Buff
    //lifeTime="30"  存在时间(单位秒)，不设置就是永远存放
    //deadthToKill="true" 召唤者死亡时是否杀死他的召唤物
    //summonType="1" 召唤类型 0.属性从表中读取，1.属性从召唤者继承*表里设置的属性百分比，2.属性从召唤者继承*表里设置的属性百分比 模型外观变成召唤者
    //               可以继承的属性 CreatureDesc.xlsx maxHP 到 mBouncePercent 召唤类型为1或2时 这些值设置为继承的百分比 0 ~ 100
    //followMe = "true" 召唤物是否跟随召唤者
    ///>
    //</summonCell>

[Hotfix]
    public class SummonCell : ISkillCell
    {    
    public SummonCellDesc m_summonDesc = null;               
    private SkillBase m_skillBase = null;
    Vector3[] m_posList = new Vector3[8];

    public override void Init(ISkillCellData cellData, SkillBase skillBase)
    {
        m_skillBase = skillBase;
        m_summonDesc = (SummonCellDesc)cellData;

        // 初始化位置
        if (m_summonDesc.npcPosType == 0) // 9宫格
        {
            m_posList[0] = new Vector3(1f, 0, 0);
            m_posList[1] = new Vector3(-1f, 0, 0);

            m_posList[2] = new Vector3(0, 0, 1f);
            m_posList[3] = new Vector3(0, 0, -1f);

            m_posList[4] = new Vector3(1f, 0, 1f);
            m_posList[5] = new Vector3(-1f, 0, 1f);

            m_posList[6] = new Vector3(1f, 0, -1f);
            m_posList[7] = new Vector3(-1f, 0, -1f);
        }
        else
        if (m_summonDesc.npcPosType == 1) // 前排
        {
            m_posList[0] = new Vector3(0, 0, 1f);
            m_posList[1] = new Vector3(1f, 0, 1f);
            m_posList[2] = new Vector3(-1f, 0, 1f);

            m_posList[3] = new Vector3(0, 0, 2f);
            m_posList[4] = new Vector3(1f, 0, 2f);
            m_posList[5] = new Vector3(-1f, 0, 2f);
        }
        else
        if (m_summonDesc.npcPosType == 2) // 后排
        {
            m_posList[0] = new Vector3(0, 0, -1f);
            m_posList[1] = new Vector3(1f, 0, -1f);
            m_posList[2] = new Vector3(-1f, 0, -1f);

            m_posList[3] = new Vector3(0, 0, -2f);
            m_posList[4] = new Vector3(1f, 0, -2f);
            m_posList[5] = new Vector3(-1f, 0, -2f);
        }
    }

    // PoolManager
    void OnEnable()
    {
        CancelInvoke("Start");
        Invoke("Start", 0.000001f);
    }

    void OnDestroy()
    {
        CancelInvoke("Start");
    }
    
    void OnDisable()
    {
        CancelInvoke("Start");
    }

	// Use this for initialization
	void Start () 
	{
        CancelInvoke("Start");
        ////
        //m_skillBase = this.transform.root.gameObject.GetComponent<SkillBase>();
        if (m_skillBase == null || m_skillBase.m_actor == null)
            return;

        // 普通分身不产生分身
        if (m_skillBase.m_actor.m_bSummonMonster)
            return;

        // 是否删除之前召唤的生物
        if (m_summonDesc.killBefore == 1)
        {
            if(m_skillBase.m_actor.m_skillSummonList==null)
                m_skillBase.m_actor.m_skillSummonList = new List<ActorObj.SummonData>();

            int index = 0;
            while (true)
            {
                if (m_skillBase.m_actor.m_skillSummonList.Count <= index)
                    break;
                ActorObj.SummonData summon = m_skillBase.m_actor.m_skillSummonList[index];
                if (m_skillBase.m_actor.m_skillSummonList[index].SummonID == m_summonDesc.npcID)
                {
                    OnSummonDeath(m_skillBase.m_actor.m_skillSummonList[index].entityid);
                    m_skillBase.m_actor.m_skillSummonList.Remove(summon);
                }
                else
                    index++;
            }
        }

        Vector3 origin = m_skillBase.m_actor.transform.position;


        int POS_TYPE_TARGET  =  6 ;
        for (int i = 0; i < m_summonDesc.npcCount; i++)
        {

            Vector3 summonPos = GetSummonPos(m_skillBase.m_actor.transform, i);

            if (m_summonDesc.npcPosType == POS_TYPE_TARGET)  //召唤位置在敌方脚下
            {
                ActorObj actorObj = m_skillBase.m_actor.GetAttackObj();

                if (actorObj == null)
                {
                    actorObj = m_skillBase.m_hitActor;
                }
                else
                {
                    return; 
                }

                summonPos = actorObj.transform.position; 
            }
            else
            {
                summonPos = GetSummonPos(m_skillBase.m_actor.transform, i);

            }


            if (summonPos != Vector3.zero)
            {
                // 是否有障碍
                Vector3 bornPos = BaseTool.instance.GetWallHitPoint(origin, summonPos);
                // 地面高度
                bornPos = BaseTool.instance.GetGroundPoint(bornPos);
                // 载入
                GameObject obj = null;

                if (m_summonDesc.summonType == 2)
                {
                    CoreEntry.gObjPoolMgr.PushToPool(m_skillBase.m_actor.resid); // 加到对象池子里 
                    CoreEntry.gGameMgr.SpawnMonster(m_skillBase.m_actor.resid, bornPos, out obj);

                    
                }
                else
                {
                    CoreEntry.gObjPoolMgr.PushToPool(m_summonDesc.npcID); // 加到内存池子里 
                    CoreEntry.gGameMgr.SpawnMonster(m_summonDesc.npcID, bornPos, out obj);
                }

                obj.transform.localRotation = m_skillBase.m_actor.transform.localRotation;

                ActorObj summonActorbase = obj.GetComponent<ActorObj>();
                if (m_summonDesc.killBefore == 1)
                {
                    ActorObj.SummonData data = new ActorObj.SummonData();
                    data.SummonID     = m_summonDesc.npcID;
                    data.entityid     = summonActorbase.entityid;
                    data.lifeTime     = m_summonDesc.lifeTime;
                    data.deadthToKill = m_summonDesc.deadthToKill;
                    data.startTime    = Time.time;
                    m_skillBase.m_actor.m_skillSummonList.Add(data);
                }

                summonActorbase.m_bSummonMonster = true; // 召唤生物标记

                if (m_skillBase.m_actor.mActorType == ActorType.AT_PVP_PLAYER)
                    summonActorbase.mActorType = m_skillBase.m_actor.mActorType;

                if (m_skillBase.m_actor.mActorType == ActorType.AT_LOCAL_PLAYER)
                {
                    summonActorbase.TeamType = 1;
                }
                else
                {
                    summonActorbase.TeamType = 3;
                }

                if (summonActorbase.mActorType == ActorType.AT_BOSS)
                {
                    summonActorbase.mActorType = ActorType.AT_AVATAR;
                }

                // 1.属性从召唤者继承*表里设置的属性百分比
                if (m_summonDesc.summonType == 1)
                {
                   // m_skillBase.m_actor.mBaseAttr.SummonTo(summonActorbase.mBaseAttr);
                    summonActorbase.curHp = summonActorbase.mBaseAttr.MaxHP;

                    // 重置AI
                    Destroy(summonActorbase.GetComponent<ActorAgent>());
                    ActorAgent actorAgent = null;
                    if (m_summonDesc.followMe)
                        actorAgent = summonActorbase.gameObject.AddComponent<PlayerAgent>();
                   

                    ActorObj actorobj = summonActorbase.GetComponent<ActorObj>();

                    actorAgent.enabled = true;
                    actorobj.enabled = true;
                    //summonActorbase.actorCreatureInfo.battleSpeed = m_skillBase.m_actor.actorCreatureInfo.battleSpeed;
                    //summonActorbase.mBaseAttr.Speed = summonActorbase.actorCreatureInfo.battleSpeed;
                    actorobj.SetSpeed(summonActorbase.mBaseAttr.Speed);
                }
                else
                // 2.属性从召唤者继承*表里设置的属性百分比 模型外观变成召唤者
                if (m_summonDesc.summonType == 2)
                {
                    // 设置当前血量百分比
                    BaseAttr newCardAttr = new BaseAttr();
                    summonActorbase.mBaseAttr = newCardAttr;
                    summonActorbase.mBaseAttr = newCardAttr;
                   // m_skillBase.m_actor.mBaseAttr.SummonTo(summonActorbase.mBaseAttr);
                    float fHpRatio = (float)m_skillBase.m_actor.curHp / (float)m_skillBase.m_actor.mBaseAttr.MaxHP;
                    summonActorbase.curHp = (int)(summonActorbase.mBaseAttr.MaxHP * fHpRatio);
                    // 
                    //MonsterHeath monsterHeath = summonActorbase.GetComponent<MonsterHeath>();
                    //if (monsterHeath != null)
                    //    monsterHeath.ResetHp(summonActorbase.mBaseAttr.MaxHP);
                    //PlayerHeath  playerHeath  = summonActorbase.GetComponent<PlayerHeath>();
                    //if (playerHeath != null)
                    //    playerHeath.OnResetHP();

                    // 重置AI
                    Destroy(summonActorbase.GetComponent<ActorAgent>());
                    
                    ActorAgent actorAgent = null;
                    if(m_summonDesc.followMe)
                        actorAgent = summonActorbase.gameObject.AddComponent<PlayerAgent>();
                    
                    // 设置速度
                    if (actorAgent != null)
                    {
                        actorAgent.enabled = true;
                        ActorObj actorobj = summonActorbase.GetComponent<ActorObj>();
                        actorobj.enabled = true;

                        //summonActorbase.actorCreatureInfo.battleSpeed = m_skillBase.m_actor.actorCreatureInfo.battleSpeed;
                        //summonActorbase.mBaseAttr.Speed = summonActorbase.actorCreatureInfo.battleSpeed;
                        actorobj.SetSpeed(summonActorbase.mBaseAttr.Speed);
                    }

                    // 颜色
                    Color color = new Color(67f / 255f, 146f / 255f, 223f / 255f, 205f / 255f);
                    summonActorbase.SetMaterialColor("DZSMobile/CharacterColor", "_BobyColor", color);
                    //summonActorbase.NoRecoverShader();

                    summonActorbase.m_bNoDieAction = true; // 不播放死亡动作
                }

                // 
                if (m_summonDesc.buffID != 0)
                {
           //         summonActorbase.Addbuff(m_summonDesc.buffID, m_skillBase.m_actor, 0);
                }
            }
        }
	}

    Vector3 GetSummonPos(Transform tran, int posIndex)
    {
        Vector3 sumPos = new Vector3(0,0,0);
        if(m_posList[posIndex]!=Vector3.zero)
        {
            sumPos = m_posList[posIndex] * m_summonDesc.npcDis;
            sumPos = tran.TransformPoint(sumPos);
        }
        return sumPos;
    }
    
    // 某个召唤物需要被杀死
    static public void OnSummonDeath(int entityid)
    {
        ActorObj actorObj = CoreEntry.gActorMgr.GetActorByEntityID(entityid);
        if (actorObj != null)
        {
            
            if (!actorObj.IsDeath())
            {
                if (actorObj.mActorType == ActorType.AT_TRAP)
                {
                    //GameObject.Destroy(actorObj.gameObject);
                }
                actorObj.ChangeState(ACTOR_STATE.AS_DEATH);
                CoreEntry.gActorMgr.RemoveActor(actorObj);

                //CoreEntry.gObjPoolMgr.RecycleObject(actorObj.actorCreatureInfo.ID, actorObj.gameObject);
            }
        }
    }
    
    // 杀死某角色的所有召唤物
    static public void OnSummonKillAll(ActorObj actorBase)
    {
        // 
        if (actorBase.m_skillSummonList != null)
        {
            int index = 0;
            while (true)
            {
                if (actorBase.m_skillSummonList.Count <= index)
                    break;
                ActorObj.SummonData summon = actorBase.m_skillSummonList[index];
                if (actorBase.m_skillSummonList[index].deadthToKill)
                {
                    OnSummonDeath(actorBase.m_skillSummonList[index].entityid);
                    actorBase.m_skillSummonList.Remove(summon);
                }
                else
                    index++;
            }
        }
    }
    
    // 有生命时间的 召唤物需要更新
    static public void OnSummonUpdate(ActorObj actor)
    {
        // 召唤物按时间消失
        if (actor.m_skillSummonList != null)
        {
            for (int i = 0; i < actor.m_skillSummonList.Count; i++)
            {
                if (actor.m_skillSummonList[i].lifeTime != float.MaxValue)
                {
                    if ((actor.m_skillSummonList[i].startTime + actor.m_skillSummonList[i].lifeTime) < Time.time)
                    {
                        SummonCell.OnSummonDeath(actor.m_skillSummonList[i].entityid);
                        actor.m_skillSummonList.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}
};  //end SG

