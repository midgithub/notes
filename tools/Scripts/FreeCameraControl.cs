using XLua;
﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace SG
{
[Hotfix]
    public class FreeCameraControl : MonoBehaviour
    {

        #region 对外操作----------------------------------------------------------------

        /// <summary>
        /// 触摸按下。
        /// </summary>
        /// <param name="data">触摸数据。</param>
        public void OnPointDown(BaseEventData data)
        {
            PointerEventData ped = data as PointerEventData;
            m_CheckClick = true;
            m_TouchPosition = ped.position;
#if !UNITY_EDITOR
            m_TouchDown = true;
            m_OldDistance = 0;
#endif

        }

        /// <summary>
        /// 触摸拖拽。
        /// </summary>
        /// <param name="data">触摸数据。</param>
        public void OnDrag(BaseEventData data)
        {
            LogMgr.LogError("OnDrag ...");
            //取消点击的判断距离
            PointerEventData ped = data as PointerEventData;
            if (m_CheckClick)
            {
                if ((ped.position - m_TouchPosition).sqrMagnitude >= CancelClickDistance * CancelClickDistance)
                {
                    m_CheckClick = false;
                }
            }

#if UNITY_EDITOR 

            if (!m_CheckClick)
            {
                Vector2 delta = ped.delta;
                if (Input.GetKey(KeyCode.S))
                {
                    //缩放
                    float d = (delta.x + delta.y) / 2;
                    Scale(d);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    //平移                
                    Move(delta.x, delta.y);
                }
                else
                {
                    //旋转
                    Rotate(delta.x, delta.y);
                }
            }
#endif
        }

        /// <summary>
        /// 触摸弹起。
        /// </summary>
        /// <param name="data">触摸数据。</param>
        public void OnPointUp(BaseEventData data)
        {
            PointerEventData ped = data as PointerEventData;
            if (m_CheckClick)
            {
                Click(ped.position.x, ped.position.y);
                m_CheckClick = false;
            }
#if !UNITY_EDITOR
            m_TouchDown = false;
#endif
        }

        #endregion

        #region 对外属性----------------------------------------------------------------

        /// <summary>
        /// 长按时间。
        /// </summary>
        public static float LongPressTime = 0.5f;

        /// <summary>
        /// 旋转系数。
        /// </summary>
        public static float RotateRatio = 0.25f;

        /// <summary>
        /// 高度调整系数。
        /// </summary>
        public static float HeightRatio = 0.10f;

        /// <summary>
        /// 移动系数。
        /// </summary>
        public static float MoveRatio = 0.05f;

        /// <summary>
        /// 缩放系数。
        /// </summary>
        public static float ScaleRatio = 0.05f;

        #endregion

        #region 内部操作----------------------------------------------------------------

        private void OnEnable()
        {
            m_CF = CoreEntry.gCameraMgr.MainCamera.GetComponent<CameraFollow>();
        }

        private void OnDisable()
        {
            m_CF = null;
        }

        /// <summary>
        /// 帧更新。
        /// </summary>
        public void Update()
        {
#if !UNITY_EDITOR
            if (m_TouchDown)
            {
                CheckTouch();
            }            
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (Input.GetMouseButton(1))
            {
                float m_fDeltaX = Input.GetAxis("Mouse X") * Time.deltaTime * 1000;
                float m_fDeltaY = Input.GetAxis("Mouse Y") * Time.deltaTime * 1000;
                Rotate(m_fDeltaX, m_fDeltaY);
            }

            float len = Input.GetAxis("Mouse ScrollWheel");
            Scale(len * 100);
#endif

        }

#if !UNITY_EDITOR

        /// <summary>
        /// 触摸检测。
        /// </summary>
        private void CheckTouch()
        {
            if (Input.touchCount == 1)
            {
                CheckSingle(Input.GetTouch(0));
                m_OldDistance = 0;
            }
            else if (Input.touchCount == 2)
            {
                m_CheckClick = false;
                CheckTwo(Input.GetTouch(0), Input.GetTouch(1));
            }
            else
            {
                m_OldDistance = 0;
            }
        }

        /// <summary>
        /// 检查单个手指的触摸情况。
        /// </summary>
        /// <param name="touch">触摸数据。</param>
        private void CheckSingle(Touch touch)
        {
            if (touch.phase == TouchPhase.Began)
            {
                m_CheckClick = true;
                m_TouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (!m_CheckClick)
                {
                    //旋转
                    Vector2 delta = touch.deltaPosition;
                    Rotate(delta.x, delta.y);
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                m_CheckClick = false;
            }
        }

        /// <summary>
        /// 检查两个手指的触摸情况。
        /// </summary>
        /// <param name="touch1">手指1触摸数据。</param>
        /// <param name="touch2">手指2触摸数据。</param>
        private void CheckTwo(Touch touch1, Touch touch2)
        {
            //至少有一个点在移动
            if (touch1.phase != TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
            {
                return;
            }

            float dis = Vector2.Distance(touch1.position, touch2.position);
            if (m_OldDistance <= 0)
            {
                m_OldDistance = dis;
            }
            else
            {
                //根据移动夹角判断操作
                if (Vector2.Angle(touch1.deltaPosition, touch2.deltaPosition) < 90)
                {
                    //两点的移动量控制平移
                    Vector2 deltamove = (touch1.deltaPosition + touch2.deltaPosition) / 2;
                    Move(deltamove.x, deltamove.y);
                    m_OldDistance = dis;
                }        
                else
                {
                    //两点距离控制缩放
                    float deltadis = dis - m_OldDistance;
                    Scale(deltadis);
                    m_OldDistance = dis;
                }
            }
        }

#endif


        /// <summary>
        /// 旋转操作。
        /// </summary>
        /// <param name="dx">X屏幕移动量。</param>
        /// <param name="dy">Y屏幕移动量。</param>
        private void Rotate(float dx, float dy)
        {
            if (m_CF != null)
            {
                m_CF.m_rotationAngle += dx * RotateRatio;
                m_CF.m_height = Mathf.Clamp(m_CF.m_height - dy * HeightRatio, 0, 30);
            }
        }

        /// <summary>
        /// 移动操作。
        /// </summary>
        /// <param name="dx">X屏幕移动量。</param>
        /// <param name="dy">X屏幕移动量。</param>
        private void Move(float dx, float dy)
        {
            
        }

        /// <summary>
        /// 缩放操作。
        /// </summary>
        /// <param name="d">屏幕移动量。</param>
        private void Scale(float d)
        {
            if (m_CF != null)
            {
                m_CF.m_distance = Mathf.Clamp(m_CF.m_distance - d * ScaleRatio, 2, 50);
            }
        }

        /// <summary>
        /// 点击操作。
        /// </summary>
        /// <param name="x">X位置。</param>
        /// <param name="y">Y位置。</param>
        private void Click(float x, float y)
        {
            float curt = Time.realtimeSinceStartup;
            if (curt - m_LastClickTime <= 0.5)
            {
                m_LastClickTime = 0;
                MainPanelMgr.Instance.Close("TipShowUI");
            }
            else
            {
                m_LastClickTime = curt;
            }
        }

        #endregion

        #region 内部数据----------------------------------------------------------------

        /// <summary>
        /// 取消点击的距离。
        /// </summary>
        private static float CancelClickDistance = 25;

#if !UNITY_EDITOR
        
        /// <summary>
        /// 缩放操作的两点距离。
        /// </summary>
        private float m_OldDistance = 0;
        
        /// <summary>
        /// 是否按下，用于只检测区域内的Touch。
        /// </summary>
        private bool m_TouchDown = false;
#endif

        CameraFollow m_CF = null;

        /// <summary>
        /// 按下位置。
        /// </summary>
        private Vector2 m_TouchPosition;

        /// <summary>
        /// 是否检测点击。
        /// </summary>
        private bool m_CheckClick = false;

        /// <summary>
        /// 上次点击时间。
        /// </summary>
        private float m_LastClickTime = 0;

        #endregion
    }

}

