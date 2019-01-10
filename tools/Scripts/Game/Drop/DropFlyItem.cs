/**
* @file     : DropFlyItem
* @brief    : ʰȡ����ͼ��
* @details  : ʰȡ����ͼ��
* @author   : CW
* @date     : 2017-10-20
*/
using XLua;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SG
{
[Hotfix]
    public class DropFlyItem : MonoBehaviour 
    {
        public float mAniTime = 1.0f;
        public FlyTextCurve mAniCurve;
        public Image mItemIcon;
        public float mAniStart;
        public Vector3 mStartPos;
        public Vector3 mEndPos;
        private float f = 0f;
        public string mItemName;
        public int mItemQuality;

        public void Init(float startTime, string icon, Vector3 startPos, Vector3 endPos, string itemName, int quality)
        {
            mAniStart = startTime;
            mItemIcon.sprite = AtlasSpriteManager.Instance.GetSprite(icon);
            mItemIcon.SetNativeSize();
            mStartPos = Vector3.zero;
            mEndPos = endPos;
            mItemName = itemName;
            mItemQuality = quality;

            Camera uicamera = MainPanelMgr.Instance.uiCamera;
            if (null != uicamera && null != CoreEntry.gCameraMgr.MainCamera)
            {
                Vector3 screenPos = CoreEntry.gCameraMgr.MainCamera.WorldToScreenPoint(startPos);
                mStartPos = uicamera.ScreenToWorldPoint(screenPos);
            }

            transform.position = mStartPos;

            f = 1 / mAniTime;
        }

        /// <summary>
        /// ����Item
        /// </summary>
        /// <param name="realTime"></param>
        public void UpdateItem(float realTime)
        {
            float curTime = realTime - mAniStart;
            curTime = curTime * f;
            if (curTime > 1.0f)
            {
                curTime = 1.0f;
            }
            else if (curTime < 0f)
            {
                curTime = 0f;
            }

            float x = mAniCurve.PosXCurve.Evaluate(curTime);
            x = mStartPos.x + (mEndPos.x - mStartPos.x) * x;

            float y = mAniCurve.PosYCurve.Evaluate(curTime);
            y = mStartPos.y + (mEndPos.y - mStartPos.y) * y;

            transform.position = new Vector3(x, y, mStartPos.z);

            x = mAniCurve.ScaleCurve.Evaluate(curTime);
            transform.localScale = new Vector3(x, x, x);

            x = mAniCurve.AlphaCurve.Evaluate(curTime);
            Color c = mItemIcon.color;
            c.a = x;
            mItemIcon.color = c;
        }

        /// <summary>
        /// ��ȡ����ʱ��
        /// </summary>
        /// <returns></returns>
        public float GetStayTime()
        {
            return mAniStart + mAniTime;
        }
    }
}

