using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace SG
{

    //玩家组件
[Hotfix]
    public class OtherPlayer : ActorObj
    {

        //常量
        public const bool IS_TOUCH_CONTROL = false;

        //自己的组件
        private TouchMove m_touchMove = null;
       
        //private EventMgr m_eventMgr = null;
        //private GameLogicMgr m_gameMgr = null;

        //private ActorObj m_SelectTargetObject = null;

        //protected PlayerHeath m_heath = null;


        protected Dictionary<int, int> m_skillBindsDict = new Dictionary<int, int>();

        public Dictionary<int, int> SkillBindsDict
        {
            get { return m_skillBindsDict; }
            set { m_skillBindsDict = value; }
        }
 

        private int m_loadIndex;
        public int LoadIndex
        {
            set
            {
                m_loadIndex = value;
            }
            get { return m_loadIndex; }
        }

       
        public void PlayerMove()
        {

            
        }

        public override bool IsCanAttack(ActorObj obj)
        {
            /*
            switch (PKStatus)
            {
                case (int)EPKMode.M_PK_Peace:
                    return false;
                case (int)EPKMode.M_PK_Camp:
                    return !(faction == Faction);
                case (int)EPKMode.M_PK_All:
                    return true;
                case (int)EPKMode.M_PK_Evil:
                    //todo
                    return false;
                default:
                    return false;
            }*/
            return true;
        }

        public override void Recover()
        {
            

        }

        //生物id
        public override void Init(int resID, int configID, long entityID, string strEnterAction = "", bool isNpc = false)
        {
            base.Init(resID, configID, entityID, strEnterAction, isNpc);        //此函数会读取配置表修改mActorType
            mActorType = ActorType.AT_REMOTE_PLAYER;

            m_bodyType = 3;
            m_TeamType = 3;
            //add by Alex 20150324 使用这个接口修改本地玩家类型为无人控制的pvp玩家


            if (isNpc)
            {
                mActorType = ActorType.AT_PVP_PLAYER;
                TeamType = 3;  //敌对阵营
            }

            if (ArenaMgr.Instance.IsArenaFight)
            {
                m_move = this.gameObject.GetComponent<ServerMoveAgent>();
                if (m_move == null)
                {
                    m_move = this.gameObject.AddComponent<ServerMoveAgent>();
                }
                m_move.DoInit(this);
            }
            else
            {
                m_move = this.gameObject.GetComponent<ServerMoveAgent>();
                if (m_move == null)
                {
                    m_move = this.gameObject.AddComponent<ServerMoveAgent>();
                }
                m_move.DoInit(this);
            }

            mBaseAttr.Speed = 4.0f;

            SetSpeed(mBaseAttr.Speed);

      
            OnPostInit();

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                m_touchMove = this.gameObject.AddComponent<TouchMove>();
                m_touchMove.SetCallbackMoveFun(MoveToPos);
                m_touchMove.enabled = false;
            }
            mHealth.OnCreateHPBar();

            CommonTools.SetLayer(this.gameObject, LayerMask.NameToLayer("player"));
        }

        public void InitAttr(MsgData_sSceneObjectEnterHuman es)
        {
            mBaseAttr.InitOtherPlayerAttr(es);
            mPKStatus = es.PKStatus;
            Faction = es.Faction;
            if (mHealth != null)
            {
                mHealth.OnResetHP();
            }
        }

        public void InitEnterHumanAttr(MsgData_sSceneObjectEnterVirtualPlayer es)
        {
            mBaseAttr.InitEnterVirtualAttr(es);
            if (mHealth != null)
            {
                mHealth.OnResetHP();
            }
        }

        public void InitPlayerPVPAgent()
        {
            m_actorAI = this.gameObject.GetComponent<ActorAgent>();
            if (m_actorAI)
            {
                m_actorAI.enabled = false;
            }

            //m_actorAI = this.gameObject.GetComponent<PlayerPvpAgent>();
            //if (m_actorAI == null)
            //{
            //    m_actorAI = this.gameObject.AddComponent<PlayerPvpAgent>();
            //}
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
           

            Random.seed = System.Guid.NewGuid().GetHashCode();        

            //注册事件
            RegisterEvent();


            //脚步声
            InvokeRepeating("PlayerRunSound", 1, 0.1f);

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


           
        }


        //// Update is called once per frame
        //public override void Update()
        //{
        //    base.Update();

        //    if (NavGate)
        //    {
        //        return;
        //    }

         
        
        //}

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveEvent();
        }

        //快捷键处理
        bool OnAccelerator()
        {
            return false;
        }

        void UpdateSelectTarget()
        {
            
        }
 

      

        void AutoCancelTarget()
        {
            CancelInvoke("AutoCancelTarget");
            SelectTarget(null);
        }

        //private bool PlayerMoveToDir(Vector3 dir)
        //{
        //    m_isJoystickMove = true;
           

        //    base.MoveToDir(dir);

        //    return true;
        //}

        //private void PlayerStopMove()
        //{
        //    m_isJoystickMove = false;

        //    base.StopMove(true);
        //}

        public void LookAtDir()
        {
           
            //m_move.LookAtDir();
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


            m_eventMgr.AddListener(GameEvent.GE_BOSS_SKILL, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_BOSS_SKILL_END, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_CC_GameGraphicSetting, OnChangeGrphicQuality);
             
        }

        void RemoveEvent()
        {
            //lmjedit  主界面 destroy 这个模型时 将被调用
            if (m_eventMgr != null)
            {
                m_eventMgr.RemoveListener(GameEvent.GE_PLAYER_RECOVER, EventFunction);
                m_eventMgr.RemoveListener(GameEvent.GE_PLAYER_RESET_HP, EventFunction);

                m_eventMgr.RemoveListener(GameEvent.GE_BOSS_SKILL, EventFunction);
                m_eventMgr.RemoveListener(GameEvent.GE_BOSS_SKILL_END, EventFunction);
                m_eventMgr.RemoveListener(GameEvent.GE_CC_GameGraphicSetting, OnChangeGrphicQuality);
                 
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

        public void ShowModel()
        {
            mVisiable = true;
            ShowSelf();
            ShowBlobShadow();
            ShowEffect();
            ShowHorse();
            ShowWing();
            ShowMagic();
            ShowZhenFa();

            List<ActorObj> pets = CoreEntry.gActorMgr.GetAllPetActors();
            for (int i = 0; i < pets.Count; i++)
            {
                PetObj pet = pets[i] as PetObj;
                if (null != pet && pet.m_MasterServerID == serverID)
                {
                    pet.ShowSelf();

                    break;
                }
            }
        }

        public void HideModel()
        {
            mVisiable = false;
            HideSelf();
            HideBlobShadow();
            HideEffect();
            HideHorse();
            HideWing();
            HideMagic();
            HideZhenFa();

            List<ActorObj> pets = CoreEntry.gActorMgr.GetAllPetActors();
            for (int i = 0; i < pets.Count; i++)
            {
                PetObj pet = pets[i] as PetObj;
                if (null != pet && pet.m_MasterServerID == serverID)
                {
                    pet.HideSelf();

                    break;
                }
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

                if (mHealth != null)
                {
                    mHealth.OnResetHP();
                }
            }
       


        }

        public override void ForceToRevive()
        {
            base.ForceToRevive();
            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_STAND;
            param.skillID = 1;

            EnterState(param);       

            //todo，属性恢复  
            Recover(false);
            if (mHealth != null)
            {
                mHealth.OnCreateHPBar();
            }
        }



        void PlayerRunSound()
        {
           
            if (curActorState != ACTOR_STATE.AS_RUN)
            {
                return;
            }

            if (IsPlayingSound())
            {
                return;
            }

            bool isFloorGround = true;

            //随机脚本,根据材质使用脚本声音
            Vector3 rayPos = transform.position + new Vector3(0, 5, 0);

            RaycastHit beHit;
            int layerMask = 1 << LayerMask.NameToLayer("ground");
            if (!Physics.Raycast(rayPos, -Vector3.up, out beHit, 200, layerMask))
            {
                //主角浮空状态
                return;
            }


            string sound = "";
            if (isFloorGround)
            {
                int index = Random.Range(1, 8);
                sound = @"Sound/player/step_stone_0" + index;
                return;   //屏蔽走路音效没资源
            }
            else
            {
                int whichFoot = Random.Range(0, 2);

                //Random.seed = System.Guid.NewGuid().GetHashCode();           
                int index = Random.Range(1, 4);
                if (whichFoot == 0)
                {
                    sound = @"Sound/player/step_wood_L" + index;
                }
                else
                {
                    sound = @"Sound/player/step_wood_R" + index;
                }
            }

            if (!IsPlayingSound())
            {
                PlaySound(sound);
            }
        }

        public override float GetDeathDuration()
        {
            return 1.0f;
        }

        /// <summary>
        /// PK模式。
        /// </summary>
        private int mPKStatus;

        /// <summary>
        /// 获取角色的PK模式。
        /// </summary>
        public int PKStatus
        {
            get { return mPKStatus; }
        }

        public override void ChangePKStatus(int value)
        {
            if (mPKStatus == value)
            {
                return;
            }
            
            mPKStatus = value;
            mHealth.OnPKModeStatus();
        }
    }

};  //end SG

