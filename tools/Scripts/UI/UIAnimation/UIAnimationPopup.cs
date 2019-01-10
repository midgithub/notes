using XLua;
using UnityEngine;
using UnityEngine.UI;
using System;

[Hotfix]
public class UIAnimationPopup:MonoBehaviour
{
    public AnimationCurve ScaleAnimation = new AnimationCurve();
    public AnimationCurve AlphaAnimation = new AnimationCurve();
     
    public AnimationCurve PosXAnimation = new AnimationCurve();
    public AnimationCurve PosYAnimaton = new AnimationCurve();

    public Keyframe[] ScaleAnimationFrame = null;
    public Keyframe[] AlphaAnimationFrame = null;
    public Keyframe[] PosXAnimationFrame = null;
    public Keyframe[] PosYAnimatonFrame = null;

    public CanvasGroup AlphaObj;
    public RectTransform rectTransform;
    public float mDuration = 0;
    public float mElapsedTime = 0;
    public bool bPlaying = false;
    public Action mCallBack = null;
    public Vector2 originalPos = Vector2.zero;
    public System.Action CallBack
    {
        get { return mCallBack; }
        set { mCallBack = value; }
    }

    public RectTransform Frame;
    public CanvasScaler scalerObj;
    public float scalerWidth = Screen.width;
    void Awake()
    {
        AlphaObj = this.GetComponent<CanvasGroup>();
        rectTransform = this.GetComponent<RectTransform>();
        if (AlphaObj == null)
        {
            AlphaObj = this.gameObject.AddComponent<CanvasGroup>();
        }

        scalerObj = this.GetComponent<CanvasScaler>();
        if(scalerObj != null)
        {
            scalerWidth = scalerObj.referenceResolution.x;
        }
        Frame = transform.Find("Frame") as RectTransform;           //�������ݶ�����Frame��
    }
    public void Play()
    {
        mDuration = GetAnimationTime();
        mElapsedTime = 0;
        Evaluate(0);
        bPlaying = true;
    }

    public void Update()
    {
        if (!bPlaying)
            return;

        mElapsedTime += Time.deltaTime;
        Evaluate(mElapsedTime);
        if (mElapsedTime >= mDuration)
        {
            if (mCallBack != null)
            {
                mCallBack();
            }

            bPlaying = false;
            //GameObject.DestroyImmediate(this);
        }
    }
    void Evaluate(float t)
    {
        if (!this.gameObject.activeSelf || rectTransform == null)
            return;
        float s = ScaleAnimation.Evaluate(t);
        if (AlphaAnimation.length > 0)
        {
            AlphaObj.alpha = AlphaAnimation.Evaluate(t);
        }
        float x =  PosXAnimation.Evaluate(t) + originalPos.x;
        float y =  PosYAnimaton.Evaluate(t) + originalPos.y;
        if (PosXAnimation.length > 0 && PosYAnimaton.length > 0)
        {
            rectTransform.anchoredPosition = new Vector2(x, y);
        }
        else if (PosXAnimation.length > 0)
        {
            rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
        }
        else if (PosYAnimaton.length > 0)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
        }

        if (ScaleAnimation.length > 0)
        {
            if (Frame != null)
            {
                Frame.localScale = new Vector3(s, s, 1);
            }
            else if (scalerObj != null)
            {
                //Vector2 screenSize = new Vector2(Screen.width, Screen.height);
                Vector2 sol = scalerObj.referenceResolution;
                sol.x = scalerWidth / Math.Max(0.00001f, s);
                scalerObj.referenceResolution = sol;
                //Debug.Log("t:" + t + "  alpha:" + AlphaObj.alpha + " localScale:" + s + " sol:" + sol.ToString() + " screen : " + screenSize.ToString());
            }
            else
            {
               rectTransform.localScale = new Vector3(s, s, 1);
            }

        }

    }

    public void SetAnimationFrame()
    {
         ScaleAnimationFrame = ScaleAnimation.keys;
         AlphaAnimationFrame = AlphaAnimation.keys;
         PosXAnimationFrame = PosXAnimation.keys;
         PosYAnimatonFrame = PosYAnimaton.keys;
    }

    public float GetAnimationTime()
    {
        float maxTime = 0;
        if (ScaleAnimationFrame.Length > 0)
            maxTime = Mathf.Max(ScaleAnimationFrame[ScaleAnimation.keys.Length - 1].time, maxTime);
        if (AlphaAnimationFrame.Length > 0)
            maxTime = Mathf.Max(AlphaAnimationFrame[AlphaAnimation.keys.Length - 1].time, maxTime);
        if (PosXAnimationFrame.Length > 0)
            maxTime = Mathf.Max(PosXAnimationFrame[PosXAnimation.keys.Length - 1].time, maxTime);
        if (PosYAnimatonFrame.Length > 0)
            maxTime = Mathf.Max(PosYAnimatonFrame[PosYAnimaton.keys.Length - 1].time, maxTime);
        return maxTime;
    }

}

