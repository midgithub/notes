/**
* @file     : TipCurve.cs
* @brief    : Tip动画曲线编辑
* @details  : 
* @author   : XuXiang
* @date     : 2017-10-20 10:04
*/

using UnityEngine;
using System.Collections;
using XLua;

[LuaCallCSharp]
[Hotfix]
public class TipCurve : MonoBehaviour
{
    public Vector2 PosUnit = new Vector2(100, 100);
    public AnimationCurve PosXCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
    public AnimationCurve PosYCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
    public AnimationCurve ScaleCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
    public AnimationCurve AlphaCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

    public float GetAnimationTime()
    {
        float maxTime = 0;
        maxTime = Mathf.Max(PosXCurve.keys[PosXCurve.keys.Length - 1].time, maxTime);
        maxTime = Mathf.Max(PosYCurve.keys[PosYCurve.keys.Length - 1].time, maxTime);
        maxTime = Mathf.Max(ScaleCurve.keys[ScaleCurve.keys.Length - 1].time, maxTime);
        maxTime = Mathf.Max(AlphaCurve.keys[AlphaCurve.keys.Length - 1].time, maxTime);
        return maxTime;
    }
}

