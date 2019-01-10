using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;

[Hotfix]
public class OnEnableTweenPositionLeft : MonoBehaviour
{
 
 

    UIWidget m_widget;
    Vector3 m_beginPosition;


    public AnimationCurve m_AnimationCurve;

    public float m_time = 0.8f;


    public int m_offsetX = -500 ;
    public int m_offsetY = -0; 


    void Awake()
    {
        m_widget = GetComponent<UIWidget>();
        m_beginPosition = transform.localPosition;

        //m_AnimationCurve = CurveUtil.CreateDialogIn(m_time);
        //CurveUtil.scaleCurve(m_AnimationCurve, m_time); 
    }
	// Use this for initialization
	void Start () {

	}


    void PlayLeaveAnimation(float time)
    {
        /*TweenPosition scale = */TweenPosition.Begin(gameObject, time, new Vector3(m_beginPosition.x + m_offsetX, m_beginPosition.y + m_offsetY, m_beginPosition.z));

        //tr.animationCurve = m_AnimationCurve;
    }
 
 
    void OnEnable()
    {
        if (m_widget != null)
        {
            //m_widget.alpha = 0;
        }
        transform.localPosition = new Vector3(m_beginPosition.x + m_offsetX, m_beginPosition.y + m_offsetY, m_beginPosition.z);

        TweenPosition scale = TweenPosition.Begin(gameObject, m_time, m_beginPosition);
        scale.animationCurve = m_AnimationCurve;
         

  	}
}

