using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{

//子弹类
[System.Serializable]
[Hotfix]
public class BulletParam
{
    public float flySpeed;
    public float flyTime;
    
    //目标
    public Transform aimTransform = null;              

    //伤害计算距离
    public float distance = 0;

    //伤害目标类型
    public List<ActorType> damageActorTypeList;                              

    //技能释放对象
    public GameObject castObj;  
    
    //技能ID
    public int skillID;

    public bool bThroughFlag;

    public Vector3 targetpos;

    public float projectile;    // 抛物线角度 -15 ~ 15  等于0表示非抛物线弹道



    public SkillBase m_skillBase;



    public int   TanTanLeCount;           //弹弹乐次数

    public float TanTanLeDis;             //弹弹乐距离

    public float dizzyTime;               //弹弹乐眩晕时间


    public bool bTakeAwayTarget;   //带走目标

    public bool disappearWhenTouchWall; //碰到墙是否消失

    public bool bCanNotMoveWhenTakenAway; //带走的时候能不能移动

    public bool bTraceTarget; //跟踪目标

    public bool bAttachToSkill;

    public bool bAttackMoreThanOnce;

    public bool bEndTheSkillWhenDisappear;
};

[Hotfix]
public class Bullet : MonoBehaviour {
    public GameObject m_efxPrefab = null;

    protected Transform m_transform = null;    

    public BulletParam m_param = new BulletParam();

    protected EventMgr m_eventMgr;
    protected GameLogicMgr m_gameManager;
    
    public Transform m_aimTransform = null;

    protected float projectileAngle = 45; // 抛物线角度 -15 ~ 15
    protected float distanceToTarget;

    protected GameObject objEfx = null;

    protected bool bAutoEnd = false;

    protected BaseTool m_baseTool;


    protected EfxAttachActionPool m_RemainEfx = null;

    Dictionary<int, GameObject> m_TakeAwayGameObjectMap = new Dictionary<int, GameObject>();

    bool IsRunning = false;

    void Awake()
    {
        m_transform = this.transform;
        m_eventMgr = CoreEntry.gEventMgr;
        m_gameManager = CoreEntry.gGameMgr;
    }

	// Use this for initialization
	void Start () 
	{
        if (bAutoEnd == true)
        {
            SetActiveMeshRender(true);
            bAutoEnd = false;
        }

        //加载特效
        if (m_efxPrefab != null)
        {
            objEfx = Instantiate(m_efxPrefab) as GameObject;
            if (objEfx != null)
            {
                objEfx.transform.parent = this.transform;
                objEfx.transform.localPosition = Vector3.zero;
                objEfx.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }  
        }
        else
            {
                LuaTable skill_action = CoreEntry.gSkillMgr.GetSkillActon(m_param.skillID);
                if(skill_action!=null)
                {
                    string skilleft = skill_action.Get<string>("skilleffect");
                    if (skilleft.Length > 0)
                    {
                        objEfx = GameObject.Instantiate(CoreEntry.gResLoader.Load(skilleft)) as GameObject;

                        if (objEfx != null)
                        {
                            objEfx.transform.parent = this.transform;
                            objEfx.transform.localPosition = Vector3.zero;
                            objEfx.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                        }
                    }
                }
              
               
            }

            //清空带走队列
            m_TakeAwayGameObjectMap.Clear();

        IsRunning = false;

        Invoke("StartRunning",0.01f);

        //结束时间        
        Invoke("AutoEnd", m_param.flyTime);
    }

    void StartRunning()
    {
        IsRunning = true;
    }

	// Update is called once per frame
    protected virtual void Update()
    {
        if (bAutoEnd)
            return;

        Vector3 oldPos = this.transform.position;
        //是否有目标
        if (m_aimTransform != null)
        {
            if (m_param.projectile!=0) // 抛物线弹道
            {
                Vector3 targetPos = m_aimTransform.transform.position;
                this.transform.LookAt(targetPos);
                float currentDist = Vector3.Distance(this.transform.position, targetPos);
                float angle = Mathf.Min(1, currentDist / distanceToTarget) * m_param.projectile;
                this.transform.rotation = this.transform.rotation * Quaternion.Euler(Mathf.Clamp(-angle, -projectileAngle, projectileAngle), 0, 0);
                this.transform.Translate(Vector3.forward * Mathf.Min(Time.deltaTime * 10 * m_param.flySpeed, currentDist));
            }
            else
            {
                //面向目标
                m_transform.LookAt(new Vector3(m_aimTransform.position.x, m_transform.position.y, m_aimTransform.position.z));
                m_transform.Translate(new Vector3(0, 0, Time.deltaTime * 10 * m_param.flySpeed));
            }
        }
        else
        if(m_param.projectile!=0&&m_param.targetpos!=Vector3.zero)
        {
            Vector3 targetPos = m_param.targetpos;
            this.transform.LookAt(targetPos);
            Vector3 pos1 = new Vector3(this.transform.position.x, 0,this.transform.position.z);
            Vector3 pos2 = new Vector3(targetPos.x, 0,targetPos.z);
            float currentDist = Vector3.Distance(pos1, pos2);
            float angle = Mathf.Min(1, currentDist / distanceToTarget) * m_param.projectile;
            this.transform.rotation = this.transform.rotation * Quaternion.Euler(Mathf.Clamp(-angle, -projectileAngle, projectileAngle), 0, 0);
            this.transform.Translate(Vector3.forward * (Time.deltaTime * 10 * m_param.flySpeed));
            if (0.3f > currentDist || Time.deltaTime * 10 * m_param.flySpeed > currentDist)
                m_param.projectile = 0; // 到达指定地点，沿着直线飞行
        }
        else
        {
            m_transform.Translate(new Vector3(0, 0, Time.deltaTime * 10 * m_param.flySpeed));
        }

        TakeAwayTargets(m_transform.position - oldPos);
	}

    
    public virtual void Init(BulletParam param)
    {
        m_param = param;   
        m_aimTransform = m_param.aimTransform;

        if (m_param.bTraceTarget && m_param.m_skillBase.m_hitActor != null)
        {
            //ActorObj actorBase = m_param.m_skillBase.GetTargetFromSelector(null);
            //if (actorBase != null)
            //{
                m_aimTransform = m_param.m_skillBase.m_hitActor.transform;

                ////转向
                //m_param.m_skillBase.m_actor.transform.LookAt(m_aimTransform);
            //}
        }

        //是否绑在技能上
        if (m_param.bAttachToSkill)
        {
          
            m_param.m_skillBase.AttachABullet(this);
            BaseTool.ResetTransform(transform);
        }

        if (m_aimTransform != null)
            distanceToTarget = Vector3.Distance(this.transform.position, m_aimTransform.position);
        else
        if(m_param.targetpos!=Vector3.zero)
        {
            distanceToTarget = Vector3.Distance(this.transform.position, m_param.targetpos);
        }
    }

    protected void SetActiveMeshRender(bool a)
    {
        // 拖尾特效，关掉模型、带子渲染，粒子延后销毁
        MeshRenderer[] meshList = gameObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshList.Length; i++)
        {
            meshList[i].enabled = a;
        }

        SkinnedMeshRenderer[] skinMeshList = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < skinMeshList.Length; i++)
        {
            skinMeshList[i].enabled = a;
        }

        TrailRenderer[] tailList = gameObject.GetComponentsInChildren<TrailRenderer>();
        for (int i = 0; i < tailList.Length; i++)
        {
            tailList[i].enabled = a;
        }

        ParticleSystem[] particleList = gameObject.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < particleList.Length; i++)
        {
                ParticleSystem.EmissionModule em = particleList[i].emission;
                em.enabled = a;
        }
    }


    public void AutoEnd()
    {
        CancelInvoke("AutoEnd");
        bAutoEnd = true;

        DisableTakeWayEffect();
        ProcessAutoEnd();

        SetActiveMeshRender(false);
        Destroy(this.gameObject);
    }



    protected void AutoEnd(Transform tTransform)
    {
        CancelInvoke("AutoEnd");
        bAutoEnd = true;

        //是否需要残留

        //地表残留
        //LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(m_param.m_skillBase.m_skillID);
        LuaTable skill_action = CoreEntry.gSkillMgr.GetSkillActon(m_param.m_skillBase.m_skillID);
        if(skill_action!= null)
          {
                string remanEfx = skill_action.Get<string>("remain");

                if (remanEfx.Length > 0)
                {
                    //GameObject efxObj = (GameObject)Object.Instantiate(CoreEntry.gResLoader.LoadResource(remanEfx));//CoreEntry.gGameObjPoolMgr.InstantiateEffect(remanEfx);
                    GameObject efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(remanEfx);

                    if (efxObj == null)
                    {
                        //efxObj = (GameObject)Object.Instantiate(CoreEntry.gResLoader.LoadResource(remanEfx));//CoreEntry.gGameObjPoolMgr.InstantiateEffect(remanEfx);
                        efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(remanEfx);
                    }

                    //yy 特效和模型按等比例缩放
                    //if (m_param.m_skillBase.m_actor.actorCreatureDisplayDesc.sacle > 1.0f)
                    //{
                    //    ParticleScaler ScaleComponet = efxObj.GetComponent<ParticleScaler>();
                    //    if (ScaleComponet == null)
                    //        ScaleComponet = efxObj.AddComponent<ParticleScaler>();

                    //    ScaleComponet.particleScale = m_param.m_skillBase.m_actor.actorCreatureDisplayDesc.sacle;

                    //}


                    float maxEfxTime = 0;
                    NcCurveAnimation[] efxAnimations = efxObj.GetComponentsInChildren<NcCurveAnimation>();
                    for (int i = 0; i < efxAnimations.Length; ++i)
                    {
                        float efxTime = efxAnimations[i].m_fDelayTime + efxAnimations[i].m_fDurationTime;
                        if (efxTime > maxEfxTime)
                        {
                            maxEfxTime = efxTime;
                        }
                    }

                    if (skill_action.Get<float>("skillEfxLength") > 0)
                    {
                        maxEfxTime = skill_action.Get<float>("skillEfxLength");
                    }

                    //特效存在时间
                    if (maxEfxTime <= 0.001)
                    {
                        maxEfxTime = m_param.m_skillBase.m_actor.GetActionLength(skill_action.Get<string>("animation"));

                    }

                    EfxAttachActionPool efx = efxObj.GetComponent<EfxAttachActionPool>();
                    if (efx == null)
                        efx = efxObj.AddComponent<EfxAttachActionPool>();

                    //yy test

                    efx.InitRemain(transform, maxEfxTime, false);

                    Invoke("DestoryEfx", maxEfxTime);

                    m_RemainEfx = efx;

                }
            }
 

        DisableTakeWayEffect();
        ProcessAutoEnd();

        SetActiveMeshRender(false);
        Destroy(this.gameObject,1.5f);
    }

    //触发器
    public virtual void OnTriggerEnter(Collider other)
    {
        if (bAutoEnd)
            return;

        int groundLayer = LayerMask.NameToLayer("ground");
                                     
        //碰到地面就消失
        if (IsRunning && other.gameObject.layer == groundLayer)
        {
            AutoEnd(other.gameObject.transform); 
            return;
        }

        if ( m_param.disappearWhenTouchWall)
        {
            int wallLayer = LayerMask.NameToLayer("wall");

            //被带走的，碰到墙壁就消失
            if (other.gameObject.layer == wallLayer)
            {

                AutoEnd(other.gameObject.transform);
                return;
            }
        }

        bool bIsMonster = false;
        
        ActorObj actorBase = other.transform.root.gameObject.GetComponent<ActorObj>();
        if (actorBase == null)
        {
            actorBase = other.transform.gameObject.GetComponent<ActorObj>();
            bIsMonster = true;
        }

        if (actorBase == null)
        {
            //判断是否是破碎物体
            if ((other.gameObject.CompareTag("broked") || other.gameObject.layer == 13) && m_param.m_skillBase.m_actor.mActorType == ActorType.AT_LOCAL_PLAYER)
            {
                GameObject[] brokedObjs = CoreEntry.gSceneMgr.brokedObjArray;
                for (int i = 0; i < brokedObjs.Length; ++i)
                {
                    if (brokedObjs[i] == null)
                    {
                        continue;
                    }
   
                    //伤害对象
                
                        Broked broked = brokedObjs[i].GetComponent<Broked>();
                        int weight = 0;  //m_param.m_skillBase.m_skillDesc.weight;
                        broked.DoBroked(m_param.m_skillBase.m_actor.thisGameObject , weight);
                    }
                                  

                if (!m_param.bThroughFlag)
                    AutoEnd( );

            }

            return;
        }

        //临时判断
        ActorObj castBase = m_param.castObj.GetComponent<ActorObj>();
        //if (actorBase.mActorType == castBase.mActorType)
        //{
        //    return;
        //}


        if (!castBase.IsAimActorType(actorBase))//IsAimActorType(actorBase.mActorType))
        {
            return;
        }


        if (m_param.bAttackMoreThanOnce == false)
        {
            //增加个判断，判断是不是受过此子弹技能的伤害
            for (int i = 0; i < m_param.m_skillBase.m_AttackList.Count; i++)
            {
                if (actorBase.entityid == m_param.m_skillBase.m_AttackList[i].entityid)
                {
                    return;
                }
            }
        }

        // m_param.m_skillBase.m_AttackList.Find(actorBase);
       
        //增加到攻击列表
        m_param.m_skillBase.m_AttackList.Add(actorBase);

          
        //纠正被击表现
        DamageParam damageParam = new DamageParam();
        damageParam.skillID = m_param.skillID;
        damageParam.attackActor = castBase;// m_param.castObj;

        if (bIsMonster)
        {
            damageParam.behitActor = actorBase;
        }
        else
        {
            damageParam.behitActor = actorBase;
        }

        CoreEntry.gSkillMgr.OnSkillDamage(damageParam);

        m_baseTool = CoreEntry.gBaseTool;

        //是否被带走
        if (m_param.bTakeAwayTarget)
        {
            bool shouldTakeAway = true;
            if (actorBase != null )
            {
                //boss在气绝状态下才能被带走
                if (actorBase.CheckIfBossNotQiJue())
                {
                    shouldTakeAway = false;
                }

                //无敌状态不被带走
                if (actorBase.IsRecoverHealth)
                {
                    shouldTakeAway = false;
                }
            }

            if (shouldTakeAway)
            {
                SetTakeAwayTarget(damageParam, bIsMonster);
            }
        }

        if (!m_param.bThroughFlag)
            AutoEnd( );
    }

    void TakeAwayTargets(Vector3 translation)
    {
        Vector3 aimPos, wallPos,pos0,dir;
        List<int> NoUseList = null;
        if (m_TakeAwayGameObjectMap.Count > 0)
        {
            NoUseList = new List<int>();
        }

        if (m_param.bCanNotMoveWhenTakenAway)
        {
            bool canMoveTarget = false;
            Vector3 tmpPos = this.transform.position;
            if (m_TakeAwayGameObjectMap.Count > 0)
            {
                //只有在地面上的位置，才是移动目标的有效位置
                if (m_baseTool.IsAboveTheGround(tmpPos))
                {
                    
                    canMoveTarget = true;
                }
            }

            if (canMoveTarget)
            {
                foreach (int i in m_TakeAwayGameObjectMap.Keys)
                {
                    ActorObj actorBase = m_TakeAwayGameObjectMap[i].GetComponent<ActorObj>();
                    if (actorBase != null)
                    {
                        float radius = actorBase.GetColliderRadius();
                        if (BaseTool.instance.CanMoveToPos(actorBase.transform.position, tmpPos, radius))
                        {
                            //m_TakeAwayGameObjectMap[i].transform.position = new Vector3(tmpPos.x, m_TakeAwayGameObjectMap[i].transform.position.y, tmpPos.z);
                            BaseTool.SetPosition(m_TakeAwayGameObjectMap[i].transform,tmpPos);
                        }
                    }
                  
                }
            }
        }
        else
        {
            foreach (int i in m_TakeAwayGameObjectMap.Keys)
            {
                if (m_TakeAwayGameObjectMap[i] != null)
                {
                    aimPos = m_TakeAwayGameObjectMap[i].transform.position + translation;
                    wallPos = m_baseTool.GetWallHitPoint(m_TakeAwayGameObjectMap[i].transform.position, aimPos);
                    if (!wallPos.Equals(aimPos))
                    {
                        // Debug.DrawLine(wallPos, aimPos,Color.cyan);

                        dir = translation;
                        dir.Normalize();
                        //存在空气墙
                        //unity的raycast是向下忽略的，所以不能再下边界做判断，给定的时候，往回取一点值                            
                        Vector3 wallGroundPos = m_baseTool.GetGroundPoint(wallPos - dir * 0.1f);

                        float distance = Vector3.Distance(m_TakeAwayGameObjectMap[i].transform.position, wallGroundPos);

                        distance -= 0.4f;

                        pos0 = m_TakeAwayGameObjectMap[i].transform.position +
                     translation.normalized * distance;

                        aimPos = m_baseTool.GetGroundPoint(pos0);

                        m_TakeAwayGameObjectMap[i].transform.Translate(aimPos - m_TakeAwayGameObjectMap[i].transform.position, Space.World);

                        NoUseList.Add(i);
                    }
                    else
                    {
                        m_TakeAwayGameObjectMap[i].transform.Translate(translation, Space.World);
                    }
                }
            }
        }

        if (NoUseList != null)
        {
            for (int i = 0; i < NoUseList.Count; ++i)
            {
                //清除物件前，恢复动作
                ActorObj actorBase = m_TakeAwayGameObjectMap[NoUseList[i]].GetComponent<ActorObj>();
                if (actorBase != null)
                {
                    actorBase.CanCurveMove();
                }
                m_TakeAwayGameObjectMap.Remove(NoUseList[i]);
            }
        }
    }

    void SetTakeAwayTarget(DamageParam damageParam, bool bIsMonster)
    {
        if (bIsMonster)
        {
            if (damageParam.behitActor != null )
            {
                ActorObj actor = damageParam.behitActor;
                if (actor != null && actor.IsDeath() == false && actor.IsGod == false && actor.CanKnock == 0)
                {
                    //只能被带走一次
                    if (actor.BeTakenAway == false)
                    {
                        if (!m_TakeAwayGameObjectMap.ContainsKey(actor.entityid))
                        {
                            m_TakeAwayGameObjectMap.Add(actor.entityid, damageParam.behitActor.gameObject);
                            actor.CanotCurveMove();

                            actor.BeTakenAway = true;
                            actor.BeTakenAwayOwner = gameObject;

                            //带走的时候是否不能动
                            if (m_param.bCanNotMoveWhenTakenAway)
                            {
                                    actor.IgnoredGravityMotion = true;

                                //需要打断技能
                                StateParameter stateParm = new StateParameter();
                                stateParm.state = ACTOR_STATE.AS_BEHIT;
                                stateParm.AttackActor = actor;

                                    actor.m_AttackState.BreakSkill(stateParm);

                                    //能否切换到被击状态                    
                                    actor.RequestChangeState(stateParm);

                                    actor.StopAll();
                                    actor.PlayAction("hit011", false);
                            }
                        }
                    }
                }
            }
        }
    }

    void DisableTakeWayEffect()
    {
        ActorObj actorBase = null;
        foreach (int i in m_TakeAwayGameObjectMap.Keys)
        {
            if (m_TakeAwayGameObjectMap[i] != null)
            {
                actorBase = m_TakeAwayGameObjectMap[i].GetComponent<ActorObj>();

                if (actorBase != null)
                {
                    actorBase.CanCurveMove();

                    if (m_param.bCanNotMoveWhenTakenAway)
                    {
                        actorBase.BeTakenAway = false;
                        actorBase.IgnoredGravityMotion = false;
                       // Vector3 groundPos = BaseTool.instance.GetGroundPoint(actorBase.m_transform.position);
                       // actorBase.m_transform.position = groundPos;
                        actorBase.FallDown();

                        
                    }
                }
            }
        }

        m_TakeAwayGameObjectMap.Clear();
    }

    bool IsAimActorType(ActorType aimActorType)
    {
        if(m_param.damageActorTypeList == null)
        {
            return false;
        }


        for (int i = 0; i < m_param.damageActorTypeList.Count; ++i)
        {
            if (aimActorType == m_param.damageActorTypeList[i])
            {
                return true;
            }            
        }

        return false;
    }

    void DestoryEfx()
    {
        CancelInvoke("DestoryEfx");

        if (m_RemainEfx != null)
        {
            m_RemainEfx.DetachObject();
        }
    }

    void ProcessAutoEnd()
    {
        if( m_param != null )
        {
            if (m_param.bEndTheSkillWhenDisappear)
            {
                if (m_param.m_skillBase.IsOver == false)
                {
                    m_param.m_skillBase.SkillEnd();
                }
            }
        }
    }

}

};  //end SG

