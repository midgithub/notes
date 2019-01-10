/**
* @file     : ActorObj.cs
* @brief    : 
* @details  : 角色对象基类
* @author   : 
* @date     : 2014-11-28 9:31
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using XLua;

#if !UNITY_5_3_8
using UnityEngine.AI;
#endif


namespace SG
{
    [LuaCallCSharp]
    public enum EPKMode
    {
        M_PK_Peace = 0,
        M_PK_Team,
        M_PK_Guild,
        M_PK_Server,
        M_PK_Camp,
        M_PK_Evil,
        M_PK_All,
        M_PK_Custom,
        M_PK_MAX,
    };

    //object的基类组件
    [LuaCallCSharp]
[Hotfix]
    public class ActorObj : Entity
    {
        //动画组件
        private AnimationMgr m_AnimationMgr;


        //public bool m_bIsInAttack = false;

        //移动组件
        public IMoveAgent m_move = null;

        public IMove m_AutoMove = null;


        public bool m_bAutoMove = false;

        public bool m_bRunToAttack = false;

        // 头像顺序编号
        protected int m_uiIndex = -1;
        public int UIIndex
        {
            get { return m_uiIndex; }
            set { m_uiIndex = value; }
        }


        private LuaTable m_curSkillDesc = null;

        //
        //组件
        //private Animation m_ani = null;
        private AudioSource m_audioSource = null;
        public UnityEngine.AudioSource AudioSource1
        {
            get { return m_audioSource; }
        }
        private AudioSource m_audioSourceBody = null;
        public UnityEngine.AudioSource AudioSourceBody
        {
            get { return m_audioSourceBody; }
        }


        //管理器
        public GameDBMgr m_gameDBMgr = null;
        protected EventMgr m_eventMgr = null;
        protected GameLogicMgr m_gameMgr = null;
        protected BaseTool m_baseTool = null;
        
        //被击特效挂点
        protected Transform m_behitEfxTransform = null;
        protected Transform m_behitEfxPosition = null;
        
        /// <summary>
        /// 胸口挂点
        /// </summary>
        private Transform chestSocket = null;

        public Transform ChestSocket
        {
            get { return chestSocket; }
        }

        private Transform centerSocket = null;
        public Transform CenterSocket
        {
            get { return centerSocket; }
        }


        //技能
        private int m_curCastSkillID = 0;
        public int curCastSkillID
        {
            get { return m_curCastSkillID; }
            set { m_curCastSkillID = value; }
        }


        private int m_curBeHitSkillID = 0;
        public int curBeHitSkillID
        {
            get { return m_curBeHitSkillID; }
        }
        
        //Buff
        public List<Buff> buffList = new List<Buff>();


        //召唤技能 召唤出来的怪物
        public List<SummonData> m_skillSummonList = null;


        //攻击者
        private ActorObj m_hitActorObject;

        ////挂机，攻击者
        //ActorObj hitActorInAutoMode;
        //public ActorObj HitActorInAutoMode
        //{
        //    get
        //    {
        //        return hitActorInAutoMode;
        //    }
        //}

        //当前动画
        private string m_curActionName = "";

        //复活保护
        private bool m_isRecoverHealth = false;

        /// <summary>
        /// 角色健康表现。
        /// </summary>
        protected ActorHealth mHealth;

        /// <summary>
        /// 获取角色健康表现。
        /// </summary>
        public ActorHealth Health
        {
            get { return mHealth; }
        }

        /// <summary>
        /// 是否是无敌状态
        /// </summary>
        public bool IsRecoverHealth
        {
            get { return m_isRecoverHealth; }
            //set { m_isRecoverHealth = value; }
        }
        

        int normaAttackNum = -1;

        public int NormaAttackNum
        {
            get { return normaAttackNum; }
            //set { normaAttackNum = value; }
        }

        //2S内可以表现次受击
        public bool bUserBehit = true;

        public bool m_bFlee = false;

        public bool m_bMoveEnd = false;

        public bool m_bUseBehitColor = false;

        public bool m_bSummonMonster = false;
        public bool m_bNoDieAction = false;

        /// <summary>
        /// 技能前防止被打断的措施
        /// </summary>
        public bool IsProtectedBeforeSkillWhenBeHit = false;

        //int skillIDWillToDo = -1;
        protected float skillRangeWillToDo = -1f;

        //public int SkillIDWillToDo
        //{
        //    get { return skillIDWillToDo; }
        //    set
        //    {

        //        skillIDWillToDo = value;
        //    }
        //}

        public float m_actionStartPlayTime = 0;

        //技能预警特效
        public GameObject m_WarningefxObj = null;


        //技能释放CD时间
        private Dictionary<int, float> m_castSkillCDMap = new Dictionary<int, float>();

        public AnimationCurveBase m_animationCurveBase = null;

        public bool m_bIsTower = false;


        public int castSkillID
        {
            get { return m_curCastSkillID; }
        }

        public int m_RuncastSKillID = 0;


        public int behitSkillID
        {
            get { return m_curBeHitSkillID; }
        }

        public ActorObj hitActorObject
        {
            get { return m_hitActorObject; }
            set { m_hitActorObject = value; }
        }

        /// <summary>
        /// 模型配置。
        /// </summary>
        private LuaTable m_ModelConfig;

        /// <summary>
        /// 获取模型配置。
        /// </summary>
        public LuaTable ModelConfig
        {
            get { return m_ModelConfig; }
        }

        //蒙皮渲染器
        public SkinnedMeshRenderer[] m_skinnedMeshRenderer;

        //oldshader
        //private Shader[,] m_oldShaderList;


        //悬空阶段
        public GravityMotionBase m_gravityMotionBase = null;

        //使用美术位移标记
        private int m_useCurveMoveUUID = 0;


        //附加动画挂点
        private Dictionary<string, Transform> m_mapAttachHangPoint = new Dictionary<string, Transform>();
        ////附加的挂点object    
        //private List<GameObject> m_attachObjectList = new List<GameObject>();

    

        struct ShareCDData
        {
            public int nSkillID;
            public float fSkillTime;
            public int cdTime; //cd时长
        };


        private Dictionary<int, ShareCDData> m_SkillShareCDMap = new Dictionary<int, ShareCDData>();

        private Collider[] m_allCollider = null;


        //public ActorHealth HealthFrame = null;

        public bool m_bIsInQiJue = false;
        public bool IsInQiJue()
        {
            return m_bIsInQiJue;
        }

        public void SetQiJue(bool bValue)
        {
            m_bIsInQiJue = bValue;
        }


        /// <summary>
        /// 技能CD缩放
        /// </summary>
        public float SkillCDScale = 1f;


        // game paused
        public bool m_bPaused = false;
        public int GetBehaviorDeltaFrames()
        {
            if (m_bPaused == false)
                return 1;
            else
                return 0;

        }

        public ActorObj m_SelectTargetObject = null;

        //public ActorObj m_PlayerSelect = null;          //手动选取的目标
        //public bool m_bPlayerSelect = false;      //是否是player选取


        //状态机
        protected RunState m_RunState = null;
        protected StandState m_StandState = null;
        protected DeathState m_DeathState = null;

        public AttackState m_AttackState = null;
        public BehitState m_BehitState = null;

        protected EnterState m_EnterState = null;
        protected DizzyState m_DizzyState = null;
        protected LimitState m_LimitState = null;
        protected SheepState m_SheepState = null;
        protected RunToAttackState m_RunToAttackState = null;

        protected FearState m_FearState = null;
        protected AimingState m_AimingState = null;
        protected StoneState m_StoneState = null;

        protected CollectState m_CollectState = null;

        //剧情相关状态机
        protected StoryStandState m_StoryStandState = null;
        protected StoryPlayAnimState m_StoryPlayAnimState = null;
        protected StoryPlayAnimLoopState m_StoryPlayAnimLoopState = null;
        protected FitState m_fitState= null;

        protected IActorState m_IActorState = null;


        public IActorState mActorState
        {
            get { return m_IActorState; }
            //set { m_IActorState = value; }
        }

        private HitFlyMotionBase mHitFlyCurveBase = null;

        public int GetCurStateToNumber()
        {
            return (int)curActorState;
        }

        //当前状态
        public ACTOR_STATE GetCurState()
        {
            return curActorState;
        }


        public ACTOR_STATE curActorState
        {
            get
            {
                if (m_IActorState == null)
                {
                    return ACTOR_STATE.AS_NONE;
                }

                return m_IActorState.m_state;
            }
        }
        
        //被击参数(攻击者，技能ID，修正表现效果)
        private BehitParam m_behitParam = null;

        //眩晕参数    
        private DizzyParam m_dizzyParam = null;

        //限制操作    
        private LimitParam m_limitParam = null;
        private SheepParam m_SheepParam = null;

        private FearParam m_FearParam = null;

        private List<LimitParam> actorLimitList = new List<LimitParam>();
        //缴械状态
        private bool m_bDisarmState = false;


        public bool m_bStealthState = false;

        public bool m_bCharmState = false;


        //是否非控制状态
        public bool IsNonControl
        {
            get
            {
                if (mActorState == null)
                {
                    return true;
                }

                return mActorState.IsNonControl;
            }
        }

        public bool isHaveRigidbody
        {
            get { return HaveRigidbody; }
        }

        public BehitParam damageBebitParam
        {
            get { return m_behitParam; }
        }

        public DizzyParam dizzyParam
        {
            get { return m_dizzyParam; }
        }

        public LimitParam limitParam
        {
            get { return m_limitParam; }
        }

        public SheepParam sheepParam
        {
            get { return m_SheepParam; }
        }

        public FearParam fearParam
        {
            get { return m_FearParam; }
        }



        //属性    
        private long m_curHP = 0;                    //当前的hp  
        //private int m_secondHP = 0;              //第二hp 

        public long curHp
        {
            set { mBaseAttr.CurHP = value; }
            get { return mBaseAttr.CurHP; }
        }

        public long maxHp
        {
            get { return mBaseAttr.MaxHP; }
            set { mBaseAttr.MaxHP = value; }
        }

        public EventMgr gEventMgr
        {
            get { return m_eventMgr; }
        }

        public BehitState behitState
        {
            get { return m_BehitState; }
        }

        public bool IsMainPlayer()
        {
            return mActorType == ActorType.AT_LOCAL_PLAYER;
        }

        //AI
        protected behaviac.Agent m_actorAI = null;


        //#endif    

        public ActorObj mActorBase
        {
            get { return this; }
        }

        /// <summary>
        /// 普攻动作时间缩放参数
        /// </summary>
        public float NormalAttackSpeedScale = 1.0f;

        /// <summary>
        /// 普攻动作粒子效果延迟时间
        /// </summary>
        public float NoramalAttackEffectDelayTime = 0f;

        BaseAttr m_BaseAttr;
        public SG.BaseAttr mBaseAttr
        {
            get { return m_BaseAttr; }
            set { m_BaseAttr = value; }
        }

        /// <summary>
        /// 角色阵营
        /// </summary>
        private int mFaction = 0;
        public int Faction
        {
            get { return mFaction; }
            set { mFaction = value; }
        }

        public virtual void UpdatePetAttr(MsgData_sSceneObjHoneYanLevel attrs)
        { 
        }

        public virtual void UpdateAttr(List<MsgData_sClientAttr> attrs)
        {
            long oldhp = mBaseAttr.CurHP;
            int oldlevel = mBaseAttr.Level;
            int oldLord = mBaseAttr.Lord;
            for (int i = 0; i < attrs.Count; ++i)
            {
                MsgData_sClientAttr ca = attrs[i];
                BasicAttrEnum type = BaseAttr.GetBasicAttrTypeFromStatType(ca.AttrType);
                if (type != BasicAttrEnum.Unkonw)
                {
                    m_BaseAttr.SetBasicAttrValue((int)type, ca.AttrValue);
                }
                else
                {
                    OnUpdateOtherAttr(ca.AttrType, ca.AttrValue);
                }
            }

            //升级了重新创建血条
            if (oldlevel != mBaseAttr.Level)
            {
                mHealth.OnCreateHPBar();
                if (IsRiding || (IsWing && m_IsInFly))
                {
                    //骑马或翅膀状态要更新绑点
                    if (IsWing && m_IsInFly)
                    {
                        mHealth.OnNodeChange(gameObject, "Wing_E_top");         //优先翅膀
                    }
                    else
                    {
                        mHealth.OnNodeChange(mMount, "Horse_E_top");
                    }
                }
            }
            else if (oldhp != mBaseAttr.CurHP)
            {
                mHealth.OnHPChange();
            }
            else if (oldLord != mBaseAttr.Lord)
            {
                //LogMgr.LogError("11111111111111");
                mHealth.OnCreateHPBar();
            }
        }

        /// <summary>
        /// 更新其他非基础属性。
        /// </summary>
        /// <param name="type">属性类型。</param>
        /// <param name="value">属性值。</param>
        public virtual void OnUpdateOtherAttr(sbyte type, double value)
        {

        }

        /// <summary>
        /// 是否是上下步结构
        /// </summary>
        protected bool isUpperAndBasePart = false;

        public bool IsUpperAndBasePart
        {
            get { return isUpperAndBasePart; }
            protected set { isUpperAndBasePart = value; }
        }

        protected Transform upperPart = null;

        public Transform UpperPart
        {
            get { return upperPart; }
            protected set { upperPart = value; }
        }

        protected Transform basePart = null;

        public Transform BasePart
        {
            get { return basePart; }
            protected set { basePart = value; }
        }

        public LineRenderer LineRender = null;

        private int m_BuffDamageReduction;
        public void SetBuffDamageReduction(int reduction = 0, float time = 0f)
        {
            m_BuffDamageReduction = reduction;
            if (m_BuffDamageReduction != 0)
            {
                CancelInvoke("SetBuffDamageReduction0");
                Invoke("SetBuffDamageReduction0", time);
            }
        }
        private void SetBuffDamageReduction0()
        {
            m_BuffDamageReduction = 0;
        }
        
        public void SetBuffDuelActor(GameObject actor = null, float time = 0f)
        {
            m_BuffDuelActor = actor;
            if (m_BuffDuelActor != null)
            {
                CancelInvoke("SetBuffDuelActorNull");
                Invoke("SetBuffDuelActorNull", time);
            }
        }
        private void SetBuffDuelActorNull()
        {
            m_BuffDuelActor = null;
        }

        public float m_BuffDamageTransferPer;  // 伤害转移 百分比
        private GameObject m_BuffDamageTransfer;  // 伤害转移
        public void SetBuffDamageTransfer(GameObject actor = null, float time = 0f)
        {
            m_BuffDamageTransfer = actor;
            if (m_BuffDamageTransfer != null)
            {
                CancelInvoke("SetBuffDamageTransferNull");
                Invoke("SetBuffDamageTransferNull", time);
            }
        }
        private void SetBuffDamageTransferNull()
        {
            m_BuffDamageTransfer = null;
        }

        //被击是否转身,0-转身，1-不转
        protected int m_iHitTurn = 0;
        public bool bHitTurn
        {
            get { return m_iHitTurn == 0; }
        }

        //体型大小
        protected int m_bodyType = 0;
        public int BodyType
        {
            get { return m_bodyType; }
        }

        //被击声音
        protected int m_beHitVoice = 0;
        public int BehitVoice
        {
            get { return m_beHitVoice; }
        }

        //击中音效
        protected int m_beHitSound = 0;
        public int BehitSound
        {
            get { return m_beHitSound; }
        }

        //死亡音效
        protected int m_DieSound = 0;
        public int DieSound
        {
            get { return m_DieSound; }
        }

        //尸体存在时间
        protected float m_bodyKeepTime = 0.0f;
        public float BodyKeepTime
        {
            get { return m_bodyKeepTime; }
        }

        protected int m_CanKnock = 0;
        public int CanKnock
        {
            get { return m_CanKnock; }
        }

        //表现模版信息
        //private List<SkillClassDisplayDesc> m_skillDisplayList = new List<SkillClassDisplayDesc>();

        public struct SummonData
        {
            public int SummonID;
            public int entityid;
            public float lifeTime;
            public float startTime;
            public bool deadthToKill;
        }
        
        //是否有刚体
        private bool m_isHaveRigidbody = false;

        public bool HaveRigidbody
        {
            get { return m_isHaveRigidbody; }
            set { m_isHaveRigidbody = value; }
        }


        //root刚体
        public Rigidbody m_rootRigidbody = null;
        
        //不可移动,主动，被动
        private bool m_canCurveMove = true;

        public int m_nNavIndex = -1;



        public int NavIndex
        {
            get { return m_nNavIndex; }
            set { m_nNavIndex = value; }

        }

        private Vector3 m_vNavPos;
        public Vector3 NavPos
        {
            get { return m_vNavPos; }
            set { m_vNavPos = value; }

        }

        //yy 阵营 1为玩家 2为友好NPC 3为敌对方
        public int m_TeamType;

        public int TeamType
        {
            get { return m_TeamType; }
            set { m_TeamType = value; }
        }


        /// <summary>
        /// 控制只能被带走一次
        /// </summary>
        public bool BeTakenAway { get; set; }

        public GameObject BeTakenAwayOwner { get; set; }



        public float GetBehaviorDeltaTime()
        {
            return UnityEngine.Time.deltaTime * (m_bPaused ? 0.0f : 1.0f);
        }


        protected bool m_isJoystickMove = false;
        public bool isJoystickMove
        {
            get { return m_isJoystickMove; }
            set { m_isJoystickMove = value;}
        }

        /// <summary>
        /// 战斗状态
        /// </summary>
        protected bool m_IsInFight = false;
        /// <summary>
        /// 是否处于战斗状态
        /// </summary>
        public bool InInFight
        {
            get { return m_IsInFight; }
        }
        /// <summary>
        /// 退出战斗状态倒计时
        /// </summary>
        protected float m_OutFightCD = 0f;

        /// <summary>
        /// 是否处于飞行状态
        /// </summary>
        protected bool m_IsInFly = false;
        /// <summary>
        /// 进入飞行状态倒计时
        /// </summary>
        protected float m_FlyCD = 0f;

        //用于跨服同阵营判定
        public bool IsSameCamp()
        {
            return (Faction == 14 || Faction == 15 || Faction == 16) && (PlayerData.Instance.Faction == Faction);
        }

        public virtual void SelectTarget(ActorObj selObject)
        {

        }

        public virtual ActorObj AutoSelTarget(float fSelectDist)
        {
            return null;
        }


        public virtual void SetTarget(ActorObj obj)
        {
            m_SelectTargetObject = obj;
        }


        public bool IsSkillAim(sbyte AimType, ActorObj target)
        {
            if (target.mActorType == ActorType.AT_NON_ATTACK)
                return false;

            //友方
            if (AimType == 2)
            {
                // 技能给友方
                if (IsAimActorType(target))
                    return false;
            }
            else
            {
                // 技能给敌方
                if (!IsAimActorType(target))
                    return false;
            }
            return true;
        }

        //判断是否是敌对阵营
        public bool IsAimActorType(ActorObj target)
        {
            if (target.mActorType == ActorType.AT_NON_ATTACK)
                return false;
            if (target.mActorType == ActorType.AT_TRAP)
                return false;
            if (target.mActorType == ActorType.AT_SCENE_BUFF)//场景buff也是中立的
                return false;

            //如果是PVP状态
            if (CoreEntry.gGameMgr.IsPvpState())
            {
                if (m_TeamType != TeamType)
                    return true;
            }
            
            //玩家
            if (m_TeamType == 1)
            {
                return target.TeamType == 3;
            }

            //NPC
            if (m_TeamType == 2)
            {
                return target.TeamType == 3;
            }

            //monster
            if (m_TeamType == 3)
            {
                return target.TeamType != 3;
            }

            return false;
        }



        //初始化.对外接口
        public override void Init(int resID, int configID, long ServerID, string strEnterAction = "", bool isNpc = false)
        {
            base.Init(resID, configID, ServerID, strEnterAction, isNpc);
            if(mBaseAttr == null)
                mBaseAttr = new BaseAttr();
            if(m_AnimationMgr == null)
                m_AnimationMgr = new AnimationMgr();
            m_AnimationMgr.Init(this);

            m_resID = resID;
            m_useCurveMoveUUID = 0;
            m_bSummonMonster = false;


            CanNotBeControlledByInputCounter = 0;
            CanBeControlledByInput = true;
            CanBeControlledByInputFromNormalAttack = true;
            IsProtectedBeforeSkillWhenBeHit = false;

            m_IsInFight = false;
            mShowHorse = true;
            mShowWing = true;
            mShowMagic = true;
            mOnHorse = false;
            mInWing = false;
            mInMagic = false;
            
            mSpeedScale = 1f;
            m_ModelConfig = ConfigManager.Instance.Common.GetModelConfig(resID);
            if (m_ModelConfig != null)
            {
                m_strEnterAction = m_ModelConfig.Get<string>("san_born");
                CheckNormalAttackNum();
                mActorType = (ActorType)m_ModelConfig.Get<int>("type");
            }

            m_animationCurveBase = new AnimationCurveBase();
            m_animationCurveBase.Init(this.gameObject);

            m_skinnedMeshRenderer = this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

            //m_oldShaderList = new Shader[m_skinnedMeshRenderer.Length, 3];

            //保存现有的shader
            if (m_skinnedMeshRenderer.Length > 0 && mActorType != ActorType.AT_BROKED && !this.gameObject.CompareTag("broked"))
            {
                m_bUseBehitColor = true;

            }

            //if (m_bUseBehitColor)
            //{
            //    for (int i = 0; i < m_skinnedMeshRenderer.Length; i++)
            //    {
            //        for (int j = 0; j < m_skinnedMeshRenderer[i].materials.Length; ++j)
            //        {
            //            m_oldShaderList[i, j] = m_skinnedMeshRenderer[i].materials[j].shader;
            //        }

            //    }
            //}




            //刚体
            InitRigidbody();

            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_STAND;
            RequestChangeState(param);

            //发送消息，激活状态
            EventParameter eventParam = EventParameter.Get();
            eventParam.intParameter = entityid;
            m_eventMgr.TriggerEvent(GameEvent.GE_ACTOR_CAN_BEHIT, eventParam);

            //恢复碰撞
            RecoverCollider();

            if (m_gravityMotionBase != null)
            {
                m_gravityMotionBase.Init();
            }
            
            NavMeshObstacle[] navMeshObstacles = this.gameObject.GetComponentsInChildren<NavMeshObstacle>();
            for (int i = 0; i < navMeshObstacles.Length; i++)
            {
                navMeshObstacles[i].enabled = true;
            }

            MonsterObj monster = this.gameObject.GetComponent<MonsterObj>();

            if (monster != null && monster.m_IsUpperAndBasePart)
            {
                UpperPart = GetChildTransform("upper");
                BasePart = GetChildTransform("base");
                IsUpperAndBasePart = true;
            }
            else
            {
                IsUpperAndBasePart = false;
            }

            mHealth = new ActorHealth(this);
            if (HeadTypeLogo.Instance != null)
            {
                HeadTypeLogo.Instance.OnModelEventUpdate(this);
            }
        }

        public void OnPostInit()
        {
        }

        //复活
        public void Recover(bool needResetPos = true)
        {
            GameObject obj = this.gameObject;
            if (null == obj || null == mBaseAttr )
                return;
            ActorObj actorObject = obj.GetComponent<ActorObj>();
            if (actorObject != null)
            {
                CoreEntry.gActorMgr.AddActorObj(actorObject);
            }
            
            curHp = mBaseAttr.MaxHP;
            
            //设置位置为复活点
            if (mActorType == ActorType.AT_LOCAL_PLAYER)
            {
                SpawnPlayer[] startPoints = CoreEntry.gGameDBMgr.GetSpawnPlayerPos();
                if (needResetPos)
                {
                    if(startPoints.Length > 0)
                        SetPosition(startPoints[0].pos);
                }

                //重新激活AI
                bool bIsMainPlayer = true;
                PlayerAgent playeAgent = GetComponent<PlayerAgent>();

                if (playeAgent)
                {
                    playeAgent.gameObject.SetActive(true);
                }

                ////主玩家
                //if (bIsMainPlayer)
                //{
                //    //即时更新所有血条位置
                //    EventToUI.SendEvent("EU_UPDATE_TOP_BLOOD_POSITION");
                //}

                //如果是主玩家
                if (bIsMainPlayer)
                {
                    //立刻更新相机位置
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_UPDATE_CAMERA_POSITION_IMMEDIATE, null);
                    if (playeAgent)
                    {
                        playeAgent.enabled = false;
                    }
                }
                else
                {
                    if (playeAgent)
                    {
                        playeAgent.enabled = true;
                    }
                }
            }

            ////播放复活特效
            //string enterEfx = "Effect/skill/buff/fx_buff_fuhuo";

            //SceneEfxPool efx = null;

            //GameObject efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(enterEfx);
            //if (null != efxObj)
            //{
            //    efx = efxObj.GetComponent<SceneEfxPool>();
            //}
            //if (efx == null)
            //{
            //    efx = efxObj.AddComponent<SceneEfxPool>();
            //}
            //if (efx != null)
            //{
            //    efx.Init(transform.position, 1.0f);
            //}



            //重置CD
            ClearAllSkillCD();

            EventParameter eventBuffParam = EventParameter.Get();
            eventBuffParam.goParameter = gameObject;
            eventBuffParam.objParameter = this;
            eventBuffParam.intParameter = -1;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EVENT_REBORN, eventBuffParam);

        }


        public virtual void SetMoveSpeed(float fSpeed)
        {
            //自动寻路
            if (m_bAutoMove && m_AutoMove != null)
            {
                m_AutoMove.SetMoveSpeed(fSpeed);
            }

        }
        public override void NavigateTo(Vector3 pos)
        {
        }


        //移动接口，给组件回调
        public virtual bool MoveToPos(Vector3 pos, bool bChangeState = true)
        {
			//Debug.LogError ("===移动接口，给组件回调=="+pos +bChangeState);

            if (IsDeath())
            {
                return false;
            }

            if (m_move == null)
            {
                return false;
            }

            if (!bChangeState)
            {
                m_move.MovePosition(pos);
                return true;
            }

            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_RUN;

            bool ret = RequestChangeState(param);
			//Debug.LogError ("===移动接口，给组件回调=="+pos +ret);

            if (ret)
            {
                //自动寻路
                if (m_bAutoMove && m_AutoMove != null)
                {
                    m_AutoMove.MovePos(pos);
                }
                else
                {
                    return m_move.MovePosition(pos);
                }
                return true;
            }
            else
            {

                if (ProcessMoveDirWhenCannotMove(pos - GetPosition()))
                {
                    return false;
                }

                //带移动的技能
                if (CanMove())
                {
                    return m_move.MovePosition(pos);
                }
            }

            return false;
        }

        /// <summary>
        /// 获取寻路剩余的路点
        /// </summary>
        /// <returns></returns>
        public virtual List<Vector3> GetRestPathPos()
        {
            if (null != m_move)
            {
                return m_move.GetRestPath();
            }

            return null;
        }

        public void SetServerPosition(Vector2 pos)
        {
            if (m_move != null)
            {
                m_move.SetServerPosition(pos);
            }
        }

        // 是否能走过去
        public virtual bool CanArrived(Vector3 pos)
        {
            if (m_move == null)
            {
                return false;
            }

            return m_move.IsCanArrived(pos);
        }

        public virtual void MoveToDir(Vector3 dir)
        {
            if (IsDeath())
            {
                return;
            }

            if (CanBeControlledByInput == false)
            {
                return;
            }

            if (CanBeControlledByInputFromNormalAttack == false)
            {
                return;
            }

            if (m_move == null)
            {
                return;
            }

            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_RUN;

            bool ret = RequestChangeState(param);
            if (ret)
            {
                m_move.MoveDirection(dir);
            }
            else
            {
                ProcessMoveDirWhenCannotMove(dir);

                //带移动的技能
                if (CanMove())
                {
                    m_move.MoveDirection(dir);
                }
            }
        }

        /// <summary>
        /// 不能移动的状态下处理方向
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private bool ProcessMoveDirWhenCannotMove(Vector3 dir)
        {
            //aiming状态，旋转方向
            if (curActorState == ACTOR_STATE.AS_AIMING)
            {
                m_move.FaceTo(dir);
                return true;
            }

            return false;
        }

        public virtual void StopMove(bool isSendEvent)
        {
            if (IsDeath())
            {
                return;
            }

            if (m_move == null)
            {
                return;
            }

            //保存当前的方向
            //m_move.m_curTouchDir = Vector3.zero;


            if (m_bAutoMove && m_AutoMove != null)
            {
                m_AutoMove.Stop(isSendEvent);
            }
            else
            {
                //m_move.Stop(isSendEvent);
                if (m_move.Status != MoveStatus.Stopped)
                {
                    m_move.Stop();
                }
            }
        }

        public override void SetSpeed(float speed)
        {
            if (m_move == null)
            {
                return;
            }
        }

        public virtual float GetSpeed()
        {
            if (null != mBaseAttr)
            {
                return mBaseAttr.Speed;
            }

            return 4.0f;
        }

        protected bool m_AutoPathFind = false;
        public bool AutoPathFind
        {
            set
            {
                m_AutoPathFind = value;

                if (mActorType == ActorType.AT_LOCAL_PLAYER)
                {
                    gEventMgr.TriggerEvent(GameEvent.GE_AUTO_PATH_FIND_STATE, null);
                }
            }
            get
            {
                return m_AutoPathFind;
            }
        }

        /// <summary>
        /// 忽略条件，强制复活
        /// </summary>
        public override void ForceToRevive()
        {
            base.ForceToRevive();
            if (mHealth != null)
            {
                mHealth.OnResetHP();
            }
        }

        public override void ForceToRebirth()
        {
            base.ForceToRebirth();

            PlayAction("rebirth");

            //播放复活特效
            string enterEfx = "Effect/skill/buff/fx_buff_fuhuo";

            SceneEfxPool efx = null;

            GameObject efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(enterEfx);
            if (null != efxObj)
            {
                efx = efxObj.GetComponent<SceneEfxPool>();
                if (efx == null)
                {
                    efx = efxObj.AddComponent<SceneEfxPool>();
                }
                if (efx != null)
                {
                    efx.Init(transform.position, 2.0f);
                }
            }
           
        }

        float AUDIO_MIN_SOUND = 2.1f;
        float AUDIO_MAX_SOUND = 50f;


        //实例化调用
        public override void Awake()
        {
            base.Awake();

            m_gameDBMgr = CoreEntry.gGameDBMgr;
            m_eventMgr = CoreEntry.gEventMgr;
            m_gameMgr = CoreEntry.gGameMgr;
            m_baseTool = CoreEntry.gBaseTool;

            m_audioSource = this.gameObject.GetComponent<AudioSource>();
            if (m_audioSource == null)
            {
                m_audioSource = this.gameObject.AddComponent<AudioSource>();
                m_audioSource.playOnAwake = false;

                m_audioSource.minDistance = AUDIO_MIN_SOUND;
                m_audioSource.maxDistance = AUDIO_MAX_SOUND;

            }
            m_audioSource.spatialBlend = 1.0f;

            Transform[] childs = this.gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < childs.Length; ++i)
            {
                if (childs[i].name == "E_top")
                {
                    m_audioSourceBody = childs[i].GetComponent<AudioSource>();
                    break;
                }
            }
            if (m_audioSourceBody == null)
            {
                m_audioSourceBody = this.gameObject.AddComponent<AudioSource>();
                m_audioSourceBody.playOnAwake = false;

                m_audioSourceBody.minDistance = AUDIO_MIN_SOUND;
                m_audioSourceBody.maxDistance = AUDIO_MAX_SOUND;
            }
            m_audioSourceBody.spatialBlend = 1.0f;

            m_RunState = this.gameObject.GetComponent<RunState>();
            if (null == m_RunState)
            {
                m_RunState = this.gameObject.AddComponent<RunState>();
            }
            m_StandState = this.gameObject.GetComponent<StandState>();
            if (null == m_StandState)
            {
                m_StandState = this.gameObject.AddComponent<StandState>();
            }
            m_DeathState = this.gameObject.GetComponent<DeathState>();
            if (null == m_DeathState)
            {
                m_DeathState = this.gameObject.AddComponent<DeathState>();
            }
            m_AttackState = this.gameObject.GetComponent<AttackState>();
            if (null == m_AttackState)
            {
                m_AttackState = this.gameObject.AddComponent<AttackState>();
            }
            m_BehitState = this.gameObject.GetComponent<BehitState>();
            if (null == m_BehitState)
            {
                m_BehitState = this.gameObject.AddComponent<BehitState>();
            }
            m_EnterState = this.gameObject.GetComponent<EnterState>();
            if (null == m_EnterState)
            {
                m_EnterState = this.gameObject.AddComponent<EnterState>();
            }
            m_DizzyState = this.gameObject.GetComponent<DizzyState>();
            if (null == m_DizzyState)
            {
                m_DizzyState = this.gameObject.AddComponent<DizzyState>();
            }
            m_LimitState = this.gameObject.GetComponent<LimitState>();
            if (null == m_LimitState)
            {
                m_LimitState = this.gameObject.AddComponent<LimitState>();
            }
            m_SheepState = this.gameObject.GetComponent<SheepState>();
            if (null == m_SheepState)
            {
                m_SheepState = this.gameObject.AddComponent<SheepState>();
            }
            m_RunToAttackState = this.gameObject.GetComponent<RunToAttackState>();
            if (null == m_RunToAttackState)
            {
                m_RunToAttackState = this.gameObject.AddComponent<RunToAttackState>();
            }
            m_FearState = this.gameObject.GetComponent<FearState>();
            if (null == m_FearState)
            {
                m_FearState = this.gameObject.AddComponent<FearState>();
            }
            m_AimingState = this.gameObject.GetComponent<AimingState>();
            if (null == m_AimingState)
            {
                m_AimingState = this.gameObject.AddComponent<AimingState>();
            }
            m_StoneState = this.gameObject.GetComponent<StoneState>();
            if (null == m_StoneState)
            {
                m_StoneState = this.gameObject.AddComponent<StoneState>();
            }

            m_CollectState = this.gameObject.GetComponent<CollectState>();
            if (null == m_CollectState)
            {
                m_CollectState = this.gameObject.AddComponent<CollectState>();
            }

            m_StoryStandState = this.gameObject.GetComponent<StoryStandState>();
            if (null == m_StoryStandState)
            {
                m_StoryStandState = this.gameObject.AddComponent<StoryStandState>();
            }
            m_StoryPlayAnimState = this.gameObject.GetComponent<StoryPlayAnimState>();
            if (null == m_StoryPlayAnimState)
            {
                m_StoryPlayAnimState = this.gameObject.AddComponent<StoryPlayAnimState>();
            }
            m_StoryPlayAnimLoopState = this.gameObject.GetComponent<StoryPlayAnimLoopState>();
            if (null == m_StoryPlayAnimLoopState)
            {
                m_StoryPlayAnimLoopState = this.gameObject.AddComponent<StoryPlayAnimLoopState>();
            }
            m_fitState = this.gameObject.GetComponent<FitState>();
            if(null == m_fitState)
            {
                m_fitState = this.gameObject.AddComponent<FitState>();
            }

            //重力组件

            m_gravityMotionBase = this.gameObject.GetComponent<GravityMotionBase>();
            if (null == m_gravityMotionBase)
            {
                m_gravityMotionBase = this.gameObject.AddComponent<GravityMotionBase>();
            }

            mHitFlyCurveBase = this.gameObject.GetComponent<HitFlyMotionBase>();
            if (null == mHitFlyCurveBase)
            {
                mHitFlyCurveBase = this.gameObject.AddComponent<HitFlyMotionBase>();
            }

            chestSocket = GetChildTransform("E_Spine");
            centerSocket = transform.DeepFindChild("Bip001");

            //刚体
            //InitRigidbody();  

            //所有的collider
            m_allCollider = this.gameObject.GetComponentsInChildren<Collider>();
            RecoverCollider();

            //注册事件
            RegisterEvent();
        }

        // Update is called once per frame
        public virtual void Update()
        {

            if (IsDeath())
            {
                return;
            }

            if (m_IActorState)
            {
                m_IActorState.FrameUpdate();
            }

            if (m_BehitState != null)
            {
                m_BehitState.FrameUpdate();
            }


            if (m_animationCurveBase != null)
            {
                m_animationCurveBase.UpdateFrame();
            }

            // Buff更新
            Buff.OnBuffUpdateAll(this);

            // 召唤物按时间消失
            SummonCell.OnSummonUpdate(this);

            if (mHealth != null)
            {
                mHealth.Update(Time.deltaTime);
            }

            if (m_IsInFight && m_OutFightCD > 0)
            {
                m_OutFightCD -= Time.deltaTime;
                if (m_OutFightCD <= 0f)
                {
                    m_OutFightCD = 0f;

                    m_IsInFight = false;
                    if (m_RideOnOutFight)
                    {
                        m_RideOnOutFight = false;
                        ReqRideHorse();
                    }
                }
            }

            if (IsWing && m_FlyCD > 0)
            {
                m_FlyCD -= Time.deltaTime;
                if (m_FlyCD <= 0)
                {
                    m_FlyCD = 0f;

                    m_IsInFly = true;

                    if (IsPlayingAction(StateParameter.ANI_NAME_RUN))
                    {
                        PlayAction(StateParameter.ANI_NAME_RUN);
                    }
                }
            }
        }



        //下面是一些组件接口

        //1,播放动画
        public virtual void PlayAction(string strAction, bool isCrossFade = true, ActionVoiceDesc voiceDesc = null)
        {
            if (m_AnimationMgr == null) return;
            //加个预防判断
            if (m_IActorState != null && m_IActorState.m_state == ACTOR_STATE.AS_ATTACK)
            {
                //攻击状态不能设置stand
                if (strAction.CompareTo(StateParameter.ANI_NAME_STAND) == 0)
                {
                    return;
                }

                if (m_curActionName == strAction)
                {
                    if (m_AnimationMgr.IsPlayingAction(strAction))
                    {
                        return;
                    }
                }

            }


            if (m_curActionName == "idle" && IsPlayingAction("idle") && strAction == "stand")
            {
                return;
            }

            //坐骑状态动画要进行修改
            if (IsRiding)
            {
                if (strAction.CompareTo(StateParameter.ANI_NAME_STAND) == 0)
                {
                    strAction = StateParameter.ANI_NAME_MOUNT_STAND;
                    mMount.GetComponent<Animation>().CrossFade(StateParameter.ANI_NAME_STAND);
                }
                else if (strAction.CompareTo(StateParameter.ANI_NAME_RUN) == 0)
                {
                    strAction = StateParameter.ANI_NAME_MOUNT_RUN;
                    mMount.GetComponent<Animation>().CrossFade(StateParameter.ANI_NAME_RUN);
                }
                else if (strAction.CompareTo("hit001") == 0)
                {
                    strAction = StateParameter.ANI_NAME_MOUNT_BEHIT;
                }
            }
            else if (IsWing && m_IsInFly)
            {
                if (strAction.CompareTo(StateParameter.ANI_NAME_STAND) == 0)
                {
                    strAction = StateParameter.ANI_NAME_FLY_STAND;
                    mWing.GetComponent<Animation>().CrossFade(StateParameter.ANI_NAME_STAND);
                }
                else if (strAction.CompareTo(StateParameter.ANI_NAME_RUN) == 0)
                {
                    strAction = StateParameter.ANI_NAME_FLY_RUN;
                    mWing.GetComponent<Animation>().CrossFade(StateParameter.ANI_NAME_RUN);
                }
                else if (strAction.CompareTo("hit001") == 0)
                {
                    strAction = StateParameter.ANI_NAME_FLY_BEHIT;
                    mWing.GetComponent<Animation>().CrossFade(StateParameter.ANI_NAME_RUN);
                }
            }

            if (m_AnimationMgr != null)
            {
                m_AnimationMgr.PlayAction(strAction, isCrossFade, voiceDesc);
                m_curActionName = m_AnimationMgr.m_curActionName;
            }

        }

        public virtual void StopAction(string strAction)
        {
            if (m_AnimationMgr != null)
                m_AnimationMgr.StopAction(strAction);
        }

        public virtual void StopAll()
        {
            if (m_AnimationMgr != null)
                m_AnimationMgr.StopAll();
        }

        public virtual bool IsPlayingAction(string strAction)
        {
            if (m_AnimationMgr != null)
                return m_AnimationMgr.IsPlayingAction(strAction);

            return false;

        }

        public virtual float GetActionLength(string strAction)
        {
            if (m_AnimationMgr != null)
                return m_AnimationMgr.GetActionLength(strAction);

            return 0f;
        }

        public virtual void SetActionSpeed(string strAction, float speed)
        {
            if (m_AnimationMgr != null)
                m_AnimationMgr.SetActionSpeed(strAction, speed);

        }

        public virtual void SetSkillActionSpeed(string strAction, float speed, LuaTable skillDesc)
        {
            if (m_AnimationMgr != null)
                m_AnimationMgr.SetActionSpeed(strAction, speed);
        }

        public virtual void SetActionTime(string strAction, float timeValue)
        {
            if (m_AnimationMgr != null)
                m_AnimationMgr.SetActionSpeed(strAction, timeValue);
        }

        //获取动作剩余时间
        public float GetLeftActionTime(string strAction)
        {
            if (m_AnimationMgr != null)
                return m_AnimationMgr.GetLeftActionTime(strAction);

            return 0f;
        }

        //获取当前动作播放的时间
        public float GetCurActionTime(string strAction)
        {
            if (m_AnimationMgr != null)
                return m_AnimationMgr.GetCurActionTime(strAction);

            return 0f;
        }

        //是否存在action
        public virtual bool IsHadAction(string strAction)
        {
            if (m_AnimationMgr != null)
                return m_AnimationMgr.IsHadAction(strAction);

            return false;
        }

        public virtual bool IsHadAction(string strAction, Animation anim)
        {
            if (m_AnimationMgr != null)
                return m_AnimationMgr.IsHadAction(strAction, anim);

            return false;
        }

        //获取当前播放的动作
        public string GetCurPlayAction()
        {
            if (m_AnimationMgr != null)
                return m_AnimationMgr.GetCurPlayAction();

            return null;
        }

        //播放声音
        public void PlaySound(string strClip)
        {
            //add by Alex 20150416 音效控制开关
            if (!CoreEntry.cfg_bEaxToggle)
            {
                return;
            }

            CoreEntry.SoundEffectMgr.PlaySoundEffect(this, m_audioSource, strClip);
        }

        public void StopSound()
        {
            if (m_audioSource)
            {
                //m_audioSource.Stop();
            }
        }

        public bool IsPlayingSound()
        {
            if (null != m_audioSource)
            {
                return m_audioSource.isPlaying;
            }
            return false;
        }

        public void PlaySound2(string strClip)
        {
            //add by Alex 20150421 音效控制开关
            if (!CoreEntry.cfg_bEaxToggle)
            {
                return;
            }

            CoreEntry.SoundEffectMgr.PlaySoundEffect(this, m_audioSourceBody, strClip);
        }

        public void StopSound2()
        {
            if (m_audioSourceBody)
            {
                //m_audioSourceBody.Stop();
            }
        }


        public bool IsDeath()
        {
            return curActorState == ACTOR_STATE.AS_DEATH;
        }

        //是否用服务器的死亡消息
        public bool IsUseServerDeath()
        {
            bool isUseServerDeath = false;

            return isUseServerDeath;
        }

        private bool m_IgnoredGravityMotion = false;

        public bool IgnoredGravityMotion
        {
            get { return m_IgnoredGravityMotion; }
            set { m_IgnoredGravityMotion = value; }
        }

        public bool IsActorEndure()
        {
            return false;
        }


        public bool ChangeState(ACTOR_STATE state)
        {
            StateParameter param = new StateParameter();
            param.state = state;
            return RequestChangeState(param);
        }

        ACTOR_STATE lastState = ACTOR_STATE.AS_NONE;  //最后状态

        //请求切换状态
        public bool RequestChangeState(StateParameter param)
        {
            if (m_IActorState == null)
            {
                if (ACTOR_STATE.AS_ATTACK == param.state)
                {
                    m_curCastSkillID = param.skillID;
                    LogMgr.Log("clear  skill ID 66666666 :" + m_curCastSkillID);
                }
                else if (ACTOR_STATE.AS_BEHIT == param.state)
                {
                    m_curBeHitSkillID = param.skillID;
                    m_hitActorObject = param.AttackActor;
                }

                //直接切换
                IActorState actorStateMgr = GetActorState(param.state);
                if (actorStateMgr != null)
                {
                    actorStateMgr.CurParam = param;
                    actorStateMgr.OnEnter(this);
                }
                m_IActorState = actorStateMgr;
                return true;
            }
            
            //yy修改，没进受击，也要知道被谁打
            if (ACTOR_STATE.AS_BEHIT == param.state || ACTOR_STATE.AS_DEATH == param.state)
            {
                m_curBeHitSkillID = param.skillID;
                m_hitActorObject = param.AttackActor;
            }

            //受技能前置保护时不能被受击打断
            if (ACTOR_STATE.AS_BEHIT == param.state && IsProtectedBeforeSkillWhenBeHit)
            {
                return false;
            }
            if (m_IActorState == null) return false;
            //当前状态能否退出
            bool ret = m_IActorState.CanChangeState(param);
            if (!ret)
            {
                return false;
            }

            //操作限制类Buff add by yuxj
            if (TestCanDo(param) == false)
            {
                return false;
            }


            //原状态退出
            m_IActorState.OnExit(param);

            int skillID = param.skillID;

            //释放技能
            if (ACTOR_STATE.AS_ATTACK == param.state && param.isComposeSkill && m_AttackState.IsUseComposeSkill())
            {
                //组合技能                        
                skillID = param.composeSkillDesc.composeSkillID;
                m_actionStartPlayTime = param.composeSkillDesc.startTime;



            }

            if (ACTOR_STATE.AS_ATTACK == param.state)
            {
                m_curCastSkillID = skillID;
                //  LogMgr.ErrorLog("RequestChangeState  skill ID 7777777  :" + skillID);
            }
            if (ACTOR_STATE.AS_RUN_TO_ATTACK == param.state)
            {
                m_RuncastSKillID = skillID;
                //  LogMgr.ErrorLog("RequestChangeState  skill ID 888888 :" + skillID);
            }


            else if (ACTOR_STATE.AS_BEHIT == param.state)
            {
                m_curBeHitSkillID = skillID;
                m_hitActorObject = param.AttackActor;
            }


            EnterState(param);
            if (mActorType == ActorType.AT_LOCAL_PLAYER)
            {
                // if (m_TeamType == 1 && lastState == ACTOR_STATE.AS_RUN && lastState != ACTOR_STATE.AS_NONE && lastState != m_IActorState.m_state)
                if (m_TeamType == 1 && lastState != ACTOR_STATE.AS_NONE && lastState != m_IActorState.m_state
                    && TaskMgr.bRunAndTasking && (lastState == ACTOR_STATE.AS_RUN || lastState == ACTOR_STATE.AS_STAND))
                {
                    EventParameter par = EventParameter.Get();
                    par.objParameter = param;
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_CurMoveState, par);
                }
            }
            lastState = param.state;
            //LogMgr.UnityLog("state="+m_IActorState.m_state);
            return true;
        }

        public void EnterState(StateParameter param)
        {
            //新状态进入
            ACTOR_STATE oldas = m_IActorState == null ? ACTOR_STATE.AS_NONE : m_IActorState.m_state;
            IActorState newActorStateMgr = GetActorState(param.state);
            if (newActorStateMgr == null)
            {
                return;
            }
            m_IActorState = newActorStateMgr;

            if (m_IActorState == null)
            {
                return;
            }
            m_IActorState.CurParam = param;

            newActorStateMgr.OnEnter(this);
            ACTOR_STATE newas = m_IActorState.m_state;

            //主角移动后，开始飞行计时
            if (oldas != newas && this.IsMainPlayer())
            {
                if (oldas == ACTOR_STATE.AS_RUN)
                {
                    m_FlyCD = 0;
                }
                else if (newas == ACTOR_STATE.AS_RUN)
                {
                    if (IsWing && !m_IsInFly)
                    {
                        m_FlyCD = ConfigManager.Instance.Consts.GetValue<float>(404, "fval");
                    }
                }
            }

            if (mActorType == ActorType.AT_LOCAL_PLAYER)
            {
                if (newas != ACTOR_STATE.AS_STAND &&
                    newas != ACTOR_STATE.AS_RUN)
                {
                    m_IsInFight = true;
                    m_OutFightCD = 0;
                    m_IsInFly = false;
                }
                else if (m_IsInFight)
                {
                    if ((oldas != ACTOR_STATE.AS_STAND && oldas != ACTOR_STATE.AS_RUN) && (newas == ACTOR_STATE.AS_STAND ||
                        newas == ACTOR_STATE.AS_RUN))
                    {
                        m_OutFightCD = ConfigManager.Instance.Consts.GetValue<float>(405, "fval");
                    }
                }
            }
        }

        public bool TestCanDo(StateParameter param)
        {
            if (m_limitParam != null && m_limitParam.iState != 0)
            {
                if (!m_limitParam.TestCanDo(param))
                    return false;
            }

            ///可以多个限制在一身
            for (int i = 0; i < actorLimitList.Count; ++i)
            {
                if (actorLimitList[i] != null && actorLimitList[i].iState != 0)
                {
                    if (!actorLimitList[i].TestCanDo(param))
                        return false;
                }
            }

            // 霸体、抗眩晕和限制
            if (ACTOR_STATE.AS_DIZZY == param.state || ACTOR_STATE.AS_LIMIT == param.state)
            {
                if (IsActorEndure())
                    return false;
            }

            return true;
        }

        public bool TestCanDo(int skillID)
        {
            StateParameter param = new StateParameter();
            param.skillID = skillID;
            param.state = ACTOR_STATE.AS_ATTACK;
            return TestCanDo(param);
        }


        /// <summary>
        /// 是否能切换状态
        /// </summary>
        /// <param name="stateParm"></param>
        /// <returns></returns>
        public bool CanChangeState(StateParameter stateParm)
        {
            if (m_IActorState != null)
            {
                return m_IActorState.CanChangeState(stateParm);
            }

            return false;
        }

        public IActorState GetActorState(ACTOR_STATE state)
        {
            switch (state)
            {
                case ACTOR_STATE.AS_ATTACK:
                    return m_AttackState;
                case ACTOR_STATE.AS_BEHIT:
                    return m_BehitState;
                case ACTOR_STATE.AS_STAND:
                    return m_StandState;
                case ACTOR_STATE.AS_RUN:
                    return m_RunState;
                case ACTOR_STATE.AS_DEATH:
                    return m_DeathState;
                case ACTOR_STATE.AS_ENTER:
                    return m_EnterState;
                case ACTOR_STATE.AS_DIZZY:
                    return m_DizzyState;
                case ACTOR_STATE.AS_LIMIT:
                    return m_LimitState;
                case ACTOR_STATE.AS_SHEEP:
                    return m_SheepState;
                case ACTOR_STATE.AS_RUN_TO_ATTACK:
                    return m_RunToAttackState;

                case ACTOR_STATE.AS_FEAR:
                    return m_FearState;
                case ACTOR_STATE.AS_AIMING:
                    return m_AimingState;
                case ACTOR_STATE.AS_STONE:
                    return m_StoneState;
                case ACTOR_STATE.AS_COLLECT:
                    return m_CollectState;
                case ACTOR_STATE.AS_STORY_STAND:
                    return m_StoryStandState;
                case ACTOR_STATE.AS_STORY_PLAY_ANIM:
                    return m_StoryPlayAnimState;
                case ACTOR_STATE.AS_STORY_PLAY_ANIM_LOOP:
                    return m_StoryPlayAnimLoopState;
                case ACTOR_STATE.AS_FIT:
                    return m_fitState;
                default:
                    break;
            }

            return null;
        }


        public void OnHideSkillScope(int skillID)
        {
            if (m_WarningefxObj != null)
            {
                Destroy(m_WarningefxObj);
                m_WarningefxObj = null;
            }

        }

        public void OnShowSkillScope(int skillID)
        {

            LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(skillID);

            if (skillDesc != null)
            {
                SkillClassDisplayDesc skillClass = null;

                if (skillDesc.Get<int>("e_type_1") == 1)
                {
                    skillClass = m_gameDBMgr.GetSkillClassDisplayDesc(skillDesc.Get<int>("effect_1"));
                }

                if (skillClass != null)
                {
                    //GameObject obj = (GameObject)Object.Instantiate(CoreEntry.gResLoader.LoadResource(skillClass.prefabPath));
                    GameObject obj = CoreEntry.gGameObjPoolMgr.InstantiateSkillBase(skillClass.prefabPath);

                    if (obj != null)
                    {
                        //设置脚本组件
                        SkillBase skillBase = obj.GetComponent<SkillBase>();
                        skillBase.Init(this, null, skillID, true);

                        m_AttackState.m_curSkillBase = skillBase;
                        m_AttackState.m_curSkillID = skillID;

                        skillBase.ShowSkillScope();
                    }
                }
            }

        }

        public override ActorObj GetSelTarget()
        {
            if (ArenaMgr.Instance.IsArenaFight)
            {
                return CoreEntry.gActorMgr.GetPlayerActorByServerID(ArenaMgr.Instance.GetAttackID(serverID));
            }
            return m_SelectTargetObject;
        }

        public int GetSelTargetID()
        {
            if (m_SelectTargetObject != null)
            {
                return m_SelectTargetObject.EntityID;
            }

            return 0;
        }
        
        public void OnRunToAttack(int skillID)
        {
            if(mActorType == ActorType.AT_LOCAL_PLAYER)
            {
                LuaTable skillCfg = ConfigManager.Instance.Skill.GetSkillConfig(skillID);
                if (skillCfg.Get<int>("showtype") == (int)SkillShowType.ST_BEAUTYMANHETI 
                    || skillCfg.Get<int>("priority") == -1) //红颜合体技不需要转向,王者vip技能
                {
                    if(m_curCastSkillID != skillID)
                    {
                        //LogMgr.UnityError("OnRunToAttack: castSkillid" + m_curCastSkillID + " skillid:" + skillID);
                        NetLogicGame.Instance.SendReqBreakSkill(m_curCastSkillID, 0, 0, 0);
                        BreakSkillWhenAttack(m_curCastSkillID);
                    }
                }


            }



            if (mActorType == ActorType.AT_LOCAL_PLAYER && ACTOR_STATE.AS_ATTACK != curActorState)
            {
                //是否组合技能              
                LuaTable skillDesc = GetCurSkillDesc(skillID);
                if (skillDesc == null)
                {
                    return;
                }

                if (skillDesc.Get<int>("showtype") == (int)SkillShowType.ST_BEAUTYMANHETI) //红颜合体技能不需要移动再释放，直接释放
                {
                    m_bRunToAttack = false;
                }
                else
                {
                    ActorObj selObject = GetSelTarget();
                    float attackdistance = skillDesc.Get<float>("min_dispk");
                    if (selObject != null)
                    {
                        float distance = Vector3.Distance(transform.position, selObject.transform.position);
                        if (distance > attackdistance * 1.1f) // 适当增加攻击距离
                        {
                            StateParameter param = new StateParameter();
                            param.state = ACTOR_STATE.AS_RUN_TO_ATTACK;
                            param.skillID = skillID;
                            param.isComposeSkill = false;
                            RequestChangeState(param);
                            return;
                        }

                    }
                }
            }


            OnCastSkill(skillID);


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attackDistance"></param>
        /// <param name="target"></param>
        /// <returns>true代表正在跑，false代表不需要跑</returns>
        public bool RunForAttack(float attackDistance, ActorObj target)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > attackDistance * 1.1f) // 适当增加攻击距离
            {
                bool changeState = true;
                if (curActorState == ACTOR_STATE.AS_RUN)
                {
                    changeState = false;
                }

                MoveToPos(target.transform.position, changeState);
                return true;
            }

            return false;
        }

        public void CastAnySkill(int skillID)
        {
            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_ATTACK;
            param.skillID = skillID;
            param.isComposeSkill = false;

            //进入攻击模式
            bool ret = RequestChangeState(param);
            if (ret)
            {

                if (IsMainPlayer())
                {
                    //技能释放成功
                    EventParameter parameter = EventParameter.Get();
                    parameter.intParameter = skillID;
                    parameter.intParameter1 = m_curCastSkillID;
                    parameter.intParameter2 = entityid;
                    parameter.goParameter = this.gameObject;                    
                }
            }
        }


        public void DelayCastSkill()
        {
            CancelInvoke("DelayCastSkill");

            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_ATTACK;
            param.skillID = m_curCastSkillID;
            param.isComposeSkill = false;

            //是否组合技能              

            ComposeSkillDesc composeSkillDesc = m_gameDBMgr.GetComposeSkillDesc(m_curCastSkillID, m_curCastSkillID);
            if (composeSkillDesc != null)
            {
                param.isComposeSkill = true;
                param.composeSkillDesc = composeSkillDesc;
            }

            //进入攻击模式
            RequestChangeState(param);
            if (composeSkillDesc != null && composeSkillDesc.isOrder)
            {
                m_gameDBMgr.SetComposeSkillOrder(composeSkillDesc.srcSkillID,
                    composeSkillDesc.dstSkillID, composeSkillDesc.composeSkillID);
            }
        }

        public List<long> m_TargetList = new List<long>();
        
        public void ChooseSkillTarget()
        {
            //计算伤害  群体
            List<ActorObj> objList = CoreEntry.gActorMgr.GetAllMonsterActors();

            Dictionary<float, long> m_TargetDir = new Dictionary<float, long>();

            m_TargetList.Clear();

            if (m_curSkillDesc == null)
            {
                return;
            }

            //先加当前选择目标
            ActorObj actor = null;
            actor = GetSelTarget();
            if (actor != null)
            {
                //按伤害范围算出受伤害对象  
                bool isSkillSuccess = CoreEntry.gSkillMgr.IsSkillDamageRange(m_curSkillDesc.Get<int>("effect_1"), transform,
                    actor.transform, actor.GetColliderRadius());

                //伤害对象
                if (isSkillSuccess && IsCanAttack(actor))
                    m_TargetList.Add(actor.ServerID);
            }

            int nCountActor = 0;

            if (m_curSkillDesc != null)
            {
                LuaTable skilleffect = ConfigManager.Instance.Skill.GetEffectConfig(m_curSkillDesc.Get<int>("effect_1"));
                if (skilleffect != null)
                {
                    nCountActor = skilleffect.Get<int>("max_num");
                }
            }

            for (int k = 0; k < objList.Count; ++k)
            {
                if (objList[k] == null)
                {
                    continue;
                }

                //对IOS出现怪物不动 报错的异常  进行错误处理
                if (GameLogicMgr.checkValid(objList[k].gameObject) == false)
                {
                    continue;
                }

                actor = objList[k];

                //这个判断可以去掉
                if (actor.EntityID == EntityID)
                    continue;

                //去重
                if (m_SelectTargetObject != null && actor.EntityID == m_SelectTargetObject.EntityID)
                {
                    continue;
                }

                if (!IsCanAttack(actor))
                    continue;

                //按伤害范围算出受伤害对象  
                bool isSkillSuccess = CoreEntry.gSkillMgr.IsSkillDamageRange(m_curSkillDesc.Get<int>("effect_1"), transform,
                    actor.transform, actor.GetColliderRadius());

                //伤害对象
                if (isSkillSuccess)
                {
                    // 使用离当前选中的怪物最近
                    float distance = 0;
                    if (m_SelectTargetObject != null)
                    {
                        distance = Vector3.Distance(m_SelectTargetObject.GetPosition(), actor.GetPosition());
                    }
                    else
                    {
                        distance = Vector3.Distance(GetPosition(), actor.GetPosition());
                    }
                     

                    if (!actor.IsDeath())
                    {
                        //nCountActor--;
                        if (m_TargetDir.ContainsKey(distance))
                        {
                            m_TargetDir[distance] = actor.ServerID;

                        }
                        else
                        {
                            m_TargetDir.Add(distance, actor.ServerID);
                        }

                    }
                }
            }

            Dictionary<float, long> dic_SortedByKey = m_TargetDir.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);

            foreach (KeyValuePair<float, long> item in dic_SortedByKey)
            {
                if (nCountActor <= 1)
                {
                    break;
                }

                m_TargetList.Add(item.Value);

                nCountActor--;
            }            
        }

        float lastChangeSkillTime = 0f;
        const float MinChangeInterval = 5f;

        public int CheckSkillChange(int nSrcID)
        {
            int nDestID = -1;

            //加入时间限制，避免连续放两次冲锋
            if (m_ActorType == ActorType.AT_LOCAL_PLAYER)
            {
                var diff = Time.time - lastChangeSkillTime;
                if (diff < MinChangeInterval)
                {
                    return nDestID;
                }
            }

            ActorObj selObject = GetSelTarget();
            if (selObject != null)
            {
                Vector3 aimPos = selObject.transform.position;

                //是否超过了攻击范围                    
                float aimDistance = Vector3.Distance(transform.position, aimPos);

                LuaTable desc = GetCurSkillDesc(nSrcID);

                float skillDistance = 0;
                if (desc != null)
                {
                    skillDistance = desc.Get<float>("min_dis");   //desc.min_dispk

                }

                if (aimDistance > skillDistance)
                {
                    //有变招                        
                    int changeID = m_gameDBMgr.GetChangeSkillID(nSrcID, (int)ChangeSkillType.CST_DISTANCE);
                    if (changeID != -1)
                    {
                        nDestID = changeID;
                    }
                    else
                    {
                        nDestID = -1;
                    }
                }
            }

            if (nDestID != -1)
            {
                lastChangeSkillTime = Time.time;
            }

            return nDestID;
        }

        public virtual bool IsCanAttack(ActorObj obj)
        {
            return true;
        }

        //释放技能流程
        public void OnCastSkill(int skillID)
        {
            LuaTable skillCfg = ConfigManager.Instance.Skill.GetSkillConfig(skillID);
            if (skillCfg.Get<int>("showtype") != (int)SkillShowType.ST_BEAUTYMANHETI && m_SelectTargetObject != null) //红颜合体技不需要转向
            {
                FaceTo(m_SelectTargetObject.GetPosition());

                m_move.SendDirection2Server(transform.rotation);
            }


            //能否释放技能
            if (!CanCastSkill(skillID))
            {
                return;
            }
            
            //判断是否要做变招
            int nDestID = 0;
            nDestID = CheckSkillChange(skillID);
            if (nDestID != -1)
            {
                skillID = nDestID;
            }


            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_ATTACK;
            param.skillID = skillID;
            param.isComposeSkill = false;

            //是否组合技能              
            ComposeSkillDesc composeSkillDesc = m_gameDBMgr.GetComposeSkillDesc(m_curCastSkillID, skillID);

            if (composeSkillDesc != null)
            {
                param.isComposeSkill = true;
                param.composeSkillDesc = composeSkillDesc;
            }
            else
            {
                m_curCastSkillID = skillID;
            }



            //进入攻击模式
            bool ret = RequestChangeState(param);

            LuaTable skillcfg = ConfigManager.Instance.Skill.GetSkillConfig(m_curCastSkillID);

            //法宝技能客户端先走cd add by lzp 法宝技能63
            if (skillcfg.Get<int>("showtype") == (int)SkillShowType.ST_MAGICKEY)
            {
                MsgData_sCooldown msg = new MsgData_sCooldown();
                msg.CasterID = ServerID;
                msg.SkillID = skillID;
                msg.CD = skillcfg.Get<int>("cd");
                msg.GroupCD = skillcfg.Get<int>("group_cd");
                msg.GroupID = skillcfg.Get<int>("group_cd_id");
                ModuleServer.MS.GSkillCastMgr.OnMagicCollDown(GameEvent.GE_SKILL_MAGICCOLLDOWN, EventParameter.Get(msg));
                EventParameter mkMsg = EventParameter.Get();
                mkMsg.objParameter = this;
                mkMsg.intParameter = skillID;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_UseMagicKeySkill, mkMsg);
                ret = true;
            }
            if (ret)
            {
                //这个技能可能被切换成新的连招
                LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(m_curCastSkillID);
                m_curSkillDesc = skillDesc;

                LuaTable skillClass1 = null;//1类型效果
                LuaTable skillClass2 = null;//3位移类效果
                bool hasCustomEffect = false;//是否有2类型效果，陷阱，光环和召唤类
                if (skillDesc != null)
                {
                    int nIndex1 = 0;
                    int nIndex3 = 0;
                    //判断技能效果类型
                    if (skillDesc.Get<int>("e_type_1") == (int)SkillEffectType.COMON)
                    {
                        nIndex1 = skillDesc.Get<int>("effect_1");
                    }
                    else if (skillDesc.Get<int>("e_type_1") == (int)SkillEffectType.CUSTOM)
                    {
                        hasCustomEffect = true;
                    }
                    else if (skillDesc.Get<int>("e_type_1") == (int)SkillEffectType.MOVE)
                    {
                        nIndex3 = skillDesc.Get<int>("effect_1");
                    }

                    if (skillDesc.Get<int>("e_type_2") == (int)SkillEffectType.COMON)
                    {
                        nIndex1 = skillDesc.Get<int>("effect_2");
                    }
                    else if (skillDesc.Get<int>("e_type_2") == (int)SkillEffectType.CUSTOM)
                    {
                        hasCustomEffect = true;
                    }
                    else if (skillDesc.Get<int>("e_type_2") == (int)SkillEffectType.MOVE)
                    {
                        nIndex3 = skillDesc.Get<int>("effect_2");
                    }

                    if (skillDesc.Get<int>("e_type_3") == (int)SkillEffectType.COMON)
                    {
                        nIndex1 = skillDesc.Get<int>("effect_3");
                    }
                    else if (skillDesc.Get<int>("e_type_3") == (int)SkillEffectType.CUSTOM)
                    {
                        hasCustomEffect = true;
                    }
                    if (skillDesc.Get<int>("e_type_3") == (int)SkillEffectType.MOVE)
                    {
                        nIndex3 = skillDesc.Get<int>("effect_3");
                    }

                    if (nIndex1 > 0)//普通效果
                    {
                        skillClass1 = ConfigManager.Instance.Skill.GetEffectConfig(nIndex1);
                    }

                    //冲锋
                    if (nIndex3 > 0)
                    {
                        skillClass2 = ConfigManager.Instance.Skill.GetEffectConfig(nIndex3);
                    }

                    if (nIndex1 > 0 || nIndex3 > 0)
                    {
                        ChooseSkillTarget();
                    }
                }

                long nTargetID = 0;

                //如果有冲锋
                if (skillClass2 != null)
                {
                    //冲锋
                    if (skillClass2.Get<int>("skill_eff") == 1)
                    {
                        //这个位置是移动后的位置
                        //Quaternion r0 = Quaternion.Euler(transform.eulerAngles);
                        string skillData = skillClass2.Get<string>("skill_param");
                        string[] skillDist = skillData.Split(',');

                        Vector3 pos = GetPosition() + transform.forward * System.Convert.ToInt32(skillDist[0]);

                        Vector3 destPos = CoreEntry.gBaseTool.GetLineReachablePos(transform.position, pos);

                        //使用目标的位置, 前面0.5米
                        if (m_SelectTargetObject != null)
                        {
                            float dist1 = Vector3.Distance(transform.position, m_SelectTargetObject.transform.position) - 0.5f;
                            float dist2 = Vector3.Distance(transform.position, destPos) - 0.5f;
                            if (dist1 > dist2)
                            {
                                dist1 = dist2;
                            }

                            destPos = transform.position + transform.forward * System.Convert.ToInt32(dist1);
                        }
                        
                        NetLogicGame.Instance.SendReqCastSkill(m_curCastSkillID, nTargetID, destPos.x, destPos.z, transform.position.x, transform.position.z,
                            (float)(transform.rotation.eulerAngles.y * System.Math.PI / 180.0f), m_TargetList);
                    }

                }
                else
                {

                    if (skillClass1 != null)
                    {
                        //发送目标位置
                        if (skillClass1.Get<int>("range") == (int)SkillRangeType.SKILL_TARGET || skillClass1.Get<int>("range") == (int)SkillRangeType.SKILL_TARGET_CIRCLE)
                        {
                            Vector3 vPos = transform.position;
                            if (m_SelectTargetObject != null)
                            {
                                vPos = m_SelectTargetObject.transform.position;
                                nTargetID = m_SelectTargetObject.ServerID;
                            }

                            NetLogicGame.Instance.SendReqCastSkill(m_curCastSkillID, nTargetID, vPos.x, vPos.z, transform.position.x, transform.position.z,
                                (float)(transform.rotation.eulerAngles.y * System.Math.PI / 180.0f), m_TargetList);

                        }
                        else
                        {
                            NetLogicGame.Instance.SendReqCastSkill(m_curCastSkillID, nTargetID, transform.position.x, transform.position.z,
                                transform.position.x, transform.position.z, (float)(transform.rotation.eulerAngles.y * System.Math.PI / 180.0f), m_TargetList);

                        }
                    }
                    else
                    {
                        if (hasCustomEffect)
                        {
                            NetLogicGame.Instance.SendReqCastSkill(m_curCastSkillID, nTargetID, transform.position.x, transform.position.z,
                                transform.position.x, transform.position.z, (float)(transform.rotation.eulerAngles.y * System.Math.PI / 180.0f), m_TargetList);
                        }
                    }

                }

                //if (mActorType == ActorType.AT_LOCAL_PLAYER)
                //    m_bPlayerSelect = true;

                if (composeSkillDesc != null && composeSkillDesc.isOrder)
                {
                    m_gameDBMgr.SetComposeSkillOrder(composeSkillDesc.srcSkillID,
                        composeSkillDesc.dstSkillID, composeSkillDesc.composeSkillID);
                }

            }
        }


        //
        public void OnCastSkill(int nSkill, long CasterID, long TargetID, double PosX, double PosY)
        {
            if (nSkill == 0 || CasterID != ServerID)
                return;


            ActorObj attackActor = CoreEntry.gActorMgr.GetActorByServerID(CasterID);
            ActorObj behitActor = CoreEntry.gActorMgr.GetActorByServerID(TargetID);

            m_SelectTargetObject = behitActor;

            if (behitActor != null)
            {
                FaceTo(behitActor.transform.position);
            }


            //能否释放技能
            if (mActorType == ActorType.AT_LOCAL_PLAYER || mActorType == ActorType.AT_PET)//本地玩家和幻灵才需要走CD
            {
                if (!CanCastSkill(nSkill))
                {
                    return;
                }
            }

            Vector3 vDest = new Vector3((float)PosX * 0.001f, attackActor.transform.position.y, (float)PosY * 0.001f);
            m_CurveMovePos = vDest;

            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_ATTACK;
            param.skillID = nSkill;
            param.isComposeSkill = false;

            param.AttackActor = attackActor;
            param.HitActor = behitActor;

            //  param.hitActor

            //进入攻击模式
            RequestChangeState(param);
        }

        //处理击退
        public void OnHitBack(ActorObj attack, int MotionSpeed, double PosX, double PosY, int MotionTime)
        {
            //float backSpeed = MotionSpeed;
            Vector3 hitbackPos = new Vector3((float)PosX, transform.position.y, (float)PosY);
            Vector3 destPos = CoreEntry.gBaseTool.GetLineReachablePos(transform.position, hitbackPos);

            hitbackPos = destPos; //CoreEntry.gBaseTool.GetGroundPoint(destPos);
            Vector3 hitDir = hitbackPos - transform.position;
            hitDir.Normalize();

            float HitBackDistance = Vector3.Distance(transform.position, hitbackPos);

            StateParameter stateParam = new StateParameter();
            stateParam.state = ACTOR_STATE.AS_BEHIT;
            stateParam.IsHitBack = true;
            stateParam.AttackActor = attack;

            ////能否切换到被击状态                    
            RequestChangeState(stateParam);
            m_BehitState.OnBehitMove(HitBackDistance, MotionTime, hitbackPos, hitDir, MotionSpeed);
        }



        //技能模版的返回
        public void SkillEnd(int skillID)
        {
            //技能结束，待机
            m_IActorState.Reset();

            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_STAND;
            RequestChangeState(param);
        }

        void RemoveShader()
        {
            CancelInvoke("RemoveShader");
            RemoveShader("YY/VNContour");
        }


        void ResetBehit()
        {
            CancelInvoke("ResetBehit");
            bUserBehit = true;

        }
        
        //受到技能攻击
        public void OnSkillBeHit(BehitParam behitParam)
        {
            if (behitParam == null) return;
            //yy edit
            bool bIsMainPlayer = false;

            if (behitParam.damgageInfo.behitActor != null
                && behitParam.damgageInfo.behitActor.mActorType == ActorType.AT_LOCAL_PLAYER)
            {
                bIsMainPlayer = true;
            }

            ////伤害这里计算
            if (!OnDamage(behitParam.hp, behitParam.damgageInfo.skillID, bIsMainPlayer, behitParam))
            {
                return;
            }

            // 无敌状态，减益技能无效
            LuaTable skillDesc = GetCurSkillDesc(behitParam.damgageInfo.skillID);

            if (m_isRecoverHealth && skillDesc != null)
            {
                if (skillDesc.Get<int>("faction_limit") == (sbyte)SkillFactionType.FACTION_ENEMY)
                    return;
            }

            //被击特效
            if (skillDesc != null)
                OnPlayBehitEfx(behitParam.damgageInfo.skillID);


            // 技能目标是敌人是才播放受击动作
            if (skillDesc != null && skillDesc.Get<int>("faction_limit") == (byte)SkillFactionType.FACTION_ENEMY)
            {
                //塔只播放声音
                MonsterObj monsterObj = this.gameObject.GetComponent<MonsterObj>();


                if (m_bIsTower && m_BehitState)
                {
                    if (behitParam != null)
                    {
                        m_BehitState.PlayBehitSound(this, behitParam.damgageInfo.attackActor, skillDesc);
                    }
                    return;
                }


                //如果是BOSS，直接播受击的效果
                if (mActorType == ActorType.AT_BOSS && monsterObj && !monsterObj.m_bNoUseBehit && AddShader("YY/VNContour"))
                {
                    if (bUserBehit)
                    {
                        bUserBehit = false;
                    }
                    else
                    {
                        Invoke("ResetBehit", 2f);
                    }

                    Invoke("RemoveShader", 0.3f);

                }



                //被击参数
                if (m_behitParam != null)
                {
                    m_behitParam.clean();
                }   
                m_behitParam = behitParam;

                StateParameter stateParam = new StateParameter();
                stateParam.state = ACTOR_STATE.AS_BEHIT;
                stateParam.skillID = behitParam.damgageInfo.skillID;
                stateParam.AttackActor = behitParam.damgageInfo.attackActor;

                //能否切换到被击状态                    
                bool rel = RequestChangeState(stateParam);
                if (!rel && m_BehitState != null)
                {
                    if (behitParam != null)
                    {
                        m_BehitState.PlayBehitSound(this, behitParam.damgageInfo.attackActor, skillDesc);
                    }
                }
            }
            return;
        }

        //吸血
        public void DoSuck(BehitParam behitParam)
        {
            //yy edit
            bool bIsMainPlayer = IsMainPlayer();

            //伤害这里计算
            OnDamage(behitParam.hp, behitParam.damgageInfo.skillID, bIsMainPlayer, behitParam);

        }

        //反伤
        public void DoBonce(BehitParam behitParam)
        {
            //yy edit
            bool bIsMainPlayer = IsMainPlayer();

            //伤害这里计算
            OnDamage(behitParam.hp, behitParam.damgageInfo.skillID, bIsMainPlayer, behitParam);

        }

        //眩晕状态请求
        public void OnEnterDizzy(DizzyParam param)
        {
            m_dizzyParam = param;

            StateParameter stateParam = new StateParameter();
            stateParam.state = ACTOR_STATE.AS_DIZZY;

            m_IActorState.Reset();

            //能否切换到被击状态                    
            RequestChangeState(stateParam);

        }


        //恐惧状态请求
        public void OnEnterFear(FearParam param)
        {
            m_FearParam = param;
            if (m_FearParam.keepTime == 0) // 
                return;

            if (m_bIsTower)
            {
                return;
            }

            StateParameter stateParam = new StateParameter();
            stateParam.state = ACTOR_STATE.AS_FEAR;
            m_IActorState.Reset();
            SetTimerStateParam(stateParam, param);

            //能否切换到被击状态                    
            RequestChangeState(stateParam);
        }



        //缴械请求
        public void OnEnterDisarm(DisarmParam param)
        {
            m_bDisarmState = true;

            CancelInvoke("DisarmExitState");
            Invoke("DisarmExitState", param.keepTime);
        }


        public void DisarmExitState()
        {
            m_bDisarmState = false;
        }

        public int m_OldTeamType = 0;

        //魅惑状态请求  直接改变玩家的阵营
        public void OnEnterCharm(CharmParam param)
        {
            if (hitActorObject == null || mActorType == ActorType.AT_BOSS)
            {
                return;
            }

            if (m_bIsTower)
            {
                return;
            }

            int nTeamType = hitActorObject.GetComponent<ActorObj>().m_TeamType;
            m_OldTeamType = mActorBase.m_TeamType;
            m_bCharmState = true;

            //玩家
            if (mActorBase.m_TeamType == 1)
            {
                if (nTeamType == 3)
                    mActorBase.m_TeamType = 3;
            }

            //NPC
            if (mActorBase.m_TeamType == 2)
            {
                if (nTeamType == 1)
                    mActorBase.m_TeamType = 3;
                else
                    mActorBase.m_TeamType = 1;
            }

            //monster
            if (mActorBase.m_TeamType == 3)
            {
                if (nTeamType == 1)
                    mActorBase.m_TeamType = 1;
            }

            List<ActorObj> actorList = CoreEntry.gActorMgr.GetAllMonsterActors();
            ActorObj actorObject = null;

            for (int i = 0; i < actorList.Count; ++i)
            {
                //对IOS出现怪物不动 报错的异常  进行错误处理
                if (GameLogicMgr.checkValid(actorList[i].gameObject) == false)
                {
                    continue;
                }
                actorObject = actorList[i];
                if (actorObject.IsDeath())
                {
                    continue;
                }
            }

            CancelInvoke("CharmExitState");
            Invoke("CharmExitState", param.keepTime);

        }

        public void CharmExitState()
        {
            mActorBase.m_TeamType = m_OldTeamType;
            m_bCharmState = false;
        }
        
        //眩晕状态请求
        public void OnEnterLimit(LimitParam param)
        {
            m_limitParam = param;
            if (m_limitParam.keepTime == 0) // 这里只是清空Limit状态
                return;

            if (curActorState == ACTOR_STATE.AS_LIMIT)
            {
                return;
            }

            ACTOR_STATE oldActorState = curActorState;
            int iOldState = limitParam.iState;
            float floatingTime = m_LimitState.floatingTime;

            StateParameter stateParam = new StateParameter();
            stateParam.state = ACTOR_STATE.AS_LIMIT;

            m_IActorState.Reset();

            SetTimerStateParam(stateParam, param);
            //能否切换到被击状态                    
            RequestChangeState(stateParam);

            if (oldActorState == ACTOR_STATE.AS_LIMIT && (iOldState == (int)limitType.Floating))
            {
                if (floatingTime != 0)
                {
                    m_LimitState.floatingState = 1; // 如果已经是浮空状态，直接设置 1浮空状态
                    SetPosition(m_LimitState.floatingPos[1]);
                }
            }

            return;
        }

        /// <summary>
        /// boss是否不在气绝状态吓
        /// </summary>
        /// <returns></returns>
        public bool CheckIfBossNotQiJue()
        {
            if (mActorType == ActorType.AT_BOSS && !IsInQiJue())
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 是否不是boss，或者是boss但是不在气绝状态
        /// </summary>
        /// <returns></returns>
        public bool CheckIfNotBossOrInQijue()
        {
            if (mActorType != ActorType.AT_BOSS)
            {
                return true;
            }
            else if (IsInQiJue())
            {
                return true;
            }

            return false;
        }

        //眩晕状态请求
        public void OnEnterSheep(SheepParam param)
        {
            //BOSS在非霸体状态不进变羊,但是可以进入变羊的解除状态
            if (param.iState != 0 && mActorType == ActorType.AT_BOSS && !IsInQiJue())
            {
                return;
            }

            if (m_bIsTower)
            {
                return;
            }

            // 与变形互斥,这里退出隐身状态
            if (m_bStealthState == true)
            {
                OnStealthState(false);
            }

            m_SheepParam = param;
            if (m_limitParam == null)
                m_limitParam = new LimitParam();
            if (m_SheepParam.keepTime == 0) // 清空Limit状态
            {
                m_limitParam.iState = 0; // 解除限制
                m_SheepParam.iState = 0; // 解除变形
                if (m_IActorState.m_state == ACTOR_STATE.AS_SHEEP)
                {
                    StateParameter paramStand = new StateParameter();
                    paramStand.state = ACTOR_STATE.AS_STAND;
                    RequestChangeState(paramStand);
                }
                return;
            }
            m_limitParam.iState = (int)(limitType.Attack | limitType.Skill); // 限制攻击和技能

            StateParameter stateParam = new StateParameter();
            stateParam.state = ACTOR_STATE.AS_SHEEP;
            m_IActorState.Reset();
            SetTimerStateParam(stateParam, param);

            //能否切换到指定状态                    
            RequestChangeState(stateParam);
            return;
        }

        void SetTimerStateParam(StateParameter stateParam, TimerStateParam param)
        {
            TimerState timerState = GetActorState(stateParam.state) as TimerState;
            if (timerState != null)
            {
                //设置参数
                timerState.Param = param;
            }
        }

        /// <summary>
        /// 重置Timer状态的计时器
        /// </summary>
        public void OnResetTimerState(ACTOR_STATE stateFlag)
        {
            //不是当前激活状态，不可以更新
            if (m_IActorState != null && m_IActorState.m_state == stateFlag)
            {
                TimerState state = m_IActorState as TimerState;
                if (state != null)
                {
                    state.ResetTimer();
                }
            }
        }

        // 隐身状态
        public void OnStealthState(bool bStealth, bool bAlwayHide = false)
        {
            // 与变形互斥,被变羊了不能隐身了
            if (bStealth && m_IActorState != null && m_IActorState.m_state == ACTOR_STATE.AS_SHEEP)
                return;

            m_bStealthState = bStealth;

            if (m_bStealthState)
            {
                if (mActorType != ActorType.AT_LOCAL_PLAYER || bAlwayHide)
                {
                    HideBlobShadow(); // 阴影不显示
                    if (mActorType == ActorType.AT_MONSTER)
                    {
                        //MonsterHeath heath = GetComponent<MonsterHeath>();
                        //if (heath)
                        //    heath.TopBloodHide(); // 血条不显示
                    }
                    HideSelf();
                    BuffOnStealth(true); // 不是队友时隐藏Buff特效
                }
                else
                    StealthSelf(true);
            }
            else
            {
                if (mActorType != ActorType.AT_LOCAL_PLAYER || bAlwayHide)
                {
                    if (!IsDeath())
                    {
                        ShowBlobShadow(); // 阴影显示
                        if (mActorType == ActorType.AT_MONSTER)
                        {
                            //MonsterHeath heath = GetComponent<MonsterHeath>();
                            //if (heath)
                            //    heath.TopBloodShow();   // 血条显示
                        }
                        ShowSelf();
                    }
                }
                else
                    StealthSelf(false);
            }
            if (m_bStealthState == false)
                BuffOnStealth(bStealth);
        }

        private GameObject m_BuffDuelActor;   // 决斗NPC  将只受到这个NPC的伤害

        public bool OnDamage(int hp, int skillID, bool bIsMainPlayer, BehitParam behitParam)
        {
            if (behitParam.damgageInfo == null) return false;
            if (behitParam.damgageInfo.attackActor == null || behitParam.damgageInfo.attackActor.gameObject == null)
            {
                return false;
            }

            // 攻击、治疗、死前事件 by yuxj
            EventParameter eventBuffParam = EventParameter.Get();
            eventBuffParam.autoRecycle = false;
            eventBuffParam.intParameter = skillID;
            eventBuffParam.goParameter = gameObject;
            eventBuffParam.objParameter = this;

            if (behitParam.damgageInfo != null)
            {
                eventBuffParam.goParameter1 = behitParam.damgageInfo.attackActor.gameObject;
                eventBuffParam.objParameter1 = behitParam.damgageInfo.attackActor;
            }

            if (hp >= 0)
            {
                //if(!bIsMainPlayer)
                //    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EVENT_BEHIT, eventBuffParam);
            }
            else
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EVENT_CURE, eventBuffParam);


            // 无敌状态, GE_EVENT_DIE 会修改这个标志
            if (m_isRecoverHealth && hp > 0)
                return false;

            // --------
            m_curHP -= hp;
            //HealthFrame.OnHPChange();

            if (m_curHP <= 0)
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EVENT_DIE, eventBuffParam);

            //服务器下发伤害才飘血  
            if (!behitParam.damgageInfo.IsClient)
            {
                DoDamage(hp, bIsMainPlayer, behitParam);

                //发送消息。技能模块需要
                EventParameter eventParam = EventParameter.Get();
                eventParam.intParameter = entityid;
                eventParam.intParameter1 = (int)m_curHP;
                eventParam.intParameter2 = hp;
                //受击者
                eventParam.goParameter = gameObject;
                eventParam.objParameter = this;
                //攻击者
                eventParam.goParameter1 = behitParam.damgageInfo.attackActor.gameObject;
                eventParam.objParameter1 = behitParam.damgageInfo.attackActor;
                eventParam.objParameter2 = behitParam;
                m_eventMgr.TriggerEvent(GameEvent.GE_DAMAGE_CHANGE, eventParam);

                if (behitParam.displayType == DamageDisplayType.DDT_DOUBLE)
                {
                    EventParameter doubleDmgEventParam = EventParameter.Get();
                    doubleDmgEventParam.intParameter = behitParam.damgageInfo.skillID;
                    doubleDmgEventParam.intParameter1 = (int)m_curHP;
                    doubleDmgEventParam.intParameter2 = hp;
                    doubleDmgEventParam.goParameter = behitParam.damgageInfo.attackActor.gameObject;
                    doubleDmgEventParam.objParameter = behitParam.damgageInfo.attackActor;
                    doubleDmgEventParam.goParameter1 = behitParam.damgageInfo.behitActor.gameObject;
                    doubleDmgEventParam.objParameter1 = behitParam.damgageInfo.behitActor;
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EVENT_DOUBLEDAMAGE, doubleDmgEventParam);
                }

                if ((IsUseServerDeath() == false) && IsDeath())
                {
                    OnDead(skillID, behitParam.damgageInfo.attackActor, behitParam, eventBuffParam);
                    return false;
                }

            }
            EventParameter.Cache(eventBuffParam);
            return true;
        }

        public override void DoDamage(int hp, bool bIsMainPlayer, BehitParam behitParam)//lmjedit 真正的暴击 
        {
        }

        public void OnDead(int skillID, ActorObj attackObj, BehitParam behitParam, EventParameter eventBuffParam)
        {

            //进入死亡状态
            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_DEATH;
            param.skillID = skillID;

            param.AttackActor = attackObj;

            if (m_BehitState != null && behitParam != null && behitParam.damgageInfo != null && behitParam.damgageInfo.attackActor != null)
            {
                LuaTable desc = ConfigManager.Instance.Skill.GetSkillConfig(skillID);
                m_BehitState.PlayBehitSound(this, behitParam.damgageInfo.attackActor, desc);
            }
            if (RequestChangeState(param))
            {
                if (behitParam != null && behitParam.damgageInfo != null)
                {
                    // 死亡时面向攻击者,这样被击飞时方向不会错乱
                    if (behitParam.damgageInfo.attackActor != null)
                    {
                        FaceTo(behitParam.damgageInfo.attackActor.transform.position);
                        eventBuffParam.goParameter = behitParam.damgageInfo.attackActor.gameObject;
                    }
                    else
                    {
                        eventBuffParam.goParameter = null;
                    }

                    // 击杀事件
                    eventBuffParam.intParameter = skillID;
                    eventBuffParam.objParameter = this;
                    eventBuffParam.goParameter1 = behitParam.damgageInfo.behitActor.gameObject;
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EVENT_KILL, eventBuffParam);
                }

                ///延迟删除物体，防止攻击顺序出错
                Invoke("DelayRemoveObj", 0.01f);
            }
            if (IsRiding)
            {
                GetDownHorse();
            }
            if (IsWing && mWing != null)
            {
                GetComponent<Animation>().Play(StateParameter.ANI_NAME_DIE);
                mWing.GetComponent<Animation>().Stop();
            }
            if (behitParam != null)
            {
                behitParam.clean();
                behitParam = null;
            }
        }

        void DelayRemoveObj()
        {
            m_gameMgr.RemoveObj(this.gameObject, this);
        }

        public GameObject GetOwnObject()
        {
            return this.gameObject;
        }


        public void SetBrightenColor()
        {
            if (m_skinnedMeshRenderer.Length > 0)
            {
                for (int i = 0; i < m_skinnedMeshRenderer.Length; i++)
                {
                    for (int j = 0; j < m_skinnedMeshRenderer[i].materials.Length; ++j)
                    {
                        Color bColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
                        m_skinnedMeshRenderer[i].materials[j].SetColor("_fBrightenColor", bColor);
                    }

                }
            }
        }

        public void CancleBrightenColor()
        {
            if (m_skinnedMeshRenderer.Length > 0)
            {
                for (int i = 0; i < m_skinnedMeshRenderer.Length; i++)
                {
                    for (int j = 0; j < m_skinnedMeshRenderer[i].materials.Length; ++j)
                    {
                        Color bColor = new Color(0.0f, 0.0f, 0.0f, 0.5f);
                        m_skinnedMeshRenderer[i].materials[j].SetColor("fRimLightRampColor", bColor);
                        cacheBehitBrightenMaterial.SetFloat("fRimLightScale", 1); 
                    }

                }
            }
        }

        private static Material BehitBrightenMaterial
        {
            get
            {
                if (cacheBehitBrightenMaterial == null)
                {
                    Color bColor = new Color(1.0f, 0.847f, 0f, 1.0f);                    
                    cacheBehitBrightenMaterial = new Material(CoreEntry.gResLoader.LoadShader("DZSMobile/Character"));
                    cacheBehitBrightenMaterial.SetColor("fRimLightRampColor", bColor);
                    cacheBehitBrightenMaterial.SetFloat("fRimLightScale", 20); 
                    
                }
                return cacheBehitBrightenMaterial;
            }
        }

        private static Material cacheBehitBrightenMaterial;

        private bool mIsBehitBrighten = false;
        private Material mDefaultMaterial;


        public void SetBrightenShader()
        {
            if (m_skinnedMeshRenderer.Length == 1 && !mIsBehitBrighten)
            {
                mIsBehitBrighten = true;
                if (mDefaultMaterial == null)
                {
                    mDefaultMaterial = m_skinnedMeshRenderer[0].material;
                }
                
                Texture texture = mDefaultMaterial.mainTexture;
                Material m = BehitBrightenMaterial;
                m.SetTexture("_MainTex", texture);
                m_skinnedMeshRenderer[0].sharedMaterial = m;
            }
        }

        public void CancelBrightenShader()
        {
            if (m_skinnedMeshRenderer.Length == 1 && mIsBehitBrighten)
            {
                mIsBehitBrighten = false;
                m_skinnedMeshRenderer[0].sharedMaterial = mDefaultMaterial;
            }
        }

        public void RequestChangeState(ACTOR_STATE stateID)
        {
            if (GetCurState() != stateID)
            {
                StateParameter param = new StateParameter();
                param.state = stateID;
                RequestChangeState(param);
            }
        }

        public bool ChangeShader(string shaderPath)
        {
            Shader behitShade = CoreEntry.gResLoader.LoadShader(shaderPath);
            if (behitShade == null)
            {
                LogMgr.UnityLog("can't find shader: " + shaderPath);
                return false;
            }

            if (m_skinnedMeshRenderer == null)
            {
                return false;
            }

            if (m_skinnedMeshRenderer.Length > 0)
            {
                for (int i = 0; i < m_skinnedMeshRenderer.Length; i++)
                {
                    for (int j = 0; j < m_skinnedMeshRenderer[i].materials.Length; ++j)
                    {
                        m_skinnedMeshRenderer[i].materials[j].shader = behitShade;
                        m_skinnedMeshRenderer[i].materials[j].color = new Color(1.0f, 1.0f, 1.0f);
                    }
                }
            }

            return true;
        }

        public bool AddShader(string shaderPath)
        {
            Shader behitShade = CoreEntry.gResLoader.LoadShader(shaderPath);
            if (behitShade == null)
            {
                LogMgr.UnityLog("can't find shader: " + shaderPath);
                return false;
            }

            if (m_skinnedMeshRenderer == null)
            {
                return false;
            }

            if (m_skinnedMeshRenderer.Length > 0)
            {
                for (int i = 0; i < m_skinnedMeshRenderer.Length; i++)
                {
                    if (m_skinnedMeshRenderer[i].materials.Length > 1)
                    {
                        m_skinnedMeshRenderer[i].materials[0].shader = behitShade;
                        m_skinnedMeshRenderer[i].materials[0].color = new Color(1.0f, 0.3f, 0.3f);
                    }
                    else
                    {
                        m_skinnedMeshRenderer[i].materials[0] = new Material(behitShade);
                        m_skinnedMeshRenderer[i].materials[0].color = new Color(1.0f, 0.3f, 0.3f);
                    }
                }
            }

            return true;
        }


        public bool RemoveShader(string shaderPath)
        {
            Shader behitShade = CoreEntry.gResLoader.LoadShader(shaderPath);
            if (behitShade == null)
            {
                LogMgr.UnityLog("can't find shader: " + shaderPath);
                return false;
            }

            if (m_skinnedMeshRenderer == null)
            {
                return false;
            }

            if (m_skinnedMeshRenderer.Length > 0)
            {
                for (int i = 0; i < m_skinnedMeshRenderer.Length; i++)
                {
                    if (m_skinnedMeshRenderer[i].materials.Length > 1)
                    {
                        m_skinnedMeshRenderer[i].materials[0].shader = null;
                        m_skinnedMeshRenderer[i].materials[0].hideFlags = HideFlags.HideAndDontSave;
                    }
                }
            }
            return true;
        }

        //public void RecoverShader()
        //{
        //    CancelInvoke("RecoverShader");
        //    if (m_skinnedMeshRenderer.Length > 0)
        //    {
        //        for (int i = 0; i < m_skinnedMeshRenderer.Length; i++)
        //        {
        //            for (int j = 0; j < m_skinnedMeshRenderer[i].materials.Length; ++j)
        //            {
        //                m_skinnedMeshRenderer[i].materials[j].shader = m_oldShaderList[i, j];

        //            }

        //        }
        //    }

        //    // 通知buff有人修改了材质
        //    EventParameter eventBuffParam = EventParameter.Get();
        //    eventBuffParam.goParameter = gameObject;
        //    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EVENT_SHADER, eventBuffParam);
        //}

        // 保持当前材质不恢复了
        //public void NoRecoverShader()
        //{
        //    if (m_skinnedMeshRenderer.Length > 0)
        //    {
        //        for (int i = 0; i < m_skinnedMeshRenderer.Length; i++)
        //        {
        //            for (int j = 0; j < m_skinnedMeshRenderer[i].materials.Length; ++j)
        //            {
        //                m_oldShaderList[i, j] = m_skinnedMeshRenderer[i].materials[j].shader;
        //            }

        //        }
        //    }
        //}

        //被击特效    
        void OnPlayBehitEfx(int skillID)
        {
            do
            {
                Transform attachObj = GetBehitEfxTransform(skillID);

                if (attachObj == null)
                {
                    LogMgr.LogError("can't find behit efx attach transform");
                    break;
                }

                //加载对应的动作特效            
                string behitefxPrefab = "";
                LuaTable desc = ConfigManager.Instance.Skill.GetSkillConfig(skillID);
                if (desc == null) return;

                behitefxPrefab = (desc.Get<string>("pfx_hurt"));

                if (behitefxPrefab.Length <= 0)
                {
                    // 非友方时才使用默认受击效果
                    if (desc.Get<int>("faction_limit") == (byte)SkillFactionType.FACTION_FRIEND)
                        behitefxPrefab = "Effect/skill/hurt/fx_beiji03";//m_actorCreatureDisplayDesc.behitEfxPrefab;
                }

                if (behitefxPrefab.Length > 0)
                {
                    string[] behitEfts = behitefxPrefab.Split(';');
                    for (int j = 0; j < behitEfts.Length; j++)
                    {
                        GameObject efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(behitEfts[j]);

                        if (efxObj == null)
                        {
#if UNITY_EDITOR
                            LogMgr.LogError("找不到behitefxPrefab：" + behitEfts[j] + " 技能：" + desc.Get<string>("name"));
#endif

                            continue;
                        }

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

                        if (maxEfxTime <= 0.001)
                        {
                            maxEfxTime = 1.5f;
                        }

                        SceneEfxPool efx = efxObj.GetComponent<SceneEfxPool>();
                        if (efx == null)
                            efx = efxObj.AddComponent<SceneEfxPool>();

                        //bool isFollowMove = true;
                        if (desc.Get<int>("behitEfxPos") == (int)BehitEfxPos.GROUND)
                        {
                            attachObj = gameObject.transform;
                            //isFollowMove = false;
                        }

                        efxObj.transform.parent = attachObj;
                        efxObj.transform.localScale = Vector3.one;
                        efxObj.transform.eulerAngles = new Vector3(attachObj.transform.eulerAngles.x + Random.Range(180f, 360f), attachObj.transform.eulerAngles.y + Random.Range(0, 360f), attachObj.transform.eulerAngles.z + Random.Range(0, 360f));
                        efx.Init(attachObj.transform.position, maxEfxTime);
                        //efx.Offset = new Vector3(0, 0.2f, 0);
                    }
                }
            } while (false);

            //底下残留血
            if (mActorType == ActorType.AT_MONSTER || mActorType == ActorType.AT_BOSS)
            {
                Random.seed = System.Guid.NewGuid().GetHashCode();
                int index = Random.Range(1, 6);
                
                GameObject obj1 = CoreEntry.gGameObjPoolMgr.InstantiateEffect(@"Effect/skill/hurt/fodder_blood0" + index);
                if (obj1 != null)
                {
                    SceneEfxPool efx1 = obj1.GetComponent<SceneEfxPool>();
                    if (efx1 == null)
                        efx1 = obj1.AddComponent<SceneEfxPool>();

                    Vector3 groundPos = BaseTool.instance.GetGroundPoint(transform.position);

                    efx1.Init(groundPos, 1.5f);
                }
            }
        }

        //死亡动作使用物理
        void InitRigidbody()
        {
            //所有的刚体效果关闭        
            Rigidbody[] bodys = this.gameObject.GetComponentsInChildren<Rigidbody>();
            if (bodys.Length == 0)
            {
                return;
            }

            m_isHaveRigidbody = true;
            for (int i = 0; i < bodys.Length; ++i)
            {
                bodys[i].isKinematic = true;
                bodys[i].useGravity = false;
                bodys[i].Sleep();
            }

            //找到跟骨骼
            m_rootRigidbody = bodys[0];
        }

        //死亡动画
        public void PlayRigidbodyDeath(float force)
        {
        }

        //死亡动画
        public void PlayRigidbodyDeath(float upForce, float backForce, float rightForce)
        {
            Rigidbody[] bodys = this.gameObject.GetComponentsInChildren<Rigidbody>();
            if (bodys.Length == 0)
            {
                return;
            }

            for (int i = 0; i < bodys.Length; ++i)
            {
                bodys[i].isKinematic = false;
                bodys[i].useGravity = true;
                bodys[i].WakeUp();

                bodys[i].AddForce(Vector3.up * upForce);
                bodys[i].AddForce(-transform.forward * backForce);
                bodys[i].AddForce(transform.right * rightForce);
            }
        }
        
        //设置动画曲线,使用保存的数据
        public void UseCurveData1(string strClipName, float hitBackDistance, bool bHitBack = false)
        {
            Vector3 dstPos = transform.position + (-transform.forward) * hitBackDistance;

            if (curActorState == ACTOR_STATE.AS_BEHIT && behitState.AttackerActor != null)
            {
                dstPos = transform.position + behitState.AttackerActor.transform.forward * hitBackDistance;
            }

            //是否在地下
            if (m_baseTool.IsGroundDown(dstPos))
            {
                dstPos = m_baseTool.GetGroundPoint(dstPos);
            }

            m_animationCurveBase.MoveToPos(strClipName, dstPos, bHitBack);
        }
        
        //设置动画曲线,使用保存的数据 
        public int UseCurveData2(string strClipName, float hitBackDistance, CurveMoveParam param)
        {
            if (!IsCanCurveMove())
            {
                return -1;
            }

            Vector3 dstPos = transform.position + (-transform.forward) * hitBackDistance;

            //是否在地下
            if (m_baseTool.IsGroundDown(dstPos))
            {
                dstPos = m_baseTool.GetGroundPoint(dstPos);
            }

            int uuid = ++m_useCurveMoveUUID;

            m_animationCurveBase.StartMove(strClipName, dstPos, param, uuid);

            return uuid;
        }


        //设置动画曲线,使用保存的数据 
        public int UseCurveData3(string strClipName, Vector3 dstPos, CurveMoveParam param = null)
        {
            if (!IsCanCurveMove())
            {
                return -1;
            }

            //使用默认点
            if (dstPos == Vector3.zero)
            {
                Vector3 srcEndPos = m_animationCurveBase.GetCurveMoveEndPosition(strClipName);

                if (srcEndPos == Vector3.zero)
                {
#if UNITY_EDITOR
                    LogMgr.LogError("UseCurveData() error!");
#endif
                    return -1;
                }

                //获取地面点
                Vector3 dstEndPos = BaseTool.instance.GetGroundPoint(srcEndPos);
                if (dstEndPos == Vector3.zero)
                {
                    dstEndPos = srcEndPos;
                }

                dstPos = dstEndPos;
            }

            int uuid = ++m_useCurveMoveUUID;            
            m_animationCurveBase.StartMove(strClipName, dstPos, param, uuid);
            return uuid;
        }

        public void UseCurveData(string strClipName, Vector3 dstPos)
        {
            m_animationCurveBase.MoveToPos(strClipName, dstPos);
        }


        public void ExitAnimationCurveMove(int uuid)
        {
            if (m_animationCurveBase == null)
            {
                return;
            }

            m_animationCurveBase.SetExit(uuid);
        }

        public void ExitAnimationCurveMove()
        {
            if (m_animationCurveBase == null)
            {
                return;
            }

            m_animationCurveBase.SetExit();
        }


        public float GetAnimationCurveLength(string strClipName)
        {
            Vector3 srcEndPos = m_animationCurveBase.GetCurveMoveEndPosition(strClipName);
            if (srcEndPos == Vector3.zero)
            {
                LogMgr.LogError("UseCurveData() error!");
                return 0;
            }

            return Vector3.Distance(srcEndPos, transform.position);
        }



        //能否释放技能        
        public bool ComposeSkillCanCastSkill(float changeTime)
        {
            //当前技能进行的时间            
            string curAction = GetCurPlayAction();
            if (curAction == null || curAction == string.Empty) return false;
            if (!curAction.Contains("attack"))
            {
                return false;
            }


            float curTime = GetCurActionTime(curAction);
            if (curTime >= changeTime)
            {
                return true;
            }

            return false;
        }

        //复活保护,免疫一切伤害
        public void RecoverHealth(float protectTime)
        {
            m_isRecoverHealth = true;
            CancelInvoke("ResetRecoverHealth");
            Invoke("ResetRecoverHealth", protectTime);
        }

        public void ResetRecoverHealth()
        {
            CancelInvoke("ResetRecoverHealth");
            m_isRecoverHealth = false;
        }

        //事件处理
        void RegisterEvent()
        {
            //怪物,boss,npc的
            m_eventMgr.AddListener(GameEvent.GE_ACTOR_MOVE_BEGIN, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_ACTOR_MOVE_STOP, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_ACTOR_MOVE_END, EventFunction);

            //主角的
            m_eventMgr.AddListener(GameEvent.GE_PLAYER_MOVE_BEGIN, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_PLAYER_MOVE_STOP, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_PLAYER_MOVE_END, EventFunction);

            m_eventMgr.AddListener(GameEvent.GE_PLAYER_DEATH, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_MONSTER_DEATH, EventFunction);
        }

        void UnRegisterEvent()
        {
            m_eventMgr.RemoveListener(GameEvent.GE_ACTOR_MOVE_BEGIN, EventFunction);
            m_eventMgr.RemoveListener(GameEvent.GE_ACTOR_MOVE_STOP, EventFunction);
            m_eventMgr.RemoveListener(GameEvent.GE_ACTOR_MOVE_END, EventFunction);

            m_eventMgr.RemoveListener(GameEvent.GE_PLAYER_MOVE_BEGIN, EventFunction);
            m_eventMgr.RemoveListener(GameEvent.GE_PLAYER_MOVE_STOP, EventFunction);
            m_eventMgr.RemoveListener(GameEvent.GE_PLAYER_MOVE_END, EventFunction);

            m_eventMgr.RemoveListener(GameEvent.GE_PLAYER_DEATH, EventFunction);
            m_eventMgr.RemoveListener(GameEvent.GE_MONSTER_DEATH, EventFunction);
        }

        void EventFunction(GameEvent ge, EventParameter parameter)
        {
            switch (ge)
            {
                case GameEvent.GE_ACTOR_MOVE_BEGIN:
                    {
                        //开始移动  
                        if (parameter.goParameter != this.gameObject)
                        {
                            return;
                        }
                    }
                    break;

                case GameEvent.GE_PLAYER_MOVE_BEGIN:
                    {
                        //clear nav info
                        if (mActorType == ActorType.AT_MONSTER || mActorType == ActorType.AT_MECHANICS)
                        {
                            NavIndex = -1;
                        }

                        //开始移动  
                        if (parameter.goParameter != this.gameObject)
                        {
                            return;
                        }

                    }
                    break;
                case GameEvent.GE_ACTOR_MOVE_STOP:
                case GameEvent.GE_ACTOR_MOVE_END:
                case GameEvent.GE_PLAYER_MOVE_STOP:
                case GameEvent.GE_PLAYER_MOVE_END:
                    {
                        //移动到目的地
                        if (parameter.goParameter != this.gameObject)
                        {
                            return;
                        }

                        if (curActorState == ACTOR_STATE.AS_RUN_TO_ATTACK)
                        {
                            OnRunToAttack(m_RunToAttackState.m_curSkillID);
                            break;
                        }

                        StateParameter param = new StateParameter();
                        param.state = ACTOR_STATE.AS_STAND;
                        RequestChangeState(param);
                    }
                    break;

                case GameEvent.GE_PLAYER_DEATH:
                    {
                        //
                        ActorObj actor = parameter.goParameter.GetComponent<ActorObj>();

                        ActorObj AttackActor = GetAttackObj();
                        if (AttackActor && (actor.entityid == AttackActor.entityid))
                        {
                            NavIndex = -1;

                            //  RemoveAttackObj(parameter.intParameter);
                        }

                        if (m_BuffDuelActor != null)
                        {
                            if (parameter.goParameter == m_BuffDuelActor)
                                m_BuffDuelActor = null;
                        }


                        if (parameter.intParameter != entityid)
                        {
                            break;
                        }


                        //自己死亡
                        if (m_WarningefxObj)
                            Destroy(m_WarningefxObj);

                    }
                    break;

                case GameEvent.GE_MONSTER_DEATH:
                    {
                        //如果怪物是攻击对象，怪物死亡
                        ActorObj actorObj = GetAttackObj();
                        if (actorObj != null && actorObj.entityid == parameter.intParameter)
                        {
                            NavIndex = -1;
                        }


                        if (parameter.intParameter != entityid)
                        {
                            break;
                        }

                        if (m_BuffDuelActor != null)
                        {
                            if (parameter.goParameter == m_BuffDuelActor)
                                m_BuffDuelActor = null;
                        }

                        //自己死亡
                        if (m_WarningefxObj)
                            Destroy(m_WarningefxObj);


                    }
                    break;
                default:
                    break;
            }
        }

        //获取当前技能信息    
        public LuaTable GetCurSkillDesc(int skillID)
        {
            if (skillID <= 0)
            {
                return null;
            }
            return ConfigManager.Instance.Skill.GetSkillConfig(skillID);
        }

        public LuaTable GetCurSkillDesc()
        {
            return ConfigManager.Instance.Skill.GetSkillConfig(m_curCastSkillID);
        }
        
        //是否学习了该技能
        public bool IsHadLearnSkill(int skillID)
        {
            return true;
        }

        //获取技能攻击距离
        public float GetSkillAttackDist(int skillID, ActorObj target)
        {
            float targetRadius = 0f;

            if (target != null && target != null)
            {
                //先取一半的碰撞半径
                targetRadius = 0.5f;
            }

            LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(skillID);
            if (skillDesc == null)
            {
                return 0;
            }

            return skillDesc.Get<int>("min_dis") + targetRadius;

        }

        //方向移动，还是点移动
        public bool CanMove()
        {
            return m_IActorState.CanMove();
        }

        private void OnAttachAnimation(int resid, string strAction, bool isCrossFade)
        {
        }

        private Transform GetAttachAnimHangPoint(string strHangPoint)
        {
            Transform hangPoint = null;
            if (!m_mapAttachHangPoint.TryGetValue(strHangPoint, out hangPoint))
            {
                //没有挂点缓存，获取下    
                Transform[] childTransform = this.gameObject.GetComponentsInChildren<Transform>();
                for (int i = 0; i < childTransform.Length; ++i)
                {
                    if (childTransform[i].name.Equals(strHangPoint))
                    {
                        hangPoint = childTransform[i];
                        m_mapAttachHangPoint.Add(strHangPoint, hangPoint);
                        break;
                    }
                }
            }

            return hangPoint;
        }

        //旋转角度
        public void TurnAround(float angle)
        {
            transform.localEulerAngles += new Vector3(0, angle, 0);
        }

        public void TurnAround(Vector3 dir)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
        
        public ActorObj GetAttackObj(bool bChangeTarget = false)
        {
            if (ArenaMgr.Instance.IsArenaFight)
            {
                return CoreEntry.gActorMgr.GetPlayerActorByServerID(ArenaMgr.Instance.GetAttackID(serverID));
            }
            return m_SelectTargetObject;
        }


        //对象面向某一点
        public void FaceTo(Vector3 facePos)
        {
            // yuxj 眩晕时不可以转向
            if (m_IActorState != null && m_IActorState.m_state == ACTOR_STATE.AS_DIZZY)
                return;

            //攻击硬直状态把转向
            if (m_IActorState != null && m_IActorState.m_state == ACTOR_STATE.AS_ATTACK && m_IActorState.IsEndure())
                return;

            // 死亡不转向
            if (IsDeath())
                return;

            if (UpperPart != null)
            {
                //如果是上下分部结构
                Vector3 lookRot = facePos - transform.position;
                lookRot.y = 0;
                if (lookRot == Vector3.zero)
                {
                    return;
                }

                UpperPart.rotation = Quaternion.LookRotation(lookRot, Vector3.up);
            }
            else
            {
                //如果是普通结构
                Vector3 lookRot = facePos - transform.position;
                lookRot.y = 0;
                if (lookRot == Vector3.zero)
                {
                    return;
                }

                transform.rotation = Quaternion.LookRotation(lookRot);

            }
        }

        public bool IsInEndure()
        {
            if (GetCurState() == ACTOR_STATE.AS_ATTACK)
            {
                if (m_AttackState != null)
                {
                    return m_AttackState.IsEndure();
                }

            }
            return false;

        }

        //能否释放技能
        public bool CanCastSkill(int skillID)
        {
            if (IsInCoolDownTime(skillID))
            {
                return false;
            }

            LuaTable desc = GetCurSkillDesc(skillID);
            if (desc == null)
            {
                LogMgr.UnityError("技能配置错误! SkillDesc.xlsx 找不到技能 skillid=" + skillID);
                return false;
            }

            //判断是否在硬直状态
            //法宝技能不用判断硬直时间 
            if (IsInEndure() && desc.Get<int>("showtype") != (int)SkillShowType.ST_MAGICKEY)
            {
                //LogMgr.UnityLog("Endure:" + skillID);
                return false;
            }

            //缴械状态不能使用普通攻击
            if (m_bDisarmState && desc.Get<int>("showtype") == 2)
            {
                return false;
            }


            return true;
        }

        //公共技能CD组
        public bool IsInShareCDTime(int nSkillID)
        {
            //m_SkillShareCDMap

            LuaTable skillDesc = GetCurSkillDesc(nSkillID);

            if (skillDesc == null)
            {
                return false;
            }
            
            if (!m_SkillShareCDMap.ContainsKey(skillDesc.Get<int>("group_cd_id")))
            {
                return false;
            }
            
            ShareCDData data = m_SkillShareCDMap[skillDesc.Get<int>("group_cd_id")];
            float lastTime = data.fSkillTime;
            float curTime = Time.time;


            if ((curTime - lastTime) * 1000 <= data.cdTime)
            {
                return true;
            }

            return false;

        }

        public int GetSkillCDTime(int nSkillID)
        {
            int nCD = 0;
            LuaTable skillDesc = GetCurSkillDesc(nSkillID);
            if (skillDesc != null)
            {
                nCD = GetSkillCDTime(skillDesc);

            }
            return nCD;
        }

        public int GetSkillCDTime(LuaTable skillDesc)
        {
            return (int)(skillDesc.Get<int>("cd") * SkillCDScale);
        }

        //当前距CD时间结束还有多久
        public float QueryShareCDOverTime(int skillID)
        {
            LuaTable skillDesc = GetCurSkillDesc(skillID);
            if (skillDesc == null)
                return 0f;

            if (!m_SkillShareCDMap.ContainsKey(skillDesc.Get<int>("group_cd_id")))
            {
                return 0f;
            }


            ShareCDData data = m_SkillShareCDMap[skillDesc.Get<int>("group_cd_id")];
            float lastTime = data.fSkillTime;
            float curTime = Time.time;


            float remainTime = data.cdTime - (curTime - lastTime) * 1000;
            if (remainTime < 0)
            {
                remainTime = 0f;
            }

            return remainTime;
        }

        //更新CD组，如果CD组的剩余时间为0，则删除当前CD组
        public void UpdateShareCDOverTime()
        {
            if (m_SkillShareCDMap.Count == 0)
            {
                return;
            }

            Dictionary<int, ShareCDData>.Enumerator iter = m_SkillShareCDMap.GetEnumerator();
            List<int> delList = null;
            while (iter.MoveNext())
            {
                ShareCDData data = iter.Current.Value;
                float lastTime = data.fSkillTime;
                int cdTime = data.cdTime;
                float curTime = Time.time;

                float remainTime = cdTime - (curTime - lastTime) * 1000;
                if (remainTime <= 0)
                {
                    if (delList == null)
                        delList = new List<int>();
                    delList.Add(iter.Current.Key);
                }
            }

            if (delList != null)
            {
                for (int i = 0; i < delList.Count; i++)
                {
                    m_SkillShareCDMap.Remove(delList[i]);
                }
            }
        }
        
        public void SetShareCoolDownTime(int skillID, int GroupId, int GroupCD)
        {
            float curTime = Time.time;

            //查询技能组
            LuaTable skillDesc = GetCurSkillDesc(skillID);
            if (skillDesc == null) return;
            ShareCDData data = new ShareCDData();
            data.fSkillTime = curTime;
            data.nSkillID = skillID;
            data.cdTime = GroupCD;

            if (skillDesc.Get<int>("group_cd_id") != GroupId)
            {
                LogMgr.Log(string.Format("服务器技能cd组与客户端cd组不一致skillid:{0} 服务器：groupId:{1}  客户端groupid:{2}", skillID, GroupId, skillDesc.Get<int>("group_cd_id")));
            }

            if (m_SkillShareCDMap.ContainsKey(skillDesc.Get<int>("group_cd_id")))
            {
                m_SkillShareCDMap[skillDesc.Get<int>("group_cd_id")] = data;
                return;
            }

            m_SkillShareCDMap.Add(skillDesc.Get<int>("group_cd_id"), data);
        }

        public float GetShareSkillCDBegin(int skillId)
        {
            float value = 0;
            LuaTable skillDesc = GetCurSkillDesc(skillId);
            if (skillDesc == null) return 0f;
            ShareCDData data;
            if (!m_SkillShareCDMap.TryGetValue(skillDesc.Get<int>("group_cd_id"), out data))
            {
                return 0f;
            }
            value = data.fSkillTime;

            return value;
        }
        public int GetShareSkillCDTime(LuaTable skillDesc)
        {
            int value = 0;
            ShareCDData data;
            if (!m_SkillShareCDMap.TryGetValue(skillDesc.Get<int>("group_cd_id"), out data))
            {
                return value;
            }
            value = data.cdTime;
            return value;
        }

        //清理所有技能CD  
        public void ClearAllSkillCD()
        {
            List<int> buffer = new List<int>(m_castSkillCDMap.Keys);
            foreach (int key in buffer)
            {
                m_castSkillCDMap[key] = 0.0f;
            }
        }
        
        //是否CD时间中
        public bool IsInCoolDownTime(int skillID)
        {
            return ModuleServer.MS.GSkillCastMgr.InCDCoolState(skillID);
        }

        //当前距CD时间结束还有多久
        public float QuerySkillCDOverTime(int skillID)
        {
            return ModuleServer.MS.GSkillCastMgr.QuerySkillCDOverTime(skillID);
        }


        public float GetSkillCDBegin(int skillId)
        {
            return ModuleServer.MS.GSkillCastMgr.GetSkillCDBegin(skillId);
        }


        public void SetCoolDownTime(int skillID)
        {
            if (m_castSkillCDMap == null)
                return;
            float curTime = Time.time;
            if (m_castSkillCDMap.ContainsKey(skillID))
            {
                m_castSkillCDMap[skillID] = curTime;
                return;
            }

            m_castSkillCDMap.Add(skillID, curTime);
        }

        //释放
        protected virtual void OnDestroy()
        {
            UnRegisterEvent();
            if (mHealth != null)
            {
                mHealth.OnRemoveHPBar(true);
            }
            if (HeadTypeLogo.Instance != null)
            {
                HeadTypeLogo.Instance.OnModelRemoveUpdate(this);
            }
            if (m_hitActorObject != null)
                m_hitActorObject = null;
            if (m_castSkillCDMap != null) {
                m_castSkillCDMap.Clear();
                m_castSkillCDMap = null;
            }

            if (m_animationCurveBase != null)
            {
                m_animationCurveBase.m_actorObject = null;
                m_animationCurveBase = null;
            }
            m_audioSource = null;
        }
        
        //获取gameobject身上挂点
        private Transform GetBehitEfxTransform(int skillID)
        {
            if (m_behitEfxTransform == null)
            {
                LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(skillID);
                if (skillDesc != null)
                {
                    if (skillDesc.Get<int>("behitEfxPos") == (int)BehitEfxPos.BIP)
                    {
                        Transform bipTrans = transform.FindChild("Bip001");

                        if (bipTrans == null)
                        {
                            bipTrans = transform.FindChild("Bip01");

                            if (bipTrans == null)
                            {
                                bipTrans = transform;
                            }
                        }
                        
                        m_behitEfxTransform = bipTrans;
                    }
                    if (skillDesc.Get<int>("behitEfxPos") == (int)BehitEfxPos.FOOT)
                    {
                        Transform bipTrans = transform.FindChild("E_Root");

                        if (bipTrans == null)
                        {
                            bipTrans = transform;
                        }

                        m_behitEfxTransform = bipTrans;
                    }
                }
                else
                {
                    m_behitEfxTransform = transform;
                }
            }

            return m_behitEfxTransform;
        }

        public Vector3 GetBehitEfxPosition()
        {
            if (m_behitEfxPosition == null)
            {
                m_behitEfxPosition = GetChildTransform("E_Spine");
                if (m_behitEfxPosition == null)
                    m_behitEfxPosition = GetChildTransform("Bip001");
                if (m_behitEfxPosition == null)
                    m_behitEfxPosition = GetChildTransform("Bip01");
                if (m_behitEfxPosition == null)
                    m_behitEfxPosition = GetChildTransform("Bone001");

                if (m_behitEfxPosition == null)
                    m_behitEfxPosition = transform;

            }
            return m_behitEfxPosition.position;
        }

        public Transform GetChildTransform(string transformName)
        {
            Transform[] transforms = this.gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < transforms.Length; ++i)
            {
                if (transforms[i].name.Equals(transformName))
                {
                    return transforms[i];
                }
            }

            return null;
        }

        //是否倒地
        public bool IsHitDownState()
        {
            if (m_IActorState.m_state != ACTOR_STATE.AS_BEHIT)
            {
                return false;
            }

            return m_BehitState.m_isHitDownState;
        }

        public void CanotCurveMove()
        {
            //中断当前的位移
            ExitAnimationCurveMove(m_useCurveMoveUUID);
            m_canCurveMove = false;
        }

        public void CanCurveMove()
        {
            m_canCurveMove = true;
        }

        public bool IsCanCurveMove()
        {
            return m_canCurveMove;
        }

        /// <summary>
        /// 选择了目标。
        /// </summary>
        /// <param name="oldobj">原来的目标。</param>
        /// <param name="newobj">新的目标。</param>
        public virtual void OnSelectTarget(ActorObj oldobj, ActorObj newobj)
        {

        }

        public void SelectNpc(ActorObj npc, bool bSet)
        {

            if (m_SelectTargetObject != null)
            {
                if (CoreEntry.gSkillMgr.m_beSelectTagFriend == null)
                {
                   // Debug.LogError("======SelectNpc=====1===" + m_SelectTargetObject.name);
                    CoreEntry.gSkillMgr.m_beSelectTagFriend = CoreEntry.gSkillMgr.GetSelectTag(false);// GameObject.Instantiate(CoreEntry.gResLoader.Load("Effect/common/fx_select_lv")) as GameObject;
                }
                if (CoreEntry.gSkillMgr.m_beSelectTagFriend == null)
                {
                    return;
                }
               // Debug.LogError("======SelectNpc=====2===" + m_SelectTargetObject.name);
                CoreEntry.gSkillMgr.m_beSelectTagFriend.SetActive(bSet);
                CoreEntry.gSkillMgr.m_beSelectTagFriend.transform.parent = npc.transform;
                CoreEntry.gSkillMgr.m_beSelectTagFriend.transform.localPosition = new Vector3(0, 0.1f, 0);
                CoreEntry.gSkillMgr.m_beSelectTagFriend.transform.localRotation = Quaternion.identity;
            }
        }
        
        //取消碰撞
        public bool CancelCollider()
        {
            if (m_allCollider == null)
            {
                return false;
            }

            for (int i = 0; i < m_allCollider.Length; ++i)
            {
                m_allCollider[i].enabled = false;
            }

            return true;
        }

        //恢复碰撞
        public bool RecoverCollider()
        {
            if (m_allCollider == null)
            {
                return false;
            }

            for (int i = 0; i < m_allCollider.Length; ++i)
            {
                m_allCollider[i].enabled = true;
            }

            return true;
        }

        /// <summary>
        /// 获取指定buff
        /// </summary>
        /// <param name="buffId"></param>
        /// <returns></returns>
        public Buff GetBuff(long buffId)
        {
            for (int i = 0; i < buffList.Count; ++i)
            {
                if (buffList[i].mBuffId == buffId)
                {
                    return buffList[i];
                }
            }

            return null;
        }
        /// <summary>
        /// 获取指定BUFF， 通过配置表ID
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public Buff GetBuffWithCfgId(int configId)
        {
            foreach (var item in buffList)
            {
                if (item.mBuffType == configId)
                {
                    return item;
                }
            }
            return null;
        }

        public void Addbuff(BuffData buffdata, ActorObj target, bool isShowTip = true)
        {
            AddbuffWithTempParameter(buffdata, target, 0, isShowTip);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iBuffId"></param>
        /// <param name="giver"></param>
        /// <param name="skillID"></param>
        /// <param name="tempParameter">临时参数，用于buff之间传递参数</param>
        /// <param name="isShowTip"></param>
        public void AddbuffWithTempParameter(BuffData buffdata, ActorObj target, int tempParameter, bool isShowTip = true)
        {
            //if (buffdata.buffType == 24100001 || buffdata.buffType == 24100002)
            //{
            //    LogMgr.LogError("添加iBuffId = "+ buffdata.buffType);
            //}
            if (buffdata.BufferInstanceID == 0)
                return;
            LuaTable desc = ConfigManager.Instance.Skill.GetBuffConfig(buffdata.buffType);
            if (desc == null)
            {
                //LogMgr.LogError("不存在buff " + iBuffId);
                return;
            }
            //LogMgr.LogError("ActorObj 2 AddbuffWithTempParameter buffList.Add " + buffdata.buffType);
            // 死亡状态不能添加的Buff
            if (IsDeath())
                return;

            // 添加Buff添加提示信息
            string bname = desc.Get<string>("name");
            if (isShowTip && bname.Length > 0 && target != null)
            {
                if (IsMainPlayer() || target.IsMainPlayer()) // 只有主角显示提示
                {
                    SetFlyText(bname, desc.Get<int>("buff_type"));
                }
            }
            //LogMgr.LogError("ActorObj 3 AddbuffWithTempParameter buffList.Add " + buffdata.buffType);
            // 如果原来有这个Buff类别就调用替换Buff函数 
            //bool rel = ReplaceBuffWithType(buffdata.buffType, target, tempParameter);
            //if (rel)
            //    return;

            Buff newBuff = new Buff(buffdata, this, target);
            newBuff.TempParameter = tempParameter;
            //LogMgr.LogError("ActorObj 4 AddbuffWithTempParameter buffList.Add " + newBuff.mBuffType);
            buffList.Add(newBuff);
            newBuff.OnEnter();
        }


        public void SetFlyText(string text, int isGood)
        {
            EventParameter param = EventParameter.Get();
            param.intParameter = (int)FlyTextType.Buff;
            param.goParameter = gameObject;
            param.objParameter = text;
            param.intParameter2 = isGood;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FLYTEXT, param);
        }

        // 如果删除成功返回true
        public bool RemoveBuff(long iBuffId)
        {
            for (int i = 0; i < buffList.Count; i++)
            {
                Buff buff = buffList[i];
                if (buff.mBuffId == iBuffId)
                {
                    if (iBuffId == 24100001 || iBuffId == 24100001)   //
                    {

                    }
                    //LogMgr.UnityError("ActorObj RemoveBuff 1 buff.OnDie " + buff.mBuffType);
                    buff.OnDie();
                    //LogMgr.UnityError("ActorObj RemoveBuff 2 buff.OnDie " + buff.mBuffType);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 随机清除N个指定类型的buff
        /// </summary>
        /// <param name="type"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool RemoveSpecialTypeBuff(BuffActionType type, int num)
        {
            List<Buff> tmpBuffList = new List<Buff>();
            for (int i = 0; i < buffList.Count; i++)
            {
                Buff curr = buffList[i];
                if (curr != null && curr.desc != null && curr.desc.Get<int>("buff_type") == (sbyte)type)
                {
                    tmpBuffList.Add(curr);
                }
            }

            bool ret = false;
            for (int i = 0; i < num; ++i)
            {
                if (tmpBuffList.Count > 0)
                {
                    int pos = Random.Range(0, tmpBuffList.Count);
                    tmpBuffList[pos].OnDie();
                    tmpBuffList.RemoveAt(pos);
                    ret = true;
                }

            }
            return ret;
        }

        // 如果删除成功返回true
        public bool RemoveBuff(int iType, int count)
        {
            for (int i = 0; i < buffList.Count; i++)
            {
                Buff buff = buffList[i];
                if ((buff.desc.Get<int>("buff_type") & iType) == buff.desc.Get<int>("buff_type"))
                {
                    buff.OnDie();
                    if (count-- <= 0)
                        return true;
                }
            }
            return false;
        }
/*
        // 替换原来的Buff, 高等级替换低等级,
        public bool ReplaceBuffWithType(int iBuffId, ActorObj giver, int tempParameter)
        {
            bool rel = false; // false 表示本函数调用后可以添加Buff
            LuaTable desc = ConfigManager.Instance.Skill.GetBuffConfig(iBuffId);
            List<Buff> toRemove = new List<Buff>();
            for (int i = 0; i < buffList.Count; i++)
            {
                Buff curr = buffList[i];
                if (curr.desc.Get<int>("buff_type") == desc.Get<int>("buff_type"))
                {
                    // 如果buffID相同
                    if (curr.desc.Get<int>("id") == desc.Get<int>("id"))
                    {
                        // 叠加类Buff
                        curr.Stacking();
                        // 重设Buff
                        curr.ReplaceBuff();
                        curr.TempParameter = tempParameter;
                        rel = true; // 返回true表示替换成功,不需要添加Buff了
                    }
                    else
                    {
                        if (curr.desc.Get<int>("priority") <= desc.Get<int>("priority")) // 优先级
                        {
                            toRemove.Add(curr);
                            rel = false; // false表示低有限度的Buff被删除
                        }
                        else if (curr.desc.Get<int>("priority") > desc.Get<int>("priority"))
                        {
                            LogMgr.LogError("curr.desc.Get<int>(id) " + curr.desc.Get<int>("id") + "  equalID " + desc.Get<int>("id"));
                            rel = true;
                        }
                    }
                }
            }
            for (int i = 0; i < toRemove.Count; i++)
            {
                toRemove[i].OnExit();
                buffList.Remove(toRemove[i]);
            }
            return rel;
        }
 
*/
        // 替换原来的Buff, 高等级替换低等级,
        public bool ReplaceBuffWithType(int iBuffId, ActorObj giver, int tempParameter)
        {
            bool rel = false; // false 表示本函数调用后可以添加Buff
            LuaTable desc = ConfigManager.Instance.Skill.GetBuffConfig(iBuffId);
            List<Buff> toRemove = new List<Buff>();
            for (int i = 0; i < buffList.Count; i++)
            {
                Buff curr = buffList[i];
                if (curr.desc.Get<int>("buff_type") == desc.Get<int>("buff_type"))
                {
                    // 如果buffID相同
                    if (curr.desc.Get<int>("id") == desc.Get<int>("id"))
                    {
                        // 叠加类Buff
                        curr.Stacking();
                        // 重设Buff
                        curr.ReplaceBuff();
                        curr.TempParameter = tempParameter;
                        rel = true; // 返回true表示替换成功,不需要添加Buff了
                    }
                    else
                    {
                        if (desc.Get<int>("mutex") > 0) //互斥buff,mutex相同，
                        {
                            if (curr.desc.Get<int>("mutex") == desc.Get<int>("mutex"))
                            {
                                toRemove.Add(curr);
                                rel = false; // false表示低有限度的Buff被删除
                            }
                        }
                        else
                        {
                            //魔神变身，引起的buff-bug，先屏蔽再说
                            //if (curr.desc.Get<int>("priority") <= desc.Get<int>("priority")) // 优先级
                            //{
                            //    toRemove.Add(curr);
                            //    rel = false; // false表示低有限度的Buff被删除
                            //}
                            //else if (curr.desc.Get<int>("priority") > desc.Get<int>("priority"))
                            //{
                            //    //LogMgr.UnityError("curr.desc.Get<int>(id) " + curr.desc.Get<int>("id") + "  equalID " + desc.Get<int>("id"));
                            //    rel = true;
                            //}
                        }                      
                    }
                }             
            }
            for (int i = 0; i < toRemove.Count; i++)
            {
                //LogMgr.LogError("ActorObj ReplaceBuffWithType " + toRemove[i].mBuffType);
                toRemove[i].OnExit();
                buffList.Remove(toRemove[i]);
            }
            return rel;
        }

        public bool ResetBuffState()
        {
            bool bHasState = false;
            for (int i = 0; i < buffList.Count; ++i)
            {
                Buff buff = buffList[i];
                bHasState = buff.ResetBuffState();
                if (bHasState == true)
                    break;
            }
            return bHasState;
        }
        public void BuffOnDie()
        {
            int index = 0;
            while (true)
            {
                if (buffList.Count <= index)
                    break;
                Buff buff = buffList[index];
                buff.OnDie();
                if (buff.IsFinished())
                {
                    //LogMgr.LogError("ActorObj BuffOnDie " + buff.mBuffType);
                    buff.OnExit();
                    buffList.Remove(buff);
                }
                else
                    index++;
            }
        }
        public void BuffOnStealth(bool bStealth)
        {
            int index = 0;
            while (true)
            {
                if (buffList.Count <= index)
                    break;
                Buff buff = buffList[index];
                buff.OnStealth(m_bStealthState);
                index++;
            }
        }

        public void UpdateBuff(BuffData buffdata)
        {
            Buff buff = GetBuff(buffdata.BufferInstanceID);
            if (buff != null)
            {
                buff.ResetBuffState();

            }

        }
        
        public void ClearBuff()
        {
            for (int i = 0; i < buffList.Count; ++i)
            {
                Buff buff = buffList[i];
                buff.OnExit();
            }
            buffList.Clear();
        }

        public void PreloadCommonObj()
        {
            for (int index = 1; index < 6; index++)
            {
                GameObject obj1 = CoreEntry.gGameObjPoolMgr.InstantiateEffect(@"Effect/skill/hurt/fodder_blood0" + index);

                if (obj1 != null)
                {
                    CoreEntry.gGameObjPoolMgr.Destroy(obj1);
                }
            }

        }

        public void BreakSkillWhenHit(int skillID)
        {
            StateParameter stateParm = new StateParameter();
            stateParm.state = ACTOR_STATE.AS_BEHIT;

            stateParm.skillID = skillID;
            m_AttackState.BreakSkill(stateParm);
        }
        public void BreakSkillWhenAttack(int skillID)
        {
            StateParameter stateParm = new StateParameter();
            stateParm.state = ACTOR_STATE.AS_STAND;

            stateParm.skillID = skillID;
            m_AttackState.BreakSkill(stateParm);
        }

        public bool CanChangeAttack()
        {
            return m_AttackState.CanChangeAttack();
        }

        public void HideEffect()
        {
            if (centerSocket != null)
            {
                centerSocket.gameObject.SetActive(false);
            }
        }

        public void ShowEffect()
        {
            if (centerSocket != null)
            {
                centerSocket.gameObject.SetActive(true);
            }
        }

        protected bool mVisiable = true;
        public bool Visiable
        {
            get { return mVisiable; }
        }

        protected bool mShowHorse = true;
        protected bool mOnHorse = false;
        protected bool mShowWing = true;
        protected bool mInWing = false;
        protected bool mShowMagic = true;
        protected bool mInMagic = false;
        public void HideHorse()
        {
            mShowHorse = false;
            if (null != mMount)
            {
                mMount.SetActive(false);
            }
        }

        public void ShowHorse()
        {
            mShowHorse = true;
            if (null != mMount)
            {
                mMount.SetActive(mOnHorse);
            }
        }

        public void HideWing()
        {
            mShowWing = false;
            if (null != mWing)
            {
                mWing.SetActive(false);
            }
        }

        public void ShowWing()
        {
            mShowWing = true;
            if (null != mWing)
            {
                mWing.SetActive(mInWing);
            }
        }

        public void HideZhenFa()
        {
            if(null != mZhenFa)
            {
                mZhenFa.SetActive(false);
            }
        }

        public void ShowZhenFa()
        {
            if(null != mZhenFa)
            {
                mZhenFa.SetActive(true);
            }
        }

        public void ShowMagic()
        {
            mShowMagic = true;
            if (null != MagicKeyModel)
            {
                MagicKeyModel.SetActive(mInMagic);
            }
        }

        public void HideMagic()
        {
            mShowMagic = false;
            if (null != MagicKeyModel)
            {
                MagicKeyModel.SetActive(false);
            }
        }

        /// <summary>
        /// 自然下落
        /// </summary>
        public void FallDown()
        {
            m_gravityMotionBase.StartHitToSky(0.1f, 270f);
        }

        public void DobehitedFly(string clipName, float distance, ActorObj attacker)
        {
            if(null == mHitFlyCurveBase)
            {
                return;
            }
            if (!m_AnimationMgr.IsHadAction(clipName))
            {
                return;
            }

            float f = ConfigManager.Instance.Consts.GetValue<float>(432, "fval");
            mHitFlyCurveBase.StartMove(m_AnimationMgr.GetActionLength(clipName) * f, distance, attacker);
        }

        public bool IsLocalPlayer()
        {
            if (mActorType == ActorType.AT_LOCAL_PLAYER)
                return true;

            return false;
        }

        /// <summary>
        /// 设置即将回收
        /// </summary>
        public void SetToRecycle()
        {
            DisableAttachingEffect();
            CoreEntry.gActorMgr.RemoveActor(this);
            CoreEntry.gObjPoolMgr.RecycleObject(resid, gameObject);
        }

        public void SetToRecycleDelay()
        {
            DisableAttachingEffect();
            BeginRecycleSelf();

        }

        void BeginRecycleSelf()
        {
            HideBody();
            Invoke("EndRecycleSelf", 1f);
        }

        void EndRecycleSelf()
        {
            ShowBody();
            CoreEntry.gActorMgr.RemoveActor(this);
            CoreEntry.gObjPoolMgr.RecycleObject(resid, gameObject);
        }

        public void HideBody()
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform tr = transform.GetChild(i);
                if (tr != null)
                {
                    tr.gameObject.SetActive(false);
                }
            }
        }

        public void ShowBody()
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform tr = transform.GetChild(i);
                if (tr != null)
                {
                    tr.gameObject.SetActive(true);
                }
            }
        }

        public void DisableAttachingEffect()
        {
            //去掉阴影
            HideBlobShadow();
        }

        public void EnableAttachingEffect()
        {
            //去掉阴影
            ShowBlobShadow();
        }

        public void DisableInWorld()
        {
            DisableAttachingEffect();

            CoreEntry.gActorMgr.RemoveActor(this);
        }

        public void EnableInWorld()
        {
            EnableAttachingEffect();

            CoreEntry.gActorMgr.AddActorObj(this);
        }

        /// <summary>
        /// 获得普通攻击的ID
        /// </summary>
        /// <returns></returns>
        public int GetNormalAttack()
        {
            int r = Random.Range(0, NormaAttackNum);
            return SwitchNormalAttack(r);
        }

        void CheckNormalAttackNum()
        {
            if (normaAttackNum < 0)
            {
                int maxId = 1;
                normaAttackNum = maxId;
            }

        }

        public int SwitchNormalAttack(int index)
        {
            int skillId = 0;
            switch (index)
            {
                case 0:
                    skillId = 3000001;
                    break;
                case 1:
                    skillId = 3000002;
                    break;
                case 2:
                    skillId = 3000003;
                    break;


            }
            return skillId;
        }

        public int GetNextNormalAttack(ref int currentIndex)
        {
            //没有普攻直接返回0
            if (NormaAttackNum <= 0)
            {
                return 0;
            }

            currentIndex++;
            if (currentIndex >= NormaAttackNum)
            {
                currentIndex = currentIndex % NormaAttackNum;
            }


            return SwitchNormalAttack(currentIndex);
        }

        public void AddLimitParam(LimitParam v)
        {
            if (actorLimitList.Contains(v) == false)
            {
                actorLimitList.Add(v);
                LogMgr.LogWarning("actorLimitList size:{0}", actorLimitList.Count);
            }
        }

        public void RemoveLimiParam(LimitParam v)
        {
            if (actorLimitList.Contains(v) == true)
            {
                actorLimitList.Remove(v);
            }
        }

        public LimitParam ForbidToUseSkill()
        {
            LimitParam v = new LimitParam();
            if (v != null)
            {
                v.iState = (int)limitType.Skill;
            }
            AddLimitParam(v);
            return v;
        }

        public void AllowToUseSkill(LimitParam tag)
        {
            RemoveLimiParam(tag);
        }

        bool m_IsGod = false;
        public bool IsGod
        {
            get
            {
#if UNITY_EDITOR
                return m_IsGod;
#else
                return false; 
#endif
            }
            set { m_IsGod = value; }
        }

        public virtual float GetDeathDuration()
        {
            return 0f;
        }


        public ActorObj GetTargetFromSelector(LuaTable skillDesc, DamageCell dmgCell)
        {
            return null;
        }

        /// <summary>
        /// 是否能被输入控制该actor
        /// </summary>
        bool canBeControlledByInput = true;

        public bool CanBeControlledByInput
        {
            get { return canBeControlledByInput; }
            private set { canBeControlledByInput = value; }
        }

        /// <summary>
        /// 禁止玩家操作申请的计数
        /// </summary>
        int CanNotBeControlledByInputCounter = 0;
        /// <summary>
        /// 申请禁止玩家操作
        /// </summary>
        public void ReqirueCanNotBeControlledByInput()
        {
            CanNotBeControlledByInputCounter++;
            if (CanNotBeControlledByInputCounter > 0)
            {
                CanBeControlledByInput = false;
            }
        }

        /// <summary>
        /// 释放禁止玩家操作
        /// </summary>
        public void ReleaseCanNotBeControlledByInput()
        {
            CanNotBeControlledByInputCounter--;
            if (CanNotBeControlledByInputCounter <= 0)
            {
                CanBeControlledByInput = true;
                CanNotBeControlledByInputCounter = 0;
            }
        }

        /// <summary>
        /// 这个只给普通攻击使用
        /// </summary>
        public bool CanBeControlledByInputFromNormalAttack
        {
            get;
            private set;
        }

        /// <summary>
        /// 申请禁止玩家操作，只能是普攻调用
        /// </summary>
        public void ReqirueCanNotBeControlledByInputFromNormalAttack()
        {

            CanBeControlledByInputFromNormalAttack = false;
            //增加保护机制
            CancelInvoke("ReleaseCanNotBeControlledByInputFromNormalAttack");
            Invoke("ReleaseCanNotBeControlledByInputFromNormalAttack", 1f);

        }

        /// <summary>
        /// 释放禁止玩家操作，只能是普攻调用
        /// </summary>
        public void ReleaseCanNotBeControlledByInputFromNormalAttack()
        {
            CancelInvoke("ReleaseCanNotBeControlledByInputFromNormalAttack");
            CanBeControlledByInputFromNormalAttack = true;
        }

        public void Suicide()
        {
            DamageParam damageParam = new DamageParam();
            damageParam.skillID = 0;
            damageParam.attackActor = this;
            damageParam.behitActor = this;

            BehitParam behitParam = new BehitParam();
            behitParam.hp = (int)mBaseAttr.MaxHP;
            behitParam.displayType = DamageDisplayType.DDT_NORMAL;
            behitParam.damgageInfo = damageParam;
            OnDamage((int)mBaseAttr.MaxHP, 0, true, behitParam);
        }

        public bool IsInStealthState(ActorObj target)
        {
            if (target != null)
            {
                //塔可以看到隐身玩家
                if (target.m_bIsTower)
                {
                    return false;
                }

                return m_bStealthState;
            }
            else
            {
                return m_bStealthState;
            }
        }

        public void SetBehaviorPause(bool bPause)
        {
            m_bPaused = bPause;
            if (m_bPaused)
                m_AutoMove.SetPaused(true);
            else
                m_AutoMove.SetPaused(false);
        }

        public override void RecycleObj()
        {
            base.RecycleObj();
            mHealth.OnRemoveHPBar(true);

            //已经上马的得先下马
            if (IsRiding)
            {
                GetDownHorse();
            }
            //清掉坐骑资源
            if (mMount != null)
            {
                Destroy(mMount);
                mMount = null;
            }

            //清掉武器
            if (mRightWeapon != null)
            {
                Destroy(mRightWeapon);
                mRightWeapon = null;
            }

            //清掉翅膀
            if (mWing != null)
            {
                Destroy(mWing);
                mWing = null;
            }

            if(mZhenFa != null)
            {
                Destroy(mZhenFa);
                mZhenFa = null;
            }

            if (MagicKeyModel != null)
            {
                Destroy(MagicKeyModel);
                MagicKeyModel = null;
            }

            if(buffList != null)
            {
                buffList.Clear();
            }

            Destroy(m_audioSource);
            Destroy(m_audioSourceBody);
        }

        public static Transform RecursiveFindChild(Transform t, string name)
        {
            if (t.name.CompareTo(name) == 0)
            {
                return t;
            }

            Transform ret = null;
            foreach (Transform child in t)
            {
                ret = RecursiveFindChild(child, name);
                if (ret != null)
                {
                    break;
                }
            }

            return ret;
        }
        
        private GameObject mMount;                          //坐骑对象
        private Transform mMountBack;                       //坐骑背部挂接人物的骨骼
        private Transform mSelfBone;                        //玩家挂到坐骑上的骨骼
        private int mCurRideSkin;                           //当前坐骑皮肤编号

        public GameObject MagicKeyModel;                   //法宝模型

        public int mMagicKeyId = 0;
        public int mMagicKeyStar = 0;
        /// <summary>
        /// 是否在坐骑状态
        /// </summary>
        public bool IsRiding
        {
            get { return mMount != null && mOnHorse; }
        }

        private void LoadMount(int skin)
        {
            if (mCurRideSkin == skin)
            {
                return;
            }

            if (mSelfBone == null)
            {
                mSelfBone = transform.FindChild("DM_mount");
            }

            //卸掉原来的坐骑对象
            if (mMount != null)
            {
                Destroy(mMount);
                mMount = null;
                mMountBack = null;
            }

            //加载新的坐骑
            string path = ConfigManager.Instance.Ride.GetRideSkin(skin);
            GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(path, typeof(GameObject));
            if (prefab == null)
            {
                LogMgr.ErrorLog("加载坐骑失败 skin:{0} path:{1}", skin, path);
                return;
            }

            //初始化坐骑
            mCurRideSkin = skin;
            mMount = Instantiate(prefab) as GameObject;
            mMount.transform.SetParent(transform);
            mMount.transform.localPosition = Vector3.zero;
            mMount.transform.localScale = Vector3.one;
            mMount.transform.forward = transform.forward;
            mMountBack = RecursiveFindChild(mMount.transform, "Horse_E_back");
            if (mMountBack == null)
            {
                LogMgr.ErrorLog("坐骑没有\"E_back\"节点，用根节点代替 skin:{0}", skin);
                mMountBack = mMount.transform;
            }

            SetShadow(mMount, ConfigManager.Instance.Ride.GetModelID(skin));
        }

        /// <summary>
        /// 上马。
        /// </summary>
        /// <param name="skin">马的皮肤。</param>
        public void FuckHorse(int skin)
        {
            if (CoreEntry.gMorphMgr.IsInMorph(serverID))//变身不上马
            {
                return;
            }

            //坐骑编号为0则下马
            if (skin == 0)
            {
                mOnHorse = false;
                GetDownHorse();
                return;
            }

            //已经骑着同皮肤坐骑则返回
            if (IsRiding)
            {
                if (mCurRideSkin == skin)
                {
                    return;
                }
                GetDownHorse();
            }

            mOnHorse = true;
            LoadMount(skin);
            if (mMount == null || mSelfBone == null)
            {
                mOnHorse = false;
                return;
            }


            string pani = curActorState == ACTOR_STATE.AS_RUN ? StateParameter.ANI_NAME_MOUNT_RUN : StateParameter.ANI_NAME_MOUNT_STAND;
            string mani = curActorState == ACTOR_STATE.AS_RUN ? StateParameter.ANI_NAME_RUN : StateParameter.ANI_NAME_STAND;
            mMount.SetActive(true);
            SetShadow(mMount, ConfigManager.Instance.Ride.GetModelID(skin));

            mMount.GetComponent<Animation>().Play(mani);
            GetComponent<Animation>().Play(pani);
            mSelfBone.SetParent(mMountBack);
            mSelfBone.forward = mMountBack.forward;
            mSelfBone.localPosition = Vector3.zero;
            mSelfBone.localScale = Vector3.one;

            if (mHealth != null)
            {
                mHealth.OnNodeChange(mMount, "Horse_E_top");
            }

            //上马特效
            if (m_ActorType == ActorType.AT_LOCAL_PLAYER)
            {
                GameObject mEffectObj = CoreEntry.gGameObjPoolMgr.Instantiate("Effect/zuoqi/Mount_vfx");
                if (null != mEffectObj)
                {
                    SceneEfxPool efx = mEffectObj.GetComponent<SceneEfxPool>();
                    if (efx == null)
                    {
                        efx = mEffectObj.AddComponent<SceneEfxPool>();
                    }
                    if (efx != null)
                    {
                        efx.Init(transform.position, 2.5f);
                    }
                }
            }

            mMount.SetActive(mShowHorse);
        }

        /// <summary>
        /// 下马。
        /// </summary>
        public void GetDownHorse()
        {
            mOnHorse = false;
            if (mMount == null)
            {
                return;
            }
          
            string pani = "";
            if (IsWing && m_IsInFly)
            {
                pani = curActorState == ACTOR_STATE.AS_RUN ? StateParameter.ANI_NAME_FLY_RUN : StateParameter.ANI_NAME_FLY_STAND;
            }
            else
            {
                pani = curActorState == ACTOR_STATE.AS_RUN ? StateParameter.ANI_NAME_RUN : StateParameter.ANI_NAME_STAND;
            }
            mMount.SetActive(false);
            SetShadow(mMount, ConfigManager.Instance.Ride.GetModelID(mCurRideSkin));
            if (null != mSelfBone)
            {
                mSelfBone.SetParent(transform);
            }
            PlayAction(pani);
            if (mHealth != null)
            {
                mHealth.OnNodeChange(gameObject, IsWing && m_IsInFly ? "Wing_E_top" : "E_top");
            }
        }

        /// <summary>
        /// 请求上马
        /// </summary>
        public virtual void ReqRideHorse()
        {
            if (AutoPathFind)
            {
                if (RideData.CheckMountDistance(MulScenesPathFinder.Instance.TargetMapID, MulScenesPathFinder.Instance.TargetPosition))
                {
                    if (!m_IsInFight)
                    {
                        PlayerData.Instance.RideData.SendChangeRideStateRequest(1);
                    }
                    m_RideOnOutFight = m_IsInFight;
                }

            }
        }

        private bool m_RideOnOutFight = false;              //脱战后是否上马

        private Transform mRightHand;                       //右手挂接点
        private Transform mLeftHand;                        //左手挂接点

        private GameObject mRightWeapon;                    //右手武器
        private GameObject mLeftWeapon;                     //左手武器

        /// <summary>
        /// 当前武器外观。
        /// </summary>
        public int mCurWeapon = -1;

        /// <summary>
        /// 改变武器。
        /// </summary>
        public void ChangeWeapon(int id)
        {
            if(mActorType == ActorType.AT_PET)
            {
                return;
            }
            float scale = 1.0f;
            if (CoreEntry.gMorphMgr.IsInMorph(serverID))//变身不改变武器
            {
                scale = CoreEntry.gMorphMgr.GetMorphWeaponScale(serverID);
                return;
            }

            if (mCurWeapon == id)
            {
                return;
            }

            if (mRightHand == null)
            {
                mRightHand = RecursiveFindChild(transform, "DM_R_Hand");
                if (mRightHand == null)
                {
                    LogMgr.WarningLog("找不到节点\"DM_R_Hand\"", name);
                    return;
                }
            }

            GameObject obj = SceneLoader.LoadModelObject(id);
            if (obj == null)
            {
                LogMgr.WarningLog("加载武器id:{0}失败", id);
                return;
            }

            SetShadow(obj, id);

            bool isPet = false;
            //if(mActorType == ActorType.AT_PET)
            //{
            //    PetObj pet = this as PetObj;
            //    if(null != pet)
            //    {
            //        ActorObj owner = CoreEntry.gActorMgr.MainPlayer;
            //        if (null != owner && pet.m_MasterServerID == owner.ServerID && 4 == owner.mBaseAttr.Prof)
            //        {
            //            isPet = true;
            //        }
            //        else
            //        {
            //            MsgData_sSceneObjectEnterHuman ownerData = CoreEntry.gSceneObjMgr.GetEntityData(pet.m_MasterServerID) as MsgData_sSceneObjectEnterHuman;
            //            if (null != ownerData && 4 == ownerData.Prof)
            //            {
            //                isPet = true;
            //            }
            //        }
            //    }
            //}
            if (mBaseAttr.Prof == 4 || isPet)
            {
                //刺客时双手武器特殊处理
                if (mRightWeapon != null)
                {
                    Destroy(mRightWeapon);
                    mRightWeapon = null;
                }
                mRightWeapon = obj.transform.Find("DM_R_wuqi01").gameObject;
                mRightWeapon.SetActive(true);
                mRightWeapon.transform.SetParent(mRightHand);
                mRightWeapon.transform.localPosition = Vector3.zero;
                mRightWeapon.transform.localScale = new Vector3(scale, scale, scale);
                mRightWeapon.transform.localRotation = Quaternion.identity;

                if (mLeftHand == null)
                {
                    mLeftHand = RecursiveFindChild(transform, "DM_L_Hand");
                    if (mLeftHand == null)
                    {
                        LogMgr.WarningLog("找不到节点\"DM_L_Hand\"", name);
                        return;
                    }
                }

                if (mLeftWeapon != null)
                {
                    Destroy(mLeftWeapon);
                    mLeftWeapon = null;
                }
                mLeftWeapon = obj.transform.Find("DM_L_wuqi01").gameObject;
                mLeftWeapon.SetActive(true);
                mLeftWeapon.transform.SetParent(mLeftHand);
                mLeftWeapon.transform.localPosition = Vector3.zero;
                mLeftWeapon.transform.localScale = new Vector3(scale, scale, scale);
                mLeftWeapon.transform.localRotation = Quaternion.identity;
                Destroy(obj);       //移除空对象
            }
            else
            {
                if (mRightWeapon != null)
                {
                    Destroy(mRightWeapon);
                    mRightWeapon = null;
                }

                mRightWeapon = obj;
                mRightWeapon.SetActive(true);
                mRightWeapon.transform.SetParent(mRightHand);
                mRightWeapon.transform.localPosition = Vector3.zero;
                mRightWeapon.transform.localScale = new Vector3(scale, scale, scale);
                mRightWeapon.transform.localRotation = Quaternion.identity;
            }
            mCurWeapon = id;

            if (mActorType == ActorType.AT_LOCAL_PLAYER || mActorType == ActorType.AT_REMOTE_PLAYER)
            {
                List<ActorObj> pets = CoreEntry.gActorMgr.GetAllPetActors();
                for (int i = 0; i < pets.Count; i++)
                {
                    PetObj pet = pets[i] as PetObj;
                    if (null != pet && pet.m_MasterServerID == serverID)
                    {
                        pet.ChangeWeapon(id);

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 是否穿戴翅膀
        /// </summary>
        public bool IsWing
        {
            get { return mWing != null; }
        }


        /// <summary>
        /// 当前翅膀对应的模型id
        /// </summary>
        public int mCurWing = 0;

        /// <summary>
        /// 当前翅膀对象。
        /// </summary>
        private GameObject mWing;

        /// <summary>
        /// 当前阵法对象
        /// </summary>
        private GameObject mZhenFa;

        /// <summary>
        /// 翅膀根节点。
        /// </summary>
        private Transform mWingRoot;

        /// <summary>
        /// 人物背部挂点。
        /// </summary>
        private Transform mBackHolder;

        /// <summary>
        /// 改变翅膀外观。
        /// </summary>
        /// <param name="id">翅膀模型ID。</param>
        public void ChangeWing(int id)
        {
            mInWing = false;
            if (CoreEntry.gMorphMgr.IsInMorph(serverID))//变身不改变翅膀
            {
                return;
            }

            if (mCurWing == id)
            {
                return;
            }

            if (mBackHolder == null)
            {
                mBackHolder = RecursiveFindChild(transform, "E_back");
            }

            //更新翅膀
            if (id != 0)
            {
                GameObject obj = SceneLoader.LoadModelObject(id);
                if (obj == null)
                {
                    LogMgr.WarningLog("加载翅膀id:{0}失败", id);
                    return;
                }

                //移除原来翅膀
                if (mWing != null)
                {
                    Destroy(mWing);
                    mWing = null;
                }

                //挂接新翅膀
                mWing = obj;             
                mWing.transform.parent = mBackHolder;
                mWing.transform.localPosition = Vector3.zero;
                mWing.transform.localRotation = Quaternion.identity;
                mWing.SetActive(true);

                SetShadow(mWing, id);
            }
            else
            {
                Destroy(mWing);
                mWing = null;
            }
            mCurWing = id;

            //更新动作
            if (!IsRiding)
            {
                if (IsWing && m_IsInFly)
                {
                    string pani = curActorState == ACTOR_STATE.AS_RUN ? StateParameter.ANI_NAME_FLY_RUN : StateParameter.ANI_NAME_FLY_STAND;
                    GetComponent<Animation>().Play(pani);
                    string wingpani = curActorState == ACTOR_STATE.AS_RUN ? StateParameter.ANI_NAME_RUN : StateParameter.ANI_NAME_STAND;
                    if(mWing!=null)
                    mWing.GetComponent<Animation>().Play(wingpani);
                }
                else
                {
                    string pani = curActorState == ACTOR_STATE.AS_RUN ? StateParameter.ANI_NAME_RUN : StateParameter.ANI_NAME_STAND;
                    GetComponent<Animation>().Play(pani);
                }

                //飞行状态更换节点
                if (mHealth != null)
                {
                    mHealth.OnNodeChange(gameObject, IsWing && m_IsInFly ? "Wing_E_top" : "E_top");
                }
            }

            mInWing = true;
            if (null != mWing)
            {
                mWing.SetActive(mShowWing);
            }
        }

        //显示阵法
        public void ChangeZhenFa(int id)
        {
            if (mZhenFa != null)
                mZhenFa.SetActive(false);

            LuaTable row = RawLuaConfig.Instance.GetRowData("t_zhenfa", id);
            if(row != null)
            {
                LuaTable cfg = RawLuaConfig.Instance.GetRowData("t_model", row.Get<int>("ui_show_sen"));
                if (cfg == null) return;

                var objRes = CoreEntry.gResLoader.Load(cfg.Get<string>("skl"));
                if(objRes != null)
                {
                    mZhenFa = GameObject.Instantiate(objRes) as GameObject;
                    if(mZhenFa != null)
                    {
                        mZhenFa.SetActive(true);
                        mZhenFa.transform.parent = transform;
                        mZhenFa.transform.localPosition = new Vector3(0, 0, 0);
                        mZhenFa.transform.localRotation = Quaternion.identity;
                    }
                }
            }

            if(null != mZhenFa && !Visiable)
            {
                mZhenFa.SetActive(false);
            }
        }

        //显示星级
        public void ShowMagicKeyByStar(int v)
        {
            int modelidpre = MagicKeyDataMgr.Instance.GetMagickeyModelByStar(mMagicKeyId, mMagicKeyStar);
            int modelidnext = MagicKeyDataMgr.Instance.GetMagickeyModelByStar(mMagicKeyId, v);

            mMagicKeyStar = v;
            if(mMagicKeyId > 0)
            {
                EventParameter ep = EventParameter.Get();
                ep.objParameter = this;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_MAGICKEY_INFO_CREATE, ep);
            }
            if (modelidpre != modelidnext)
            {
                ShowMagicKey(0);
                ShowMagicKey(mMagicKeyId);
            }
        }
        //显示法宝
        public void ShowMagicKey(int v)
        {
            mInMagic = true;
            if (mMagicKeyId == v)
                return;
            mMagicKeyId = v;
            if (mMagicKeyId == 0)
            {
                if (MagicKeyModel != null)
                {
                    Destroy(MagicKeyModel);
                    MagicKeyModel = null;
                }

                mInMagic = false;
            }
            else
            {
                float scale = (float)MagicKeyDataMgr.Instance.GetMagickeyModelScale(mMagicKeyId);
                int modelid = MagicKeyDataMgr.Instance.GetMagickeyModelByStar(mMagicKeyId,mMagicKeyStar);
                if (modelid != 0)
                {
                    CoreEntry.gSceneLoader.LoadMagicKey(this, modelid, transform.position, transform.eulerAngles, new Vector3(scale, scale, scale));
                }
                else
                {
                    LogMgr.LogError(string.Format("没有找到法宝模型id  : {0} {1}", mMagicKeyId, mMagicKeyStar));
                }
            }

            if (null != MagicKeyModel)
            {
                MagicKeyModel.SetActive(mShowMagic);
            }
        }


        /// <summary>
        /// 当前穿的衣服外观。
        /// </summary>
        public int mClothes = 0;

        /// <summary>
        /// 改变衣服。
        /// </summary>
        public void ChangeClothes(int id)
        {
            if (mClothes == id)
            {
                return;
            }

            if (CoreEntry.gMorphMgr.IsInMorph(serverID))
            {
                return;
            }

            GameObject prefab = SceneLoader.LoadModelObject(id, true);
            if (prefab == null)
            {
                LogMgr.WarningLog("加载衣服id:{0}失败", id);
                return;
            }

            //Debug.LogError("====改变衣服=====");

            SkinnedMeshRenderer selfRender = GetSkinnedMeshRenderer(transform);
            SkinnedMeshRenderer toRender = GetSkinnedMeshRenderer(prefab.transform);

            if (toRender!= null && selfRender !=null)
            {
                SetBones(selfRender.gameObject, toRender.gameObject, gameObject);
                selfRender.sharedMesh = toRender.sharedMesh;
                selfRender.materials = toRender.sharedMaterials;
            }

            MeterialSet(transform, prefab.transform);


            mClothes = id;
        }

        /// <summary>
        /// 穿戴称号
        /// </summary>
        /// <param name="id">称号ID</param>
        public void ChangeHeroTitle(int id)
        {
            //print(string.Format("称号ID------>{0}", id));
            if(id == 0)
            {
                mHealth.OnHeroTitleChange(null);
            }
            else
            {
                LuaTable row = RawLuaConfig.Instance.GetRowData("t_title", id);
                if(row !=null)
                mHealth.OnHeroTitleChange(row.Get<string>("icon"));
            }
        }

        public void ChangeFaction(int iCamp)
        {
            mHealth.OnChangeFaction(iCamp);
        }

        /// <summary>
        /// 获取衣服蒙皮。
        /// </summary>
        /// <param name="transform">衣服对象节点。</param>
        /// <returns>衣服蒙皮。</returns>
        public static SkinnedMeshRenderer GetSkinnedMeshRenderer(Transform transform)
        {
            //遍历子节点(不能递归，否则会可能会加载)
            SkinnedMeshRenderer render = null;
            foreach (Transform t in transform)
            {
                render = t.GetComponent<SkinnedMeshRenderer>();
                if (render != null)
                {
                    break;
                }
            }
            return render;
        }

        //获取高中低配置
        public static MaterialSetting GetMateialSetting(Transform transform)
        {  
            SkinnedMeshRenderer render = null;
            MaterialSetting set = null;
            foreach (Transform t in transform)
            {
                render = t.GetComponent<SkinnedMeshRenderer>();
                if (render != null)
                {
                    set = t.GetComponent<MaterialSetting>();
                    break;
                }
            }
            return set;
        }

        // 刷新骨骼数据   将root物体的bodyPart骨骼更新为avatarPart
        public static void SetBones(GameObject goBodyPart, GameObject goAvatarPart, GameObject root)
        {
            var bodyRender = goBodyPart.GetComponentInChildren<SkinnedMeshRenderer>();
            var avatarRender = goAvatarPart.GetComponentInChildren<SkinnedMeshRenderer>();
            var myBones = new Transform[avatarRender.bones.Length];
            for (var i = 0; i < avatarRender.bones.Length; i++)
            {
                myBones[i] = BaseTool.instance.FindChildTransform(root.transform.gameObject, avatarRender.bones[i].name);
            }
            bodyRender.bones = myBones;
        }
        //刷新材质配置
        public static void MeterialSet(Transform self,Transform to)
        {
            Transform selfTrs = self.transform.FindChild("body");
            Transform toTrs = to.transform.FindChild("body");

            if (selfTrs == null || toTrs == null)
                return;

            MaterialSetting selfSet = selfTrs.GetComponent<MaterialSetting>();
            MaterialSetting toSet = toTrs.GetComponent<MaterialSetting>();
            if (selfSet == null)
            {
                selfSet = selfTrs.gameObject.AddComponent<MaterialSetting>();
                selfSet.mats = null;
            }
            if (toSet != null)
            {
                selfSet.Awake();
                selfSet.mats = toSet.mats;
                selfSet.Set();
            }
            else
            {
                selfSet.mats = null;
            }
        }
        public virtual void ChangePKStatus(int value) { }

        public void SetShadow(GameObject go, int modelID, float cameraSize = 4.8f)
        {
            LuaTable cfg = ConfigManager.Instance.Common.GetModelConfig(modelID);
            if (null == cfg)
            {
                return;
            }
            //特殊处理一下飞行坐骑金龙
            if (mMount != null && mMount.activeSelf &&
                mMount.gameObject.name.Contains("mount_jinlong_012_pre"))
            {
                cameraSize = 8.8f;
            }

            //特殊处理一下海豚
            if (mMount != null && mMount.activeSelf &&
                mMount.gameObject.name.Contains("mount_kun_018_pre"))
            {
                cameraSize = 5.8f;
            }

            //特殊处理一下变身
            if (go.name.Contains("bianshen_huoyan_1_pre"))
            {
                cameraSize = 6.8f;
            }


            if (null != m_blodShadow && null != m_blodShadow.shadowPro)
            {
                Camera cam = this.m_blodShadow.shadowPro.GetComponentInChildren<Camera>();
                cam.orthographicSize = cameraSize;
            }
            HashSet<string> nodes = new HashSet<string>();
            string[] nodeNames = cfg.Get<string>("castShadow").Split('#');
            for (int i = 0; i < nodeNames.Length; i++)
            {
                nodes.Add(nodeNames[i]);
            }

            if (m_ActorType == ActorType.AT_LOCAL_PLAYER)
            {
                CommonTools.SetLayer(go, LayerMask.NameToLayer("mainplayer"), nodes);
            }
            else
            {
                CommonTools.SetLayer(go, LayerMask.NameToLayer("player"), nodes);
            }
        }

        public virtual void OnExitState(ACTOR_STATE state) { }
    }
}