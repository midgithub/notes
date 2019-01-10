using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public class CameraZoom : ISkillCell
    {
        float beginTime = 0f;
        CameraZoomCellDesc desc;

        private CameraBase m_cameraFollow;
        private Transform camTransform;

        //编辑器中设置曲线
        public AnimationCurve m_animationCurve = new AnimationCurve();
        float endTime = 0f;

        private void Awake()
        {
            if (m_animationCurve.length > 0)
            {
                endTime = m_animationCurve[m_animationCurve.length - 1].time;
                //Debug.LogError("endTime : " + endTime);
            }
        }

        public override void Init(ISkillCellData cellData, SkillBase skillBase)
        {
            if (skillBase == null)
            {
                return;
            }
            bool can = skillBase.m_actor.IsMainPlayer() || skillBase.m_actor.mActorType == ActorType.AT_BOSS;
            if (!can)
            {
                return;
            }

            desc = cellData as CameraZoomCellDesc;
            if (desc == null)
            {
                return;
            }

            beginTime = Time.time;
            isBlurEnd = false;
            setBlur = false;
            isZoomEnd = false;
        }

        private void OnEnable()
        {
            GameObject obj = GameObject.FindGameObjectWithTag("MainCamera");
            if (obj != null)
            {
                m_cameraFollow = obj.GetComponent<CameraBase>();
                camTransform = obj.transform;
            }

            if (desc == null)
            {
                return;
            }
            beginTime = Time.time;
            isBlurEnd = false;
            setBlur = false;
            isZoomEnd = false;
        }

        private void OnDestroy()
        {
            EndZoom();
            EndBlur();
        }

        private void OnDisable()
        {
            EndZoom();
            EndBlur();
        }

        
        private void Update()
        {
            if (desc == null)
            {
                return;
            }
            if (m_cameraFollow == null)
            {
                return;
            }
            if (camTransform == null)
            {
                return;
            }

            UpdateZoom();
            UpdateBlur();
        }

        bool setBlur = false;
        bool isBlurEnd = false;
        void UpdateBlur()
        {
            if (isBlurEnd)
            {
                return;
            }

            if (desc.blurStart < 0)
            {
                return;
            }

            var diff = Time.time - beginTime;
            diff -= desc.blurStart;

            if (diff < 0)
            {
                return;
            }
            if (!setBlur)
            {
                if (camTransform != null)
                {
                    DeemoRadialBlur blur = camTransform.GetComponent<DeemoRadialBlur>();
                    if (blur != null)
                    {
                        blur.enabled = true;
                    }
                }
                
                setBlur = true;
            }

            diff -= desc.blurDuration;
            if (diff >= 0)
            {
                isBlurEnd = true;
                if (camTransform != null)
                {
                    var blur = camTransform.GetComponent<DeemoRadialBlur>();
                    if (blur != null)
                    {
                        blur.enabled = false;
                    }
                }
            }
        }

        Vector3? origin = null;
        Vector3 forward = Vector3.zero;
        bool isZoomEnd = false;
        void UpdateZoom()
        {
            if (isZoomEnd)
            {
                return;
            }

            var diff = Time.time - beginTime;
            diff -= desc.playTime;
            if (m_cameraFollow == null) return;
            if (diff < 0)
            {
                m_cameraFollow.m_cameraZoom = false;
                return;
            }

            //if (origin == null)
            {
                origin = m_cameraFollow.UpdateMainCameraTransform().position; //camTransform.position;
                forward = camTransform.forward.normalized;
                //Debug.LogError("origin : " + origin + ",   forward : " + forward);
            }

            m_cameraFollow.m_cameraZoom = true;

            float p = 0f;

            if (diff >= 0.001f)
            {
                p = m_animationCurve.Evaluate(diff);
            }

            float d = p * desc.distance;

            Vector3 add = d * forward;
            Vector3 pos = origin.Value + add;
            camTransform.position = pos;

            //Debug.LogError("p : " + p + ",   d : " + d + ",   add : " + add.magnitude + ",   time : " + diff);

            if (diff >= endTime)
            {
                isZoomEnd = true;
                if (m_cameraFollow != null)
                {
                    m_cameraFollow.m_cameraZoom = false;
                }
            }
        }

        void EndZoom()
        {
            desc = null;
            origin = null;
            if (m_cameraFollow != null)
            {
                m_cameraFollow.m_cameraZoom = false;
            }
            isZoomEnd = false;
        }

        void EndBlur()
        {
            desc = null;
            isBlurEnd = false;
            setBlur = false;
            if (camTransform != null)
            {
                var blur = camTransform.GetComponent<DeemoRadialBlur>();
                if (blur != null)
                {
                    blur.enabled = false;
                }
            }
        }
    }
}

