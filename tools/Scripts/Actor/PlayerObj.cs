using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
namespace SG
{

    //玩家组件
    [LuaCallCSharp]
    [Hotfix]
    public class PlayerObj : ActorObj
    {
        float lastTime = 0.0f;

        //常量
        public const bool IS_TOUCH_CONTROL = false;

        //自己的组件
        private TouchMove m_touchMove = null;


        private CharacterController m_ch = null;

        //protected PlayerHeath m_heath = null;

        //private Vector3 m_curJoystickDir = Vector3.zero;

        public Dictionary<int, int> m_skillBindsDict = new Dictionary<int, int>();
        public Dictionary<int, int> SkillBindsDict
        {
            get { return m_skillBindsDict; }
            set { m_skillBindsDict = value; }
        }

        protected SkillGiftsMgr m_skillGiftsMgr = null;


        public bool m_bBossSkill = false;

        private bool m_bEscapeBoss = false;   //闪避BOSS

        public bool IsEscapeBoss
        {
            get { return m_bEscapeBoss; }
            set
            {
                m_bEscapeBoss = value;

                CancelInvoke("ResetEscapeBoss");
                if (m_bEscapeBoss == true)
                {
                    Invoke("ResetEscapeBoss", 4.0f);
                }
            }
        }

        private void ResetEscapeBoss()
        {
            m_bEscapeBoss = false;
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

        // 无敌设置(GM 模块)
        public bool m_bWuDi = false;


        public bool m_EditTest = false;



        //yy
        public bool m_bUseAI = false;

        /// <summary>
        /// 取消玩家目标距离。
        /// </summary>
        public float m_CancelPlayerTargetDis = 18;

        /// <summary>
        /// 取消怪物目标距离。
        /// </summary>
        public float m_CancelMonsterTargetDis = 18;

        /// <summary>
        /// 技能选择距离。
        /// </summary>
        public float m_SkillSelectDis = 10;


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


        public void setMainPlayer(bool isMainPlayer)
        {
            //点击移动
            //if ( IS_TOUCH_CONTROL )
            //{

            m_bAutoMove = !isMainPlayer;

            if (!IsMainPlayer())
            {
                return;
            }


            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                if (m_touchMove)
                    m_touchMove.enabled = isMainPlayer;
            }
            else
            {
                if (m_touchMove)
                    m_touchMove.enabled = false;
            }


            if (isMainPlayer)
            {
                CoreEntry.gJoystickMgr.SetMoveCallbackFunc(PlayerMoveToDir);
                CoreEntry.gJoystickMgr.SetStopCallbackFunc(PlayerStopMove);
            }

            InitAutoMove();



            setLightActive(isMainPlayer);

            if (isMainPlayer == true)
            {
                setAudioListenerEnabled(gameObject, true);

                AudioSource1.volume = 1.0f;
                AudioSourceBody.volume = 1.0f;
            }
            else
            {
                setAudioListenerEnabled(gameObject, false);
                AudioSource1.volume = 0.4f;
                AudioSourceBody.volume = 0.4f;
            }

        }

        public void setAudioListenerEnabled(GameObject gameObject, bool enabled)
        {
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
            // 天赋技能
            if (m_skillGiftsMgr == null)
            {
                m_skillGiftsMgr = new SkillGiftsMgr();
                m_skillGiftsMgr.Init(this, false);
            }
            m_skillGiftsMgr.Recover(this);
        }

        //生物id
        public override void Init(int resID, int configID, long entityID, string strEnterAction = "", bool isNpc = false)
        {
            //m_shadowType = GameGraphicSetting.IsLowQuality ? 1 : 2;
            m_shadowType = 1;

            InitAutoMove();
            base.Init(resID, configID, MainRole.Instance.serverID, strEnterAction, isNpc);
            mActorType = ActorType.AT_LOCAL_PLAYER;
            mBaseAttr.InitFromPlayerData();
            m_bodyType = 3;
            m_TeamType = 1;

            ServerID = MainRole.Instance.serverID;
            mDestPos = Vector3.zero;

            m_move = this.gameObject.GetComponent<MoveController>();
            if (m_move == null)
            {
                m_move = this.gameObject.AddComponent<MoveController>();
            }
            m_move.DoInit(this);

            //设置跑步速度 addby yuxj

            mBaseAttr.Speed = 4.0f;

            SetSpeed(mBaseAttr.Speed);

            m_skillGiftsMgr = null;

            //初始化后回调

            OnPostInit();


            ////通用逻辑要写到这行之上##########################
            if (isNpc || !IsMainPlayer())
            {
                return;
            }

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                m_touchMove = this.gameObject.AddComponent<TouchMove>();
                m_touchMove.enabled = false;
            }


