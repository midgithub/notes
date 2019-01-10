using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{

    //技能元素：顿帧，加载就触发
[Hotfix]
    public class FrameStopCell : ISkillCell
    {
        private float m_frameStopStartTime = 0;             //顿帧开始时间  
        private float m_frameStopTime = 0f;                 //顿帧持续时间  
                                                            //private FrameStopCellDesc m_frameStopCellDesc = null;

        public ActorObj m_actor;

        public override void Init(ISkillCellData cellData, SkillBase skillBase, ActorObj actorBase)
        {
            m_actor = actorBase;

            //m_frameStopTime = param.timeSet;                
            // m_frameStopCellDesc = (FrameStopCellDesc)cellData;
            m_frameStopStartTime = 0;
            m_frameStopTime = 0.2f; //m_frameStopCellDesc.frameStopTime;
        }

        // PoolManager debug
        void OnDisable()
        {
            if (m_frameStopTime > 0 || m_frameStopStartTime > 0)
            {
                //CoreEntry.gTimeMgr.TimeScale  = 1f;
                TimeScaleCore.ResetValue();

                m_frameStopTime = 0f;
                m_frameStopStartTime = 0f;
            }
            CancelInvoke("Start");
        }

        // PoolManager
        void OnEnable()
        {
            CancelInvoke("Start");
            Invoke("Start", 0.000001f);
        }

        // Use this for initialization
        void Start()
        {
            CancelInvoke("Start");
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
                    //CoreEntry.gTimeMgr.TimeScale  = 1f;
                    // TimeScaleCore.ResetValue();

                    m_actor.SetActionSpeed(m_actor.GetCurPlayAction(), 1.0f);

                    m_frameStopTime = 0f;
                    m_frameStopStartTime = 0f;
                }
            }
        }

        void OnDestroy()
        {
            if (m_frameStopTime > 0 || m_frameStopStartTime > 0)
            {
                //CoreEntry.gTimeMgr.TimeScale  = 1f;
                TimeScaleCore.ResetValue();

                m_frameStopTime = 0f;
                m_frameStopStartTime = 0f;
            }
        }

        void SkillFrameStopStart()
        {
            if (m_actor != null)
            {
                m_actor.SetActionSpeed(m_actor.GetCurPlayAction(), 1.5f);
                m_frameStopStartTime = Time.realtimeSinceStartup;
            }
        }
    }
}

