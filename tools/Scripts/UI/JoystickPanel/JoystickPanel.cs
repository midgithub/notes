using XLua;
﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SG;
using DG.Tweening;

[Hotfix]
public class JoystickPanel : PanelBase 
{
    public Image thumb;
    public Image backImg;
    public Transform direction;
    public Transform staticTran;
    public float AniShowX = -468f;
    public float AniHideX = -768f;
    public float AniDuration = 0.5f;

    private float backRadius = 0f;

    private Camera uiCamera;

    private bool isPress;

    private bool init = false;

	public void Init()
    {
        if (!init)
        {
            init = true;
            backImg = transform.FindChild("Background").GetComponent<Image>();
            thumb = transform.FindChild("Thumb").GetComponent<Image>();
            direction = transform.FindChild("Direction");
            staticTran = transform.FindChild("Static");
            backRadius = (backImg.rectTransform.sizeDelta.x + backImg.rectTransform.sizeDelta.y) * 0.25f;
        }        
    }

    public override void OnShow()
    {
        base.Show();
        Init();

        uiCamera = MainPanelMgr.Instance.uiCamera;
        isPress = false;
        CoreEntry.gAutoAIMgr.IsSuspend = isPress;

        CoreEntry.gEventMgr.AddListener(GameEvent.GE_JOYSTICK_DOWN, GE_JOYSTICK_DOWN);
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_JOYSTICK_UP, GE_JOYSTICK_UP);
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_JOYSTICK_STATE, OnJoyStickState);

        CoreEntry.gJoystickMgr.SetPositionUpdateCallback(UpdateJoystick);
        CoreEntry.gJoystickMgr.Load();

        bool isShow = CoreEntry.gJoystickMgr.IsShow;
        if(isShow)
        {
            staticTran.localPosition = new Vector3(AniShowX, staticTran.localPosition.y, staticTran.localPosition.z);
        }
        else
        {
            staticTran.localPosition = new Vector3(AniHideX, staticTran.localPosition.y, staticTran.localPosition.z);
        }
    }
    
    public override void OnHide()
    {
        base.Hide();

        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_JOYSTICK_DOWN, GE_JOYSTICK_DOWN);
        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_JOYSTICK_UP, GE_JOYSTICK_UP);
        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_JOYSTICK_STATE, OnJoyStickState);

        CoreEntry.gJoystickMgr.SetPositionUpdateCallback(null);
        CoreEntry.gJoystickMgr.Unload();
    }

    private void GE_JOYSTICK_DOWN(GameEvent ge, EventParameter parameter)
    {
        isPress = true;
        staticTran.gameObject.SetActive(false);
        thumb.gameObject.SetActive(true);
        direction.gameObject.SetActive(true);
        backImg.gameObject.SetActive(true);
        CoreEntry.gAutoAIMgr.IsSuspend = isPress;

        SDKMgr.Instance.TrackGameOptLog(2, 1);
    }

    private void GE_JOYSTICK_UP(GameEvent ge, EventParameter parameter)
    {
        isPress = false;
        staticTran.gameObject.SetActive(true);
        thumb.gameObject.SetActive(false);
        direction.gameObject.SetActive(false);
        backImg.gameObject.SetActive(false);
        CoreEntry.gAutoAIMgr.IsSuspend = isPress;
    }

    /// <summary>
    /// 摇杆状态改变。
    /// </summary>
    private void OnJoyStickState(GameEvent ge, EventParameter parameter)
    {
        bool show = parameter.intParameter == 0;            //是否显示
        bool ani = parameter.intParameter1 == 1;            //是否播放切换动画

        CoreEntry.gJoystickMgr.IsShow = show;

        if (ani)
        {
            DOTween.Kill(staticTran);
            if (show)
            {
                staticTran.DOLocalMoveX(AniShowX, AniDuration);
            }
            else
            {
                staticTran.DOLocalMoveX(AniHideX, AniDuration);
            }
        }
        else
        {
            if(show)
            {
                staticTran.localPosition = new Vector3(AniShowX, staticTran.localPosition.y, staticTran.localPosition.z);
            }
            else
            {
                staticTran.localPosition = new Vector3(AniHideX, staticTran.localPosition.y, staticTran.localPosition.z);
            }
        }
    }

    private void UpdateJoystick(Vector2 thumbPos, Vector2 stickCenterPos)
    {
        if (!isPress)
            return;

        Vector3 pos1 = uiCamera.ScreenToWorldPoint(thumbPos);
        thumb.transform.position = pos1;
        pos1 = thumb.transform.localPosition;
        pos1.z = 0;
        thumb.transform.localPosition = pos1;

        Vector3 pos2 = uiCamera.ScreenToWorldPoint(stickCenterPos);
        backImg.transform.position = pos2;
        pos2 = backImg.transform.localPosition;
        pos2.z = 0;
        backImg.transform.localPosition = pos2;

        float l = (pos1 - pos2).sqrMagnitude;
        if (l < 1200f)
        {
            l = backRadius;
        }
        else
        {
            l = backRadius +  (l - 1200f) * 0.005f;
        }
        Vector3 dir = (pos1 - pos2).normalized * l;
        dir = pos2 + dir;
        direction.position = dir;
        dir.z = 0;
        direction.localPosition = dir;
        float angle = Quaternion.FromToRotation(Vector3.right, pos1 - pos2).eulerAngles.z;
        if (pos1.y == pos2.y)
        {
            if (pos1.x >= pos2.x)
            {
                angle = 0;

                dir = pos2;
                dir.x += l;
                direction.localPosition = dir;
            }
            else
            {
                angle = 180;

                dir = pos2;
                dir.x -= l;
                direction.localPosition = dir;
            }
        }
        direction.localEulerAngles = new Vector3(0f, 0f, angle);
    }

}

