using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using XLua;

namespace SG
{
    //每个技能模版都有这个组件。保存的是释放者的信息
    /*
     * 技能释放分2个阶段：吟唱+释放。只有吟唱成功后，才释放技能
     */

[Hotfix]
    public class SkillBase : MonoBehaviour
    {

        public ActorObj m_actor = null;
        private Transform m_transform = null;

        public LuaTable m_skillDesc = null;
        public LuaTable m_skilleffect = null;

        public GameObject efxObj = null;

        //技能释放结束时间
        private float m_overTime = 0;

        public float SkillOverTime
        {
            get { return m_overTime; }
            set { m_overTime = value; }
        }
        public bool IsOver = false;

        public int m_skillID = 0;

        public string m_strActionName = "";     //动作名    
        public float m_speed = 1;               //动作播放速度    
        public float m_startPlayTimeSet = 0;    //动作开始播放时间        

        private GameDBMgr m_gameDBMgr = null;

        //动作特效
        EfxAttachActionPool m_actionEfx = null;

        //带有挂点的特效
        List<GameObject> m_attachEfxObjectlist = new List<GameObject>();

        //技能元素object
        List<GameObject> m_skillCellObjectlist = new List<GameObject>();


        //伤害列表
        public List<ActorObj> m_AttackList = new List<ActorObj>();
        /// <summary>
        /// 技能打到的目标的引用
        /// </summary>
        public ActorObj m_hitActor = null;

        /// <summary>
        /// 是否有组合技能cell
        /// </summary>
        CompositedSkillCell compositedSkillCell = null;

        //吟唱时间
        private float m_prepareKeepTime = 0;


        private bool m_bIsAoe = true;

        public bool m_onlyShowSkillScope = false;

        public float prepareKeepTime
        {
            get { return m_prepareKeepTime; }
        }

        public bool m_bSendAttackEven = false;

        private bool shouldUpdatePositionToCastObj = true;

        public bool ShouldUpdatePositionToCastObj
        {
            get { return shouldUpdatePositionToCastObj; }
            set { shouldUpdatePositionToCastObj = value; }
        }

        //播放参数
        struct SkillPlayParam
        {
            public string action;
            public float startTime;
            public float speed;

            public string sound;
            public string voice;
            public string actionEfx;

            public string remainEfx;    //地表残留
        }

        void Awake()
        {
            m_transform = this.transform;
            m_gameDBMgr = CoreEntry.gGameDBMgr;



            //m_speed = 2;                 
        }

        // PoolManager LogMgr
        void OnDestroy()
        {
            CancelInvoke("Start");
        }