            ReBindSkill();
            mHealth.OnCreateHPBar();

            //常量表读取三个距离配置
            m_CancelPlayerTargetDis = ConfigManager.Instance.Consts.GetValue<float>("playerDistance", "val1");
            m_CancelMonsterTargetDis = ConfigManager.Instance.Consts.GetValue<float>("monsterDistance", "val1");
            m_SkillSelectDis = ConfigManager.Instance.Consts.GetValue<float>("attackDistance", "val1");

            CommonTools.SetLayer(this.gameObject, LayerMask.NameToLayer("mainplayer"));
        }

        public void ReBindSkill()
        {
            m_skillBindsDict.Clear();
            Dictionary<int, int>.Enumerator iter = PlayerData.Instance.SkillData.Skills.GetEnumerator();
            while (iter.MoveNext())
            {
                int groundid = iter.Current.Key;
                int skillid = iter.Current.Value;

                LuaTable groupCfg = ConfigManager.Instance.Skill.GetSkillGroupIndexConfig(groundid);
                if (groupCfg != null && groupCfg.Get<int>("bindSkillButtonID") > 0)
                {
                    m_skillBindsDict[groupCfg.Get<int>("bindSkillButtonID")] = skillid;
                }
            }
        }

        public override bool MoveToPos(Vector3 pos, bool bChangeState = true)
        {
            if (mDestPos != Vector3.zero && mDestPos != pos)
            {
                HideMoveArrow();
                mDestPos = Vector3.zero;
            }

            return base.MoveToPos(pos, bChangeState);
        }

        public void InitPlayerPVPAgent()
        {
            m_actorAI = this.gameObject.GetComponent<ActorAgent>();
            if (m_actorAI)
            {
                m_actorAI.enabled = false;
            }
          
        }

        void OnEnable()
        {
            Start();
        }

        void Start()
        {
            lastTime = Time.time;
            m_ch = this.gameObject.GetComponent<CharacterController>();
            Random.seed = System.Guid.NewGuid().GetHashCode();


            //注册事件
            RegisterEvent();
            

            //脚步声
            InvokeRepeating("PlayerRunSound", 1, 0.1f);

            if (m_skillGiftsMgr == null)
            {
                m_skillGiftsMgr = new SkillGiftsMgr();
                m_skillGiftsMgr.Init(this, false);
            }
            m_skillGiftsMgr.OnEnter(this);
        }

        public void RebindSkillButton()
        {
            //设置技能
            foreach (KeyValuePair<int, int> e in m_skillBindsDict)
            {
                ModuleServer.MS.GSkillCastMgr.SetBtnSkillID(EntityID, e.Key, e.Value);
            }
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
            //  selObject = actor.AutoSelTarget(m_curSkillID);
            base.Update();

            if (NavGate)
            {
                return;
            }

            //快捷键处理
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                //OnAccelerator();
            }

            //有目标时定时检测目标是否死亡或脱离视野
            if (m_SelectTargetObject != null)
            {
                if (Time.time - lastTime > 1)
                {
                    UpdateSelectTarget();
                    lastTime = Time.time;
                }
            }

            if (curActorState != ACTOR_STATE.AS_RUN)
            {
                HideMoveArrow();
            }

            CheckMovePosition();

