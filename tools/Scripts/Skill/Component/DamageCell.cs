using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{

//伤害模块:伤害类型,时间,数据都从表格读取
[Hotfix]
public class DamageCell : ISkillCell{
    //public gamedb.DamageCellDesc m_oneDamageInfo = null;    
    protected OneDamageInfo m_oneDamageInfo = null;
    protected Transform m_transform;
    protected SkillBase m_skillBase;
    protected GameLogicMgr m_gameManager;
    protected SkillMgr m_skillMgr;
    protected GameDBMgr m_gameDBMgr;
    protected WarningDecel m_WarningDecel = null;   //贴花预警效果

    protected WarningDecel m_SkillScope = null;


    protected EfxAttachActionPool m_RemainEfx = null;    
   

    //实例化的数据
    public int m_dataIndex = 0;
    protected int m_repeateKeep = 0;

    //顿帧，屏震一次伤害只处理一次
    protected bool m_isHadLoadFrameStop = false;

    public OneDamageInfo GetOneDamageInfo() { return m_oneDamageInfo; }

    void Awake()
    {
        m_transform = this.transform; 
        m_gameManager = CoreEntry.gGameMgr;               
        m_skillMgr = CoreEntry.gSkillMgr;        
        m_gameDBMgr = CoreEntry.gGameDBMgr;
    }

    public override void Init(ISkillCellData cellData, SkillBase skillBase) 
    {
        //m_dataIndex = param.damageCellIndex;                            
        m_oneDamageInfo = (OneDamageInfo)cellData;

        m_skillBase = skillBase;

        //重新计算hittime
        if (skillBase.m_skilleffect != null)
         {
               // m_oneDamageInfo.hitTime = skillBase.m_skilleffect.delay * 0.001f;
         }

        m_dataIndex = 0;
        m_repeateKeep = 0;
        m_isHadLoadFrameStop = false;
    }

    // PoolManager
    protected virtual void OnEnable()
    {
        CancelInvoke("Start");
        Invoke("Start", 0.000001f);
    }

	// Use this for initialization
	void Start ()
    {
        CancelInvoke("Start");

        if (m_skillBase == null || m_skillBase.m_actor == null)
            return;

        m_isHadLoadFrameStop = false;

        //yy test
        //LuaTable desc =  ConfigManager.Instance.Skill.GetSkillConfig( m_skillBase.m_skillID ) ;

        ////伤害计算
        //m_skillBase = this.transform.root.gameObject.GetComponent<SkillBase>();

        if (m_skillBase.m_onlyShowSkillScope)
            return;

        //gamedb.SkillDesc skillDesc = m_skillBase.m_actor.GetCurSkillDesc(m_skillBase.m_skillID);
        //SkillClassDisplayDesc skillClass = m_gameDBMgr.GetSkillClassDisplayDesc(m_skillBase.m_skillID);
            
        //if (skillClass.damageList.Count <= m_dataIndex)
        //{
        //    return;
        //}

        //m_oneDamageInfo = skillDesc.astDamageCellArray[m_dataIndex];
        //m_oneDamageInfo = skillClass.damageList[m_dataIndex];
        if (m_oneDamageInfo == null)
        {
            return;
        }

        m_repeateKeep = m_oneDamageInfo.keepTime;
        
        float hitTime = m_oneDamageInfo.hitTime / m_skillBase.m_speed ;

        //float endTime = (m_oneDamageInfo.keepTime * m_oneDamageInfo.damageDiff) / m_skillBase.m_speed;            

        float delayTime = 0;

        float curTime = m_skillBase.GetCurActionTime();
  
        //if (desc.attackAction.Length <1 )
        // {
        //    curTime = 0; 
        //}


        //LogMgr.UnityLog("hitTime=" + hitTime + ", curTime=" + curTime + ", skillid=" + m_skillBase.m_skillID);

        if (m_skillBase.SkillOverTime < hitTime)
        {
            LogMgr.UnityError(string .Format("技能结束时间:{0} < 打击时间:{1} , 将引发释放不出来 的BUG  skillID:{2}",m_skillBase.SkillOverTime ,hitTime ,  m_skillBase.m_skillID ) ); 
        }


        if (curTime >= hitTime)
        {
            CalculateDamage();
            LogMgr.UnityLog("----curTime:" + curTime + "     hitTime:" + hitTime); 
        }
        else
        {
            delayTime = hitTime - curTime;
            Invoke("CalculateDamage", delayTime);
        }

        if (m_oneDamageInfo.isRepeatedly)
        {
            //Invoke("EndDamage", endTime);
        }            
	}

    void EndDamage()
    {
        CancelInvoke("CalculateDamage");


        if (m_RemainEfx != null)
        {
            m_RemainEfx.DetachObject();
        }
    }

    protected virtual void CalculateDamage()
    {
       // System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
      //  sw.Start();
        CancelInvoke("CalculateDamage");

        bool isDamageSuccess = false;

        LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(m_skillBase.m_skillID);

        if (skillDesc == null)
             return;

            if (m_skillMgr.m_bShowSkillScope)
        {
            Vector3 pos = m_skillBase.m_actor.transform.position;
                if (null == m_SkillScope)
                {
                    if (m_skillBase.m_actor.mActorType == ActorType.AT_LOCAL_PLAYER || m_skillBase.m_actor.mActorType == ActorType.AT_BOSS)
                    {
                        if (DamageTypeID.DTID_FUN == m_oneDamageInfo.type)
                        {

                            if(m_oneDamageInfo.damageNode.funDamage.angle >= 360)
                            {
                                m_SkillScope = WarningDecel.CreateSectorDecal("Effect/skill/remain/fx_yujing_yuan", pos,
                                    m_oneDamageInfo.damageNode.funDamage.radius * 2, m_oneDamageInfo.damageNode.funDamage.angle);
                            }
                            else
                            {
                                m_SkillScope = WarningDecel.CreateSectorDecal("Effect/skill/remain/fx_yujing_shanxing", pos,
                                   m_oneDamageInfo.damageNode.funDamage.radius * 2, m_oneDamageInfo.damageNode.funDamage.angle);
                            }
                           

                            if (m_oneDamageInfo.damageNode.funDamage.angle < 360)
                            {
                                float ActorAngle = 0.0f;
                                Vector3 ActorAxis = Vector3.zero;
                                m_skillBase.m_actor.transform.rotation.ToAngleAxis(out ActorAngle, out ActorAxis);
                                if(m_SkillScope!=null)
                                {
                                    float angle = Mathf.Acos(Vector3.Dot(m_skillBase.m_actor.transform.forward.normalized, new Vector3(1.0f, 0f, 0f))) * Mathf.Rad2Deg;
                                    if (ActorAngle >= 90 && ActorAngle <= 270)
                                    {
                                        m_SkillScope.transform.RotateAround(pos, Vector3.up, angle);
                                    }
                                    else
                                    {
                                        m_SkillScope.transform.RotateAround(pos, Vector3.up, -angle);
                                    }
                                }

                                //LogMgr.UnityLog("angle="+angle.ToString());
                        

                               // m_SkillScope.transform.position += m_SkillScope.transform.up * 0.1f;
                            }
                        }
                        else if (DamageTypeID.DTID_RECT == m_oneDamageInfo.type)
                        {
                            m_SkillScope = WarningDecel.CreateRectangleDecal("Effect/skill/remain/fx_yujing_changfang", pos,
                                     m_oneDamageInfo.damageNode.rectDamage.width, m_oneDamageInfo.damageNode.rectDamage.length);
                            if(m_SkillScope!=null)
                            {
                                m_SkillScope.transform.rotation = m_skillBase.m_actor.transform.rotation;
                                m_SkillScope.transform.position += m_oneDamageInfo.damageNode.rectDamage.length / 2 * m_skillBase.m_actor.transform.forward;

                            }
                        }
                    }
                    //Invoke("HideSkillScope", m_oneDamageInfo.hitTime);
                }
        }
 
        //单体伤害   
        if(!m_bIsAoe)
        {
            ActorObj targetObj =  m_skillBase.m_actor.GetSelTarget();

            if (targetObj != null && m_skillBase.m_actor.IsSkillAim((sbyte)m_skillBase.m_skillDesc.Get<int>("faction_limit"), targetObj))
            {
                ActorObj actorBase = targetObj;

                //按伤害范围算出受伤害对象，具体有没有伤害，还要看对应的属性(免疫等)
                bool isSkillSuccess = m_skillMgr.IsSkillDamageRange(skillDesc.Get<int>("effect_1") , m_transform,
                    targetObj.transform, actorBase.GetColliderRadius());

                //伤害对象
                if (isSkillSuccess)
                {
                    isDamageSuccess = true;

                    //纠正被击表现
                    DamageParam damageParam = new DamageParam();
                    damageParam.skillID = m_skillBase.m_skillID;
                    //damageParam.attackObj = m_skillBase.m_castOwnObj;
                    damageParam.attackActor = m_skillBase.m_actor;

                    //damageParam.behitObj = attackObj.gameObject;
                    damageParam.behitActor = actorBase;      
                    damageParam.IsClient = true;

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
                }

            }
            


        }
        else
        {   
            //计算伤害  群体
            if(m_skillBase.m_actor == null)
             {
                  return;
             }

            ActorObj actor = null;

            for( int k = 0 ; k < m_skillBase.m_actor.m_TargetList.Count ;++k )
            {
                actor = CoreEntry.gActorMgr.GetActorByServerID(m_skillBase.m_actor.m_TargetList[k]);

                if (ArenaMgr.Instance.IsArenaFight)
                {
                    actor = CoreEntry.gActorMgr.GetPlayerActorByServerID(m_skillBase.m_actor.m_TargetList[k]);
                }

                if (actor == null)
                {
                    continue;
                }

                 //对IOS出现怪物不动 报错的异常  进行错误处理
                 if (GameLogicMgr.checkValid(actor.gameObject) == false)
                 {
                      continue;
                 }
          
                {
                    isDamageSuccess = true;

                    //纠正被击表现
                    DamageParam damageParam = new DamageParam();
                    damageParam.skillID = m_skillBase.m_skillID;
                   // damageParam.attackObj = m_skillBase.m_castOwnObj;
                    damageParam.attackActor = m_skillBase.m_actor;
                   // damageParam.behitObj = obj.gameObject;
                    damageParam.behitActor = actor;
                    damageParam.weight = m_oneDamageInfo.resetSkillWeight;
                    damageParam.isNotUseCurveMove = m_oneDamageInfo.isNotUseCurveMove;

                    damageParam.IsClient = true;
                     
                    CoreEntry.gSkillMgr.OnSkillDamage(damageParam);               

                        //是否有眩晕效果
                    if (m_oneDamageInfo.dizzyParamDesc != null)
                    {
                        DizzyParam param = new DizzyParam();
                        param.keepTime = m_oneDamageInfo.dizzyParamDesc.keepTime;

                            actor.OnEnterDizzy(param);
                    }

                    if (isDamageSuccess && actor.mActorType == ActorType.AT_BOSS && m_skillBase.m_actor.IsMainPlayer())
                    {
                        m_isHadLoadFrameStop = true;

                        //顿帧             
                        if (m_oneDamageInfo.frameStopDesc != null)
                        {
                            //GameObject frameStopObj = Instantiate(
                            //    CoreEntry.gResLoader.LoadResource(m_oneDamageInfo.frameStopDesc.prefabPath)) as GameObject;
                            GameObject frameStopObj = CoreEntry.gGameObjPoolMgr.InstantiateSkillCell(m_oneDamageInfo.frameStopDesc.prefabPath);

                            ISkillCell skillCell = frameStopObj.GetComponent<ISkillCell>();
                            if (skillCell)
                            {

                                skillCell.Init(m_oneDamageInfo.frameStopDesc, m_skillBase, actor);
                                m_skillBase.AddSkillCell(frameStopObj);

                            }
                            else
                            {
                                //没有skillcell说明，坏掉了马上清理掉
                                Destroy(frameStopObj);
                            }
                        }
                    }
                }          
            }

        }

       
        LuaTable skill_action = CoreEntry.gSkillMgr.GetSkillActon(m_skillBase.m_skillID);


        string remanEfx = null;
        if (skill_action != null)
                remanEfx = skill_action.Get<string>("remain");


        //地表残留
            if (remanEfx.Length > 0 && m_oneDamageInfo.m_bUseRemain)
        {
            //GameObject efxObj = (GameObject)Object.Instantiate(CoreEntry.gResLoader.LoadResource(remanEfx));//CoreEntry.gGameObjPoolMgr.InstantiateEffect(remanEfx);
            GameObject efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(remanEfx);


            if (efxObj == null)
            {
                //efxObj = (GameObject)Object.Instantiate(CoreEntry.gResLoader.LoadResource(remanEfx));//CoreEntry.gGameObjPoolMgr.InstantiateEffect(remanEfx);
                efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(remanEfx);
            }


            //yy 特效和模型按等比例缩放
            //if (m_skillBase.m_actor.actorCreatureDisplayDesc.sacle > 1.0f)
            //{
            //    ParticleScaler ScaleComponet = efxObj.GetComponent<ParticleScaler>();
            //    if (ScaleComponet == null)
            //        ScaleComponet = efxObj.AddComponent<ParticleScaler>();

            //    ScaleComponet.particleScale = m_skillBase.m_actor.actorCreatureDisplayDesc.sacle;

            //}


            float maxEfxTime = 0;

            if (efxObj)
            {
                NcCurveAnimation[] efxAnimations = efxObj.GetComponentsInChildren<NcCurveAnimation>();
                for (int i = 0; i < efxAnimations.Length; ++i)
                {
                    // efxAnimations[i].m_fDelayTime /= m_speed;
                    // efxAnimations[i].m_fDurationTime /= m_speed;

                    float efxTime = efxAnimations[i].m_fDelayTime + efxAnimations[i].m_fDurationTime;
                    if (efxTime > maxEfxTime)
                    {
                        maxEfxTime = efxTime;
                    }
                }
            }

            if (skill_action != null)
                {
                    if (skill_action.Get<float>("skillEfxLength") > 0)
                    {
                        maxEfxTime = skill_action.Get<float>("skillEfxLength");
                    }

                    //特效存在时间
                    if (maxEfxTime <= 0.001)
                    {
                        maxEfxTime = m_skillBase.m_actor.GetActionLength(skill_action.Get<string>("animation"));


                    }
                }


            EfxAttachActionPool efx = efxObj.GetComponent<EfxAttachActionPool>();
            if (efx == null)
                efx = efxObj.AddComponent<EfxAttachActionPool>();

            //yy test
 
            efx.InitRemain(m_skillBase.m_actor.transform, maxEfxTime, false);
          

            m_RemainEfx = efx;

        }



        //场景物件，主角破坏                
        if (m_skillBase.m_actor.mActorType == ActorType.AT_LOCAL_PLAYER)
        {
            GameObject[] brokedObjs = CoreEntry.gSceneMgr.brokedObjArray;
            for (int i = 0; i < brokedObjs.Length; ++i)
            {
                if (brokedObjs[i] == null)
                {
                    continue;
                }

                bool isSkillSuccess = m_skillMgr.IsSkillDamageRange(skillDesc.Get<int>("effect_1"), m_transform, brokedObjs[i].transform, 0.5f);

                //伤害对象
                    if (isSkillSuccess)
                {
                    Broked broked = brokedObjs[i].GetComponent<Broked>();
                     int weight =  3 ;//m_skillBase.m_skillDesc.weight;
                    if (m_oneDamageInfo.resetSkillWeight > 0)
                    {
                        weight = m_oneDamageInfo.resetSkillWeight;                          
                    }

                    broked.DoBroked(m_skillBase.m_actor.thisGameObject, weight);
                }                                                
            }                                        
        }
                
        //if (isDamageSuccess && !m_isHadLoadFrameStop)
        //{
        //    m_isHadLoadFrameStop = true;
                
        //    //顿帧
        //    if (m_oneDamageInfo.frameStopDesc != null)
        //    {
        //        GameObject frameStopObj = Instantiate(
        //            CoreEntry.gResLoader.LoadResource(m_oneDamageInfo.frameStopDesc.prefabPath)) as GameObject;

        //        ISkillCell skillCell = frameStopObj.GetComponent<ISkillCell>();

        //        skillCell.Init(m_oneDamageInfo.frameStopDesc,m_skillBase.m_actor);

        //        m_skillBase.AddSkillCell(frameStopObj);
        //    }

        //}

        //屏震    不是主玩家 ， 就不震屏幕
        if (m_skillBase.m_actor.IsMainPlayer() || m_skillBase.m_actor.mActorType == ActorType.AT_BOSS)
        {
            if (m_oneDamageInfo.cameraShakeDesc != null)
            {
                if (m_oneDamageInfo.cameraShakeDesc.activeWhenHit == false ||
                    (m_oneDamageInfo.cameraShakeDesc.activeWhenHit == true && isDamageSuccess == true)
                  )
                {
                    //GameObject cellObj = Instantiate(
                    //    CoreEntry.gResLoader.LoadResource(m_oneDamageInfo.cameraShakeDesc.prefabPath)) as GameObject;

                    GameObject cellObj = CoreEntry.gGameObjPoolMgr.InstantiateSkillCell(m_oneDamageInfo.cameraShakeDesc.prefabPath);
                    cellObj.transform.parent = transform;

                    // m_oneDamageInfo.cameraShakeDesc.playTime = m_oneDamageInfo.hitTime;

                        ISkillCell skillCell = cellObj.GetComponent<ISkillCell>();
                    skillCell.Init(m_oneDamageInfo.cameraShakeDesc, m_skillBase);

                    m_skillBase.AddSkillCell(cellObj);
                }
            }
        }
       
   

        //是否持续伤害                
        if (m_oneDamageInfo.isRepeatedly && m_repeateKeep > 0)
        {            
            Invoke("CalculateDamage", m_oneDamageInfo.damageDiff / m_skillBase.m_speed);
            --m_repeateKeep;
        }

       // sw.Stop();
        if (m_skillBase.m_actor.IsLocalPlayer())
        {
           // Debug.LogError("3....." + sw.ElapsedMilliseconds + "毫秒.....");
        }
    }

    bool IsAimActorType(ActorType mActorType)
    {
        //for (int i = 0; i < m_oneDamageInfo.aimActorTypeList.Count; ++i)
        //{
        //    if (mActorType == m_oneDamageInfo.aimActorTypeList[i])
        //    {
        //        return true;
        //    }            
        //}
        
        //return false;

        return true;
    }

    void HideDecal()
    {
        //if (m_WarningDecel)
        //{
        //    m_WarningDecel.HideDecal();
        //}

        if (m_skillBase == null || m_skillBase.m_actor == null)
        {
            return;
        }

        if (m_skillBase.m_actor.m_WarningefxObj)
            Destroy(m_skillBase.m_actor.m_WarningefxObj);


        if (m_skillBase.m_actor.mActorType == ActorType.AT_BOSS)
        {
            //如果是BOSS预警
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BOSS_SKILL_END, null);
        }

    }

    void HideSkillScope()
    {
        if (m_SkillScope)
        {
            m_SkillScope.HideDecal();
            m_SkillScope.Destory();
            m_SkillScope = null;
        }
    }

    void OnDestroy()
    {
        CancelInvoke("Start");
        HideSkillScope();
        HideDecal();
    }

    protected void OnDisable()
    {
        CancelInvoke("Start");
        HideSkillScope();
        HideDecal();
        CancelInvoke("CalculateDamage");
    }

    public override void ShowSkillScope()
    {
        //伤害计算
        Vector3 pos = m_skillBase.m_actor.transform.position;
        if (null == m_skillBase.m_actor.m_WarningefxObj)
        {
            if (m_skillBase.m_actor.m_WarningefxObj != null)
            {
                Destroy(m_skillBase.m_actor.m_WarningefxObj);
                m_skillBase.m_actor.m_WarningefxObj = null;
            }
            float fbig = 0.20f;   //贴花比实际伤害范围大百分之fbig
            if (DamageTypeID.DTID_FUN == m_oneDamageInfo.type || !m_bIsAoe)
            {
                if (m_oneDamageInfo.damageNode.funDamage.angle == 360 || !m_bIsAoe )
                    m_skillBase.m_actor.m_WarningefxObj = Instantiate(
                        CoreEntry.gResLoader.LoadResource("Effect/skill/remain/fx_yujing_yuanlan")) as GameObject;
                else
                    m_skillBase.m_actor.m_WarningefxObj = Instantiate(
                        CoreEntry.gResLoader.LoadResource("Effect/skill/remain/fx_yujing_shanxinglan")) as GameObject;
               
                if( m_skillBase.m_actor.m_WarningefxObj== null)return;
                
                ParticleScaler ScaleComponet = m_skillBase.m_actor.m_WarningefxObj.GetComponent<ParticleScaler>();
                if (ScaleComponet==null)
                    ScaleComponet = m_skillBase.m_actor.m_WarningefxObj.AddComponent<ParticleScaler>();

                if (m_oneDamageInfo.damageNode.funDamage.angle == 360 || !m_bIsAoe )
                {
                    ScaleComponet.particleScale = m_oneDamageInfo.damageNode.funDamage.radius * 2 * (1 + fbig);
                    m_skillBase.m_actor.m_WarningefxObj.transform.position = new Vector3(pos.x, pos.y + 0.35f, pos.z);
                    m_skillBase.m_actor.m_WarningefxObj.transform.forward = m_skillBase.m_actor.transform.forward;
                }
                else
                {
                    ScaleComponet.particleScale = m_oneDamageInfo.damageNode.funDamage.radius * (0.83f + fbig);
                    m_skillBase.m_actor.m_WarningefxObj.transform.position = new Vector3(pos.x, pos.y - (0.35f * (ScaleComponet.particleScale/6f)), pos.z);
                    m_skillBase.m_actor.m_WarningefxObj.transform.forward = m_skillBase.m_actor.transform.forward;
                }
            }
            else if (DamageTypeID.DTID_RECT == m_oneDamageInfo.type)
            {
                m_skillBase.m_actor.m_WarningefxObj = Instantiate(
                    CoreEntry.gResLoader.LoadResource("Effect/skill/remain/fx_yujing_changfanglan")) as GameObject;
                if(m_skillBase.m_actor.m_WarningefxObj == null)return;
                m_skillBase.m_actor.m_WarningefxObj.transform.localScale = new Vector3(m_oneDamageInfo.damageNode.rectDamage.width * 2f, 0f, m_oneDamageInfo.damageNode.rectDamage.length * 1f);
                m_skillBase.m_actor.m_WarningefxObj.transform.forward = m_skillBase.m_actor.transform.forward;
                m_skillBase.m_actor.m_WarningefxObj.transform.position = new Vector3(pos.x, pos.y + 0.35f, pos.z);

                if (m_oneDamageInfo.damageNode.rectDamage.offDistance != 0)
                {
                    m_skillBase.m_actor.m_WarningefxObj.transform.localPosition += m_skillBase.m_actor.m_WarningefxObj.transform.forward * (m_oneDamageInfo.damageNode.rectDamage.offDistance);

                }

                //if (m_oneDamageInfo.damageNode.rectDamage.offDistanceX != 0)
                //{
                //    m_skillBase.m_actor.m_WarningefxObj.transform.localPosition += m_skillBase.m_actor.m_WarningefxObj.transform.right * (m_oneDamageInfo.damageNode.rectDamage.offDistanceX);

                //}
                //m_skillBase.m_actor.m_WarningefxObj.transform.localPosition -= new Vector3(0, 0,  m_oneDamageInfo.damageNode.rectDamage.length * 0.5f);
                //m_skillBase.m_actor.m_WarningefxObj.transform.position += m_oneDamageInfo.damageNode.rectDamage.length / 2 * m_skillBase.m_actor.transform.forward;
            }
        }
    }

    public override void Preload(ISkillCellData cellData, SkillBase skillBase)
    {
        //m_dataIndex = param.damageCellIndex;                            
        m_oneDamageInfo = (OneDamageInfo)cellData;
        m_skillBase = skillBase;

        GameObject frameStopObj = CoreEntry.gGameObjPoolMgr.InstantiateSkillCell("Skill/Cell/frameStop");
        if (frameStopObj != null)
        {
            frameStopObj.transform.parent = transform;

            ISkillCell skillCell = frameStopObj.GetComponent<ISkillCell>();

            skillCell.Preload(m_oneDamageInfo.frameStopDesc, m_skillBase);

            m_skillBase.AddSkillCell(frameStopObj);
        }

        if (m_oneDamageInfo.cameraShakeDesc != null)
        {
            GameObject cellObj = CoreEntry.gGameObjPoolMgr.InstantiateSkillCell(m_oneDamageInfo.cameraShakeDesc.prefabPath);
            cellObj.transform.parent = transform;

            ISkillCell skillCell = cellObj.GetComponent<ISkillCell>();
            skillCell.Preload(m_oneDamageInfo.cameraShakeDesc, m_skillBase);

            m_skillBase.AddSkillCell(cellObj);
        }

    }

}

};  //end SG