        // PoolManager
        void OnDisable()
        {
            m_bSendAttackEven = false;
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_EVENT_BEHIT, EventFunction);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_NOTIFY_HIDE_SKILL_SCOPE, EventFunction);
            CancelInvoke("Start");
            CancelInvoke("AutoLookatTarget");
            CancelInvoke("HideSelf");
            CancelInvoke("CastSkill");
            CancelInvoke("SkillEnd");
        }

        // PoolManager
        void OnEnable()
        {
            m_bSendAttackEven = false;
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EVENT_BEHIT, EventFunction);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_NOTIFY_HIDE_SKILL_SCOPE, EventFunction);

            CancelInvoke("Start");
            Invoke("Start", 0.000001f);
        }

        void EventFunction(GameEvent ge, EventParameter parameter)
        {
            if (m_actor == null)
                return;

            switch (ge)
            {
                case GameEvent.GE_NOTIFY_HIDE_SKILL_SCOPE:
                    {
                        if (parameter.intParameter == m_skillID && parameter.goParameter == m_actor.gameObject)
                        {
                            HideSkillScope();
                        }

                    }
                    break;

                default:
                    break;
            }
        }

        // Use this for initialization
        void Start()
        {
            IsOver = false;
            CancelInvoke("Start");

            if (m_onlyShowSkillScope)
                return;

            m_AttackList.Clear();
            compositedSkillCell = null;

            ShowBossSkillWarning();

            SkillClassDisplayDesc skillClass =
                m_gameDBMgr.GetSkillClassDisplayDesc(m_skillDesc.Get<int>("skillDisplayID"));
            if(skillClass == null)
            {
                Debug.LogError("===skillClass== is null===");
                return;
            }
            if (null != m_skilleffect && m_skilleffect.Get<int>("range") == (int)SkillRangeType.SKILL_TARGET)
            {
                m_bIsAoe = false;
            }

            if (skillClass.prepareStage == null || skillClass.prepareStage.open == false)
            {
                //直接进入释放技能阶段
                CastSkill();
                return;
            }

            //吟唱阶段
            bool isLocal = false;
            if (m_actor is PlayerObj || m_actor is PetObj)
            {
                isLocal = true;
            }
            for (int i = 0; i < skillClass.prepareStageDataList.Count; ++i)
            {
                if (null != m_actor && !isLocal)
                {
                    if (skillClass.prepareStageDataList[i] is MovePosAttackDesc)
                    {
                        continue;
                    }
                }

                //GameObject cellObj = Instantiate(
                //    CoreEntry.gResLoader.LoadResource(skillClass.prepareStageDataList[i].prefabPath)) as GameObject;
                GameObject cellObj = CoreEntry.gGameObjPoolMgr.InstantiateSkillCell(skillClass.prepareStageDataList[i].prefabPath);
                cellObj.transform.parent = transform;

                ISkillCell skillCell = cellObj.GetComponent<ISkillCell>();

                skillCell.Init(skillClass.prepareStageDataList[i], this);
                skillCell.SetAoeState(m_bIsAoe);

                AddSkillCell(cellObj);
            }

            if (skillClass.prepareStage.isLookAtTarget)
            {
                InvokeRepeating("AutoLookatTarget", 0f, 0.1f);
            }

            m_prepareKeepTime = skillClass.prepareStage.keepTime;

            //吟唱结束，进入释放阶段        
            Invoke("CastSkill", skillClass.prepareStage.keepTime);
            Syncm_transform();
        }

        // Update is called once per frame
        //void Update () {

        //}

        void LateUpdate()
        {
            Syncm_transform();
        }

        //吟唱阶段，自动转向目标
        void AutoLookatTarget()
        {
            ActorObj aimActorObj = m_actor.GetAttackObj();
            if (aimActorObj == null)
            {
                LogMgr.UnityLog("AutoLookatTarget() no target");
                return;
            }

            m_actor.FaceTo(aimActorObj.thisGameObject.transform.position);
        }

        public void Init(ActorObj attackObj, ActorObj beHitObj, int skillID, bool onlyShowSkillScope = false)
        {

            //-------------------Init---------------------
            m_actor = null;
            m_skillDesc = null;

            //技能释放结束时间
            m_overTime = 0;

            m_strActionName = "";     //动作名    
            m_speed = 1.0f;               //动作播放速度    
            m_startPlayTimeSet = 0;    //动作开始播放时间        

            m_actionEfx = null;

            //m_RemainEfx = null;

            m_attachEfxObjectlist.Clear();
            m_skillCellObjectlist.Clear();
            attachingBulletList.Clear();
            m_prepareKeepTime = 0;
            m_bIsAoe = true;
            //----------------------------------------------

            m_skillID = skillID;
            m_actor = attackObj;
            m_hitActor = beHitObj;

            m_skillDesc = m_actor.GetCurSkillDesc(m_skillID);
            m_skilleffect = null;
            if (null != m_skillDesc && m_skillDesc.Get<int>("e_type_1") != (int)SkillEffectType.CUSTOM)
            {
                m_skilleffect = ConfigManager.Instance.Skill.GetEffectConfig(m_skillDesc.Get<int>("effect_1"));
            }

            m_startPlayTimeSet = m_actor.m_actionStartPlayTime;

            m_onlyShowSkillScope = onlyShowSkillScope;
            shouldUpdatePositionToCastObj = true;

            SubSkill = false;

            //防止出现普攻打击频率出错的问题
            canMove = false;
            canBeBroke = true;
            IsOver = false;

            //跟技能释放者同步位置
            Syncm_transform();

        }

        //释放技能        
        void CastSkill()
        {
            CancelInvoke("CastSkill");
            CancelInvoke("AutoLookatTarget");

            // 如果不是激活状态不往下运行
            if (!gameObject.activeInHierarchy)
                return;

            //释放吟唱阶段的动作特效
            SendEvent(SkillCellEventType.SE_DESTROY_ACTION_EFX, null);

            //销毁吟唱阶段数据
            DestroyEfx();

            //播放动作，特效，声音        
            SkillPlayParam param = new SkillPlayParam();
            LuaTable skill_action = CoreEntry.gSkillMgr.GetSkillActon(m_skillID);
            if(skill_action == null)
            {
                Debug.LogError("skill_action is null...m_skillID = " + m_skillID);
                return;
            }
            if (skill_action != null)
            {
                param.action = skill_action.Get<string>("animation");

                param.actionEfx = skill_action.Get<string>("skilleffect");

                param.startTime = m_startPlayTimeSet;
                param.speed = m_speed;

                param.remainEfx = skill_action.Get<string>("remain");

            }

            //播放声音
            string sound1 = "";
            string sound2 = "";
            AudioCore.GenerateAudio(m_skillDesc.Get<int>("sound_id"), ref sound1);
            AudioCore.GenerateAudio(m_skillDesc.Get<int>("talk_sound_id"), ref sound2);

            param.sound = sound1;
            param.voice = sound2;
              
            StartCoroutine(PlayActionEfxSound(param, skill_action.Get<float>("skillEfxDelay")));

            //技能元素
            SkillClassDisplayDesc skillClass = m_gameDBMgr.GetSkillClassDisplayDesc(m_skillDesc.Get<int>("skillDisplayID"));
            if (skillClass == null)
            {
                Debug.LogError("===skillClass== is null===");
                return;
            }
            bool isLocal = false;
            if (m_actor is PlayerObj || m_actor is PetObj)
            {
                isLocal = true;
            }
            for (int i = 0; i < skillClass.castStageDataList.Count; ++i)
            {
                if (null != m_actor && !isLocal)
                {
                    if (skillClass.castStageDataList[i] is MovePosAttackDesc)
                    {
                        continue;
                    }
                }

                GameObject cellObj = CoreEntry.gGameObjPoolMgr.InstantiateSkillCell(skillClass.castStageDataList[i].prefabPath);
                cellObj.transform.parent = transform;

                ISkillCell skillCell = cellObj.GetComponent<ISkillCell>();

                skillCell.Init(skillClass.castStageDataList[i], this);
                skillCell.SetAoeState(m_bIsAoe);

                CheckSkillCell(skillCell);
                AddSkillCell(cellObj);
            }

            //技能结束时间                
            if (m_skillDesc.Get<int>("skill_end") > 0)
            {
                m_overTime = (m_skillDesc.Get<int>("skill_end") / 1000f - m_startPlayTimeSet) / m_speed;
            }
            else
            {
                m_overTime = (m_actor.GetActionLength(m_strActionName) - m_startPlayTimeSet) / m_speed;
            }

            //如果是不可打破机能，释放出后恢复动作
            if (canBeBroke == false)
            {
                float time = (m_actor.GetActionLength(m_strActionName) - m_startPlayTimeSet) / m_speed;
                CancelInvoke("SkillAnimationIsOver");
                Invoke("SkillAnimationIsOver", time);
            }
            LogMgr.UnityLog("overtime="+m_overTime+", skillid="+m_skillID);

            //技能结束
            Invoke("SkillEnd", m_overTime);
        }

        void SkillAnimationIsOver()
        {
            if (m_actor.enabled && SubSkill == false)
            {
                //通知技能结束            
                m_actor.SkillEnd(0);
            }
        }

        //播放动画，特效，声音                
        IEnumerator PlayActionEfxSound(SkillPlayParam param, float delayTime)
        {
            if (m_actor == null)
                yield  break; 
            m_strActionName = param.action;
            if (m_strActionName != null && m_strActionName.Length > 0)
            {
                //动作带位移 的 不能延迟播放,  延迟播放将影响 位移曲线计算
                m_actor.PlayAction(m_strActionName, false);
                m_actor.SetActionTime(m_strActionName, param.startTime);
                m_actor.SetSkillActionSpeed(m_strActionName, param.speed, m_skillDesc);
            }

            //其它玩家不播放了
            if (m_actor.mActorType == ActorType.AT_REMOTE_PLAYER)
            {
                yield break;
            }

            if (m_actor.mActorType == ActorType.AT_PET)
            {
                PetObj pet = m_actor as PetObj;
                if (null != pet && pet.m_MasterActor != CoreEntry.gActorMgr.MainPlayer)
                {
                    yield break;
                }
            }

            //播放声音
            string sound1 = param.sound;
            string sound2 = param.voice;
            if (sound1 != null && sound1.Length > 0)
            {
                m_actor.StopSound();
                m_actor.PlaySound(sound1);
            }

            if (sound2 != null && sound2.Length > 0)
            {
                m_actor.StopSound2();
                m_actor.PlaySound2(sound2);
            }

            LuaTable skill_action = CoreEntry.gSkillMgr.GetSkillActon(m_skillID);
            if(skill_action == null)
            {
                Debug.LogError("==skill_action == is null m_skillID = "+m_skillID);
                yield break;
            }
            Vector3 efxPos = m_actor.transform.position;
            bool isBind = skill_action.Get<bool>("isBind");
            int bindType = skill_action.Get<int>("skillEfxPos");
            if (!isBind)
            {
                if (bindType == 1)
                {
                    ActorObj target = m_actor.GetSelTarget();
                    if (null != target)
                    {
                        efxPos = target.transform.position;
                    }
                }
            }

            if (delayTime > 0.0001f)
            {
                yield return new WaitForSeconds(delayTime);

            }

            //if (ByteToString.toString(m_skillDesc.szAttackEfxPrefab).Length > 0)
            if (param.actionEfx != null && param.actionEfx.Length > 0)
            {

                if (/*m_skillDesc.skilltype == 0 && */m_actor.mActorType == ActorType.AT_BOSS)
                {
                    yield return new WaitForSeconds(m_actor.NoramalAttackEffectDelayTime);
                }
                if (skill_action != null)
                {
                    param.actionEfx = skill_action.Get<string>("skilleffect");
                    //efxObj = Instantiate(CoreEntry.gResLoader.LoadResource(param.actionEfx)) as GameObject;//CoreEntry.gGameObjPoolMgr.InstantiateEffect(param.actionEfx);
                    efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(param.actionEfx);
                    if (efxObj == null)
                    {
                        LogMgr.LogError("找不到attackEfxPrefab：" + skill_action.Get<string>("skilleffect") + " " + m_skillDesc.Get<string>("name"));
                    }
                }

                float maxEfxTime = 0;
                if (skill_action.Get<float>("skillEfxLength") > 0)
                {
                    maxEfxTime = skill_action.Get<float>("skillEfxLength");
                }


                //LogMgr.UnityLog("maxEfxTime=" + maxEfxTime + ", " + m_strActionName);

                //特效存在时间
                if (maxEfxTime <= 0.001)
                {
                    maxEfxTime = m_actor.GetActionLength(m_strActionName);

                    if (maxEfxTime <= 0.001)
                    {
                        LogMgr.UnityError("技能 动作时间 没有配置 skillID:" + m_skillDesc.Get<int>("id"));
                    }
                }


                bool isFollowMove = false;
                EfxAttachActionPool efx = efxObj.GetComponent<EfxAttachActionPool>();
                if (efx == null)
                    efx = efxObj.AddComponent<EfxAttachActionPool>();

                if (efx != null)
                {
                    efx.Init(m_actor.transform, maxEfxTime, isFollowMove);

                    m_actionEfx = efx;

                    if (isBind)
                    {
                        ActorObj bindActor = null;
                        if (bindType == 0)
                        {
                            bindActor = m_actor;
                        }
                        else
                        {
                            bindActor = m_actor.GetSelTarget();
                        }

                        Transform bindTran = null;
                        if (null != bindActor)
                        {
                            string hangPoint = skill_action.Get<string>("hangPoint");
                            if (!string.IsNullOrEmpty(hangPoint))
                            {
                                bindTran = bindActor.transform.FindChild(hangPoint);
                            }
                            if (null == bindTran)
                            {
                                bindTran = bindActor.transform;
                            }
                        }
                        else
                        {
                            bindTran = m_actor.transform;
                        }

                        m_actionEfx.transform.parent = bindTran;
                        m_actionEfx.transform.localPosition = Vector3.zero;
                        m_actionEfx.transform.localScale = Vector3.one;
                        m_actionEfx.transform.rotation = m_actor.transform.rotation;
                    }
                    else
                    {
                        if (bindType == 1)
                        {
                            m_actionEfx.transform.position = efxPos;
                        }
                        else
                        {
                            m_actionEfx.transform.position = m_actor.transform.position;
                        }
                        m_actionEfx.transform.localScale = Vector3.one;
                        m_actionEfx.transform.rotation = m_actor.transform.rotation;
                    }

                    //设置有挂点的特效            
                    Transform[] childTransform = efxObj.GetComponentsInChildren<Transform>();
                    //foreach (Transform childTrans in childTransform)
                    for (int i = 0; i < childTransform.Length; ++i)
                    {
                        Transform childTrans = childTransform[i];
                        EfxSetAttachPoint setAttach = childTrans.gameObject.GetComponent<EfxSetAttachPoint>();
                        if (setAttach == null || setAttach.m_attachPointEnum == AttachPoint.E_None)
                        {
                            continue;
                        }

                        setAttach.Init(false);

                        Transform parent = m_actor.GetChildTransform(setAttach.m_attachPointEnum.ToString());
                        if (parent != null)
                        {
                            childTrans.parent = parent;
                            childTrans.localPosition = Vector3.zero;
                            childTrans.localRotation = Quaternion.identity;
                            childTrans.localScale = Vector3.one;

                            m_attachEfxObjectlist.Add(childTrans.gameObject);
                        }
                    }

                    //影子
                    ghostMesh[] ghostMesh = efx.GetComponentsInChildren<ghostMesh>();

                    if (ghostMesh.Length > 0)
                    {
                        SkinnedMeshRenderer MianSkinMesh = m_actor.m_skinnedMeshRenderer[0];
                        for (int i = 0; i < m_actor.m_skinnedMeshRenderer.Length; ++i)
                        {
                            if (m_actor.m_skinnedMeshRenderer[i].name.Contains("weapon"))
                            {
                                continue;
                            }
                            MianSkinMesh = m_actor.m_skinnedMeshRenderer[i];
                        }

                        for (int i = 0; i < ghostMesh.Length; ++i)
                        {
                            ghostMesh[i].characterMesh[0] = MianSkinMesh;

                        }
                    }

                }
            }

            yield return 1;
        }

        List<Bullet> attachingBulletList = new List<Bullet>();
        public void AttachABullet(Bullet bullet)
        {
            if (bullet != null)
            {
                bullet.transform.parent = transform;
                attachingBulletList.Add(bullet);
            }
        }

        public void SkillEnd()
        {
            if (m_actor == null)
            {
                return;
            }


            if (m_bSendAttackEven)
            {
                EventParameter eventBuffParam = EventParameter.Get();
                eventBuffParam.intParameter = m_skillID;
                eventBuffParam.goParameter = m_actor.gameObject;
                eventBuffParam.objParameter = m_actor;
                if (m_hitActor != null)
                {
                    eventBuffParam.goParameter1 = m_hitActor.gameObject;
                }
                eventBuffParam.objParameter1 = m_hitActor;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EVENT_ATTACK, eventBuffParam);
                m_bSendAttackEven = false;
            }

            CancelInvoke("SkillEnd");

            SkillIsOver();


            //清除子弹效果
            for (int i = 0; i < attachingBulletList.Count; ++i)
            {
                if (attachingBulletList[i] != null)
                {
                    attachingBulletList[i].AutoEnd();
                }
            }

            if (m_actor.enabled && SubSkill == false)
            {
                //通知技能结束            
                m_actor.SkillEnd(0);
            }

            m_actor.curCastSkillID = 0;
            DestroyEfx();

            //Destroy(this.gameObject);
            CoreEntry.gGameObjPoolMgr.Destroy(this.gameObject);


        }

        //打断吟唱
        public void BreakPrepareSkill()
        {
            CancelInvoke("CastSkill");
        }

        //获取当前动作时间
        public float GetCurActionTime()
        {
            if (m_actor != null)
                return m_actor.GetCurActionTime(m_actor.GetCurPlayAction());
            return 0;
        }

        //技能被打断
        public void BreakSkill(StateParameter stateParm)
        {
            if (null != m_skillDesc && (int)SkillType.SKILL_XUANFANZHAN == m_skillDesc.Get<int>("subtype"))
            {
                return;
            }

            if (m_bSendAttackEven && m_actor != null)
            {
                EventParameter eventBuffParam = EventParameter.Get();
                eventBuffParam.intParameter = m_skillID;
                eventBuffParam.goParameter = m_actor.gameObject;
                eventBuffParam.objParameter = m_actor;
                if (m_hitActor != null)
                {
                    eventBuffParam.goParameter1 = m_hitActor.gameObject;
                    eventBuffParam.objParameter1 = m_hitActor;
                }
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EVENT_ATTACK, eventBuffParam);
                m_bSendAttackEven = false;
            }

            CancelInvoke("AutoLookatTarget");
            CancelInvoke("HideSelf");

            CancelInvoke("CastSkill");
            CancelInvoke("SkillEnd");
            CancelInvoke("Start");

            SkillIsOver();

            //清除skill所有的skillcell
            for (int i = 0; i < m_skillCellObjectlist.Count; ++i)
            {
                m_skillCellObjectlist[i].SetActive(false);
            }

            DestroyEfx();

            if (stateParm != null)
            {
                //死亡,被击状态，删除对应的特效
                if (stateParm.state == ACTOR_STATE.AS_DEATH
                    || stateParm.state == ACTOR_STATE.AS_BEHIT)
                {
                    if (m_actionEfx != null)
                    {
                        m_actionEfx.DestoryObject();
                    }

                    SendEvent(SkillCellEventType.SE_DESTROY_ACTION_EFX, null);
                }
            }

            
            CoreEntry.gGameObjPoolMgr.Destroy(this.gameObject);

        }

        void SkillIsOver()
        {
            IsOver = true;
        }

        public bool CanBeBrokenByHit(StateParameter stateParm, SkillBehitDisplayDesc behitDisplay)
        {
            if (CoreEntry.gGameMgr.IsPvpState())
            {
                bool isBroken = false;
                if (behitDisplay != null)
                {
                    if (m_skillDesc != null)
                    {
                        if (behitDisplay.behitType >= BehitType.BT_HITBACK)
                        {
                            isBroken = true;
                        }
                    }
                }

                if (isBroken)
                {
                    if (m_actor != null)
                    {
                        m_actor.SetFlyText("打断", 0);
                    }
                }
            }
            return false;
        }

        public bool CanBreakSkill(StateParameter stateParm)
        {
            return CanBeBroke;
        }

        void DestroyEfx()
        {
            if (m_actionEfx != null)
            {
                m_actionEfx.DetachObject();


            }

            for (int i = 0; i < m_attachEfxObjectlist.Count; ++i)
            {
                //Destroy(m_attachEfxObjectlist[i].gameObject);
                CoreEntry.gGameObjPoolMgr.Destroy(m_attachEfxObjectlist[i].gameObject);
            }
            m_attachEfxObjectlist.Clear();

            for (int i = 0; i < m_skillCellObjectlist.Count; ++i)
            {
                if (m_skillCellObjectlist[i] != null)
                {
                    CoreEntry.gGameObjPoolMgr.Destroy(m_skillCellObjectlist[i].gameObject);
                }
            }
            m_skillCellObjectlist.Clear();

            //技能结束，显示模型
            ShowSelf();
            CancelInvoke("HideSelf");
        }

        public void Syncm_transform()
        {
            if (ShouldUpdatePositionToCastObj && m_actor != null)
            {

                BaseTool.SetPosition(transform, m_actor.transform.position);

                m_transform.rotation = m_actor.transform.rotation;
            }
        }

        public Transform FindChildTransform(string transformName)
        {
            Transform[] transforms = m_actor.gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < transforms.Length; ++i)
            {
                if (transforms[i].name.Equals(transformName))
                {
                    return transforms[i];
                }
            }

            return null;
        }

        //释放技能中能否行走
        public bool CanMove()
        {
            return canMove;
        }

        bool canMove = false;

        //player玩家才用
        public ActorObj GetSelTarget()
        {
            ActorObj actorObj = m_actor;
            if (actorObj == null)
            {
                return null;
            }

            return actorObj.GetSelTarget();
        }

        void HideSelf()
        {
            CancelInvoke("HideSelf");

            if (m_actor != null)
            {
                m_actor.HideSelf();
            }
        }

        void ShowSelf()
        {
        }

        //添加技能元素
        public void AddSkillCell(GameObject skillCell)
        {
            m_skillCellObjectlist.Add(skillCell);

            skillCell.transform.parent = m_transform;
            skillCell.transform.localPosition = Vector3.zero;
            skillCell.transform.localRotation = Quaternion.identity;
            skillCell.transform.localScale = Vector3.one;
        }

        //元素之间的发送消息
        public void SendEvent(SkillCellEventType eventType, params ValueArg[] valueArgs)
        {
            for (int i = 0; i < m_skillCellObjectlist.Count; ++i)
            {
                ISkillCell skillCell = m_skillCellObjectlist[i].GetComponent<ISkillCell>();
                skillCell.OnEvent(eventType, valueArgs);
            }
        }


        public void HideSkillScope()
        {
            //已经释放，在倒计时的技能不能马上被删
            if (IsInvoking("SkillEnd") == false)
            {
                DestroyEfx();
                //Destroy(this.gameObject);
                CoreEntry.gGameObjPoolMgr.Destroy(this.gameObject);
            }

        }

        public void ShowSkillScope()
        {
            SkillClassDisplayDesc skillClass =
                m_gameDBMgr.GetSkillClassDisplayDesc(m_skillDesc.Get<int>("skillDisplayID"));
            if (skillClass == null)
            {
                Debug.LogError("===skillClass== is null===");
                return;
            }
            LuaTable skilleffect = ConfigManager.Instance.Skill.GetEffectConfig(m_skillID);

            if (skilleffect.Get<int>("range") == (int)SkillRangeType.SKILL_TARGET)
            //       if ((int)m_skillDesc.atkType == (int)Configs.skillConfig.AtkTypeEnum.SINGLE)
            {
                m_bIsAoe = false;
            }

            //吟唱阶段
            for (int i = 0; i < skillClass.prepareStageDataList.Count; ++i)
            {
                GameObject cellObj = CoreEntry.gGameObjPoolMgr.InstantiateSkillCell(skillClass.prepareStageDataList[i].prefabPath);
                cellObj.transform.parent = transform;

                ISkillCell skillCell = cellObj.GetComponent<ISkillCell>();

                skillCell.Init(skillClass.prepareStageDataList[i], this);
                skillCell.SetAoeState(m_bIsAoe);

                AddSkillCell(cellObj);
            }

            //技能元素
            for (int i = 0; i < skillClass.castStageDataList.Count; ++i)
            {
                GameObject cellObj = CoreEntry.gGameObjPoolMgr.InstantiateSkillCell(skillClass.castStageDataList[i].prefabPath);
                cellObj.transform.parent = transform;

                ISkillCell skillCell = cellObj.GetComponent<ISkillCell>();

                skillCell.Init(skillClass.castStageDataList[i], this);
                skillCell.SetAoeState(m_bIsAoe);

                AddSkillCell(cellObj);
            }

            // 删除原来的预警贴花
            if (m_actor.m_WarningefxObj != null)
            {
                Destroy(m_actor.m_WarningefxObj);
                m_actor.m_WarningefxObj = null;
            }

            // 读取新的预警贴花
            for (int i = 0; i < m_skillCellObjectlist.Count; i++)
            {
                ISkillCell skillCell = m_skillCellObjectlist[i].GetComponent<ISkillCell>();
                skillCell.ShowSkillScope();
                if (m_actor.m_WarningefxObj != null)
                    break;
            }

            if (m_actor.m_WarningefxObj != null)
            {
                EfxAttachActionPool efx = m_actor.m_WarningefxObj.GetComponent<EfxAttachActionPool>();
                if (efx == null)
                    efx = m_actor.m_WarningefxObj.AddComponent<EfxAttachActionPool>();
                efx.Init(m_actor.transform, 100000f, true);
            }
        }

        public void Preload(ActorObj own, int skillID)
        {
            m_skillID = skillID;
            m_onlyShowSkillScope = true;
            m_actor = own;
            m_skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(m_skillID);
            if (m_skillDesc == null)
                return;

            SkillClassDisplayDesc skillClass = m_gameDBMgr.GetSkillClassDisplayDesc(m_skillDesc.Get<int>("skillDisplayID"));
            if (skillClass == null)
                return;

            //吟唱阶段
            for (int i = 0; i < skillClass.prepareStageDataList.Count; ++i)
            {
                GameObject cellObj = CoreEntry.gGameObjPoolMgr.InstantiateSkillCell(skillClass.prepareStageDataList[i].prefabPath);
                cellObj.transform.parent = transform;

                ISkillCell skillCell = cellObj.GetComponent<ISkillCell>();
                skillCell.Preload(skillClass.prepareStageDataList[i], this);
                AddSkillCell(cellObj);
            }

            //技能元素
            for (int i = 0; i < skillClass.castStageDataList.Count; ++i)
            {
                GameObject cellObj = CoreEntry.gGameObjPoolMgr.InstantiateSkillCell(skillClass.castStageDataList[i].prefabPath);
                if (cellObj == null)
                {
                    LogMgr.UnityError("Can not found prefab: " + skillClass.castStageDataList[i].prefabPath + " ! of skillID " + skillClass.skillID + " : " + skillClass.castStageDataList[i].ToString());
                    continue;
                }

                cellObj.transform.parent = transform;

                ISkillCell skillCell = cellObj.GetComponent<ISkillCell>();
                skillCell.Preload(skillClass.castStageDataList[i], this);
                AddSkillCell(cellObj);
            }

            // 动作特效加载
            LuaTable skill_action = CoreEntry.gSkillMgr.GetSkillActon(skillID);
            if (skill_action != null && skill_action.Get<string>("skilleffect").Length > 0)
            {
                GameObject efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(skill_action.Get<string>("skilleffect"));
                if (efxObj != null)
                {
                    CoreEntry.gGameObjPoolMgr.Destroy(efxObj);
                }
                else
                {
                    LogMgr.LogError("can not find attackEfxPrefab path: " + skill_action.Get<string>("skilleffect"));
                }
            }

            // 受击特效加载
            string pfx_hurt = m_skillDesc.Get<string>("pfx_hurt");
            if (pfx_hurt.Length > 0)
            {
                GameObject efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(pfx_hurt);
                if (efxObj != null)
                    CoreEntry.gGameObjPoolMgr.Destroy(efxObj);
            }

            DestroyEfx();
        }

        public ActorObj GetTargetFromSelector(DamageCell dmgCell)
        {
            if (m_actor != null)
            {
                return m_actor.GetTargetFromSelector(m_skillDesc, dmgCell);
            }

            return null;
        }

        void CheckSkillCell(ISkillCell skillCell)
        {
            if (skillCell != null)
            {
                if (skillCell is PositionCell)
                {
                    canMove = false;
                    canBeBroke = false;
                }
                else if (skillCell is CompositedSkillCell)
                {
                    compositedSkillCell = skillCell as CompositedSkillCell;
                }
            }

        }
        //是否可以被打破
        bool canBeBroke = true;

        public bool CanBeBroke
        {
            get { return canBeBroke; }
        }

        public bool SubSkill { get; set; }
        /// <summary>
        /// 是否是子技能
        /// </summary>
        /// <param name="skillId"></param>
        /// <returns></returns>
        public bool IsSubSkill(int skillId)
        {
            if (compositedSkillCell != null)
            {
                for (int i = 0; i < compositedSkillCell.SkillList.Count; ++i)
                {
                    if (compositedSkillCell.SkillList[i].SkillId == skillId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static int GetSkillVersion(ActorObj actor, LuaTable desc)
        {
            return 0;
        }

        private void ShowBossSkillWarning()
        {
            if (null == m_actor || m_actor.mActorType != ActorType.AT_BOSS)
            {
                return;
            }

            if (null == m_skillDesc || null == m_skilleffect)
            {
                return;
            }

            int castSkillType = m_skillDesc.Get<int>("faction_limit");
            if (castSkillType == 2)
            {
                return;
            }
            castSkillType = m_skillDesc.Get<int>("subtype");
            if (castSkillType < (int)SkillType.SKILL_CHARGE || castSkillType > (int)SkillType.SKILL_BIGAOE)
            {
                LogMgr.UnityError(string.Format("skill:{0} has an invalid skill subtype:{1}", m_skillID, castSkillType));

                return;
            }
            if (castSkillType == (int)SkillType.SKILL_NORMAL)
            {
                return;
            }

            int rangeType = m_skilleffect.Get<int>("range");
            int param1 = m_skilleffect.Get<int>("distance");
            int param2 = m_skilleffect.Get<int>("angle");

            GameObject efxObj = null;
            Vector3 pos;
            bool isRectWarning = false;
            if (rangeType == (int)SkillRangeType.SKILL_TARGET || rangeType == (int)SkillRangeType.SKILL_TARGET_CIRCLE)
            {
                ActorObj target = m_actor.GetSelTarget();
                if (null == target)
                {
                    return;
                }

                pos = target.transform.position;
                efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect("Effect/skill/remain/fx_yujing_yuan");
                if (null != efxObj)
                {
                    efxObj.transform.localRotation = Quaternion.identity;
                    efxObj.transform.localScale = new Vector3(param1 * 2.0f, 1.0f, param1 * 2.0f);
                }
            }
            else
            {
                pos = m_actor.transform.position;

                if (rangeType == (int)SkillRangeType.SKILL_SELF_FUN)
                {
                    efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect("Effect/skill/remain/fx_yujing_shanxing");
                    if (null != efxObj)
                    {
                        efxObj.transform.rotation = m_actor.transform.rotation;
                        efxObj.transform.localScale = new Vector3(param1, 1.0f, param1);
                    }
                }
                else if (rangeType == (int)SkillRangeType.SKILL_SELF_RECT)
                {
                    efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect("Effect/skill/remain/fx_yujing_changfang");
                    if (null != efxObj)
                    {
                        efxObj.transform.rotation = m_actor.transform.rotation;
                        efxObj.transform.localScale = new Vector3(param2, 1.0f, param1);
                    }
                    isRectWarning = true;
                }
                else if (rangeType == (int)SkillRangeType.SKILL_SELF_CIRCLE)
                {
                    efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect("Effect/skill/remain/fx_yujing_yuan");
                    if (null != efxObj)
                    {
                        efxObj.transform.localRotation = Quaternion.identity;
                        efxObj.transform.localScale = new Vector3(param1 * 2.0f, 1.0f, param1 * 2.0f);
                    }
                }
            }

            if (null == efxObj)
            {
                return;
            }

            efxObj.transform.position = pos;

            int delayTime = m_skilleffect.Get<int>("delay");
            EfxAttachActionPool efx = efxObj.GetComponent<EfxAttachActionPool>();
            if (efx == null)
                efx = efxObj.AddComponent<EfxAttachActionPool>();

            if (efx != null)
            {
                efx.Init(null, delayTime * 0.001f + 0.5f);
            }

            Transform aniTransform = efxObj.transform.FindChild("liquan");
            if (null != aniTransform)
            {
                if (isRectWarning)
                {
                    aniTransform.localPosition = Vector3.zero;
                    TweenPosition.Begin(aniTransform.gameObject, delayTime * 0.001f, Vector3.forward);
                }
                else
                {
                    aniTransform.localScale = Vector3.zero;
                    TweenScale.Begin(aniTransform.gameObject, delayTime * 0.001f, Vector3.one);
                }
            }
        }
    }
}

