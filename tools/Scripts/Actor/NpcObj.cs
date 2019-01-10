using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{

    //玩家组件
[Hotfix]
    public class NpcObj : ActorObj
    {
        //private EventMgr m_eventMgr = null;
        //private GameLogicMgr m_gameMgr = null;

        //private ActorObj m_SelectTargetObject = null;

        //protected PlayerHeath m_heath = null;

        private int m_loadIndex;
        public int LoadIndex
        {
            set
            {
                m_loadIndex = value;
            }
            get { return m_loadIndex; }
        }


        // 无敌设置(GM 模块)
        public bool m_bWuDi = false;


        public bool m_EditTest = false;


        //yy
        public bool m_bUseAI = false;


        public behaviac.Agent PlayerAgent
        {
            get
            {
                if (m_actorAI == null)
                {
                    m_actorAI = this.gameObject.GetComponent<ActorAgent>();
                }
                return m_actorAI;
            }
        }


        public void setAutoMove()
        {

            m_AutoMove = this.gameObject.GetComponent<PlayerAutoMove>();
            if (m_AutoMove == null)
            {
                m_AutoMove = this.gameObject.AddComponent<PlayerAutoMove>();

                if (m_AutoMove)
                {
                    m_AutoMove.Init();
                }
            }
            m_AutoMove.SetAgentEnable(true);
            m_AutoMove.SetMoveSpeed(6);
            m_bAutoMove = true;
        }

        public void InitAutoMove()
        {
            if (m_bAutoMove)
            {
                m_AutoMove = this.gameObject.GetComponent<PlayerAutoMove>();
                if (m_AutoMove == null)
                {
                    m_AutoMove = this.gameObject.AddComponent<PlayerAutoMove>();

                    if (m_AutoMove)
                    {
                        m_AutoMove.Init();
                    }
                }
                m_AutoMove.SetAgentEnable(true);
            }
            else
            {
                if (m_AutoMove != null)
                {
                    m_AutoMove.SetAgentEnable(false);
                }

            }

        }


        public override void Recover()
        {

        }

        //生物id
        public override void Init(int resID, int configID, long entityID, string strEnterAction = "", bool isNpc = false)
        {
              InitAutoMove();

            base.Init(resID, configID, entityID, strEnterAction, isNpc);
            m_bodyType = 3;
            m_TeamType = 2;

            m_move = this.gameObject.GetComponent<ServerMoveAgent>();
            if(m_move == null)
            {
                m_move = this.gameObject.AddComponent<ServerMoveAgent>();
            }
            m_move.DoInit(this);

            // m_heath = this.gameObject.AddComponent<PlayerHeath>();


            //初始化后回调
            {
                OnPostInit();
            }

        }

        //void Awake()
        //{
        //    base.Awake();
        //}

        void OnEnable()
        {
            Start();
        }

        void Start()
        {
            //m_transform = this.transform;
            m_eventMgr = CoreEntry.gEventMgr;
            m_gameMgr = CoreEntry.gGameMgr;

            //  Random.seed = System.Guid.NewGuid().GetHashCode();

            //注册事件
            RegisterEvent();


            //脚步声
            InvokeRepeating("PlayerRunSound", 1, 0.1f);

        }

        void PlayerRunSound()
        {

        }

        bool NeedAIMode()
        {
            return false;
        }


        bool m_bNavGate = false;
        public bool NavGate
        {
            get { return m_bNavGate; }
            set
            {
                m_bNavGate = value;
                if (value)
                {
                    PlayAction("run");
                }
                else
                {
                    PlayAction("stand");

                }
            }
        }

        public override void NavigateTo(Vector3 pos)
        {
            setAutoMove();
            NavGate = true;
            m_AutoMove.MovePos(pos);
        }


        // Update is called once per frame
        public override void Update()
        {
            base.Update();

            //自动取消已经死亡的对象
            UpdateSelectTarget();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveEvent();
        }


        void UpdateSelectTarget()
        {
            //当前选中的怪是否死亡
            if (m_SelectTargetObject != null)
            {
                if (m_SelectTargetObject.IsDeath())
                {
                    SelectTarget(null);
                }

                //如果距离太远也取消
                if (m_SelectTargetObject != null)
                {
                    float distance = Vector3.Distance(transform.position, m_SelectTargetObject.transform.position);
                    if (distance >= 8)
                    {
                        SelectTarget(null);
                    }
                }
            }
        }

       

        public ActorObj GetSelTarget(int nSkillID)
        {
            if (m_SelectTargetObject == null)
            {
                return null;
            }


            //增加个判断，判断是选择友方还是敌方。
            LuaTable skillDesc = GetCurSkillDesc(nSkillID);

            //判断是否要切换目标
            bool bChangeTarget = false;

            if (skillDesc != null && skillDesc.Get<int>("faction_limit") == (byte)SkillFactionType.FACTION_FRIEND
                && m_SelectTargetObject.mActorType == mActorType)
            {
                bChangeTarget = true;
            }

            if (bChangeTarget)
            {
                m_SelectTargetObject = null;
            }

            return m_SelectTargetObject;
        }

        //自动选择一个最近的
        public ActorObj AutoSelTarget(int nSkillID)
        {

            return null;
        }

        void AutoCancelTarget()
        {
            CancelInvoke("AutoCancelTarget");
            SelectTarget(null);
        }



        //获取碰撞体半径
        public override float GetColliderRadius()
        {
            //if (m_ch != null)
            //{
            //    return m_ch.radius * m_transform.localScale.x;
            //}

            return 0.5f;
        }

        void RegisterEvent()
        {
            RemoveEvent();
            //怪物,boss,npc的
            m_eventMgr.AddListener(GameEvent.GE_PLAYER_RECOVER, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_PLAYER_RESET_HP, EventFunction);

            m_eventMgr.AddListener(GameEvent.GE_CC_GameGraphicSetting, OnChangeGrphicQuality);

            m_eventMgr.AddListener(GameEvent.GE_BOSS_SKILL, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_BOSS_SKILL_END, EventFunction);

            //m_eventMgr.AddListener(GameEvent.GE_EVENT_AFTER_ALL_LOADED_OF_SCENE, EventFunction);
        }

        void RemoveEvent()
        {
            //lmjedit  主界面 destroy 这个模型时 将被调用
            if (m_eventMgr != null)
            {
                m_eventMgr.RemoveListener(GameEvent.GE_PLAYER_RECOVER, EventFunction);
                m_eventMgr.RemoveListener(GameEvent.GE_PLAYER_RESET_HP, EventFunction);

                m_eventMgr.RemoveListener(GameEvent.GE_CC_GameGraphicSetting, OnChangeGrphicQuality);
                m_eventMgr.RemoveListener(GameEvent.GE_BOSS_SKILL, EventFunction);
                m_eventMgr.RemoveListener(GameEvent.GE_BOSS_SKILL_END, EventFunction);
                //m_eventMgr.RemoveListener(GameEvent.GE_EVENT_AFTER_ALL_LOADED_OF_SCENE, EventFunction);
            }
        }
        void OnChangeGrphicQuality(GameEvent ge, EventParameter paramter)
        {
            if (m_blodShadow != null)
            {
                //m_shadowType = GameGraphicSetting.IsLowQuality ? 1 : 2;
                m_blodShadow.ChangeShow(m_shadowType);
            }
        }

        void EventFunction(GameEvent ge, EventParameter parameter)
        {
            switch (ge)
            {


                default:
                    break;
            }
        }

        public void DoRevive()
        {
            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_STAND;
            param.skillID = 1;

            bool ret = RequestChangeState(param);

            // LogMgr.ErrorLog("DoRevive22222+++++++++ :" + m_actor.mBaseAttr.CardRecord.name);

            if (ret)
            {
                //   LogMgr.ErrorLog("DoRevive333333 :" + m_actor.mBaseAttr.CardRecord.name);

                //todo，属性恢复  
                Recover();

                //ui恢复
                if (mHealth != null)
                {
                    mHealth.OnResetHP();
                }
            }
            else
            {
                //int nBreak = 1;
            }

        }

        public override void ForceToRevive()
        {
            base.ForceToRevive();
            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_STAND;
            param.skillID = 1;

            EnterState(param);

            // LogMgr.ErrorLog("DoRevive22222+++++++++ :" + m_actor.mBaseAttr.CardRecord.name);


            //   LogMgr.ErrorLog("DoRevive333333 :" + m_actor.mBaseAttr.CardRecord.name);

            //todo，属性恢复  
            Recover(false);
        }


    }

};  //end SG

