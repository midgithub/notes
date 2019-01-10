using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;

[Hotfix]
public class OnEnableTweenAlpha : MonoBehaviour
{
    TweenAlpha m_TweenAlpha;

    UIWidget m_widget;

    public AnimationCurve m_AnimationCurve; 

    public float m_time  = 0.6f; 

    void Awake()
    {
        m_widget = GetComponent<UIWidget>();
    }
    // Use this for initialization
    void Start()
    {

    }
    void PlayLeaveAnimation( float time )
    {
        gameObject.GetComponent<Collider>().enabled = false; 

        TweenAlpha.Begin(gameObject, time, 0);
        //tr.animationCurve = m_AnimationCurve;
    }


    void OnEnable()
    {
        if (gameObject.GetComponent<Collider>()!=null)
            gameObject.GetComponent<Collider>().enabled =  true; 

        if (m_widget != null)
        {
            m_widget.alpha = 0;
        }
        TweenAlpha tr=  TweenAlpha.Begin(gameObject, m_time, 0.88f);
        if (m_AnimationCurve!=null)
            tr.animationCurve = m_AnimationCurve;
        //CurveUtil.scaleCurve(tr.animationCurve, m_time); 

    }
}

