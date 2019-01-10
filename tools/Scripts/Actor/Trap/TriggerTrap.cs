using XLua;
﻿using UnityEngine;
using System.Collections;
using System;
namespace SG
{
    //[ExecuteInEditMode]
[Hotfix]
    public class TriggerTrap : ActorObj
    {

        //public int m_resID; 
        Vector3 pos;
        public UnityEngine.Vector3 Pos
        {
            get { return pos; }
            set { pos = value; }
        }
        public float m_widthX = 2.0f;
        public float m_widthZ = 2.0f;


        public float m_startTime = 1f;

        public int m_nTriggerNum = 0; // 触发次数

        private int m_nCurTriggerNum = 0; // 当前触发次数

        //private bool m_bIsTrigger = false;


        //休息时间
        public float m_sleepTime = 1f;

        //警告
        public float m_warningTime = 3f;

        //攻击时间
        public float m_workTime = 2f;

        public int m_trapType = 0;


        GameObject m_warningDecel = null;
        //private Configs.CreatureDesc mCreatureDesc;
        Animation m_anim;
        //private Configs.CreatureDisplayDesc mCreatureDisplayDesc;

        public enum TrapShape
        {
            SHAPE_circel,
            SHAPE_rectangle,
        }


        public TrapShape m_shape = TrapShape.SHAPE_rectangle;
        private SkillBase m_curSkillBase;


        //SkillClassDisplayDesc m_skillClass;


        void OnEnable()
        {

            //LogMgr.UnityLog("------- Trap   OnEnable name:" + gameObject.name + "     pos:" + transform.position.ToString());

        }
        // Use this for initialization
        void Start()
        {
            playAction("stand");

            m_anim = GetComponent<Animation>();

            //mCreatureDesc = CsvMgr.GetRecordCreatureDesc(m_resID);
            //mCreatureDisplayDesc = CsvMgr.GetRecordCreatureDisplayDesc(m_resID);
            //if (mCreatureDesc != null)
            //{
            //    m_workTime = mCreatureDesc.workTime;
            //    m_trapType = mCreatureDesc.trapType;


            //    Configs.SkillDesc skill = ConfigManager.Instance.Skill.GetSkillConfig(mCreatureDesc.normalAttID1);

            //    if (skill == null)
            //    {
            //        LogMgr.UnityError("陷阱技能配置错误:   normalAttID1" + mCreatureDesc.normalAttID1);
            //        return;
            //    }
            //    m_skillClass = CoreEntry.gGameDBMgr.GetSkillClassDisplayDesc(skill.skillDisplayID);

            //}

        }

        void playAction(string actionName)
        {
            if (m_anim != null)
            {
                try
                {
                    m_anim.CrossFade(actionName, 0.5f);
                }
                catch (Exception e)
                {
                    LogMgr.DebugLog("陷阱没有找到动作名:{0} msg:{1}", actionName, e.Message);
                }
            }
        }

        public virtual void OnTriggerEnter(Collider other)
        {
            //触发次数
            if (m_nTriggerNum > 0 && m_nCurTriggerNum > m_nTriggerNum)
            {
                return;
            }

            ActorObj actorBase = other.transform.root.gameObject.GetComponent<ActorObj>();
            if (actorBase == null)
            {
                actorBase = other.transform.gameObject.GetComponent<ActorObj>();
                
            }
            if (actorBase)
            {
                if (actorBase.mActorType == ActorType.AT_LOCAL_PLAYER)
                {
                    damagePlayer();
                    //m_bIsTrigger = true;
                    m_nCurTriggerNum++;

                }
            }

        }



        //生物id
        public override void Init(int resID, int configID, long entityID, string strEnterAction = "", bool isNpc = false)
        {
            m_resID = resID;
            base.Init(resID, configID, entityID, strEnterAction, isNpc);
            playAction("stand");
        }


        Vector2 getSize(SkillClassDisplayDesc skillClass)
        {
            Vector2 re = new Vector2(0, 0);
            if (skillClass.castStageDataList.Count > 0)
            {
                OneDamageInfo damageInfo = (OneDamageInfo)skillClass.castStageDataList[0];
                if (damageInfo != null)
                {
                    if (DamageTypeID.DTID_RECT == damageInfo.type)
                    {
                        m_shape = TrapShape.SHAPE_rectangle;
                        m_widthZ = damageInfo.damageNode.rectDamage.length;
                        m_widthX = damageInfo.damageNode.rectDamage.width;
                    }
                    else if (DamageTypeID.DTID_FUN == damageInfo.type)
                    {
                        m_shape = TrapShape.SHAPE_circel;

                        m_widthZ = damageInfo.damageNode.funDamage.radius;
                        m_widthX = damageInfo.damageNode.funDamage.radius;
                    }
                }
            }
            return re;
        }


        void damagePlayer()
        {
            playAction("attack001");

            //GameObject skillObj = (GameObject)Instantiate(CoreEntry.gResLoader.LoadResource(m_skillClass.prefabPath));//CoreEntry.gGameObjPoolMgr.InstantiateSkillBase(m_skillClass.prefabPath);
            //CoreEntry.gGameObjPoolMgr.InstantiateSkillBase(m_skillClass.prefabPath);
        }

        void showDecal()
        {
            //触发次数
            if (m_nTriggerNum > 0 && m_nCurTriggerNum > m_nTriggerNum)
            {
                return;
            }

            if (m_warningDecel != null)
            {
                m_warningDecel.gameObject.SetActive(true);
                return;
            }
            Vector3 pos = transform.position;
            pos = CoreEntry.gBaseTool.GetGroundPoint(pos);


            if (m_shape == TrapShape.SHAPE_rectangle)
            {
          
                
                UnityEngine.Object wObj = CoreEntry.gResLoader.LoadResource("Effect/skill/remain/fx_yujing_changfang");
                if (wObj != null)
                {
                    m_warningDecel = Instantiate(wObj) as GameObject; //(damageInfo.efxWarning)) as GameObject;
                    if (m_warningDecel != null)
                    {
                        //m_warningDecel = Instantiate(CoreEntry.gResLoader.LoadResource("Effect/skill/remain/fx_yujing_changfang")) as GameObject; //(damageInfo.efxWarning)) as GameObject;

                        m_warningDecel.transform.position = pos += Vector3.up * 0.1f;
                        m_warningDecel.transform.localPosition -= new Vector3(0, 0, m_widthX * 0.5f);

                        m_warningDecel.transform.localScale = new Vector3(1.0f * m_widthX, 0f, 1.0f * m_widthZ);
                    }
                }

            }
            else if (m_shape == TrapShape.SHAPE_circel)
            {
                UnityEngine.Object wObj = CoreEntry.gResLoader.LoadResource("Effect/skill/remain/fx_yujing_yuan");
                if (wObj != null)
                {

                    m_warningDecel = Instantiate(wObj) as GameObject; //(damageInfo.efxWarning)) as GameObject;
                    if (m_warningDecel != null)
                    {
                        //m_warningDecel = Instantiate(CoreEntry.gResLoader.LoadResource("Effect/skill/remain/fx_yujing_yuan")) as GameObject; //(damageInfo.efxWarning)) as GameObject;

                        m_warningDecel.transform.position = pos += Vector3.up * 0.1f;


                        m_warningDecel.transform.localScale = new Vector3(1.0f, 0f, 1.0f) * m_widthX * 2 * (1 + 0.2f);
                    }
                }


            }


        }
        void hideDecel()
        {
            if (m_warningDecel != null)
            {
                m_warningDecel.gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        public override void Update()
        {
            GetComponent<Rigidbody>().WakeUp();
        }

    }















}

