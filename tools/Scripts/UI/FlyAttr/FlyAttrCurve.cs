/**
* @file     : FlyAttrCurve.cs
* @brief    : 
* @details  : 文件用途说明
* @author   : 
* @date     : 
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public class FlyAttrCurve : MonoBehaviour
    {
        public CanvasGroup AlphaShow;
        public RectTransform SelfRT;

        public Vector2 PosUnit = new Vector2(100, 100);
        public AnimationCurve PosXCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        public AnimationCurve PosYCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        public AnimationCurve ScaleCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        public AnimationCurve AlphaCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

        private Vector2 mStart;
        private float mDuration;
        private float mCount;
        private float mDelay;
        private bool mEnd;        

        public void Init(float x, float y, float delay)
        {
            mStart = new Vector2(x, y);
            mDuration = GetAnimationTime();
            mCount = 0;
            mEnd = false;
            mDelay = delay;            
            Evaluate(0);
            transform.localScale = mDelay > 0 ? Vector3.zero : Vector3.one;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (mEnd)
            {
                return;
            }

            //延迟控制
            if (mDelay > 0)
            {
                mDelay -= Time.deltaTime;
                if (mDelay <= 0)
                {
                    mDelay = 0;
                    transform.localScale = Vector3.one;
                    Evaluate(0);
                }
                return;
            }
            
            mCount += Time.deltaTime;
            Evaluate(mCount);
            if (mCount > mDuration)
            {
                mEnd = true;
                ModuleServer.MS.GFlyAttrMgr.RemoveFlyAttr(GetComponent<ItemFlyAttr>());
            }
        }

        void Evaluate(float t)
        {
            float s = ScaleCurve.Evaluate(t);
            float x = mStart.x + PosUnit.x * PosXCurve.Evaluate(t);
            float y = mStart.y + PosUnit.y * PosYCurve.Evaluate(t);
            SelfRT.anchoredPosition = new Vector2(x, y);
            SelfRT.localScale = new Vector3(s, s, 1);
            AlphaShow.alpha = AlphaCurve.Evaluate(t);
        }

        public float GetAnimationTime()
        {
            float maxTime = Mathf.Max(PosXCurve.keys[PosXCurve.keys.Length - 1].time, PosYCurve.keys[PosYCurve.keys.Length - 1].time);
            maxTime = Mathf.Max(ScaleCurve.keys[ScaleCurve.keys.Length - 1].time, maxTime);
            maxTime = Mathf.Max(AlphaCurve.keys[AlphaCurve.keys.Length - 1].time, maxTime);

            return maxTime;
        }
    }
}

