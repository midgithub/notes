using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{

    //玩家组件
[Hotfix]
    public class PetObj : ActorObj
    {
        public ActorObj m_MasterActor = null;     //宠物主人
        public long m_MasterServerID = 0;
        public GameObject m_rarityHalo = null; //魔神脚下光环
        public List<int> m_skillList = new List<int>();   //技能列表

        private float m_CurSpeed = 0.0f;
        private bool m_IsLerp = false;
        private float m_TimeParam = 0f;
        private float m_TimeScale = 0f;

        /// <summary>
        /// 
        /// </summary>
        protected LuaTable m_PetConfig;
        public XLua.LuaTable PetConfig
        {
            get { return m_PetConfig; }
            set { m_PetConfig = value; }
        }

        private int m_starlevel = 0;
        public int Starlevel
        {
            get { return m_starlevel; }
            set { m_starlevel = value; }
        }


        private int m_Qua = 0;
        public int Qua
        {
            get { return m_Qua; }
            set { m_Qua = value; }
        }

        public string mOwnName = "";
        public string OwnName
        {
            get { return mOwnName; }
            set { mOwnName = value; }
        }

        public behaviac.Agent PetAgent
        {
            get
            {
                if (m_actorAI == null)
                {
                    m_actorAI = this.gameObject.GetComponent<PetAgent>();
                }
                return m_actorAI;
            }
        }


        //生物id
        public override void Init(int resID, int configID, long entityID, string strEnterAction = "", bool isNpc = false)
        {

            base.Init(resID, configID, entityID, strEnterAction, isNpc);
            m_bodyType = 3;
            m_TeamType = 1;
            mActorType = ActorType.AT_PET;
            m_PetConfig = ConfigManager.Instance.HeroConfig.GetHeroConfig(configID);

 
            string rarityHalo = m_PetConfig.Get<string>("rarityHalo");

            Object obj = CoreEntry.gResLoader.Load(rarityHalo);

            if (obj != null)
            {
                Transform bipTrans = transform.FindChild("E_Root");
                if(bipTrans != null)
                {
                    UiUtil.ClearAllChildImmediate(bipTrans);
                    m_rarityHalo = GameObject.Instantiate(obj) as GameObject;

                    m_rarityHalo.transform.parent = bipTrans;
                    m_rarityHalo.transform.localPosition = Vector3.zero;
                    m_rarityHalo.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                    m_rarityHalo.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    m_rarityHalo.SetActive(true);
                     
                }
            }
            

            m_move = this.gameObject.GetComponent<ServerMoveAgent>();
            if (m_move == null)
            {
                m_move = this.gameObject.AddComponent<ServerMoveAgent>();
            }

            m_move.DoInit(this);

            OnPostInit();
            mHealth.OnCreateHPBar();

            //InitPetAgent();

            //m_SpeedParam = ConfigManager.Instance.Consts.GetValue<float>("BattlePetMove", "param");
        }

        public void InitSpeed(float speed)
        {
            mBaseAttr.Speed = speed;
            m_CurSpeed = speed;
            m_IsLerp = false;
        }

        public void InitPetAgent()
        {
            m_actorAI = this.gameObject.GetComponent<PetAgent>();
            if (m_actorAI)
            {
                m_actorAI.enabled = false;
            }
            else
            {
                m_actorAI = this.gameObject.AddComponent<PetAgent>();
            }

        }

        //public virtual void Awake()
        //{
        //    base.Awake();
        //}

        public void OnEnable()
        {
            Start();
        }

        public void Start()
        {
            //注册事件
            RegisterEvent();

        }

        public void SetMaster(ActorObj master)
        {
            if(null != master)
                m_MasterServerID = master.ServerID;
            m_MasterActor = master;
            this.Health.OnPetLevelChange();
            //Debug.LogError("ResetMaserName " + m_MasterServerID);

        }

        public void SetMaster(long ownerID)
        {
            m_MasterServerID = ownerID;
            m_MasterActor = CoreEntry.gActorMgr.GetPlayerActorByServerID(ownerID); 
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
            NavGate = true;
            m_AutoMove.MovePos(pos);
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();

            if (m_IsLerp)
            {
                if(null == mBaseAttr || m_CurSpeed == mBaseAttr.Speed)
                {
                    m_IsLerp = false;
                    m_TimeParam = 0f;

                    return;
                }

                m_TimeParam += Time.deltaTime;
                m_CurSpeed = Mathf.Lerp(m_CurSpeed, mBaseAttr.Speed, m_TimeParam * m_TimeScale);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveEvent();
        }

        public override float GetSpeed()
        {
            return m_CurSpeed;
        }
        public override void UpdatePetAttr(MsgData_sSceneObjHoneYanLevel attrs)
        {
            Starlevel = attrs.Level;
            Qua = attrs.Qua;
            OwnName = UiUtil.GetNetString(attrs.RoleName);
            if (mHealth != null)
            {
                mHealth.OnPetLevelChange();
            }

        }
        public override void UpdateAttr(List<MsgData_sClientAttr> attrs)
        {
            base.UpdateAttr(attrs);

            m_IsLerp = false;
            if (null != mBaseAttr)
            {
                m_IsLerp = mBaseAttr.Speed != m_CurSpeed;
                m_TimeScale = Mathf.Abs(mBaseAttr.Speed - m_CurSpeed) * 0.5f;
                m_TimeParam = 0f;
            }
        }

        void RegisterEvent()
        {
            RemoveEvent();


            //m_eventMgr.AddListener(GameEvent.GE_EVENT_AFTER_ALL_LOADED_OF_SCENE, EventFunction);
        }

        void RemoveEvent()
        {
            if (m_eventMgr != null)
            {
                //m_eventMgr.RemoveListener(GameEvent.GE_EVENT_AFTER_ALL_LOADED_OF_SCENE, EventFunction);
            }
        }

        void EventFunction(GameEvent ge, EventParameter parameter)
        {
            switch (ge)
            {
                case GameEvent.GE_PLAYER_RECOVER:
                    {
                        //if (parameter.intParameter != entityid)
                        //{
                        //    break;
                        //}
                        //StateParameter param = new StateParameter();
                        //param.state = ACTOR_STATE.AS_STAND;
                        //param.skillID = 1;

                        //bool ret = RequestChangeState(param);

                        //if (ret)
                        //{                  
                        //    Recover();
                        //}

                    }
                    break;


                default:
                    break;
            }
        }

        public void DoRevive()
        {


        }

        public override void ForceToRevive()
        {
        }

        public override float GetDeathDuration()
        {
            return 1.0f;
        }

    }

}

