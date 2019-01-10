//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using XLua;
using UnityEngine;

/// <summary>
/// 抛物线运动
/// </summary>

[AddComponentMenu("NGUI/Tween/TweenPositionY")]

public class TweenPositionY : UITweener
{
	public Vector3 from;
	public Vector3 to;

	[HideInInspector]
	public bool worldSpace = false;

	Transform mTrans;
	UIRect mRect;

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

	[System.Obsolete("Use 'value' instead")]
	public Vector3 position { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public Vector3 value
	{
		get
		{
			return worldSpace ? cachedTransform.position : cachedTransform.localPosition;
		}
		set
		{
			if (mRect == null || !mRect.isAnchored || worldSpace)
			{
				if (worldSpace) cachedTransform.position = value;
				else cachedTransform.localPosition = value;
			}
			else
			{
				value -= cachedTransform.localPosition;
				//NGUIMath.MoveRect(mRect, value.x, value.y);
			}
		}
	}

	void Awake () { 
        
        mRect = GetComponent<UIRect>();
        //animationCurve = CreateMoveCuve(duration);
        //m_height = 2;

    }

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) {
        Vector3 temp = from * (1f - factor) + to * factor;
        float scale = m_heightCurve.Evaluate(factor);

        temp.y += m_height  *(scale); 

        value = temp; 

    
    }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>
    static public TweenPositionY BeginOffset(GameObject go, float duration, Vector3 pos, float height)
    {
        Vector3 from = go.transform.position;
        return Begin(go, duration, from + pos, height); 
    }


    public AnimationCurve m_heightCurve; 
	static public TweenPositionY Begin (GameObject go, float duration, Vector3 pos , float height)
	{
		TweenPositionY comp = UITweener.Begin<TweenPositionY>(go, duration);
		comp.from = comp.value;
		comp.to = pos;
        comp.m_heightCurve = CreateMoveCuve(1);
        comp.style = Style.Once;
        comp.ignoreTimeScale = false; 
		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}


    public float m_height = 5;
    public float Height
    {
        get { return m_height; }
        set { m_height = value; }
    }

    public static AnimationCurve CreateMoveCuve(float scaleTime)
    {
        Keyframe[] ks = new Keyframe[3];

        float scale = scaleTime / 10f;

        ks[0] = new Keyframe(0, 0, 5f / scaleTime, 5 / scaleTime);
        //ks[0].inTangent = 0;
        ks[1] = new Keyframe(5f * scale, 1,   0,0);
        //ks[2].inTangent = 45;
        ks[2] = new Keyframe(10f * scale, 0.2f, -5 / scaleTime, -5 / scaleTime);


        //ks[3].inTangent = 45;

        return new AnimationCurve(ks);
    }










	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue () { from = value; }

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue () { to = value; }

	[ContextMenu("Assume value of 'From'")]
	void SetCurrentValueToStart () { value = from; }

	[ContextMenu("Assume value of 'To'")]
	void SetCurrentValueToEnd () { value = to; }



}

