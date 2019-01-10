using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;

[Hotfix]
public class OnEnableTweenPosition : MonoBehaviour
{
 
 

    UIWidget m_widget;
    Vector3 m_beginPosition;


    public AnimationCurve m_AnimationCurve;

    public float m_time = 0.8f; 

    void Awake()
    {
        m_widget = GetComponent<UIWidget>();
        m_beginPosition = transform.localPosition;

       // m_AnimationCurve = CurveUtil.CreateDialogIn(m_time);
        //CurveUtil.scaleCurve(m_AnimationCurve, m_time); 
    }
	// Use this for initialization
	void Start () {

	}


    void PlayLeaveAnimation(float time)
    {
        TweenPosition.Begin(gameObject, time, new Vector3(m_beginPosition.x, m_beginPosition.y + 400, m_beginPosition.z));

        //tr.animationCurve = m_AnimationCurve;
    }
 
 
    void OnEnable()
    {
        if (m_widget != null)
        {
            //m_widget.alpha = 0;
        }
        transform.localPosition = new Vector3(m_beginPosition.x, m_beginPosition.y + 400, m_beginPosition.z);         

        TweenPosition scale = TweenPosition.Begin(gameObject, 0.6f, m_beginPosition);
        scale.animationCurve = m_AnimationCurve;
         

  	}
}

