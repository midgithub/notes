/**
* @file     : ExpItem
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-07-17
*/
using XLua;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SG
{
[Hotfix]
    public class FixedFlyItem : MonoBehaviour 
    {
        public FixedFlyType mType;
        public Text ShowText;
        public FlyTextCurve AniCurve;
        public float AniStart;
        public GameObject EffectGo;

        public void Init(float startTime, string text, FixedFlyType type)
        {
            AniStart = startTime;
            ShowText.text = text;
            mType = type;

            if (null != EffectGo)
            {
                EffectGo.SetActive(false);
            }
            if (type == FixedFlyType.Huanling || type == FixedFlyType.Mochong)
            {
                EffectGo.SetActive(true);
            }
        }

        public void GetAnimationPos(float time, ref float x, ref float y)
        {
            x = AniCurve.PosXCurve.Evaluate(time) * 100;
            y = AniCurve.PosYCurve.Evaluate(time) * 100;
        }

        public float GetAnimationScale(float time)
        {
            float scale = 1.0f;
            if (null != AniCurve.ScaleCurve)
            {
                scale = AniCurve.ScaleCurve.Evaluate(time);
            }

            return scale;
        }

        public float GetAnimationAlpha(float time)
        {
            float alpha = 1.0f;
            if (null != AniCurve.AlphaCurve)
            {
                alpha = AniCurve.AlphaCurve.Evaluate(time);
            }

            return alpha;
        }

        public float GetAnimationTime()
        {
            return AniCurve.GetAnimationTime();
        }

        public void Recycle()
        {
            if (null != EffectGo)
            {
                EffectGo.SetActive(false);
            }
        }
    }
}

