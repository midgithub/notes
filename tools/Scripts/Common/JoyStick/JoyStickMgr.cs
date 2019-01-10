using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[Hotfix]
public class JoyStickMgr : MonoBehaviour
{
    #region Callback methods
    public delegate void JoystickPosition(Vector2 touchPos, Vector2 stickPos);
    public delegate bool JoystickMove(Vector3 dir);
    public delegate void JoystickStop();

    private JoystickPosition positionCallback = null;
    private JoystickMove moveCallback = null;
    private JoystickStop stopCallback = null;
    #endregion

    #region properties
    private bool isInit = false;

    private int screenWidth;
    private int screenHeight;

    private int posXMax;
    private int posYMax;

    private Vector2 initPos;
    private float thumbRadius;
    private float thumbMaxOffDistance;
    private float thumbMaxMoveSpeed = 1200.0f;

    //private float touchSize;
    private float thumbSize;

    private Vector2 stickCenterPos;
    private Vector2 thumbLastPos;
    private Vector2 thumbPos;

    private int uiLayerMask;

    private Camera uiCamera = null;
    private Vector3 cameraDir;

    protected int fingerID;
    private bool isTouching = false;
    /// <summary>
    /// 是否显示摇杆
    /// </summary>
    public bool IsShow
    {
        set;
        get;
    }

    #endregion

    public void SetMoveCallbackFunc(JoystickMove moveFunc)
    {
        moveCallback = moveFunc;
    }

    public void SetStopCallbackFunc(JoystickStop stopFunc)
    {
        stopCallback = stopFunc;
    }

    public void SetPositionUpdateCallback(JoystickPosition posFunc)
    {
        positionCallback = posFunc;
    }

    private void RestJoystick()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        posXMax = screenWidth / 3;
        posYMax = screenHeight / 2;

        initPos.x = screenWidth / 4;
        initPos.y = screenWidth / 4;

        //touchSize = screenHeight / 9;
        thumbSize = screenHeight / 20;

        stickCenterPos = initPos;
        thumbPos = stickCenterPos;
        thumbLastPos = thumbPos;
        thumbMaxOffDistance = screenHeight / 12;

        if (null != positionCallback)
        {
            positionCallback(thumbPos, stickCenterPos);
        }
    }

    /// <summary>
    /// 加载摇杆
    /// </summary>
    public void Load()
    {
        fingerID = -1;
        isTouching = false;
        uiCamera = MainPanelMgr.Instance.uiCamera;
        isInit = true;
        RestJoystick();
    }

    /// <summary>
    /// 卸载摇杆
    /// </summary>
    public void Unload()
    {
        fingerID = -1;
        isTouching = false;
        isInit = false;
        RestJoystick();
    }

    /// <summary>
    /// 检测屏幕点是否在UI上面
    /// </summary>
    /// <param name="pos">屏幕位置，滤过摇杆</param>
    /// <returns></returns>
    private bool IsPositionOverUI(Vector2 pos)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(pos.x, pos.y);
        EventSystem.current.RaycastAll(eventData, results);
        for (int i = 0; i < results.Count; i++)
        {
            if (!results[i].gameObject.CompareTag("UI"))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsMoveOverUI()
    {
        Ray ray = uiCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 1000.0f, uiLayerMask))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 判断点是否在可触碰范围内
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool IsPositinInTouchArea(Vector2 pos)
    {
        if (pos.x <= (posXMax) && pos.y <= (posYMax))
            return true;

        return false;
    }

    /// <summary>
    /// 是否处于摇杆操作中
    /// </summary>
    /// <param name="touchPos"></param>
    /// <returns></returns>
    private bool IsTouching(ref Vector2 touchPos)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
        {
            if (isTouching)
            {
                touchPos = Input.mousePosition;

                return true;
            }
            if (!isTouching && IsPositinInTouchArea(Input.mousePosition) && !IsPositionOverUI(Input.mousePosition))
            {
                touchPos = Input.mousePosition;

                return true;
            }
        }
#elif UNITY_ANDROID || UNITY_IOS
        int count = Input.touchCount;
        for (int i = 0; i < count; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (!isTouching && IsPositinInTouchArea(touch.position) && TouchPhase.Began == touch.phase)//按下摇杆
            {
                if (!IsPositionOverUI(touch.position))
                {
                    fingerID = touch.fingerId;
                    touchPos = touch.position;

                    return true;
                }
            }

            if (isTouching && fingerID != -1 && fingerID == touch.fingerId && 
                TouchPhase.Canceled != touch.phase && TouchPhase.Ended != touch.phase)//摇杆按下过程
            {
                touchPos = touch.position;

                return true;
            }
        }
