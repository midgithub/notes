/**
* @file     : RoleSelectDrag.cs
* @brief    : 选择角色旋转
* @details  : 选择角色旋转
* @author   : CW
* @date     : 2017-06-26
*/
using XLua;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[Hotfix]
public class RoleSelectDrag : MonoBehaviour, IPointerClickHandler, IDragHandler 
{
    private GameObject mRoleModelObj;
    private Animation mAnimation;
    private GameObject mPlatformObj;

    private float mLastClickTime = 0.0f;

    void Start()
    {
        mPlatformObj = GameObject.Find("UIRoot3D/RoleUI/cs_juese_pingtai001/Object/cs_fb_taizi03");
    }

    public void SetModel(GameObject modelObj)
    {
        mRoleModelObj = modelObj;
        mAnimation = mRoleModelObj.GetComponent<Animation>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (null != mRoleModelObj)
        {
            float absX = Mathf.Abs(eventData.delta.x);
            if (absX > 0)
            {
                float dt = eventData.delta.x / absX * 3;
                Vector3 rotateDir = new Vector3(0f, -dt, 0f);
                mRoleModelObj.transform.Rotate(rotateDir);

                if (null != mPlatformObj)
                {
                    mPlatformObj.transform.Rotate(rotateDir, Space.World);
                }
            }
        }
    }

    int animationIndex = 0;
    string[] animationNames = { "skill001", "skill002", "skill003", "skill004" };
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - mLastClickTime < 1.5f)
        {
            return;
        }

        if (null == mRoleModelObj || null == mAnimation)
        {
            return;
        }

        if (animationIndex >= animationNames.Length)
        {
            animationIndex = 0;
        }

        string curAnim = animationNames[animationIndex];
        mAnimation.CrossFade(curAnim);
        mAnimation.CrossFadeQueued("standby");

        mLastClickTime = Time.time;

        animationIndex++;
    }
}

