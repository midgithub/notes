//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using XLua;
using UnityEngine;

/// <summary>
/// Time class has no timeScale-independent time. This class fixes that.
/// </summary>

[Hotfix]
public class RealTime : MonoBehaviour
{
	static RealTime mInst;

	float mRealTime = 0f;
	float mRealDelta = 0f;

	/// <summary>
	/// Real time since startup.
	/// </summary>

	static public float time
	{
		get
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) return Time.realtimeSinceStartup;
#endif
			if (mInst == null) Spawn();
            if (mInst == null) return 0.0f;
			return mInst.mRealTime;
		}
	}

	/// <summary>
	/// Real delta time.
	/// </summary>

	static public float deltaTime
	{
		get
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) return 0f;
#endif
			if (mInst == null) Spawn();
            if (mInst == null) return 0.0f;
            return mInst.mRealDelta;
		}
	}

	static void Spawn ()
	{
		GameObject go = new GameObject("_RealTime");
		DontDestroyOnLoad(go);
		mInst = go.AddComponent<RealTime>();
		mInst.mRealTime = Time.realtimeSinceStartup;
	}

	void Update ()
	{
		float rt = Time.realtimeSinceStartup;
		mRealDelta = Mathf.Clamp01(rt - mRealTime);
		mRealTime = rt;
	}
}