#endif
        return false;
    }

    private void DoBeginTouch(Vector2 touchPos)
    {
        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_JOYSTICK_DOWN, null);

      
        PlayerObj player = CoreEntry.gActorMgr.MainPlayer;
        if (null == player)
        {
            return;
        }
        //if (CoreEntry.gGameMgr.AutoFight)
        //{
        //    player.CancleAutoSet();
        //}

        stickCenterPos = touchPos;
        thumbPos = touchPos;

        //盘边缘检测
        float thumbEdgeMax = thumbMaxOffDistance + thumbSize;
        if (stickCenterPos.x < thumbEdgeMax)//左边缘
        {
            stickCenterPos.x = thumbEdgeMax;
        }
        if (stickCenterPos.y < thumbEdgeMax)//下边缘
        {
            stickCenterPos.y = thumbEdgeMax;
        }
        if (stickCenterPos.y > screenHeight - thumbEdgeMax)//上边缘
        {
            stickCenterPos.y = screenHeight - thumbEdgeMax;
        }

        Vector2 dir = thumbPos - stickCenterPos;
        if (dir.magnitude > thumbMaxOffDistance)
        {
            thumbPos = stickCenterPos + dir.normalized * thumbMaxOffDistance;
        }
        thumbLastPos = thumbPos;
    }

    private void DoProcessTouch(Vector2 touchPos)
    {
        thumbPos = touchPos;

        Vector2 dir = thumbPos - thumbLastPos;
        float tmpFloat1 = dir.magnitude;
        float tmpFloat2 = Time.deltaTime * thumbMaxMoveSpeed;
        if (tmpFloat1 > tmpFloat2)
        {
            thumbPos = thumbLastPos + dir.normalized * tmpFloat2;
        }

        //盘边缘检测
        //dir = thumbPos - stickCenterPos;
        //tmpFloat1 = dir.magnitude;
        //if (tmpFloat1 > thumbMaxOffDistance)
        //{
        //    stickCenterPos = stickCenterPos + dir.normalized * (tmpFloat1 - thumbMaxOffDistance);
        //}

        //tmpFloat1 = thumbMaxOffDistance + thumbSize;
        //if (stickCenterPos.x < tmpFloat1)
        //{
        //    stickCenterPos.x = tmpFloat1;
        //}
        //if (stickCenterPos.x > posXMax - tmpFloat1)
        //{
        //    stickCenterPos.x = posXMax - tmpFloat1;
        //}
        //if (stickCenterPos.y < tmpFloat1)
        //{
        //    stickCenterPos.y = tmpFloat1;
        //}
        //if (stickCenterPos.y > posYMax - tmpFloat1)
        //{
        //    stickCenterPos.y = posYMax - tmpFloat1;
        //}

        //检测
        dir = thumbPos - stickCenterPos;
        if (dir.magnitude > thumbMaxOffDistance)
        {
            thumbPos = stickCenterPos + dir.normalized * thumbMaxOffDistance;
        }

        thumbLastPos = thumbPos;
        
        UpdateMove();
    }

    private void UpdateMove()
    {
        Vector2 dir = thumbPos - stickCenterPos;
        if (dir.magnitude < 0.01f)
        {
            return;
        }

        PlayerObj player = CoreEntry.gActorMgr.MainPlayer;
        if (null == player)
        {
            return;
        }
        player.m_bAutoMove = false;

        if (null == CoreEntry.gCameraMgr.MainCamera)
        {
            return;
        }

        cameraDir = CoreEntry.gCameraMgr.MainCamera.transform.forward;
        cameraDir.y = 0.0f;
        cameraDir.Normalize();
        dir.Normalize();

        Vector3 vRight = Vector3.Cross(Vector3.up, cameraDir);
        Vector3 vMoveDir = cameraDir * dir.y * Time.deltaTime * 1.0f;
        vMoveDir += vRight * dir.x * Time.deltaTime * 1.0f;
        vMoveDir.Normalize();

        if (moveCallback != null)
        {
            moveCallback(vMoveDir);
        }
    }

    private void DoEndTouch()
    {
        fingerID = -1;

        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_JOYSTICK_UP, null); 

        stickCenterPos = initPos;
        thumbPos = stickCenterPos;
        thumbLastPos = thumbPos;

        if (null != stopCallback)
        {
            stopCallback();
        }
    }

    void Start()
    {
        uiLayerMask = 1 << LayerMask.NameToLayer("UI");
    }

    void Update()
    {
        if (!isInit)
            return;

        if (!IsShow)
        {
            return;
        }

        Vector2 touchPos = new Vector2();
        bool bTouching = IsTouching(ref touchPos);

        if (isTouching)
        {
            if (bTouching)
            {
                DoProcessTouch(touchPos);
            }
            else
            {
                DoEndTouch();
            }
        }
        else
        {
            if (bTouching)
            {
                DoBeginTouch(touchPos);
            }
        }
        isTouching = bTouching;

        if (null != positionCallback)//更新按钮和盘的位置
        {
            positionCallback(thumbPos, stickCenterPos);
        }
    }

    /// <summary>
    /// 摇杆是否在操作中
    /// </summary>
    /// <returns></returns>
    public bool IsTouch()
    {
        return isTouching;
    }
}

