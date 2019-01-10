/**
* @file     : FlyTextCurve.cs
* @brief    : 飘字动画
* @details  : 飘字动画
* @author   : CW
* @date     : 2017-07-13
*/
using XLua;
using UnityEngine;
using System.Collections;

[Hotfix]
public class FlyTextCurve : MonoBehaviour 
{
    public AnimationCurve PosXCurve;
    public AnimationCurve PosYCurve;
    public AnimationCurve ScaleCurve;
    public AnimationCurve AlphaCurve;

    private Keyframe[] posXFrame;
    private Keyframe[] posYFrame;
    private Keyframe[] scaleFrame;
    private Keyframe[] alphaFrame;

    void Awake()
    {
        posXFrame = PosXCurve.keys;
        posYFrame = PosYCurve.keys;
        scaleFrame = ScaleCurve.keys;
        alphaFrame = AlphaCurve.keys;
    }

    public float GetAnimationTime()
    {
        float maxTime = Mathf.Max(posXFrame[posXFrame.Length - 1].time, posYFrame[posYFrame.Length - 1].time);
        maxTime = Mathf.Max(scaleFrame[scaleFrame.Length - 1].time, maxTime);
        maxTime = Mathf.Max(alphaFrame[alphaFrame.Length - 1].time, maxTime);

        return maxTime;
    }
}