            CheckRunState();
        }

        private string mRideClip = string.Empty;
        public string RideClip
        {
            get
            {
                if(string.IsNullOrEmpty(mRideClip))
                {
                    AudioCore.GenerateAudio(200006, ref mRideClip);
                }

                return mRideClip;
            }
        }
        private string mRunClip = string.Empty;
        public string RunClip
        {
            get
            {
                if(string.IsNullOrEmpty(mRunClip))
                {
                    AudioCore.GenerateAudio(200005, ref mRunClip);
                }

                return mRunClip;
            }
        }

        private float lastCheckTime = 0f;
        private byte RunType = 0;
        private void CheckRunState()
        {
            if (Time.time - lastCheckTime > 0.2f)
            {
                lastCheckTime = Time.time;
                if (curActorState == ACTOR_STATE.AS_RUN)
                {
                    if (IsRiding && RunType != 1)
                    {
                        RunType = 1;
                        PlaySound(RideClip);

                        AudioSource1.loop = true;
                    }
                    else if (!IsRiding && IsWing && RunType != 2)
                    {
                        RunType = 2;
                        AudioSource1.loop = false;
                        AudioSource1.Stop();
                    }
                    else if (!IsRiding && !IsWing && RunType != 3)
                    {
                        RunType = 3;
                        PlaySound(RunClip);

                        AudioSource1.loop = true;
                    }
                }
            }
        }

        public override void OnExitState(ACTOR_STATE state)
        {
            if (state == ACTOR_STATE.AS_RUN && curActorState == ACTOR_STATE.AS_RUN)
            {
                RunType = 0;
                if (AudioSource1 != null)
                {
                    AudioSource1.loop = false;
                    AudioSource1.Stop();
                }
            }
        }

        private Vector3 m_LastMovePosition = Vector3.zero;

        /// <summary>
        /// 检查移动位置。
        /// </summary>
        private void CheckMovePosition()
        {
            Vector3 pos = transform.position;
            Vector3 mv = pos - m_LastMovePosition;
            if (Mathf.Abs(mv.x) + Mathf.Abs(mv.z) > 1.0f)
            {
                m_LastMovePosition = pos;

                EventParameter param = EventParameter.Get();
                param.objParameter = m_LastMovePosition;
                m_eventMgr.TriggerEvent(GameEvent.GE_PLAYER_MOVE_POSITION, param);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveEvent();
        }

        //快捷键处理
        bool OnAccelerator()
        {
            if (m_EditTest)
            {
                bool bMove = false;
                if (Input.GetKey(KeyCode.W))
                {
                    transform.Translate(new Vector3(1.0f, 0.0f, 0f) * Time.deltaTime * 10);
                    bMove = true;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    transform.Translate(new Vector3(-1.0f, 0.0f, 0f) * Time.deltaTime * 10);
                    bMove = true;
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    transform.Translate(new Vector3(0.0f, 0.0f, 1.0f) * Time.deltaTime * 10);
                    bMove = true;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    transform.Translate(new Vector3(0.0f, 0.0f, -1.0f) * Time.deltaTime * 10);
                    bMove = true;
                }
                if (bMove)
                {
                    Vector3 rayPos = transform.position + new Vector3(0, 5, 0);

                    RaycastHit beHit;
                    int layerMask = 1 << LayerMask.NameToLayer("ground");

                    if (Physics.Raycast(rayPos, -Vector3.up, out beHit, 20, layerMask))
                    {
                        //Transform mTransform = transform;
                        transform.position = new Vector3(transform.position.x, beHit.transform.position.y, transform.position.z);
                    }

                }
            }
            else
            if (IsMainPlayer())
            {
                if (IsDeath())
                {
                    return false;
                }

                if (Input.GetKey(KeyCode.W))
                {
                    //StateParameter param = new StateParameter();
                    //param.state = ACTOR_STATE.AS_RUN;

                    //RequestChangeState(param);

                    MoveToDir(transform.forward.normalized);
                }

                if (Input.GetKeyUp(KeyCode.W))
                {
                    //if (curActorState == ACTOR_STATE.AS_RUN)
                    //{
                    //    StateParameter param = new StateParameter();
                    //    param.state = ACTOR_STATE.AS_STAND;

                    //    RequestChangeState(param);
                    //}
                }

                if (Input.GetKey(KeyCode.W) && curActorState == ACTOR_STATE.AS_RUN)
                {
                    //m_move.FaceTo(transform.position + transform.forward.normalized);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    transform.Rotate(Vector3.up, -100 * Time.deltaTime);
                    return true;
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    transform.Rotate(Vector3.up, -100 * Time.deltaTime);
                    return true;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    transform.Rotate(Vector3.up, 100 * Time.deltaTime);
                    return true;
                }
                else if (Input.GetKeyUp(KeyCode.Z))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查技能目标，没有则选一个。
        /// </summary>
        public ActorObj CheckSkillTarget()
        {
            if (m_SelectTargetObject != null)
            {
                return m_SelectTargetObject;
            }

            ActorObj obj = AutoSelTarget(m_SkillSelectDis);
            if (obj == null)
            {
                return null;
            }
            SelectTarget(obj);
            return m_SelectTargetObject;
        }

        /// <summary>
        /// 更新选择的目标，目标死亡或离开一定范围后取消。
        /// </summary>
        void UpdateSelectTarget()
        {
            if (m_SelectTargetObject != null)
            {
                if (m_SelectTargetObject.IsDeath())
                {
                    SelectTarget(null);
                    return;
                }

                float distance = Vector3.Distance(transform.position, m_SelectTargetObject.transform.position);
                float maxdis = m_SelectTargetObject.mActorType == ActorType.AT_REMOTE_PLAYER ? m_CancelPlayerTargetDis : m_CancelMonsterTargetDis;
                if (distance >= maxdis)
                {
                    SelectTarget(null);
                }
            }
        }
        
        /// <summary>
        /// 选择了目标。
        /// </summary>
        /// <param name="oldobj">原来的目标。</param>
        /// <param name="newobj">新的目标。</param>
        public override void OnSelectTarget(ActorObj oldobj, ActorObj newobj)
        {
            //取消选中标记和血条
            if (oldobj != null)
            {
                if (oldobj is MonsterObj)
                {
                    oldobj.Health.OnRemoveHPBar();
                }
                if(newobj != null)
                {
                    CoreEntry.gSkillMgr.HideSelectTag();
                }
            }

            //设置选中标记和显示血条
            if (newobj != null)
            {
                bool att = false;
                if (newobj is MonsterObj)
                {
                    //怪物可攻击
                    att = true;
                    newobj.Health.OnCreateHPBar();
                }        
                else if (newobj.mActorType == ActorType.AT_REMOTE_PLAYER)
                {
                    //玩家判断PK状态
                    att = PlayerData.Instance.IsCanAttack(newobj as OtherPlayer);
                }
                else
                {
                    //其它都不可攻击
                    att = false;
                }

                if(newobj.mActorType == ActorType.AT_MECHANICS)
                {
                    //不显示选中光圈, 镖车类型，使用机械类
                }else
                {
                    //显示选中圈    
                    GameObject obj = CoreEntry.gSkillMgr.GetSelectTag(att);
                    Transform t = obj.transform;
                    obj.SetActive(true);
                    t.SetParent(newobj.transform);
                    t.localPosition = new Vector3(0, 0.1f, 0);
                    t.localScale = Vector3.one;
                    t.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
            if (null!= newobj && newobj.mActorType == ActorType.AT_MECHANICS)
            {
                //不抛出选中事件, 镖车类型，使用机械类
            }
            else
            {
                EventParameter ep = EventParameter.Get();
                ep.objParameter = newobj;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SELECT_TARGET, ep);
            }
        }

        //选中monster
        public override void SelectTarget(ActorObj selObject)
        {
            ActorObj oldactor = m_SelectTargetObject;
            m_SelectTargetObject = selObject;
            OnSelectTarget(oldactor, m_SelectTargetObject);
        }

        /// <summary>
        /// 查找目标。
        /// </summary>
        /// <param name="dis">查找范围。</param
        /// <returns>查找的目标对象。</returns>
        public ActorObj FindTarget(float dis, int mid = 0)
        {
            //如果已经有目标了
            UpdateSelectTarget();
            if (mid != 0 && m_SelectTargetObject != null)
            {
                if (m_SelectTargetObject.mActorType == ActorType.AT_MONSTER || m_SelectTargetObject.mActorType == ActorType.AT_BOSS)
                {
                    if (mid == (m_SelectTargetObject as MonsterObj).ConfigID)
                    {
                        return m_SelectTargetObject;
                    }
                }
            }

            float minDistance = Mathf.Min(dis, 100000.0f);
            ActorObj selectActor = null;
            if (PlayerData.Instance.CurPKMode != PKMode.PK_MODE_PEACE)
            {
                //非和平模式优先搜索玩家
                selectActor = GetNearestActor(CoreEntry.gActorMgr.GetAllPlayerActors(), ref minDistance);
            }
            if (selectActor == null)
            {
                selectActor = GetNearestActor(CoreEntry.gActorMgr.GetAllMonsterActors(), ref minDistance);
            }
            SelectTarget(selectActor);
            return selectActor;
        }


        //自动选择一个最近的
        public override ActorObj AutoSelTarget(float fSelectDist)
        {
            float minDistance = Mathf.Min(fSelectDist, 100000.0f);
            ActorObj selectActor = null;
            if (PlayerData.Instance.CurPKMode != PKMode.PK_MODE_PEACE)
            {
                //非和平模式优先搜索玩家
                selectActor = GetNearestActor(CoreEntry.gActorMgr.GetAllPlayerActors(), ref minDistance);
            }
            if (selectActor == null)
            {
                selectActor = GetNearestActor(CoreEntry.gActorMgr.GetAllMonsterActors(), ref minDistance);
            }

            return selectActor;
        }

        /// <summary>
        /// 获取最近的角色。
        /// </summary>
        /// <param name="objList">角色列表。</param>
        /// <param name="dis">距离限制。</param>
        /// <returns>最近的角色。</returns>
        public ActorObj GetNearestActor(List<ActorObj> objList, ref float dis, int mid = 0)
        {
            ActorObj selectActor = null;
            for (int i = 0; i < objList.Count; i++)
            {
                //对IOS出现怪物不动 报错的异常  进行错误处理 隐身或死亡状态
                ActorObj actor = objList[i];
                if (null == actor)
                    continue;
                if (!GameLogicMgr.checkValid(actor.gameObject) || actor.IsInStealthState(this) || actor.IsDeath())
                {
                    continue;
                }
                
                //PK模式的玩家对象筛选
                if (actor.mActorType == ActorType.AT_REMOTE_PLAYER || actor.mActorType == ActorType.AT_LOCAL_PLAYER || actor.mActorType == ActorType.AT_MECHANICS)
                {
                    //排除玩家自身、宠物和不可攻击的玩家
                    if (actor.mActorType != ActorType.AT_REMOTE_PLAYER || !PlayerData.Instance.IsCanAttack(actor as OtherPlayer))
                    {
                        continue;
                    }
                }

                //距离最近
                float distance = Vector3.Distance(transform.position, actor.transform.position);
                if (distance > dis)
                {
                    continue;
                }

                if((actor is MonsterObj) && actor.IsSameCamp())
                {
                    continue;
                }

                if (mid != 0)
                {
                    if (actor.mActorType == ActorType.AT_MONSTER || actor.mActorType == ActorType.AT_BOSS)
                    {
                        MonsterObj monsterobj = actor as MonsterObj;
                        if (monsterobj.ConfigID == mid)
                        {
                            dis = distance;
                            selectActor = actor;
                        }
                    }
                    continue;
                }

                dis = distance;
                selectActor = actor;
            }

            return selectActor;
        }

        void AutoCancelTarget()
        {
            CancelInvoke("AutoCancelTarget");
            SelectTarget(null);
        }

        private bool PlayerMoveToDir(Vector3 dir)
        {
            HideMoveArrow();

            m_bRunToAttack = false;

             m_isJoystickMove = true;
            //m_curJoystickDir = dir;

            base.MoveToDir(dir);

            return true;
        }

        private void PlayerStopMove()
        {
            m_isJoystickMove = false;

            base.StopMove(true);
        }

        //获取碰撞体半径
        public override float GetColliderRadius()
        {
            if (m_ch != null)
            {
                return m_ch.radius * transform.localScale.x;
            }

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


            m_eventMgr.AddListener(GameEvent.GE_PLAYER_LV, OnLevelUp);

           // m_eventMgr.AddListener(GameEvent.GE_CC_TaskUpdate, OnTaskFinish);
            m_eventMgr.AddListener(GameEvent.GE_CC_GameGraphicSetting, OnChangeGrphicQuality);

            m_eventMgr.AddListener(GameEvent.GE_LOADSCENE_FINISH, OnSceneloaded);
        }

        public override bool IsCanAttack(ActorObj obj)
        {
            if (obj.mActorType == ActorType.AT_REMOTE_PLAYER || obj.mActorType == ActorType.AT_MECHANICS)
            {
                return PlayerData.Instance.IsCanAttack(obj as OtherPlayer);
            }
            return true;
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
                m_eventMgr.RemoveListener(GameEvent.GE_PLAYER_LV, OnLevelUp);

             //   m_eventMgr.RemoveListener(GameEvent.GE_CC_TaskUpdate, OnTaskFinish);

                m_eventMgr.RemoveListener(GameEvent.GE_CC_GameGraphicSetting, OnChangeGrphicQuality);

                m_eventMgr.RemoveListener(GameEvent.GE_LOADSCENE_FINISH, OnSceneloaded);
            }
        }

        void EventFunction(GameEvent ge, EventParameter parameter)
        {
            switch (ge)
            {
                case GameEvent.GE_PLAYER_RECOVER:
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
                            Recover();
                        }
                    }
                    break;
                case GameEvent.GE_PLAYER_RESET_HP:
                    {
                        //todo，属性恢复  --- 喝血瓶
                        Recover();

                    }
                    break;
                case GameEvent.GE_BOSS_SKILL:
                    {
                        m_bBossSkill = true;
                    }
                    break;
                case GameEvent.GE_BOSS_SKILL_END:
                    {
                        m_bBossSkill = false;

                        IsEscapeBoss = false;
                    }
                    break;
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
            if (ret)
            {
                Recover();
            }
        }

        public override void ForceToRevive()
        {
            base.ForceToRevive();
            StateParameter param = new StateParameter();
            param.state = ACTOR_STATE.AS_STAND;
            param.skillID = 1;
            EnterState(param);
            Recover(false);
            if (mHealth != null)
            {
                mHealth.OnCreateHPBar();
            }
        }



        void PlayerRunSound()
        {
            if (m_EditTest)
            {
                return;
            }

            if (curActorState != ACTOR_STATE.AS_RUN)
            {
                return;
            }

            //开始移动
            if (IsMainPlayer() && m_bBossSkill)
            {
                IsEscapeBoss = true;
            }


            if (IsPlayingSound())
            {
                return;
            }

            bool isFloorGround = true;

            ////随机脚本,根据材质使用脚本声音
            //Transform groundTransform = null;

            Vector3 rayPos = transform.position + new Vector3(0, 5, 0);

            RaycastHit beHit;
            int layerMask = 1 << LayerMask.NameToLayer("ground");

            if (Physics.Raycast(rayPos, -Vector3.up, out beHit, 200, layerMask))
            {
                //groundTransform = beHit.transform;
            }
            else
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

        void OnLevelUp(GameEvent ge, EventParameter parameter)
        {
            GameObject mLvupEffectObj = CoreEntry.gGameObjPoolMgr.Instantiate("Effect/skill/buff/fx_buff_shengji");
            if (null != mLvupEffectObj)
            {
                mLvupEffectObj.transform.parent = transform;
                mLvupEffectObj.transform.localPosition = Vector3.zero;
                mLvupEffectObj.transform.localRotation = Quaternion.identity;
                mLvupEffectObj.transform.localScale = Vector3.one;
                SceneEfxPool efx = mLvupEffectObj.GetComponent<SceneEfxPool>();

                if (efx == null)
                {
                    efx = mLvupEffectObj.AddComponent<SceneEfxPool>();
                }
                if (efx != null)
                {
                    CoreEntry.gAudioMgr.PlayUISound(900007, mLvupEffectObj);
                    efx.Init(transform.position, 1.5f);
                }
            }
        }
        public void ZhuanSheng()
        {
            GameObject mLvupEffectObj = CoreEntry.gGameObjPoolMgr.Instantiate("Effect/skill/buff/fx_buff_zhuansheng");
            if (null != mLvupEffectObj)
            {
                mLvupEffectObj.transform.parent = transform;
                mLvupEffectObj.transform.localPosition = Vector3.zero;
                mLvupEffectObj.transform.localRotation = Quaternion.identity;
                mLvupEffectObj.transform.localScale = Vector3.one;
                SceneEfxPool efx = mLvupEffectObj.GetComponent<SceneEfxPool>();

                if (efx == null)
                {
                    efx = mLvupEffectObj.AddComponent<SceneEfxPool>();
                }
                if (efx != null)
                {
                    //CoreEntry.gAudioMgr.PlayUISound(900007, mLvupEffectObj);
                    efx.Init(transform.position, 5f);
                }
            }
        }

        GameObject finishTaskEffectObj;
        void OnTaskFinish(GameEvent ge, EventParameter parameter)
        {
            TaskMgr.TaskEnum type = (TaskMgr.TaskEnum)parameter.intParameter;
            if(type == TaskMgr.TaskEnum.finish)    // 任务完成
            {                
                if(finishTaskEffectObj == null)
                {
                    finishTaskEffectObj = CoreEntry.gGameObjPoolMgr.Instantiate("Effect/ui/uf_wanchengrenwu");
                    PanelBase parentBase = CoreEntry.gMainPanelMgr.GetPanel("UIMain");
                    if(parentBase != null)
                    {
                        finishTaskEffectObj.transform.SetParent(parentBase.gameObject.transform);
                        finishTaskEffectObj.transform.localPosition = new Vector3(0,200,0);
                        finishTaskEffectObj.transform.localScale = Vector3.one;
                    }                
                }
                finishTaskEffectObj.SetActive(true);
                Invoke("HideTaskEffect",1.5f);
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

        void HideTaskEffect()
        {
            if(finishTaskEffectObj != null)
            {
                finishTaskEffectObj.SetActive(false);
            }
        }
        
        public override float GetDeathDuration()
        {
            return 1.0f;
        }

        /// <summary>
        /// 开始骑马。
        /// </summary>
        public void StartRide()
        {
            RideData rd = PlayerData.Instance.RideData;
            if (rd.RideState == 0)
            {
                ActorObj actor = CoreEntry.gActorMgr.GetPlayerActorByServerID(PlayerData.Instance.RoleID);
                PlayerObj player = actor as PlayerObj;
                if (player != null)
                {
                    player.FuckHorse(rd.RideID);
                }
                rd.RideState = 1;
            }
        }

        public override void ChangePKStatus(int value)
        {
            mHealth.OnPKModeStatus();
        }

        private Vector3 mDestPos;
        private GameObject arrowObj;
        public void ShowMoveArrow(Vector3 pos)
        {
            mDestPos = pos;
            if (null == arrowObj)
            {
                Object obj = CoreEntry.gResLoader.Load("Effect/scence/uf_zhilingjiantou");
                if (null != obj)
                {
                    arrowObj = GameObject.Instantiate(obj) as GameObject;
                }
            }

            if (null != arrowObj)
            {
                arrowObj.transform.position = pos;
                arrowObj.SetActive(true);
            }
        }

        public void HideMoveArrow()
        {
            if (null != arrowObj)
            {
                arrowObj.SetActive(false);
            }
        }

        public override ActorObj GetSelTarget()
        {
            if (ArenaMgr.Instance.IsArenaFight)
            {
                return CoreEntry.gActorMgr.GetPlayerActorByServerID(ArenaMgr.Instance.GetAttackID(serverID));
            }

            UpdateSelectTarget();
            CheckSkillTarget();
            return m_SelectTargetObject;
        }

        private void OnSceneloaded(GameEvent ge, EventParameter parameter)
        {
            //ShowEnterEfx();
        }

        void ShowEnterEfx()
        {
            GameObject enterFx = CoreEntry.gGameObjPoolMgr.Instantiate("Effect/scence/sf_effect/sf_playstart_vfx");
            if (null != enterFx)
            {
                enterFx.transform.parent = transform;
                enterFx.transform.localPosition = Vector3.zero;
                enterFx.transform.localRotation = Quaternion.identity;
                enterFx.transform.localScale = Vector3.one;
                SceneEfxPool efx = enterFx.GetComponent<SceneEfxPool>();

                if (efx == null)
                {
                    efx = enterFx.AddComponent<SceneEfxPool>();
                }
                if (efx != null)
                {
                    //CoreEntry.gAudioMgr.PlayUISound(900007, enterFx);
                    efx.Init(transform.position, 2.5f);
                }
            }
        }

        /// <summary>
        /// 装备改变特效。
        /// </summary>
        public static string EqupChangeEffect = "Effect/skill/remain/fx_zhuangzhuangbei";

        /// <summary>
        /// 显示装备改变。
        /// </summary>
        public void ShowEquipChange()
        {
            CommonTools.AddSubChild(gameObject, EqupChangeEffect);
        }
    }
}