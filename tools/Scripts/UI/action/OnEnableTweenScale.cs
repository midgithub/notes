using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;

[Hotfix]
public class OnEnableTweenScale : MonoBehaviour
{
 
 

    UIWidget m_widget;

    public AnimationCurve m_AnimationCurve;

    public float m_time = 0.6f; 

    void Awake()
    {
        m_widget = GetComponent<UIWidget>();

        if (m_AnimationCurve==null )
        {
            m_AnimationCurve = CurveUtil.CreateDialogIn(m_time);
            CurveUtil.scaleCurve(m_AnimationCurve, m_time); 
        }

    }

	// Use this for initialization
	void Start () {

	}


    void PlayLeaveAnimation(float time)
    {
        TweenScale scale = TweenScale.Begin(gameObject, time, minScale);
        scale.animationCurve = null;
        scale.SetOnFinished(DisableTweenOnFinished);

        //tr.animationCurve = m_AnimationCurve;
    }

    void DisableTweenOnFinished()
    {

    }
 
    Vector3 minScale= new Vector3(0.02f,0.02f,0.02f) ;

    float  lastRealTime = 999999999f; 
    void OnEnable()
    {
        if (m_widget != null)
        {
            //m_widget.alpha = 0;
        }

        lastRealTime = RealTime.time; 
        transform.localScale = minScale;
        TweenScale scale = TweenScale.Begin(gameObject, m_time, Vector3.one);
        //scale.ignoreTimeScale = true; 
        scale.animationCurve = m_AnimationCurve;
        scale.SetOnFinished(enableTweenOnFinished);
        Invoke("delayCheckScale", m_time + 0.1f); 

  	}

    void delayCheckScale()
    {
        if (transform.localScale.x < 1)
        {
            transform.localScale = Vector3.one;
        }
    }

    void enableTweenOnFinished()
    {
       
        SendMessageUpwards("onEnableTweenFinished");
        if (RealTime.time - lastRealTime > 0.01f + m_time )
        {
            /*TweenScale scale = */TweenScale.Begin(gameObject, .1f,  new Vector3( 1.001f, 1, 1.01f));

            //transform.localScale = new Vector3( 1.001f, 1, 1.01f);
        }
    }
    void onEnableTweenFinished()
    {
        //SendMessageUpwards("onEnableTweenFinished"); 
    }
    
}

