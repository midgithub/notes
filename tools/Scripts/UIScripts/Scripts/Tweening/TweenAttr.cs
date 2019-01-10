//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using XLua;
using UnityEngine;

/// <summary>
/// Tween the object's alpha.
/// </summary>

[AddComponentMenu("NGUI/Tween/Tween NuberChange")]

public class TweenAttr : UITweener
{
 
    public float from = 0;
    public float to = 100;
 
	/// <summary>
	/// Tween's current value.
	/// </summary>
    protected float mCurValue = 0 ;

    public float value { 
        get { return mCurValue; } 
        set {             
            mCurValue = value;
            fnAttr(mCurValue); 
        }
    
    }

	/// <summary>
	/// Tween the value.
	/// </summary>

    protected override void OnUpdate(float factor, bool isFinished) {
        value = from +  count * factor ;  
    }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

    public float count  = 0; 
 
    public   delegate void SetAttr(float attr) ;

    public SetAttr fnAttr;

    static public TweenAttr Begin(GameObject go, SetAttr fnAttr, float duration, float from, float to)
	{
        TweenAttr comp = UITweener.Begin<TweenAttr>(go, duration);
        comp.from = from;
        comp.to = to;
        comp.count = to - from;
        comp.fnAttr = fnAttr;
        //comp.value = from;

        if (duration <= 0f  )
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}

		return comp;
	}

	public override void SetStartToCurrentValue () { from = value; }
	public override void SetEndToCurrentValue () { to = value; }
}

