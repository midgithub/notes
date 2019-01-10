/**
* @file     : BossDeathFromSky.cs
* @brief    : 
* @details  : boos技能，死从天降
* @author   : zerodeng
* @date     : 2014-12-26
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
/*
 * 播放动作后，降boss拉到空中，去掉脚底影子，x秒后，选择目标位置，并且地面播放预警特效，影子出现，位移恢复
 */

[Hotfix]
public class BossDeathFromSky : ISkillCell {
    private BossDeathFromSkyDesc m_bossDeathFromSkyDesc = null;
    private SkillBase m_skillBase = null;
    ActorObj m_aimActorObject = null;
    private WarningDecel m_WarningDecel = null;

    GameObject m_efxObj = null;


    public override void Init(ISkillCellData cellData, SkillBase skillBase)
    {
        m_skillBase = skillBase;
        m_bossDeathFromSkyDesc = (BossDeathFromSkyDesc)cellData;
    }

    // PoolManager
    void OnEnable()
    {
        CancelInvoke("Start");
        Invoke("Start", 0.000001f);
    }

	// Use this for initialization
	void Start () 
	{
        CancelInvoke("Start");

        if (m_bossDeathFromSkyDesc == null)
        {
            return;
        }                    
    
        //m_skillBase = this.transform.root.gameObject.GetComponent<SkillBase>();
    
        m_aimActorObject = m_skillBase.m_actor.GetAttackObj();
        if (m_aimActorObject == null)
            m_aimActorObject = m_skillBase.m_hitActor;

        //影子去掉        	
        m_skillBase.m_actor.HideBlobShadow();

        //拉到空中                        
        Invoke("RiseToSky", m_bossDeathFromSkyDesc.riseTime);
        
        //下落准备，位移恢复
        Invoke("PrepareDownTime", m_bossDeathFromSkyDesc.prepareDownTime);

        //落到地面，触发事件
        Invoke("DownOnGround", m_bossDeathFromSkyDesc.downTime);
	}
	
	// Update is called once per frame
	//void Update () {
	
	//}

    void OnDestroy()
    {
        CancelInvoke("RiseToSky");
        CancelInvoke("PrepareDownTime");
        CancelInvoke("DownOnGround");

        // 关掉贴花，关掉特效
        if (m_WarningDecel)
        {
            m_WarningDecel.HideDecal();
        }
        if (m_efxObj)
        {
            Destroy(m_efxObj);
            m_efxObj = null;
        }
    }    

