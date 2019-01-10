using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{
    /// <summary>
    /// TSR动画控制。
    /// </summary>
[Hotfix]
    public class TransformAnimation : MonoBehaviour
    {
        /// <summary>
        /// 移动曲线单位。
        /// </summary>
        public Vector3 MoveUnit = new Vector3(100, 100, 100);

        /// <summary>
        /// X移动曲线。
        /// </summary>
        public AnimationCurve CurveMoveX = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));

        /// <summary>
        /// Y移动曲线。
        /// </summary>

        public AnimationCurve CurveMoveY = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));

        /// <summary>
        /// X缩放曲线。
        /// </summary>
        public AnimationCurve CurveScaleX = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

        /// <summary>
        /// Y缩放曲线。
        /// </summary>
        public AnimationCurve CurveScaleY = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

        /// <summary>
        /// 旋转曲线。
        /// </summary>
        public AnimationCurve CurveRotateZ = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        
        /// <summary>
        /// 唤醒时是否播放。
        /// </summary>
        public bool PlayOnAwake;

        /// <summary>
        /// 是否循环。
        /// </summary>
        public bool Loop;

        /// <summary>
        /// 初始位置。
        /// </summary>
        public Vector3 InitPosition { get; set; }

        /// <summary>
        /// 计时。
        /// </summary>
        private float m_Count = 0;

        /// <summary>
        /// 动画时长。
        /// </summary>
        private float m_Duration = 0;

        /// <summary>
        /// 是否播放中。
        /// </summary>
        private bool m_IsPlaying = false;

        /// <summary>
        /// 控制组建。
        /// </summary>
        private Transform m_ControlT;

        /// <summary>
        /// 控制组建。
        /// </summary>
        private RectTransform m_ControlRT;

        /// <summary>
        /// 获取动画时长。
        /// </summary>
        public float Duration
        {
            get { return m_Duration; }
        }

        /// <summary>
        /// 唤醒。
        /// </summary>
        void Awake()
        {
            m_ControlT = transform;
            m_ControlRT = m_ControlT as RectTransform;
            ResetInitPosition();
            if (PlayOnAwake)
            {
                Play();
            }
        }

        /// <summary>
        /// 重置初始位置。
        /// </summary>
        public void ResetInitPosition()
        {
            InitPosition = m_ControlRT != null ? m_ControlRT.anchoredPosition3D : m_ControlT.localPosition;
        }

        /// <summary>
        /// 播放动画。
        /// </summary>
        public void Play()
        {
            this.enabled = true;
            m_Count = 0;
            m_Duration = CurveMoveX.length > 0 ? CurveMoveX[CurveMoveX.length - 1].time : 0;
            m_Duration = Mathf.Max(m_Duration, CurveMoveY.length > 0 ? CurveMoveY[CurveMoveY.length - 1].time : 0);
            m_Duration = Mathf.Max(m_Duration, CurveScaleX.length > 0 ? CurveScaleX[CurveScaleX.length - 1].time : 0);
            m_Duration = Mathf.Max(m_Duration, CurveScaleY.length > 0 ? CurveScaleY[CurveScaleY.length - 1].time : 0);
            m_Duration = Mathf.Max(m_Duration, CurveRotateZ.length > 0 ? CurveRotateZ[CurveRotateZ.length - 1].time : 0);
            m_IsPlaying = true;
            Evaluate(0);
        }

        /// <summary>
        /// stop Animation Curve 
        /// </summary> 

        public void Stop()
        {
            m_IsPlaying = false;
        }

        /// <summary>
        /// 帧更新。
        /// </summary>
        void Update()
        {
            if (!m_IsPlaying)
            {
                this.enabled = false;
                return;
            }

            m_Count += Time.deltaTime;
            Evaluate(m_Count);
            if (m_Count >= m_Duration)
            {
                if (Loop)
                {
                    m_Count = 0;
                }
                else
                {
                    m_IsPlaying = false;
                }                
            }
        }

        /// <summary>
        /// 设置动画状态。
        /// </summary>
        /// <param name="t">当前时间。</param>
        private void Evaluate(float t)
        {
            //移动
            float mx = CurveMoveX.Evaluate(t) * MoveUnit.x;
            float my = CurveMoveY.Evaluate(t) * MoveUnit.y;
            float mz = 0;
            Vector3 pos = InitPosition + new Vector3(mx, my, mz);
            if (m_ControlRT != null)
            {
                m_ControlRT.anchoredPosition = pos;
            }
            else
            {
                m_ControlT.localPosition = pos;
            }

            //缩放
            float sx = CurveScaleX.Evaluate(t);
            float sy = CurveScaleY.Evaluate(t);
            m_ControlT.localScale = new Vector3(sx, sy, m_ControlT.localScale.z);

            //旋转
            float rz = CurveRotateZ.Evaluate(t) * 360;
            Quaternion q = m_ControlT.rotation;
            m_ControlT.rotation = Quaternion.Euler(q.x, q.y, rz);
        }
    }
}

