using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{

    //技能元素：顿帧，加载就触发
[Hotfix]
    public class FrameStopCellEx : ISkillCell
    {
        private float m_frameStopStartTime = 0;             //顿帧开始时间  
        private float m_frameStopTime = 0f;			        //顿帧持续时间  
        private FrameStopCellDesc m_frameStopCellDesc = null;
        public ActorObj m_actor;

        public override void Init(ISkillCellData cellData, SkillBase skillBase, ActorObj actorBase)
        {
            m_actor = actorBase;

            //m_frameStopTime = param.timeSet;                
            m_frameStopCellDesc = (FrameStopCellDesc)cellData;
            m_frameStopTime = m_frameStopCellDesc.frameStopTime;
        }

        void OnEnable()
        {
            CancelInvoke("Start");
            Invoke("Start", 0.000001f);
        }

        void OnDisable()
        {
            if (m_frameStopTime > 0 || m_frameStopStartTime > 0)
            {
                TimeScaleCore.ResetValue();

                m_frameStopTime = 0f;
                m_frameStopStartTime = 0f;
            }
            CancelInvoke("Start");
        }

        // Use this for initialization
        void Start()
        {
            if (m_frameStopTime > 0f)
            {
                SkillFrameStopStart();
            }
        }


        // Update is called once per frame
        void Update()
        {
            //顿帧
            if (m_frameStopTime > 0f && m_frameStopStartTime > 0f)
            {
                if (m_frameStopTime < Time.realtimeSinceStartup - m_frameStopStartTime)
                {
                    //Time.timeScale = 1f;
                    TimeScaleCore.ResetValue();

                    m_frameStopTime = 0f;
                    m_frameStopStartTime = 0f;
                }
            }
        }

        void OnDestroy()
        {
            if (m_frameStopTime > 0 || m_frameStopStartTime > 0)
            {
                //Time.timeScale = 1f;
                TimeScaleCore.ResetValue();

                m_frameStopTime = 0f;
                m_frameStopStartTime = 0f;
            }
        }

        void SkillFrameStopStart()
        {
            //CancelInvoke("NormalAttackTimeScaleBegin");
            //m_frameStopStartTime = Time.realtimeSinceStartup;
            //Time.timeScale = 0.0f;

            if (TimeScaleCore.SetValue(0))
            {
                m_frameStopStartTime = Time.realtimeSinceStartup;
            }
        }
    }

};  //end sg

