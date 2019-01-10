/**
* @file     : ImmuneCell.cs
* @brief    : 
* @details  : 技能无敌元素
* @author   : 
* @date     : 2014-12-9
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{

[Hotfix]
public class LightningChainCell : DamageCell
{
    public override void Init(ISkillCellData cellData, SkillBase skillBase)
    {
        base.Init(cellData, skillBase);
    }

        public void CalculateDamage(GameObject obj)
        {
            bool isDamageSuccess = true;
            ActorObj actorBase = (obj.GetComponent<ActorObj>());

            //纠正被击表现
            DamageParam damageParam = new DamageParam();
            damageParam.skillID = m_skillBase.m_skillID;
            damageParam.attackActor = m_skillBase.m_actor;
            damageParam.behitActor = actorBase;
            damageParam.weight = m_oneDamageInfo.resetSkillWeight;
            damageParam.isNotUseCurveMove = m_oneDamageInfo.isNotUseCurveMove;

            CoreEntry.gSkillMgr.OnSkillDamage(damageParam);

            //是否有眩晕效果
            if (m_oneDamageInfo.dizzyParamDesc != null)
            {
                DizzyParam param = new DizzyParam();
                param.keepTime = m_oneDamageInfo.dizzyParamDesc.keepTime;

                actorBase.OnEnterDizzy(param);
            }

            if (isDamageSuccess && (actorBase.mActorType == ActorType.AT_MONSTER || actorBase.mActorType == ActorType.AT_MECHANICS))
            {
                m_isHadLoadFrameStop = true;


                GameObject frameStopObj = CoreEntry.gGameObjPoolMgr.InstantiateSkillCell("Skill/Cell/frameStop");
                frameStopObj.transform.parent = transform;

                ISkillCell skillCell = frameStopObj.GetComponent<ISkillCell>();

                skillCell.Init(m_oneDamageInfo.frameStopDesc, m_skillBase, actorBase);

                m_skillBase.AddSkillCell(frameStopObj);

            }
        }

    protected override void CalculateDamage()
    {
        CancelInvoke("CalculateDamage");

        //bool isDamageSuccess = false;

        List<GameObject>  targetList = new List<GameObject>();

        LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(m_skillBase.m_skillID);


            // 把自己添加进去
         targetList.Insert(0, m_skillBase.m_actor.gameObject);

        //复制目标队列
        List<ActorObj> actors = CoreEntry.gActorMgr.GetAllMonsterActors();
        GameObject[] objList = new GameObject[actors.Count];
        float[] fDisList = new float[objList.Length];
        for (int i = 0; i < objList.Length; i++)
        {
            objList[i] = actors[i].gameObject;
            fDisList[i] = Vector3.Distance(m_skillBase.m_actor.transform.position, objList[i].transform.position);
        }

        // 按距离排序，否则特效的效果会不好
        // 冒泡排序
        for (int i = 1; i < objList.Length; i++)
        {
            for (int j = objList.Length - 1; j >= i ; j--)
            {
                if (fDisList[j] < fDisList[j - 1])
                {
                    float tmp = fDisList[j - 1];
                    fDisList[j - 1] = fDisList[j];
                    fDisList[j] = tmp;

                    GameObject tmpObj = objList[j - 1];
                    objList[j - 1] = objList[j];
                    objList[j] = tmpObj;
                }
            }
        }

        for (int i = 0; i < objList.Length ; i++ )
        {
            GameObject obj = objList[i];
            ActorObj actorBase = (obj.GetComponent<ActorObj>());

            if (!m_skillBase.m_actor.IsSkillAim((sbyte)m_skillBase.m_skillDesc.Get<int>("faction_limit"), actorBase))
                continue;

            //按伤害范围算出受伤害对象，具体有没有伤害，还要看对应的属性(免疫等)
            bool isSkillSuccess = m_skillMgr.IsSkillDamageRange(skillDesc.Get<int>("effect_1"), targetList[targetList.Count - 1].transform, // 已最后一个受击者为中立
                obj.transform, actorBase.GetColliderRadius());
            //伤害对象
            if (isSkillSuccess)
            {
                targetList.Add(obj);
            }
        }

        //是否持续伤害                
        if (m_oneDamageInfo.isRepeatedly && m_repeateKeep > 0)
        {
            Invoke("CalculateDamage", m_oneDamageInfo.damageDiff / m_skillBase.m_speed);
            --m_repeateKeep;
        }

        if (targetList.Count > 1)
        {
            //GameObject obj1 = Instantiate(CoreEntry.gResLoader.LoadResource("Effect/scence/fx_shandianlian")) as GameObject;//CoreEntry.gGameObjPoolMgr.InstantiateEffect("Effect/scence/fx_shandianlian");
            GameObject obj1 = CoreEntry.gGameObjPoolMgr.InstantiateEffect("Effect/scence/fx_shandianlian");
            EfxLightningChain lighting = obj1.GetComponent<EfxLightningChain>();
            lighting.Init(this,targetList);

            SceneEfxPool efx1 = obj1.GetComponent<SceneEfxPool>();
            if (efx1 == null)
                efx1 = obj1.AddComponent<SceneEfxPool>();

            efx1.Init(Vector3.zero, 2f);     
        }
    }

    public override void Preload(ISkillCellData cellData, SkillBase skillBase)
    {
        base.Preload(cellData,skillBase);
        GameObject obj1 = CoreEntry.gGameObjPoolMgr.InstantiateEffect("Effect/scence/fx_shandianlian");
        CoreEntry.gGameObjPoolMgr.Destroy(obj1);
        GameObject obj2 = CoreEntry.gGameObjPoolMgr.InstantiateEffect("Effect/scence/fx_shandian");
        CoreEntry.gGameObjPoolMgr.Destroy(obj2);

    }

}

};  //end SG

