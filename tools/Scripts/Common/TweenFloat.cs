using XLua;
﻿using UnityEngine;
using System.Collections;

/// <summary>
/// 浮点值过度工具，用于两个数值之间的过度
/// add by Jiang
/// </summary>

[Hotfix]
public class TweenFloat : MonoBehaviour {

	public delegate void FinishCallBackDelegate();
	public delegate void ChangeValueDelegate(float number);
	
	public float m_CurrentValue{get{return m_fromValue;}}//当前进度

	bool IsAdd = true;
	float m_fromValue;
	float m_toValue;
	float m_addNumber;
	ChangeValueDelegate m_ChangeValueDelegate;
	FinishCallBackDelegate m_FinishCallBack;
    bool DestroyObjWhenFinish = true;
/// <summary>
/// 开始计数
/// </summary>
/// <param name="duration">计数所需时间</param>
/// <param name="fromValue">开始数值</param>
/// <param name="toValue">目标数值</param>
/// <param name="changeValueDelegate">设置数值回调，每帧都会回调</param>
/// <param name="finishCallBack">完成计数回调</param>
	public static GameObject Begin(float duration,float fromValue,float toValue,ChangeValueDelegate changeValueDelegate,FinishCallBackDelegate finishCallBack,bool destroyObjWhenFinish = true)
	{
		if(fromValue == toValue)
		{
			if(changeValueDelegate!=null)
			{
				changeValueDelegate(toValue);
			}
			if(finishCallBack!=null)
			{
				finishCallBack();
			}
			return null;
		}
        GameObject tweenObj = new GameObject("TweenFloat");

        Begin(tweenObj,duration,fromValue,toValue,changeValueDelegate,finishCallBack,destroyObjWhenFinish);
		return tweenObj;
	}
    public static TweenFloat Begin(GameObject obj, float duration, float fromValue, float toValue, ChangeValueDelegate changeValueDelegate, FinishCallBackDelegate finishCallBack, bool destroyObjWhenFinish = true)
    {
        if (fromValue == toValue)
        {
            if (changeValueDelegate != null)
            {
                changeValueDelegate(toValue);
            }
            if (finishCallBack != null)
            {
                finishCallBack();
            }
            return null;
        }
        GameObject tweenObj = obj;
        TweenFloat tweenFloatComponent = tweenObj.AddComponent<TweenFloat>();
        tweenFloatComponent.BeginTweenFloat(duration, fromValue, toValue, changeValueDelegate, finishCallBack, destroyObjWhenFinish);
        return tweenFloatComponent;
    }

    public void BeginTweenFloat(float duration, float fromValue, float toValue, ChangeValueDelegate changeValueDelegate, FinishCallBackDelegate finishCallBack, bool destroyObjWhenFinish)
    {
        DestroyObjWhenFinish = destroyObjWhenFinish;
        m_addNumber = (toValue - fromValue) / duration;
        m_fromValue = fromValue;
        m_toValue = toValue;
        m_ChangeValueDelegate = changeValueDelegate;
        IsAdd = fromValue < toValue;
        m_FinishCallBack = finishCallBack;
    }

	void Update()
	{
		m_fromValue+= m_addNumber*Time.deltaTime;
		if(IsAdd)
        {
            SetValueCallback(m_fromValue);
			if(m_fromValue>=m_toValue)
			{
                m_fromValue = m_toValue;
                SetValueCallback(m_fromValue);
				FinishAndCallback();
            }
		}else
        {
            SetValueCallback(m_fromValue);
			if(m_fromValue<=m_toValue)
			{
                m_fromValue = m_toValue;
                SetValueCallback(m_fromValue);
				FinishAndCallback();
            }
		}
	}

	public void PauseTweenFloat()
	{
		enabled = false;
	}

	public void ContinueTweenFloat()
	{
		enabled = true;
	}

	void SetValueCallback(float value)
	{
		if(m_ChangeValueDelegate!=null)
		{
			m_ChangeValueDelegate(value);
		}
	}

	void FinishAndCallback()
    {
        if (DestroyObjWhenFinish)
            Destroy(gameObject);
        else
            Destroy(gameObject.GetComponent<TweenFloat>());

		if(m_FinishCallBack!=null)
		{
			m_FinishCallBack();
		}
	}

}

