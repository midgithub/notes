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
    public class DropGoldFlyItem : MonoBehaviour
    {
        public float mAniStart = 0.0f;
        public FlyTextCurve mAniCurve;
        public Text mItemText;
        public Vector3 mItemPos;

        public void Init(float startTime, int count, Vector3 pos)
        {
            mAniStart = startTime;
            mItemText.text = string.Format("+ {0}", count);
            mItemPos = pos;
        }

        public void UpdateItem(float t)
        {
            Vector3 pos = Vector3.zero;
            Camera uicamera = MainPanelMgr.Instance.uiCamera;
            if (null != uicamera && null != CoreEntry.gCameraMgr.MainCamera)
            {
                pos = CoreEntry.gCameraMgr.MainCamera.WorldToScreenPoint(mItemPos);
                pos = uicamera.ScreenToWorldPoint(pos);
            }
            t -= mAniStart;
            pos.x += mAniCurve.PosXCurve.Evaluate(t);
            pos.y += mAniCurve.PosYCurve.Evaluate(t);
            transform.position = pos;

            float f = mAniCurve.ScaleCurve.Evaluate(t);
            transform.localScale = new Vector3(f, f, f);

            f = mAniCurve.AlphaCurve.Evaluate(t);
            Color c = mItemText.color;
            c.a = f;
            mItemText.color = c;
        }


        public float GetAnimationTime()
        {
            float ret = 0;
            if (mAniCurve)
            {
                ret = mAniCurve.GetAnimationTime();
            }
            return ret;
        }
    }
}

