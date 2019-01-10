/**
* @file     : #FileName#
* @brief    : 
* @details  : 
* @author   : #Author#
* @date     : #Date#
*/
using XLua;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SG
{
[Hotfix]
    public class DropFlyTipItem : MonoBehaviour
    {
        public Text mItemText;
        public Outline mTextOutline;
        public Image mItemBg;
        public Image mItemBg2;
        public FlyTextCurve mAniCurve;
        public float mAniStart;

        public void Init(float startTime, string text, Color c, Color outLineColor)
        {
            mAniStart = startTime;
            mItemText.text = text;
            mItemText.color = c;
            if (null == mTextOutline)
            {
                mTextOutline = mItemText.GetComponent<Outline>();
            }
            if(null != mTextOutline)
            {
                mTextOutline.effectColor = outLineColor;
            }
        }

        public void UpdateItem(float t)
        {
            float curTime = t - mAniStart;
            float x = 0, y = 0;
            x = mAniCurve.PosXCurve.Evaluate(curTime);
            y = mAniCurve.PosYCurve.Evaluate(curTime);
            transform.localPosition = new Vector3(x, y, 0f);

            x = mAniCurve.ScaleCurve.Evaluate(curTime);
            transform.localScale = new Vector3(x, x, x);

            x = mAniCurve.AlphaCurve.Evaluate(curTime);
            Color c = mItemText.color;
            c.a = x;
            mItemText.color = c;
            c = mItemBg.color;
            c.a = x;
            mItemBg.color = c;
            mItemBg2.color = c;
        }

        public float GetAnimationTime()
        {
            return mAniCurve.GetAnimationTime();
        }
    }
}