    //拉到空中,其实就是为了去掉阻挡。     
    void RiseToSky()
    {
        CancelInvoke("RiseToSky");
            
        m_skillBase.m_actor.transform.position += new Vector3(0,5,0);

        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BOSS_RISE_TO_SKY_START, null);        
    }

    void PrepareDownTime()
    {
        CancelInvoke("PrepareDownTime");
        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BOSS_RISE_TO_SKY_STATED, null);             
        //位移恢复
        m_skillBase.m_actor.transform.position -= new Vector3(0, 5, 0);

        if (m_aimActorObject != null)
        {
            m_skillBase.m_actor.transform.position = m_aimActorObject.thisGameObject.transform.position;
        }        

        //影子恢复
        m_skillBase.m_actor.ShowBlobShadow();   

        //脚底特效
        if (!string.IsNullOrEmpty(m_bossDeathFromSkyDesc.efxPrefab)&& m_aimActorObject != null)
        {
            Object oj = CoreEntry.gResLoader.LoadResource(m_bossDeathFromSkyDesc.efxPrefab);
            if(oj!=null)
            {
                 GameObject efxObj = Instantiate(oj) as GameObject;
                 SceneEfx efx = efxObj.AddComponent<SceneEfx>();
                 efx.Init(m_aimActorObject.thisGameObject.transform.position, 5);
            }

        }

        //预警贴花
        if (!string.IsNullOrEmpty(m_bossDeathFromSkyDesc.efxPrefab) && m_aimActorObject != null)
        {
            if (m_WarningDecel)
            {
                Vector3 pos = m_skillBase.m_actor.transform.position;
                m_WarningDecel.SetPositionOnGround(pos);
                m_WarningDecel.ShowDecal();
            }
            else
            {
                float fbig = 0.20f;   //贴花比实际伤害范围大百分之fbig
                DamageCell damageCell = m_skillBase.GetComponentInChildren<DamageCell>();
                //DamageCell damageCell = m_skillBase.m_actor.GetComponent<DamageCell>();
                if (damageCell)
                {
                    OneDamageInfo damageInfo = damageCell.GetOneDamageInfo();
                    if (null != damageInfo && !string.IsNullOrEmpty(m_bossDeathFromSkyDesc.efxPrefab))
                    {
                        //Vector3 pos = m_skillBase.m_actor.transform.position;

                        m_efxObj = Instantiate(
                        CoreEntry.gResLoader.LoadResource(m_bossDeathFromSkyDesc.efxPrefab)) as GameObject; //(damageInfo.efxWarning)) as GameObject;
                        
                        if(m_efxObj== null)return;

                        ParticleScaler ScaleComponet = m_efxObj.GetComponent<ParticleScaler>();
                        if (ScaleComponet == null)
                            ScaleComponet = m_efxObj.AddComponent<ParticleScaler>();
                        // ScaleComponet.particleScale = m_actor.actorCreatureDisplayDesc.sacle ;

                        //策划要求缩放里面的例子
                        //ScaleComponet.prevScale = 1.0f;


                        m_efxObj.transform.position = new Vector3(m_skillBase.m_actor.transform.position.x,
                            m_skillBase.m_actor.transform.position.y + 0.2f, m_skillBase.m_actor.transform.position.z);
                        if (DamageTypeID.DTID_FUN == damageInfo.type)
                        {
                            m_efxObj.transform.forward = m_skillBase.m_actor.transform.forward;
                            //m_efxObj.transform.localScale = new Vector3(1.0f, 0f, 1.0f) * damageInfo.damageNode.funDamage.radius * 2 * (1 + fbig);

                            ScaleComponet.particleScale = damageInfo.damageNode.funDamage.radius * 2 * (1 + fbig);

                        }
                        else if (DamageTypeID.DTID_RECT == damageInfo.type)
                        {
                            m_efxObj.transform.forward = m_skillBase.m_actor.transform.forward;
                            //m_efxObj.transform.localScale = new Vector3(1.0f, 0f, 1.0f) * damageInfo.damageNode.funDamage.radius * 2 * (1 + fbig);
                            ScaleComponet.particleScale = damageInfo.damageNode.funDamage.radius * 2 * (1 + fbig);

                        }



                        //Vector3 decalPos = pos + 0.2f * m_skillBase.m_actor.m_transform.up;  //实际贴花位置比原来高0.2米  防止看不到
                        //if (DamageTypeID.DTID_FUN == damageInfo.type)
                        //{
                        //    m_WarningDecel = WarningDecel.CreateSectorDecal(m_bossDeathFromSkyDesc.efxPrefab, decalPos,
                        //         damageInfo.damageNode.funDamage.radius * 2 * (1 + fbig), damageInfo.damageNode.funDamage.angle);
                        //}
                        //else if (DamageTypeID.DTID_RECT == damageInfo.type)
                        //{
                        //    m_WarningDecel = WarningDecel.CreateRectangleDecal(m_bossDeathFromSkyDesc.efxPrefab, decalPos,
                        //             damageInfo.damageNode.rectDamage.width * (1 + fbig), damageInfo.damageNode.rectDamage.length * (1 + fbig));
                        //}
                    }
                }
            }
        }    
    }

    void DownOnGround()
    {
        CancelInvoke("DownOnGround");

        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BOSS_RISE_TO_SKY_END, null);

        if (m_WarningDecel)
        {
            m_WarningDecel.HideDecal();
        }
        if (m_efxObj)
        {
            Destroy(m_efxObj);
            m_efxObj = null;
        }
    }
}

};  //end SG

