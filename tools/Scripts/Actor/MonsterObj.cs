using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{

    //玩家组件
[Hotfix]
    public class MonsterObj : ActorObj
    {
        //private TouchMove m_touchMove = null;
        private CapsuleCollider m_capCollider = null;
        private BoxCollider m_boxCollider = null;
        private Collider_Type m_colliderType = Collider_Type.CT_NONE;

        //private GameLogicMgr m_gameMgr = null;

        //protected SkillGiftsMgr m_skillGiftsMgr = null;
        //private MonsterHeath m_heath = null;

        /// <summary>
        /// 标记塔的重要序号，1位最重要的塔，然后2，再次是3
        /// </summary>
        public int IndexAsTower = 1;

        public bool m_bNoUseBehit = false;

        /// <summary>
        /// 是不是被保护的重要对象
        /// </summary>
        public bool IsVip = false;

        /// <summary>
        /// vip被攻击的时候站立几秒
        /// </summary>
        public float StandTimeWhenBehitAsVip = 3;

        /// <summary>
        /// vip是否正在被攻击
        /// </summary>
        public bool IsUnderAttackAsVip = false;

        /// <summary>
        /// 是否是上下分部结构
        /// </summary>
        public bool m_IsUpperAndBasePart = false;

        public float ProtectedOffset = 2f;

        /// <summary>
        /// 怪物是否为Boss。
        /// </summary>
        private bool mIsBoss;

        /// <summary>
        /// 获取怪物是否位Boss。
        /// </summary>
        public bool IsBoss
        {
            get { return mIsBoss; }
        }

        //protected Configs.monsterConfig m_monsterInfo = null;
        //public Configs.monsterConfig MonsterInfo
        //{
        //    get { return m_monsterInfo; }
        //}

        /// <summary>
        /// 怪物配置。
        /// </summary>
        protected LuaTable m_MonsterConfig;

        /// <summary>
        /// 获取怪物配置。
        /// </summary>
        public LuaTable MonsterConfig
        {
            get { return m_MonsterConfig; }
        }

        public Vector3 GetProtectedOffset()
        {
            return transform.forward * ProtectedOffset + transform.right * 1f;
        }

        public Collider_Type ColliderType
        {
            get { return m_colliderType; }
        }

        public override void Awake()
        {
            base.Awake();

          //  m_transform = this.transform;
            m_capCollider = this.gameObject.GetComponent<CapsuleCollider>();
            m_boxCollider = this.gameObject.GetComponent<BoxCollider>();
            if (m_capCollider != null)
            {
                m_colliderType = Collider_Type.CT_CAPSULE;
            }
            else if (m_boxCollider != null)
            {
                m_colliderType = Collider_Type.CT_BOX;
            }


            RegisterEvent();
        }

        //bool isBoss = false;
        public override void Init(int resID, int configID, long entityID, string strEnterAction = "", bool isNpc = false)
        {
             
            m_gameMgr = CoreEntry.gGameMgr;
            base.Init(resID, configID, entityID, strEnterAction);
            m_MonsterConfig = ConfigManager.Instance.Actor.GetMonsterConfig(configID);
            if (m_MonsterConfig == null) return;
            //m_monsterInfo = CSVConfigManager.GetmonsterConfig(configID);
            if (m_MonsterConfig.Get<int>("hpCount") > 0)
            {
                mActorType = ActorType.AT_BOSS;
            }

            mIsBoss = mActorType == ActorType.AT_BOSS;
            m_bodyType = 3;
            m_TeamType = 3;

            m_move = this.gameObject.GetComponent<ServerMoveAgent>();
            if (m_move == null)
            {
                m_move = this.gameObject.AddComponent<ServerMoveAgent>();
            }
            m_move.DoInit(this);

            //m_move.Init();

            

            //设置跑步速度
            if (null != m_MonsterConfig)
            {
                m_iHitTurn = m_MonsterConfig.Get<int>("hit_turn");
                m_bodyType = m_MonsterConfig.Get<int>("size");
                m_DieSound = m_MonsterConfig.Get<int>("DeathSound");
                m_CanKnock = m_MonsterConfig.Get<int>("can_knock");
                mBaseAttr.Speed = m_MonsterConfig.Get<int>("chase_speed");
                SetSpeed(mBaseAttr.Speed);
            }
            OnPostInit();
        }

        public void InitAttr(MsgData_sSceneObjectEnterMonster es)
        {
            mBaseAttr.InitMonsterAttr(es);
            if (mHealth != null)
            {
                mHealth.OnResetHP();
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        //void Update()
        //{

        //}


        public void VipIsUnderAttack()
        {
            CancelInvoke("AttackTimeOut");
            IsUnderAttackAsVip = true;
            Invoke("AttackTimeOut", StandTimeWhenBehitAsVip);
        }

        void AttackTimeOut()
        {
            CancelInvoke("AttackTimeOut");
            IsUnderAttackAsVip = false;
        }

        public override void DoDamage(int hp, bool bIsMainPlayer, BehitParam behitParam)//lmjedit 真正的暴击 
        {
            base.DoDamage(hp, bIsMainPlayer, behitParam);
            if (mActorType == ActorType.AT_BOSS)
            {
                //EventToUI.SetArg(UIEventArg.Arg1, curHp * 1.0f / maxHp);
                //EventToUI.SetArg(UIEventArg.Arg2, behitParam);
                //EventToUI.SendEvent("EU_BOSSBLOOD_SETVALUE");
            }
        }

        //获取碰撞体半径
        public override float GetColliderRadius()
        {
            if (m_capCollider != null)
            {
                return m_capCollider.radius * transform.localScale.x;
            }

            if (m_boxCollider != null)
            {
                return Mathf.Min(m_boxCollider.size.x, m_boxCollider.size.z) * transform.localScale.x;
            }

            return 0.5f;
        }

        public override Vector3 GetBoxColliderSize()
        {
            Vector3 boxSize = Vector3.zero;

            if (m_boxCollider)
            {
                boxSize = m_boxCollider.size;
            }

            return boxSize;
        }

        public override Collider GetCollider()
        {
            if (m_colliderType == Collider_Type.CT_BOX)
            {
                return m_boxCollider;
            }
            else if (m_colliderType == Collider_Type.CT_CAPSULE)
            {
                return m_capCollider;
            }

            return null;
        }

        public override float GetDeathDuration()
        {
            float timeplus = 0f;
            if (mActorType == ActorType.AT_BOSS)
            {
                timeplus = CoreEntry.GameEffectMgr.BossDeadEffect.Length;
            }

            return m_MonsterConfig.Get<int>("lying_time") + timeplus;
        }

        void RegisterEvent()
        {
            //怪物,boss,npc的
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_ACTOR_CAN_BEHIT, EventFunction);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_MONSTER_DEATH, EventFunction);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CC_GameGraphicSetting, OnChangeGrphicQuality);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_PVP_PLAYER_RECOVER, EventFunction);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveEvent();
            m_capCollider = null;
            m_boxCollider = null;
            m_MonsterConfig = null;
        }

        void RemoveEvent()
        {
            //怪物,boss,npc的
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_ACTOR_CAN_BEHIT, EventFunction);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_MONSTER_DEATH, EventFunction);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_CC_GameGraphicSetting, OnChangeGrphicQuality);


            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_PVP_PLAYER_RECOVER, EventFunction);
        }
        void OnChangeGrphicQuality(GameEvent ge, EventParameter paramter)
        {
            if (m_blodShadow != null)
            {
                //m_shadowType = GameGraphicSetting.IsLowQuality ? 1 : 2;
                m_blodShadow.ChangeShow(m_shadowType);
            }
        }
        public override ActorObj AutoSelTarget(float fDis)
        {
            return m_SelectTargetObject;
        }


        void EventFunction(GameEvent ge, EventParameter parameter)
        {
            switch (ge)
            {
                case GameEvent.GE_ACTOR_CAN_BEHIT:
                    {


                        if (parameter.intParameter != entityid)
                        {
                            break;
                        }

                        //激活血条
                        if (mActorType == ActorType.AT_BOSS)
                        {
                            ////计算血条值
                            //int nBloodNum = 10 ; //m_actor.maxHp / ;

                            //EventToUI.SetArg(UIEventArg.Arg1, nBloodNum);
                            //EventToUI.SetArg(UIEventArg.Arg2, m_actor.actorCreatureDisplayDesc.name);
                            //EventToUI.SetArg(UIEventArg.Arg3, m_actor.gameObject);

                            //EventToUI.SendEvent("EU_BOSSBLOOD_SHOW");                    
                            BossBloodShow blood = gameObject.GetComponent<BossBloodShow>();
                            if (blood == null)
                            {
                                blood = gameObject.AddComponent<BossBloodShow>();
                                blood.m_actor = this;
                            }
                        }

                    }
                    break;
                case GameEvent.GE_MONSTER_DEATH:
                    {

                        if (parameter.intParameter != entityid)
                        {
                            break;
                        }

                        ////自己死亡
                        //if (mActorType == ActorType.AT_BOSS)
                        //{
                        //    EventToUI.SendEvent("EU_BOSSBLOOD_HIDE");
                        //}
                    }
                    break;

                //PVP玩家复活
                case GameEvent.GE_PVP_PLAYER_RECOVER:
                    {
                        if (parameter.intParameter != entityid)
                        {
                            break;
                        }

                        StateParameter param = new StateParameter();
                        param.state = ACTOR_STATE.AS_STAND;
                        param.skillID = 1;

                        bool ret = RequestChangeState(param);
                        if (ret)
                        {
                            //todo，属性恢复  
                            Recover();

                        }

                    }
                    break;


                default:
                    break;
            }
        }

    }
}

